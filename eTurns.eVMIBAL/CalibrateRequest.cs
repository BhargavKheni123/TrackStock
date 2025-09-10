using eTurns.DAL;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using eTurns.eVMIBAL.Wrappers;
using eTurns.eVMIBAL.DTO;

namespace eTurns.eVMIBAL
{
    public class CalibrateRequest : PollRequestBase
    {
        #region Step 1

        //public List<ProcessedCalibrateRequest> Step1Process()
        //{
        //    List<ProcessedCalibrateRequest> procesedRequests = new List<ProcessedCalibrateRequest>();
        //    List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

        //    // loop through enterprises which have evmi rooms
        //    //foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
        //    Parallel.ForEach<EVMIRoomDTO>(eVMIEnterprises, enterprise =>
        //    {
        //        try
        //        {

        //            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(enterprise.EnterpriseDBName);
        //            var rooms = calibrateRequestDAL.GetItemLocationCalibrateStep1Rooms();
        //            UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

        //            // loop through rooms in enterprise
        //            foreach (var room in rooms)
        //            {
        //                int pageNo = 1;

        //                List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep1RequestByRoom(room.RoomID, room.CompanyID, pageNo);

        //                int NoOfPages = 0;
        //                int TotalRecords = 0;
        //                int RecordsPerPage = 0;

        //                if (requests.Count > 0)
        //                {
        //                    PollEmailDTOCollection<CalibrateDoneEmail> calibrateEmailDTOsList = new PollEmailDTOCollection<CalibrateDoneEmail>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

        //                    NoOfPages = requests[0].NoOfPages;
        //                    TotalRecords = requests[0].TotalRequest;
        //                    RecordsPerPage = requests[0].RecordsPerPage;

        //                    for (int i = 1; i <= NoOfPages; i++)
        //                    {
        //                        foreach (ILCalibrateRequestDTO request in requests)
        //                        {
        //                            CalibrateDoneEmail calibrateEmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
        //                            , request.ItemGUID, request.CompanyID, request.RoomID
        //                            , request.RoomName, request.CompanyName
        //                            , request.BinNumber, request.ItemNumber
        //                            , CalibrateStepEnum.Step1
        //                            );

        //                            Step1ProcessForItem(request, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser, calibrateEmailDTO);
        //                            calibrateEmailDTOsList.Add(calibrateEmailDTO);

        //                            procesedRequests.Add(new ProcessedCalibrateRequest()
        //                            {
        //                                Request = request,
        //                                EnterpriseDBName = enterprise.EnterpriseDBName,
        //                                EnterpriseID = enterprise.EnterpriseID,
        //                                SystemUser = systemUser,
        //                                CalibrateStep1EmailDTO = calibrateEmailDTO
        //                            });
        //                        }

        //                        pageNo++;
        //                        if (pageNo <= NoOfPages)
        //                        {
        //                            requests = calibrateRequestDAL.GetILCalibrateStep1RequestByRoom(room.RoomID, room.CompanyID, pageNo);
        //                        }
        //                    }

        //                    //EmailWrapper.SendCalibrateDoneEmail(calibrateEmailDTOsList);
        //                }

        //            } // loop through rooms


        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.WriteExceptionLog("Calibrate Step1Process", ex);
        //        }


        //    });

        //    return procesedRequests;
        //}

        private void Step1ProcessForItem(ItemLocationCalibrateRequestDTO request, string EnterpriseDBName, long EnterpriseID, long systemUserID,
            CalibrateDoneEmail calibrateEmailDTO)
        {
            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);

            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(request.RoomID, request.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                request.IsStep1Started = true;
                request.Step1StartTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Tare start status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);

                //double dbWeight = 0;
                bool isStep1Completed = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(request.ComPortName))
                    {
                        try
                        {
                            var res = comWrapper.CalibrationStep1(request.ScaleID, request.ChannelID);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                calibrateEmailDTO.ComError = errorDescription;
                                isStep1Completed = false;

                                WriteComErrorLog("Calibrate Step1 Process Error", EnterpriseID, request.CompanyID,
                                   request.RoomID, request.ItemGUID, request.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isStep1Completed = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            calibrateEmailDTO.ComError = ex.Message;
                            WriteComErrorLog("Calibrate Step1 Process Error", EnterpriseID, request.CompanyID,
                                  request.RoomID, request.ItemGUID, request.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                if (!isStep1Completed)
                {
                    request.ErrorDescription += errorDescription;
                }
                else
                {
                    request.IsStep1Completed = true;
                    request.ErrorDescription = null;
                }

                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                request.Step1CompletionTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Completed status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Calibrate Step1 Process Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, request.CompanyID, request.RoomID,
                                  request.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                calibrateEmailDTO.ExceptionError = ex.Message;
            }
        }

        #endregion

        #region Step 2

        //public void Step2Process()
        //{
        //    List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

        //    // loop through enterprises which have evmi rooms
        //    //foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
        //    Parallel.ForEach<EVMIRoomDTO>(eVMIEnterprises, enterprise =>
        //    {
        //        try
        //        {

        //            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(enterprise.EnterpriseDBName);
        //            var rooms = calibrateRequestDAL.GetItemLocationCalibrateStep2Rooms();
        //            UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

        //            // loop through rooms in enterprise
        //            foreach (var room in rooms)
        //            {
        //                int pageNo = 1;

        //                List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep2RequestByRoom(room.RoomID, room.CompanyID, pageNo);

        //                int NoOfPages = 0;
        //                int TotalRecords = 0;
        //                int RecordsPerPage = 0;

        //                if (requests.Count > 0)
        //                {
        //                    PollEmailDTOCollection<CalibrateDoneEmail> calibrateEmailDTOsList = new PollEmailDTOCollection<CalibrateDoneEmail>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

        //                    NoOfPages = requests[0].NoOfPages;
        //                    TotalRecords = requests[0].TotalRequest;
        //                    RecordsPerPage = requests[0].RecordsPerPage;

        //                    for (int i = 1; i <= NoOfPages; i++)
        //                    {
        //                        foreach (ILCalibrateRequestDTO request in requests)
        //                        {
        //                            CalibrateDoneEmail calibrateEmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
        //                            , request.ItemGUID, request.CompanyID, request.RoomID
        //                            , request.RoomName, request.CompanyName
        //                            , request.BinNumber, request.ItemNumber
        //                            , CalibrateStepEnum.Step2
        //                            );

        //                            Step2ProcessForItem(request, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID, calibrateEmailDTO);
        //                            calibrateEmailDTOsList.Add(calibrateEmailDTO);

        //                        }

        //                        pageNo++;
        //                        if (pageNo <= NoOfPages)
        //                        {
        //                            requests = calibrateRequestDAL.GetILCalibrateStep2RequestByRoom(room.RoomID, room.CompanyID, pageNo);
        //                        }
        //                    }

        //                    //EmailWrapper.SendCalibrateDoneEmail(calibrateEmailDTOsList);
        //                }

        //            } // loop through rooms


        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.WriteExceptionLog("Calibrate Step2 Process", ex);
        //        }


        //    });

        //}

        private void Step2ProcessForItem(ItemLocationCalibrateRequestDTO request, string EnterpriseDBName, long EnterpriseID, long systemUserID,
            CalibrateDoneEmail calibrateEmailDTO)
        {
            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);

            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(request.RoomID, request.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                request.IsStep2Started = true;
                request.Step2StartTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Tare start status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);

                //double dbWeight = 0;
                bool isStep2Completed = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(request.ComPortName))
                    {
                        try
                        {
                            var res = comWrapper.CalibrationStep2(request.ScaleID, request.ChannelID);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                calibrateEmailDTO.ComError = errorDescription;
                                isStep2Completed = false;

                                WriteComErrorLog("Calibrate Step2 Process Error", EnterpriseID, request.CompanyID,
                                   request.RoomID, request.ItemGUID, request.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isStep2Completed = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            calibrateEmailDTO.ComError = ex.Message;
                            WriteComErrorLog("Calibrate Step2 Process Error", EnterpriseID, request.CompanyID,
                                  request.RoomID, request.ItemGUID, request.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                if (!isStep2Completed)
                {
                    request.ErrorDescription += errorDescription;
                }
                else
                {
                    request.IsStep2Completed = true;
                    request.ErrorDescription = null;
                }

                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                request.Step2CompletionTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Completed status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Calibrate Step2 Process Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, request.CompanyID, request.RoomID,
                                  request.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                calibrateEmailDTO.ExceptionError = ex.Message;
            }
        }

        #endregion

        #region Step 3

        //public void Step3Process()
        //{
        //    List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

        //    // loop through enterprises which have evmi rooms
        //    //foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
        //    Parallel.ForEach<EVMIRoomDTO>(eVMIEnterprises, enterprise =>
        //    {
        //        try
        //        {

        //            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(enterprise.EnterpriseDBName);
        //            var rooms = calibrateRequestDAL.GetItemLocationCalibrateStep3Rooms();
        //            UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

        //            // loop through rooms in enterprise
        //            foreach (var room in rooms)
        //            {
        //                int pageNo = 1;

        //                List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep3RequestByRoom(room.RoomID, room.CompanyID, pageNo);

        //                int NoOfPages = 0;
        //                int TotalRecords = 0;
        //                int RecordsPerPage = 0;

        //                if (requests.Count > 0)
        //                {
        //                    PollEmailDTOCollection<CalibrateDoneEmail> calibrateEmailDTOsList = new PollEmailDTOCollection<CalibrateDoneEmail>(enterprise.EnterpriseID, enterprise.EnterpriseDBName);

        //                    NoOfPages = requests[0].NoOfPages;
        //                    TotalRecords = requests[0].TotalRequest;
        //                    RecordsPerPage = requests[0].RecordsPerPage;

        //                    for (int i = 1; i <= NoOfPages; i++)
        //                    {
        //                        foreach (ILCalibrateRequestDTO request in requests)
        //                        {
        //                            CalibrateDoneEmail calibrateEmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
        //                            , request.ItemGUID, request.CompanyID, request.RoomID
        //                            , request.RoomName, request.CompanyName
        //                            , request.BinNumber, request.ItemNumber
        //                            , CalibrateStepEnum.Step3
        //                            );

        //                            Step3ProcessForItem(request, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser, calibrateEmailDTO);
        //                            calibrateEmailDTOsList.Add(calibrateEmailDTO);

        //                        }

        //                        pageNo++;
        //                        if (pageNo <= NoOfPages)
        //                        {
        //                            requests = calibrateRequestDAL.GetILCalibrateStep3RequestByRoom(room.RoomID, room.CompanyID, pageNo);
        //                        }
        //                    }

        //                    //EmailWrapper.SendCalibrateDoneEmail(calibrateEmailDTOsList);
        //                }

        //            } // loop through rooms


        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.WriteExceptionLog("Calibrate Step3 Process", ex);
        //        }


        //    });

        //}

        private void Step3ProcessForItem(ItemLocationCalibrateRequestDTO request, string EnterpriseDBName, 
            long EnterpriseID, long systemUserID,
            CalibrateDoneEmail calibrateEmailDTO)
        {
            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);

            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(request.RoomID, request.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                request.IsStep3Started = true;
                request.Step3StartTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Tare start status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);

                //double dbWeight = 0;
                bool isStep3Completed = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(request.ComPortName))
                    {
                        try
                        {
                            var res = comWrapper.CalibrationStep3(request.ScaleID, request.ChannelID);

                            if (!res.IsSuccess)
                            {
                                errorDescription = res.CommandData.ErrorInfo;
                                calibrateEmailDTO.ComError = errorDescription;
                                isStep3Completed = false;

                                WriteComErrorLog("Calibrate Step3 Process Error", EnterpriseID, request.CompanyID,
                                   request.RoomID, request.ItemGUID, request.ComPortName
                                   , errorDescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            isStep3Completed = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            calibrateEmailDTO.ComError = ex.Message;
                            WriteComErrorLog("Calibrate Step3 Process Error", EnterpriseID, request.CompanyID,
                                  request.RoomID, request.ItemGUID, request.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                if (!isStep3Completed)
                {
                    request.ErrorDescription += errorDescription;
                }
                else
                {
                    request.IsStep3Completed = true;
                    request.ErrorDescription = null;
                }

                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                request.Step3CompletionTime = CurrentTimeofTimeZone;
                request.UpdatedBy = systemUserID;
                // update Completed status
                calibrateRequestDAL.UpdateItemLocationCalibrateRequest(request);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "Calibrate Step3 Process Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2} , ItemGUID : {3} "
                                , EnterpriseID, request.CompanyID, request.RoomID,
                                  request.ItemGUID.ToString());
                Logger.WriteExceptionLog(log, ex);
                calibrateEmailDTO.ExceptionError = ex.Message;
            }
        }

        #endregion


        /// <summary>
        /// Proces Step1 And Step2 for same request
        /// </summary>
        /// <returns></returns>
        public string ProcesStep1(Int64 calRequestID,
            string EnterpriseDBName, long EnterpriseID, long CompanyID, long RoomID, long systemUserID)
        {
            string msg = "";
            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);
            List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep1RequestByRoom(RoomID, CompanyID, 0, calRequestID);

            ItemLocationCalibrateRequestDTO calRequest = requests.FirstOrDefault();

            // process for step 1
            CalibrateDoneEmail CalibrateStep1EmailDTO = new CalibrateDoneEmail(calRequest.BinID, calRequest.ScaleID, calRequest.ChannelID
                                    , calRequest.ItemGUID, calRequest.CompanyID, calRequest.RoomID
                                    , calRequest.RoomName, calRequest.CompanyName
                                    , calRequest.BinNumber, calRequest.ItemNumber
                                    , CalibrateStepEnum.Step1
                                    );

            this.Step1ProcessForItem(calRequest, EnterpriseDBName
                , EnterpriseID, systemUserID, CalibrateStep1EmailDTO);
                        
            if (calRequest.IsStep1Completed)
            {
               
            }
            else
            {
                msg += "Command 1 Error - " + calRequest.ErrorDescription;
            }

            return msg;
        }

        public string ProcesStep2(Int64 calRequestID,
           string EnterpriseDBName, long EnterpriseID, long CompanyID, long RoomID, long systemUserID)
        {
            string msg = "";
            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);
            List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep2RequestByRoom(RoomID, CompanyID, 0, calRequestID);

            ItemLocationCalibrateRequestDTO calRequest = requests.FirstOrDefault();
                        
            if (calRequest.IsStep1Completed)
            {

                //var request = calibrateRequestDAL.GetILCalibrateStep2RequestByRoom(RoomID, CompanyID, 0, calRequestID).FirstOrDefault();
                var request = calRequest;
                CalibrateDoneEmail CalibrateStep2EmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
                                   , request.ItemGUID, request.CompanyID, request.RoomID
                                   , request.RoomName, request.CompanyName
                                   , request.BinNumber, request.ItemNumber
                                   , CalibrateStepEnum.Step2
                                   );

                this.Step2ProcessForItem(calRequest, EnterpriseDBName, EnterpriseID, systemUserID, CalibrateStep2EmailDTO);

                if (calRequest.IsStep2Completed == false)
                {
                    msg += "Command 2 Error - " + calRequest.ErrorDescription;
                }

            }           

            return msg;
        }


        ///// <summary>
        ///// Proces Step1 And Step2 for same request
        ///// </summary>
        ///// <returns></returns>
        //public string ProcesStep1AndStep2(Int64 calRequestID,
        //    string EnterpriseDBName, long EnterpriseID, long CompanyID, long RoomID, long systemUserID)
        //{
        //    string msg = "";
        //    ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);
        //    List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep1RequestByRoom(RoomID, CompanyID, 0, calRequestID);

        //    ItemLocationCalibrateRequestDTO calRequest = requests.FirstOrDefault();

        //    // process for step 1
        //    CalibrateDoneEmail CalibrateStep1EmailDTO = new CalibrateDoneEmail(calRequest.BinID, calRequest.ScaleID, calRequest.ChannelID
        //                            , calRequest.ItemGUID, calRequest.CompanyID, calRequest.RoomID
        //                            , calRequest.RoomName, calRequest.CompanyName
        //                            , calRequest.BinNumber, calRequest.ItemNumber
        //                            , CalibrateStepEnum.Step1
        //                            );

        //    this.Step1ProcessForItem(calRequest, EnterpriseDBName
        //        , EnterpriseID, systemUserID, CalibrateStep1EmailDTO);

        //    // process for step 2
        //    if (calRequest.IsStep1Completed)
        //    {
        //        //var request = calRequest;
        //        var request = calibrateRequestDAL.GetILCalibrateStep2RequestByRoom(RoomID, CompanyID, 0, calRequestID).FirstOrDefault();

        //        CalibrateDoneEmail CalibrateStep2EmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
        //                           , request.ItemGUID, request.CompanyID, request.RoomID
        //                           , request.RoomName, request.CompanyName
        //                           , request.BinNumber, request.ItemNumber
        //                           , CalibrateStepEnum.Step2
        //                           );

        //        this.Step2ProcessForItem(calRequest, EnterpriseDBName, EnterpriseID, systemUserID, CalibrateStep2EmailDTO);

        //        if (calRequest.IsStep2Completed == false)
        //        {
        //            msg += "Command 2 Error - " + calRequest.ErrorDescription;
        //        }
                

        //    }
        //    else
        //    {
        //        msg += "Command 1 Error - " + calRequest.ErrorDescription;
        //    }

        //    return msg;
        //}

        public string ProcesStep3(Int64 calRequestID, double CalibrateWeight,
            string EnterpriseDBName, long EnterpriseID, long CompanyID, long RoomID, long systemUserID)
        {
            string msg = "";

            ItemLocationCalibrateRequestDAL calibrateRequestDAL = new ItemLocationCalibrateRequestDAL(EnterpriseDBName);
            List<ILCalibrateRequestDTO> requests = calibrateRequestDAL.GetILCalibrateStep3RequestByRoom(RoomID, CompanyID, 0, calRequestID);

            ItemLocationCalibrateRequestDTO request = requests.FirstOrDefault();

            if(request != null)
            {
                request.CalibrationWeight = CalibrateWeight;
            }

            CalibrateDoneEmail CalibrateStep3EmailDTO = new CalibrateDoneEmail(request.BinID, request.ScaleID, request.ChannelID
                                    , request.ItemGUID, request.CompanyID, request.RoomID
                                    , request.RoomName, request.CompanyName
                                    , request.BinNumber, request.ItemNumber
                                    , CalibrateStepEnum.Step3
                                    );

            this.Step3ProcessForItem(request, EnterpriseDBName
               , EnterpriseID, systemUserID, CalibrateStep3EmailDTO);

            if(request.IsStep3Completed == false)
            {
                msg += "Command 3 Error - " + request.ErrorDescription;
            }

            return msg;
        }

    }// class
}
