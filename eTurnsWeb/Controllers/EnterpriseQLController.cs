using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{    
    public class EnterpriseQLController : eTurnsControllerBase
    {
        readonly string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        readonly string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);

        public ActionResult EnterpriseQLList()
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);

            if ((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL)
            { 
                return View("List");    
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult EnterpriseQuickListAjax(JQueryDataTableParamModel param)
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            
            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL ))
            {
                return RedirectToAction(ActName, CtrlName);
            }

            string sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = SessionHelper.CurrentTimeZone;
            EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EnterpriseQLDTO> DataFromDB = enterpriseQuickListDAL.GetPagedEnterpriseQuickList(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsDeleted,IsArchived, RoomDateFormat, CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateEnterpriseQL()
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            
            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL))
            {
                return RedirectToAction(ActName, CtrlName);
            }
             
            var model = new EnterpriseQLDTO(){ GUID = Guid.NewGuid() };
            return PartialView("_CreateEnterpriseQL", model); 
        }

        public ActionResult EditEnterpriseQL(Guid GUID)
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            
            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL))
            {
                return RedirectToAction(ActName, CtrlName);
            }

            EnterpriseQLDTO quickListMaster;

            if (GUID != Guid.Empty)
            { 
                EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
                quickListMaster = enterpriseQuickListDAL.GetEnterpriseQuickListByGuidNormal(GUID); 
                string itemGUIDString = Convert.ToString(GUID);
                Session["EnterpriseQLDetail" + itemGUIDString] = enterpriseQuickListDAL.GetEnterpriseQLDetailByQLMGuidPlain(GUID);
            }
            else
            {
                quickListMaster = new EnterpriseQLDTO(){ GUID = Guid.NewGuid() };
            }
            
            return PartialView("_CreateEnterpriseQL", quickListMaster);
        }
        

        [HttpPost]
        public JsonResult SaveEnterpriseQL(EnterpriseQLDTO EnterpriseQL)
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            
            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL)) 
            {
                return Json(new { Message = ResCommon.NoRights, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            string message = string.Empty;
            string status = string.Empty;
            string itemGUIDString = Convert.ToString(EnterpriseQL.GUID);
            List<EnterpriseQLDetailDTO> lstEnterpriseQLDetail = new List<EnterpriseQLDetailDTO>();
            EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
            DataTable dtTable = new DataTable();

            var isDuplicate = enterpriseQuickListDAL.IsEnterpriseQLDuplicate(EnterpriseQL.ID,EnterpriseQL.Name);
            
            if (isDuplicate)
            {
                message = eTurns.DTO.Resources.ResMessage.EnterpriseQLDuplicate;
                status = "duplicate";                
            }
            else
            { 
                List<EnterpriseQLDetailTbl> EntQLList = new List<EnterpriseQLDetailTbl>();

                if (Session["EnterpriseQLDetail" + itemGUIDString] != null)
                {
	                lstEnterpriseQLDetail = ((List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.QLDetailNumber) && t.Quantity > 0).ToList();

                    foreach(var QLDetail in lstEnterpriseQLDetail)
                    { 
                        EnterpriseQLDetailTbl entQL = new EnterpriseQLDetailTbl(){ ID = QLDetail.ID , QLDetailNumber = QLDetail.QLDetailNumber,Quantity = QLDetail.Quantity };
                        EntQLList.Add(entQL);
                    }                     
                }

                dtTable = CommonUtilityHelper.ToDataTable(EntQLList);                    
                bool result = false;

                if (EnterpriseQL.ID > 0)
                { 
                    result = enterpriseQuickListDAL.EditEnterpriseQuickList(EnterpriseQL.GUID, SessionHelper.UserID,EnterpriseQL.Name,EnterpriseQL.RoomIds, dtTable);
                }
                else
                { 
                  result = enterpriseQuickListDAL.InsertEnterpriseQuickList(SessionHelper.UserID,SessionHelper.EnterPriceID,EnterpriseQL.Name,EnterpriseQL.RoomIds, dtTable);  
                } 

                if (result)
                {
                    Session["EnterpriseQLDetail" + itemGUIDString] = null;  
                    status = "ok";
                    message = eTurns.DTO.Resources.ResMessage.EnterpriseQLSuccess;
                }
                else
                { 
                    status = "fail";
                    message = eTurns.DTO.Resources.ResMessage.EnterpriseQLFailure;
                }
            }
            

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomList(string SelectedRoomIds, long Id)
        {
            bool AppendCompanyname = true;
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<KeyValDTO> lstRoomDTO = new List<KeyValDTO>();
            Int64 RoleID = SessionHelper.RoleID;
            Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<RoomDTO> DBRoomDTO = objEnterpriseMasterDAL.GetAllRoomsByEnterpriseIdNormal(SessionHelper.EnterPriceID);
            //List<RoomDTO> DBRoomDTO = objRoomDAL.GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, Convert.ToString(Session_EnterPriceID), RoleID, Session_EnterPriceID);
            long SelectedRoom = Id > 0 ? 0 : SessionHelper.RoomID;

            if (DBRoomDTO != null && DBRoomDTO.Any() && !string.IsNullOrEmpty(SelectedRoomIds) && !string.IsNullOrWhiteSpace(SelectedRoomIds) && 
                (SelectedRoomIds ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count() > 0)
            {
                long convertedInt;
                var EntListToShowReleaseNo = SelectedRoomIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new { didConvert = long.TryParse(s.Trim(), out convertedInt), convertedValue = convertedInt })
                        .Where(w => w.didConvert)
                        .Select(s => s.convertedValue)
                        .ToList();

                if (EntListToShowReleaseNo != null && EntListToShowReleaseNo.Any() && EntListToShowReleaseNo.Count() > 0)
                { 
                    var selectedRooms = DBRoomDTO.Where(t => EntListToShowReleaseNo.Contains(t.ID));
                    if (selectedRooms != null && selectedRooms.Any() && selectedRooms.Count() > 0)
                    { 
                        lstRoomDTO = (from c in selectedRooms
                              select new KeyValDTO
                              {
                                  key = c.ID.ToString(),
                                  value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                              }).OrderBy(x => x.value).ToList();      
                    }
                
                    var nonSelectedRooms = DBRoomDTO.Where(t => !EntListToShowReleaseNo.Contains(t.ID));

                    if (nonSelectedRooms != null && nonSelectedRooms.Any() && nonSelectedRooms.Count() > 0)
                    { 
                        lstRoomDTO.AddRange((from c in nonSelectedRooms
                              select new KeyValDTO
                              {
                                  key = c.ID.ToString(),
                                  value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                              }).OrderBy(x => x.value).ToList());     
                    }    
                }
                else
                { 
                    lstRoomDTO = (from c in DBRoomDTO
                              select new KeyValDTO
                              {
                                  key = c.ID.ToString(),
                                  value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                              }).OrderBy(x => x.value).ToList();    
                }                
            }
            else
            { 
                if (Id > 0)
                { 
                    lstRoomDTO = (from c in DBRoomDTO
                                  select new KeyValDTO
                                  {
                                      key = c.ID.ToString(),
                                      value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                                  }).OrderBy(x => x.value).ToList(); 
                }
                else
                { 
                    lstRoomDTO = (from c in DBRoomDTO.Where(e=> e.ID == SessionHelper.RoomID)
                          select new KeyValDTO
                          {
                              key = c.ID.ToString(),
                              value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                          }).ToList();

                    lstRoomDTO.AddRange((from c in DBRoomDTO.Where(e=> e.ID != SessionHelper.RoomID)
                              select new KeyValDTO
                              {
                                  key = c.ID.ToString(),
                                  value = ((AppendCompanyname) ? (c.RoomName + "-(" + c.CompanyName + ")") : (c.RoomName))
                              }).OrderBy(x => x.value).ToList());
                }
                
            }
            
            return Json(new { Status = true, RoomList = lstRoomDTO, Selected = SelectedRoom }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadEnterpriseQuickListDetail(Guid ItemGUID, int? AddCount)
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);
            
            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL))
            {
                return RedirectToAction(ActName, CtrlName);
            }

            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            List<EnterpriseQLDetailDTO> lstEnterpriseQLDetail = new List<EnterpriseQLDetailDTO>();

            if (Session["EnterpriseQLDetail" + itemGuidString] != null)
            {
                lstEnterpriseQLDetail = (List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + itemGuidString];
            }

            if (AddCount.HasValue && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstEnterpriseQLDetail.Add(new EnterpriseQLDetailDTO() { ID = 0, SessionSr = lstEnterpriseQLDetail.Count + 1, QLMasterGUID = ItemGUID, GUID = Guid.NewGuid(), Quantity = 1 });
                }
            }

            return PartialView("_EnterpriseQLDetail", lstEnterpriseQLDetail.OrderBy(t => t.QLDetailNumber).ToList());
        }

        public ActionResult LoadEnterpriseQLDetailBySupplierPartNo(Guid ItemGUID, int? AddCount, string SuppllierPartNos)
        {
            var isEnterpriseQL = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseItemQuickList);

            if (!((SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2) && isEnterpriseQL))
            {
                return RedirectToAction(ActName, CtrlName);
            }

            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            List<EnterpriseQLDetailDTO> lstEnterpriseQLDetail = new List<EnterpriseQLDetailDTO>();

            if (Session["EnterpriseQLDetail" + itemGuidString] != null)
            {
                lstEnterpriseQLDetail = (List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + itemGuidString];
            }

            var supplierPartNoList = SuppllierPartNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if(supplierPartNoList != null && supplierPartNoList.Any() && supplierPartNoList.Count() > 0)
            {
                foreach(var spn in supplierPartNoList)
                {
                    var existingEntry = lstEnterpriseQLDetail.Where(e=> e.QLDetailNumber.Trim() == spn.Trim()).FirstOrDefault();
                    if(!(existingEntry != null && !string.IsNullOrEmpty(existingEntry.QLDetailNumber) && !string.IsNullOrWhiteSpace(existingEntry.QLDetailNumber)))
                    {
                        lstEnterpriseQLDetail.Add(new EnterpriseQLDetailDTO() { ID = 0, QLDetailNumber = spn, SessionSr = lstEnterpriseQLDetail.Count + 1, QLMasterGUID = ItemGUID, GUID = Guid.NewGuid(), Quantity = 1 });
                    }
                }
                //for (int i = 0; i < supplierPartNoList.Count(); i++)
                //{
                //    lstEnterpriseQLDetail.Add(new EnterpriseQLDetailDTO() { ID = 0, SessionSr = lstEnterpriseQLDetail.Count + 1, QLMasterGUID = ItemGUID, GUID = Guid.NewGuid(), Quantity = 1 });
                //}
            }
            

            return PartialView("_EnterpriseQLDetail", lstEnterpriseQLDetail.OrderBy(t => t.QLDetailNumber).ToList());
        }

        public JsonResult SaveToSeesionQLDetailSingle(long ID, int SessionSr, Guid GUID, Guid ITEMGUID, string QLDetailNumber, double Quantity)
        {
            Guid newGUID = new Guid();
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<EnterpriseQLDetailDTO> lstEnterpriseQLDetail = null;

            if (Session["EnterpriseQLDetail" + itemGuidString] != null)
            {
                lstEnterpriseQLDetail = (List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + itemGuidString];
            }
            else
            {
                lstEnterpriseQLDetail = new List<EnterpriseQLDetailDTO>();
            }

            if (ID > 0 && SessionSr == 0)
            {
                EnterpriseQLDetailDTO objDTO = lstEnterpriseQLDetail.Where(t => t.ID == ID).FirstOrDefault();
                
                if (objDTO != null)
                {
                    lstEnterpriseQLDetail.Remove(lstEnterpriseQLDetail.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.QLDetailNumber = QLDetailNumber;
                    objDTO.Quantity = Quantity;
                    objDTO.QLMasterGUID = ITEMGUID;
                    objDTO.GUID = Guid.NewGuid();
                    newGUID = objDTO.GUID;

                    lstEnterpriseQLDetail.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    EnterpriseQLDetailDTO objDTO = lstEnterpriseQLDetail.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    
                    if (objDTO != null)
                    {
                        lstEnterpriseQLDetail.Remove(lstEnterpriseQLDetail.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.QLDetailNumber = QLDetailNumber;
                        objDTO.Quantity = Quantity;
                        objDTO.QLMasterGUID = ITEMGUID;
                        objDTO.GUID = Guid.NewGuid();
                        newGUID = objDTO.GUID;
                        lstEnterpriseQLDetail.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new EnterpriseQLDetailDTO
                        {
                            ID = 0,
                            QLDetailNumber = QLDetailNumber,
                            Quantity = Quantity,
                            QLMasterGUID = ITEMGUID,
                            GUID = GUID,
                            SessionSr = lstEnterpriseQLDetail.Count + 1
                        };
                        newGUID = objDTO.GUID;
                        lstEnterpriseQLDetail.Add(objDTO);
                    }
                }
                else
                {
                    EnterpriseQLDetailDTO objDTO = new EnterpriseQLDetailDTO
                    {
                        ID = 0,
                        QLDetailNumber = QLDetailNumber,
                        Quantity = Quantity,
                        QLMasterGUID = ITEMGUID,
                        GUID = GUID,
                        SessionSr = lstEnterpriseQLDetail.Count + 1
                    };
                    newGUID = objDTO.GUID;
                    lstEnterpriseQLDetail.Add(objDTO);
                }
            }

            Session["EnterpriseQLDetail" + itemGuidString] = lstEnterpriseQLDetail;

            return Json(new { status = "ok", newGUID = newGUID }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteToSeesionQLDetailSingle(long ID, Guid GUID, Guid ITEMGUID)
        {
            List<EnterpriseQLDetailDTO> lstBinReplanish = null;
            string itemGuidString = Convert.ToString(ITEMGUID);

            if (Session["EnterpriseQLDetail" + itemGuidString] != null)
            {
                lstBinReplanish = (List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<EnterpriseQLDetailDTO>();
            }
            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["EnterpriseQLDetail" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    Session["EnterpriseQLDetail" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
    
        public JsonResult DeleteEnterpriseQL(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, "EnterpriseQL", true, SessionHelper.UserID,SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public JsonResult UnDeleteRecords(string ids, string ModuleName)
        { 
            try
            { 
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ModuleUnDeleteDTO objModuleUnDeleteDTO = new ModuleUnDeleteDTO();
                objModuleUnDeleteDTO = objCommonDAL.UnDeleteModulewise(ids, "EnterpriseQL", true, SessionHelper.UserID, true,SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID);               
                response = objModuleUnDeleteDTO.CommonMessage;   
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception)
            { 
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllSupplierPartNo(string SupplierPartNo,string RoomIds)
        {
            ItemSupplierDetailsDAL itemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            var supplierPartNos = itemSupplierDetailsDAL.GetItemSupplierPartNoByRoomIds(RoomIds, SupplierPartNo);
            supplierPartNos = supplierPartNos.OrderBy(b => b.SupplierNumber.Trim()).ToList();
            return Json(supplierPartNos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSupplierPartNoDDLData(string SupplierPartNo, string RoomIds, string GUID)
        {
            ItemSupplierDetailsDAL itemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            List<EnterpriseQLDetailDTO> lstEnterpriseQLDetail = new List<EnterpriseQLDetailDTO>();
            var supplierPartNos = itemSupplierDetailsDAL.GetItemSupplierPartNoByRoomIds(RoomIds, SupplierPartNo);
            List<string> selected = new List<string>();

            if (!string.IsNullOrEmpty(GUID) && !string.IsNullOrWhiteSpace(GUID) && Session["EnterpriseQLDetail" + GUID] != null)
            {
                lstEnterpriseQLDetail = (List<EnterpriseQLDetailDTO>)Session["EnterpriseQLDetail" + GUID];
                
                if (lstEnterpriseQLDetail != null && lstEnterpriseQLDetail.Any() && lstEnterpriseQLDetail.Count() > 0 &&
                   supplierPartNos!= null && supplierPartNos.Any())
                {
                    List<SupplierPartDTO> supplierPartNoList = new List<SupplierPartDTO>();
                    var supplierPartNosToSelect = lstEnterpriseQLDetail.Select(e=> e.QLDetailNumber).ToArray().Distinct();
                    var selectedSPN = supplierPartNos.Where(t => supplierPartNosToSelect.Contains(t.SupplierNumber));
                    
                    if (selectedSPN != null && selectedSPN.Any() && selectedSPN.Count() > 0)
                    {
                        supplierPartNoList.AddRange(selectedSPN.OrderBy(e=>e.SupplierNumber));
                        selected = supplierPartNosToSelect.ToList(); //selected = string.Join(",", supplierPartNosToSelect);
                    }

                    var nonSelectedSPN = supplierPartNos.Where(t => !supplierPartNosToSelect.Contains(t.SupplierNumber));
                    if (nonSelectedSPN != null && nonSelectedSPN.Any() && nonSelectedSPN.Count() > 0)
                    {
                        supplierPartNoList.AddRange(nonSelectedSPN.OrderBy(e => e.SupplierNumber));
                    }
                    supplierPartNos = supplierPartNoList;
                }
                else
                {
                    supplierPartNos = supplierPartNos.OrderBy(b => b.SupplierNumber.Trim()).ToList();
                }
            }
            else
            {
                supplierPartNos = supplierPartNos.OrderBy(b => b.SupplierNumber.Trim()).ToList();
            }
            
            return Json(new {supplierPartNos, selected }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyIdsAndRoomIdsByEnterpriseQL(string EnterpriseQLGuids)
        {
            EnterpriseQLRoomCompany enterpriseQLRoomCompany;
            if (!string.IsNullOrEmpty(EnterpriseQLGuids) && !string.IsNullOrWhiteSpace(EnterpriseQLGuids))
            {
                EnterpriseQuickListDAL enterpriseQuickListDAL = new EnterpriseQuickListDAL(SessionHelper.EnterPriseDBName);
                enterpriseQLRoomCompany = enterpriseQuickListDAL.GetCompanyIdsAndRoomIdsByEnterpriseQLGuids(EnterpriseQLGuids);
            }
            else 
            {
                enterpriseQLRoomCompany = new EnterpriseQLRoomCompany();
            }
            return Json(new { enterpriseQLRoomCompany }, JsonRequestBehavior.AllowGet);
        }
    }
}
