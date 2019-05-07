using EbusDataProvider;
using Newtonsoft.Json;
using SpecialHire.Models;
using SpecialHire.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
                List<BookingQuoteinfo> response = DBHelper.SearchBookingQuotes(QuotationID, PhoneNumber, Name);
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
                BookingQuoteinfo response = DBHelper.GetBookingQuoteByID(QuotationID);

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
                Bookinginfo response = DBHelper.GetBookingByQuotationID(QuotationID);

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
                List<BookingVehicleInfoModel> response = DBHelper.GetVehicleDetailsByQuotationID(QuotationID);

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
                List<BookingTrailerInfoModel> response = DBHelper.GetTrailerDetailsByQuotationID(QuotationID);

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
                return Json("Record saved successfully!!", JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(JsonConvert.SerializeObject(ex,
                       new JsonSerializerSettings
                       {
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                       }));
            }
            //return Json("Failed");
        }

        private BookingQuoteInfoModel MapBookingLiteToModel(BookingQuoteInfoBase bookingQuote)
        {
            return new BookingQuoteInfoModel()
            {
                ID = bookingQuote.ID,
                AlternateID = bookingQuote.AlternateID,
                Title = bookingQuote.Title,
                FirstName = bookingQuote.FirstName,
                SurName = bookingQuote.SurName,
                TelephoneNumber = bookingQuote.TelephoneNumber,
                CellNumber = bookingQuote.CellNumber,
                EmailAddress = bookingQuote.EmailAddress,
                CompanyName = bookingQuote.CompanyName,
                Address = bookingQuote.Address,
                PostalCode = bookingQuote.PostalCode,
                CompTelephoneNumber = bookingQuote.CompTelephoneNumber,
                CompTelephoneExtension = bookingQuote.CompTelephoneExtension,
                FaxNumber = bookingQuote.FaxNumber,
                IsReturnJourney = bookingQuote.IsReturnJourney,
                IsSingleJourney = bookingQuote.IsSingleJourney,
                IsTrailerRequired = bookingQuote.IsTrailerRequired,
                PickUpDate = bookingQuote.PickUpDate,
                PickUpTime = bookingQuote.PickUpTime,
                ReturnDate = bookingQuote.ReturnDate,
                ReturnTime = bookingQuote.ReturnTime,
                FromLocation = bookingQuote.FromLocation,
                ToLocation = bookingQuote.ToLocation,
                Distance = bookingQuote.Distance,
                Passengers = bookingQuote.Passengers,
                EventID = bookingQuote.EventID,
                EventDescription = bookingQuote.EventDescription,
                ExtraInformation = bookingQuote.ExtraInformation,
                PaymentTermsID = bookingQuote.PaymentTermsID,
                PaymentTerm = bookingQuote.PaymentTerm,
                IsQuoteValidTillAdded = bookingQuote.IsQuoteValidTillAdded,
                QuoteValidTill = bookingQuote.QuoteValidTill,
                QuotationValue = bookingQuote.QuotationValue,
                CompanyID = bookingQuote.CompanyID,
                CompanyLogo = bookingQuote.CompanyLogo,
                CompanyPrefix = bookingQuote.CompanyPrefix,
                CompanyNameAddress = bookingQuote.CompanyNameAddress,
                QuotationFileName = bookingQuote.QuotationFileName,
                PDFPaymentTerms = bookingQuote.PDFPaymentTerms,
                Status = bookingQuote.Status,
                ModifiedBy = bookingQuote.ModifiedBy,
                ModifiedOn = bookingQuote.ModifiedOn,
                BookingVehicleInfo = bookingQuote.BookingVehicleInfo,
                BookingTrailerInfo = bookingQuote.BookingTrailerInfo
            };
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
                return Json(JsonConvert.SerializeObject(ex,
                      new JsonSerializerSettings
                      {
                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                      }));
            }
            //return Json("Failed");
        }

        [HttpPost]
        public JsonResult GetBusTypeDetails(int BusTypeID)
        {
            try
            {
                BusType response = DBHelper.GetBusTypeDetails(BusTypeID);

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
                CompanyDetail response = DBHelper.GetCompanyDetailsBasedOnID(companyID);

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
                TrailerType response = DBHelper.GetTrailerTypeDetails(TrailerTypeID);

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

            int count = 0;
            string temp = "";
            List<BookingVehicleInfoModel> tempVehicles = bookingQuoteInfo.BookingVehicleInfo;
            bookingQuoteInfo.BookingVehicleInfo = new List<BookingVehicleInfoModel>();
            List<BookingVehicleInfoModel> vehicles = tempVehicles.AsEnumerable()
                           .Select(row => row)
                           .OrderBy(x => x.BusType)
                           .ToList();

            for (int i = 0; i < vehicles.Count; i++)
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
                else
                {
                    temp = vehicles[i].BusType;
                    count = count + 1;
                }
            }
            bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = temp, Quantity = count });

            if (bookingQuoteInfo.IsTrailerRequired)
            {
                count = 0;
                temp = "";
                List<BookingTrailerInfoModel> tempTrailers = bookingQuoteInfo.BookingTrailerInfo;
                bookingQuoteInfo.BookingTrailerInfo = new List<BookingTrailerInfoModel>();
                List<BookingTrailerInfoModel> trailers = tempTrailers.AsEnumerable()
                              .Select(row => row)
                              .OrderBy(x => x.TrailerType)
                              .ToList();
                for (int i = 0; i < trailers.Count; i++)
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
                    else
                    {
                        temp = trailers[i].TrailerType;
                        count = count + 1;
                    }
                }
                bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = temp, Quantity = count });
            }

            if (bookingQuoteInfo.CompTelephoneExtension != string.Empty)
            {
                bookingQuoteInfo.CompTelephoneNumber = string.Empty;
                bookingQuoteInfo.CompTelephoneNumber = bookingQuoteInfo.CompTelephoneNumber + bookingQuoteInfo.CompTelephoneExtension;
            }
            string extension = ".pdf";
            string fileName = ConfigurationSettings.CompanyName + "_Quotation_" + bookingQuoteInfo.AlternateID.ToString() + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + extension;
            try
            {
                Rotativa.ViewAsPdf actionResult = new Rotativa.ViewAsPdf("ExportQuotationToPDF", bookingQuoteInfo);
                byte[] byteArray = actionResult.BuildPdf(this.ControllerContext);
                FileStream fileStream = new FileStream(Server.MapPath("~/PDF/" + fileName), FileMode.Create,FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

            bookingQuoteInfo.QuotationFileName = fileName;
            Models.CompanyConfigurationInfo settings = ConfigurationSettings;
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
            int count = 0;
            string temp = "";
            List<BookingVehicleInfoModel> tempVehicles = bookingQuoteInfo.BookingVehicleInfo;
            bookingQuoteInfo.BookingVehicleInfo = new List<BookingVehicleInfoModel>();
            List<BookingVehicleInfoModel> vehicles = tempVehicles.AsEnumerable()
                           .Select(row => row)
                           .OrderBy(x => x.BusType)
                           .ToList();
            for (int i = 0; i < vehicles.Count; i++)
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
                else
                {
                    temp = vehicles[i].BusType;
                    count = count + 1;
                }
            }
            bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel() { BusType = temp, Quantity = count });

            if (bookingQuoteInfo.IsTrailerRequired)
            {
                count = 0;
                temp = "";
                List<BookingTrailerInfoModel> tempTrailers = bookingQuoteInfo.BookingTrailerInfo;
                bookingQuoteInfo.BookingTrailerInfo = new List<BookingTrailerInfoModel>();
                List<BookingTrailerInfoModel> trailers = tempTrailers.AsEnumerable()
                              .Select(row => row)
                              .OrderBy(x => x.TrailerType)
                              .ToList();
                for (int i = 0; i < trailers.Count; i++)
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
                    else
                    {
                        temp = trailers[i].TrailerType;
                        count = count + 1;
                    }
                }
                bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel() { TrailerType = temp, Quantity = count });
            }

            if (bookingQuoteInfo.CompTelephoneExtension != string.Empty)
            {
                bookingQuoteInfo.CompTelephoneNumber = string.Empty;
                bookingQuoteInfo.CompTelephoneNumber = bookingQuoteInfo.CompTelephoneNumber + bookingQuoteInfo.CompTelephoneExtension;
            }
            string extension = ".pdf";
            string fileName = ConfigurationSettings.CompanyName + "_Invoice_" + bookingQuoteInfo.BookingInfo.AlternateID.ToString() + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + extension;

            Rotativa.ViewAsPdf actionResult = new Rotativa.ViewAsPdf("ExportInvoiceToPDF", bookingQuoteInfo);
            byte[] byteArray = actionResult.BuildPdf(ControllerContext);
            FileStream fileStream = new FileStream(Server.MapPath("~/PDF/" + fileName), FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            bookingQuoteInfo.BookingInfo.InvoiceFileName = fileName;
            Models.CompanyConfigurationInfo settings = ConfigurationSettings;

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