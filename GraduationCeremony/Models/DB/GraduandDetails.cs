using System.ComponentModel.DataAnnotations;
using System.Net;

namespace GraduationCeremony.Models.DB
{
    public class GraduandDetails
    {
        [Key]
        public int PersonCode { get; set; } 
        public Graduand graduands { get; set; }
        public Award awards { get; set; }
        public GraduandAward graduandAwards { get; set; }
    }
}

