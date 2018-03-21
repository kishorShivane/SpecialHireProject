using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class BookingAvailabilityInfoModel
    {
        public System.DateTime PickUpDate { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public Nullable<int> NoOfBusses { get; set; }
        public Nullable<int> NoOfTrailers { get; set; }
        public Nullable<int> TotalBusses { get; set; }
        public Nullable<int> TotalTrailer { get; set; }

        public string ID { get; set; }
        public string Title { get; set; }
        public int SomeImportantKeyID { get; set; }
        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public string StatusString { get; set; }
        public string Color { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
    }
}