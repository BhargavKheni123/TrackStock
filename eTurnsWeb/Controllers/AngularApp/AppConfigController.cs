using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eTurnsMaster.DAL;
using System.Configuration;
using System.Net.Http.Headers;
using System.Web;
using System.Net.Http;

namespace eTurnsWeb.Controllers.AngularApp
{
    public class AppConfigController : Controller
    {
        #region [Global Declareation]
        private PullMasterDAL pullMasterDAL { get; set; }
        private CommonDAL commonDAL { get; set; }
        string enterPriseDBName;
        #endregion
        
        #region [Class Constructor]
        public AppConfigController()
        {
            enterPriseDBName = SessionHelper.EnterPriseDBName;
            pullMasterDAL = new PullMasterDAL(this.enterPriseDBName);
            commonDAL = new CommonDAL(this.enterPriseDBName);
        }
        #endregion

        #region [Class Methods]
        [System.Web.Mvc.HttpGet]
        public ActionResult InitializeAngularSession()
        {
            string CompanyLogoName = Url.Content("~/Content/Images/CompanyLogo.jpg");
            string EnterpriseLogoName = Url.Content("~/Content/Images/EnterpariseLogo.jpg");
            if (eTurnsWeb.Helper.SessionHelper.EnterPriceID > 0)
            {
                if ((string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl)))
                {
                    EnterpriseDTO objEnterpriseDTO = new eTurnsMaster.DAL.EnterpriseMasterDAL().GetEnterpriseByIdPlain(eTurnsWeb.Helper.SessionHelper.EnterPriceID);
                    if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseLogo))
                    {
                        EnterpriseLogoName = Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + objEnterpriseDTO.EnterpriseLogo);
                        eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                    }
                    else
                    {
                        eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl = EnterpriseLogoName;
                    }
                }
                else
                {
                    string _entLogo = Server.MapPath(Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl));
                    if (System.IO.File.Exists(_entLogo))
                    {
                        EnterpriseLogoName = Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl);
                    }

                }
            }
            if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
            {
                if ((string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl)))
                {
                    CompanyMasterDTO objCompanyMasterDTO = new eTurns.DAL.CompanyMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetCompanyByID(eTurnsWeb.Helper.SessionHelper.CompanyID);
                    if (objCompanyMasterDTO != null && !string.IsNullOrWhiteSpace(objCompanyMasterDTO.CompanyLogo))
                    {
                        CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + objCompanyMasterDTO.CompanyLogo);
                        eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                    }
                    else
                    {
                        eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl = CompanyLogoName;
                    }
                }
                else
                {
                    // CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl);
                    string _cmpLogo = Server.MapPath(Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl));
                    if (System.IO.File.Exists(_cmpLogo))
                    {
                        CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl);
                    }

                }
            }

            bool AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

            List<EnterpriseDTO> lstEnterPrises;
            List<CompanyMasterDTO> lstCompanies;
            List<RoomDTO> lstRooms;
            if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0)
            {
                //lstEnterPrises = eTurnsWeb.Helper.SessionHelper.EnterPriseList.Where(t => t.IsActive == true).ToList();
                lstEnterPrises = eTurnsWeb.Helper.SessionHelper.EnterPriseList.OrderBy(t => t.Name).ToList();

            }
            else
            {
                lstEnterPrises = new List<EnterpriseDTO>();
            }
            if (eTurnsWeb.Helper.SessionHelper.CompanyList != null && eTurnsWeb.Helper.SessionHelper.CompanyList.Count > 0)
            {
                //lstCompanies = eTurnsWeb.Helper.SessionHelper.CompanyList.Where(t => t.EnterPriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.IsActive == true).ToList();
                lstCompanies = eTurnsWeb.Helper.SessionHelper.CompanyList.Where(t => t.EnterPriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID).ToList();
            }
            else
            {
                lstCompanies = new List<CompanyMasterDTO>();
            }
            if (eTurnsWeb.Helper.SessionHelper.RoomList != null && eTurnsWeb.Helper.SessionHelper.RoomList.Count > 0)
            {
                //lstRooms = eTurnsWeb.Helper.SessionHelper.RoomList.Where(t => t.EnterpriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.CompanyID == eTurnsWeb.Helper.SessionHelper.CompanyID && t.IsRoomActive == true).ToList();
                lstRooms = eTurnsWeb.Helper.SessionHelper.RoomList.Where(t => t.EnterpriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.CompanyID == eTurnsWeb.Helper.SessionHelper.CompanyID).ToList();
            }
            else
            {
                lstRooms = new List<RoomDTO>();
            }
            string cookieToken, formToken;

            string oldCookieToken = null;
            HttpCookie antiForgeryCookieAngular = Request.Cookies.Get("__RequestVerificationToken_Angular");
            if (antiForgeryCookieAngular != null)
            {
                oldCookieToken = antiForgeryCookieAngular.Value;
            }
            System.Web.Helpers.AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);

            if (cookieToken == null)
            {
                cookieToken = oldCookieToken;
            }
            return Json(new
            {
                cookieToken = cookieToken,
                formToken = formToken,
                JwtToken = GenerateJwtToken(),
                EnterPriceID = eTurnsWeb.Helper.SessionHelper.EnterPriceID,
                CompanyID = eTurnsWeb.Helper.SessionHelper.CompanyID,
                RoomID = eTurnsWeb.Helper.SessionHelper.RoomID,
                CurrentCult = eTurns.DTO.Resources.ResourceHelper.CurrentCult.Name,
                EnterpriseLogoName = EnterpriseLogoName,
                UserName = SessionHelper.UserName,
                CompanyLogoName = CompanyLogoName,
                EnterPrisesList = lstEnterPrises,
                CompaniesList = lstCompanies,
                RoomsList = lstRooms.OrderBy(c=>c.RoomName),
                SearchPattern = SessionHelper.SearchPattern,
                RoomDateFormat = SessionHelper.RoomDateFormat,
                RoomTimeFormat = SessionHelper.RoomTimeFormat,
                RoomDateJSFormat = SessionHelper.RoomDateJSFormat,
                DateTimeFormat = SessionHelper.DateTimeFormat,
                CurrencyDecimalDigits = SessionHelper.CurrencyDecimalDigits,
                NumberDecimalDigits = SessionHelper.NumberDecimalDigits,
                TurnUsageFormat = SessionHelper.NumberAvgDecimalPoints,
                WeightDecimalPoints = SessionHelper.WeightDecimalPoints,
                NewEulaAccept=SessionHelper.NewEulaAccept,
                IsLicenceAccepted=SessionHelper.IsLicenceAccepted,
                AnotherLicenceAccepted=SessionHelper.AnotherLicenceAccepted,
                HasPasswordChanged= SessionHelper.HasPasswordChanged,
                EulaFileName= new eTurnsMaster.DAL.EulaMasterDAL().GetLastestFilePath(),
                IsCost = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.HideCostMarkUpSellPrice),
                CurrencySymbol = SessionHelper.CurrencySymbol,
                IsNgNLFAllowed=SessionHelper.IsNgNLFAllowed,
                ReleaseNumber= SiteSettingHelper.ReleaseNumber,
                IsABEnterprise = eTurnsWeb.Helper.SessionHelper.IsABEnterprise,
                IsFromSelectedDomain = eTurnsWeb.Helper.SessionHelper.IsFromSelectedDomain
            },
                        JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult LoginSessionAlive()
        {
            return Json(new { Message = "Active", EnterPriceID=SessionHelper.EnterPriceID, CompanyID=SessionHelper.CompanyID, RoomID=SessionHelper.RoomID }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult GetHeaderData()
        {

            string CompanyLogoName = Url.Content("~/Content/Images/CompanyLogo.jpg");
            string EnterpriseLogoName = Url.Content("~/Content/Images/EnterpariseLogo.jpg");
            if (eTurnsWeb.Helper.SessionHelper.EnterPriceID > 0)
            {
                if ((string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl)))
                {
                    EnterpriseDTO objEnterpriseDTO = new eTurnsMaster.DAL.EnterpriseMasterDAL().GetEnterpriseByIdPlain(eTurnsWeb.Helper.SessionHelper.EnterPriceID);
                    if (objEnterpriseDTO != null && !string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterpriseLogo))
                    {
                        EnterpriseLogoName = Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + objEnterpriseDTO.EnterpriseLogo);
                        eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                    }
                    else
                    {
                        eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl = EnterpriseLogoName;
                    }
                }
                else
                {
                    string _entLogo = Server.MapPath(Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl));
                    if (System.IO.File.Exists(_entLogo))
                    {
                        EnterpriseLogoName = Url.Content("~/Uploads/EnterpriseLogos/" + eTurnsWeb.Helper.SessionHelper.EnterPriceID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.EnterpriseLogoUrl);
                    }

                }
            }
            if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
            {
                if ((string.IsNullOrWhiteSpace(eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl)))
                {
                    CompanyMasterDTO objCompanyMasterDTO = new eTurns.DAL.CompanyMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetCompanyByID(eTurnsWeb.Helper.SessionHelper.CompanyID);
                    if (objCompanyMasterDTO != null && !string.IsNullOrWhiteSpace(objCompanyMasterDTO.CompanyLogo))
                    {
                        CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + objCompanyMasterDTO.CompanyLogo);
                        eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                    }
                    else
                    {
                        eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl = CompanyLogoName;
                    }
                }
                else
                {
                    // CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl);
                    string _cmpLogo = Server.MapPath(Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl));
                    if (System.IO.File.Exists(_cmpLogo))
                    {
                        CompanyLogoName = Url.Content("~/Uploads/CompanyLogos/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + eTurnsWeb.Helper.SessionHelper.CompanyLogoUrl);
                    }

                }
            }

            bool AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

            List<EnterpriseDTO> lstEnterPrises;
            List<CompanyMasterDTO> lstCompanies;
            List<RoomDTO> lstRooms;
            if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0)
            {
                //lstEnterPrises = eTurnsWeb.Helper.SessionHelper.EnterPriseList.Where(t => t.IsActive == true).ToList();
                lstEnterPrises = eTurnsWeb.Helper.SessionHelper.EnterPriseList.OrderBy(t => t.Name).ToList();

            }
            else
            {
                lstEnterPrises = new List<EnterpriseDTO>();
            }
            if (eTurnsWeb.Helper.SessionHelper.CompanyList != null && eTurnsWeb.Helper.SessionHelper.CompanyList.Count > 0)
            {
                //lstCompanies = eTurnsWeb.Helper.SessionHelper.CompanyList.Where(t => t.EnterPriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.IsActive == true).ToList();
                lstCompanies = eTurnsWeb.Helper.SessionHelper.CompanyList.Where(t => t.EnterPriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID).ToList();
            }
            else
            {
                lstCompanies = new List<CompanyMasterDTO>();
            }
            if (eTurnsWeb.Helper.SessionHelper.RoomList != null && eTurnsWeb.Helper.SessionHelper.RoomList.Count > 0)
            {
                //lstRooms = eTurnsWeb.Helper.SessionHelper.RoomList.Where(t => t.EnterpriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.CompanyID == eTurnsWeb.Helper.SessionHelper.CompanyID && t.IsRoomActive == true).ToList();
                lstRooms = eTurnsWeb.Helper.SessionHelper.RoomList.Where(t => t.EnterpriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && t.CompanyID == eTurnsWeb.Helper.SessionHelper.CompanyID).ToList();
            }
            else
            {
                lstRooms = new List<RoomDTO>();
            }

            return Json(new
            {
                EnterPriceID = eTurnsWeb.Helper.SessionHelper.EnterPriceID,
                CompanyID = eTurnsWeb.Helper.SessionHelper.CompanyID,
                RoomID = eTurnsWeb.Helper.SessionHelper.RoomID,

                CurrencySymbol=SessionHelper.CurrencySymbol,
                CurrentCult = eTurns.DTO.Resources.ResourceHelper.CurrentCult.Name,
                EnterpriseLogoName = EnterpriseLogoName,
                UserName = SessionHelper.UserName,
                CompanyLogoName = CompanyLogoName,
                EnterPrisesList = lstEnterPrises,
                CompaniesList = lstCompanies,
                RoomsList = lstRooms.OrderBy(c=>c.RoomName),
                SearchPattern = SessionHelper.SearchPattern,
                RoomDateFormat = SessionHelper.RoomDateFormat,
                RoomTimeFormat = SessionHelper.RoomTimeFormat,
                RoomDateJSFormat = SessionHelper.RoomDateJSFormat,
                DateTimeFormat = SessionHelper.DateTimeFormat,
                CurrencyDecimalDigits = SessionHelper.CurrencyDecimalDigits,
                NumberDecimalDigits = SessionHelper.NumberDecimalDigits,
                TurnUsageFormat = SessionHelper.NumberAvgDecimalPoints,
                WeightDecimalPoints = SessionHelper.WeightDecimalPoints,
                IsCost = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.HideCostMarkUpSellPrice)

            },
                        JsonRequestBehavior.AllowGet); ;
        }
                
        [System.Web.Mvc.HttpPost]
        public ActionResult GetLanguageResources()
        {

            List<KeyValDTO> ResLayoutList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResLayout", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResCommonList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResCommon", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResDashboardList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResDashboard", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResItemMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResItemMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResMoveMaterialList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResMoveMaterial", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);

            List<KeyValDTO> ResPullMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResPullMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResProjectMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResProjectMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResUnitMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResUnitMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResCategoryMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResCategoryMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResGLAccountList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResGLAccount", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResRequisitionMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResRequisitionMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResMessageList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResMessage", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResGridHeaderList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResGridHeader", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResBinList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResBin", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResPullDetailsList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResPullDetails", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResNarrowSearchList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResNarrowSearch", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResOrderList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResOrder", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResTermsAndConditionList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResTermsAndCondition", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResCartItemList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResCartItem", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResItemLocationDetailsList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResItemLocationDetails", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResToolCheckInOutHistoryList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResToolCheckInOutHistory", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResToolMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResToolMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResTransferList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResTransfer", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResToolCategoryList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResToolCategory", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResReceiveOrderDetailsList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResReceiveOrderDetails", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResLabelPrintingList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResLabelPrinting", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResItemManufacturerDetailsList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResItemManufacturerDetails", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResItemLocationQTYList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResItemLocationQTY", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResInventoryCountList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResInventoryCount", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResInventoryAnalysisList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResInventoryAnalysis", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResCustomerList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResCustomer", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResCostUOMMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResCostUOMMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            List<KeyValDTO> ResAssetMasterList = eTurns.DTO.Resources.ResourceHelper.GetResourceData("ResAssetMaster", eTurns.DTO.Resources.ResourceHelper.FileCulterExtension);
            
            return Json(new
            {

                ResLayout = ResLayoutList,
                ResCommon = ResCommonList,
                ResDashboard = ResDashboardList,
                ResItemMaster = ResItemMasterList,
                ResMoveMaterial = ResMoveMaterialList,
                ResPullMaster = ResPullMasterList,
                ResProjectMaster = ResProjectMasterList,
                ResUnitMaster = ResUnitMasterList,
                ResCategoryMaster = ResCategoryMasterList,
                ResGLAccount = ResGLAccountList,
                ResRequisitionMaster = ResRequisitionMasterList,
                ResMessage = ResMessageList,
                ResGridHeader = ResGridHeaderList,
                ResBin = ResBinList,
                ResPullDetails = ResPullDetailsList,
                ResNarrowSearch = ResNarrowSearchList,
                ResOrder = ResOrderList,
                ResTermsAndCondition= ResTermsAndConditionList,
                ResCartItem= ResCartItemList,
                ResItemLocationDetails= ResItemLocationDetailsList,
                ResToolCheckInOutHistory= ResToolCheckInOutHistoryList,
                ResToolMaster= ResToolMasterList,
                ResTransfer= ResTransferList,
                ResToolCategory= ResToolCategoryList,
                ResReceiveOrderDetails= ResReceiveOrderDetailsList,
                ResLabelPrinting= ResLabelPrintingList,
                ResItemManufacturerDetails= ResItemManufacturerDetailsList,
                ResItemLocationQTY= ResItemLocationQTYList,
                ResInventoryCount= ResInventoryCountList,
                ResInventoryAnalysis= ResInventoryAnalysisList,
                ResCustomer= ResCustomerList,
                ResCostUOMMaster= ResCostUOMMasterList,
                ResAssetMaster= ResAssetMasterList
            },
                            JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult PullLotSrSelectionForCreditPopup(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Int64 BinID = 0;
            double PullQuantity = 0;
            double EnteredPullQuantity = 0;

            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string CurrentDeletedLoaded = Convert.ToString(Request["CurrentDeletedLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsStagginLocation = false;
            EnteredPullQuantity = PullQuantity;

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            int TotalRecordCount = 0;
            PullTransactionDAL objPullDetails = new PullTransactionDAL(this.enterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();

            List<ItemLocationLotSerialDTO> lstsetPulls = new List<ItemLocationLotSerialDTO>();

            string[] arrItem;

            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();

            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;
            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(this.enterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.BinNumber = string.Empty;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;//oItem.DefaultPullQuantity.GetValueOrDefault(0);

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[1]);
                                    objpull.Expiration = Convert.ToString(arrItem[1]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[2]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[0]);
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    objpull.ExpirationDate = Convert.ToDateTime(arrItem[0]);
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                    }

                    lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullForMoreCredit(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, false, string.Empty, IsStagginLocation);

                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && !oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                        {
                            ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ItemGUID = ItemGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.BinNumber = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.BinNumber = objLocDTO.BinNumber;
                            }
                            if (oItem.SerialNumberTracking)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                            oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && !oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.BinNumber)
                                            && !oItem.SerialNumberTracking
                                            && !oItem.LotNumberTracking
                                            && !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();

                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;

                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                        retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullForMoreCredit(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, PullQuantity, true, string.Empty, IsStagginLocation);
                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.BinNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, false, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            if (PullQuantity > 0)
            {
                if (lstsetPulls != null && lstsetPulls.Count > 0)
                {
                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.SerialNumberTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber)).ToList();
                    }
                    else if (oItem.LotNumberTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber)).ToList();
                    }
                    else if (oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                }
                for (int i = 0; i < lstsetPulls.Count(); i++)
                {
                    PullQuantity -= lstsetPulls[i].PullQuantity;

                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = (oItem.SerialNumberTracking ? lstsetPulls[i].SerialNumber : lstsetPulls[i].LotNumber);
                    oLotSr.Expiration = (oItem.DateCodeTracking ? lstsetPulls[i].Expiration : string.Empty);
                    oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                    oLotSr.BinNumber = string.Empty;
                    if (objLocDTO != null && objLocDTO.ID > 0)
                    {
                        oLotSr.BinNumber = objLocDTO.BinNumber;
                    }
                    if (oItem.SerialNumberTracking)
                    {
                        oLotSr.PullQuantity = 1;
                    }
                    else
                    {
                        oLotSr.PullQuantity = lstsetPulls[i].PullQuantity;
                    }
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                    retlstLotSrs.Add(oLotSr);
                }
            }

            if (CurrentDeletedLoaded != "")
            {
                string[] arrDeletedIds = new string[] { };
                if (!string.IsNullOrWhiteSpace(CurrentDeletedLoaded))
                {
                    arrDeletedIds = CurrentDeletedLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrDeletedIds.Count() > 0)
                    {
                        string[] arrSerialLots = new string[arrDeletedIds.Count()];
                        for (int i = 0; i < arrDeletedIds.Count(); i++)
                        {
                            PullQuantity += 1;
                            if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                                || !oItem.DateCodeTracking)
                            {
                                arrItem = new string[2];
                                arrItem[0] = arrDeletedIds[i].Substring(0, arrDeletedIds[i].LastIndexOf("_"));
                                arrItem[1] = arrDeletedIds[i].Replace(arrItem[0] + "_", "");
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                }
                            }
                            else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[1]).Date);
                                        else
                                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[1]))
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[1]).Date);
                                        else
                                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                }
                            }
                            else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[0]).Date);
                                    }
                                }
                            }
                            else
                            {
                                arrItem = arrDeletedIds[i].Split('_');
                                if (arrItem.Length > 1)
                                {
                                    if (oItem.SerialNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
                                    }
                                    if (oItem.LotNumberTracking)
                                    {
                                        retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
                                    }
                                    if (oItem.DateCodeTracking)
                                    {
                                        if (!string.IsNullOrWhiteSpace(arrItem[0]))
                                            retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == Convert.ToDateTime(arrItem[0]).Date);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            PullQuantity = EnteredPullQuantity - retlstLotSrs.Sum(x => x.PullQuantity);

            if (PullQuantity > 0)
            {
                if (oItem.SerialNumberTracking)
                {
                    for (int i = 0; i < PullQuantity; i++)
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;
                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                }
                else
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = string.Empty;
                    oLotSr.Expiration = string.Empty;
                    oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                    oLotSr.BinNumber = string.Empty;

                    if (objLocDTO != null && objLocDTO.ID > 0)
                    {
                        oLotSr.BinNumber = objLocDTO.BinNumber;
                    }
                    oLotSr.PullQuantity = PullQuantity;
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                    retlstLotSrs.Add(oLotSr);
                }
            }

            retlstLotSrs.ForEach(x => x.KitDetailGUID = Guid.NewGuid());

            return Json(new
            {
                draw = param.draw,
                recordsTotal = TotalRecordCount,
                recordsFiltered = TotalRecordCount, //filteredCompanies.Count(),
                data = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Private Methods]
        private string GenerateJwtToken()
        {
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
            UserMasterDTO userMasterDTO = objEnterPriseUserMasterDAL.CheckAuthanticationUserNameOnly(SessionHelper.UserName);
            if (userMasterDTO != null)
            {

                string CoreWebApiURL = ConfigurationManager.AppSettings["CoreWebApiURL"];
                HttpClient client = new HttpClient();
                AuthenticateRequest authenticateRequest = new AuthenticateRequest();
                authenticateRequest.Email = userMasterDTO.UserName;
                authenticateRequest.Password = userMasterDTO.Password;
                HttpResponseMessage httpResponseMessage = client.PostAsJsonAsync<AuthenticateRequest>(CoreWebApiURL + "/api/User/Authenticate", authenticateRequest).Result;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    WebApiResponse webApiResponse = httpResponseMessage.Content.ReadAsAsync<WebApiResponse>().Result;
                    if (webApiResponse != null && webApiResponse.Code == "200")
                    {
                        return Convert.ToString(webApiResponse.Data);
                    }
                }
            }
            return "";
        }

        #endregion

        public static void InvalidateCache(string cacheKey)
        {
            try
            {
                EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
                string Token = objEnterPriseUserMasterDAL.GetJwtToken(SessionHelper.UserID);
                if (!String.IsNullOrEmpty(Token))
                {
                    string CoreWebApiURL = ConfigurationManager.AppSettings["CoreWebApiURL"];
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue("Bearer", Token);
                    InvalidateCacheRequest invalidateCacheRequest = new InvalidateCacheRequest();
                    invalidateCacheRequest.CacheKey = cacheKey;

                    HttpResponseMessage httpResponseMessage = client.PostAsJsonAsync<InvalidateCacheRequest>(CoreWebApiURL + "/api/InvalidateCache/RemoveCache", invalidateCacheRequest).Result;
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                    }
                }
            }
            catch { }
        }
        public static void InvalidateCacheByKeyStartWith(string cacheKey)
        {
            try
            {
                EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
                string Token = objEnterPriseUserMasterDAL.GetJwtToken(SessionHelper.UserID);
                if (!String.IsNullOrEmpty(Token))
                {
                    string CoreWebApiURL = ConfigurationManager.AppSettings["CoreWebApiURL"];
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization
                                     = new AuthenticationHeaderValue("Bearer", Token);
                    InvalidateCacheRequest invalidateCacheRequest = new InvalidateCacheRequest();
                    invalidateCacheRequest.CacheKey = cacheKey;

                    HttpResponseMessage httpResponseMessage = client.PostAsJsonAsync<InvalidateCacheRequest>(CoreWebApiURL + "/api/InvalidateCache/InvalidateCacheByKeyStartWith", invalidateCacheRequest).Result;
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                    }
                }
            }
            catch { }
        }
    }
}
