using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages.Jobs;

public class Show : PageModel
{
	public Job? Job { get; set; }
	private readonly JobService _jobService;

	public Show(JobService jobService)
	{
		_jobService = jobService;
	}
	
	public async Task<IActionResult> OnGet(string slug)
	{
		Job = await _jobService.GetBySlug(slug);
		if (Job == null)
			return Redirect("/");

		_jobService.UpdateJobViews(Job);
		return Page();
	}
	
}