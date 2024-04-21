using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages.Admin;

[Authorize(Roles = CustomRoles.Admin)]
public class Dashboard : PageModel
{
	public List<Job> Jobs { get; set; }
	private readonly JobService _jobService;

	public Dashboard(JobService jobService)
	{
		_jobService = jobService;
	}

	public async Task OnGet()
	{
		Jobs = await _jobService.GetNewsletterJobs();
	}
}
