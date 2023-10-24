﻿using System;
using System.Collections.Generic;

namespace GraduationCeremony.Models.DB
{
    public partial class GraduandAward
    {
        public int GraduandAwardId { get; set; }
        public string AwardCode { get; set; } = null!;
        public int PersonCode { get; set; }
        public string? Major1 { get; set; }
        public string? Major2 { get; set; }
        public DateTime Completion { get; set; }
        public DateTime Awarded { get; set; }
        public DateTime YearAchieved { get; set; }
        public string? IncludeInSdr { get; set; }
        public string? AcademicDressRequirements1 { get; set; }
        public string? AcademicDressRequirements2 { get; set; }
    }
}
