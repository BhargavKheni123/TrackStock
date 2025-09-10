using eTurns.DAL;
using eTurns.DTO.LabelPrinting;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using eTurns.DTO;

public static class GridHeader
{
    public static MvcHtmlString GridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings, eTurnsWeb.Helper.SessionHelper.ModuleList strModuleValue = 0)
    {
        var GridBuilder = new StringBuilder();

        LabelModuleTemplateDetailDTO objMTDetailDTO = null;
        LabelModuleTemplateDetailDAL objMTDetailDAL = new LabelModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
        IEnumerable<LabelModuleTemplateDetailDTO> lstMTDetailDTO = null;
        bool IsReportView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

        bool IsShowGlobalReprotBuilder = true;
        //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
        string _strIsShowGlobalReprotBuilder = "yes";
        //if (Settinfile.Element("IsShowGlobalReprotBuilder") != null)
        //{
        //    _strIsShowGlobalReprotBuilder = Convert.ToString(Settinfile.Element("IsShowGlobalReprotBuilder").Value);
        //}

        if (eTurns.DTO.SiteSettingHelper.IsShowGlobalReprotBuilder != string.Empty)
        {
            _strIsShowGlobalReprotBuilder = Convert.ToString(eTurns.DTO.SiteSettingHelper.IsShowGlobalReprotBuilder);
        }

        if ((_strIsShowGlobalReprotBuilder ?? string.Empty).ToLower() == "no")
        {
            IsShowGlobalReprotBuilder = false;
        }

        bool IsExportPermission = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);

        //bool IsReportEdit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        if (SessionHelper.EnterPriceID > 0 && SessionHelper.CompanyID > 0)
        {
            //lstMTDetailDTO = objMTDetailDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID);
            lstMTDetailDTO = objMTDetailDAL.GetLabelModuleTemplateDetailByRoomIDCompanyID(SessionHelper.CompanyID, SessionHelper.RoomID);
        }
        else
        {
            lstMTDetailDTO = new List<LabelModuleTemplateDetailDTO>().AsEnumerable();
        }


        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            GridBuilder.Append("<div id=\"divContextMenu\" class=\"ContextMenu\" ><ul>");

            //Show New in context menu only if insert rights 
            if (strModuleValue != 0)
            {
                bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                if (isInsert)
                {
                    if (settings.ColumnSetupFor.ToLower() != "QuoteItemList".ToLower())
                    {
                        GridBuilder.Append("<li><a onclick=\"TabItemClickedbyContext()\">");
                        //GridBuilder.Append("<img title=\"New\" alt=\"New\" src=\"/content/images/refresh.png\"><span>New</span></a></li>");    
                        GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResGridHeader.AddNew + "' src=\"/content/images/drildown_open.jpg\"><span>"+ResGridHeader.AddNew+"</span></a></li>");
                    }
                }
            }
            //Show New in context menu only if insert rights

            //if (settings.ColumnSetupFor == "ReceiveMasterList" && strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Receive)
            //{
            //    GridBuilder.Append("<li><a onclick=\"$('#CloseOderLineItemDialog').modal();$('#divCtxMenu').hide();\">");
            //    GridBuilder.Append("<img title=\"New\" alt=\"New\" src=\"/content/images/refresh.png\"><span>Close Line Item</span></a></li>");
            //}

            if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard") && (settings.ColumnSetupFor ?? string.Empty) != "ToolHistoryList")
                GridBuilder.Append("<li><a id=\"refreshGrid\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            if (settings.DisplaySettings)
            {
                if (settings.ShowReorder)
                {
                    GridBuilder.Append("<li><a id=\"ColumnOrderSetup_Context\">");
                    GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
                }
            }
            if (settings.DisplayUDFButton)
            {
                if (strModuleValue != 0)
                {
                    bool isAllowUDF = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowUDF);
                    if (isAllowUDF)
                    {
                        GridBuilder.Append("<li><a id=\"UDFSetup_Context\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\">\r\n");
                        GridBuilder.Append("<img title=\"" + ResGridHeader.UDFSetup + "\" alt=\"" + ResGridHeader.UDFSetup + "\" src=\"/content/images/udf-setup.png\"><span>" + ResGridHeader.UDFSetup + "</span></a></li>\r\n");
                    }
                }
            }

            GridBuilder.Append("</ul></div>");
        }

        if ((Convert.ToString(strModuleValue) ?? string.Empty).ToString().ToLower().StartsWith("dashboard"))
        {
            GridBuilder.Append("<div class=\"userHead\" style=\"float:right;width:22%;\">\r\n");
        }
        //else if (strModuleValue == SessionHelper.ModuleList.ItemMaster && SessionHelper.AllowABIntegration)
        //{
        //    GridBuilder.Append("<div class=\"userHead\" style=\"height:42px;\">\r\n");
        //}
        else
        {
            GridBuilder.Append("<div class=\"userHead\">\r\n");
        }



        #region "Data View Type"
        if (settings.dataViewType == DataViewType.Both)
        {
            if (settings.PictureViewhref == "PictureViewhref")
            {
                //that means its list page 
                GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
            }
            else if (settings.PictureViewhref == "#")
            {
                //that means its picture view page
                if (settings.ListViewhref == "ItemMasterList" && settings.BinViewhref == "ItemBinMasterView")
                {
                    GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view-active.png\" alt=\"" + ResGridHeader.ItemPictureview + "\" title=\"" + ResGridHeader.ItemPictureview + "\"/></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view-inactive.png\" alt=\"" + ResGridHeader.ItemListView + "\" title=\"" + ResGridHeader.ItemListView + "\" /></a> <a href=\"" + settings.BinViewhref + "\" class=\"view\"><img src=\"/content/images/list-view-inactive.png\" alt=\"" + ResGridHeader.ItembinlistView + "\" title=\"" + ResGridHeader.ItembinlistView + "\" /></a> </div>\r\n");
                }
                else
                {
                    GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view-active.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view-inactive.png\" alt=\"\" /></a></div>\r\n");
                }
            }
            else
            {
                if (settings.ColumnSetupFor == "ItemBinMasterList")
                {
                    GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"" + ResGridHeader.ItemPictureview + "\" title=\"" + ResGridHeader.ItemPictureview + "\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view-inactive.png\" alt=\"" + ResGridHeader.ItemListView + "\" title=\"" + ResGridHeader.ItemListView + "\" /></a> <a href=\"" + settings.BinViewhref + "\" class=\"view\"><img src=\"/content/images/list-view-1.png\" alt=\"" + ResGridHeader.ItembinlistView + "\" title=\"" + ResGridHeader.ItembinlistView + "\" /></a> </div>\r\n");
                }
                else if (settings.ColumnSetupFor == "ItemMasterList" && settings.BinViewhref == "ItemBinMasterView")
                {
                    GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"" + ResGridHeader.ItemPictureview + "\" title=\"" + ResGridHeader.ItemPictureview + "\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"" + ResGridHeader.ItemListView + "\" title=\"" + ResGridHeader.ItemListView + "\" /></a> <a href=\"" + settings.BinViewhref + "\" class=\"view\"><img src=\"/content/images/list-view-inactive-1.png\" alt=\"" + ResGridHeader.ItembinlistView + "\" title=\"" + ResGridHeader.ItembinlistView + "\" /></a> </div>\r\n");
                }
                else
                {
                    GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.PictureViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
                }
            }
        }
        if (settings.dataViewType == DataViewType.ListAndGrouped)
        {
            if (settings.GroupedViewhref == "GroupedViewhref")
            {
                //that means its list page 
                GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.GroupedViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
            }
            else if (settings.GroupedViewhref == "#")
            {
                //that means its picture view page
                GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.GroupedViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view-active.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view-inactive.png\" alt=\"\" /></a></div>\r\n");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.GroupedViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"" + settings.ListViewhref + "\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
            }
        }
        else if (settings.dataViewType == DataViewType.ListView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"" + settings.ListViewhref + "\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.GroupedView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"" + settings.GroupedViewhref + "\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.PictureView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"" + settings.ListViewhref + "\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.None)
        {
            if (settings.ModuleName == "SendEmailList")
            {
                GridBuilder.Append("<div class=\"paginationBlock\" style=\"right:279px !important\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");

                //GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='SendEmailStatus' style='float:left;'><option value='currentFlag'>"+ ResGridHeader.Current +"</option><option value='successFlag'>"+ResCommon.Success+"</option><option value='failFlag'>"+ ResCommon.Fail + "</option></select><input type='button' id='btnReSendEmail' value='" + ResGridHeader.ReSend + "' class='btnGeneral'></input></div>"); 
                //GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style=\"display: none\">&nbsp;</div>");
            }

        }
        else if (settings.dataViewType == DataViewType.radioButtons)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><input id='rdoInventoryLocation' name='ShowStagingLocation' value='1' /><lable for='rdoInventoryLocation'>"+ResMaterialStagingDetail.InventoryLocation+"</lable><input id='rdoInventoryLocation' name='ShowStagingLocation' value='2' /><lable for='rdoStagingLocation'>"+ResMaterialStagingDetail.StagingBinName+"</lable></div>");
        }
        else if (settings.dataViewType == DataViewType.text)
        {
            GridBuilder.Append("<div class=\"viewBlock\">" + settings.TextToDispaly + "</div>");
        }

        #endregion

        if (settings.DisplayGoToPage)
        {
            if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Orders || strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder)
            {
                GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /> <input id=\"btnChangeOrder\" class=\"CreateBtn\" type=\"button\" style=\"margin: 0 25px 0 25px;display:none\" value='"+ResOrder.ChangeOrder+"'> </div>");
            }
            else if (settings.ColumnSetupFor == "WrittenOffToolList")
            {
                GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"WOTPageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"WOTGobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
            }
            //else if (strModuleValue == SessionHelper.ModuleList.ItemMaster && SessionHelper.AllowABIntegration)
            //{
            //    GridBuilder.Append("<div class=\"paginationBlock\" style=\"width:285px;\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /> <div class=\"add-cart\"><a href=\"/Product/ProductList\" title=\"" + ResABProducts.ProductSearch + "\" id=\"aProductSearch\" >" + ResABProducts.ProductSearch +"</a> </div> </div>");
            //}
            else
            {
                GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
            }
        }
        if (settings.ShowCartPageAction)
        {
            //if (settings.ColumnSetupFor.ToLower() == "cartforreturnlist")
            //{
            //    GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='slectreturnaction' style='float:left;'><option value=''>Select</option><option value='5'>Create Suggested Return</option></select><input type='button' id='btnCreateSuggestedReturn' class='btnGeneral' value='Create' /></div>");
            //}
            //else
            //{
            GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='slectAction' style='float:left;'><option value=''>"+ ResCartItem.Select + "</option><option value='3'>"+ResCartItem.CreateTransfers+ "</option><option value='4'>" + ResCartItem.CreateOrders + "</option><option value='5'>" + ResCartItem.CreateReturns + "</option><option value='6'>" + ResCartItem.CreateQuotes + "</option></select><input type='button' id='btnCheckout' class='btnGeneral' value='"+ResCartItem.Create+"' /></div>"); 
            //}
            //<option value='2'>Create manual</option><option value='1'>Create all</option>
        }

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region DisplayPrintBlock
        if (settings.DisplayPrintBlock)
        {
            if (settings.ColumnSetupFor == "RequisitionMaster" || settings.ColumnSetupFor == "WorkOrder" || settings.ColumnSetupFor == "OrderMasterList" || settings.ColumnSetupFor == "PullMaster" || settings.ColumnSetupFor == "ReceiveMasterList" || settings.ColumnSetupFor == "MaterialStaging" ||
               settings.ColumnSetupFor == "InventoryCountList" || settings.ColumnSetupFor == "KitMasterList" || settings.ColumnSetupFor == "EnterpriseQuickList" ||
               settings.ColumnSetupFor == "QuoteMasterList")
            {
                if (settings.ColumnSetupFor == "ReceiveMasterList")
                {
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 6);
                    if (objMTDetailDTO != null || IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div >");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Receivable Items');\"><span>"+ResReceiveOrderDetails.EmailReceivableItems + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Received Items');\"><span>"+ResReceiveOrderDetails.EmailReceivedItems+"</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Receivable Items');\"><span>"+ResReceiveOrderDetails.Receivable+"</span></a>");
                        GridBuilder.Append("</li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Receivable Items','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResReceiveOrderDetails.Receivable + "</span></a>");
                        GridBuilder.Append("</li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Pdf\" onclick=\"ReportExecutionDataInFile('Receivable Items','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResReceiveOrderDetails.Receivable + "</span></a>");
                        GridBuilder.Append("</li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt2\" onclick=\"ReportExecutionData('Received Items');\"><span>"+ResReceiveOrderDetails.Received+"</span></a>");
                        GridBuilder.Append("</li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt2Excel\" onclick=\"ReportExecutionDataInFile('Received Items','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResReceiveOrderDetails.Received + "</span></a>");
                        GridBuilder.Append("</li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt2Pdf\" onclick=\"ReportExecutionDataInFile('Received Items','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResReceiveOrderDetails.Received + "</span></a></li>");
                    }
                    //else
                    //{
                    //    GridBuilder.Append("<li>");
                    //}

                    if (objMTDetailDTO != null)
                    {

                        //                        GridBuilder.Append("</li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,6);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aPrintAttachedDocs\" onclick=\"javascript:return PrintReceivedAttachedDocs(this);\"  href=\"javascript:void(null);\" ><span>" + ResGridHeader.DownloadDocs + "</span></a>");
                    }

                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "OrderMasterList")
                {
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 4);
                    if (objMTDetailDTO != null || IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div >");
                        GridBuilder.Append("<ul>");
                    }
                    //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',false);\"><span>Orders</span></a>");
                    if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Orders)
                    {
                        if (IsReportView)
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('" + settings.ColumnSetupFor + "');\"><span>" + ResCommon.Email + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('" + settings.ColumnSetupFor + "');\"><span>"+ ResLayout.Orders +"</span></a></li>");

                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.Orders + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.Orders + "</span></a></li>");
                        }
                    }
                    else if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder)
                    {
                        if (IsReportView)
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('ReturnOrder');\"><span>" + ResCommon.Email + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('ReturnOrder');\"><span>"+ResLayout.ReturnOrders+"</span></a></li>");

                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('ReturnOrder','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.ReturnOrders + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('ReturnOrder','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.ReturnOrders + "</span></a></li>");
                        }
                    }
                    //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','PDF');\"><img title=\"" + "pdf" + "\" alt=\"" + "pdf" + "\" src=\"/content/images/pdf.png\"><span>Orders</span></a></li>");

                    //GridBuilder.Append("</li>");

                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,4);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a>");
                        GridBuilder.Append("</li>");
                    }
                    if(strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Orders)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aPrintAttachedDocs\" onclick=\"javascript:return PrintAttachedDocs(this);\"  href=\"javascript:void(null);\" ><span>" + ResGridHeader.DownloadDocs + "</span></a>");
                    }
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "QuoteMasterList")
                {
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 4);
                    if (objMTDetailDTO != null || IsReportView)
                    {
                        
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div >");
                        GridBuilder.Append("<ul>");
                    }
                    if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Quote)
                    {
                        if (IsReportView)
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('" + settings.ColumnSetupFor + "');\"><span>" + ResCommon.Email + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('" + settings.ColumnSetupFor + "');\"><span>"+ ResQuoteMaster.Quotes + "</span></a></li>");

                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResQuoteMaster.Quotes + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResQuoteMaster.Quotes + "</span></a></li>");
                        }
                    }
                     
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "MaterialStaging")
                {
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div><div class=\"refresh\">");
                    }
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 7);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,7);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a>");
                        GridBuilder.Append("</li></ul></div>");
                    }
                    GridBuilder.Append("</div>");

                }
                else if (settings.ColumnSetupFor == "WorkOrder")
                {
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 12);
                    if (objMTDetailDTO != null || IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('" + settings.ColumnSetupFor + "');\"><span>" + ResCommon.Email + "</span></a></li>");
                    }
                    else
                    {
                        GridBuilder.Append("<div><div class=\"refresh\">");
                        GridBuilder.Append("<div >");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('" + settings.ColumnSetupFor + "');\"><span>"+ResNarrowSearch.WorkOrder+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResNarrowSearch.WorkOrder + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResNarrowSearch.WorkOrder + "</span></a></li>");
                    }

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,12);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a>");
                        GridBuilder.Append("</li>");
                    }

                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aPrintAttachedDocs\" onclick=\"javascript:return PrintAttachedDocs(this);\"  href=\"javascript:void(null);\" ><span>"+ ResGridHeader.DownloadDocs + "</span></a>"); 
                    GridBuilder.Append("</li>");

                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");
                }
                else if (settings.ColumnSetupFor == "PullMaster")
                {
                    //GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Pull');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Pull');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Pull');\"><span>"+ResLayout.Pull+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Pull','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.Pull + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Pull','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.Pull + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");

                }
                else if (settings.ColumnSetupFor == "RequisitionMaster")
                {
                    if (IsReportView)
                    {
                        // GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Requisition');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Requisition');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Requisition');\"><span>"+ResRequisitionMaster.Requisition+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Requisition','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResRequisitionMaster.Requisition + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Requisition','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResRequisitionMaster.Requisition + "</span></a></li>");
                    }
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aPrintAttachedDocs\" onclick=\"javascript:return PrintAttachedDocs(this);\"  href=\"javascript:void(null);\" ><span>" + ResGridHeader.DownloadDocs + "</span></a>");
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");
                }
                else if (settings.ColumnSetupFor == "InventoryCountList")
                {
                    if (IsReportView)
                    {
                        //GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Inventory Count');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Inventory Count');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Inventory Count');\"><span>"+ResLayout.Count+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Inventory Count','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.Count + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Inventory Count','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.Count + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");
                }
                else if (settings.ColumnSetupFor == "KitMasterList")
                {

                    // GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Requisition');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsExportPermission)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('KitMasterList');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('KitMasterList');\"><span>"+ ResGridHeader.Kit + "</span></a></li>"); 

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('KitMasterList','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResGridHeader.Kit + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('KitMasterList','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResGridHeader.Kit + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");

                }
                else if(settings.ColumnSetupFor == "EnterpriseQuickList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    GridBuilder.Append("</ul></div></div>");
                }
                else
                {
                    if (IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',false);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");

                }

            }
            else
            {
                //GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"callPrint('" + settings.DataTableName + "','" + settings.ColumnSetupFor + "',true);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                if (settings.ColumnSetupFor == "ItemMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('InStock');\"><span>"+ResCommon.Email+"</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Items');\"><span>"+ ResItemMaster.ItemListReport +"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('InStock');\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('InStock','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('InStock','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");
                    }

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','ItemLocationCSV');\">" + ResCommon.ItemLocExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','ItemLocationQtyCSV');\"><span>" + ResCommon.ItemLocQtyExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','KitsCSV');\"><span>" + ResCommon.KitExport + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt2\" onclick=\"ShowGlobalReprotBuilder('OrderedItems',false);\"><span>" + ResCommon.BarCodeLabel + "</span></a>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 2);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,2);\"  href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabelsWithBins\" href=\"javascript:void(0)\"><span>"+ResItemMaster.BinBarcodes+"</span></a></li>"); 
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabelsWithSerialLot\" href=\"javascript:void(0)\"><span>" + ResItemMaster.SerialLotBarcodes + "</span></a></li>");
                    }
                    bool isCatalogReport = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

                    eTurns.DTO.CatalogReportDetailDTO objDetail = new CatalogReportDAL(SessionHelper.EnterPriseDBName).GetDefaultCatalogReport(SessionHelper.CompanyID);
                    if (isCatalogReport && objDetail != null && objDetail.ID > 0)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aItemCatalogReport\" onclick=\"javascript:return OpenCatalogPopup(0,'ItemList');\" href=\"javascript:void(0)\"><span>" + ResItemMaster.CatalogReport + "</span></a></li>");
                    }

                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "BOMItemMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsExportPermission)
                    {
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt2\" onclick=\"ShowGlobalReprotBuilder('OrderedItems',false);\"><span>" + ResCommon.BarCodeLabel + "</span></a>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ItemBinMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('ItemsBinList');\"><span>" + ResItemMaster.ItemListReport + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('InStock');\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('InStock','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('InStock','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResItemMaster.InstockReport + "</span></a></li>");
                    }

                    if (IsExportPermission)
                    {
                        // settings.ColumnSetupFor 'ItemBinMasterList' is replace with 'ItemMasterList' in all
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','ItemLocationCSV');\">" + ResCommon.ItemLocExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','ItemLocationQtyCSV');\"><span>" + ResCommon.ItemLocQtyExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','KitsCSV');\"><span>" + ResCommon.KitExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 2);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,2);\"  href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabelsWithBins\" href=\"javascript:void(0)\"><span>Bin Barcodes</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabelsWithSerialLot\" href=\"javascript:void(0)\"><span>" + ResItemMaster.SerialLotBarcodes + "</span></a></li>");
                    }
                    bool isCatalogReport = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.CatalogReport, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

                    eTurns.DTO.CatalogReportDetailDTO objDetail = new CatalogReportDAL(SessionHelper.EnterPriseDBName).GetDefaultCatalogReport(SessionHelper.CompanyID);
                    if (isCatalogReport && objDetail != null && objDetail.ID > 0)
                    {
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aItemCatalogReport\" onclick=\"javascript:return OpenCatalogPopup(0,'ItemList');\" href=\"javascript:void(0)\"><span>Catalog Report</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aItemCatalogReport\" onclick=\"javascript:return OpenCatalogPopup(1,'ItemBinMasterList');\" href=\"javascript:void(0)\"><span>" + ResItemMaster.CatalogReport + "</span></a></li>");
                    }

                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "TransferMasterList")
                {
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 9);
                    if (objMTDetailDTO != null || IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");

                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");

                        GridBuilder.Append("<div >");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('" + settings.ColumnSetupFor + "');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('" + settings.ColumnSetupFor + "');\"><span>"+ResLayout.Transfer+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.Transfer + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('" + settings.ColumnSetupFor + "','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.Transfer + "</span></a></li>");
                    }
                    //GridBuilder.Append("</ul></div>");
                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 9);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,9);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div >");
                }
                else if (settings.ColumnSetupFor == "AssetMasterList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Assets');\"><span>" + ResCommon.EmailAssets + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Asset Maintenance');\"><span>" + ResCommon.EmailAssetsMaint + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Assets');\"><span>" + ResCommon.RptAssets + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Asset Maintenance');\"><span>" + ResCommon.RptAssetsMaint + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Assets','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.AssetsExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Assets','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.AssetsPDF + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.AssetsMainExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.AssetsMainPDF + "</span></a></li>");
                    }
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 1);
                    if (objMTDetailDTO != null)
                    {

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,1);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ToolList" || settings.ColumnSetupFor == "KitToolMasterList" || settings.ColumnSetupFor == "ToolListNew" || settings.ColumnSetupFor == "KitToolMasterListNew")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Tools Checkout');\"><span>Checkout Tool</span></a></li>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Tools');\"><span>" + ResCommon.EmailTools + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Tools Checked Out');\"><span>" + ResCommon.EmailCheckoutTool + "</span></a></li>");
                        if (settings.ColumnSetupFor == "ToolList")
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Asset Maintenance');\"><span>" + ResCommon.EmailToolMaint + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.ToolListExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.ToolListPDF + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.checkoutToolListExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.checkoutToolListPDF + "</span></a></li>");

                        if (settings.ColumnSetupFor == "ToolList")
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.MaintToolListExcel + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.MaintToolListPDF + "</span></a></li>");
                        }
                    }

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.ExportCheckoutHistory + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolExpCSV\" onclick=\"ExportData('ToolMaster','CSV');\"><span>" + ResCommon.ToolMasterCSVExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolCOStatusCSV\" onclick=\"ExportData('ToolCheckoutStatus','CSV');\"><span>" + ResCommon.ToolCheckoutStatusExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 8);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,8);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                    }

                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "ToolHistoryList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Tools Checkout');\"><span>Checkout Tool</span></a></li>");
                    //if (IsReportView)
                    //{
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools','Excel');\"><img title=\"" + "Excel" + "\" alt=\"" + "Excel" + "\" src=\"/content/images/excel.png\"><span>Tool List</span></a></li>");
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools','PDF');\"><img title=\"" + "pdf" + "\" alt=\"" + "pdf" + "\" src=\"/content/images/pdf.png\"><span>Tool List</span></a></li>");
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','Excel');\"><img title=\"" + "Excel" + "\" alt=\"" + "Excel" + "\" src=\"/content/images/excel.png\"><span>Checkout Tool</span></a></li>");
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','PDF');\"><img title=\"" + "pdf" + "\" alt=\"" + "pdf" + "\" src=\"/content/images/pdf.png\"><span>Checkout Tool</span></a></li>");
                    //}
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Tools');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Tools Checked Out');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Asset Maintenance');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.ToolListExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.ToolListPDF + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.checkoutToolListExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.checkoutToolListPDF + "</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.MaintToolListExcel + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Asset Maintenance','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.MaintToolListPDF + "</span></a></li>");
                    }
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportHistoryData('ToolHistoryList','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportHistoryData('ToolHistoryList','CSV');\"><span>" + ResCommon.ExportCheckoutHistory + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolExpCSV\" onclick=\"ExportData('ToolMaster','CSV');\"><span>" + ResCommon.ToolMasterCSVExport + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolCOStatusCSV\" onclick=\"ExportData('ToolCheckoutStatus','CSV');\"><span>" + ResCommon.ToolCheckoutStatusExport + "</span></a></li>");
                        //objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 8);
                        //if (objMTDetailDTO != null)
                        //{
                        //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,8);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                        //}
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ItemMasterChangeLog")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportHistoryData('ItemMasterChangeLog','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportHistoryData('ItemMasterChangeLog','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "KitMasterList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 3);
                    if (objMTDetailDTO != null)
                    {
                        if (IsReportView)
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('InStock');\"><span>"+ResLayout.Kit+"</span></a></li>");

                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('InStock','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResLayout.Kit + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('InStock','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.Kit + "</span></a></li>");
                        }
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,3);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");
                }
                else if (settings.ColumnSetupFor == "QuickList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 5);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,5);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "ItemLocationPollList" || settings.ColumnSetupFor == "ItemWeightPerPieceList" || settings.ColumnSetupFor == "ResetRequestList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    //objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 5);
                    //if (objMTDetailDTO != null)
                    //{
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,5);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                    //}
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "EVMITareRequestList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    //objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 5);
                    //if (objMTDetailDTO != null)
                    //{
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,5);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                    //}
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "ItemMasterBinList")
                {
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('ItemLocationChangeImport','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('ItemLocationChangeImport','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                        GridBuilder.Append("</ul></div></div>");
                    }
                }
                else if (settings.ColumnSetupFor == "CategoryMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "BinMasterList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 11);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("</li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,11);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a>");
                    }

                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "CustomerMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "InventoryClassificationMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "CostUOMMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "OrderUOMMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "FreightTypeMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "GLAccountMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "SupplierMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Supplier');\"><span>"+ ResGridHeader.SupplierListReport + "</span></a></li>"); 
                    }

                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ShipViaMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "TechnicianList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }

                    objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 10);
                    if (objMTDetailDTO != null)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,10);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a>");
                    }

                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "UnitMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "LocationMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ToolCategoryList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "AssetCategoryList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "VenderMasterList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "ManufacturerMasterList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "MeasurementTermList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView && IsShowGlobalReprotBuilder)
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><span>" + ResCommon.ReportButton + "</span></a></li>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ProjectList")
                {
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Project Spend');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Project Spend');\"><span>"+ResGridHeader.Projects+"</span></a></li>"); 

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Project Spend','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>"+ ResGridHeader.Projects + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Project Spend','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>"+ ResGridHeader.Projects + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "CartItemList")
                {
                    if (IsReportView)
                    {
                        //GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Suggested Orders');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    else
                    {
                        GridBuilder.Append("<div ><div class=\"refresh\">");
                        GridBuilder.Append("<div class=\"refreshBlock\">");
                        GridBuilder.Append("<ul>");
                    }
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Suggested Orders');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li id='liSuggestedOrderPrint' class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Suggested Orders');\"><span>"+ResCartItem.SuggestedOrders+"</span></a></li>");

                        GridBuilder.Append("<li id='liSuggestedOrderPrintExcel' class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Suggested Orders','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCartItem.SuggestedOrders + "</span></a></li>");
                        GridBuilder.Append("<li id='liSuggestedOrderPrintPDF' class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Suggested Orders','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCartItem.SuggestedOrders + "</span></a></li>");

                        #region For Return Order

                        GridBuilder.Append("<li id='liSuggestedReturnPrint' class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Suggested Orders');\"><span>" + ResCartItem.SuggestedReturns + "</span></a></li>");

                        GridBuilder.Append("<li id='liSuggestedReturnPrintExcel' class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Suggested Orders','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCartItem.SuggestedReturns + "</span></a></li>");
                        GridBuilder.Append("<li id='liSuggestedReturnPrintPDF' class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Suggested Orders','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCartItem.SuggestedReturns + "</span></a></li>");

                        #endregion
                    }
                    GridBuilder.Append("</ul></div>");
                    GridBuilder.Append("</div>");

                }
                else if (settings.ColumnSetupFor == "RoomList")
                {
                    GridBuilder.Append("<div id=\"divDeletedRoomPrint\" style=\"display:none;\" class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('DeletedRoom','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('DeletedRoom','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div></div>");

                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Room');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div >");
                    }
                }
                else if (settings.ColumnSetupFor == "BarcodeItemMasterList" || settings.ColumnSetupFor == "BarcodeAssetMasterList")
                {
                    if (IsExportPermission)
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ExportData('BarcodeMasterCSV','CSV');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                }
                else if (settings.ColumnSetupFor == "CompanyList")
                {
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Company');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div >");
                    }
                }
                else if (settings.ColumnSetupFor == "EnterpriseList")
                {
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Enterprises List');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div >");
                    }
                }
                else if (settings.ColumnSetupFor == "UserList")
                {
                    if (IsReportView)
                    {
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ReportExecutionData('Users');\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div >");
                    }
                }
                else if ((settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsExportPermission)
                    {
                        if (settings.ExportDashboardFromReportFile == true)
                        {
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportDataForDashboardByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','Excel', true);\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpPDF\" onclick=\"ExportDataForDashboardByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','PDF', true);\"><span>" + ResGridHeader.PDFExport + "</span></a></li>");
                        }
                        else
                        {
                            if (settings.ColumnSetupFor == "DashboardItemMasterList")
                            {
                                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportDataForDashboardItemByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','Excel', false);\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportDataForDashboardItemByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','CSV', false);\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                            }
                            else
                            {
                                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportDataForDashboardByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','Excel', false);\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportDataForDashboardByTableName('" + settings.DataTableName + "', '" + settings.ExportModuleName + "','CSV', false);\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                            }
                        }
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ItemMasterChangeLog")
                {
                    //GridBuilder.Append("<div class=\"print\"><a onclick=\"PrintChangeLog('" + settings.ColumnSetupFor + "')\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkPrintAllColm\" onclick=\"PrintChangeLog('" + settings.ColumnSetupFor + "');\"><span>"+ResGridHeader.Print+"</span></a></li>");
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkPrintChangeColm\" onclick=\"PrintChangeLog('" + settings.ColumnSetupFor + "','True');\"><span>"+ResGridHeader.PrintChangedonly+"</span></a></li>");

                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "WrittenOffToolList")
                {

                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('WrittenOffTools');\"><span>"+ResGridHeader.WrittenOffToolsReport+"</span></a></li>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('WrittenOffTools','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>"+ResLayout.ToolList+"</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('WrittenOffTools','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResLayout.ToolList + "</span></a></li>");
                    }
                    if (IsExportPermission)
                    {
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportWrittenOffToolData('WrittenOffToolList','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportWrittenOffToolData('WrittenOffToolList','CSV');\"><span>" + ResCommon.ExportCheckoutHistory + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolExpCSV\" onclick=\"ExportData('ToolMaster','CSV');\"><span>" + ResCommon.ToolMasterCSVExport + "</span></a></li>");
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolCOStatusCSV\" onclick=\"ExportData('ToolCheckoutStatus','CSV');\"><span>" + ResCommon.ToolCheckoutStatusExport + "</span></a></li>");
                        //objMTDetailDTO = lstMTDetailDTO.FirstOrDefault(x => x.ModuleID == 8);
                        //if (objMTDetailDTO != null)
                        //{
                        //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"aBarcodeLabels\" onclick=\"javascript:return OpenBarcodeLabels(this,8);\" href=\"javascript:void(null);\" ><span>" + ResCommon.BarCodeLabel + "</span></a></li>");
                        //}
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "NotificationMasterList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");

                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");

                    }
                    GridBuilder.Append("</ul></div></div>");
                }
                else if (settings.ColumnSetupFor == "ToolsMaintenanceList")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Maintenance Due');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Maintenance Due');\"><span>"+ResGridHeader.MaintenanceHistory+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Maintenance Due','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResGridHeader.MaintenanceHistory + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Maintenance Due','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResGridHeader.MaintenanceHistory + "</span></a></li>");
                    }
                    if (IsExportPermission)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    GridBuilder.Append("</ul></div></div>");

                }
                else if (settings.ColumnSetupFor == "ToolsMaintenanceListDue")
                {
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<div class=\"refreshBlock\">");
                    GridBuilder.Append("<ul>");
                    if (IsReportView)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Email\" onclick=\"SendReportInMail('Maintenance Due');\"><span>" + ResCommon.Email + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Maintenance Due');\"><span>"+ResCommon.MaintenanceDue+"</span></a></li>");

                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Maintenance Due','Excel');\"><img title=\"" + ResGridHeader.Excel + "\" alt=\"" + ResGridHeader.Excel + "\" src=\"/content/images/excel.png\"><span>" + ResCommon.MaintenanceDue + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Maintenance Due','PDF');\"><img title=\"" + ResGridHeader.PDF + "\" alt=\"" + ResGridHeader.PDF + "\" src=\"/content/images/pdf.png\"><span>" + ResCommon.MaintenanceDue + "</span></a></li>");
                    }
                    //if (IsExportPermission)
                    //{
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                    //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    //}
                    GridBuilder.Append("</ul></div></div>");

                }
                else
                {
                    if (IsReportView 
                        && IsShowGlobalReprotBuilder 
                        && settings.ColumnSetupFor != "AssetToolSchedulerList"
                        && settings.ColumnSetupFor != "ToolScheduleMappingList"
                        && settings.ColumnSetupFor != "ReceiveOrderStatusGrid")
                    {
                        GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\" onclick=\"ShowGlobalReprotBuilder('" + settings.ColumnSetupFor + "',true);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<div>");
                    }
                }
            }

        }
        else
        {
            GridBuilder.Append("<div class=\"print\">");
        }

        #endregion

        //GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        if (settings.DisplaySaveButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"saveRows\"><img src=\"/content/images/save.png\" alt=\"\" /></a>");
        }

        if (settings.ShowSelectAll)
        {
            if (settings.ReorderByDataTableName)
            {
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"" + settings.DataTableName + "_SelectAll\" class=\"clsSelectAllByDataTableName\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' onclick=\"SelectAllByDataTableNam('" + settings.DataTableName + "')\" /></a>");
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"" + settings.DataTableName + "_DeSelectAll\" class=\"clsDeselectAllByDataTableName\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' onclick=\"DeselectAllByDataTableNam('" + settings.DataTableName + "')\" /></a>");
            }
            else if (strModuleValue == SessionHelper.ModuleList.AllowToolWrittenOff && settings.ColumnSetupFor == "WrittenOffToolDetail")
            {
                GridBuilder.Append("<span class='label' style='float:left;' >" + ResGridHeader.GoToPage + "</span><input type='text' id='InnerGridPageNumber' class='inputNum' style='float:left;' /><input type='button' id='InnerGridGobtn' class='go' value='" + ResGridHeader.Go+ "' style='float:left;' />");
                GridBuilder.Append("<a style='float:right;' title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAll\" class=\"WOTDetailclsactionSelectAll\" onclick=\"SelectWrittenOffToolDetail('" + HttpContext.Current.Session["WOTDetailToolGuid"] + "')\" data-WOTDToolGuid='" + HttpContext.Current.Session["WOTDetailToolGuid"] + "' ><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
                GridBuilder.Append("<a title='" + ResCommon.SelectAll+ "' style='display:none;float:right;' href=\"javascript:void(null);\" id=\"actionDeSelectAll\" class=\"WOTDetailclsactionDeSelectAll\" onclick=\"DeselectWrittenOffToolDetail('" + HttpContext.Current.Session["WOTDetailToolGuid"] + "')\" data-WOTDToolGuid='" + HttpContext.Current.Session["WOTDetailToolGuid"] + "' ><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            }
            else
            {
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAll\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAll\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            }
        }

        if (settings.ShowCloseButton)
        {
            if (settings.ColumnSetupFor == "InventoryCountList") 
                GridBuilder.Append("<a title='"+ ResInventoryCount.CloseSelectedCounts + "' href=\"javascript:void(null);\" id=\"actionCloseItems\"><img src=\"/content/images/closeitms.png\" alt='" + ResInventoryCount.CloseSelectedCounts + "' /></a>");
            else if (settings.ColumnSetupFor == "OrderMasterList")
                GridBuilder.Append("<a title='"+ ResOrder.CloseSelectedOrders + "' href=\"javascript:void(null);\" id=\"actionCloseOrder\"><img src=\"/content/images/closeitms.png\" alt='"+ ResOrder.CloseSelectedOrders + "' /></a>");
            else if (settings.ColumnSetupFor == "WorkOrder")
                GridBuilder.Append("<a title='"+ ResWorkOrder.CloseSelectedWorkorders + "' href=\"javascript:void(null);\" id=\"actionCloseWorkOrder\"><img src=\"/content/images/closeitms.png\" alt='" + ResWorkOrder.CloseSelectedWorkorders + "' /></a>");
            else if (settings.ColumnSetupFor == "QuoteMasterList") 
                GridBuilder.Append("<a title='"+ ResQuoteMaster.CloseSelectedQuotes + "' href=\"javascript:void(null);\" id=\"actionCloseQuote\"><img src=\"/content/images/closeitms.png\" alt='"+ ResQuoteMaster.CloseSelectedQuotes + "' /></a>");
            else if (settings.ColumnSetupFor == "TransferMasterList") 
                GridBuilder.Append("<a title='" + ResTransfer.CloseSelectedTransfers + "' href=\"javascript:void(null);\" id=\"actionCloseTransfer\"><img src=\"/content/images/closeitms.png\" alt='" + ResTransfer.CloseSelectedTransfers + "' /></a>");
            else if (settings.ColumnSetupFor == "RequisitionMaster")
                GridBuilder.Append("<a title='" + ResRequisitionMaster.CloseSelectedRequisition + "' href=\"javascript:void(null);\" id=\"actionCloseRequisition\"><img src=\"/content/images/closeitms.png\" alt='" + ResRequisitionMaster.CloseSelectedRequisition + "' /></a>");
            else
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionCloseItems\"><img src=\"/content/images/closeitms.png\" alt='"+ResWorkOrder.Close+"' /></a>");
        }

        if (settings.ShowCopyButton)
        {
            if (settings.ColumnSetupFor.ToLower() == "templateconfigurationlist") 
                GridBuilder.Append("<a title='"+ ResLabelPrinting.CopyLabelTemplate + "' href=\"javascript:void(null);\" id=\"actionCopyItems\"><img src=\"/content/images/copy.png\" alt='" + ResLabelPrinting.CopyLabelTemplate + "' /></a>");
        }



        if (settings.DisplayDeleteButton && strModuleValue != 0 && settings.ShowDelete)
        {
            bool isDelete = strModuleValue == SessionHelper.ModuleList.AllowToolWrittenOff
                    ? eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowToolWrittenOff)
                    : eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);

            if (isDelete)
            {
                if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Kits)
                    GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteKitRows\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                else if (strModuleValue == SessionHelper.ModuleList.AllowToolWrittenOff && settings.ColumnSetupFor == "WrittenOffToolList")
                {
                    GridBuilder.Append("<a title='"+ResToolMaster.Delete+"' href=\"javascript:void(null);\" id=\"deleteWrittenOffTool\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<a title='"+ eTurns.DTO.ResToolMaster.UnwrittenOffTool + "' href=\"javascript:void(null);\" id=\"UnwrittenOffTools\"><img src=\"/content/images/eraser-icon.png\" alt=\"\" /></a>");
                }
                else if (strModuleValue == SessionHelper.ModuleList.AllowToolWrittenOff && settings.ColumnSetupFor == "WrittenOffToolDetail")
                {
                    GridBuilder.Append("<a style='float:right;' title='" + ResToolMaster.Delete+"' href=\"javascript:void(null);\" class=\"deleteWrittenOffToolDetail\" onclick=\"deleteWrittenOffToolDetail('" + HttpContext.Current.Session["WOTDetailToolGuid"] + "')\" ><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                }
                //else if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.QuickListPermission)
                //    GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteQuickListRows\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                else
                {
                    GridBuilder.Append("<a title='"+ResToolMaster.Delete+"' href=\"javascript:void(null);\" id=\"deleteRows\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                    GridBuilder.Append("<a title='"+ResGridHeader.UndeleteRecord+"' href=\"javascript:void(null);\" style=display:none id=\"undeleteRows\"><img src=\"/content/images/unDelete.png\" alt=\"\" /></a>");
                }

            }

        }

        if (settings.DisplayArchiveButton && strModuleValue != 0)
        {
            bool isArchive = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
            if (isArchive)
            {

                GridBuilder.Append("<a title='"+ResGridHeader.Archive+"' href=\"javascript:void(null);\" id=\"aArchiveRows\"><img src=\"/content/images/Archive.png\" alt=\"\" /></a>");
                GridBuilder.Append("<a title='"+ResGridHeader.UnArchive+"' href=\"javascript:void(null);\" style=display:none id=\"aUnArchiveRows\"><img src=\"/content/images/UnArchive.png\" alt=\"\" /></a>");
            }

        }
        //if (settings.DisplayRefreshButton)
        //{
        //    GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"refreshGrid\"><img src=\"/content/images/refresh-icon.png\" alt=\"\" /></a>");
        //}


        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton && !(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
        {
            if (settings.ColumnSetupFor == "ToolHistoryList")
            {
                GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
                //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");            
                GridBuilder.Append("<div id=\"ColumnSortableModalTHL\" style=\"display: none;\">");
                GridBuilder.Append("<div class=\"sortableContainer\">");
                GridBuilder.Append("<ul id=\"ColumnSortableTHL\">");
                GridBuilder.Append("</ul>");
                GridBuilder.Append("</div>");

                GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrderTHL\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderToolHistory('ToolHistoryList')\" />");

                GridBuilder.Append("</div>");
                GridBuilder.Append("</div>");
            }
            else if (settings.ColumnSetupFor == "ItemMasterChangeLog")
            {
                GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
                //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");            
                GridBuilder.Append("<div id=\"ColumnSortableModalICL\" style=\"display: none;\">");
                GridBuilder.Append("<div class=\"sortableContainer\">");
                GridBuilder.Append("<ul id=\"ColumnSortableICL\">");
                GridBuilder.Append("</ul>");
                GridBuilder.Append("</div>");

                GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrderICL\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderItemChangeLog('ItemMasterHistoryList')\" />");

                GridBuilder.Append("</div>");
                GridBuilder.Append("</div>");
            }
            else if (settings.ColumnSetupFor == "WrittenOffToolList")
            {
                GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
                GridBuilder.Append("<div id=\"ColumnSortableModalWOT\" style=\"display: none;\">");
                GridBuilder.Append("<div class=\"sortableContainer\">");
                GridBuilder.Append("<ul id=\"ColumnSortableWOT\">");
                GridBuilder.Append("</ul>");
                GridBuilder.Append("</div>");

                GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrderWOT\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderWrittenOffToolList('WrittenOffToolList')\" />");

                GridBuilder.Append("</div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
                //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");            
                GridBuilder.Append("<div id=\"ColumnSortableModal\" style=\"display: none;\">");
                GridBuilder.Append("<div class=\"sortableContainer\">");
                GridBuilder.Append("<ul id=\"ColumnSortable\">");
                GridBuilder.Append("</ul>");
                GridBuilder.Append("</div>");
                if (settings.ShowReorder)
                {
                    if ((settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
                    {
                        GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateDashboardColumnOrder()\" />");
                    }
                    else if ((settings.ColumnSetupFor ?? string.Empty) == "ToolHistoryList")
                    {
                        GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderToolHistory('ToolHistoryList')\" />");
                    }
                    else if ((settings.ColumnSetupFor ?? string.Empty) == "ItemMasterChangeLog")
                    {
                        GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderToolHistory('ItemMasterChangeLog')\" />");
                    }
                    else
                    {
                        GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrder('" + settings.ColumnSetupFor + "')\" />");
                    }
                }
                GridBuilder.Append("</div>");
                GridBuilder.Append("</div>");
            }
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersetting\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");
            if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard") && (settings.ColumnSetupFor ?? string.Empty) != "ToolHistoryList")
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"refreshGrid\"><img src=\"/content/images/refresh.png\" alt=\"" + ResGridHeader.Refresh + "\" title=\"" + ResGridHeader.Refresh + "\"><span>" + ResGridHeader.Refresh + "</span></a>");
                GridBuilder.Append("</li>");
                GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"GridAutoRefresh\" type=\"checkbox\"  title='"+ ResGridHeader.AutoRefresh + "'>" + ResGridHeader.AutoRefresh + "</input>");
                GridBuilder.Append("</li>");
            }

            if (strModuleValue != 0)
            {
                bool isShowDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowDeleted);
                if (settings.ColumnSetupFor.ToLower() != "roomlist")
                {
                    if (settings.ColumnSetupFor.ToLower() == "itembinmasterlist"
                        || settings.ColumnSetupFor.ToLower() == "quoteitemlist")
                        isShowDelete = false;

                    if (isShowDelete)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsDeletedRecords\" type=\"checkbox\" name=\"FilterActions\" GridFilter=\"true\" >" + ResGridHeader.IncludeDeleted + "</input>");
                        GridBuilder.Append("</li>");
                    }
                }
                else
                {
                    if (isShowDelete && ((SessionHelper.UserType == 1) || (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)))
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsDeletedRecords\" type=\"checkbox\" name=\"FilterActions\" GridFilter=\"true\" >" + ResGridHeader.IncludeDeleted + "</input>");
                        GridBuilder.Append("</li>");
                    }
                }
                bool isShowArchived = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowArchived);
                if (settings.ColumnSetupFor.ToLower() != "roomlist")
                {
                    if (settings.ColumnSetupFor.ToLower() == "quoteitemlist")
                        isShowArchived = false;

                    if (isShowArchived && settings.ColumnSetupFor.ToLower() != "quotemasterlist")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsArchivedRecords\" type=\"checkbox\" name=\"FilterActions\" GridFilter=\"true\" >" + ResGridHeader.IncludeArchived + "</input>");
                        GridBuilder.Append("</li>");
                    }
                }
                else
                {
                    if (isShowArchived && ((SessionHelper.UserType == 1) || (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)) && settings.ColumnSetupFor.ToLower() != "quotemasterlist")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsArchivedRecords\" type=\"checkbox\" name=\"FilterActions\" GridFilter=\"true\" >" + ResGridHeader.IncludeArchived + "</input>");
                        GridBuilder.Append("</li>");
                    }
                }

            }
            if (settings.ShowReorder)
            {
                if (settings.ReorderByDataTableName == true)
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupByDataTableName\" onclick=\"OpenReorderPopupByDataTableName('" + settings.DataTableName + "', '" + settings.ColumnSetupFor + "')\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                    GridBuilder.Append("</li>");
                }
                else
                {
                    if (settings.ColumnSetupFor == "ToolHistoryList")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupTHL\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                        GridBuilder.Append("</li>");
                    }
                    else if (settings.ColumnSetupFor == "ItemMasterChangeLog")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupICL\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                        GridBuilder.Append("</li>");
                    }
                    else if (settings.ColumnSetupFor == "WrittenOffToolList")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupWOT\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                        GridBuilder.Append("</li>");
                    }
                    else
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetup\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                        GridBuilder.Append("</li>");
                    }
                }
            }

            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ViewHistory\"><img src=\"/content/images/clock-history.png\" alt=\"" + ResGridHeader.ViewHistory + "\" title=\"" + ResGridHeader.ViewHistory+ "\"><span>" + ResGridHeader.ViewHistory + "</span></a>");
            //GridBuilder.Append("</li>");


            if (settings.DisplayUDFButton && !String.IsNullOrEmpty(settings.UDFSetupFor))
            {
                if (strModuleValue != 0)
                {
                    bool isAllowUDF = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowUDF);
                    if (isAllowUDF)
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResGridHeader.UDFSetup + "</span></a>");
                        GridBuilder.Append("</li>");

                        if (settings.ColumnSetupFor == "ItemMasterList" || settings.ColumnSetupFor == "ItemBinMasterList")
                        {
                            int ModuleId = 0;
                            string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey("binudf", out ModuleId);
                            bool isAllowBinUDF = eTurnsWeb.Helper.SessionHelper.GetModulePermission((SessionHelper.ModuleList)ModuleId, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowUDF);
                            if (isAllowBinUDF)
                            {

                                settings.UDFSetupFor = "BinUDF&UDFHeader=Bin Item Master Header";
                                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"BinUDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResBinUDF.BinUDFSetup + "</span></a>");
                                GridBuilder.Append("</li>");
                            }
                        }
                        //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"BinUDFSetup\" href=\"/UDF/UDFList?t=BinUDF\"><img src=\"/content/images/udf-setup.png\" alt=\"BinUDFSetup\" title=\"BinUDFSetup\"><span>BinUDFSetup</span></a>");
                        //GridBuilder.Append("</li>");
                        if (settings.ColumnSetupFor == "ToolList" || settings.ColumnSetupFor == "KitToolMasterList" || settings.ColumnSetupFor == "ToolListNew" || settings.ColumnSetupFor == "KitToolMasterListNew")
                        {
                            settings.UDFSetupFor = "checkouttool&UDFHeader=Tool Checkout";
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup1\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>"+ResToolMaster.CheckOutUDFSetup+"</span></a>"); 
                            GridBuilder.Append("</li>");
                        }
                        if (settings.ColumnSetupFor == "OrderMasterList")
                        {
                            settings.UDFSetupFor = "OrderDetails&UDFHeader=Order Details";
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup1\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>"+ ResOrder.OrderDetailUDFSetup + "</span></a>"); 
                            GridBuilder.Append("</li>");
                        }
                        if (settings.ColumnSetupFor == "ToolAssetOrderMasterList")
                        {
                            settings.UDFSetupFor = "ToolAssetOrderDetails&UDFHeader=Tool Asset Order Details";
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup1\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>"+ ResOrder.OrderDetailUDFSetup + "</span></a>");
                            GridBuilder.Append("</li>");
                        }
                        if (settings.ColumnSetupFor == "QuoteMasterList")
                        {
                            settings.UDFSetupFor = "QuoteDetails&UDFHeader=Quote Detail";
                            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup1\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>"+ ResQuoteMaster.QuoteDetailUDFSetup + "</span></a>"); 
                            GridBuilder.Append("</li>");
                        }
                    }
                }
            }
            // add button in item master setting wheel
            if ((strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster || strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.DashboardItemMasterList)
                && !(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
            {
                GridBuilder.Append("<li><a id=\"UDFSetup_Context\" onclick=\"OpenSupplierCatalogPopup(this);\">\r\n");
                GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.AddfromCatalog + "</span></a></li>\r\n");
            }

            if (strModuleValue == SessionHelper.ModuleList.ItemMaster && SessionHelper.AllowABIntegration)
            {
                GridBuilder.Append("<li><a id=\"AddFromABCatalog\" onclick=\"AddFromABCatalog(this);\">\r\n");
                GridBuilder.Append("<img title=\"" + ResItemMaster.AddFromABCatalog + "\" alt=\"" + ResItemMaster.AddFromABCatalog + "\" src=\"/content/images/column-setup.png\"><span>" + ResItemMaster.AddFromABCatalog + "</span></a></li>\r\n");

                GridBuilder.Append("<li><a id=\"AddFromPastABOrder\" onclick=\"AddFromPastABOrder(this);\">\r\n");
                GridBuilder.Append("<img title=\"" + ResItemMaster.AddFromPastABOrder + "\" alt=\"" + ResItemMaster.AddFromPastABOrder + "\" src=\"/content/images/column-setup.png\"><span>" + ResItemMaster.AddFromPastABOrder + "</span></a></li>\r\n");
            }


            if ((settings.ColumnSetupFor ?? string.Empty).ToLower() == "userlist")
            {
                if (eTurnsWeb.Helper.SessionHelper.UserType <= 2)
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"btndownload\"><span>" + ResGridHeader.Download + "</span></a>");
                    GridBuilder.Append("</li>");

                }
            }
            //Font icons
            if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title='"+ResGridHeader.ChangeFontSize+ "'><span><a id=\"ChangeViewToLarge\" href=\"#\" title='" + ResGridHeader.Large + "'><img src=\"/content/images/large.png\" alt='" + ResGridHeader.Large + "' /></a><a id=\"ChangeViewToMedium\" href=\"#\" title='" + ResGridHeader.Medium + "'><img src=\"/content/images/medium.png\" alt='" + ResGridHeader.Medium + "' /></a><a id=\"ChangeViewToSmall\" href=\"#\" title='" + ResGridHeader.Small + "'><img src=\"/content/images/small.png\" alt='" + ResGridHeader.Small + "'/></a></span></a>");
            GridBuilder.Append("</li>");
            //Font icons

            if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.PullMaster)
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"btnOpenBillingPopup\" onclick='return AddBillingOrPullOrderNoToPullList();' ><span>"+ResPullMaster.CompletePulls+"</span></a>"); 
                GridBuilder.Append("</li>");
            }

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString GridTopHeaderForResources(this HtmlHelper htmlHelper, GridHeaderSettings settings, eTurnsWeb.Helper.SessionHelper.ModuleList strModuleValue = 0)
    {
        var GridBuilder = new StringBuilder();

        GridBuilder.Append("<div class=\"userHead\" style=\"margin:0.5% 0.5% 0; width:99%;\">\r\n");
        GridBuilder.Append("<div class=\"BtnBlock\" style=\"margin-right: 0px; margin-bottom: 2px;\">");

        if (settings.ModuleName == "HelpDocument")
        { 
            GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
            GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='HelpDocType' style='float:left;'>" +
                               "<option value='3'>"+ResGridHeader.Mobile + "</option>" +
                               "<option value='1'>"+ResCommon.Module+"</option>" +
                               "<option value='2'>"+ResGridHeader.Report+"</option></select></div>");
            GridBuilder.Append("</div>");
        }


        if (settings.DisplayPrintBlock)
        {
            GridBuilder.Append("<div class=\"print\" style=\"margin-top: 2px; height:28px;\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"printBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"PrintResources()\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
            GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
            GridBuilder.Append("        text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
            GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel + "' /><span>" + ResGridHeader.Excel + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
            GridBuilder.Append("        </div>");
            GridBuilder.Append("    </a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
            GridBuilder.Append("        height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt=" + ResGridHeader.CSV + " /><span>" + ResGridHeader.CSV + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt=" + ResGridHeader.PDF + " /><span>" + ResGridHeader.PDF + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_0\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt=" + ResGridHeader.Copy + " /><span>" + ResGridHeader.Copy + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("</ul>");
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }


        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");


        //Font icons
        GridBuilder.Append("<div class=\"refresh\" style=\"margin-top: 2px; height:28px;\">");
        GridBuilder.Append("<a href=\"javascript:void(null);\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
        GridBuilder.Append("<div class=\"refreshBlock\">");
        GridBuilder.Append("<ul>");
        GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title='"+ResGridHeader.ChangeFontSize+ "'><span><a id=\"ChangeViewToLarge\" href=\"#\" title='" + ResGridHeader.Large + "'><img src=\"/content/images/large.png\" alt='" + ResGridHeader.Large + "' /></a><a id=\"ChangeViewToMedium\" href=\"#\" title='" + ResGridHeader.Medium + "'><img src=\"/content/images/medium.png\" alt='" + ResGridHeader.Medium + "' /></a><a id=\"ChangeViewToSmall\" href=\"#\" title='" + ResGridHeader.Small + "'><img src=\"/content/images/small.png\" alt='" + ResGridHeader.Small + "'/></a></span></a>");
        GridBuilder.Append("</li>");
        GridBuilder.Append("</ul>");
        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");
        //Font icons
        bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        bool Isinsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        if (Isinsert || IsUpdate)
        {
            //            GridBuilder.Append(@"<div class=""BtnBlockform"" style=""float: right; margin-left: 10px; text-align: right;margin-top: 0px;"">
            //                           </div>");
        }
        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        GridBuilder.Append("<script type=\"text/javascript\"></script>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString GridTopHeaderForHistory(this HtmlHelper htmlHelper, GridHeaderSettings settings, eTurnsWeb.Helper.SessionHelper.ModuleList strModuleValue = 0)
    {
        var GridBuilder = new StringBuilder();
        bool IsExportPermission = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);
        GridBuilder.Append("<div class=\"userHead\">\r\n");
        GridBuilder.Append("<div class=\"BtnBlock\" style=\"margin-right: 0px;\">");

        if (settings.DisplayPrintBlock)
        {
            if (settings.ColumnSetupFor == "ShipViaChangeLog" || settings.ColumnSetupFor == "EnterpriseChangeLog" ||
                settings.ColumnSetupFor == "RoomChangeLog" || settings.ColumnSetupFor == "UserChangeLog" ||
                settings.ColumnSetupFor == "CompanyChangeLog" || settings.ColumnSetupFor == "AssetCategoryChangeLog" ||
                settings.ColumnSetupFor == "BinChangeLog" || settings.ColumnSetupFor == "CategoryChangeLog" ||
                settings.ColumnSetupFor == "CostUOMChangeLog" || settings.ColumnSetupFor == "CustomerChangeLog" ||
                settings.ColumnSetupFor == "GLAccountChangeLog" || settings.ColumnSetupFor == "InventoryClassificationChangeLog" ||
                settings.ColumnSetupFor == "LocationChangeLog" || settings.ColumnSetupFor == "ManufacturerChangeLog" ||
                settings.ColumnSetupFor == "SupplierChangeLog" || settings.ColumnSetupFor == "TechnicianChangeLog" ||
                settings.ColumnSetupFor == "ToolCategoryChangeLog" || settings.ColumnSetupFor == "UnitChangeLog" ||
                settings.ColumnSetupFor == "VenderChangeLog" || settings.ColumnSetupFor == "QuickListChangeLog" ||
                settings.ColumnSetupFor == "CountChangeLog" || settings.ColumnSetupFor == "MaterialStagingChangeLog" ||
                settings.ColumnSetupFor == "RequisitionChangeLog" || settings.ColumnSetupFor == "WorkOrderChangeLog" ||
                settings.ColumnSetupFor == "ProjectSpendChangeLog" || settings.ColumnSetupFor == "CartItemsChangeLog" ||
                settings.ColumnSetupFor == "OrdersChangeLog" || settings.ColumnSetupFor == "TransferChangeLog" ||
                settings.ColumnSetupFor == "ToolChangeLog" || settings.ColumnSetupFor == "AssetsChangeLog" || settings.ColumnSetupFor == "NotificationMasterListChangeLog" || 
                settings.ColumnSetupFor == "FTPChangeLog" || settings.ColumnSetupFor == "WrittenOffCategoryChangeLog" || settings.ColumnSetupFor == "QuoteChangeLog"
                || settings.ColumnSetupFor == "PermissionTemplateChangeLog")
            {

                if (settings.ColumnSetupFor == "UserChangeLog")
                    GridBuilder.Append("<div class=\"printUser\" style=\"margin-top: -28px !important; margin-right:45px !important;\" ><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                else
                    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                
                GridBuilder.Append("<div class=\"refreshBlock\">");
                GridBuilder.Append("<ul>");

                if (IsExportPermission)
                {
                    if (settings.ColumnSetupFor == "QuoteChangeLog")
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportHistoryData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportHistoryData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                    else
                    {
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
                        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.CSVExport + "</span></a></li>");
                    }
                }
                GridBuilder.Append("</ul></div></div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
                GridBuilder.Append("<div class=\"printBlock\">");
                GridBuilder.Append("<ul>");
                GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"callPrint('" + settings.DataTableName + "')\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
                GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
                GridBuilder.Append("        text-align: center;\">");
                GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
                GridBuilder.Append("</li>");
                GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
                GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
                GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel +"' /><span>" + ResGridHeader.Excel + "</span>");
                GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
                GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
                GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
                GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
                GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
                GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
                GridBuilder.Append("        </div>");
                GridBuilder.Append("    </a></li>");
                GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
                GridBuilder.Append("        height: 17px; text-align: center;\">");
                GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt='"+ ResGridHeader.CSV +"' /><span>" + ResGridHeader.CSV + "</span>");
                GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
                GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
                GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
                GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
                GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
                GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
                GridBuilder.Append("</div>");
                GridBuilder.Append("</a></li>");
                GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
                GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
                GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt='"+ ResGridHeader.PDF +"' /><span>" + ResGridHeader.PDF + "</span>");
                GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
                GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
                GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
                GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
                GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
                GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
                GridBuilder.Append("</div>");
                GridBuilder.Append("</a></li>");
                GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_0\"");
                GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
                GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt='"+ ResGridHeader.Copy +"' /><span>" + ResGridHeader.Copy + "</span>");
                GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
                GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
                GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
                GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
                GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
                GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
                GridBuilder.Append("</div>");
                GridBuilder.Append("</a></li>");
                GridBuilder.Append("</ul>");
                GridBuilder.Append("</div>");
                GridBuilder.Append("</div>");
            }
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }

        if (settings.DisplayDeleteButton)
        {
            if (strModuleValue != 0)
            {
                bool isDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
                if (isDelete)
                    GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteHistoryRows\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
            }
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString QuickListItemsGridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings, eTurnsWeb.Helper.SessionHelper.ModuleList strModuleValue = 0)
    {
        var GridBuilder = new StringBuilder();

        bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        bool isUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);

        #region Context Menu
        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            GridBuilder.Append("<div id=\"divContextMenu1\" class=\"ContextMenu\" onmouseleave=\"javascript:$(this).hide();\"><ul>");

            GridBuilder.Append("<li><a onclick=\"$('#divContextMenu1').hide();OpenItemModel();\">");
            GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New + "' src=\"/content/images/refresh.png\"><span>" + ResCommon.New + "</span></a></li>"); 

            GridBuilder.Append("<li><a id=\"refreshGrid1\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            if (settings.DisplaySettings)
            {
                GridBuilder.Append("<li><a id=\"ColumnOrderSetup_Context1\">");
                GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
            }
            if (settings.DisplayUDFButton)
            {
                GridBuilder.Append("<li><a id=\"UDFSetup_Context\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\">\r\n");
                GridBuilder.Append("<img title=\"" + ResGridHeader.UDFSetup + "\" alt=\"" + ResGridHeader.UDFSetup + "\" src=\"/content/images/udf-setup.png\"><span>" + ResGridHeader.UDFSetup + "</span></a></li>\r\n");
            }
            GridBuilder.Append("</ul></div>");
        }
        //else if (settings.DisplayContextMenu && settings.DataTableName.ToLower().Contains("recieveorderlineitem"))
        //{
        //    GridBuilder.Append("<div id=\"divCtxMenu\" class=\"ContextMenu\" onmouseleave=\"javascript:$(this).hide();\"><ul>");
        //    GridBuilder.Append("<li><a onclick=\"$('#CloseOderLineItemDialog').modal();$('#divCtxMenu').hide();\">");
        //    GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>Close Line Item</span></a></li>");
        //    GridBuilder.Append("</ul></div>");

        //}

        #endregion

        GridBuilder.Append("<div class=\"userHead\" style=\"height: 25px;\">\r\n");

        #region "Data View Type"
        if (settings.dataViewType == DataViewType.Both)
        {
            GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"#\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
        }
        else if (settings.dataViewType == DataViewType.ListView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"#\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.PictureView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.None)
        {
            GridBuilder.Append("<div class=\"viewBlock\">&nbsp;</div>");
        }

        #endregion


        if (settings.DisplayGoToPage)
        {
            GridBuilder.Append("<div class=\"paginationBlock\" ><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber1\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn1\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
        }


        if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance)
        {
            GridBuilder.Append("<div class=\"BtnBlock\" id=\"btnblockAsset\">");
        }
        else if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging)
        {
            GridBuilder.Append("<div class=\"BtnBlock\" id=\"btnblockms\">");
        }
        else
        {
            GridBuilder.Append("<div class=\"BtnBlock\" id=\"btnblock\">");
        }


        #region PrintBlock
        if (settings.DisplayPrintBlock)
        {
            GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"printBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"callPrint1('" + settings.DataTableName + "')\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
            GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
            GridBuilder.Append("        text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
            GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel +"' /><span>" + ResGridHeader.Excel + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
            GridBuilder.Append("        </div>");
            GridBuilder.Append("    </a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
            GridBuilder.Append("        height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt='"+ ResGridHeader.CSV +"' /><span>" + ResGridHeader.CSV + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt='"+ ResGridHeader.PDF +"' /><span>" + ResGridHeader.PDF + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_Copy\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt='"+ ResGridHeader.Copy +"' /><span>" + ResGridHeader.Copy + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }

        #endregion


        if (settings.DisplaySaveButton && isUpdate)
        {
            GridBuilder.Append("<a title='"+ResCommon.Save+"' href=\"javascript:void(null);\" id=\"saveRows\"><img src=\"/content/images/save.png\" alt='"+ResCommon.Save+"' /></a>");
        }

        if (settings.ShowApplyButton)
        {
            GridBuilder.Append("<a title='"+ResInventoryCountDetail.Apply+"' href=\"javascript:void(null);\" id=\"ApplyAction\"><img src=\"/content/images/apply.png\" alt='"+ResInventoryCountDetail.Apply+"' /></a>");
        }

        if (settings.ShowSelectAll)
        {
            if (settings.DataTableName.Contains("RecieveOrderLineItem") || settings.DataTableName.Contains("TransferLineItem") || settings.DataTableName.Contains("TransferLineItemForReceive") 
                || settings.DataTableName.Contains("FullFillTransferLineItem") || settings.DataTableName.Contains("QuoteLineItem"))
            {
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"aSelectAll\" class=\"clsSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"aDeSelectAll\" class=\"clsDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            }
            else if (settings.DataTableName == "Count" || strModuleValue == SessionHelper.ModuleList.Count)
            {
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAllLI\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
                GridBuilder.Append("<a title='"+ResGridHeader.DeSelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAllLI\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResGridHeader.DeSelectAll+"' /></a>");
            }
            else
            {
                GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAllLI\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");

            }
            //GridBuilder.Append("<div class=\"selectall\">");
            //GridBuilder.Append("<div class=\"refresh\"><a href=\"javascript: void(null); \" ><img src=\"/content/images/selectall.png\" alt=\"\" /></a>");
            //GridBuilder.Append("<div class=\"refreshBlock\">");
            //GridBuilder.Append("<ul>");
            //if (settings.DataTableName.Contains("RecieveOrderLineItem") || settings.DataTableName.Contains("TransferLineItem") || settings.DataTableName.Contains("TransferLineItemForReceive") || settings.DataTableName.Contains("FullFillTransferLineItem"))
            //{
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"aSelectAll\" class=\"clsSelectAll\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //    GridBuilder.Append("</li>");
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.UnSelectAll + "' style='' href=\"javascript:void(null);\" id=\"aDeSelectAll\" class=\"clsDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='" + ResCommon.UnSelectAll + "' />" + ResCommon.UnSelectAll + "</a>");
            //    GridBuilder.Append("</li>");
            //}
            //else if (settings.DataTableName == "Count")
            //{
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"actionSelectAllLI\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //    GridBuilder.Append("</li>");
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.UnSelectAll + "' style='' href=\"javascript:void(null);\" id=\"actionDeSelectAllLI\"><img src=\"/content/images/UnSelectAll.png\" alt='" + ResCommon.UnSelectAll + "' />" + ResCommon.UnSelectAll + "</a>");
            //    GridBuilder.Append("</li>");
            //}
            //else
            //{
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"actionSelectAllLI\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //    GridBuilder.Append("</li>");

            //}
            //GridBuilder.Append("</ul>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
            // GridBuilder.Append("<a title='Select all' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAllLI\"><img src=\"/content/images/UnSelectAll.png\" alt='Select all' /></a>");
        }

        if (settings.ShowCloseButton)
        {
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionCloseItems\"><img src=\"/content/images/closeitms.png\" alt='"+ResWorkOrder.Close+"' /></a>");
        }
        if (settings.DisplayDeleteButton)
        {
            if (strModuleValue != 0)
            {
                bool isDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
                if (isDelete)
                {
                    if (strModuleValue == eTurnsWeb.Helper.SessionHelper.ModuleList.AssetMaintenance)
                    {
                        GridBuilder.Append("<a title='"+ResToolMaster.Delete+"' href=\"javascript:void(null);\" id=\"deleteRows2\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
                    }
                    else
                    {
                        GridBuilder.Append("<a title='"+ResToolMaster.Delete+"' href=\"javascript:void(null);\" id=\"deleteRows1\"><img src=\"/content/images/delete.png\" alt='"+ResToolMaster.Delete+"' /></a>");
                    }
                }
            }
        }



        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");
            GridBuilder.Append("<div id=\"ColumnSortableModal1\" class=\"DetailGridColumn\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortable1\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("<input type=\"button\" class=\"CreateBtn\" id=\"btnSaveOrder1\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrder1('" + settings.ColumnSetupFor + "')\" />");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersetting1\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div id=\"divRefreshBlock1\" class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"refreshGrid1\"><img src=\"/content/images/refresh.png\" alt=\"" + ResGridHeader.Refresh + "\" title=\"" + ResGridHeader.Refresh + "\"><span>" + ResGridHeader.Refresh + "</span></a>");
            //GridBuilder.Append("</li>");
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"GridAutoRefresh\" type=\"checkbox\"  title=\"Auto Refresh\">" + ResGridHeader.AutoRefresh + "</input>");
            //GridBuilder.Append("</li>");



            if (settings.ColumnSetupFor == "OrderLineItemList" || settings.ColumnSetupFor == "ReceivedItemDetailGrid")
            {
                bool isShowDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowDeleted);

                if (isShowDelete)
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsDeletedRecordsOrdLn\" type=\"checkbox\"  title='" + ResGridHeader.IncludeDeleted + "'>" + ResGridHeader.IncludeDeleted + "</input>");
                    GridBuilder.Append("</li>");
                }
            }

            if (settings.ColumnSetupFor == "QuoteDetailList")
            {
                bool isShowDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(strModuleValue, eTurnsWeb.Helper.SessionHelper.PermissionType.ShowDeleted);

                if (isShowDelete)
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsDeletedRecordsQuoteLn\" type=\"checkbox\"  title='" + ResGridHeader.IncludeDeleted + "'>" + ResGridHeader.IncludeDeleted + "</input>");
                    GridBuilder.Append("</li>");
                }
            }

            //GridBuilder.Append("<li class=\"refreshBlockswf\"><input id=\"IsArchivedRecords\" type=\"checkbox\" title=\"Include Archived\">" + ResGridHeader.IncludeArchived + "</input>");
            //GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetup1\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
            GridBuilder.Append("</li>");
            if (settings.ColumnSetupFor == "OrderLineItemList" || settings.ColumnSetupFor == "ReceivedItemDetailGrid")
            {
                GridBuilder.Append("<li id='OrderlineitemDownloadDocsli' class=\"refreshBlockswf\"><a id=\"aPrintAttachedDocs\" onclick=\"javascript:return DownloadReceivedAttachedDocs(this);\"  href=\"javascript:void(null);\" ><span>" + ResGridHeader.DownloadDocs + "</span></a>");
            }
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ViewHistory\"><img src=\"/content/images/clock-history.png\" alt=\"" + ResGridHeader.ViewHistory + "\" title=\"" + ResGridHeader.ViewHistory+ "\"><span>" + ResGridHeader.ViewHistory + "</span></a>");
            //GridBuilder.Append("</li>");
            if (!String.IsNullOrEmpty(settings.UDFSetupFor))
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResGridHeader.UDFSetup + "</span></a>");
                GridBuilder.Append("</li>");
            }


            //Font icons
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title=\"Change Font Size\"><span><a id=\"ChangeViewToLarge\" href=\"#\" title=\"Large\"><img src=\"/content/images/large.png\" alt=\"Large\" /></a><a id=\"ChangeViewToMedium\" href=\"#\" title=\"Medium\"><img src=\"/content/images/medium.png\" alt=\"Medium\" /></a><a id=\"ChangeViewToSmall\" href=\"#\" title=\"Small\"><img src=\"/content/images/small.png\" alt=\"Small\"/></a></span></a>");
            //GridBuilder.Append("</li>");
            //Font icons

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString ItemModelGridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings)
    {
        var GridBuilder = new StringBuilder();

        #region Context Menu
        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            //GridBuilder.Append("<div id=\"divContextMenu1\" class=\"ContextMenu\" onmouseleave=\"javascript:$(this).hide();\"><ul>");

            //GridBuilder.Append("<li><a onclick=\"$('#divContextMenu1').hide();OpenItemModel();\">");
            //GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>New</span></a></li>");

            //GridBuilder.Append("<li><a id=\"refreshGrid1\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            //if (settings.DisplaySettings)
            //{
            //    GridBuilder.Append("<li><a id=\"ColumnOrderSetup_Context1\">");
            //    GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
            //}
            //if (settings.DisplayUDFButton)
            //{
            //    GridBuilder.Append("<li><a id=\"UDFSetup_Context\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\">\r\n");
            //    GridBuilder.Append("<img title=\"" + ResGridHeader.UDFSetup + "\" alt=\"" + ResGridHeader.UDFSetup + "\" src=\"/content/images/udf-setup.png\"><span>" + ResGridHeader.UDFSetup + "</span></a></li>\r\n");
            //}
            //GridBuilder.Append("</ul></div>");
        }

        #endregion

        GridBuilder.Append("<div class=\"userHead\" style=\"height:25px;float:left\">\r\n");

        #region "Data View Type"
        if (settings.dataViewType == DataViewType.Both)
        {
            GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"#\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
        }
        else if (settings.dataViewType == DataViewType.ListView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"#\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.PictureView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.None)
        {
            if (settings.ModuleName == "NewPull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='NewPullAction' style='float:left;'><option value='Pull'>"+ResLayout.Pull+ "</option><option value='Credit'>" + ResCommon.Credit + "</option><option value='CreditMS'>" + ResPullMaster.CreditMS + "</option></select></div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }
            if (settings.ModuleName == "NewPull" && settings.PullForPage == "mpull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock' style='right:90px;'><select class='selectBox' id='slctItemstoLoad' style='float:left;'><option value='1'>" + ResCommon.ScheduleItems + "</option><option value='2'>" + ResCommon.AllItems + "</option></select></div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }

        }
        else if (settings.dataViewType == DataViewType.radioButtons)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><ul><li style='float:left;'><input checked='checked' type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='1' /><lable for='rdoInventoryLocation'>" + ResGridHeader.IL + "</lable></li><li style='float:left;'><input type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='2' /><lable for='rdoStagingLocation'>" + ResGridHeader.SL+"</lable></li></ul></div>");
        }
        #endregion

        if (settings.DisplayGoToPage)
        {
            GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumberIM\" class=\"inputNum\" /><input type=\"button\" id=\"GobtnIM\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
        }

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region PrintBlock
        if (settings.DisplayPrintBlock)
        {
            GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"printBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"callPrint('" + settings.DataTableName + "')\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
            GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
            GridBuilder.Append("        text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
            GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel +"' /><span>" + ResGridHeader.Excel + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
            GridBuilder.Append("        </div>");
            GridBuilder.Append("    </a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
            GridBuilder.Append("        height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt='"+ ResGridHeader.CSV +"' /><span>" + ResGridHeader.CSV + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt='"+ ResGridHeader.PDF +"' /><span>" + ResGridHeader.PDF + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_Copy\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt='"+ ResGridHeader.Copy +"' /><span>" + ResGridHeader.Copy + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }

        #endregion
        if (settings.ShowSelectAll)
        {
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAll2\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAll2\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");

            //GridBuilder.Append("<div class=\"selectall\">");
            //GridBuilder.Append("<div class=\"refresh\"><a href=\"javascript: void(null); \" ><img src=\"/content/images/selectall.png\" alt=\"\" /></a>");
            //GridBuilder.Append("<div class=\"refreshBlock\">");
            //GridBuilder.Append("<ul>");

            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"actionSelectAll2\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //GridBuilder.Append("</li>");
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.UnSelectAll + "' style='' href=\"javascript:void(null);\" id=\"actionDeSelectAll2\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='" + ResCommon.UnSelectAll + "' />" + ResCommon.UnSelectAll + "</a>");
            //GridBuilder.Append("</li>");

            //GridBuilder.Append("</ul>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");

        }
        if (settings.DisplaySaveButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"saveRows\"><img src=\"/content/images/save.png\" alt=\"\" /></a>");
        }
        if (settings.DisplayDeleteButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteRows1\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
        }

        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");
            GridBuilder.Append("<div id=\"ColumnSortableModalIM\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortableIM\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("<input type=\"button\" class=\"CreateBtn\" id=\"btnSaveOrderIM\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderIM('" + settings.ColumnSetupFor + "')\" />");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersettingIM\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div id=\"divRefreshBlockIM\" class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"refreshGridIM\"><img src=\"/content/images/refresh.png\" alt=\"" + ResGridHeader.Refresh + "\" title=\"" + ResGridHeader.Refresh + "\"><span>" + ResGridHeader.Refresh + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupIM\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
            GridBuilder.Append("</li>");
            if (!String.IsNullOrEmpty(settings.UDFSetupFor))
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResGridHeader.UDFSetup + "</span></a>");
                GridBuilder.Append("</li>");
            }

            //Font icons
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title='" + ResGridHeader.ChangeFontSize + "'><span><a id=\"ChangeViewToLarge\" href=\"#\" title='" + ResGridHeader.Large + "'><img src=\"/content/images/large.png\" alt='" + ResGridHeader.Large + "' /></a><a id=\"ChangeViewToMedium\" href=\"#\" title='" + ResGridHeader.Medium + "'><img src=\"/content/images/medium.png\" alt='" + ResGridHeader.Medium + "' /></a><a id=\"ChangeViewToSmall\" href=\"#\" title='" + ResGridHeader.Small + "'><img src=\"/content/images/small.png\" alt='" + ResGridHeader.Small + "'/></a></span></a>");
            GridBuilder.Append("</li>");
            //Font icons

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString BOMItemModelGridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings)
    {
        var GridBuilder = new StringBuilder();

        #region Context Menu
        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            //GridBuilder.Append("<div id=\"divContextMenu1\" class=\"ContextMenu\" onmouseleave=\"javascript:$(this).hide();\"><ul>");

            //GridBuilder.Append("<li><a onclick=\"$('#divContextMenu1').hide();OpenItemModel();\">");
            //GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>New</span></a></li>");

            //GridBuilder.Append("<li><a id=\"refreshGrid1\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            //if (settings.DisplaySettings)
            //{
            //    GridBuilder.Append("<li><a id=\"ColumnOrderSetup_Context1\">");
            //    GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
            //}
            //if (settings.DisplayUDFButton)
            //{
            //    GridBuilder.Append("<li><a id=\"UDFSetup_Context\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\">\r\n");
            //    GridBuilder.Append("<img title=\"" + ResGridHeader.UDFSetup + "\" alt=\"" + ResGridHeader.UDFSetup + "\" src=\"/content/images/udf-setup.png\"><span>" + ResGridHeader.UDFSetup + "</span></a></li>\r\n");
            //}
            //GridBuilder.Append("</ul></div>");
        }

        #endregion

        GridBuilder.Append("<div class=\"userHead\" style=\"height:25px;float:left\">\r\n");

        #region "Data View Type"
        if (settings.dataViewType == DataViewType.Both)
        {
            GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"#\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
        }
        else if (settings.dataViewType == DataViewType.ListView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"#\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.PictureView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.None)
        {
            if (settings.ModuleName == "NewPull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='NewPullAction' style='float:left;'><option value='Pull'>" + ResLayout.Pull + "</option><option value='Credit'>"+ResCommon.Credit+"</option><option value='CreditMS'>"+ ResPullMaster.CreditMS + "</option></select></div>"); 
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }
            if (settings.ModuleName == "NewPull" && settings.PullForPage == "mpull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock' style='right:90px;'><select class='selectBox' id='slctItemstoLoad' style='float:left;'><option value='1'>" + ResCommon.ScheduleItems + "</option><option value='2'>" + ResCommon.AllItems + "</option></select></div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }

        }
        else if (settings.dataViewType == DataViewType.radioButtons)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><ul><li style='float:left;'><input checked='checked' type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='1' /><lable for='rdoInventoryLocation'>" + ResGridHeader.IL + "</lable></li><li style='float:left;'><input type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='2' /><lable for='rdoStagingLocation'>" + ResGridHeader.SL + "</lable></li></ul></div>");
        }
        #endregion

        if (settings.DisplayGoToPage)
        {
            GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumberIM\" class=\"inputNum\" /><input type=\"button\" id=\"GobtnIM\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
        }

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region PrintBlock
        if (settings.DisplayPrintBlock)
        {
            GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"printBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"callPrint('" + settings.DataTableName + "')\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
            GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
            GridBuilder.Append("        text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
            GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel +"' /><span>" + ResGridHeader.Excel + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
            GridBuilder.Append("        </div>");
            GridBuilder.Append("    </a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
            GridBuilder.Append("        height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt='"+ ResGridHeader.CSV +"' /><span>" + ResGridHeader.CSV + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt='"+ ResGridHeader.PDF +"' /><span>" + ResGridHeader.PDF + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_Copy\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt='"+ ResGridHeader.Copy +"' /><span>" + ResGridHeader.Copy + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }

        #endregion
        if (settings.ShowSelectAll)
        {
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAllBOM\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAllBOM\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            //GridBuilder.Append("<div class=\"selectall\">");
            //GridBuilder.Append("<div class=\"refresh\"><a href=\"javascript: void(null); \" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            //GridBuilder.Append("<div class=\"refreshBlock\">");
            //GridBuilder.Append("<ul>");

            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"actionSelectAllBOM\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //GridBuilder.Append("</li>");
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.UnSelectAll + "' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAllBOM\"><img src=\"/content/images/UnSelectAll.png\" alt='" + ResCommon.UnSelectAll + "' />" + ResCommon.UnSelectAll + "</a>");
            //GridBuilder.Append("</li>");

            //GridBuilder.Append("</ul>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
        }
        if (settings.DisplaySaveButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"saveRows\"><img src=\"/content/images/save.png\" alt=\"\" /></a>");
        }
        if (settings.DisplayDeleteButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteRows1\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
        }

        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");
            GridBuilder.Append("<div id=\"ColumnSortableModalIM\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortableIM\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("<input type=\"button\" class=\"CreateBtn\" id=\"btnSaveOrderIM\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderIM('" + settings.ColumnSetupFor + "')\" />");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersettingIM\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div id=\"divRefreshBlockIM\" class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"refreshGridIM\"><img src=\"/content/images/refresh.png\" alt=\"" + ResGridHeader.Refresh + "\" title=\"" + ResGridHeader.Refresh + "\"><span>" + ResGridHeader.Refresh + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupIM\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
            GridBuilder.Append("</li>");
            if (!String.IsNullOrEmpty(settings.UDFSetupFor))
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResGridHeader.UDFSetup + "</span></a>");
                GridBuilder.Append("</li>");
            }

            //Font icons
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title='"+ResGridHeader.ChangeFontSize+"'><span><a id=\"ChangeViewToLarge\" href=\"#\" title='" + ResGridHeader.Large + "'><img src=\"/content/images/large.png\" alt='" + ResGridHeader.Large + "' /></a><a id=\"ChangeViewToMedium\" href=\"#\" title='" + ResGridHeader.Medium + "'><img src=\"/content/images/medium.png\" alt='" + ResGridHeader.Medium + "' /></a><a id=\"ChangeViewToSmall\" href=\"#\" title='" + ResGridHeader.Small + "'><img src=\"/content/images/small.png\" alt='" + ResGridHeader.Small + "'/></a></span></a>");
            GridBuilder.Append("</li>");
            //Font icons

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString ItemModelGridTopHeaderWithContextMenu(this HtmlHelper htmlHelper, GridHeaderSettings settings)
    {
        var GridBuilder = new StringBuilder();

        #region Context Menu
        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            GridBuilder.Append("<div id=\"divContextMenu1\" class=\"ContextMenu\" onmouseleave=\"javascript:$(this).hide();\"><ul>");

            //GridBuilder.Append("<li><a onclick=\"$('#divContextMenu1').hide();OpenItemModel();\">");
            //GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>New</span></a></li>");

            GridBuilder.Append("<li><a id=\"refreshGrid1\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            if (settings.DisplaySettings)
            {
                GridBuilder.Append("<li><a id=\"ColumnOrderSetup_Context1\">");
                GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
            }
            //if (settings.DisplayUDFButton)
            //{
            //    GridBuilder.Append("<li><a id=\"UDFSetup_Context\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\">\r\n");
            //    GridBuilder.Append("<img title=\"" + ResGridHeader.UDFSetup + "\" alt=\"" + ResGridHeader.UDFSetup + "\" src=\"/content/images/udf-setup.png\"><span>" + ResGridHeader.UDFSetup + "</span></a></li>\r\n");
            //}
            GridBuilder.Append("</ul></div>");
        }

        #endregion

        GridBuilder.Append("<div class=\"userHead\" style=\"height:25px;float:left\">\r\n");

        #region "Data View Type"
        if (settings.dataViewType == DataViewType.Both)
        {
            GridBuilder.Append("<div class=\"viewBlock\"> <span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a> <a href=\"#\" class=\"view\"> <img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>\r\n");
        }
        else if (settings.dataViewType == DataViewType.ListView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span><a href=\"#\" class=\"view\"><img src=\"/content/images/list-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.PictureView)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><span class=\"label\">" + settings.ViewTypeText + ":</span> <a href=\"#\" class=\"view\"><img src=\"/content/images/thumb-view.png\" alt=\"\" /></a></div>");
        }
        else if (settings.dataViewType == DataViewType.None)
        {
            if (settings.ModuleName == "NewPull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='NewPullAction' style='float:left;'><option value='Pull'>" + ResLayout.Pull + "</option><option value='Credit'>" + ResCommon.Credit + "</option><option value='CreditMS'>" + ResPullMaster.CreditMS + "</option></select></div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }

            if (settings.ModuleName == "NewPull" && settings.PullForPage == "mpull")
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>");
                GridBuilder.Append("<div class='actionBlock' style='right:90px;'><select class='selectBox' id='slctItemstoLoad' style='float:left;'><option value='1'>" + ResCommon.ScheduleItems + "</option><option value='2'>" + ResCommon.AllItems + "</option></select></div>");
                GridBuilder.Append("</div>");
            }
            else
            {
                GridBuilder.Append("<div class=\"viewBlock\" style='float:left'>&nbsp; </div>");
            }



        }
        else if (settings.dataViewType == DataViewType.radioButtons)
        {
            GridBuilder.Append("<div class=\"viewBlock\"><ul><li style='float:left;'><input checked='checked' type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='1' /><lable for='rdoInventoryLocation'>" + ResGridHeader.IL + "</lable></li><li style='float:left;'><input type='radio' id='rdoInventoryLocation' name='ShowStagingLocation' value='2' /><lable for='rdoStagingLocation'>" + ResGridHeader.SL + "</lable></li></ul></div>");
        }
        #endregion

        if (settings.DisplayGoToPage)
        {
            GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumberIM\" class=\"inputNum\" /><input type=\"button\" id=\"GobtnIM\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
        }

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region PrintBlock
        if (settings.DisplayPrintBlock)
        {
            GridBuilder.Append("<div class=\"print\"><a href=\"javascript:void(null);\"><img src=\"/content/images/print.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"printBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a onclick=\"callPrint('" + settings.DataTableName + "')\" class=\"DTTT_button ui-button ui-state-default DTTT_button_print\"");
            GridBuilder.Append("        id=\"ToolTables_example_4\" title='"+ResGridHeader.ViewPrint+"' style=\"width: 80%; height: 17px;");
            GridBuilder.Append("        text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/print.png\" alt='"+ ResGridHeader.Print +"' /><span>" + ResGridHeader.Print + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_xls\"");
            GridBuilder.Append("        id=\"ToolTables_example_2\" style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/excel.png\" alt='"+ ResGridHeader.Excel +"' /><span>" + ResGridHeader.Excel + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=3&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_3\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_3\">");
            GridBuilder.Append("        </div>");
            GridBuilder.Append("    </a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_csv\" id=\"ToolTables_example_1\" style=\"width: 80%;");
            GridBuilder.Append("        height: 17px; text-align: center;\">");
            GridBuilder.Append("        <img src=\"/Content/images/csv.png\" alt='"+ ResGridHeader.CSV +"' /><span>" + ResGridHeader.CSV + "</span>");
            GridBuilder.Append("        <div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("            <embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=2&amp;width=43&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_2\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_2\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_pdf\" id=\"ToolTables_example_3\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/pdf.png\" alt='"+ ResGridHeader.PDF +"' /><span>" + ResGridHeader.PDF + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=4&amp;width=40&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_4\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_4\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("<li class=\"printBlockswf\"><a class=\"DTTT_button DTTT_button_copy\" id=\"ToolTables_example_Copy\"");
            GridBuilder.Append("style=\"width: 80%; height: 17px; text-align: center;\">");
            GridBuilder.Append("<img src=\"/Content/images/Copy.png\" alt='"+ ResGridHeader.Copy +"' /><span>" + ResGridHeader.Copy + "</span>");
            GridBuilder.Append("<div style=\"position: absolute; left: 0px; top: 0px; width: 100%; height: 31px; z-index: 99;\">");
            GridBuilder.Append("<embed height=\"31\" width=\"100%\" align=\"middle\" wmode=\"transparent\" flashvars=\"id=1&amp;width=48&amp;height=31\"");
            GridBuilder.Append("pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\"");
            GridBuilder.Append("allowfullscreen=\"false\" allowscriptaccess=\"always\" name=\"ZeroClipboard_TableToolsMovie_1\"");
            GridBuilder.Append("bgcolor=\"#ffffff\" quality=\"best\" menu=\"false\" loop=\"false\" src=\"/Content/swf/copy_csv_xls_pdf.swf\"");
            GridBuilder.Append("id=\"ZeroClipboard_TableToolsMovie_1\">");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</a></li>");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }
        else
        {
            GridBuilder.Append("&nbsp;");
        }

        #endregion
        if (settings.ShowSelectAll)
        {
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' href=\"javascript:void(null);\" id=\"actionSelectAll\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='"+ResCommon.SelectAll+"' /></a>");
            GridBuilder.Append("<a title='"+ResCommon.SelectAll+"' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAll3\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='"+ResCommon.SelectAll+"' /></a>");

            //GridBuilder.Append("<div class=\"selectall\">");
            //GridBuilder.Append("<div class=\"refresh\"><a href=\"javascript: void(null); \" ><img src=\"/content/images/selectall.png\" alt=\"\" /></a>");
            //GridBuilder.Append("<div class=\"refreshBlock\">");
            //GridBuilder.Append("<ul>");

            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.SelectAll + "' href=\"javascript:void(null);\" id=\"actionSelectAll\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='" + ResCommon.SelectAll + "' />" + ResCommon.SelectAll + "</a>");
            //GridBuilder.Append("</li>");
            //GridBuilder.Append("<li class=\"refreshBlockswf\"><a title='" + ResCommon.UnSelectAll + "' style='' href=\"javascript:void(null);\" id=\"actionDeSelectAll3\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='" + ResCommon.UnSelectAll + "' />" + ResCommon.UnSelectAll + "</a>");
            //GridBuilder.Append("</li>");

            //GridBuilder.Append("</ul>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
            //GridBuilder.Append("</div>");
        }
        if (settings.DisplaySaveButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"saveRows\"><img src=\"/content/images/save.png\" alt=\"\" /></a>");
        }
        if (settings.DisplayDeleteButton)
        {
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"deleteRows1\"><img src=\"/content/images/delete.png\" alt=\"\" /></a>");
        }

        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");
            GridBuilder.Append("<div id=\"ColumnSortableModalIM\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortableIM\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("<input type=\"button\" class=\"CreateBtn\" id=\"btnSaveOrderIM\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderIM('" + settings.ColumnSetupFor + "')\" />");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersettingIM\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div id=\"divRefreshBlockIM\" class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"refreshGridIM\"><img src=\"/content/images/refresh.png\" alt=\"" + ResGridHeader.Refresh + "\" title=\"" + ResGridHeader.Refresh + "\"><span>" + ResGridHeader.Refresh + "</span></a>");
            GridBuilder.Append("</li>");
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupIM\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
            GridBuilder.Append("</li>");
            if (!String.IsNullOrEmpty(settings.UDFSetupFor))
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"UDFSetup\" href=\"/UDF/UDFList?t=" + settings.UDFSetupFor + "\"><img src=\"/content/images/udf-setup.png\" alt=\"" + ResGridHeader.UDFSetup + "\" title=\"" + ResGridHeader.UDFSetup + "\"><span>" + ResGridHeader.UDFSetup + "</span></a>");
                GridBuilder.Append("</li>");
            }
            if (settings.ModuleName == "NewPull")
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"POSetup\" href=\"/Pull/PullPOMasterList\"><img src=\"/content/images/udf-setup.png\" alt=\"" + eTurns.DTO.Resources.ResLayout.PullPOMasterList + "\" title=\"" + eTurns.DTO.Resources.ResLayout.PullPOMasterList + "\"><span>" + eTurns.DTO.Resources.ResLayout.PullPOMasterList + "</span></a>");
                GridBuilder.Append("</li>");
            }
            //Font icons
            GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title='"+ResGridHeader.ChangeFontSize+"'><span><a id=\"ChangeViewToLarge\" href=\"#\" title='" + ResGridHeader.Large + "'><img src=\"/content/images/large.png\" alt='" + ResGridHeader.Large + "' /></a><a id=\"ChangeViewToMedium\" href=\"#\" title='" + ResGridHeader.Medium + "'><img src=\"/content/images/medium.png\" alt='" + ResGridHeader.Medium + "' /></a><a id=\"ChangeViewToSmall\" href=\"#\" title='" + ResGridHeader.Small + "'><img src=\"/content/images/small.png\" alt='" + ResGridHeader.Small + "'/></a></span></a>");
            GridBuilder.Append("</li>");
            //Font icons

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString ConvertNullToDouble(this HtmlHelper htmlHelper, Nullable<decimal> ValueDeC)
    {
        if (ValueDeC == null)
            return MvcHtmlString.Create("0.00");
        else
            return MvcHtmlString.Create(((decimal)ValueDeC).ToString("0.00"));
    }
    public static MvcHtmlString ConvertNullToDouble(this HtmlHelper htmlHelper, Nullable<double> ValueDeC)
    {
        if (ValueDeC == null)
            return MvcHtmlString.Create("0.00");
        else
            return MvcHtmlString.Create(((double)ValueDeC).ToString("0.00"));
    }


    public static MvcHtmlString GridInnerGridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings)
    {
        var GridBuilder = new StringBuilder();
        //bool IsReportView = false;//eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Reports, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
        //bool IsExportPermission = false;//eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ExportPermission);

        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            GridBuilder.Append("<div id=\"divContextMenu\" class=\"ContextMenu\" ><ul>");

            //Show New in context menu only if insert rights 

            GridBuilder.Append("<li><a onclick=\"TabItemClickedbyContext()\">");
            //GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>New</span></a></li>");    
            GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New + "' src=\"/content/images/drildown_open.jpg\"><span>" + ResCommon.New + "</span></a></li>");

            if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard") && (settings.ColumnSetupFor ?? string.Empty) != "ToolHistoryList")
                GridBuilder.Append("<li><a id=\"refreshGrid\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            if (settings.DisplaySettings)
            {
                if (settings.ShowReorder)
                {
                    GridBuilder.Append("<li><a id=\"ColumnOrderSetupInnerGrid_Context\">");
                    GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
                }
            }

            GridBuilder.Append("</ul></div>");
        }

        GridBuilder.Append("<div class=\"userHead\">\r\n");

        #region "Data View Type"
        //GridBuilder.Append("<div class=\"viewBlock\" style=\"display: none\">&nbsp;</div>");

        #endregion

        if (settings.DisplayGoToPage)
        {
            //GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
            GridBuilder.Append("<div class=\"InnerGridPaginationBlock\"><input type=\"button\" id=\"InnerGridGobtn\" class=\"go\" value=\"" + ResGridHeader.Go + "\" /><input type=\"text\" id=\"InnerGridPageNumber\" class=\"inputNum\" /><span class=\"label\">" + ResGridHeader.GoToPage + "</span></div>");

        }
        //if (settings.ShowCartPageAction)
        //{
        //    GridBuilder.Append("<div class='actionBlock'><select class='selectBox' id='slectAction' style='float:left;'><option value=''>Select</option><option value='3'>Create transfers</option><option value='4'>Create orders</option><option value='5'>Create returns</option></select><input type='button' id='btnCheckout' class='btnGeneral' value='Create' /></div>");
        //}

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region DisplayPrintBlock
        //if (settings.DisplayPrintBlock)
        //{
        //    GridBuilder.Append("<div class=\"print\"><div class=\"refresh\"><a href=\"javascript:void(null);\" ><img src=\"/content/images/print.png\" alt=\"\" /></a>");
        //    GridBuilder.Append("<div class=\"refreshBlock\">");
        //    GridBuilder.Append("<ul>");
        //    //GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1\" onclick=\"ReportExecutionData('Tools Checkout');\"><span>Checkout Tool</span></a></li>");
        //    if (IsReportView)
        //    {
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools','Excel');\"><img title=\"" + "Excel" + "\" alt=\"" + "Excel" + "\" src=\"/content/images/excel.png\"><span>Tool List</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools','PDF');\"><img title=\"" + "pdf" + "\" alt=\"" + "pdf" + "\" src=\"/content/images/pdf.png\"><span>Tool List</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1Excel\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','Excel');\"><img title=\"" + "Excel" + "\" alt=\"" + "Excel" + "\" src=\"/content/images/excel.png\"><span>Checkout Tool</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkRpt1PDF\" onclick=\"ReportExecutionDataInFile('Tools Checked Out','PDF');\"><img title=\"" + "pdf" + "\" alt=\"" + "pdf" + "\" src=\"/content/images/pdf.png\"><span>Checkout Tool</span></a></li>");
        //    }

        //    if (IsExportPermission)
        //    {
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpExcel\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','Excel');\"><span>" + ResCommon.ExcelExport + "</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkExpCSV\" onclick=\"ExportData('" + settings.ColumnSetupFor + "','CSV');\"><span>" + ResCommon.ExportCheckoutHistory + "</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolExpCSV\" onclick=\"ExportData('ToolMaster','CSV');\"><span>" + ResCommon.ToolMasterCSVExport + "</span></a></li>");
        //        GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"lnkToolCOStatusCSV\" onclick=\"ExportData('ToolCheckoutStatus','CSV');\"><span>" + ResCommon.ToolCheckoutStatusExport + "</span></a></li>");
        //    }

        //    GridBuilder.Append("</ul></div></div>");
        //}
        //else
        //{
        GridBuilder.Append("<div class=\"print\">");
        //}

        #endregion

        //GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        //if (settings.ShowSelectAll)
        //{
        //    if (settings.ReorderByDataTableName)
        //    {
        //        GridBuilder.Append("<a title='Select all' href=\"javascript:void(null);\" id=\"" + settings.DataTableName + "_SelectAll\" class=\"clsSelectAllByDataTableName\"><img src=\"/content/images/selectall.png\" alt='Select all' onclick=\"SelectAllByDataTableNam('" + settings.DataTableName + "')\" /></a>");
        //        GridBuilder.Append("<a title='Select all' style='display:none' href=\"javascript:void(null);\" id=\"" + settings.DataTableName + "_DeSelectAll\" class=\"clsDeselectAllByDataTableName\"><img src=\"/content/images/UnSelectAll.png\" alt='Select all' onclick=\"DeselectAllByDataTableNam('" + settings.DataTableName + "')\" /></a>");
        //    }

        //    GridBuilder.Append("<a title='Select all' href=\"javascript:void(null);\" id=\"actionSelectAll\" class=\"clsactionSelectAll\"><img src=\"/content/images/selectall.png\" alt='Select all' /></a>");
        //    GridBuilder.Append("<a title='Select all' style='display:none' href=\"javascript:void(null);\" id=\"actionDeSelectAll\" class=\"clsactionDeSelectAll\"><img src=\"/content/images/UnSelectAll.png\" alt='Select all' /></a>");
        //}

        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");            
            GridBuilder.Append("<div id=\"ColumnSortableModalInnerGrid\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortableInnerGrid\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");

            if (settings.ShowReorder)
            {
                GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderInnerGrid('" + settings.ColumnSetupFor + "')\" />");
            }

            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersetting\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");

            if (settings.ShowReorder)
            {
                if (settings.ReorderByDataTableName == true)
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupByDataTableName\" onclick=\"OpenReorderPopupByDataTableName('" + settings.DataTableName + "', '" + settings.ColumnSetupFor + "')\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                    GridBuilder.Append("</li>");
                }
                else
                {
                    GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupInnerGrid\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                    GridBuilder.Append("</li>");
                }
            }

            //Font icons
            //if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard"))
            //    GridBuilder.Append("<li class=\"refreshBlockswf\"><a href=\"#\"><img src=\"/content/images/font-view.png\" title=\"Change Font Size\"><span><a id=\"ChangeViewToLarge\" href=\"#\" title=\"Large\"><img src=\"/content/images/large.png\" alt=\"Large\" /></a><a id=\"ChangeViewToMedium\" href=\"#\" title=\"Medium\"><img src=\"/content/images/medium.png\" alt=\"Medium\" /></a><a id=\"ChangeViewToSmall\" href=\"#\" title=\"Small\"><img src=\"/content/images/small.png\" alt=\"Small\"/></a></span></a>");
            //GridBuilder.Append("</li>");
            //Font icons
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }

    public static MvcHtmlString GridChildGridTopHeader(this HtmlHelper htmlHelper, GridHeaderSettings settings)
    {
        var GridBuilder = new StringBuilder();

        //Context menu in all list pages
        if (settings.DisplayContextMenu)
        {
            GridBuilder.Append("<div id=\"divContextMenu\" class=\"ContextMenu\"><ul>");

            //Show New in context menu only if insert rights 

            GridBuilder.Append("<li><a onclick=\"TabItemClickedbyContext()\">");
            //GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New +"' src=\"/content/images/refresh.png\"><span>New</span></a></li>");    
            GridBuilder.Append("<img title='"+ ResCommon.New +"' alt='"+ ResCommon.New + "' src=\"/content/images/drildown_open.jpg\"><span>" + ResCommon.New + "</span></a></li>");

            if (!(settings.ColumnSetupFor ?? string.Empty).ToLower().StartsWith("dashboard") && (settings.ColumnSetupFor ?? string.Empty) != "ToolHistoryList")
                GridBuilder.Append("<li><a id=\"refreshGrid\"><img title=\"" + ResGridHeader.Refresh + "\" alt=\"" + ResGridHeader.Refresh + "\" src=\"/content/images/refresh.png\"><span>" + ResGridHeader.Refresh + "</span></a></li>");

            if (settings.DisplaySettings)
            {
                if (settings.ShowReorder)
                {
                    GridBuilder.Append("<li><a id=\"ColumnOrderSetupChildGrid_Context\">");
                    GridBuilder.Append("<img title=\"" + ResGridHeader.Reorder + "\" alt=\"" + ResGridHeader.Reorder + "\" src=\"/content/images/column-setup.png\"><span>" + ResGridHeader.Reorder + "</span></a></li>\r\n");
                }
            }

            GridBuilder.Append("</ul></div>");
        }

        GridBuilder.Append("<div class=\"userHead\">\r\n");

        //if (settings.DisplayGoToPage)
        //{
        //    //GridBuilder.Append("<div class=\"paginationBlock\"><span class=\"label\">" + settings.GoToPageText + ":</span><input type=\"text\" id=\"PageNumber\" class=\"inputNum\" /><input type=\"button\" id=\"Gobtn\" class=\"go\" value=\"" + eTurns.DTO.Resources.ResGridHeader.Go + "\" /></div>");
        //    GridBuilder.Append("<div class=\"InnerGridPaginationBlock\"><input type=\"button\" id=\"InnerGridGobtn\" class=\"go\" value=\"" + ResGridHeader.Go + "\" /><input type=\"text\" id=\"InnerGridPageNumber\" class=\"inputNum\" /><span class=\"label\">" + ResGridHeader.GoToPage + "</span></div>");
        //}

        GridBuilder.Append("<div class=\"BtnBlock\">");

        #region DisplayPrintBlock
        GridBuilder.Append("<div class=\"print\">");

        #endregion

        //GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        //COLUMN SETUP BUTTON
        if (settings.DisplayColumnSetupButton)
        {
            GridBuilder.Append("<div class=\"refresh setup\" style=\"display:none;\">");
            //GridBuilder.Append("<a id=\"ColumnOrderSetup\" href=\"javascript:void(null);\"><img src=\"/content/images/column-setup.png\" alt=\"\" /></a><a href=\"javascript:void(null);\" class=\"setting-arrow\"></a>");            
            GridBuilder.Append("<div id=\"ColumnSortableModalChildGrid\" style=\"display: none;\">");
            GridBuilder.Append("<div class=\"sortableContainer\">");
            GridBuilder.Append("<ul id=\"ColumnSortableChildGrid\">");
            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");

            if (settings.ShowReorder)
            {
                GridBuilder.Append("<input type=\"submit\" class=\"CreateBtn\" id=\"btnSaveOrder\" value=\"" + ResGridHeader.Reorder + "\" onclick=\"UpdateColumnOrderChildGrid('" + settings.ColumnSetupFor + "')\" />");
            }

            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        //SETTINGS BUTTON
        if (settings.DisplaySettings)
        {
            GridBuilder.Append("<div class=\"refresh\">");
            GridBuilder.Append("<a href=\"javascript:void(null);\" id=\"reordersetting\" class=\"columnsetup\"><img src=\"/content/images/setting.png\" alt=\"\" /></a>");
            GridBuilder.Append("<div class=\"refreshBlock\">");
            GridBuilder.Append("<ul>");

            if (settings.ShowReorder)
            {
                GridBuilder.Append("<li class=\"refreshBlockswf\"><a id=\"ColumnOrderSetupChildGrid\"><img src=\"/content/images/column-setup.png\" alt=\"" + ResGridHeader.Reorder + "\" title=\"" + ResGridHeader.Reorder + "\"><span>" + ResGridHeader.Reorder + "</span></a>");
                GridBuilder.Append("</li>");
            }

            GridBuilder.Append("</ul>");
            GridBuilder.Append("</div>");
            GridBuilder.Append("</div>");
        }

        GridBuilder.Append("</div>");
        GridBuilder.Append("</div>");

        return MvcHtmlString.Create(GridBuilder.ToString());
    }
}





/// <summary>
/// Specifies the type of value that represents the View type e.g. Grid View or Print View.
/// </summary>
/// <seealso cref="PagerSettings"/>
public enum DataViewType
{
    Both,
    ListView,
    PictureView,
    GroupedView,
    ListAndGrouped,
    None,
    radioButtons,
    text
}

public class GridHeaderSettings
{

    public GridHeaderSettings()
    {
        dataViewType = DataViewType.None;
        ViewTypeText = ResGridHeader.View;// "View";
        DisplayGoToPage = true;
        GoToPageText = ResGridHeader.GoToPage;// "Go to page";
        DisplayPrintBlock = true;
        DisplaySettings = true;
        DisplayDeleteButton = true;
        DisplaySaveButton = false;
        DisplayRefreshButton = true;
        DisplayUDFButton = true;
        DataTableName = "myDataTable";
        DisplayContextMenu = true;
        PictureViewhref = "";
        ListViewhref = "";
        GroupedViewhref = "";
        ShowDelete = true;
        ShowReorder = true;
        ReorderByDataTableName = false;
        ShowCartPageAction = false;
        ShowApplyButton = false;
        ShowSelectAll = false;
        ModuleName = "";
        ShowCloseButton = false;
        ShowLocationCombo = false;
        ShowCopyButton = false;
        ExportDashboardFromReportFile = false;
        BinViewhref = "";
    }

    public DataViewType dataViewType { get; set; }
    public string ViewTypeText { get; set; }

    public string GoToPageText { get; set; }
    public bool DisplayGoToPage { get; set; }
    public bool DisplayPrintBlock { get; set; }
    public bool DisplaySettings { get; set; }
    public bool ShowReorder { get; set; }
    public bool ReorderByDataTableName { get; set; }

    public bool DisplayDeleteButton { get; set; }
    public bool DisplaySaveButton { get; set; }
    public bool DisplayRefreshButton { get; set; }

    public bool DisplayUDFButton { get; set; }
    public string UDFSetupFor { get; set; }
    public string TextToDispaly { get; set; }
    public bool DisplayColumnSetupButton { get; set; }
    public string ColumnSetupFor { get; set; }

    public bool DisplayContextMenu { get; set; }

    public string DataTableName { get; set; }

    public string PictureViewhref { get; set; }
    public string GroupedViewhref { get; set; }
    public string ListViewhref { get; set; }
    public string BinViewhref { get; set; }
    public bool ShowDelete { get; set; }
    public bool ShowCartPageAction { get; set; }
    public bool ShowApplyButton { get; set; }

    public bool ShowCloseButton { get; set; }
    public bool ShowSelectAll { get; set; }
    public bool ShowLocationCombo { get; set; }
    public string ModuleName { get; set; }
    public string PullForPage { get; set; }
    public bool ShowCopyButton { get; set; }
    public string ExportModuleName { get; set; }
    public bool? ExportDashboardFromReportFile { get; set; }
    public bool DisplayArchiveButton { get; set; }
}

