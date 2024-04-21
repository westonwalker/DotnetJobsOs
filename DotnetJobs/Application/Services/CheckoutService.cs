using DotnetJobs.Application.Database;
using DotnetJobs.Application.Helpers;
using DotnetJobs.Application.Models;
using DotnetJobs.Application.Lib;
using DotnetJobs.Pages.Jobs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spark.Library.Environment;
using Stripe;
using Stripe.Checkout;
using Microsoft.IdentityModel.Tokens;

namespace DotnetJobs.Application.Services
{
	public class CheckoutService
	{
		private readonly DatabaseContext _db;
		private readonly JobService _jobService;
		private readonly CompanyService _companyService;
		private readonly ImageKitService _imageKitService;

		public CheckoutService(DatabaseContext db, JobService jobService, CompanyService companyService, ImageKitService imageKitService)
		{
			_db = db;
			_jobService = jobService;
			_companyService = companyService;
			_imageKitService = imageKitService;	
		}

		public async Task<Company> CreateCompany(CreateJobForm job)
		{
			// TODO: Update to map checkout job
			var company = new Company();
			// company.Logo = await _imageKitService.UploadImage(job.CompanyLogo);
			company.Logo = !String.IsNullOrEmpty(job.CompanyLogo) ? job.CompanyLogo : "/images/company.png";
			company.CreatedAt = DateTime.Now;
			company.Slug = StringHelpers.Slugify(job.CompanyName);

			var existingCompanyBySlug = await _companyService.GetBySlug(company.Slug);
			if (existingCompanyBySlug != null)
			{
				company.Slug = company.Slug + "-" + Guid.NewGuid().ToString();
			}

			company.Verified = true;
			var existingCompany = await _companyService.GetByEmail(job.CompanyEmail);
			if (existingCompany != null)
			{
				_db.Attach(existingCompany);
				// existingCompany.Logo = job.CompanyLogo;
				existingCompany.Location = job.CompanyLocation;
				//existingCompany.Description = job.CompanyDescription;
				existingCompany.Website = job.CompanyWebsite.RemoveSubstrings("www.", "https://");
				existingCompany.Name = job.CompanyName;
				existingCompany.Slug = company.Slug;
				_db.SaveChanges();
				return existingCompany;
			}
			else
			{
				company.Email = job.CompanyEmail;
				//company.Logo = job.CompanyLogo;
				company.Location = job.CompanyLocation;
				//company.Description = job.CompanyDescription;
				company.Website = job.CompanyWebsite.RemoveSubstrings("www.", "https://", "http://");
				company.Name = job.CompanyName;
				_db.Companies.Add(company);
				_db.SaveChanges();
				return company;
			}
		}

		public async Task<Job> CreateJob(CreateJobForm checkoutJob, Company company)
		{
			var job = new Job();
			job.CompanyId = company.Id;
			job.CreatedAt = DateTime.Now;
			job.HasPaid = false;
			job.IsPremium = true;
			job.ApplyCount = 0;
			job.ViewCount = 0;
			job.PaymentCallbackGuid = Guid.NewGuid().ToString();
			job.Slug = StringHelpers.Slugify($"{checkoutJob.Title} {company.Name}");
			job.StripeIdentifier = "";

			var existingJobBySlug = await _jobService.GetBySlug(job.Slug);
			if (existingJobBySlug != null)
			{
				job.Slug = job.Slug + "-" + Guid.NewGuid().ToString();
			}
			job.Title = checkoutJob.Title;
			//job.Description = checkoutJob.Description;
			job.WorkTypeId = checkoutJob.WorkTypeId;
			job.RemotePolicyId = checkoutJob.RemotePolicyId;
			job.ExperienceLevelId = checkoutJob.ExperienceLevelId;
			job.SalaryStart = checkoutJob.SalaryStart;
			job.SalaryEnd = checkoutJob.SalaryEnd;
			job.ApplyLink = checkoutJob.ApplyLink.RemoveSubstrings("www.", "https://");

			if (!String.IsNullOrEmpty(checkoutJob.JobLocation))
				job.Location = checkoutJob.JobLocation;

			if (!String.IsNullOrEmpty(checkoutJob.JobLocationRestrictions))
				job.LocationRestrictions = checkoutJob.JobLocationRestrictions;
			// Convert location restrictions to colon seperated list
			//if (checkoutJob.LocationRestrictions.Count > 0)
			//{
			//	var locationRestrictions = "";
			//	foreach (var location in checkoutJob.JobLocationRestrictions)
			//	{
			//		var verifiedLocation = LocationRestrictions.GetAll().FirstOrDefault(x => x.Value == location);
			//		if (verifiedLocation != null)
			//		{
			//			locationRestrictions += $"{verifiedLocation.Value};";
			//		}
			//	}
			//	job.LocationRestrictions = locationRestrictions;
			//}

			_db.Jobs.Add(job);
			_db.SaveChanges();
			return job;
		}

		public Session CreateStripeUrl(Job job)
		{
			var priceId = Env.Get("STRIPE_PRICE_ID");
			var domain = Env.Get("APP_URL");

			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>
					{
					  new SessionLineItemOptions
					  {
						Price = priceId,
						Quantity = 1,
					  },
					},
				Mode = "payment",
				SuccessUrl = domain + $"/post-job/success/{job.PaymentCallbackGuid}",
				CancelUrl = domain + "/post-job",
			};
			var service = new SessionService();
			return service.Create(options);
		}
	}
}
