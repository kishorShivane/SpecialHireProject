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
    
    public partial class CompanyConfigurationInfo
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string ConnectionKey { get; set; }
        public Nullable<int> MinimumDistance { get; set; }
        public Nullable<int> DistanceCalculation { get; set; }
        public string DefaultEventDescription { get; set; }
        public Nullable<int> QuotationValidityPeriod { get; set; }
        public Nullable<int> QuotationMinimumValue { get; set; }
        public Nullable<bool> EnforcePassengerCount { get; set; }
        public string DefaultCompanyDetails { get; set; }
        public string DefaultPaymentTerms { get; set; }
        public string PDFPaymentTerms { get; set; }
        public string DefaultPaymentMode { get; set; }
        public string QuotationEmailTemplate { get; set; }
        public string InvoiceEmailTemplate { get; set; }
        public string QuotationPDFPath { get; set; }
        public string InvoicePDFPath { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public string EmailSMTP { get; set; }
        public string EmailPort { get; set; }
        public Nullable<bool> Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual Company Company { get; set; }
    }
}