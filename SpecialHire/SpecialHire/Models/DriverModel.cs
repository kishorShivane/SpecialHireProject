using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class DriverModel
    {
        public int ID { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
        public string EmployeeNumber { get; set; }
        public string DriverContactNumber { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}