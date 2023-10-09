using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationCeremony.Models.DB
{
    public partial class Graduation
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
        public string? Major1 { get; set; }
        public string? Major2 { get; set; }
        public short? Credits { get; set; }
        public DateTime? Completion { get; set; }
        public DateTime? Awarded { get; set; }
        public DateTime? YearArchieved { get; set; }
        public bool? IncludeInSdr { get; set; }
        public string? Status { get; set; }
        public byte? BadDebtStatus { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Ethnicity1 { get; set; }
        public string? Ethnicity2 { get; set; }
        public string? Ethnicity3 { get; set; }
        public string? Iwi1 { get; set; }
        public short? Iwi2 { get; set; }
        public short? Iwi3 { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? AddressLine4 { get; set; }
        public string? Town { get; set; }
        public string? PostCode { get; set; }
        public string? CollegeEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public int? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Campus { get; set; }
        public string? School { get; set; }
        public string? AcademicDressRequirements1 { get; set; }
        public string? AcademicDressRequirements2 { get; set; }
        public string? Comments { get; set; }
        public DateTime? DateRecordAddedToMasterList { get; set; }
    }
}
