using DotnetJobs.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp.Authenticators.OAuth2;
using RestSharp;
using System.Text.Json.Serialization;
using Spark.Library.Environment;
using DotnetJobs.Application.Lib;
using DotnetJobs.Application.Database;
using Spark.Library.Extensions;
using Stripe.Issuing;
using System.Text.Json;
using DotnetJobs.Application.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace DotnetJobs.Pages.Admin.JobUpload;

[Authorize(Roles = CustomRoles.Admin)]
public class Create : PageModel
{
	[BindProperty]
	public string CompanyName { get; set; }
	[BindProperty]
	public string ApplyLink { get; set; }
	[BindProperty]
	public string Description { get; set; }
	public string Status { get; set; }

	private readonly DatabaseContext _db;

	public Create(DatabaseContext db)
	{
		_db = db;
	}

	public void OnGet()
    {
    }

	public async Task<IActionResult> OnPost()
	{

		var jobForm = new DotnetJobs.Application.Models.JobUpload();
		try
		{
			jobForm.Description = Description;
			jobForm.ApplyLink = ApplyLink;
			jobForm.CompanyName = CompanyName;
			jobForm.Status = JobUploadStatus.NotProcessed.ToString();
			_db.JobUploads.Save(jobForm);

			var company = _db.Companies.Where(x => x.Name == jobForm.CompanyName).FirstOrDefault();

			if (company == null)
			{
				var newCompany = new Company();
				newCompany.Name = jobForm.CompanyName;
				newCompany.Email = "wes@dotnetdevs.co";
				newCompany.Location = "Unknown";
				newCompany.Description = "N/A";
				newCompany.Slug = jobForm.CompanyName.ToSlug();
				newCompany.Logo = "/images/company.png";
				newCompany.Website = "";
				newCompany.Verified = false;
				_db.Companies.Save(newCompany);

				company = newCompany;
			}

			// var jobJson = await GetJobInfoFromOpenAi();
			// create job
			// todo: setup job stuff
			// call openai to parse info from apply link
			// https://api.openai.com/v1/completions
			//var description = await GetJobInfo(jobForm.Description);
			var result = await GetJobInfoFromOpenAi(jobForm.Description);
			var choice = result.Choices.FirstOrDefault();
			if (choice != null)
			{
				var message = choice.Message;
				var chatJob = JsonSerializer.Deserialize<ChatJob>(message.Content);
				var newJob = new Job();
				newJob.Slug = StringHelpers.Slugify($"{chatJob.jobTitle} {company.Name}");
				var existingJobBySlug = _db.Jobs
								.Where(d => d.Slug == newJob.Slug)
								.FirstOrDefault();
				if (existingJobBySlug != null)
				{
					newJob.Slug = newJob.Slug + "-" + Guid.NewGuid().ToString();
				}
				newJob.CompanyId = company.Id;
				newJob.Title = chatJob.jobTitle;
				newJob.Description = BuildJobDescription(chatJob.description);
				// TODO: newJob.Description = chatJob.description;
				newJob.WorkTypeId = chatJob.workType;
				newJob.RemotePolicyId = chatJob.remotePolicy;
				newJob.ExperienceLevelId = chatJob.experienceLevel;
				newJob.ApplyLink = jobForm.ApplyLink.RemoveSubstrings("www.", "https://");
				newJob.IsPremium = false;
				newJob.HasPaid = true;
				newJob.LocationRestrictions = chatJob.locationRestrictions;
				newJob.Location = chatJob.officeLocation;
				newJob.IsClosed = false;
				newJob.StripeIdentifier = "";
				newJob.PaymentCallbackGuid = "";
				if (chatJob.salaryRangeStart != null && chatJob.salaryRangeEnd != null)
				{
					newJob.SalaryStart = chatJob.salaryRangeStart;
					newJob.SalaryEnd = chatJob.salaryRangeEnd;
				}
				_db.Jobs.Save(newJob);

				jobForm.Status = JobUploadStatus.Finished.ToString();
				_db.JobUploads.Save(jobForm);

				HttpContext.SetPageFlash("success", "added job");
			}
			else
			{
				HttpContext.SetPageFlash("success", "no reponse from openai. job was not created");
			}
			return Redirect("/admin/job-uploads/create");
		}
		catch (Exception ex)
		{
			jobForm.Status = JobUploadStatus.Error.ToString();
			_db.JobUploads.Save(jobForm);
			HttpContext.SetPageFlash("success", ex.Message);
			return Redirect("/admin/job-uploads/create");
		}
	}

	private string BuildJobDescription(ChatJobDescriptions description)
	{
		var markdown = "";

		markdown += $"### Job Summary\n";
		foreach (var content in description.summary)
		{
			markdown += $"- {content}\n";
		}
		markdown += "\n";
		markdown += $"### Responsibilities\n";
		foreach (var content in description.responsibilities)
		{
			markdown += $"- {content}\n";
		}
		markdown += "\n";
		markdown += $"### Qualifications\n";
		foreach (var content in description.qualifications)
		{
			markdown += $"- {content}\n";
		}
		markdown += "\n";
		markdown += $"### Benefits\n";
		foreach (var content in description.benefits)
		{
			markdown += $"- {content}\n";
		}
		return markdown;
	}

	private async Task<ChatResponse> GetJobInfoFromOpenAi(string jobPosting)
	{
		var key = Env.Get("CHATGPT_KEY");
		string baseUrl = "https://api.openai.com";
		var options = new RestClientOptions(baseUrl)
		{
			ThrowOnAnyError = false,
			Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(key, "Bearer"),
			MaxTimeout = 200000
		};
		var payload = new ChatRequest();
		payload.Messages.Add(new Message()
		{
			Role = "system",
			Content = "You are a helpful assistant designed to parse a job posting and output JSON."
		});
		payload.Messages.Add(new Message()
		{
			Role = "user",
			Content = $@"I'm going to give you a job posting. Parse it and return json with the following properties: jobTitle, description, workType, remotePolicy, experienceLevel, locationRestrictions, officeLocation, salaryRangeStart, salaryRangeEnd. The description should be 4 sections as json attributes. summary, responsibilities, qualifications, benefits. Each description section should be a list of relevant info from the job posting in a json array. The workType will be 1 for full time, 2 for part time, 3 for internship, 4 for freelance, 5 for contract. remotePolicy will be 1 for remote, 2 for hybrid, or 3 for on site. experienceLevel will be 1 for junior, 2 for mid level, 3 for senior, 4 for lead. officeLocation should only be provided if it's not a remote job, otherwise its null. locationRestrictions is a string but should be null if no restrictions are found. salaryRangeStart and salaryRangeEnd should be null if no salary range is found."
		});
		payload.Messages.Add(new Message()
		{
			Role = "user",
			Content = $"Here is the job posting: {jobPosting}"
		});
		var client = new RestClient(options);
		var request = new RestRequest($"/v1/chat/completions", Method.Post)
			.AddJsonBody(payload);
		var result = await client.ExecuteAsync<ChatResponse>(request);
		return result.Data!;
	}
}

public class ChatResult
{
	public string JobTitle { get; set; }
	public string Description { get; set; }
	public int WorkType { get; set; }
	public int RemotePolicy { get; set; }
	public int ExperienceLevel { get; set; }
	public string LocationRestrictions { get; set; }
	public string Location { get; set; }
}

public class ChatRequest
{
	[JsonPropertyName("model")]
	public string Model { get; set; } = "gpt-3.5-turbo-1106";

	[JsonPropertyName("response_format")]
	public ResponseFormat ResponseFormat { get; set; } = new();

	[JsonPropertyName("messages")]
	public List<Message> Messages { get; set; } = new();

}

public class ResponseFormat
{
	[JsonPropertyName("type")]
	public string Type { get; set; } = "json_object";
}

public class Message
{
	[JsonPropertyName("role")]
	public string Role { get; set; }
	[JsonPropertyName("content")]
	public string Content { get; set; }
}
public class ChatResponse
{
	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("object")]
	public string Object { get; set; }

	[JsonPropertyName("choices")]
	public List<ChatChoices> Choices { get; set; } = new();
}

public class ChatChoices
{
	[JsonPropertyName("index")]
	public int Index { get; set; }

	[JsonPropertyName("message")]
	public ChatMessage Message { get; set; }

	[JsonPropertyName("finish_reason")]
	public string FinishReason { get; set; }
}

public class ChatMessage
{
	[JsonPropertyName("role")]
	public string Role { get; set; }

	[JsonPropertyName("content")]
	public string Content { get; set; }
}

public class ChatJob
{
	public ChatJobDescriptions description { get; set; } = new();
	[DefaultValue(".NET Developer")]
	public string jobTitle { get; set; }
	[DefaultValue(1)]
	public int workType { get; set; }
	[DefaultValue(1)]
	public int remotePolicy { get; set; }
	[DefaultValue(2)]
	public int experienceLevel { get; set; }
	[DefaultValue(null)]
	public string locationRestrictions { get; set; }
	[DefaultValue("")]
	public string officeLocation { get; set; }
	[DefaultValue(null)]
	public int? salaryRangeStart { get; set; }
	[DefaultValue(null)]
	public int? salaryRangeEnd { get; set; }
}

public class ChatJobDescriptions
{
	public List<string> summary { get; set; } = new();
	public List<string> qualifications { get; set; } = new();
	public List<string> responsibilities { get; set; } = new();
	public List<string> benefits { get; set; } = new();
}
