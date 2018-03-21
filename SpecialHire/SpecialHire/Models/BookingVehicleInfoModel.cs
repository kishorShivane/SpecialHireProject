using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpecialHire.Models
{
    public class BookingVehicleInfoModel
    {
        public int ID { get; set; }
        public string BusType { get; set; }
        public int BusTypeID { get; set; }
        public int Capacity { get; set; }
        public int Standing { get; set; }
        public int Sitting { get; set; }
        public bool AC { get; set; }
        public bool TV { get; set; }
        public bool Toilet { get; set; }
        public bool Curtains { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

    }
}