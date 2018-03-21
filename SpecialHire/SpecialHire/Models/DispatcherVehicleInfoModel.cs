using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Models
{
    public class DispatcherVehicleInfoModel
    {
        public List<SelectListItem> BusNumbers{ get; set; }
        public List<SelectListItem> Drivers { get; set; }
        public List<SelectListItem> TrailerNumbers { get; set; }

        public int QuotationID { get; set; } = 0;
        public int BookingVehicleID { get; set; } = 0;
        public bool DidHeKnowTheRoute { get; set; }
        public bool DidHeAdhereToTrafficRules { get; set; }
        public bool AreMapsAvailable { get; set; }
        public int BusID { get; set; } = 0;
        public int DriverID { get; set; } = 0;
        public int TrailerID { get; set; } = 0;
        public bool FullUniform { get; set; }
        public bool BriefedPickUpPoint { get; set; }
        public bool BriefedFinalDestination { get; set; }
        public bool Permit { get; set; }
        public bool JobCardAndMaps { get; set; }
        public bool RadioFace { get; set; }
        public bool WasDrivingOnTime { get; set; }
        public bool WasDriverProperlyAttired { get; set; }
        public bool IsTrailerAssigned { get; set; }
        public bool IsVehicleAssigned { get; set; }
        public bool IsJobCompleted { get; set; }
        public string TimeOfLeavingDepot { get; set; }
        public string SpeedoReadingBeforeLeavingFromDepot { get; set; }
        public string TimeOfReturnDepot { get; set; }
        public string SpeedoReadingReturnToDepot { get; set; }
        public string CustomerName { get; set; }
        public string CustomerComments { get; set; }
        public string ToLocation { get; set; }
        public string FromLocation { get; set; }
        public List<SelectListItem> Times { get; set; }

    }
}