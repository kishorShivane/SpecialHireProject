using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class TrailerController : BaseController
    {
        // GET: Trailer
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
        public ActionResult Index(TrailerModel trailer)
        {
            var response = true;
            try
            {
                response = DBHelper.SaveTrailerInformation(trailer);
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
        public JsonResult SearchTrailers(string FleetNumber, string VINNumber, int TrailerTypeID)
        {
            try
            {
                var response = DBHelper.SearchTrailers(FleetNumber, VINNumber, TrailerTypeID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public TrailerModel BindIndexView()
        {
            TrailerModel trailerModel = new TrailerModel();
            try
            {
                trailerModel.TrailerTypes = DBHelper.GetTrailerTypes();
            }
            catch (System.Exception)
            {
                throw;
            }
            return trailerModel;
        }

        [HttpPost]
        public JsonResult CheckDependency(int TrailerID)
        {
            try
            {
                var response = DBHelper.CheckTrailerDependency(TrailerID);
                return Json(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}