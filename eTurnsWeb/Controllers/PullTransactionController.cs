using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace eTurnsWeb.Controllers
{
    public partial class PullController : eTurnsControllerBase
    {
        [HttpPost]
        public ActionResult PullItemQuantity(List<PullMasterDTO> lstPullRequestInfo)
        {
            List<PullMasterDTO> lstPullRequest = new List<PullMasterDTO>();
            foreach (PullMasterDTO objPullMasterDTO in lstPullRequestInfo)
            {
                if (!lstPullRequest.Select(x => x.ItemGUID).Contains(objPullMasterDTO.ItemGUID))
                    lstPullRequest.Add(objPullMasterDTO);
            }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            lstPullRequestInfo = objPullMasterDAL.GetPullWithDetails(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID);

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,AllowPullBeyondAvailableQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            if(objRoomDTO != null && objRoomDTO.ID > 0)
            {
                ViewBag.AllowPullBeyondAvailableQty = objRoomDTO.AllowPullBeyondAvailableQty;
            }
            return PartialView("PullLotSrSelection", lstPullRequestInfo);
        }


        public JsonResult GetSerialLots(Guid? ItemGUID, string name_startsWith)
        {
            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            long BinID = 0;
            List<ItemLocationLotSerialDTO> lstLotSrs = objPullDetails.GetItemLocationsLotSerials(ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false);
            if (name_startsWith == " " && !string.IsNullOrEmpty(name_startsWith))
            {

            }
            else
            {
                lstLotSrs = lstLotSrs.Where(t => (t.LotOrSerailNumber ?? string.Empty).Contains(name_startsWith)).ToList();
            }

            return Json(lstLotSrs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialLotNumber(Guid? ItemGuid, string SerialOrLotNumber, long BinID, Guid? MaterialStagingGUID)
        {
            bool IsStagginLocation = false;

            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }
            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO;

            if (MaterialStagingGUID.HasValue && MaterialStagingGUID != Guid.Empty)
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, SerialOrLotNumber, IsStagginLocation, MaterialStagingGUID.GetValueOrDefault(Guid.Empty)).FirstOrDefault();
            }
            else
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForPull(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, SerialOrLotNumber, IsStagginLocation).FirstOrDefault();
            }

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ItemGUID = ItemGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.BinNumber = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }

        public ActionResult PullLotSrSelection(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid ToolGUID = Guid.Empty;
            Guid MaterialStagingGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);

            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsStagginLocation = false;
            Guid.TryParse(Convert.ToString(Request["MaterialStagingGUID"]), out MaterialStagingGUID);
            string[] arrIds = new string[] { };
            bool IsFromKit = Convert.ToBoolean(Request["IsFromKit"]);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,AllowPullBeyondAvailableQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;
            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                //objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, BinID, null,null).FirstOrDefault();
                //objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, BinID, null,null).FirstOrDefault();
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }


            int TotalRecordCount = 0;
            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.BinNumber = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;//oItem.DefaultPullQuantity.GetValueOrDefault(0);

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    string[] separatingStrings = { "@@" };
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            //string[] arrItem = arrIds[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            arrItem = new string[2];
                            if (IsFromKit)
                            {
                                arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("@@"));
                                arrItem[1] = arrIds[i].Replace(arrItem[0] + "@@", "");
                            }
                            else
                            {
                                arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                                arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            }
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                if (!dicSerialLots.ContainsKey(arrItem[0]))
                                {
                                    dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                                }                                    
                            }
                        }
                        else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        {
                            if (IsFromKit)
                            {
                                arrItem = arrIds[i].Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                            }
                            else
                            {
                                arrItem = arrIds[i].Split('_');
                            }
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                                if (!dicSerialLots.ContainsKey(arrItem[0]))
                                {
                                    dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));
                                }                                
                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            if (IsFromKit)
                            {
                                arrItem = arrIds[i].Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                            }
                            else
                            {
                                arrItem = arrIds[i].Split('_');
                            }
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                if (!dicSerialLots.ContainsKey(arrItem[0]))
                                {
                                    dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                                }                                
                            }
                        }
                        else
                        {
                            if (IsFromKit)
                            {
                                arrItem = arrIds[i].Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                            }
                            else
                            {
                                arrItem = arrIds[i].Split('_');
                            }
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                if (!dicSerialLots.ContainsKey(arrItem[0]))
                                {
                                    dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                                }                                
                            }
                        }
                    }

                    if (MaterialStagingGUID != Guid.Empty)
                    {
                        lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, false, string.Empty, IsStagginLocation, MaterialStagingGUID);
                    }
                    else
                    {
                        if (objRoomDTO != null
                               && objRoomDTO.AllowPullBeyondAvailableQty == true
                               && oItem.SerialNumberTracking == false
                               && oItem.LotNumberTracking == false
                               && oItem.DateCodeTracking == false
                               && IsStagginLocation == false)
                        {
                            retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForNegativePull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation);
                        }
                        else
                        {
                            lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, false, string.Empty, IsStagginLocation);
                        }
                    }

                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && !oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                        {
                            ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ItemGUID = ItemGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.BinNumber = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.BinNumber = objLocDTO.BinNumber;
                            }
                            if (oItem.SerialNumberTracking)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                            oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            //retlstLotSrs = retlstLotSrs.Union(lstLotSrs.Where(t =>
                            //    ((!arrSerialLots.Contains(t.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                            //|| (!arrSerialLots.Contains(t.SerialLotExpirationcombin) && oItem.DateCodeTracking)
                            //|| (!arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).Take(1)).ToList();

                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && !oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.BinNumber)
                                            && !oItem.SerialNumberTracking
                                            && !oItem.LotNumberTracking
                                            && !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();

                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;

                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                    {
                        if (MaterialStagingGUID != Guid.Empty)
                        {
                            retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation, MaterialStagingGUID);
                        }
                        else
                        {
                            if (objRoomDTO != null
                                && objRoomDTO.AllowPullBeyondAvailableQty == true
                                && oItem.SerialNumberTracking == false
                                && oItem.LotNumberTracking == false
                                && oItem.DateCodeTracking == false
                                && IsStagginLocation == false)
                            {
                                retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForNegativePull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation);
                            }
                            else
                            {
                                retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation);
                            }
                        }
                    }
                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.BinNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, false, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
            {
                if (objRoomDTO != null
                                && objRoomDTO.AllowPullBeyondAvailableQty == true
                                && oItem.SerialNumberTracking == false
                                && oItem.LotNumberTracking == false
                                && oItem.DateCodeTracking == false)
                { }
                else
                {
                    retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();
                }
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PullSerialsAndLots(ItemPullInfo objItemPullInfo)
        {
            bool isAllowPullBeyondAvailableQty = false;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,AllowPullBeyondAvailableQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            if (objRoomDTO != null && objRoomDTO.ID > 0)
            { isAllowPullBeyondAvailableQty = objRoomDTO.AllowPullBeyondAvailableQty; }

            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
            objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);

            if (isAllowPullBeyondAvailableQty
                  && objItemmasterDTO != null
                  && !objItemmasterDTO.SerialNumberTracking
                  && !objItemmasterDTO.LotNumberTracking
                  && !objItemmasterDTO.DateCodeTracking)
            {
                BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objItemPullInfo.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                {
                    objItemPullInfo.IsStatgingLocationPull = true;
                }
                string ActionType = "pull";
                if (objItemPullInfo.IsStatgingLocationPull)
                {
                    ActionType = "MS Pull";
                }
                ReqPullAllJsonResponse errMsg = null;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PullController objPullController = new PullController();
                JsonResult repsonse = objPullController.UpdatePullData(ID: 0, ItemGUID: objItemmasterDTO.GUID.ToString(), ProjectGUID: Convert.ToString(objItemPullInfo.ProjectSpendGUID), PullCreditQuantity: objItemPullInfo.PullQuantity, BinID: objItemPullInfo.BinID, PullCredit: ActionType, TempPullQTY: objItemPullInfo.PullQuantity, UDF1: objItemPullInfo.UDF1, UDF2: objItemPullInfo.UDF2, UDF3: objItemPullInfo.UDF3, UDF4: objItemPullInfo.UDF4, UDF5: objItemPullInfo.UDF5, RequisitionDetailGUID: Convert.ToString(objItemPullInfo.RequisitionDetailsGUID), WorkOrderDetailGUID: Convert.ToString(objItemPullInfo.WorkOrderDetailGUID),ICDtlGUID: null, ProjectSpendName: objItemPullInfo.ProjectSpendName, PullOrderNumber: objItemPullInfo.PullOrderNumber, SupplierAccountNumberGuid: objItemPullInfo.SupplierAccountGuid, callFrom: "multipull", PullType: 1);
                errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                objItemPullInfo.ErrorList = new List<PullErrorInfo>();

                if (errMsg.Status.ToUpper() == "FAIL")
                {
                    objItemPullInfo.ErrorMessage = errMsg.Message;
                }
                else if (!String.IsNullOrWhiteSpace(errMsg.LocationMSG))
                {
                    objItemPullInfo.ErrorMessage = errMsg.LocationMSG;
                }
            }
            else
            {
                PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
                objItemPullInfo.CompanyId = SessionHelper.CompanyID;
                objItemPullInfo.RoomId = SessionHelper.RoomID;
                objItemPullInfo.CreatedBy = SessionHelper.UserID;
                objItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                objItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                objItemPullInfo.ValidateProjectSpendLimits = true;
                objItemPullInfo.ErrorList = new List<PullErrorInfo>();
                objItemPullInfo = ValidateLotAndSerial(objItemPullInfo);
                long SessionUserId = SessionHelper.UserID;
                if (objItemPullInfo.ErrorList.Count < 1)
                {
                    objItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                    if (objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                        objItemPullInfo = objPullMasterDAL.PullItemQuantity(objItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, SessionUserId,SessionHelper.EnterPriceID);
                    else if (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                        objItemPullInfo = objPullMasterDAL.PullItemQuantity(objItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, SessionUserId,SessionHelper.EnterPriceID);
                    else
                        objItemPullInfo = objPullMasterDAL.PullItemQuantity(objItemPullInfo, 0, SessionUserId,SessionHelper.EnterPriceID);
                }
            }

            return Json(objItemPullInfo);
        }


        [HttpPost]
        public JsonResult PullSerialsAndLotsNew(List<ItemPullInfo> objItemPullInfo)
        {
            bool isAllowPullBeyondAvailableQty = false;
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,AllowPullBeyondAvailableQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            if (objRoomDTO != null && objRoomDTO.ID > 0)
            { isAllowPullBeyondAvailableQty = objRoomDTO.AllowPullBeyondAvailableQty; }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);

            bool isFromPull = true;

            var isFromWOORREQ = objItemPullInfo.Where(i => i.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || i.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
            if (isFromWOORREQ != null && isFromWOORREQ.Count > 0)
            {
                isFromPull = false;
            }

            long ModuleId = 0;
            foreach (ItemPullInfo item in objItemPullInfo)
            {
                if (item.RequisitionDetailsGUID != null && item.RequisitionDetailsGUID != Guid.Empty)
                    ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions;
                else if (item.WorkOrderDetailGUID != null && item.WorkOrderDetailGUID != Guid.Empty)
                    ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders;

                RequisitionDetailsDTO objReqDetailsDTO = null;
                if (item.RequisitionDetailsGUID != null && item.RequisitionDetailsGUID != Guid.Empty)
                {
                    objReqDetailsDTO = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRequisitionDetailsByGUIDPlain((Guid)item.RequisitionDetailsGUID);
                    if (objReqDetailsDTO != null && objReqDetailsDTO.QuantityApproved.GetValueOrDefault(0) < (item.PullQuantity + objReqDetailsDTO.QuantityPulled.GetValueOrDefault(0)))
                    {
                        item.ErrorMessage = ResPullMaster.PullQtyGreaterThanApproveQty;
                        oReturn.Add(item);
                        continue;
                    }
                }

                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                if (isAllowPullBeyondAvailableQty
                   && objItemmasterDTO != null
                   && !objItemmasterDTO.SerialNumberTracking
                   && !objItemmasterDTO.LotNumberTracking
                   && !objItemmasterDTO.DateCodeTracking)
                {
                    BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    {
                        item.IsStatgingLocationPull = true;
                    }
                    string ActionType = "pull";
                    if (item.IsStatgingLocationPull)
                    {
                        ActionType = "MS Pull";
                    }
                    ReqPullAllJsonResponse errMsg = null;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    PullController objPullController = new PullController();
                    JsonResult repsonse = objPullController.UpdatePullData(ID: 0, ItemGUID: objItemmasterDTO.GUID.ToString(), ProjectGUID: Convert.ToString(item.ProjectSpendGUID), PullCreditQuantity: item.PullQuantity, BinID: item.BinID, PullCredit: ActionType, TempPullQTY: item.PullQuantity, UDF1: item.UDF1, UDF2: item.UDF2, UDF3: item.UDF3, UDF4: item.UDF4, UDF5: item.UDF5, RequisitionDetailGUID: Convert.ToString(item.RequisitionDetailsGUID), WorkOrderDetailGUID: Convert.ToString(item.WorkOrderDetailGUID),ICDtlGUID: null, ProjectSpendName: item.ProjectSpendName, PullOrderNumber: item.PullOrderNumber, SupplierAccountNumberGuid: item.SupplierAccountGuid, callFrom: "multipull", PullType: 1,EditedSellPrice: item.PullCost);
                    errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                    item.ErrorList = new List<PullErrorInfo>();

                    if (errMsg.Status.ToUpper() == "FAIL")
                    {
                        item.ErrorMessage = errMsg.Message;
                        oReturn.Add(item);
                        continue;
                    }
                    else if (!String.IsNullOrWhiteSpace(errMsg.LocationMSG))
                    {
                        item.ErrorMessage = errMsg.LocationMSG;
                        oReturn.Add(item);
                        continue;
                    }  else
                    { oReturn.Add(item); }
                }
                else
                {
                    bool HasOnTheFlyEntryRight = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OnTheFlyEntry);
                    bool IsProjectSpendInsertAllow = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
                    if (((item.ProjectSpendGUID == null || item.ProjectSpendGUID == Guid.Empty) && !string.IsNullOrEmpty(item.ProjectSpendName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                    {
                        if (!string.IsNullOrEmpty(item.ProjectSpendName))
                        {
                            ProjectMasterDTO objProjectDTO = objPrjMsgDAL.GetProjectspendByName(item.ProjectSpendName.Trim(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                            if (objProjectDTO == null)
                            {
                                item.ErrorMessage = ResPullMaster.NoProjectspendOntheFlyRight;
                                oReturn.Add(item);
                                continue;
                            }
                        }
                        else
                        {
                            item.ErrorMessage = ResPullMaster.NoProjectspendOntheFlyRight;
                            oReturn.Add(item);
                            continue;
                        }
                    }
                    else if (((item.ProjectSpendGUID != null && item.ProjectSpendGUID != Guid.Empty) && !string.IsNullOrEmpty(item.ProjectSpendName)) && (HasOnTheFlyEntryRight == false || IsProjectSpendInsertAllow == false))
                    {

                        ProjectMasterDTO objProjectDTO = objPrjMsgDAL.GetProjectMasterByGuidNormal(item.ProjectSpendGUID.Value);
                        if (objProjectDTO != null)
                        {
                            if (objProjectDTO.ProjectSpendName.Trim().ToLower() != item.ProjectSpendName.Trim().ToLower())
                            {
                                item.ErrorMessage = ResPullMaster.NoProjectspendOntheFlyRight;
                                oReturn.Add(item);
                                continue;
                            }
                        }
                    }


                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                        if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                        {
                            //-------------------------------------Get Item Master-------------------------------------
                            //
                            //ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            //ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                            //objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                            //-----------------------------------------------------------------------------------------
                            //
                            ItemPullInfo oItemPullInfo = item;
                            oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                            oItemPullInfo.RoomId = SessionHelper.RoomID;
                            oItemPullInfo.CreatedBy = SessionHelper.UserID;
                            oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                            oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                            oItemPullInfo.ValidateProjectSpendLimits = true;
                            oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                            oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                            if (oItemPullInfo.ErrorList.Count == 0)
                            {
                                ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                                PullMasterDAL objPullMasterDAL1 = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                                BinMasterDTO objBINDTO = new BinMasterDTO();
                                BinMasterDAL objBINDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                                ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                MaterialStagingPullDetailDAL objMSPDetailsDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);

                                //-----------Project Span---------------
                                //
                                if (!(item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                        && (item.ProjectSpendName != null && item.ProjectSpendName.Trim() != ""))
                                {
                                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                                    ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectByName(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);

                                    if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                                    {
                                        item.ProjectSpendGUID = projectMaster.GUID;
                                    }
                                    else
                                    {
                                        ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                                        objProjectSpendDTO.ProjectSpendName = item.ProjectSpendName;
                                        objProjectSpendDTO.AddedFrom = "Web";
                                        objProjectSpendDTO.EditedFrom = "Web";
                                        objProjectSpendDTO.CompanyID = SessionHelper.CompanyID;
                                        objProjectSpendDTO.Room = SessionHelper.RoomID;
                                        objProjectSpendDTO.DollarLimitAmount = 0;
                                        objProjectSpendDTO.Description = string.Empty;
                                        objProjectSpendDTO.DollarUsedAmount = null;
                                        objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                                        objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objProjectSpendDTO.CreatedBy = SessionHelper.UserID;
                                        objProjectSpendDTO.LastUpdatedBy = SessionHelper.UserID;
                                        objProjectSpendDTO.UDF1 = string.Empty;
                                        objProjectSpendDTO.UDF2 = string.Empty;
                                        objProjectSpendDTO.UDF3 = string.Empty;
                                        objProjectSpendDTO.UDF4 = string.Empty;
                                        objProjectSpendDTO.UDF5 = string.Empty;
                                        objProjectSpendDTO.GUID = Guid.NewGuid();
                                        //objProjectSpendDTO.ProjectSpendItems = Guid.Parse(ItemGUID);

                                        List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                                        projectSpendItem.QuantityLimit = null;
                                        projectSpendItem.QuantityUsed = null;
                                        projectSpendItem.DollarLimitAmount = null;
                                        projectSpendItem.DollarUsedAmount = null;
                                        projectSpendItem.ItemGUID = item.ItemGUID;
                                        projectSpendItem.CreatedBy = SessionHelper.UserID;
                                        projectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                                        projectSpendItem.Room = SessionHelper.RoomID;
                                        projectSpendItem.CompanyID = SessionHelper.CompanyID;
                                        if (objItemmasterDTO != null)
                                            projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;
                                        projectSpendItem.IsArchived = false;
                                        projectSpendItem.IsDeleted = false;

                                        projectSpendItem.ProjectSpendName = item.ProjectSpendName;
                                        projectSpendItem.IsDeleted = false;
                                        projectSpendItem.IsArchived = false;
                                        projectSpendItemList.Add(projectSpendItem);

                                        objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;

                                        objProjectSpendDTO.IsDeleted = false;
                                        objProjectSpendDTO.IsArchived = false;


                                        objProjectSpendDAL.Insert(objProjectSpendDTO);
                                        projectSpendItem.ProjectGUID = objProjectSpendDTO.GUID;

                                        item.ProjectSpendGUID = objProjectSpendDTO.GUID;
                                    }
                                }

                                if (objItemmasterDTO.ItemType != 4)
                                {
                                    if (item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                    {
                                        //--------------------------------------
                                        //
                                        int IsCreditPullNothing = 2;
                                        bool IsProjectSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                                        var tmpsupplierIds = new List<long>();
                                        //ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == item.ItemGUID).SingleOrDefault();
                                        ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds, Convert.ToString(item.ItemGUID)).FirstOrDefault();
                                        ProjectMasterDTO objPrjMstDTO = objPrjMsgDAL.GetRecord(item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                                        string ItemLocationMSG = "";
                                        bool IsPSLimitExceed = false;

                                        //--------------------------------------
                                        //
                                        PullMasterViewDTO oPull = new PullMasterViewDTO();
                                        oPull.TempPullQTY = item.PullQuantity;

                                        //--------------------------------------
                                        //
                                        List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();
                                        List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = new List<ItemLocationDetailsDTO>();
                                        List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                        List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                        if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                        {
                                            double CurrentPullQuantity = 0;
                                            foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                            {
                                                string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                        : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));
                                                if (item.IsStatgingLocationPull)
                                                {
                                                    if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                        lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    else
                                                        lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                    {
                                                        foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO != null)
                                                                lstMSPDetails.Add(objItemLocationDetailsDTO);
                                                        }
                                                    }

                                                    objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                    objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                    //------------------------------------------------------------------------
                                                    //
                                                    if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                    {
                                                        //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                        double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;


                                                        foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    //objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    //PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;

                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }

                                                        foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    //objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    //PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;

                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                    objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;

                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    else
                                                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);

                                                    if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                    {
                                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO != null)
                                                                lstItemLocationDetails.Add(objItemLocationDetailsDTO);
                                                        }
                                                    }

                                                    objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                    objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                    //------------------------------------------------------------------------
                                                    //
                                                    if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                    {
                                                        //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                        double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    //objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    //PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;

                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;

                                                                }
                                                            }
                                                        }

                                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    //objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    //PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;

                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                    objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        //--------------------------------------
                                        //
                                        if (objPullMasterDAL1.ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, oPull, objItemmasterDTO, IsProjectSpendAllowed, out IsPSLimitExceed, lstItemLocationDetails,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name,SessionHelper.CompanyID,SessionHelper.RoomID))
                                        {
                                            item.ErrorMessage = ItemLocationMSG;
                                            oReturn.Add(item);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                        {
                                            List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                            List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                            List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = null;
                                            double CurrentPullQuantity = 0;
                                            foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                            {
                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                        : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));

                                                if (item.IsStatgingLocationPull)
                                                {
                                                    if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                        lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    else
                                                        lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                    {
                                                        double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                        foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }

                                                        foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                    objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    else
                                                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                    //------------------------------------------------------------------------
                                                    //
                                                    if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                    {
                                                        //TODO: Commented by CP for Pull issue, Wrong quantity pulled for normal item. on 2017-08-31
                                                        //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                        double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }

                                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                        {
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                            {
                                                                objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                {
                                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                    objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    //--------------------------------------
                                    //
                                    string ActionType1 = "Pull";
                                    if (oItemPullInfo.IsStatgingLocationPull)
                                    {
                                        ActionType1 = "MS Pull";
                                    }
                                    oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                                    long SessionUserId = SessionHelper.UserID;
                                    if (oItemPullInfo.RequisitionDetailsGUID != null && oItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                                    {
                                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, SessionUserId,SessionHelper.EnterPriceID, ActionType1);
                                    }
                                    else if (oItemPullInfo.WorkOrderDetailGUID != null && oItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                                    {
                                        bool isFromWorkOrder = true;
                                        bool AllowEditItemSellPriceonWorkOrderPull = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowEditItemSellPriceonWorkOrderPull);
                                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, SessionUserId, SessionHelper.EnterPriceID, ActionType1, AllowEditItemSellPriceonWorkOrderPull, isFromWorkOrder);
                                    }
                                    else if (oItemPullInfo.IsStatgingLocationPull)
                                    {
                                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, SessionUserId, SessionHelper.EnterPriceID, ActionType1);
                                    }
                                    else
                                    {
                                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, 0, SessionUserId, SessionHelper.EnterPriceID, ActionType1);
                                    }

                                }
                                else
                                {
                                    UpdatePullDataForLaborType(oItemPullInfo);

                                }

                                //if(ActionType1 == "Pull")
                                //{
                                //    QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                                //    objQBItemDAL.InsertQuickBookItem(oItemPullInfo.ItemGUID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", null, SessionHelper.UserID, "Web", null, "Pull");
                                //}
                            }

                            oReturn.Add(oItemPullInfo);
                        }
                        List<UDFDTO> objUDFDTO = new List<UDFDTO>();
                        UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                        objUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                        if (objUDFDTO != null && objUDFDTO.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(item.UDF1))
                            {
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == false).Any())
                                {
                                    Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().ID;
                                    if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                    {
                                        UDFController objUDFController = new UDFController();
                                        objUDFController.InsertUDFOption(UDFId, item.UDF1, "PullMaster", SessionHelper.EnterPriceID);
                                    }
                                }

                            }
                            if (!string.IsNullOrWhiteSpace(item.UDF2))
                            {
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == false).Any())
                                {
                                    Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().ID;
                                    if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                    {
                                        UDFController objUDFController = new UDFController();
                                        objUDFController.InsertUDFOption(UDFId, item.UDF2, "PullMaster", SessionHelper.EnterPriceID);
                                    }
                                }

                            }
                            if (!string.IsNullOrWhiteSpace(item.UDF3))
                            {
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == false).Any())
                                {
                                    Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().ID;
                                    if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                    {
                                        UDFController objUDFController = new UDFController();
                                        objUDFController.InsertUDFOption(UDFId, item.UDF3, "PullMaster", SessionHelper.EnterPriceID);
                                    }
                                }

                            }
                            if (!string.IsNullOrWhiteSpace(item.UDF4))
                            {
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == false).Any())
                                {
                                    Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().ID;
                                    if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                    {
                                        UDFController objUDFController = new UDFController();
                                        objUDFController.InsertUDFOption(UDFId, item.UDF4, "PullMaster", SessionHelper.EnterPriceID);
                                    }
                                }

                            }
                            if (!string.IsNullOrWhiteSpace(item.UDF5))
                            {
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == false).Any())
                                {
                                    Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().ID;
                                    if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                    {
                                        UDFController objUDFController = new UDFController();
                                        objUDFController.InsertUDFOption(UDFId, item.UDF5, "PullMaster", SessionHelper.EnterPriceID);
                                    }
                                }

                            }
                        }
                    }
                    else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        RequisitionItemsToPull obj = new RequisitionItemsToPull()
                        {
                            ToolGUID = item.ToolGUID,
                            RequisitionDetailGUID = item.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty).ToString(),
                            RequisitionMasterGUID = objReqDetailsDTO.RequisitionGUID.GetValueOrDefault(Guid.Empty).ToString(),
                            PullCreditQuantity = item.PullQuantity,
                            PullCredit = "checkout",
                            TechnicianGUID = item.TechnicianGUID,
                            TempPullQTY = item.PullQuantity,
                            ToolCheckoutUDF1 = item.ToolCheckoutUDF1,
                            ToolCheckoutUDF2 = item.ToolCheckoutUDF2,
                            ToolCheckoutUDF3 = item.ToolCheckoutUDF3,
                            ToolCheckoutUDF4 = item.ToolCheckoutUDF4,
                            ToolCheckoutUDF5 = item.ToolCheckoutUDF5,
                        };
                        JsonResult repsonse = RequisitionToolCheckout(obj);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        ReqPullAllJsonResponse errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                        ToolMasterDAL toolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        ToolMasterDTO toolDTO = toolDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                        errMsg.ItemNumber = toolDTO.ToolName;
                        if (errMsg.Message != "ok")
                        {
                            item.ErrorMessage = toolDTO.ToolName + ": " + errMsg.Message;
                            item.ItemNumber = toolDTO.ToolName;
                        }
                        else
                        {
                            item.ErrorMessage = "";
                        }
                        item.ErrorList = new List<PullErrorInfo>();
                        oReturn.Add(item);
                    }
                }
            }

            if (isFromPull)
            {
                try
                {
                    List<Guid> listpullGUIDs = oReturn.Where(p => ((p.ErrorMessage ?? string.Empty) == string.Empty || (p.ErrorMessage ?? string.Empty).ToLower() == "ok") && (p.PullGUID ?? Guid.Empty) != Guid.Empty).ToList().Select(x => x.PullGUID.GetValueOrDefault(Guid.Empty)).ToList();


                    string pullGUIDs = string.Join(",", listpullGUIDs);
                    string dataGUIDs = "<DataGuids>" + pullGUIDs + "</DataGuids>";
                    string eventName = "OPC";
                    string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstNotification != null && lstNotification.Count > 0)
                    {
                        objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                    }
                }
                catch (Exception ex)
                {

                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }

            }

            if (ModuleId == 66)
            {
                List<Guid> listpullGUIDs = oReturn.Where(p => ((p.ErrorMessage ?? string.Empty) == string.Empty || (p.ErrorMessage ?? string.Empty).ToLower() == "ok") && (p.PullGUID ?? Guid.Empty) != Guid.Empty).ToList().Select(x => x.PullGUID.GetValueOrDefault(Guid.Empty)).ToList();

                string pullGUIDs = string.Join(",", listpullGUIDs);
                string dataGUIDs = "<DataGuids>" + pullGUIDs + "</DataGuids>";
                string eventName = "ORPC";
                string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                List<NotificationDTO> lstORPCCNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (lstORPCCNotification != null && lstORPCCNotification.Count > 0)
                {
                    objNotificationDAL.SendMailForImmediate(lstORPCCNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                }
            }
            if (ModuleId == 67)
            {
                List<Guid> listpullGUIDs = oReturn.Where(p => ((p.ErrorMessage ?? string.Empty) == string.Empty || (p.ErrorMessage ?? string.Empty).ToLower() == "ok") && (p.PullGUID ?? Guid.Empty) != Guid.Empty).ToList().Select(x => x.PullGUID.GetValueOrDefault(Guid.Empty)).ToList();

                string pullGUIDs = string.Join(",", listpullGUIDs);
                string dataGUIDs = "<DataGuids>" + pullGUIDs + "</DataGuids>";
                string eventName = "OWPC";
                string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                List<NotificationDTO> lstOWPCNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (lstOWPCNotification != null && lstOWPCNotification.Count > 0)
                {
                    objNotificationDAL.SendMailForImmediate(lstOWPCNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                }
            }

            return Json(oReturn);

            //if (oReturnError.Count > 0)
            //{
            //    return Json(oReturnError);
            //}
            //else
            //{
            //    foreach (ItemPullInfo item in objItemPullInfo)
            //    {
            //        ItemPullInfo oItemPullInfo = item;
            //        //oItemPullInfo.CompanyId = SessionHelper.CompanyID;
            //        //oItemPullInfo.RoomId = SessionHelper.RoomID;
            //        //oItemPullInfo.CreatedBy = SessionHelper.UserID;
            //        //oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
            //        //oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
            //        //oItemPullInfo.ValidateProjectSpendLimits = true;
            //        //oItemPullInfo.ErrorList = new List<PullErrorInfo>();
            //        //oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);
            //        item.EnterpriseId = SessionHelper.EnterPriceID;
            //        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo);
            //        oReturn.Add(oItemPullInfo);
            //    }
            //    return Json(oReturn);
            //}
        }

        [HttpPost]
        public JsonResult PullSerialsAndLotsForCount(List<ItemPullInfo> objItemPullInfo, string PullCredit)
        {
            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            long SessionUserId = SessionHelper.UserID;
            foreach (ItemPullInfo item in objItemPullInfo)
            {
                ItemPullInfo oItemPullInfo = item;
                oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                oItemPullInfo.RoomId = SessionHelper.RoomID;
                oItemPullInfo.CreatedBy = SessionHelper.UserID;
                oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                oItemPullInfo.ValidateProjectSpendLimits = false;
                oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                oItemPullInfo = ValidateLotAndSerialForCount(oItemPullInfo);

                if (oItemPullInfo.ErrorList.Count == 0)
                {
                    oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                    if (oItemPullInfo.RequisitionDetailsGUID != null && oItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, SessionUserId, SessionHelper.EnterPriceID, PullCredit);
                    else if (oItemPullInfo.WorkOrderDetailGUID != null && oItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, SessionUserId, SessionHelper.EnterPriceID, PullCredit);
                    else
                        oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, 0, SessionUserId, SessionHelper.EnterPriceID, PullCredit);

                }

                oReturn.Add(oItemPullInfo);
            }

            return Json(oReturn);
        }

        public JsonResult GetLotOrSerailNumberList(int maxRows, string name_startsWith, Guid? ItemGuid, long BinID, string prmSerialLotNos = null, Guid? materialStagingGUID = null)
        {
            bool IsStagginLocation = false;

            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO;
            if (materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, string.Empty, IsStagginLocation, materialStagingGUID.GetValueOrDefault(Guid.Empty));
            }
            else
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForPull(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, string.Empty, IsStagginLocation);
            }

            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }
            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        private ItemPullInfo ValidateLotAndSerial(ItemPullInfo objItemPullInfo)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            eTurnsRegionInfo objRegionalSettings = new eTurnsRegionInfo();
            objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            string CurrentRoomTimeZone = "UTC";
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            double? _PullCost = null;
            if (objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                _PullCost = objItemMasterDAL.GetItemPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, (Guid)objItemPullInfo.ItemGUID, false);
            }
            else if (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
            {
                bool AllowEditItemSellPriceonWorkOrderPull = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowEditItemSellPriceonWorkOrderPull);
                if (AllowEditItemSellPriceonWorkOrderPull)
                {
                    _PullCost = objItemPullInfo.PullCost;
                }
                else
                {
                    _PullCost = objItemMasterDAL.GetItemPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, (Guid)objItemPullInfo.ItemGUID, false);
                }
            }
            if (objItem.PullQtyScanOverride && objItem.DefaultPullQuantity > 0)
            {
                if (objItemPullInfo.PullQuantity < objItem.DefaultPullQuantity || (decimal)objItemPullInfo.PullQuantity % (decimal)objItem.DefaultPullQuantity != 0)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, objItem.DefaultPullQuantity) });
                }
            }

            #region UDF validation
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            string udfRequier = string.Empty;
            
            foreach (var i in DataFromDB)
            {
                    bool UDFIsRequired = false;
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objItemPullInfo.UDF1))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objItemPullInfo.UDF2))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objItemPullInfo.UDF3))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objItemPullInfo.UDF4))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objItemPullInfo.UDF5))
                        UDFIsRequired = true;

                    if (UDFIsRequired)
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;

                        if (string.IsNullOrWhiteSpace(udfRequier))
                            udfRequier = objItem.ItemNumber + ": ";
                        udfRequier += (string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName)+ " ");
                    }
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = udfRequier });
            }
            #endregion

            #region Requisition validation
            if (objItemPullInfo.RequisitionDetailsGUID.HasValue && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                RequisitionDetailsDTO objRequisitionDetail = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRequisitionDetailsByGUIDPlain(objItemPullInfo.RequisitionDetailsGUID ?? Guid.Empty);
                //RequisitionDetail objRequisitionDetail = context.RequisitionDetails.FirstOrDefault(t => t.GUID == objItemPullInfo.RequisitionDetailsGUID);
                if (objRequisitionDetail != null)
                {
                    if (objRequisitionDetail != null && ((objRequisitionDetail.QuantityApproved ?? 0) < ((objItemPullInfo.PullQuantity) + (objRequisitionDetail.QuantityPulled ?? 0))))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "5", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgReqPullGreaterApproved });
                    }
                }
            }
            #endregion

            // RoomDTO objRoomDTO = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(objItemPullInfo.RoomId);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objItemPullInfo.RoomId.ToString() + "", "");

            #region Project Spend validation
            if (objItem != null && objItem.ItemType != 4 && objRoomDTO != null && objRoomDTO.IsProjectSpendMandatory)
            {
                if (objItemPullInfo.ProjectSpendGUID == null && !string.IsNullOrWhiteSpace(objItemPullInfo.ProjectSpendName))
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                    ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectByName(objItemPullInfo.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);

                    if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                    {
                        objItemPullInfo.ProjectSpendGUID = projectMaster.GUID;
                    }
                    else
                    {
                        ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                        objProjectSpendDTO.ProjectSpendName = objItemPullInfo.ProjectSpendName;
                        objProjectSpendDTO.AddedFrom = "Web";
                        objProjectSpendDTO.EditedFrom = "Web";
                        objProjectSpendDTO.CompanyID = SessionHelper.CompanyID;
                        objProjectSpendDTO.Room = SessionHelper.RoomID;
                        objProjectSpendDTO.DollarLimitAmount = 0;
                        objProjectSpendDTO.Description = string.Empty;
                        objProjectSpendDTO.DollarUsedAmount = null;
                        objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.CreatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.LastUpdatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.UDF1 = string.Empty;
                        objProjectSpendDTO.UDF2 = string.Empty;
                        objProjectSpendDTO.UDF3 = string.Empty;
                        objProjectSpendDTO.UDF4 = string.Empty;
                        objProjectSpendDTO.UDF5 = string.Empty;
                        objProjectSpendDTO.GUID = Guid.NewGuid();

                        //ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);

                        List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = objItemPullInfo.ItemGUID;
                        projectSpendItem.CreatedBy = SessionHelper.UserID;
                        projectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                        projectSpendItem.Room = SessionHelper.RoomID;
                        projectSpendItem.CompanyID = SessionHelper.CompanyID;
                        if (objItemmasterDTO != null)
                            projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;

                        projectSpendItem.ProjectSpendName = objItemPullInfo.ProjectSpendName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItemList.Add(projectSpendItem);

                        objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;

                        objProjectSpendDTO.IsDeleted = false;
                        objProjectSpendDTO.IsArchived = false;

                        objProjectSpendDAL.Insert(objProjectSpendDTO);
                        projectSpendItem.ProjectGUID = objProjectSpendDTO.GUID;

                        objItemPullInfo.ProjectSpendGUID = objProjectSpendDTO.GUID;
                    }
                }

                if (objItemPullInfo.ProjectSpendGUID == null)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.ProjectSpendMandatorySelectIt });
                }
            }
            #endregion

            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
            bool IsStagginLocation = false;
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objItemPullInfo.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            double AvailQty = 0;
            if (objItem.ItemType == 4)
            {
                AvailQty = objItemPullInfo.PullQuantity;
            }
            else
            {
                if (IsStagginLocation)
                {
                    List<MaterialStagingPullDetailDTO> oLocQty = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinID(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                    if (oLocQty != null)
                    {
                        AvailQty = oLocQty.Sum(x => (x.ConsignedQuantity ?? 0) + (x.CustomerOwnedQuantity ?? 0));
                    }
                }
                else
                {
                    ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                    if (oLocQty != null)
                    {
                        AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
                    }
                }
            }
            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            //double AvailQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (AvailQty >= objItemPullInfo.PullQuantity)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking)
                {
                    if (IsStagginLocation)
                    {
                        lstAvailableQty = objPullMasterDAL.GetStageLocationsByItemGuidAndBinId(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                        }
                    }
                    else
                    {
                        lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.PullQuantity, true);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                        }
                    }
                }
                else
                {
                    if (objItem.LotNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.LotNumber, string.Empty);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.LotNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.LotNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;

                                        t.IsStagingLocationLotSerial = true;
                                        t.LotNumber = lstLotQty.LotNumber;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                            else
                            {
                                List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.LotNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.LotNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }
                    }
                    else if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;

                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, string.Empty, t.SerialNumber);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.SerialNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.SerialNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;

                                        t.IsStagingLocationLotSerial = true;
                                        t.SerialNumber = lstLotQty.LotNumber;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                            else
                            {
                                List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.SerialNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.SerialNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }

                    }
                    else if (objItem.DateCodeTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                DateTime CurrentDate = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                                DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                                if (objItemPullInfo.isValidateExpiredItem && ExpirationDateUTC.Date < CurrentDate.Date)
                                {   
                                    t.ValidationMessage = string.Format(ResPullMaster.ItemForExpirationDateExpired, objItem.ItemNumber, t.Expiration);
                                }
                                else
                                {
                                    List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMSLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, ExpirationDateUTC);
                                    if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                    {
                                        var lstLotQty = (from il in objItemLocationDetail
                                                         group il by new { il.ExpirationDate } into grpms
                                                         select new
                                                         {
                                                             CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                             ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                             ExpirationDate = grpms.Key.ExpirationDate,
                                                         }).FirstOrDefault();

                                        if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                        {
                                            t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                        }
                                        else
                                        {
                                            if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                                t.CustomerOwnedQuantity = t.PullQuantity;
                                            else
                                                t.ConsignedQuantity = t.PullQuantity;

                                            t.IsStagingLocationLotSerial = true;
                                            t.ExpirationDate = lstLotQty.ExpirationDate;
                                        }
                                    }
                                    else
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                    }
                                }
                            }
                            else
                            {
                                DateTime CurrentDate = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                                DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));

                                if (objItemPullInfo.isValidateExpiredItem && ExpirationDateUTC.Date < CurrentDate.Date)
                                {                                    
                                    t.ValidationMessage = string.Format(ResPullMaster.ItemForExpirationDateExpired, objItem.ItemNumber, t.Expiration);
                                }
                                else
                                {
                                    List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, ExpirationDateUTC);
                                    if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                    {
                                        var lstLotQty = (from il in objItemLocationDetail
                                                         group il by new { il.ExpirationDate } into grpms
                                                         select new
                                                         {
                                                             CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                             ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                             ExpirationDate = grpms.Key.ExpirationDate,
                                                         }).FirstOrDefault();

                                        if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                        {
                                            t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                        }
                                        else
                                        {
                                            if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                                t.CustomerOwnedQuantity = t.PullQuantity;
                                            else
                                                t.ConsignedQuantity = t.PullQuantity;
                                            t.ExpirationDate = lstLotQty.ExpirationDate;
                                        }
                                    }
                                    else
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                    }
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty)
                            && lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty).Contains(string.Format(ResPullMaster.ItemForExpirationDateExpired, objItem.ItemNumber, t.Expiration)))) 
                        {
                            foreach (ItemLocationLotSerialDTO itm in lstAvailableQty.Where(t => (t.ValidationMessage ?? string.Empty).Contains(string.Format(ResPullMaster.ItemForExpirationDateExpired, objItem.ItemNumber, t.Expiration))).ToList())
                            {
                                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = itm.ValidationMessage });
                            }
                        }
                        else if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }
                    }
                }
            }
            else
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
            }
            if (IsStagginLocation)
            {
                objItemPullInfo.IsStatgingLocationPull = true;
            }
            return objItemPullInfo;
        }

        private ItemPullInfo ValidateLotAndSerialForCount(ItemPullInfo objItemPullInfo)
        {
            ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);

            #region UDF validation
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            string udfRequier = string.Empty;
            foreach (var i in DataFromDB)
            {
                    bool UDFIsRequired = false;
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objItemPullInfo.UDF1))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objItemPullInfo.UDF2))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objItemPullInfo.UDF3))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objItemPullInfo.UDF4))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objItemPullInfo.UDF5))
                        UDFIsRequired = true;

                    if (UDFIsRequired)
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;

                        if (string.IsNullOrWhiteSpace(udfRequier))
                            udfRequier = objItem.ItemNumber + ": ";
                        udfRequier += (string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName) + " ");
                    }
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = udfRequier });
            }
            #endregion

            #region Requisition validation
            if (objItemPullInfo.RequisitionDetailsGUID.HasValue && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                RequisitionDetailsDTO objRequisitionDetail = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRequisitionDetailsByGUIDPlain(objItemPullInfo.RequisitionDetailsGUID ?? Guid.Empty);
                //RequisitionDetail objRequisitionDetail = context.RequisitionDetails.FirstOrDefault(t => t.GUID == objItemPullInfo.RequisitionDetailsGUID);
                if (objRequisitionDetail != null)
                {
                    if (objRequisitionDetail != null && ((objRequisitionDetail.QuantityApproved ?? 0) < ((objItemPullInfo.PullQuantity) + (objRequisitionDetail.QuantityPulled ?? 0))))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "5", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgReqPullGreaterApproved });
                    }
                }
            }
            #endregion




            double TotalPulled = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);

            double AvailQty = 0;
            ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            if (oLocQty != null)
            {
                AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
            }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            //InventoryCountDAL oInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
            //double AvailQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (AvailQty >= objItemPullInfo.PullQuantity)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking)
                {
                    double ConsignedTakenTotal = 0;
                    double CustownedTakenTotal = 0;
                    lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.PullQuantity, true);
                    lstAvailableQty.ForEach(il =>
                    {
                        double ConsignedDiff = objItemPullInfo.TotalConsignedTobePulled - ConsignedTakenTotal;
                        double CustownedDiff = objItemPullInfo.TotalCustomerOwnedTobePulled - CustownedTakenTotal;
                        ConsignedTaken = 0;
                        CustownedTaken = 0;

                        if (ConsignedDiff > 0)
                        {
                            if (ConsignedDiff <= (il.ConsignedQuantity ?? 0))
                                ConsignedTaken = ConsignedDiff;
                            else
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                            ConsignedTakenTotal += ConsignedTaken;
                        }

                        if (CustownedDiff > 0)
                        {
                            if (objItemPullInfo.TotalCustomerOwnedTobePulled <= (il.CustomerOwnedQuantity ?? 0))
                                CustownedTaken = objItemPullInfo.TotalCustomerOwnedTobePulled;
                            else
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            CustownedTakenTotal += CustownedTaken;
                        }

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (il.Cost ?? 0));

                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));
                    });

                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                }
                else
                {
                    if (objItem.LotNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                 }).FirstOrDefault();

                                if ((t.PullQuantity) > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    //if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) > 0)
                                    //    t.CustomerOwnedQuantity = t.PullQuantity;
                                    //else
                                    //    t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (il.Cost ?? 0));

                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        }
                    }
                    else if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    //if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) > 0)
                                    //    t.CustomerOwnedQuantity = t.PullQuantity;
                                    //else
                                    //    t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (il.Cost ?? 0));

                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }

                    }
                    else if (objItem.DateCodeTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, ExpirationDateUTC);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     ExpirationDate = grpms.Key.ExpirationDate
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                    //if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) > 0)
                                    //    t.CustomerOwnedQuantity = t.PullQuantity;
                                    //else
                                    //    t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (il.Cost ?? 0));

                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }

                    }
                }
            }
            else
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
            }
            return objItemPullInfo;
        }



        private ItemPullInfo UpdatePullDataForLaborType(ItemPullInfo objItemPullInfo)
        {
            bool IsPSLimitExceed = false;
            long ModuleId = 0;
            PullMasterDAL obj = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            PullMasterViewDTO objDTO = new PullMasterViewDTO();
            objDTO.ID = 0;
            objDTO.ItemGUID = objItemPullInfo.ItemGUID; //Guid.Parse(ItemGUID);

            if (objItemPullInfo.BinID > 0)
                objDTO.BinID = objItemPullInfo.BinID;

            objDTO.PoolQuantity = objItemPullInfo.PullQuantity;
            objDTO.TempPullQTY = objItemPullInfo.PullQuantity;//TempPullQTY;
            objDTO.RequisitionDetailGUID = objItemPullInfo.RequisitionDetailsGUID; //as of now
            objDTO.WorkOrderDetailGUID = objItemPullInfo.WorkOrderDetailGUID; //as of now
            objDTO.CountLineItemGuid = null; //as of now
            objDTO.PullOrderNumber = objItemPullInfo.PullOrderNumber;
            objDTO.UDF1 = objItemPullInfo.UDF1;
            objDTO.UDF2 = objItemPullInfo.UDF2;
            objDTO.UDF3 = objItemPullInfo.UDF3;
            objDTO.UDF4 = objItemPullInfo.UDF4;
            objDTO.UDF5 = objItemPullInfo.UDF5;
            objDTO.SupplierID = (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any()) ? SessionHelper.UserSupplierIds[0] : 0;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.PullCredit = "pull";
            ModuleId = 0;
            objDTO.ProjectSpendGUID = null; //as of now

            int IsCreditPullNothing = 2;
            #region "Check Project Spend Condition"
            bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
            #endregion

            string ItemLocationMSG = "";

            if (objDTO.RequisitionDetailGUID != null && objDTO.RequisitionDetailGUID != Guid.Empty)
                ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions;
            else if (objDTO.WorkOrderDetailGUID != null && objDTO.WorkOrderDetailGUID != Guid.Empty)
                ModuleId = (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders;

            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            obj.UpdatePullData(objDTO, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, ModuleId, out ItemLocationMSG, IsProjecSpendAllowed, out IsPSLimitExceed, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowNegetive: false);


            bool isError = false;
            if (ItemLocationMSG != "")
            {
                isError = true;
            }
            if (IsPSLimitExceed)
            {
                isError = true;
            }
            if (isError)
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = ItemLocationMSG });
            }
            else
            {
                ItemMasterDAL objItemMaster = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                objItemMaster.EditDate(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), "EditPulledDate");
            }

            return objItemPullInfo;
        }

        #region WI-5451 Pull Import for Serial Lot and Expiration 

        private ItemPullInfo ValidateLotAndSerialForImport(ItemPullInfo objItemPullInfo)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            double? _PullCost = null;
            if (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
            {
                bool AllowEditItemSellPriceonWorkOrderPull = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowEditItemSellPriceonWorkOrderPull);
                if (AllowEditItemSellPriceonWorkOrderPull && objItemPullInfo.PullCost > 0)
                {
                    _PullCost = objItemPullInfo.PullCost;
                }
                else
                {
                    _PullCost = objItemMasterDAL.GetItemPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, (Guid)objItemPullInfo.ItemGUID, false);
                }
            }

            if (objItem.PullQtyScanOverride && objItem.DefaultPullQuantity > 0)
            {
                if (objItemPullInfo.PullQuantity < objItem.DefaultPullQuantity || (decimal)objItemPullInfo.PullQuantity % (decimal)objItem.DefaultPullQuantity != 0)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, objItem.DefaultPullQuantity) });
                }
            }

            #region UDF validation
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            string udfRequier = string.Empty;
            foreach (var i in DataFromDB)
            {
                    bool UDFIsRequired = false;
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objItemPullInfo.UDF1))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objItemPullInfo.UDF2))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objItemPullInfo.UDF3))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objItemPullInfo.UDF4))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objItemPullInfo.UDF5))
                        UDFIsRequired = true;

                    if (UDFIsRequired)
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;

                        if (string.IsNullOrWhiteSpace(udfRequier))
                            udfRequier = objItem.ItemNumber + ": ";
                        udfRequier += (string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName) + " ");
                    }
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "2", ErrorMessage = udfRequier });
                foreach (ItemLocationLotSerialDTO obj in objItemPullInfo.lstItemPullDetails)
                {
                    obj.ValidationMessage = udfRequier;
                }
            }
            #endregion

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objItemPullInfo.RoomId.ToString() + "", "");

            #region Project Spend validation
            if (objItem != null && objItem.ItemType != 4 && objRoomDTO != null && objRoomDTO.IsProjectSpendMandatory)
            {
                if (objItemPullInfo.ProjectSpendGUID == null && !string.IsNullOrWhiteSpace(objItemPullInfo.ProjectSpendName))
                {
                    ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                    ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectByName(objItemPullInfo.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);

                    if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                    {
                        objItemPullInfo.ProjectSpendGUID = projectMaster.GUID;
                    }
                    else
                    {
                        ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                        objProjectSpendDTO.ProjectSpendName = objItemPullInfo.ProjectSpendName;
                        objProjectSpendDTO.AddedFrom = "Web";
                        objProjectSpendDTO.EditedFrom = "Web";
                        objProjectSpendDTO.CompanyID = SessionHelper.CompanyID;
                        objProjectSpendDTO.Room = SessionHelper.RoomID;
                        objProjectSpendDTO.DollarLimitAmount = 0;
                        objProjectSpendDTO.Description = string.Empty;
                        objProjectSpendDTO.DollarUsedAmount = null;
                        objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                        objProjectSpendDTO.CreatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.LastUpdatedBy = SessionHelper.UserID;
                        objProjectSpendDTO.UDF1 = string.Empty;
                        objProjectSpendDTO.UDF2 = string.Empty;
                        objProjectSpendDTO.UDF3 = string.Empty;
                        objProjectSpendDTO.UDF4 = string.Empty;
                        objProjectSpendDTO.UDF5 = string.Empty;
                        objProjectSpendDTO.GUID = Guid.NewGuid();

                        //ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);

                        List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                        ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                        projectSpendItem.QuantityLimit = null;
                        projectSpendItem.QuantityUsed = null;
                        projectSpendItem.DollarLimitAmount = null;
                        projectSpendItem.DollarUsedAmount = null;
                        projectSpendItem.ItemGUID = objItemPullInfo.ItemGUID;
                        projectSpendItem.CreatedBy = SessionHelper.UserID;
                        projectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                        projectSpendItem.Room = SessionHelper.RoomID;
                        projectSpendItem.CompanyID = SessionHelper.CompanyID;
                        if (objItemmasterDTO != null)
                            projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;
                        projectSpendItem.IsArchived = false;
                        projectSpendItem.IsDeleted = false;

                        projectSpendItem.ProjectSpendName = objItemPullInfo.ProjectSpendName;
                        projectSpendItem.IsDeleted = false;
                        projectSpendItem.IsArchived = false;
                        projectSpendItemList.Add(projectSpendItem);

                        objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;

                        objProjectSpendDTO.IsDeleted = false;
                        objProjectSpendDTO.IsArchived = false;

                        objProjectSpendDAL.Insert(objProjectSpendDTO);
                        projectSpendItem.ProjectGUID = objProjectSpendDTO.GUID;

                        objItemPullInfo.ProjectSpendGUID = objProjectSpendDTO.GUID;
                    }
                }

                if (objItemPullInfo.ProjectSpendGUID == null)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "3", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.ProjectSpendMandatorySelectIt });
                }
            }
            #endregion

            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
            bool IsStagginLocation = false;
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objItemPullInfo.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();

            #region General Item

            if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking)
            {
                double AvailQty = 0;
                if (objItem.ItemType == 4)
                {
                    AvailQty = objItemPullInfo.PullQuantity;
                }
                else
                {
                    if (IsStagginLocation)
                    {
                        List<MaterialStagingPullDetailDTO> oLocQty = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinID(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        if (oLocQty != null)
                        {
                            AvailQty = oLocQty.Sum(x => (x.ConsignedQuantity ?? 0) + (x.CustomerOwnedQuantity ?? 0));
                        }
                    }
                    else
                    {
                        ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        if (oLocQty != null)
                        {
                            AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
                        }
                    }
                }

                if (AvailQty >= objItemPullInfo.PullQuantity)
                {
                    if (IsStagginLocation)
                    {
                        lstAvailableQty = objPullMasterDAL.GetStageLocationsByItemGuidAndBinId(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                        }
                    }
                    else
                    {
                        lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.PullQuantity, true);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                        }
                    }
                }
                else
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "7", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
                    foreach (ItemLocationLotSerialDTO obj in objItemPullInfo.lstItemPullDetails)
                    {
                        obj.ValidationMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable;
                    }
                }
            }

            #endregion
            #region Lot Serial DateCode Item
            else
            {
                #region LotNumberTracking + DateCodeTracking

                if (objItem.LotNumberTracking && objItem.DateCodeTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                    lstAvailableQty.ForEach(t =>
                    {
                        if (IsStagginLocation)
                        {
                            DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.strExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                            List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetStagingSerLotQtyForImport(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.LotNumber, string.Empty, ExpirationDateUTC);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;

                                    t.IsStagingLocationLotSerial = true;
                                    t.LotNumber = lstLotQty.LotNumber;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQtyForImport(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.strExpirationDate);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber, il.Expiration } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                     ExpirationDate = grpms.Key.Expiration
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "5", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                    }
                    //else
                    //{
                    lstAvailableQty.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList().ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                    });
                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty; //.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList();
                    if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                    {
                        objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                    }
                    //}
                }

                #endregion

                #region SerialNumberTracking  + DateCodeTracking

                else if (objItem.SerialNumberTracking && objItem.DateCodeTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;

                    lstAvailableQty.ForEach(t =>
                    {
                        if (IsStagginLocation)
                        {
                            DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.strExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                            List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetStagingSerLotQtyForImport(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, string.Empty, t.SerialNumber, ExpirationDateUTC);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;

                                    t.IsStagingLocationLotSerial = true;
                                    t.SerialNumber = lstLotQty.LotNumber;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQtyForImport(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.strExpirationDate);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber, il.Expiration } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                     SerialNumber = grpms.Key.SerialNumber,
                                                     ExpirationDate = grpms.Key.Expiration,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                    }
                    //else
                    //{
                    lstAvailableQty.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList().ForEach(il =>
                    {

                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                    });
                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty; //.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList();
                    if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                    {
                        objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                    }
                    //}

                }

                #endregion

                #region LotNumberTracking

                else if (objItem.LotNumberTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                    lstAvailableQty.ForEach(t =>
                    {
                        if (IsStagginLocation)
                        {
                            List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.LotNumber, string.Empty);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;

                                    t.IsStagingLocationLotSerial = true;
                                    t.LotNumber = lstLotQty.LotNumber;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "5", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                    }
                    //else
                    //{
                    lstAvailableQty.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList().ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                    });
                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty; //.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList();
                    if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                    {
                        objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                    }
                    //}
                }

                #endregion

                #region SerialNumberTracking

                else if (objItem.SerialNumberTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;

                    lstAvailableQty.ForEach(t =>
                    {
                        if (IsStagginLocation)
                        {
                            List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, string.Empty, t.SerialNumber);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;

                                    t.IsStagingLocationLotSerial = true;
                                    t.SerialNumber = lstLotQty.LotNumber;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                     SerialNumber = grpms.Key.SerialNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                    }
                    //else
                    //{
                    lstAvailableQty.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList().ForEach(il =>
                    {

                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                    });
                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty; //.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList();
                    if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                    {
                        objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                    }
                }

                #endregion

                #region DateCodeTracking

                else if (objItem.DateCodeTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                    lstAvailableQty.ForEach(t =>
                    {
                        if (IsStagginLocation)
                        {
                            DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.strExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                            List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMSLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, ExpirationDateUTC);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     ExpirationDate = grpms.Key.ExpirationDate,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;

                                    t.IsStagingLocationLotSerial = true;
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQtyForImport(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.strExpirationDate);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     ExpirationDate = grpms.Key.ExpirationDate
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }
                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "5", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                    }
                    //else
                    //{
                    lstAvailableQty.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList().ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                    });
                    objItemPullInfo.PullCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty; //.Where(x => (x.ValidationMessage ?? string.Empty) == string.Empty).ToList();
                    if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                    {
                        objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpendForImport(objItemPullInfo);
                    }
                    //}
                }

                #endregion
            }

            #endregion

            if (IsStagginLocation)
            {
                objItemPullInfo.IsStatgingLocationPull = true;
            }
            return objItemPullInfo;
        }

        [HttpPost]
        public JsonResult PullSerialsAndLotsNewForImport(List<ItemPullLotSerialInfo> lstPullImportWithLotSerial)
        {
            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            List<ItemPullLotSerialInfo> oReturnError = new List<ItemPullLotSerialInfo>();

            List<ItemPullInfo> objItemPullInfo = (from pull in lstPullImportWithLotSerial
                                                  group pull by new
                                                  {
                                                      BinID = pull.BinID,
                                                      ItemNumber = pull.ItemNumber,
                                                      BinNumber = pull.BinNumber,
                                                      ItemGUID = pull.ItemGUID,
                                                      ItemID = pull.ItemID,
                                                      UDF1 = pull.UDF1,
                                                      UDF2 = pull.UDF2,
                                                      UDF3 = pull.UDF3,
                                                      UDF4 = pull.UDF4,
                                                      UDF5 = pull.UDF5,
                                                      ProjectSpendName = pull.ProjectSpendName.Trim(),
                                                      PullOrderNumber = pull.PullOrderNumber.Trim(),
                                                      WorkOrderDetailGUID = pull.WorkOrderDetailGUID,
                                                      PullCost = pull.PullCost
                                                  } into newGroup
                                                  select new ItemPullInfo
                                                  {
                                                      BinID = newGroup.Key.BinID,
                                                      ItemNumber = newGroup.Key.ItemNumber,
                                                      ItemGUID = newGroup.Key.ItemGUID,
                                                      ItemID = newGroup.Key.ItemID,
                                                      PullQuantity = newGroup.Sum(x => x.PullQuantity),
                                                      BinNumber = newGroup.Key.BinNumber,
                                                      UDF1 = newGroup.Key.UDF1,
                                                      UDF2 = newGroup.Key.UDF2,
                                                      UDF3 = newGroup.Key.UDF3,
                                                      UDF4 = newGroup.Key.UDF4,
                                                      UDF5 = newGroup.Key.UDF5,
                                                      ProjectSpendName = newGroup.Key.ProjectSpendName.Trim(),
                                                      PullOrderNumber = newGroup.Key.PullOrderNumber.Trim(),
                                                      WorkOrderDetailGUID = newGroup.Key.WorkOrderDetailGUID,
                                                      PullCost = newGroup.Key.PullCost
                                                  }).ToList();

            foreach (ItemPullInfo ItemPullInfo in objItemPullInfo)
            {
                List<ItemLocationLotSerialDTO> lstpullDetails = new List<ItemLocationLotSerialDTO>();
                lstpullDetails = (from pull in lstPullImportWithLotSerial
                                  where pull.ItemNumber == ItemPullInfo.ItemNumber
                                        && pull.BinNumber == ItemPullInfo.BinNumber
                                        && pull.UDF1 == ItemPullInfo.UDF1
                                        && pull.UDF2 == ItemPullInfo.UDF2
                                        && pull.UDF3 == ItemPullInfo.UDF3
                                        && pull.UDF4 == ItemPullInfo.UDF4
                                        && pull.UDF5 == ItemPullInfo.UDF5
                                        && pull.ProjectSpendName == ItemPullInfo.ProjectSpendName
                                        && pull.PullOrderNumber == ItemPullInfo.PullOrderNumber
                                        && pull.WorkOrderDetailGUID == ItemPullInfo.WorkOrderDetailGUID
                                  select new ItemLocationLotSerialDTO
                                  {
                                      ItemNumber = pull.ItemNumber,
                                      ItemGUID = pull.ItemGUID,
                                      ID = pull.ItemID,
                                      PullQuantity = pull.PullQuantity,
                                      BinID = pull.BinID,
                                      BinNumber = pull.BinNumber,
                                      DateCodeTracking = pull.DateCodeTracking,
                                      SerialNumberTracking = pull.SerialNumberTracking,
                                      LotNumberTracking = pull.LotNumberTracking,
                                      SerialNumber = pull.SerialNumber,
                                      LotNumber = pull.LotNumber,
                                      LotOrSerailNumber = pull.LotOrSerailNumber,
                                      ExpirationDate = pull.ExpirationDate,
                                      strExpirationDate = pull.strExpirationDate,
                                      SerialLotExpirationcombin = pull.SerialLotExpirationcombin,
                                      PullCost = pull.PullCost
                                  }).ToList();

                ItemPullInfo.lstItemPullDetails = new List<ItemLocationLotSerialDTO>();
                ItemPullInfo.lstItemPullDetails.AddRange(lstpullDetails);
            }

            bool isFromPull = true;

            var isFromWOORREQ = objItemPullInfo.Where(i => i.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || i.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
            if (isFromWOORREQ != null && isFromWOORREQ.Count > 0)
            {
                isFromPull = false;
            }

            foreach (ItemPullInfo item in objItemPullInfo)
            {

                RequisitionDetailsDTO objReqDetailsDTO = null;
                //if (item.RequisitionDetailsGUID != null && item.RequisitionDetailsGUID != Guid.Empty)
                //{
                //    objReqDetailsDTO = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRequisitionDetailsByGUIDPlain((Guid)item.RequisitionDetailsGUID);
                //    if (objReqDetailsDTO != null && objReqDetailsDTO.QuantityApproved.GetValueOrDefault(0) < (item.PullQuantity + objReqDetailsDTO.QuantityPulled.GetValueOrDefault(0)))
                //    {
                //        item.ErrorMessage = "Pull quantity is greater than approve quantinty.";
                //        oReturn.Add(item);
                //        oReturnError.Add(item);
                //        continue;
                //    }
                //}

                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        //-------------------------------------Get Item Master-------------------------------------
                        //
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                        //-----------------------------------------------------------------------------------------
                        //
                        ItemPullInfo oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        oItemPullInfo.ValidateProjectSpendLimits = true;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerialForImport(oItemPullInfo);

                        ItemPullLotSerialInfo errorDetaillist = new ItemPullLotSerialInfo();
                        errorDetaillist.ItemNumber = oItemPullInfo.ItemNumber;
                        errorDetaillist.BinNumber = oItemPullInfo.BinNumber;
                        errorDetaillist.ProjectSpendName = oItemPullInfo.ProjectSpendName;
                        errorDetaillist.PullOrderNumber = oItemPullInfo.PullOrderNumber;
                        errorDetaillist.UDF1 = oItemPullInfo.UDF1;
                        errorDetaillist.UDF2 = oItemPullInfo.UDF2;
                        errorDetaillist.UDF3 = oItemPullInfo.UDF3;
                        errorDetaillist.UDF4 = oItemPullInfo.UDF4;
                        errorDetaillist.UDF5 = oItemPullInfo.UDF5;
                        errorDetaillist.WorkOrderDetailGUID = oItemPullInfo.WorkOrderDetailGUID;
                        errorDetaillist.ErrorList = new List<PullErrorInfo>();
                        errorDetaillist.lstItemPullDetails = new List<ItemLocationLotSerialDTO>();
                        errorDetaillist.ErrorList = oItemPullInfo.ErrorList;
                        errorDetaillist.lstItemPullDetails = oItemPullInfo.lstItemPullDetails.Where(x => !string.IsNullOrWhiteSpace(x.ValidationMessage)).ToList();
                        oReturnError.Add(errorDetaillist);

                        bool isvalidtoPull = true;
                        if (oItemPullInfo.ErrorList.Count > 0)
                        {
                            if (oItemPullInfo.ErrorList.Where(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)
                                 &&
                                 (x.ErrorCode == "1" || x.ErrorCode == "2"
                                 || x.ErrorCode == "3" || x.ErrorCode == "7")
                                ).ToList().Count() > 0)
                            {
                                isvalidtoPull = false;
                            }
                        }

                        if (isvalidtoPull && oItemPullInfo.ErrorList.Count > 0)
                        {
                            oItemPullInfo.lstItemPullDetails = item.lstItemPullDetails = oItemPullInfo.lstItemPullDetails.Where(x => string.IsNullOrWhiteSpace(x.ValidationMessage)).ToList();

                            if (oItemPullInfo != null
                                && oItemPullInfo.lstItemPullDetails != null
                                && oItemPullInfo.lstItemPullDetails.Count > 0)
                            {
                                oItemPullInfo.PullQuantity = item.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);
                            }
                            else
                            {
                                oItemPullInfo.PullQuantity = item.PullQuantity = 0;
                            }
                        }

                        if (isvalidtoPull && oItemPullInfo != null
                            && oItemPullInfo.lstItemPullDetails != null
                            && oItemPullInfo.lstItemPullDetails.Count > 0)
                        {
                            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                            PullMasterDAL objPullMasterDAL1 = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                            BinMasterDTO objBINDTO = new BinMasterDTO();
                            BinMasterDAL objBINDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                            MaterialStagingPullDetailDAL objMSPDetailsDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);

                            //-----------Project Span---------------
                            //
                            if (!(item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                    && (item.ProjectSpendName != null && item.ProjectSpendName.Trim() != ""))
                            {
                                ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                                ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectByName(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);

                                if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                                {
                                    item.ProjectSpendGUID = projectMaster.GUID;
                                }
                                else
                                {
                                    ProjectMasterDTO objProjectSpendDTO = new ProjectMasterDTO();
                                    objProjectSpendDTO.ProjectSpendName = item.ProjectSpendName;
                                    objProjectSpendDTO.AddedFrom = "Web";
                                    objProjectSpendDTO.EditedFrom = "Web";
                                    objProjectSpendDTO.CompanyID = SessionHelper.CompanyID;
                                    objProjectSpendDTO.Room = SessionHelper.RoomID;
                                    objProjectSpendDTO.DollarLimitAmount = 0;
                                    objProjectSpendDTO.Description = string.Empty;
                                    objProjectSpendDTO.DollarUsedAmount = null;
                                    objProjectSpendDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objProjectSpendDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objProjectSpendDTO.Created = DateTimeUtility.DateTimeNow;
                                    objProjectSpendDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objProjectSpendDTO.CreatedBy = SessionHelper.UserID;
                                    objProjectSpendDTO.LastUpdatedBy = SessionHelper.UserID;
                                    objProjectSpendDTO.UDF1 = string.Empty;
                                    objProjectSpendDTO.UDF2 = string.Empty;
                                    objProjectSpendDTO.UDF3 = string.Empty;
                                    objProjectSpendDTO.UDF4 = string.Empty;
                                    objProjectSpendDTO.UDF5 = string.Empty;
                                    objProjectSpendDTO.GUID = Guid.NewGuid();
                                    //objProjectSpendDTO.ProjectSpendItems = Guid.Parse(ItemGUID);

                                    List<ProjectSpendItemsDTO> projectSpendItemList = new List<ProjectSpendItemsDTO>();
                                    ProjectSpendItemsDTO projectSpendItem = new ProjectSpendItemsDTO();
                                    projectSpendItem.QuantityLimit = null;
                                    projectSpendItem.QuantityUsed = null;
                                    projectSpendItem.DollarLimitAmount = null;
                                    projectSpendItem.DollarUsedAmount = null;
                                    projectSpendItem.ItemGUID = item.ItemGUID;
                                    projectSpendItem.CreatedBy = SessionHelper.UserID;
                                    projectSpendItem.LastUpdatedBy = SessionHelper.UserID;
                                    projectSpendItem.Room = SessionHelper.RoomID;
                                    projectSpendItem.CompanyID = SessionHelper.CompanyID;
                                    if (objItemmasterDTO != null)
                                        projectSpendItem.ItemNumber = objItemmasterDTO.ItemNumber;
                                    projectSpendItem.IsArchived = false;
                                    projectSpendItem.IsDeleted = false;

                                    projectSpendItem.ProjectSpendName = item.ProjectSpendName;
                                    projectSpendItem.IsDeleted = false;
                                    projectSpendItem.IsArchived = false;
                                    projectSpendItemList.Add(projectSpendItem);

                                    objProjectSpendDTO.ProjectSpendItems = projectSpendItemList;

                                    objProjectSpendDTO.IsDeleted = false;
                                    objProjectSpendDTO.IsArchived = false;


                                    objProjectSpendDAL.Insert(objProjectSpendDTO);
                                    projectSpendItem.ProjectGUID = objProjectSpendDTO.GUID;

                                    item.ProjectSpendGUID = objProjectSpendDTO.GUID;
                                }
                            }

                            if (objItemmasterDTO.ItemType != 4)
                            {
                                if (item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                {
                                    //--------------------------------------
                                    //
                                    int IsCreditPullNothing = 2;
                                    bool IsProjectSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                                    var tmpsupplierIds = new List<long>();
                                    //ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == item.ItemGUID).SingleOrDefault();
                                    ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds, Convert.ToString(item.ItemGUID)).FirstOrDefault();
                                    ProjectMasterDTO objPrjMstDTO = objPrjMsgDAL.GetRecord(item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                                    string ItemLocationMSG = "";
                                    bool IsPSLimitExceed = false;

                                    //--------------------------------------
                                    //
                                    PullMasterViewDTO oPull = new PullMasterViewDTO();
                                    oPull.TempPullQTY = item.PullQuantity;

                                    //--------------------------------------
                                    //
                                    List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();
                                    List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = new List<ItemLocationDetailsDTO>();
                                    List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                    List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                    {
                                        double CurrentPullQuantity = 0;
                                        foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                        {
                                            string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                    : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));
                                            if (item.IsStatgingLocationPull)
                                            {
                                                if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                    lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                else
                                                    lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                {
                                                    foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO != null)
                                                            lstMSPDetails.Add(objItemLocationDetailsDTO);
                                                    }
                                                }

                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                //------------------------------------------------------------------------
                                                //
                                                if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                {
                                                    //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                    double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;


                                                    foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                            {
                                                                //objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                //PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;

                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }

                                                    foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                            {
                                                                //objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                //PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;

                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;

                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                    lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                else
                                                    lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);

                                                if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                {
                                                    foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO != null)
                                                            lstItemLocationDetails.Add(objItemLocationDetailsDTO);
                                                    }
                                                }

                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                //------------------------------------------------------------------------
                                                //
                                                if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                {
                                                    //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                    double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                    foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                            {
                                                                //objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                //PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;

                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;

                                                            }
                                                        }
                                                    }

                                                    foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                            {
                                                                //objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                //PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;

                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //--------------------------------------
                                    //
                                    if (objPullMasterDAL1.ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, oPull, objItemmasterDTO, IsProjectSpendAllowed, out IsPSLimitExceed, lstItemLocationDetails, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, SessionHelper.CompanyID, SessionHelper.RoomID))
                                    {
                                        item.ErrorMessage = ItemLocationMSG;
                                        oReturn.Add(item);

                                        ItemPullLotSerialInfo Detaillist = new ItemPullLotSerialInfo();
                                        Detaillist.ItemNumber = item.ItemNumber;
                                        Detaillist.BinNumber = item.BinNumber;
                                        Detaillist.ProjectSpendName = item.ProjectSpendName;
                                        Detaillist.UDF1 = item.UDF1;
                                        Detaillist.UDF2 = item.UDF2;
                                        Detaillist.UDF3 = item.UDF3;
                                        Detaillist.UDF4 = item.UDF4;
                                        Detaillist.UDF5 = item.UDF5;
                                        Detaillist.PullOrderNumber = item.PullOrderNumber;
                                        Detaillist.WorkOrderDetailGUID = item.WorkOrderDetailGUID;
                                        Detaillist.ErrorList = new List<PullErrorInfo>();
                                        Detaillist.lstItemPullDetails = new List<ItemLocationLotSerialDTO>();
                                        Detaillist.ErrorList = item.ErrorList;
                                        Detaillist.lstItemPullDetails = item.lstItemPullDetails;
                                        oReturnError.Add(Detaillist);

                                        //oReturnError.Add((item);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                    {
                                        List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                        List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                        List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = null;
                                        double CurrentPullQuantity = 0;
                                        foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                        {
                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                            objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                            objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                            string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                    : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));

                                            if (item.IsStatgingLocationPull)
                                            {
                                                if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                    lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                else
                                                    lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                {
                                                    double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                    foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                            {
                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }

                                                    foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                            {
                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                    lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                else
                                                    lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                //------------------------------------------------------------------------
                                                //
                                                if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                {
                                                    //TODO: Commented by CP for Pull issue, Wrong quantity pulled for normal item. on 2017-08-31
                                                    //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                    double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                    foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                            {
                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }

                                                    foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                            if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                            {
                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                                //--------------------------------------
                                //
                                string ActionType1 = "Pull";
                                if (oItemPullInfo.IsStatgingLocationPull)
                                {
                                    ActionType1 = "MS Pull";
                                }
                                long SessionUserId = SessionHelper.UserID;
                                oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                                if (oItemPullInfo.RequisitionDetailsGUID != null && oItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                                {
                                    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, SessionUserId, SessionHelper.EnterPriceID,ActionType1);
                                }
                                else if (oItemPullInfo.WorkOrderDetailGUID != null && oItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                                {
                                    bool isFromWorkOrder = true;
                                    bool AllowEditItemSellPriceonWorkOrderPull = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowEditItemSellPriceonWorkOrderPull);
                                    if(oItemPullInfo.PullCost <= 0)
                                    {
                                        isFromWorkOrder = false;
                                        AllowEditItemSellPriceonWorkOrderPull = false;
                                    }
                                    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, SessionUserId, SessionHelper.EnterPriceID, ActionType1,AllowEditItemSellPriceonWorkOrderPull,isFromWorkOrder);
                                }
                                else if (oItemPullInfo.IsStatgingLocationPull)
                                {
                                    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, SessionUserId, SessionHelper.EnterPriceID, ActionType1);
                                }
                                else
                                {
                                    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, 0, SessionUserId, SessionHelper.EnterPriceID, ActionType1);
                                }

                            }
                            else
                            {
                                UpdatePullDataForLaborType(oItemPullInfo);

                            }

                        }

                        oReturn.Add(oItemPullInfo);
                    }
                    List<UDFDTO> objUDFDTO = new List<UDFDTO>();
                    UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                    objUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objUDFDTO != null && objUDFDTO.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.UDF1, "PullMaster", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.UDF2, "PullMaster", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.UDF3, "PullMaster", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.UDF4, "PullMaster", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.UDF5, "PullMaster", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                    }
                }
                else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    RequisitionItemsToPull obj = new RequisitionItemsToPull()
                    {
                        ToolGUID = item.ToolGUID,
                        RequisitionDetailGUID = item.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty).ToString(),
                        RequisitionMasterGUID = objReqDetailsDTO.RequisitionGUID.GetValueOrDefault(Guid.Empty).ToString(),
                        PullCreditQuantity = item.PullQuantity,
                        PullCredit = "checkout",
                        TechnicianGUID = item.TechnicianGUID,
                        TempPullQTY = item.PullQuantity,
                        ToolCheckoutUDF1 = item.ToolCheckoutUDF1,
                        ToolCheckoutUDF2 = item.ToolCheckoutUDF2,
                        ToolCheckoutUDF3 = item.ToolCheckoutUDF3,
                        ToolCheckoutUDF4 = item.ToolCheckoutUDF4,
                        ToolCheckoutUDF5 = item.ToolCheckoutUDF5,
                    };
                    JsonResult repsonse = RequisitionToolCheckout(obj);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ReqPullAllJsonResponse errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                    ToolMasterDAL toolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    ToolMasterDTO toolDTO = toolDAL.GetToolByGUIDPlain(item.ToolGUID.GetValueOrDefault(Guid.Empty));
                    errMsg.ItemNumber = toolDTO.ToolName;
                    if (errMsg.Message != "ok")
                    {
                        item.ErrorMessage = toolDTO.ToolName + ": " + errMsg.Message;
                        item.ItemNumber = toolDTO.ToolName;
                    }
                    else
                    {
                        item.ErrorMessage = "";
                    }
                    item.ErrorList = new List<PullErrorInfo>();
                    oReturn.Add(item);
                }
            }

            if (isFromPull)
            {
                try
                {
                    List<Guid> listpullGUIDs = oReturn.Where(p => ((p.ErrorMessage ?? string.Empty) == string.Empty || (p.ErrorMessage ?? string.Empty).ToLower() == "ok") && (p.PullGUID ?? Guid.Empty) != Guid.Empty).ToList().Select(x => x.PullGUID.GetValueOrDefault(Guid.Empty)).ToList();


                    string pullGUIDs = string.Join(",", listpullGUIDs);
                    string dataGUIDs = "<DataGuids>" + pullGUIDs + "</DataGuids>";
                    string eventName = "OPC";
                    string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                    NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                    List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    if (lstNotification != null && lstNotification.Count > 0)
                    {
                        objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                    }
                }
                catch (Exception ex)
                {

                    CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                }

            }

            return Json(oReturnError);
        }

        #endregion

        #region 3055 Pull Qty edit

        [HttpPost]
        public ActionResult PullItemQuantityEdit(Guid PullGUID, Guid ItemGuid, double OldPullQuantity, double NewPullQuantity, string PullCreditType)
        {
            string _returnmessage = ResMessage.SaveMessage;
            string _returnstatus = "OK";

            #region take item data

            ItemMasterDTO oItemRecord = new ItemMasterDAL(this.enterPriseDBName).GetItemWithoutJoins(null, ItemGuid);

            #endregion

            #region take Pull Master data from pull guid

            PullMasterDAL objpullMasterDAL = this.pullMasterDAL;
            PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
            PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

            objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(PullGUID);           
            objnewPullMasterData = objoldPullMasterData;

            //need to update oter fileds

            #endregion

            #region Check project spend or bin deleted

            BinMasterDTO objBinCheck = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objBinCheck != null && objBinCheck.ID > 0 && (objBinCheck.IsDeleted.GetValueOrDefault(false) == true || objBinCheck.IsArchived.GetValueOrDefault(false) == true))
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            else if (objBinCheck == null)
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objBinCheck.BinNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            ProjectMasterDAL objPrjMsgDAL = new ProjectMasterDAL(this.enterPriseDBName);
            ProjectMasterDTO objPrjMstDTO = new ProjectMasterDTO();
            objPrjMstDTO = objPrjMsgDAL.GetProjectMasterByGuidNormal(objoldPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty));
            if (objPrjMstDTO != null && objPrjMstDTO.ID > 0 && (objPrjMstDTO.IsDeleted.GetValueOrDefault(false) == true || objPrjMstDTO.IsArchived.GetValueOrDefault(false) == true))
            {
                return Json(new { Message = string.Format(ResPullMaster.DeletedSoPullUpdateNotAllowed, objPrjMstDTO.ProjectSpendName), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            #endregion

            #region take Pull details data from pull guid

            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();

            lstloldPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            #endregion

            if (objoldPullMasterData != null && lstloldPullDetailsDTO != null)
            {
                #region For Pull

                if (!string.IsNullOrWhiteSpace(PullCreditType) && PullCreditType.ToLower().Equals("pull"))
                {
                    bool isBinlevelEnforce = false;
                    if (objoldPullMasterData.BinID != 0)
                    {
                        BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objoldPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objBin != null && objBin.IsEnforceDefaultPullQuantity.GetValueOrDefault(false) == true)
                        {
                            if (NewPullQuantity < objBin.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)objBin.DefaultPullQuantity != 0)
                            {
                                return Json(new { Message = string.Format(ResPullMaster.LocationPullQtyMustBeDefaultPullQty, objBin.BinNumber, objBin.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                isBinlevelEnforce = true;
                            }
                        }
                    }

                    if (oItemRecord.PullQtyScanOverride && oItemRecord.DefaultPullQuantity > 0 && isBinlevelEnforce == false)
                    {
                        if (NewPullQuantity < oItemRecord.DefaultPullQuantity || (decimal)NewPullQuantity % (decimal)oItemRecord.DefaultPullQuantity != 0)
                        {
                            return Json(new { Message = string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, oItemRecord.DefaultPullQuantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    bool IsPSLimitExceed = false;
                    string locationMSG = "";
                    long ModuleId = 0;

                    try
                    {
                        #region "Check Project Spend Condition"
                        bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        #endregion

                        //need to update other fileds as required                            

                        string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                        #region New Quantity is grater than or less than or equal to old quantity 

                        if (NewPullQuantity >= OldPullQuantity
                            || NewPullQuantity <= OldPullQuantity)
                        {
                            //need to update other fileds as required
                            objnewPullMasterData.PoolQuantity = NewPullQuantity; //(NewPullQuantity - OldPullQuantity);
                            objnewPullMasterData.TempPullQTY = NewPullQuantity; // (NewPullQuantity - OldPullQuantity);

                            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
                            objnewPullMasterData = objPullMasterDAL.GetPullWithDetailsForEdit(objnewPullMasterData, SessionHelper.RoomID, SessionHelper.CompanyID);
                            return PartialView("PullLotSrSelectionEdit", objnewPullMasterData);
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                        _returnmessage = string.Format(ResMessage.SaveErrorMsg, ex.Message);
                        _returnstatus = "fail";
                    }
                }

                #endregion
            }

            return PartialView("PullLotSrSelectionEdit", new PullMasterViewDTO());
            //}
            //else
            //{
            //    return PartialView("PullLotSrSelectionEdit", new PullMasterViewDTO());
            //}
        }

        public ActionResult PullLotSrSelectionDetailsEdit(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid PullGUID = Guid.Empty;
            Guid MaterialStagingGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            Guid.TryParse(Convert.ToString(Request["PullGUID"]), out PullGUID);

            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsStagginLocation = false;
            Guid.TryParse(Convert.ToString(Request["MaterialStagingGUID"]), out MaterialStagingGUID);
            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;
            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
                objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                    IsStagginLocation = true;
            }

            int TotalRecordCount = 0;
            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.BinNumber = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity; 

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                        else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));
                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                        else
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                    }

                    if (MaterialStagingGUID != Guid.Empty)
                    {
                        lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, false, string.Empty, IsStagginLocation, MaterialStagingGUID);
                    }
                    else
                    {
                        lstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullEdit(ItemGUID,PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, false, string.Empty, IsStagginLocation);
                    }

                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && !oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                        {
                            ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ItemGUID = ItemGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.BinNumber = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.BinNumber = objLocDTO.BinNumber;
                            }
                            if (oItem.SerialNumberTracking)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                            oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && !oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.BinNumber)
                                            && !oItem.SerialNumberTracking
                                            && !oItem.LotNumberTracking
                                            && !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();
                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;

                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                    {
                        if (MaterialStagingGUID != Guid.Empty)
                        {
                            retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation, MaterialStagingGUID);
                        }
                        else
                        {
                            retlstLotSrs = objPullDetails.GetItemLocationsWithLotSerialsForPullEdit(ItemGUID,PullGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, true, string.Empty, IsStagginLocation);
                        }
                    }
                }

                foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.BinNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, false, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialLotNumberForEdit(Guid? ItemGuid,Guid? PullGUID, string SerialOrLotNumber, long BinID, Guid? MaterialStagingGUID)
        {
            bool IsStagginLocation = false;

            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }
            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO;

            if (MaterialStagingGUID.HasValue && MaterialStagingGUID != Guid.Empty)
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, SerialOrLotNumber, IsStagginLocation, MaterialStagingGUID.GetValueOrDefault(Guid.Empty)).FirstOrDefault();
            }
            else
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForPullEdit(ItemGuid.GetValueOrDefault(Guid.Empty), PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, SerialOrLotNumber, IsStagginLocation).FirstOrDefault();
            }

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ItemGUID = ItemGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.BinNumber = string.Empty;
                objItemLocationLotSerialDTO.PullGUID = PullGUID;
            }
            return Json(objItemLocationLotSerialDTO);
        }

        private ItemPullInfo ValidateLotAndSerialForEdit(ItemPullInfo objItemPullInfo)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            double? _PullCost = null;
            //if (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
            //{
            //    _PullCost = objItemMasterDAL.GetItemPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, (Guid)objItemPullInfo.ItemGUID, false);
            //}

            if (objItem.PullQtyScanOverride && objItem.DefaultPullQuantity > 0)
            {
                if (objItemPullInfo.PullQuantity < objItem.DefaultPullQuantity || (decimal)objItemPullInfo.PullQuantity % (decimal)objItem.DefaultPullQuantity != 0)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + string.Format(ResPullMaster.PullQtyMustBeDefaultPullQty, objItem.DefaultPullQuantity) });
                }
            }

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + objItemPullInfo.RoomId.ToString() + "", "");

            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            bool IsStagginLocation = false;
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objItemPullInfo.BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            double AvailQty = 0;
            if (objItem.ItemType == 4)
            {
                AvailQty = objItemPullInfo.PullQuantity;
            }
            else
            {
                if (IsStagginLocation)
                {
                    List<MaterialStagingPullDetailDTO> oLocQty = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinID(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                    if (oLocQty != null)
                    {
                        AvailQty = oLocQty.Sum(x => (x.ConsignedQuantity ?? 0) + (x.CustomerOwnedQuantity ?? 0));
                    }
                }
                else
                {
                    ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                    if (oLocQty != null)
                    {
                        AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
                    }
                }
            }
            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (AvailQty >= objItemPullInfo.PullQuantity)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking)
                {
                    if (IsStagginLocation)
                    {
                        lstAvailableQty = objPullMasterDAL.GetStageLocationsByItemGuidAndBinId(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                        }
                    }
                    else
                    {
                        lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.PullQuantity, true);
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objItemPullInfo.PullQuantity;
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objItemPullInfo.PullCost = PullCost;
                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                        if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                        {
                            objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                        }
                    }
                }
                else
                {
                    if (objItem.LotNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.LotNumber, string.Empty);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.LotNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.LotNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;

                                        t.IsStagingLocationLotSerial = true;
                                        t.LotNumber = lstLotQty.LotNumber;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                            else
                            {
                                List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.LotNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.LotNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }
                    }
                    else if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;

                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByItemGUIDANDBinIDForLotSr(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, string.Empty, t.SerialNumber);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.SerialNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.SerialNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;

                                        t.IsStagingLocationLotSerial = true;
                                        t.SerialNumber = lstLotQty.LotNumber;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                            else
                            {
                                List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.SerialNumber } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         LotNumber = grpms.Key.SerialNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }
                    }
                    else if (objItem.DateCodeTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;

                        lstAvailableQty.ForEach(t =>
                        {
                            if (IsStagginLocation)
                            {
                                List<MaterialStagingPullDetailDTO> objItemLocationDetail = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMSLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.ExpirationDate.GetValueOrDefault(DateTime.Now));
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.ExpirationDate } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         ExpirationDate = grpms.Key.ExpirationDate,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;

                                        t.IsStagingLocationLotSerial = true;
                                        t.ExpirationDate = lstLotQty.ExpirationDate;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                            else
                            {                                
                                List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, t.ExpirationDate.GetValueOrDefault(DateTime.Now));
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.ExpirationDate } into grpms
                                                     select new
                                                     {
                                                         CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                         ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                         ExpirationDate = grpms.Key.ExpirationDate,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                            t.CustomerOwnedQuantity = t.PullQuantity;
                                        else
                                            t.ConsignedQuantity = t.PullQuantity;
                                        t.ExpirationDate = lstLotQty.ExpirationDate;
                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.PullCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                            if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                            {
                                objItemPullInfo = objPullMasterDAL.ValidatePullProjectSpend(objItemPullInfo);
                            }
                        }
                    }
                }
            }
            else
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
            }
            if (IsStagginLocation)
            {
                objItemPullInfo.IsStatgingLocationPull = true;
            }
            return objItemPullInfo;
        }

        public JsonResult GetLotOrSerailNumberListForEdit(int maxRows, string name_startsWith, Guid? ItemGuid, Guid? PullGUID, long BinID, string prmSerialLotNos = null, Guid? materialStagingGUID = null)
        {
            bool IsStagginLocation = false;

            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO;
            if (materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, string.Empty, IsStagginLocation, materialStagingGUID.GetValueOrDefault(Guid.Empty));
            }
            else
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForPullEdit(ItemGuid.GetValueOrDefault(Guid.Empty), PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, string.Empty, IsStagginLocation);
            }

            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }
            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PullSerialsAndLotsNewEdit(List<ItemPullInfo> objItemPullInfo)
        {
            PullTransactionDAL objPullTransactionMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            PullMasterDAL objpullMasterDAL = this.pullMasterDAL;
            ProjectMasterDAL projDAL = new ProjectMasterDAL(this.enterPriseDBName);
            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBINDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingPullDetailDAL objMSPDetailsDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            bool isFromPull = true;

            var isFromWOORREQ = objItemPullInfo.Where(i => i.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty || i.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
            if (isFromWOORREQ != null && isFromWOORREQ.Count > 0)
            {
                isFromPull = false;
            }

            foreach (ItemPullInfo item in objItemPullInfo)
            {
                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        //-------------------------------------Get Item Master-------------------------------------
                        //

                        string strWhatWhereAction = "EPQ" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                        //-----------------------------------------------------------------------------------------
                        //

                        #region take Pull Master data from pull guid

                        PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
                        PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

                        objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(item.PullGUID.GetValueOrDefault(Guid.Empty));
                        objoldPullMasterData.WhatWhereAction = strWhatWhereAction;
                        objnewPullMasterData = objoldPullMasterData;

                        //need to update oter fileds

                        #endregion

                        #region take Pull details data from pull guid

                        List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();

                        lstloldPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(item.PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                        double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                        double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                        #endregion

                        ItemPullInfo oItemPullInfo = item;

                        #region Get Project name by guid

                        ProjectMasterDTO projDTO = new ProjectMasterDTO();
                        if (objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                            projDTO = projDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                        #endregion

                        oItemPullInfo.PullOrderNumber = objnewPullMasterData.PullOrderNumber;
                        oItemPullInfo.ProjectSpendGUID = objnewPullMasterData.ProjectSpendGUID;
                        oItemPullInfo.ProjectSpendName = (projDTO != null ? projDTO.ProjectSpendName : null);
                        oItemPullInfo.WorkOrderDetailGUID = objnewPullMasterData.WorkOrderDetailGUID;
                        oItemPullInfo.UDF1 = objnewPullMasterData.UDF1;
                        oItemPullInfo.UDF2 = objnewPullMasterData.UDF2;
                        oItemPullInfo.UDF3 = objnewPullMasterData.UDF3;
                        oItemPullInfo.UDF4 = objnewPullMasterData.UDF4;
                        oItemPullInfo.UDF5 = objnewPullMasterData.UDF5;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        oItemPullInfo.ValidateProjectSpendLimits = true;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();

                        #region For Pull

                        if (objnewPullMasterData.PullCredit.ToLower().Equals("pull"))
                        {
                            #region For More or Less Pull

                            if (oItemPullInfo.PullQuantity >= objnewPullMasterData.PoolQuantity
                                || oItemPullInfo.PullQuantity <= objnewPullMasterData.PoolQuantity)
                            {
                                #region Revert back previous pull quantity for that pull from pull detail and item location table
                                /// new entry done like previous deleted and new added
                                /// existing serial but not selected from current transaction
                                /// 

                                List<PullDetailsDTO> lstRevertedPullDetailsForExec = new List<PullDetailsDTO>();

                                List<string> lstseriallotexp = new List<string>();
                                List<KeyValDTO> seriallotexp = new List<KeyValDTO>();
                                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    seriallotexp = (from x in oItemPullInfo.lstItemPullDetails
                                                    select new KeyValDTO()
                                                    {
                                                        key = x.SerialNumber,
                                                        value = Convert.ToDateTime(x.ExpirationDate).ToString("MM/dd/yyyy")
                                                    }).ToList();
                                }
                                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    seriallotexp = (from x in oItemPullInfo.lstItemPullDetails
                                                    select new KeyValDTO()
                                                    {
                                                        key = x.LotNumber,
                                                        value = Convert.ToDateTime(x.ExpirationDate).ToString("MM/dd/yyyy")
                                                    }).ToList();
                                }
                                else if (objItemmasterDTO.SerialNumberTracking)
                                {
                                    lstseriallotexp = oItemPullInfo.lstItemPullDetails.Select(x => x.SerialNumber).ToList();
                                }
                                else if (objItemmasterDTO.LotNumberTracking)
                                {
                                    lstseriallotexp = oItemPullInfo.lstItemPullDetails.Select(x => x.LotNumber).ToList();
                                }
                                else if (objItemmasterDTO.DateCodeTracking)
                                {
                                    lstseriallotexp = oItemPullInfo.lstItemPullDetails.Select(x => Convert.ToDateTime(x.ExpirationDate).ToString("MM/dd/yyyy")).ToList();
                                }

                                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !seriallotexp.Any(z => x.SerialNumber == z.key && x.Expiration == z.value) && x.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.SerialNumber = pullDetailsDTO.SerialNumber;
                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                    }
                                }
                                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(item1 => !seriallotexp.Any(item2 => item1.LotNumber == item2.key && item1.Expiration == item2.value) && item1.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                    }
                                }
                                else if (objItemmasterDTO.SerialNumberTracking)
                                {
                                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.SerialNumber) && x.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.SerialNumber = pullDetailsDTO.SerialNumber;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                    }
                                }
                                else if (objItemmasterDTO.LotNumberTracking)
                                {
                                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.LotNumber)).ToList())
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity =  pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                    }
                                }
                                else if (objItemmasterDTO.DateCodeTracking)
                                {
                                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.Expiration)).ToList())
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity =  pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                    }
                                }

                                #endregion

                                #region take Pull details data from pull guid after revert back existing serial but not selected from current transaction

                                List<PullDetailsDTO> lstafterrevertPullDetailsDTO = new List<PullDetailsDTO>();
                                lstafterrevertPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(item.PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.PoolQuantity.GetValueOrDefault(0) > 0).ToList();
                                if (lstRevertedPullDetailsForExec.Count > 0)
                                {
                                    lstafterrevertPullDetailsDTO = lstafterrevertPullDetailsDTO.Where(item1 => !lstRevertedPullDetailsForExec.Any(item2 => item1.GUID == item2.GUID)).ToList();
                                }

                                #endregion

                                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    oItemPullInfo.lstItemPullDetails = oItemPullInfo.lstItemPullDetails.Where(item1 => !lstafterrevertPullDetailsDTO.Any(item2 => item1.SerialNumber == item2.SerialNumber && Convert.ToDateTime(item1.ExpirationDate).ToString("MM/dd/yyyy") == item2.Expiration)).ToList();
                                    oItemPullInfo.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);
                                }
                                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                {
                                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => seriallotexp.Any(z => x.LotNumber == z.key && x.Expiration == z.value)).ToList();

                                    lstoldserialDetailsDTO =
                                                (from c in lstoldserialDetailsDTO
                                                 group c by new
                                                 {
                                                     c.LotNumber,
                                                     c.Expiration
                                                 } into gcs
                                                 select new PullDetailsDTO()
                                                 {
                                                     LotNumber = gcs.Key.LotNumber,
                                                     Expiration = gcs.Key.Expiration,
                                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                                 }).ToList();

                                    oItemPullInfo.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);

                                    List<ItemLocationLotSerialDTO> lstnewserialDetailsDTO = new List<ItemLocationLotSerialDTO>();
                                    lstnewserialDetailsDTO = oItemPullInfo.lstItemPullDetails.Where(x => seriallotexp.Any(z => x.LotNumber == z.key && Convert.ToDateTime(x.ExpirationDate).ToString("MM/dd/yyyy") == z.value)).ToList();

                                    lstnewserialDetailsDTO =
                                               (from c in lstnewserialDetailsDTO
                                                group c by new
                                                {
                                                    c.LotNumber,
                                                    c.ExpirationDate,
                                                    c.Expiration,
                                                    c.BinNumber,
                                                    c.BinID,
                                                    c.DateCodeTracking,
                                                    c.SerialNumberTracking,
                                                    c.LotNumberTracking,
                                                    c.LotOrSerailNumber,
                                                    c.ID,
                                                    c.ItemGUID
                                                } into gcs
                                                select new ItemLocationLotSerialDTO()
                                                {
                                                    LotNumber = gcs.Key.LotNumber,
                                                    ExpirationDate = gcs.Key.ExpirationDate,
                                                    Expiration = gcs.Key.Expiration,
                                                    BinNumber = gcs.Key.BinNumber,
                                                    BinID = gcs.Key.BinID,
                                                    DateCodeTracking = gcs.Key.DateCodeTracking,
                                                    SerialNumberTracking = gcs.Key.SerialNumberTracking,
                                                    LotNumberTracking = gcs.Key.LotNumberTracking,
                                                    LotOrSerailNumber = gcs.Key.LotOrSerailNumber,
                                                    ID = gcs.Key.ID,
                                                    ItemGUID = gcs.Key.ItemGUID,
                                                    ConsignedTobePulled = gcs.Sum(x => x.ConsignedTobePulled),
                                                    CustomerOwnedTobePulled = gcs.Sum(x => x.CustomerOwnedTobePulled),
                                                    PullQuantity = (gcs.Sum(x => x.PullQuantity)),
                                                    TotalTobePulled = (gcs.Sum(x => x.TotalTobePulled))
                                                }).ToList();

                                    foreach (ItemLocationLotSerialDTO pullDetails in lstnewserialDetailsDTO)
                                    {
                                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber && x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber && x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                                        double newLotPullQuantity = (pullDetails.PullQuantity);

                                        if (newLotPullQuantity < oldLotPullQuantity)
                                        {
                                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber && x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).ToList())
                                            {
                                                if (rmainingQty > 0)
                                                {
                                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                                    }
                                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.ConsignedQuantity = rmainingQty;
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                                            }
                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = 0;
                                                    }
                                                }
                                            }
                                            pullDetails.PullQuantity = 0;
                                        }
                                        else if (newLotPullQuantity > oldLotPullQuantity)
                                        {
                                            pullDetails.PullQuantity = (newLotPullQuantity - oldLotPullQuantity);
                                        }
                                        else
                                        {
                                            pullDetails.PullQuantity = 0;
                                        }
                                    }

                                    oItemPullInfo.lstItemPullDetails = lstnewserialDetailsDTO.Where(x => x.PullQuantity > 0).ToList();
                                    oItemPullInfo.PullQuantity = lstnewserialDetailsDTO.Sum(x => x.PullQuantity);
                                }
                                else if (objItemmasterDTO.SerialNumberTracking)
                                {
                                    oItemPullInfo.lstItemPullDetails = oItemPullInfo.lstItemPullDetails.Where(item1 => !lstafterrevertPullDetailsDTO.Any(item2 => item1.SerialNumber == item2.SerialNumber)).ToList();
                                    oItemPullInfo.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);
                                }
                                else if (objItemmasterDTO.LotNumberTracking)
                                {
                                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => lstseriallotexp.Contains(x.LotNumber)).ToList();

                                    lstoldserialDetailsDTO =
                                                (from c in lstoldserialDetailsDTO
                                                 group c by new
                                                 {
                                                     c.LotNumber
                                                 } into gcs
                                                 select new PullDetailsDTO()
                                                 {
                                                     LotNumber = gcs.Key.LotNumber,
                                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                                 }).ToList();

                                    oItemPullInfo.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);

                                    List<ItemLocationLotSerialDTO> lstnewserialDetailsDTO = new List<ItemLocationLotSerialDTO>();
                                    lstnewserialDetailsDTO = oItemPullInfo.lstItemPullDetails.Where(x => lstseriallotexp.Contains(x.LotNumber)).ToList();

                                    lstnewserialDetailsDTO =
                                               (from c in lstnewserialDetailsDTO
                                                group c by new
                                                {
                                                    c.LotNumber,
                                                    c.BinNumber,
                                                    c.BinID,
                                                    c.DateCodeTracking,
                                                    c.SerialNumberTracking,
                                                    c.LotNumberTracking,
                                                    c.LotOrSerailNumber,
                                                    c.ID,
                                                    c.ItemGUID
                                                } into gcs
                                                select new ItemLocationLotSerialDTO()
                                                {
                                                    LotNumber = gcs.Key.LotNumber,
                                                    BinNumber = gcs.Key.BinNumber,
                                                    BinID = gcs.Key.BinID,
                                                    DateCodeTracking = gcs.Key.DateCodeTracking,
                                                    SerialNumberTracking = gcs.Key.SerialNumberTracking,
                                                    LotNumberTracking = gcs.Key.LotNumberTracking,
                                                    LotOrSerailNumber = gcs.Key.LotOrSerailNumber,
                                                    ID = gcs.Key.ID,
                                                    ItemGUID = gcs.Key.ItemGUID,
                                                    ConsignedTobePulled = gcs.Sum(x => x.ConsignedTobePulled),
                                                    CustomerOwnedTobePulled = gcs.Sum(x => x.CustomerOwnedTobePulled),
                                                    PullQuantity = (gcs.Sum(x => x.PullQuantity)),
                                                    TotalTobePulled = (gcs.Sum(x => x.TotalTobePulled))
                                                }).ToList();

                                    foreach (ItemLocationLotSerialDTO pullDetails in lstnewserialDetailsDTO)
                                    {
                                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                                        double newLotPullQuantity = (pullDetails.PullQuantity);

                                        if (newLotPullQuantity < oldLotPullQuantity)
                                        {
                                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.LotNumber == pullDetails.LotNumber).ToList())
                                            {
                                                if (rmainingQty > 0)
                                                {
                                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity =  pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                                    }
                                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.ConsignedQuantity = rmainingQty;
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                                            }
                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = 0;
                                                    }                                                    
                                                }
                                            }
                                            pullDetails.PullQuantity = 0;
                                        }
                                        else if (newLotPullQuantity > oldLotPullQuantity)
                                        {
                                            pullDetails.PullQuantity = (newLotPullQuantity - oldLotPullQuantity);
                                        }
                                        else
                                        {
                                            pullDetails.PullQuantity = 0;
                                        }
                                    }

                                    oItemPullInfo.lstItemPullDetails = lstnewserialDetailsDTO.Where(x => x.PullQuantity > 0).ToList();
                                    oItemPullInfo.PullQuantity = lstnewserialDetailsDTO.Sum(x => x.PullQuantity);
                                }
                                else if (objItemmasterDTO.DateCodeTracking)
                                {
                                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => lstseriallotexp.Contains(x.Expiration)).ToList();

                                    lstoldserialDetailsDTO =
                                                (from c in lstoldserialDetailsDTO
                                                 group c by new
                                                 {
                                                     c.Expiration
                                                 } into gcs
                                                 select new PullDetailsDTO()
                                                 {
                                                     Expiration = gcs.Key.Expiration,
                                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                                 }).ToList();

                                    oItemPullInfo.PullQuantity = oItemPullInfo.lstItemPullDetails.Sum(x => x.PullQuantity);

                                    List<ItemLocationLotSerialDTO> lstnewserialDetailsDTO = new List<ItemLocationLotSerialDTO>();
                                    lstnewserialDetailsDTO = oItemPullInfo.lstItemPullDetails.Where(x => lstseriallotexp.Contains(Convert.ToDateTime(x.ExpirationDate).ToString("MM/dd/yyyy"))).ToList();

                                    lstnewserialDetailsDTO =
                                               (from c in lstnewserialDetailsDTO
                                                group c by new
                                                {
                                                    c.ExpirationDate,
                                                    c.BinNumber,
                                                    c.BinID,
                                                    c.DateCodeTracking,
                                                    c.SerialNumberTracking,
                                                    c.LotNumberTracking,
                                                    c.LotOrSerailNumber,
                                                    c.ID,
                                                    c.ItemGUID
                                                } into gcs
                                                select new ItemLocationLotSerialDTO()
                                                {
                                                    ExpirationDate = gcs.Key.ExpirationDate,
                                                    BinNumber = gcs.Key.BinNumber,
                                                    BinID = gcs.Key.BinID,
                                                    DateCodeTracking = gcs.Key.DateCodeTracking,
                                                    SerialNumberTracking = gcs.Key.SerialNumberTracking,
                                                    LotNumberTracking = gcs.Key.LotNumberTracking,
                                                    LotOrSerailNumber = gcs.Key.LotOrSerailNumber,
                                                    ID = gcs.Key.ID,
                                                    ItemGUID = gcs.Key.ItemGUID,
                                                    ConsignedTobePulled = gcs.Sum(x => x.ConsignedTobePulled),
                                                    CustomerOwnedTobePulled = gcs.Sum(x => x.CustomerOwnedTobePulled),
                                                    PullQuantity = (gcs.Sum(x => x.PullQuantity)),
                                                    TotalTobePulled = (gcs.Sum(x => x.TotalTobePulled))
                                                }).ToList();

                                    foreach (ItemLocationLotSerialDTO pullDetails in lstnewserialDetailsDTO)
                                    {
                                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                                        double newLotPullQuantity = (pullDetails.PullQuantity);

                                        if (newLotPullQuantity < oldLotPullQuantity)
                                        {
                                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpirationDate).ToString("MM/dd/yyyy")).ToList())
                                            {
                                                if (rmainingQty > 0)
                                                {
                                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                                    }
                                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                                    {
                                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                                        {
                                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                                            }
                                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.ConsignedQuantity =  rmainingQty;
                                                            }
                                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                            {
                                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                                            }
                                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                                        }
                                                        rmainingQty = 0;
                                                    }
                                                }
                                            }
                                            pullDetails.PullQuantity = 0;
                                        }
                                        else if (newLotPullQuantity > oldLotPullQuantity)
                                        {
                                            pullDetails.PullQuantity = (newLotPullQuantity - oldLotPullQuantity);
                                        }
                                        else
                                        {
                                            pullDetails.PullQuantity = 0;
                                        }
                                    }

                                    oItemPullInfo.lstItemPullDetails = lstnewserialDetailsDTO.Where(x => x.PullQuantity > 0).ToList();
                                    oItemPullInfo.PullQuantity = lstnewserialDetailsDTO.Sum(x => x.PullQuantity);
                                }

                                if (oItemPullInfo.PullQuantity > 0)
                                {
                                    // validate for serial,lot and date code
                                    oItemPullInfo = ValidateLotAndSerialForEdit(oItemPullInfo);

                                    #region Validation for Serial number alaredy credited or not
                                    if (objItemmasterDTO.SerialNumberTracking
                                       || (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking) )
                                    {
                                        string lstSerialNumber = string.Empty;
                                        foreach (PullDetailsDTO revertSerial in lstRevertedPullDetailsForExec)
                                        {
                                            string serailErrorMessage = commonDAL.CheckDuplicateSerialNumbers(revertSerial.SerialNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID, revertSerial.ItemGUID.GetValueOrDefault(Guid.Empty));

                                            if (serailErrorMessage.ToLower().Trim() == "duplicate")
                                            {
                                                if (lstSerialNumber != string.Empty)
                                                    lstSerialNumber = lstSerialNumber + " , " + revertSerial.SerialNumber;
                                                else
                                                    lstSerialNumber = revertSerial.SerialNumber;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(lstSerialNumber))
                                        { 
                                            oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = string.Format(ResPullMaster.CantEditPullForCreditedSerials, lstSerialNumber) });
                                        }
                                    }
                                    #endregion

                                    #region Validation for Lot and Expiration item
                                    if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                    {
                                        string lstLotNumber = string.Empty;
                                        foreach (PullDetailsDTO revertLotDateCode in lstRevertedPullDetailsForExec)
                                        {
                                            DateTime dtExpDate;
                                            DateTime.TryParseExact(Convert.ToDateTime(revertLotDateCode.Expiration).ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);
                                            //DateTime.TryParseExact(revertLotDateCode.Expiration, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);

                                            if (dtExpDate <= DateTime.MinValue)
                                            {
                                                oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = ResPullMaster.EnterValidExpirationDate });
                                            }

                                            string msg = commonDAL.CheckDuplicateLotAndExpiration(revertLotDateCode.LotNumber, revertLotDateCode.Expiration, dtExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, revertLotDateCode.ItemGUID.GetValueOrDefault(Guid.Empty),SessionHelper.UserID,SessionHelper.EnterPriceID);
                                            if (!string.IsNullOrWhiteSpace(msg) && !msg.ToLower().Equals("ok"))
                                            {
                                                if (lstLotNumber != string.Empty)
                                                    lstLotNumber = lstLotNumber + " , " + msg;
                                                else
                                                    lstLotNumber = msg;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(lstLotNumber))
                                        {
                                            oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = lstLotNumber });
                                        }
                                    }
                                    #endregion

                                    if (oItemPullInfo.ErrorList.Count == 0 && oItemPullInfo.lstItemPullDetails.Where(x => !string.IsNullOrEmpty(x.ValidationMessage)).Count() == 0)
                                    {
                                        #region  execution  for revert back quantity
                                        bool isneedtorevertpullCost = false;
                                        foreach (PullDetailsDTO revertData in lstRevertedPullDetailsForExec)
                                        {
                                            isneedtorevertpullCost = true;
                                            PullDetailsDTO pullDetailsDTO = objPullDetails.GetPullDetailByGuidNormal(revertData.GUID);
                                            if (pullDetailsDTO != null && pullDetailsDTO.ID > 0
                                                && revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty))
                                            {
                                                ItemLocationDetailsDTO itemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                                itemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if (itemLocationDetailsDTO != null && itemLocationDetailsDTO.ID > 0)
                                                {
                                                    if (revertData.ConsignedQuantity > 0)
                                                    {
                                                        itemLocationDetailsDTO.ConsignedQuantity = itemLocationDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) + revertData.ConsignedQuantity;
                                                    }
                                                    if (revertData.CustomerOwnedQuantity > 0)
                                                    {
                                                        itemLocationDetailsDTO.CustomerOwnedQuantity = itemLocationDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + revertData.CustomerOwnedQuantity;
                                                    }
                                                    itemLocationDetailsDTO.EditedFrom = "Web";
                                                    itemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                                    itemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;

                                                    objItemLocationDetailsDAL.Edit(itemLocationDetailsDTO);

                                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                                    {
                                                        pullDetailsDTO.ConsignedQuantity = revertData.PoolQuantity;
                                                    }
                                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                    {
                                                        pullDetailsDTO.CustomerOwnedQuantity = revertData.PoolQuantity;
                                                    }
                                                    pullDetailsDTO.EditedFrom = "Web";
                                                    pullDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                                    pullDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                                    pullDetailsDTO.PoolQuantity = (pullDetailsDTO.ConsignedQuantity + pullDetailsDTO.CustomerOwnedQuantity);
                                                    objPullDetails.Edit(pullDetailsDTO);
                                                }
                                            }
                                        }

                                        #region Update PullQty,PullCost,PullPrice and projectspend after revert back
                                        if (isneedtorevertpullCost)
                                        {
                                            List<PullDetailsDTO> lstPullDtl = objPullDetails.GetPullDetailsByPullGuidPlain(objoldPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                            if (lstPullDtl != null && lstPullDtl.Count > 0)
                                            {
                                                #region item on hand quantity update

                                                double OnHandQuantity = objItemmasterDTO.OnHandQuantity.GetValueOrDefault(0);
                                                ItemLocationDetailsDTO itemlocationQuatnity = objItemLocationDetailsDAL.GetItemLocationDetailsByItemGUID(objoldPullMasterData.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if(itemlocationQuatnity !=null && itemlocationQuatnity.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                                {
                                                    OnHandQuantity = (itemlocationQuatnity.CustomerOwnedQuantity.GetValueOrDefault(0)
                                                                     +
                                                                     itemlocationQuatnity.ConsignedQuantity.GetValueOrDefault(0)
                                                                        );
                                                }
                                                #endregion

                                                double OldPullCost = objnewPullMasterData.PullCost ?? 0;
                                                double OldPullQuantity = objnewPullMasterData.PoolQuantity ?? 0;

                                                objoldPullMasterData.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                                objoldPullMasterData.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                                                objoldPullMasterData.PoolQuantity = (
                                                                    lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                                        +
                                                                    lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                                    );

                                                objoldPullMasterData.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                                                objoldPullMasterData.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                                                objoldPullMasterData.WhatWhereAction = strWhatWhereAction;
                                                objoldPullMasterData.ItemOnhandQty = OnHandQuantity;
                                                objoldPullMasterData.ItemStageQty = objItemmasterDTO.StagedQuantity;
                                                objoldPullMasterData.ItemLocationOnHandQty = 0;

                                                ItemLocationQTYDTO objItemLocationQuantity = objItemLocationDetailsDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, objoldPullMasterData.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                                if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                                                {
                                                    objoldPullMasterData.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                                                }

                                                objpullMasterDAL.EditForPullQty(objoldPullMasterData);
                                                //objpullMasterDAL.InsertPullEditHistory(objoldPullMasterData.GUID, objoldPullMasterData.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, strWhatWhereAction);
                                                //objpullMasterDAL.Edit(objoldPullMasterData);

                                                double DiffPullCost = (OldPullCost - (objoldPullMasterData.PullCost ?? 0));
                                                double DiffPoolQuantity = (OldPullQuantity - (objoldPullMasterData.PoolQuantity ?? 0));

                                                if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                                {
                                                    objpullMasterDAL.UpdateProjectSpendWithCostEditPull(objItemmasterDTO, objoldPullMasterData, DiffPullCost, DiffPoolQuantity, objoldPullMasterData.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                }
                                            }
                                        }

                                        #endregion

                                        #endregion

                                        BinMasterDTO objBINDTO = new BinMasterDTO();

                                        //-----------Project Span---------------
                                        //
                                        if (!(item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                                && (item.ProjectSpendName != null && item.ProjectSpendName.Trim() != ""))
                                        {
                                            ProjectMasterDAL objProjectSpendDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                                            ProjectMasterDTO projectMaster = objProjectSpendDAL.GetProjectByName(item.ProjectSpendName, SessionHelper.RoomID, SessionHelper.CompanyID, null);

                                            if (projectMaster != null && projectMaster.GUID != Guid.Empty)
                                            {
                                                item.ProjectSpendGUID = projectMaster.GUID;
                                            }
                                        }

                                        if (objItemmasterDTO.ItemType != 4)
                                        {
                                            if (item.ProjectSpendGUID != null && item.ProjectSpendGUID.HasValue && item.ProjectSpendGUID != Guid.Empty)
                                            {
                                                //--------------------------------------
                                                //
                                                int IsCreditPullNothing = 2;
                                                bool IsProjectSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                                                var tmpsupplierIds = new List<long>();
                                                //ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetAllRecords(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds).Where(x => x.ItemGUID == item.ItemGUID).SingleOrDefault();
                                                ProjectSpendItemsDTO objPrjSpenItmDTO = objPrjSpenItmDAL.GetProjectSpendItem(item.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds, Convert.ToString(item.ItemGUID)).FirstOrDefault();
                                                ProjectMasterDTO objPrjMstDTO = projDAL.GetRecord(item.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                                                string ItemLocationMSG = "";
                                                bool IsPSLimitExceed = false;

                                                //--------------------------------------
                                                //
                                                PullMasterViewDTO oPull = new PullMasterViewDTO();
                                                oPull.TempPullQTY = item.PullQuantity;

                                                //--------------------------------------
                                                //
                                                List<ItemLocationDetailsDTO> lstItemLocationDetails = new List<ItemLocationDetailsDTO>();
                                                List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = new List<ItemLocationDetailsDTO>();
                                                List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                                List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                                {
                                                    double CurrentPullQuantity = 0;
                                                    foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                                    {
                                                        string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                                : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));
                                                        if (item.IsStatgingLocationPull)
                                                        {
                                                            if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                                lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            else
                                                                lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);

                                                            if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                            {
                                                                foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO != null)
                                                                        lstMSPDetails.Add(objItemLocationDetailsDTO);
                                                                }
                                                            }

                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                            //------------------------------------------------------------------------
                                                            //
                                                            if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                            {
                                                                double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                                foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                            objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }

                                                                foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                            objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                                lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            else
                                                                lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                            {
                                                                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO != null)
                                                                        lstItemLocationDetails.Add(objItemLocationDetailsDTO);
                                                                }
                                                            }

                                                            objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                            objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                            //------------------------------------------------------------------------
                                                            //
                                                            if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                            {
                                                                double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                            objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }

                                                                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                            objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                //--------------------------------------                                                                    
                                                if (objpullMasterDAL.ProjectWiseQuantityCheck(objPrjSpenItmDTO, objPrjMstDTO, out ItemLocationMSG, oPull, objItemmasterDTO, IsProjectSpendAllowed, out IsPSLimitExceed, lstItemLocationDetails, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, SessionHelper.CompanyID, SessionHelper.RoomID))
                                                {
                                                    item.ErrorMessage = ItemLocationMSG;
                                                    oReturn.Add(item);
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                                                {
                                                    List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                                    List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                                    List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = null;
                                                    double CurrentPullQuantity = 0;
                                                    foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                                    {
                                                        objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                                        objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                                        objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                                        objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                                        string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                                : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));

                                                        if (item.IsStatgingLocationPull)
                                                        {
                                                            if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                                lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            else
                                                                lstMSPDetailsTmp = objMSPDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            if (lstMSPDetailsTmp != null && lstMSPDetailsTmp.Count > 0)
                                                            {
                                                                double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                                foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                            objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }

                                                                foreach (MaterialStagingPullDetailDTO objItemLocationDetailsDTO in lstMSPDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                            objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                                                lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            else
                                                                lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                            //------------------------------------------------------------------------
                                                            //
                                                            if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                            {
                                                                double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                                            objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                                            objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }

                                                                foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                                {
                                                                    if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                                                    {
                                                                        objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                                        if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                                        {
                                                                            CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                                            objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                                            PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                                            objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            //--------------------------------------
                                            //
                                            string ActionType1 = "Pull";
                                            if (oItemPullInfo.IsStatgingLocationPull)
                                            {
                                                ActionType1 = "MS Pull";
                                            }
                                            oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                                            long SessionUserId = SessionHelper.UserID;
                                            if (oItemPullInfo.IsStatgingLocationPull)
                                            {
                                                oItemPullInfo = objPullTransactionMasterDAL.PullItemEditQuantity(oItemPullInfo, objItemmasterDTO, ItemCost, ItemPrice, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, SessionUserId, strWhatWhereAction, SessionHelper.EnterPriceID, ActionType1);
                                            }
                                            else
                                            {
                                                oItemPullInfo = objPullTransactionMasterDAL.PullItemEditQuantity(oItemPullInfo, objItemmasterDTO, ItemCost, ItemPrice, 0, SessionUserId, strWhatWhereAction, SessionHelper.EnterPriceID, ActionType1);
                                            }
                                            
                                            QBItemQOHProcess((Guid)objItemmasterDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "Pull Edit");
                                        }
                                    }
                                }
                                else if (lstRevertedPullDetailsForExec != null && lstRevertedPullDetailsForExec.Count > 0)
                                {
                                    #region Validation for Serial number alaredy credited or not

                                    if (objItemmasterDTO.SerialNumberTracking
                                        || (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking))
                                    {
                                        string lstSerialNumber = string.Empty;
                                        foreach (PullDetailsDTO revertSerial in lstRevertedPullDetailsForExec)
                                        {
                                            string serailErrorMessage = commonDAL.CheckDuplicateSerialNumbers(revertSerial.SerialNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID, revertSerial.ItemGUID.GetValueOrDefault(Guid.Empty));

                                            if (serailErrorMessage.ToLower().Trim() == "duplicate")
                                            {
                                                if (lstSerialNumber != string.Empty)
                                                    lstSerialNumber = lstSerialNumber + " , " + revertSerial.SerialNumber;
                                                else
                                                    lstSerialNumber = revertSerial.SerialNumber;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(lstSerialNumber))
                                        { 
                                            oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = string.Format(ResPullMaster.CantEditPullForCreditedSerials, lstSerialNumber) });
                                        }
                                    }

                                    #endregion

                                    #region Validation for Lot and Expiration item
                                    if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                    {
                                        string lstLotNumber = string.Empty;
                                        foreach (PullDetailsDTO revertLotDateCode in lstRevertedPullDetailsForExec)
                                        {
                                            DateTime dtExpDate;
                                            DateTime.TryParseExact(Convert.ToDateTime(revertLotDateCode.Expiration).ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);
                                            //DateTime.TryParseExact(revertLotDateCode.Expiration, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);

                                            if (dtExpDate <= DateTime.MinValue)
                                            {
                                                oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = ResPullMaster.EnterValidExpirationDate });
                                            }

                                            string msg = commonDAL.CheckDuplicateLotAndExpiration(revertLotDateCode.LotNumber, revertLotDateCode.Expiration, dtExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, revertLotDateCode.ItemGUID.GetValueOrDefault(Guid.Empty),SessionHelper.UserID,SessionHelper.EnterPriceID);
                                            if (!string.IsNullOrWhiteSpace(msg) && !msg.ToLower().Equals("ok"))
                                            {
                                                if (lstLotNumber != string.Empty)
                                                    lstLotNumber = lstLotNumber + " , " + msg;
                                                else
                                                    lstLotNumber = msg;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(lstLotNumber))
                                        {
                                            oItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "16", ErrorMessage = lstLotNumber });
                                        }
                                    }
                                    #endregion

                                    if (oItemPullInfo.ErrorList.Count == 0 && oItemPullInfo.lstItemPullDetails.Where(x => string.IsNullOrEmpty(x.ValidationMessage)).Count() == 0)
                                    {
                                        #region  execution  for revert back quantity

                                        foreach (PullDetailsDTO revertData in lstRevertedPullDetailsForExec)
                                        {
                                            PullDetailsDTO pullDetailsDTO = objPullDetails.GetPullDetailByGuidNormal(revertData.GUID);
                                            if (pullDetailsDTO != null && pullDetailsDTO.ID > 0
                                                && revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty))
                                            {
                                                ItemLocationDetailsDTO itemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                                itemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                                if (itemLocationDetailsDTO != null && itemLocationDetailsDTO.ID > 0)
                                                {
                                                    if (revertData.ConsignedQuantity > 0)
                                                    {
                                                        itemLocationDetailsDTO.ConsignedQuantity = itemLocationDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) + revertData.ConsignedQuantity;
                                                    }
                                                    if (revertData.CustomerOwnedQuantity > 0)
                                                    {
                                                        itemLocationDetailsDTO.CustomerOwnedQuantity = itemLocationDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) + revertData.CustomerOwnedQuantity;
                                                    }
                                                    itemLocationDetailsDTO.EditedFrom = "Web";
                                                    itemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                                    itemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;

                                                    objItemLocationDetailsDAL.Edit(itemLocationDetailsDTO);

                                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                                    {
                                                        pullDetailsDTO.ConsignedQuantity = revertData.PoolQuantity;
                                                    }
                                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                                    {
                                                        pullDetailsDTO.CustomerOwnedQuantity = revertData.PoolQuantity;
                                                    }
                                                    pullDetailsDTO.EditedFrom = "Web";
                                                    pullDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                                    pullDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                                    pullDetailsDTO.PoolQuantity = (pullDetailsDTO.ConsignedQuantity + pullDetailsDTO.CustomerOwnedQuantity);
                                                    objPullDetails.Edit(pullDetailsDTO);
                                                }
                                            }
                                        }

                                        objPullTransactionMasterDAL.PullItemEditQuantityForRevertExe(oItemPullInfo, objItemmasterDTO, ItemCost, ItemPrice, 0, SessionHelper.UserID, strWhatWhereAction,SessionHelper.EnterPriceID, "Pull");
                                        
                                        QBItemQOHProcess((Guid)objItemmasterDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "Pull Edit");

                                        #endregion

                                        if (oItemPullInfo != null && oItemPullInfo.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                        {
                                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                            objWOLDAL.UpdateWOItemAndTotalCost(oItemPullInfo.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        }
                                    }
                                }
                            }

                            #endregion

                            oReturn.Add(oItemPullInfo);
                        }

                        #endregion
                    }
                }

                //if (isFromPull)
                //{
                //    // need to as immedit pull email should be fire?
                //    try
                //    {
                //        List<Guid> listpullGUIDs = oReturn.Where(p => ((p.ErrorMessage ?? string.Empty) == string.Empty || (p.ErrorMessage ?? string.Empty).ToLower() == "ok") && (p.PullGUID ?? Guid.Empty) != Guid.Empty).ToList().Select(x => x.PullGUID.GetValueOrDefault(Guid.Empty)).ToList();

                //        string pullGUIDs = string.Join(",", listpullGUIDs);
                //        string dataGUIDs = "<DataGuids>" + pullGUIDs + "</DataGuids>";
                //        string eventName = "OPC";
                //        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                //        NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                //        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                //        if (lstNotification != null && lstNotification.Count > 0)
                //        {
                //            objNotificationDAL.SendMailForImmediate(lstNotification, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, eTurnsScheduleDBName, dataGUIDs);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                //    }

                //}                
            }

            return Json(oReturn);
        }

        #region For Credit Edit

        public JsonResult GetItemLocationsWithLotSerialsForCreditEdit(int maxRows, string name_startsWith, Guid? ItemGuid, Guid? PullGUID, long BinID, string prmSerialLotNos = null, Guid? materialStagingGUID = null)
        {
            bool IsStagginLocation = false;

            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO;
            if (materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForRequisition(ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, false, string.Empty, IsStagginLocation, materialStagingGUID.GetValueOrDefault(Guid.Empty));
            }
            else
            {
                objItemLocationLotSerialDTO = objPullDetails.GetItemLocationsWithLotSerialsForCreditEdit(ItemGuid.GetValueOrDefault(Guid.Empty), PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, 0, false, string.Empty, IsStagginLocation);
            }

            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }
            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePullCreditEdit(List<ItemInfoToCredit> CreditDetails)
        {
            PullTransactionDAL objPullTransactionMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            PullDetailsDAL objPullDetails = new PullDetailsDAL(this.enterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            PullMasterDAL objpullMasterDAL = this.pullMasterDAL;
            ProjectMasterDAL projDAL = new ProjectMasterDAL(this.enterPriseDBName);
            ProjectSpendItemsDAL objPrjSpenItmDAL = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBINDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingPullDetailDAL objMSPDetailsDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,IsIgnoreCreditRule,IsProjectSpendMandatory";
            RoomDTO objRoomDTO = commonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            string message = string.Empty;
            string ErrorMessage = string.Empty;
            var enterpriseId = SessionHelper.EnterPriceID;

            foreach (var item in CreditDetails)
            {
                double OldCreditQuantity = 0;
                double NewCreditQuantity = item.Quantity;
                string strWhatWhereAction = "EPQ" + DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

                //-------------------------------------Get Item Master-------------------------------------
                //

                ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGuid);

                //-----------------------------------------------------------------------------------------
                //

                #region take Pull Master data from pull guid

                PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
                PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

                objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(item.PullGUID.GetValueOrDefault(Guid.Empty));
                OldCreditQuantity = objoldPullMasterData.PoolQuantity.GetValueOrDefault(0);
                objnewPullMasterData = objoldPullMasterData;

                //need to update oter fileds

                #endregion

                #region take Pull details data from pull guid

                List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();

                lstloldPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(item.PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                #endregion

                #region Get Project name by guid

                ProjectMasterDTO projDTO = new ProjectMasterDTO();
                if (objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    projDTO = projDAL.GetRecord(objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                #endregion

                ItemInfoToCredit oItemCreditInfo = item;
                oItemCreditInfo.ErrorList = new List<PullErrorInfo>();
                oItemCreditInfo.EditedFrom = objnewPullMasterData.EditedFrom;
                oItemCreditInfo.PullGUID = objnewPullMasterData.GUID;
                oItemCreditInfo.SupplierAccountGuid = objnewPullMasterData.SupplierAccountGuid;
                oItemCreditInfo.PullOrderNumber = objnewPullMasterData.PullOrderNumber;
                oItemCreditInfo.ProjectSpendGUID = objnewPullMasterData.ProjectSpendGUID;
                oItemCreditInfo.ProjectName = (projDTO != null ? projDTO.ProjectSpendName : null);
                oItemCreditInfo.WOGuid = objnewPullMasterData.WorkOrderDetailGUID;
                oItemCreditInfo.UDF1 = objnewPullMasterData.UDF1;
                oItemCreditInfo.UDF2 = objnewPullMasterData.UDF2;
                oItemCreditInfo.UDF3 = objnewPullMasterData.UDF3;
                oItemCreditInfo.UDF4 = objnewPullMasterData.UDF4;
                oItemCreditInfo.UDF5 = objnewPullMasterData.UDF5;

                if (objoldPullMasterData.BinID != 0)
                {
                    BinMasterDTO objBin = new BinMasterDAL(this.enterPriseDBName).GetBinByID(objnewPullMasterData.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objBin != null)
                        oItemCreditInfo.Bin = objBin.BinNumber;
                }

                double ItemCost = lstloldPullDetailsDTO.FirstOrDefault().ItemCost.GetValueOrDefault(0);
                double ItemPrice = lstloldPullDetailsDTO.FirstOrDefault().ItemPrice.GetValueOrDefault(0);

                oItemCreditInfo.ItemGuid = objItemmasterDTO.GUID;
                oItemCreditInfo.IsModelShow = false;
                oItemCreditInfo.ItemNumber = objItemmasterDTO.ItemNumber;
                oItemCreditInfo.ItemType = objItemmasterDTO.ItemType;

                List<ItemLocationDetailsDTO> itemLocations = null;
                ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(this.enterPriseDBName);
                ItemMasterDAL objItemDAL = null;
                List<Guid> ItemGuids = new List<Guid>();

                #region Revert back previous pull quantity for that pull from pull detail and item location table
                /// new entry done like previous deleted and new added
                /// existing serial but not selected from current transaction
                /// 

                List<PullDetailsDTO> lstRevertedPullDetailsForExec = new List<PullDetailsDTO>();

                List<string> lstseriallotexp = new List<string>();
                List<KeyValDTO> seriallotexp = new List<KeyValDTO>();
                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    seriallotexp = (from x in oItemCreditInfo.PrevPullsToCredit
                                           select new KeyValDTO()
                                           {
                                             key = x.Serial,
                                             value = Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy")
                                           }).ToList();
                    //seriallotexp = oItemCreditInfo.PrevPullsToCredit.Select(x => x.Serial, x => Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"));
                }
                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    seriallotexp = (from x in oItemCreditInfo.PrevPullsToCredit
                                    select new KeyValDTO()
                                    {
                                        key = x.Lot,
                                        value = Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy")
                                    }).ToList();
                    //seriallotexp = oItemCreditInfo.PrevPullsToCredit.Distinct().ToList(x => x.Lot, x => Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"));
                }
                else if (objItemmasterDTO.SerialNumberTracking)
                {
                    lstseriallotexp = oItemCreditInfo.PrevPullsToCredit.Select(x => x.Serial).ToList();
                }
                else if (objItemmasterDTO.LotNumberTracking)
                {
                    lstseriallotexp = oItemCreditInfo.PrevPullsToCredit.Select(x => x.Lot).ToList();
                }
                else if (objItemmasterDTO.DateCodeTracking)
                {
                    lstseriallotexp = oItemCreditInfo.PrevPullsToCredit.Select(x => Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy")).ToList();
                }

                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !seriallotexp.Any(z => x.SerialNumber == z.key && x.Expiration == z.value) && x.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                    {
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                            objRevertedPullDetailsForExec.SerialNumber = pullDetailsDTO.SerialNumber;
                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                        }
                    }
                }
                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(item1 => !seriallotexp.Any(item2 => item1.LotNumber == item2.key && item1.Expiration == item2.value) && item1.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                    {
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                        }
                    }
                }
                else if (objItemmasterDTO.SerialNumberTracking)
                {
                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.SerialNumber) && x.PoolQuantity.GetValueOrDefault(0) > 0).ToList())
                    {
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                            objRevertedPullDetailsForExec.SerialNumber = pullDetailsDTO.SerialNumber;
                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                        }
                    }
                }
                else if (objItemmasterDTO.LotNumberTracking)
                {
                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.LotNumber)).ToList())
                    {
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                        }
                    }
                }
                else if (objItemmasterDTO.DateCodeTracking)
                {
                    foreach (PullDetailsDTO pullDetailsDTO in lstloldPullDetailsDTO.Where(x => !lstseriallotexp.Contains(x.Expiration)).ToList())
                    {
                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                        {
                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.ConsignedQuantity =  pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                        }
                    }
                }

                #endregion

                #region take Pull details data from pull guid after revert back existing serial but not selected from current transaction

                List<PullDetailsDTO> lstafterrevertPullDetailsDTO = new List<PullDetailsDTO>();
                lstafterrevertPullDetailsDTO = objPullDetails.GetPullDetailsByPullGuid(item.PullGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.PoolQuantity.GetValueOrDefault(0) > 0).ToList();
                if (lstRevertedPullDetailsForExec.Count > 0)
                {
                    lstafterrevertPullDetailsDTO = lstafterrevertPullDetailsDTO.Where(item1 => !lstRevertedPullDetailsForExec.Any(item2 => item1.GUID == item2.GUID)).ToList();
                }

                #endregion

                if (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    oItemCreditInfo.PrevPullsToCredit = oItemCreditInfo.PrevPullsToCredit.Where(item1 => !lstafterrevertPullDetailsDTO.Any(item2 => item1.Serial == item2.SerialNumber && Convert.ToDateTime(item1.ExpireDate).ToString("MM/dd/yyyy") == item2.Expiration)).ToList();
                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);
                }
                else if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                {
                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => seriallotexp.Any(z => x.LotNumber == z.key && x.Expiration == z.value)).ToList();

                    lstoldserialDetailsDTO =
                                (from c in lstoldserialDetailsDTO
                                 group c by new
                                 {
                                     c.LotNumber,
                                     c.Expiration
                                 } into gcs
                                 select new PullDetailsDTO()
                                 {
                                     LotNumber = gcs.Key.LotNumber,
                                     Expiration = gcs.Key.Expiration,
                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                 }).ToList();

                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);

                    List<PullDetailToCredit> lstnewserialDetailsDTO = new List<PullDetailToCredit>();
                    lstnewserialDetailsDTO = oItemCreditInfo.PrevPullsToCredit.Where(x => seriallotexp.Any(z => x.Lot == z.key && Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy") == z.value)).ToList();

                    lstnewserialDetailsDTO =
                               (from c in lstnewserialDetailsDTO
                                group c by new
                                {
                                    c.Lot,
                                    c.ExpireDate
                                } into gcs
                                select new PullDetailToCredit()
                                {
                                    Lot = gcs.Key.Lot,
                                    ExpireDate = gcs.Key.ExpireDate,                                   
                                    Qty = (gcs.Sum(x => x.Qty))
                                }).ToList();

                    foreach (PullDetailToCredit pullDetails in lstnewserialDetailsDTO)
                    {
                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot && x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot && x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                        double newLotPullQuantity = (pullDetails.Qty);

                        if (newLotPullQuantity < oldLotPullQuantity)
                        {
                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot && x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).ToList())
                            {
                                if (rmainingQty > 0)
                                {
                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                    }
                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.ConsignedQuantity = rmainingQty;
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                            }
                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = 0;
                                    }
                                }
                            }
                            pullDetails.Qty = 0;
                        }
                        else if (newLotPullQuantity > oldLotPullQuantity)
                        {
                            pullDetails.Qty = (newLotPullQuantity - oldLotPullQuantity);
                        }
                        else
                        {
                            pullDetails.Qty = 0;
                        }
                    }

                    oItemCreditInfo.PrevPullsToCredit = lstnewserialDetailsDTO.Where(x => x.Qty > 0).ToList();
                    oItemCreditInfo.Quantity = lstnewserialDetailsDTO.Sum(x => x.Qty);
                }
                else if (objItemmasterDTO.SerialNumberTracking)
                {
                    oItemCreditInfo.PrevPullsToCredit = oItemCreditInfo.PrevPullsToCredit.Where(item1 => !lstafterrevertPullDetailsDTO.Any(item2 => item1.Serial == item2.SerialNumber)).ToList();
                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);
                }
                else if (objItemmasterDTO.LotNumberTracking)
                {
                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => lstseriallotexp.Contains(x.LotNumber)).ToList();

                    lstoldserialDetailsDTO =
                                (from c in lstoldserialDetailsDTO
                                 group c by new
                                 {
                                     c.LotNumber
                                 } into gcs
                                 select new PullDetailsDTO()
                                 {
                                     LotNumber = gcs.Key.LotNumber,
                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                 }).ToList();

                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);

                    List<PullDetailToCredit> lstnewserialDetailsDTO = new List<PullDetailToCredit>();
                    lstnewserialDetailsDTO = oItemCreditInfo.PrevPullsToCredit.Where(x => lstseriallotexp.Contains(x.Lot)).ToList();

                    lstnewserialDetailsDTO =
                               (from c in lstnewserialDetailsDTO
                                group c by new
                                {
                                    c.Lot
                                } into gcs
                                select new PullDetailToCredit()
                                {
                                    Lot = gcs.Key.Lot,
                                    Qty = (gcs.Sum(x => x.Qty))
                                }).ToList();

                    foreach (PullDetailToCredit pullDetails in lstnewserialDetailsDTO)
                    {
                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                        double newLotPullQuantity = (pullDetails.Qty);

                        if (newLotPullQuantity < oldLotPullQuantity)
                        {
                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.LotNumber == pullDetails.Lot).ToList())
                            {
                                if (rmainingQty > 0)
                                {
                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                    }
                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            objRevertedPullDetailsForExec.LotNumber = pullDetailsDTO.LotNumber;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.ConsignedQuantity = rmainingQty;
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                            }
                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = 0;
                                    }
                                }
                            }
                            pullDetails.Qty = 0;
                        }
                        else if (newLotPullQuantity > oldLotPullQuantity)
                        {
                            pullDetails.Qty = (newLotPullQuantity - oldLotPullQuantity);
                        }
                        else
                        {
                            pullDetails.Qty = 0;
                        }
                    }

                    oItemCreditInfo.PrevPullsToCredit = lstnewserialDetailsDTO.Where(x => x.Qty > 0).ToList();
                    oItemCreditInfo.Quantity = lstnewserialDetailsDTO.Sum(x => x.Qty);
                }
                else if (objItemmasterDTO.DateCodeTracking)
                {
                    List<PullDetailsDTO> lstoldserialDetailsDTO = new List<PullDetailsDTO>();
                    lstoldserialDetailsDTO = lstafterrevertPullDetailsDTO.Where(x => lstseriallotexp.Contains(x.Expiration)).ToList();

                    lstoldserialDetailsDTO =
                                (from c in lstoldserialDetailsDTO
                                 group c by new
                                 {
                                     c.Expiration
                                 } into gcs
                                 select new PullDetailsDTO()
                                 {
                                     Expiration = gcs.Key.Expiration,
                                     ConsignedQuantity = gcs.Sum(x => x.ConsignedQuantity),
                                     CustomerOwnedQuantity = gcs.Sum(x => x.CustomerOwnedQuantity),
                                     PoolQuantity = (gcs.Sum(x => x.ConsignedQuantity) + gcs.Sum(x => x.CustomerOwnedQuantity))
                                 }).ToList();

                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);

                    List<PullDetailToCredit> lstnewserialDetailsDTO = new List<PullDetailToCredit>();
                    lstnewserialDetailsDTO = oItemCreditInfo.PrevPullsToCredit.Where(x => lstseriallotexp.Contains(Convert.ToDateTime(x.ExpireDate).ToString("MM/dd/yyyy"))).ToList();

                    lstnewserialDetailsDTO =
                               (from c in lstnewserialDetailsDTO
                                group c by new
                                {
                                    c.ExpireDate
                                } into gcs
                                select new PullDetailToCredit()
                                {
                                    ExpireDate = gcs.Key.ExpireDate,
                                    Qty = (gcs.Sum(x => x.Qty))
                                }).ToList();

                    foreach (PullDetailToCredit pullDetails in lstnewserialDetailsDTO)
                    {
                        double oldLotConsignedQuantity = lstoldserialDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).Sum(y => y.ConsignedQuantity.GetValueOrDefault(0));
                        double oldLotCustomerOwnedQuantity = lstoldserialDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).Sum(y => y.CustomerOwnedQuantity.GetValueOrDefault(0));
                        double oldLotPullQuantity = (oldLotConsignedQuantity + oldLotCustomerOwnedQuantity);

                        double newLotPullQuantity = (pullDetails.Qty);

                        if (newLotPullQuantity < oldLotPullQuantity)
                        {
                            double rmainingQty = (oldLotPullQuantity - newLotPullQuantity);
                            foreach (PullDetailsDTO pullDetailsDTO in lstafterrevertPullDetailsDTO.Where(x => x.Expiration == Convert.ToDateTime(pullDetails.ExpireDate).ToString("MM/dd/yyyy")).ToList())
                            {
                                if (rmainingQty > 0)
                                {
                                    if (pullDetailsDTO.PoolQuantity <= rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PoolQuantity = 0;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.ConsignedQuantity = pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0);
                                            if (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0);

                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = rmainingQty - (pullDetailsDTO.PoolQuantity.GetValueOrDefault(0));
                                    }
                                    else if (pullDetailsDTO.PoolQuantity > rmainingQty)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        if (objItemLocationDetailsDTO != null && objItemLocationDetailsDTO.ID > 0)
                                        {
                                            PullDetailsDTO objRevertedPullDetailsForExec = new PullDetailsDTO();

                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.PoolQuantity = (pullDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - rmainingQty);
                                            }
                                            objRevertedPullDetailsForExec.Expiration = pullDetailsDTO.Expiration;
                                            objRevertedPullDetailsForExec.PULLGUID = pullDetailsDTO.PULLGUID;
                                            objRevertedPullDetailsForExec.GUID = pullDetailsDTO.GUID;
                                            objRevertedPullDetailsForExec.ItemGUID = pullDetailsDTO.ItemGUID;

                                            objRevertedPullDetailsForExec.ItemLocationDetailGUID = pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty);
                                            if (pullDetailsDTO.ConsignedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.ConsignedQuantity = rmainingQty;
                                            }
                                            if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                            {
                                                objRevertedPullDetailsForExec.CustomerOwnedQuantity = rmainingQty;
                                            }
                                            lstRevertedPullDetailsForExec.Add(objRevertedPullDetailsForExec);
                                        }
                                        rmainingQty = 0;
                                    }
                                }
                            }
                            pullDetails.Qty = 0;
                        }
                        else if (newLotPullQuantity > oldLotPullQuantity)
                        {
                            pullDetails.Qty = (newLotPullQuantity - oldLotPullQuantity);
                        }
                        else
                        {
                            pullDetails.Qty = 0;
                        }
                    }

                    oItemCreditInfo.PrevPullsToCredit = lstnewserialDetailsDTO.Where(x => x.Qty > 0).ToList();
                    oItemCreditInfo.Quantity = lstnewserialDetailsDTO.Sum(x => x.Qty);
                }

                if (oItemCreditInfo.Quantity > 0)
                {
                    oItemCreditInfo.PrevPullsToCredit = pullMasterDAL.GetPreviousPullsForCredit(oItemCreditInfo, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID,SessionHelper.EnterPriceID,ResourceHelper.CurrentCult.Name);
                    oItemCreditInfo.Quantity = oItemCreditInfo.PrevPullsToCredit.Sum(x => x.Qty);

                    if (!string.IsNullOrEmpty(oItemCreditInfo.ErrorMessage))
                    {
                        return Json(new { Message = oItemCreditInfo.ErrorMessage, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                    }

                    if (!objRoomDTO.IsIgnoreCreditRule)
                    {
                        if (oItemCreditInfo.PrevPullsToCredit == null || oItemCreditInfo.PrevPullsToCredit.Count == 0)
                        {
                            oItemCreditInfo.IsModelShow = true;
                            oItemCreditInfo.ErrorMessage = string.Format(ResPullMaster.CreditQtyGreaterThanPreviousPullQty, oItemCreditInfo.Quantity); 
                        }
                        if (oItemCreditInfo.IsModelShow && !string.IsNullOrWhiteSpace(oItemCreditInfo.ErrorMessage))
                        {
                            return Json(new { Message = string.Format(ResPullMaster.CreditQtyGreaterThanPreviousPullQty, oItemCreditInfo.Quantity), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(oItemCreditInfo.ErrorMessage))
                    {
                        #region  execution  for revert back quantity
                        bool isneedtorevertpullCost = false;

                        List<PullCreditHistoryDTO> lstPullCreditHistory = new List<PullCreditHistoryDTO>();
                        lstPullCreditHistory = objPullDetails.GetCreditHistoryDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                        foreach (PullDetailsDTO revertData in lstRevertedPullDetailsForExec)
                        {
                            PullCreditHistoryDTO pullCreditHistoryDTO = new PullCreditHistoryDTO();
                            pullCreditHistoryDTO = lstPullCreditHistory.Where(x => x.CreditDetailGuid == revertData.GUID).FirstOrDefault();
                            
                            isneedtorevertpullCost = true;
                            PullDetailsDTO pullDetailsDTO = objPullDetails.GetPullDetailByGuidNormal(revertData.GUID);
                            if (pullDetailsDTO != null && pullDetailsDTO.ID > 0
                                && revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty))
                            {
                                ItemLocationDetailsDTO itemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                itemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (itemLocationDetailsDTO != null && itemLocationDetailsDTO.ID > 0)
                                {
                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                    {
                                        itemLocationDetailsDTO.ConsignedQuantity = itemLocationDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - revertData.ConsignedQuantity;
                                    }
                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                    {
                                        itemLocationDetailsDTO.CustomerOwnedQuantity = itemLocationDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - revertData.CustomerOwnedQuantity;
                                    }
                                    itemLocationDetailsDTO.EditedFrom = "Web";
                                    itemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                    itemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;

                                    objItemLocationDetailsDAL.Edit(itemLocationDetailsDTO);

                                    if (pullCreditHistoryDTO != null)
                                    {
                                        if (pullDetailsDTO.ConsignedQuantity > 0)
                                        {
                                            pullCreditHistoryDTO.CreditConsignedQuantity = revertData.PoolQuantity;
                                        }
                                        if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                        {
                                            pullCreditHistoryDTO.CreditCustomerOwnedQuantity = revertData.PoolQuantity;
                                        }
                                    }

                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                    {
                                        pullDetailsDTO.ConsignedQuantity = revertData.PoolQuantity;
                                    }
                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                    {
                                        pullDetailsDTO.CustomerOwnedQuantity = revertData.PoolQuantity;
                                    }
                                    pullDetailsDTO.EditedFrom = "Web";
                                    pullDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                    pullDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                    pullDetailsDTO.PoolQuantity = (pullDetailsDTO.ConsignedQuantity + pullDetailsDTO.CustomerOwnedQuantity);
                                    objPullDetails.Edit(pullDetailsDTO);
                                    
                                    //EditCreditHistory
                                    objPullDetails.UpdateCreditHistory(pullCreditHistoryDTO);

                                    //EDit PullDetails for Credit Quantity Update
                                    objPullDetails.UpdatePullDetailsForCreditQuantity(pullCreditHistoryDTO);

                                    // Edit PullMaster For Creedit Quantity
                                    objPullDetails.UpdatePullMasterForCreditQuantity(pullCreditHistoryDTO);
                                }
                            }
                        }

                        #region Update PullQty,PullCost,PullPrice and projectspend after revert back
                        if (isneedtorevertpullCost)
                        {
                            List<PullDetailsDTO> lstPullDtl = objPullDetails.GetPullDetailsByPullGuidPlain(objoldPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (lstPullDtl != null && lstPullDtl.Count > 0)
                            {
                                #region Get Item onHand Quanttiy

                                double OnHandQuantity = objItemmasterDTO.OnHandQuantity.GetValueOrDefault(0);
                                ItemLocationDetailsDTO itemlocationQuatnity = objItemLocationDetailsDAL.GetItemLocationDetailsByItemGUID(objoldPullMasterData.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (itemlocationQuatnity != null && itemlocationQuatnity.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    OnHandQuantity = (itemlocationQuatnity.CustomerOwnedQuantity.GetValueOrDefault(0)
                                                     +
                                                     itemlocationQuatnity.ConsignedQuantity.GetValueOrDefault(0)
                                                        );
                                }
                                #endregion


                                double OldPullCost = objnewPullMasterData.PullCost ?? 0;
                                double OldPullQuantity = objnewPullMasterData.PoolQuantity ?? 0;

                                objoldPullMasterData.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                objoldPullMasterData.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                                objoldPullMasterData.PoolQuantity = (
                                                    lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                        +
                                                    lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                                    );

                                objoldPullMasterData.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                                objoldPullMasterData.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                                //objoldPullMasterData.WhatWhereAction = strWhatWhereAction;

                                objoldPullMasterData.ItemOnhandQty = OnHandQuantity;
                                objoldPullMasterData.ItemStageQty = objItemmasterDTO.StagedQuantity;
                                objoldPullMasterData.ItemLocationOnHandQty = 0;

                                ItemLocationQTYDTO objItemLocationQuantity = objItemLocationDetailsDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, objItemmasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                                if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                                {
                                    objoldPullMasterData.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                                }

                                objpullMasterDAL.EditForPullQty(objoldPullMasterData);
                                //objpullMasterDAL.Edit(objoldPullMasterData);

                                double DiffPullCost = (OldPullCost - (objoldPullMasterData.PullCost ?? 0));
                                double DiffPoolQuantity = (OldPullQuantity - (objoldPullMasterData.PoolQuantity ?? 0));

                                if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    objpullMasterDAL.UpdateProjectSpendWithCostEditCredit(objItemmasterDTO, objoldPullMasterData, DiffPullCost, DiffPoolQuantity, objoldPullMasterData.ProjectSpendGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }

                        #endregion

                        #endregion

                        /* WI-4693-Allow specified rooms to ignore credit rules */

                        if (!objRoomDTO.IsIgnoreCreditRule)
                        {
                            List<PullDetailsDTO> prepulls = pullMasterDAL.GetPrevPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);
                            itemLocations = new List<ItemLocationDetailsDTO>();
                            foreach (var prePullItem in prepulls)
                            {
                                if (prePullItem.SerialNumber != null && (!string.IsNullOrWhiteSpace(prePullItem.SerialNumber)))
                                {
                                    #region Validation for Serial number alaredy credited or not

                                    if (objItemmasterDTO.SerialNumberTracking
                                        || (objItemmasterDTO.SerialNumberTracking && objItemmasterDTO.DateCodeTracking))
                                    {
                                        string lstSerialNumber = string.Empty;
                                        string serailErrorMessage = commonDAL.CheckDuplicateSerialNumbers(prePullItem.SerialNumber, 0, SessionHelper.RoomID, SessionHelper.CompanyID, objItemmasterDTO.GUID);

                                        if (serailErrorMessage.ToLower().Trim() == "duplicate")
                                        {
                                            if (lstSerialNumber != string.Empty)
                                                lstSerialNumber = lstSerialNumber + " , " + prePullItem.SerialNumber;
                                            else
                                                lstSerialNumber = prePullItem.SerialNumber;
                                        }
                                        if (!string.IsNullOrEmpty(lstSerialNumber))
                                        {
                                            oItemCreditInfo.ErrorMessage = string.Format(ResPullMaster.CantEditCreditForCreditedSerials, lstSerialNumber);
                                            return Json(new { Message = string.Format(ResPullMaster.CantEditCreditForCreditedSerials, lstSerialNumber), Status = "Fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }

                                    #endregion

                                    if (ildDAL.CheckSerialExistsOrNot(prePullItem.SerialNumber, item.ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID))
                                    {
                                        itemLocations.Add(pullMasterDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                    }
                                }
                                else if (prePullItem.LotNumber != null && (!string.IsNullOrWhiteSpace(prePullItem.LotNumber))
                                        && prePullItem.Expiration != null && (!string.IsNullOrWhiteSpace(prePullItem.Expiration)))
                                {

                                    #region Validation for Lot and Expiration item
                                    if (objItemmasterDTO.LotNumberTracking && objItemmasterDTO.DateCodeTracking)
                                    {
                                        string lstLotNumber = string.Empty;
                                        DateTime dtExpDate;
                                        DateTime.TryParseExact(Convert.ToDateTime(prePullItem.Expiration).ToString(SessionHelper.RoomDateFormat), SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);
                                        //DateTime.TryParseExact(prePullItem.Expiration, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture, System.Globalization.DateTimeStyles.None, out dtExpDate);

                                        if (dtExpDate <= DateTime.MinValue)
                                        {
                                            oItemCreditInfo.ErrorMessage = ResPullMaster.EnterValidExpirationDate;
                                            return Json(new { Message = ResPullMaster.EnterValidExpirationDate, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                                        }

                                        string msg = commonDAL.CheckDuplicateLotAndExpiration(prePullItem.LotNumber, prePullItem.Expiration, dtExpDate, 0, SessionHelper.RoomID, SessionHelper.CompanyID, objItemmasterDTO.GUID,SessionHelper.UserID,SessionHelper.EnterPriceID);
                                        if (!string.IsNullOrWhiteSpace(msg) && !msg.ToLower().Equals("ok"))
                                        {
                                            if (lstLotNumber != string.Empty)
                                                lstLotNumber = lstLotNumber + " , " + msg;
                                            else
                                                lstLotNumber = msg;
                                        }
                                        if (!string.IsNullOrEmpty(lstLotNumber))
                                        {
                                            oItemCreditInfo.ErrorMessage = lstLotNumber;
                                            return Json(new { Message = lstLotNumber, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                                        }
                                        itemLocations.Add(pullMasterDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                    }
                                    #endregion
                                }
                                else
                                {
                                    itemLocations.Add(pullMasterDAL.ConvertPullDetailtoItemLocationDetail(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }
                            }

                            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                            long SessionUserId = SessionHelper.UserID;
                            if (ildDAL.ItemLocationDetailsEditForCreditPullnew(itemLocations, objoldPullMasterData.PoolQuantity.GetValueOrDefault(0), objnewPullMasterData.PoolQuantity.GetValueOrDefault(0), "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,enterpriseId, "credit", strWhatWhereAction))
                            {
                                pullMasterDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                pullMasterDAL.InsertPullEditHistory(objoldPullMasterData.GUID, NewCreditQuantity, OldCreditQuantity, strWhatWhereAction);

                                if (oItemCreditInfo != null && oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                    objWOLDAL.UpdateWOItemAndTotalCost(oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                        }
                        else
                        {
                            #region WI-4693-Allow specified rooms to ignore credit rules

                            List<PullDetailsDTO> prepulls = pullMasterDAL.GetPrevPull(item, SessionHelper.RoomID, SessionHelper.CompanyID);

                            double TotalAvailablePulls = prepulls.Sum(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)));
                            double TotalRemainingCredit = (item.Quantity - TotalAvailablePulls);
                            List<PullDetailsDTO> pulls = new List<PullDetailsDTO>();
                            if (TotalRemainingCredit > 0)
                            {
                                pulls = pullMasterDAL.GetPrevPullForCreditEntry(prepulls, item, TotalRemainingCredit, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.RoomDateFormat, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                                prepulls.AddRange(pulls);
                            }
                            bool IsValid = true;
                            if (prepulls.Where(x => x.EditedFrom == "Fail").Count() > 0)
                            {
                                IsValid = false;
                            }

                            if (IsValid)
                            {
                                itemLocations = new List<ItemLocationDetailsDTO>();
                                foreach (var prePullItem in prepulls)
                                {
                                    itemLocations.Add(pullMasterDAL.ConvertPullDetailtoItemLocationDetailForCreditRule(prePullItem, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.RoomDateFormat));
                                }
                                long SessionUserId = SessionHelper.UserID;
                                List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
                                if (ildDAL.ItemLocationDetailsEditForCreditPullnew(itemLocations,objoldPullMasterData.PoolQuantity.GetValueOrDefault(0),objnewPullMasterData.PoolQuantity.GetValueOrDefault(0), "Credit", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,enterpriseId, "credit", strWhatWhereAction))
                                {
                                    pullMasterDAL.UpdatePullRecordsForCreditQuantity(prepulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, lstCreditGuids);
                                    pullMasterDAL.InsertPullEditHistory(objoldPullMasterData.GUID, NewCreditQuantity, OldCreditQuantity, strWhatWhereAction);
                                    if (pulls != null && pulls.Count > 0)
                                    {
                                        List<PullDetailsDTO> lstPulls = new List<PullDetailsDTO>();
                                        lstPulls.AddRange(pulls);
                                        pullMasterDAL.InsertintoCreditHistory(lstPulls, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Pull Credit");
                                    }
                                    if (oItemCreditInfo != null && oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                    {
                                        WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                                        objWOLDAL.UpdateWOItemAndTotalCost(oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                            }
                            else
                            {
                                ErrorMessage = ErrorMessage + prepulls[0].AddedFrom + Environment.NewLine;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        return Json(new { Message = oItemCreditInfo.ErrorMessage, Status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (lstRevertedPullDetailsForExec != null && lstRevertedPullDetailsForExec.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(oItemCreditInfo.ErrorMessage))
                    {
                        #region  execution  for revert back quantity
                        List<PullCreditHistoryDTO> lstPullCreditHistory = new List<PullCreditHistoryDTO>();
                        lstPullCreditHistory = objPullDetails.GetCreditHistoryDetailsByPullGuid(objnewPullMasterData.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                        
                        foreach (PullDetailsDTO revertData in lstRevertedPullDetailsForExec)
                        {
                            PullCreditHistoryDTO pullCreditHistoryDTO = new PullCreditHistoryDTO();
                            pullCreditHistoryDTO = lstPullCreditHistory.Where(x => x.CreditDetailGuid == revertData.GUID).FirstOrDefault();

                            PullDetailsDTO pullDetailsDTO = objPullDetails.GetPullDetailByGuidNormal(revertData.GUID);
                            if (pullDetailsDTO != null && pullDetailsDTO.ID > 0
                                && revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty) == pullDetailsDTO.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty))
                            {
                                ItemLocationDetailsDTO itemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                itemLocationDetailsDTO = objItemLocationDetailsDAL.GetItemLocationDetailsByLocationGuid(revertData.ItemLocationDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (itemLocationDetailsDTO != null && itemLocationDetailsDTO.ID > 0)
                                {
                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                    {
                                        itemLocationDetailsDTO.ConsignedQuantity = itemLocationDetailsDTO.ConsignedQuantity.GetValueOrDefault(0) - revertData.ConsignedQuantity;
                                    }
                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                    {
                                        itemLocationDetailsDTO.CustomerOwnedQuantity = itemLocationDetailsDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - revertData.CustomerOwnedQuantity;
                                    }
                                    itemLocationDetailsDTO.EditedFrom = "Web";
                                    itemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                    itemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;

                                    objItemLocationDetailsDAL.Edit(itemLocationDetailsDTO);

                                    if (pullCreditHistoryDTO != null)
                                    {
                                        if (pullDetailsDTO.ConsignedQuantity > 0)
                                        {
                                            pullCreditHistoryDTO.CreditConsignedQuantity = revertData.PoolQuantity;
                                        }
                                        if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                        {
                                            pullCreditHistoryDTO.CreditCustomerOwnedQuantity = revertData.PoolQuantity;
                                        }
                                    }

                                    if (pullDetailsDTO.ConsignedQuantity > 0)
                                    {
                                        pullDetailsDTO.ConsignedQuantity = revertData.PoolQuantity;
                                    }
                                    if (pullDetailsDTO.CustomerOwnedQuantity > 0)
                                    {
                                        pullDetailsDTO.CustomerOwnedQuantity = revertData.PoolQuantity;
                                    }
                                    pullDetailsDTO.EditedFrom = "Web";
                                    pullDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                    pullDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                    pullDetailsDTO.PoolQuantity = (pullDetailsDTO.ConsignedQuantity + pullDetailsDTO.CustomerOwnedQuantity);
                                    objPullDetails.Edit(pullDetailsDTO);
                                    
                                    //EditCreditHistory
                                    objPullDetails.UpdateCreditHistory(pullCreditHistoryDTO);

                                    //EDit PullDetails for Credit Quantity Update
                                    objPullDetails.UpdatePullDetailsForCreditQuantity(pullCreditHistoryDTO);

                                    // Edit PullMaster For Creedit Quantity
                                    objPullDetails.UpdatePullMasterForCreditQuantity(pullCreditHistoryDTO);
                                }
                            }
                        }

                        objPullTransactionMasterDAL.PullItemEditCreditQuantityForRevertExe(oItemCreditInfo, objItemmasterDTO, ItemCost, ItemPrice, 0, SessionHelper.UserID, strWhatWhereAction, enterpriseId, "Pull");

                        #endregion

                        if (oItemCreditInfo != null && oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(this.enterPriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(oItemCreditInfo.WOGuid.GetValueOrDefault(Guid.Empty).ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                        }
                    }
                }

                if (ItemGuids.IndexOf(item.ItemGuid.GetValueOrDefault(Guid.Empty)) < 0)
                    ItemGuids.Add(item.ItemGuid.GetValueOrDefault(Guid.Empty));

                if (objRoomDTO.IsIgnoreCreditRule && !string.IsNullOrEmpty(ErrorMessage))
                    return Json(new { Message = ErrorMessage, Status = false }, JsonRequestBehavior.AllowGet);

                #region "Update Ext Cost And Avg Cost"
                if (ItemGuids.Count > 0)
                {
                    long SessionUserId = SessionHelper.UserID;
                    objItemDAL = new ItemMasterDAL(this.enterPriseDBName);
                    foreach (var guid in ItemGuids)
                    {
                        objItemDAL.UpdateItemCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID, "Web", SessionUserId,enterpriseId);
                        objItemDAL.GetAndUpdateExtCostAndAvgCost(guid, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                    objItemDAL = null;
                }
                #endregion
            }

            return Json(new { Message = ResPullMaster.ItemCredited, Status = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }
}