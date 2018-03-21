using EbusDataProvider;
using SpecialHire.Models;
using SpecialHire.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ApplicationUser applicationUser)
        {
            try
            {
                applicationUser = DBHelper.ValidateLogin(applicationUser);
                if (applicationUser != null)
                {
                    HttpContext.Session["USER"] = applicationUser;
                    var configurationSettings = DBHelper.LoadConfigurationSettings(applicationUser.CompanyID.Value);
                    HttpContext.Session["CONFIGURATION"] = configurationSettings;
                    if (applicationUser.UserRoleID == Convert.ToInt32(UserRoll.Dispatcher))
                    { return RedirectToAction("Index", "Dispatch"); }
                    else
                    { return RedirectToAction("Index", "BookingQuote"); }
                }
                else
                {
                    TempData["GlobalMessage"] = commonHelper.SetMessage("User Name or Password is Invalid!!", "E");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session["USER"] = null;
            HttpContext.Session["CONFIGURATION"] = null;
            TempData["GlobalMessage"] = commonHelper.SetMessage("Logged Out Successfully..!!", "S");
            return RedirectToAction("Index", "Home");
        }
    }
}