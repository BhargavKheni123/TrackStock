using System;
using System.Collections.Generic;

namespace eTurnsWeb.Models
{
    public class HeaderWithMenuModel
    {
        public bool isCompany = false;
        public bool isCompany_Create = false;
        public bool isCustomer = false;
        public bool isCustomer_create = false;
        public bool isEnterprise = false;
        public bool isEnterprise_Create = false;
        public bool isFreightType = false;
        public bool isFreightType_Create = false;
        public bool isGLAccounts = false;
        public bool isGLAccounts_Create = false;
        public bool isGXPRConsignedJob = false;
        public bool isGXPRConsignedJob_Create = false;
        public bool isJobType = false;
        public bool isJobType_Create = false;

        public bool isSupplier = false;
        public bool SupplierList_Create = false;
        public bool isShipVia = false;
        public bool isShipVia_Create = false;
        public bool isVendor = false;
        public bool isVendor_Create = false;
        public bool isTechnician = false;
        public bool isTechnician_Create = false;
        public bool isTool = false;
        public bool isTool_Create = false;
        public bool isUnit = false;
        public bool isUnit_Create = false;
        public bool isRoom = false;
        public bool isRoom_Create = false;
        public bool isLocation = false;
        public bool isLocation_Create = false;
        public bool isManufacturer = false;
        public bool isManufacturer_Create = false;
        public bool isMeasurementTerm = false;
        public bool isMeasurementTerm_Create = false;
        public bool isRole = false;
        public bool isRole_Create = false;
        public bool isUser = false;
        public bool isUser_Create = false;
        public bool isPermissionTemplate = false;
        public bool isPermissionTemplate_Create = false;
        public bool isResource = false;
        public bool isToolCategory = false;
        public bool isToolCategory_Create = false;
        public bool isAssetCategory = false;
        public bool isAssetCategory_Create = false;
        public bool isQuickList = false;
        public bool isQuickList_Create = false;
        public bool isItemList = false;
        public bool isItemList_Create = false;
        public bool isPullList = false;
        public bool isPullList_Create = false;
        public bool isRequisitions = false;
        public bool isRequisitions_Create = false;
        public bool isWorkorders = false;
        public bool isWorkorders_Create = false;
        public bool isProjectspend = false;
        public bool isProjectspend_Create = false;

        public bool IsOrderList = false;
        public bool IsOrderList_Create = false;

        public bool IsToolAssetOrderList = false;
        public bool IsToolAssetOrderList_Create = false;

        public bool IsReturnOrderList = false;
        public bool IsReturnOrderList_Create = false;

        public bool IsQuoteList = false;
        public bool IsQuoteList_Create = false;
        public bool IsQuoteToOrder = false;

        public bool IsCart = false;
        public bool IsCart_Create = false;

        public bool IsTransfer = false;
        public bool IsTransfer_Create = false;
        public bool IsReceive = false;
        public bool IsReceive_Create = false;
        public bool IsAsset = false;
        public bool IsAsset_Create = false;

        public bool IsATScheduler = false;
        public bool IsATScheduler_Create = false;

        public bool IsATSchedulerMapping = false;
        public bool IsATSchedulerMapping_Create = false;

        public bool IsAssetMaintance = false;
        public bool IsAssetMaintance_Create = false;

        public bool IsKits = false;
        public bool IsKits_Create = false;

        public bool IseVMISetUp = false;
        public bool IsHelpDocumentSetUp = false;

        public bool IsImport = false;
        public bool IsExport = false;
        public bool isProject = false;
        public bool isLabelPrinting = false;
        public bool isInsertrLabelPrinting = false;
        public bool isCompanyConfig = false;
        public bool IsEmailConfig = false;
        public bool IsEnterpriseConfig = false;
        public bool isCatalogReport = false;
        public bool isInsertCatalogReport = false;
        public bool IsPDAColumnSetUp = false;

        public bool isBin = false;
        public bool isBin_Create = false;
        public bool isCategory = false;
        public bool isCategory_Create = false;
        public bool isCostUOM_Create = false;
        public bool isCostUOM = false;
        public bool isInventoryClassification = false;
        public bool isInventoryClassification_Create = false;

        public bool isFTP = false;
        public bool isFTP_Create = false;
        public bool isBarcodeList = false;

        public string BuildBreakNotification { get; set; } 
        public string TotalKits = "0";
        public string QuickListNotification = "";
        public string CountNotification = "";
        public string SCNotification = "";
        public string MSNotification = "";
        public string TotalInventory = "0";

        public string ReceiveNotification = "";
        public string TransferNotification = "";
        public string TotalReplanish = "0";

        public string ToolsNotification = "";
        public string AssetsNotification = "";

        public string TotalAssets = "0";
        public string CartNotification = "";
        public string OrderNotification = "";
        public string ToolAssetOrderNotification = "";
        public string PullNotification = "";
        public string RequisitionNotification = "";
        public string WorkOrderNotification = "";
        public string ProjectSpendNotification = "";
        public string TotalConsume = "0";
        public bool ShowAuthanticationMenu = false;
        public bool ShowMastersMenu = false;
        public bool ShowBomMenu = false;
        public bool ShowAssetsMenu = false;
        public bool ShowInventrysMenu = false;
        public bool ShowConsumeMenu = false;
        public bool ShowReplenishMenu = false;
        public bool ShowKitsMenu = false;

        public bool ShowConfigMenu = true;
        public bool ShowMenutoThisUser = false;
        public string ReleaseNumber = string.Empty;
        public string AccessQryUserNames = string.Empty;
        public string AccessQryRoleIds = string.Empty;

        public bool isSupplierCatalog = false;
        public bool isMaterialstaging = false;
        public bool isMoveMaterial = false;
        //public bool isMoveMaterial_Create = false;
        public bool isMaterialstaging_Create = false;
        public bool isCount = false;
        public bool isCount_Create = false;

        public bool IsWIP = false;

        public bool IsReportView = true;
        public bool IsReportEdit = true;
        public bool IsReportInsert = true;
        public bool IsReportDelete = true;

        public bool IsNotificationsView = true;
        public bool IsNotificationsEdit = true;
        public bool IsNotificationsInsert = true;
        public bool IsNotificationsDelete = true;
        public bool IsConsignedCreditPull = true;
                

        public bool IsWrittenOffCategory = false;
        public bool IsWrittenOffCategoryInsert = false;



        public bool AllowtoViewDashboard { get; set; } = true;
        public bool AllowtoViewMinMaxDashboard { get; set; } = true;

        public bool IsShowDashboard
        {
            get
            {
                return AllowtoViewDashboard || AllowtoViewMinMaxDashboard;
            }
        }

        public long RoleID { get; set; }
        public bool AllowToolOrdering;
        public DateTime timeZoneTiming = new DateTime();
        public DateTime currentDate = DateTime.UtcNow;
        public TimeSpan timespanObj = new TimeSpan();
        public string TimeZoneName = string.Empty;
        public int UserType { get; set; }

        //public List<TwoLevelMenu> MasterMenu { get; set; }

        public bool IsShowReportMenu = false;
        public bool IsDataArchivalView = true;
        public bool IsDataArchivalInsert = true;
        public bool IsToolKit = false;
        public bool IsToolKit_Create = false;
        public bool IsViewReport = false;
        public bool IsScheduleReport = false;
        public bool IsCustomizeReport = false;
        public bool IsEnterpriseGridColumnSetup = false;
        public bool IsEnterpriseUDFSetup = false;
        public bool IsQuickBooksIntegration = false;
        public bool IsEnterpriseItemQuickList = false;
        public bool IsSensorBinsRFIDeTags = false;
    }

    public class TwoLevelMenu
    {
        public int SortOrder { get; set; }
        public MenuLink Menu { get; set; }
        public List<MenuLink> SubMenu { get; set; }
    }

    public class MenuLink
    {
        public int SortOrder { get; set; }
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }

        public string Protocol { get; set; }

        public string Fragment { get; set; }
        public string HostName { get; set; }

        public object RouteValues { get; set; }

        public object HtmlAttributes { get; set; }
    }
}