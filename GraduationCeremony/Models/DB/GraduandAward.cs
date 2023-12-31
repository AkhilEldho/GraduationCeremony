﻿using System;
using System.Collections.Generic;

namespace GraduationCeremony.Models.DB
{
    public partial class GraduandAward
    {
        public GraduandAward()
        {
            CheckIns = new HashSet<CheckIn>();
        }

        public int GraduandAwardId { get; set; }
        public string AwardCode { get; set; } = null!;
        public int PersonCode { get; set; }
        public string? Major1 { get; set; }
        public string? Major2 { get; set; }
        public DateTime Completion { get; set; }
        public DateTime? Awarded { get; set; }
        public DateTime YearAchieved { get; set; }

        public virtual Award? AwardCodeNavigation { get; set; }
        public virtual Graduand? PersonCodeNavigation { get; set; } 
        public virtual ICollection<CheckIn> CheckIns { get; set; }
    }
}
