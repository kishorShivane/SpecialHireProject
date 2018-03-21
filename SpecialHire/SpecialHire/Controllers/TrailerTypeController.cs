using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class TrailerTypeController : BaseController
    {
        // GET: TrailerType
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
        public ActionResult Index(TrailerTypeModel trailerTypeModel)
        {
            var response = true;
            try
            {
                response = DBHelper.SaveTrailerTypeInformation(trailerTypeModel);
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
        public JsonResult SearchTrailerTypes(string TrailerType)
        {
            try
            {
                var response = DBHelper.SearchTrailerTypes(TrailerType);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult CheckDependency(int TrailerTypeID)
        {
            try
            {
                var response = DBHelper.CheckTrailerTypeDependency(TrailerTypeID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}