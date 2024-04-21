using DotnetJobs.Application.Helpers;
using DotnetJobs.Application.Lib;
using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using FluentValidation;
using Hydro;
using System.ComponentModel.DataAnnotations;

namespace DotnetJobs.Pages.Jobs;

public class CreateJobForm : HydroComponent
{
	public List<RemotePolicy> RemotePolicies { get; set; } = new();
	public List<ExperienceLevel> ExperienceLevels { get; set; } = new();
	public List<WorkType> WorkTypes { get; set; } = new();
	public Dictionary<int, string> SalaryOptions { get; set; } = new();
	public List<LocationRestriction> LocationRestrictions { get; set; } = new();

	// create job form properties
	public string CompanyName { get; set; }
	public string CompanyEmail { get; set; }
	public string CompanyLocation { get; set; }
	public string CompanyLogo { get; set; }
	public string CompanyWebsite { get; set; }
	public string Title { get; set; }
	public int WorkTypeId { get; set; }
	public int RemotePolicyId { get; set; }
	public int ExperienceLevelId { get; set; }
	public int? SalaryStart { get; set; }
	public int? SalaryEnd { get; set; }
	public string ApplyLink { get; set; }
	public string JobLocation { get; set; }
	public string JobLocationRestrictions { get; set; }

	private readonly ExperienceLevelService _expService;
	private readonly RemotePolicyService _remotePolicyService;
	private readonly WorkTypeService _workTypeService;
	private readonly CheckoutService _checkoutService;
	private readonly IValidator<CreateJobForm> _validator;

	public CreateJobForm(IValidator<CreateJobForm> validator, ExperienceLevelService expService, RemotePolicyService remotePolicyService, WorkTypeService workTypeService, CheckoutService checkoutService)
	{
		_expService = expService;
		_remotePolicyService = remotePolicyService;
		_workTypeService = workTypeService;
		_checkoutService = checkoutService;
		_validator = validator;
	}
	
	public override async Task MountAsync()
	{
		await Setup();
	}

	public async Task Add()
	{
		var test = HttpContext.Request;
		if (!this.Validate(_validator))
		{
			return;
		}

		var savedCompany = await _checkoutService.CreateCompany(this);
		var savedJob = await _checkoutService.CreateJob(this, savedCompany);
		var stripeSession = _checkoutService.CreateStripeUrl(savedJob);
		Redirect(stripeSession.Url);
	}

	private async Task Setup()
	{
		ExperienceLevels = await _expService.GetAll();
		RemotePolicies = await _remotePolicyService.GetAll();
		WorkTypes = await _workTypeService.GetAll();
		LocationRestrictions = Application.Helpers.LocationRestrictions.GetAll();

		Dictionary<int, string> salaryOptions = new();
		var salary = 30000;
		while (salary <= 600000)
		{
			salaryOptions.Add(salary, $"${salary}K per year (USD)");
			salary += 10000;
		}
		SalaryOptions = salaryOptions;
	}
    
	public class Validator : AbstractValidator<CreateJobForm>
	{
		public Validator()
		{
			RuleFor(c => c.CompanyName)
				.NotEmpty().WithMessage("Company name is required");
			RuleFor(c => c.CompanyEmail)
				.NotEmpty().WithMessage("Company email is required");
			RuleFor(c => c.CompanyLocation)
				.NotEmpty().WithMessage("Company location is required");
			RuleFor(c => c.CompanyWebsite)
				.NotEmpty().WithMessage("Company website is required");
			RuleFor(c => c.Title)
				.NotEmpty().WithMessage("Title is required");
			RuleFor(c => c.Title)
				.NotEmpty().WithMessage("Title is required");
			RuleFor(c => c.WorkTypeId)
				.NotEmpty().WithMessage("Work type is required");
			RuleFor(c => c.RemotePolicyId)
				.NotEmpty().WithMessage("Remote policy is required");
			RuleFor(c => c.ExperienceLevelId)
				.NotEmpty().WithMessage("Experience level is required");
			RuleFor(c => c.ApplyLink)
				.NotEmpty().WithMessage("Apply link is required");
		}
	}
}