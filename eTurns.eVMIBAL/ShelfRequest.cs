using eTurns.DAL;
using eTurns.DTO;
using eTurns.eVMIBAL.DTO;
using eTurns.eVMIBAL.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eTurns.eVMIBAL
{
    public class ShelfRequest : PollRequestBase
    {
        public void ShelfRequestProcess()
        {
            List<EVMIRoomDTO> eVMIEnterprises = SharedData.eVMIRooms;

            // loop through enterprises which have evmi rooms
            foreach (EVMIRoomDTO enterprise in eVMIEnterprises)
            {
                try
                {

                    eVMIShelfRequestDAL shelfRequestDAL = new eVMIShelfRequestDAL(enterprise.EnterpriseDBName);
                    var rooms = shelfRequestDAL.GeteVMIShelfRequestRooms();
                    UserMasterDTO systemUser = GetSystemUser(enterprise.EnterpriseDBName);

                    // loop through rooms in enterprise
                    foreach (var room in rooms)
                    {
                        int pageNo = 1;

                        List<eVMIShelfReqDTO> requests = shelfRequestDAL.GeteVMIShelfRequestByRoom(room.RoomID, room.CompanyID, false, false,null);

                        int NoOfPages = 0;
                        int TotalRecords = 0;
                        int RecordsPerPage = 0;

                        if (requests.Count > 0)
                        {
                            NoOfPages = requests[0].NoOfPages;
                            TotalRecords = requests[0].TotalRequest;
                            RecordsPerPage = requests[0].RecordsPerPage;

                            for (int i = 1; i <= NoOfPages; i++)
                            {
                                foreach (eVMIShelfReqDTO request in requests)
                                {
                                    ShelfRequestProcessForRoom(request, enterprise.EnterpriseDBName, enterprise.EnterpriseID, systemUser.ID);
                                }

                                pageNo++;
                                if (pageNo <= NoOfPages)
                                {
                                    requests = shelfRequestDAL.GeteVMIShelfRequestByRoom(room.RoomID, room.CompanyID, false, false,null);
                                }
                            }


                        }

                    } // loop through rooms


                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("ShelfRequestProcess", ex);
                }


            }

        }

        private void ShelfRequestProcessForRoom(eVMIShelfReqDTO shelfRequest, string EnterpriseDBName, long EnterpriseID, long systemUserID)
        {
            ShelfIDEmailDTO shelfEmailDTO = new ShelfIDEmailDTO(EnterpriseDBName, EnterpriseID, shelfRequest.CompanyID, shelfRequest.RoomID
                , shelfRequest.RoomName, shelfRequest.CompanyName);

            eVMIShelfRequestDAL shelfRequestDAL = new eVMIShelfRequestDAL(EnterpriseDBName);
            object lockObject = new object();
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
                eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(shelfRequest.RoomID, shelfRequest.CompanyID, 0);
                string CurrentRoomTimeZone = "UTC";
                if (objRegionalSettings != null)
                {
                    CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
                }
                DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

                shelfRequest.IsShelfStarted = true;
                shelfRequest.ShelfStartTime = CurrentTimeofTimeZone;
                shelfRequest.UpdatedBy = systemUserID;
                // update start status
                shelfRequestDAL.UpdateEVMIShelfRequest(shelfRequest);

                //double dbWeight = 0;
                bool isShelfRequestCompleted = true;
                string errorDescription = "";

                lock (lockObject)
                {
                    using (COMWrapper comWrapper = new COMWrapper(shelfRequest.ComPortName))
                    {
                        try
                        {
                            if (shelfRequest.RequestType == (int)eVMIShelfRequestType.GetShelfID
                                || shelfRequest.RequestType == (int)eVMIShelfRequestType.GetShelfIDImmediate
                                )

                            {
                                // get scale id
                                shelfEmailDTO.ShelfRequestType = eVMIShelfRequestType.GetShelfID;
                                var res = comWrapper.GetScaleID();

                                if (res.IsSuccess)
                                {
                                    shelfRequest.ShelfID = res.data ?? 0;
                                    shelfEmailDTO.ScaleID = (int)shelfRequest.ShelfID;
                                }
                                else
                                {
                                    errorDescription = res.CommandData.ErrorInfo;

                                    shelfEmailDTO.ComError = errorDescription;

                                    isShelfRequestCompleted = false;

                                    WriteComErrorLog("ShelfRequest GetShelf Error", EnterpriseID, shelfRequest.CompanyID,
                                       shelfRequest.RoomID, null, shelfRequest.ComPortName
                                       , errorDescription);
                                }

                            }
                            else if (shelfRequest.RequestType == (int)eVMIShelfRequestType.SetShelfID
                                || shelfRequest.RequestType == (int)eVMIShelfRequestType.SetShelfIDImmediate
                                )
                            {
                                // set scale id
                                int scaleId = (int)shelfRequest.ShelfID;
                                shelfEmailDTO.ShelfRequestType = eVMIShelfRequestType.SetShelfID;
                                shelfEmailDTO.ScaleID = scaleId;

                                var res = comWrapper.SetScaleID(scaleId);


                                if (res.IsSuccess)
                                {

                                }
                                else
                                {
                                    errorDescription = res.CommandData.ErrorInfo;
                                    shelfEmailDTO.ComError = errorDescription;
                                    isShelfRequestCompleted = false;

                                    WriteComErrorLog("ShelfRequest SetShelf Error", EnterpriseID, shelfRequest.CompanyID,
                                       shelfRequest.RoomID, null, shelfRequest.ComPortName
                                       , errorDescription);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            isShelfRequestCompleted = false;
                            errorDescription = Logger.GetExceptionDetails(ex);
                            shelfEmailDTO.ComError = ex.Message;

                            WriteComErrorLog("ShelfRequest Error", EnterpriseID, shelfRequest.CompanyID,
                                  shelfRequest.RoomID, null, shelfRequest.ComPortName
                                  , errorDescription, ex);

                        }
                    }
                }

                shelfRequest.IsShelfCompleted = true;

                if (!isShelfRequestCompleted)
                {
                    shelfRequest.ErrorDescription = errorDescription;
                }
                else
                {
                    shelfRequest.ErrorDescription = null;
                }
                CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
                shelfRequest.ShelfCompletionTime = CurrentTimeofTimeZone;
                shelfRequest.UpdatedBy = systemUserID;
                // update completed status
                shelfRequestDAL.UpdateEVMIShelfRequest(shelfRequest);
            }
            catch (Exception ex)
            {
                string log = string.Format(Environment.NewLine
                                + "ShelfRequest Error - EnterpriseID : {0} , CompanyID : {1} , Room : {2}  "
                                , EnterpriseID, shelfRequest.CompanyID, shelfRequest.RoomID);
                Logger.WriteExceptionLog(log, ex);
                shelfEmailDTO.ExceptionError = ex.Message;
            }

            EmailWrapper.SendGetOrSetShelfIDDoneEmail(shelfEmailDTO);
        }

        public int ShelfRequestProcessForRoom(long requestID, string enterpriseDBName, long enterpriseID, long companyId, long roomId, long userID)
        {
            int shelfId = 0;
            try
            {
                eVMIShelfRequestDAL shelfRequestDAL = new eVMIShelfRequestDAL(enterpriseDBName);
                eVMIShelfReqDTO request = shelfRequestDAL.GeteVMIShelfRequestByRoom(roomId, companyId, false, false, requestID).FirstOrDefault();

                if (request != null)
                {
                    ShelfRequestProcessForRoom(request, enterpriseDBName, enterpriseID, userID);
                    shelfId = request.ShelfID;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteExceptionLog("ShelfRequest", ex);
            }

            return shelfId;
        }

    }// class
}
