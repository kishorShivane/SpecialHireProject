//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EbusDataProvider
{
    using System;
    using System.Collections.Generic;
    
    public partial class Driver
    {
        public int ID { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
        public string EmployeeNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Depot { get; set; }
        public bool IsSpecialHireDriver { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
