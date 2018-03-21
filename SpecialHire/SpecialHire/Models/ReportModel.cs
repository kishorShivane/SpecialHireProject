using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Models
{
    public class ReportModel
    {
        public string QuotationNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrganiserName { get; set; }
        public string Companyname { get; set; }
        public DateTime QuotationDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceTotal { get; set; }
        public string BusType { get; set; }
        public string FleetNumber { get; set; }
        public string BookingDone { get; set; }
        public string DriverName { get; set; }
    }
}