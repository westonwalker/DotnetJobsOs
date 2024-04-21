using DotnetJobs.Application.Jobs;
using Coravel;

namespace DotnetJobs.Application.Startup;

public static class ScheduledJobsRegistration
{
    public static IServiceProvider SetupScheduledJobs(this IServiceProvider services)
    {
        services.UseScheduler(scheduler =>
        {
            // example scheduled job
            //scheduler
            //    .Schedule<ExampleJob>()
            //    .EveryFiveMinutes();
        });
        return services;
    }
}
