using EbusDataProvider;
using SpecialHire.Utilities;
using System;
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
                    CompanyConfigurationInfo configurationSettings = DBHelper.LoadConfigurationSettings(applicationUser.CompanyID.Value);
                    Models.CompanyConfigurationInfo config = new Models.CompanyConfigurationInfo()
                    {
                        ID = configurationSettings.ID,
                        CompanyID = configurationSettings.CompanyID,
                        CompanyName = configurationSettings.Company.Company1,
                        ConnectionKey = configurationSettings.ConnectionKey,
                        DefaultCompanyDetails = configurationSettings.DefaultCompanyDetails,
                        DefaultEventDescription = configurationSettings.DefaultEventDescription,
                        DefaultPaymentMode = configurationSettings.DefaultPaymentMode,
                        DefaultPaymentTerms = configurationSettings.DefaultPaymentTerms,
                        DistanceCalculation = configurationSettings.DistanceCalculation,
                        EmailPassword = configurationSettings.EmailPassword,
                        EmailPort = configurationSettings.EmailPort,
                        EmailSMTP = configurationSettings.EmailSMTP,
                        EmailUserName = configurationSettings.EmailUserName,
                        EnforcePassengerCount = configurationSettings.EnforcePassengerCount,
                        InvoiceEmailTemplate = configurationSettings.InvoiceEmailTemplate,
                           InvoicePDFPath = configurationSettings.InvoicePDFPath,
                           MinimumDistance = configurationSettings.MinimumDistance,
                           ModifiedBy = configurationSettings.ModifiedBy,
                           ModifiedOn = configurationSettings.ModifiedOn,
                           PDFPaymentTerms = configurationSettings.PDFPaymentTerms,
                           QuotationEmailTemplate = configurationSettings.QuotationEmailTemplate,
                           QuotationMinimumValue = configurationSettings.QuotationMinimumValue,
                           QuotationPDFPath = configurationSettings.QuotationPDFPath,
                           QuotationValidityPeriod = configurationSettings.QuotationValidityPeriod,
                           Status = configurationSettings.Status
                    };
                    HttpContext.Session["CONFIGURATION"] = config;
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