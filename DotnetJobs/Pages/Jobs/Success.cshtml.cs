using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages.Jobs;

public class Success : PageModel
{
	public Job Job { get; set; }

	private readonly JobService _jobService;
	private readonly EmailService _emailService;

	public Success(JobService jobService, EmailService emailService)
	{
		_jobService = jobService;
		_emailService = emailService;
	}

	public async Task OnGet(string paymentGuid)
	{
		var job = await _jobService.GetByPaymentId(paymentGuid);
		if (job == null)
		{
			Redirect("/");
		}

		Job = job;

		if (!job.HasPaid)
		{
			job = _jobService.UpdateHasPaid(job);
			_emailService.SendAdminAlert(job);
		}
	}
}
