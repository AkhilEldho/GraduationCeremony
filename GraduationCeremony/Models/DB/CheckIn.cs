using System.ComponentModel.DataAnnotations;

namespace GraduationCeremony.Models.DB
{
    public class CheckIn
    {
        [Key]
        public int PersonCode { get; set; }
        public string? Forenames { get; set; }
        public string? Surname { get; set; }
        public int? Nsn { get; set; }
        public string? AwardCode { get; set; }
        public string? QualificationCode { get; set; }
        public string? AwardDescription { get; set; }
        public string? Level { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CollegeEmail { get; set; }
        public int? Mobile { get; set; }
    }
}
