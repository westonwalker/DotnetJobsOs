using Spark.Library.Database;
using System.ComponentModel.DataAnnotations;

namespace DotnetJobs.Application.Models
{
    public class WorkType
    {
        public int Id { get; set; }

        // Full time, part time, internship, freelance, contract
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
