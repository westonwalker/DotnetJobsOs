using Spark.Library.Database;
using System.ComponentModel.DataAnnotations;

namespace DotnetJobs.Application.Models
{
    public class RemotePolicy
    {
        public int Id { get; set; }

        // Remote, Hybrid, In Office
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
