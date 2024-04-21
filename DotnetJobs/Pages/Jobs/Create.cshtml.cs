using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotnetJobs.Application.Helpers;
using DotnetJobs.Application.Lib;
using DotnetJobs.Application.Models;
using DotnetJobs.Application.Services;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages.Jobs;

public class Create : PageModel
{
	public void OnGet()
	{
	}
}