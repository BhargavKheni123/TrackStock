using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class ArchiveDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public ArchiveDAL(base.DataBaseName)
        //{

        //}

        public ArchiveDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ArchiveDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        public void ArchiveDataAuto(string RoomIDs, string ModuleName, int NoOfYearsKeepData)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomIDs", RoomIDs),
                                                new SqlParameter("@YearBack", NoOfYearsKeepData*-1),
                                                new SqlParameter("@DateFrom", DateTimeUtility.DateTimeNow.ToString("yyyy-MM-dd")),
                                                new SqlParameter("@ModuleName", ModuleName),

                };
                context.Database.SqlQuery<object>("EXEC [dbo].[ArchiveRecordByModule] @RoomIDs,@YearBack,@DateFrom,@ModuleName", params1);
            }
        }
        public void ArchiveDataManually(string DataGuids, Int64 RoomID, string ModuleName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomIDs", RoomID.ToString()),
                                                new SqlParameter("@YearBack", 1),
                                                new SqlParameter("@DateFrom", DateTimeUtility.DateTimeNow.ToString("yyyy-MM-dd")),
                                                new SqlParameter("@ModuleName", ModuleName),
                                                new SqlParameter("@DataGuids", DataGuids)
                };

                context.Database.SqlQuery<object>("EXEC [dbo].[ArchiveRecordByModule] @RoomIDs,@YearBack,@DateFrom,@ModuleName,@DataGuids", params1);
            }
        }
        public void UnArchiveDataManually(string DataGuids, Int64 RoomID, string ModuleName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomIDs", RoomID.ToString()),
                                                new SqlParameter("@YearBack", 1),
                                                new SqlParameter("@DateFrom", DateTimeUtility.DateTimeNow.ToString("yyyy-MM-dd")),
                                                new SqlParameter("@ModuleName", ModuleName),
                                                new SqlParameter("@DataGuids", DataGuids)
                };

                context.Database.SqlQuery<object>("EXEC [dbo].[ArchiveToLiveRecordByModule] @RoomIDs,@YearBack,@DateFrom,@ModuleName,@DataGuids", params1);
            }
        }

        public string ArchiveRecords(string Ids, long RoomID, string ModuleName, bool IsGuid, string ArchivedFrom, string ArchiveDescription, out string NotificationClass,long EnterpriceId,long companyID,long userID)
        {
            NotificationClass = "errorIcon";
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring); //@Ids,@IsGUID,@RoomIDs,@ModuleName,@ArchivedFrom,@ArchiveDescription
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "ArchiveRecords", Ids, IsGuid, Convert.ToString(RoomID), ModuleName, ArchivedFrom, ArchiveDescription);
            DataTable dt = new DataTable();
            string msg = string.Empty;
            string reasonToFail = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;

            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, companyID, userID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            
            string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriceId, companyID);
            string ResourceFileReqMaster = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResRequisitionMaster", currentCulture, EnterpriceId, companyID);
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                        if (ModuleName.ToLower() == "requisition")
                        {
                            string MsgRecordArchievedFailure = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionArchievedValidation", ResourceFileReqMaster, EnterpriceId, companyID, RoomID, "ResRequisitionMaster", currentCulture);
                            reasonToFail += "<br> " + string.Format(MsgRecordArchievedFailure, Convert.ToString(dr["ErrorItem"]));
                        }
                        else
                        {
                            reasonToFail += "<br> " + dr["Message"].ToString();
                        }
                        
                    }
                }

                if (Successcnt > 0)
                {
                    string MsgRecordArchievedSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordArchievedSuccess", ResourceFileCommon,EnterpriceId,companyID,RoomID, "ResCommon",currentCulture);
                    msg = Successcnt + " "+ MsgRecordArchievedSuccess;
                    NotificationClass = "succesIcon";
                }

                if (Failcnt > 0)
                {
                    string MsgFailtoArchieve = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailtoArchieve", ResourceFileCommon,EnterpriceId, companyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        //msg = Failcnt + " record(s) used in another module. Reason To Fail: " + reasonToFail;
                        msg = Failcnt + " "+ MsgFailtoArchieve + " " + reasonToFail;
                        NotificationClass = "errorIcon";
                    }
                    else
                    {
                        //msg = msg + " " + Failcnt + " record(s) used in another module. Reason To Fail: "+ reasonToFail;
                        msg = msg + " " + Failcnt + " " + MsgFailtoArchieve + " "  + reasonToFail;
                        NotificationClass = "WarningIcon";
                    }
                }
            }

            return msg;
        }

        public string UnarchiveRecords(string Ids, long RoomID, string ModuleName, bool IsGuid, out string NotificationClass, long EnterpriceId, long companyID, long userID)
        {
            NotificationClass = "errorIcon";
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "UnArchiveRecords", Ids, IsGuid, Convert.ToString(RoomID), ModuleName);
            DataTable dt = new DataTable();
            string msg = string.Empty;
            string reasonToFail = string.Empty;
            int Failcnt = 0;
            int Successcnt = 0;
            string currentCulture = "en-US";
            if (HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }
                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, companyID, userID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, EnterpriceId, companyID);
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Status"].ToString() == "Success")
                    {
                        Successcnt += 1;
                    }
                    else if (dr["Status"].ToString() == "Fail")
                    {
                        Failcnt += 1;
                        reasonToFail += "<br> " + dr["Message"].ToString();
                    }
                }

                if (Successcnt > 0)
                {
                    string MsgRecordUnArchievedSuccess = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgRecordUnArchievedSuccess", ResourceFileCommon,EnterpriceId,companyID,RoomID, "ResCommon",currentCulture);
                    msg = Successcnt + " "+ MsgRecordUnArchievedSuccess;
                    NotificationClass = "succesIcon";
                }

                if (Failcnt > 0)
                {
                    string MsgFailtoUnArchieve = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFailtoUnArchieve", ResourceFileCommon, EnterpriceId, companyID, RoomID, "ResCommon", currentCulture);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = Failcnt + " "+ MsgFailtoUnArchieve + " " + reasonToFail;
                        NotificationClass = "errorIcon";
                    }
                    else
                    {
                        msg = msg + " " + Failcnt + " "+ MsgFailtoUnArchieve + " " + reasonToFail;
                        NotificationClass = "WarningIcon";
                    }
                }
            }

            return msg;
        }

        public ArchiveScheduleDTO SaveArchiveSchedule(ArchiveScheduleDTO ArchiveSchedule)
        {
            if (ArchiveSchedule == null)
                return null;

            short[] smodes = new short[] { 1, 2, 3, 4 };
            bool ReclacSchedule = false;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy);
            if (!string.IsNullOrEmpty(ArchiveSchedule.ScheduleRunTime))
            {
                if (ArchiveSchedule.ScheduleMode > 0 && ArchiveSchedule.ScheduleMode < 4)
                {
                    string strtmp = datetimetoConsider.ToShortDateString() + " " + ArchiveSchedule.ScheduleRunTime;
                    ArchiveSchedule.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                    ArchiveSchedule.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(ArchiveSchedule.ScheduleRunDateTime, ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy) ?? ArchiveSchedule.ScheduleRunDateTime;
                    //ArchiveSchedule.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(ArchiveSchedule.ScheduleRunDateTime, ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy) ?? ArchiveSchedule.ScheduleRunDateTime;
                }
            }

            if (ArchiveSchedule.ScheduleMode < 1 || ArchiveSchedule.ScheduleMode > 4)
            {
                string strtmp = datetimetoConsider.ToShortDateString() + " " + ArchiveSchedule.ScheduleRunTime;
                ArchiveSchedule.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                ArchiveSchedule.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(ArchiveSchedule.ScheduleRunDateTime, ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy) ?? ArchiveSchedule.ScheduleRunDateTime;
            }

            if (ArchiveSchedule.ScheduleMode == 4)
            {
                ArchiveSchedule.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, ArchiveSchedule.HourlyAtWhatMinute, 0);
                ArchiveSchedule.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(ArchiveSchedule.ScheduleRunDateTime, ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy) ?? ArchiveSchedule.ScheduleRunDateTime;
            }

            ArchiveSchedule objArchiveSchedule = new ArchiveSchedule();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (ArchiveSchedule.ScheduleID < 1)
                {
                    objArchiveSchedule = new ArchiveSchedule();
                    objArchiveSchedule.CompanyId = ArchiveSchedule.CompanyId;
                    objArchiveSchedule.Created = DateTimeUtility.DateTimeNow;
                    objArchiveSchedule.CreatedBy = ArchiveSchedule.CreatedBy;
                    objArchiveSchedule.DailyRecurringDays = ArchiveSchedule.DailyRecurringDays;
                    objArchiveSchedule.DailyRecurringType = ArchiveSchedule.DailyRecurringType;
                    objArchiveSchedule.ScheduleID = 0;
                    objArchiveSchedule.LastUpdatedBy = ArchiveSchedule.CreatedBy;
                    objArchiveSchedule.MonthlyDateOfMonth = ArchiveSchedule.MonthlyDateOfMonth;
                    objArchiveSchedule.MonthlyDayOfMonth = ArchiveSchedule.MonthlyDayOfMonth;
                    objArchiveSchedule.MonthlyRecurringMonths = ArchiveSchedule.MonthlyRecurringMonths;
                    objArchiveSchedule.MonthlyRecurringType = ArchiveSchedule.MonthlyRecurringType;
                    objArchiveSchedule.RoomId = ArchiveSchedule.RoomId;
                    objArchiveSchedule.ScheduleFor = ArchiveSchedule.LoadSheduleFor;
                    objArchiveSchedule.ScheduleMode = ArchiveSchedule.ScheduleMode;
                    if (ArchiveSchedule.ScheduleRunDateTime == DateTime.MinValue)
                    {
                        ArchiveSchedule.ScheduleRunDateTime = DateTime.UtcNow.Date;
                    }
                    objArchiveSchedule.ScheduleRunTime = ArchiveSchedule.ScheduleRunDateTime;
                    //                    new RegionSettingDAL(base.DataBaseName).ConvertLocalDateTimeToUTCDateTime(ArchiveSchedule.ScheduleRunDateTime, ArchiveSchedule.RoomId, ArchiveSchedule.CompanyId, ArchiveSchedule.CreatedBy) ?? ArchiveSchedule.ScheduleRunDateTime;
                    objArchiveSchedule.SubmissionMethod = ArchiveSchedule.SubmissionMethod;
                    objArchiveSchedule.SupplierId = ArchiveSchedule.SupplierId;
                    objArchiveSchedule.WeeklyOnFriday = ArchiveSchedule.WeeklyOnFriday;
                    objArchiveSchedule.WeeklyOnMonday = ArchiveSchedule.WeeklyOnMonday;
                    objArchiveSchedule.WeeklyOnSaturday = ArchiveSchedule.WeeklyOnSaturday;
                    objArchiveSchedule.WeeklyOnSunday = ArchiveSchedule.WeeklyOnSunday;
                    objArchiveSchedule.WeeklyOnThursday = ArchiveSchedule.WeeklyOnThursday;
                    objArchiveSchedule.WeeklyOnTuesday = ArchiveSchedule.WeeklyOnTuesday;
                    objArchiveSchedule.WeeklyOnWednesday = ArchiveSchedule.WeeklyOnWednesday;
                    objArchiveSchedule.WeeklyRecurringWeeks = ArchiveSchedule.WeeklyRecurringWeeks;
                    objArchiveSchedule.Updated = DateTimeUtility.DateTimeNow;
                    objArchiveSchedule.IsScheduleActive = ArchiveSchedule.IsScheduleActive;
                    objArchiveSchedule.AssetToolID = ArchiveSchedule.AssetToolID;
                    objArchiveSchedule.ReportID = ArchiveSchedule.ReportID;
                    objArchiveSchedule.ReportDataSelectionType = ArchiveSchedule.ReportDataSelectionType;
                    objArchiveSchedule.ReportDataSince = ArchiveSchedule.ReportDataSince;
                    objArchiveSchedule.HourlyAtWhatMinute = ArchiveSchedule.HourlyAtWhatMinute;
                    objArchiveSchedule.HourlyRecurringHours = ArchiveSchedule.HourlyRecurringHours;
                    objArchiveSchedule.ScheduledBy = ArchiveSchedule.ScheduledBy;
                    objArchiveSchedule.ScheduleName = ArchiveSchedule.ScheduleName;
                    objArchiveSchedule.Duration = ArchiveSchedule.Duration;
                    objArchiveSchedule.DurationType = ArchiveSchedule.DurationType;
                    objArchiveSchedule.ModuleId = ArchiveSchedule.ModuleId;

                    if (!string.IsNullOrWhiteSpace(ArchiveSchedule.ScheduleRunTime))
                    {
                        objArchiveSchedule.ScheduleTime = Convert.ToDateTime(ArchiveSchedule.ScheduleRunTime).TimeOfDay;
                    }
                    if (ArchiveSchedule.ScheduleRunDateTime == DateTime.MinValue)
                    {
                        ArchiveSchedule.ScheduleRunDateTime = DateTime.UtcNow.Date;
                    }
                    objArchiveSchedule.ScheduleRunTime = ArchiveSchedule.ScheduleRunDateTime;
                    //objArchiveSchedule.UserID = ArchiveSchedule.UserID;
                    context.ArchiveSchedules.Add(objArchiveSchedule);
                    context.SaveChanges();
                    ArchiveSchedule.ScheduleID = objArchiveSchedule.ScheduleID;

                    if (ArchiveSchedule.ScheduleMode != 0)
                    {
                        context.Database.ExecuteSqlCommand("EXEC SetArchiveScheduleNextRunDate " + ArchiveSchedule.ScheduleID + "");
                    }
                }
                else
                {
                    objArchiveSchedule = context.ArchiveSchedules.FirstOrDefault(t => t.ScheduleID == ArchiveSchedule.ScheduleID);

                    if (objArchiveSchedule != null)
                    {
                        if (objArchiveSchedule.ScheduleMode != ArchiveSchedule.ScheduleMode)
                        {
                            ReclacSchedule = true;
                        }
                        else if ((objArchiveSchedule.ScheduleRunTime.Hour != ArchiveSchedule.ScheduleRunDateTime.Hour) || (objArchiveSchedule.ScheduleRunTime.Minute != ArchiveSchedule.ScheduleRunDateTime.Minute))
                        {
                            ReclacSchedule = true;
                        }
                        else
                        {
                            switch (ArchiveSchedule.ScheduleMode)
                            {
                                case 1:
                                    if ((objArchiveSchedule.DailyRecurringType != ArchiveSchedule.DailyRecurringType))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objArchiveSchedule.DailyRecurringType == 1 && (objArchiveSchedule.DailyRecurringDays != ArchiveSchedule.DailyRecurringDays))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 2:
                                    if ((objArchiveSchedule.WeeklyRecurringWeeks != ArchiveSchedule.WeeklyRecurringWeeks) || (objArchiveSchedule.WeeklyOnSunday != ArchiveSchedule.WeeklyOnSunday) || (objArchiveSchedule.WeeklyOnMonday != ArchiveSchedule.WeeklyOnMonday) || (objArchiveSchedule.WeeklyOnTuesday != ArchiveSchedule.WeeklyOnTuesday) || (objArchiveSchedule.WeeklyOnWednesday != ArchiveSchedule.WeeklyOnWednesday) || (objArchiveSchedule.WeeklyOnThursday != ArchiveSchedule.WeeklyOnThursday) || (objArchiveSchedule.WeeklyOnFriday != ArchiveSchedule.WeeklyOnFriday) || (objArchiveSchedule.WeeklyOnSaturday != ArchiveSchedule.WeeklyOnSaturday))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 3:
                                    if (objArchiveSchedule.MonthlyRecurringType != ArchiveSchedule.MonthlyRecurringType)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objArchiveSchedule.MonthlyRecurringMonths != ArchiveSchedule.MonthlyRecurringMonths)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objArchiveSchedule.MonthlyRecurringType == 1 && (objArchiveSchedule.MonthlyDateOfMonth != ArchiveSchedule.MonthlyDateOfMonth))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objArchiveSchedule.MonthlyRecurringType == 2 && ((objArchiveSchedule.MonthlyDateOfMonth != ArchiveSchedule.MonthlyDateOfMonth) || (objArchiveSchedule.MonthlyDayOfMonth != ArchiveSchedule.MonthlyDayOfMonth)))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 4:
                                    if ((objArchiveSchedule.HourlyRecurringHours != ArchiveSchedule.HourlyRecurringHours) || (objArchiveSchedule.HourlyAtWhatMinute != ArchiveSchedule.HourlyAtWhatMinute))
                                    {
                                        ReclacSchedule = true;
                                    }

                                    break;
                            }
                        }
                        //if (ReclacSchedule)
                        //{
                        //    objArchiveSchedule.NextRunDate = null;
                        //}
                        var UpdateSchedule = false;

                        if (ReclacSchedule || (!objArchiveSchedule.IsScheduleActive && (objArchiveSchedule.IsScheduleActive != ArchiveSchedule.IsScheduleActive)))
                        {
                            UpdateSchedule = true;
                        }

                        objArchiveSchedule.DailyRecurringDays = ArchiveSchedule.DailyRecurringDays;
                        objArchiveSchedule.DailyRecurringType = ArchiveSchedule.DailyRecurringType;
                        objArchiveSchedule.LastUpdatedBy = ArchiveSchedule.LastUpdatedBy;
                        objArchiveSchedule.MonthlyDateOfMonth = ArchiveSchedule.MonthlyDateOfMonth;
                        objArchiveSchedule.MonthlyDayOfMonth = ArchiveSchedule.MonthlyDayOfMonth;
                        objArchiveSchedule.MonthlyRecurringMonths = ArchiveSchedule.MonthlyRecurringMonths;
                        objArchiveSchedule.MonthlyRecurringType = ArchiveSchedule.MonthlyRecurringType;
                        objArchiveSchedule.ScheduleMode = ArchiveSchedule.ScheduleMode;
                        objArchiveSchedule.SubmissionMethod = ArchiveSchedule.SubmissionMethod;
                        objArchiveSchedule.WeeklyOnFriday = ArchiveSchedule.WeeklyOnFriday;
                        objArchiveSchedule.WeeklyOnMonday = ArchiveSchedule.WeeklyOnMonday;
                        objArchiveSchedule.WeeklyOnSaturday = ArchiveSchedule.WeeklyOnSaturday;
                        objArchiveSchedule.WeeklyOnSunday = ArchiveSchedule.WeeklyOnSunday;
                        objArchiveSchedule.WeeklyOnThursday = ArchiveSchedule.WeeklyOnThursday;
                        objArchiveSchedule.WeeklyOnTuesday = ArchiveSchedule.WeeklyOnTuesday;
                        objArchiveSchedule.WeeklyOnWednesday = ArchiveSchedule.WeeklyOnWednesday;
                        objArchiveSchedule.WeeklyRecurringWeeks = ArchiveSchedule.WeeklyRecurringWeeks;
                        objArchiveSchedule.IsScheduleActive = ArchiveSchedule.IsScheduleActive;
                        //objArchiveSchedule.ScheduleRunTime = ArchiveSchedule.ScheduleRunDateTime;
                        objArchiveSchedule.AssetToolID = ArchiveSchedule.AssetToolID;
                        objArchiveSchedule.ReportID = ArchiveSchedule.ReportID;
                        objArchiveSchedule.ReportDataSelectionType = ArchiveSchedule.ReportDataSelectionType;
                        objArchiveSchedule.ReportDataSince = ArchiveSchedule.ReportDataSince;
                        objArchiveSchedule.HourlyAtWhatMinute = ArchiveSchedule.HourlyAtWhatMinute;
                        objArchiveSchedule.HourlyRecurringHours = ArchiveSchedule.HourlyRecurringHours;
                        //objRoomSchedule.NextRunDate = GetScheduleNextRunDate(ArchiveSchedule);
                        objArchiveSchedule.Updated = DateTimeUtility.DateTimeNow;
                        objArchiveSchedule.ScheduleName = ArchiveSchedule.ScheduleName;
                        objArchiveSchedule.Duration = ArchiveSchedule.Duration;
                        objArchiveSchedule.DurationType = ArchiveSchedule.DurationType;
                        objArchiveSchedule.ModuleId = ArchiveSchedule.ModuleId;
                        //objRoomSchedule.ScheduledBy = ArchiveSchedule.ScheduledBy;

                        if (!string.IsNullOrWhiteSpace(ArchiveSchedule.ScheduleRunTime))
                        {
                            objArchiveSchedule.ScheduleTime = Convert.ToDateTime(ArchiveSchedule.ScheduleRunTime).TimeOfDay;
                        }
                        if (UpdateSchedule && smodes.Contains(ArchiveSchedule.ScheduleMode))
                        {
                            objArchiveSchedule.ScheduleRunTime = ArchiveSchedule.ScheduleRunDateTime;
                            objArchiveSchedule.NextRunDate = null;
                        }

                        context.SaveChanges();

                        if (UpdateSchedule)
                        {
                            if (objArchiveSchedule.ScheduleMode != 0)
                            {
                                context.Database.ExecuteSqlCommand("EXEC SetArchiveScheduleNextRunDate " + objArchiveSchedule.ScheduleID + "");
                            }
                        }
                    }
                }
            }

            ArchiveSchedule.RecalcSchedule = ReclacSchedule;
            return ArchiveSchedule;
        }

        public IEnumerable<ArchiveScheduleDTO> GetPagedArchiveSchedules(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyId, bool IsDeleted)
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                        SearchTerm = Fields[2];
                    else
                        SearchTerm = string.Empty;
                }
                else
                {
                    SearchTerm = string.Empty;
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    //new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@RoomId", RoomID),
                    new SqlParameter("@CompanyID", CompanyId)

                };

                List<ArchiveScheduleDTO> lstLocations = context.Database.SqlQuery<ArchiveScheduleDTO>("exec [GetPagedArchiveSchedules] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@IsDeleted,@RoomId,@CompanyID", sqlParams).ToList();
                TotalCount = 0;
                if (lstLocations != null && lstLocations.Count > 0)
                {
                    TotalCount = lstLocations.First().TotalRecords;
                }

                return lstLocations;
            }

        }

        public bool IsArchiveScheduleExist(long RoomId, int ModuleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var exist = false;
                var archiveSchedule = context.ArchiveSchedules.Where(asl => (asl.RoomId ?? 0) == RoomId && asl.ModuleId == ModuleId && asl.IsDeleted == false && asl.ScheduleFor == 10).FirstOrDefault();
                if (archiveSchedule != null && archiveSchedule.ScheduleID > 0)
                {
                    exist = true;
                }
                return exist;
            }
        }
        public bool IsArchiveScheduleDuplicateForEdit(long RoomId, int ModuleId, long ScheduleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var exist = false;
                var archiveSchedule = context.ArchiveSchedules.Where(asl => (asl.RoomId ?? 0) == RoomId && asl.ModuleId == ModuleId && asl.IsDeleted == false && asl.ScheduleFor == 10 && asl.ScheduleID != ScheduleId).FirstOrDefault();
                if (archiveSchedule != null && archiveSchedule.ScheduleID > 0)
                {
                    exist = true;
                }
                return exist;
            }
        }

        public ArchiveScheduleDTO GetArchiveScheduleByIdPlain(long ScheduleId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ScheduleID", ScheduleId) };
                var archiveSchedule = context.Database.SqlQuery<ArchiveScheduleDTO>("exec [GetArchiveScheduleByIdPlain] @ScheduleID", params1).FirstOrDefault();

                if (archiveSchedule != null && archiveSchedule.ScheduleID > 0)
                {
                    archiveSchedule.ScheduleRunTime = archiveSchedule.ScheduleRunDateTime.ToString("HH:mm");
                }
                return archiveSchedule;
            }
        }

        public List<ArchiveScheduleDTO> GetAllArchiveSchedules(DateTime ToDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseETurnsMasterEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToDate", ToDate) };
                return context.Database.SqlQuery<ArchiveScheduleDTO>("exec [GetArchiveScheduletoRunNow] @ToDate", params1).ToList();
            }
        }

        public void ArchiveRecordsByDuration(string Module, DateTime ToDate, long RoomId, string ArchivedFrom, string ArchiveDescription)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                                                {
                                                    new SqlParameter("@ModuleName", Module),
                                                    new SqlParameter("@RoomId", RoomId),
                                                    new SqlParameter("@ToDate", ToDate),
                                                    new SqlParameter("@ArchivedFrom", ArchivedFrom),
                                                    new SqlParameter("@ArchiveDescription", ArchiveDescription)
                                                };
                context.Database.ExecuteSqlCommand("EXEC [ArchiveRecordsByDuration] @ModuleName,@RoomId,@ToDate,@ArchivedFrom,@ArchiveDescription", params1);
            }
        }

        public void UpdateNextRunDateOfArchiveSchedule(long ScheduleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "EXEC SetArchiveScheduleNextRunDate " + ScheduleID;
                context.Database.ExecuteSqlCommand(qry);
            }
        }

        public void CleanArchiveScheduleRunHistory(string ScheduleDBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[CleanArchiveScheduleRunHistory]");
            }
        }

        public void InsertArchiveScheduleHistory(ArchiveScheduleRunHistoryDTO ArchiveScheduleRunHistory, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@RecGUID", ArchiveScheduleRunHistory.RecGUID ),
                new SqlParameter("@EnterpriseID", ArchiveScheduleRunHistory.EnterpriseID?? (object)DBNull.Value ),
                new SqlParameter("@CompanyID", ArchiveScheduleRunHistory.CompanyID?? (object)DBNull.Value ),
                new SqlParameter("@RoomID", ArchiveScheduleRunHistory.RoomID?? (object)DBNull.Value ),
                new SqlParameter("@ScheduleID", ArchiveScheduleRunHistory.ScheduleID?? (object)DBNull.Value ),
                new SqlParameter("@MasterScheduleID", ArchiveScheduleRunHistory.MasterScheduleID?? (object)DBNull.Value ),
                new SqlParameter("@NextRunDate", ArchiveScheduleRunHistory.NextRunDate?? (object)DBNull.Value ),
                new SqlParameter("@Created", ArchiveScheduleRunHistory.Created ),
                new SqlParameter("@LastUpdated", ArchiveScheduleRunHistory.LastUpdated ),
                new SqlParameter("@ScheduleFor", ArchiveScheduleRunHistory.ScheduleFor?? (object)DBNull.Value ),
                new SqlParameter("@IsArchiveScheduleStarted", ArchiveScheduleRunHistory.IsArchiveScheduleStarted ),
                new SqlParameter("@IsArchiveScheduleCompleted", ArchiveScheduleRunHistory.IsArchiveScheduleCompleted ),
                new SqlParameter("@ArchiveScheduleStartedTime", ArchiveScheduleRunHistory.ArchiveScheduleStartedTime?? (object)DBNull.Value ),
                new SqlParameter("@ArchiveScheduleCompletedTime", ArchiveScheduleRunHistory.ArchiveScheduleCompletedTime?? (object)DBNull.Value ),
                new SqlParameter("@ArchiveScheduleExceptionTime", ArchiveScheduleRunHistory.ArchiveScheduleExceptionTime?? (object)DBNull.Value ),
                new SqlParameter("@ArchiveScheduleException", ArchiveScheduleRunHistory.ArchiveScheduleExceptionTime?? (object)DBNull.Value ),
                new SqlParameter("@ArchiveScheduleAttempt", ArchiveScheduleRunHistory.ArchiveScheduleAttempt?? (object)DBNull.Value ),
                new SqlParameter("@ArchiveScheduleAttemptTime", ArchiveScheduleRunHistory.ArchiveScheduleAttemptTime?? (object)DBNull.Value ),
                new SqlParameter("@ExternalFilter", ArchiveScheduleRunHistory.DataGuids?? (object)DBNull.Value )
            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[InsertArchiveScheduleHistory] @RecGUID,@EnterpriseID,@CompanyID,@RoomID,@ScheduleID,@MasterScheduleID,@NextRunDate,@Created,@LastUpdated,@ScheduleFor,@IsArchiveScheduleStarted,@IsArchiveScheduleCompleted,@ArchiveScheduleStartedTime,@ArchiveScheduleCompletedTime,@ArchiveScheduleExceptionTime,@ArchiveScheduleException,@ArchiveScheduleAttempt,@ArchiveScheduleAttemptTime,@ExternalFilter", params1);
            }
        }

        public List<ArchiveScheduleRunHistoryDTO> GetArchiveScheduleRecords(int OperationType, int NumberOfInstance, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ArchiveScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetArchiveScheduleRecords] @OperationType,@NumberOfInstance", params1).ToList();
            }
        }

        public void SetCompletedArchiveScheduleRunHistory(int OperationType, long ID, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[SetCompletedArchiveScheduleRunHistory] @OperationType,@ID", params1);
            }
        }

        public void UpdateErrorForArchiveSchedule(int OperationType, long ID, string ExceptionDetails, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID), new SqlParameter("@ExceptionDetails", (ExceptionDetails ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[UpdateErrorForArchiveSchedule] @OperationType,@ID,@ExceptionDetails", params1);
            }
        }
    }
}
