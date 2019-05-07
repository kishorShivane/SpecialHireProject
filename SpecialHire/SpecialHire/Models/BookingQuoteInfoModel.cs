using EbusDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SpecialHire.Models
{
    public class BookingQuoteInfoModel: BookingQuoteInfoBase
    {
        public BookingQuoteInfoModel()
        {
            BookingInfo = new BookinginfoModel();
        }

        public BookinginfoModel BookingInfo { get; set; }
        public List<SelectListItem> Titles { get; set; }
        public List<SelectListItem> Time { get; set; }
        public List<SelectListItem> Events { get; set; }
        public List<SelectListItem> PaymentTerms { get; set; }
        public List<SelectListItem> PaymentModes { get; set; }
        public List<SelectListItem> BusTypes { get; set; }
        public List<SelectListItem> TrailerTypes { get; set; }
        public List<SelectListItem> CompanyDetails { get; set; }        
    }

    public class BookingQuoteInfoBase
    {

        public BookingQuoteInfoBase()
        {
            BookingVehicleInfo = new List<BookingVehicleInfoModel>();
            BookingTrailerInfo = new List<BookingTrailerInfoModel>();
        }
        public int ID { get; set; }
        public string AlternateID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string TelephoneNumber { get; set; }
        public string CellNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string CompTelephoneNumber { get; set; }
        public string CompTelephoneExtension { get; set; }
        public string FaxNumber { get; set; }
        public bool IsReturnJourney { get; set; }
        public bool IsSingleJourney { get; set; }
        public bool IsTrailerRequired { get; set; }
        public System.DateTime PickUpDate { get; set; }
        public string PickUpTime { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string ReturnTime { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public double Distance { get; set; }
        public int Passengers { get; set; }
        public int EventID { get; set; }
        public string EventDescription { get; set; }
        public string ExtraInformation { get; set; }
        public int PaymentTermsID { get; set; }
        public string PaymentTerm { get; set; }
        public bool IsQuoteValidTillAdded { get; set; }
        public System.DateTime QuoteValidTill { get; set; }
        public double QuotationValue { get; set; }
        public int CompanyID { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyPrefix { get; set; }
        public string CompanyNameAddress { get; set; }
        public string QuotationFileName { get; set; }
        public string PDFPaymentTerms { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public List<BookingVehicleInfoModel> BookingVehicleInfo { get; set; }
        public List<BookingTrailerInfoModel> BookingTrailerInfo { get; set; }
    }


    public class CompanyConfigurationInfo
    {
        public int ID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
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
    }

    public class BookinginfoModel
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
    }
}