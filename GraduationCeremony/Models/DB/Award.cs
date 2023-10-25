using System;
using System.Collections.Generic;

namespace GraduationCeremony.Models.DB
{
    public partial class Award
    {
        public string AwardCode { get; set; } = null!;
        public string QualificationCode { get; set; } = null!;
        public string AwardDescription { get; set; } = null!;
        public string Level { get; set; } = null!;
        public int Credits { get; set; }
        public string? School { get; set; }
    }
}
