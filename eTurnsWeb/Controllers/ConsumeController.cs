using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ConsumeController : eTurnsControllerBase
    {
        #region "Private Variables"

        bool IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RequisitionApproval, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
        bool IsClose = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RequisitionClosing, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
        UDFController objUDFDAL = new UDFController();

        #endregion

        #region "Requisition Master"

        public ActionResult RequisitionList()
        {
            return View();
        }


        public ActionResult RequisitionCreate()
        {
            Int64 DefualtRoomSupplier = 0;
            int DefaultRequisitionRequiredDays = 0;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSupplier = null;

            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
            RequisitionMasterDAL requisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);

            int? _AttachingWOWithRequisition = (int)AttachingWOWithRequisition.New;
            string columnList = "ID,RoomName,DefaultSupplierID,DefaultRequisitionRequiredDays,AttachingWOWithRequisition";
            RoomDTO objRoom = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            //RoomDTO objRoom = objRoomDAL.GetRoomByIDPlain(RoomID);
            if (objRoom != null)
            {
                DefualtRoomSupplier = objRoom.DefaultSupplierID.GetValueOrDefault(0);
                DefaultRequisitionRequiredDays = objRoom.DefaultRequisitionRequiredDays.GetValueOrDefault(0);
                _AttachingWOWithRequisition = objRoom.AttachingWOWithRequisition.GetValueOrDefault((int)AttachingWOWithRequisition.New);
            }

            AutoSequenceDAL objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
            AutoOrderNumberGenerate objAutoNumber = null;
            objAutoNumber = objAutoSeqDAL.GetNextRequisitionNumber(SessionHelper.RoomID, SessionHelper.CompanyID, 0,SessionHelper.EnterPriceID);

            string requisitionNumber = objAutoNumber.RequisitionNumber;
            if (requisitionNumber != null && (!string.IsNullOrEmpty(requisitionNumber)))
            {
                requisitionNumber = requisitionNumber.Length > 22 ? requisitionNumber.Substring(0, 22) : requisitionNumber;
            }

            string releaseNo = "1";

            if (!string.IsNullOrWhiteSpace(requisitionNumber))
            {
                var releaseNumber = requisitionMasterDAL.GetRequisitionReleaseNumber(0, requisitionNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
                releaseNo = Convert.ToString(releaseNumber);
            }

            RequisitionMasterDTO objDTO = new RequisitionMasterDTO()
            {
                RequiredDate = datetimetoConsider.AddDays(DefaultRequisitionRequiredDays),
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                UpdatedByName = SessionHelper.UserName,
                Room = SessionHelper.RoomID,
                RoomName = SessionHelper.RoomName,
                CompanyID = SessionHelper.CompanyID,
                IsArchived = false,
                IsDeleted = false,
                //WorkorderName = "#R" + nextWONo,
                AutoOrderNumber = objAutoNumber,
                RequisitionStatus = "Unsubmitted",
                RequisitionNumber = requisitionNumber, //nextReqNo,
                ReleaseNumber = releaseNo,
                RequisitionType = "Requisition",
                SupplierId = DefualtRoomSupplier,
                AttachingWOWithRequisition = _AttachingWOWithRequisition

            };
            SupplierMasterDAL objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()
                        && (lstSupplier != null && lstSupplier.Any()))
            {
                lstSupplier = lstSupplier.Where(x => SessionHelper.UserSupplierIds.Contains(x.ID)).OrderBy(x => x.SupplierName).ToList();
            }

            if (lstSupplier == null)
            {
                lstSupplier = new List<SupplierMasterDTO>();
            }

            lstSupplier.Insert(0, null);

            ViewBag.SupplierList = lstSupplier;

            objDTO.SupplierAccountGuid = Guid.Empty;
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(objDTO.SupplierId), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            objDTO.IsRecordEditable = IsRecordEditable(objDTO);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room WITH (NOLOCK) where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            #region "Project Spend DropDownList"

            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);
                ViewBag.ProjectSpent = lstProject;
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            #region [Technician DropdownList]
            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });

            ViewBag.TechnicianBAG = technicianlist.OrderBy(t => t.TechnicianCode).ToList();
            #endregion

            #region [WorkOrder DropdownList]
            WorkOrderDAL objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> WOList = new List<WorkOrderDTO>();
            if (_AttachingWOWithRequisition > (int)AttachingWOWithRequisition.New)
            {
                //objWODAL.GetAllWorkOrders(RoomID, CompanyID, new string[] { "WorkOrder", "Reqn" })
                string tmpWOType = "WorkOrder,Reqn";
                WOList = objWODAL.GetWorkOrdersByRoomWOTypeAndStatusPlain(SessionHelper.RoomID, SessionHelper.CompanyID, tmpWOType).OrderBy(x => x.WOName).ToList();                
            }

            ViewBag.WorkOrderBAG = WOList;
            #endregion

            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = "Unsubmitted" });
            ViewBag.RequisitionStatusBag = ItemType;

            ViewBag.RequisitionTypeBag = GetRequisitionType();
            objDTO.RequiredDateStr = objDTO.RequiredDate.HasValue ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
            SetStagingViewBag();
            Session["REQEvent"] = "ORQC";

            return PartialView("_CreateRequisition", objDTO);
        }

        public JsonResult GetAllWorkOrdersForRequisition(string NameStartWith)
        {
            WorkOrderDAL objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            string tmpWOType = "WorkOrder,Reqn";
            IEnumerable<WorkOrderDTO> WOList = objWODAL.GetWorkOrdersByRoomWOTypeAndStatusPlain(SessionHelper.RoomID, SessionHelper.CompanyID, tmpWOType).OrderBy(x => x.WOName);                

            if (WOList != null && WOList.Any() && WOList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    WOList = WOList.Where(x => x.WOName.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.WOName);
                    return Json(WOList, JsonRequestBehavior.AllowGet);
                }
                else if (NameStartWith.Contains(" "))
                {
                    return Json(WOList, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(WOList, JsonRequestBehavior.AllowGet);
        }

        public bool PrintFile(string sFileName, string sPrinter)
        {
            string sArgs = " /t \"" + sFileName + "\" \"" + sPrinter + "\"";
            System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Program Files (x86)\Foxit Software\Foxit Reader\Foxit Reader.exe";
            //C:\Program Files (x86)\Foxit Software\Foxit Reader
            startInfo.Arguments = sArgs;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            System.Diagnostics.Process proc = Process.Start(startInfo);
            proc.WaitForExit(10000); // Wait a maximum of 10 sec for the process to finish
            if (!proc.HasExited)
            {
                proc.Kill();
                proc.Dispose();
                return false;
            }
            return true;
        }

        /// 
        /// Get a list of installed printers.
        /// 
        /// A subclass of List which contains instances of the Printer class.
        //public List<SelectListItem> GetPrinterList()
        //{
        //    List<SelectListItem> lstPrinters = new List<SelectListItem>();
        //    using (ManagementClass printerClass = new ManagementClass("win32_printer"))
        //    {
        //        ManagementObjectCollection printers = printerClass.GetInstances();
        //        foreach (ManagementObject printer in printers)
        //        {
        //            if ((bool)printer["Shared"] == true)
        //            {
        //                lstPrinters.Add(new SelectListItem() { Text = Convert.ToString(printer["Name"]), Value = Convert.ToString(printer["ShareName"]) });
        //            }
        //            //lstPrinters.Add(new SelectListItem((string)printer["Name"], (string)printer["ShareName"]));
        //        }
        //    }
        //    return lstPrinters;
        //}

        public ActionResult GetRequisitionFiles(Guid RequisitionGuid)
        {
            RequisitionImageDetailDAL reqImageDetailDAL = new RequisitionImageDetailDAL(SessionHelper.EnterPriseDBName);
            List<RequisitionImageDetailDTO> listRequisitionImageDetail = reqImageDetailDAL.GetRequisitionImagesByGuidPlain(RequisitionGuid).ToList();
            Dictionary<string, Guid> retData = new Dictionary<string, Guid>();

            if (listRequisitionImageDetail != null && listRequisitionImageDetail.Any())
            {
                foreach (RequisitionImageDetailDTO woimg in listRequisitionImageDetail.Where(e => !string.IsNullOrEmpty(e.ImageName) && !string.IsNullOrWhiteSpace(e.ImageName)))
                {
                    if (!retData.ContainsKey(woimg.ImageName))
                    {
                        retData.Add(woimg.ImageName, woimg.GUID);
                    }
                }
            }

            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);

        }

        public int DeleteReqExistingFiles(string FileId, Guid requisitionGuid)
        {
            try
            {
                RequisitionImageDetailDAL objReqImageDetailDAL = new RequisitionImageDetailDAL(SessionHelper.EnterPriseDBName);
                objReqImageDetailDAL.DeleteRecords(FileId, SessionHelper.UserID, requisitionGuid);
                return 1;
            }
            catch
            {
                return 0;
            }
        }


        public ActionResult RequisitionEdit(string RequisitionGUID)
        {
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            Guid mntsGUID = Guid.Empty;
            List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
            bool IsHitory = false;
            bool IsArchived = false;
            bool IsDeleted = false;

            if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
            if (Request["IsDeleted"] != null && !string.IsNullOrEmpty(Request["IsDeleted"].ToString()))
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());

            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);

            ViewBag.CanBeUnclosed = !IsArchived;
            int? _AttachingWOWithRequisition = (int)AttachingWOWithRequisition.New;
            string columnList = "ID,RoomName,AttachingWOWithRequisition";
            RoomDTO objRoom = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            //RoomDTO objRoom = objRoomDAL.GetRoomByIDPlain(RoomID);
            if (objRoom != null)
            {
                _AttachingWOWithRequisition = objRoom.AttachingWOWithRequisition.GetValueOrDefault((int)AttachingWOWithRequisition.New);
            }

            RequisitionMasterDTO objDTO = IsArchived ? obj.GetArchivedRequisitionByGUIDFull(Guid.Parse(RequisitionGUID)) : obj.GetRequisitionByGUIDFull(Guid.Parse(RequisitionGUID));

            if (objDTO.WorkorderGUID != null && objDTO.WorkorderGUID != Guid.Empty)
            {
                //WorkOrderDTO objWODTO = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID ?? Guid.Empty);
                //if (objWODTO != null)
                //{
                //    objDTO.Technician = objWODTO.Technician;
                //    objDTO.TechnicianID = objWODTO.TechnicianID;
                //}
            }

            if (IsHitory)
            {
                objDTO.IsRecordEditable = false;
                objDTO.IsHistory = true;
            }
            else
                objDTO.IsRecordEditable = IsRecordEditable(objDTO);

            objDTO.IsStagingEditable = true;

            if (objDTO != null)
            {
                var requisitionLineItems = objReqDDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                Double PullSum = requisitionLineItems.Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();
                ViewBag.PulledQuantity = PullSum;

                if ((objDTO.RequisitionStatus.ToUpper() != "UNSUBMITTED" && objDTO.RequisitionStatus.ToUpper() != "SUBMITTED") || (requisitionLineItems != null && requisitionLineItems.Any()))
                {
                    objDTO.IsStagingEditable = false;
                }
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objAutoSeqDAL = new AutoSequenceDAL(SessionHelper.EnterPriseDBName);
            objAutoNumber = objAutoSeqDAL.GetNextWorkOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);
            string nextWONo = objAutoNumber.OrderNumber;

            if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
            {
                nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
            }

            ViewBag.WOName = nextWONo;
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room WITH (NOLOCK) where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);


            #region "Project Spend DropDownList"

            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }
                ViewBag.ProjectSpent = lstProject.OrderBy(x => x.ProjectSpendName).ToList();
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            #region [Technician DropdownList]
            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrWhiteSpace(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });
            ViewBag.TechnicianBAG = technicianlist.OrderBy(t => t.TechnicianCode).ToList();

            if (objDTO.TechnicianID != null)
            {
                objDTO.Technician = technicianlist.Where(p => p.ID == objDTO.TechnicianID).FirstOrDefault().Technician;
            }
                #endregion


                #region [WorkOrder DropdownList]
                WorkOrderDAL objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
            List<WorkOrderDTO> WOList = new List<WorkOrderDTO>();
            if (_AttachingWOWithRequisition > (int)AttachingWOWithRequisition.New)
            {
                //objWODAL.GetAllWorkOrders(RoomID, CompanyID, new string[] { "WorkOrder", "Reqn" })
                string tmpWOType = "WorkOrder,Reqn";
                WOList = objWODAL.GetWorkOrdersByRoomWOTypeAndStatusPlain(SessionHelper.RoomID, SessionHelper.CompanyID, tmpWOType).OrderBy(x => x.WOName).ToList();                
            }

            ViewBag.WorkOrderBAG = WOList;
            #endregion

            ViewBag.RequisitionStatusBag = GetRequisitionStatus(objDTO.RequisitionStatus);
            ViewBag.RequisitionTypeBag = GetRequisitionType();
            objDTO.rdoApprovelChoice = 3;
            objDTO.RequiredDateStr = objDTO.RequiredDate.HasValue ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
            objDTO.TotalCost = obj.GetTotal(Guid.Parse(RequisitionGUID), SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions);
            objDTO.TotalSellPrice = obj.GetTotalSellPrice(Guid.Parse(RequisitionGUID), SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions);
            SupplierMasterDAL objSupDAL = null;
            objSupDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                string strSupplierIds = string.Empty;
                strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);

                var suppliers = objSupDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (suppliers != null && suppliers.Any())
                {
                    lstSupplier.AddRange(suppliers);
                }
                if (lstSupplier != null && lstSupplier.Any() && lstSupplier.Count() > 0)
                {
                    lstSupplier = lstSupplier.OrderBy(x => x.SupplierName).ToList();
                }
            }
            else
            {
                lstSupplier = objSupDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false).OrderBy(x => x.SupplierName).ToList();
            }

            lstSupplier.Insert(0, null);
            ViewBag.SupplierList = lstSupplier;
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.SupplierId.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
            objDTO.AttachingWOWithRequisition = _AttachingWOWithRequisition;
            Session["REQEvent"] = "ORQE";
            SetStagingViewBag();
            return PartialView("_CreateRequisition", objDTO);
        }

        public ActionResult RequisitionEditMainatenance(Int64 ID)
        {
            bool IsHitory = true;
            ViewBag.FromMaintenance = true;
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objDTO = obj.GetRequisitionByIDFull(ID);

            if (objDTO.WorkorderGUID != null && objDTO.WorkorderGUID != Guid.Empty)
            {
                if (objDTO.WorkorderGUID.HasValue)
                {
                    objDTO.WorkorderName = new WorkOrderDAL(SessionHelper.EnterPriseDBName).GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID.Value).WOName;
                }
            }

            if (IsHitory)
            {
                objDTO.IsRecordEditable = false;
                objDTO.IsHistory = true;
            }
            else
                objDTO.IsRecordEditable = IsRecordEditable(objDTO);

            if (objDTO != null)
            {
                Double PullSum = objReqDDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID).Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();
                ViewBag.PulledQuantity = PullSum;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room WITH (NOLOCK) where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            #region "Project Spend DropDownList"

            //ProjectMasterController objProjectApi = new ProjectMasterController();
            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }
                ViewBag.ProjectSpent = lstProject.OrderBy(x => x.ProjectSpendName).ToList();
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            ViewBag.RequisitionStatusBag = GetRequisitionStatus(objDTO.RequisitionStatus);
            ViewBag.RequisitionTypeBag = GetRequisitionType();
            SetStagingViewBag();
            return PartialView("_CreateRequisition", objDTO);
        }

        [NonAction]
        private List<CommonDTO> GetRequisitionStatus(string RequisitionStatus)
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            if (RequisitionStatus == "Unsubmitted")
            {
                ItemType.Add(new CommonDTO() { Text = "Unsubmitted" });
                ItemType.Add(new CommonDTO() { Text = "Submitted" });
                if (IsApprove)
                {
                    ItemType.Add(new CommonDTO() { Text = "Approved" });
                }

                if (IsClose)
                {
                    ItemType.Add(new CommonDTO() { Text = "Closed" });
                }
            }
            else if (RequisitionStatus == "Submitted")
            {
                ItemType.Add(new CommonDTO() { Text = "Submitted" });
                if (IsApprove)
                {
                    ItemType.Add(new CommonDTO() { Text = "Approved" });
                    //ItemType.Add(new CommonDTO() { Text = "Closed" });
                }

                if (IsClose)
                {
                    ItemType.Add(new CommonDTO() { Text = "Closed" });
                }
            }
            else if (RequisitionStatus == "Approved")
            {
                ItemType.Add(new CommonDTO() { Text = "Approved" });
                if (IsClose)
                {
                    ItemType.Add(new CommonDTO() { Text = "Closed" });
                }
            }
            else if (RequisitionStatus == "Closed")
            {
                ItemType.Add(new CommonDTO() { Text = "Closed" });
            }

            return ItemType;
        }

        [NonAction]
        private List<CommonDTO> GetRequisitionType()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResRequisitionMaster.Requisition });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeWorkorder });
            ItemType.Add(new CommonDTO() { Text = ResRequisitionMaster.ToolService });
            ItemType.Add(new CommonDTO() { Text = ResRequisitionMaster.AssetService });
            return ItemType;
        }
        /// <summary>
        /// UpdateRequistionToClose
        /// </summary>
        /// <param name="RequisitionID"></param>
        /// <returns></returns>
        public JsonResult UpdateRequistionToClose(Int64 RequisitionID)
        {
            RequisitionMasterDAL obj = null;
            RequisitionMasterDTO objDTO = null;
            try
            {
                string requisitionDataLog = "Method - UpdateRequistionToClose; Controller-ConsumeController : on " + DateTime.UtcNow.ToString();
                obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                objDTO = obj.GetRequisitionByIDPlain(RequisitionID);
                objDTO.WhatWhereAction = "Requisition";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.EditedFrom = "Web";
                objDTO.RequisitionStatus = "Closed";

                requisitionDataLog = requisitionDataLog + "; " + "RequisitionStatus-Closed,Username :" + (SessionHelper.UserName != null ? SessionHelper.UserName : "");
                objDTO.RequisitionDataLog = requisitionDataLog;
                bool ReturnVal = obj.Edit(objDTO);

                obj.CloseRequisition(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                //---------------------------------------------------------------------------------
                //
                RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<RequisitionDetailsDTO> lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                long SessionUserId = SessionHelper.UserID;

                if (lstRequisitionDetails != null && lstRequisitionDetails.Count() > 0)
                {
                    foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionDetails)
                    {
                        if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                        {
                            objRequisitionDetailsDAL.UpdateRequisitionedQuantity("Delete", objRequisitionDetails, objRequisitionDetails);
                        }
                        //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Req.DetailDAL >> Save Requisition Close");
                        new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Consume >> Close Requisition", SessionUserId);
                        if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                        {
                            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty));
                            if (ItemDTO != null && ItemDTO.ID > 0)
                            {
                                /* WI-5009 update QtyToMeetDemand */
                                if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                                {
                                    new KitDetailDAL(SessionHelper.EnterPriseDBName).UpdateQtyToMeedDemand(ItemDTO.GUID, SessionHelper.UserID, SessionUserId);
                                }
                                /* WI-5009 update QtyToMeetDemand */
                            }
                        }
                    }
                }

                if (ReturnVal)
                {
                    CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                    CustomerMasterDTO customer = new CustomerMasterDTO();
                    customer = objcust.GetCustomerByGUID(objDTO.CustomerGUID ?? Guid.Empty);

                    WorkOrderDAL objwo = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                    WorkOrderDTO WO = new WorkOrderDTO();
                    WO = objwo.GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID ?? Guid.Empty);
                    if (WO != null && customer != null)
                    {
                        WO.CustomerID = customer.ID;
                        WO.Customer = customer.Customer;
                        WO.CustomerGUID = objDTO.CustomerGUID;
                        objwo.Edit(WO);
                    }
                    if (WO != null && objDTO.CustomerGUID == null)
                    {
                        WO.CustomerID = null;
                        WO.Customer = null;
                        WO.CustomerGUID = null;
                        objwo.Edit(WO);
                    }
                    return Json(new { Message = ResRequisitionMaster.MsgRequisitionClosedSuccessfully, Status = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = ResRequisitionMaster.MsgRequisitionNotClosed, Status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                obj = null;
                objDTO = null;
            }


        }
        public ActionResult RequisitionMasterListAjax(JQueryDataTableParamModel param)
        {
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "requisitionnumber ASC";
            }
            else
                sortColumnName = "requisitionnumber ASC";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.Trim().ToLower().Contains("requisitionnumber"))
            {
                //  sortColumnName = sortColumnName.ToLower().Replace("requisitionnumber", "requisitionnumberforsorting");
            }

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            //string MainFilter = "";
            if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
            {
                //MainFilter = "true";
            }
            string reqType = string.Empty;
            bool UserConsignmentAllowed = true;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsRequisitionList(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, UserConsignmentAllowed, reqType, string.Empty, 0, ChartHelper.GetReqStatus(), ChartHelper.GetReqTypes(), false, SessionHelper.RoomDateFormat, CurrentTimeZone).ToList();
            //RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
            string columnList = "ID,RoomName,RequestedXDays,RequestedYDays";
            RoomDTO roomDTO = new CommonDAL(SessionHelper.EnterPriseDBName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            int RequestedXDays = 0;
            int RequestedYDays = 0;
            if (roomDTO != null && roomDTO.ID > 0)
            {
                RequestedXDays = roomDTO.RequestedXDays.GetValueOrDefault(0);
                RequestedYDays = roomDTO.RequestedYDays.GetValueOrDefault(0);
            }
            DataFromDB.ForEach(x =>
            {
                x.IsRecordEditable = IsRecordEditable(x);
                x.ShowRequisitionPullNotification = GetRequisitionPullNotification(x, RequestedXDays, RequestedYDays);
                x.RequiredDateStr = x.RequiredDate.HasValue ?  x.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public DataTable GetReqStatus()
        {
            DataTable ReqStatus = new DataTable();
            DataColumn Dt = new DataColumn("StatusName", typeof(string));
            Dt.MaxLength = 255;
            ReqStatus.Columns.Add(Dt);
            Dt = new DataColumn("StatusValue", typeof(string));
            Dt.MaxLength = 255;
            ReqStatus.Columns.Add(Dt);

            ReqStatus.Rows.Add("Unsubmitted", ResRequisitionMaster.Unsubmitted);
            ReqStatus.Rows.Add("Submited", ResRequisitionMaster.Submitted);
            ReqStatus.Rows.Add("Approved", ResRequisitionMaster.Approved);
            ReqStatus.Rows.Add("Closed", ResRequisitionMaster.Closed);
            return ReqStatus;
        }

        public DataTable GetReqTypes()
        {
            DataTable ReqType = new DataTable();
            DataColumn Dt = new DataColumn("StatusName", typeof(string));
            Dt.MaxLength = 255;
            ReqType.Columns.Add(Dt);
            Dt = new DataColumn("StatusValue", typeof(string));
            Dt.MaxLength = 255;
            ReqType.Columns.Add(Dt);

            ReqType.Rows.Add("Asset Service", ResRequisitionMaster.AssetService);
            ReqType.Rows.Add("Requisition", ResRequisitionMaster.Requisition);
            ReqType.Rows.Add("Tool Service", ResRequisitionMaster.ToolService);
            return ReqType;
        }

        public string GetRequisitionPullNotification(RequisitionMasterDTO obj, int RequireXDays, int RequireyDays)
        {

            if (RequireXDays > 0 && obj.RequiredDate.HasValue && obj.RequisitionStatus != "Closed")
            {

                DateTime PastStartDate = obj.RequiredDate.Value.AddDays(RequireXDays * (-1)).Date;
                if (DateTimeUtility.DateTimeNow.Date >= PastStartDate)
                {
                    return "Red";
                }
                return "Green";
            }
            else
            {
                return "";
            }
        }
        public string GetCustName(Int64 CustID, bool IsArchived, bool IsDeleted)
        {
            CustomerMasterDAL objCustDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            var CustDATA = objCustDAL.GetCustomerByID(CustID);
            if (CustDATA != null)
            {
                return CustDATA.Customer;
            }
            else
                return "";
        }

        public JsonResult DeleteRequisitionMasterRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                //CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //response = objCommonDAL.DeleteModulewise(ids, "Requisition", true, SessionHelper.UserID);
                long SessionUserId = SessionHelper.UserID;
                RequisitionMasterDAL objReqDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                if (objReqDAL.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionUserId,SessionHelper.EnterPriceID))
                {
                    //eTurns.DAL.CacheHelper<IEnumerable<RequisitionMasterDTO>>.InvalidateCache();
                    return Json(new { Message = string.Format(ResCommon.MsgDeletedSuccessfully,ResRequisitionMaster.Requisition), Status = "ok" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = ResRequisitionMaster.MsgRequisitionNotDeleted, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);

            }
        }

        public string RequisitionDetails(Int64 RequisitionID)
        {
            ViewBag.RequisitionID = RequisitionID;

            RequisitionMasterDAL objRequisition = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            var objREquisitionDATA = objRequisition.GetRequisitionByIDPlain(RequisitionID);

            ViewBag.RequisitionGUID = objREquisitionDATA.GUID;

            RequisitionDetailsDAL objAPI = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetReqLinesByReqGUIDFull(objREquisitionDATA.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);

            return RenderRazorViewToString("_RequisitionDetails", objModel);
        }

        public ActionResult ItemRequisitionDetailsAjax(JQueryDataTableParamModel param)
        {
            Int64 RequisitionID = Int64.Parse(Request["ItemID"].ToString());

            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////
            ViewBag.RequisitionID = RequisitionID;
            RequisitionMasterDAL objRequisition = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            var objRequisitionDATA = objRequisition.GetRequisitionByIDPlain(RequisitionID);

            ViewBag.RequisitionGUID = objRequisitionDATA.GUID;

            RequisitionDetailsDAL objAPI = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            //var objModel = objAPI.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, objRequisitionDATA.GUID);
            var objModel = new List<RequisitionDetailsDTO>();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }
        public string RequisitionDetailsDelete(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                RequisitionDetailsDAL obj = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionUserId,SessionHelper.EnterPriceID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult UpdatePullData(string RequisitionDetailGUID, string RequisitionGUID, string ItemGUID, string PullGUID, Int64 BinID, string ProjectGUID, int PullQuantity, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5)
        {
            bool IsPSLimitExceed = false;
            string message = "";
            string status = "";
            string locationMSG = "";
            //PullAPIController obj = new PullAPIController();
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            PullMasterViewDTO objDTO = new PullMasterViewDTO();
            objDTO.GUID = Guid.Parse(PullGUID);
            objDTO.ItemGUID = Guid.Parse(ItemGUID);

            if (BinID != 0)
                objDTO.BinID = BinID;
            if (ProjectGUID != string.Empty)
                objDTO.ProjectSpendGUID = Guid.Parse(ProjectGUID);
            if (PullQuantity != 0)
            {
                objDTO.PoolQuantity = PullQuantity;
                objDTO.TempPullQTY = PullQuantity;
            }
            objDTO.UDF1 = UDF1;
            objDTO.UDF2 = UDF2;
            objDTO.UDF3 = UDF3;
            objDTO.UDF4 = UDF4;
            objDTO.UDF5 = UDF5;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";

            if (!string.IsNullOrEmpty(ItemGUID))
                objDTO.ItemGUID = Guid.Parse(ItemGUID);
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.PullCredit = "pull";
            long SessionUserId = SessionHelper.UserID;
            try
            {
                // <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing
                int IsCreditPullNothing = 0; // false means its PULL
                IsCreditPullNothing = 2;
                string ItemLocationMSG = "";

                #region "Check Project Spend Condition"
                bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                #endregion

                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                RoomDTO objRoomDTO = objRoomDAL.GetRoomByNamePlain(SessionHelper.CompanyID, SessionHelper.RoomName);
                bool AllowNegetive = false;
                if (objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true
                   && objDTO.ActionType.ToLower().Equals("pull"))
                {
                    AllowNegetive = true;
                }

                obj.UpdatePullData(objDTO, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, out ItemLocationMSG, IsProjecSpendAllowed, out IsPSLimitExceed, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name, AllowNegetive);

                if (ItemLocationMSG != "")
                    locationMSG = ItemLocationMSG;

                message = ResMessage.SaveMessage;
                status = "ok";

                #region "Code For Update Details to Requisition Detials Table After Successful PULL"
                RequisitionDetailsDTO objReqDetDTO = new RequisitionDetailsDTO();
                RequisitionDetailsDAL objReqDetDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                objReqDetDTO = objReqDetDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(RequisitionDetailGUID));
                if (objReqDetDTO != null)
                {
                    objReqDetDTO.QuantityPulled = PullQuantity;
                    objReqDetDTO.ProjectSpendGUID = Guid.Parse(ProjectGUID);
                    objReqDetDTO.BinID = BinID;
                    objReqDetDAL.Edit(objReqDetDTO, SessionUserId);
                }
                #endregion
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            return Json(new { Message = message, Status = status, LocationMSG = locationMSG, PSLimitExceed = IsPSLimitExceed }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CloseRequisitionIfPullCompletedByDetailId(string RequisitionDetailGUID)
        {
            RequisitionDetailsDAL objReqDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDTO objReqDTO = objReqDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(RequisitionDetailGUID));
            if (objReqDTO != null && objReqDTO.RequisitionGUID != null && objReqDTO.RequisitionGUID != Guid.Empty)
            {
                return CloseRequisitionIfPullCompleted(objReqDTO.RequisitionGUID.ToString());
            }
            else
            {
                return Json(new { Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CloseRequisitionIfPullCompleted(string RequisitionGUID)
        {
            RequisitionMasterDAL objReqDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objReqDTO = objReqDAL.GetRequisitionByGUIDFull(Guid.Parse(RequisitionGUID));

            if (objReqDTO != null && objReqDTO.ID > 0)
            {
                RequisitionDetailsDAL objReqDtlDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<RequisitionDetailsDTO> objReqDtlDTO = objReqDtlDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.RequisitionGUID == objReqDTO.GUID).ToList();
                RequisitionMasterDTO objReqDTO1 = objReqDtlDAL.GetRequisitionQty(SessionHelper.RoomID, SessionHelper.CompanyID, objReqDTO.GUID);
                if (objReqDTO1 != null)
                {
                    objReqDTO.RequisitionedQuantity = objReqDTO1.RequisitionedQuantity;
                    objReqDTO.ApprovedQuantity = objReqDTO1.ApprovedQuantity;
                    objReqDTO.PulledQuantity = objReqDTO1.PulledQuantity;

                }
                if (objReqDTO.ApprovedQuantity == objReqDTO.PulledQuantity)
                {
                    if (objReqDTO != null)
                    {
                        objReqDTO.RequisitionStatus = "Closed";
                        return SaveRequisition(objReqDTO);
                    }
                }
                //AddReqItemsToWOItems(Guid.Parse(RequisitionGUID));

                //if (objReqDtlDTO != null && objReqDtlDTO.Count() > 0)
                //{
                //    double ApprovedQTY = objReqDtlDTO.Select(x => x.QuantityApproved.GetValueOrDefault(0)).Sum();
                //    double PulledQTY = objReqDtlDTO.Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();
                //    if (ApprovedQTY == PulledQTY)
                //    {
                //        if (objReqDTO != null)
                //        {
                //            objReqDTO.RequisitionStatus = "Closed";
                //            return SaveRequisition(objReqDTO);
                //        }
                //    }
                //}
                return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        private static void AddExternalUser(List<UserMasterDTO> objUsers, string EmailTemplate)
        {

            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(EmailTemplate, true, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (lstExternalUser != null)
            {
                foreach (EmailUserConfigurationDTO item in lstExternalUser)
                {
                    UserMasterDTO objExtUser = new UserMasterDTO();
                    objExtUser.Email = item.Email;
                    objUsers.Add(objExtUser);
                }
            }
        }
        public void SendMailToApprover(RequisitionMasterDTO objRequisition, string AprvRej)
        {
            List<eMailAttachmentDTO> objeMailAttchList = new List<eMailAttachmentDTO>();
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            StringBuilder MessageBody = null;
            //eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            ////EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //eTurnsUtility objUtils = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            try
            {
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.RequisitionApproval, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
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
                        string StrSubject = ResRequisitionMaster.Requisition + " " + AprvRej;// "Order Approval Request";
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            //string FromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();

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

                            MessageBody.Replace("@@ORDERNO@@", objRequisition.RequisitionNumber);
                            MessageBody.Replace("@@REQUISITIONNO@@", objRequisition.RequisitionNumber);
                            MessageBody.Replace("@@TABLE@@", GetMailBody(objRequisition));
                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                            //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

                            string replacePart = string.Empty;
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (Request == null)
                            {
                                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }



                            //objUtils = new eTurnsUtility();
                            //objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                            //objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
                            //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, null, "Web => Requisition => RequisitionToApprove");
                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objRequisition.GUID.ToString());
                            objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                            if (!string.IsNullOrWhiteSpace(strToAddress))
                            {
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (EmailAddrs != null && EmailAddrs.Count > 0)
                                {
                                    foreach (var emailitem in EmailAddrs)
                                    {
                                        string strdata = objRequisition.ID + "^" + objRequisition.Room + "^" + objRequisition.CompanyID + "^" + (objRequisition.LastUpdatedBy ?? objRequisition.CreatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objRequisition.LastUpdatedBy.GetValueOrDefault(0) + "^" + emailitem;
                                        string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                        string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");

                                        List<eMailAttachmentDTO> objeMailAttchListNew = new List<eMailAttachmentDTO>();
                                        
                                        if (objeMailAttchList != null && objeMailAttchList.Any() && objeMailAttchList.Count() > 0)
                                        {
                                            foreach (var item in objeMailAttchList)
                                            {
                                                objeMailAttchListNew.Add(item);
                                            }
                                        }                                        

                                        MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/RequisitionStatus?eKey=" + approvalURLData + "'>'"+ResCommon.Approve + "'</a> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<a href='" + replacePart + "/EmailLink/RequisitionStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                        objAlertMail.CreateAlertMail(objeMailAttchListNew, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);
                                    }
                                }
                            }


                        }
                    });
                }

            }
            finally
            {
                MessageBody = null;
                //objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //objUtils = null;
                //objEmailDAL = null;
            }
        }

        public void SendMailForOrderAprOrRej(RequisitionMasterDTO objRequisition, string AprvRej, string ReqRequesterEmailAddress,string ReqApproverEmailAddress)
        {
            List<eMailAttachmentDTO> objeMailAttchList = new List<eMailAttachmentDTO>();
            Helper.AlertMail objAlertMail = new Helper.AlertMail();
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            StringBuilder MessageBody = null;
            //eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            ////EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            //eTurnsUtility objUtils = null;
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterprise(SessionHelper.EnterPriceID);
            try
            {
                lstNotifications = objNotificationDAL.GetAllSchedulesByEmailTemplate((long)MailTemplate.RequisitionApproveReject, SessionHelper.RoomID, SessionHelper.CompanyID, ResourceHelper.CurrentCult.Name);
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
                        string StrSubject = ResRequisitionMaster.Requisition + " " + AprvRej;// "Order Approval Request";
                        string strToAddress = t.EmailAddress;
                        if (!string.IsNullOrEmpty(strToAddress))
                        {
                            if (strToAddress.Contains("[Requester]"))
                            {
                                if (!string.IsNullOrWhiteSpace(ReqRequesterEmailAddress))
                                    strToAddress = strToAddress.Replace("[Requester]", ReqRequesterEmailAddress);
                                else
                                    strToAddress = strToAddress.Replace("[Requester]", "");
                            }
                            if (strToAddress.Contains("[Approver]"))
                            {
                                if (!string.IsNullOrWhiteSpace(ReqApproverEmailAddress))
                                    strToAddress = strToAddress.Replace("[Approver]", ReqApproverEmailAddress);
                                else
                                    strToAddress = strToAddress.Replace("[Approver]", "");
                            }

                            //string FromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                            //string strCCAddress = "";
                            MessageBody = new StringBuilder();

                            objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            if (t.EmailTemplateDetail.lstEmailTemplateDtls != null && t.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
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

                            MessageBody.Replace("@@ORDERNO@@", objRequisition.RequisitionNumber);
                            MessageBody.Replace("@@REQUISITIONNO@@", objRequisition.RequisitionNumber);
                            MessageBody.Replace("@@TABLE@@", GetMailBody(objRequisition));
                            MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                            MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                            MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                            MessageBody.Replace("@@APRVREJ@@", AprvRej);
                            //MessageBody.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));

                            string replacePart = string.Empty;
                            if (objEnterpriseDTO != null && (!string.IsNullOrWhiteSpace(objEnterpriseDTO.EnterPriseDomainURL)))
                            {
                                replacePart = objEnterpriseDTO.EnterPriseDomainURL;
                            }
                            else if (Request == null)
                            {
                                replacePart = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                            }
                            else
                            {
                                string urlPart = Request.Url.ToString();
                                replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
                            }

                            //objUtils = new eTurnsUtility();
                            //objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                            //objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
                            //objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, null, "Web => Requisition => RequisitionToApprove");
                            Dictionary<string, string> Params = new Dictionary<string, string>();
                            Params.Add("DataGuids", objRequisition.GUID.ToString());
                            objeMailAttchList = objAlertMail.GenerateBytesBasedOnAttachmentForAlert(t, objEnterpriseDTO, Params);

                            if (!string.IsNullOrWhiteSpace(strToAddress))
                            {
                                List<string> EmailAddrs = strToAddress.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                EmailAddrs = EmailAddrs.Distinct().ToList();
                                if (EmailAddrs != null && EmailAddrs.Count > 0)
                                {
                                    foreach (var emailitem in EmailAddrs)
                                    {
                                        string strdata = objRequisition.ID + "^" + objRequisition.Room + "^" + objRequisition.CompanyID + "^" + (objRequisition.LastUpdatedBy ?? objRequisition.CreatedBy) + "^" + SessionHelper.EnterPriceID.ToString() + "^" + objRequisition.LastUpdatedBy.GetValueOrDefault(0) + "^" + emailitem;
                                        string approvalURLData = StringCipher.Encrypt(strdata + "^APRV");
                                        string rejectURLData = StringCipher.Encrypt(strdata + "^RJKT");
                                        List<eMailAttachmentDTO> objeMailAttchListNew = new List<eMailAttachmentDTO>();
                                        
                                        if (objeMailAttchList != null && objeMailAttchList.Any() && objeMailAttchList.Count() > 0)
                                        {
                                            foreach (var item in objeMailAttchList)
                                            {
                                                objeMailAttchListNew.Add(item);
                                            }
                                        }                                        

                                        MessageBody.Replace("@@APPROVEREJECT@@", @"<a href='" + replacePart + "/EmailLink/RequisitionStatus?eKey=" + approvalURLData + "'>'" + ResCommon.Approve + "'</a> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<a href='" + replacePart + "/EmailLink/RequisitionStatus?eKey=" + rejectURLData + "'>'" + ResCommon.Reject + "'</a>");
                                        objAlertMail.CreateAlertMail(objeMailAttchListNew, StrSubject, MessageBody.ToString(), emailitem, t, objEnterpriseDTO);
                                    }
                                }
                            }


                        }
                    });
                }

            }
            finally
            {
                MessageBody = null;
                //objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                //objUtils = null;
                //objEmailDAL = null;
            }
        }

        private string GetMailBody(RequisitionMasterDTO obj)
        {
            int PriseSelectionOption = 1;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomModuleSettingsDTO objRoomModuleSettingsDTO = new RoomModuleSettingsDTO();
            objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)ModuleInfo.Requisitions);

            if (objRoomModuleSettingsDTO != null && objRoomModuleSettingsDTO.ID > 0)
            {
                PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption.GetValueOrDefault(0);
            }

            string CostTitle = ResRequisitionMaster.TotalCost;
            CostTitle = (PriseSelectionOption == 1 ? ResRequisitionMaster.TotalSellPrice : ResRequisitionMaster.TotalCost);

            string mailBody = "";

            mailBody = @"<table style=""margin-left: 0px; width: 99%; border: 0px solid;"">
                <tr>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label style=""font-weight: bold;"">
                                        " + ResRequisitionMaster.RequisitionNumber + @": </label>
                                    <label  style=""font-weight: bold;"">
                                        " + obj.RequisitionNumber + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResRequisitionMaster.Description + @": </label>
                                    <label>
                                        " + obj.Description + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResRequisitionMaster.WorkorderName + @": </label>
                                    <label>
                                        " + obj.WorkorderName + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style=""width: 48%"">
                        <table style=""margin-left: 0px; width: 99%;"">
                            <tr>
                                <td>
                                    <label>
                                       " + ResRequisitionMaster.RequiredDate + @": </label>
                                    <label>
                                        " + (obj.RequiredDate == null ? "&nbsp;" : obj.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture)) + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + CostTitle + @": </label>
                                    <label>
                                        " +
                                          (Convert.ToString(PriseSelectionOption) == "1" ? obj.TotalSellPrice.ToString("#.##") : obj.TotalCost.GetValueOrDefault(0).ToString("#.##"))
                                          + @"</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <label>
                                        " + ResRequisitionMaster.RequisitionStatus + @": </label>
                                    <label>
                                        " + obj.RequisitionStatus + @"</label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan=""2"" style=""width: 99%"">
                        <table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.ItemNumber + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResOrder.Bin + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResRequisitionDetails.QuantityRequisitioned + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResItemMaster.OnHandQuantity + @"
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        " + ResRequisitionDetails.RequiredDate + @"
                                    </th>
                                    
                                </tr>
                            </thead>
                            <tbody>
            ##TRS##
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
            ";
            string trs = "";
            if (obj.RequisitionListItem == null || obj.RequisitionListItem.Count <= 0)
            {
                RequisitionDetailsDAL objDetailDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.RequisitionListItem = objDetailDAL.GetReqLinesByReqGUIDFull(obj.GUID, 0, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0)).ToList();
            }

            if (obj.RequisitionListItem != null && obj.RequisitionListItem.Count > 0)
            {

                foreach (var item in obj.RequisitionListItem)
                {
                    string binname = "&nbsp;";
                    string ReqQty = "&nbsp;";
                    string ItemOHQty = "&nbsp;";
                    string ReqDate = "&nbsp;";
                    if (item.BinID != null && item.BinID > 0)
                    {
                        //binname = new BinMasterController().GetRecord(Int64.Parse(Convert.ToString(item.Bin)), SessionHelper.RoomID, SessionHelper.CompanyID, false, false).BinNumber;
                        BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(Int64.Parse(Convert.ToString(item.BinID)), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, Int64.Parse(Convert.ToString(item.BinID)),null,null).FirstOrDefault();
                        if (objBinMasterDTO != null)
                        {
                            binname = objBinMasterDTO.BinNumber;
                        }

                    }
                    if (item.QuantityRequisitioned != null)
                        //ReqQty = item.QuantityApproved.ToString();
                        ReqQty = item.QuantityRequisitioned.ToString();

                    if (item.RequiredDate != null)
                        ReqDate = item.RequiredDate.GetValueOrDefault(DateTime.MinValue).ToString(SessionHelper.DateTimeFormat, SessionHelper.RoomCulture);

                    ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemByGuidPlain(item.ItemGUID ?? Guid.Empty, obj.Room ?? 0, obj.CompanyID ?? 0);
                    if (objItem != null)
                    {
                        ItemOHQty = (objItem.OnHandQuantity ?? 0).ToString();
                    }
                    trs += @"<tr>
                        <td>
                            " + (!string.IsNullOrEmpty(item.ItemNumber) ? item.ItemNumber : "&nbsp;") + @"
                        </td>
                        <td>
                            " + binname + @"
                        </td>
                        <td>
                            " + ReqQty + @"
                        </td>
                        <td>
                            " + ItemOHQty + @"
                        </td>
                        <td>
                            " + ReqDate + @"
                        </td>
                        
                    </tr>";
                }
            }
            else
            {
                trs += @"<tr>
                        <td colspan=""4"" style=""text-align:center"">
                           There is no item for this order
                        </td>
                    </tr>";
            }
            mailBody = mailBody.Replace("##TRS##", trs);

            return mailBody;
        }


        private StringBuilder EmailTemplateReplaceURL(string EmailTemplateName)
        {
            StringBuilder MessageBody = new StringBuilder();
            if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName))
            {
                MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName));
            }

            string urlPart = Request.Url.ToString();
            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

            //MessageBody = MessageBody.Replace("../CommonImages/logo.gif", replacePart + "CommonImages/logo.gif");
            if (Request.ApplicationPath != "/")
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart + Request.ApplicationPath);
            else
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart);
            return MessageBody;
        }

        public ActionResult DownloadRequisitionDocument(List<Guid> lstReqGuids)
        {
            List<string> retData = new List<string>();
            foreach (var reqguid in lstReqGuids)
            {
                RequisitionImageDetailDAL requisitionImageDetailDAL = new RequisitionImageDetailDAL(SessionHelper.EnterPriseDBName);
                RequisitionMasterDAL requisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                var lstrequisitionimagedetail = requisitionImageDetailDAL.GetRequisitionImagesByGuidPlain(reqguid);
                string requisitionfilepath = SiteSettingHelper.RequisitionFilePaths;  //Settinfile.Element("WorkOrderFilePaths").Value;
                requisitionfilepath = requisitionfilepath.Replace("~", string.Empty);
                Guid reqGUID = Guid.Empty;
                RequisitionMasterDTO requisitionMasterDTO = new RequisitionMasterDTO();
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.AbsolutePath, "");
                baseURL = SessionHelper.CurrentDomainURL;
                string RequisitionPath = baseURL + requisitionfilepath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + requisitionMasterDTO.ID;
                if (Guid.TryParse(reqguid.ToString(), out reqGUID))
                {
                    requisitionMasterDTO = requisitionMasterDAL.GetRequisitionByGUIDPlain(reqGUID);
                    RequisitionPath = baseURL + requisitionfilepath + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/" + requisitionMasterDTO.ID;
                }

                foreach (RequisitionImageDetailDTO img in lstrequisitionimagedetail)
                {
                    retData.Add(RequisitionPath + "/" + img.ImageName);
                }
            }

            //return File(data, "application/csv", dtoModuleDetail.ResourceModuleName + "_" + dtoModuleDetail.PageName + "_MobileRes_" + DateTimeUtility.DateTimeNow.ToString("yyyyMMddHHmmss") + ".csv");
            return Json(new { Message = "Sucess", Status = true, ReturnFiles = retData }, JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveRequisition(RequisitionMasterDTO objDTO)
        {
            if (!ModelState.IsValid)
                return Json(new { Message = ResMessage.InvalidModel, Status = "fail" }, JsonRequestBehavior.AllowGet);

            string message = "";
            string status = "";

            CommonDAL objCDAL = objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            WorkOrderDAL objWODAL = null;
            CommonDAL objC = null;
            WorkOrderDTO objWODTO = new WorkOrderDTO();
            // RoomDTO roomDTO = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,IsAllowRequisitionDuplicate,AttachingWOWithRequisition";
            RoomDTO roomDTO = objCDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            long SessionUserId = SessionHelper.UserID;
            string reportFilePath = "";
            string reportFileURLPath = "";
            string strOK1 = "ok";
            if (!roomDTO.IsAllowRequisitionDuplicate.GetValueOrDefault(true))
                strOK1 = obj.RequisitionNumberDuplicateCheck(objDTO.ID, objDTO.RequisitionNumber, SessionHelper.CompanyID, SessionHelper.RoomID);

            if (strOK1 == "duplicate")
            {
                message = string.Format(ResMessage.DuplicateMessage, ResRequisitionMaster.RequisitionNumber, objDTO.RequisitionNumber);
                status = "fail";
                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    int Totalrecs = 0;
                    List<RequisitionDetailsDTO> lstRequisitionLineItems = objRequisitionDetailsDAL.GetPagedRequisitionDetails(0, 5000, out Totalrecs, string.Empty, "ID DESC", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, true, objDTO.GUID);

                    if (objDTO.RequisitionStatus.ToUpper() == "SUBMITTED"
                        || objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                    {
                        foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionLineItems)
                        {
                            if (objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            {
                                ItemMasterDTO oItemRecord = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objRequisitionDetails.ItemGUID);

                                if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0)
                                {
                                    if (objDTO.RequisitionStatus.ToUpper() == "SUBMITTED")
                                    {
                                        if (objRequisitionDetails.QuantityRequisitioned < oItemRecord.DefaultPullQuantity || (decimal)objRequisitionDetails.QuantityRequisitioned % (decimal)oItemRecord.DefaultPullQuantity != 0)
                                        {
                                            message = string.Format(ResRequisitionMaster.RequisitionQtyEqualsDefaultPullQty, oItemRecord.DefaultPullQuantity, objRequisitionDetails.ItemNumber);
                                            status = "fail";
                                            return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                                        }

                                    }
                                    else if (objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                                    {
                                        if (objRequisitionDetails.QuantityApproved < oItemRecord.DefaultPullQuantity || (decimal)objRequisitionDetails.QuantityApproved % (decimal)oItemRecord.DefaultPullQuantity != 0)
                                        {
                                            message = string.Format(ResRequisitionMaster.ApprovedQtyEqualsDefaultPullQty, oItemRecord.DefaultPullQuantity, objRequisitionDetails.ItemNumber);
                                            status = "fail"; 
                                            return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                    {                        
                        foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionLineItems)
                        {
                            if (objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty && objRequisitionDetails.ToolGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty
                                     && objRequisitionDetails.ToolCategoryID.GetValueOrDefault(0) > 0)
                            {
                                message = ResRequisitionMaster.SelectToolCategory; //"Select tool aginst each tool category";
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (lstRequisitionLineItems != null && lstRequisitionLineItems.Count > 0)
                        {
                            bool IsDuplicateToolExist = obj.CheckDuplicateToolRequisition(objDTO.GUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (IsDuplicateToolExist)
                            {
                                message = ResRequisitionMaster.DuplicateToolRequisition; //"duplicate tool not allow to add.";
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        if (objDTO.SupplierId != null && objDTO.SupplierId.GetValueOrDefault(0) > 0)
                        {
                            UserAccessDTO objUserAccess = null;
                            if (SessionHelper.UserType == 1)
                            {
                                eTurnsMaster.DAL.UserMasterDAL objUserdal = new eTurnsMaster.DAL.UserMasterDAL();
                                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            else
                            {
                                eTurns.DAL.UserMasterDAL objUserdal = new UserMasterDAL(SessionHelper.EnterPriseDBName);
                                objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }


                            if (objUserAccess != null && !string.IsNullOrWhiteSpace(objUserAccess.SupplierIDs))
                            {
                                List<string> strSupplier = objUserAccess.SupplierIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (strSupplier != null && strSupplier.Count > 0)
                                {
                                    if (strSupplier.Contains(objDTO.SupplierId.ToString()) == false)
                                    {
                                        //message = "requisition not allow to approve for this supplier";
                                        message = ResRequisitionMaster.SupplierApprove;
                                        status = "fail";
                                        return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                        }


                    }

                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.Room = SessionHelper.RoomID;
                    if (!string.IsNullOrWhiteSpace(objDTO.RequiredDateStr))
                    {
                        objDTO.RequiredDate = DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    }

                    #region "Add WorkOrder"

                    objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                    obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                    objC = new CommonDAL(SessionHelper.EnterPriseDBName);
                    bool isWoInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                    if ((!string.IsNullOrEmpty(objDTO.WorkorderName) && objDTO.WorkorderGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty))
                    {
                        //  #region "Duplicate for WO"




                        //string strOK = objC.DuplicateCheck(objDTO.WorkorderName, "add", 0, "WorkOrder", "WOName", SessionHelper.RoomID, SessionHelper.CompanyID);
                        //if (strOK == "duplicate")

                        //objWODTO = objWODAL.GetRecord(Convert.ToInt32(SessionHelper.RoomID), objDTO.WorkorderName, Convert.ToInt32(SessionHelper.CompanyID));
                        objWODTO = objWODAL.GetWorkOrdersByNamePlainSingle(objDTO.WorkorderName, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objWODTO != null && (objWODTO.WOStatus ?? string.Empty).ToLower() == WorkOrderStatus.Open.ToString().ToLower())
                        {

                            //objWODTO = objWODAL.GetRecord(Convert.ToInt32(SessionHelper.RoomID), objDTO.WorkorderName, Convert.ToInt32(SessionHelper.CompanyID));
                            //if (objWODTO != null)
                            //{
                            //    objDTO.WorkorderGUID = objWODTO.GUID;
                            //}

                            int? _AttachingWOWithRequisition = (int)AttachingWOWithRequisition.New;

                            _AttachingWOWithRequisition = roomDTO.AttachingWOWithRequisition.GetValueOrDefault((int)AttachingWOWithRequisition.New);

                            if (_AttachingWOWithRequisition == (int)AttachingWOWithRequisition.New)
                            {
                                message = ResRequisitionMaster.CannotAssignExistingWOToReq;
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                            }
                            else if ((_AttachingWOWithRequisition == (int)AttachingWOWithRequisition.Mixed || _AttachingWOWithRequisition == (int)AttachingWOWithRequisition.Existing) && objWODTO.GUID != Guid.Empty )
                            { 
                               objDTO.WorkorderGUID = objWODTO.GUID;    
                            }
                        }
                        else if (isWoInsert)
                        {
                            string _ReleaseNumber = objWODAL.GenerateAndGetReleaseNumber(objDTO.WorkorderName, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            objWODTO = new WorkOrderDTO()
                            {
                                Created = DateTimeUtility.DateTimeNow,
                                Updated = DateTimeUtility.DateTimeNow,
                                CreatedBy = SessionHelper.UserID,
                                CreatedByName = SessionHelper.UserName,
                                LastUpdatedBy = SessionHelper.UserID,
                                Room = SessionHelper.RoomID,
                                CompanyID = SessionHelper.CompanyID,
                                RoomName = SessionHelper.RoomName,
                                UpdatedByName = SessionHelper.UserName,
                                IsArchived = false,
                                IsDeleted = false,
                                WOName = objDTO.WorkorderName,
                                WOStatus = "Open",
                                WOType = "Reqn",
                                WhatWhereAction = "Requisition",
                                ReleaseNumber = _ReleaseNumber,
                                //TechnicianID = objDTO.TechnicianID,
                                //Technician = objDTO.Technician
                            };

                            objDTO.WorkorderGUID = objWODAL.Insert(objWODTO);
                        }
                        else
                        { 
                            message = ResMessage.NoRightsToInsertWorkorder;
                            status = "fail";
                            return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (objDTO != null && objDTO.WorkorderGUID.GetValueOrDefault(Guid.Empty) != null && objDTO.WorkorderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            objWODTO = objWODAL.GetWorkOrderByGUIDPlain((objDTO.WorkorderGUID ?? Guid.Empty));
                            if (objWODTO != null)
                                objDTO.WorkorderName = objWODTO.WOName;
                        }
                    }
                    #endregion

                    #region "Add Project Spend"                    
                    
                    if (!string.IsNullOrWhiteSpace(objDTO.ProjectSpendName))
                    {
                        bool isAllowProjectSpendInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                        if (isAllowProjectSpendInsert)
                        {
                            objDTO.ProjectSpendGUID = objCDAL.GetOrInsertProject(objDTO.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Web").GUID;
                        }
                        else
                        {
                            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                            var projectMaster = objProjectMasterDAL.GetProjectByName(objDTO.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);
                            if (projectMaster == null || projectMaster.GUID == null && projectMaster.GUID == Guid.Empty)
                            {
                                message = ResMessage.NoRightsToInsertProjectSpend;
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                            }
                            else {
                                objDTO.ProjectSpendGUID = projectMaster.GUID;
                            }
                        }
                    }
                    #endregion

                    //var releaseNumber = obj.GetRequisitionReleaseNumber(objDTO.ID,objDTO.RequisitionNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
                    //objDTO.ReleaseNumber = Convert.ToString(releaseNumber);

                    if (objDTO.ID == 0)
                    {
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.WhatWhereAction = "Requisition";

                        objDTO.StagingID = objCDAL.GetOrInsertMaterialStagingIDByName(objDTO.StagingName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        objDTO.MaterialStagingGUID = objCDAL.GetOrInsertMaterialStagingGUIDByName(objDTO.StagingName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (objDTO.RequisitionStatus.ToUpper() == "SUBMITTED")
                        {
                            objDTO.RequesterID = SessionHelper.UserID;
                        }
                        else if (objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                        {
                            objDTO.ApproverID = SessionHelper.UserID;
                        }

                        if ((objDTO.TechnicianID == null || objDTO.TechnicianID < 0) && (!string.IsNullOrEmpty(objDTO.Technician)))
                        {
                            try
                            {
                                String[] spearator = { " --- " };
                                String[] strlist = objDTO.Technician.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                TechnicianMasterDTO technician = new TechnicianMasterDTO();
                                if (strlist.Length > 1)
                                    technician = objCDAL.GetOrInsertTechnician(strlist[0], strlist[1], SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                else
                                    technician = objCDAL.GetOrInsertTechnician(strlist[0], null , SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                                objDTO.TechnicianID = technician.ID;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Guid ReturnVal = obj.Insert(objDTO).GUID;

                        if (ReturnVal != Guid.Empty)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            WorkOrderDTO WO = new WorkOrderDTO();
                            customer = objcust.GetCustomerByGUID(objDTO.CustomerGUID ?? Guid.Empty);

                            /*////CODE FOR UPDATE WO WHEN TECHNICIAN SELECT////*/
                            if (objDTO.TechnicianID != objWODTO.TechnicianID)
                            {
                                objWODTO.Updated = DateTimeUtility.DateTimeNow;
                                objWODTO.LastUpdatedBy = SessionHelper.UserID;
                                objWODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objWODTO.UpdatedByName = SessionHelper.UserName;
                                objWODTO.WOName = objDTO.WorkorderName;
                                objWODTO.WOType = "Reqn";
                                objWODTO.WhatWhereAction = "Requisition";
                                if (objDTO.TechnicianID.HasValue && objDTO.TechnicianID.Value > 0)
                                {
                                    objWODTO.TechnicianID = objDTO.TechnicianID;
                                    objWODTO.Technician = objDTO.Technician;
                                }
                                else
                                {
                                    objWODTO.TechnicianID = null;
                                    objWODTO.Technician = null;
                                }
                                objWODAL.Edit(objWODTO);
                            }
                            /*////CODE FOR UPDATE WO WHEN TECHNICIAN SELECT////*/

                            if (objWODTO != null && customer != null)
                            {
                                objWODTO.CustomerID = customer.ID;
                                objWODTO.Customer = customer.Customer;
                                objWODTO.CustomerGUID = objDTO.CustomerGUID;
                                objWODAL.Edit(objWODTO);
                            }

                            if (objWODTO != null && objDTO.CustomerGUID == null)
                            {
                                objWODTO.CustomerID = null;
                                objWODTO.Customer = null;
                                objWODTO.CustomerGUID = null;
                                objWODAL.Edit(objWODTO);
                            }
                        }
                        else
                        {
                            var recordDeleted = ResCommon.RecordDeletedSuccessfully;
                            var recordsUsedInOtherModuele = ResCommon.MsgRecordsUsedInOtherModule;
                            objWODAL.DeleteRecords(objDTO.WorkorderGUID.ToString(), SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionUserId, recordDeleted, recordsUsedInOtherModuele,SessionHelper.EnterPriceID);
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }

                    }
                    else
                    {

                        objDTO.WhatWhereAction = "Requisition";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        objDTO.RequisitionApprover = SessionHelper.UserName;
                        var requisition = obj.GetRequisitionByGUIDFull(objDTO.GUID);
                        var tmpStagingName = (string.IsNullOrEmpty(objDTO.StagingName) || string.IsNullOrWhiteSpace(objDTO.StagingName)) ? string.Empty : objDTO.StagingName;
                        var tmpRequisitionStagingName = (string.IsNullOrEmpty(requisition.StagingName) || string.IsNullOrWhiteSpace(requisition.StagingName)) ? string.Empty : requisition.StagingName;

                        if (tmpRequisitionStagingName != tmpStagingName)
                        {
                            if (objDTO.RequisitionStatus.ToUpper() == "UNSUBMITTED" || objDTO.RequisitionStatus.ToUpper() == "SUBMITTED")
                            {
                                var lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);

                                if (lstRequisitionDetails != null && lstRequisitionDetails.Any())
                                {
                                    message = ResMessage.StagingCantBeChanged;
                                    status = "fail";
                                    return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    objDTO.StagingID = objCDAL.GetOrInsertMaterialStagingIDByName(objDTO.StagingName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    objDTO.MaterialStagingGUID = objCDAL.GetOrInsertMaterialStagingGUIDByName(objDTO.StagingName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                            else
                            {
                                message = ResMessage.StagingCantBeChanged;
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        if (requisition != null && requisition.RequesterID.GetValueOrDefault(0) > 0)
                        {
                            objDTO.RequesterID = requisition.RequesterID;
                        }
                        if (requisition != null && requisition.ApproverID.GetValueOrDefault(0) > 0)
                        {
                            objDTO.ApproverID = requisition.ApproverID;
                        }
                        if (requisition.RequisitionStatus.ToUpper() == "UNSUBMITTED"
                            && objDTO.RequisitionStatus.ToUpper() == "SUBMITTED")
                        {
                            objDTO.RequesterID = SessionHelper.UserID;                            
                        }
                        if (requisition.RequisitionStatus.ToUpper() == "SUBMITTED"
                            && objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                        {
                            objDTO.ApproverID = SessionHelper.UserID;
                        }
                        if (requisition.RequisitionStatus.ToUpper() == "UNSUBMITTED"
                            && objDTO.RequisitionStatus.ToUpper() == "APPROVED")
                        {
                            objDTO.RequesterID = SessionHelper.UserID;
                            objDTO.ApproverID = SessionHelper.UserID;
                        }

                        if ((objDTO.TechnicianID == null || objDTO.TechnicianID < 0) &&  (!string.IsNullOrEmpty(objDTO.Technician)))
                        {
                            try
                            {
                                String[] spearator = { " --- " };
                                String[] strlist = objDTO.Technician.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                TechnicianMasterDTO technician = new TechnicianMasterDTO();
                                if (strlist.Length > 1)
                                    technician = objCDAL.GetOrInsertTechnician(strlist[0], strlist[1], SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                else
                                    technician = objCDAL.GetOrInsertTechnician(strlist[0], null, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                                objDTO.TechnicianID = technician.ID;
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        string requisitionDataLog = "Method - SaveRequisition; Controller-ConsumeController : on " + DateTime.UtcNow.ToString();                        
                        requisitionDataLog = requisitionDataLog + "; " + "RequisitionStatus-"+ objDTO.RequisitionStatus+ ",Username :" + (SessionHelper.UserName != null ? SessionHelper.UserName : "");
                        objDTO.RequisitionDataLog = requisitionDataLog;

                        bool ReturnVal = obj.Edit(objDTO);

                        if (objDTO.RequisitionStatus == "Approved")
                        {
                            IEnumerable<RequisitionDetailsDTO> lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (lstRequisitionDetails != null && lstRequisitionDetails.Count() > 0)
                            {
                                foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionDetails)
                                {
                                    objRequisitionDetailsDAL.UpdateItemOnRequisitionQty(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, objRequisitionDetails.LastUpdatedBy);
                                    //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Req.DetailDAL >> Edit");
                                    new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Consume >> Save Requisition", SessionUserId);

                                    ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                    ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty));
                                    if (ItemDTO != null && ItemDTO.ID > 0)
                                    {
                                        /* WI-5009 update QtyToMeetDemand */
                                        if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                                        {
                                            new KitDetailDAL(SessionHelper.EnterPriseDBName).UpdateQtyToMeedDemand(ItemDTO.GUID, SessionHelper.UserID, SessionUserId);
                                        }
                                        /* WI-5009 update QtyToMeetDemand */
                                    }

                                }
                            }

                            reportFilePath = CreateRequisitonPDFFileToPrint(objDTO.GUID.ToString());
                            string oldPath = Server.MapPath("/Downloads/");
                            string newpth = Convert.ToString(reportFilePath).Replace(oldPath, "");
                            reportFileURLPath = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.RawUrl.ToString(), "/") + "Downloads/" + newpth;
                        }
                        else if (objDTO.RequisitionStatus == "Closed")
                        {
                            obj.CloseRequisition(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                            //---------------------------------------------------------------------------------
                            //
                            IEnumerable<RequisitionDetailsDTO> lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (lstRequisitionDetails != null && lstRequisitionDetails.Count() > 0)
                            {
                                foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionDetails)
                                {
                                    if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                                    {
                                        objRequisitionDetailsDAL.UpdateRequisitionedQuantity("Delete", objRequisitionDetails, objRequisitionDetails);
                                    }
                                    //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Req.DetailDAL >> Save Requisition Close");
                                    new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Consume >> Close Requisition", SessionUserId);
                                    if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                                    {
                                        ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                        ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty));
                                        if (ItemDTO != null && ItemDTO.ID > 0)
                                        {
                                            /* WI-5009 update QtyToMeetDemand */
                                            if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                                            {
                                                new KitDetailDAL(SessionHelper.EnterPriseDBName).UpdateQtyToMeedDemand(ItemDTO.GUID, SessionHelper.UserID, SessionUserId);
                                            }
                                            /* WI-5009 update QtyToMeetDemand */
                                        }
                                    }
                                }
                            }
                        }
                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                            CustomerMasterDAL objcust = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
                            CustomerMasterDTO customer = new CustomerMasterDTO();
                            customer = objcust.GetCustomerByGUID(objDTO.CustomerGUID ?? Guid.Empty);

                            WorkOrderDAL objwo = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                            WorkOrderDTO WO = new WorkOrderDTO();
                            WO = objwo.GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID ?? Guid.Empty);
                            bool isWOSave = false;

                            /*////CODE FOR UPDATE WO WHEN TECHNICIAN SELECT////*/
                            if (WO != null && objDTO.TechnicianID != WO.TechnicianID)
                            {
                                WO.Updated = DateTimeUtility.DateTimeNow;
                                WO.LastUpdatedBy = SessionHelper.UserID;
                                WO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                WO.UpdatedByName = SessionHelper.UserName;
                                WO.WOName = objDTO.WorkorderName;
                                WO.WOType = "Reqn";
                                WO.WhatWhereAction = "Requisition";
                                //if (objDTO.TechnicianID.HasValue && objDTO.TechnicianID.Value > 0)
                                //{
                                //    WO.TechnicianID = objDTO.TechnicianID;
                                //    WO.Technician = objDTO.Technician;
                                //}
                                //else
                                //{
                                //    WO.TechnicianID = null;
                                //    WO.Technician = null;
                                //}
                                //objWODAL.Edit(WO);
                                isWOSave = true;
                            }
                            /*////CODE FOR UPDATE WO WHEN TECHNICIAN SELECT////*/

                            if (WO != null && customer != null)
                            {
                                WO.CustomerID = customer.ID;
                                WO.Customer = customer.Customer;
                                WO.CustomerGUID = objDTO.CustomerGUID;
                                // objwo.Edit(WO);
                                isWOSave = true;
                            }
                            if (WO != null && objDTO.CustomerGUID == null)
                            {
                                WO.CustomerID = null;
                                WO.Customer = null;
                                WO.CustomerGUID = null;
                                // objWODAL.Edit(WO);
                                isWOSave = true;
                            }
                            if (isWOSave)
                            {
                                objWODAL.Edit(WO);
                            }

                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }

                    if (objDTO.RequisitionStatus == "Submitted")
                    {
                        SendMailToApprover(objDTO, "APPROVED");
                    }
                    if (objDTO.RequisitionStatus == "Approved")
                    {
                        eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                        UserMasterDTO ReqUser = new UserMasterDTO();

                        string ReqRequesterEmailAddress = "";
                        string ReqApproverEmailAddress = "";
                        if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                        {
                            ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                            if (ReqUser == null)
                            {
                                ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.RequesterID.GetValueOrDefault(0));
                            }
                            if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                            {
                                ReqRequesterEmailAddress = ReqUser.Email;
                            }
                        }
                        if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                        {
                            ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                            if (ReqUser == null)
                            {
                                ReqUser = userMasterDAL.GetUserByIdPlain(objDTO.ApproverID.GetValueOrDefault(0));
                            }
                            if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                            {
                                ReqApproverEmailAddress = ReqUser.Email;
                            }
                        }
                        SendMailForOrderAprOrRej(objDTO, "APPROVED", ReqRequesterEmailAddress, ReqApproverEmailAddress);
                    }

                    if (Session != null && Session["REQEvent"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Session["REQEvent"])))
                    {
                        try
                        {
                            if (status == "ok")
                            {
                                string requisitionGUIDs = "<DataGuids>" + Convert.ToString(objDTO.GUID) + "</DataGuids>";
                                string eventName = Convert.ToString(Session["REQEvent"]);
                                string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                                NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                                List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                if (lstNotification != null && lstNotification.Count > 0)
                                {
                                    objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, requisitionGUIDs);
                                    Session["REQEvent"] = "";
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                        }
                    }


                    if (Session != null)
                    {
                        Session["IsInsert"] = "True";
                    }

                    return Json(new { Message = message, Status = status, ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO, ReportPDFFilePath = reportFilePath, ReportFileHTTPURL = reportFileURLPath }, JsonRequestBehavior.AllowGet);

                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    return Json(new { Message = ResMessage.SaveMessageWithReportFileError, Status = "ok", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message.ToString(), Status = "fail", ID = objDTO.ID, GUID = objDTO.GUID, reqobj = objDTO }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    obj = null; ;
                    objWODAL = null;
                    objC = null;
                    objWODTO = null;
                }
            }

        }

        [HttpGet]
        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            Session["REQEvent"] = "";
            return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public bool IsRecordEditable(RequisitionMasterDTO objDTO)
        {
            bool isEditable = true;
            string IsInsertSession = (Session == null || Session["IsInsert"] == null ? "" : Session["IsInsert"].ToString());

            if (objDTO.IsArchived.GetValueOrDefault(false) || objDTO.IsDeleted.GetValueOrDefault(false))
            {
                isEditable = false;
                return isEditable;
            }

            if (!(IsInsert || IsUpdate || IsDelete || IsApprove))
            {
                isEditable = false;
                return isEditable;
            }

            if (objDTO.ID <= 0 && !IsInsert)
            {
                isEditable = false;
            }
            else if (IsUpdate || IsApprove || IsInsertSession == "True" || IsInsert)
            {
                if (objDTO.RequisitionStatus == "Unsubmitted")
                {
                    if (Convert.ToString(IsInsertSession) == "")
                    {
                        if (IsUpdate) // Update only 
                        {
                            isEditable = true;
                        }
                        else
                        {
                            if (objDTO.ID <= 0 && IsInsert) // Insert only  first time
                                isEditable = true;
                            else if (objDTO.ID > 0 && IsInsert)// Edit mode with View only 
                                isEditable = false;
                            else if (objDTO.ID > 0 && !IsInsert)
                            {
                                isEditable = false;
                                if (IsApprove)
                                    objDTO.IsOnlyStatusUpdate = true;
                            }
                        }
                    }
                    else
                    {


                        isEditable = true;
                    }
                }
                else if (objDTO.RequisitionStatus == "Submitted")
                {
                    if (IsUpdate && IsApprove)
                        isEditable = true;
                    else if (!IsUpdate && IsApprove)
                    {
                        isEditable = false;
                        objDTO.IsOnlyStatusUpdate = true;
                    }
                    else if (!IsUpdate && !IsApprove)
                        isEditable = false;
                }
                else if (objDTO.RequisitionStatus == "Approved")
                {
                    isEditable = true;
                    //if (IsApprove)
                    //    objDTO.IsOnlyStatusUpdate = true;
                }
                else if (objDTO.RequisitionStatus == "Closed")
                {
                    isEditable = false;
                }
            }

            return isEditable;
        }

        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            RequisitionMasterDTO objDTO = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName).GetRequisitionByGUIDPlain(Guid.Parse(ParentGuid));

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Consume/AddItemToDetailTable/",
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = eTurns.DTO.ResRequisitionMaster.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Consume/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Consume/GetItemsModelMethod/",
                CallingFromPageName = "RQ",
                ReqRequiredDate = SessionHelper.RoomDateFormat != null ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat) : objDTO.RequiredDate.Value.ToString("MM/dd/yyyy"),
                StagingBinId = objDTO.StagingID.ToString(),
            };

            return PartialView("ItemMasterModel", obj);
        }

        public ActionResult LoadToolsOnModel(string ParentGuid)
        {
            RequisitionMasterDTO objDTO = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName).GetRequisitionByGUIDPlain(Guid.Parse(ParentGuid));

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Consume/AddToolToDetailTable/",
                PerentID = objDTO.ID.ToString(),
                PerentGUID = ParentGuid,
                ModelHeader = "Tools",
                AjaxURLAddMultipleItemToSession = "~/Consume/AddToolToDetailTable/",
                //AjaxURLToFillItemGrid = "~/Consume/GetItemsModelMethod/",
                CallingFromPageName = "RQ",
                ReqRequiredDate = SessionHelper.RoomDateFormat != null ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat) : objDTO.RequiredDate.Value.ToString("MM/dd/yyyy"),
            };

            return PartialView("ToolsModel", obj);
        }

        public ActionResult LoadToolsCategoryOnModel(string ParentGuid)
        {
            RequisitionMasterDTO objDTO = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName).GetRequisitionByGUIDPlain(Guid.Parse(ParentGuid));
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.ToolCategory = objCommon.GetDDData("ToolCategoryMaster", "ToolCategory", SessionHelper.CompanyID, SessionHelper.RoomID);

            ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = ResCategoryMaster.SelectCategory }); 
            ViewBag.ToolCategoryList = lstToolCategory;

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Consume/AddToolCategoryToDetailTable/",
                PerentID = objDTO.ID.ToString(),
                PerentGUID = ParentGuid,
                ModelHeader = "Tools",
                AjaxURLAddMultipleItemToSession = "~/Consume/AddToolCategoryToDetailTable/",
                CallingFromPageName = "RQ",
                ReqRequiredDate = SessionHelper.RoomDateFormat != null ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat) : objDTO.RequiredDate.Value.ToString("MM/dd/yyyy"),
            };

            return PartialView("ToolsCategoryModel", obj);
        }

        public string CreateRequisitonPDFFileToPrint(string ReqGUID)
        {
            ReportMasterDAL objReportMasterDAL = null;
            ReportBuilderDTO objRPTDTO = null;
            ReportBuilderController objRPTCTRL = null;
            MasterController objMSTCTRL = null;
            KeyValDTO objKeyVal = null;
            List<KeyValDTO> objKeyValList = null;
            JavaScriptSerializer objJSSerial = null;
            JsonResult objJSON = null;
            try
            {
                objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);

                //objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.ModuleName == "Consume_Requisition" && x.ReportType == 2 && x.SetAsDefaultPrintReport.GetValueOrDefault(false) == true);
                objRPTDTO = objReportMasterDAL.GetDefaultReportByModuleAndCompany(SessionHelper.CompanyID, "Consume_Requisition");

                if (objRPTDTO == null)
                {
                    //objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "Consume_Requisition" && x.ReportType == 2 && x.ReportName == "Requisition");
                    objRPTDTO = objReportMasterDAL.GetReportByReportNameAndCompany("Requisition", true);
                }

                if (objRPTDTO != null)
                {
                    objKeyValList = new List<KeyValDTO>();

                    objKeyVal = new KeyValDTO() { key = "DataGuids", value = ReqGUID };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "CompanyIDs", value = SessionHelper.CompanyID.ToString() };
                    objKeyValList.Add(objKeyVal);
                    objKeyVal = new KeyValDTO() { key = "RoomIDs", value = SessionHelper.RoomID.ToString() };
                    objKeyValList.Add(objKeyVal);

                    if ((objRPTDTO.HideHeader ?? false))
                    {
                        objKeyVal = new KeyValDTO() { key = "HideHeader", value = Convert.ToString("1") };
                        objKeyValList.Add(objKeyVal);
                    }
                    if ((objRPTDTO.ShowSignature ?? false))
                    {
                        objKeyVal = new KeyValDTO() { key = "ShowSignature", value = Convert.ToString("1") };
                        objKeyValList.Add(objKeyVal);
                    }

                    objMSTCTRL = new MasterController();
                    objMSTCTRL.SetPDFReportParaDictionary(objKeyValList, objRPTDTO.ID.ToString(), null);

                    objRPTCTRL = new ReportBuilderController();
                    objJSON = objRPTCTRL.GenerateBytesFromReport(objRPTDTO.ID, "PDF");
                    objJSON.MaxJsonLength = int.MaxValue;

                    objJSSerial = new JavaScriptSerializer();
                    var json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objJSON.Data));

                    if (Convert.ToString(json["Message"]) == "ok")
                    {
                        return Convert.ToString(json["FilePath"]);
                    }
                }
                return string.Empty;
            }
            finally
            {
                objReportMasterDAL = null;
                objRPTDTO = null;
                objRPTCTRL = null;
                objMSTCTRL = null;
                objKeyVal = null;
                objKeyValList = null;
                objJSSerial = null;
                objJSON = null;
            }

        }

        /// <summary>
        /// PrintRequisitionOnSubmitt
        /// </summary>
        /// <param name="PdfFilePath"></param>
        /// <returns></returns>
        public ActionResult PrintRequisitionOnSubmit(string PdfFilePath)
        //public ActionResult PrintRequisitionOnSubmit(FormCollection frm)
        {
            iTextSharp.text.pdf.PdfReader reader = null;
            iTextSharp.text.pdf.PdfStamper stamper = null;
            FileStream fss = null;
            byte[] bytes = null;
            string mimeType = "application/pdf";
            try
            {
                // string PdfFilePath = frm["hdnPath"];
                if (!string.IsNullOrEmpty(PdfFilePath) && System.IO.File.Exists(PdfFilePath))
                {
                    string newFilePath = PdfFilePath.Substring(0, PdfFilePath.LastIndexOf("."));
                    newFilePath += Session.SessionID + "_" + Guid.NewGuid().ToString() + ".pdf";
                    reader = new iTextSharp.text.pdf.PdfReader(PdfFilePath);
                    using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
                    {
                        stamper = new iTextSharp.text.pdf.PdfStamper(reader, fs);
                        stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                        stamper.Close();
                    }
                    reader.Close();
                    fss = new FileStream(newFilePath, FileMode.Open);
                    bytes = new byte[fss.Length];
                    fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
                    fss.Close();
                    System.IO.File.Delete(newFilePath);
                }
                string fName = "Req_" + DateTime.UtcNow.ToString("yyyyMMddhhmmss") + ".pdf";
                return File(bytes, mimeType, fName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }

                reader = null;

                if (stamper != null)
                {
                    stamper.Dispose();
                }
                stamper = null;

                if (fss != null)
                {
                    fss.Dispose();
                }
                fss = null;
                bytes = null;
            }
        }

        /// <summary>
        /// Not Used
        /// </summary>
        /// <param name="ReqGUID"></param>
        /// <returns></returns>
        public ActionResult PrintSubmittedRequisition(string ReqGUID)
        //public FileStreamResult PrintSubmittedRequisition(string ReqGUID)
        {
            //ReportMasterDAL objReportMasterDAL = null;
            //ReportBuilderDTO objRPTDTO = null;
            //ReportBuilderController objRPTCTRL = null;
            //MasterController objMSTCTRL = null;
            //KeyValDTO objKeyVal = null;
            ////JavaScriptSerializer objJSSerial = null;
            //JsonResult objJSON = null;
            //iTextSharp.text.pdf.PdfReader reader = null;
            //iTextSharp.text.pdf.PdfStamper stamper = null;
            FileStream fss = null;
            //byte[] bytes = null;

            string mimeType = "application/pdf";
            string reportFilePath = ReqGUID;// CreateRequisitonPDFFileToPrint(ReqGUID);
            fss = new FileStream(reportFilePath, FileMode.Open);
            // System.IO.File.Delete(reportFilePath);
            string fName = "Req_" + DateTime.UtcNow.ToString("yyyyMMddhhmmss") + ".pdf";
            return File(fss, mimeType);

            //try
            //{
            //    objReportMasterDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            //    objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "Consume_Requisition" && x.ReportType == 2 && x.SetAsDefaultPrintReport.GetValueOrDefault(false) == true);

            //    if (objRPTDTO == null)
            //        objRPTDTO = objReportMasterDAL.GetReportList().FirstOrDefault(x => x.IsBaseReport && x.ModuleName == "Consume_Requisition" && x.ReportType == 2 && x.ReportName == "Requisition");

            //    if (objRPTDTO != null)
            //    {
            //        objKeyValList = new List<KeyValDTO>();

            //        objKeyVal = new KeyValDTO() { key = "DataGuids", value = ReqGUID };
            //        objKeyValList.Add(objKeyVal);
            //        objKeyVal = new KeyValDTO() { key = "CompanyIDs", value = SessionHelper.CompanyID.ToString() };
            //        objKeyValList.Add(objKeyVal);
            //        objKeyVal = new KeyValDTO() { key = "RoomIDs", value = SessionHelper.RoomID.ToString() };
            //        objKeyValList.Add(objKeyVal);

            //        objMSTCTRL = new MasterController();
            //        objMSTCTRL.SetPDFReportParaDictionary(objKeyValList, objRPTDTO.ID.ToString(), null);

            //        objRPTCTRL = new ReportBuilderController();
            //        objJSON = objRPTCTRL.GenerateBytesFromReport(objRPTDTO.ID, "PDF");
            //        objJSON.MaxJsonLength = int.MaxValue;

            //        objJSSerial = new JavaScriptSerializer();
            //        var json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objJSON.Data));

            //        if (Convert.ToString(json["Message"]) == "ok")
            //        {
            //            //fileBytes = System.IO.File.ReadAllBytes(Convert.ToString(json["FilePath"]));
            //            mimeType = Convert.ToString(json["FileMimeType"]);

            //            reader = new iTextSharp.text.pdf.PdfReader(Convert.ToString(json["FilePath"]));
            //            string newFilePath = Server.MapPath("~/Downloads/Req_" + ReqGUID + "_" + Session.SessionID + "_" + Guid.NewGuid().ToString() + ".pdf");
            //            using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
            //            {
            //                stamper = new iTextSharp.text.pdf.PdfStamper(reader, fs);
            //                stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
            //                stamper.Close();
            //            }

            //            reader.Close();
            //            fss = new FileStream(newFilePath, FileMode.Open);
            //            bytes = new byte[fss.Length];
            //            //fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            //            //fss.Close();
            //            System.IO.File.Delete(newFilePath);
            //        }
            //    }

            //    //return File(bytes, mimeType);
            //    return File(fss, mimeType, fName);
            //}
            //finally
            //{
            //    objReportMasterDAL = null;
            //    objRPTDTO = null;
            //    objRPTCTRL = null;
            //    objMSTCTRL = null;
            //    objKeyVal = null;
            //    objKeyValList = null;
            //    objJSSerial = null;
            //    bytes = null;
            //    objJSON = null;
            //}

        }


        #endregion

        #region "Requisition Details"
        public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            long stagingBinId = 0;
            long.TryParse(Request["StagingBinId"].ToString(), out stagingBinId);

            //make changes to resolve an issue of Sort (WI-431)
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber asc";

            if (sortColumnName.ToLower().Replace("asc", "").Trim().Length <= 0)
                sortColumnName = "ItemNumber asc";

            if (sortColumnName.ToLower().Replace("desc", "").Trim().Length <= 0)
                sortColumnName = "ItemNumber asc";

            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.ToLower().Contains("itemudf"))
                sortColumnName = sortColumnName.ToLower().Replace("item", "");

            if ((sortColumnName.Equals("null desc") || sortColumnName.Equals("null asc")))
                sortColumnName = string.Empty;

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            List<RequisitionDetailsDTO> objQLItems = null;
            string ItemsIDs = "";

            if (objQLItems != null && objQLItems.Count > 0)
            {
                foreach (var item in objQLItems)
                {
                    if (!string.IsNullOrEmpty(ItemsIDs))
                        ItemsIDs += ",";

                    ItemsIDs += item.ItemGUID.ToString();
                }
            }

            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<ItemMasterDTO> DataFromDB = obj.GetPagedItemsForModel(param.iDisplayStart,
                param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName,
                SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, true, true, true, SessionHelper.UserID, "requisition", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, ItemsIDs, null);

            if (DataFromDB != null && DataFromDB.Any() && stagingBinId > 0)
            {
                var bin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetDefaultStagingBinForRequisition(SessionHelper.RoomID, SessionHelper.CompanyID);
                if (bin != null && bin.ID > 0)
                {
                    DataFromDB = DataFromDB.Select(c => { c.DefaultLocation = bin.ID; c.DefaultLocationName = bin.BinNumber; return c; }).ToList();
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

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetToolModelAjax(JQueryDataTableParamModel param)
        {
            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);

            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            string ToolType = string.Empty;
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            Guid RequisitionGuid = Guid.Parse(Request["RequisitionGuid"]);

            if (sortColumnName.Trim() == "ToolUDF1")
                sortColumnName = "UDF1";
            else if (sortColumnName.Trim() == "ToolUDF2")
                sortColumnName = "UDF2";
            else if (sortColumnName.Trim() == "ToolUDF3")
                sortColumnName = "UDF3";
            else if (sortColumnName.Trim() == "ToolUDF4")
                sortColumnName = "UDF4";
            else if (sortColumnName.Trim() == "ToolUDF5")
                sortColumnName = "UDF5";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
            {
                sortColumnName = "ID desc";

            }
            string searchQuery = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["ToolType"]) && Request["ToolType"] != null)
            {
                ToolType = Convert.ToString(Request["ToolType"]);
            }

            RequisitionDetailsDAL rqDetail = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            List<RequisitionDetailsDTO> lstReqDetail = rqDetail.GetReqToolLinesByReqGUIDPlain(RequisitionGuid, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            IEnumerable<string> lstGuids = lstReqDetail.Select(x => x.ToolGUID.ToString());
            string toolGuids = string.Join(",", lstGuids);

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);

            var DataFromDB = obj.GetPagedTools(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone, toolGuids, Type: ToolType);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);


        }



        public JsonResult AddQLToDetailTable(Guid? ReqGUID, Guid? QuickListGUID, double? QLQTY, string PullUDF1, string PullUDF2, string PullUDF3, string PullUDF4, string PullUDF5)
        {

            string message = "";
            string status = "";
            long SessionUserId = SessionHelper.UserID;

            QuickListDAL quickListDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            var supplierIds = new List<long>();
            List<QuickListDetailDTO> objQLItems = quickListDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(QuickListGUID ?? Guid.Empty), false, false, supplierIds);

            foreach (var QLitem in objQLItems)
            {
                if (QLitem.PullQtyScanOverride && QLitem.DefaultPullQuantity > 0)
                {
                    double QuantityRequisitioned = (QLitem.Quantity.GetValueOrDefault(0) * QLQTY.GetValueOrDefault(0));

                    if (QuantityRequisitioned < QLitem.DefaultPullQuantity || (decimal)QuantityRequisitioned % (decimal)QLitem.DefaultPullQuantity != 0)
                    {
                        message = string.Format(ResRequisitionMaster.RequisitionQtyEqualsDefaultPullQty, QLitem.DefaultPullQuantity, QLitem.ItemNumber);
                        status = "fail"; 
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            objRequisitionDetailsDAL.AddQLToReq(ReqGUID ?? Guid.Empty, QuickListGUID ?? Guid.Empty, QLQTY ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionHelper.UserID, PullUDF1, PullUDF2, PullUDF3, PullUDF4, PullUDF5, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, SessionUserId);
            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            string ItemLevelErrMsg = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            RequisitionDetailsDTO[] QLDetails = s.Deserialize<RequisitionDetailsDTO[]>(para);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objReqDTO = null;
            RequisitionDetailsDAL objApi = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            ToolsMaintenanceDAL objToosMtnDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ToolsMaintenanceDTO objToolsMtnDTO = null;
            long SessionUserId = SessionHelper.UserID;

            string ReturnMessage = "";
            string ReturnStatus = "Ok";
            var msgInvalidTechnician = ResToolMaster.MsgInvalidTechnicianWithName;
            foreach (RequisitionDetailsDTO item in QLDetails)
            {
                ReturnStatus = "Ok";
                if (objReqDTO == null)
                    objReqDTO = objRequisitionMasterDAL.GetRequisitionByGUIDPlain((Guid)item.RequisitionGUID);

                if (item.RequiredDate != null)
                {
                    item.RequiredDate = DateTime.ParseExact(item.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                }
                else if (!string.IsNullOrWhiteSpace(item.RequiredDateStr))
                {
                    item.RequiredDate = DateTime.ParseExact(item.RequiredDateStr.ToString(SessionHelper.RoomCulture), SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                }
                //if ((item.GUID == null || item.GUID == Guid.Empty) && item.ItemGUID != null && item.BinID != null && item.RequisitionGUID != null)
                //{
                //    RequisitionDetailsDTO objItem = objApi.GetRequisitionDetailsItemGUIDBinIDSinglePlain((Guid)item.ItemGUID, item.BinID ?? 0, (Guid)item.RequisitionGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    if (objItem != null && objItem.GUID != null && objItem.GUID != Guid.Empty)
                //    {
                //        item.GUID = objItem.GUID;
                //    }
                //}

                item.RequisitionStatus = objReqDTO.RequisitionStatus;
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CompanyID = SessionHelper.CompanyID;
                item.SupplierID = ((SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0);
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;

                if (item.RequiredDate == null)
                    item.RequiredDate = objReqDTO.RequiredDate;

                if (!string.IsNullOrEmpty(item.Description) && item.Description == "null")
                    item.Description = string.Empty;

                if (item.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    item.ItemCost = objItemMasterDAL.GetItemCostByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, (Guid)item.ItemGUID, false);
                    item.ItemSellPrice = objItemMasterDAL.GetItemSellPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, (Guid)item.ItemGUID, false) ?? 0;

                    if ((item.GUID == null || item.GUID == Guid.Empty))
                    {
                        var isStagingBin = (objReqDTO != null && objReqDTO.MaterialStagingGUID.HasValue && objReqDTO.MaterialStagingGUID != Guid.Empty);
                        if (isStagingBin && (string.IsNullOrEmpty(item.BinName) || string.IsNullOrWhiteSpace(item.BinName)))
                        {
                            item.BinName = "[|EmptyStagingBin|]";
                        }
                        BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objbinDTO = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID.Value, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, isStagingBin, materialStagingGUID: objReqDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty));

                        if (objbinDTO != null)
                        {
                            item.BinID = objbinDTO.ID;
                            item.BinName = objbinDTO.BinNumber;
                        }
                    }


                    int TotalRecordCount = 0;
                    var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, "PullMaster", SessionHelper.RoomID);

                    if (item.QuickListItemGUID != null && item.QuickListItemGUID != Guid.Empty)
                    {
                        Dictionary<string, string> dictReturn = objApi.GetRequisitionUDFs(item.GUID);

                        //UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                        //List<string> lstMissingUDFs = objUDFDAL.GetMissingUDFSetup(SessionHelper.RoomID, SessionHelper.CompanyID, "PullMaster");
                        List<string> lstMissingUDFs = null;
                        if (DataFromDBUDF != null)
                            lstMissingUDFs = (new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" }).Where(x => !DataFromDBUDF.Where(z => !String.IsNullOrEmpty(z.UDFControlType) && (z.IsDeleted == false)).Select(y => y.UDFColumnName).Contains(x)).ToList();
                        else
                            lstMissingUDFs = new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" };

                        if (dictReturn != null)
                        {
                            if ((string.IsNullOrEmpty(item.PullUDF1) || string.IsNullOrWhiteSpace(item.PullUDF1)) && (!string.IsNullOrEmpty(dictReturn["UDF1"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF1"])) && lstMissingUDFs.Contains("UDF1"))
                                item.PullUDF1 = dictReturn["UDF1"];

                            if ((string.IsNullOrEmpty(item.PullUDF2) || string.IsNullOrWhiteSpace(item.PullUDF2)) && (!string.IsNullOrEmpty(dictReturn["UDF2"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF2"])) && lstMissingUDFs.Contains("UDF2"))
                                item.PullUDF2 = dictReturn["UDF2"];

                            if ((string.IsNullOrEmpty(item.PullUDF3) || string.IsNullOrWhiteSpace(item.PullUDF3)) && (!string.IsNullOrEmpty(dictReturn["UDF3"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF3"])) && lstMissingUDFs.Contains("UDF3"))
                                item.PullUDF3 = dictReturn["UDF3"];

                            if ((string.IsNullOrEmpty(item.PullUDF4) || string.IsNullOrWhiteSpace(item.PullUDF4)) && (!string.IsNullOrEmpty(dictReturn["UDF4"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF4"])) && lstMissingUDFs.Contains("UDF4"))
                                item.PullUDF4 = dictReturn["UDF4"];

                            if ((string.IsNullOrEmpty(item.PullUDF5) || string.IsNullOrWhiteSpace(item.PullUDF5)) && (!string.IsNullOrEmpty(dictReturn["UDF5"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF5"])) && lstMissingUDFs.Contains("UDF5"))
                                item.PullUDF5 = dictReturn["UDF5"];
                        }
                    }

                    bool UDF1Encrypt = false;
                    bool UDF2Encrypt = false;
                    bool UDF3Encrypt = false;
                    bool UDF4Encrypt = false;
                    bool UDF5Encrypt = false;

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

                    item.PullUDF1 = (UDF1Encrypt == true && item.PullUDF1 != null && (!string.IsNullOrWhiteSpace(item.PullUDF1))) ? objCommon.GetEncryptValue(item.PullUDF1) : item.PullUDF1;
                    item.PullUDF2 = (UDF2Encrypt == true && item.PullUDF2 != null && (!string.IsNullOrWhiteSpace(item.PullUDF2))) ? objCommon.GetEncryptValue(item.PullUDF2) : item.PullUDF2;
                    item.PullUDF3 = (UDF3Encrypt == true && item.PullUDF3 != null && (!string.IsNullOrWhiteSpace(item.PullUDF3))) ? objCommon.GetEncryptValue(item.PullUDF3) : item.PullUDF3;
                    item.PullUDF4 = (UDF4Encrypt == true && item.PullUDF4 != null && (!string.IsNullOrWhiteSpace(item.PullUDF4))) ? objCommon.GetEncryptValue(item.PullUDF4) : item.PullUDF4;
                    item.PullUDF5 = (UDF5Encrypt == true && item.PullUDF5 != null && (!string.IsNullOrWhiteSpace(item.PullUDF5))) ? objCommon.GetEncryptValue(item.PullUDF5) : item.PullUDF5;
                    item.TechnicianGUID = null;
                    item.ToolCheckoutUDF1 = null;
                    item.ToolCheckoutUDF2 = null;
                    item.ToolCheckoutUDF3 = null;
                    item.ToolCheckoutUDF4 = null;
                    item.ToolCheckoutUDF5 = null;
                    item.ToolGUID = null;

                    if (!string.IsNullOrEmpty(item.Technician) && !string.IsNullOrWhiteSpace(item.Technician))
                    {
                        string TechnicianCode = "";
                        string TechnicianName = item.Technician.Trim();
                        
                        if (TechnicianName.Contains(" --- "))
                        {
                            TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                            TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        else
                        {
                            TechnicianCode = TechnicianName;
                            TechnicianName = "";
                        }

                        if (TechnicianCode.Trim() == "")
                        {
                            ItemLevelErrMsg += (" " + string.Format(msgInvalidTechnician, item.Technician));
                            continue;
                        }

                        if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                            || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                        {
                            ItemLevelErrMsg += " " + ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician + " : " + item.Technician;
                            continue;
                        }

                        TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                        TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (objTechnicianMasterDTO == null)
                        {
                            bool HasOnTheFlyEntryRight = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.OnTheFlyEntry);
                            bool HasInsertTechnicianRight = SessionHelper.GetModulePermission(SessionHelper.ModuleList.TechnicianMaster, SessionHelper.PermissionType.Insert);
                            
                            if (!HasOnTheFlyEntryRight || !HasInsertTechnicianRight)
                            {
                                ItemLevelErrMsg += " " + ResMessage.NoRightsToInsertTechnician + " : " + item.Technician;
                                continue;
                            }

                            objTechnicianMasterDTO = new TechnicianMasterDTO();
                            objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                            objTechnicianMasterDTO.Technician = TechnicianName;
                            objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                            objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.GUID = Guid.NewGuid();
                            objTechnicianMasterDTO.IsArchived = false;
                            objTechnicianMasterDTO.IsDeleted = false;
                            Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                            objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                        else
                        {
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                    }

                    ItemMasterDTO oItemRecord = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                    if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0)
                    {
                        if (item.QuantityRequisitioned < oItemRecord.DefaultPullQuantity || (decimal)item.QuantityRequisitioned.GetValueOrDefault(0) % (decimal)oItemRecord.DefaultPullQuantity != 0)
                        {
                            if (!string.IsNullOrWhiteSpace(ReturnMessage))
                            {
                                ReturnMessage = ReturnMessage + "<br/>" + string.Format(ResRequisitionMaster.RequisitionQtyEqualsDefaultPullQty, oItemRecord.DefaultPullQuantity, item.ItemNumber);
                            }
                            else
                            {
                                ReturnMessage = string.Format(ResRequisitionMaster.RequisitionQtyEqualsDefaultPullQty, oItemRecord.DefaultPullQuantity, item.ItemNumber);
                            }                            
                            ReturnStatus = "fail";
                            continue;
                            //return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #region "Add Project Spend"


                    CommonDAL objCDAL = objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    if (!string.IsNullOrWhiteSpace(item.ProjectSpendName))
                    {
                        bool isAllowProjectSpendInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                        if (isAllowProjectSpendInsert)
                        {
                            item.ProjectSpendGUID = objCDAL.GetOrInsertProject(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Web").GUID;
                        }
                        else
                        {

                            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                            var projectMaster = objProjectMasterDAL.GetProjectByName(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);
                            if (projectMaster == null || projectMaster.GUID == null && projectMaster.GUID == Guid.Empty)
                            {
                                message = ResMessage.NoRightsToInsertProjectSpend;
                                status = "fail";
                                return Json(new { Message = message.ToString(), Status = "fail"}, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                item.ProjectSpendGUID = projectMaster.GUID;
                            }
                        }
                    }
                    #endregion
                    if (item.GUID != Guid.Empty)
                    {
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        objApi.Edit(item, SessionUserId, false);
                    }
                    else
                    {
                        item.CreatedBy = SessionHelper.UserID;
                        item.CreatedByName = SessionHelper.UserName;
                        //List<RequisitionDetailsDTO> tempDTO = objApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.RequisitionGUID == item.RequisitionGUID && x.ItemGUID == item.ItemGUID).ToList();
                        //if (tempDTO == null || tempDTO.Where(t => t.IsDeleted == false).Count() == 0)
                        objApi.Insert(item, SessionUserId);

                        #region "Tool Maintenance Line Item"
                        if (objToolsMtnDTO == null)
                            objToolsMtnDTO = objToosMtnDAL.GetCachedData(item.RequisitionGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        if (item.RequisitionGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty && objToolsMtnDTO != null && objToolsMtnDTO.ID > 0)
                        {
                            ToolMaintenanceDetailsDAL objToolDtlDAL = new ToolMaintenanceDetailsDAL(SessionHelper.EnterPriseDBName);
                            ToolMaintenanceDetailsDTO itemMtn = new ToolMaintenanceDetailsDTO();
                            itemMtn.Room = SessionHelper.RoomID;
                            itemMtn.RoomName = SessionHelper.RoomName;
                            itemMtn.CreatedBy = SessionHelper.UserID;
                            itemMtn.CreatedByName = SessionHelper.UserName;
                            itemMtn.UpdatedByName = SessionHelper.UserName;
                            itemMtn.LastUpdatedBy = SessionHelper.UserID;
                            itemMtn.CompanyID = SessionHelper.CompanyID;
                            itemMtn.ItemGUID = item.ItemGUID;
                            itemMtn.ItemCost = item.ItemCost;
                            itemMtn.Quantity = item.QuantityRequisitioned;
                            itemMtn.MaintenanceGUID = objToolsMtnDTO.GUID;
                            objToolDtlDAL.Insert(itemMtn);
                        }
                        #endregion
                    }
                }
                else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    ToolMasterDTO objDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                    string ToolName = "";
                    
                    if (objDTO != null && objDTO.ID > 0)
                    {
                        item.ItemCost = objDTO.Cost;
                        ToolName = item.ToolName;
                    }
                    else
                    {
                        item.ItemCost = 0;
                    }

                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                    item.PullUDF1 = null;
                    item.PullUDF2 = null;
                    item.PullUDF3 = null;
                    item.PullUDF4 = null;
                    item.PullUDF5 = null;
                    item.ProjectSpendGUID = null;
                    item.BinID = null;

                    if (!string.IsNullOrEmpty(item.Technician) && !string.IsNullOrWhiteSpace(item.Technician))
                    {
                        string TechnicianCode = "";
                        string TechnicianName = item.Technician.Trim();
                        if (TechnicianName.Contains(" --- "))
                        {
                            TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                            TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        else
                        {
                            TechnicianCode = TechnicianName;
                            TechnicianName = "";
                        }

                        if (TechnicianCode.Trim() == "")
                        {
                            ItemLevelErrMsg += (" " + string.Format(msgInvalidTechnician, item.Technician));
                            continue;
                        }

                        if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                            || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                        {
                            ItemLevelErrMsg += " "+ ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician + " : " + item.Technician;
                            continue;
                        }

                        TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                        TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);
                        
                        if (objTechnicianMasterDTO == null)
                        {
                            bool HasOnTheFlyEntryRight = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.OnTheFlyEntry);
                            bool HasInsertTechnicianRight = SessionHelper.GetModulePermission(SessionHelper.ModuleList.TechnicianMaster, SessionHelper.PermissionType.Insert);

                            if (!HasOnTheFlyEntryRight || !HasInsertTechnicianRight)
                            {
                                ItemLevelErrMsg += " "+ ResMessage.NoRightsToInsertTechnician + " : " + item.Technician;
                                continue;
                            }

                            objTechnicianMasterDTO = new TechnicianMasterDTO();
                            objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                            objTechnicianMasterDTO.Technician = TechnicianName;
                            objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                            objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.GUID = Guid.NewGuid();
                            objTechnicianMasterDTO.IsArchived = false;
                            objTechnicianMasterDTO.IsDeleted = false;
                            Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                            objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                        else
                        {
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                    }

                    if (item.GUID != Guid.Empty)
                    {
                        objApi.Edit(item, SessionUserId);
                    }
                    else
                    {
                        item.Created = DateTimeUtility.DateTimeNow;
                        item.CreatedBy = SessionHelper.UserID;
                        item.CreatedByName = SessionHelper.UserName;
                        item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        item.AddedFrom = "Web";
                        objApi.Insert(item, SessionUserId);
                    }
                }
                else if (item.ToolCategoryID.GetValueOrDefault(0) > 0)
                {

                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.LastUpdated = DateTimeUtility.DateTimeNow;
                    item.PullUDF1 = null;
                    item.PullUDF2 = null;
                    item.PullUDF3 = null;
                    item.PullUDF4 = null;
                    item.PullUDF5 = null;
                    item.ProjectSpendGUID = null;
                    item.BinID = null;
                    if (item.GUID != Guid.Empty)
                    {
                        objApi.Edit(item, SessionUserId);
                    }
                }

            }

            //AddReqItemsToWOItems(QLDetails[0].RequisitionGUID.Value);
            if (objReqDTO != null)
            {
                objApi.UpdateRequisitionTotalCost(objReqDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            if (!string.IsNullOrEmpty(ItemLevelErrMsg) && !string.IsNullOrWhiteSpace(ItemLevelErrMsg))
            {
                message = ResRequisitionMaster.FailToUpdateQtyOfItems +  " " + ItemLevelErrMsg; 
                status = "fail";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ReturnMessage))
                {
                    message = ReturnMessage;
                    status = "fail";
                }
                else
                {
                    message = ResAssetMaster.MsgItemQtyUpdated;
                    status = "ok";
                }
            }
            
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToolToDetailTable(string para)
        {
            string message = "";
            string status = "";
            string ItemLevelErrMsg = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            RequisitionDetailsDTO[] reqDetails = s.Deserialize<RequisitionDetailsDTO[]>(para);

            RequisitionDetailsDAL objApi = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objReqDTO = objRequisitionMasterDAL.GetRequisitionByGUIDPlain((Guid)reqDetails[0].RequisitionGUID);
            long SessionUserId = SessionHelper.UserID;
            var msgInvalidTechnician = ResToolMaster.MsgInvalidTechnicianWithName;
            foreach (RequisitionDetailsDTO item in reqDetails)
            {
                if (!string.IsNullOrWhiteSpace(item.RequiredDateStr))
                    item.RequiredDate = DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                else
                    item.RequiredDate = objReqDTO.RequiredDate;

                if (!string.IsNullOrEmpty(item.Technician) && !string.IsNullOrWhiteSpace(item.Technician))
                {
                    string TechnicianCode = "";
                    string TechnicianName = item.Technician.Trim();
                    
                    if (TechnicianName.Contains(" --- "))
                    {
                        TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                        TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    else
                    {
                        TechnicianCode = TechnicianName;
                        TechnicianName = "";
                    }

                    if (TechnicianCode.Trim() == "")
                    {
                        ItemLevelErrMsg += (" " + string.Format(msgInvalidTechnician, item.Technician));
                        continue;
                    }

                    if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                        || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                    {
                        ItemLevelErrMsg += " " + ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician + ": " + item.Technician;
                        continue;
                    }

                    TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                    TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (objTechnicianMasterDTO == null)
                    {
                        bool HasOnTheFlyEntryRight = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.OnTheFlyEntry);
                        bool HasInsertTechnicianRight = SessionHelper.GetModulePermission(SessionHelper.ModuleList.TechnicianMaster, SessionHelper.PermissionType.Insert);

                        if (!HasOnTheFlyEntryRight || !HasInsertTechnicianRight)
                        {
                            ItemLevelErrMsg += " " + ResMessage.NoRightsToInsertTechnician + " : " + item.Technician;
                            continue;
                        }

                        objTechnicianMasterDTO = new TechnicianMasterDTO();
                        objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                        objTechnicianMasterDTO.Technician = TechnicianName;
                        objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                        objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                        objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                        objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                        objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                        objTechnicianMasterDTO.GUID = Guid.NewGuid();
                        objTechnicianMasterDTO.IsArchived = false;
                        objTechnicianMasterDTO.IsDeleted = false;
                        Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                        objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                    }
                    else
                    {
                        item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                    }
                }

                item.Room = SessionHelper.RoomID;
                item.CompanyID = SessionHelper.CompanyID;
                item.SupplierID = ((SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0);
                item.CreatedBy = SessionHelper.UserID;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.ReceivedOn = DateTimeUtility.DateTimeNow;
                item.EditedFrom = "Web";
                item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                item.AddedFrom = "Web";
                objApi.Insert(item, SessionUserId);
            }

            if (objReqDTO != null)
            {
                objApi.UpdateRequisitionTotalCost(objReqDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            if (!string.IsNullOrEmpty(ItemLevelErrMsg) && !string.IsNullOrWhiteSpace(ItemLevelErrMsg))
            {
                message = ResRequisitionMaster.FailToAddTools + " " + ItemLevelErrMsg;
                status = "fail";
            }
            else
            {
                message = ResRequisitionMaster.ToolAddedSuccessfully;
                status = "ok";
            }
            
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToolCategoryToDetailTable(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            RequisitionDetailsDTO[] reqDetails = s.Deserialize<RequisitionDetailsDTO[]>(para);

            RequisitionDetailsDAL objRequisitionDetailDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objReqDTO = objRequisitionMasterDAL.GetRequisitionByGUIDPlain((Guid)reqDetails[0].RequisitionGUID);
            long SessionUserId = SessionHelper.UserID;
            foreach (RequisitionDetailsDTO item in reqDetails)
            {
                if (item.ToolCategoryID.GetValueOrDefault(0) > 0)
                {
                    RequisitionDetailsDTO objExistReqDtlDTo = objRequisitionDetailDAL.GetReqLinesByReqGUIDToolCategory((Guid)item.RequisitionGUID, item.ToolCategoryID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objExistReqDtlDTo == null)
                    {
                        if (!string.IsNullOrWhiteSpace(item.RequiredDateStr))
                            item.RequiredDate = DateTime.ParseExact(item.RequiredDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
                        else
                            item.RequiredDate = objReqDTO.RequiredDate;

                        item.Room = SessionHelper.RoomID;
                        item.CompanyID = SessionHelper.CompanyID;
                        item.SupplierID = ((SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0);
                        item.CreatedBy = SessionHelper.UserID;
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        item.AddedFrom = "Web";
                        item.IsToolCategoryInsert = false;
                        item.QuantityRequisitioned = 1;
                        objRequisitionDetailDAL.Insert(item, SessionUserId);
                    }
                }
            }
            //if (objReqDTO != null)
            //{
            //    objRequisitionDetailDAL.UpdateRequisitionTotalCost(objReqDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //}

            message = ResRequisitionMaster.ToolCategoryAddedSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApprovedRequisitionUDFUpdate(string para)
        {
            string message = "";
            string status = "";
            string ItemLevelErrMsg = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            RequisitionDetailsDTO[] QLDetails = s.Deserialize<RequisitionDetailsDTO[]>(para);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDAL objApi = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            var msgInvalidTechnician = ResToolMaster.MsgInvalidTechnicianWithName;
            foreach (RequisitionDetailsDTO item in QLDetails)
            {
                if ((item.GUID == null || item.GUID == Guid.Empty) && item.ItemGUID != null && item.BinID != null && item.RequisitionGUID != null)
                {
                    RequisitionDetailsDTO objItem = objApi.GetRequisitionDetailsItemGUIDBinIDSinglePlain((Guid)item.ItemGUID, item.BinID ?? 0, (Guid)item.RequisitionGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (objItem != null && objItem.GUID != null && objItem.GUID != Guid.Empty)
                    {
                        item.GUID = objItem.GUID;
                    }
                }
                
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CompanyID = SessionHelper.CompanyID;
                item.SupplierID = ((SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0);
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;

                if (!string.IsNullOrEmpty(item.Technician) && !string.IsNullOrWhiteSpace(item.Technician))
                {
                        string TechnicianCode = "";
                        string TechnicianName = item.Technician.Trim();
                        
                        if (TechnicianName.Contains(" --- "))
                        {
                            TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                            TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        else
                        {
                            TechnicianCode = TechnicianName;
                            TechnicianName = "";
                        }

                        if (TechnicianCode.Trim() == "")
                        {
                            ItemLevelErrMsg += (" " + string.Format(msgInvalidTechnician, item.Technician));
                            continue;
                        }

                        if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                            || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                        {
                            ItemLevelErrMsg += " " + ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician + " : " + item.Technician;
                            continue;
                        }

                        TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                        TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (objTechnicianMasterDTO == null)
                        {
                            bool HasOnTheFlyEntryRight = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.OnTheFlyEntry);
                            bool HasInsertTechnicianRight = SessionHelper.GetModulePermission(SessionHelper.ModuleList.TechnicianMaster, SessionHelper.PermissionType.Insert);
                            
                            if (!HasOnTheFlyEntryRight || !HasInsertTechnicianRight)
                            {
                                ItemLevelErrMsg += " " + ResMessage.NoRightsToInsertTechnician + " : " + item.Technician;
                                continue;
                            }

                            objTechnicianMasterDTO = new TechnicianMasterDTO();
                            objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                            objTechnicianMasterDTO.Technician = TechnicianName;
                            objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                            objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTechnicianMasterDTO.GUID = Guid.NewGuid();
                            objTechnicianMasterDTO.IsArchived = false;
                            objTechnicianMasterDTO.IsDeleted = false;
                            Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                            objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                        else
                        {
                            item.TechnicianGUID = objTechnicianMasterDTO.GUID;
                        }
                }

                if (item.GUID != null && item.GUID != Guid.Empty && item.ItemGUID != null)
                {
                    RequisitionDetailsDTO objReqDtl = objApi.GetRequisitionDetailsByGUIDPlain((Guid)item.GUID);
                    if (objReqDtl != null)
                    {
                        #region "Add Project Spend"
                        CommonDAL objCDAL = objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                        if (!string.IsNullOrWhiteSpace(item.ProjectSpendName))
                        {
                            bool isAllowProjectSpendInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                            if (isAllowProjectSpendInsert)
                            {
                                item.ProjectSpendGUID = objCDAL.GetOrInsertProject(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Web").GUID;
                            }
                            else
                            {
                                ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                                var projectMaster = objProjectMasterDAL.GetProjectByName(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);
                                if (projectMaster == null || projectMaster.GUID == null && projectMaster.GUID == Guid.Empty)
                                {
                                    message = ResMessage.NoRightsToInsertProjectSpend;
                                    status = "fail";
                                    return Json(new { Message = message.ToString(), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    item.ProjectSpendGUID = projectMaster.GUID;
                                }
                            }
                        }
                        #endregion
                        objReqDtl.ProjectSpendGUID = item.ProjectSpendGUID;
                        objReqDtl.PullOrderNumber = item.PullOrderNumber;
                        objReqDtl.TechnicianGUID = item.TechnicianGUID;

                        if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        { 
                            objReqDtl.ToolCheckoutUDF1 = item.ToolCheckoutUDF1;
                            objReqDtl.ToolCheckoutUDF2 = item.ToolCheckoutUDF2;
                            objReqDtl.ToolCheckoutUDF3 = item.ToolCheckoutUDF3;
                            objReqDtl.ToolCheckoutUDF4 = item.ToolCheckoutUDF4;
                            objReqDtl.ToolCheckoutUDF5 = item.ToolCheckoutUDF5;
                        }

                        objApi.Edit(objReqDtl, SessionHelper.UserID, false);
                    }
                }

                if (item.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
                    int TotalRecordCount = 0;
                    var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, "PullMaster", SessionHelper.RoomID);

                    if (item.QuickListItemGUID != null && item.QuickListItemGUID != Guid.Empty)
                    {
                        Dictionary<string, string> dictReturn = objApi.GetRequisitionUDFs(item.GUID);
                        List<string> lstMissingUDFs = null;

                        if (DataFromDBUDF != null)
                            lstMissingUDFs = (new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" }).Where(x => !DataFromDBUDF.Where(z => !String.IsNullOrEmpty(z.UDFControlType) && (z.IsDeleted == false)).Select(y => y.UDFColumnName).Contains(x)).ToList();
                        else
                            lstMissingUDFs = new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" };

                        if (dictReturn != null)
                        {
                            if ((string.IsNullOrEmpty(item.PullUDF1) || string.IsNullOrWhiteSpace(item.PullUDF1)) && (!string.IsNullOrEmpty(dictReturn["UDF1"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF1"])) && lstMissingUDFs.Contains("UDF1"))
                                item.PullUDF1 = dictReturn["UDF1"];

                            if ((string.IsNullOrEmpty(item.PullUDF2) || string.IsNullOrWhiteSpace(item.PullUDF2)) && (!string.IsNullOrEmpty(dictReturn["UDF2"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF2"])) && lstMissingUDFs.Contains("UDF2"))
                                item.PullUDF2 = dictReturn["UDF2"];

                            if ((string.IsNullOrEmpty(item.PullUDF3) || string.IsNullOrWhiteSpace(item.PullUDF3)) && (!string.IsNullOrEmpty(dictReturn["UDF3"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF3"])) && lstMissingUDFs.Contains("UDF3"))
                                item.PullUDF3 = dictReturn["UDF3"];

                            if ((string.IsNullOrEmpty(item.PullUDF4) || string.IsNullOrWhiteSpace(item.PullUDF4)) && (!string.IsNullOrEmpty(dictReturn["UDF4"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF4"])) && lstMissingUDFs.Contains("UDF4"))
                                item.PullUDF4 = dictReturn["UDF4"];

                            if ((string.IsNullOrEmpty(item.PullUDF5) || string.IsNullOrWhiteSpace(item.PullUDF5)) && (!string.IsNullOrEmpty(dictReturn["UDF5"]) && !string.IsNullOrWhiteSpace(dictReturn["UDF5"])) && lstMissingUDFs.Contains("UDF5"))
                                item.PullUDF5 = dictReturn["UDF5"];
                        }
                    }

                    bool UDF1Encrypt = false;
                    bool UDF2Encrypt = false;
                    bool UDF3Encrypt = false;
                    bool UDF4Encrypt = false;
                    bool UDF5Encrypt = false;

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
                    CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                    item.PullUDF1 = (UDF1Encrypt == true && item.PullUDF1 != null && (!string.IsNullOrWhiteSpace(item.PullUDF1))) ? objCommon.GetEncryptValue(item.PullUDF1) : item.PullUDF1;
                    item.PullUDF2 = (UDF2Encrypt == true && item.PullUDF2 != null && (!string.IsNullOrWhiteSpace(item.PullUDF2))) ? objCommon.GetEncryptValue(item.PullUDF2) : item.PullUDF2;
                    item.PullUDF3 = (UDF3Encrypt == true && item.PullUDF3 != null && (!string.IsNullOrWhiteSpace(item.PullUDF3))) ? objCommon.GetEncryptValue(item.PullUDF3) : item.PullUDF3;
                    item.PullUDF4 = (UDF4Encrypt == true && item.PullUDF4 != null && (!string.IsNullOrWhiteSpace(item.PullUDF4))) ? objCommon.GetEncryptValue(item.PullUDF4) : item.PullUDF4;
                    item.PullUDF5 = (UDF5Encrypt == true && item.PullUDF5 != null && (!string.IsNullOrWhiteSpace(item.PullUDF5))) ? objCommon.GetEncryptValue(item.PullUDF5) : item.PullUDF5;
                    if (item.GUID != Guid.Empty)
                    {
                        item.LastUpdated = DateTimeUtility.DateTimeNow;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        objApi.EditUDFDetailOnly(item);
                    }

                }
            }

            if (!string.IsNullOrEmpty(ItemLevelErrMsg) && !string.IsNullOrWhiteSpace(ItemLevelErrMsg))
            {
                message = ResRequisitionMaster.FailToUpdateUDF + " "+ ItemLevelErrMsg;
                status = "fail";
            }
            else
            {
                message = ResRequisitionMaster.ItemUDFUpdatedSuccessfully;
                status = "ok";
            }
            
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeRequisitionStatus(string RequisitionID, string OldStatus, string NewStatus)
        {
            string message = "";
            string status = "";
            RequisitionMasterDAL objReqMastDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objReqMstDTO = objReqMastDAL.GetRequisitionByIDPlain(int.Parse(RequisitionID));

            objReqMstDTO.RequisitionStatus = NewStatus;
            objReqMstDTO.WhatWhereAction = "Requisition";
            objReqMstDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objReqMstDTO.EditedFrom = "Web";

            string requisitionDataLog = "Method - ChangeRequisitionStatus; Controller-ConsumeController : on " + DateTime.UtcNow.ToString();
            requisitionDataLog = requisitionDataLog + "; " + "RequisitionStatus-" + objReqMstDTO.RequisitionStatus + ",Username :" + (SessionHelper.UserName != null ? SessionHelper.UserName : "");
            objReqMstDTO.RequisitionDataLog = requisitionDataLog;

            objReqMastDAL.Edit(objReqMstDTO);

            message = ResRequisitionMaster.StatusUpdatedSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadRequisitionItems(Int64 RequisitionID)
        {
            ViewBag.RequisitionID = RequisitionID;
            RequisitionMasterDAL objDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            List<RequisitionDetailsDTO> ReqDetaiData = new List<RequisitionDetailsDTO>();
            bool IsArchived = false;
            bool IsDeleted = false;

            if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
            if (Request["IsDeleted"] != null && !string.IsNullOrEmpty(Request["IsDeleted"].ToString()))
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            RequisitionMasterDTO objDTO = IsArchived ? objDAL.GetArchivedRequisitionByIDPlain(RequisitionID) 
                                                     : objDAL.GetRequisitionByIDPlain(RequisitionID);                

            if (objDTO != null && objDTO.ID > 0)
            {
                ViewBag.IsStagingRequisition = (objDTO.StagingID.HasValue && objDTO.StagingID.Value > 0);
                ViewBag.WorkOrderGUID = objDTO.WorkorderGUID;
                ViewBag.MaterialStagingGUID = objDTO.MaterialStagingGUID.HasValue && objDTO.MaterialStagingGUID.Value != Guid.Empty ? objDTO.MaterialStagingGUID.Value : Guid.Empty;
                bool IsHitory = false;

                if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                    IsHitory = bool.Parse(Request["IsHistory"].ToString());

                if (IsHitory)
                {
                    objDTO.IsRecordEditable = false;
                    ViewBag.IsRecordEditableBag = false;
                }
                else
                {
                    objDTO.IsRecordEditable = IsRecordEditable(objDTO);
                    ViewBag.IsRecordEditableBag = IsRecordEditable(objDTO);
                }

                ViewBag.RequisitionStatus = objDTO.RequisitionStatus;
                ViewBag.RequisitionGUID = objDTO.GUID;
                ViewBag.ReqDTO = objDTO;
                RequisitionDetailsDAL obj = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                int Totalrecs = 0;

                ReqDetaiData = obj.GetPagedRequisitionDetails(0, 1000, out Totalrecs, string.Empty, "ID DESC", SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, objDTO.GUID);

                bool UDF1Encrypt = false;
                bool UDF2Encrypt = false;
                bool UDF3Encrypt = false;
                bool UDF4Encrypt = false;
                bool UDF5Encrypt = false;
                UDFDAL objUdf = new UDFDAL(SessionHelper.EnterPriseDBName);
                int TotalRecordCount = 0;
                var DataFromDBUDF = objUdf.GetPagedUDFsByUDFTableNamePlain(0, 5, out TotalRecordCount, "ID asc", SessionHelper.CompanyID, "PullMaster", SessionHelper.RoomID);

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
                CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                foreach (RequisitionDetailsDTO r in ReqDetaiData)
                {
                    r.PullUDF1 = (UDF1Encrypt == true && r.PullUDF1 != null) ? objCommon.GetDecryptValue(r.PullUDF1) : r.PullUDF1;
                    r.PullUDF2 = (UDF2Encrypt == true && r.PullUDF2 != null) ? objCommon.GetDecryptValue(r.PullUDF2) : r.PullUDF2;
                    r.PullUDF3 = (UDF3Encrypt == true && r.PullUDF3 != null) ? objCommon.GetDecryptValue(r.PullUDF3) : r.PullUDF3;
                    r.PullUDF4 = (UDF4Encrypt == true && r.PullUDF4 != null) ? objCommon.GetDecryptValue(r.PullUDF4) : r.PullUDF4;
                    r.PullUDF5 = (UDF5Encrypt == true && r.PullUDF5 != null) ? objCommon.GetDecryptValue(r.PullUDF5) : r.PullUDF5;
                }
            }
            else
            {
                ViewBag.IsStagingRequisition = false;
                ViewBag.MaterialStagingGUID = Guid.Empty;
            }

            CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,AllowPullBeyondAvailableQty";
            RoomDTO ROOMDTO = commonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            if(ROOMDTO != null && ROOMDTO.ID > 0)
            {
                ViewBag.AllowPullBeyondAvailableQty = ROOMDTO.AllowPullBeyondAvailableQty;
            }

            return PartialView("_CreateRequisitionItems", ReqDetaiData);
        }

        public string RequisitionItemsDelete(string ids, string RequisitionGUID)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                RequisitionDetailsDAL obj = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionUserId,SessionHelper.EnterPriceID);
                //AddReqItemsToWOItems(Guid.Parse(RequisitionGUID));
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult PullMasterListByRequisitionAjax(JQueryDataTableParamModel param)
        {
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);

            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            //var DataFromDB = objPullDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.RequisitionDetailGUID == Guid.Parse(Request["RequisitionDetailGUID"].ToString()));
            var DataFromDB = objPullDAL.GetPullsByRequisitionDetailGuidNormal(Guid.Parse(Request["RequisitionDetailGUID"]), SessionHelper.RoomID, SessionHelper.CompanyID);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public string RequisitionPulls(string RequisitionDetailGUID, Int64 BigID)
        {
            RequisitionDetailsDAL reqDetailDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            RequisitionDetailsDTO reqDetailDTO = reqDetailDAL.GetRequisitionDetailsByGUIDPlain(Guid.Parse(RequisitionDetailGUID));
            ViewBag.RequisitionDetailGUID = RequisitionDetailGUID;
            ViewBag.RequisitionDetailHistoryID = BigID;
            if (reqDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                //var DataFromDB = obj.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.RequisitionDetailGUID == Guid.Parse(RequisitionDetailGUID)).OrderByDescending(x => x.PoolQuantity.GetValueOrDefault(0));
                var DataFromDB = obj.GetPullsByRequisitionDetailGuidNormal(Guid.Parse(RequisitionDetailGUID), SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.PoolQuantity.GetValueOrDefault(0));
                return RenderRazorViewToString("_RequisitionPulls", DataFromDB);
            }
            else if (reqDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                ViewBag.ToolGUID = reqDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty);
                ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == reqDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty)).ToList();
                var objModel = objDAL.GetTCIOHsByToolGUIDFull(reqDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                return RenderRazorViewToString("_RequisitionedTools", objModel);
            }

            return "";
        }

        public JsonResult CloseSelectedRequistionItem(Guid[] RequisitionDetailGUIDs)
        {
            string message = "";
            string status = "";
            RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            objRequisitionDetailsDAL.CloseSelectedRequistionItem(RequisitionDetailGUIDs, SessionHelper.UserID);

            if (RequisitionDetailGUIDs != null && RequisitionDetailGUIDs.Count() > 0 && RequisitionDetailGUIDs[0] != Guid.Empty)
                CloseRequisitionIfPullCompletedByDetailId(RequisitionDetailGUIDs[0].ToString());

            message = ResRequisitionMaster.ItemClosedSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Requisition History
        //private object GetUDFDataPageWise(string PageName)
        //{
        //    UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID);

        //    var result = from c in DataFromDB
        //                 select new UDFDTO
        //                 {
        //                     ID = c.ID,
        //                     CompanyID = c.CompanyID,
        //                     Room = c.Room,
        //                     UDFTableName = c.UDFTableName,
        //                     UDFColumnName = c.UDFColumnName,
        //                     UDFDefaultValue = c.UDFDefaultValue,
        //                     UDFOptionsCSV = c.UDFOptionsCSV,
        //                     UDFControlType = c.UDFControlType,
        //                     UDFIsRequired = c.UDFIsRequired,
        //                     UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
        //                     Created = c.Created,
        //                     Updated = c.Updated,
        //                     UpdatedByName = c.UpdatedByName,
        //                     CreatedByName = c.CreatedByName,
        //                     IsDeleted = c.IsDeleted,
        //                 };
        //    return result;
        //}
        public ActionResult RequisitionHistoryView(Int64 ID)
        {
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objDTO = obj.GetRequisitionHistoryByHistoryID(ID);
            RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            if (objDTO != null)
            {
                Double PullSum = objReqDDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID).Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();
                ViewBag.PulledQuantity = PullSum;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            //BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.BinMaster = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //ViewBag.BinMaster = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room WITH (NOLOCK) where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            #region "Project Spend DropDownList"
            //ProjectMasterController objProjectApi = new ProjectMasterController();
            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }

                ViewBag.ProjectSpent = lstProject.OrderBy(x => x.ProjectSpendName).ToList();
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            ViewBag.RequisitionStatusBag = GetRequisitionStatus(objDTO.RequisitionStatus);
            ViewBag.RequisitionTypeBag = GetRequisitionType();
            return PartialView("_CreateRequisition_History", objDTO);
        }

        public ActionResult RequisitionHistoryViewMaintenance(Int64 ID)
        {

            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objDTO = obj.GetHistoryRecordMaintenance(ID);
            RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
            if (objDTO != null)
            {
                Double PullSum = objReqDDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID).Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();//.Where(x => x.RequisitionGUID == objDTO.GUID)
                ViewBag.PulledQuantity = PullSum;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            //BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.BinMaster = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //ViewBag.BinMaster = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            CustomerMasterDAL objCustApi = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room WITH (NOLOCK) where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            #region "Project Spend DropDownList"
            //ProjectMasterController objProjectApi = new ProjectMasterController();
            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }
                ViewBag.ProjectSpent = lstProject.OrderBy(x => x.ProjectSpendName).ToList();
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            ViewBag.RequisitionStatusBag = GetRequisitionStatus(objDTO.RequisitionStatus);
            ViewBag.RequisitionTypeBag = GetRequisitionType();
            return PartialView("_CreateRequisition_History", objDTO);
        }

        public ActionResult LoadRequisitionLineItemsHistory(Int64 historyID)
        {
            RequisitionMasterDTO objDTO = null;
            objDTO = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName).GetRequisitionHistoryByHistoryID(historyID);
            if (objDTO != null)
                objDTO.RequisitionListItem = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetHistoryRecordByRequisitionId(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            return PartialView("_CreateRequisitionItems_History", objDTO);
        }

        public JsonResult UncloseRequisition(string ReqGUID)
        {
            string message = string.Empty;
            string status = string.Empty;
            RequisitionMasterDAL obj = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
            RequisitionMasterDTO objDTO = new RequisitionMasterDTO();
            objDTO = obj.GetRequisitionByGUIDPlain(Guid.Parse(ReqGUID));


            if (objDTO != null)
            {
                objDTO.RequisitionStatus = "Unsubmitted";
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                try
                {
                    objDTO.WhatWhereAction = "Requisition";
                    bool IsUpdate = false;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    objDTO.RequesterID = null;
                    objDTO.ApproverID = null;


                    string requisitionDataLog = "Method - UncloseRequisition; Controller-ConsumeController : on " + DateTime.UtcNow.ToString();
                    requisitionDataLog = requisitionDataLog + "; " + "RequisitionStatus-" + objDTO.RequisitionStatus + ",Username :" + (SessionHelper.UserName != null ? SessionHelper.UserName : "");
                    objDTO.RequisitionDataLog = requisitionDataLog;

                    IsUpdate = obj.Edit(objDTO);
                    if (IsUpdate)
                    { //---------------------------------------------------------------------------------
                      //
                        RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<RequisitionDetailsDTO> lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (lstRequisitionDetails != null && lstRequisitionDetails.Count() > 0)
                        {
                            foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionDetails)
                            {
                                objRequisitionDetailsDAL.UpdateItemOnRequisitionQty(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.Room, objRequisitionDetails.CompanyID, objRequisitionDetails.LastUpdatedBy);
                            }
                        }
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        //eTurns.DAL.CacheHelper<IEnumerable<RequisitionMasterDTO>>.InvalidateCache();
                    }
                    else
                    {
                        message = ResMessage.SaveErrorMsg;
                        status = "fail";
                    }
                }
                catch (Exception)
                {
                    message = ResMessage.SaveErrorMsg;
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Consume RedCount
        /// <summary>
        /// GetReplinshRedCount
        /// </summary>
        /// <param name="CurrentModule"></param>
        /// <returns></returns>
        public JsonResult GetConsumeRedCount()
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<RedCountDTO> lstRedCount = objCommonDAL.GetRedCountByModuleType("Consume", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem));
            Int64 ConsumeMenuButtonCount = 0;
            Int64 RequisitionMenuLinkCount = 0;
            List<RedCountDTO> lstReqRedCount = lstRedCount.Where(x => x.ModuleName == "Requisition").ToList();
            RequisitionMenuLinkCount = lstReqRedCount.Where(x => x.Status != "Closed").Sum(x => x.RecCircleCount);
            bool isRequisitions = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

            if (!isRequisitions)
            {
                RequisitionMenuLinkCount = 0;
            }

            ConsumeMenuButtonCount = RequisitionMenuLinkCount;

            return Json(new
            {
                Message = "ok",
                Status = "ok",
                ModuleType = "Consume",
                ConsumeMenuButtonCount = ConsumeMenuButtonCount,
                RequisitionMenuLinkCount = RequisitionMenuLinkCount,
                RequisitionRedCount = lstReqRedCount,

            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// This method is used to set Staging ViewBag for create/edit requisition
        /// </summary>
        private void SetStagingViewBag()
        {
            //List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(t => t.StagingName).ToList();
            List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName).ToList();
            lstStaging.Insert(0, new MaterialStagingDTO());
            ViewBag.StagingList = lstStaging;
        }
        public ActionResult GetRequisitionNarrwSearchHTML(string tabName,bool IsDeleted,bool IsArchived)
        {
            CommonDTO commonDTO = new CommonDTO
            {
                ListName = string.IsNullOrEmpty(tabName) ? "RequisitionMaster" : "RequisitionMaster" + tabName,
                PageName = "RequisitionMaster",
                IsDeleted = IsDeleted,
                IsArchived = IsArchived
            };
            return PartialView("_RequisitionNarrowSearch", commonDTO);
        }

        [HttpPost]
        public JsonResult UpdateSelectedRequisitionToClose(string GUIDs)
        {
            string message, status;
            int result;

            try
            {
                RequisitionMasterDAL requisitionMasterDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in GUIDs.Split(','))
                {
                    if (item != string.Empty)
                    {
                        requisitionMasterDAL.ClosedRequistionByIDs(item, SessionHelper.UserID);
                        requisitionMasterDAL.CloseRequisition(Guid.Parse(item), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //---------------------------------------------------------------------------------//
                        IEnumerable<RequisitionDetailsDTO> lstRequisitionDetails = objRequisitionDetailsDAL.GetReqLinesByReqGUIDPlain(Guid.Parse(item), 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (lstRequisitionDetails != null && lstRequisitionDetails.Count() > 0)
                        {
                            foreach (RequisitionDetailsDTO objRequisitionDetails in lstRequisitionDetails)
                            {
                                if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                                {
                                    objRequisitionDetailsDAL.UpdateRequisitionedQuantity("Delete", objRequisitionDetails, objRequisitionDetails);
                                }
                                //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Req.DetailDAL >> Save Requisition Close");
                                new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty), objRequisitionDetails.LastUpdatedBy, "web", "Consume >> Close Requisition", SessionHelper.UserID);
                                if (objRequisitionDetails.QuantityPulled == null || objRequisitionDetails.QuantityPulled <= 0)
                                {
                                    ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                                    ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objRequisitionDetails.ItemGUID.GetValueOrDefault(Guid.Empty));
                                    if (ItemDTO != null && ItemDTO.ID > 0)
                                    {
                                        /* WI-5009 update QtyToMeetDemand */
                                        if (ItemDTO.ItemType == 3 && ItemDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                                        {
                                            new KitDetailDAL(SessionHelper.EnterPriseDBName).UpdateQtyToMeedDemand(ItemDTO.GUID, SessionHelper.UserID, SessionHelper.UserID);
                                        }
                                        /* WI-5009 update QtyToMeetDemand */
                                    }
                                }
                            }
                        }
                    }
                }

                status = "ok";
                message = ResRequisitionMaster.SelectedRequisitionClosedSuccessfully;
            }
            catch (Exception ex)
            {
                status = "fail";
                message = "";
            }
            return Json(new { Message = message, Status = status });
        }

        public ActionResult Get(string path)
        {
            string referrer = Request.UrlReferrer?.Host;
            if (referrer != null)// && referrer.Contains("localhost")
            {
                string filePath = Server.MapPath(path);
                return File(filePath, MimeMapping.GetMimeMapping(filePath));
            }

            return HttpNotFound();
        }
    }
}
