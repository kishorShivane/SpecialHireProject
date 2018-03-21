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
        public CompanyConfigurationInfo ConfigurationSettings { get{
            return (HttpContext.Session["CONFIGURATION"] != null)?(CompanyConfigurationInfo)(CompanyConfigurationInfo)HttpContext.Session["CONFIGURATION"]:new CompanyConfigurationInfo();}
        }
    }
}