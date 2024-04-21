using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spark.Library.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetJobs.Application.Models
{
    public class Company : BaseModel
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

		[StringLength(250)]
		[Required]
		public string Email { get; set; }

		[StringLength(250)]
		[Required]
		public string Location { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

		[StringLength(250)]
		public string? Logo { get; set; }

        [Required]
        [StringLength(250)]
        public string Website { get; set; }

        public bool Verified { get; set; } = false;

        public virtual List<Job> Jobs { get; set; }
	}
}
