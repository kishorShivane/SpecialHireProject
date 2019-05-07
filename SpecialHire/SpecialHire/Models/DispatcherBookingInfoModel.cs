using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class DispatcherBookingInfoModel
    {
        public string OrganiserName { get; set; }
        public string CellNumber { get; set; }
        public string PickUpTime { get; set; }
        public string ReturnTime { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public int QuotationID { get; set; }
        public int BookingID { get; set; }
        public int BookingVehicleID { get; set; }
        public string BusType { get; set; }
        public bool IsTrailerAssigned { get; set; }
        public bool IsVehicleAssigned { get; set; }
        public bool IsJobCompleted { get; set; }
        public int NumberOfBusses{ get; set; }
        public int NumberOfTrailers { get; set; }
        public string FleetNumber { get; set; }
        public string DriverName { get; set; }
        public int NumberOfBussesAssigned { get; set; }
        public int NumberOfTrailersAssigned { get; set; }
    }
}