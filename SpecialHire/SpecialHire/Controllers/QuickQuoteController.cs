using EbusDataProvider;
using Newtonsoft.Json;
using SpecialHire.Models;
using SpecialHire.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class QuickQuoteController : BaseController
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
                bookingQuoteInfo.PaymentTerms = DBHelper.GetPaymentTerms();
                bookingQuoteInfo.CompanyDetails = DBHelper.GetCompanyDetails();
                bookingQuoteInfo.FromLocations = DBHelper.GetFromLocations();
                bookingQuoteInfo.ToLocations = DBHelper.GetToLocations();
            }
            catch (System.Exception)
            {
                throw;
            }

            return View(bookingQuoteInfo);
        }

        [HttpPost]
        public JsonResult GenerateBookingQuote(BookingQuoteInfoModel bookingQuoteInfo)
        {
            try
            {
                SelectListItem generalEvent = DBHelper.GetEvents().FirstOrDefault(x => x.Text.Equals("General"));
                bookingQuoteInfo.EventID = Convert.ToInt32(generalEvent.Value);
                bookingQuoteInfo.EventDescription = generalEvent.Text;
                LoadTempVehicleAdnTrailerDetails(bookingQuoteInfo);
                DBHelper.GenerateBookingQuote(ref bookingQuoteInfo);
                string filePath = GenerateQuotationToPDF(bookingQuoteInfo);
                return Json(filePath, JsonRequestBehavior.AllowGet);
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

        private void LoadTempVehicleAdnTrailerDetails(BookingQuoteInfoModel bookingQuoteInfo)
        {
            if (bookingQuoteInfo.NumberOfBusses > 0)
            {
                List<SelectListItem> busTypes = DBHelper.GetBusTypes();
                for (int i = 0; i < bookingQuoteInfo.NumberOfBusses; i++)
                {
                    bookingQuoteInfo.BookingVehicleInfo.Add(new BookingVehicleInfoModel()
                    {
                        ID = 0,
                        BusTypeID = 1,
                        BusType = busTypes.FirstOrDefault(x => x.Value == "1").Text
                    });
                }
            }

            if (bookingQuoteInfo.NumberOfTrailers > 0)
            {
                bookingQuoteInfo.IsTrailerRequired = true;
                List<SelectListItem> trailerTypes = DBHelper.GetTrailerTypes();
                for (int i = 0; i < bookingQuoteInfo.NumberOfTrailers; i++)
                {
                    bookingQuoteInfo.BookingTrailerInfo.Add(new BookingTrailerInfoModel()
                    {
                        ID = 0,
                        TrailerTypeID = 1,
                        TrailerType = trailerTypes.FirstOrDefault(x => x.Value == "1").Text
                    });
                }
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


        public ActionResult ExportQuotationToPDF()
        {
            return View();
        }

        public string GenerateQuotationToPDF(BookingQuoteInfoModel bookingQuoteInfo)
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
                byte[] byteArray = actionResult.BuildPdf(ControllerContext);
                FileStream fileStream = new FileStream(Server.MapPath("~/PDF/" + fileName), FileMode.Create, FileAccess.Write);
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

            try
            {
                if (bookingQuoteInfo.EmailAddress != null && bookingQuoteInfo.EmailAddress != string.Empty)
                {
                    new Task(() =>
                    {
                        EmailHelper.SendQuotationConfirmation(bookingQuoteInfo, settings);
                    }).Start();
                }
            }
            catch (Exception)
            {
                //ignore
            }

            UriBuilder urlBuilder =
                                    new System.UriBuilder(Request.Url.AbsoluteUri)
                                    {
                                        Path = Url.Content("~/PDF/" + fileName),
                                        Query = null,
                                    };
            string url = urlBuilder.ToString();

            return url;
        }
    }
}