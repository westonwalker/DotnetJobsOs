using DotnetJobs.Application.Database;
using DotnetJobs.Application.Events.Listeners;
using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services.Auth;
using DotnetJobs.Application.Services;
using Spark.Library.Database;
using Coravel;
using Spark.Library.Auth;
using DotnetJobs.Application.Jobs;
using DotnetJobs.Pages.Jobs;
using DotnetJobs.Pages.Subscribers;
using FluentValidation;
using Spark.Library.Mail;
using Vite.AspNetCore.Extensions;
using DotnetJobs.Pages.Auth;

namespace DotnetJobs.Application.Startup;

public static class AppServiceRegistration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddViteServices();
        services.AddCustomServices();
        services.AddDatabase<DatabaseContext>(config);
        services.AddAuthorization(config, new string[] { CustomRoles.Admin, CustomRoles.User });
        services.AddAuthentication<IAuthValidator>(config);
        services.AddJobServices();
        services.AddScheduler();
        services.AddQueue();
        services.AddEventServices();
        services.AddEvents();
        services.AddMailer(config);
        services.AddRazorPages();
        services.AddDistributedMemoryCache();
        services.AddSession(options => {
            options.Cookie.Name = ".dotnetjobs";
            options.IdleTimeout = TimeSpan.FromMinutes(1);
        });
        return services;
    }

    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // add custom services
        services.AddScoped<UsersService>();
        services.AddScoped<RolesService>();
		services.AddScoped<IAuthValidator, SparkAuthValidator>();
		services.AddScoped<AuthService>();
		services.AddScoped<JobService>();
        services.AddScoped<CompanyService>();
        services.AddScoped<ExperienceLevelService>();
        services.AddScoped<RemotePolicyService>();
        services.AddScoped<WorkTypeService>();
        services.AddScoped<EmailService>();
        services.AddScoped<ImageKitService>();
        services.AddScoped<CheckoutService>();
        services.AddScoped<ConvertKit>();
        services.AddScoped<IValidator<CreateSubscriber>, CreateSubscriber.Validator>();
        services.AddScoped<IValidator<CreateJobForm>, CreateJobForm.Validator>();
		services.AddScoped<IValidator<LoginForm>, LoginFormValidator>();
		return services;
    }

    private static IServiceCollection AddEventServices(this IServiceCollection services)
    {
        // add custom events here
        services.AddTransient<EmailNewUser>();
        return services;
    }

    private static IServiceCollection AddJobServices(this IServiceCollection services)
    {
        // add custom background tasks here
        services.AddTransient<ExampleJob>();
        return services;
    }
}
