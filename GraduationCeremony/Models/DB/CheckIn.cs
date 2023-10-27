﻿using System;
using System.Collections.Generic;

namespace GraduationCeremony.Models.DB
{
    public partial class CheckIn
    {
        public int PersonCode { get; set; }
        public string Forenames { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Nsn { get; set; }
        public string AwardCode { get; set; } = null!;
        public string QualificationCode { get; set; } = null!;
        public string AwardDescription { get; set; } = null!;
        public string Level { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string CollegeEmail { get; set; } = null!;
        public string? Mobile { get; set; }
    }
}
