using System;
using System.Collections.Generic;

namespace GraduationCeremony.Models.DB
{
    public partial class Graduation
    {
        public int PersonCode { get; set; }
        public string? Forenames { get; set; }
        public string? Surname { get; set; }
        public int? Nsn { get; set; }
        public string? AwardCode { get; set; }
        public string? QualificationCode { get; set; }
        public string? AwardDescription { get; set; }
        public string? Level { get; set; }
        public string? Major1 { get; set; }
        public string? Major2 { get; set; }
        public short? Credits { get; set; }
        public DateTime? Completion { get; set; }
        public string? Awarded { get; set; }
        public short? YearArchieved { get; set; }
        public byte? BadDebtStatus { get; set; }
        public DateTime? DateOfBirth { get; set; }
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
        public string? PostCode { get; set; }
        public string? CollegeEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public int? Mobile { get; set; }
        public string? Campus { get; set; }
        public string? School { get; set; }
    }
}
