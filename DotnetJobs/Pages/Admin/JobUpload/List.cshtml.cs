using DotnetJobs.Application.Database;
using DotnetJobs.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotnetJobs.Pages.Admin.JobUpload;

[Authorize(Roles = CustomRoles.Admin)]
public class List : PageModel
{
	public List<DotnetJobs.Application.Models.JobUpload> JobUploads { get; set; } = new();

	private readonly DatabaseContext _db;

	public List(DatabaseContext db)
	{
		_db = db;
	}

    public void OnGet()
	{
		JobUploads = _db.JobUploads.OrderByDescending(x => x.CreatedAt).ToList();
	}
}