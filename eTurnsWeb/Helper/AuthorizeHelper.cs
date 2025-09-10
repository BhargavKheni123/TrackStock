using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eTurns.DTO;
using System.Web.Routing;

namespace eTurnsWeb.Helper
{


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeHelper : FilterAttribute, IAuthorizationFilter
    {
        string skipActionName = "eturnsnewsblogs,userloginnews,userlogin,itemlocations,locationdetails,itemlocationsdelete,getcustomeraddessbyid,loadsupplierofitem,loadlocationsofitem,checkbinexists,getbinsofitembyorderid,getcompanylanguage,getalllocationofroom,getbinsofitembytransferid,getlocationsinitemmaster,previoususerloggedin,getutcstartandenddate,sendmailfortransferapproved,abgetapp";//dashboard remove for WI-3929
        List<string> lstOnlyIfRoomAvailable = GetOnlyIfRoomAvailableActionMethodList();



        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            string DefaultRedirectURL = "~/Inventory/ItemMasterList";

            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            string ActionName = filterContext.ActionDescriptor.ActionName;
            string ControllerName = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();



            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml")); 
            string _strApplyNewAuthorization = "no";
            //if (Settinfile.Element("ApplyNewAuthorization") != null)  
            //{
            //    _strApplyNewAuthorization = Convert.ToString(Settinfile.Element("ApplyNewAuthorization").Value);
            //}

            if (eTurns.DTO.SiteSettingHelper.ApplyNewAuthorization != string.Empty) 
            {
                _strApplyNewAuthorization = eTurns.DTO.SiteSettingHelper.ApplyNewAuthorization;
            }
                       


            if (true)
            {

                // OldAuthorization:
                #region [OLD AUTHORIZATION]

                // only redirect for GET requests, 
                // otherwise the browser might not propagate the verb and request body correctly.
                if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    //return;
                    goto NewAuthorization;
                }


                var currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection;
                //currentConnectionSecured = webHelper.IsCurrentConnectionSecured();

                //var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();


                if (SessionHelper.RoomID <= 0 || SessionHelper.CompanyID <= 0 || SessionHelper.EnterPriceID <= 0)
                {
                    if (lstOnlyIfRoomAvailable != null && lstOnlyIfRoomAvailable.Count > 0)
                    {
                        if (lstOnlyIfRoomAvailable.Any(x => x.ToLower() == ActionName.ToLower()))
                        {
                            filterContext.Result = new RedirectResult("~/Master/MyProfile");
                            return;
                        }
                    }
                }


                if (ControllerName.ToLower() == "bom")
                {
                    if (eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster))
                    {
                        return;
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                            filterContext.Result = new RedirectResult(DefaultRedirectURL);
                            return;
                        }

                    }
                }

                SessionHelper.ModuleList objModuleName = SessionHelper.GetModuleIDfromPageName(ActionName);

                if (SessionHelper.UserID > 0 && ActionName.ToLower() == "logoutuser")
                {
                    return;

                }
                if (SessionHelper.UserID <= 0 && ActionName.ToLower() != "eturnsnewsblogs" && ActionName.ToLower() != "userloginnews" && ActionName.ToLower() != "userlogin" && ActionName.ToLower() != "forgotpassword" && ActionName.ToLower() != "updatepassword" && ActionName.ToLower() != "resetpassword")
                {
                    filterContext.Result = new RedirectResult("~/Master/UserLogin");
                    return;
                }

                if (SessionHelper.UserID > 0 && (!SessionHelper.IsLicenceAccepted || !SessionHelper.NewEulaAccept || !SessionHelper.AnotherLicenceAccepted) && ActionName.ToLower() != "termsandcondition" && ActionName.ToLower() != "changepassword" && ActionName.ToLower() != "logoutuser" && !filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectResult("~/Master/TermsAndCondition");
                    return;
                }

                if (SessionHelper.UserID > 0 && !SessionHelper.HasPasswordChanged && ActionName.ToLower() != "termsandcondition" && ActionName.ToLower() != "changepassword" && ActionName.ToLower() != "logoutuser" && !filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectResult("~/Master/ChangePassword");
                    return;
                }

                if (SessionHelper.UserID > 0 && ActionName.ToLower() == "dashboard")
                {
                    if (eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewDashboard))
                    {
                        DefaultRedirectURL = "~/Master/Dashboard";
                    }
                    else
                    {
                        DefaultRedirectURL = "~/Master/MyProfile";
                    }
                }

                if (SessionHelper.UserID > 0 && ActionName.ToLower() == "itemmasterlist")
                {
                    if (eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster))
                    {
                        DefaultRedirectURL = "~/Inventory/ItemMasterList";
                    }
                    else
                    {
                        DefaultRedirectURL = "~/Master/MyProfile";
                    }
                }

                if (SessionHelper.UserID > 0 && ControllerName.ToLower() == "master" &&  ActionName.ToLower() == "InventoryAnalysis".ToLower())
                {
                    if (eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewMinMaxDashboard))
                    {
                        DefaultRedirectURL = "~/Master/InventoryAnalysis";
                    }
                    else
                    {
                        DefaultRedirectURL = "~/Master/MyProfile";
                    }
                }
                


                if (skipActionName.ToLower().Contains(ActionName.ToLower()))
                    return;
                else if (objModuleName == SessionHelper.ModuleList.None)
                    return;



                //if (SessionHelper.UserType == 1)
                //{
                //    //SessionHelper.PermissionType objPermissionType = SessionHelper.GetPermissionTypefromPageName(ActionName);

                //   // bool hasPermission = eTurnsWeb.Helper.SessionHelper.GetModulePermission(objModuleName, objPermissionType);
                //   // if (hasPermission)
                //   // {
                //        // filterContext.Result = new ViewResult { MasterName = ControllerName, ViewName = ActionName };
                //        return;
                //   // }
                //}
                //else
                //{
                //if (SessionHelper.UserID > 0 && SessionHelper.CompanyID > 0)
                //{
                SessionHelper.PermissionType objPermissionType = SessionHelper.GetPermissionTypefromPageName(ActionName);

                bool hasPermission = eTurnsWeb.Helper.SessionHelper.GetModulePermission(objModuleName, objPermissionType);
                if (hasPermission)
                {
                    // filterContext.Result = new ViewResult { MasterName = ControllerName, ViewName = ActionName };
                    return;
                }
                else
                {
                    if (objPermissionType == SessionHelper.PermissionType.Update)
                    {
                        hasPermission = eTurnsWeb.Helper.SessionHelper.GetModulePermission(objModuleName, SessionHelper.PermissionType.View);
                        if (hasPermission == true)
                        {
                            return;
                        }
                        else
                        {
                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                filterContext.Result = new JsonResult
                                {
                                    Data = new
                                    {
                                        message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                    },
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                };
                            }
                            else
                            {
                                //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                                filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                return;
                            }
                        }
                    }
                    else if (objPermissionType != SessionHelper.PermissionType.View)
                    {
                        if (objPermissionType == SessionHelper.PermissionType.Approval)
                        {
                            if (ActionName.ToLower().Contains("emailconfi"))
                            {
                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                {
                                    filterContext.Result = new JsonResult
                                    {
                                        Data = new
                                        {
                                            message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                        },
                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                    };
                                }
                                else
                                {
                                    //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                                    filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                    return;
                                }
                            }
                            else if (ActionName.ToLower().Contains("import"))
                            {
                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                {
                                    filterContext.Result = new JsonResult
                                    {
                                        Data = new
                                        {
                                            message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                        },
                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                    };
                                }
                                else
                                {
                                    //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                                    filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                    return;
                                }
                            }
                            else
                            {
                                string URL = AuthorizationRedirectURL(objModuleName);
                                if (!string.IsNullOrEmpty(URL))
                                {
                                    //filterContext.Result = new RedirectResult("~/" + ControllerName + "/" + URL, true);
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                    {
                                        action = URL,
                                        controller = ControllerName,
                                        area = ""
                                    }));
                                }
                            }
                        }
                        else
                        {

                            string URL = AuthorizationRedirectURL(objModuleName);
                            if (!string.IsNullOrEmpty(URL))
                            {
                                if (ActionName.ToLower().Contains("create"))
                                {
                                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                                    {
                                        filterContext.Result = new JsonResult
                                        {
                                            Data = new
                                            {
                                                message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                            },
                                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                        };
                                    }
                                    else
                                    {
                                        //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                                        filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                        return;
                                    }
                                }
                                else
                                {
                                    //filterContext.Result = new RedirectResult("~/" + ControllerName + "/" + URL, true);
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                    {
                                        action = URL,
                                        controller = ControllerName,
                                        area = ""
                                    }));
                                }
                            }
                        }



                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            //filterContext.Result = new RedirectResult("~/Master/Dashboard");
                            filterContext.Result = new RedirectResult(DefaultRedirectURL);
                            return;
                        }
                    }
                }
                return;
            //}
            //else
            //{
            //    if (ActionName.ToLower() != "userlogin")
            //        filterContext.Result = new RedirectResult("~/Master/UserLogin");
            //    return;
            //}
            //}
            #endregion


            NewAuthorization:
                #region [NEW AUTHORIZATION]


                if ((ActionName ?? string.Empty).ToLower().Contains("session"))
                {
                    return;
                }


                List<AlleTurnsActionMethodsDTO> eTurnsAllMethods = null;
                AlleTurnsActionMethodsDTO currentOperationDTO = null;
                if (SessionHelper.UserID > 0)
                {

                    if (SessionHelper.UserID > 0 && (!SessionHelper.IsLicenceAccepted || !SessionHelper.NewEulaAccept || !SessionHelper.AnotherLicenceAccepted) && ActionName.ToLower() != "termsandcondition" && ActionName.ToLower() != "changepassword" && ActionName.ToLower() != "logoutuser" && !filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new RedirectResult("~/Master/TermsAndCondition");
                        return;
                    }

                    if (SessionHelper.UserID > 0 && !SessionHelper.HasPasswordChanged && ActionName.ToLower() != "termsandcondition" && ActionName.ToLower() != "changepassword" && ActionName.ToLower() != "logoutuser" && !filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new RedirectResult("~/Master/ChangePassword");
                        return;
                    }


                    eTurnsAllMethods = CommonUtility.GetAlleTurnsMethods();

                    if (eTurnsAllMethods == null || eTurnsAllMethods.Count == 0)
                    {
                        CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                        if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                        {
                            //goto OldAuthorization;
                            return;
                        }
                        else
                        {
                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                filterContext.Result = new JsonResult
                                {
                                    Data = new
                                    {
                                        message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                    },
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                };
                            }
                            else
                            {
                                DefaultRedirectURL = "~/Master/MyProfile";
                                filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                return;
                            }
                        }
                    }
                    else
                    {
                        long recordID = 0;
                        bool isFirstTimeInsert = false;
                        string udfListName = string.Empty;

                        List<AlleTurnsActionMethodsDTO> AllcommonMethod = eTurnsAllMethods.Where(x => (x.Module ?? string.Empty).ToLower() == "common").ToList();
                        if (AllcommonMethod != null && AllcommonMethod.Where(x => (x.ActionMethod ?? string.Empty).ToLower() == ActionName.ToLower() && (x.Controller ?? string.Empty).ToLower() == ControllerName.ToLower()).Count() > 0)
                        {
                            return;
                        }


                        if (ActionName.ToLower() == "udflist" && ControllerName.ToLower() == "udf")
                        {
                            udfListName = Convert.ToString(filterContext.HttpContext.Request.Params["t"]);
                            bool isAllowUDF = false;
                            if (!string.IsNullOrWhiteSpace(udfListName))
                            {
                                isAllowUDF = CheckUDFPermissionAsPerCurrentRequest(udfListName);
                                if (isAllowUDF == false)
                                {
                                    CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                    if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                    {
                                        //goto OldAuthorization;
                                        return;
                                    }
                                    else
                                    {
                                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                                        {
                                            filterContext.Result = new JsonResult
                                            {
                                                Data = new
                                                {
                                                    message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                },
                                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                            };
                                        }
                                        else
                                        {
                                            DefaultRedirectURL = "~/Master/MyProfile";
                                            filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                {
                                    //goto OldAuthorization;
                                    return;
                                }
                                else
                                {
                                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                                    {
                                        filterContext.Result = new JsonResult
                                        {
                                            Data = new
                                            {
                                                message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                            },
                                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                        };
                                    }
                                    else
                                    {
                                        DefaultRedirectURL = "~/Master/MyProfile";
                                        filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                        return;
                                    }
                                }
                            }

                            return;
                        }

                        if (filterContext.HttpContext.Request.Form != null && filterContext.HttpContext.Request.Form["ID"] != null)
                        {
                            long.TryParse(filterContext.HttpContext.Request.Form["ID"], out recordID);
                        }

                        if (IsActionNameAllowForIsInsert(ActionName) && filterContext.HttpContext.Session["IsInsert"] != null && Convert.ToString(filterContext.HttpContext.Session["IsInsert"]) != "")
                        {
                            bool.TryParse(Convert.ToString(filterContext.HttpContext.Session["IsInsert"]), out isFirstTimeInsert);
                        }

                        //this step is for finding current action methods and permission from master data
                        bool isAllowActionMethod = false;
                        var currentActionOptDtl = eTurnsAllMethods.Where(x => (x.ActionMethod ?? string.Empty).ToLower() == ActionName.ToLower() && (x.Controller ?? string.Empty).ToLower() == ControllerName.ToLower()).ToList();
                        if (currentActionOptDtl == null || !currentActionOptDtl.Any())
                        {
                            CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                            if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                            {
                                //goto OldAuthorization;
                                return;
                            }
                            else
                            {
                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                {
                                    filterContext.Result = new JsonResult
                                    {
                                        Data = new
                                        {
                                            message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                        },
                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                    };
                                }
                                else
                                {
                                    DefaultRedirectURL = "~/Master/MyProfile";
                                    filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                    return;
                                }
                            }
                        }
                        else if (currentActionOptDtl.Count > 1)
                        {
                            //string _requestMethod = filterContext.HttpContext.Request.HttpMethod;
                            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                            {
                                //post
                                if ((ActionName ?? string.Empty).ToLower().Contains("save"))
                                {

                                    currentOperationDTO = currentActionOptDtl.Where(x => x.IsInsert.GetValueOrDefault(false) == true
                                                                                        || x.IsUpdate.GetValueOrDefault(false) == true
                                                                                        || x.IsDelete.GetValueOrDefault(false) == true).FirstOrDefault();
                                    if (currentOperationDTO == null)
                                    {
                                        CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                        if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                        {
                                            //goto OldAuthorization;
                                            return;
                                        }
                                        else
                                        {
                                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                                            {
                                                filterContext.Result = new JsonResult
                                                {
                                                    Data = new
                                                    {
                                                        message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                    },
                                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                };
                                            }
                                            else
                                            {
                                                DefaultRedirectURL = "~/Master/MyProfile";
                                                filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isAllowActionMethod = CheckPermissionAsPerCurrentRequest(currentOperationDTO, recordID, isFirstTimeInsert);
                                        if (isAllowActionMethod == false)
                                        {
                                            CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                            if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                            {
                                                //goto OldAuthorization;
                                                return;
                                            }
                                            else
                                            {
                                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                                {
                                                    filterContext.Result = new JsonResult
                                                    {
                                                        Data = new
                                                        {
                                                            message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                        },
                                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                    };
                                                }
                                                else
                                                {
                                                    DefaultRedirectURL = "~/Master/MyProfile";
                                                    filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    currentOperationDTO = currentActionOptDtl.Where(x => x.IsView.GetValueOrDefault(false) == true).FirstOrDefault();

                                    if (currentOperationDTO == null)
                                    {
                                        CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                        if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                        {
                                            //goto OldAuthorization;
                                            return;
                                        }
                                        else
                                        {
                                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                                            {
                                                filterContext.Result = new JsonResult
                                                {
                                                    Data = new
                                                    {
                                                        message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                    },
                                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                };
                                            }
                                            else
                                            {
                                                DefaultRedirectURL = "~/Master/MyProfile";
                                                filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isAllowActionMethod = CheckPermissionAsPerCurrentRequest(currentOperationDTO, recordID, isFirstTimeInsert);
                                        if (isAllowActionMethod == false)
                                        {
                                            CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                            if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                            {
                                                //goto OldAuthorization;
                                                return;
                                            }
                                            else
                                            {
                                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                                {
                                                    filterContext.Result = new JsonResult
                                                    {
                                                        Data = new
                                                        {
                                                            message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                        },
                                                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                    };
                                                }
                                                else
                                                {
                                                    DefaultRedirectURL = "~/Master/MyProfile";
                                                    filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //get
                                currentOperationDTO = currentActionOptDtl.FirstOrDefault();
                                if (currentOperationDTO == null)
                                {
                                    CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                    if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                    {
                                        //goto OldAuthorization;
                                        return;
                                    }
                                    else
                                    {
                                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                                        {
                                            filterContext.Result = new JsonResult
                                            {
                                                Data = new
                                                {
                                                    message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                },
                                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                            };
                                        }
                                        else
                                        {
                                            DefaultRedirectURL = "~/Master/MyProfile";
                                            filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    isAllowActionMethod = CheckPermissionAsPerCurrentRequest(currentOperationDTO, recordID, isFirstTimeInsert);
                                    if (isAllowActionMethod == false)
                                    {
                                        CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);
                                        if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                        {
                                            //goto OldAuthorization;
                                            return;
                                        }
                                        else
                                        {
                                            if (filterContext.HttpContext.Request.IsAjaxRequest())
                                            {
                                                filterContext.Result = new JsonResult
                                                {
                                                    Data = new
                                                    {
                                                        message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                    },
                                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                };
                                            }
                                            else
                                            {
                                                DefaultRedirectURL = "~/Master/MyProfile";
                                                filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }

                            }
                        }
                        else
                        {
                            currentOperationDTO = currentActionOptDtl.FirstOrDefault();
                            if (currentOperationDTO != null && currentOperationDTO.Module.ToLower() == "common")
                                return;
                            else
                            {
                                isAllowActionMethod = CheckPermissionAsPerCurrentRequest(currentOperationDTO, recordID, isFirstTimeInsert);
                                if (isAllowActionMethod == false)
                                {
                                    CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "Authorization", string.Empty);

                                    if ((_strApplyNewAuthorization ?? string.Empty).ToLower() == "no")
                                    {
                                        //goto OldAuthorization;
                                        return;
                                    }
                                    else
                                    {
                                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                                        {
                                            filterContext.Result = new JsonResult
                                            {
                                                Data = new
                                                {
                                                    message = (ControllerName ?? string.Empty) + "_" + (ActionName ?? string.Empty) + "_accessdenied_forthisrequest_user"
                                                },
                                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                            };
                                        }
                                        else
                                        {
                                            DefaultRedirectURL = "~/Master/MyProfile";
                                            filterContext.Result = new RedirectResult(DefaultRedirectURL);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                    }
                }
                else if (SessionHelper.UserID <= 0 && ActionName.ToLower() != "userlogin" && ActionName.ToLower() != "forgotpassword" && ActionName.ToLower() != "updatepassword" && ActionName.ToLower() != "resetpassword")
                {
                    filterContext.Result = new RedirectResult("~/Master/UserLogin");
                    return;

                }

                #endregion

            }

        }

        private string AuthorizationRedirectURL(SessionHelper.ModuleList ModuleName)
        {
            switch (ModuleName)
            {
                case SessionHelper.ModuleList.QuickListPermission:
                    {
                        return "QuickList";

                    }
                case SessionHelper.ModuleList.TechnicianMaster:
                    {
                        return "TechnicianList";

                    }
                case SessionHelper.ModuleList.BinMaster:
                    {
                        return "BinList";

                    }
                case SessionHelper.ModuleList.CategoryMaster:
                    {
                        return "CategoryList";

                    }
                case SessionHelper.ModuleList.ToolCategory:
                    {
                        return "ToolCategoryList";

                    }
                case SessionHelper.ModuleList.AssetCategory:
                    {
                        return "AssetCategoryList";

                    }
                case SessionHelper.ModuleList.CompanyMaster:
                    {
                        return "CompanyList";

                    }
                case SessionHelper.ModuleList.CustomerMaster:
                    {
                        return "CustomerList";

                    }
                case SessionHelper.ModuleList.EnterpriseMaster:
                    {
                        return "EnterpriseList";

                    }
                case SessionHelper.ModuleList.FreightTypeMaster:
                    {
                        return "FreightTypeList";

                    }
                case SessionHelper.ModuleList.GLAccountsMaster:
                    {
                        return "GLAccountsList";


                    }
                case SessionHelper.ModuleList.GXPRConsignedJobMaster:
                    {
                        return "GXPRConsignedJobList";

                    }
                case SessionHelper.ModuleList.JobTypeMaster:
                    {
                        return "JobTypeList";

                    }
                case SessionHelper.ModuleList.ProjectMaster:
                    {
                        return "ProjectList";


                    }
                case SessionHelper.ModuleList.SupplierMaster:
                    {
                        return "SupplierList";

                    }
                case SessionHelper.ModuleList.ShipViaMaster:
                    {
                        return "ShipViaList";

                    }
                case SessionHelper.ModuleList.ToolMaster:
                    {
                        return "ToolList";


                    }
                case SessionHelper.ModuleList.UnitMaster:
                    {
                        return "UnitList";

                    }
                case SessionHelper.ModuleList.RoomMaster:
                    {
                        return "RoomList";

                    }
                case SessionHelper.ModuleList.LocationMaster:
                    {
                        return "LocationList";

                    }
                case SessionHelper.ModuleList.ManufacturerMaster:
                    {
                        return "ManufacturerList";

                    }
                case SessionHelper.ModuleList.MeasurementTermMaster:
                    {
                        return "MeasurementTermList";

                    }
                case SessionHelper.ModuleList.RoleMaster:
                    {
                        return "RoleList";

                    }
                case SessionHelper.ModuleList.UserMaster:
                    {
                        return "UserList";

                    }
                case SessionHelper.ModuleList.ResourceMaster:
                    {
                        return "ResourceList";

                    }
                case SessionHelper.ModuleList.Reports:
                    {
                        return "ViewReports";

                    }
                case SessionHelper.ModuleList.PullMaster:
                    {
                        return "PullMasterList";

                    }
                case SessionHelper.ModuleList.ItemMaster:
                    {
                        return "ItemMasterList";

                    }
                default:
                    {
                        return "UserLogin";

                    }
            }

        }

        private bool CheckPermissionAsPerCurrentRequest(AlleTurnsActionMethodsDTO currentActionOptDTO, long _recordID = 0, bool _isFirstTimeInsert = false)
        {
            bool isAllow = false;

            if (currentActionOptDTO.PermissionModuleID.GetValueOrDefault(0) == 0)
            {
                return true;
            }

            if (currentActionOptDTO.IsInsert.GetValueOrDefault(false) == true || currentActionOptDTO.IsUpdate.GetValueOrDefault(false) == true)
            {
                if (_recordID == 0)
                {
                    currentActionOptDTO.IsInsert = true;
                    currentActionOptDTO.IsUpdate = false;
                }
                else if (_recordID > 0)
                {
                    currentActionOptDTO.IsUpdate = true;
                    currentActionOptDTO.IsInsert = false;
                }
            }


            if (currentActionOptDTO.IsChecked.GetValueOrDefault(false) == true)
            {
                //for adminpermission
                isAllow = eTurnsWeb.Helper.SessionHelper.GetAdminPermissionAsPerCurrentRequest(currentActionOptDTO);
            }
            else
            {
                //for modulepermission
                SessionHelper.PermissionType objPermissionType = SessionHelper.GetPermissionTypefromCurrentRequest(currentActionOptDTO);
                UserRoleModuleDetailsDTO permissionDTO = eTurnsWeb.Helper.SessionHelper.GetModulePermissionAsPerCurrentRequest(objPermissionType, currentActionOptDTO);

                if (permissionDTO != null)
                {

                    if (objPermissionType == SessionHelper.PermissionType.View)
                        isAllow = permissionDTO.IsView;
                    else if (objPermissionType == SessionHelper.PermissionType.Insert)
                        isAllow = permissionDTO.IsInsert;
                    else if (objPermissionType == SessionHelper.PermissionType.Update)
                    {
                        if (_isFirstTimeInsert == true)
                        {
                            isAllow = true;
                        }
                        else
                        {
                            isAllow = permissionDTO.IsUpdate;
                        }
                    }
                    else if (objPermissionType == SessionHelper.PermissionType.Delete)
                        isAllow = permissionDTO.IsDelete;
                    else if (objPermissionType == SessionHelper.PermissionType.ShowDeleted)
                        isAllow = permissionDTO.ShowDeleted;
                    else if (objPermissionType == SessionHelper.PermissionType.ShowArchived)
                        isAllow = permissionDTO.ShowArchived;
                    else if (objPermissionType == SessionHelper.PermissionType.ShowUDF)
                        isAllow = permissionDTO.ShowUDF;
                    else if (objPermissionType == SessionHelper.PermissionType.Approval)
                        isAllow = permissionDTO.IsChecked;
                    else if (objPermissionType == SessionHelper.PermissionType.Submit)
                        isAllow = permissionDTO.IsChecked;
                    else if (objPermissionType == SessionHelper.PermissionType.ChangeOrder)
                        isAllow = permissionDTO.IsChecked;
                    else if (objPermissionType == SessionHelper.PermissionType.AllowPull)
                        isAllow = permissionDTO.IsChecked;
                    else if (objPermissionType == SessionHelper.PermissionType.IsChecked)
                        isAllow = permissionDTO.IsChecked;
                    else if (objPermissionType == SessionHelper.PermissionType.ShowChangeLog)
                        isAllow = permissionDTO.ShowChangeLog;

                }

            }

            return isAllow;
        }

        private bool CheckUDFPermissionAsPerCurrentRequest(string _udfListName)
        {
            int ModuleId = 0;
            bool isAllowUDF = false;
            string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey((_udfListName ?? string.Empty).ToLower(), out ModuleId);
            isAllowUDF = SessionHelper.GetModulePermission((SessionHelper.ModuleList)ModuleId, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowUDF);
            return isAllowUDF;
        }

        private bool IsActionNameAllowForIsInsert(string _strActionName)
        {
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));  
            if (eTurns.DTO.SiteSettingHelper.MethodsToCheckIsInsert  != string.Empty)
            {
                string MethodsToCheckIsInsert = eTurns.DTO.SiteSettingHelper.MethodsToCheckIsInsert; //Settinfile.Element("MethodsToCheckIsInsert").Value;
                List<string> lstMethodsToCheckIsInsert = (MethodsToCheckIsInsert ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower()).ToList();
                if (lstMethodsToCheckIsInsert.Contains((_strActionName ?? string.Empty).ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<string> GetOnlyIfRoomAvailableActionMethodList()
        {
            
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml")); 
            if (eTurns.DTO.SiteSettingHelper.OnlyIfRoomAvailable != string.Empty)
            {
                string OnlyIfRoomAvailable = eTurns.DTO.SiteSettingHelper.OnlyIfRoomAvailable; //Settinfile.Element("OnlyIfRoomAvailable").Value;
                List<string> lstOnlyIfRoomAvailable = (OnlyIfRoomAvailable ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower()).ToList();

                return lstOnlyIfRoomAvailable;
            }
            return null;
        }
    }

    public class AjaxOrChildActionOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest() && !filterContext.IsChildAction)
            {
                filterContext.Result = new RedirectResult("~/Account/PageNotFound");
            }
        }
    }
}