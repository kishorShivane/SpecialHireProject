using EbusDataProvider;
using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SpecialHire.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult TrackBookings()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult SearchBookingsForTrackBookings(string OrganiserName, string CompanyName, string InvoiceNumber,
            string DriverName, string DriverNumber, string FleetNumber, string QuotationNumber, string DateFrom, string DateTo)
        {
            try
            {
                var response = DBHelper.SearchBookingsForTrackBookings(OrganiserName, CompanyName, InvoiceNumber, DriverName, DriverNumber, FleetNumber, QuotationNumber, DateFrom, DateTo);
                Session["TRACKBOOKINGS"] = response;
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ExportTrackBookingReportToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();

            if (Session["TRACKBOOKINGS"] != null)
            {
                var resp = (List<BookingReporting>)Session["TRACKBOOKINGS"];
                grid.DataSource = from r in resp
                                  select new
                                  {
                                      QuotationNo = r.QuotationNumber,
                                      InvoiceNo = r.InvoiceNumber,
                                      Organiser = r.OrganiserName,
                                      Company = r.CompanyName,
                                      QuotationDate = r.QuotationDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoicedDate = r.InvoiceDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoiceTotal = r.InvoiceTotal,
                                      BusType = r.BusType,
                                      FleetNo = r.FleetNumber
                                  };
            }

            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=TrackBooking_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        public ActionResult ExportTrackBookingReportToPDF()
        {
            var resp = new List<BookingReporting>();
            try
            {
                if (Session["TRACKBOOKINGS"] != null)
                {
                    resp = (List<BookingReporting>)Session["TRACKBOOKINGS"];
                }
                return new Rotativa.ViewAsPdf("PreviewTrackBookingReportToPDF", resp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult Quotations()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult SearchBookingsForQuotations(string ReportType, string DateFrom, string DateTo)
        {
            try
            {
                var response = DBHelper.SearchBookingsForQuotations(ReportType, DateFrom, DateTo);
                Session["QUOTATIONS"] = response;
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ExportQuotationsToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();

            if (Session["QUOTATIONS"] != null)
            {
                var resp = (List<BookingReporting>)Session["QUOTATIONS"];
                grid.DataSource = from r in resp
                                  select new
                                  {
                                      QuotationNo = r.QuotationNumber,
                                      BookingDone = r.IsBookingDone,
                                      InvoiceNo = r.InvoiceNumber,
                                      Organiser = r.OrganiserName,
                                      Company = r.CompanyName,
                                      QuotationDate = r.QuotationDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoicedDate = r.InvoiceDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoiceTotal = r.InvoiceTotal,
                                      BusType = r.BusType,
                                      FleetNo = r.FleetNumber
                                  };
            }

            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Quotations_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        public ActionResult ExportQuotationsReportToPDF()
        {
            var resp = new List<BookingReporting>();
            try
            {
                if (Session["QUOTATIONS"] != null)
                {
                    resp = (List<BookingReporting>)Session["QUOTATIONS"];
                }
                return new Rotativa.ViewAsPdf("PreviewQuotationsReportToPDF", resp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult Invoices()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult SearchBookingsForInvoices(string DateFrom, string DateTo)
        {
            try
            {
                var response = DBHelper.SearchBookingsForInvoices(DateFrom, DateTo);
                Session["INVOICES"] = response;
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ExportInvoicesToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();

            if (Session["INVOICES"] != null)
            {
                var resp = (List<BookingReporting>)Session["INVOICES"];
                grid.DataSource = from r in resp
                                  select new
                                  {
                                      QuotationNo = r.QuotationNumber,
                                      BookingDone = r.IsBookingDone,
                                      InvoiceNo = r.InvoiceNumber,
                                      Organiser = r.OrganiserName,
                                      Company = r.CompanyName,
                                      InvoicedDate = r.InvoiceDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoiceTotal = r.InvoiceTotal,
                                      BusType = r.BusType,
                                      FleetNo = r.FleetNumber,
                                      DriverName = r.DriverName
                                  };
            }

            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Invoices_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        public ActionResult ExportInvoicesReportToPDF()
        {
            var resp = new List<BookingReporting>();
            try
            {
                if (Session["INVOICES"] != null)
                {
                    resp = (List<BookingReporting>)Session["INVOICES"];
                }
                return new Rotativa.ViewAsPdf("PreviewInvoicesReportToPDF", resp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult Revenue()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetMonthlyRevenue(string year)
        {
            try
            {
                var response = DBHelper.GetMonthlyRevenue(year);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ScheduledWorked()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public JsonResult SearchBookingsForScheduledWorked(string OrganiserName, string CompanyName, string InvoiceNumber,
            string DriverName, string DriverNumber, string FleetNumber, string QuotationNumber, string DateFrom, string DateTo)
        {
            try
            {
                var response = DBHelper.SearchBookingsForScheduledWorked(OrganiserName, CompanyName, InvoiceNumber, DriverName, DriverNumber, FleetNumber, QuotationNumber, DateFrom, DateTo);
                Session["SCHEDULEDWORKED"] = response;
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void ExportScheduledWorkedToExcel()
        {
            var grid = new System.Web.UI.WebControls.GridView();

            if (Session["SCHEDULEDWORKED"] != null)
            {
                var resp = (List<BookingReporting>)Session["SCHEDULEDWORKED"];
                grid.DataSource = from r in resp
                                  select new
                                  {
                                      InvoiceNumber = r.InvoiceNumber,
                                      OrganiserName = r.OrganiserName,
                                      Organiser = r.OrganiserName,
                                      CompanyName = r.CompanyName,
                                      QuotationDate = r.QuotationDate.Value.ToString("dd-MMMM-yyyy"),
                                      InvoiceDate = r.InvoiceDate.Value.ToString("dd-MMMM-yyyy"),
                                      Distance = r.Distance,
                                      OperatedKms = r.OperatedKms,
                                      DifferenceKms = r.DifferenceKms,
                                      QuotationValue = r.QuotationValue,
                                      InvoiceTotal = r.InvoiceTotal,
                                      PaymentDifference = r.PaymentDifference,
                                      BusType = r.BusType,
                                      FleetNumber = r.FleetNumber
                                  };
            }

            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=ScheduledVsWorked_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        public ActionResult ExportScheduledWorkedReportToPDF()
        {
            var resp = new List<BookingReporting>();
            try
            {
                if (Session["SCHEDULEDWORKED"] != null)
                {
                    resp = (List<BookingReporting>)Session["SCHEDULEDWORKED"];
                }
                return new Rotativa.ViewAsPdf("PreviewScheduledWorkedReportToPDF", resp);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}