using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobSeed.Models
{
    [Table("JobType")]
    public class JobType
    {
        [Key]
        public int Id { get; set; }
        public string JobTitle{set;get;}=string.Empty;
        public List<Job>? Jobs{set;get;}
    }
}