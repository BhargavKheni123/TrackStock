using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using eTurnsMaster.DAL;
using eTurns.DTO;
using eTurns.DAL;
using System.Threading.Tasks;
using eTurns.eVMIBAL;
using eTurns.eVMIBAL.Wrappers;
using eTurns.eVMIBAL.DTO;
using System.Configuration;

namespace eTurns.eVMIBAL
{
    public class SchedulePollRequest : PollRequestBase
    {
        public SchedulePollRequest()
        {

        }

        /// <summary>
        /// Add poll request as per evmi setup
        /// </summary>
        public void AddPollRequestProcess()
        {
            List<EVMIRoomDTO> lstEntDTO = SharedData.eVMIRooms;

            // loop through all enterprises , get evmi setup data
            foreach (EVMIRoomDTO enterprise in lstEntDTO)
            //Parallel.ForEach<EVMIRoomDTO>(lstEntDTO, objDTO =>
            {
                try
                {
                    eVMISetupDAL objeVMIDAL = new eVMISetupDAL(enterprise.EnterpriseDBName);

                    // get all evmi setup records in enterprise
                    List<eVMISetupDTO> lsteVMISetupDTO = objeVMIDAL.GetAlleVMIRecordForeVMIRoom();

                    foreach (eVMISetupDTO objeVMISetupDTO in lsteVMISetupDTO)
                    //Parallel.ForEach<eVMISetupDTO>(lsteVMISetupDTO, objeVMISetupDTO =>
                    {
                        try
                        {
                            bool _IsOldeVMIRoom = EVMICommon.IsOldeVMIRoom(enterprise.EnterpriseID, objeVMISetupDTO.CompanyID ?? 0, objeVMISetupDTO.Room ?? 0);
                            if (_IsOldeVMIRoom == false)
                            {
                                if (objeVMISetupDTO.PollType == (int)SchedulePollRequestType.TimePoll)
                                {
                                    ProcessTimePollRequest(objeVMISetupDTO, objeVMIDAL, enterprise.EnterpriseDBName);
                                }
                                else if (objeVMISetupDTO.PollType == (int)SchedulePollRequestType.SchedulePollTime)
                                {
                                    ProcessSchedulePollRequest(objeVMISetupDTO, enterprise.EnterpriseDBName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteExceptionLog("PollRequestProcess", ex);
                        }

                    }
                    //); // parallel loop
                }
                catch (Exception ex)
                {
                    Logger.WriteExceptionLog("PollRequestProcess", ex);
                }

            }
            //); // parallel loop


        }

        public void ProcesseVMISchedules()
        {
            try
            {
                long SessionUserId = 0;
                string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                int BatchSize_ForeVMITimer = 1000;
                int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["BatchSize_ForeVMITimer"]), out BatchSize_ForeVMITimer);
                if (BatchSize_ForeVMITimer < 1)
                {
                    BatchSize_ForeVMITimer = 1000;
                }
                NotificationDAL objNotificationDAL = new NotificationDAL("eTurns");
                List<eVMIScheduleRunHistoryDTO> lsteVMIScheduleRunHistoryDTO = objNotificationDAL.GetRecordToRuneVMISchedule(11, BatchSize_ForeVMITimer, eTurnsScheduleDBName);
                List<EnterpriseDTO> lstEnterprise = null;
                if (lsteVMIScheduleRunHistoryDTO != null && lsteVMIScheduleRunHistoryDTO.Count > 0)
                {
                    List<eVMIScheduleRunHistoryDTO> lstDailyScheduleRunHistoryEnterprise;
                    lstEnterprise = GetEnterpriseByIds(string.Join(",", lsteVMIScheduleRunHistoryDTO.Select(t => t.EnterpriseID).Distinct().ToArray()));

                    if (lstEnterprise != null && lstEnterprise.Count() > 0)
                    {
                        foreach (EnterpriseDTO objEnterpriseDTO in lstEnterprise)
                        {
                            if (SharedData.eVMIRooms.Any(y => y.EnterpriseID == objEnterpriseDTO.ID))
                            {
                                try
                                {
                                    lstDailyScheduleRunHistoryEnterprise = new List<eVMIScheduleRunHistoryDTO>();
                                    long UserID = 0;
                                    UserMasterDTO objeTurnsUser = GetSystemUser(objEnterpriseDTO.EnterpriseDBName);
                                    if (objeTurnsUser != null)
                                    {
                                        UserID = objeTurnsUser.ID;
                                        SessionUserId = UserID;
                                    }
                                    eVMISetupDAL objeVMISetupDAL = new eVMISetupDAL(objEnterpriseDTO.EnterpriseDBName);
                                    lstDailyScheduleRunHistoryEnterprise = lsteVMIScheduleRunHistoryDTO.Where(t => t.EnterpriseID == objEnterpriseDTO.ID).ToList();
                                    foreach (eVMIScheduleRunHistoryDTO objglobal in lstDailyScheduleRunHistoryEnterprise)
                                    {
                                        eVMISetupDTO objeVMISetupDTO = objeVMISetupDAL.GetRecord(objglobal.RoomID ?? 0, objglobal.CompanyID ?? 0);
                                        if (objeVMISetupDTO != null && objeVMISetupDTO.ID > 0)
                                        {
                                            try
                                            {
                                                InsertItemLocationPollRequests(objEnterpriseDTO.EnterpriseDBName, objeVMISetupDTO);
                                                objNotificationDAL.SetCompletedeVMIScheduleRunHistory(11, objglobal.ID, eTurnsScheduleDBName);
                                            }
                                            catch (Exception ex)
                                            {
                                                objNotificationDAL.UpdateErrorForOrderSchedule(11, objglobal.ID, (Convert.ToString(ex) ?? "ProcessCartForOrder Exception null"), eTurnsScheduleDBName);
                                                CommonMasterDAL objCommonDAL = new CommonMasterDAL();
                                                ReportSchedulerError objReportSchedulerError = new ReportSchedulerError();
                                                objReportSchedulerError.CompanyID = objeVMISetupDTO.CompanyID ?? 0;
                                                objReportSchedulerError.EnterpriseID = objEnterpriseDTO.ID;
                                                objReportSchedulerError.Exception = Convert.ToString(ex);
                                                objReportSchedulerError.ID = 0;
                                                objReportSchedulerError.NotificationID = objeVMISetupDTO.ID;
                                                objReportSchedulerError.RoomID = objeVMISetupDTO.Room ?? 0;
                                                objReportSchedulerError.ScheduleFor = 11;
                                                objCommonDAL.SaveNotificationError(objReportSchedulerError);
                                            }
                                        }
                                    }

                                }
                                catch (Exception exenterloop)
                                {

                                    CommonMasterDAL objCommonDAL = new CommonMasterDAL();
                                    ReportSchedulerError objReportSchedulerError = new ReportSchedulerError();
                                    objReportSchedulerError.CompanyID = 0;
                                    objReportSchedulerError.EnterpriseID = 0;
                                    objReportSchedulerError.Exception = "Global Trycatch:Enterprise loop ProcessCartForOrder: " + (Convert.ToString(exenterloop) ?? string.Empty);
                                    objReportSchedulerError.ID = 0;
                                    objReportSchedulerError.NotificationID = 0;
                                    objReportSchedulerError.RoomID = 0;
                                    objReportSchedulerError.ScheduleFor = 0;
                                    objCommonDAL.SaveNotificationError(objReportSchedulerError);
                                }

                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                CommonMasterDAL objCommonDAL = new CommonMasterDAL();
                ReportSchedulerError objReportSchedulerError = new ReportSchedulerError();
                objReportSchedulerError.CompanyID = 0;
                objReportSchedulerError.EnterpriseID = 0;
                objReportSchedulerError.Exception = "Global Trycatch:ProcessCartForOrder : " + (Convert.ToString(ex) ?? string.Empty);
                objReportSchedulerError.ID = 0;
                objReportSchedulerError.NotificationID = 0;
                objReportSchedulerError.RoomID = 0;
                objReportSchedulerError.ScheduleFor = 0;
                objCommonDAL.SaveNotificationError(objReportSchedulerError);
                throw;
            }
        }

        public List<EnterpriseDTO> GetEnterpriseByIds(string EnterpriseIds)
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            return objEnterpriseMasterDAL.GetEnterprisesByIds(EnterpriseIds);
        }
        private DateTime GetCurrentTimeofTimeZone(string EnterpriseDBName, long roomID, long companyID)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(roomID, companyID, 0);
            string CurrentRoomTimeZone = "UTC";
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }

            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            return CurrentTimeofTimeZone;
        }

        private DateTime GetCurrentTimeofTimeZoneUTC(string EnterpriseDBName, long roomID, long companyID)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(roomID, companyID, 0);
            string CurrentRoomTimeZone = "UTC";
            if (objRegionalSettings != null)
            {
                CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
            }

            DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
            CurrentTimeofTimeZone = CurrentTimeofTimeZone.ToUniversalTime();
            return CurrentTimeofTimeZone;
        }

        /// <summary>
        /// Add poll request for time interval setup
        /// </summary>
        /// <param name="objeVMISetupDTO"></param>
        /// <param name="CurrentTimeofTimeZone"></param>
        /// <param name="objeVMIDAL"></param>
        /// <param name="EnterpriseDBName"></param>
        private void ProcessTimePollRequest(eVMISetupDTO objeVMISetupDTO,
            eVMISetupDAL objeVMIDAL,
            string EnterpriseDBName)
        {

            DateTime CurrentTimeofTimeZone = GetCurrentTimeofTimeZoneUTC(EnterpriseDBName, objeVMISetupDTO.Room.GetValueOrDefault(0), objeVMISetupDTO.CompanyID.GetValueOrDefault(0));
            SchedulerDTO objSchedulerDTO = null;
            NotificationDAL objNotificationDAL = new NotificationDAL(EnterpriseDBName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(EnterpriseDBName);
            objSchedulerDTO = objNotificationDAL.GetScheduleByRoomScheduleFor(Convert.ToInt64(objeVMISetupDTO.Room), Convert.ToInt64(objeVMISetupDTO.CompanyID), (int)eVMIScheduleFor.eVMISchedule);

            if (objSchedulerDTO != null && objSchedulerDTO.IsScheduleActive
                    && (objeVMISetupDTO.NextPollDate == null || CurrentTimeofTimeZone >= objeVMISetupDTO.NextPollDate))
            {
                InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterpriseDBName);
                ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(EnterpriseDBName);
                SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(EnterpriseDBName);
                ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
                UserMasterDTO objUserDTO = GetSystemUser(EnterpriseDBName);

                objILPollRequestDTO.RoomID = Convert.ToInt64(objeVMISetupDTO.Room);
                objILPollRequestDTO.CompanyID = Convert.ToInt64(objeVMISetupDTO.CompanyID);
                objILPollRequestDTO.RequestType = (int)PollRequestType.TimePoll;
                objILPollRequestDTO.IsPollStarted = false;
                objILPollRequestDTO.CreatedBy = objUserDTO.ID;
                InventoryCountDTO objInventoryCountDTO = objInventoryCountDAL.InsertPollCount(objeVMISetupDTO.Room ?? 0, objeVMISetupDTO.CompanyID ?? 0, objUserDTO.ID, "web");
                if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                {
                    objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
                }
                objILPollRequestDAL.InsertItemLocationPollAllRequest(objILPollRequestDTO);

                objSchedulerDTO.NextRunDate = null;
                string strtmp = objSchedulerDTO.ScheduleRunDateTime.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                objSupplierMasterDAL.SaveSchedule(objSchedulerDTO);

                objeVMIDAL.UpdateNextRoomSchedulePollDate(objSchedulerDTO.ScheduleID);

            }

        }



        /// <summary>
        /// Add poll request for scheduled time setup
        /// </summary>
        /// <param name="objeVMISetupDTO"></param>
        /// <param name="CurrentTimeofTimeZone"></param>
        /// <param name="EnterpriseDBName"></param>
        private void ProcessSchedulePollRequest(eVMISetupDTO objeVMISetupDTO,

            string EnterpriseDBName)
        {
            string strPollTime1 = string.Empty;
            string strPollTime2 = string.Empty;
            string strPollTime3 = string.Empty;
            string strPollTime4 = string.Empty;
            string strPollTime5 = string.Empty;
            string strPollTime6 = string.Empty;
            List<TimeSpan> lstPollTime = new List<TimeSpan>();

            if (objeVMISetupDTO.PollTime1 != null)
            {
                strPollTime1 = Convert.ToString(objeVMISetupDTO.PollTime1);
                lstPollTime.Add(Convert.ToDateTime(strPollTime1).TimeOfDay);
            }
            if (objeVMISetupDTO.PollTime2 != null)
            {
                strPollTime2 = Convert.ToString(objeVMISetupDTO.PollTime2);
                lstPollTime.Add(Convert.ToDateTime(strPollTime2).TimeOfDay);
            }
            if (objeVMISetupDTO.PollTime3 != null)
            {
                strPollTime3 = Convert.ToString(objeVMISetupDTO.PollTime3);
                lstPollTime.Add(Convert.ToDateTime(strPollTime3).TimeOfDay);
            }
            if (objeVMISetupDTO.PollTime4 != null)
            {
                strPollTime4 = Convert.ToString(objeVMISetupDTO.PollTime4);
                lstPollTime.Add(Convert.ToDateTime(strPollTime4).TimeOfDay);
            }
            if (objeVMISetupDTO.PollTime5 != null)
            {
                strPollTime5 = Convert.ToString(objeVMISetupDTO.PollTime5);
                lstPollTime.Add(Convert.ToDateTime(strPollTime5).TimeOfDay);
            }
            if (objeVMISetupDTO.PollTime6 != null)
            {
                strPollTime6 = Convert.ToString(objeVMISetupDTO.PollTime6);
                lstPollTime.Add(Convert.ToDateTime(strPollTime6).TimeOfDay);
            }
            UserMasterDTO objUserDTO = GetSystemUser(EnterpriseDBName);

            if (lstPollTime != null && lstPollTime.Count > 0)
            {
                DateTime CurrentTimeofTimeZone = GetCurrentTimeofTimeZoneUTC(EnterpriseDBName, objeVMISetupDTO.Room.GetValueOrDefault(0), objeVMISetupDTO.CompanyID.GetValueOrDefault(0));
                string strCurrentTime = CurrentTimeofTimeZone.ToString("HH:mm:00").Replace(".", ":");
                lstPollTime = lstPollTime.OrderBy(x => x).ToList();

                NotificationDAL objNotificationDAL = new NotificationDAL(EnterpriseDBName);
                SchedulerDTO objSchedulerDTO = null;
                objSchedulerDTO = objNotificationDAL.GetScheduleByRoomScheduleFor(Convert.ToInt64(objeVMISetupDTO.Room), Convert.ToInt64(objeVMISetupDTO.CompanyID), (int)eVMIScheduleFor.eVMISchedule);

                if (objSchedulerDTO != null && objSchedulerDTO.IsScheduleActive && (objeVMISetupDTO.NextPollDate == null || CurrentTimeofTimeZone >= objeVMISetupDTO.NextPollDate))
                {
                    InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterpriseDBName);
                    ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(EnterpriseDBName);
                    eVMISetupDAL objeVMIDAL = new eVMISetupDAL(EnterpriseDBName);
                    ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
                    objILPollRequestDTO.RoomID = Convert.ToInt64(objeVMISetupDTO.Room);
                    objILPollRequestDTO.CompanyID = Convert.ToInt64(objeVMISetupDTO.CompanyID);
                    objILPollRequestDTO.RequestType = (int)PollRequestType.SchedulePoll;
                    objILPollRequestDTO.IsPollStarted = false;
                    objILPollRequestDTO.CreatedBy = objUserDTO.ID;
                    InventoryCountDTO objInventoryCountDTO = objInventoryCountDAL.InsertPollCount(objeVMISetupDTO.Room ?? 0, objeVMISetupDTO.CompanyID ?? 0, objUserDTO.ID, "web");
                    if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                    {
                        objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
                    }
                    objILPollRequestDAL.InsertItemLocationPollAllRequest(objILPollRequestDTO);

                    TimeSpan? SchedulePollTime = null;
                    bool IsNextDaySchedule = false;
                    TimeSpan timeOfDay = CurrentTimeofTimeZone.TimeOfDay;
                    if (objSchedulerDTO.ScheduleTime != null)
                        timeOfDay = (TimeSpan)objSchedulerDTO.ScheduleTime;
                    foreach (TimeSpan item in lstPollTime)
                    {
                        if (item > timeOfDay)
                        {
                            SchedulePollTime = item;
                            break;
                        }
                    }
                    if (SchedulePollTime == null)
                    {
                        SchedulePollTime = lstPollTime.FirstOrDefault();
                        IsNextDaySchedule = true;

                    }
                    if (objSchedulerDTO != null)
                    {
                        SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(EnterpriseDBName);
                        objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunDateTime.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime);
                        objSchedulerDTO.ScheduleRunTime = SchedulePollTime.ToString();
                        objSchedulerDTO.LoadSheduleFor = (int)eVMIScheduleFor.eVMISchedule;
                        objSchedulerDTO.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;
                        if (IsNextDaySchedule)
                            objSchedulerDTO.ScheduleRunDateTime = objSchedulerDTO.ScheduleRunDateTime.AddDays(1);

                        //objSchedulerDTO.ScheduleRunDateTime = objSchedulerDTO.ScheduleRunDateTime.ToUniversalTime();
                        DateTime dtConsider = Convert.ToDateTime(objSchedulerDTO.ScheduleRunDateTime.ToShortDateString() + " " + SchedulePollTime);
                        objSchedulerDTO.NextRunDate = dtConsider.ToUniversalTime();
                        objeVMISetupDTO.NextPollDate = objSchedulerDTO.NextRunDate;
                        objSchedulerDTO = objSupplierMasterDAL.SaveSchedule(objSchedulerDTO);
                        objeVMIDAL.Edit(objeVMISetupDTO);
                    }

                }
            }
        }

        private void InsertItemLocationPollRequests(string EnterpriseDBName, eVMISetupDTO objeVMISetupDTO)
        {
            InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(EnterpriseDBName);
            ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
            UserMasterDTO objUserDTO = GetSystemUser(EnterpriseDBName);
            ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(EnterpriseDBName);

            objILPollRequestDTO.RoomID = Convert.ToInt64(objeVMISetupDTO.Room);
            objILPollRequestDTO.CompanyID = Convert.ToInt64(objeVMISetupDTO.CompanyID);
            objILPollRequestDTO.RequestType = (int)PollRequestType.TimePoll;
            objILPollRequestDTO.IsPollStarted = false;
            objILPollRequestDTO.CreatedBy = objUserDTO.ID;
            InventoryCountDTO objInventoryCountDTO = objInventoryCountDAL.InsertPollCount(objeVMISetupDTO.Room ?? 0, objeVMISetupDTO.CompanyID ?? 0, objUserDTO.ID, "web");
            if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
            {
                objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
            }
            objILPollRequestDAL.InsertItemLocationPollAllRequest(objILPollRequestDTO);
        }

    }// class
}
