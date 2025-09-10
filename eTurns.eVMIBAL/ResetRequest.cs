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
    public class ResetRequest : PollRequestBase
    {
        public void ResetProcess()
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {

                    EVMIResetRequestDAL objResetRequestDAL = new EVMIResetRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objResetRequestDAL.GetResetRequestRooms();
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<eVMIResetReqDTO> resetRequests = objResetRequestDAL.GetResetRequestByRoom(room.RoomID, room.CompanyID, false, false, null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (resetRequests.Count > 0)
                        {
                            PollEmailDTOCollection<ResetDoneEmailDTO> resetEmailList = new PollEmailDTOCollection<ResetDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

                            NoOfPages = resetRequests[0].NoOfPages;
                            TotalRecords = resetRequests[0].TotalRequest;
                            RecordsPerPage = resetRequests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (eVMIResetReqDTO resetRequest in resetRequests)
                                {
                                    ResetDoneEmailDTO resetEmailDTO = new ResetDoneEmailDTO(resetRequest.BinID, resetRequest.ScaleID, resetRequest.ChannelID
                                    , resetRequest.ItemGUID, resetRequest.CompanyID, resetRequest.RoomID
                                    , resetRequest.RoomName, resetRequest.CompanyName
                                    , resetRequest.BinNumber, resetRequest.ItemNumber
                                    );

                                    ResetProcessForItem(resetRequest, resetEmailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID);
                                    resetEmailList.Add(resetEmailDTO);

                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    resetRequests = objResetRequestDAL.GetResetRequestByRoom(room.RoomID, room.CompanyID, false, false, null);
                                }
                            }

                            EmailWrapper.SendResetDoneEmail(resetEmailList);
                        }

                    } // loop through rooms


                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("TareProcess", ex);
                }


            }

        }

        private void ResetProcessForItem(eVMIResetReqDTO resetRequest, ResetDoneEmailDTO resetEmailDTO
            , string EnterpriseDBName, long EnterpriseID, long systemUserID)
        {
            EVMIResetRequestDAL objResetRequestDAL = new EVMIResetRequestDAL(EnterpriseDBName);
            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(resetRequest.RoomID, resetRequest.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                resetRequest.IsResetStarted = true;
                resetRequest.ResetStartTime = CurrentTimeofTimeZone;
                resetRequest.UpdatedBy = systemUserID;
                // update Tare start status
                objResetRequestDAL.UpdateResetRequest(resetRequest);

                //double dbWeight = 0;
                bool isTareCompleted = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(resetRequest.ComPortName))
                    {
                        try
                        {
                            var res = comWrapper.ResetScale(resetRequest.ScaleID);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                resetEmailDTO.ComError = errorDescription;
                                isTareCompleted = false;

                                WriteComErrorLog("Reset Process Error", EnterpriseID, resetRequest.CompanyID,
                                   resetRequest.RoomID, resetRequest.ItemGUID, resetRequest.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isTareCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            resetEmailDTO.ComError = ex.Message;

                            WriteComErrorLog("Reset Process Error", EnterpriseID, resetRequest.CompanyID,
                                  resetRequest.RoomID, resetRequest.ItemGUID, resetRequest.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                resetRequest.IsResetCompleted = true;

                if (!isTareCompleted)
                {
                    resetRequest.ErrorDescription = errorDescription;
                }
                else
                {
                    resetRequest.ErrorDescription = null;
                }
                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                resetRequest.ResetCompletionTime = CurrentTimeofTimeZone;
                resetRequest.UpdatedBy = systemUserID;
                // update Tare completed status
                objResetRequestDAL.UpdateResetRequest(resetRequest);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Reset Process Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, resetRequest.CompanyID, resetRequest.RoomID,
                                  resetRequest.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                resetEmailDTO.ExceptionError = ex.Message;
            }
        }

        public void ResetProcessForItem(long requestID, string enterpriseDBName, long enterpriseID, long companyId, long roomId,long userID)
        {
            try
            {
                EVMIResetRequestDAL objResetRequestDAL = new EVMIResetRequestDAL(enterpriseDBName);
                eVMIResetReqDTO resetRequest = objResetRequestDAL.GetResetRequestByRoom(roomId, companyId, false, false, requestID).FirstOrDefault();
                if (resetRequest != null)
                {
                    PollEmailDTOCollection<ResetDoneEmailDTO> resetEmailList = new PollEmailDTOCollection<ResetDoneEmailDTO>(enterpriseID, enterpriseDBName);
                    ResetDoneEmailDTO resetEmailDTO = new ResetDoneEmailDTO(resetRequest.BinID, resetRequest.ScaleID, resetRequest.ChannelID
                                           , resetRequest.ItemGUID, resetRequest.CompanyID, resetRequest.RoomID
                                           , resetRequest.RoomName, resetRequest.CompanyName
                                           , resetRequest.BinNumber, resetRequest.ItemNumber
                                           );
                    
                    ResetProcessForItem(resetRequest, resetEmailDTO, enterpriseDBName, enterpriseID, userID);
                    resetEmailList.Add(resetEmailDTO);
                    EmailWrapper.SendResetDoneEmail(resetEmailList);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("ResetProcessForItem", ex);
            }

        }

    }
}
