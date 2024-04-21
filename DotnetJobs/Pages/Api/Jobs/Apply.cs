using DotnetJobs.Application.Services;
using Spark.Library.Routing;

namespace DotnetJobs.Pages.Api.Jobs;

public class Apply : IRoute
{
	public void Map(WebApplication app)
	{
		app.MapGet("/api/jobs/apply/{id}", (int id, JobService jobService) =>
		{
			var job = jobService.Find(id);
			if (job == null)
				return Results.NotFound();

			jobService.UpdateApplyCount(job);
			return Results.Redirect($"https://{job.ApplyLink}");
		});
	}
}