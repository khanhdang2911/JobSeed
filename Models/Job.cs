using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JobSeed.Models
{
    [Table("Job")]
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string JobName { get; set;}=string.Empty;
        public string JobDescription { get; set;}=string.Empty;
        public DateTime PublishDate{set;get;}=DateTime.Now;
        public string CompanyName{set;get;}=string.Empty;
        public string Location{set;get;}=string.Empty;
        public decimal FromSalary{set;get;}
        public decimal ToSalary{set;get;}
        public bool Status{set;get;}=false;
        public int JobTypeId{set;get;}
        [NotMapped]
        public IFormFile FormFile{set;get;}
        public string? ImageLink{set;get;}

        public bool? Gender{set;get;}
        public string? Experiences{set;get;}=string.Empty;
        public string? Qualifications{set;get;}=string.Empty;
        public string? Benefits{set;get;}=string.Empty;
        [ForeignKey("JobTypeId")]
        public JobType? JobType{set;get;}

    }
}