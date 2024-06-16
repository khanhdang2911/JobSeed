using System.ComponentModel.DataAnnotations.Schema;

namespace JobSeed.Models
{
    [Table("UsersRole")]
    public class UsersRole
    {
        public int UsersId{set;get;}
        public int RoleId{set;get;}
    }
}