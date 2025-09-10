using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace eTurnsWeb.Controllers
{

    public partial class ToolController : eTurnsControllerBase
    {





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OpenPopupForToolMoveMaterial(ToolAssetMoveMaterialDTO MoveMTRDTO)
        {

            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }

            ToolMasterDTO itmDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(MoveMTRDTO.ToolGUID.GetValueOrDefault(Guid.Empty));

            MoveMTRDTO.ToolName = itmDTO.ToolName;
            MoveMTRDTO.ToolQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.AvailableToolQty);// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);

            if (MoveMTRDTO.SourceToolBinID > 0)
            {
                ToolLocationDetailsDTO objLOC = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocationDetailsByID(MoveMTRDTO.SourceToolBinID, RoomID, CompanyID);


                if (objLOC != null && objLOC.ID > 0)
                {
                    MoveMTRDTO.SourceLocation = objLOC.ToolLocationName;
                }
                else
                {
                    MoveMTRDTO.SourceLocation = "";
                }



                ToolAssetQuantityDetailDAL objTAQtyDetail = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                ToolAssetQuantityDetailDTO TAQtyDtlDTO = objTAQtyDetail.GetRecordByLocationToolByToolBinId(MoveMTRDTO.ToolGUID.GetValueOrDefault(Guid.Empty), MoveMTRDTO.SourceToolBinID, RoomID, CompanyID);

                if (TAQtyDtlDTO != null)
                {
                    MoveMTRDTO.MoveQuanity = TAQtyDtlDTO.Quantity;
                }



            }
            ViewBag.ToolGUID = MoveMTRDTO.ToolGUID.ToString();
            MoveMTRDTO.IsOnlyFromToolUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_ToolMoveMaterialLocationPopup", MoveMTRDTO);
        }

        public ActionResult OpenPopupForToolMoveMaterialLotSerial(ToolAssetMoveMaterialDTO MoveMTRDTO)
        {
            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }


            ToolMasterDTO itmDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(MoveMTRDTO.ToolGUID.GetValueOrDefault(Guid.Empty));

            MoveMTRDTO.ToolName = itmDTO.ToolName;
            MoveMTRDTO.ToolQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.AvailableToolQty);// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);


            if (MoveMTRDTO.SourceToolBinID > 0)
            {
                ToolLocationDetailsDTO objLOC = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocationDetailsByID(MoveMTRDTO.SourceToolBinID, RoomID, CompanyID);


                if (objLOC != null && objLOC.ID > 0)
                {
                    MoveMTRDTO.SourceLocation = objLOC.ToolLocationName;
                }
                else
                {
                    MoveMTRDTO.SourceLocation = "";
                }


                ToolAssetQuantityDetailDAL objTAQtyDetail = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                ToolAssetQuantityDetailDTO TAQtyDtlDTO = objTAQtyDetail.GetRecordByLocationToolByToolBinId(MoveMTRDTO.ToolGUID.GetValueOrDefault(Guid.Empty), MoveMTRDTO.SourceToolBinID, RoomID, CompanyID);

                if (TAQtyDtlDTO != null)
                {
                    MoveMTRDTO.MoveQuanity = TAQtyDtlDTO.Quantity;
                }

            }
            ViewBag.ToolGUID = MoveMTRDTO.ToolGUID.ToString();
            MoveMTRDTO.IsOnlyFromToolUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_ToolMoveMaterialLocationPopupLotSr", MoveMTRDTO);
        }

        public ActionResult LotSrSelectionForMove(JQueryDataTableParamModel param)
        {
            Guid ToolGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            int intMoveType = 0;
            int.TryParse(Convert.ToString(Request["MoveType"]), out intMoveType);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ToolMasterDTO oItem = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDNormal(ToolGUID);


            int TotalRecordCount = 0;
            ToolAssetQuantityDetailDAL objILD = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);

            List<ToolQuantityLotSerialDTO> lstLotSrs = new List<ToolQuantityLotSerialDTO>();
            List<ToolQuantityLotSerialDTO> retlstLotSrs = new List<ToolQuantityLotSerialDTO>();

            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            if (arrIds.Count() > 0)
            {
                string[] arrSerialLots = new string[arrIds.Count()];
                for (int i = 0; i < arrIds.Count(); i++)
                {
                    string[] arrItem = arrIds[i].Split(new string[] { "{|}" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrItem.Length > 2)
                    {
                        arrSerialLots[i] = arrItem[0] + "_" + arrItem[2];
                        dicSerialLots.Add(arrItem[0] + "_" + arrItem[2], Convert.ToDouble(arrItem[1]));
                    }
                    else if (arrItem.Length > 1)
                    {
                        arrSerialLots[i] = arrItem[0];
                        dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                    }
                    else if (arrItem.Length > 0)
                    {
                        arrSerialLots[i] = arrItem[0];
                    }
                }

                if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                    lstLotSrs = objILD.GetToolLocationsWithLotSerialsForMove(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID);


                retlstLotSrs = lstLotSrs.Where(t => ((arrSerialLots.Contains(t.LotOrSerailNumber + "_" + t.Location) && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))).ToList();

                if (!IsDeleteRowMode)
                {
                    retlstLotSrs = retlstLotSrs.Union(lstLotSrs.Where(t => ((!arrSerialLots.Contains(t.LotOrSerailNumber + "_" + t.Location) && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))).Take(1)).ToList();
                }
            }
            else
            {
                if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                    retlstLotSrs = objILD.GetToolLocationsWithLotSerialsForMove(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Take(1).ToList();

            }

            foreach (ToolQuantityLotSerialDTO item in retlstLotSrs)
            {
                if (dicSerialLots.ContainsKey(item.LotOrSerailNumber + "_" + item.Location) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                {
                    double value = dicSerialLots[item.LotOrSerailNumber + "_" + item.Location];
                    item.QuantityToMove = value;
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


        public JsonResult GetLocationByType(string NameStartWith, Guid ToolGuid, string locType, MoveType moveType, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            ToolAssetQuantityDetailDAL itmLocQtyDAL = null;
            List<ToolAssetQuantityDetailDTO> itmLocQtyList = null;

            if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
            {
                QtyFromate = "N" + SessionHelper.NumberDecimalDigits;
            }
            try
            {
                if (locType == "SL")
                {
                    itmLocQtyDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    itmLocQtyList = itmLocQtyDAL.GetToolLocationWithQtyByToolGuid(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGuid);
                    foreach (var item in itmLocQtyList)
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.GUID = item.LocationGUID.GetValueOrDefault(Guid.Empty);
                        objAutoDTO.ID = item.ToolBinID.GetValueOrDefault(0);
                        objAutoDTO.Key = item.Location + " (" + item.Quantity.ToString() + ")";
                        objAutoDTO.Value = item.Location;
                        objAutoDTO.Quantity = item.Quantity;
                        locations.Add(objAutoDTO);
                    }



                }
                else if (locType == "DL")
                {

                    // Only Bind locations
                    //ItmLocLevelQtyDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    itmLocQtyDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);


                    if (!string.IsNullOrEmpty(NameStartWith) && NameStartWith.Trim().Length > 0)
                    {
                        itmLocQtyList = itmLocQtyDAL.GetToolLocationWithQtyByToolGuid(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGuid).Where(x => (x.Location ?? string.Empty).ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    else
                    {
                        itmLocQtyList = itmLocQtyDAL.GetToolLocationWithQtyByToolGuid(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGuid).ToList();
                    }

                    foreach (var item in itmLocQtyList)
                    {
                        if (locations.FindIndex(x => x.ID == item.ToolBinID) < 0)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                            {
                                GUID = item.LocationGUID.GetValueOrDefault(Guid.Empty),
                                ID = item.ToolBinID.GetValueOrDefault(0),
                                Key = item.Location,
                                Value = item.Location,
                                Quantity = item.Quantity,
                            };

                            locations.Add(objAutoDTO);
                        }
                    }
                    if (locations != null && locations.Count > 0)
                    {
                        locations = locations.OrderBy(x => x.Value).ToList();
                    }

                    if (IsLoadMoreLocations.HasValue)
                    {
                        if (IsLoadMoreLocations.Value)
                        {
                            IEnumerable<LocationMasterDTO> objLocationDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, (NameStartWith ?? string.Empty)).OrderBy(x => x.Location);
                            if (objLocationDTOList != null && objLocationDTOList.Count() > 0)
                            {
                                foreach (var item in objLocationDTOList)
                                {
                                    if (!locations.Any(x => x.Key == item.Location))
                                    {
                                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                        {
                                            GUID = item.GUID,
                                            ID = item.ID,
                                            Key = item.Location ?? string.Empty,
                                            Value = item.Location,
                                            Quantity = 0,
                                        };
                                        locations.Add(objAutoDTO);
                                    }
                                }
                                if (locations != null && locations.Count > 0)
                                {
                                    locations = locations.OrderBy(x => x.Value).ToList();
                                }
                            }
                        }
                        else
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.GUID = Guid.Empty;
                            objAutoDTO.ID = 0;
                            objAutoDTO.Key = ResBin.MoreLocations;
                            objAutoDTO.Value = ResBin.MoreLocations;
                            objAutoDTO.Quantity = 0;
                            locations.Add(objAutoDTO);
                        }
                    }



                }
            }
            catch (Exception)
            {

            }


            if (!string.IsNullOrWhiteSpace(NameStartWith) && locations != null && locations.Count > 0)
            {
                locations = locations.Where(x => x.Value.ToLower().StartsWith(NameStartWith.Trim().ToLower())).ToList();
            }



            return Json(locations, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult ValidateSerialLotNumberInMove(Guid? ToolGuid, string SerialOrLotNumber, string Location, int intMoveType)
        {
            ToolAssetQuantityDetailDAL objILD = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
            ToolQuantityLotSerialDTO objItemLocationLotSerialDTO = null;

            objItemLocationLotSerialDTO = objILD.GetToolLocationsWithLotSerialsForMove(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => (x.Location ?? string.Empty) == (Location ?? string.Empty) && x.LotOrSerailNumber == SerialOrLotNumber).FirstOrDefault();

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ToolQuantityLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = 0;
                objItemLocationLotSerialDTO.ID = 0;
                objItemLocationLotSerialDTO.ToolGUID = ToolGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Location = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }

        public JsonResult GetLotOrSerailNumberListForMove(int maxRows, string name_startsWith, Guid? ToolGuid, int intMoveType)
        {
            ToolAssetQuantityDetailDAL objILD = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
            List<ToolQuantityLotSerialDTO> objItemLocationLotSerialDTO = null;

            if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                objItemLocationLotSerialDTO = objILD.GetToolLocationsWithLotSerialsForMove(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);


            var lstLotSr = objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith))
                .Select(x => new
                {
                    x.LotOrSerailNumber,
                    x.Location,
                    DisplatText = x.LotOrSerailNumber + " (" + (x.Location ?? string.Empty) + ")"
                }).Distinct();



            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult MoveQuantitySourceToDestination(ToolAssetMoveMaterialDTO moveMTRDTO)
        {
            List<string> lstErrors = new List<string>();
            try
            {

                if (ValidateMove(moveMTRDTO, lstErrors))
                {
                    moveMTRDTO.CreatedBy = SessionHelper.UserID;
                    moveMTRDTO.UpdatedBy = SessionHelper.UserID;
                    moveMTRDTO.RoomID = RoomID;
                    moveMTRDTO.CompanyID = CompanyID;
                    moveMTRDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                    moveMTRDTO.CreatedOn = DateTimeUtility.DateTimeNow;

                    string[] stringSeparators = new string[] { };

                    if (string.IsNullOrEmpty(moveMTRDTO.DestinationLocation))
                    {
                        moveMTRDTO.DestinationLocation = "";
                    }
                    else if (!string.IsNullOrEmpty(moveMTRDTO.DestinationLocation) && moveMTRDTO.DestinationLocation.Contains("[||]"))
                    {
                        stringSeparators = moveMTRDTO.DestinationLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.DestinationLocation = stringSeparators[1];
                    }


                    if (!string.IsNullOrEmpty(moveMTRDTO.SourceLocation) && moveMTRDTO.SourceLocation.Contains("[||]"))
                    {
                        stringSeparators = new string[] { };
                        stringSeparators = moveMTRDTO.SourceLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.SourceLocation = stringSeparators[1];
                    }

                    ToolAssetMoveMaterialDAL objMoveDAL = new ToolAssetMoveMaterialDAL(SessionHelper.EnterPriseDBName);
                    //TODO: Chirag 08-08-2017, Do not change AddedFrom and Editedfrom is used in trigger to prevent same twice record in ATT
                    moveMTRDTO.AddedFrom = "Web-Move";
                    moveMTRDTO.EditedFrom = "Web-Move";

                    bool returnStatus = objMoveDAL.MoveTool(moveMTRDTO);

                    return Json(new { Message = "ok", Status = returnStatus, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lstErrors.Add(ex.Message.ToString());
                return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult MoveQuantitySourceToDestinationLotSr(List<ToolAssetMoveMaterialDTO> moveMTRDTOList)
        {
            List<string> lstErrors = new List<string>();
            try
            {


                if (ValidateMoveLotSr(moveMTRDTOList, lstErrors))
                {
                    foreach (ToolAssetMoveMaterialDTO moveMTRDTO in moveMTRDTOList)
                    {
                        moveMTRDTO.CreatedBy = SessionHelper.UserID;
                        moveMTRDTO.UpdatedBy = SessionHelper.UserID;
                        moveMTRDTO.RoomID = RoomID;
                        moveMTRDTO.CompanyID = CompanyID;
                        moveMTRDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                        moveMTRDTO.CreatedOn = DateTimeUtility.DateTimeNow;

                        string[] stringSeparators = new string[] { };
                        if (!string.IsNullOrEmpty(moveMTRDTO.DestinationLocation) && moveMTRDTO.DestinationLocation.Contains("[||]"))
                        {
                            stringSeparators = moveMTRDTO.DestinationLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                            if (stringSeparators != null && stringSeparators.Count() > 0)
                                moveMTRDTO.DestinationLocation = stringSeparators[1];
                        }

                        if (!string.IsNullOrEmpty(moveMTRDTO.SourceLocation) && moveMTRDTO.SourceLocation.Contains("[||]"))
                        {
                            stringSeparators = new string[] { };
                            stringSeparators = moveMTRDTO.SourceLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                            if (stringSeparators != null && stringSeparators.Count() > 0)
                                moveMTRDTO.SourceLocation = stringSeparators[1];
                        }

                        ToolAssetMoveMaterialDAL objMoveDAL = new ToolAssetMoveMaterialDAL(SessionHelper.EnterPriseDBName);
                        bool returnStatus = objMoveDAL.MoveTool(moveMTRDTO);
                    }

                    return Json(new { Message = "ok", Status = true, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lstErrors.Add(ex.Message.ToString());
                return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool ValidateMove(ToolAssetMoveMaterialDTO objDTO, List<string> lstErrorsList)
        {
            bool isValid = true;
            if (objDTO.SourceToolBinID <= 0)
            {
                lstErrorsList.Add(ResToolAssetMoveMaterial.ErrorMsgSourceLocation);
                isValid = false;
            }

            if (objDTO.MoveQuanity <= 0)
            {
                lstErrorsList.Add(ResToolAssetMoveMaterial.ErrorMsgMoveQty);
                isValid = false;
            }

            /*
            if (string.IsNullOrEmpty(objDTO.DestinationLocation))
            {
                lstErrorsList.Add(ResToolAssetMoveMaterial.ErrorMsgDestinationLocation);
                isValid = false;
            }*/

            if (objDTO.MoveType == (int)MoveType.InvToInv && objDTO.SourceToolBinID > 0 && objDTO.SourceToolBinID == objDTO.DestToolBinID)
            {
                lstErrorsList.Add(ResToolAssetMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                isValid = false;
            }


            double oveTotalSourceQty = 0;
            if (objDTO.MoveQuanity > 0)
            {
                if (objDTO.MoveType == (int)MoveType.InvToInv || objDTO.MoveType == (int)MoveType.InvToStag)
                {
                    ToolAssetQuantityDetailDAL objItmLocDtlsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<ToolAssetQuantityDetailDTO> SourceItemLocations = objItmLocDtlsDAL.GetQuantityByLIFOFIFO(false, objDTO.SourceToolBinID, RoomID, CompanyID, objDTO.ToolGUID.GetValueOrDefault(Guid.Empty), null);
                    if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                    {
                        oveTotalSourceQty = SourceItemLocations.Sum(x => x.Quantity);
                    }
                    objItmLocDtlsDAL = null;
                    SourceItemLocations = null;

                }

                if (oveTotalSourceQty < objDTO.MoveQuanity)
                {
                    lstErrorsList.Add(ResToolAssetMoveMaterial.ErrorMsgNotSuffucientQtyAtSource);
                    isValid = false;
                }
            }

            return isValid;
        }

        private bool ValidateMoveLotSr(List<ToolAssetMoveMaterialDTO> moveMTRDTOList, List<string> lstErrorsList)
        {
            bool isValid = true;

            foreach (ToolAssetMoveMaterialDTO moveMTRDTO in moveMTRDTOList)
            {

                string strLotSerialNumber = string.Empty;
                if (moveMTRDTO.SerialNumberTracking)
                    strLotSerialNumber = moveMTRDTO.SerialNumber;
                else if (moveMTRDTO.LotNumberTracking)
                    strLotSerialNumber = moveMTRDTO.LotNumber;

                if ((moveMTRDTO.SerialNumberTracking || moveMTRDTO.LotNumberTracking) && string.IsNullOrWhiteSpace(strLotSerialNumber))
                { 
                    lstErrorsList.Add(moveMTRDTO.SerialNumberTracking ? ": " + ResToolAssetMoveMaterial.SerialNoNotBlank : ": " + ResToolAssetMoveMaterial.LotNoNotBlank);
                    isValid = false;
                }

                if (moveMTRDTO.SourceToolBinID <= 0)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResToolAssetMoveMaterial.ErrorMsgSourceLocation);
                    isValid = false;
                }

                if (moveMTRDTO.MoveQuanity <= 0)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResToolAssetMoveMaterial.ErrorMsgMoveQty);
                    isValid = false;
                }

                if (moveMTRDTO.MoveType == (int)MoveType.InvToInv && moveMTRDTO.SourceToolBinID > 0 && moveMTRDTO.SourceToolBinID == moveMTRDTO.DestToolBinID)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResToolAssetMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                    isValid = false;
                }

                double oveTotalSourceQty = 0;
                if (moveMTRDTO.MoveQuanity > 0)
                {
                    if (moveMTRDTO.MoveType == (int)MoveType.InvToInv || moveMTRDTO.MoveType == (int)MoveType.InvToStag)
                    {
                        ToolAssetQuantityDetailDAL objItmLocDtlsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<ToolAssetQuantityDetailDTO> SourceItemLocations = objItmLocDtlsDAL.GetQuantityByLIFOFIFOForLotSr(false, moveMTRDTO.SourceToolBinID, RoomID, CompanyID, moveMTRDTO.ToolGUID.GetValueOrDefault(Guid.Empty), null, moveMTRDTO.LotNumber, moveMTRDTO.SerialNumber);
                        if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                        {
                            oveTotalSourceQty = SourceItemLocations.Sum(x => x.Quantity);
                        }
                        objItmLocDtlsDAL = null;
                        SourceItemLocations = null;
                    }


                    if (oveTotalSourceQty < moveMTRDTO.MoveQuanity)
                    {
                        lstErrorsList.Add(strLotSerialNumber + ": " + ResToolAssetMoveMaterial.ErrorMsgNotSuffucientQtyAtSource);
                        isValid = false;
                    }
                }
            }

            return isValid;
        }


        private List<CommonDTO> GetMoveTypeList(ToolMoveDialogOpenFrom openFrom)
        {
            List<CommonDTO> lstCommon = new List<CommonDTO>();
            if (openFrom == ToolMoveDialogOpenFrom.FromMove)
            {
                lstCommon.Add(new CommonDTO() { ID = 1, Text = ResToolAssetMoveMaterial.MoveTypeItemInvtoInv });
            }
            if (openFrom == ToolMoveDialogOpenFrom.FromTool)
            {
                lstCommon.Add(new CommonDTO() { ID = 1, Text = ResToolAssetMoveMaterial.MoveTypeItemInvtoInv });

            }

            return lstCommon;
        }

    }
}
