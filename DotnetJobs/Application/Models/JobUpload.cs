using Spark.Library.Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DotnetJobs.Application.Models;

public class JobUpload : BaseModel
{
	public string CompanyName { get; set; }
	public string ApplyLink { get; set; }
	[Column(TypeName = "text")]
	public string Description { get; set; }
	public string Status { get; set; }
}