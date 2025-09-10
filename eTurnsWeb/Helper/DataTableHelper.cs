using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Controllers;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

public static class DataTableHelper
{




    public static MvcHtmlString RenderColumnsArray(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, string UDFPrefix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);//.OrderBy(e => e.Updated).ToList();
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName).Where(c => c.EnterpriseId == SessionHelper.EnterPriceID).ToList();
        }
        //var result = from c in DataFromDB
        //             select new UDFDTO
        //             {
        //                 ID = c.ID,
        //                 CompanyID = c.CompanyID,
        //                 Room = c.Room,
        //                 UDFTableName = c.UDFTableName,
        //                 UDFColumnName = c.UDFColumnName,
        //                 UDFDefaultValue = c.UDFDefaultValue,
        //                 UDFOptionsCSV = c.UDFOptionsCSV,
        //                 UDFControlType = c.UDFControlType,
        //                 UDFIsRequired = c.UDFIsRequired,
        //                 UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
        //                 Created = c.Created,
        //                 Updated = c.Updated,
        //                 UpdatedByName = c.UpdatedByName,
        //                 CreatedByName = c.CreatedByName,
        //                 IsDeleted = c.IsDeleted,
        //             };

        if (result != null && result.Any() && result.Count() > 0)
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                ColumnsArray.Append("{ \"mDataProp\": \"" + UDFPrefix + i.UDFColumnName + "\", \"sClass\": \"read_only\" }");
                ColumnsArray.Append(",");
            }
        }
        if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
        {
            ColumnsArray.Insert(0, ',');
        }
        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }
    //public static string RenderRazorViewToString(this HtmlHelper htmlHelper, Controller controller, string viewName, object model)
    //{
    //    controller.ViewData.Model = model;
    //    using (var sw = new StringWriter())
    //    {
    //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
    //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
    //        viewResult.View.Render(viewContext, sw);
    //        viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
    //        return sw.GetStringBuilder().ToString();
    //    }
    //}

    //public static string RenderRazorViewToString(this HtmlHelper htmlHelper, Controller controller, string viewName)
    //{
    //    using (var sw = new StringWriter())
    //    {
    //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
    //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
    //        viewResult.View.Render(viewContext, sw);
    //        viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
    //        return sw.GetStringBuilder().ToString();
    //    }
    //}
    public static MvcHtmlString RenderColumnsArrayForDOM(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        IEnumerable<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName);
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated);
        }

        bool UDF1Encrypt = false;
        bool UDF2Encrypt = false;
        bool UDF3Encrypt = false;
        bool UDF4Encrypt = false;
        bool UDF5Encrypt = false;
        if (TableName == "PullMaster")
        {
            UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, TableName, SessionHelper.RoomID);

            foreach (UDFDTO u in DataFromDBUDF)
            {
                if (u.UDFColumnName == "UDF1")
                {
                    UDF1Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF2")
                {
                    UDF2Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF3")
                {
                    UDF3Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF4")
                {
                    UDF4Encrypt = u.IsEncryption ?? false;
                }
                if (u.UDFColumnName == "UDF5")
                {
                    UDF5Encrypt = u.IsEncryption ?? false;
                }
            }
        }
        var ColumnsArray = new StringBuilder();


        foreach (var i in result)
        {
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                ColumnsArray.Append("<td>");

                PropertyInfo info = obj.GetType().GetProperty(UDFPrefix + i.UDFColumnName);
                string strValue = Convert.ToString(info.GetValue(obj, null));
                if (TableName == "PullMaster")
                {
                    if (i.UDFColumnName == "UDF1" && UDF1Encrypt && (!string.IsNullOrWhiteSpace(strValue)))
                    {
                        CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                        strValue = objCommon.GetDecryptValue(strValue);
                    }
                    if (i.UDFColumnName == "UDF2" && UDF2Encrypt && (!string.IsNullOrWhiteSpace(strValue)))
                    {
                        CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                        strValue = objCommon.GetDecryptValue(strValue);
                    }
                    if (i.UDFColumnName == "UDF3" && UDF3Encrypt && (!string.IsNullOrWhiteSpace(strValue)))
                    {
                        CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                        strValue = objCommon.GetDecryptValue(strValue);
                    }
                    if (i.UDFColumnName == "UDF4" && UDF4Encrypt && (!string.IsNullOrWhiteSpace(strValue)))
                    {
                        CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                        strValue = objCommon.GetDecryptValue(strValue);
                    }
                    if (i.UDFColumnName == "UDF5" && UDF5Encrypt && (!string.IsNullOrWhiteSpace(strValue)))
                    {
                        CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                        strValue = objCommon.GetDecryptValue(strValue);
                    }
                }
                ColumnsArray.Append(strValue);
                ColumnsArray.Append("</td>");
            }
        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditable(this HtmlHelper htmlHelper, string TableName)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            StringBuilder sbTemp = new StringBuilder();

            if (i.UDFControlType == "Textbox")
            {
                sbTemp.Append("<input type=\"text\" id=\"" + i.UDFColumnName + "\" class=\"text-boxinner\"  />");
            }
            else
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                sbTemp.Append("<select id=\"" + i.UDFColumnName + "\" class=\"selectBox\">");
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    sbTemp.Append("<option value=\"" + item.ID + "\">" + item.UDFOption + "</option>");
                }
                sbTemp.Append("</select>");
            }

            ColumnsArray.Append("{ \"mDataProp\":  \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\", \"sDefaultContent\": '" + sbTemp.ToString() + "' }");
            ColumnsArray.Append(",");
        }

        if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()))
        {
            ColumnsArray.Insert(0, ',');
        }

        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());

    }

    public static bool GetUdfIsRequiredStatus(this HtmlHelper htmlHelper, string TableName, string UDF)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        return objUDFApiController.GetUDFRequiredStatus(UDF, TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
    }
    public static Int64 GetUFDSetupCountTableWise(this HtmlHelper htmlHelper, string TableName)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        var result = (from c in DataFromDB
                      where c.UDFControlType != null && (!string.IsNullOrWhiteSpace(c.UDFControlType))
                      select new UDFDTO
                      {

                      }).ToList();

        if (result == null || result.Count <= 0)
            return 0;
        else
            return result.Count;
    }


    public static MvcHtmlString RenderColumnsArrayOnDemandEditableObject(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string PushColumnObject = "", bool isUDFOrderRequire = false)
    {
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
        int iUDFMaxLength = 200;

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        if (isUDFOrderRequire && result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();

        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox loadondemandudf' data-value=" + i.ID + " data-udfdefaultvalue='" + i.UDFDefaultValue + "' style='width:93%;'>");
                    //sbTemp.Append("<option></option>");
                    var selectedoption = OBJUDFDATA.Where(x => x.UDFOption == i.UDFDefaultValue).FirstOrDefault();
                    if (selectedoption != null)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + selectedoption.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(selectedoption.UDFOption)) + "</option>");
                    }
                    //foreach (var item in OBJUDFDATA)
                    //{
                    //    if (item.UDFOption == i.UDFDefaultValue)
                    //    {
                    //        sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption)) + "</option>");
                    //    }
                    //    else
                    //    {
                    //        sbTemp.Append("<option value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption)) + "</option>");
                    //    }
                    //}
                    sbTemp.Append("</select>");


                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                }
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input style='width:93%;' type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                    }
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                }

            }
        }

        if (string.IsNullOrEmpty(PushColumnObject))
        {
            if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
            {
                ColumnsArray.Insert(0, ',');
            }
        }

        if (string.IsNullOrEmpty(PushColumnObject) && ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObject(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string PushColumnObject = "", bool isUDFOrderRequire = false)
    {
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
        int iUDFMaxLength = 200;

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //var result = (from c in DataFromDB
        //              select new UDFDTO
        //              {
        //                  ID = c.ID,
        //                  CompanyID = c.CompanyID,
        //                  Room = c.Room,
        //                  UDFTableName = c.UDFTableName,
        //                  UDFColumnName = c.UDFColumnName,
        //                  UDFDefaultValue = c.UDFDefaultValue,
        //                  UDFOptionsCSV = c.UDFOptionsCSV,
        //                  UDFControlType = c.UDFControlType,
        //                  UDFIsRequired = c.UDFIsRequired,
        //                  UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
        //                  Created = c.Created,
        //                  Updated = c.Updated,
        //                  UpdatedByName = c.UpdatedByName,
        //                  CreatedByName = c.CreatedByName,
        //                  IsDeleted = c.IsDeleted,
        //                  UDFMaxLength = c.UDFMaxLength
        //              }).ToList();//.OrderBy(e=>e.Updated).ToList();

        if (isUDFOrderRequire && result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();

        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (TableName == "OrderDetails")
                        {
                            if (item.UDFOption == i.UDFDefaultValue)
                            {
                                sbTemp.Append("<option selected='selected'  value='" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                            }
                            else
                            {
                                sbTemp.Append("<option value='" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                            }
                        }
                        else
                        {
                            if (item.UDFOption == i.UDFDefaultValue)
                            {
                                sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                            }
                            else
                            {
                                sbTemp.Append("<option value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                            }
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                }
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " ")) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " ")) + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input style='width:93%;' type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                    }
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                }

            }
        }

        if (string.IsNullOrEmpty(PushColumnObject))
        {
            if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
            {
                ColumnsArray.Insert(0, ',');
            }
        }

        if (string.IsNullOrEmpty(PushColumnObject) && ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }


    public static MvcHtmlString RenderColumnsArrayEditableObject_PullMasterList(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string PushColumnObject = "")
    {
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
        int iUDFMaxLength = 200;
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        var ColumnsArray = new StringBuilder();

        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " "))) + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");



                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }});");
                    }
                }
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();

                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' onchange='UpdateUDFInPullHistory(this);' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;' onchange='UpdateUDFInPullHistory(this);'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " ")) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption.Replace("\n", " ")) + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox' onchange='UpdateUDFInPullHistory(this);'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input style='width:93%;' type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }

                sbTemp.Append("<input type='hidden' id='hdn" + i.UDFColumnName + "' value='\"+obj.aData." + i.UDFColumnName + "+\"' />");

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        //ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    else
                    {
                        //ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){  return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    ColumnsArray.Append(",");
                }
                else
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass NotHide\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }});");
                    }
                }

            }
        }

        if (string.IsNullOrEmpty(PushColumnObject))
        {
            if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
            {
                ColumnsArray.Insert(0, ',');
            }
        }

        if (string.IsNullOrEmpty(PushColumnObject) && ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }


    public static MvcHtmlString RenderColumnsArrayEditableObject_PullMasterListNew(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string PushColumnObject = "")
    {
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
        int iUDFMaxLength = 200;



        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.Updated).ToList();

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {


                StringBuilder sbTemp = new StringBuilder();
                /*
                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%;' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox' style='width:93%;'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption)) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + Convert.ToString(CommonUtility.htmlEscape(item.UDFOption)) + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' id='" + i.UDFColumnName + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");
                */


                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    sbTemp = new StringBuilder();
                    sbTemp.Append("if (obj.aData." + i.UDFColumnName + " != null && obj.aData." + i.UDFColumnName + " != '') { return '<div class=\"editudf\"></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '<div class=\"editudf\"></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\"></span>'; }");
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide improvement','sDefaultContent':'','fnRender':function (obj, val){  " + sbTemp.ToString() + "; }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only improvement','sDefaultContent':'','fnRender':function (obj, val){  " + sbTemp.ToString() + "; }}");
                    }
                    //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                    ColumnsArray.Append(",");
                }
                else
                {
                    sbTemp = new StringBuilder();
                    sbTemp.Append("if (obj.aData." + i.UDFColumnName + " != null && obj.aData." + i.UDFColumnName + " != '') { return '<div class=\"editudf\"></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '<div class=\"editudf\"></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\"></span>'; }");

                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only NotHide improvement','sDefaultContent':'','fnRender':function (obj, val){  " + sbTemp.ToString() + "; }});");
                    }
                    else
                    {
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({'mDataProp':'" + i.UDFColumnName + "','sClass':'read_only improvement','sDefaultContent':'','fnRender':function (obj, val){  " + sbTemp.ToString() + "; }});");
                    }
                }
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();

                if (string.IsNullOrEmpty(PushColumnObject))
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        sbTemp = new StringBuilder();
                        sbTemp.Append("if (obj.aData." + i.UDFColumnName + " != null && obj.aData." + i.UDFColumnName + " != '') { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\"  data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\"></span>'; }");

                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only NotHide improvement\",'fnRender':function (obj, val){ " + sbTemp.ToString() + "; }}");
                    }
                    else
                    {
                        sbTemp = new StringBuilder();
                        sbTemp.Append("if (obj.aData." + i.UDFColumnName + " != null && obj.aData." + i.UDFColumnName + " != '') { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\" data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\"  data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\"></span>'; }");

                        ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only improvement\",'fnRender':function (obj, val){  " + sbTemp.ToString() + "; }}");
                    }
                    ColumnsArray.Append(",");
                }
                else
                {

                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        sbTemp.Append("if ((obj.aData." + i.UDFColumnName + " != null) && (obj.aData." + i.UDFColumnName + " != '')) { if((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\"  data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '+ obj.aData." + i.UDFColumnName + " +' } } else {if((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\"  data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\"></span>'; } else { return '' } }");
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only NotHide improvement\",'fnRender':function (obj, val) { " + sbTemp.ToString() + ";} });");
                    }
                    else
                    {
                        sbTemp.Append("if ((obj.aData." + i.UDFColumnName + " != null) && (obj.aData." + i.UDFColumnName + " != '')) { if((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return '<div class=\"editudf\"><img src=\"/Content/images/editico.png\" title=\"Edit UDF\" /></div><span class=\"spnimprove\"  data-ctype=\"" + i.UDFControlType + "\" data-udfname=\"" + i.UDFColumnName + "\" data-id=\"" + i.ID.ToString() + "\" data-maxl=\"" + i.UDFMaxLength.GetValueOrDefault(200).ToString() + "\" style=\"position:relative;\">' + obj.aData." + i.UDFColumnName + " +'</span>'; } else { return '+ obj.aData." + i.UDFColumnName + " +' } } else { return ''; }");
                        ColumnsArray.Append("[#]" + PushColumnObject + ".push({ \"mDataProp\": \"" + i.UDFColumnName + "\", \"sClass\": \"read_only improvement\",'fnRender':function (obj, val){ " + sbTemp.ToString() + ";} });");
                    }
                }

            }
        }

        if (string.IsNullOrEmpty(PushColumnObject))
        {
            if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
            {
                ColumnsArray.Insert(0, ',');
            }
        }

        if (string.IsNullOrEmpty(PushColumnObject) && ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }


    public static MvcHtmlString RenderColumnsArrayForPullPO(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string FixedName = "", string ModuleName = "", string SelectedValue = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        //IEnumerable<UDFDTO> DataFromDB;
        var result = new List<UDFDTO>();

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 22;

        result = objUDFApiController.GetNonDeletedPOs(SessionHelper.CompanyID, SessionHelper.RoomID);
        //result = (from c in DataFromDB
        //          select new UDFDTO
        //          {
        //              ID = c.ID,
        //              CompanyID = c.CompanyID,
        //              Room = c.Room,
        //              UDFColumnName = c.UDFColumnName,
        //              UDFDefaultValue = string.Empty,
        //              UDFControlType = c.UDFControlType,
        //              UDFMaxLength = c.UDFMaxLength
        //          }).ToList();


        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {


            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {
                StringBuilder sbTemp = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(FixedName))
                {
                    i.UDFColumnName = FixedName;
                }
                if (i.UDFControlType == "Textbox")
                {
                    if (ModuleName == "Requisition")
                        sbTemp.Append("<input type='text' id='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(SelectedValue)) + "' class='text-boxinner' style='width:93%' maxlength='" + iUDFMaxLength + "' />");
                    else
                        sbTemp.Append("<input type='text' id='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:93%' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    PullMasterDAL objPull = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                    List<DTOForAutoComplete> OBJUDFDATA;

                    OBJUDFDATA = objPull.GetPullOrderNumberForNewPullGrid(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                    if (ModuleName == "")
                        sbTemp.Append("<select id='txtPullOrderNumber' style='width:93%;' class='selectBox'>");
                    else if (ModuleName == "Requisition")
                        sbTemp.Append("<select class='selectBox' style='width:93%;' id='txtPullOrderNumber' >");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (ModuleName == "Requisition" && SelectedValue.ToLower() == item.Key.ToLower())
                        {
                            sbTemp.Append("<option value='" + item.ID + "' selected='true'>" + item.Key.Replace("\n", " ") + "</option>");
                        }
                        else
                            sbTemp.Append("<option value='" + item.ID + "'>" + item.Key.Replace("\n", " ") + "</option>");
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    if (ModuleName == "Requisition" && !string.IsNullOrEmpty(SelectedValue))
                        sbTemp.Append("<input type='text' id='txtPullOrderNumber' name='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(SelectedValue)) + "' class='text-boxinner AutoPullOrderNumber'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    else
                        sbTemp.Append("<input type='text' id='txtPullOrderNumber' name='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner AutoPullOrderNumber'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='ShowAllOptionsOrderNumber' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select'  /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");

                if (ModuleName == "")
                {
                    if (i.UDFIsRequired.GetValueOrDefault(false))
                    {
                        ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    else
                    {
                        ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                    }
                    //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                }
                else
                {
                    ColumnsArray.Append("" + sbTemp.ToString() + "");
                }
                ColumnsArray.Append(",");
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();
                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='txtPullOrderNumber' class='selectBox'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner AutoPullOrderNumber'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='ShowAllOptionsOrderNumber' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\",\"bSortable\": false, \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"" + sbTemp.ToString() + "\";} else{ return obj.aData." + i.UDFColumnName.ToString() + ";} }}");
                ColumnsArray.Append(",");

            }
        }
        if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
        {
            ColumnsArray.Insert(0, ',');
        }
        if (result == null || result.Count <= 0)
        {
            StringBuilder strtemp = new StringBuilder();
            strtemp.Append("<input type='text' id='txtPullOrderNumber' value='' class='text-boxinner' style='width:93%' maxlength='200' />");
            if (ModuleName == "Requisition")
            {
                ColumnsArray.Append("" + strtemp.ToString() + "");
            }
            else
            {
                ColumnsArray.Append("{\"mDataProp\":null,\"bSortable\": false,\"sClass\":\"read_only\",\"sDefaultContent\":\"\",\"fnRender\":function (){ return \"" + strtemp.ToString() + "\"; }}");
            }
            ColumnsArray.Append(",");
        }
        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }


    public static MvcHtmlString RenderColumnsArrayForPullProject(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string FixedName = "")
    {
        ProjectMasterDAL objDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
        List<ProjectMasterDTO> lstDTO = objDAL.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

        ProjectMasterDTO objDTO = objDAL.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        if (objDTO != null && lstDTO != null && lstDTO.Count > 0)
            lstDTO = lstDTO.Where(x => x.ID == objDTO.ID).ToList();
        var ColumnsArray = new StringBuilder();
        StringBuilder sbTemp = new StringBuilder();
        sbTemp.Append("<select id='txtProjectSpent' style='width:93%;' class='selectBox'>");
        sbTemp.Append("<option></option>");
        foreach (var item in lstDTO)
        {
            sbTemp.Append("<option value='" + item.GUID + "'>" + item.ProjectSpendName + "</option>");
        }
        sbTemp.Append("</select>");
        ColumnsArray.Append("{'mDataProp':'txtProjectSpentCol','bSortable': false,'sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
        ColumnsArray.Append(",");

        if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
        {
            ColumnsArray.Insert(0, ',');
        }
        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayForPullPO_PullMaster(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, bool IsObjectRender = true, string FixedName = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        //IEnumerable<UDFDTO> DataFromDB;
        var result = new List<UDFDTO>();

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        result = objUDFApiController.GetNonDeletedPOs(SessionHelper.CompanyID, SessionHelper.RoomID);
        //result = (from c in DataFromDB
        //          select new UDFDTO
        //          {
        //              ID = c.ID,
        //              CompanyID = c.CompanyID,
        //              Room = c.Room,
        //              UDFColumnName = c.UDFColumnName,
        //              UDFDefaultValue = string.Empty,
        //              UDFControlType = c.UDFControlType,
        //              UDFMaxLength = c.UDFMaxLength
        //          }).ToList();


        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {


            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (IsObjectRender)
            {
                StringBuilder sbTemp = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(FixedName))
                {
                    i.UDFColumnName = FixedName;
                }
                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' onchange='UpdatePullOrderNumberInPullHistory(this);' class='text-boxinner' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    PullMasterDAL objPull = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                    List<DTOForAutoComplete> OBJUDFDATA;

                    OBJUDFDATA = objPull.GetPullOrderNumberForNewPullGrid(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                    sbTemp.Append("<select id='txtPullOrderNumber' style='width:93%;' class='selectBox' onchange='UpdatePullOrderNumberInPullHistory(this);'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        sbTemp.Append("<option value='" + item.ID + "'>" + item.Key + "</option>");
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' name='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner AutoPullOrderNumber' onchange='UpdatePullOrderNumberInPullHistory(this);'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;width:93%' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='ShowAllOptionsOrderNumber' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select'  /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' onchange='UpdatePullOrderNumberInPullHistory(this);'/>");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");

                if (i.UDFIsRequired.GetValueOrDefault(false))
                {
                    ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return \"" + sbTemp.ToString() + "\"; }}");
                }
                else
                {
                    bool IsPullInsert = SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                    if (IsPullInsert)
                        ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only improvement','sDefaultContent':'','fnRender':function (obj, val){ if ((obj.aData.ScheduleMode == 6) && (obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"<input type='hidden' id='hdnPullOrderNumber' value='\"+obj.aData.PullOrderNumber +\"' />" + sbTemp.ToString() + "\";} else{ return \"<div id='dveditPullOrderNumber' class='editpullOrder'><img src='/Content/images/editico.png' title='Edit Pull OrderNumber' /></div><span id='spnEditPullOrderNumber' class='spnimprove' data-ctype=\'" + i.UDFControlType + "\' data-maxl=\'" + iUDFMaxLength + "\' style='position: relative'>\"+obj.aData.PullOrderNumber +\"</span>\";  } }}");
                    else
                        ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only','sDefaultContent':'','fnRender':function (obj, val){ if ((obj.aData.ScheduleMode == 6) && (obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"<input type='hidden' id='hdnPullOrderNumber' value='\"+obj.aData.PullOrderNumber +\"' />" + sbTemp.ToString() + "\";} else{ return obj.aData.PullOrderNumber;} }}");
                }
                //ColumnsArray.Append("{ \"mDataProp\": '" + i.UDFColumnName + "', \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }, \"fnRender\": \"" + sbTemp.ToString() + "\" }");
                ColumnsArray.Append(",");
            }
            else
            {
                StringBuilder sbTemp = new StringBuilder();
                if (i.UDFControlType == "Textbox")
                {
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' maxlength='" + iUDFMaxLength + "' />");
                }
                else if (i.UDFControlType == "Dropdown")
                {
                    //Controller objUDF = new UDFApiController();
                    UDFController objUDF = new UDFController();
                    var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                    sbTemp.Append("<select id='txtPullOrderNumber' class='selectBox'>");
                    sbTemp.Append("<option></option>");
                    foreach (var item in OBJUDFDATA)
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                        }
                    }
                    sbTemp.Append("</select>");
                }
                else if (i.UDFControlType == "Dropdown Editable")
                {
                    sbTemp.Append("<span style='position:relative'>");
                    sbTemp.Append("<input type='text' id='txtPullOrderNumber' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner AutoPullOrderNumber'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                    sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='ShowAllOptionsOrderNumber' >");
                    sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' />");
                }
                ColumnsArray.Append("{ \"mDataProp\": \"" + i.UDFColumnName + "\",\"bSortable\": false, \"sClass\": \"read_only\",'fnRender':function (obj, val){ if ((obj.aData.ScheduleMode == 6) && (obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) { return \"<input type='hidden' id='hdnPullOrderNumber' value='\"+obj.aData.PullOrderNumber +\"' />" + sbTemp.ToString() + "\";} else{ return obj.aData.PullOrderNumber;} }}");
                ColumnsArray.Append(",");

            }
        }

        if (string.IsNullOrWhiteSpace(ColumnsArray.ToString()))
        {
            bool IsPullInsert = SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (IsPullInsert)
                ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only NotHide improvement','sDefaultContent':'','fnRender':function (obj, val){ return \"<div id='dveditPullOrderNumber' class='editpullOrder'><img src='/Content/images/editico.png' title='Edit Pull OrderNumber' /></div><span id='spnEditPullOrderNumber' class='spnimprove' data-ctype='Textbox' data-maxl='200' style='position: relative'>\"+obj.aData.PullOrderNumber +\"</span>\"; }}");
            else
                ColumnsArray.Append("{'mDataProp':null,'bSortable': false,'sClass':'read_only NotHide','sDefaultContent':'','fnRender':function (obj, val){ return obj.aData.PullOrderNumber; }}");
            ColumnsArray.Append(",");
        }

        if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()) && PrependComma)
        {
            ColumnsArray.Insert(0, ',');
        }

        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }


    public static MvcHtmlString RenderColumnsArrayForPullOrderMaster(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, string controlIDPostfix = "")
    {

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        //IEnumerable<UDFDTO> DataFromDB;
        var result = new List<UDFDTO>();

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        result = objUDFApiController.GetNonDeletedPOs(SessionHelper.CompanyID, SessionHelper.RoomID);
        //result = (from c in DataFromDB
        //          select new UDFDTO
        //          {
        //              ID = c.ID,
        //              CompanyID = c.CompanyID,
        //              Room = c.Room,
        //              UDFColumnName = c.UDFColumnName,
        //              UDFDefaultValue = string.Empty,
        //              UDFControlType = c.UDFControlType,
        //              UDFMaxLength = c.UDFMaxLength
        //          }).ToList();


        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {


            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }


            StringBuilder sbTemp = new StringBuilder();


            //Controller objUDF = new UDFApiController();
            PullMasterDAL objPull = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            List<DTOForAutoComplete> OBJUDFDATA;

            OBJUDFDATA = objPull.GetPullOrderNumberForNewPullGrid(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            if (!string.IsNullOrEmpty(controlIDPostfix))
            {
                sbTemp.Append("<span id='hdnSpnPullPONumberCommon" + controlIDPostfix + "' style='display: none'></span>");
                sbTemp.Append("<select id='txtPullOrderNumberCommon" + controlIDPostfix + "' style='width:93%;' class='selectBox' >");
            }
            else
            {
                sbTemp.Append("<span id='hdnSpnPullPONumberCommon' style='display: none'></span>");
                sbTemp.Append("<select id='txtPullOrderNumberCommon' style='width:93%;' class='selectBox' >");
            }

            sbTemp.Append("<option></option>");
            foreach (var item in OBJUDFDATA)
            {

                sbTemp.Append("<option value='" + item.ID + "'>" + item.Key + "</option>");

            }
            sbTemp.Append("</select>");

            ColumnsArray.Append(sbTemp.ToString());



        }

        return MvcHtmlString.Create(ColumnsArray.ToString());

    }

    public static MvcHtmlString RenderArrayForPullOrderMaster(this HtmlHelper htmlHelper, string TableName, string ModelValue, bool PrependComma = true, string controlIDPostfix = "")
    {

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        var result = new List<UDFDTO>();
        int iUDFMaxLength = 200;

        result = objUDFApiController.GetNonDeletedPOs(SessionHelper.CompanyID, SessionHelper.RoomID);

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {


            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }


            StringBuilder sbTemp = new StringBuilder();


            //Controller objUDF = new UDFApiController();
            PullMasterDAL objPull = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            List<DTOForAutoComplete> OBJUDFDATA;

            OBJUDFDATA = objPull.GetPullOrderNumberForNewPullGrid(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            if (!string.IsNullOrEmpty(controlIDPostfix))
            {
                sbTemp.Append("<span id='hdnSpnPullPONumberCommon" + controlIDPostfix + "' style='display: none'></span>");
                sbTemp.Append("<select id='PullOrderNumber" + controlIDPostfix + "' style='width:93%;' class='selectBox' >");
            }
            else
            {
                sbTemp.Append("<span id='hdnSpnPullPONumberCommon' style='display: none'></span>");
                sbTemp.Append("<select id='PullOrderNumber' name='PullOrderNumber' style='width:35%;' class='selectBox' >");
            }

            sbTemp.Append("<option></option>");
            foreach (var item in OBJUDFDATA)
            {
                if (item.Key == ModelValue)
                    sbTemp.Append("<option selected='selected' value='" + item.Key + "'>" + item.Key + "</option>");
                else
                    sbTemp.Append("<option value='" + item.Key + "'>" + item.Key + "</option>");

            }
            sbTemp.Append("</select>");

            ColumnsArray.Append(sbTemp.ToString());



        }

        return MvcHtmlString.Create(ColumnsArray.ToString());

    }


    public static MvcHtmlString RenderColumnsArrayForPullProjectspend(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, string controlIDPostfix = "")
    {

        ProjectMasterDAL objDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
        List<ProjectMasterDTO> lstDTO = objDAL.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
        ProjectMasterDTO objDTO = objDAL.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        var ColumnsArray = new StringBuilder();
        StringBuilder sbTemp = new StringBuilder();

        if (objDTO != null && lstDTO != null && lstDTO.Count > 0)
            lstDTO = lstDTO.Where(x => x.ID == objDTO.ID).ToList();

        sbTemp.Append("<span id='hdnSpnPullProjectSpendCommon' style='display: none'></span>");
        sbTemp.Append("<select id='txtProjectSpentCommon' style='width:93%;' class='selectBox' >");
        sbTemp.Append("<option></option>");
        foreach (var item in lstDTO)
        {
            sbTemp.Append("<option value='" + item.GUID + "'>" + item.ProjectSpendName + "</option>");
        }

        sbTemp.Append("</select>");
        ColumnsArray.Append(sbTemp.ToString());
        return MvcHtmlString.Create(ColumnsArray.ToString());

    }

    public static MvcHtmlString RenderColumnsArrayReceivableObjectForDOM(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, string controlIDPostfix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.ID).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append("<td class='td-udfs'>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='true' style='width:80px;' maxlength='" + iUDFMaxLength + "' />");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='false' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;'>");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;'>");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (item.UDFOption == i.UDFDefaultValue)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                    }
                    else
                    {
                        sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }
    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOM(this HtmlHelper htmlHelper, string TableName, bool PrependComma = true, string controlIDPostfix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append("<td class='td-udfs'>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='true' style='width:80px;' maxlength='" + iUDFMaxLength + "' />");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='false' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;'>");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;'>");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (TableName == "OrderDetails")
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + CommonUtility.htmlEscape(item.UDFOption) + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + CommonUtility.htmlEscape(item.UDFOption) + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                    }
                    else
                    {
                        if (item.UDFOption == i.UDFDefaultValue)
                        {
                            sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                        else
                        {
                            sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                        }
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMByName(this HtmlHelper htmlHelper, string TableName, string controlIDPostfix)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.UDFColumnName).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {

            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append("<td class='td-udfs'>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='true' style='width:80px;' maxlength='" + iUDFMaxLength + "' />");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' udfrequired='false' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;'>");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;'>");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (item.UDFOption == i.UDFDefaultValue)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                    }
                    else
                    {
                        sbTemp.Append("<option value='" + item.ID + "'>" + CommonUtility.htmlEscape(item.UDFOption) + "</option>");
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                else
                {
                    sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + controlIDPostfix + "' style='display: none'>" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='width:90px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + Convert.ToString(CommonUtility.htmlEscape(i.UDFDefaultValue)) + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMWithValue(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "", string controlIDPostfix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();

            string strValue = CommonUtility.htmlEscape(i.UDFDefaultValue);
            if (obj != null)
            {
                PropertyInfo info = obj.GetType().GetProperty(UDFPrefix + i.UDFColumnName);
                if (info != null)
                {
                    strValue = Convert.ToString(info.GetValue(obj, null));
                }
            }

            sbTemp.Append("<td>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='true' style='width:80px;'  maxlength='" + iUDFMaxLength + "'  />");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='false' style='width:80px;'   maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;' >");
                }
                else
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;' >");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (item.UDFOption == strValue)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + item.UDFOption + "</option>");
                    }
                    else
                    {
                        sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                else
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMWithValueCombineQL(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "", string controlIDPostfix = "")
    {

        //----------------------------------------------------------------------------------------
        //
        //string MissingUDFs = "";
        string ObjectType = "";
        RequisitionDetailsDTO objRequisitionDetailsDTO = null;
        QuickListDAL objQuickListDAL = null;
        if (obj.GetType() == typeof(RequisitionDetailsDTO))
        {
            objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            objRequisitionDetailsDTO = (RequisitionDetailsDTO)obj;
            ObjectType = "RQ";
        }

        //----------------------------------------------------------------------------------------
        //
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;


        //List<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        List<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        List<UDFDTO> result = (from c in DataFromDB
                               select new UDFDTO
                               {
                                   ID = c.ID,
                                   CompanyID = c.CompanyID,
                                   Room = c.Room,
                                   UDFTableName = c.UDFTableName,
                                   UDFColumnName = c.UDFColumnName,
                                   UDFDefaultValue = c.UDFDefaultValue,
                                   UDFOptionsCSV = c.UDFOptionsCSV,
                                   UDFControlType = c.UDFControlType,
                                   UDFIsRequired = c.UDFIsRequired,
                                   UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                                   Created = c.Created,
                                   Updated = c.Updated,
                                   //UpdatedByName = c.UpdatedByName,
                                   //CreatedByName = c.CreatedByName,
                                   IsDeleted = c.IsDeleted,
                                   IsQuickListUDF = false,
                                   UDFMaxLength = c.UDFMaxLength
                               }).OrderBy(e => e.Updated).ToList();

        //----------------------------------------------------------------------------------------
        //
        //List<UDFDTO> QLResult = objUDFApiController.GetUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID);
        List<UDFDTO> QLResult = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID);

        //----------------------------------------------------------------------------------------
        //
        UDFDTO objUDFDTO = new UDFDTO();
        foreach (UDFDTO i in result)
        {
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            if (i.IsDeleted || String.IsNullOrEmpty(i.UDFControlType))
            {
                objUDFDTO = QLResult.Where(x => (x.IsDeleted == false)
                                            && !String.IsNullOrEmpty(x.UDFControlType)
                                            && x.UDFColumnName.Trim().ToUpper() == i.UDFColumnName.Trim().ToUpper()).FirstOrDefault();
                if (objUDFDTO != null)
                {
                    i.ID = objUDFDTO.ID;
                    i.CompanyID = objUDFDTO.CompanyID;
                    i.Room = objUDFDTO.Room;
                    i.UDFTableName = objUDFDTO.UDFTableName;
                    i.UDFColumnName = objUDFDTO.UDFColumnName;
                    i.UDFDefaultValue = objUDFDTO.UDFDefaultValue;
                    i.UDFOptionsCSV = objUDFDTO.UDFOptionsCSV;
                    i.UDFControlType = objUDFDTO.UDFControlType;
                    i.UDFIsRequired = objUDFDTO.UDFIsRequired;
                    i.UDFIsSearchable = objUDFDTO.UDFIsSearchable;
                    i.Created = objUDFDTO.Created;
                    i.Updated = objUDFDTO.Updated;
                    //i.UpdatedByName = objUDFDTO.UpdatedByName;
                    //i.CreatedByName = objUDFDTO.CreatedByName;
                    i.IsDeleted = objUDFDTO.IsDeleted;
                    i.IsQuickListUDF = true;
                    i.UDFMaxLength = objUDFDTO.UDFMaxLength;
                }
            }
        }

        //----------------------------------------------------------------------------------------
        //
        var ColumnsArray = new StringBuilder();
        bool DrawControl = false;
        foreach (UDFDTO i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            DrawControl = false;
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                if (i.IsQuickListUDF == false)
                {
                    DrawControl = true;
                }
                else if (ObjectType == "RQ" && (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty))
                {
                    DrawControl = true;
                }

                if (DrawControl == true)
                {
                    StringBuilder sbTemp = new StringBuilder();

                    string strValue = CommonUtility.htmlEscape(i.UDFDefaultValue);
                    if (obj != null)
                    {
                        PropertyInfo info = obj.GetType().GetProperty(UDFPrefix + i.UDFColumnName);
                        if (info != null)
                        {
                            strValue = Convert.ToString(info.GetValue(obj, null));
                        }
                    }

                    sbTemp.Append("<td>");
                    if (i.UDFControlType == "Textbox")
                    {
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='true' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='false' style='width:80px;'  maxlength='" + iUDFMaxLength + "'  />");
                        }

                    }
                    else if (i.UDFControlType == "Dropdown")
                    {
                        //Controller objUDF = new UDFApiController();
                        UDFController objUDF = new UDFController();
                        var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                        //bool IsSelectedValueExists = false;
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;' >");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;' >");
                        }
                        sbTemp.Append("<option></option>");
                        foreach (var item in OBJUDFDATA)
                        {
                            if (item.UDFOption == strValue)
                            {
                                //IsSelectedValueExists = true;
                                sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + item.UDFOption + "</option>");
                            }
                            else
                            {
                                sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                            }
                        }

                        if (ObjectType == "RQ")
                        {
                            if (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty)
                            {
                                string QLUDFValue = objQuickListDAL.GetUDFValueOrDefault(objRequisitionDetailsDTO.CompanyID, objRequisitionDetailsDTO.Room, (Guid)objRequisitionDetailsDTO.QuickListItemGUID, 0, i.UDFColumnName);
                                if (!string.IsNullOrEmpty(QLUDFValue) && !string.IsNullOrWhiteSpace(QLUDFValue) && !OBJUDFDATA.Select(x => x.UDFOption).Contains(QLUDFValue))
                                {
                                    if (QLUDFValue == strValue)
                                    {
                                        //IsSelectedValueExists = true;
                                        sbTemp.Append("<option selected='selected'  value='0'>" + QLUDFValue + "</option>");
                                    }
                                    else
                                    {
                                        sbTemp.Append("<option value='0'>" + QLUDFValue + "</option>");
                                    }
                                }
                            }
                        }

                        //if(IsSelectedValueExists == false && !string.IsNullOrEmpty(strValue) && !string.IsNullOrWhiteSpace(strValue))
                        //{
                        //    if(obj.GetType() == typeof(RequisitionDetailsDTO))
                        //    {
                        //        RequisitionDetailsDTO objRequisitionDetailsDTO = (RequisitionDetailsDTO)obj;
                        //        if (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty)
                        //        {
                        //            long objUDFOptionId = 0;
                        //            QuickListDAL objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                        //            if (objQuickListDAL.CheckIfValueExistsInQLUDF(objRequisitionDetailsDTO.CompanyID, objRequisitionDetailsDTO.Room, (Guid)objRequisitionDetailsDTO.QuickListItemGUID, 0, i.UDFColumnName, strValue, out objUDFOptionId))
                        //            {
                        //                sbTemp.Append("<option selected='selected'  value='" + objUDFOptionId.ToString() + "'>" + strValue + "</option>");
                        //            }
                        //        }
                        //    }
                        //}

                        sbTemp.Append("</select>");
                    }
                    else if (i.UDFControlType == "Dropdown Editable")
                    {
                        sbTemp.Append("<span style='position:relative'>");
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                        }

                        if (ObjectType == "RQ")
                        {
                            sbTemp.Append("<input id='hdnAutoCompleteQLGuid" + i.ID + "' type='hidden' value='" + (objRequisitionDetailsDTO.QuickListItemGUID != null ? objRequisitionDetailsDTO.QuickListItemGUID : Guid.Empty).ToString() + "' />");
                        }

                        sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                        sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                    }
                    else
                    {
                        sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' style='width:80px;'  />");
                    }
                    sbTemp.Append("</td>");
                    ColumnsArray.Append(sbTemp.ToString());
                }
                else
                {
                    ColumnsArray.Append("<td></td>");
                }
            }
        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMWithValueCombineQLWithName(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "", string controlIDPostfix = "")
    {

        //----------------------------------------------------------------------------------------
        //
        //string MissingUDFs = "";
        string ObjectType = "";
        RequisitionDetailsDTO objRequisitionDetailsDTO = null;
        QuickListDAL objQuickListDAL = null;
        if (obj.GetType() == typeof(RequisitionDetailsDTO))
        {
            objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            objRequisitionDetailsDTO = (RequisitionDetailsDTO)obj;
            ObjectType = "RQ";
        }

        //----------------------------------------------------------------------------------------
        //
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        List<UDFDTO> result = (from c in DataFromDB
                               select new UDFDTO
                               {
                                   ID = c.ID,
                                   CompanyID = c.CompanyID,
                                   Room = c.Room,
                                   UDFTableName = c.UDFTableName,
                                   UDFColumnName = c.UDFColumnName,
                                   UDFDefaultValue = c.UDFDefaultValue,
                                   UDFOptionsCSV = c.UDFOptionsCSV,
                                   UDFControlType = c.UDFControlType,
                                   UDFIsRequired = c.UDFIsRequired,
                                   UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                                   Created = c.Created,
                                   Updated = c.Updated,
                                   //UpdatedByName = c.UpdatedByName,
                                   //CreatedByName = c.CreatedByName,
                                   IsDeleted = c.IsDeleted,
                                   IsQuickListUDF = false,
                                   UDFMaxLength = c.UDFMaxLength

                               }).OrderBy(e => e.Updated).ToList();

        #region Below code commented due to WI-6312 Pull History, Complete Pull design should proper

        //----------------------------------------------------------------------------------------
        //
        //List<UDFDTO> QLResult = objUDFApiController.GetUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID);

        //----------------------------------------------------------------------------------------
        //
        //UDFDTO objUDFDTO = new UDFDTO();
        //foreach (UDFDTO i in result)
        //{
        //    if (i.IsDeleted || String.IsNullOrEmpty(i.UDFControlType))
        //    {
        //        objUDFDTO = QLResult.Where(x => (x.IsDeleted == false)
        //                                    && !String.IsNullOrEmpty(x.UDFControlType)
        //                                    && x.UDFColumnName.Trim().ToUpper() == i.UDFColumnName.Trim().ToUpper()).FirstOrDefault();
        //        if (objUDFDTO != null)
        //        {
        //            i.ID = objUDFDTO.ID;
        //            i.CompanyID = objUDFDTO.CompanyID;
        //            i.Room = objUDFDTO.Room;
        //            i.UDFTableName = objUDFDTO.UDFTableName;
        //            i.UDFColumnName = objUDFDTO.UDFColumnName;
        //            i.UDFDefaultValue = objUDFDTO.UDFDefaultValue;
        //            i.UDFOptionsCSV = objUDFDTO.UDFOptionsCSV;
        //            i.UDFControlType = objUDFDTO.UDFControlType;
        //            i.UDFIsRequired = objUDFDTO.UDFIsRequired;
        //            i.UDFIsSearchable = objUDFDTO.UDFIsSearchable;
        //            i.Created = objUDFDTO.Created;
        //            i.Updated = objUDFDTO.Updated;
        //            //i.UpdatedByName = objUDFDTO.UpdatedByName;
        //            //i.CreatedByName = objUDFDTO.CreatedByName;
        //            i.IsDeleted = objUDFDTO.IsDeleted;
        //            i.IsQuickListUDF = true;
        //            i.UDFMaxLength = objUDFDTO.UDFMaxLength;
        //        }
        //    }
        //}

        //----------------------------------------------------------------------------------------
        //

        #endregion
        var ColumnsArray = new StringBuilder();
        bool DrawControl = false;
        foreach (UDFDTO i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            DrawControl = false;
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                if (i.IsQuickListUDF == false)
                {
                    DrawControl = true;
                }
                else if (ObjectType == "RQ" && (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty))
                {
                    DrawControl = true;
                }

                if (DrawControl == true)
                {
                    StringBuilder sbTemp = new StringBuilder();

                    string strValue = CommonUtility.htmlEscape(i.UDFDefaultValue);
                    if (obj != null)
                    {
                        PropertyInfo info = obj.GetType().GetProperty(i.UDFColumnName);
                        if (info != null)
                        {
                            strValue = Convert.ToString(info.GetValue(obj, null));
                        }
                    }

                    sbTemp.Append("<td>");
                    if (i.UDFControlType == "Textbox")
                    {
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input type='text' name='[" + UDFPrefix + "]." + i.UDFColumnName + "' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='true' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input type='text' name='[" + UDFPrefix + "]." + i.UDFColumnName + "' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='false' style='width:80px;'  maxlength='" + iUDFMaxLength + "' />");
                        }

                    }
                    else if (i.UDFControlType == "Dropdown")
                    {
                        //Controller objUDF = new UDFApiController();
                        UDFController objUDF = new UDFController();
                        var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                        //bool IsSelectedValueExists = false;
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><select name='[" + UDFPrefix + "]." + i.UDFColumnName + "' id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;' >");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><select name='[" + UDFPrefix + "]." + i.UDFColumnName + "' id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;' >");
                        }
                        sbTemp.Append("<option></option>");
                        foreach (var item in OBJUDFDATA)
                        {
                            if (item.UDFOption == strValue)
                            {
                                //IsSelectedValueExists = true;
                                sbTemp.Append("<option selected='selected'  value='" + item.UDFOption + "'>" + item.UDFOption + "</option>");
                            }
                            else
                            {
                                sbTemp.Append("<option value='" + item.UDFOption + "'>" + item.UDFOption + "</option>");
                            }
                        }

                        if (ObjectType == "RQ")
                        {
                            if (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty)
                            {
                                string QLUDFValue = objQuickListDAL.GetUDFValueOrDefault(objRequisitionDetailsDTO.CompanyID, objRequisitionDetailsDTO.Room, (Guid)objRequisitionDetailsDTO.QuickListItemGUID, 0, i.UDFColumnName);
                                if (!string.IsNullOrEmpty(QLUDFValue) && !string.IsNullOrWhiteSpace(QLUDFValue) && !OBJUDFDATA.Select(x => x.UDFOption).Contains(QLUDFValue))
                                {
                                    if (QLUDFValue == strValue)
                                    {
                                        //IsSelectedValueExists = true;
                                        sbTemp.Append("<option selected='selected'  value='0'>" + QLUDFValue + "</option>");
                                    }
                                    else
                                    {
                                        sbTemp.Append("<option value='0'>" + QLUDFValue + "</option>");
                                    }
                                }
                            }
                        }

                        //if(IsSelectedValueExists == false && !string.IsNullOrEmpty(strValue) && !string.IsNullOrWhiteSpace(strValue))
                        //{
                        //    if(obj.GetType() == typeof(RequisitionDetailsDTO))
                        //    {
                        //        RequisitionDetailsDTO objRequisitionDetailsDTO = (RequisitionDetailsDTO)obj;
                        //        if (objRequisitionDetailsDTO.QuickListItemGUID != null && objRequisitionDetailsDTO.QuickListItemGUID != Guid.Empty)
                        //        {
                        //            long objUDFOptionId = 0;
                        //            QuickListDAL objQuickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                        //            if (objQuickListDAL.CheckIfValueExistsInQLUDF(objRequisitionDetailsDTO.CompanyID, objRequisitionDetailsDTO.Room, (Guid)objRequisitionDetailsDTO.QuickListItemGUID, 0, i.UDFColumnName, strValue, out objUDFOptionId))
                        //            {
                        //                sbTemp.Append("<option selected='selected'  value='" + objUDFOptionId.ToString() + "'>" + strValue + "</option>");
                        //            }
                        //        }
                        //    }
                        //}

                        sbTemp.Append("</select>");
                    }
                    else if (i.UDFControlType == "Dropdown Editable")
                    {
                        sbTemp.Append("<span style='position:relative'>");
                        if ((i.UDFIsRequired ?? false))
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='[" + UDFPrefix + "]." + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                        }
                        else
                        {
                            sbTemp.Append("<span id='hdnSpn" + i.UDFColumnName + "' style='display: none'>" + strValue + "</span><input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='[" + UDFPrefix + "]." + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'   maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                        }

                        if (ObjectType == "RQ")
                        {
                            sbTemp.Append("<input id='hdnAutoCompleteQLGuid" + i.ID + "' type='hidden' value='" + (objRequisitionDetailsDTO.QuickListItemGUID != null ? objRequisitionDetailsDTO.QuickListItemGUID : Guid.Empty).ToString() + "' />");
                        }

                        sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                        sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
                    }
                    else
                    {
                        sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' style='width:80px;'  />");
                    }
                    sbTemp.Append("</td>");
                    ColumnsArray.Append(sbTemp.ToString());
                }
                else
                {
                    ColumnsArray.Append("<td></td>");
                }
            }
        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayEditableObject(this HtmlHelper htmlHelper, string TableName, string UDF1Value, string UDF2Value, string UDF3Value, string UDF4Value, string UDF5Value)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {

            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();
            if (i.UDFControlType == "Textbox")
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + "' class='text-boxinner'  maxlength='" + iUDFMaxLength + "' />");
            }
            else
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                sbTemp.Append("<select id='" + i.UDFColumnName + "' class='selectBox'>");
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                }
                sbTemp.Append("</select>");
            }

            ColumnsArray.Append("{ \"mDataProp\": null, \"sClass\": \"read_only\", \"sDefaultContent\": \"" + sbTemp.ToString() + "\" }");
            ColumnsArray.Append(",");

        }
        //if (!string.IsNullOrWhiteSpace(ColumnsArray.ToString()))
        //{
        //    ColumnsArray.Insert(0, ',');
        //}
        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }




    public static MvcHtmlString RenderColumnsHeader(this HtmlHelper htmlHelper, string TableName)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {
            ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");
        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeader(this HtmlHelper htmlHelper, string TableName, Type model)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        IEnumerable<UDFDTO> result = null;
        if (TableName != "Enterprise" && TableName != "UserMaster")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else if (TableName == "UserMaster")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, 0, 0);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName).Where(c => c.EnterpriseId == SessionHelper.EnterPriceID);
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated);
        }
        //casted the property to its actual type dynamically


        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {

            if (!i.IsDeleted && i.UDFControlType != null)
            {
                //ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");

                //System.Reflection.PropertyInfo propInfo = model.GetProperty(i.UDFColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static);
                //string UDFColumnNmae = Convert.ToString(propInfo.GetValue(model, null));
                //ColumnsHeaderArray.Append(" <th> " + UDFColumnNmae + " </th>");
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room" && TableName.ToLower() != "usermaster")
                {
                    isUDFName = true;
                }
                bool ForEnterPriseSetup = false;
                if (TableName.ToLower() == "usermaster")
                {
                    ForEnterPriseSetup = true;
                }
                if (TableName == "BOMItemMaster")
                {
                    ColumnsHeaderArray.Append(" <th class='tblPullCommonUDFthead'> " + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, false, ForEnterPriseSetup: ForEnterPriseSetup) + " </th>");
                }
                else
                {
                    ColumnsHeaderArray.Append(" <th class='tblPullCommonUDFthead'> " + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + " </th>");
                }

            }

        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeader(this HtmlHelper htmlHelper, string TableName, Type model, string Prefix)
    {

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName).ToList();
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }
        //casted the property to its actual type dynamically
        string prfx = Prefix + " ";

        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {

            if (!i.IsDeleted && i.UDFControlType != null)
            {
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                {
                    isUDFName = true;
                }
                //ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");

                //System.Reflection.PropertyInfo propInfo = model.GetProperty(i.UDFColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static);
                //string UDFColumnNmae = Convert.ToString(propInfo.GetValue(model, null));
                //ColumnsHeaderArray.Append(" <th> " + UDFColumnNmae + " </th>");
                //string udfRequired = @"<input type='hidden' id='hdn" + prfx.Trim().Replace(" ", "") + i.UDFColumnName.Trim().Replace(" ", "") + "_IsRequired' value='" + i.UDFIsRequired.GetValueOrDefault(false) + "' /> ";
                ColumnsHeaderArray.Append("<th>" + prfx + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName) + "</th>");

            }

        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeaderRecieveInnerGrid(this HtmlHelper htmlHelper, string TableName, Type model, string Prefix)
    {

        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        IEnumerable<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName);
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.ID);
        }
        //casted the property to its actual type dynamically
        string prfx = Prefix + " ";

        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {

            if (!i.IsDeleted && i.UDFControlType != null)
            {
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                {
                    isUDFName = true;
                }
                //ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");

                //System.Reflection.PropertyInfo propInfo = model.GetProperty(i.UDFColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static);
                //string UDFColumnNmae = Convert.ToString(propInfo.GetValue(model, null));
                //ColumnsHeaderArray.Append(" <th> " + UDFColumnNmae + " </th>");
                //string udfRequired = @"<input type='hidden' id='hdn" + prfx.Trim().Replace(" ", "") + i.UDFColumnName.Trim().Replace(" ", "") + "_IsRequired' value='" + i.UDFIsRequired.GetValueOrDefault(false) + "' /> ";
                ColumnsHeaderArray.Append("<th>" + prfx + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName) + "</th>");

            }

        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }
    public static MvcHtmlString RenderColumnsHeaderCombineQL(this HtmlHelper htmlHelper, string TableName, Type model, string Prefix)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.Updated).ToList();
        }
        else
        {
            result = objUDFDAL.GetAllRecords(TableName).OrderBy(e => e.Updated).ToList();
        }

        //----------------------------------------------------------------------------------------
        //
        List<UDFDTO> QLResult = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

        //----------------------------------------------------------------------------------------
        //

        UDFDTO objUDFDTO = new UDFDTO();
        foreach (UDFDTO i in result)
        {
            if (i.IsDeleted || String.IsNullOrEmpty(i.UDFControlType))
            {
                objUDFDTO = QLResult.Where(x => (x.IsDeleted == false)
                                            && !String.IsNullOrEmpty(x.UDFControlType)
                                            && x.UDFColumnName.Trim().ToUpper() == i.UDFColumnName.Trim().ToUpper()).FirstOrDefault();
                if (objUDFDTO != null)
                {
                    i.ID = objUDFDTO.ID;
                    i.CompanyID = objUDFDTO.CompanyID;
                    i.Room = objUDFDTO.Room;
                    i.UDFTableName = objUDFDTO.UDFTableName;
                    i.UDFColumnName = objUDFDTO.UDFColumnName;
                    i.UDFDefaultValue = objUDFDTO.UDFDefaultValue;
                    i.UDFOptionsCSV = objUDFDTO.UDFOptionsCSV;
                    i.UDFControlType = objUDFDTO.UDFControlType;
                    i.UDFIsRequired = objUDFDTO.UDFIsRequired;
                    i.UDFIsSearchable = objUDFDTO.UDFIsSearchable;
                    i.Created = objUDFDTO.Created;
                    i.Updated = objUDFDTO.Updated;
                    i.UpdatedByName = objUDFDTO.UpdatedByName;
                    i.CreatedByName = objUDFDTO.CreatedByName;
                    i.IsDeleted = objUDFDTO.IsDeleted;
                    i.IsQuickListUDF = true;
                }
            }
        }

        //------------------------------------------------------------
        //
        //casted the property to its actual type dynamically
        string prfx = Prefix + " ";

        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {

            if (!i.IsDeleted && i.UDFControlType != null)
            {
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                {
                    isUDFName = true;
                }
                //ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");

                //System.Reflection.PropertyInfo propInfo = model.GetProperty(i.UDFColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static);
                //string UDFColumnNmae = Convert.ToString(propInfo.GetValue(model, null));
                //ColumnsHeaderArray.Append(" <th> " + UDFColumnNmae + " </th>");
                //string udfRequired = @"<input type='hidden' id='hdn" + prfx.Trim().Replace(" ", "") + i.UDFColumnName.Trim().Replace(" ", "") + "_IsRequired' value='" + i.UDFIsRequired.GetValueOrDefault(false) + "' /> ";
                if (i.IsQuickListUDF.GetValueOrDefault(false) == true)
                {
                    ColumnsHeaderArray.Append("<th>" + "QL " + eTurnsWeb.Helper.ResourceUtils.GetResource("ResQuickListItems", i.UDFColumnName, isUDFName) + "</th>");
                }
                else
                {
                    ColumnsHeaderArray.Append("<th>" + prfx + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName) + "</th>");
                }

            }

        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeaderRecieveQL(this HtmlHelper htmlHelper, string TableName, Type model, string Prefix)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        List<UDFDTO> result = null;
        if (TableName != "Enterprise")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName).ToList();
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        string prfx = Prefix + " ";

        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {
            if (!i.IsDeleted && i.UDFControlType != null)
            {
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room")
                {
                    isUDFName = true;
                }
                string udfRequired = @"<input type='hidden' id='hdn" + i.UDFColumnName.Trim().Replace(" ", "") + "_IsRequired' value='" + i.UDFIsRequired.GetValueOrDefault(false) + "' /> ";
                ColumnsHeaderArray.Append("<th>" + prfx + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName) + " " + udfRequired + " </th>");
            }
        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeaderByName(this HtmlHelper htmlHelper, string TableName, Type model)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = null;

        if (TableName != "Enterprise" && TableName != "UserMaster")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        }
        else if (TableName == "UserMaster")
        {
            result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, 0, 0);
        }
        else
        {
            eTurnsMaster.DAL.UDFDAL objUDFDAL = new eTurnsMaster.DAL.UDFDAL();
            result = objUDFDAL.GetAllRecords(TableName).Where(c => c.EnterpriseId == SessionHelper.EnterPriceID).ToList();
        }

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.UDFColumnName).ToList();
        }

        //casted the property to its actual type dynamically


        var ColumnsHeaderArray = new StringBuilder();
        foreach (var i in result)
        {

            if (!i.IsDeleted && i.UDFControlType != null)
            {
                //ColumnsHeaderArray.Append(" <th> " + i.UDFColumnName + " </th>");

                //System.Reflection.PropertyInfo propInfo = model.GetProperty(i.UDFColumnName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static);
                //string UDFColumnNmae = Convert.ToString(propInfo.GetValue(model, null));
                //ColumnsHeaderArray.Append(" <th> " + UDFColumnNmae + " </th>");
                bool isUDFName = false;
                if (TableName.ToLower() != "companymaster" && TableName.ToLower() != "enterprise" && TableName.ToLower() != "room" && TableName.ToLower() != "usermaster")
                {
                    isUDFName = true;
                }
                bool ForEnterPriseSetup = false;
                if (TableName.ToLower() == "usermaster")
                {
                    ForEnterPriseSetup = true;
                }
                if (TableName == "BOMItemMaster")
                {
                    ColumnsHeaderArray.Append(" <th class='tblPullCommonUDFthead'> " + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, false, ForEnterPriseSetup: ForEnterPriseSetup) + " </th>");
                }
                else
                {
                    ColumnsHeaderArray.Append(" <th class='tblPullCommonUDFthead'> " + eTurnsWeb.Helper.ResourceUtils.GetResource(model.Name, i.UDFColumnName, isUDFName, ForEnterPriseSetup: ForEnterPriseSetup) + " </th>");
                }

            }

        }
        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsHeaderForLocationDetails(this HtmlHelper htmlHelper, bool IsSerialNumberTracking, bool IsLotNumberTracking, bool IsDateCodeTracking, bool Consignment, bool isAllowConsigned)
    {
        var ColumnsHeaderArray = new StringBuilder();

        ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.ID + " </th>");
        if (IsSerialNumberTracking)
        {
            ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.SerialNumber + " </th>");
        }
        if (IsLotNumberTracking)
        {
            ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.LotNumber + " </th>");
        }
        if (IsDateCodeTracking)
        {
            ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.ExpirationDate + " </th>");
        }
        if (!IsSerialNumberTracking)
        {
            if (Consignment)
            {
                ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.ConsignedQuantity + " </th>");
            }
            else
            {
                ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.CustomerOwnedQuantity + " </th>");
            }
        }
        ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.BinNumber + " </th>");
        ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.Cost + " </th>");
        ColumnsHeaderArray.Append(" <th> " + eTurns.DTO.ResItemLocationDetails.ReceivedDate + " </th>");

        return MvcHtmlString.Create(ColumnsHeaderArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayCommon(this HtmlHelper htmlHelper, string TableName, string ColumnNametoAppend)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //var result = from c in DataFromDB
        //             select new UDFDTO
        //             {
        //                 ID = c.ID,
        //                 CompanyID = c.CompanyID,
        //                 Room = c.Room,
        //                 UDFTableName = c.UDFTableName,
        //                 UDFColumnName = ColumnNametoAppend + c.UDFColumnName,
        //                 UDFDefaultValue = c.UDFDefaultValue,
        //                 UDFOptionsCSV = c.UDFOptionsCSV,
        //                 UDFControlType = c.UDFControlType,
        //                 UDFIsRequired = c.UDFIsRequired,
        //                 UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
        //                 Created = c.Created,
        //                 Updated = c.Updated,
        //                 UpdatedByName = c.UpdatedByName,
        //                 CreatedByName = c.CreatedByName,
        //                 IsDeleted = c.IsDeleted,
        //             };


        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //if (!i.IsDeleted && i.UDFControlType != null)
            //{
            ColumnsArray.Append("ColumnObject.push({ \"mDataProp\": \"" + ColumnNametoAppend + i.UDFColumnName + "\", \"sClass\": \"read_only\" })");
            ColumnsArray.Append(",");
            //}
        }
        if (ColumnsArray.ToString().LastIndexOf(",") != -1)
            return MvcHtmlString.Create(ColumnsArray.ToString().Remove(ColumnsArray.ToString().LastIndexOf(","), 1));
        else
            return MvcHtmlString.Create(ColumnsArray.ToString());
    }
    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMWithValueBinUDF(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "", string controlIDPostfix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();

            string strValue = CommonUtility.htmlEscape(i.UDFDefaultValue);
            if (obj != null)
            {
                PropertyInfo info = obj.GetType().GetProperty(UDFPrefix + i.UDFColumnName);
                strValue = Convert.ToString(info.GetValue(obj, null));
                //PropertyInfo infoBinNumber = obj.GetType().GetProperty("ID");
                //if (info != null && Convert.ToString(infoBinNumber.GetValue(obj, null)) != "0")
                //{
                //    strValue = Convert.ToString(info.GetValue(obj, null));
                //}
            }

            sbTemp.Append("<td>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='true' style='width:80px;'  maxlength='" + iUDFMaxLength + "'  />");
                }
                else
                {
                    sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' udfrequired='false' style='width:80px;'   maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;' >");
                }
                else
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;' >");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (item.UDFOption == strValue)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + item.ID + "'>" + item.UDFOption + "</option>");
                    }
                    else
                    {
                        sbTemp.Append("<option value='" + item.ID + "'>" + item.UDFOption + "</option>");
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                else
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' value='" + strValue + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }
    public static MvcHtmlString RenderColumnsArrayEditableObjectForDOMWithValueBinUDFKO(this HtmlHelper htmlHelper, string TableName, object obj, string UDFPrefix = "", string controlIDPostfix = "")
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);

        //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

        int iUDFMaxLength = 200;

        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }

        var ColumnsArray = new StringBuilder();
        foreach (var i in result)
        {
            //int.TryParse(Convert.ToString(Settinfile.Element("UDFMaxLength").Value), out iUDFMaxLength);
            int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

            if (i.UDFMaxLength != null && i.UDFMaxLength.GetValueOrDefault(0) > 0)
            {
                iUDFMaxLength = i.UDFMaxLength.GetValueOrDefault(0);
            }

            StringBuilder sbTemp = new StringBuilder();

            string strValue = CommonUtility.htmlEscape(i.UDFDefaultValue);
            if (obj != null)
            {
                PropertyInfo info = obj.GetType().GetProperty(UDFPrefix + i.UDFColumnName);
                if (info != null)
                {
                    strValue = Convert.ToString(info.GetValue(obj, null));
                }
            }

            sbTemp.Append("<td>");
            if (i.UDFControlType == "Textbox")
            {
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input type='text' data-bind='value:" + UDFPrefix + i.UDFColumnName + " ' id='" + i.UDFColumnName + controlIDPostfix + "' class='text-boxinner' udfrequired='true' style='width:80px;'  maxlength='" + iUDFMaxLength + "'  />");
                }
                else
                {
                    sbTemp.Append("<input type='text' data-bind='value:" + UDFPrefix + i.UDFColumnName + " ' id='" + i.UDFColumnName + controlIDPostfix + "'  class='text-boxinner' udfrequired='false' style='width:80px;'   maxlength='" + iUDFMaxLength + "' />");
                }

            }
            else if (i.UDFControlType == "Dropdown")
            {
                //Controller objUDF = new UDFApiController();
                UDFController objUDF = new UDFController();
                var OBJUDFDATA = objUDF.GetUDFOptionsByUDFForEditable(i.ID);
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='true' style='width:80px;' data-bind='value:" + UDFPrefix + i.UDFColumnName + "' >");
                }
                else
                {
                    sbTemp.Append("<select id='" + i.UDFColumnName + controlIDPostfix + "' class='selectBox' udfrequired='false' style='width:80px;' data-bind='value:" + UDFPrefix + i.UDFColumnName + "' >");
                }
                sbTemp.Append("<option></option>");
                foreach (var item in OBJUDFDATA)
                {
                    if (item.UDFOption == strValue)
                    {
                        sbTemp.Append("<option selected='selected'  value='" + item.UDFOption + "'>" + item.UDFOption + "</option>");
                    }
                    else
                    {
                        sbTemp.Append("<option value='" + item.UDFOption + "'>" + item.UDFOption + "</option>");
                    }
                }
                sbTemp.Append("</select>");
            }
            else if (i.UDFControlType == "Dropdown Editable")
            {
                sbTemp.Append("<span style='position:relative'>");
                if ((i.UDFIsRequired ?? false))
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='true' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' data-bind='value:" + UDFPrefix + i.UDFColumnName + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                else
                {
                    sbTemp.Append("<input id='hdn" + i.ID + "' type='hidden' value='" + i.ID + "' /><input type='text' udfrequired='false' id='" + i.UDFColumnName + controlIDPostfix + "' name='" + i.UDFColumnName + controlIDPostfix + "' data-bind='value:" + UDFPrefix + i.UDFColumnName + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox'  maxlength='" + iUDFMaxLength + "' style='min-width:80px;' >");
                }
                sbTemp.Append("<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' >");
                sbTemp.Append("<img src='" + VirtualPathUtility.ToAbsolute("~/Content/images/arrow_down_black.png") + "' alt='select' /></a></span>");
            }
            else
            {
                sbTemp.Append("<input type='text' id='" + i.UDFColumnName + controlIDPostfix + "' data-bind='value:" + UDFPrefix + i.UDFColumnName + "' class='text-boxinner' style='width:80px;'  />");
            }
            sbTemp.Append("</td>");
            ColumnsArray.Append(sbTemp.ToString());

        }

        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayCommonNonEditable(this HtmlHelper htmlHelper, string TableName, PullMasterDTO pullMasterDTO)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }
        var ColumnsArray = new StringBuilder();
        if (TableName == "PullMaster")
        {
            foreach (var i in result)
            {
                switch (i.UDFColumnName)
                {
                    case "UDF1":
                        if (pullMasterDTO.isPullUDF1Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF1 + "</span></td>");
                        break;
                    case "UDF2":
                        if (pullMasterDTO.isPullUDF2Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF2 + "</span></td>");
                        break;
                    case "UDF3":
                        if (pullMasterDTO.isPullUDF3Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF3 + "</span></td>");
                        break;
                    case "UDF4":
                        if (pullMasterDTO.isPullUDF4Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF4 + "</span></td>");
                        break;
                    case "UDF5":
                        if (pullMasterDTO.isPullUDF5Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF5 + "</span></td>");
                        break;

                }
            }
        }else if(TableName == "ToolCheckInOutHistory")
        {
            foreach (var i in result)
            {
                switch (i.UDFColumnName)
                {
                    case "UDF1":
                        if (pullMasterDTO.isToolUDF1Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.ToolCheckoutUDF1 + "</span></td>");
                        break;
                    case "UDF2":
                        if (pullMasterDTO.isToolUDF2Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.ToolCheckoutUDF2 + "</span></td>");
                        break;
                    case "UDF3":
                        if (pullMasterDTO.isToolUDF3Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.ToolCheckoutUDF3 + "</span></td>");
                        break;
                    case "UDF4":
                        if (pullMasterDTO.isToolUDF4Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.ToolCheckoutUDF4 + "</span></td>");
                        break;
                    case "UDF5":
                        if (pullMasterDTO.isToolUDF5Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.ToolCheckoutUDF5 + "</span></td>");
                        break;

                }
            }
        }
        return MvcHtmlString.Create(ColumnsArray.ToString());
    }

    public static MvcHtmlString RenderColumnsArrayCommonNonEditable(this HtmlHelper htmlHelper, string TableName, PullMasterViewDTO pullMasterDTO)
    {
        UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(1, TableName);
        List<UDFDTO> result = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain(TableName, SessionHelper.RoomID, SessionHelper.CompanyID);
        if (result != null && result.Any())
        {
            result = result.OrderBy(e => e.Updated).ToList();
        }
        var ColumnsArray = new StringBuilder();
        if (TableName == "PullMaster")
        {
            foreach (var i in result)
            {
                switch (i.UDFColumnName)
                {
                    case "UDF1":
                        if (pullMasterDTO.isPullUDF1Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF1 + "</span></td>");
                        break;
                    case "UDF2":
                        if (pullMasterDTO.isPullUDF2Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF2 + "</span></td>");
                        break;
                    case "UDF3":
                        if (pullMasterDTO.isPullUDF3Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF3 + "</span></td>");
                        break;
                    case "UDF4":
                        if (pullMasterDTO.isPullUDF4Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF4 + "</span></td>");
                        break;
                    case "UDF5":
                        if (pullMasterDTO.isPullUDF5Deleted == false)
                            ColumnsArray.Append("<td><span>" + pullMasterDTO.UDF5 + "</span></td>");
                        break;

                }
            }
        }
        return MvcHtmlString.Create(ColumnsArray.ToString());
    }
}
