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
    public class EVMICommonRequest : PollRequestBase
    {


        public void CommonRequestProcess(eVMICOMCommonRequestType requestType)
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {

                    var objRequestDAL = new eVMICOMCommonRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objRequestDAL.GetCOMCommonRequestRooms(requestType);
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<eVMICommonReqDTO> commonRequests = objRequestDAL.GetCOMCommonRequestByRoom(requestType, room.RoomID, room.CompanyID, false, false,  null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (commonRequests.Count > 0)
                        {
                            PollEmailDTOCollection<CommonRequestDoneEmailDTO> emailList = new PollEmailDTOCollection<CommonRequestDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

                            NoOfPages = commonRequests[0].NoOfPages;
                            TotalRecords = commonRequests[0].TotalRequest;
                            RecordsPerPage = commonRequests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (eVMICommonReqDTO commonRequest in commonRequests)
                                {
                                    var emailDTO = new CommonRequestDoneEmailDTO(commonRequest.BinID, commonRequest.ScaleID ?? 0
                                        , 0
                                    , Guid.Empty, commonRequest.CompanyID, commonRequest.RoomID
                                    , commonRequest.RoomName, commonRequest.CompanyName
                                    , "", ""
                                    );

                                    ProcessCommonRequestForScale(commonRequest, emailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID, requestType);
                                    emailList.Add(emailDTO);

                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    commonRequests = objRequestDAL.GetCOMCommonRequestByRoom(requestType, room.RoomID, room.CompanyID, false, false, null);
                                }
                            }

                            EmailWrapper.SendCommonCommandDoneEmail(emailList);
                        }

                    } // loop through rooms


                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("CommonRequest", ex);
                }


            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="enterpriseDBName"></param>
        /// <param name="enterpriseID"></param>
        /// <param name="companyId"></param>
        /// <param name="roomId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public eVMICommonReqDTO ProcessCommonRequestForScale(long requestID, string enterpriseDBName, long enterpriseID, long companyId, long roomId, long userID
            , eVMICOMCommonRequestType requestType
            )
        {
            
            eVMICommonReqDTO commonRequest = null;
            try
            {
                eVMICOMCommonRequestDAL requestDAL = new eVMICOMCommonRequestDAL(enterpriseDBName);
                commonRequest = requestDAL.GetCOMCommonRequestByRoom(requestType, roomId, companyId,
                    false, false,  requestID).FirstOrDefault();

                if (commonRequest != null)
                {
                    var emailList = new PollEmailDTOCollection<CommonRequestDoneEmailDTO>(enterpriseID, enterpriseDBName);

                    var emailDTO = new CommonRequestDoneEmailDTO(commonRequest.BinID, commonRequest.ScaleID ?? 0
                                        , 0
                                    , Guid.Empty, commonRequest.CompanyID, commonRequest.RoomID
                                    , commonRequest.RoomName, commonRequest.CompanyName
                                    , "", ""
                                    );

                    ProcessCommonRequestForScale(commonRequest, emailDTO, enterpriseDBName, enterpriseID, userID, requestType);
                    emailList.Add(emailDTO);
                    
                    EmailWrapper.SendCommonCommandDoneEmail(emailList);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("CommonRequest", ex);
            }

            return commonRequest;
        }

        private void ProcessCommonRequestForScale(eVMICommonReqDTO requestDTO,
            CommonRequestDoneEmailDTO emailDTO , 
            string EnterpriseDBName, long EnterpriseID, long systemUserID, eVMICOMCommonRequestType requestType)
        {
            eVMICOMCommonRequestDAL requestDAL = new eVMICOMCommonRequestDAL(EnterpriseDBName);
            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(requestDTO.RoomID, requestDTO.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                requestDTO.IsComReqStarted = true;
                requestDTO.ComStartTime = CurrentTimeofTimeZone;
                requestDTO.UpdatedBy = systemUserID;
                // update start status
                requestDAL.UpdateCOMCommonRequest(requestDTO);

                //double dbWeight = 0;
                bool isReqCompleted = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(requestDTO.ComPortName))
                    {
                        try
                        {
                            COMCmdResponse res = ExecuteCommand(comWrapper, requestType, requestDTO);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                emailDTO.ComError = errorDescription;
                                isReqCompleted = false;

                                WriteComErrorLog("CommonRequest Error", EnterpriseID, requestDTO.CompanyID,
                                   requestDTO.RoomID, requestDTO.ItemGUID, requestDTO.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isReqCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            emailDTO.ComError = ex.Message;

                            WriteComErrorLog("CommonRequest Error", EnterpriseID, requestDTO.CompanyID,
                                  requestDTO.RoomID, requestDTO.ItemGUID, requestDTO.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                requestDTO.IsComCompleted = true;

                if (!isReqCompleted)
                {
                    requestDTO.ErrorDescription = errorDescription;
                }
                else
                {
                    requestDTO.ErrorDescription = null;
                }
                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                requestDTO.ComCompletionTime = CurrentTimeofTimeZone;
                requestDTO.UpdatedBy = systemUserID;
                // update completed status
                requestDAL.UpdateCOMCommonRequest(requestDTO);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "CommonRequest Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, requestDTO.CompanyID, requestDTO.RoomID,
                                  requestDTO.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                emailDTO.ExceptionError = ex.Message;
            }
        }

        private COMCmdResponse ExecuteCommand(COMWrapper comWrapper, eVMICOMCommonRequestType requestType, 
            eVMICommonReqDTO requestDTO)
        {
            COMCmdResponse res = null;

            if (requestType == eVMICOMCommonRequestType.GetFirmWareVersion ||
                requestType == eVMICOMCommonRequestType.GetFirmWareVersionImmediate
                )
            {
                res = comWrapper.GetFirmwareVersion(requestDTO.ScaleID ?? 0);

                string version = ((COMCmdResponse<string>)res).data ?? "";

                if (res.IsSuccess)
                {
                    requestDTO.Version = version;
                }
            }
            else if (requestType == eVMICOMCommonRequestType.GetSerialNo ||
                requestType == eVMICOMCommonRequestType.GetSerialNoImmediate
                )
            {
                res = comWrapper.GetSerialNo(requestDTO.ScaleID ?? 0);
                string SerialNumber = ((COMCmdResponse<string>)res).data ?? "";

                if (res.IsSuccess)
                {
                    requestDTO.SerialNumber = SerialNumber;
                }
            }
            else if (requestType == eVMICOMCommonRequestType.GetModelNo ||
                requestType == eVMICOMCommonRequestType.GetModelNoImmediate
                )
            {
                res = comWrapper.GetModelNumber(requestDTO.ScaleID ?? 0);
                string ModelNumber = ((COMCmdResponse<string>)res).data ?? "";

                if (res.IsSuccess)
                {
                    requestDTO.ModelNumber = ModelNumber;
                }
            }
            else if (requestType == eVMICOMCommonRequestType.SetModelNo ||
                requestType == eVMICOMCommonRequestType.SetModelNoImmediate
                )
            {
                res = comWrapper.SetModelNumber(requestDTO.ScaleID ?? 0, requestDTO.ModelNumber);
                string ModelNumber = ((COMCmdResponse<string>)res).data ?? "";

            }

            return res;
        }

        //public void GetFirmWareVersionProcess()
        //{
        //    List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

        //    // loop through enterprises which have evmi rooms
        //    foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
        //    {
        //        try
        //        {

        //            eVMICOMCommonRequestDAL objRequestDAL = new eVMICOMCommonRequestDAL(enterprise.EnterpriseDBName);
        //            var rooms = objRequestDAL.GetCOMCommonRequestRooms(eVMICOMCommonRequestType.GetFirmWareVersion);

        //            UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

        //            // loop through rooms in enterprise
        //            foreach (var room in rooms)
        //            {
        //                int pageNo = 1;

        //                List<eVMIResetReqDTO> resetRequests = objRequestDAL.GetCOMCommonRequestByRoom( eVMICOMCommonRequestType.GetFirmWareVersion ,room.RoomID,
        //                    room.CompanyID, false, false, null);

        //                int NoOfPages = 0;
        //                int TotalRecords = 0;
        //                int RecordsPerPage = 0;

        //                if (resetRequests.Count > 0)
        //                {
        //                    PollEmailDTOCollection<ResetDoneEmailDTO> resetEmailList = new PollEmailDTOCollection<ResetDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

        //                    NoOfPages = resetRequests[0].NoOfPages;
        //                    TotalRecords = resetRequests[0].TotalRequest;
        //                    RecordsPerPage = resetRequests[0].RecordsPerPage;

        //                    for (int i = 1; i <= NoOfPages; i++)
        //                    {
        //                        foreach (eVMIResetReqDTO resetRequest in resetRequests)
        //                        {
        //                            ResetDoneEmailDTO resetEmailDTO = new ResetDoneEmailDTO(resetRequest.BinID, resetRequest.ScaleID, resetRequest.ChannelID
        //                            , resetRequest.ItemGUID, resetRequest.CompanyID, resetRequest.RoomID
        //                            , resetRequest.RoomName, resetRequest.CompanyName
        //                            , resetRequest.BinNumber, resetRequest.ItemNumber
        //                            );

        //                            ResetProcessForItem(resetRequest, resetEmailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID);
        //                            resetEmailList.Add(resetEmailDTO);

        //                        }

        //                        pageNo++;
        //                        if (pageNo <= NoOfPages)
        //                        {
        //                            resetRequests = objRequestDAL.GetResetRequestByRoom(room.RoomID, room.CompanyID, false, false, null);
        //                        }
        //                    }

        //                    EmailWrapper.SendResetDoneEmail(resetEmailList);
        //                }

        //            } // loop through rooms


        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.WriteExceptionLog("TareProcess", ex);
        //        }


        //    }
        //}

    }
}
