using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JobSeed.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set;}=string.Empty;
        public List<Job>? Jobs { get; set; }
    }
}