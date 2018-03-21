using EbusDataProvider;
using SpecialHire.Models;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class DispatchController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.UserName = commonHelper.GetLoggedInUserName();
            DispatcherVehicleInfoModel dispatcherVehicleInfoModel = new DispatcherVehicleInfoModel();
            try
            {
                dispatcherVehicleInfoModel.Times = commonHelper.GetTime();
            }
            catch (System.Exception)
            {
                throw;
            }
            return View(dispatcherVehicleInfoModel);
        }

        [HttpPost]
        public JsonResult SearchBookings(string BookingDate, string BookingID)
        {
            try
            {
                var response = DBHelper.SearchBookings(BookingDate, BookingID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult GetDispatcherVehicleInfo(int QuotationID, int BookingVehicleID)
        {
            try
            {
                var response = DBHelper.GetDispatcherVehicleInfo(QuotationID, BookingVehicleID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult SaveAssignedBusAndTrailer(DispatcherVehicleInfoModel dispatcherVehicleInfo)
        {
            try
            {
                DBHelper.SaveAssignedBusAndTrailer(dispatcherVehicleInfo);
                return Json("Record saved successfully!!");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult SaveDispatcherInformation(DispatcherVehicleInfoModel dispatcherVehicleInfo)
        {
            try
            {
                DBHelper.SaveDispatcherInformation(dispatcherVehicleInfo);
                return Json("Record saved successfully!!");
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult DownloadDriverWorkSheet(int QuotationID, int BookingVehicleID)
        {
            try
            {
                var driverWorkSheetModel = DBHelper.GetVehicleDriverSheetInfo(QuotationID,BookingVehicleID);
                return new Rotativa.ViewAsPdf("ExportDriverSheetToPDF", driverWorkSheetModel);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public ActionResult ShowRouteMap(string FromLocation, string ToLocation)
        {
            try
            {
                return View();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}