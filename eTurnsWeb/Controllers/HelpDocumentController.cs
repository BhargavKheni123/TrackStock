using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTurns.DTO;
using eTurnsWeb.Helper;
using System.Configuration;

namespace eTurnsWeb.Controllers
{
    public class HelpDocumentController : eTurnsControllerBase
    {
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);

        //
        // GET: /HelpDocument/
        public ActionResult HelpDocument()
        {
            bool IsAllowHelpDocument = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.HelpDocumentPermission, eTurnsWeb.Helper.SessionHelper.PermissionType.IsChecked);
            if (SessionHelper.RoleID == -1 || IsAllowHelpDocument == true || SessionHelper.RoleID == -2)
            {
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }

        }

        public ActionResult GetHelpDocumentList(JQueryDataTableParamModelForResource param)
        {
            eTurnsMaster.DAL.HelpDocumentDAL obj = new eTurnsMaster.DAL.HelpDocumentDAL();
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            string sortColumnName = string.Empty;
            string HelpDocFilter = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            sortColumnName = Request["SortingField"].ToString();
            HelpDocFilter = Request["DocTypeFilter"].ToString();
            param.iDisplayLength *= 20;
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ModuleName desc";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ModuleName desc";

            if (string.IsNullOrWhiteSpace(HelpDocFilter))
            {
                HelpDocFilter = Convert.ToString((int)HelpDocType.Module);
            }
            //var DataFromDB = obj.GetAllRecord();
            List<HelpDocumentMasterDTO> DataFromDB = obj.GetPagedHelpDocumentRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, HelpDocFilter);
            if (DataFromDB != null && DataFromDB.Count > 0)
            {
                eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
                foreach (HelpDocumentMasterDTO objDoc in DataFromDB)
                {
                    List<HelpDocumentDetailDTO> lstDTO = objHelpDAL.GetHelpDocumentDetailByMasterID(objDoc.ID);
                    if (lstDTO != null && lstDTO.Count > 0)
                    {
                        objDoc.HelpDocDetail = lstDTO;
                    }
                }
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHelpDocumentByReportID(Int64 ReportID)
        {
            string ModuleName = string.Empty;
            string ModuleDocName = string.Empty;
            string ModuleDocPath = string.Empty;
            string ModuleVideoPath = string.Empty;
            bool IsDocHelp = false;
            bool IsVideoHelp = false;
            if (ReportID > 0)
            {
                eTurnsMaster.DAL.HelpDocumentDAL objHelpDocDAL = new eTurnsMaster.DAL.HelpDocumentDAL();
                HelpDocumentMasterDTO objHelpDTO = objHelpDocDAL.GetHelpDocumentByReportID(ReportID, SessionHelper.EnterPriseDBName);
                if (objHelpDTO != null)
                {
                    ModuleName = objHelpDTO.ModuleName;
                    ModuleDocName = objHelpDTO.ModuleDocName;
                    if (!string.IsNullOrWhiteSpace(objHelpDTO.ModuleDocPath))
                        ModuleDocPath = objHelpDTO.ModuleDocPath.Replace("..", "");
                    if (!string.IsNullOrWhiteSpace(objHelpDTO.ModuleVideoPath))
                        ModuleVideoPath = objHelpDTO.ModuleVideoPath.Replace("../Uploads/HelpVideo/", "");
                    IsDocHelp = objHelpDTO.IsDoc ?? false;
                    IsVideoHelp = objHelpDTO.IsVideo ?? false;
                }
            }

            var ObjReturn = new
            {
                vModuleName = ModuleName,
                vModuleDocName = ModuleDocName,
                vModuleDocPath = ModuleDocPath,
                vModuleVideoPath = ModuleVideoPath,
                vIsDocHelp = IsDocHelp,
                vIsVideoHelp = IsVideoHelp
            };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetHelpDocumentDetailByReportID(Int64 ReportID)
        {
            string ModuleName = string.Empty;
            bool IsDocHelp = false;
            bool IsVideoHelp = false;
            List<HelpDocumentDetailDTO> lstHelpDtlDTO = null;
            if (ReportID > 0)
            {
                eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDocDtlDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
                lstHelpDtlDTO = objHelpDocDtlDAL.GetHelpDocumentDetailByReportID(ReportID, SessionHelper.EnterPriseDBName);
                if (lstHelpDtlDTO != null && lstHelpDtlDTO.Count > 0)
                {
                    ModuleName = lstHelpDtlDTO.FirstOrDefault().ModuleName;
                    IsDocHelp = lstHelpDtlDTO.FirstOrDefault().IsDocShow;
                    IsVideoHelp = lstHelpDtlDTO.FirstOrDefault().IsVideoShow;
                }
            }

            var ObjReturn = new
            {
                vModuleName = ModuleName,
                vIsDocHelp = IsDocHelp,
                vIsVideoHelp = IsVideoHelp,
                vlstHelpDtlDTO = lstHelpDtlDTO
            };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHelpDocumentMasterByDocType(string ModuleName, int? DocType)
        {
            string HelpModuleName = string.Empty;
            bool IsDocHelp = false;
            bool IsVideoHelp = false;
            List<HelpDocumentDetailDTO> lstHelpDtlDTO = null;

            if (DocType == null)
                DocType = (int)HelpDocType.Module;

            if (!string.IsNullOrWhiteSpace(ModuleName))
            {
                eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDocDtlDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
                lstHelpDtlDTO = objHelpDocDtlDAL.GetHelpDocumentDetailByDocType(ModuleName, Convert.ToInt32(DocType));
                if (lstHelpDtlDTO != null && lstHelpDtlDTO.Count > 0)
                {
                    HelpModuleName = lstHelpDtlDTO.FirstOrDefault().ModuleName;
                    IsDocHelp = lstHelpDtlDTO.FirstOrDefault().IsDocShow;
                    IsVideoHelp = lstHelpDtlDTO.FirstOrDefault().IsVideoShow;
                }
            }

            var ObjReturn = new
            {
                vModuleName = HelpModuleName,
                vIsDocHelp = IsDocHelp,
                vIsVideoHelp = IsVideoHelp,
                vlstHelpDtlDTO = lstHelpDtlDTO
            };

            return Json(ObjReturn, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteHelpdocument(int HelpDocumentID, int HelpDocumentMasterID)
        {
            bool Result = false;
            eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
            try
            {
                string HelpDocument = SiteSettingHelper.HelpDocumentFolderPath;
                HelpDocumentDetailDTO objHelpDocumentDetailDTO = objHelpDAL.GetHelpDocumentDetailByID(HelpDocumentID);
                string DoumentPath = objHelpDocumentDetailDTO.ModuleDocPath;


                if (System.IO.File.Exists(Server.MapPath(DoumentPath)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Server.MapPath(DoumentPath));

                }
                string DodumentusingLink = Server.MapPath(HelpDocument + Convert.ToString(objHelpDocumentDetailDTO.HelpDocMasterID) + "/" + objHelpDocumentDetailDTO.ModuleDocName);
                if (System.IO.File.Exists((DodumentusingLink)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Server.MapPath(DodumentusingLink));

                }
                objHelpDAL.DeleteHelpdocument(HelpDocumentID, SessionHelper.UserID);
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult DeleteHelpVideo(int HelpVideoID, int HelpDocumentMasterID)
        {

            eTurnsMaster.DAL.HelpDocumentDetailDAL objHelpDAL = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
            try
            {
                string HelpDocument = SiteSettingHelper.HelpDocumentFolderPath;
                HelpDocumentDetailDTO objHelpDocumentDetailDTO = objHelpDAL.GetHelpDocumentDetailByID(HelpVideoID);
                string DoumentPath = objHelpDocumentDetailDTO.ModuleVideoPath;

                objHelpDAL.DeleteHelpdocument(HelpVideoID, SessionHelper.UserID);
                if (System.IO.File.Exists(Server.MapPath(DoumentPath)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Server.MapPath(DoumentPath));
                    Console.WriteLine("File deleted.");
                }
                string VideousingLink = Server.MapPath(HelpDocument + Convert.ToString(objHelpDocumentDetailDTO.HelpDocMasterID) + "/" + objHelpDocumentDetailDTO.ModuleVideoName);
                if (System.IO.File.Exists((VideousingLink)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Server.MapPath(VideousingLink));

                }
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult HowToVideos()
        {
            return View();
        }
        public ActionResult QuickStartGuide()
        {
            return View();
        }
        public ActionResult ProductTour()
        {
            return View();
        }
        public ActionResult HelpDocs()
        {
            eTurnsMaster.DAL.HelpDocumentDetailDAL obj = new eTurnsMaster.DAL.HelpDocumentDetailDAL();
            
            List<HelpDocumentDetailDTO> DataFromDB = obj.GetHelpDocumentDetailAllDocType();
            HelpDocumentMasterList helpDocumentMasterList = new HelpDocumentMasterList();
            //Inventory
            var itemspdf = DataFromDB.Where(x => x.ModuleName == "Items" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Items" && x.ModuleDocPath != null && x.ModuleDocPath != "" && x.DocType == 1).FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ModuleItemsPDFPath = (itemspdf != "" && itemspdf != null) ? itemspdf : "";
            var Quicklist = DataFromDB.Where(x => x.ModuleName == "Quicklist" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Quicklist" && x.ModuleDocPath != null && x.ModuleDocPath != "" && x.DocType == 1).FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ModuleQLPDFPath = (Quicklist != "" && Quicklist != null) ? Quicklist : "";
            var Count = DataFromDB.Where(x => x.ModuleName == "Count" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Count" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ModuleCountPDFPath = (Count != "" && Count != null) ? Count : "";
            var MaterialStaging = DataFromDB.Where(x => x.ModuleName == "MaterialStaging" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "MaterialStaging" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.MaterialStagingPDFPath = (MaterialStaging != "" && MaterialStaging != null) ? MaterialStaging : "";
            var MoveMaterial = DataFromDB.Where(x => x.ModuleName == "MoveMaterial" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "MoveMaterial" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.MoveMaterialPDFPath = (MoveMaterial != "" && MoveMaterial != null) ? MoveMaterial : "";

            //Consume
            var Pulls = DataFromDB.Where(x => x.ModuleName == "Pulls" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pulls" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.PullsPDFPath = (Pulls != "" && Pulls != null) ? Pulls : "";
            var Requisition = DataFromDB.Where(x => x.ModuleName == "Requisition" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Requisition" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RequisitionPDFPath = (Requisition != "" && Requisition != null) ? Requisition : "";
            var WorkOrder = DataFromDB.Where(x => x.ModuleName == "WorkOrder" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "WorkOrder" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.WorkOrderPDFPath = (WorkOrder != "" && WorkOrder != null) ? WorkOrder : "";
            var ProjectSpend = DataFromDB.Where(x => x.ModuleName == "ProjectSpend" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "ProjectSpend" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ProjectSpendPDFPath = (ProjectSpend != "" && ProjectSpend != null) ? ProjectSpend : "";

            //Replenish
            var Cart = DataFromDB.Where(x => x.ModuleName == "Cart" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Cart" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.CartPDFPath = (Cart != "" && Cart != null) ? Cart : "";
            var Orders = DataFromDB.Where(x => x.ModuleName == "Orders" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Orders" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.OrdersPDFPath = (Orders != "" && Orders != null) ? Orders : "";
            var Receive = DataFromDB.Where(x => x.ModuleName == "Receive" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Receive" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ReceivePDFPath = (Receive != "" && Receive != null) ? Receive : "";
            var Transfer = DataFromDB.Where(x => x.ModuleName == "Transfer" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Transfer" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.TransferPDFPath = (Transfer != "" && Transfer != null) ? Transfer : "";
            var ReturnOrder = DataFromDB.Where(x => x.ModuleName == "ReturnOrder" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "ReturnOrder" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ReturnOrderPDFPath = (ReturnOrder != "" && ReturnOrder != null) ? ReturnOrder : "";
            var Quote = DataFromDB.Where(x => x.ModuleName == "Quote" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Quote" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.QuotePDFPath = (Quote != "" && Quote != null) ? Quote : "";

            //Assets
            var Tools = DataFromDB.Where(x => x.ModuleName == "Tools" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tools" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.ToolsPDFPath = (Tools != "" && Tools != null) ? Tools : "";
            var Asset = DataFromDB.Where(x => x.ModuleName == "Asset" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Asset" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.AssetPDFPath = (Asset != "" && Asset != null) ? Asset : "";
            var Maintenance = DataFromDB.Where(x => x.ModuleName == "Maintenance" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Maintenance" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.MaintenancePDFPath = (Maintenance != "" && Maintenance != null) ? Maintenance : "";

            //Kit
            var Kits = DataFromDB.Where(x => x.ModuleName == "Kits" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Kits" && x.DocType == 1 && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.KitsPDFPath = (Kits != "" && Kits != null) ? Kits : "";

            //Reports
            var AuditTrail = DataFromDB.Where(x => x.ModuleName == "Audit Trail" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Audit Trail" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptAuditTrail = (AuditTrail != "" && AuditTrail != null) ? AuditTrail : "";
            var InventoryReconciliation = DataFromDB.Where(x => x.ModuleName == "Inventory Reconciliation" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Reconciliation" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryReconciliation = (InventoryReconciliation != "" && InventoryReconciliation != null) ? InventoryReconciliation : "";
            var PullSummarybyQuarter = DataFromDB.Where(x => x.ModuleName == "Pull Summary by Quarter" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Summary by Quarter" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullSummarybyQuarter = (PullSummarybyQuarter != "" && PullSummarybyQuarter != null) ? PullSummarybyQuarter : "";
            var AuditTrailTransaction = DataFromDB.Where(x => x.ModuleName == "AuditTrail Transaction" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "AuditTrail Transaction" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptAuditTrailTransaction = (AuditTrailTransaction != "" && AuditTrailTransaction != null) ? AuditTrailTransaction : "";
            var InventoryStockOut = DataFromDB.Where(x => x.ModuleName == "Inventory Stock Out" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Stock Out" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryStockOut = (InventoryStockOut != "" && InventoryStockOut != null) ? InventoryStockOut : "";
            var PullSummaryByWO = DataFromDB.Where(x => x.ModuleName == "Pull Summary By WO" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Summary By WO" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullSummaryByWO = (PullSummaryByWO != "" && PullSummaryByWO != null) ? PullSummaryByWO : "";
            var ItemReceivedReceivable = DataFromDB.Where(x => x.ModuleName == "Item Received Receivable" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Item Received Receivable" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptItemReceivedReceivable = (ItemReceivedReceivable != "" && ItemReceivedReceivable != null) ? ItemReceivedReceivable : "";
            var QuoteSummary = DataFromDB.Where(x => x.ModuleName == "Quote Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Quote Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptQuoteSummary = (QuoteSummary != "" && QuoteSummary != null) ? QuoteSummary : "";
            var Company = DataFromDB.Where(x => x.ModuleName == "Company" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Company" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptCompany = (Company != "" && Company != null) ? Company : "";
            var ReceivableItems = DataFromDB.Where(x => x.ModuleName == "Receivable Items" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Receivable Items" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptReceivableItems = (ReceivableItems != "" && ReceivableItems != null) ? ReceivableItems : "";
            var CreditPull = DataFromDB.Where(x => x.ModuleName == "Credit Pull" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Credit Pull" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptCreditPull = (CreditPull != "" && CreditPull != null) ? CreditPull : "";
            var ItemSerialNumber = DataFromDB.Where(x => x.ModuleName == "Item Serial Number" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Item Serial Number" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptItemSerialNumber = (ItemSerialNumber != "" && ItemSerialNumber != null) ? ItemSerialNumber : "";
            var ReceivedItems = DataFromDB.Where(x => x.ModuleName == "Received Items" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Received Items" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptReceivedItems = (ReceivedItems != "" && ReceivedItems != null) ? ReceivedItems : "";
            var CumulativePull = DataFromDB.Where(x => x.ModuleName == "Cumulative Pull" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Cumulative Pull" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptCumulativePull = (CumulativePull != "" && CumulativePull != null) ? CumulativePull : "";
            var ItemStockOutHistory = DataFromDB.Where(x => x.ModuleName == "Item Stock Out History" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Item Stock Out History" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptItemStockOutHistory = (ItemStockOutHistory != "" && ItemStockOutHistory != null) ? ItemStockOutHistory : "";
            var ReceivedItemsMoreThanApproved = DataFromDB.Where(x => x.ModuleName == "Received Items More Than Approved" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Received Items More Than Approved" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptReceivedItemsMoreThanApproved = (ReceivedItemsMoreThanApproved != "" && ReceivedItemsMoreThanApproved != null) ? ReceivedItemsMoreThanApproved : "";
            var DiscrepancyReport = DataFromDB.Where(x => x.ModuleName == "Discrepancy Report" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Discrepancy Report" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptDiscrepancyReport = (DiscrepancyReport != "" && DiscrepancyReport != null) ? DiscrepancyReport : "";
            var ItemsWithSuppliers = DataFromDB.Where(x => x.ModuleName == "Items With Suppliers" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Items With Suppliers" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptItemsWithSuppliers = (ItemsWithSuppliers != "" && ItemsWithSuppliers != null) ? ItemsWithSuppliers : "";
            var RequisitionItemSummary = DataFromDB.Where(x => x.ModuleName == "Requisition Item Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Requisition Item Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptRequisitionItemSummary = (RequisitionItemSummary != "" && RequisitionItemSummary != null) ? RequisitionItemSummary : "";
            var EnterpriseRoom = DataFromDB.Where(x => x.ModuleName == "EnterpriseRoom" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "EnterpriseRoom" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptEnterpriseRoom = (EnterpriseRoom != "" && EnterpriseRoom != null) ? EnterpriseRoom : "";
            var KitDetail = DataFromDB.Where(x => x.ModuleName == "Kit Detail" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Kit Detail" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptKitDetail = (KitDetail != "" && KitDetail != null) ? KitDetail : "";
            var RequisitionWithLineItems = DataFromDB.Where(x => x.ModuleName == "Requisition With LineItems" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Requisition With LineItems" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptRequisitionWithLineItems = (RequisitionWithLineItems != "" && RequisitionWithLineItems != null) ? KitDetail : "";
            var EnterprisesList = DataFromDB.Where(x => x.ModuleName == "Enterprises List" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Enterprises List" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptEnterprisesList = (EnterprisesList != "" && EnterprisesList != null) ? EnterprisesList : "";
            var KitSerial = DataFromDB.Where(x => x.ModuleName == "Kit Serial" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Kit Serial" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptKitSerial = (KitSerial != "" && KitSerial != null) ? KitSerial : "";
            var ReturnItemCandidates = DataFromDB.Where(x => x.ModuleName == "Return Item Candidates" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Return Item Candidates" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptReturnItemCandidates = (ReturnItemCandidates != "" && ReturnItemCandidates != null) ? ReturnItemCandidates : "";
            var EnterpriseUser = DataFromDB.Where(x => x.ModuleName == "EnterpriseUser" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "EnterpriseUser" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptEnterpriseUser = (EnterpriseUser != "" && EnterpriseUser != null) ? EnterpriseUser : "";
            var KitSummary = DataFromDB.Where(x => x.ModuleName == "Kit Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Kit Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptKitSummary = (KitSummary != "" && KitSummary != null) ? KitSummary : "";
            var Room = DataFromDB.Where(x => x.ModuleName == "Room" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Room" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptRoom = (Room != "" && Room != null) ? Room : "";
            var eVMIPollHistory = DataFromDB.Where(x => x.ModuleName == "eVMI Poll History" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "eVMI Poll History" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RpteVMIPollHistory = (eVMIPollHistory != "" && eVMIPollHistory != null) ? eVMIPollHistory : "";
            var MaintenanceDue = DataFromDB.Where(x => x.ModuleName == "Maintenance Due" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Maintenance Due" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptMaintenanceDue = (MaintenanceDue != "" && MaintenanceDue != null) ? MaintenanceDue : "";
            var Supplier = DataFromDB.Where(x => x.ModuleName == "Supplier" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Supplier" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptSupplier = (Supplier != "" && Supplier != null) ? Supplier : "";
            var eVMIUsage = DataFromDB.Where(x => x.ModuleName == "eVMI Usage" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "eVMI Usage" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RpteVMIUsage = (eVMIUsage != "" && eVMIUsage != null) ? eVMIUsage : "";
            var NotPulledReport = DataFromDB.Where(x => x.ModuleName == "Not Pulled Report" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Not Pulled Report" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptNotPulledReport = (NotPulledReport != "" && NotPulledReport != null) ? NotPulledReport : "";
            var ToolAuditTrail = DataFromDB.Where(x => x.ModuleName == "Tool Audit Trail" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tool Audit Trail" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolAuditTrail = (ToolAuditTrail != "" && ToolAuditTrail != null) ? ToolAuditTrail : "";
            var eVMIUsageManualCount = DataFromDB.Where(x => x.ModuleName == "eVMI Usage Manual Count" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "eVMI Usage Manual Count" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RpteVMIUsageManualCount = (eVMIUsageManualCount != "" && eVMIUsageManualCount != null) ? eVMIUsageManualCount : "";
            var OrderItemSummary = DataFromDB.Where(x => x.ModuleName == "Order Item Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Order Item Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptOrderItemSummary = (OrderItemSummary != "" && OrderItemSummary != null) ? OrderItemSummary : "";
            var ToolAuditTrailTransaction = DataFromDB.Where(x => x.ModuleName == "Tool Audit Trail Transaction" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tool Audit Trail Transaction" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolAuditTrailTransaction = (ToolAuditTrailTransaction != "" && ToolAuditTrailTransaction != null) ? ToolAuditTrailTransaction : "";
            var eVMIUsageNoHeader = DataFromDB.Where(x => x.ModuleName == "eVMI Usage No Header" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "eVMI Usage No Header" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RpteVMIUsageNoHeader = (eVMIUsageNoHeader != "" && eVMIUsageNoHeader != null) ? eVMIUsageNoHeader : "";
            var OrderSummary = DataFromDB.Where(x => x.ModuleName == "Order Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Order Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptOrderSummary = (OrderSummary != "" && OrderSummary != null) ? OrderSummary : "";
            var ExpiringItems = DataFromDB.Where(x => x.ModuleName == "Expiring Items" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Expiring Items" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptExpiringItems = (ExpiringItems != "" && ExpiringItems != null) ? ExpiringItems : "";
            var OrderSummaryLineItem = DataFromDB.Where(x => x.ModuleName == "Order Summary LineItem" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Order Summary LineItem" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptOrderSummaryLineItem = (OrderSummaryLineItem != "" && OrderSummaryLineItem != null) ? OrderSummaryLineItem : "";
            var ToolAssetOrder = DataFromDB.Where(x => x.ModuleName == "ToolAssetOrder" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "ToolAssetOrder" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolAssetOrder = (ToolAssetOrder != "" && ToolAssetOrder != null) ? ToolAssetOrder : "";
            var InStock = DataFromDB.Where(x => x.ModuleName == "InStock" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "InStock" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInStock = (InStock != "" && InStock != null) ? InStock : "";
            var OrdersClosed = DataFromDB.Where(x => x.ModuleName == "Orders Closed" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Orders Closed" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptOrdersClosed = (OrdersClosed != "" && OrdersClosed != null) ? OrdersClosed : "";
            var ToolAssetOrdersWithLineItems = DataFromDB.Where(x => x.ModuleName == "ToolAssetOrders With LineItems" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "ToolAssetOrders With LineItems" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolAssetOrdersWithLineItems = (ToolAssetOrdersWithLineItems != "" && ToolAssetOrdersWithLineItems != null) ? ToolAssetOrdersWithLineItems : "";
            var InStockByActivity = DataFromDB.Where(x => x.ModuleName == "InStock By Activity" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "InStock By Activity" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInStockByActivity = (InStockByActivity != "" && InStockByActivity != null) ? InStockByActivity : "";
            var OrdersWithLineItems = DataFromDB.Where(x => x.ModuleName == "Orders With LineItems" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Orders With LineItems" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptOrdersWithLineItems = (OrdersWithLineItems != "" && OrdersWithLineItems != null) ? OrdersWithLineItems : "";
            var Toolscheckedout = DataFromDB.Where(x => x.ModuleName == "Tools checked out" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tools checked out" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolscheckedout = (Toolscheckedout != "" && Toolscheckedout != null) ? Toolscheckedout : "";
            var InStockMargin = DataFromDB.Where(x => x.ModuleName == "InStock Margin" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "InStock Margin" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInStockMargin = (InStockMargin != "" && InStockMargin != null) ? InStockMargin : "";
            var PreciseDemandPlanning = DataFromDB.Where(x => x.ModuleName == "Precise Demand Planning" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Precise Demand Planning" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPreciseDemandPlanning = (PreciseDemandPlanning != "" && PreciseDemandPlanning != null) ? PreciseDemandPlanning : "";
            var ToolsCheckInoutHistory = DataFromDB.Where(x => x.ModuleName == "Tools CheckIn-out History" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tools CheckIn-out History" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolsCheckInoutHistory = (ToolsCheckInoutHistory != "" && ToolsCheckInoutHistory != null) ? ToolsCheckInoutHistory : "";
            var InstockwithQOH = DataFromDB.Where(x => x.ModuleName == "Instock with QOH" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Instock with QOH" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInstockwithQOH = (InstockwithQOH != "" && InstockwithQOH != null) ? InstockwithQOH : "";
            var PreciseDemandPlanningByItem = DataFromDB.Where(x => x.ModuleName == "Precise Demand Planning By Item" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Precise Demand Planning By Item" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPreciseDemandPlanningByItem = (PreciseDemandPlanningByItem != "" && PreciseDemandPlanningByItem != null) ? PreciseDemandPlanningByItem : "";
            var TransferWithLineItems = DataFromDB.Where(x => x.ModuleName == "Transfer With LineItems" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Transfer With LineItems" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptTransferWithLineItems = (TransferWithLineItems != "" && TransferWithLineItems != null) ? TransferWithLineItems : "";
            var InventoryCountConsigned = DataFromDB.Where(x => x.ModuleName == "Inventory Count - Consigned" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Count - Consigned" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryCountConsigned = (InventoryCountConsigned != "" && InventoryCountConsigned != null) ? InventoryCountConsigned : "";
            var PullCompleted = DataFromDB.Where(x => x.ModuleName == "Pull Completed" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Completed" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullCompleted = (PullCompleted != "" && PullCompleted != null) ? PullCompleted : "";
            var Users = DataFromDB.Where(x => x.ModuleName == "Users" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Users" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptUsers = (Users != "" && Users != null) ? Users : "";
            var InventoryCountCustomerOwned = DataFromDB.Where(x => x.ModuleName == "Inventory Count - Customer Owned" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Count - Customer Owned" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryCountCustomerOwned = (InventoryCountCustomerOwned != "" && InventoryCountCustomerOwned != null) ? InventoryCountCustomerOwned : "";
            var PullIncomplete = DataFromDB.Where(x => x.ModuleName == "Pull Incomplete" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Incomplete" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullIncomplete = (PullIncomplete != "" && PullIncomplete != null) ? PullIncomplete : "";
            var WorkOrderLastCost = DataFromDB.Where(x => x.ModuleName == "Work Order Last Cost" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Work Order Last Cost" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptWorkOrderLastCost = (WorkOrderLastCost != "" && WorkOrderLastCost != null) ? WorkOrderLastCost : "";
            var InventoryDailyHistory = DataFromDB.Where(x => x.ModuleName == "Inventory Daily History" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Daily History" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryDailyHistory = (InventoryDailyHistory != "" && InventoryDailyHistory != null) ? InventoryDailyHistory : "";
            var PullItemSummary = DataFromDB.Where(x => x.ModuleName == "Pull Item Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Item Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullItemSummary = (PullItemSummary != "" && PullItemSummary != null) ? PullItemSummary : "";
            var WorkOrderWithAttachment = DataFromDB.Where(x => x.ModuleName == "Work Order With Attachment" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Work Order With Attachment" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptWorkOrderWithAttachment = (WorkOrderWithAttachment != "" && WorkOrderWithAttachment != null) ? WorkOrderWithAttachment : "";
            var InventoryDailyHistoryWithDateRange = DataFromDB.Where(x => x.ModuleName == "Inventory Daily History With Date Range" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Inventory Daily History With Date Range" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptInventoryDailyHistoryWithDateRange = (InventoryDailyHistoryWithDateRange != "" && InventoryDailyHistoryWithDateRange != null) ? InventoryDailyHistoryWithDateRange : "";
            var PullNoHeader = DataFromDB.Where(x => x.ModuleName == "Pull No Header" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull No Header" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullNoHeader = (PullNoHeader != "" && PullNoHeader != null) ? PullNoHeader : "";
            var WorkOrderWithGroupedPulls = DataFromDB.Where(x => x.ModuleName == "Work Order With Grouped Pulls" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Work Order With Grouped Pulls" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptWorkOrderWithGroupedPulls = (WorkOrderWithGroupedPulls != "" && WorkOrderWithGroupedPulls != null) ? WorkOrderWithGroupedPulls : "";
            var PullSummary = DataFromDB.Where(x => x.ModuleName == "Pull Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullSummary = (PullSummary != "" && PullSummary != null) ? PullSummary : "";
            var WorkordersList = DataFromDB.Where(x => x.ModuleName == "Workorders List" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Workorders List" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptWorkordersList = (WorkordersList != "" && WorkordersList != null) ? WorkordersList : "";
            var PullSummaryByConsignedPO = DataFromDB.Where(x => x.ModuleName == "Pull Summary By ConsignedPO" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Pull Summary By ConsignedPO" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptPullSummaryByConsignedPO = (PullSummaryByConsignedPO != "" && PullSummaryByConsignedPO != null) ? PullSummaryByConsignedPO : "";
            var WrittenOffTools = DataFromDB.Where(x => x.ModuleName == "WrittenOffTools" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "WrittenOffTools" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptWrittenOffTools = (WrittenOffTools != "" && WrittenOffTools != null) ? WrittenOffTools : "";
            var AuditTrailTransactionSummary = DataFromDB.Where(x => x.ModuleName == "AuditTrail Transaction Summary" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "AuditTrail Transaction Summary" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptAuditTrailTransactionSummary = (AuditTrailTransactionSummary != "" && AuditTrailTransactionSummary != null) ? AuditTrailTransactionSummary : "";
            var ItemSerialLotDatcode = DataFromDB.Where(x => x.ModuleName == "Item Serial Lot Datcode" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Item Serial Lot Datcode" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptItemSerialLotDatcode = (ItemSerialLotDatcode != "" && ItemSerialLotDatcode != null) ? ItemSerialLotDatcode : "";
            var ToolInstock = DataFromDB.Where(x => x.ModuleName == "Tool Instock" && x.DocType == (int)HelpDocType.Report && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault() != null ? DataFromDB.Where(x => x.ModuleName == "Tool Instock" && x.ModuleDocPath != null && x.ModuleDocPath != "").FirstOrDefault().ModuleDocPath.ToString() : "";
            helpDocumentMasterList.RptToolInstock = (ToolInstock != "" && ToolInstock != null) ? ToolInstock : "";

            return View(helpDocumentMasterList);
        }

    }
}
