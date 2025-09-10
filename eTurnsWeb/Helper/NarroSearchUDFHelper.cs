using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

public static class NarroSearchUDFHelper
{

    public static MvcHtmlString RenderUDFForNarrowSearch(this HtmlHelper htmlHelper, string TableName)
    {
        List<UDFDTO> result = null;
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        eTurnsMaster.DAL.UDFDAL objMasterUDFDAL = new eTurnsMaster.DAL.UDFDAL();
        if (TableName == "EnterpriseMaster")
        {
            result = objMasterUDFDAL.GetAllRecords(TableName).ToList();
        }
        else
        {
            if (TableName == "ProjectList")
                TableName = "ProjectMaster";
            if (TableName == "InventoryCountList")
                TableName = "InventoryCount";
            if (TableName == "KitToolMaster" || TableName == "ToolMasterNew")
                TableName = "ToolMaster";
            if (TableName == "ItemBinMaster")
                TableName = "ItemMaster";
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, eTurnsWeb.Helper.SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        var NarrowUDFs = new StringBuilder();
        foreach (var i in result)
        {
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                if (i.UDFIsSearchable.HasValue ? (bool)i.UDFIsSearchable : false)
                {
                    //if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable"))
                    if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable" || i.UDFControlType == "Textbox"))
                    {
                        bool isUDFName = false;
                        bool ForEnterPriseSetup = false;
                        if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room" && TableName.ToLower() != "usermaster")
                        {
                            isUDFName = true;
                        }
                        if (TableName.ToLower() == "usermaster")
                        {
                            ForEnterPriseSetup = true;
                        }
                        NarrowUDFs.Append("<li attrsortOrder ='20" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        //@if (Model.PageName == "ItemMaster" || Model.PageName == "RequisitionMaster" || Model.PageName == "WorkOrder")
                        if (TableName.ToLower() == "itemmaster" || TableName.ToLower() == "requisitionmaster" || TableName.ToLower() == "workorder" || 
                            TableName.ToLower() == "toolmaster" || TableName.ToLower() == "assetmaster" || TableName.ToLower() == "pullmaster" || TableName.ToLower() == "quotemaster")
                        {
                            NarrowUDFs.Append("<p class=\"handle\"><img  /></p>");
                        }
                        if (TableName == "BOMItemMaster")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, false, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");
                        }
                        else
                        {
                            if (TableName.ToLower() == "pullmaster")
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"pullns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");
                            }
                            else if (TableName.ToLower() == "toolmaster"
                                || TableName.ToLower() == "toolmasternew")
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"toolns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");
                            }
                            else if (TableName.ToLower() == "room" && SessionHelper.RoomID > 0)
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName: true, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");
                            }
                            else
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");
                            }

                        }
                        NarrowUDFs.Append("<div class=\"accordion\" id=\"accordisdfsdfon\">");
                        NarrowUDFs.Append("                <a href='#' class='downarrow'>");
                        NarrowUDFs.Append("                    <img src=\"/Content/images/down-arrow.gif\" alt=\"\" height=\"24\"></a>");
                        NarrowUDFs.Append("                <div class='dropcontent' id=\"" + i.UDFColumnName + "Collapse" + "\">");
                        NarrowUDFs.Append("                </div>");
                        NarrowUDFs.Append("            </div>");
                        NarrowUDFs.Append("</li>");
                    }
                }
            }
        }
        return MvcHtmlString.Create(NarrowUDFs.ToString());
    }

    public static MvcHtmlString RenderUDFForNarrowSearchForItemModel(this HtmlHelper htmlHelper, string TableName, string ControlIdSufix, string ItemModelCallFrom)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        IEnumerable<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, eTurnsWeb.Helper.SessionHelper.RoomID, SessionHelper.CompanyID);

        var NarrowUDFs = new StringBuilder();
        foreach (var i in result)
        {
                if (i.UDFIsSearchable.HasValue ? (bool)i.UDFIsSearchable : false)
                {
                    if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable" || i.UDFControlType == "Textbox"))
                    {
                        bool isUDFName = false;
                        if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                        {
                            isUDFName = true;
                        }
                        if (TableName.ToLower() != "toolcheckinouthistory" && TableName.ToLower() != "technicianmaster")
                        {
                            NarrowUDFs.Append("<li attrsortOrder='20" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        }
                        else
                        {
                            if (TableName.ToLower() == "toolcheckinouthistory")
                            {
                                NarrowUDFs.Append("<li attrsortOrder='30" + i.UDFColumnName.Replace("UDF", "") + "'>");
                            }
                            if (TableName.ToLower() == "technicianmaster")
                            {
                                NarrowUDFs.Append("<li attrsortOrder='40" + i.UDFColumnName.Replace("UDF", "") + "'>");
                            }
                        }
                        if (TableName.ToLower() == "itemmaster" || TableName.ToLower() == "requisitionmaster" || TableName.ToLower() == "workorder" || TableName.ToLower() == "toolmaster" || TableName.ToLower() == "toolcheckinouthistory" || TableName.ToLower() == "technicianmaster"
                            || TableName.ToLower() == "itemcountlist")
                        {
                            NarrowUDFs.Append("<p class=\"handle\"><img  /></p>");
                        }
                        if (TableName.ToLower() == "toolcheckinouthistory"
                            || TableName.ToLower() == "toolmaster")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"toolchkns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        else if (TableName.ToLower() == "technicianmaster")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"tooltechns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        else
                        {
                            if (TableName.ToLower().Equals("itemcountlist"))
                            {
                                TableName = "ItemMaster";
                            }

                            if (!string.IsNullOrEmpty(ItemModelCallFrom) && !string.IsNullOrWhiteSpace(ItemModelCallFrom) && ItemModelCallFrom.ToLower() == "movemtr")
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"movemtr" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                            }
                            else
                            {
                                NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                            }
                            
                        }
                        NarrowUDFs.Append("<div class=\"accordion\" id=\"accordisdfsdfon" + ControlIdSufix + "\">");
                        NarrowUDFs.Append("                <a href='#' class='downarrow'>");
                        NarrowUDFs.Append("                    <img src=\"/Content/images/down-arrow.gif\" alt=\"\" height=\"24\"></a>");
                        NarrowUDFs.Append("                <div class='dropcontent' id=\"" + i.UDFColumnName + "Collapse" + ControlIdSufix + "\">");
                        NarrowUDFs.Append("                </div>");
                        NarrowUDFs.Append("            </div>");
                        NarrowUDFs.Append("</li>");
                    }
                }
            
        }
        return MvcHtmlString.Create(NarrowUDFs.ToString());
    }


    public static MvcHtmlString RenderUDFForNarrowSearchForToolHistory(this HtmlHelper htmlHelper, string TableName, string ControlIdSufix)
    {
        string UDFTableName = TableName;
        if (TableName == "ToolMasterHistoryNew")
        {
            UDFTableName = "ToolMaster";
        }
        if (TableName == "ToolCheckInOutHistoryNew")
        {
            UDFTableName = "ToolCheckInOutHistory";
        }
        if (TableName == "TechnicianMasterNew")
        {
            UDFTableName = "TechnicianMaster";
        }

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        IEnumerable<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(UDFTableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        var NarrowUDFs = new StringBuilder();
        foreach (var i in result)
        {
                if (i.UDFIsSearchable.HasValue ? (bool)i.UDFIsSearchable : false)
                {
                    if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable" || i.UDFControlType == "Textbox"))
                    {
                        bool isUDFName = false;
                        if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                        {
                            isUDFName = true;
                        }
                        if (TableName.ToLower() == "toolmaster")
                            NarrowUDFs.Append("<li attrsortOrderTHL='20" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        else if (TableName.ToLower() == "toolcheckinouthistory")
                            NarrowUDFs.Append("<li attrsortOrderTHL='30" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        else if (TableName.ToLower() == "technicianmaster")
                            NarrowUDFs.Append("<li attrsortOrderTHL='40" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        else
                            NarrowUDFs.Append("<li attrsortOrderTHL='20" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        if (TableName.ToLower() == "itemmaster" || TableName.ToLower() == "requisitionmaster" || TableName.ToLower() == "workorder" || TableName.ToLower() == "toolmaster" || TableName.ToLower() == "toolcheckinouthistory" || TableName.ToLower() == "technicianmaster")
                        {
                            NarrowUDFs.Append("<p class=\"handle\"><img  /></p>");
                        }
                        if (TableName.ToLower() == "toolmaster")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"toolhns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        else if (TableName.ToLower() == "toolcheckinouthistory")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"toolhchkns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        else if (TableName.ToLower() == "technicianmaster")
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" class=\"showloadmore\" data-unique=\"toolhtechns" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        else
                        {
                            NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName) + "_dd" + ControlIdSufix + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist" + ControlIdSufix + "\" size=\"5\"></select>");
                        }
                        NarrowUDFs.Append("<div class=\"accordion\" id=\"accordisdfsdfon" + ControlIdSufix + "\">");
                        NarrowUDFs.Append("                <a href='#' class='downarrow'>");
                        NarrowUDFs.Append("                    <img src=\"/Content/images/down-arrow.gif\" alt=\"\" height=\"24\"></a>");
                        NarrowUDFs.Append("                <div class='dropcontent' id=\"" + i.UDFColumnName + "Collapse" + ControlIdSufix + "\">");
                        NarrowUDFs.Append("                </div>");
                        NarrowUDFs.Append("            </div>");
                        NarrowUDFs.Append("</li>");
                    }
                }
            
        }
        return MvcHtmlString.Create(NarrowUDFs.ToString());
    }

    public static string GetResourceFileName(string TableName)
    {
        switch (TableName)
        {
            case "TechnicianMaster":
                return "ResTechnician";
            case "BinMaster":
                return "ResBin";
            case "CategoryMaster":
                return "ResCategoryMaster";
            case "CustomerMaster":
                return "ResCustomer";
            case "FreightTypeMaster":
                return "ResGLAccount";
            case "GLAccountMaster":
                return "ResGLAccount";
            case "GXPRConsigmentJobMaster":
                return "ResGXPRConsignedJob";
            case "JobTypeMaster":
                return "ResJobType";
            case "LocationMaster":
                return "ResLocation";
            case "ManufacturerMaster":
                return "ResManufacturer";
            case "MeasurementTerm":
                return "ResMeasurementTerm";
            case "ShipViaMaster":
                return "ResShipVia";
            case "SupplierBlankePOMaster":
                return "ResSupplierMaster";
            case "SupplierMaster":
                return "ResSupplierMaster";
            case "ToolCategoryMaster":
                return "ResToolCategory";
            case "AssetCategoryMaster":
                return "ResAssetCategory";
            case "ToolMaster":
                return "ResToolMaster";
            case "EnterpriseMaster":
                return "ResEnterprise";
            case "CompanyMaster":
                return "ResCompany";
            case "RoleMaster":
                return "ResRoleMaster";
            case "ItemMaster":
                return "ResItemMaster";
            case "QuickList":
                return "ResQuickList";
            case "VenderMasterList":
                return "ResVenderMaster";
            case "WorkOrder":
                return "ResWorkOrder";
            case "AssetMaster":
                return "ResAssetMaster";
            case "ProjectList":
                return "ResProjectMaster";
            case "CartItemList":
                return "ResCartItem";
            case "RequisitionMaster":
                return "ResRequisitionMaster";
            case "PullMaster":
                return "ResPullMaster";
            case "AssetToolSchedulerList":
                return "";
            case "ProjectMaster":
                return "ResProjectMaster";
            case "ReceivedOrderTransferDetail":
                return "ResReceiveOrderDetails";
            case "toolschedulemapping":
                return "ResToolsSchedulerMapping";
            case "InventoryCount":
                return "ResInventoryCount";
            case "MaterialStaging":
                return "ResMaterialStaging";
            case "OrderMaster":
                return "ResOrder";
            case "ToolCheckInOutHistory":
                return "ResToolCheckInOutHistory";
            case "Room":
                return "ResRoomMaster";
            case "UserMaster":
                return "ResUserMasterUDF";
            case "BOMItemMaster":
                return "ResBomItemMaster";
        }
        return "";
    }

    public static MvcHtmlString RenderUDFForNarrowSearchForMM(this HtmlHelper htmlHelper, string TableName)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        IEnumerable<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        var NarrowUDFs = new StringBuilder();

        foreach (var i in result)
        {
                if (i.UDFIsSearchable.HasValue ? (bool)i.UDFIsSearchable : false)
                {
                    //if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable"))
                    if ((i.UDFControlType == "Dropdown" || i.UDFControlType == "Dropdown Editable" || i.UDFControlType == "Textbox"))
                    {
                        bool isUDFName = false;
                        bool ForEnterPriseSetup = false;
                        if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room" && TableName.ToLower() != "usermaster")
                        {
                            isUDFName = true;
                        }
                        if (TableName.ToLower() == "usermaster")
                        {
                            ForEnterPriseSetup = true;
                        }
                        NarrowUDFs.Append("<li attrsortOrder ='20" + i.UDFColumnName.Replace("UDF", "") + "'>");
                        NarrowUDFs.Append("<p class='handle'><img /></p>");
                        NarrowUDFs.Append("<select id=\"" + eTurnsWeb.Helper.ResourceUtils.GetResource(GetResourceFileName(TableName), i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + "_dd" + "\" UID=\"" + i.UDFColumnName + "\" multiple=\" multiple\" name=\"udflist\" size=\"5\"></select>");

                        NarrowUDFs.Append("<div class=\"accordion\" id=\"accordisdfsdfon\">");
                        NarrowUDFs.Append("                <a href='#' class='downarrow'>");
                        NarrowUDFs.Append("                    <img src=\"/Content/images/down-arrow.gif\" alt=\"\" height=\"24\"></a>");
                        NarrowUDFs.Append("                <div class='dropcontent' id=\"" + i.UDFColumnName + "Collapse" + "\">");
                        NarrowUDFs.Append("                </div>");
                        NarrowUDFs.Append("            </div>");
                        NarrowUDFs.Append("</li>");
                    }
                }
            
        }
        return MvcHtmlString.Create(NarrowUDFs.ToString());
    }
}
