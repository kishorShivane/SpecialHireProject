using EbusDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class DriverController : BaseController
    {
        // GET: Driver
        public ActionResult Index()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserName = commonHelper.GetLoggedInUserName();
            return View();
        }

        [HttpPost]
        public ActionResult Index(Driver driver)
        {
            var response = true;
            try
            {
                response = DBHelper.SaveDriverInformation(driver);
                if (response)
                { TempData["GlobalMessage"] = commonHelper.SetMessage("Record Saved Successfully!!", "S"); }
                else
                { TempData["GlobalMessage"] = commonHelper.SetMessage("Record Already Exist!!", "E"); }
            }
            catch (System.Exception)
            {
                throw;
            }

            return View();
        }

        [HttpPost]
        public JsonResult SearchDrivers(string DriverName, string DriverNumber, string Depot)
        {
            try
            {
                var response = DBHelper.SearchDrivers(DriverName, DriverNumber, Depot);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult CheckDependency(int DriverID)
        {
            try
            {
                var response = DBHelper.CheckDriverDependency(DriverID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}