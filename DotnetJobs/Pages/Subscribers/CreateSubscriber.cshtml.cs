using System.ComponentModel.DataAnnotations;
using DotnetJobs.Application.Helpers;
using DotnetJobs.Application.Services;
using FluentValidation;
using Hydro;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using Spark.Library.Environment;

namespace DotnetJobs.Pages.Subscribers;

public class CreateSubscriber : HydroComponent
{
	[Required]
	public string Email { get; set; }
	public bool IsSubscribed { get; set; }
	private readonly ConvertKit _convertKit;
	private readonly IValidator<CreateSubscriber> _validator;

	public CreateSubscriber(ConvertKit convertKit, IValidator<CreateSubscriber> validator)
	{
		_convertKit = convertKit;
		_validator = validator;
	}
	
	public async Task Subscribe()
	{
		if (!this.Validate(_validator))
		{
			return;
		}
		if (Env.Get("ASPNETCORE_ENVIRONMENT") != "Development")
		{
			await _convertKit.Subscribe(Email);
		}
		IsSubscribed = true;
	}
    
	public class Validator : AbstractValidator<CreateSubscriber>
	{
		public Validator()
		{
			RuleFor(c => c.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email");
		}
	}
}