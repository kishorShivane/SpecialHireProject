using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Models
{
    public class TrailerModel
    {
        public int ID { get; set; }
        public int TrailerTypeID { get; set; }
        public string TrailerType { get; set; }
        public string FleetNumber { get; set; }
        public string VINNumber { get; set; }
        public string Registration { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public  List<SelectListItem> TrailerTypes { get; set; }
    }
}