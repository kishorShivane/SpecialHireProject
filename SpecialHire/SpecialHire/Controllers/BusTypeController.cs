using EbusDataProvider;
using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class BusTypeController : BaseController
    {
        // GET: BusType
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
        public ActionResult Index(BusTypeModel busTypeModal)
        {
            var response = true;
            try
            {
                response = DBHelper.SaveBusTypeInformation(busTypeModal);
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
        public JsonResult SearchBusTypes(string BusType)
        {
            try
            {
                var response = DBHelper.SearchBusTypes(BusType);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult CheckDependency(int BusTypeID)
        {
            try
            {
                var response = DBHelper.CheckBusTypeDependency(BusTypeID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}