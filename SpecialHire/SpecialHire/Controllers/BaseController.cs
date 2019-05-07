using EbusDataProvider;
using SpecialHire.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            DBHelper = new DBHelper();
            commonHelper = new CommonHelper();
            EmailHelper = new EmailHelper();
        }

        public DBHelper DBHelper { get; set; }
        public CommonHelper commonHelper { get; set; }
        public EmailHelper EmailHelper { get; set; }
        public Models.CompanyConfigurationInfo ConfigurationSettings { get{
            return (HttpContext.Session["CONFIGURATION"] != null)?(Models.CompanyConfigurationInfo)(Models.CompanyConfigurationInfo)HttpContext.Session["CONFIGURATION"]:new Models.CompanyConfigurationInfo();}
            //return DBHelper.LoadConfigurationSettings(applicationUser.CompanyID.Value);
            }
    }
}