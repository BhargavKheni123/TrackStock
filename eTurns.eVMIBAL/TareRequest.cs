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
    public class TareRequest : PollRequestBase
    {
        public void TareProcess()
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {

                    ItemLocationTareRequestDAL objILTareRequestDAL = new ItemLocationTareRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objILTareRequestDAL.GetItemLocationTareRequestRooms();
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<ILTareRequestDTO> tareRequests = objILTareRequestDAL.GetItemLocationTareRequestByRoom(room.RoomID, room.CompanyID, false, false, null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (tareRequests.Count > 0)
                        {
                            PollEmailDTOCollection<TareDoneEmailDTO> tareEmailList = new PollEmailDTOCollection<TareDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

                            NoOfPages = tareRequests[0].NoOfPages;
                            TotalRecords = tareRequests[0].TotalRequest;
                            RecordsPerPage = tareRequests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (ILTareRequestDTO tareRequest in tareRequests)
                                {
                                    TareDoneEmailDTO tareEmailDTO = new TareDoneEmailDTO(tareRequest.BinID, tareRequest.ScaleID, tareRequest.ChannelID
                                    , tareRequest.ItemGUID, tareRequest.CompanyID, tareRequest.RoomID
                                    , tareRequest.RoomName, tareRequest.CompanyName
                                    , tareRequest.BinNumber, tareRequest.ItemNumber
                                    );

                                    TareProcessForItem(tareRequest, tareEmailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID, 
                                        systemUser.ID);
                                    tareEmailList.Add(tareEmailDTO);

                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    tareRequests = objILTareRequestDAL.GetItemLocationTareRequestByRoom(room.RoomID, room.CompanyID, false, false, null);
                                }
                            }

                            EmailWrapper.SendTareDoneEmail(tareEmailList);
                        }

                    } // loop through rooms


                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("TareProcess", ex);
                }


            }

        }

        private void TareProcessForItem(ILTareRequestDTO tareRequest, TareDoneEmailDTO tareEmailDTO
            , string EnterpriseDBName, long EnterpriseID, long systemUserID)
        {
            ItemLocationTareRequestDAL objTareRequestDAL = new ItemLocationTareRequestDAL(EnterpriseDBName);
            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(tareRequest.RoomID, tareRequest.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                tareRequest.IsTareStarted = true;
                tareRequest.TareStartTime = CurrentTimeofTimeZone;
                tareRequest.UpdatedBy = systemUserID;
                // update Tare start status
                objTareRequestDAL.UpdateItemLocationTareRequest(tareRequest);

                //double dbWeight = 0;
                bool isTareCompleted = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(tareRequest.ComPortName))
                    {
                        try
                        {
                            var res = comWrapper.ZeroWeight(tareRequest.ScaleID, tareRequest.ChannelID);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                tareEmailDTO.ComError = errorDescription;
                                isTareCompleted = false;

                                WriteComErrorLog("Tare Process Error", EnterpriseID, tareRequest.CompanyID,
                                   tareRequest.RoomID, tareRequest.ItemGUID, tareRequest.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isTareCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            tareEmailDTO.ComError = ex.Message;

                            WriteComErrorLog("Tare Process Error", EnterpriseID, tareRequest.CompanyID,
                                  tareRequest.RoomID, tareRequest.ItemGUID, tareRequest.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                tareRequest.IsTareCompleted = true;

                if (!isTareCompleted)
                {
                    tareRequest.ErrorDescription = errorDescription;
                }
                else
                {
                    tareRequest.ErrorDescription = null;
                }
                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                tareRequest.TareCompletionTime = CurrentTimeofTimeZone;
                tareRequest.UpdatedBy = systemUserID;
                // update Tare completed status
                objTareRequestDAL.UpdateItemLocationTareRequest(tareRequest);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Tare Process Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, tareRequest.CompanyID, tareRequest.RoomID,
                                  tareRequest.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                tareEmailDTO.ExceptionError = ex.Message;
            }
        }

        public void TareProcessForItem(long requestID, string enterpriseDBName, long enterpriseID,long companyId, long roomId,long userID)
        {
            try
            {
                ItemLocationTareRequestDAL objILTareRequestDAL = new ItemLocationTareRequestDAL(enterpriseDBName);
                ILTareRequestDTO tareRequest = objILTareRequestDAL.GetItemLocationTareRequestByRoom(roomId, companyId, false, false, requestID).FirstOrDefault();
                if (tareRequest != null)
                {
                    PollEmailDTOCollection<TareDoneEmailDTO> tareEmailList = new PollEmailDTOCollection<TareDoneEmailDTO>(enterpriseID, enterpriseDBName);
                    TareDoneEmailDTO tareEmailDTO = new TareDoneEmailDTO(tareRequest.BinID, tareRequest.ScaleID, tareRequest.ChannelID
                                           , tareRequest.ItemGUID, tareRequest.CompanyID, tareRequest.RoomID
                                           , tareRequest.RoomName, tareRequest.CompanyName
                                           , tareRequest.BinNumber, tareRequest.ItemNumber
                                           );
                    
                    TareProcessForItem(tareRequest, tareEmailDTO, enterpriseDBName, enterpriseID, userID);
                    tareEmailList.Add(tareEmailDTO);
                    EmailWrapper.SendTareDoneEmail(tareEmailList);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("TareProcess", ex);
            }

        }

    }
}
