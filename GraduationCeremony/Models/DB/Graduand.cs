using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationCeremony.Models.DB
{
    public partial class Graduand
    {
        public Graduand()
        {
            GraduandAwards = new HashSet<GraduandAward>();
        }

        public int PersonCode { get; set; }
        public string Forenames { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Nsn { get; set; }
        public string? BadDebtStatus { get; set; }

        //removing the time
        [DataType(DataType.Date)]

        public DateTime DateOfBirth { get; set; }
        public string? Ethnicity1 { get; set; }
        public string? Ethnicity2 { get; set; }
        public string? Ethnicity3 { get; set; }
        public string? Iwi1 { get; set; }
        public string? Iwi2 { get; set; }
        public string? Iwi3 { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? AddressLine4 { get; set; }
        public string? Town { get; set; }
        public string? Postcode { get; set; }
        public string? CollegeEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public string? Mobile { get; set; }
        public string? Campus { get; set; }
        public string? School { get; set; }

        public virtual ICollection<GraduandAward> GraduandAwards { get; set; }
    }
}
