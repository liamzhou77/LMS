using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submission = new HashSet<Submission>();
        }

        public string Name { get; set; }
        public uint CatId { get; set; }
        public string Contents { get; set; }
        public DateTime Due { get; set; }
        public uint Points { get; set; }
        public uint AssId { get; set; }

        public virtual AssignmentCat Cat { get; set; }
        public virtual ICollection<Submission> Submission { get; set; }
    }
}
