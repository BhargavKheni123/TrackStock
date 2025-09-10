using System;
using System.Web.Mvc;
using Sustainsys.Saml2.HttpModule;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Collections.Generic;

namespace Sustainsys.Saml2.Mvc
{

    
    /// <summary>
    /// Mvc Controller that provides the authentication functionality.
    /// </summary>
    [AllowAnonymous]
    public class Saml2Controller : Controller
    {
        

        private static IOptions options = null;

        /// <summary>
        /// The options used by the controller. By default read from config, 
        /// but can be set.
        /// </summary>
        public static IOptions Options {
            get
            {
                if(options == null)
                {
                    options = Configuration.Options.FromConfiguration;
                }
                return options;
            }
            set
            {
                options = value;
            }
        }

        /// <summary>
        /// SignIn action that sends the AuthnRequest to the Idp.
        /// </summary>
        /// <returns>Redirect with sign in request</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HandledResult")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CommandResult")]
        public ActionResult SignIn()
        {
            var result = CommandFactory.GetCommand(CommandFactory.SignInCommandName).Run(
                Request.ToHttpRequestData(),
                Options);

            if(result.HandledResult)
            {
                throw new NotSupportedException("The MVC controller doesn't support setting CommandResult.HandledResult.");
            }

            result.ApplyCookies(Response, Options.Notifications.EmitSameSiteNone(Request.UserAgent));
            return result.ToActionResult();
        }

        /// <summary>
        /// Assertion consumer Url that accepts the incoming Saml response.
        /// </summary>
        /// <returns>Redirect to start page on success.</returns>
        /// <remarks>The action effectively accepts the SAMLResponse, but
        /// due to using common infrastructure it is read for the current
        /// http request.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HandledResult")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CommandResult")]
        public ActionResult Acs()
        {
            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName).Run(
                Request.ToHttpRequestData(),
                Options);

            if(result.HandledResult)
            {
                throw new NotSupportedException("The MVC controller doesn't support setting CommandResult.HandledResult.");
            }

            ClaimsPrincipal claimsPrincipal = result.Principal;
            
            //if(this.OnLogInSuccessEvent != null)
            {
                SamlLogInSuccessEventArgs args = new SamlLogInSuccessEventArgs();
                args.SAMLClaimsPrincipal = claimsPrincipal;
                //Session["SamlLogInSuccessEventArgs"] = args;
                TempData["SamlLogInSuccessEventArgs"] = args;
                TempData["SamlLogInSuccessCustomEventArgs"] = claimsPrincipal;
            }

            //result.SignInOrOutSessionAuthenticationModule(); // commented by amit t. Dont create user in Identity

            result.ApplyCookies(Response, Options.Notifications.EmitSameSiteNone(Request.UserAgent));
            return result.ToActionResult();
        }

        /// <summary>
        /// Metadata of the service provider.
        /// </summary>
        /// <returns>ActionResult with Metadata</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HandledResult")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CommandResult")]
        public ActionResult Index()
        {
            var result = CommandFactory.GetCommand(CommandFactory.MetadataCommand).Run(
                Request.ToHttpRequestData(),
                Options);

            result.ApplyHeaders(Response);

            if (result.HandledResult)
            {
                throw new NotSupportedException("The MVC controller doesn't support setting CommandResult.HandledResult.");
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Logout locally and if Idp supports it, perform a federated logout
        /// </summary>
        /// <returns>ActionResult</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HandledResult")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CommandResult")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
        public ActionResult Logout()
        {
            var result = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(Request.ToHttpRequestData(), Options);

            if (result.HandledResult)
            {
                throw new NotSupportedException("The MVC controller doesn't support setting CommandResult.HandledResult.");
            }

            result.SignInOrOutSessionAuthenticationModule();
            result.ApplyCookies(Response, Options.Notifications.EmitSameSiteNone(Request.UserAgent));
            return result.ToActionResult();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// Added by amit
    public class SamlLogInSuccessEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ClaimKeyVal { get; set; }

        ClaimsPrincipal _SAMLClaimsPrincipal;
        /// <summary>
        /// 
        /// </summary>
        public ClaimsPrincipal SAMLClaimsPrincipal
        {
            get
            {
                return _SAMLClaimsPrincipal;
            }
            set
            {
                _SAMLClaimsPrincipal = value;
                                

                ClaimKeyVal = new Dictionary<string, string>();

                if (_SAMLClaimsPrincipal != null)
                {
                  
                    foreach (var cl in _SAMLClaimsPrincipal.Claims)
                    {
                        var val = cl.Value;
                        var typ = cl.Type;
                        var valType = cl.ValueType;
                        ClaimKeyVal.Add(typ, val);
                    }
                }
                else
                {
                    ClaimKeyVal = null;
                }
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserEmail {
            get
            {
               if(ClaimKeyVal != null)
                {
                    return ClaimKeyVal["SttlUserEmail"]; // configure in saml app in okta in Attribute Statement in Basic format
                }
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserLoginName
        {
            get
            {
                if (ClaimKeyVal != null)
                {
                    return ClaimKeyVal["SttlUserLogin"];
                }
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserFirstName
        {
            get
            {
                if (ClaimKeyVal != null)
                {
                    return ClaimKeyVal["SttlUserFirstName"];
                }
                return "";
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public string UserLastName
        {
            get
            {
                if (ClaimKeyVal != null)
                {
                    return ClaimKeyVal["SttlUserLastName"];
                }
                return "";
            }
        }
    }




}
