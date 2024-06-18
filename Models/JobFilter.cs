using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JobSeed.Models
{
    public class JobFilter
    {
        public string? SearchKeyword{set;get;}=string.Empty;
        public string? Location{set;get;}=string.Empty;
        public int? Category{set;get;}
        public int? JobType{set;get;}
        public bool? Gender{set;get;}=false;
    }
}