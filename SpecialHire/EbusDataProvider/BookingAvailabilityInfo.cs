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
    
    public partial class BookingAvailabilityInfo
    {
        public System.DateTime PickUpDate { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public Nullable<int> NoOfBooking { get; set; }
        public Nullable<int> NoOfBusses { get; set; }
        public Nullable<int> NoOfTrailers { get; set; }
        public Nullable<int> TotalBusses { get; set; }
        public Nullable<int> TotalTrailer { get; set; }
        public int NoOfBussesConfirmed { get; set; }
        public int NoOfTrailersConfirmed { get; set; }
    }
}
