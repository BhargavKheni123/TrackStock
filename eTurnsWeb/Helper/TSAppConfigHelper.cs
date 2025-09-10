using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace eTurnsWeb.Helper
{
    public class TSAppConfigHelper
    {
        public static string AbSignupLandingPageURL
        {
            get
            {
                string _AbSignupLandingPageURL = "https://www.eturns.com/subscribetrackstock";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["AbSignupLandingPageURL"])))
                {
                    _AbSignupLandingPageURL = Convert.ToString(ConfigurationManager.AppSettings["AbSignupLandingPageURL"]);
                }
                return _AbSignupLandingPageURL;
            }
        }
    }
}