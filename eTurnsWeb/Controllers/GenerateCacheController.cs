using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    public class GenerateCacheController : eTurnsControllerBase
    {
        public ActionResult GenerateCache()
        {
            return View();

        }

        public List<SelectListItem> GetCompany()
        {
            List<SelectListItem> lstItem = new List<SelectListItem>();
            try
            {
                CompanyMasterDAL objdal = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                List<eTurns.DTO.CompanyMasterDTO> objCompany = new List<CompanyMasterDTO>();
                if (SessionHelper.CompanyList != null)
                {

                    objCompany = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).ToList();
                }

                if (objCompany != null)
                {
                    if (objCompany.Count > 0)
                    {
                        // lstItem = new List<SelectListItem>();
                        foreach (var item in objCompany)
                        {
                            SelectListItem obj = new SelectListItem();
                            obj.Text = item.Name;
                            obj.Value = item.ID.ToString();
                            lstItem.Add(obj);
                        }

                    }
                }
                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                lstItem = null;
            }

        }

        [HttpGet]
        public JsonResult UpdateSiteSettings(string settingKeyVal)
        {
            try
            {
                string sKey = settingKeyVal.Split('#')[0];
                string sVal = settingKeyVal.Split('#')[1];

                XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                if (Settinfile.Element(sKey) != null)
                {
                    //Settinfile.Element("ApplyNewAuthorization").Value = sVal;
                    //string fName = System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml");
                    //Settinfile.Save(fName);

                    SiteSettingHelper.ApplyNewAuthorization = sVal;
                    SiteSettingInfoHelper objDAL = new SiteSettingInfoHelper();
                    SiteSettingInfo objSiteInfo = new SiteSettingInfo();
                    objSiteInfo = objDAL.GetSiteSettingInfo();
                    if (objSiteInfo != null)
                    {
                        objSiteInfo.ApplyNewAuthorization = sVal;
                        objDAL.SaveSiteSettingInfo(objSiteInfo);
                    }
                }

                return Json(new { Message = "Ok", Status = "Success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "Fail" }, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        public JsonResult ReGenerateCache(string para = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            Int64[] CompanyList = s.Deserialize<Int64[]>(para);

            string releaseNumber;
            CacheHelper<string>.InvalidateCacheByKey("System.String-ReleaseNumber");
            releaseNumber = CommonUtility.GetReleaseNumber();

            bool isLoadEnterpriseGridOrdering;
            CacheHelper<bool?>.InvalidateCacheByKey("System.Boolean-LoadEnterpriseGridOrdering");
            isLoadEnterpriseGridOrdering = CommonUtility.GetIsLoadEnterpriseGridOrdering();

            eTurnsMaster.DAL.CommonMasterDAL objCommonDAL = new eTurnsMaster.DAL.CommonMasterDAL();
            //CacheHelper<IEnumerable<SiteUrlSettingDTO>>.InvalidateCacheByKey("Cached_SiteUrlSettingDTO");
            objCommonDAL.RefreshSiteURLSetting();

            if (CompanyList.Count() > 0)
            {
                #region Object Initialize
                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CartItemDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();

                GXPRConsignedJobMasterDAL objGXPRConsignedJobMasterDAL = null;
                CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.InvalidateCache();

                ItemLocationDetailsDAL objItemLocationDetailsDAL = null;
                CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.InvalidateCache();

                //ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = null;
                //CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();

                ItemSupplierDetailsDAL objItemSupplierDetailsDAL = null;
                CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();

                JobTypeMasterDAL objJobTypeMasterDAL = null;
                CacheHelper<IEnumerable<JobTypeMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<KitDetailDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<KitMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<LocationMasterDTO>>.InvalidateCache();

                // ManufacturerMasterDAL objManufacturerMasterDAL = null;
                CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();

                //MaterialStagingDAL objMaterialStagingDAL = null;
                CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();

                MeasurementTermDAL objMeasurementTermDAL = null;
                CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.InvalidateCache();

                //ModuleMasterDAL objModuleMasterDAL = null;
                //CacheHelper<IEnumerable<ModuleMasterDTO>>.InvalidateCache();

                //OrderDetailsDAL objOrderDetailsDAL = null;
                CacheHelper<IEnumerable<OrderDetailsDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ProjectSpendItemsDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<QuickListMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<QuickListDetailDTO>>.InvalidateCache();

                ReceiveOrderDetailsDAL objReceiveOrderDetailsDAL = null;
                CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.InvalidateCache();

                ResourceDAL objResourceDAL = null;
                MobileResourcesDAL objMobileResourcesDAL = null;
                CacheHelper<IEnumerable<ResourceLanguageDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ResourceModuleMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<MobileResourcesDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<RoleMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<RoomDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ToolMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<TransferMasterDAL>>.InvalidateCache();

                CacheHelper<IEnumerable<TransferDetailDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<UDFDTO>>.InvalidateCache();


                CacheHelper<IEnumerable<UserMasterDTO>>.InvalidateCache();


                //WorkOrderLineItemsDAL objWorkOrderLineItemsDAL = null;
                CacheHelper<IEnumerable<WorkOrderLineItemsDTO>>.InvalidateCache();

                
                CacheHelper<IEnumerable<AlleTurnsActionMethodsDTO>>.InvalidateCache();

                #endregion
                CacheHelper<IEnumerable<UsersUISettingsDTO>>.InvalidateCacheByKeyStartWith("Cached_UsersUISettingsDTO");
                try
                {
                    foreach (Int64 item in CompanyList)
                    {


                        objGXPRConsignedJobMasterDAL = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
                        objGXPRConsignedJobMasterDAL.GetCachedData(0, item, false, false);

                        //objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                        //objItemLocationDetailsDAL.GetCachedData(Guid.NewGuid(), 0, item);

                        //objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        //objItemManufacturerDetailsDAL.GetCachedData(0, item);


                        //objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                        //objItemSupplierDetailsDAL.GetCachedData(0, item);

                        //objJobTypeMasterDAL = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
                        //objJobTypeMasterDAL.GetCachedData(0, item);


                        //objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                        //objManufacturerMasterDAL.GetManufacturerByRoomNormal(0, item, false, false, null);



                        objMeasurementTermDAL = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
                        objMeasurementTermDAL.GetCachedData(0, item, false, false);

                        //objModuleMasterDAL = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                        //objModuleMasterDAL.GetCachedData();// 


                        //objReceiveOrderDetailsDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        //objReceiveOrderDetailsDAL.GetCachedData(0, item);


                        objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                        objResourceDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                        objResourceDAL.GetCachedResourceModuleMasterData(item);

                        //objMobileResourcesDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                        //objMobileResourcesDAL.GetCachedData(item);
                        

                    }


                    objCommonDAL = new eTurnsMaster.DAL.CommonMasterDAL();
                    objCommonDAL.GetAlleTurnsActionMethodsData();

                    message = string.Format(ResCommon.CacheGeneratedSuccessfully, HttpStatusCode.OK);
                    status = ResCommon.CacheGeneratedSuccessfully;
                }
                catch (Exception)
                {
                    #region Null all the Object
                    objGXPRConsignedJobMasterDAL = null;
                    objItemLocationDetailsDAL = null;
                    //objItemManufacturerDetailsDAL = null;
                    //objItemSupplierDetailsDAL = null;
                    //objJobTypeMasterDAL = null;                    
                    //objManufacturerMasterDAL = null;
                    objMeasurementTermDAL = null;
                    //objModuleMasterDAL = null;
                    objReceiveOrderDetailsDAL = null;
                    objResourceDAL = null;

                    #endregion

                    message = string.Format(ResCommon.ErrorGenaratingCache, HttpStatusCode.ExpectationFailed);
                    status = ResCommon.ErrorGenaratingCache;
                }
                finally
                {
                    #region Null all the Object
                    objGXPRConsignedJobMasterDAL = null;
                    objItemLocationDetailsDAL = null;
                    //objItemManufacturerDetailsDAL = null;
                    objItemSupplierDetailsDAL = null;
                    objJobTypeMasterDAL = null;
                    //objManufacturerMasterDAL = null;
                    objMeasurementTermDAL = null;
                    //objModuleMasterDAL = null;
                    objReceiveOrderDetailsDAL = null;
                    objResourceDAL = null;

                    #endregion
                }
            }


            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GenerateCacheCompany(Int64 UserID, Int64 CompanyId)
        {
            //string message = "";
            //string status = "";

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {

                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

            }


            if (CompanyId > 0)
            {
                Int64 item = CompanyId;
                #region Object Initialize
                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CartItemDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();

                GXPRConsignedJobMasterDAL objGXPRConsignedJobMasterDAL = null;
                CacheHelper<IEnumerable<GXPRConsigmentJobMasterDTO>>.InvalidateCache();

                ItemLocationDetailsDAL objItemLocationDetailsDAL = null;
                CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.InvalidateCache();

                //ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = null;
                //CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();

                ItemSupplierDetailsDAL objItemSupplierDetailsDAL = null;
                CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();

                JobTypeMasterDAL objJobTypeMasterDAL = null;
                CacheHelper<IEnumerable<JobTypeMasterDTO>>.InvalidateCache();

                KitDetailDAL objKitDetailDAL = null;
                CacheHelper<IEnumerable<KitDetailDTO>>.InvalidateCache();

                KitMasterDAL objKitMasterDAL = null;
                CacheHelper<IEnumerable<KitMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<LocationMasterDTO>>.InvalidateCache();

                //ManufacturerMasterDAL objManufacturerMasterDAL = null;
                CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();

                //MaterialStagingDAL objMaterialStagingDAL = null;
                CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();

                MeasurementTermDAL objMeasurementTermDAL = null;
                CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.InvalidateCache();

                //ModuleMasterDAL objModuleMasterDAL = null;
                //CacheHelper<IEnumerable<ModuleMasterDTO>>.InvalidateCache();

                //OrderDetailsDAL objOrderDetailsDAL = null;
                CacheHelper<IEnumerable<OrderDetailsDTO>>.InvalidateCache();

                ProjectSpendItemsDAL objProjectSpendItemsDAL = null;
                CacheHelper<IEnumerable<ProjectSpendItemsDTO>>.InvalidateCache();

                QuickListDAL objQuickListDAL = null;
                CacheHelper<IEnumerable<QuickListMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<QuickListDetailDTO>>.InvalidateCache();

                ReceiveOrderDetailsDAL objReceiveOrderDetailsDAL = null;
                CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.InvalidateCache();

                ResourceDAL objResourceDAL = null;
                CacheHelper<IEnumerable<ResourceLanguageDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ResourceModuleMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ResourceModuleDetailsDTO>>.InvalidateCache();

                RoleMasterDAL objRoleMasterDAL = null;
                CacheHelper<IEnumerable<RoleMasterDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<RoomDTO>>.InvalidateCache();
                CacheHelper<IEnumerable<ToolMasterDTO>>.InvalidateCache();
                //TransferMasterDAL objTransferDAL = null;
                //CacheHelper<IEnumerable<TransferMasterDAL>>.InvalidateCache();
                //TransferDetailDAL objTransferDetailDAL = null;
                //CacheHelper<IEnumerable<TransferDetailDTO>>.InvalidateCache();                
                CacheHelper<IEnumerable<UDFDTO>>.InvalidateCache();
                //WorkOrderLineItemsDAL objWorkOrderLineItemsDAL = null;
                CacheHelper<IEnumerable<WorkOrderLineItemsDTO>>.InvalidateCache();
                var tmpsupplierIds = new List<long>();
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                #endregion

                try
                {
                    objGXPRConsignedJobMasterDAL = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
                    objGXPRConsignedJobMasterDAL.GetCachedData(0, item, false, false);

                    //objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    //objItemLocationDetailsDAL.GetCachedData(Guid.NewGuid(), 0, item);

                    //objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                    //objItemManufacturerDetailsDAL.GetCachedData(0, item);

                    //objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                    //objItemSupplierDetailsDAL.GetCachedData(0, item);

                    //objJobTypeMasterDAL = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
                    //objJobTypeMasterDAL.GetCachedData(0, item);

                    //objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    //objKitDetailDAL.GetCachedData(0, item, false, false, true);

                    objKitMasterDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                    objKitMasterDAL.GetCachedData(0, item, false, false, tmpsupplierIds, CurrentTimeZone);

                    //objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                    //objManufacturerMasterDAL.GetManufacturerByRoomNormal(0, item, false, false, null);

                    objMeasurementTermDAL = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
                    objMeasurementTermDAL.GetCachedData(0, item, false, false);

                    //objModuleMasterDAL = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                    //objModuleMasterDAL.GetCachedData();// 

                    //objProjectSpendItemsDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                    //objProjectSpendItemsDAL.GetCachedData(Guid.Empty, 0, item, tmpsupplierIds);

                    objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                    objQuickListDAL.GetAllRecords(0, item, false, false);

                    //objReceiveOrderDetailsDAL = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    //objReceiveOrderDetailsDAL.GetCachedData(0, item);

                    objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                    objResourceDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                    objResourceDAL.GetCachedResourceModuleMasterData(item);

                    //objRoleMasterDAL = new RoleMasterDAL(SessionHelper.EnterPriseDBName);
                    //objRoleMasterDAL.GetAllRecords(0, item);

                    //objTransferDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                    //objTransferDAL.GetCachedData(0, item);

                    //var supplierIds = new List<long>();
                    //objTransferDetailDAL = new TransferDetailDAL(SessionHelper.EnterPriseDBName);
                    //objTransferDetailDAL.GetCachedData(0, item, supplierIds);
                }
                catch (Exception)
                {
                    #region Null all the Object
                    objGXPRConsignedJobMasterDAL = null;
                    objItemLocationDetailsDAL = null;
                    //objItemManufacturerDetailsDAL = null;
                    //objItemSupplierDetailsDAL = null;
                    //objJobTypeMasterDAL = null;
                    objKitDetailDAL = null;
                    objKitMasterDAL = null;
                    //objManufacturerMasterDAL = null;
                    objMeasurementTermDAL = null;
                    //objModuleMasterDAL = null;
                    objProjectSpendItemsDAL = null;
                    objQuickListDAL = null;
                    objReceiveOrderDetailsDAL = null;
                    objResourceDAL = null;
                    objRoleMasterDAL = null;
                    //objTransferDAL = null;
                    //objTransferDetailDAL = null;                    
                    
                    //objUserMasterDAL = null;
                    #endregion

                    return "ERROR";
                }
                finally
                {
                    #region Null all the Object
                    objGXPRConsignedJobMasterDAL = null;
                    objItemLocationDetailsDAL = null;
                    //objItemManufacturerDetailsDAL = null;
                    objItemSupplierDetailsDAL = null;
                    objJobTypeMasterDAL = null;
                    objKitDetailDAL = null;
                    objKitMasterDAL = null;
                    //objManufacturerMasterDAL = null;
                    objMeasurementTermDAL = null;
                    //objModuleMasterDAL = null;
                    objProjectSpendItemsDAL = null;
                    objQuickListDAL = null;
                    objReceiveOrderDetailsDAL = null;
                    objResourceDAL = null;
                    objRoleMasterDAL = null;
                    //objTransferDAL = null;
                    //objTransferDetailDAL = null;                    
                    #endregion
                }
            }
            return "OK";
            //return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public string GenerateCacheAftereVMI(Int64 UserID, Int64 CompanyId)
        {
            //string message = "";
            //string status = "";

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {

                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

            }


            if (CompanyId > 0)
            {
                Int64 item = CompanyId;
                #region Object Initialize



                ItemLocationDetailsDAL objItemLocationDetailsDAL = null;
                CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();

                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();


                #endregion

                try
                {
                    //objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    //objItemLocationDetailsDAL.GetCachedData(Guid.NewGuid(), 0, item);
                }
                catch (Exception)
                {
                    #region Null all the Object

                    objItemLocationDetailsDAL = null;
                    //objPullMasterDAL = null;
                    #endregion

                    return "ERROR";
                    //message = string.Format("Error in cache generation!!", HttpStatusCode.ExpectationFailed);
                    //status = "Error in cache generation!!";
                }
                finally
                {
                    #region Null all the Object

                    objItemLocationDetailsDAL = null;
                    //objPullMasterDAL = null;

                    //objItemLocationLevelQuanityDAL = null;
                    #endregion
                }
            }
            return "OK";
            //return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GetUserInformationLogin(string Email, string secretkey)
        {
            string secretkeyconfig = Convert.ToString(ConfigurationManager.AppSettings["secretkey"]);

            if (secretkeyconfig == secretkey)
            {
                UserBAL objUserBAL = new UserBAL();
                UserMasterDTO objDTO = objUserBAL.CheckAuthantication(Email);
                if (objDTO != null)
                {
                    if (objDTO.UserType == 3)
                        return objDTO.ID.ToString() + "#" + objDTO.CompanyID.ToString() + "#" + objDTO.UserName.ToString() + "#" + objDTO.CompanyName.ToString() + "#" + objDTO.IsLicenceAccepted.ToString();
                    else
                        return "0#0";
                }
                return "0#0";
            }
            else
            {
                return "0#0";
            }
        }
        [HttpGet]
        public string GetUserRoomwisePDASyncBackDays(string Email, string Password)
        {
            string secretkeyconfig = Convert.ToString(ConfigurationManager.AppSettings["secretkey"]);
            string Result = string.Empty;
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objDTO = objUserBAL.CheckAuthantication(Email, Password);

            if (objDTO != null)
            {
                if (objDTO.UserType == 3)
                {
                    List<eTurnsMaster.DAL.UserRoomAccess> lstAccess = new eTurnsMaster.DAL.EnterPriseUserMasterDAL().getUserPermission(objDTO.ID);
                    if (lstAccess != null && lstAccess.Count() > 0)
                    {
                        foreach (eTurnsMaster.DAL.UserRoomAccess u in lstAccess)
                        {
                            eTurnsRegionInfo objRegionalSetting = new eTurnsRegionInfo();
                            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objDTO.EnterpriseDbName);
                            objRegionalSetting = objRegionSettingDAL.GetRegionSettingsById(u.RoomId, u.CompanyId, objDTO.ID);
                            if (objRegionalSetting != null)
                            {
                                if (string.IsNullOrWhiteSpace(Result))
                                {
                                    Result = u.RoomId + "," + objRegionalSetting.NumberOfBackDaysToSyncOverPDA.GetValueOrDefault(0);
                                }
                                else
                                {
                                    Result = Result + "#" + u.RoomId + "," + objRegionalSetting.NumberOfBackDaysToSyncOverPDA.GetValueOrDefault(0);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(Result))
                                {
                                    Result = u.RoomId + "," + 0;
                                }
                                else
                                {
                                    Result = Result + "#" + u.RoomId + "," + 0;
                                }
                            }
                        }
                    }
                    return Result;
                }
                else
                    return "0";
            }
            return "0";

        }

        [HttpGet]
        public string GetUserInformation(string Email, string Password)
        {
            //eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            //UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthantication(Email, Password);
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objDTO = objUserBAL.CheckAuthantication(Email, Password);

            if (objDTO != null)
            {
                List<eTurnsMaster.DAL.UserRoomAccess> lstAccess = new eTurnsMaster.DAL.EnterPriseUserMasterDAL().getUserPermission(objDTO.ID);

                if (objDTO.UserType == 3 && lstAccess != null && lstAccess.Count > 0)
                    return objDTO.ID.ToString() + "#" + objDTO.CompanyID.ToString() + "#" + objDTO.UserName.ToString() + "#" + objDTO.CompanyName.ToString() + "#" + objDTO.IsLicenceAccepted.ToString();
                else
                    return "0#0";
            }
            return "0#0";
        }



        [HttpPost]
        public string LogUserSyncHistory(UserSyncHistoryDTO objUserSyncHistoryDTO)
        {
            string retSRT = "fail";
            try
            {
                objUserSyncHistoryDTO.SyncTime = DateTime.UtcNow;
                eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
                objUserSyncHistoryDTO.ErrorDescription = string.Empty;
                UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(objUserSyncHistoryDTO.SyncByUserID);
                if (objDTO != null)
                {
                    CommonDAL objCommonDAL = new CommonDAL(objDTO.EnterpriseDbName);
                    objCommonDAL.InsertUserSyncHist(objUserSyncHistoryDTO);

                    if (!string.IsNullOrWhiteSpace(objUserSyncHistoryDTO.SyncStep) && objUserSyncHistoryDTO.SyncStep.ToLower() == "sync process start.")
                    {
                        objCommonDAL.UpdateUserLastSyncDetails(objUserSyncHistoryDTO);
                    }
                }
                retSRT = "ok";
            }
            catch (Exception ex)
            {
                retSRT = Convert.ToString(ex);
            }
            return retSRT;
        }

        [HttpGet]
        public string LogUserSyncHistoryPara(long ID, long SyncByUserID, string SyncStep, DateTime SyncTime, long CompanyID, long RoomID, string BuildNo, string DeviceName, Guid SyncTransactionID, string ErrorDescription)
        {
            string retSRT = "fail";
            try
            {
                UserSyncHistoryDTO objUserSyncHistoryDTO = new UserSyncHistoryDTO();

                objUserSyncHistoryDTO.SyncTime = DateTime.UtcNow;
                objUserSyncHistoryDTO.BuildNo = BuildNo;
                objUserSyncHistoryDTO.CompanyID = CompanyID;
                objUserSyncHistoryDTO.DeviceName = DeviceName;
                objUserSyncHistoryDTO.RoomID = 0;
                objUserSyncHistoryDTO.SyncByUserID = SyncByUserID;
                objUserSyncHistoryDTO.SyncStep = "After Sync call started";
                objUserSyncHistoryDTO.SyncTime = DateTime.UtcNow;
                objUserSyncHistoryDTO.SyncTransactionID = SyncTransactionID;
                objUserSyncHistoryDTO.ErrorDescription = string.Empty;

                eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();


                UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(objUserSyncHistoryDTO.SyncByUserID);
                if (objDTO != null)
                {
                    CommonDAL objCommonDAL = new CommonDAL(objDTO.EnterpriseDbName);
                    objCommonDAL.InsertUserSyncHist(objUserSyncHistoryDTO);
                }
                retSRT = "ok";
            }
            catch (Exception)
            {
                retSRT = "fail";
            }
            return retSRT;
        }

        [HttpGet]
        public string SyncComplete(Int64 UserID, Int64 CompanyId, string BuildNo = "", string DeviceName = "", Guid? SyncTransactionID = null)
        {
            string errmailbody = string.Empty;
            string errormailto = "niraj_dave@silvertouch.com,chirag.patel@silvertouch.com,rajnikant.bhadania@silvertouch.com,hardik.dave@silvertouch.com";
            eMailMasterDAL objUtils = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
            string resultMsg = "ERROR";
            ErrorLogFile("Sync Complete Method start: " + DateTime.Now.ToLongTimeString());
            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            //ErrorLogFile("Set SessionHelper Start : " + DateTime.Now.ToLongTimeString());
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            UserSyncHistoryDTO objUserSyncHistoryDTO = new UserSyncHistoryDTO();
            if (objDTO != null)
            {

                objUserSyncHistoryDTO.BuildNo = BuildNo;
                objUserSyncHistoryDTO.CompanyID = CompanyId;
                objUserSyncHistoryDTO.DeviceName = DeviceName;
                objUserSyncHistoryDTO.ID = 0;
                objUserSyncHistoryDTO.RoomID = 0;
                objUserSyncHistoryDTO.SyncByUserID = UserID;
                objUserSyncHistoryDTO.SyncStep = "After Sync call started";
                objUserSyncHistoryDTO.SyncTime = DateTime.UtcNow;
                objUserSyncHistoryDTO.SyncTransactionID = SyncTransactionID ?? Guid.Empty;
                objUserSyncHistoryDTO.ErrorDescription = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(objDTO.EnterpriseDbName);
                objCommonDAL.InsertUserSyncHist(objUserSyncHistoryDTO);

                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

            }
            //ErrorLogFile("Set SessionHelper End : " + DateTime.Now.ToLongTimeString());

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);

            #region Delete Duplicate
            try
            {
                if (objDTO != null)
                {
                    objCommon.DeleteDuplicateRecords(SessionHelper.CompanyID, UserID);
                }
            }
            catch (Exception ex)
            {

                errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, objDTO.Room ?? 0, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, SessionHelper.RoomName, "DeleteDuplicateRecords", BuildNo, DeviceName, SyncTransactionID);
                //objUtils.eMailToSend(errormailto, errormailto, "After Sync call issue on step:DeleteDuplicateRecords", mailbody, objDTO.EnterpriseId, objDTO.CompanyID, objDTO.Room ?? 0, objDTO.ID, null, string.Empty);
                ErrorLogFile(resultMsg + " in Delete Duplication.");
                //return resultMsg + " in  Delete Duplication.";
            }
            #endregion

            if (objDTO != null)
            {
                foreach (var item in SessionHelper.RoomList)
                {

                    #region [Item Location Recalculation]

                    try
                    {
                        objCommon.ItemLocationDetialsRecalcAfterSynch(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "ItemLocationDetialsRecalcAfterSynch", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in ItemLocationDetialsRecalcAfterSynch.");
                        //return resultMsg + " in  ItemLocationDetialsRecalcAfterSynch.";
                    }

                    #endregion

                    #region UpdateOnHandQtyAfterSynch
                    try
                    {
                        objCommon.UpdateOnHandQtyAfterSynch(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "UpdateOnHandQtyAfterSynch", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in UpdateOnHandQtyAfterSynch.");
                        //return resultMsg + " in UpdateOnHandQtyAfterSynch.";
                    }
                    #endregion

                    #region [Pull details and Pull Master Running Decreasing Onhand]

                    try
                    {
                        objCommon.PullMasterDetailsRecalcAfterSynch(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "PullMasterDetailsRecalcAfterSynch", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in Pull details and Pull Master Running Decreasing Onhand.");
                        //return resultMsg + " Pull details and Pull Master Running Decreasing Onhand.";
                    }

                    #endregion

                    #region UpdateConsumeModuleDataAfterSynch
                    try
                    {
                        objCommon.UpdateConsumeModuleDataAfterSynch(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "UpdateConsumeModuleDataAfterSynch", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in UpdateConsumeModuleDataAfterSynch.");
                        //return resultMsg + " in UpdateConsumeModuleDataAfterSynch.";
                    }
                    #endregion

                    #region UpdateToolAssetDataAfterSynch
                    try
                    {
                        objCommon.UpdateToolAssetDataAfterSynch(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "UpdateToolAssetDataAfterSynch", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in UpdateToolAssetDataAfterSynch.");
                        //return resultMsg + " in UpdateToolAssetDataAfterSynch.";
                    }
                    #endregion

                    #region [cart generation logic]
                    try
                    {

                        long SessionUserId = SessionHelper.UserID;
                        new CartItemDAL(SessionHelper.EnterPriseDBName).SuggestedOrderRoom(item.ID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionUserId, true);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "SuggestedOrderRoom", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in suggsted Order generation.");
                        //return resultMsg + " in after sync.";
                    }
                    #endregion

                    #region Send PDA Pending Emails After Sync
                    try
                    {
                        SendPDAPendingEmailsAfterSync(UserID, CompanyId, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "SendPDAPendingEmailsAfterSync", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in Send PDA Pending Emails After Sync.");
                        //return resultMsg + " in Send PDA Pending Emails After Sync.";
                    }
                    #endregion

                    #region Submit and approve transfer after sync
                    try
                    {
                        SubmitAndApproveTransferAfterSync(item.ID);
                        //CloseReplenishTransferAfterSync();
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "SubmitAndApproveTransferAfterSync", BuildNo, DeviceName, SyncTransactionID);
                    }
                    #endregion

                    #region UpdatePDATransactionQtyAfterSynch
                    //try
                    //{
                    //    objCommon.UpdatePDATransactionQtyAfterSynch(SessionHelper.CompanyID, SessionHelper.RoomID);
                    //}
                    //catch (Exception)
                    //{
                    //}
                    #endregion

                    #region [Update Pull billing flag true of Consigned Items for Immediate Schedule]

                    try
                    {
                        objCommon.UpdateConsignedPullBilling(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "UpdateConsignedPullBilling", BuildNo, DeviceName, SyncTransactionID);
                        ErrorLogFile(resultMsg + " in UpdateConsignedPullBilling.");
                        //return resultMsg + " in  UpdateConsignedPullBilling.";
                    }

                    #endregion

                    #region Transmit approved orders after sync
                    try
                    {
                        objCommon.TransmitApprovedOrderAfterSync(SessionHelper.CompanyID, item.ID);
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "TransmitApprovedOrderAfterSync", BuildNo, DeviceName, SyncTransactionID);
                    }
                    #endregion

                    #region "SendMailForDiscrepancy"
                    try
                    {
                        SendMailForDiscrepancy();
                    }
                    catch (Exception ex)
                    {
                        errmailbody += GetErrorBody(ex, objDTO.ID, objDTO.EnterpriseId, objDTO.CompanyID, item.ID, objDTO.UserName, objDTO.EnterpriseName, objDTO.CompanyName, item.RoomName, "SendMailForDiscrepancy", BuildNo, DeviceName, SyncTransactionID);

                    }

                    #endregion

                }
                objUserSyncHistoryDTO = new UserSyncHistoryDTO();
                objUserSyncHistoryDTO.BuildNo = BuildNo;
                objUserSyncHistoryDTO.CompanyID = CompanyId;
                objUserSyncHistoryDTO.DeviceName = DeviceName;
                objUserSyncHistoryDTO.ID = 0;
                objUserSyncHistoryDTO.RoomID = 0;
                objUserSyncHistoryDTO.SyncByUserID = UserID;
                objUserSyncHistoryDTO.SyncStep = "After Sync call Completed";
                objUserSyncHistoryDTO.SyncTime = DateTime.UtcNow;
                objUserSyncHistoryDTO.SyncTransactionID = SyncTransactionID ?? Guid.Empty;
                objUserSyncHistoryDTO.ErrorDescription = string.Empty;
                CommonDAL objCommonDAL1 = new CommonDAL(objDTO.EnterpriseDbName);
                objCommonDAL1.InsertUserSyncHist(objUserSyncHistoryDTO);
                ErrorLogFile("Sync Complete Method End: " + DateTime.Now.ToLongTimeString());
                if (!string.IsNullOrWhiteSpace(errmailbody))
                {
                    string tablHtml = "<html><head><style>table { border-left: 1px solid #000000; border-bottom: 1px solid #000000; } table td{ border-right: 1px solid #000000; border-top: 1px solid #000000; } table th{ border-right: 1px solid #000000; border-top: 1px solid #000000; }</style></head><body><table style='width:100%;' cellpadding='1' cellspacing='1'><thead><tr><th><strong>ID</strong></th><th><strong>SyncTransactionID</strong></th><th><strong>UserName</strong></th><th><strong>SyncTime</strong></th><th><strong>SyncStep</strong></th><th><strong>BuildNo</strong></th><th><strong>CompanyName</strong></th><th><strong>DeviceName</strong></th><th><strong>Error Description</strong></th></tr> </thead><tbody>[##TRS##]</tbody></table></body></html>";
                    tablHtml = tablHtml.Replace("[##TRS##]", errmailbody);
                    objUtils.eMailToSend(errormailto, errormailto, ResCommon.SyncCompleteMailSubject, errmailbody, objDTO.EnterpriseId, objDTO.CompanyID, objDTO.Room ?? 0, objDTO.ID, null, string.Empty);
                }
            }
            return "OK";
        }

        public void SendMailForDiscrepancy()
        {
            Helper.AlertMail objAlertMail = new Helper.AlertMail();

            eMailMasterDAL objEmailDAL = null;

            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            NotificationDAL objNotificationDAL = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            try
            {


                objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.DiscrepancyAfterSync, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
                lstNotifications.ForEach(t =>
                {
                    if (t.SchedulerParams.ScheduleMode == 5)
                    {
                        lstNotificationsImidiate.Add(t);
                    }
                });

                if (lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate.ForEach(t =>
                    {
                        string StrSubject = string.Empty;
                        if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                        {
                            StrSubject = t.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                        }
                        string strToAddress = t.EmailAddress;
                        string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                            StringBuilder MessageBody = new StringBuilder();
                            objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Any())
                            {
                                objEmailTemplateDetailDTO = t.EmailTemplateDetail.lstEmailTemplateDtls.First();
                            }
                            if (objEmailTemplateDetailDTO != null)
                            {
                                MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                                StrSubject = objEmailTemplateDetailDTO.MailSubject;
                            }
                            else
                            {
                                return;
                            }

                            string strReplText = ResCommon.DiscrepancyAfterSyncMailBody;

                            MessageBody.Replace("@@TABLE@@", strReplText);


                            objeMailAttchList = new List<eMailAttachmentDTO>();


                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                            //Dictionary<string, string> Params = new Dictionary<string, string>();
                            //Params.Add("DataGuids", objOrder.GUID.ToString());
                            objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, null);
                            //if item is found then only mail is send
                            if (objeMailAttchList != null && objeMailAttchList.Count() > 0)
                                objAlertMail.CreateAlertMail(objeMailAttchList, StrSubject, MessageBody.ToString(), strToAddress, t, objEnterpriseDTO);

                        }
                    });
                }



            }
            finally
            {
                //objUtils = null;
                objEmailDAL = null;
                //extUsers = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //splitEmails = null;
                objeMailAttchList = null;
                //objeMailAttch = null;
                //arrAttchament = null;
            }
        }

        #region Send PDA Pending Emails After Sync
        public string SendPDAPendingEmailsAfterSync(Int64 UserID, Int64 CompanyId, Int64 RoomID)
        {
            string resultMsg = "ERROR";

            #region Send PDA Pending Emails After Sync
            try
            {
                PDAEmailsToSendDAL objCommon = new PDAEmailsToSendDAL(SessionHelper.EnterPriseDBName);
                List<PDAEmailsToSendDTO> oPDAEmailsToSendDTOList = objCommon.GetPDAPendingEmails(RoomID, SessionHelper.CompanyID);
                foreach (PDAEmailsToSendDTO item in oPDAEmailsToSendDTOList)
                {
                    if (item.EmailTemplateType == "OrderApproval" || item.EmailTemplateType == "OrderToSupplier")
                    {
                        SetOrderMail(item.RecordGUID, RoomID, SessionHelper.CompanyID, item.EmailTemplateType);
                    }
                    else if (item.EmailTemplateType == "RequisitionApproval")
                    {
                        RequisitionMasterDAL oRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                        RequisitionMasterDTO oRequisitionMasterDTO = oRequisitionMasterDAL.GetRequisitionByGUIDFull(item.RecordGUID);

                        if (oRequisitionMasterDTO.RequisitionStatus == "Submitted")
                        {
                            ConsumeController oConsumeController = new ConsumeController();
                            oConsumeController.SendMailToApprover(oRequisitionMasterDTO, "APPROVED");
                        }
                    }

                    item.IsSent = true;
                    objCommon.Edit(item);
                }
            }
            catch (Exception ex)
            {

                ErrorLogFile(resultMsg + " in Send PDA Pending Emails After Sync.");
                throw ex;
            }
            #endregion

            return "OK";
        }

        public void SetOrderMail(Guid OrderGUID, Int64 RoomID, Int64 CompanyID, string EmailTemplateType)
        {
            OrderController oOrderController = new OrderController();
            OrderMasterDTO objOrderDTO = null;
            OrderMasterDAL objOrdDAL = null;
            List<SupplierMasterDTO> lstSuppliers = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;
            try
            {
                objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                objOrderDTO = objOrdDAL.GetOrderByGuidNormal(OrderGUID);

                if (objOrderDTO.OrderStatus == (int)OrderStatus.Submitted && EmailTemplateType == "OrderApproval")
                {
                    oOrderController.SendMailToApprovalAuthority(objOrderDTO);
                }
                else if (objOrderDTO.OrderStatus == (int)OrderStatus.Approved && EmailTemplateType == "OrderToSupplier")
                {
                    objSupplierDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    lstSuppliers = new List<SupplierMasterDTO>();
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(objOrderDTO.Supplier.GetValueOrDefault(0));
                    oOrderController.SendMailToSupplier(objSupplierDTO, objOrderDTO);
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                oOrderController = null;
                objOrderDTO = null;
                objOrdDAL = null;
                lstSuppliers = null;
                objSupplierDTO = null;
                objSupplierDAL = null;
            }
        }

        #endregion

        public static void ErrorLogFile(string objEx)
        {
            try
            {
                string LOG_FOLDER_NAME = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
                string FilePath = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog\\Log.txt";
                const long MaxFileLength = 1024 * 1024;
                bool isAppend = true;
                FileInfo objFileInfo = new FileInfo(FilePath);
                if (objFileInfo.Exists)
                    isAppend = objFileInfo.Length < MaxFileLength;

                StreamWriter sw = null;
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog");

                if (!System.IO.File.Exists(FilePath))
                {
                    FileStream fw = System.IO.File.Create(FilePath);
                    fw.Flush();
                    fw.Close();

                    sw = new StreamWriter(FilePath, true);
                }
                if (sw == null)
                    sw = new StreamWriter(FilePath, isAppend);

                //sw.WriteLine(Environment.NewLine);
                //sw.WriteLine("[TimeStamp :" + DateTimeUtility.DateTimeNow.ToString() + " ]");
                sw.WriteLine("[Log :" + objEx.ToString() + " ]");
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                //ErrorLog(ex1);
            }
        }

        #region Submit and approve transfer after sync

        private void SubmitAndApproveTransferAfterSync(long RoomId)
        {
            TransferMasterDAL oTransferMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            List<TransferMasterDTO> oTransferMasterDTOList = oTransferMasterDAL.GetTransfersByRoomTrfTypeAndStatusPlain(RoomId, SessionHelper.CompanyID, (int)RequestType.In, (int)TransferStatus.Submitted);

            foreach (TransferMasterDTO item in oTransferMasterDTOList)
            {
                item.TransferStatus = (int)TransferStatus.Transmitted;
                bool ReturnVal = oTransferMasterDAL.Edit(item);
            }
        }

        private void CloseReplenishTransferAfterSync()
        {
            TransferMasterDAL oTransferMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
            List<TransferMasterDTO> oTransferMasterDTOList = new List<TransferMasterDTO>(); //oTransferMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, (int)RequestType.In, (int)TransferStatus.Closed).ToList();

            //foreach (TransferMasterDTO oTransferMasterDTOIn in oTransferMasterDTOList)
            //{
            //    TransferMasterDTO oTransferMasterDTOOut = null;
            //    if (oTransferMasterDTOIn.RefTransferGUID.HasValue && oTransferMasterDTOIn.RefTransferGUID != Guid.Empty)
            //        oTransferMasterDTOOut = oTransferMasterDAL.GetRecord(oTransferMasterDTOIn.RefTransferGUID.Value, oTransferMasterDTOIn.ReplenishingRoomID, SessionHelper.CompanyID);
            //    else
            //        oTransferMasterDTOOut = oTransferMasterDAL.GetRecordByRefTransferID(oTransferMasterDTOIn.GUID, oTransferMasterDTOIn.ReplenishingRoomID, SessionHelper.CompanyID);

            //    if (oTransferMasterDTOOut != null && oTransferMasterDTOOut.TransferStatus != (int)TransferStatus.Closed)
            //    {



            //    }
            //}
        }

        #endregion

        [HttpGet]
        public DateTime GetServerUTCDate()
        {
            return DateTime.UtcNow;
        }

        [HttpGet]
        public Int64 InventoryLocationCreateMobile(string LocationName, Guid? MaterialStagingGUID, Guid? ItemGUID, bool? IsDefault, double? SuggestedOrderQuantity, double? CriticalQuantity, double? MinimumQuantity, double? MaximunmQuantity, bool isStaggingHeader, bool isStagging, Int64 UserID, Int64 RoomID, Int64 CompanyId, long? ParentBinId)
        {

            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {

                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);


                //Create NEw Inventory location Entry



                BinMasterDAL objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBINDTO = new BinMasterDTO();
                ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

                iResult = objBin.GetOrInsertBinForPDA(LocationName, MaterialStagingGUID, ItemGUID, IsDefault, SuggestedOrderQuantity, CriticalQuantity, MinimumQuantity, MaximunmQuantity, isStaggingHeader, isStagging, UserID, RoomID, CompanyId, ParentBinId);


            }

            return iResult;

        }

        [HttpGet]
        public Int64 ManufacturerMasterCreateMobile(string Manufacturer, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                ManufacturerMasterDAL oManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                ManufacturerMasterDTO oManufacturerMasterDTO = new ManufacturerMasterDTO();

                oManufacturerMasterDTO = oManufacturerMasterDAL.GetManufacturerByNameNormal(Manufacturer, RoomID, CompanyId, isForBOM);

                if (oManufacturerMasterDTO != null && oManufacturerMasterDTO.ID > 0)
                    return oManufacturerMasterDTO.ID;
                else
                    oManufacturerMasterDTO = new ManufacturerMasterDTO();

                oManufacturerMasterDTO.Manufacturer = Manufacturer;
                oManufacturerMasterDTO.isForBOM = isForBOM;
                oManufacturerMasterDTO.IsArchived = false;
                oManufacturerMasterDTO.IsDeleted = false;
                oManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                oManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                oManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                oManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                oManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oManufacturerMasterDTO.Room = RoomID;
                oManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                oManufacturerMasterDTO.GUID = new Guid();
                oManufacturerMasterDTO.RoomName = SessionHelper.RoomName;
                oManufacturerMasterDTO.RefBomId = null;
                oManufacturerMasterDTO.AddedFrom = "Web";
                oManufacturerMasterDTO.EditedFrom = "Web";
                oManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                oManufacturerMasterDTO.ID = oManufacturerMasterDAL.Insert(oManufacturerMasterDTO);

                iResult = oManufacturerMasterDTO.ID;
            }
            return iResult;
        }

        [HttpGet]
        public Int64 SupplierMasterCreateMobile(string SupplierName, bool IsEmailPOInBody, bool IsEmailPOInPDF, bool IsEmailPOInCSV, bool IsEmailPOInX12, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                SupplierMasterDAL oSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                SupplierMasterDTO oSupplierMasterDTO = new SupplierMasterDTO();

                oSupplierMasterDTO = oSupplierMasterDAL.GetSupplierByNamePlain(RoomID, CompanyId, isForBOM, SupplierName);

                if (oSupplierMasterDTO != null && oSupplierMasterDTO.ID > 0)
                    return oSupplierMasterDTO.ID;
                else
                    oSupplierMasterDTO = new SupplierMasterDTO();

                oSupplierMasterDTO.SupplierName = SupplierName;
                oSupplierMasterDTO.IsEmailPOInBody = IsEmailPOInBody;
                oSupplierMasterDTO.IsEmailPOInPDF = IsEmailPOInPDF;
                oSupplierMasterDTO.IsEmailPOInCSV = IsEmailPOInCSV;
                oSupplierMasterDTO.IsEmailPOInX12 = IsEmailPOInX12;
                oSupplierMasterDTO.IsSendtoVendor = false;
                oSupplierMasterDTO.IsVendorReturnAsn = false;
                oSupplierMasterDTO.IsSupplierReceivesKitComponents = false;
                oSupplierMasterDTO.isForBOM = isForBOM;
                oSupplierMasterDTO.GUID = new Guid();
                oSupplierMasterDTO.IsArchived = false;
                oSupplierMasterDTO.IsDeleted = false;
                oSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                oSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                oSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                oSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                oSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oSupplierMasterDTO.Room = RoomID;
                oSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                oSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                oSupplierMasterDTO.RefBomId = null;
                oSupplierMasterDTO.AddedFrom = "Web";
                oSupplierMasterDTO.EditedFrom = "Web";
                oSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                oSupplierMasterDTO.ID = oSupplierMasterDAL.Insert(oSupplierMasterDTO);

                iResult = oSupplierMasterDTO.ID;
            }
            return iResult;
        }

        [HttpGet]
        public Int64 UnitMasterCreateMobile(string Unit, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                UnitMasterDAL oUnitMasterDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
                UnitMasterDTO oUnitMasterDTO = new UnitMasterDTO();

                oUnitMasterDTO = oUnitMasterDAL.GetUnitByNamePlain(RoomID, CompanyId, isForBOM, Unit);

                if (oUnitMasterDTO != null && oUnitMasterDTO.ID > 0)
                    return oUnitMasterDTO.ID;
                else
                    oUnitMasterDTO = new UnitMasterDTO();

                oUnitMasterDTO.Unit = Unit;
                oUnitMasterDTO.isForBOM = isForBOM;
                oUnitMasterDTO.GUID = new Guid();
                oUnitMasterDTO.IsArchived = false;
                oUnitMasterDTO.IsDeleted = false;
                oUnitMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                oUnitMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oUnitMasterDTO.UpdatedByName = SessionHelper.UserName;
                oUnitMasterDTO.CreatedByName = SessionHelper.UserName;
                oUnitMasterDTO.CreatedBy = SessionHelper.UserID;
                oUnitMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oUnitMasterDTO.Room = RoomID;
                oUnitMasterDTO.CompanyID = SessionHelper.CompanyID;
                oUnitMasterDTO.RoomName = SessionHelper.RoomName;
                oUnitMasterDTO.AddedFrom = "Web";
                oUnitMasterDTO.EditedFrom = "Web";
                oUnitMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oUnitMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                oUnitMasterDTO.ID = oUnitMasterDAL.Insert(oUnitMasterDTO);

                iResult = oUnitMasterDTO.ID;
            }
            return iResult;
        }

        [HttpGet]
        public Int64 CostUOMMasterCreateMobile(string CostUOM, int CostUOMValue, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                CostUOMMasterDAL oCostUOMMasterDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDTO();

                oCostUOMMasterDTO = oCostUOMMasterDAL.GetCostUOMByName(CostUOM, RoomID, CompanyId);

                if (oCostUOMMasterDTO != null && oCostUOMMasterDTO.ID > 0)
                    return oCostUOMMasterDTO.ID;
                else
                    oCostUOMMasterDTO = new CostUOMMasterDTO();

                oCostUOMMasterDTO.CostUOM = CostUOM;
                oCostUOMMasterDTO.CostUOMValue = CostUOMValue;
                oCostUOMMasterDTO.GUID = new Guid();
                oCostUOMMasterDTO.IsArchived = false;
                oCostUOMMasterDTO.IsDeleted = false;
                oCostUOMMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                oCostUOMMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oCostUOMMasterDTO.UpdatedByName = SessionHelper.UserName;
                oCostUOMMasterDTO.CreatedByName = SessionHelper.UserName;
                oCostUOMMasterDTO.CreatedBy = SessionHelper.UserID;
                oCostUOMMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oCostUOMMasterDTO.Room = RoomID;
                oCostUOMMasterDTO.CompanyID = SessionHelper.CompanyID;
                oCostUOMMasterDTO.RoomName = SessionHelper.RoomName;
                oCostUOMMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oCostUOMMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                oCostUOMMasterDTO.AddedFrom = "PDA";
                oCostUOMMasterDTO.EditedFrom = "PDA";
                oCostUOMMasterDTO.ID = oCostUOMMasterDAL.Insert(oCostUOMMasterDTO);
                oCostUOMMasterDTO.isForBOM = isForBOM;
                iResult = oCostUOMMasterDTO.ID;
            }
            return iResult;
        }

        [HttpGet]
        public Int64 CategoryMasterCreateMobile(string Category, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                CategoryMasterDAL oCategoryMasterDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                CategoryMasterDTO oCategoryMasterDTO = new CategoryMasterDTO();

                if (isForBOM)
                {
                    oCategoryMasterDTO = oCategoryMasterDAL.GetSingleCategoryByNameByCompanyIDBOM(Category, SessionHelper.CompanyID);
                }
                else
                {
                    oCategoryMasterDTO = oCategoryMasterDAL.GetSingleCategoryByNameByRoomID(Category, SessionHelper.RoomID, SessionHelper.CompanyID);
                }


                if (oCategoryMasterDTO != null && oCategoryMasterDTO.ID > 0)
                    return oCategoryMasterDTO.ID;
                else
                    oCategoryMasterDTO = new CategoryMasterDTO();

                oCategoryMasterDTO.Category = Category;
                oCategoryMasterDTO.isForBOM = isForBOM;
                oCategoryMasterDTO.GUID = new Guid();
                oCategoryMasterDTO.IsArchived = false;
                oCategoryMasterDTO.IsDeleted = false;
                oCategoryMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                oCategoryMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oCategoryMasterDTO.UpdatedByName = SessionHelper.UserName;
                oCategoryMasterDTO.CreatedByName = SessionHelper.UserName;
                oCategoryMasterDTO.CreatedBy = SessionHelper.UserID;
                oCategoryMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oCategoryMasterDTO.Room = RoomID;
                oCategoryMasterDTO.CompanyID = SessionHelper.CompanyID;
                oCategoryMasterDTO.RoomName = SessionHelper.RoomName;
                oCategoryMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oCategoryMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                oCategoryMasterDTO.AddedFrom = "PDA";
                oCategoryMasterDTO.EditedFrom = "PDA";
                oCategoryMasterDTO.ID = oCategoryMasterDAL.Insert(oCategoryMasterDTO);

                iResult = oCategoryMasterDTO.ID;
            }
            return iResult;
        }

        [HttpGet]
        public Int64 InventoryClassificationMasterCreateMobile(string InventoryClassification, string baseOfInventory, double RangeStart, double RangeEnd, bool isForBOM, Int64 UserID, Int64 RoomID, Int64 CompanyId)
        {
            Int64 iResult = 0;

            eTurnsMaster.DAL.EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new eTurnsMaster.DAL.EnterPriseUserMasterDAL();
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseMasterDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.GetUserByIdForServiceNormal(UserID);
            if (objDTO != null)
            {
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserType = objDTO.UserType;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.RoomID = objDTO.Room ?? 0;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.LoggedinUser = objDTO;

                MasterController objMaster = new MasterController();
                objMaster.SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                //Create NEw Entry

                InventoryClassificationMasterDAL oInventoryClassificationMasterDAL = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
                InventoryClassificationMasterDTO oInventoryClassificationMasterDTO = new InventoryClassificationMasterDTO();

                oInventoryClassificationMasterDTO = oInventoryClassificationMasterDAL.GetInventoryClassificationByNamePlain(RoomID, CompanyId, isForBOM, InventoryClassification);

                if (oInventoryClassificationMasterDTO != null && oInventoryClassificationMasterDTO.ID > 0)
                    return oInventoryClassificationMasterDTO.ID;
                else
                    oInventoryClassificationMasterDTO = new InventoryClassificationMasterDTO();

                oInventoryClassificationMasterDTO.InventoryClassification = InventoryClassification;
                oInventoryClassificationMasterDTO.BaseOfInventory = baseOfInventory;
                oInventoryClassificationMasterDTO.RangeStart = RangeStart;
                oInventoryClassificationMasterDTO.RangeEnd = RangeEnd;
                oInventoryClassificationMasterDTO.isForBOM = isForBOM;
                oInventoryClassificationMasterDTO.GUID = new Guid();
                oInventoryClassificationMasterDTO.IsArchived = false;
                oInventoryClassificationMasterDTO.IsDeleted = false;
                oInventoryClassificationMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                oInventoryClassificationMasterDTO.Created = DateTimeUtility.DateTimeNow;
                oInventoryClassificationMasterDTO.UpdatedByName = SessionHelper.UserName;
                oInventoryClassificationMasterDTO.CreatedByName = SessionHelper.UserName;
                oInventoryClassificationMasterDTO.CreatedBy = SessionHelper.UserID;
                oInventoryClassificationMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                oInventoryClassificationMasterDTO.Room = RoomID;
                oInventoryClassificationMasterDTO.CompanyID = SessionHelper.CompanyID;
                oInventoryClassificationMasterDTO.RoomName = SessionHelper.RoomName;

                oInventoryClassificationMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                oInventoryClassificationMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                oInventoryClassificationMasterDTO.AddedFrom = "PDA";
                oInventoryClassificationMasterDTO.EditedFrom = "PDA";

                oInventoryClassificationMasterDTO.ID = oInventoryClassificationMasterDAL.Insert(oInventoryClassificationMasterDTO);

                iResult = oInventoryClassificationMasterDTO.ID;
            }
            return iResult;
        }

        public void SendDiscrepencyReport(long CompanyID, long UserID, long roomid)
        {
            //eTurnsUtility objUtils = null;
            eMailMasterDAL objEmailDAL = null;
            //IEnumerable<EmailUserConfigurationDTO> extUsers = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //string[] splitEmails = null;
            List<eMailAttachmentDTO> objeMailAttchList = null;
            eMailAttachmentDTO objeMailAttch = null;
            //ArrayList arrAttchament = null;

            try
            {
                string StrSubject = ResOrder.OrderToSupplierMailSubject;// "Order Approval Request";;// "Order Approval Request";
                string strToAddress = ConfigurationManager.AppSettings["OverrideToEmail"];
                string strBCCAddress = ConfigurationManager.AppSettings["BCCAddress"];
                string strTemplateName = "OrderToSupplier";
                strToAddress = "niraj_dave@silvertouch.com";


                if (!string.IsNullOrEmpty(strToAddress))
                {
                    objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                    string strCCAddress = "";
                    StringBuilder MessageBody = new StringBuilder();
                    objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, ResourceHelper.CurrentCult.ToString(), roomid, CompanyID);
                    if (objEmailTemplateDetailDTO != null)
                    {
                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                    }
                    else
                    {
                        return;
                    }

                    string urlPart = Request.Url.ToString();
                    string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];


                    objeMailAttchList = new List<eMailAttachmentDTO>();

                    try
                    {
                        objeMailAttch = new eMailAttachmentDTO();
                        string fileName = "DiscrepencyReport.pdf";
                        objeMailAttch.FileData = GetPDFStreamToAttachInMail(string.Empty, null, roomid.ToString(), CompanyID.ToString());
                        objeMailAttch.eMailToSendID = 0;
                        objeMailAttch.MimeType = "application/pdf";
                        objeMailAttch.AttachedFileName = fileName;
                        objeMailAttchList.Add(objeMailAttch);
                    }
                    catch (Exception ex)
                    {
                        objEmailDAL.eMailToSend(strBCCAddress, "", " " + ResCommon.DiscrepancyReportFailSubject + " " + StrSubject, MessageBody.ToString() + "<br/>" + ex.ToString(), SessionHelper.EnterPriceID, CompanyID, roomid, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier => Error During Create PDF ");
                        MessageBody.AppendLine(" ");
                        MessageBody.AppendLine(" " + ResCommon.DiscrepancyReportFailBodyNote);
                    }





                    MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);

                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, CompanyID, roomid, SessionHelper.UserID, objeMailAttchList, "Web => Order => OrderToSupplier");
                }
            }
            finally
            {
                //objUtils = null;
                objEmailDAL = null;
                //extUsers = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //splitEmails = null;
                objeMailAttchList = null;
                objeMailAttch = null;
                //arrAttchament = null;
            }
        }

        private byte[] GetPDFStreamToAttachInMail(string strHTML, string DataGuids, string RoomIds, string CompanyIds)
        {
            ReportMasterDAL objReportMasterDAL = null;
            ReportBuilderDTO objRPTDTO = null;
            ReportBuilderController objRPTCTRL = null;
            MasterController objMSTCTRL = null;
            KeyValDTO objKeyVal = null;
            List<KeyValDTO> objKeyValList = null;
            JavaScriptSerializer objJSSerial = null;
            byte[] fileBytes = null;
            JsonResult objJSON = null;
            MemoryStream fs = null;
            StringReader sr = null;
            iTextSharp.text.html.simpleparser.HTMLWorker hw = null;
            iTextSharp.text.Document doc = null;

            try
            {

                objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport == true && x.ModuleName == "Item-Discrepency" && x.ReportType == 3 && x.ReportName == "Discrepancy Report");
                if (objRPTDTO != null)
                {
                    objKeyValList = new List<KeyValDTO>();

                    objKeyVal = new KeyValDTO() { key = "DataGuids", value = DataGuids };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "CompanyIDs", value = CompanyIds };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "RoomIDs", value = RoomIds };
                    objKeyValList.Add(objKeyVal);

                    objMSTCTRL = new MasterController();
                    objMSTCTRL.SetPDFReportParaDictionary(objKeyValList, objRPTDTO.ID.ToString(), null);

                    objRPTCTRL = new ReportBuilderController();
                    objJSON = objRPTCTRL.GenerateBytesFromReport(objRPTDTO.ID, "PDF");
                    objJSON.MaxJsonLength = int.MaxValue;

                    objJSSerial = new JavaScriptSerializer();
                    var json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objJSON.Data));

                    if (Convert.ToString(json["Message"]) == "ok")
                    {
                        fileBytes = System.IO.File.ReadAllBytes(Convert.ToString(json["FilePath"]));
                        return fileBytes;
                    }
                }

                strHTML = strHTML.Replace("/Content/", Server.MapPath("/Content/"));
                fs = new MemoryStream();
                doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36f, 36f, 36f, 36f);
                iTextSharp.text.pdf.PdfWriter pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                doc.Open();
                hw = new iTextSharp.text.html.simpleparser.HTMLWorker(doc);
                sr = new StringReader(strHTML);
                hw.Parse(sr);
                pdfWriter.CloseStream = false;
                doc.Close();
                pdfWriter.Dispose();
                fs.Position = 0;
                return fs.ToArray();


            }
            finally
            {
                if (hw != null)
                {
                    hw.Dispose();
                    hw = null;
                }

                if (sr != null)
                {
                    sr.Dispose();
                    sr = null;
                }

                if (doc != null)
                {
                    doc.Dispose();
                    doc = null;
                }
                if (fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }

                objReportMasterDAL = null;
                objRPTDTO = null;
                objRPTCTRL = null;
                objMSTCTRL = null;
                objKeyVal = null;
                objKeyValList = null;
                objJSSerial = null;
                fileBytes = null;
                objJSON = null;
            }

        }


        [HttpGet]
        public string CreateRoomWiseUDFResourceFiles()
        {
            string ResourcePath = CommonUtility.ResourceBaseFilePath + @"\";

            if (Directory.Exists(ResourcePath))
            {
                string[] EntDirectotyList = Directory.GetDirectories(ResourcePath);
                foreach (string EntDirectoty in EntDirectotyList)
                {
                    DirectoryInfo dirEnterprise = new DirectoryInfo(EntDirectoty);

                    Int64 EnterpriseID = 0;
                    Int64.TryParse(dirEnterprise.Name, out EnterpriseID);
                    if (EnterpriseID > 0)
                    {
                        string DBConnectionstring = string.Empty;
                        string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID);
                        if (!string.IsNullOrEmpty(EnterpriseDBName))
                        {
                            string[] CompDirectotyList = Directory.GetDirectories(dirEnterprise.FullName);
                            foreach (string CompDirectoty in CompDirectotyList)
                            {
                                DirectoryInfo dirCompany = new DirectoryInfo(CompDirectoty);

                                Int64 CompanyID = 0;
                                Int64.TryParse(dirCompany.Name, out CompanyID);
                                if (CompanyID > 0)
                                {
                                    RoomDAL objRoom = new RoomDAL(EnterpriseDBName);
                                    List<RoomDTO> objList = objRoom.GetRoomByCompanyIDPlain(CompanyID).ToList();
                                    foreach (RoomDTO item in objList)
                                    {
                                        string RoomDirectoty = CompDirectoty + @"\" + item.ID.ToString();
                                        if (!System.IO.Directory.Exists(RoomDirectoty))
                                        {
                                            System.IO.Directory.CreateDirectory(RoomDirectoty);
                                        }

                                        string[] CompResourceFileList = System.IO.Directory.GetFiles(dirCompany.FullName);
                                        foreach (string CompResourceFile in CompResourceFileList)
                                        {
                                            FileInfo CompResFile = new FileInfo(CompResourceFile);
                                            //string destinationPath = CompResourceFile.Replace(@"\" + CompanyID.ToString(), @"\" + CompanyID.ToString() + @"\" + item.ID.ToString());
                                            string destinationPath = RoomDirectoty + @"\" + CompResFile.Name;
                                            if (!System.IO.File.Exists(destinationPath))
                                            {
                                                System.Xml.XmlDocument loResource = new System.Xml.XmlDocument();
                                                loResource.Load(CompResourceFile);

                                                using (System.Resources.ResXResourceWriter resx = new System.Resources.ResXResourceWriter(destinationPath))
                                                {
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF1']/value") != null)
                                                        resx.AddResource("UDF1", loResource.SelectSingleNode("root/data[@name='UDF1']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF2']/value") != null)
                                                        resx.AddResource("UDF2", loResource.SelectSingleNode("root/data[@name='UDF2']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF3']/value") != null)
                                                        resx.AddResource("UDF3", loResource.SelectSingleNode("root/data[@name='UDF3']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF4']/value") != null)
                                                        resx.AddResource("UDF4", loResource.SelectSingleNode("root/data[@name='UDF4']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF5']/value") != null)
                                                        resx.AddResource("UDF5", loResource.SelectSingleNode("root/data[@name='UDF5']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF6']/value") != null)
                                                        resx.AddResource("UDF6", loResource.SelectSingleNode("root/data[@name='UDF6']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF7']/value") != null)
                                                        resx.AddResource("UDF7", loResource.SelectSingleNode("root/data[@name='UDF7']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF8']/value") != null)
                                                        resx.AddResource("UDF8", loResource.SelectSingleNode("root/data[@name='UDF8']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF9']/value") != null)
                                                        resx.AddResource("UDF9", loResource.SelectSingleNode("root/data[@name='UDF9']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF10']/value") != null)
                                                        resx.AddResource("UDF10", loResource.SelectSingleNode("root/data[@name='UDF10']/value").InnerText);
                                                }
                                            }
                                        }

                                        string CompanyResourcesPath = ResourcePath + @"\" + EnterpriseID.ToString() + @"\CompanyResources";
                                        CompResourceFileList = System.IO.Directory.GetFiles(CompanyResourcesPath);
                                        foreach (string CompResourceFile in CompResourceFileList)
                                        {
                                            FileInfo CompResFile = new FileInfo(CompResourceFile);
                                            //string destinationPath = CompResourceFile.Replace(@"\CompanyResources", @"\" + CompanyID.ToString() + @"\" + item.ID.ToString());
                                            string destinationPath = RoomDirectoty + @"\" + CompResFile.Name;
                                            if (!System.IO.File.Exists(destinationPath))
                                            {
                                                System.Xml.XmlDocument loResource = new System.Xml.XmlDocument();
                                                loResource.Load(CompResourceFile);

                                                using (System.Resources.ResXResourceWriter resx = new System.Resources.ResXResourceWriter(destinationPath))
                                                {
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF1']/value") != null)
                                                        resx.AddResource("UDF1", loResource.SelectSingleNode("root/data[@name='UDF1']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF2']/value") != null)
                                                        resx.AddResource("UDF2", loResource.SelectSingleNode("root/data[@name='UDF2']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF3']/value") != null)
                                                        resx.AddResource("UDF3", loResource.SelectSingleNode("root/data[@name='UDF3']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF4']/value") != null)
                                                        resx.AddResource("UDF4", loResource.SelectSingleNode("root/data[@name='UDF4']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF5']/value") != null)
                                                        resx.AddResource("UDF5", loResource.SelectSingleNode("root/data[@name='UDF5']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF6']/value") != null)
                                                        resx.AddResource("UDF6", loResource.SelectSingleNode("root/data[@name='UDF6']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF7']/value") != null)
                                                        resx.AddResource("UDF7", loResource.SelectSingleNode("root/data[@name='UDF7']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF8']/value") != null)
                                                        resx.AddResource("UDF8", loResource.SelectSingleNode("root/data[@name='UDF8']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF9']/value") != null)
                                                        resx.AddResource("UDF9", loResource.SelectSingleNode("root/data[@name='UDF9']/value").InnerText);
                                                    if (loResource.SelectSingleNode("root/data[@name='UDF10']/value") != null)
                                                        resx.AddResource("UDF10", loResource.SelectSingleNode("root/data[@name='UDF10']/value").InnerText);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return "OK";
        }

        //Exception ex, long UserID, long EnterpriseID, string UserName, string CompanyName, string EnterpriseName, Int64 CompanyId, Int64 RoomID, string RoomName, string BuildNo = "", string DeviceName = "", Guid? SyncTransactionID = null
        private string GetErrorBody(Exception ex, long UserID, long EnterpriseID, long CompanyId, long RoomID, string UserName, string EnterpriseName, string CompanyName, string RoomName, string MethodName, string BuildNo = "", string DeviceName = "", Guid? SyncTransactionID = null)
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("<tr><td>MethodName:" + (MethodName ?? "NA").ToString() + "</td></tr>");
            sb.Append("<tr><td>");
            sb.Append("<table style='width:100%;'><tbody>");
            sb.Append("<tr>EnterpriseID:" + (EnterpriseID).ToString() + "</tr>");
            sb.Append("<tr>CompanyId:" + (CompanyId).ToString() + "</tr>");
            sb.Append("<tr>RoomID:" + (RoomID).ToString() + "</tr>");
            sb.Append("<tr>EnterpriseName:" + (EnterpriseName ?? "NA") + "</tr>");
            sb.Append("<tr>CompanyName:" + (CompanyName ?? "NA") + "</tr>");
            sb.Append("<tr>RoomName:" + (RoomName ?? "NA") + "</tr>");
            sb.Append("<tr>BuildNo:" + (BuildNo ?? "NA") + "</tr>");
            sb.Append("<tr>DeviceName:" + (DeviceName ?? "NA") + "</tr>");
            sb.Append("<tr>SyncTransactionID:" + (SyncTransactionID ?? Guid.Empty).ToString() + "</tr>");
            sb.Append("<tr>Exception:" + (Convert.ToString(ex)) + "</tr>");
            sb.Append("</tbody></table>");
            sb.Append("</td></tr>");
            return sb.ToString();
        }

        [HttpGet]
        public string eVMiItemProcess(string Items)
        {
            string retSRT = "fail";
            try
            {
                // Here put logic of Signalr, for Item grid refresh 
                retSRT = "ok";
            }
            catch (Exception ex)
            {
                retSRT = Convert.ToString(ex);
            }
            return retSRT;
        }

        [HttpGet]
        public void InvalidateUserUISettings(string CacheKey)
        {
            CacheHelper<IEnumerable<UsersUISettingsDTO>>.InvalidateUserUISettingsCacheByKey(CacheKey);
        }

    }
}