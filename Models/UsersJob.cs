using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobSeed.Models
{
    [Table("UsersJob")]
    public class UsersJob
    {
        public int JobId{set;get;}
        public int UsersId{set;get;}
        public DateTime ApplyDate{set;get;}
        [NotMapped]
        public IFormFile? FormFile{set;get;}
        public string? ImageLink{set;get;}
        public Job? Job{set;get;}
        public Users? Users{set;get;}
                



    }
}