using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using eTurns.AuthorizeApi.Providers;
using eTurns.AuthorizeApi.Models;
using eTurns.AuthorizeApi.Formats;
using System.Configuration;

namespace eTurns.AuthorizeApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            string URL = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AuthorizationServerURL"]);
            bool AllowInsecureHttp = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowInsecureHttp"]);
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            double expDays = 10;
            expDays = Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiredDays"].ToString());
            OAuthOptions = new OAuthAuthorizationServerOptions
            {

                TokenEndpointPath = new PathString("/oauth2/token"),
                Provider = new CustomOAuthProvider(),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(expDays),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = AllowInsecureHttp,
                AccessTokenFormat = new CustomJwtFormat(URL)
               
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
    }
}
