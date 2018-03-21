using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class ApprovalController : BaseController
    {
        // GET: Approval
        public ActionResult Index()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public JsonResult SearchPendingBookings(string BookingDate, string BookingID)
        {
            try
            {
                var response = DBHelper.SearchPendingBookings(BookingDate, BookingID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateBookingStatus(int BookingID, string Comment)
        {
            try
            {
                var response = DBHelper.UpdateBookingStatus(BookingID, Comment);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}