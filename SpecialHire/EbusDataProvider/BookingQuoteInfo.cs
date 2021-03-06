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
    
    public partial class BookingQuoteinfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BookingQuoteinfo()
        {
            this.Bookinginfoes = new HashSet<Bookinginfo>();
            this.BookingTrailerInfoes = new HashSet<BookingTrailerInfo>();
            this.DispatcherVehicleChecklists = new HashSet<DispatcherVehicleChecklist>();
            this.DispatcherVehicleCustomerSurveys = new HashSet<DispatcherVehicleCustomerSurvey>();
            this.BookingVehicleInfoes = new HashSet<BookingVehicleInfo>();
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
        public bool IsQuoteValidTill { get; set; }
        public bool IsTrailerAdded { get; set; }
        public System.DateTime PickUpDate { get; set; }
        public string PickUpTime { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string ReturnTime { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public double Distance { get; set; }
        public int Passengers { get; set; }
        public int EventID { get; set; }
        public string ExtraInformation { get; set; }
        public int PaymentTermsID { get; set; }
        public System.DateTime QuoteValidTill { get; set; }
        public double QuotationValue { get; set; }
        public int CompanyID { get; set; }
        public string QuotationFileName { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bookinginfo> Bookinginfoes { get; set; }
        public virtual CompanyDetail CompanyDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingTrailerInfo> BookingTrailerInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DispatcherVehicleChecklist> DispatcherVehicleChecklists { get; set; }
        public virtual Event Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DispatcherVehicleCustomerSurvey> DispatcherVehicleCustomerSurveys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingVehicleInfo> BookingVehicleInfoes { get; set; }
    }
}
