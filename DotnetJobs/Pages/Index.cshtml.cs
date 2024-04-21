using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages;

public class Index : PageModel
{
    public List<Job> Jobs { get; set; } = new();
    public string LastPosted { get; set; } = String.Empty;
    public int NextPage { get; set; }
    public bool HasNext { get; set; }

    private readonly JobService _jobService;
    private readonly ConvertKit _convertKit;

    public Index(JobService jobService, ConvertKit convertKit)
    {
        _jobService = jobService;
        _convertKit = convertKit;
    }
    
    public async Task OnGet([FromQuery]int page = 1)
    {
        var jobCount = await _jobService.BoardJobsCount();
        var take = 15;
        var nextPage = page + 1;
		
        // get jobs
        Jobs = await _jobService.GetBoardJobs(page, take);
		
        // determine next pagination
        NextPage = nextPage;
        var currentCount = take * page; // 50
        var hasNext = jobCount >= currentCount; // 80 >= 50
        HasNext = hasNext;
		
        // get latest post and format as string
        if (Jobs.Count > 0)
        {
            var lastPosted = Jobs.Max(job => job.CreatedAt);
            var hours = (DateTime.Now - lastPosted).TotalHours;
            if (hours >= 24)
            {
                hours = Math.Round(hours / 24, 0);
                LastPosted = $"{hours} days ago";
            }
            else
            {
                hours = Math.Round(hours, 0, MidpointRounding.AwayFromZero);
                LastPosted = $"{hours} {(hours == 1 ? "hour ago" : "hours ago")}";
            }
        }
    }

    public async Task<IActionResult> OnPost([FromForm] string email)
    {
        if (!String.IsNullOrEmpty(email))
        {
            await _convertKit.Subscribe(email);
        }

        return Page();
    }
}
