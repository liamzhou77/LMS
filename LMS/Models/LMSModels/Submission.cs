﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public uint AssId { get; set; }
        public string UId { get; set; }
        public uint Score { get; set; }
        public string Contents { get; set; }
        public DateTime Time { get; set; }

        public virtual Assignments Ass { get; set; }
        public virtual Students U { get; set; }
    }
}
