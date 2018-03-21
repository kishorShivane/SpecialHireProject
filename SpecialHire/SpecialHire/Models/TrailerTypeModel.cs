using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class TrailerTypeModel
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string MaxWeight { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}