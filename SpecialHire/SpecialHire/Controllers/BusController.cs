using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class BusController : BaseController
    {
        // GET: Bus
        public ActionResult Index()
        {
            if (commonHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserName = commonHelper.GetLoggedInUserName();
            return View(BindIndexView());
        }

        [HttpPost]
        public ActionResult Index(BusModel bus)
        {
            var response = true;
            try
            {
                response = DBHelper.SaveBusInformation(bus);
                if (response)
                { TempData["GlobalMessage"] = commonHelper.SetMessage("Record Saved Successfully!!", "S"); }
                else
                { TempData["GlobalMessage"] = commonHelper.SetMessage("Record Already Exist!!", "E"); }
            }
            catch (System.Exception)
            {
                throw;
            }

            return View(BindIndexView());
        }

        [HttpPost]
        public JsonResult SearchBusses(string FleetNumber, string VINNumber, int BusTypeID)
        {
            try
            {
                var response = DBHelper.SearchBusses(FleetNumber, VINNumber, BusTypeID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public BusModel BindIndexView()
        {
            BusModel busModal = new BusModel();
            try
            {
                busModal.BusTypes = DBHelper.GetBusTypes();
            }
            catch (System.Exception)
            {
                throw;
            }
            return busModal;
        }

        [HttpPost]
        public JsonResult checkDependency(int BusID)
        {
            try
            {
                var response = DBHelper.checkBusDependency(BusID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}