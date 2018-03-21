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
    
    public partial class BookingInfo
    {
        public int ID { get; set; }
        public string AlternateID { get; set; }
        public int QuotationID { get; set; }
        public string OrderNumber { get; set; }
        public int PaymentModeID { get; set; }
        public string PaymentReferenceNumber { get; set; }
        public double AmountPaid { get; set; }
        public bool IsApprovedByAdmin { get; set; }
        public bool IsConfirmationEnabled { get; set; }
        public string Comments { get; set; }
        public string InvoiceFileName { get; set; }
        public Nullable<bool> IsBookingApproved { get; set; }
        public string ApprovalComments { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual PaymentMode PaymentMode { get; set; }
        public virtual BookingQuoteInfo BookingQuoteInfo { get; set; }
    }
}
