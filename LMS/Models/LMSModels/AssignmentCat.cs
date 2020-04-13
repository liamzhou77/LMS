using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCat
    {
        public AssignmentCat()
        {
            Assignments = new HashSet<Assignments>();
        }

        public string Name { get; set; }
        public uint ClassId { get; set; }
        public uint Weight { get; set; }
        public uint CatId { get; set; }

        public virtual Classes Class { get; set; }
        public virtual ICollection<Assignments> Assignments { get; set; }
    }
}
