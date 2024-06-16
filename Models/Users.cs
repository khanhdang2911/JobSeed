using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobSeed.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int Id{set;get;}
        [EmailAddress(ErrorMessage ="You entered an email in the wrong format")]
        [Required(ErrorMessage ="You have not entered your email yet")]
        public string Email{set;get;}
        [Required(ErrorMessage ="You have not entered your name")]
        public string Name{set;get;}
        [Required(ErrorMessage ="You did not enter a phone number")]
        [Phone(ErrorMessage ="Enter the phone number in the wrong format")]
        public string Phone{set;get;}
        public string Password{set;get;}
        public ICollection<UsersRole>? usersRoles{set;get;}



    }
}