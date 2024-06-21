using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobSeed.Models
{
    [Table("UsersJob")]
    public class UsersJob
    {
        public int JobId{set;get;}
        public int UsersId{set;get;}
        public string SocialLinkAccount{set;get;}=string.Empty;
        public string Coverletter{set;get;}=string.Empty;
        public DateTime ApplyDate{set;get;}=DateTime.Now;
        [NotMapped]
        public IFormFile? FormFile{set;get;}
        public string? CV{set;get;}
        public Job? Job{set;get;}
        public Users? Users{set;get;}
                



    }
}