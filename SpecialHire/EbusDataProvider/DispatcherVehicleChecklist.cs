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
    
    public partial class DispatcherVehicleChecklist
    {
        public int ID { get; set; }
        public int QuotationID { get; set; }
        public int BookingVehicleID { get; set; }
        public bool FullUniform { get; set; }
        public bool BriefedPickUpPoint { get; set; }
        public bool BriefedFinalDestination { get; set; }
        public bool Permit { get; set; }
        public bool JobCardAndMaps { get; set; }
        public bool RadioFace { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual BookingQuoteinfo BookingQuoteinfo { get; set; }
        public virtual BookingVehicleInfo BookingVehicleInfo { get; set; }
    }
}
