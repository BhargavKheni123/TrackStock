using eTurns.DAL;
using eTurns.DTO;
using eTurns.eVMIBAL.DTO;
using eTurns.eVMIBAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eTurns.eVMIBAL
{
    public class ItemWeightPerPieceRequest : PollRequestBase
    {

        public void GetItemWeightPerPieceProcess()
        {

            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;


            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {
                    ItemWeightRequestDAL wppRequestDAL = new ItemWeightRequestDAL(enterprise.EnterpriseDBName);

                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);
                    var rooms = wppRequestDAL.GetItemWeightPerPieceRequestRooms();
                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<IWPPieceRequestDTO> weightPerPieceRequests = wppRequestDAL.
                            GetItemWeightPerPieceRequestByRoom(room.RoomID, room.CompanyID, false, false,null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (weightPerPieceRequests.Count > 0)
                        {
                            NoOfPages = weightPerPieceRequests[0].NoOfPages;
                            TotalRecords = weightPerPieceRequests[0].TotalRequest;
                            RecordsPerPage = weightPerPieceRequests[0].RecordsPerPage;

                            PollEmailDTOCollection<ItemWeightPerPieceDoneEmailDTO> emailDTOList = new PollEmailDTOCollection<ItemWeightPerPieceDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);
                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (IWPPieceRequestDTO wppRequest in weightPerPieceRequests)
                                {
                                    ItemWeightPerPieceDoneEmailDTO emailDTO = new ItemWeightPerPieceDoneEmailDTO(wppRequest.BinID, wppRequest.ScaleID, wppRequest.ChannelID
                                         , wppRequest.ItemGUID, wppRequest.CompanyID, wppRequest.RoomID
                                         , wppRequest.RoomName, wppRequest.CompanyName
                                         , wppRequest.BinNumber, wppRequest.ItemNumber
                                         );

                                    GetItemWeightPerPieceProcessForItem(wppRequest, systemUser.ID, emailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID);

                                    emailDTOList.Add(emailDTO);

                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    weightPerPieceRequests = wppRequestDAL.GetItemWeightPerPieceRequestByRoom(room.RoomID, room.CompanyID, false, false,null);
                                }
                            }

                            EmailWrapper.SendWeightPerPieceDoneEmail(emailDTOList);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("GetItemWeightPerPieceProcess", ex);
                }

            }

        }

        private void GetItemWeightPerPieceProcessForItem(IWPPieceRequestDTO wppRequest,
            long systemUserID, ItemWeightPerPieceDoneEmailDTO emailDTO, string EnterpriseDBName, long EnterpriseID)
        {
            ItemWeightRequestDAL objILPollRequestDAL = new ItemWeightRequestDAL(EnterpriseDBName);
            object lockObject = new object();

            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(wppRequest.RoomID, wppRequest.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                wppRequest.IsWeightStarted = true;
                wppRequest.WeightStartTime = CurrentTimeofTimeZone;
                wppRequest.UpdatedBy = systemUserID;
                objILPollRequestDAL.UpdateItemWeightPerPieceRequest(wppRequest);

                double dbWeight = 0;
                bool isPollCompleted = true;
                string errorDescription = "";
                COMCmdResponse<decimal?> getWeightResp = null;


                lock (lockObject)
                {
                    string evmiError = "";
                    // get weight from scale using com port , scale id , channel id
                    using (COMWrapper comWrapper = new COMWrapper(wppRequest.ComPortName))
                    {
                        string commandDataValue = "";
                        try
                        {
                            getWeightResp = comWrapper.GetWeight(wppRequest.ScaleID, wppRequest.ChannelID, string.Empty, out commandDataValue);

                            decimal dcWeight = getWeightResp.data ?? 0;
                            //dbWeight = new Random().NextDouble() + 1; //Convert.ToDouble(dcWeight);
                            dbWeight = Convert.ToDouble(dcWeight);

                            if (!getWeightResp.IsSuccess)
                            {
                                evmiError = getWeightResp.CommandData.ErrorInfo;

                                isPollCompleted = false;
                                errorDescription = evmiError;
                                emailDTO.ComError = evmiError + " ";
                                WriteComErrorLog("Get Weight Per Piece Error", EnterpriseID, wppRequest.CompanyID,
                                    wppRequest.RoomID, wppRequest.ItemGUID, wppRequest.ComPortName
                                    , evmiError + " GetItemWeightPerPieceProcessForItem : CommandDataValue : " + commandDataValue);
                            }

                        }
                        catch (Exception ex)
                        {
                            isPollCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            emailDTO.ComError += ex.Message;
                            WriteComErrorLog("Get Weight Per Piece Error", EnterpriseID, wppRequest.CompanyID,
                                    wppRequest.RoomID, wppRequest.ItemGUID, wppRequest.ComPortName
                                    , evmiError + " GetItemWeightPerPieceProcessForItem : CommandDataValue : " + commandDataValue, ex); 
                        }
                    }
                }

                wppRequest.IsWeightCompleted = true;

                emailDTO.WeightReading = dbWeight;

                bool IsUpdateItem = false;
                if (!isPollCompleted)
                {
                    wppRequest.ErrorDescription = errorDescription;

                }
                else
                {
                    wppRequest.ErrorDescription = null;
                    wppRequest.WeightReading = dbWeight;

                    if (dbWeight > 0)
                    {
                        wppRequest.ItemWeightPerPiece = (dbWeight / (wppRequest.TotalQty ?? 0));
                        IsUpdateItem = true;
                    }
                    else
                    {
                        wppRequest.ErrorDescription = "Bin not on sensor.";
                        emailDTO.ExceptionError = "Bin not on sensor.";
                    }
                }

                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                wppRequest.WeightCompletionTime = CurrentTimeofTimeZone;
                wppRequest.UpdatedBy = systemUserID;
                objILPollRequestDAL.UpdateItemWeightPerPieceRequest(wppRequest);

                if (IsUpdateItem)
                {
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(EnterpriseDBName);
                    objItemDAL.UpdateItemWeight(wppRequest.ItemGUID, wppRequest.ItemWeightPerPiece.GetValueOrDefault(0), true);
                }
            }
            catch (Exception ex)
            {
                emailDTO.ExceptionError = ex.Message;
                string log = string.Format(Environment.NewLine
                                + "Get Weight Per Piece Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, wppRequest.CompanyID, wppRequest.RoomID,
                                  wppRequest.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
            }
        }


        public double GetItemWeightPerPieceProcessForItem(long requestID, string enterpriseDBName, long enterpriseID,
            long companyId, long roomId, long userId)
        {
            double itemWeightPerPiece = 0;
            try
            {
                ItemWeightRequestDAL wppRequestDAL = new ItemWeightRequestDAL(enterpriseDBName);
                IWPPieceRequestDTO wppRequest = wppRequestDAL.GetItemWeightPerPieceRequestByRoom(roomId, companyId, false, false, requestID).FirstOrDefault();
                if (wppRequest != null)
                {
                    PollEmailDTOCollection<ItemWeightPerPieceDoneEmailDTO> emailDTOList = new PollEmailDTOCollection<ItemWeightPerPieceDoneEmailDTO>(enterpriseID,
                        enterpriseDBName);

                    ItemWeightPerPieceDoneEmailDTO emailDTO = new ItemWeightPerPieceDoneEmailDTO(wppRequest.BinID, wppRequest.ScaleID, wppRequest.ChannelID
                                        , wppRequest.ItemGUID, wppRequest.CompanyID, wppRequest.RoomID
                                        , wppRequest.RoomName, wppRequest.CompanyName
                                        , wppRequest.BinNumber, wppRequest.ItemNumber
                                        );

                    GetItemWeightPerPieceProcessForItem(wppRequest, userId, emailDTO, enterpriseDBName, enterpriseID);
                    itemWeightPerPiece = wppRequest.ItemWeightPerPiece ?? 0;

                    emailDTOList.Add(emailDTO);
                    EmailWrapper.SendWeightPerPieceDoneEmail(emailDTOList);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("TareProcess", ex);
            }

            return itemWeightPerPiece;
        }

    } // class
}
