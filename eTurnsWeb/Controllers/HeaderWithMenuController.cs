using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class HeaderWithMenuController : eTurnsControllerBase
    {
        //
        // GET: /SiteHeaderWithMenu/


        public ActionResult Index()
        {
            HeaderWithMenuModel viewModel = new HeaderWithMenuModel();
            SetModulePermission(viewModel);

            return View(viewModel);
        }
        public ActionResult GetModulePermission()
        {
            HeaderWithMenuModel viewModel = new HeaderWithMenuModel();
            SetModulePermission(viewModel);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        //private void SetMasterMenu(HeaderWithMenuModel Model)
        //{
        //    Model.MasterMenu = new List<TwoLevelMenu>();
        //    if (Model.isAssetCategory == true)
        //    {
        //        TwoLevelMenu assetMenu = new TwoLevelMenu() {
        //            SortOrder = 1,
        //            Menu = new MenuLink()
        //            {
        //                LinkText = eTurns.DTO.Resources.ResLayout.AssetCategory,
        //                ActionName = "AssetCategoryList",
        //                ControllerName = "Master",
        //                Protocol = "",
        //                HostName = "",
        //                Fragment = "",
        //                RouteValues = null,
        //                HtmlAttributes = null,
        //                SortOrder = 0
        //            }
        //        };
        //        assetMenu.SubMenu = new List<MenuLink>();
        //        assetMenu.SubMenu.Add(new MenuLink()
        //        {
        //            LinkText = eTurns.DTO.Resources.ResCommon.mnuList,
        //            ActionName = "AssetCategoryList",
        //            ControllerName = "Master",
        //            Protocol = "",
        //            HostName = "",
        //            Fragment = "list",
        //            RouteValues = new { },
        //            HtmlAttributes = new { },
        //            SortOrder = 1
        //        });

        //        if (Model.isToolCategory_Create)
        //        {
        //            assetMenu.SubMenu.Add(new MenuLink()
        //            {
        //                LinkText = eTurns.DTO.Resources.ResCommon.mnuAdd,
        //                ActionName = "AssetCategoryList",
        //                ControllerName = "Master",
        //                Protocol = "",
        //                HostName = "",
        //                Fragment = "new",
        //                RouteValues = new { },
        //                HtmlAttributes = new { },
        //                SortOrder = 2
        //            });
        //        }       

        //        Model.MasterMenu.Add(assetMenu);
        //    }
        //}

        private void SetModulePermission(HeaderWithMenuModel viewModel)
        {
            // Module permissions
            ModulePermission mpEnterpriseMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster);
            ModulePermission mpCompanyMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster);
            ModulePermission mpRoomMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster);
            ModulePermission mpBinMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster);
            ModulePermission mpFTPMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster);
            ModulePermission mpCategoryMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster);
            ModulePermission mpCostUOMMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster);
            ModulePermission mpInventoryClassificationMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster);
            ModulePermission mpCustomerMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster);
            ModulePermission mpFreightTypeMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster);
            ModulePermission mpGLAccountsMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster);
            ModulePermission mpGXPRConsignedJobMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.GXPRConsignedJobMaster);
            ModulePermission mpJobTypeMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.JobTypeMaster);
            ModulePermission mpProjectMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster);
            ModulePermission mpSupplierMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster);
            ModulePermission mpWrittenOffCategory = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WrittenOffCategory);
            var mpShipViaMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster);
            var mpVenderMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster);
            var mpTechnicianMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster);
            var mpUnitMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster);
            var mpLocationMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster);
            var mpToolCategory = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory);
            var mpAssetCategory = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory);
            var mpManufacturerMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster);
            var mpMeasurementTermMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.MeasurementTermMaster);
            var mpRoleMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.RoleMaster);
            var mpUserMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.UserMaster);
            var mpPermissionTemplates = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.PermissionTemplates);
            var mpResourceMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ResourceMaster);
            var mpItemMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ItemMaster);
            var mpQuickListPermission = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.QuickListPermission);
            var mpCount = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Count);
            var mpSuppliercatalog = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Suppliercatalog);
            var mpMaterialstaging = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Materialstaging);
            var mpPullMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.PullMaster);
            var mpRequisitions = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Requisitions);
            var mpWorkOrders = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WorkOrders);

            var mpOrders = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Orders);
            var mpToolAssetOrder = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ToolAssetOrder);
            var mpReturnOrder = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ReturnOrder);
            var mpCart = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Cart);
            var mpReceive = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Receive);
            var mpTransfer = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Transfer);
            var mpToolMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ToolMaster);
            var mpAssets = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Assets);
            var mpAssetToolScheduler = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetToolScheduler);
            var mpAssetToolSchedulerMapping = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetToolSchedulerMapping);
            var mpAssetMaintenance = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetMaintenance);
            var mpKits = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Kits);
            var mpWIP = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WIP);
            var mpLabelPrinting = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.LabelPrinting);
            var mpReports = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Reports);
            var mpNotifications = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Notifications);
            var mpAllowConsignedCreditPull = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AllowConsignedCreditPull);
            var mpCatalogReport = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.CatalogReport);
            var mpHelpDocumentPermission = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.HelpDocumentPermission);



            //eTurns.DAL.ModuleMasterDAL objModuleMaster = new eTurns.DAL.ModuleMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
            viewModel.UserType = eTurnsWeb.Helper.SessionHelper.UserType;
            viewModel.RoleID = eTurnsWeb.Helper.SessionHelper.RoleID;

            viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;
            if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0
                && eTurnsWeb.Helper.SessionHelper.CompanyList != null && eTurnsWeb.Helper.SessionHelper.CompanyList.Count > 0
                && eTurnsWeb.Helper.SessionHelper.RoomList != null && eTurnsWeb.Helper.SessionHelper.RoomList.Count > 0)
            {
                viewModel.IsShowReportMenu = true;
            }
            else
            {
                viewModel.IsShowReportMenu = false;
            }

            if (eTurnsWeb.Helper.SessionHelper.RoomPermissions != null)
            {
                if (viewModel.UserType == 1 || viewModel.UserType == 2)
                {
                    #region "UserType 1 or 2"
                    if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0)
                    {
                        if (eTurnsWeb.Helper.SessionHelper.CompanyList != null && eTurnsWeb.Helper.SessionHelper.CompanyList.Count > 0)
                        {
                            if (eTurnsWeb.Helper.SessionHelper.RoomID > 0)
                            {

                                //RoomDTO objRoomMasterDTO = new eTurns.DAL.RoomDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                                UpdateViewModel(viewModel);

                                if (eTurnsWeb.Helper.SessionHelper.RoleID == -1 || mpHelpDocumentPermission.IsChecked == true || eTurnsWeb.Helper.SessionHelper.RoleID == -2)
                                {
                                    viewModel.IsHelpDocumentSetUp = true;
                                }
                            }
                            else
                            {
                                if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
                                {
                                    viewModel.isEnterprise = true;
                                }
                                viewModel.isRoom = true;
                                viewModel.isCompany = true;
                                if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
                                {
                                    viewModel.isCompanyConfig = true;
                                }
                                else
                                {
                                    viewModel.isCompanyConfig = false;
                                }
                                //isRole = true;
                            }
                        }
                        else
                        {
                            if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
                            {
                                viewModel.isEnterprise = true;
                            }
                            viewModel.isCompany = true;
                            if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
                            {
                                viewModel.isCompanyConfig = true;
                            }
                            else
                            {
                                viewModel.isCompanyConfig = false;
                            }
                        }
                    }
                    else
                    {
                        if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
                        {
                            viewModel.isEnterprise = true;// eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "UseType else"
                    UpdateViewModel(viewModel);
                    if (eTurnsWeb.Helper.SessionHelper.RoleID == -1 || mpHelpDocumentPermission.IsChecked == true || eTurnsWeb.Helper.SessionHelper.RoleID == -2)
                    {
                        viewModel.IsHelpDocumentSetUp = true;
                    }
                    #endregion
                }
            }

            if (viewModel.isRoom == true || viewModel.isRole == true || viewModel.isResource == true || viewModel.isCompany == true || viewModel.isEnterprise == true
                || viewModel.isUser == true || viewModel.isPermissionTemplate == true)
            {
                viewModel.ShowAuthanticationMenu = true;
            }
            if (viewModel.isFTP == true || viewModel.isBin == true || viewModel.isCategory == true || viewModel.isCostUOM == true
                || viewModel.isInventoryClassification == true || viewModel.isCustomer == true
                || viewModel.isFreightType == true || viewModel.isGLAccounts == true
                || viewModel.isGXPRConsignedJob == true || viewModel.isJobType == true || viewModel.isProject == true
                || viewModel.isSupplier == true || viewModel.isShipVia == true || viewModel.isVendor == true
                || viewModel.isTechnician == true || viewModel.isUnit == true || viewModel.isLocation == true
                || viewModel.isManufacturer == true || viewModel.isMeasurementTerm == true || viewModel.isToolCategory == true
                || viewModel.isAssetCategory == true)
            {
                viewModel.ShowMastersMenu = true;
            }
            if (viewModel.isTool || viewModel.IsAsset || viewModel.IsAssetMaintance || viewModel.IsATScheduler
                || viewModel.IsATSchedulerMapping || viewModel.IsToolAssetOrderList)
            {
                viewModel.ShowAssetsMenu = true;
            }
            if (viewModel.isQuickList || viewModel.isItemList || viewModel.isCount
                || viewModel.isSupplierCatalog || viewModel.isMaterialstaging)
            {
                viewModel.ShowInventrysMenu = true;
            }
            if (viewModel.isPullList || viewModel.isRequisitions || viewModel.isWorkorders || viewModel.isProjectspend)
            {
                viewModel.ShowConsumeMenu = true;
            }
            if (viewModel.IsOrderList || viewModel.IsCart || viewModel.IsReturnOrderList
                || viewModel.IsReceive || viewModel.IsTransfer || viewModel.IsQuoteList)
            {
                viewModel.ShowReplenishMenu = true;
            }
            if (viewModel.IsKits || viewModel.IsWIP)
            {
                viewModel.ShowKitsMenu = true;
                eTurnsWeb.Controllers.KitController objKitController = new eTurnsWeb.Controllers.KitController();
                viewModel.BuildBreakNotification = objKitController.GetWIPKitCountForRedCircle();
                viewModel.TotalKits = viewModel.BuildBreakNotification;
                viewModel.BuildBreakNotification = (string.IsNullOrEmpty(viewModel.BuildBreakNotification) || viewModel.BuildBreakNotification == "0") ? "" : " (" + viewModel.BuildBreakNotification + ")";
            }

            if (viewModel.UserType == 3)
            {
                viewModel.isCompany = false;
                viewModel.isCompanyConfig = false;
                viewModel.isCompany_Create = false;
            }

            try
            {
                if (eTurnsWeb.Helper.SessionHelper.eTurnsRegionInfoProp == null)
                {
                    TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                    DateTime utc = viewModel.currentDate;
                    if (mountain != null)
                    {
                        viewModel.timeZoneTiming = TimeZoneInfo.ConvertTimeFromUtc(utc, mountain);
                        viewModel.timespanObj = mountain.BaseUtcOffset;
                        viewModel.TimeZoneName = "UTC";
                    }

                }
                else
                {
                    eTurnsRegionInfo Region = eTurnsWeb.Helper.SessionHelper.eTurnsRegionInfoProp;

                    TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById(Region.TimeZoneName);
                    DateTime utc = viewModel.currentDate;
                    if (mountain != null)
                    {
                        viewModel.timeZoneTiming = TimeZoneInfo.ConvertTimeFromUtc(utc, mountain);
                        viewModel.TimeZoneName = Region.TimeZoneName;

                        viewModel.timespanObj = mountain.BaseUtcOffset;
                    }

                }
            }
            catch (Exception)
            {
                viewModel.timeZoneTiming = new DateTime();
                viewModel.currentDate = DateTime.UtcNow;
                viewModel.timespanObj = new TimeSpan();
                viewModel.TimeZoneName = string.Empty;
            }

            
            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
            viewModel.ReleaseNumber = eTurns.DTO.SiteSettingHelper.ReleaseNumber; //Settinfile.Element("ReleaseNumber").Value;

            //viewModel.AccessQryUserNames = Settinfile.Element("AccessQryUserNames") != null ? Settinfile.Element("AccessQryUserNames").Value : string.Empty;
            //viewModel.AccessQryRoleIds = Settinfile.Element("AccessQryRoleIds") != null ? Settinfile.Element("AccessQryRoleIds").Value : string.Empty;

            viewModel.AccessQryUserNames = eTurns.DTO.SiteSettingHelper.AccessQryUserNames  != string.Empty ? eTurns.DTO.SiteSettingHelper.AccessQryUserNames : string.Empty;
            viewModel.AccessQryRoleIds = eTurns.DTO.SiteSettingHelper.AccessQryRoleIds != string.Empty ? eTurns.DTO.SiteSettingHelper.AccessQryRoleIds : string.Empty;

            if (!string.IsNullOrWhiteSpace(viewModel.AccessQryUserNames))
            {
                string[] UserNameList = viewModel.AccessQryUserNames.Split('$');
                for (int i = 0; i < UserNameList.Length; i++)
                {
                    if (UserNameList[i] == eTurnsWeb.Helper.SessionHelper.UserName)
                    {
                        viewModel.ShowMenutoThisUser = true;
                        break;
                    }
                }
            }
            if (!viewModel.ShowMenutoThisUser)
            {
                if (!string.IsNullOrWhiteSpace(viewModel.AccessQryRoleIds))
                {
                    string[] RoleIDS = viewModel.AccessQryRoleIds.Split('$');
                    for (int i = 0; i < RoleIDS.Length; i++)
                    {
                        if (Convert.ToInt64(RoleIDS[i]) == eTurnsWeb.Helper.SessionHelper.RoleID)
                        {
                            viewModel.ShowMenutoThisUser = true;
                            break;
                        }
                    }
                }
            }

        }

        private void UpdateViewModel(HeaderWithMenuModel viewModel)
        {
            ModulePermission mpEnterpriseMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster);
            ModulePermission mpCompanyMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster);
            ModulePermission mpRoomMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster);
            ModulePermission mpBinMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster);
            ModulePermission mpFTPMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster);
            ModulePermission mpCategoryMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster);
            ModulePermission mpCostUOMMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster);
            ModulePermission mpInventoryClassificationMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster);
            ModulePermission mpCustomerMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster);
            ModulePermission mpFreightTypeMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster);
            ModulePermission mpGLAccountsMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster);
            ModulePermission mpGXPRConsignedJobMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.GXPRConsignedJobMaster);
            ModulePermission mpJobTypeMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.JobTypeMaster);
            ModulePermission mpProjectMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster);
            ModulePermission mpSupplierMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster);
            ModulePermission mpWrittenOffCategory = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WrittenOffCategory);
            var mpShipViaMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster);
            var mpVenderMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster);
            var mpTechnicianMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster);
            var mpUnitMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster);
            var mpLocationMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster);
            var mpToolCategory = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory);
            var mpAssetCategory = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory);
            var mpManufacturerMaster = SessionHelper.GetModulePermissionByModule(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster);
            var mpMeasurementTermMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.MeasurementTermMaster);
            var mpRoleMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.RoleMaster);
            var mpUserMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.UserMaster);
            var mpPermissionTemplates = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.PermissionTemplates);
            var mpResourceMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ResourceMaster);
            var mpItemMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ItemMaster);
            var mpQuickListPermission = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.QuickListPermission);
            var mpCount = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Count);
            var mpSuppliercatalog = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Suppliercatalog);
            var mpMaterialstaging = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Materialstaging);
            //var mpMoveMaterial = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.MoveMaterial);
            var mpPullMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.PullMaster);
            var mpRequisitions = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Requisitions);
            var mpWorkOrders = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WorkOrders);

            var mpOrders = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Orders);
            var mpToolAssetOrder = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ToolAssetOrder);
            var mpReturnOrder = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ReturnOrder);
            var mpQuote = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Quote);
            var mpCart = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Cart);
            var mpReceive = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Receive);
            var mpTransfer = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Transfer);
            var mpToolMaster = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ToolMaster);
            var mpAssets = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Assets);
            var mpAssetToolScheduler = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetToolScheduler);
            var mpAssetToolSchedulerMapping = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetToolSchedulerMapping);
            var mpAssetMaintenance = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AssetMaintenance);
            var mpKits = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Kits);
            var mpToolKitBuildBreak = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.ToolKitBuildBreak);
            var mpWIP = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.WIP);
            var mpLabelPrinting = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.LabelPrinting);
            var mpReports = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Reports);
            var mpNotifications = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.Notifications);
            var mpAllowConsignedCreditPull = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.AllowConsignedCreditPull);
            var mpQuoteToOrder = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.QuoteToOrder);
            var mpCatalogReport = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.CatalogReport);
            var mpDataArchival = SessionHelper.GetModulePermissionByModule(SessionHelper.ModuleList.DataArchival);
            
            

            if (viewModel.UserType == (int)eTurns.DTO.Enums.UserType.SuperAdmin || viewModel.UserType == (int)eTurns.DTO.Enums.UserType.EnterpriseAdmin)
            {
                viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

                if (viewModel.UserType == (int)eTurns.DTO.Enums.UserType.SuperAdmin)
                {
                    //EnterpriseMaster
                    viewModel.isEnterprise = mpEnterpriseMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                    viewModel.isEnterprise_Create = mpEnterpriseMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                }


                // Consume Menu ends from Here

            }
            else
            {
                //EnterpriseMaster
                viewModel.isEnterprise = false;// eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

                viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

                //CompanyMaster

                if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
                {
                    viewModel.isCompanyConfig = true;
                }
                else
                {
                    viewModel.isCompanyConfig = false;
                }

                if (eTurnsWeb.Helper.SessionHelper.RoomID > 0)
                {
                    RoomDTO objRoomMasterDTO = new eTurns.DAL.RoomDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                    if (objRoomMasterDTO != null)
                    {
                        viewModel.AllowToolOrdering = objRoomMasterDTO.AllowToolOrdering;
                    }
                }

                #region Moved duplicate code outside if and commented
                //viewModel.isCompany = mpCompanyMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isCompany_Create = mpCompanyMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////RoomMaster
                //viewModel.isRoom = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isRoom_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                ////BinMaster
                //viewModel.isBin = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isBin_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.isFTP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isFTP_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.IsEmailConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EmailConfiguration);
                //viewModel.IsEnterpriseConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterPriseConfiguration);
                //viewModel.IseVMISetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.eVMISetup);
                //viewModel.IsImport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Imports);
                //viewModel.IsExport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);
                //viewModel.isMoveMaterial = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MoveMaterial);
                //viewModel.isBarcodeList = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Barcode);
                //viewModel.IsPDAColumnSetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PdaColumnsettings);
                //viewModel.ShowBomMenu = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster);
                //viewModel.AllowtoViewDashboard = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewDashboard);

                ////CategoryMaster
                //viewModel.isCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////CostUOMMaster
                //viewModel.isCostUOM = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isCostUOM_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////InventoryClassificationMaster
                //viewModel.isInventoryClassification = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isInventoryClassification_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //CustomerMaster
                //viewModel.isCustomer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isCustomer_create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);


                ////FreightTypeMaster
                //viewModel.isFreightType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isFreightType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////GLAccountsMaster
                //viewModel.isGLAccounts = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isGLAccounts_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////GXPRConsignedJobMaster
                //viewModel.isGXPRConsignedJob = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isGXPRConsignedJob_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////JobTypeMaster
                //viewModel.isJobType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isJobType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////ProjectMaster
                //viewModel.isProject = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

                ////SupplierMaster
                //viewModel.isSupplier = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.SupplierList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                ////ShipViaMaster
                //viewModel.isShipVia = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isShipVia_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //VendoerMaster
                // viewModel.isVendor = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                // viewModel.isVendor_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                ////TechnicianMaster
                //viewModel.isTechnician = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isTechnician_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                ////UnitMaster
                //viewModel.isUnit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isUnit_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                ////LocationMaster
                //viewModel.isLocation = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isLocation_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.isToolCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isToolCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.isAssetCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isAssetCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                ////ManufacturerMaster
                //viewModel.isManufacturer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isManufacturer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////MeasurementTermMaster
                //viewModel.isMeasurementTerm = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isMeasurementTerm_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////RoleMaster
                //viewModel.isRole = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isRole_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////UserMaster
                //viewModel.isUser = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isUser_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                ////Permiassion Template
                //viewModel.isPermissionTemplate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isPermissionTemplate_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                ////ResourceMaster
                //viewModel.isResource = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ResourceMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);


                //// Inventry Menu starts from Here
                //viewModel.isItemList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isItemList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                ////QuickList
                //viewModel.isQuickList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isQuickList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.isCount = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isCount_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.isSupplierCatalog = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Suppliercatalog, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isMaterialstaging = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isMaterialstaging_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                // Inventry Menu ends Here

                // Consume Menu starts from Here
                //viewModel.isPullList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isPullList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                //viewModel.isRequisitions = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isRequisitions_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.isWorkorders = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isWorkorders_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.isProjectspend = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isProjectspend_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                // Consume Menu ends from Here

                //// Replenish Menu starts from Here
                //viewModel.IsOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.IsToolAssetOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsToolAssetOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.IsReturnOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsReturnOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.IsCart = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsCart_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsReceive = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsReceive_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsTransfer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsTransfer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //// Replenish Menu ends from Here

                ////Assets Menu starts from Here
                //viewModel.isTool = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.isTool_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsAsset = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsAsset_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsATScheduler = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsATScheduler_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

                //viewModel.IsATSchedulerMapping = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsATSchedulerMapping_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


                //viewModel.IsAssetMaintance = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsAssetMaintance_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                ////Assets Menu ends from Here

                ////Kits Menu starts from Here
                //viewModel.IsKits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsKits_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsWIP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WIP, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                ////Kits Menu ends from Here
                //viewModel.isLabelPrinting = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LabelPrinting, eTurnsWeb.Helper.SessionHelper.PermissionType.View);



                //viewModel.IsReportView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsReportInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsReportEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
                //viewModel.IsReportDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);


                //viewModel.IsNotificationsView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                //viewModel.IsNotificationsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                //viewModel.IsNotificationsEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
                //viewModel.IsNotificationsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
                //viewModel.IsConsignedCreditPull = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.IsChecked);

                //viewModel.isCatalogReport = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                #endregion
            }

            // Admin permission
            viewModel.IsEmailConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EmailConfiguration);
            viewModel.IsEnterpriseConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterPriseConfiguration);
            viewModel.IseVMISetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.eVMISetup);
            viewModel.IsImport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Imports);
            //viewModel.IsExport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);
            viewModel.ShowBomMenu = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster);
            viewModel.isMoveMaterial = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MoveMaterial);
            viewModel.isBarcodeList = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Barcode);
            viewModel.IsPDAColumnSetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PdaColumnsettings);

            viewModel.AllowtoViewDashboard = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewDashboard);
            viewModel.AllowtoViewMinMaxDashboard = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewMinMaxDashboard);
            //viewModel.isMoveMaterial_Create = mpMoveMaterial.IsInsert;
            //CompanyMaster
            viewModel.isCompanyConfig = mpCompanyMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCompany = mpCompanyMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCompany_Create = mpCompanyMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //RoomMaster
            viewModel.isRoom = mpRoomMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isRoom_Create = mpRoomMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


            //BinMaster
            viewModel.isBin = mpBinMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isBin_Create = mpBinMaster.IsInsert; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.isFTP = mpFTPMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isFTP_Create = mpFTPMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //CategoryMaster
            viewModel.isCategory = mpCategoryMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCategory_Create = mpCategoryMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                     //CostUOMMaster
            viewModel.isCostUOM = mpCostUOMMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCostUOM_Create = mpCostUOMMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

            //InventoryClassificationMaster
            viewModel.isInventoryClassification = mpInventoryClassificationMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isInventoryClassification_Create = mpInventoryClassificationMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //CustomerMaster
            viewModel.isCustomer = mpCustomerMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCustomer_create = mpCustomerMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                     //FreightTypeMaster
            viewModel.isFreightType = mpFreightTypeMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isFreightType_Create = mpFreightTypeMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                           //GLAccountsMaster
            viewModel.isGLAccounts = mpGLAccountsMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isGLAccounts_Create = mpGLAccountsMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                         //GXPRConsignedJobMaster
            viewModel.isGXPRConsignedJob = mpGXPRConsignedJobMaster.IsView;  //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isGXPRConsignedJob_Create = mpGXPRConsignedJobMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //JobTypeMaster
            viewModel.isJobType = mpJobTypeMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isJobType_Create = mpJobTypeMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                   //ProjectMaster
            viewModel.isProject = mpProjectMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

            //SupplierMaster
            viewModel.isSupplier = mpSupplierMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.SupplierList_Create = mpSupplierMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //ShipViaMaster
            viewModel.isShipVia = mpShipViaMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isShipVia_Create = mpShipViaMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //VendorMaster
            viewModel.isVendor = mpVenderMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isVendor_Create = mpVenderMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //TechnicianMaster
            viewModel.isTechnician = mpTechnicianMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isTechnician_Create = mpTechnicianMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //UnitMaster
            viewModel.isUnit = mpUnitMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isUnit_Create = mpUnitMaster.IsInsert; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


            //LocationMaster
            viewModel.isLocation = mpLocationMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isLocation_Create = mpLocationMaster.IsInsert; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.isToolCategory = mpToolCategory.IsView;  //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isToolCategory_Create = mpToolCategory.IsInsert;  //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.isAssetCategory = mpAssetCategory.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isAssetCategory_Create = mpAssetCategory.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //ManufacturerMaster
            viewModel.isManufacturer = mpManufacturerMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isManufacturer_Create = mpManufacturerMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                             //MeasurementTermMaster
            viewModel.isMeasurementTerm = mpMeasurementTermMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isMeasurementTerm_Create = mpMeasurementTermMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //RoleMaster
            viewModel.isRole = mpRoleMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isRole_Create = mpRoleMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                             //UserMaster
            viewModel.isUser = mpUserMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isUser_Create = mpUserMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            // PermissionTemplateMaster
            viewModel.isPermissionTemplate = mpPermissionTemplates.IsView; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isPermissionTemplate_Create = mpPermissionTemplates.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //ResourceMaster
            viewModel.isResource = mpResourceMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ResourceMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);


            // Inventry Menu starts from Here
            viewModel.isItemList = mpItemMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isItemList_Create = mpItemMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            //QuickList
            viewModel.isQuickList = mpQuickListPermission.IsView; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isQuickList_Create = mpQuickListPermission.IsInsert; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.isCount = mpCount.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isCount_Create = mpCount.IsInsert;// eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


            viewModel.isSupplierCatalog = mpSuppliercatalog.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Suppliercatalog, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isMaterialstaging = mpMaterialstaging.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isMaterialstaging_Create = mpMaterialstaging.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                             // Inventry Menu ends Here

            // Consume Menu starts from Here
            viewModel.isPullList = mpPullMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isPullList_Create = mpPullMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.isRequisitions = mpRequisitions.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isRequisitions_Create = mpRequisitions.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.isWorkorders = mpWorkOrders.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isWorkorders_Create = mpWorkOrders.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.isProjectspend = mpProjectMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isProjectspend_Create = mpProjectMaster.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            // Replenish Menu starts from Here
            viewModel.IsOrderList = mpOrders.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsOrderList_Create = mpOrders.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsToolAssetOrderList = mpToolAssetOrder.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsToolAssetOrderList_Create = mpToolAssetOrder.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsReturnOrderList = mpReturnOrder.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsReturnOrderList_Create = mpReturnOrder.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsQuoteList = mpQuote.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsQuoteList_Create = mpQuote.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsQuoteToOrder = mpQuoteToOrder.IsChecked;


            viewModel.IsCart = mpCart.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsCart_Create = mpCart.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsReceive = mpReceive.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsReceive_Create = mpReceive.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsTransfer = mpTransfer.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsTransfer_Create = mpTransfer.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            // Replenish Menu ends from Here

            //Assets Menu starts from Here
            viewModel.isTool = mpToolMaster.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isTool_Create = mpToolMaster.IsInsert; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsAsset = mpAssets.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsAsset_Create = mpAssets.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsATScheduler = mpAssetToolScheduler.IsView;//eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsATScheduler_Create = mpAssetToolScheduler.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsATSchedulerMapping = mpAssetToolSchedulerMapping.IsView; // eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsATSchedulerMapping_Create = mpAssetToolSchedulerMapping.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

            viewModel.IsAssetMaintance = mpAssetMaintenance.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsAssetMaintance_Create = mpAssetMaintenance.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                                                                             //Assets Menu ends from Here

            //Kits Menu starts from Here
            viewModel.IsKits = mpKits.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsKits_Create = mpKits.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsWIP = mpWIP.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WIP, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
                                            //Kits Menu ends from Here
            viewModel.IsToolKit = mpToolKitBuildBreak.IsView; // For Tool kit view in kit menu
            viewModel.IsToolKit_Create = mpToolKitBuildBreak.IsInsert; // For Tool kit insert in kit menu
            viewModel.isLabelPrinting = mpLabelPrinting.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LabelPrinting, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isInsertrLabelPrinting = mpLabelPrinting.IsInsert;

            viewModel.IsReportView = mpReports.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsReportInsert = mpReports.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsReportEdit = mpReports.IsUpdate; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
            viewModel.IsReportDelete = mpReports.IsDelete; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);


            viewModel.IsNotificationsView = mpNotifications.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.IsNotificationsInsert = mpNotifications.IsInsert; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            viewModel.IsNotificationsEdit = mpNotifications.IsUpdate; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
            viewModel.IsNotificationsDelete = mpNotifications.IsDelete; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);

            viewModel.IsConsignedCreditPull = mpAllowConsignedCreditPull.IsChecked; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.IsChecked);

            viewModel.isCatalogReport = mpCatalogReport.IsView; //eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            viewModel.isInsertCatalogReport = mpCatalogReport.IsInsert;
            viewModel.IsDataArchivalView = mpDataArchival.IsView;
            viewModel.IsDataArchivalInsert = mpDataArchival.IsInsert;

            viewModel.IsWrittenOffCategory = mpWrittenOffCategory.IsView;
            viewModel.IsWrittenOffCategoryInsert = mpWrittenOffCategory.IsInsert;
            viewModel.IsViewReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.ViewReport);
            viewModel.IsScheduleReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.ScheduleReport);
            viewModel.IsCustomizeReport = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.CustomizeReport);
            viewModel.IsEnterpriseGridColumnSetup = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseGridColumnSetup);
            viewModel.IsEnterpriseUDFSetup = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseUDFSetup);
            viewModel.IsQuickBooksIntegration = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.QuickBooksIntegration);
            viewModel.IsEnterpriseItemQuickList = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            viewModel.IsSensorBinsRFIDeTags = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.SensorBinsRFIDeTags);
            mpEnterpriseMaster.Dispose();
            mpCompanyMaster.Dispose();
            mpRoleMaster.Dispose();

        }

        //private HeaderWithMenuModel GetModel()
        //{
        //    HeaderWithMenuModel viewModel = new HeaderWithMenuModel();


        //    //eTurns.DAL.ModuleMasterDAL objModuleMaster = new eTurns.DAL.ModuleMasterDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName);
        //    viewModel.UserType = viewModel.UserType;
        //    viewModel.RoleID = eTurnsWeb.Helper.SessionHelper.RoleID;

        //    viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

        //    if (eTurnsWeb.Helper.SessionHelper.RoomPermissions != null)
        //    {
        //        if (viewModel.UserType == 1 || viewModel.UserType == 2)
        //        {
        //            #region "UserType 1 or 2"
        //            if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0)
        //            {
        //                if (eTurnsWeb.Helper.SessionHelper.CompanyList != null && eTurnsWeb.Helper.SessionHelper.CompanyList.Count > 0)
        //                {
        //                    if (eTurnsWeb.Helper.SessionHelper.RoomID > 0)
        //                    {

        //                        //RoomDTO objRoomMasterDTO = new eTurns.DAL.RoomDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);

        //                        viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;


        //                        if (viewModel.UserType == 1)
        //                        {
        //                            //EnterpriseMaster
        //                            viewModel.isEnterprise = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                            viewModel.isEnterprise_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        }
        //                        //CompanyMaster
        //                        viewModel.isCompany = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCompanyConfig = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCompany_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //RoomMaster
        //                        viewModel.isRoom = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isRoom_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsEmailConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EmailConfiguration);
        //                        viewModel.IsEnterpriseConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterPriseConfiguration);
        //                        viewModel.IseVMISetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.eVMISetup);
        //                        viewModel.IsImport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Imports);
        //                        viewModel.IsExport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);

        //                        //BinMaster
        //                        viewModel.isBin = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isBin_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.isFTP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isFTP_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //CategoryMaster
        //                        viewModel.isCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //CostUOMMaster
        //                        viewModel.isCostUOM = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCostUOM_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //                        //InventoryClassificationMaster
        //                        viewModel.isInventoryClassification = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isInventoryClassification_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //CustomerMaster
        //                        viewModel.isCustomer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCustomer_create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //FreightTypeMaster
        //                        viewModel.isFreightType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isFreightType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //GLAccountsMaster
        //                        viewModel.isGLAccounts = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isGLAccounts_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //GXPRConsignedJobMaster
        //                        viewModel.isGXPRConsignedJob = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isGXPRConsignedJob_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //JobTypeMaster
        //                        viewModel.isJobType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isJobType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //ProjectMaster
        //                        viewModel.isProject = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //                        //SupplierMaster
        //                        viewModel.isSupplier = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.SupplierList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //ShipViaMaster
        //                        viewModel.isShipVia = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isShipVia_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //VendorMaster
        //                        viewModel.isVendor = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isVendor_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //                        //TechnicianMaster
        //                        viewModel.isTechnician = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isTechnician_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //UnitMaster
        //                        viewModel.isUnit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isUnit_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //                        //LocationMaster
        //                        viewModel.isLocation = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isLocation_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.isToolCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isToolCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.isAssetCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isAssetCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //ManufacturerMaster
        //                        viewModel.isManufacturer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isManufacturer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //MeasurementTermMaster
        //                        viewModel.isMeasurementTerm = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isMeasurementTerm_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //RoleMaster
        //                        viewModel.isRole = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isRole_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //UserMaster
        //                        viewModel.isUser = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isUser_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //                        // PermissionTemplateMaster
        //                        viewModel.isPermissionTemplate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isPermissionTemplate_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //ResourceMaster
        //                        viewModel.isResource = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ResourceMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //                        // Inventry Menu starts from Here
        //                        viewModel.isItemList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isItemList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        //QuickList
        //                        viewModel.isQuickList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isQuickList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.isCount = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isCount_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.isSupplierCatalog = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Suppliercatalog, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isMaterialstaging = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isMoveMaterial = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MoveMaterial);
        //                        viewModel.isBarcodeList = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Barcode);
        //                        viewModel.isMaterialstaging_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        // Inventry Menu ends Here

        //                        // Consume Menu starts from Here
        //                        viewModel.isPullList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isPullList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.isRequisitions = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isRequisitions_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.isWorkorders = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isWorkorders_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.isProjectspend = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isProjectspend_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        // Consume Menu ends from Here

        //                        // Replenish Menu starts from Here
        //                        viewModel.IsOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.IsToolAssetOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsToolAssetOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.IsReturnOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsReturnOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.IsCart = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsCart_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsReceive = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsReceive_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsTransfer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsTransfer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        // Replenish Menu ends from Here

        //                        //Assets Menu starts from Here
        //                        viewModel.isTool = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.isTool_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsAsset = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsAsset_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.IsATScheduler = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsATScheduler_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //                        viewModel.IsATSchedulerMapping = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsATSchedulerMapping_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //                        viewModel.IsAssetMaintance = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsAssetMaintance_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        //Assets Menu ends from Here

        //                        //Kits Menu starts from Here
        //                        viewModel.IsKits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsKits_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsWIP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WIP, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        //Kits Menu ends from Here

        //                        viewModel.isLabelPrinting = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LabelPrinting, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsPDAColumnSetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PdaColumnsettings);

        //                        viewModel.IsReportView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsReportInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsReportEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        //                        viewModel.IsReportDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);


        //                        viewModel.IsNotificationsView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                        viewModel.IsNotificationsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //                        viewModel.IsNotificationsEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        //                        viewModel.IsNotificationsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);

        //                        viewModel.IsConsignedCreditPull = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.IsChecked);

        //                        viewModel.ShowBomMenu = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster);
        //                        viewModel.isCatalogReport = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //                        viewModel.AllowtoViewDashboard = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewDashboard);

        //                        if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
        //                        {
        //                            viewModel.IsHelpDocumentSetUp = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
        //                        {
        //                            viewModel.isEnterprise = true;
        //                        }
        //                        viewModel.isRoom = true;
        //                        viewModel.isCompany = true;
        //                        if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
        //                        {
        //                            viewModel.isCompanyConfig = true;
        //                        }
        //                        else
        //                        {
        //                            viewModel.isCompanyConfig = false;
        //                        }
        //                        //isRole = true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
        //                    {
        //                        viewModel.isEnterprise = true;
        //                    }
        //                    viewModel.isCompany = true;
        //                    if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
        //                    {
        //                        viewModel.isCompanyConfig = true;
        //                    }
        //                    else
        //                    {
        //                        viewModel.isCompanyConfig = false;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
        //                {
        //                    viewModel.isEnterprise = true;// eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //                }
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region "UseType else"
        //            //EnterpriseMaster
        //            viewModel.isEnterprise = false;// eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterpriseMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //            viewModel.AllowToolOrdering = eTurnsWeb.Helper.SessionHelper.AllowToolOrdering;

        //            //CompanyMaster
        //            viewModel.isCompany = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            if (eTurnsWeb.Helper.SessionHelper.CompanyID > 0)
        //            {
        //                viewModel.isCompanyConfig = true;
        //            }
        //            else
        //            {
        //                viewModel.isCompanyConfig = false;
        //            }
        //            if (eTurnsWeb.Helper.SessionHelper.RoomID > 0)
        //            {
        //                RoomDTO objRoomMasterDTO = new eTurns.DAL.RoomDAL(eTurnsWeb.Helper.SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
        //                if (objRoomMasterDTO != null)
        //                {
        //                    viewModel.AllowToolOrdering = objRoomMasterDTO.AllowToolOrdering;
        //                }
        //            }
        //            viewModel.isCompany_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CompanyMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //RoomMaster
        //            viewModel.isRoom = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isRoom_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoomMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //            //BinMaster
        //            viewModel.isBin = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isBin_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BinMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isFTP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isFTP_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FTPMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.IsEmailConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EmailConfiguration);
        //            viewModel.IsEnterpriseConfig = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.EnterPriseConfiguration);
        //            viewModel.IseVMISetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.eVMISetup);
        //            viewModel.IsImport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Imports);
        //            viewModel.IsExport = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);

        //            //CategoryMaster
        //            viewModel.isCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CategoryMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //CostUOMMaster
        //            viewModel.isCostUOM = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isCostUOM_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CostUOMMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //InventoryClassificationMaster
        //            viewModel.isInventoryClassification = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isInventoryClassification_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.InventoryClassificationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //CustomerMaster
        //            viewModel.isCustomer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isCustomer_create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CustomerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);


        //            //FreightTypeMaster
        //            viewModel.isFreightType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isFreightType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.FreightTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //GLAccountsMaster
        //            viewModel.isGLAccounts = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isGLAccounts_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GLAccountsMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //GXPRConsignedJobMaster
        //            viewModel.isGXPRConsignedJob = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isGXPRConsignedJob_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //JobTypeMaster
        //            viewModel.isJobType = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isJobType_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.JobTypeMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //ProjectMaster
        //            viewModel.isProject = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //            //SupplierMaster
        //            viewModel.isSupplier = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.SupplierList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //ShipViaMaster
        //            viewModel.isShipVia = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isShipVia_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ShipViaMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //VendoerMaster
        //            viewModel.isVendor = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isVendor_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.VenderMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //TechnicianMaster
        //            viewModel.isTechnician = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isTechnician_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TechnicianMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //UnitMaster
        //            viewModel.isUnit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isUnit_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UnitMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //            //LocationMaster
        //            viewModel.isLocation = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isLocation_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LocationMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.isToolCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isToolCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.isAssetCategory = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isAssetCategory_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetCategory, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //            //ManufacturerMaster
        //            viewModel.isManufacturer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isManufacturer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ManufacturerMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //MeasurementTermMaster
        //            viewModel.isMeasurementTerm = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isMeasurementTerm_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MeasurementTermMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //RoleMaster
        //            viewModel.isRole = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isRole_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RoleMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //UserMaster
        //            viewModel.isUser = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isUser_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.UserMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //ResourceMaster
        //            viewModel.isResource = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ResourceMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //            //Permiassion Template
        //            viewModel.isPermissionTemplate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isPermissionTemplate_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PermissionTemplates, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            // Inventry Menu starts from Here
        //            viewModel.isItemList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isItemList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            //QuickList
        //            viewModel.isQuickList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isQuickList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isCount = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isCount_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isSupplierCatalog = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Suppliercatalog, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isMaterialstaging = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isMoveMaterial = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.MoveMaterial);
        //            viewModel.isBarcodeList = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Barcode);
        //            viewModel.isMaterialstaging_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            // Inventry Menu ends Here

        //            // Consume Menu starts from Here
        //            viewModel.isPullList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isPullList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isRequisitions = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isRequisitions_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isWorkorders = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isWorkorders_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.isProjectspend = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isProjectspend_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            // Consume Menu ends from Here

        //            // Replenish Menu starts from Here
        //            viewModel.IsOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.IsToolAssetOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsToolAssetOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolAssetOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.IsReturnOrderList = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsReturnOrderList_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.IsCart = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsCart_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsReceive = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsReceive_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Receive, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsTransfer = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsTransfer_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            // Replenish Menu ends from Here

        //            //Assets Menu starts from Here
        //            viewModel.isTool = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.isTool_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsAsset = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsAsset_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsATScheduler = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsATScheduler_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolScheduler, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);

        //            viewModel.IsATSchedulerMapping = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsATSchedulerMapping_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);


        //            viewModel.IsAssetMaintance = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsAssetMaintance_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            //Assets Menu ends from Here

        //            //Kits Menu starts from Here
        //            viewModel.IsKits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsKits_Create = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsWIP = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WIP, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            //Kits Menu ends from Here
        //            viewModel.isLabelPrinting = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.LabelPrinting, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //            viewModel.IsPDAColumnSetUp = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PdaColumnsettings);

        //            viewModel.IsReportView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsReportInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsReportEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        //            viewModel.IsReportDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);


        //            viewModel.IsNotificationsView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //            viewModel.IsNotificationsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        //            viewModel.IsNotificationsEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        //            viewModel.IsNotificationsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Notifications, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
        //            viewModel.IsConsignedCreditPull = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.IsChecked);
        //            viewModel.ShowBomMenu = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.BOMItemMaster);
        //            viewModel.isCatalogReport = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        //            viewModel.AllowtoViewDashboard = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowtoViewDashboard);

        //            if (eTurnsWeb.Helper.SessionHelper.RoleID == -1)
        //            {
        //                viewModel.IsHelpDocumentSetUp = true;
        //            }
        //            #endregion
        //        }
        //    }

        //    if (viewModel.isRoom == true || viewModel.isRole == true || viewModel.isResource == true || viewModel.isCompany == true || viewModel.isEnterprise == true 
        //        || viewModel.isUser == true || viewModel.isPermissionTemplate == true)
        //    {
        //        viewModel.ShowAuthanticationMenu = true;
        //    }
        //    if (viewModel.isFTP == true || viewModel.isBin == true || viewModel.isCategory == true || viewModel.isCostUOM == true 
        //        || viewModel.isInventoryClassification == true || viewModel.isCustomer == true 
        //        || viewModel.isFreightType == true || viewModel.isGLAccounts == true
        //        || viewModel.isGXPRConsignedJob == true || viewModel.isJobType == true || viewModel.isProject == true 
        //        || viewModel.isSupplier == true || viewModel.isShipVia == true || viewModel.isVendor == true 
        //        || viewModel.isTechnician == true || viewModel.isUnit == true || viewModel.isLocation == true 
        //        || viewModel.isManufacturer == true || viewModel.isMeasurementTerm == true || viewModel.isToolCategory == true 
        //        || viewModel.isAssetCategory == true)
        //    {
        //        viewModel.ShowMastersMenu = true;
        //    }
        //    if (viewModel.isTool || viewModel.IsAsset || viewModel.IsAssetMaintance || viewModel.IsATScheduler 
        //        || viewModel.IsATSchedulerMapping || viewModel.IsToolAssetOrderList)
        //    {
        //        viewModel.ShowAssetsMenu = true;
        //    }
        //    if (viewModel.isQuickList || viewModel.isItemList || viewModel.isCount 
        //        || viewModel.isSupplierCatalog || viewModel.isMaterialstaging)
        //    {
        //        viewModel.ShowInventrysMenu = true;
        //    }
        //    if (viewModel.isPullList || viewModel.isRequisitions || viewModel.isWorkorders || viewModel.isProjectspend)
        //    {
        //        viewModel.ShowConsumeMenu = true;
        //    }
        //    if (viewModel.IsOrderList || viewModel.IsCart || viewModel.IsReturnOrderList 
        //        || viewModel.IsReceive || viewModel.IsTransfer)
        //    {
        //        viewModel.ShowReplenishMenu = true;
        //    }
        //    if (viewModel.IsKits || viewModel.IsWIP)
        //    {
        //        viewModel.ShowKitsMenu = true;
        //        eTurnsWeb.Controllers.KitController objKitController = new eTurnsWeb.Controllers.KitController();
        //        viewModel.BuildBreakNotification = objKitController.GetWIPKitCountForRedCircle();
        //        viewModel.TotalKits = viewModel.BuildBreakNotification;
        //        viewModel.BuildBreakNotification = (string.IsNullOrEmpty(viewModel.BuildBreakNotification) || viewModel.BuildBreakNotification == "0") ? "" : " (" + viewModel.BuildBreakNotification + ")";
        //    }

        //    if (viewModel.UserType == 3)
        //    {
        //        viewModel.isCompany = false;
        //        viewModel.isCompanyConfig = false;
        //        viewModel.isCompany_Create = false;
        //    }

        //    try
        //    {
        //        if (eTurnsWeb.Helper.SessionHelper.eTurnsRegionInfoProp == null)
        //        {
        //            TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        //            DateTime utc = viewModel.currentDate;
        //            if (mountain != null)
        //            {
        //                viewModel.timeZoneTiming = TimeZoneInfo.ConvertTimeFromUtc(utc, mountain);
        //                viewModel.timespanObj = mountain.BaseUtcOffset;
        //                viewModel.TimeZoneName = "UTC";
        //            }

        //        }
        //        else
        //        {
        //            eTurnsRegionInfo Region = eTurnsWeb.Helper.SessionHelper.eTurnsRegionInfoProp;

        //            TimeZoneInfo mountain = TimeZoneInfo.FindSystemTimeZoneById(Region.TimeZoneName);
        //            DateTime utc = viewModel.currentDate;
        //            if (mountain != null)
        //            {
        //                viewModel.timeZoneTiming = TimeZoneInfo.ConvertTimeFromUtc(utc, mountain);
        //                viewModel.TimeZoneName = Region.TimeZoneName;

        //                viewModel.timespanObj = mountain.BaseUtcOffset;
        //            }

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        viewModel.timeZoneTiming = new DateTime();
        //        viewModel.currentDate = DateTime.UtcNow;
        //        viewModel.timespanObj = new TimeSpan();
        //        viewModel.TimeZoneName = string.Empty;
        //    }

        //    System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
        //    viewModel.ReleaseNumber = Settinfile.Element("ReleaseNumber").Value;

        //    viewModel.AccessQryUserNames = Settinfile.Element("AccessQryUserNames") != null ? Settinfile.Element("AccessQryUserNames").Value : string.Empty;
        //    viewModel.AccessQryRoleIds = Settinfile.Element("AccessQryRoleIds") != null ? Settinfile.Element("AccessQryRoleIds").Value : string.Empty;

        //    if (!string.IsNullOrWhiteSpace(viewModel.AccessQryUserNames))
        //    {
        //        string[] UserNameList = viewModel.AccessQryUserNames.Split('$');
        //        for (int i = 0; i < UserNameList.Length; i++)
        //        {
        //            if (UserNameList[i] == eTurnsWeb.Helper.SessionHelper.UserName)
        //            {
        //                viewModel.ShowMenutoThisUser = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (!viewModel.ShowMenutoThisUser)
        //    {
        //        if (!string.IsNullOrWhiteSpace(viewModel.AccessQryRoleIds))
        //        {
        //            string[] RoleIDS = viewModel.AccessQryRoleIds.Split('$');
        //            for (int i = 0; i < RoleIDS.Length; i++)
        //            {
        //                if (Convert.ToInt64(RoleIDS[i]) == eTurnsWeb.Helper.SessionHelper.RoleID)
        //                {
        //                    viewModel.ShowMenutoThisUser = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }


        //    return viewModel;
        //}

    }
}
