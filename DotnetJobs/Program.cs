using DotnetJobs.Application.Database;
using DotnetJobs.Application.Startup;
using DotnetJobs.Lib.Routing;
using DotnetJobs.Lib.Session;
using Hydro.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spark.Library.Config;
using Spark.Library.Environment;
using Spark.Library.Routing;
using Stripe;
using Vite.AspNetCore.Extensions;

EnvManager.LoadConfig();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetupAppConfig();
builder.Services.AddRazorComponents();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddAntiforgery();
builder.Services.AddHydro();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
	db.Database.Migrate();
}

StripeConfiguration.ApiKey = Env.Get("STRIPE_SECRET");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
	app.UseViteDevMiddleware();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseAntiforgery();
app.UseHydro(builder.Environment);
app.MapRazorPages();
app.MapMinimalApis<Program>();

app.Services.SetupScheduledJobs();
app.Services.SetupEvents();

app.Run();
