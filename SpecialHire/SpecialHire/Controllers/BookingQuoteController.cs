using EbusDataProvider;
using Newtonsoft.Json;
using SpecialHire.Models;
using SpecialHire.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class BookingQuoteController : BaseController
    {
        public ActionResult Index()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserName = commonHelper.GetLoggedInUserName();
            BookingQuoteInfoModel bookingQuoteInfo = new BookingQuoteInfoModel();
            try
            {
                ViewBag.ConfigurationSettings = ConfigurationSettings;
                bookingQuoteInfo.Titles = commonHelper.GetTitles();
                bookingQuoteInfo.Time = commonHelper.GetTime();
                bookingQuoteInfo.Events = DBHelper.GetEvents();
                bookingQuoteInfo.PaymentTerms = DBHelper.GetPaymentTerms();
                bookingQuoteInfo.PaymentModes = DBHelper.GetPaymentModes();
                bookingQuoteInfo.BusTypes = DBHelper.GetBusTypes();
                bookingQuoteInfo.TrailerTypes = DBHelper.GetTrailerTypes();
                bookingQuoteInfo.CompanyDetails = DBHelper.GetCompanyDetails();
            }
            catch (System.Exception)
            {
                throw;
            }

            return View(bookingQuoteInfo);
        }

        [HttpPost]
        public JsonResult SearchBookingQuotes(string QuotationID, string PhoneNumber, string Name)
        {
            try
            {
                var response = DBHelper.SearchBookingQuotes(QuotationID, PhoneNumber, Name);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetQuotationByID(int QuotationID)
        {
            try
            {
                //var bookingQuote = new BookingQuoteInfo();
                var response = DBHelper.GetBookingQuoteByID(QuotationID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetBookingByQuotationID(int QuotationID)
        {
            try
            {
                //var bookingQuote = new BookingQuoteInfo();
                var response = DBHelper.GetBookingByQuotationID(QuotationID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetVehicleDetailsByQuotationID(int QuotationID)
        {
            try
            {
                //var bookingQuote = new BookingQuoteInfo();
                var response = DBHelper.GetVehicleDetailsByQuotationID(QuotationID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetTrailerDetailsByQuotationID(int QuotationID)
        {
            try
            {
                //var bookingQuote = new BookingQuoteInfo();
                var response = DBHelper.GetTrailerDetailsByQuotationID(QuotationID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GenerateBookingQuote(BookingQuoteInfoModel bookingQuoteInfo)
        {
            try
            {
                DBHelper.GenerateBookingQuote(ref bookingQuoteInfo);
                GenerateQuotationToPDF(bookingQuoteInfo);
                return Json("Record saved successfully!!");
            }
            catch (System.Exception ex)
            {
                return Json(ex);
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult GenerateBooking(BookingQuoteInfoModel bookingQuoteInfo)
        {
            try
            {
                DBHelper.GenerateBooking(ref bookingQuoteInfo);
                GenerateInvoiceToPDF(bookingQuoteInfo);
                return Json("Record saved successfully!!");
            }
            catch (System.Exception ex)
            {
                return Json(ex);
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult GetBusTypeDetails(int BusTypeID)
        {
            try
            {
                var response = DBHelper.GetBusTypeDetails(BusTypeID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetCompanyDetailsBasedOnID(int companyID)
        {
            try
            {
                var response = DBHelper.GetCompanyDetailsBasedOnID(companyID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public JsonResult GetTrailerTypeDetails(int TrailerTypeID)
        {
            try
            {
                var response = DBHelper.GetTrailerTypeDetails(TrailerTypeID);

                return response == null ? Json("NoRecordsFound") : Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ExportQuotationToPDF()
        {
            return View();
        }

        public void GenerateQuotationToPDF(BookingQuoteInfoModel bookingQuoteInfo)
        {
            var count = 0;
            var temp = "";
            var tempVehicles = bookingQuoteInfo.BookingVehicleInfo;
            bookingQuoteInfo.BookingVehicleInfo = new List<BookingVehicleInfoModel>();
            var vehicles = tempVehicles.AsEnumerable()
                           .Select(row => row)
                           .OrderBy(x => x.BusType)
                           .ToList();

            for (var i = 0; i < vehicles.Count; i++)
            {
                if (i != 0)
                {
                    if (temp == vehicles[i].BusType)
                    { count = count + 1; }
                    else
                    {
                        bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = vehicles[i].BusType, Quantity = count });
                        temp = vehicles[i].BusType;
                        count = 1;
                    }
                }
                else {
                    temp = vehicles[i].BusType;
                    count = count + 1;
                }
            }
            bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = temp, Quantity = count });

            if (bookingQuoteInfo.IsTrailerRequired)
            {
                count = 0;
                temp = "";
                var tempTrailers = bookingQuoteInfo.BookingTrailerInfo;
                bookingQuoteInfo.BookingTrailerInfo = new List<BookingTrailerInfoModel>();
                var trailers = tempTrailers.AsEnumerable()
                              .Select(row => row)
                              .OrderBy(x => x.TrailerType)
                              .ToList();
                for (var i = 0; i < trailers.Count; i++)
                {
                    if (i != 0)
                    {
                        if (temp == trailers[i].TrailerType)
                        { count = count + 1; }
                        else
                        {
                            bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = trailers[i].TrailerType, Quantity = count });
                            temp = trailers[i].TrailerType;
                            count = 1;
                        }
                    }
                    else {
                        temp = trailers[i].TrailerType;
                        count = count + 1;
                    }
                }
                bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = temp, Quantity = count });
            }

            if (bookingQuoteInfo.CompTelephoneExtension != String.Empty)
            {
                bookingQuoteInfo.CompTelephoneNumber = string.Empty;
                bookingQuoteInfo.CompTelephoneNumber = bookingQuoteInfo.CompTelephoneNumber + bookingQuoteInfo.CompTelephoneExtension;
            }
            var extension = ".pdf";
            var fileName = ConfigurationSettings.Company.Company1+"_Quotation_" + bookingQuoteInfo.AlternateID.ToString() + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + extension;

            var actionResult = new Rotativa.ViewAsPdf("ExportQuotationToPDF", bookingQuoteInfo);
            var byteArray = actionResult.BuildPdf(ControllerContext);
            var fileStream = new FileStream(Server.MapPath("~/PDF/" + fileName), FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            bookingQuoteInfo.QuotationFileName = fileName;
            CompanyConfigurationInfo settings = ConfigurationSettings;
            new Task(() =>
            {
                DBHelper.UpdateQuotationFileName(fileName, bookingQuoteInfo.ID, settings.ConnectionKey);
            }).Start();

            if (bookingQuoteInfo.EmailAddress != null && bookingQuoteInfo.EmailAddress != string.Empty)
            {
                new Task(() =>
                {
                    EmailHelper.SendQuotationConfirmation(bookingQuoteInfo, settings);
                }).Start();
            }
        }

        public void GenerateInvoiceToPDF(BookingQuoteInfoModel bookingQuoteInfo)
        {
            var count = 0;
            var temp = "";
            var tempVehicles = bookingQuoteInfo.BookingVehicleInfo;
            bookingQuoteInfo.BookingVehicleInfo = new List<BookingVehicleInfoModel>();
            var vehicles = tempVehicles.AsEnumerable()
                           .Select(row => row)
                           .OrderBy(x => x.BusType)
                           .ToList();
            for (var i = 0; i < vehicles.Count; i++)
            {
                if (i != 0)
                {
                    if (temp == vehicles[i].BusType)
                    { count = count + 1; }
                    else
                    {
                        bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = vehicles[i].BusType, Quantity = count });
                        temp = vehicles[i].BusType;
                        count = 1;
                    }
                }
                else {
                    temp = vehicles[i].BusType;
                    count = count + 1;
                }
            }
            bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = temp, Quantity = count });

            if (bookingQuoteInfo.IsTrailerRequired)
            {
                count = 0;
                temp = "";
                var tempTrailers = bookingQuoteInfo.BookingTrailerInfo;
                bookingQuoteInfo.BookingTrailerInfo = new List<BookingTrailerInfoModel>();
                var trailers = tempTrailers.AsEnumerable()
                              .Select(row => row)
                              .OrderBy(x => x.TrailerType)
                              .ToList();
                for (var i = 0; i < trailers.Count; i++)
                {
                    if (i != 0)
                    {
                        if (temp == trailers[i].TrailerType)
                        { count = count + 1; }
                        else
                        {
                            bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = trailers[i].TrailerType, Quantity = count });
                            temp = trailers[i].TrailerType;
                            count = 1;
                        }
                    }
                    else {
                        temp = trailers[i].TrailerType;
                        count = count + 1;
                    }
                }
                bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = temp, Quantity = count });
            }

            if (bookingQuoteInfo.CompTelephoneExtension != String.Empty)
            {
                bookingQuoteInfo.CompTelephoneNumber = string.Empty;
                bookingQuoteInfo.CompTelephoneNumber = bookingQuoteInfo.CompTelephoneNumber + bookingQuoteInfo.CompTelephoneExtension;
            }
            var extension = ".pdf";
            var fileName = ConfigurationSettings.Company.Company1 + "_Invoice_" + bookingQuoteInfo.BookingInfo.AlternateID.ToString() + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + extension;

            var actionResult = new Rotativa.ViewAsPdf("ExportInvoiceToPDF", bookingQuoteInfo);
            var byteArray = actionResult.BuildPdf(ControllerContext);
            var fileStream = new FileStream(Server.MapPath("~/PDF/" + fileName), FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            bookingQuoteInfo.BookingInfo.InvoiceFileName = fileName;
            CompanyConfigurationInfo settings = ConfigurationSettings;

            new Task(() =>
            {
                DBHelper.UpdateInvoiceFileName(fileName, bookingQuoteInfo.BookingInfo.ID, settings.ConnectionKey);
            }).Start();

            if (bookingQuoteInfo.EmailAddress != null && bookingQuoteInfo.EmailAddress != string.Empty)
            {
                new Task(() =>
                {
                    EmailHelper.SendInvoiceConfirmation(bookingQuoteInfo, settings);
                }).Start();
            }
        }

    }
}