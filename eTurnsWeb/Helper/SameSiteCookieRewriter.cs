using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTurnsWeb.Helper
{
    public static class SameSiteCookieRewriter
    {
        public static void FilterSameSiteNone(object sender)
        {
            HttpApplication application = sender as HttpApplication;
            if (application != null)
            {
                application.Response.AddOnSendingHeaders(context =>
                {
                    var cookies = context.Response.Cookies;
                    for (var i = 0; i < cookies.Count; i++)
                    {
                        var cookie = cookies[i];
                        if (cookie.SameSite == SameSiteMode.None)
                        {
                            cookie.SameSite = (SameSiteMode)(-1); // Unspecified
                        }
                    }
                });
            }
        }
    }
}