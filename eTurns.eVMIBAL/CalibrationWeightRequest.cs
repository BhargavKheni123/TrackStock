using eTurns.DAL;
using eTurns.DTO;
using eTurns.eVMIBAL.DTO;
using eTurns.eVMIBAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eTurns.eVMIBAL
{
    public class CalibrationWeightRequest : PollRequestBase
    {

        public void CalWeightProcess(eVMICalibrationWeightRequestType weightRequestType)
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {

                    var objRequestDAL = new eVMICalibrationWeightRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = objRequestDAL.GetCalRequestRooms(weightRequestType);
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<eVMICalReqDTO> calRequests = objRequestDAL.GetCalWeightRequestByRoom(room.RoomID, room.CompanyID, false, false, (int)weightRequestType, null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (calRequests.Count > 0)
                        {
                            PollEmailDTOCollection<GetCalibrationWeightDoneEmailDTO> emailList = new PollEmailDTOCollection<GetCalibrationWeightDoneEmailDTO>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

                            NoOfPages = calRequests[0].NoOfPages;
                            TotalRecords = calRequests[0].TotalRequest;
                            RecordsPerPage = calRequests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (eVMICalReqDTO calRequest in calRequests)
                                {
                                    var resetEmailDTO = new GetCalibrationWeightDoneEmailDTO(calRequest.BinID, calRequest.ScaleID ?? 0
                                        , 0
                                    , calRequest.ItemGUID, calRequest.CompanyID, calRequest.RoomID
                                    , calRequest.RoomName, calRequest.CompanyName
                                    , calRequest.BinNumber, calRequest.ItemNumber
                                    );

                                    ProcessCalibrateWeightRequestForScale(calRequest, resetEmailDTO, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID, weightRequestType);
                                    emailList.Add(resetEmailDTO);

                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    calRequests = objRequestDAL.GetCalWeightRequestByRoom(room.RoomID, room.CompanyID, false, false, (int)weightRequestType, null);
                                }
                            }

                            EmailWrapper.SendCalibrationGetWeightDoneEmail(emailList);
                        }

                    } // loop through rooms


                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("GetSetCalibrateWeight", ex);
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
        public eVMICalibrationWeightRequestDTO ProcessCaliWeightRequestForScale(long requestID, string enterpriseDBName, long enterpriseID, long companyId, long roomId, long userID
            ,eVMICalibrationWeightRequestType weightRequestType
            )
        {
            double weight = 0;
            eVMICalibrationWeightRequestDTO request = null;
            try
            {
                eVMICalibrationWeightRequestDAL requestDAL = new eVMICalibrationWeightRequestDAL(enterpriseDBName);
                request = requestDAL.GetCalWeightRequestByRoom(roomId, companyId,
                    false, false,
                    (int)weightRequestType,
                    requestID).FirstOrDefault();

                if (request != null)
                {
                    var emailList = new PollEmailDTOCollection<GetCalibrationWeightDoneEmailDTO>(enterpriseID, enterpriseDBName);
                    GetCalibrationWeightDoneEmailDTO emailDTO = new GetCalibrationWeightDoneEmailDTO(request.BinID, request.ScaleID ?? 0
                        , 0
                                           , request.ItemGUID, request.CompanyID, request.RoomID
                                           , request.RoomName, request.CompanyName
                                           , request.BinNumber, request.ItemNumber
                                           );

                    ProcessCalibrateWeightRequestForScale(request, emailDTO, enterpriseDBName, enterpriseID, userID, weightRequestType);
                    emailList.Add(emailDTO);
                    weight = request.CalibrationWeight ?? 0;
                    EmailWrapper.SendCalibrationGetWeightDoneEmail(emailList);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("GetSetCalibrateWeight", ex);
            }

            return request;
        }

        private void ProcessCalibrateWeightRequestForScale(eVMICalibrationWeightRequestDTO requestDTO, 
            GetCalibrationWeightDoneEmailDTO emailDTO
            , string EnterpriseDBName, long EnterpriseID, long systemUserID, eVMICalibrationWeightRequestType weightRequestType)
        {
            eVMICalibrationWeightRequestDAL requestDAL = new eVMICalibrationWeightRequestDAL(EnterpriseDBName);
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

                requestDTO.IsCalWeightStarted = true;
                requestDTO.CalWeightStartTime = CurrentTimeofTimeZone;
                requestDTO.UpdatedBy = systemUserID;
                // update start status
                requestDAL.UpdateCalWeightRequest(requestDTO);

                //double dbWeight = 0;
                bool isReqCompleted = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(requestDTO.ComPortName))
                    {
                        try
                        {
                            COMCmdResponse res = null;

                            if (weightRequestType == eVMICalibrationWeightRequestType.GetCalibrationWeight ||
                                weightRequestType == eVMICalibrationWeightRequestType.GetCalibrationWeightImmediate
                                )
                            {
                                res = comWrapper.GetCalibrationWeight(requestDTO.ScaleID ?? 0) ;

                                decimal dcWeight = ((COMCmdResponse<decimal?>)res).data ?? 0;
                                double dbWeight = Convert.ToDouble(dcWeight);

                                if (res.IsSuccess)
                                {
                                    requestDTO.CalibrationWeight = dbWeight;
                                }
                            }
                            else if(weightRequestType == eVMICalibrationWeightRequestType.SetCalibrationWeight || 
                                weightRequestType == eVMICalibrationWeightRequestType.SetCalibrationWeightImmediate
                                )
                            {
                                res = comWrapper.SetCalibrationWeight(requestDTO.ScaleID ?? 0, requestDTO.CalibrationWeight.Value);
                            }

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                emailDTO.ComError = errorDescription;
                                isReqCompleted = false;

                                WriteComErrorLog("Get Calibrate Weight Error", EnterpriseID, requestDTO.CompanyID,
                                   requestDTO.RoomID, requestDTO.ItemGUID, requestDTO.ComPortName
                                   , errorDescription);
                            }

                        }
                        catch (Exception ex)
                        {
                            isReqCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            emailDTO.ComError = ex.Message;

                            WriteComErrorLog("Get Calibrate Weight Error", EnterpriseID, requestDTO.CompanyID,
                                  requestDTO.RoomID, requestDTO.ItemGUID, requestDTO.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                requestDTO.IsCalWeightCompleted = true;

                if (!isReqCompleted)
                {
                    requestDTO.ErrorDescription = errorDescription;
                }
                else
                {
                    requestDTO.ErrorDescription = null;
                }
                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                requestDTO.CalWeightCompletionTime = CurrentTimeofTimeZone;
                requestDTO.UpdatedBy = systemUserID;
                // update completed status
                requestDAL.UpdateCalWeightRequest(requestDTO);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Get Calibrate Weight Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, requestDTO.CompanyID, requestDTO.RoomID,
                                  requestDTO.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                emailDTO.ExceptionError = ex.Message;
            }
        }


    }// class
}
