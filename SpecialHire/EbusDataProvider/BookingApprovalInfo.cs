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
    
    public partial class BookingApprovalInfo
    {
        public int ID { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string OrganiserName { get; set; }
        public string Contact { get; set; }
        public string PickUpTime { get; set; }
        public System.DateTime PickUpDate { get; set; }
        public string ReturnTime { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Company { get; set; }
        public bool IsBookingApproved { get; set; }
        public System.DateTime ReturnDate { get; set; }
    }
}
