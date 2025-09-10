using eTurns.DTO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace eTurnsWeb.Models
{
    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }


    }

    public static class UDFDictionaryTables
    {
        //public static SortedList GetUDFTables()
        //{
        //    SortedList _UDFTables;
        //    //if (HttpContext.Current.Application["_UDFTables"] != null)
        //    //{
        //    //    _UDFTables = (SortedList)HttpContext.Current.Application["_UDFTables"];
        //    //}
        //    //else
        //    //{
        //    _UDFTables = new SortedList();
        //    _UDFTables.Add("bins", "BinMaster");
        //    _UDFTables.Add("categories", "CategoryMaster");
        //    _UDFTables.Add("costuom", "CostUOMMaster");
        //    _UDFTables.Add("inventoryclassification", "InventoryClassificationMaster");
        //    _UDFTables.Add("customers", "CustomerMaster");
        //    _UDFTables.Add("freighttypes", "FreightTypeMaster");
        //    _UDFTables.Add("glaccounts", "GLAccountMaster");
        //    _UDFTables.Add("gxprconsigmentjobs", "GXPRConsigmentJobMaster");
        //    _UDFTables.Add("jobtypes", "JobTypeMaster");
        //    _UDFTables.Add("locations", "LocationMaster");
        //    _UDFTables.Add("manufacturers", "ManufacturerMaster");
        //    _UDFTables.Add("measurementterms", "MeasurementTerm");
        //    _UDFTables.Add("projects", "ProjectMaster");
        //    _UDFTables.Add("rooms", "Room");
        //    _UDFTables.Add("shipvias", "ShipViaMaster");
        //    _UDFTables.Add("supplierblankepos", "SupplierBlankePOMaster");
        //    _UDFTables.Add("suppliers", "SupplierMaster");
        //    _UDFTables.Add("technicians", "TechnicianMaster");
        //    _UDFTables.Add("toolcategories", "ToolCategoryMaster");
        //    _UDFTables.Add("assetcategories", "AssetCategoryMaster");
        //    _UDFTables.Add("tools", "ToolMaster");
        //    _UDFTables.Add("units", "UnitMaster");
        //    _UDFTables.Add("enterprises", "Enterprise");
        //    _UDFTables.Add("companies", "CompanyMaster");
        //    _UDFTables.Add("quicklist", "QuickListMaster");
        //    _UDFTables.Add("itemmaster", "ItemMaster");
        //    _UDFTables.Add("pullmaster", "PullMaster");
        //    _UDFTables.Add("ordermaster", "OrderMaster");
        //    _UDFTables.Add("projectspenditems", "ProjectSpendItems");
        //    _UDFTables.Add("cartitemlist", "CartItemList");
        //    _UDFTables.Add("kitmaster", "KitMaster");
        //    _UDFTables.Add("requisitionmaster", "RequisitionMaster");
        //    _UDFTables.Add("materialstaging", "MaterialStaging");
        //    _UDFTables.Add("vendermaster", "VenderMaster");
        //    _UDFTables.Add("assetmaster", "AssetMaster");
        //    _UDFTables.Add("transfermaster", "TransferMaster");
        //    _UDFTables.Add("workorder", "WorkOrder");
        //    _UDFTables.Add("toolsscheduler", "ToolsScheduler");
        //    _UDFTables.Add("inventorycount", "InventoryCount");
        //    _UDFTables.Add("inventorycountlineitem", "InventoryCountLineItem");
        //    _UDFTables.Add("toolschedulemapping", "toolschedulemapping");
        //    _UDFTables.Add("receivedordertransferdetail", "ReceivedOrderTransferDetail");
        //    _UDFTables.Add("checkouttool", "ToolCheckInOutHistory");            
        //    _UDFTables.Add("toolsmaintenancelist", "ToolsMaintenanceList");
        //    _UDFTables.Add("quicklistitems", "QuickListItems");
        //    _UDFTables.Add("orderdetails", "OrderDetails");
        //    _UDFTables.Add("usermaster", "UserMaster");

        //    //    HttpContext.Current.Application["_UDFTables"] = _UDFTables;
        //    //}

        //    return _UDFTables;
        //}

        public static Dictionary<string, string[]> GetUDFTables()
        {
            Dictionary<string, string[]> _UDFTables;
            //if (HttpContext.Current.Application["_UDFTables"] != null)
            //{
            //    _UDFTables = (SortedList)HttpContext.Current.Application["_UDFTables"];
            //}
            //else
            //{
            _UDFTables = new Dictionary<string, string[]>();
            _UDFTables.Add("bins", new string[] { "BinMaster", ((int)Helper.SessionHelper.ModuleList.BinMaster).ToString() });
            _UDFTables.Add("categories", new string[] { "CategoryMaster", ((int)Helper.SessionHelper.ModuleList.CategoryMaster).ToString() });
            _UDFTables.Add("costuom", new string[] { "CostUOMMaster", ((int)Helper.SessionHelper.ModuleList.CostUOMMaster).ToString() });
            _UDFTables.Add("inventoryclassification", new string[] { "InventoryClassificationMaster", ((int)Helper.SessionHelper.ModuleList.InventoryClassificationMaster).ToString() });
            _UDFTables.Add("customers", new string[] { "CustomerMaster", ((int)Helper.SessionHelper.ModuleList.CustomerMaster).ToString() });
            _UDFTables.Add("freighttypes", new string[] { "FreightTypeMaster", ((int)Helper.SessionHelper.ModuleList.FreightTypeMaster).ToString() });
            _UDFTables.Add("glaccounts", new string[] { "GLAccountMaster", ((int)Helper.SessionHelper.ModuleList.GLAccountsMaster).ToString() });
            _UDFTables.Add("gxprconsigmentjobs", new string[] { "GXPRConsigmentJobMaster", ((int)Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster).ToString() });
            _UDFTables.Add("jobtypes", new string[] { "JobTypeMaster", ((int)Helper.SessionHelper.ModuleList.JobTypeMaster).ToString() });
            _UDFTables.Add("locations", new string[] { "LocationMaster", ((int)Helper.SessionHelper.ModuleList.LocationMaster).ToString() });
            _UDFTables.Add("manufacturers", new string[] { "ManufacturerMaster", ((int)Helper.SessionHelper.ModuleList.ManufacturerMaster).ToString() });
            _UDFTables.Add("measurementterms", new string[] { "MeasurementTerm", ((int)Helper.SessionHelper.ModuleList.MeasurementTermMaster).ToString() });
            _UDFTables.Add("projects", new string[] { "ProjectMaster", ((int)Helper.SessionHelper.ModuleList.ProjectMaster).ToString() });
            _UDFTables.Add("rooms", new string[] { "Room", ((int)Helper.SessionHelper.ModuleList.RoomMaster).ToString() });
            _UDFTables.Add("shipvias", new string[] { "ShipViaMaster", ((int)Helper.SessionHelper.ModuleList.ShipViaMaster).ToString() });
            _UDFTables.Add("supplierblankepos", new string[] { "SupplierBlankePOMaster", ((int)Helper.SessionHelper.ModuleList.SupplierMaster).ToString() });
            _UDFTables.Add("suppliers", new string[] { "SupplierMaster", ((int)Helper.SessionHelper.ModuleList.SupplierMaster).ToString() });
            _UDFTables.Add("technicians", new string[] { "TechnicianMaster", ((int)Helper.SessionHelper.ModuleList.TechnicianMaster).ToString() });
            _UDFTables.Add("toolcategories", new string[] { "ToolCategoryMaster", ((int)Helper.SessionHelper.ModuleList.ToolCategory).ToString() });
            _UDFTables.Add("assetcategories", new string[] { "AssetCategoryMaster", ((int)Helper.SessionHelper.ModuleList.AssetCategory).ToString() });
            _UDFTables.Add("tools", new string[] { "ToolMaster", ((int)Helper.SessionHelper.ModuleList.ToolMaster).ToString() });
            _UDFTables.Add("units", new string[] { "UnitMaster", ((int)Helper.SessionHelper.ModuleList.UnitMaster).ToString() });
            _UDFTables.Add("enterprises", new string[] { "Enterprise", ((int)Helper.SessionHelper.ModuleList.EnterpriseMaster).ToString() });
            _UDFTables.Add("companies", new string[] { "CompanyMaster", ((int)Helper.SessionHelper.ModuleList.CompanyMaster).ToString() });
            _UDFTables.Add("quicklist", new string[] { "QuickListMaster", ((int)Helper.SessionHelper.ModuleList.QuickListPermission).ToString() });
            _UDFTables.Add("itemmaster", new string[] { "ItemMaster", ((int)Helper.SessionHelper.ModuleList.ItemMaster).ToString() });
            _UDFTables.Add("pullmaster", new string[] { "PullMaster", ((int)Helper.SessionHelper.ModuleList.PullMaster).ToString() });
            _UDFTables.Add("ordermaster", new string[] { "OrderMaster", ((int)Helper.SessionHelper.ModuleList.Orders).ToString() });
            _UDFTables.Add("projectspenditems", new string[] { "ProjectSpendItems", ((int)Helper.SessionHelper.ModuleList.ProjectMaster).ToString() });
            _UDFTables.Add("cartitemlist", new string[] { "CartItemList", ((int)Helper.SessionHelper.ModuleList.Cart).ToString() });
            _UDFTables.Add("kitmaster", new string[] { "KitMaster", ((int)Helper.SessionHelper.ModuleList.Kits).ToString() });
            _UDFTables.Add("requisitionmaster", new string[] { "RequisitionMaster", ((int)Helper.SessionHelper.ModuleList.Requisitions).ToString() });
            _UDFTables.Add("materialstaging", new string[] { "MaterialStaging", ((int)Helper.SessionHelper.ModuleList.Materialstaging).ToString() });
            _UDFTables.Add("vendermaster", new string[] { "VenderMaster", ((int)Helper.SessionHelper.ModuleList.VenderMaster).ToString() });
            _UDFTables.Add("assetmaster", new string[] { "AssetMaster", ((int)Helper.SessionHelper.ModuleList.Assets).ToString() });
            _UDFTables.Add("transfermaster", new string[] { "TransferMaster", ((int)Helper.SessionHelper.ModuleList.Transfer).ToString() });
            _UDFTables.Add("workorder", new string[] { "WorkOrder", ((int)Helper.SessionHelper.ModuleList.WorkOrders).ToString() });
            _UDFTables.Add("toolsscheduler", new string[] { "ToolsScheduler", ((int)Helper.SessionHelper.ModuleList.ToolsScheduler).ToString() });
            _UDFTables.Add("inventorycount", new string[] { "InventoryCount", ((int)Helper.SessionHelper.ModuleList.Count).ToString() });
            _UDFTables.Add("inventorycountlineitem", new string[] { "InventoryCountLineItem", ((int)Helper.SessionHelper.ModuleList.Count).ToString() });
            _UDFTables.Add("toolschedulemapping", new string[] { "toolschedulemapping", ((int)Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping).ToString() });
            _UDFTables.Add("receivedordertransferdetail", new string[] { "ReceivedOrderTransferDetail", ((int)Helper.SessionHelper.ModuleList.Receive).ToString() });
            _UDFTables.Add("checkouttool", new string[] { "ToolCheckInOutHistory", ((int)Helper.SessionHelper.ModuleList.AllowCheckInCheckout).ToString() });
            _UDFTables.Add("toolsmaintenancelist", new string[] { "ToolsMaintenanceList", ((int)Helper.SessionHelper.ModuleList.ToolMaster).ToString() });
            _UDFTables.Add("quicklistitems", new string[] { "QuickListItems", ((int)Helper.SessionHelper.ModuleList.QuickListPermission).ToString() });
            _UDFTables.Add("orderdetails", new string[] { "OrderDetails", ((int)Helper.SessionHelper.ModuleList.Orders).ToString() });
            _UDFTables.Add("usermaster", new string[] { "UserMaster", ((int)Helper.SessionHelper.ModuleList.UserMaster).ToString() });
            _UDFTables.Add("bomitemmaster", new string[] { "BOMItemMaster", ((int)Helper.SessionHelper.ModuleList.BOMItemMaster).ToString() });
            _UDFTables.Add("toolassetordermaster", new string[] { "ToolAssetOrder", ((int)Helper.SessionHelper.ModuleList.ToolAssetOrder).ToString() });
            _UDFTables.Add("toolassetorderdetails", new string[] { "ToolAssetOrderDetails", ((int)Helper.SessionHelper.ModuleList.ToolAssetOrder).ToString() });
            _UDFTables.Add("receivedtoolassetordertransferdetail", new string[] { "ReceivedToolAssetOrderTransferDetail", ((int)Helper.SessionHelper.ModuleList.ReceiveToolAsset).ToString() });
            _UDFTables.Add("quotemaster", new string[] { "QuoteMaster", ((int)Helper.SessionHelper.ModuleList.Quote).ToString() });
            _UDFTables.Add("quotedetails", new string[] { "QuoteDetails", ((int)Helper.SessionHelper.ModuleList.Quote).ToString() });
            _UDFTables.Add("binudf", new string[] { "BinUDF", ((int)Helper.SessionHelper.ModuleList.BinUDF).ToString() });
            _UDFTables.Add("room", new string[] { "Room", ((int)Helper.SessionHelper.ModuleList.RoomMaster).ToString() });
            //    HttpContext.Current.Application["_UDFTables"] = _UDFTables;
            //}

            return _UDFTables;
        }

        public static SortedList GetUDFTablesResourceFile()
        {
            SortedList _UDFTables;
            //if (HttpContext.Current.Application["_UDFTables"] != null)
            //{
            //    _UDFTables = (SortedList)HttpContext.Current.Application["_UDFTables"];
            //}
            //else
            //{
            _UDFTables = new SortedList();
            _UDFTables.Add("BinMaster", "ResBin");
            _UDFTables.Add("CartItemList", "ResCartItem");
            _UDFTables.Add("CategoryMaster", "ResCategoryMaster");
            _UDFTables.Add("costuom", "ResCostUOMMaster");
            _UDFTables.Add("inventoryclassification", "ResInventoryClassificationMaster");
            _UDFTables.Add("CustomerMaster", "ResCustomer");
            _UDFTables.Add("FreightTypeMaster", "ResFreightType");
            _UDFTables.Add("GLAccountMaster", "ResGLAccount");
            _UDFTables.Add("GXPRConsigmentJobMaster", "ResGXPRConsignedJob");
            _UDFTables.Add("JobTypeMaster", "ResJobType");
            _UDFTables.Add("LocationMaster", "ResLocation");
            _UDFTables.Add("ManufacturerMaster", "ResManufacturer");
            _UDFTables.Add("MeasurementTerm", "ResMeasurementTerm");
            _UDFTables.Add("ProjectMaster", "ResProjectMaster");
            _UDFTables.Add("ShipViaMaster", "ResShipVia");
            _UDFTables.Add("SupplierBlankePOMaster", "");
            _UDFTables.Add("SupplierMaster", "ResSupplierMaster");
            _UDFTables.Add("TechnicianMaster", "ResTechnician");
            _UDFTables.Add("ToolCategoryMaster", "ResToolCategory");
            _UDFTables.Add("AssetCategoryMaster", "ResAssetCategory");
            _UDFTables.Add("ToolMaster", "ResToolMaster");
            _UDFTables.Add("UnitMaster", "ResUnitMaster");
            _UDFTables.Add("Enterprise", "ResEnterprise");
            _UDFTables.Add("CompanyMaster", "ResCompany");
            _UDFTables.Add("Room", "ResRoomMaster");
            _UDFTables.Add("QuickListMaster", "ResQuickList");
            _UDFTables.Add("ItemMaster", "ResItemMaster");
            _UDFTables.Add("PullMaster", "ResPullMaster");
            _UDFTables.Add("OrderMaster", "ResOrder");
            _UDFTables.Add("ProjectSpendItems", "ResProjectSpendItems");
            _UDFTables.Add("KitMaster", "ResKitMaster");
            _UDFTables.Add("RequisitionMaster", "ResRequisitionMaster");
            _UDFTables.Add("VenderMaster", "ResVenderMaster");
            _UDFTables.Add("AssetMaster", "ResAssetMaster");
            _UDFTables.Add("TransferMaster", "ResTransfer");
            _UDFTables.Add("WorkOrder", "ResWorkOrder");
            _UDFTables.Add("ToolsScheduler", "ResToolsScheduler");
            _UDFTables.Add("toolschedulemapping", "ResToolsSchedulerMapping");
            _UDFTables.Add("ReceivedOrderTransferDetail", "ResReceiveOrderDetails");
            _UDFTables.Add("InventoryCountList", "ResInventoryCount");
            _UDFTables.Add("InventoryCount", "ResInventoryCount");
            _UDFTables.Add("MaterialStaging", "ResMaterialStaging");
            _UDFTables.Add("ToolCheckInOutHistory", "ResToolCheckInOutHistory");
            _UDFTables.Add("ToolsMaintenanceList", "ResToolsMaintenance");
            _UDFTables.Add("OrderDetails", "ResOrderDetails");
            _UDFTables.Add("QuickListItems", "ResQuickListItems");
            _UDFTables.Add("UserMaster", "ResUserMasterUDF");
            _UDFTables.Add("BOMItemMaster", "ResBOMItemMaster");
            _UDFTables.Add("ToolAssetOrder", "ResToolAssetOrder");
            _UDFTables.Add("InventoryCountLineItem", "ResInventoryCountDetail");
            _UDFTables.Add("QuoteMaster", "ResQuoteMaster");
            _UDFTables.Add("QuoteDetails", "ResQuoteDetail");
            _UDFTables.Add("BinUDF", "ResBinUDF");
            //    HttpContext.Current.Application["_UDFTables"] = _UDFTables;
            //}
            return _UDFTables;
        }

        public static bool IsVaidUDFTable(string UDFTableName)
        {
            SortedList _UDFTables = new SortedList();
            Dictionary<string, string[]> UDFLIST = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTables();
            if (UDFLIST != null && UDFLIST.Count > 0)
            {
                foreach (string Key in UDFLIST.Keys)
                {
                    _UDFTables.Add(Key, UDFLIST[Key]);
                }
            }

            if (_UDFTables != null)
            {
                int index = _UDFTables.IndexOfKey(UDFTableName);
                if (index == -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static string GetUDFTableFromKey(string UDFTableName, out int ModuleId)
        {
            Dictionary<string, string[]> _UDFTables;
            _UDFTables = GetUDFTables();
            //int index = _UDFTables.IndexOfKey(UDFTableName);
            string[] Ret = _UDFTables[UDFTableName];
            if (Ret != null && Ret.Length == 2)
            {
                ModuleId = Int32.Parse(Ret[1]);
                return Ret[0];
            }
            else
            {
                ModuleId = 0;
                return "";
            }
        }

        public static string GetUDFResourceFromKey(string UDFTableName)
        {
            SortedList _UDFTables = GetUDFTablesResourceFile();

            int index = _UDFTables.IndexOfKey(UDFTableName);
            if (index >= 0)
                return (string)_UDFTables.GetByIndex(index);
            else
            {
                SortedList _UdfSmallList = new SortedList();
                foreach (DictionaryEntry kvp in _UDFTables)
                {
                    _UdfSmallList.Add(kvp.Key.ToString().ToLower(), kvp.Value);
                }
                index = _UdfSmallList.IndexOfKey(UDFTableName.ToLower());
                if (index >= 0)
                    return (string)_UdfSmallList.GetByIndex(index);
            }
            return string.Empty;
        }

        public static bool IsVaidImportUDFTable(string UDFTableName)
        {
            SortedList _UDFTables = new SortedList();
            Dictionary<string, string[]> UDFLIST = eTurnsWeb.Models.UDFDictionaryTables.GetImportUDFTables();
            if (UDFLIST != null && UDFLIST.Count > 0)
            {
                foreach (string Key in UDFLIST.Keys)
                {
                    _UDFTables.Add(Key, UDFLIST[Key]);
                }
            }

            if (_UDFTables != null)
            {
                int index = _UDFTables.IndexOfKey(UDFTableName);
                if (index == -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<string, string[]> GetImportUDFTables()
        {
            Dictionary<string, string[]> _UDFTables;
            //if (HttpContext.Current.Application["_UDFTables"] != null)
            //{
            //    _UDFTables = (SortedList)HttpContext.Current.Application["_UDFTables"];
            //}
            //else
            //{
            _UDFTables = new Dictionary<string, string[]>();
            _UDFTables.Add("binmaster", new string[] { "BinMaster", ((int)Helper.SessionHelper.ModuleList.BinMaster).ToString() });
            _UDFTables.Add("categorymaster", new string[] { "CategoryMaster", ((int)Helper.SessionHelper.ModuleList.CategoryMaster).ToString() });
            _UDFTables.Add("costuommaster", new string[] { "CostUOMMaster", ((int)Helper.SessionHelper.ModuleList.CostUOMMaster).ToString() });
            _UDFTables.Add("inventoryclassificationmaster", new string[] { "InventoryClassificationMaster", ((int)Helper.SessionHelper.ModuleList.InventoryClassificationMaster).ToString() });
            _UDFTables.Add("customermaster", new string[] { "CustomerMaster", ((int)Helper.SessionHelper.ModuleList.CustomerMaster).ToString() });
            _UDFTables.Add("freighttypemaster", new string[] { "FreightTypeMaster", ((int)Helper.SessionHelper.ModuleList.FreightTypeMaster).ToString() });
            _UDFTables.Add("glaccountmaster", new string[] { "GLAccountMaster", ((int)Helper.SessionHelper.ModuleList.GLAccountsMaster).ToString() });
            _UDFTables.Add("gxprconsigmentjobmaster", new string[] { "GXPRConsigmentJobMaster", ((int)Helper.SessionHelper.ModuleList.GXPRConsignedJobMaster).ToString() });
            _UDFTables.Add("jobtypemaster", new string[] { "JobTypeMaster", ((int)Helper.SessionHelper.ModuleList.JobTypeMaster).ToString() });
            _UDFTables.Add("locationmaster", new string[] { "LocationMaster", ((int)Helper.SessionHelper.ModuleList.LocationMaster).ToString() });
            _UDFTables.Add("manufacturermaster", new string[] { "ManufacturerMaster", ((int)Helper.SessionHelper.ModuleList.ManufacturerMaster).ToString() });
            _UDFTables.Add("measurementterm", new string[] { "MeasurementTerm", ((int)Helper.SessionHelper.ModuleList.MeasurementTermMaster).ToString() });
            _UDFTables.Add("projectmaster", new string[] { "ProjectMaster", ((int)Helper.SessionHelper.ModuleList.ProjectMaster).ToString() });
            _UDFTables.Add("rooms", new string[] { "Room", ((int)Helper.SessionHelper.ModuleList.RoomMaster).ToString() });
            _UDFTables.Add("shipviamaster", new string[] { "ShipViaMaster", ((int)Helper.SessionHelper.ModuleList.ShipViaMaster).ToString() });
            _UDFTables.Add("supplierblankepomaster", new string[] { "SupplierBlankePOMaster", ((int)Helper.SessionHelper.ModuleList.SupplierMaster).ToString() });
            _UDFTables.Add("suppliermaster", new string[] { "SupplierMaster", ((int)Helper.SessionHelper.ModuleList.SupplierMaster).ToString() });
            _UDFTables.Add("technicianmaster", new string[] { "TechnicianMaster", ((int)Helper.SessionHelper.ModuleList.TechnicianMaster).ToString() });
            _UDFTables.Add("toolcategorymaster", new string[] { "ToolCategoryMaster", ((int)Helper.SessionHelper.ModuleList.ToolCategory).ToString() });
            _UDFTables.Add("assetcategorymaster", new string[] { "AssetCategoryMaster", ((int)Helper.SessionHelper.ModuleList.AssetCategory).ToString() });
            _UDFTables.Add("toolmaster", new string[] { "ToolMaster", ((int)Helper.SessionHelper.ModuleList.ToolMaster).ToString() });
            _UDFTables.Add("unitmaster", new string[] { "UnitMaster", ((int)Helper.SessionHelper.ModuleList.UnitMaster).ToString() });
            _UDFTables.Add("enterprises", new string[] { "Enterprise", ((int)Helper.SessionHelper.ModuleList.EnterpriseMaster).ToString() });
            _UDFTables.Add("companies", new string[] { "CompanyMaster", ((int)Helper.SessionHelper.ModuleList.CompanyMaster).ToString() });
            _UDFTables.Add("quicklistmaster", new string[] { "QuickListMaster", ((int)Helper.SessionHelper.ModuleList.QuickListPermission).ToString() });
            _UDFTables.Add("itemmaster", new string[] { "ItemMaster", ((int)Helper.SessionHelper.ModuleList.ItemMaster).ToString() });
            _UDFTables.Add("pullmaster", new string[] { "PullMaster", ((int)Helper.SessionHelper.ModuleList.PullMaster).ToString() });
            _UDFTables.Add("ordermaster", new string[] { "OrderMaster", ((int)Helper.SessionHelper.ModuleList.Orders).ToString() });
            _UDFTables.Add("projectspenditems", new string[] { "ProjectSpendItems", ((int)Helper.SessionHelper.ModuleList.ProjectMaster).ToString() });
            _UDFTables.Add("cartitemlist", new string[] { "CartItemList", ((int)Helper.SessionHelper.ModuleList.Cart).ToString() });
            _UDFTables.Add("kitmaster", new string[] { "KitMaster", ((int)Helper.SessionHelper.ModuleList.Kits).ToString() });
            _UDFTables.Add("requisitionmaster", new string[] { "RequisitionMaster", ((int)Helper.SessionHelper.ModuleList.Requisitions).ToString() });
            _UDFTables.Add("materialstaging", new string[] { "MaterialStaging", ((int)Helper.SessionHelper.ModuleList.Materialstaging).ToString() });
            _UDFTables.Add("vendermaster", new string[] { "VenderMaster", ((int)Helper.SessionHelper.ModuleList.VenderMaster).ToString() });
            _UDFTables.Add("assetmaster", new string[] { "AssetMaster", ((int)Helper.SessionHelper.ModuleList.Assets).ToString() });
            _UDFTables.Add("transfermaster", new string[] { "TransferMaster", ((int)Helper.SessionHelper.ModuleList.Transfer).ToString() });
            _UDFTables.Add("workorder", new string[] { "WorkOrder", ((int)Helper.SessionHelper.ModuleList.WorkOrders).ToString() });
            _UDFTables.Add("toolsscheduler", new string[] { "ToolsScheduler", ((int)Helper.SessionHelper.ModuleList.ToolsScheduler).ToString() });
            _UDFTables.Add("inventorycount", new string[] { "InventoryCount", ((int)Helper.SessionHelper.ModuleList.Count).ToString() });
            _UDFTables.Add("inventorycountlineitem", new string[] { "InventoryCountLineItem", ((int)Helper.SessionHelper.ModuleList.Count).ToString() });
            _UDFTables.Add("toolschedulemapping", new string[] { "toolschedulemapping", ((int)Helper.SessionHelper.ModuleList.AssetToolSchedulerMapping).ToString() });
            _UDFTables.Add("receivedordertransferdetail", new string[] { "ReceivedOrderTransferDetail", ((int)Helper.SessionHelper.ModuleList.Receive).ToString() });
            _UDFTables.Add("toolcheckinouthistory", new string[] { "ToolCheckInOutHistory", ((int)Helper.SessionHelper.ModuleList.AllowCheckInCheckout).ToString() });
            _UDFTables.Add("toolsmaintenancelist", new string[] { "ToolsMaintenanceList", ((int)Helper.SessionHelper.ModuleList.ToolMaster).ToString() });
            _UDFTables.Add("quicklistitems", new string[] { "QuickListItems", ((int)Helper.SessionHelper.ModuleList.QuickListPermission).ToString() });
            _UDFTables.Add("orderdetails", new string[] { "OrderDetails", ((int)Helper.SessionHelper.ModuleList.Orders).ToString() });
            _UDFTables.Add("usermaster", new string[] { "UserMaster", ((int)Helper.SessionHelper.ModuleList.UserMaster).ToString() });
            _UDFTables.Add("bomitemmaster", new string[] { "BOMItemMaster", ((int)Helper.SessionHelper.ModuleList.BOMItemMaster).ToString() });
            _UDFTables.Add("toolassetordermaster", new string[] { "ToolAssetOrder", ((int)Helper.SessionHelper.ModuleList.ToolAssetOrder).ToString() });
            _UDFTables.Add("toolassetorderdetails", new string[] { "ToolAssetOrderDetails", ((int)Helper.SessionHelper.ModuleList.ToolAssetOrder).ToString() });
            _UDFTables.Add("receivedtoolassetordertransferdetail", new string[] { "ReceivedToolAssetOrderTransferDetail", ((int)Helper.SessionHelper.ModuleList.ReceiveToolAsset).ToString() });
            //    HttpContext.Current.Application["_UDFTables"] = _UDFTables;
            //}

            return _UDFTables;
        }
    }

    public class ItemModelPerameter
    {
        public string AjaxURLAddItemToSession { get; set; }
        public string ModelHeader { get; set; }
        public string PerentID { get; set; }
        public string AjaxURLAddMultipleItemToSession { get; set; }
        public string AjaxURLToFillItemGrid { get; set; }
        public string SupplierID { get; set; }
        public string PerentGUID { get; set; }
        public string CallingFromPageName { get; set; }
        public string OrdStagingID { get; set; }
        public string OrdRequeredDate { get; set; }
        public string QuoteRequeredDate { get; set; }
        public string ReqRequiredDate { get; set; }
        public int OrderStatus { get; set; }
        public int QuoteStatus { get; set; }
        public bool IsProjectSpendMandatoryInRoom { get; set; }
        public List<BinAutoComplete> BinAutoComplete { get; set; }
        public QuickListType SelectedQuickListType { get; set; }
        public bool? LoadNarrowSearchAfterListBind { get; set; }
        public bool? AllowPullBeyondAvailableQty { get; set; }
        public RequestType TransferRequestType { get; set; }

        public string StagingBinId { get; set; }
        public Int64 ToolCategoryID { get; set; }
    }
}