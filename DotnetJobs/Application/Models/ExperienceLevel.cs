using Spark.Library.Database;
using System.ComponentModel.DataAnnotations;

namespace DotnetJobs.Application.Models
{
    public class ExperienceLevel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
