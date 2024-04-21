using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spark.Library.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetJobs.Application.Models
{
    public class Job : BaseModel
    {
        [Required]
		[StringLength(250)]
		public string Slug { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        public string? Description { get; set; }

        [Required]
        public int WorkTypeId { get; set; }
        public virtual WorkType WorkType { get; set; }

        [Required]
        public int RemotePolicyId { get; set; }
        public virtual RemotePolicy RemotePolicy { get; set; }

        [Required]
        public int ExperienceLevelId { get; set; }
        public virtual ExperienceLevel ExperienceLevel { get; set; }

        public int? SalaryStart { get; set; }

        public int? SalaryEnd { get; set; }

        [Required]
		public string ApplyLink { get; set; }

        public int? ViewCount { get; set; }

        public int? ApplyCount { get; set; }

        public bool IsPremium { get; set; } = false;

		public bool HasPaid { get; set; } = false;

		[StringLength(250)]
		public string StripeIdentifier { get; set; }

		[StringLength(250)]
		public string PaymentCallbackGuid { get; set; }

		public string? LocationRestrictions { get; set; }

		public string? Location { get; set; }
		
		public bool IsClosed { get; set; } = false;
	}
}
