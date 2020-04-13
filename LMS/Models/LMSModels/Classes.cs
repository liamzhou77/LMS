using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCat = new HashSet<AssignmentCat>();
            Enrolled = new HashSet<Enrolled>();
        }

        public uint Year { get; set; }
        public string Season { get; set; }
        public uint CourseId { get; set; }
        public string Loc { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string UId { get; set; }
        public uint ClassId { get; set; }

        public virtual Courses Course { get; set; }
        public virtual Professors U { get; set; }
        public virtual ICollection<AssignmentCat> AssignmentCat { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
    }
}
