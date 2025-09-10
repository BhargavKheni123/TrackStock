using eTurns.DAL;
using eTurns.DTO;
using eTurns.eVMIBAL.DTO;
using eTurns.eVMIBAL.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

namespace eTurns.eVMIBAL
{
    public class PollRequest : PollRequestBase
    {
        #region Queues Request
        /// <summary>
        /// Get weight and update in db
        /// </summary>
        public void ProcessGetWight()
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {
                    ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objILPollRequestDAL.GetItemLocationPollRequestRooms();
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);
                    //PollEmailDTOCollection<BadPollEmailDTO> badPolls = new PollEmailDTOCollection<BadPollEmailDTO>(objDTO.EnterpriseID,objDTO.EnterpriseDBName);

                    foreach (var room in rooms)
                    {
                        int pageNo = 1;
                        List<ILPollRequestDTO> pollRequests = objILPollRequestDAL.GetItemLocationPollRequestByRoom(room.RoomID, room.CompanyID, false, false, false, null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (pollRequests.Count > 0)
                        {
                            NoOfPages = pollRequests[0].NoOfPages;
                            TotalRecords = pollRequests[0].TotalRequest;
                            RecordsPerPage = pollRequests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (ILPollRequestDTO pollRequest in pollRequests)
                                {
                                    BadPollEmailDTO badPollEmailDTO = new BadPollEmailDTO(pollRequest.BinID, pollRequest.ScaleID,
                                            pollRequest.ChannelID, pollRequest.ItemGUID
                                            , pollRequest.CompanyID, pollRequest.RoomID,
                                             pollRequest.WeightPerPiece ?? 0, pollRequest.RoomName,
                                             pollRequest.CompanyName
                                            , pollRequest.BinNumber
                                            , pollRequest.ItemNumber
                                            , pollRequest.WeightVariance ?? 0
                                            );

                                    ProcessGetWightForItem(pollRequest, systemUser.ID, enterprise.EnterpriseDBName, enterprise.EnterpriseID, badPollEmailDTO);
                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    pollRequests = objILPollRequestDAL.GetItemLocationPollRequestByRoom(room.RoomID, room.CompanyID, false, false, false, null);
                                }
                            }

                            //EmailWrapper.SendBadPollEmail(badPolls,comErrorEmailDTOList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("GetWightProcess", ex);
                }

            } // outer loop


        }

        /// <summary>
        /// Get weight from Scale
        /// </summary>
        /// <param name="pollRequest"></param>
        /// <param name="systemUser"></param>
        /// <param name="EnterpriseDBName"></param>
        /// <param name="EnterpriseID"></param>
        /// <param name="badPollEmailDTO"></param>
        private void ProcessGetWightForItem(ILPollRequestDTO pollRequest, long systemUserID,
            string EnterpriseDBName, long EnterpriseID, BadPollEmailDTO badPollEmailDTO)
        {

            EVMIPollErrorTypeEnum PollErrorType = EVMIPollErrorTypeEnum.None;
            object lockObject = new object();
            ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(EnterpriseDBName);

            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(pollRequest.RoomID, pollRequest.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }

                // update poll start status
                objILPollRequestDAL.UpdateItemLocationPollRequestPollStarted(pollRequest, CurrentRoomTimeZone, systemUserID, EVMIPollErrorTypeEnum.None);
                eVMISiteSettings SensorBinRoomSettings = null;
                if (EnterpriseDBName != string.Empty)
                {
                    RoomDAL RoomDAL = new RoomDAL(EnterpriseDBName);
                    SensorBinRoomSettings = RoomDAL.GetSensorBinRoomSettings(pollRequest.RoomID, EnterpriseID, pollRequest.CompanyID);
                }

                double dbWeight = 0;
                bool isPollCompleted = true;
                string comError = "";
                COMCmdResponse<decimal?> getWeightResp = null;
                string CommandText = string.Empty;

                lock (lockObject)
                {

                    // get weight from scale using com port , scale id , channel id
                    using (COMWrapper comWrapper = new COMWrapper(pollRequest.ComPortName))
                    {
                        string commandDataValue = "";
                        try
                        {
                            string PollCommand = (SensorBinRoomSettings != null ? SensorBinRoomSettings.PollCommand : "W");
                            getWeightResp = comWrapper.GetWeight(pollRequest.ScaleID, pollRequest.ChannelID, PollCommand, out commandDataValue);

                            decimal dcWeight = getWeightResp.data ?? 0;
                            dbWeight = Convert.ToDouble(dcWeight);
                            CommandText = getWeightResp.CommandData.CommandText;
                            if (!getWeightResp.IsSuccess)
                            {
                                isPollCompleted = false;
                                comError = getWeightResp.CommandData.ErrorInfo;
                                WriteComErrorLog("Get Weight Error", EnterpriseID, pollRequest.CompanyID,
                                    pollRequest.RoomID, pollRequest.ItemGUID, pollRequest.ComPortName
                                    , comError + " GetItemWeightPerPieceProcessForItem : CommandDataValue : " + commandDataValue);
                                PollErrorType = EVMIPollErrorTypeEnum.COMPortError;
                            }
                        }
                        catch (Exception ex)
                        {
                            isPollCompleted = false;
                            comError = Logger.GetExceptionDetails(ex);
                            PollErrorType = EVMIPollErrorTypeEnum.COMPortError;
                            WriteComErrorLog("Get Weight Error", EnterpriseID, pollRequest.CompanyID,
                                    pollRequest.RoomID, pollRequest.ItemGUID, pollRequest.ComPortName
                                    , comError + " GetItemWeightPerPieceProcessForItem : CommandDataValue : " + commandDataValue, ex);

                        }
                    }
                }

                badPollEmailDTO.WeightReading = dbWeight;

                if (isPollCompleted)
                {

                    // check for bad poll errors
                    EVMIPollErrorTypeEnum dataErrorType = EVMIPollErrorTypeEnum.None;
                    if (!badPollEmailDTO.Validate(out dataErrorType))
                    {
                        isPollCompleted = false;
                        comError = badPollEmailDTO.BadPollErrorDetails;
                        //badPolls.Add(badPollEmailDTO);

                        if (dataErrorType == EVMIPollErrorTypeEnum.Warning)
                        {
                            if (PollErrorType == EVMIPollErrorTypeEnum.COMPortError)
                            {
                                PollErrorType = EVMIPollErrorTypeEnum.COMPortError_AND_Warning;
                            }
                            else if (PollErrorType == EVMIPollErrorTypeEnum.None)
                            {
                                PollErrorType = dataErrorType;
                            }
                        }
                        else if (dataErrorType == EVMIPollErrorTypeEnum.DataError)
                        {
                            if (PollErrorType == EVMIPollErrorTypeEnum.COMPortError)
                            {
                                PollErrorType = EVMIPollErrorTypeEnum.COMPortError_AND_DataError;
                            }
                            else if (PollErrorType == EVMIPollErrorTypeEnum.None)
                            {
                                PollErrorType = dataErrorType;
                            }
                        }
                        else if (dataErrorType == EVMIPollErrorTypeEnum.WithinWeightVarianceRange)
                        {
                            PollErrorType = EVMIPollErrorTypeEnum.WithinWeightVarianceRange;
                        }
                        else if (dataErrorType == EVMIPollErrorTypeEnum.None)
                        {
                            //PollErrorType = PollErrorType;
                        }


                    }

                    if (dataErrorType == EVMIPollErrorTypeEnum.WithinWeightVarianceRange)
                    {
                        PollErrorType = EVMIPollErrorTypeEnum.WithinWeightVarianceRange;
                        comError = badPollEmailDTO.BadPollErrorDetails;
                    }
                }

                //if (!isPollCompleted)
                //{
                //    //objILPollRequestDTO.ErrorDescription = errorDescription;
                //}
                //else
                //{
                //    //objILPollRequestDTO.ErrorDescription = null;
                //    //comError = null;
                //}

                // update poll completed status
                objILPollRequestDAL.UpdateItemLocationPollRequestPollCompleted(pollRequest,
                    CurrentRoomTimeZone, systemUserID, dbWeight, comError, PollErrorType, CommandText);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Get Weight Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, pollRequest.CompanyID, pollRequest.RoomID,
                                  pollRequest.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
            }


        }

        /// <summary>
        /// Pull or Credit operation
        /// </summary>
        public void ProcessPullCredit()
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);
                    ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objILPollRequestDAL.GetItemLocationPollRequestRooms();

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        // get first page record , RecordsPerPage are in SP so It can very per enterprise
                        int pageNo = 1;
                        List<ILPollRequestDTO> pollRequests = objILPollRequestDAL.GetItemLocationPollRequestByRoom(room.RoomID, room.CompanyID, true, true, false, null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;
                        List<string> lstItemGUID = new List<string>();
                        if (pollRequests.Count > 0)
                        {
                            NoOfPages = pollRequests[0].NoOfPages;
                            TotalRecords = pollRequests[0].TotalRequest;
                            RecordsPerPage = pollRequests[0].RecordsPerPage;

                            PollEmailDTOCollection<PollDoneEmailDTO> pollDoneEmails = new PollEmailDTOCollection<PollDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);
                            // loop through all pages
                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                // loop through requests in page
                                foreach (ILPollRequestDTO pollRequest in pollRequests)
                                {
                                    bool IsItemCreditPull = false;
                                    PollDoneEmailDTO pollEmailDTO = pollEmailDTO = new PollDoneEmailDTO(pollRequest.BinID, pollRequest.ScaleID, pollRequest.ChannelID
                                        , pollRequest.WeightReading, pollRequest.ItemGUID, pollRequest.CompanyID, pollRequest.RoomID
                                        , pollRequest.RoomName, pollRequest.CompanyName
                                        , pollRequest.BinNumber, pollRequest.ItemNumber
                                        );

                                    ProcessPullCreditForItem(pollRequest, systemUser.ID, enterprise.EnterpriseDBName, enterprise.EnterpriseID, pollEmailDTO, out IsItemCreditPull);

                                    pollDoneEmails.Add(pollEmailDTO);
                                    if (IsItemCreditPull)
                                    {
                                        lstItemGUID.Add(pollRequest.ItemGUID.ToString());
                                    }
                                } // loop through paged records

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    pollRequests = objILPollRequestDAL.GetItemLocationPollRequestByRoom(room.RoomID, room.CompanyID, true, true, false, null);
                                }

                            }// loop through all pages

                            // Send poll done email once all pages records are processed
                            EmailWrapper.SendPollDoneEmail(pollDoneEmails);

                            // call web request for Item QOH change
                            if (lstItemGUID != null && lstItemGUID.Count > 0)
                            {
                                //SendItemWebRequestToSapphire(lstItemGUID);
                            }

                        }


                    }// loop throuth room
                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("ItemQTyPullCredit", ex);
                }
            }


        }

        //public void SendItemWebRequestToSapphire(List<string> lstItemGUID)
        //{
        //    try
        //    {
        //        string strItems = String.Join(",", lstItemGUID);
        //        string requestUri = string.Format(AppSettings.WebRequestURL, strItems);
        //        HttpWebRequest request = null;
        //        request = (HttpWebRequest)WebRequest.Create(requestUri.ToString());
        //        request.Method = "GET";
        //        request.Accept = "application/atom+xml";
        //        request.ContentType = "text/html; charset=utf-8";
        //        request.KeepAlive = false;
        //        request.Timeout = 1000000;

        //        var response = (HttpWebResponse)request.GetResponse();
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteExceptionLog("SendItemWebRequestToSapphire", ex);
        //    }

        //}

        private void ProcessPullCreditForItem(ILPollRequestDTO pollRequest, long systemUserID,
            string EnterpriseDBName, long EnterpriseID, PollDoneEmailDTO pollEmailDTO, out bool IsItemCreditPull)
        {
            ItemMasterDAL objIMDAL = new ItemMasterDAL(EnterpriseDBName);
            ItemLocationPollDetailsDTO detailsDTO = new ItemLocationPollDetailsDTO();
            detailsDTO.PollRequestID = pollRequest.ID;

            IsItemCreditPull = false;
            ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(EnterpriseDBName);
            ItemLocationQTYDAL objLocationQtyDAL = new ItemLocationQTYDAL(EnterpriseDBName);
            EVMIPollErrorTypeEnum ErrorTypeEnum = pollRequest.ErrorTypeEnum;
            try
            {
                Int64 RoomID = pollRequest.RoomID;
                Int64 CompanyID = pollRequest.CompanyID;
                Guid ItemGUID = pollRequest.ItemGUID;

                //ItemLocationDetailsDTO objItemLocationDTO = new ItemLocationDetailsDTO();
                //BinMasterDTO objBinDTO = objBinDAL.GetBinByID(objILPollRequestDTO.BinID, RoomID, CompanyID);

                detailsDTO.WeightReading = pollRequest.WeightReading;



                ItemMasterDTO objImDTO = objIMDAL.GetItemByGuidPlain(ItemGUID, RoomID, CompanyID);
                double ItemWeightPerPiece = 0;
                if (objImDTO.WeightPerPiece.GetValueOrDefault(0) > 0)
                {
                    ItemWeightPerPiece = objImDTO.WeightPerPiece.GetValueOrDefault(0);
                }

                detailsDTO.Consignment = objImDTO.Consignment;
                detailsDTO.ItemWeightPerPiece = ItemWeightPerPiece;
                pollEmailDTO.WeightPerPiece = ItemWeightPerPiece;

                double NewQuantity = 0;

                if (pollRequest.WeightReading > 0 && ItemWeightPerPiece > 0)
                {
                    NewQuantity = Convert.ToInt32(pollRequest.WeightReading / ItemWeightPerPiece);
                }

                double ItemCustQTY = 0;
                double ItemConsQTY = 0;
                detailsDTO.NewQuantity = NewQuantity;
                pollEmailDTO.NewQuantity = NewQuantity;

                if (pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.Warning)
                {
                    pollEmailDTO.WarningError = pollRequest.ErrorDescription;
                }
                else
                {
                    pollEmailDTO.ComError = pollRequest.ErrorDescription;
                }



                if (pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.None
                    || pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.Warning
                    || pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.WithinWeightVarianceRange
                    )
                {
                    #region "Pull or Credit using count logic"


                    DateTime CurrentTimeofTimeZone = RegionalSettingRoomDate(RoomID, CompanyID, EnterpriseDBName);

                    ItemLocationQTYDTO IlQTYDTO = objLocationQtyDAL.GetRecordByBinItem(ItemGUID, pollRequest.BinID, RoomID, CompanyID);

                    if (IlQTYDTO != null)
                    {
                        ItemCustQTY = IlQTYDTO.CustomerOwnedQuantity.GetValueOrDefault(0);
                        ItemConsQTY = IlQTYDTO.ConsignedQuantity.GetValueOrDefault(0);
                    }

                    detailsDTO.CustomerOwnedQuantity = ItemCustQTY;
                    detailsDTO.ConsignedQuantity = ItemConsQTY;

                    double ItemCustConsQTY = ItemCustQTY + ItemConsQTY;
                    if (NewQuantity == ItemCustConsQTY)
                    {
                        #region "Same Quantity"
                        detailsDTO.ActionType = "Same Quantity";
                        pollEmailDTO.PollAction = "Same Quantity";

                        string errorS = "Same Qty";
                        string logMsg = string.Format("ProcessPullCredit >> IsPostProcessDone = true >> EnterpriseId: {0}, CompanyId: {1}, RoomId : {2}, ItemGUID : {3}. " + errorS,
                            EnterpriseID, CompanyID, RoomID, ItemGUID.ToString()
                            );
                        Logger.WriteLog(logMsg);
                        #endregion
                    }
                    else if (NewQuantity > ItemCustConsQTY)
                    {
                        #region Credit
                        detailsDTO.ActionType = "Credit";
                        pollEmailDTO.PollAction = "Credit";
                        IsItemCreditPull = true;
                        #endregion
                    }
                    else
                    {
                        #region Pull
                        detailsDTO.ActionType = "Pull";
                        pollEmailDTO.PollAction = "Pull";
                        IsItemCreditPull = true;
                        #endregion
                    }

                    EVMIInvCountDetailDTO eVMIInvCount = new EVMIInvCountDetailDTO()
                    {
                        ItemGUID = pollRequest.ItemGUID,
                        BinGUID = pollRequest.BinGUID,
                        NewQuantity = NewQuantity,
                        BinID = pollRequest.BinID,
                        CompanyId = CompanyID,
                        RoomId = RoomID,
                        CreatedBy = systemUserID,
                        IsConsignment = objImDTO.Consignment,
                        ItemCost = objImDTO.Cost.GetValueOrDefault(0),
                        ItemPrice = objImDTO.SellPrice.GetValueOrDefault(0),
                        CountGUID = pollRequest.CountGUID

                    };

                    string emailErrorMsg = "";
                    string RoomDateFormat = GetRoomDateFormat(EnterpriseDBName, RoomID, CompanyID, systemUserID);
                    string saveCountError = SaveEVMIInventoryCountDetail(EnterpriseDBName, eVMIInvCount, systemUserID,
                        RoomDateFormat, objImDTO, out emailErrorMsg, EnterpriseID);

                    if (!string.IsNullOrWhiteSpace(saveCountError))
                    {
                        detailsDTO.ActionType = detailsDTO.ActionType + " With Error";
                        if (!string.IsNullOrWhiteSpace(detailsDTO.ErrorDescription))
                        {
                            detailsDTO.ErrorDescription += Environment.NewLine;
                        }
                        detailsDTO.ErrorDescription += saveCountError;
                        pollEmailDTO.ExceptionError = emailErrorMsg;

                        if (pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.Warning)
                        {
                            ErrorTypeEnum = EVMIPollErrorTypeEnum.Exception_AND_Warning;
                        }
                        else
                        {
                            ErrorTypeEnum = EVMIPollErrorTypeEnum.Exception;
                        }
                    }

                    #endregion
                }
                else
                {
                    pollEmailDTO.PollAction = "No Poll";
                    detailsDTO.ActionType = "Com Error/Bad Polls";
                    detailsDTO.ErrorDescription = pollRequest.ErrorDescription;
                }

                objILPollRequestDAL.UpdateItemLocationPollRequestPostProcessDone(pollRequest, systemUserID, ErrorTypeEnum);
                InsertItemLocationPollDetails(EnterpriseDBName, detailsDTO);

            }
            catch (Exception ex)
            {
                string errorEx = Logger.GetExceptionDetails(ex);

                detailsDTO.ActionType = "Exception";
                detailsDTO.ErrorDescription = errorEx;
                pollEmailDTO.PollAction = "No Poll (Technical Error)";

                if (string.IsNullOrWhiteSpace(pollEmailDTO.ExceptionError) == false)
                {
                    pollEmailDTO.ExceptionError += Environment.NewLine;
                }
                pollEmailDTO.ExceptionError += ex.Message;

                InsertItemLocationPollDetails(EnterpriseDBName, detailsDTO);
                ErrorTypeEnum = EVMIPollErrorTypeEnum.Exception;
                objILPollRequestDAL.UpdateItemLocationPollRequestPostProcessDone(pollRequest, systemUserID, ErrorTypeEnum);
                Logger.WriteExceptionLog("ItemQTyPullCredit", ex);

            }


        }

        private string SaveEVMIInventoryCountDetail(string EnterpriseDBName, EVMIInvCountDetailDTO eVMIInvCount,
         long systemUserID, string RoomDateFormat, ItemMasterDTO objImDTO, out string emailErrorMsg, long EnterpriseId)
        {
            string message = "";
            emailErrorMsg = "";
            InventoryCountDAL countDAL = new InventoryCountDAL(EnterpriseDBName);
            SaveEVMIInvCountDetailDTO countDetailResp = null;
            bool isLineItemSave = true;

            try
            {
                // insert count line items
                countDetailResp = countDAL.SaveEVMIInventoryCountDetail(eVMIInvCount);
                if (!string.IsNullOrWhiteSpace(countDetailResp.Status))
                {
                    isLineItemSave = false;
                    message = countDetailResp.Status;
                    emailErrorMsg = countDetailResp.Status + " ";
                }
            }
            catch (Exception ex)
            {
                isLineItemSave = false;
                message += " " + Logger.GetExceptionDetails(ex);
                Logger.WriteExceptionLog("SaveEVMIInventoryCountDetail", ex);
                emailErrorMsg += ex.Message;
            }

            if (isLineItemSave && countDetailResp != null && countDetailResp.CountDetailID.HasValue && countDetailResp.CountDetailGUID.HasValue)
            {
                try
                {
                    // apply count
                    List<InventoryCountDetailDTO> lstLineItems = new List<InventoryCountDetailDTO>();
                    InventoryCountDetailDTO countDetailDTO = new InventoryCountDetailDTO();
                    countDetailDTO.ID = countDetailResp.CountDetailID.Value;
                    countDetailDTO.CountDetailGUID = countDetailResp.CountDetailGUID.Value;
                    countDetailDTO.ItemGUID = eVMIInvCount.ItemGUID;
                    countDetailDTO.BinID = eVMIInvCount.BinID;
                    countDetailDTO.CountCustomerOwnedQuantity = eVMIInvCount.IsConsignment == false ? (double?)eVMIInvCount.NewQuantity : null;
                    countDetailDTO.CountConsignedQuantity = eVMIInvCount.IsConsignment == true ? (double?)eVMIInvCount.NewQuantity : null;
                    countDetailDTO.SaveAndApply = true;
                    countDetailDTO.IsStagingLocationCount = false;
                    countDetailDTO.IsOnlyFromItemUI = true;
                    countDetailDTO.InventoryCountGUID = countDetailResp.InventoryCountGUID.Value;
                    countDetailDTO.ItemLotSerialType = null;
                    countDetailDTO.LotNumberTracking = objImDTO.LotNumberTracking;
                    countDetailDTO.SerialNumberTracking = objImDTO.SerialNumberTracking;
                    countDetailDTO.ProjectSpendGUID = null;
                    countDetailDTO.SupplierAccountGuid = null;


                    lstLineItems.Add(countDetailDTO);
                    ApplyCountOnLineItemRespDTO respApply = countDAL.ApplyCountOnLineItemsNew(eVMIInvCount.RoomId,
                        eVMIInvCount.CompanyId, systemUserID
                        , lstLineItems, RoomDateFormat, false, EnterpriseId);

                    bool isApplied = respApply.Status == "ok";

                    if (!isApplied)
                    {
                        message += " " + respApply.Message;
                        emailErrorMsg += respApply.Message + " ";
                    }
                }
                catch (Exception ex)
                {
                    message += " " + Logger.GetExceptionDetails(ex);
                    Logger.WriteExceptionLog("SaveEVMIInventoryCountDetail >> ApplyCountOnLineItemsNew", ex);
                    emailErrorMsg += ex.Message;
                }
            }

            return message;
        }

        private string GetRoomDateFormat(string EnterpriseDBName, long roomId, long companyId, long userId)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(roomId, companyId, userId);
            string RoomDateFormat = "M/d/yyyy";
            if (objeTurnsRegionInfo != null)
            {
                RoomDateFormat = objeTurnsRegionInfo.ShortDatePattern;
            }
            return RoomDateFormat;
        }


        /// <summary>
        /// Insert Poll details log
        /// </summary>
        /// <param name="enterpriseDbName"></param>
        /// <param name="detailsDTO"></param>
        private void InsertItemLocationPollDetails(string enterpriseDbName, ItemLocationPollDetailsDTO detailsDTO)
        {
            try
            {
                ItemLocationPollDetailsDAL pollDetailsDAL = new ItemLocationPollDetailsDAL(enterpriseDbName);
                pollDetailsDAL.InsertItemLocationPollDetails(detailsDTO);
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("InsertItemLocationPollDetails", ex);
            }
        }

        private DateTime RegionalSettingRoomDate(Int64 RoomID, Int64 CompanyID, string EnterpriseDBName)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
            string CurrentRoomTimeZone = "UTC";
            if (objRegionalSettings != null)
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            return CurrentTimeofTimeZone;
        }
        #endregion

        #region Immediate Request

        public void ProcessGetWightForItem(long requestID, string enterpriseDBName, long enterpriseID, long companyId, long roomId, long userID)
        {
            try
            {
                ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(enterpriseDBName);
                ILPollRequestDTO pollRequest = objILPollRequestDAL.GetItemLocationPollRequestByRoom(roomId, companyId, false, false, false, requestID).FirstOrDefault();
                if (pollRequest != null)
                {
                    // Get Weight 

                    BadPollEmailDTO badPollEmailDTO = new BadPollEmailDTO(pollRequest.BinID, pollRequest.ScaleID,
                                            pollRequest.ChannelID, pollRequest.ItemGUID
                                            , pollRequest.CompanyID, pollRequest.RoomID,
                                             pollRequest.WeightPerPiece ?? 0, pollRequest.RoomName,
                                             pollRequest.CompanyName
                                            , pollRequest.BinNumber
                                            , pollRequest.ItemNumber
                                            , pollRequest.WeightVariance ?? 0
                                            );

                    ProcessGetWightForItem(pollRequest, userID, enterpriseDBName, enterpriseID, badPollEmailDTO);

                    PollEmailDTOCollection<PollDoneEmailDTO> pollDoneEmails = new PollEmailDTOCollection<PollDoneEmailDTO>(enterpriseID, enterpriseDBName);

                    // Pull or Credit
                    bool IsItemCreditPull = false;
                    PollDoneEmailDTO pollEmailDTO = pollEmailDTO = new PollDoneEmailDTO(pollRequest.BinID, pollRequest.ScaleID, pollRequest.ChannelID
                                        , pollRequest.WeightReading, pollRequest.ItemGUID, pollRequest.CompanyID, pollRequest.RoomID
                                        , pollRequest.RoomName, pollRequest.CompanyName
                                        , pollRequest.BinNumber, pollRequest.ItemNumber
                                        );
                    ProcessPullCreditForItem(pollRequest, userID, enterpriseDBName, enterpriseID, pollEmailDTO, out IsItemCreditPull);

                    if (pollRequest.IsPollCompleted && (pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.Exception_AND_Warning || pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.DataError || pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.COMPortError || pollRequest.ErrorTypeEnum == EVMIPollErrorTypeEnum.Exception))
                    {
                        pollDoneEmails.Add(pollEmailDTO);
                        EmailWrapper.SendPollDoneEmail(pollDoneEmails);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("TareProcess", ex);
            }
        }

        #endregion

    }//class
}
