using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class DriverWorkSheetModel
    {
        public int BookingID { get; set; }
        public string DriverNumber { get; set; }
        public string DispatcherName { get; set; }
        public DateTime PickUpDate { get; set; }
        public string CompanyName { get; set; }
        public string OrganiserName { get; set; }
        public string OrganiserNumber { get; set; }
        public string PickUpPoint { get; set; }
        public string PickUpTime { get; set; }
        public string Destination { get; set; }
        public string ReturnTime { get; set; }
        public string BusNumber { get; set; }
        public string TrailerNumber { get; set; }
        public string ExtraInformation { get; set; }
    }
}