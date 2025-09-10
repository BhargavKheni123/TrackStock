using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace eTurns.DAL
{
    public class RegionSettingDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public RegionSettingDAL(base.DataBaseName)
        //{

        //}

        public RegionSettingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public RegionSettingDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public eTurnsRegionInfo GetRegionSettingsById(long RoomId, long CompanyId, long UserId)
        {
            try
            {
                eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objeTurnsRegionInfo = (from rs in context.RegionalSettings
                                           where rs.RoomId == RoomId && rs.CompanyId == CompanyId
                                           select new eTurnsRegionInfo
                                           {
                                               CultureCode = rs.CultureCode,
                                               CultureDisplayName = rs.CultureCode,
                                               CultureName = rs.CultureCode,
                                               CurrencyDecimalDigits = rs.CurrencyDecimalDigits,
                                               CurrencyGroupSeparator = rs.CurrencyGroupSeparator,
                                               LongDatePattern = rs.LongDatePattern,
                                               LongTimePattern = rs.LongTimePattern,
                                               NumberDecimalDigits = rs.NumberDecimalDigits,
                                               NumberDecimalSeparator = rs.NumberDecimalSeparator,
                                               //NumberGroupSeparator = rs.NumberGroupSeparator,
                                               ShortDatePattern = rs.ShortDatePattern,
                                               ShortTimePattern = rs.ShortTimePattern,
                                               ID = rs.ID,
                                               TimeZoneName = rs.TimeZoneName,
                                               CurrencySymbol = rs.CurrencySymbol,
                                               GridRefreshTimeInSecond = rs.GridRefreshTimeInSecond,
                                               WeightDecimalPoints = rs.WeightDecimalPoints,
                                               TurnsAvgDecimalPoints = rs.TurnsAvgDecimalPoints,
                                               NumberOfBackDaysToSyncOverPDA = rs.NumberOfBackDaysToSyncOverPDA,
                                               TZDateTimeNow = DateTime.UtcNow
                                           }).FirstOrDefault();
                }
                if (objeTurnsRegionInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                    {
                        TimeZoneInfo DestTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                        objeTurnsRegionInfo.TZDateTimeNow = System.TimeZoneInfo.ConvertTime(DateTime.Now, System.TimeZoneInfo.Local, DestTimeZone);
                    }
                }
                return objeTurnsRegionInfo;
            }
            catch
            {
                return null;
            }
        }

        public eTurnsRegionInfo SaveRegionalSettings(eTurnsRegionInfo objeTurnsRegionInfo)
        {
            RegionalSetting objRegionalSetting = new RegionalSetting();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objRegionalSetting = context.RegionalSettings.FirstOrDefault(t => t.ID == objeTurnsRegionInfo.ID);
                if (objRegionalSetting == null)
                {
                    objRegionalSetting = new RegionalSetting();
                    objRegionalSetting.CompanyId = objeTurnsRegionInfo.CompanyId;
                    objRegionalSetting.CultureCode = objeTurnsRegionInfo.CultureCode;
                    objRegionalSetting.CurrencyDecimalDigits = objeTurnsRegionInfo.CurrencyDecimalDigits;
                    objRegionalSetting.CurrencyGroupSeparator = objeTurnsRegionInfo.CurrencyGroupSeparator;
                    objRegionalSetting.EnterpriseId = objeTurnsRegionInfo.EnterpriseId;
                    objRegionalSetting.ID = 0;
                    objRegionalSetting.LongDatePattern = objeTurnsRegionInfo.LongDatePattern;
                    objRegionalSetting.LongTimePattern = objeTurnsRegionInfo.LongTimePattern;
                    objRegionalSetting.NumberDecimalDigits = objeTurnsRegionInfo.NumberDecimalDigits;
                    objRegionalSetting.NumberDecimalSeparator = objeTurnsRegionInfo.NumberDecimalSeparator;
                    objRegionalSetting.RoomId = objeTurnsRegionInfo.RoomId;
                    objRegionalSetting.ShortDatePattern = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionalSetting.ShortTimePattern = objeTurnsRegionInfo.ShortTimePattern;
                    objRegionalSetting.UserId = objeTurnsRegionInfo.UserId;
                    objRegionalSetting.TimeZoneName = objeTurnsRegionInfo.TimeZoneName;
                    objRegionalSetting.TimeZoneOffSet = objeTurnsRegionInfo.TimeZoneOffSet;
                    objRegionalSetting.CurrencySymbol = objeTurnsRegionInfo.CurrencySymbol;
                    objRegionalSetting.GridRefreshTimeInSecond = objeTurnsRegionInfo.GridRefreshTimeInSecond;
                    objRegionalSetting.WeightDecimalPoints = objeTurnsRegionInfo.WeightDecimalPoints;
                    objRegionalSetting.TurnsAvgDecimalPoints = objeTurnsRegionInfo.TurnsAvgDecimalPoints;
                    objRegionalSetting.NumberOfBackDaysToSyncOverPDA = objeTurnsRegionInfo.NumberOfBackDaysToSyncOverPDA;
                    objRegionalSetting.TZSupportDayLight = objeTurnsRegionInfo.TZSupportDayLight;
                    objRegionalSetting.DayLightStartTime = objeTurnsRegionInfo.DayLightStartTime;
                    objRegionalSetting.DayLightEndTime = objeTurnsRegionInfo.DayLightEndTime;
                    context.RegionalSettings.Add(objRegionalSetting);
                    context.SaveChanges();
                    objeTurnsRegionInfo.ID = objRegionalSetting.ID;
                    //RoomSchedule objRoomSchedule = new DAL.RoomSchedule ()
                    SchedulerDTO objSchedulerDTO = new SchedulerDTO();
                    objSchedulerDTO.CompanyId = objRegionalSetting.CompanyId;
                    objSchedulerDTO.Created = DateTime.UtcNow;
                    objSchedulerDTO.CreatedBy = objRegionalSetting.UserId;
                    objSchedulerDTO.DailyRecurringDays = 1;
                    objSchedulerDTO.DailyRecurringType = 1;
                    objSchedulerDTO.IsScheduleActive = true;
                    objSchedulerDTO.LastUpdatedBy = objRegionalSetting.UserId;
                    objSchedulerDTO.LoadSheduleFor = 8;
                    objSchedulerDTO.ModuleName = "Room";
                    objSchedulerDTO.NextRunDate = null;
                    objSchedulerDTO.RoomId = objRegionalSetting.RoomId;
                    objSchedulerDTO.ScheduledBy = objRegionalSetting.UserId;
                    objSchedulerDTO.ScheduleMode = 1;
                    objSchedulerDTO.ScheduleName = "daily at twelve";
                    objSchedulerDTO.ScheduleRunTime = "00:30";
                    objSchedulerDTO.ScheduleTime = new TimeSpan(0, 30, 0);
                    objSchedulerDTO.Updated = DateTime.UtcNow;
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierMasterDAL.SaveRoomDailySchedule(objSchedulerDTO);
                }
                else
                {
                    bool recalcschule = false;
                    if (objRegionalSetting.TimeZoneName != objeTurnsRegionInfo.TimeZoneName)
                    {
                        recalcschule = true;
                    }
                    objRegionalSetting.CultureCode = objeTurnsRegionInfo.CultureCode;
                    objRegionalSetting.CurrencyDecimalDigits = objeTurnsRegionInfo.CurrencyDecimalDigits;
                    objRegionalSetting.CurrencyGroupSeparator = objeTurnsRegionInfo.CurrencyGroupSeparator;
                    objRegionalSetting.LongDatePattern = objeTurnsRegionInfo.LongDatePattern;
                    objRegionalSetting.LongTimePattern = objeTurnsRegionInfo.LongTimePattern;
                    objRegionalSetting.NumberDecimalDigits = objeTurnsRegionInfo.NumberDecimalDigits;
                    objRegionalSetting.NumberDecimalSeparator = objeTurnsRegionInfo.NumberDecimalSeparator;
                    objRegionalSetting.ShortDatePattern = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionalSetting.ShortTimePattern = objeTurnsRegionInfo.ShortTimePattern;
                    objRegionalSetting.TimeZoneName = objeTurnsRegionInfo.TimeZoneName;
                    objRegionalSetting.TimeZoneOffSet = objeTurnsRegionInfo.TimeZoneOffSet;
                    objRegionalSetting.CurrencySymbol = objeTurnsRegionInfo.CurrencySymbol;
                    objRegionalSetting.GridRefreshTimeInSecond = objeTurnsRegionInfo.GridRefreshTimeInSecond;
                    objRegionalSetting.WeightDecimalPoints = objeTurnsRegionInfo.WeightDecimalPoints;
                    objRegionalSetting.TurnsAvgDecimalPoints = objeTurnsRegionInfo.TurnsAvgDecimalPoints;
                    objRegionalSetting.NumberOfBackDaysToSyncOverPDA = objeTurnsRegionInfo.NumberOfBackDaysToSyncOverPDA;
                    objRegionalSetting.TZSupportDayLight = objeTurnsRegionInfo.TZSupportDayLight;
                    objRegionalSetting.DayLightStartTime = objeTurnsRegionInfo.DayLightStartTime;
                    objRegionalSetting.DayLightEndTime = objeTurnsRegionInfo.DayLightEndTime;
                    context.SaveChanges();
                    if (recalcschule)
                    {
                        SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
                        SchedulerDTO objSchedulerDTO = objSupplierMasterDAL.GetRoomScheduleForDailyMidNight(objRegionalSetting.RoomId, 8);
                        if (objSchedulerDTO == null || objSchedulerDTO.ScheduleID < 1)
                        {
                            objSchedulerDTO = new SchedulerDTO();
                            objSchedulerDTO.CompanyId = objRegionalSetting.CompanyId;
                            objSchedulerDTO.Created = DateTime.UtcNow;
                            objSchedulerDTO.CreatedBy = objRegionalSetting.UserId;
                            objSchedulerDTO.DailyRecurringDays = 1;
                            objSchedulerDTO.DailyRecurringType = 1;
                            objSchedulerDTO.IsScheduleActive = true;
                            objSchedulerDTO.LastUpdatedBy = objRegionalSetting.UserId;
                            objSchedulerDTO.LoadSheduleFor = 8;
                            objSchedulerDTO.ModuleName = "Room";
                            objSchedulerDTO.NextRunDate = null;
                            objSchedulerDTO.RoomId = objRegionalSetting.RoomId;
                            objSchedulerDTO.ScheduledBy = objRegionalSetting.UserId;
                            objSchedulerDTO.ScheduleMode = 1;
                            objSchedulerDTO.ScheduleName = "daily at twelve";
                            objSchedulerDTO.ScheduleRunTime = "00:30";
                            objSchedulerDTO.ScheduleTime = new TimeSpan(0, 30, 0);
                            objSchedulerDTO.Updated = DateTime.UtcNow;
                            objSupplierMasterDAL.SaveRoomDailySchedule(objSchedulerDTO);
                        }
                        else
                        {
                            //objSupplierMasterDAL.SaveRoomDailySchedule(objSchedulerDTO);
                        }

                    }
                }
                return objeTurnsRegionInfo;

            }
        }

        public DateTime GetCurrentDatetimebyTimeZone(long RoomID, long CompanyID, long UserID)
        {
            TimeZoneInfo DestTimeZone = TimeZoneInfo.Utc;
            eTurnsRegionInfo objeTurnsRegionInfo = GetRegionSettingsById(RoomID, CompanyID, UserID);
            if (objeTurnsRegionInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                {
                    DestTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                }
            }
            DateTime dataTimeByZoneId = System.TimeZoneInfo.ConvertTime(DateTime.Now, System.TimeZoneInfo.Local, DestTimeZone);
            return dataTimeByZoneId;

        }

        public DateTime GetCurrentDatetimebyTimeZone(long RoomID, long CompanyID, long UserID, CultureInfo cul, string RoomDateFormat, CultureInfo RoomCulture)
        {
            TimeZoneInfo DestTimeZone = TimeZoneInfo.Utc;
            eTurnsRegionInfo objeTurnsRegionInfo = GetRegionSettingsById(RoomID, CompanyID, UserID);
            if (objeTurnsRegionInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                {
                    DestTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                }
                if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.ShortDatePattern))
                {
                    RoomDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                }
                //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["RoomCulture"] != null)
                {
                    DateTime dtTimeZone = TimeZoneInfo.ConvertTime(DateTime.Now, DestTimeZone);
                    DateTime dataTimeByZoneId = DateTime.ParseExact(Convert.ToString(dtTimeZone.ToString(objeTurnsRegionInfo.ShortDatePattern, RoomCulture)), RoomDateFormat, RoomCulture);
                    //DateTime dataTimeByZoneId = System.TimeZoneInfo.ConvertTime(DateTime.Now, System.TimeZoneInfo.Local, DestTimeZone);
                    return dataTimeByZoneId;
                }
            }

            return DateTime.Now;

        }

        public DateTime? ConvertTimeToUTCTime(DateTime? targetDate, long RoomID, long CompanyID, long UserID)
        {

            if (targetDate.HasValue && targetDate.Value != DateTime.MinValue)
            {
                if (targetDate.Value.Kind == DateTimeKind.Utc)
                {
                    targetDate = DateTime.SpecifyKind(targetDate.Value, DateTimeKind.Unspecified);
                }
                TimeZoneInfo SourceTimeZone = TimeZoneInfo.Utc;
                eTurnsRegionInfo objeTurnsRegionInfo = GetRegionSettingsById(RoomID, CompanyID, UserID);
                if (objeTurnsRegionInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                    {
                        SourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                    }
                }
                return TimeZoneInfo.ConvertTimeToUtc(targetDate.Value, SourceTimeZone);

                //return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(targetDate.Value, DateTimeKind.Local), TimeZoneInfo.Local, TimeZoneInfo.Utc);
            }
            return null;
        }

        public DateTime? ConvertLocalDateTimeToUTCDateTime(DateTime? targetDate, long RoomID, long CompanyID, long UserID)
        {
            if (targetDate.HasValue && targetDate.Value != DateTime.MinValue)
            {
                TimeZoneInfo SourceTimeZone = TimeZoneInfo.Utc;
                eTurnsRegionInfo objeTurnsRegionInfo = GetRegionSettingsById(RoomID, CompanyID, UserID);
                if (objeTurnsRegionInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                    {
                        SourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                    }
                }

                return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(targetDate.Value, DateTimeKind.Local), TimeZoneInfo.Local, TimeZoneInfo.Utc);
            }
            return null;
        }

        public DateTime? ConvertUTCToLocalDateTime(DateTime? targetDate, long RoomID, long CompanyID, long UserID)
        {
            if (targetDate.HasValue && targetDate.Value != DateTime.MinValue)
            {
                TimeZoneInfo DestTimeZone = TimeZoneInfo.Utc;
                eTurnsRegionInfo objeTurnsRegionInfo = GetRegionSettingsById(RoomID, CompanyID, UserID);
                if (objeTurnsRegionInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                    {
                        DestTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                    }
                }
                return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(targetDate.Value, DateTimeKind.Utc), TimeZoneInfo.Utc, DestTimeZone);
            }
            return null;
        }

        public void UpdateRegionalDaylightSettings(long ID, bool Settings)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RegionalSetting objRegionalSetting = context.RegionalSettings.FirstOrDefault(t => t.ID == ID);
                if (objRegionalSetting != null)
                {
                    objRegionalSetting.TZSupportDayLight = Settings;
                    context.SaveChanges();
                }
            }

        }

        public void UpdateRegionalDaylightSettingsdATE(long ID, DateTime? DayLightStartTime, DateTime? DayLightEndTime)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RegionalSetting objRegionalSetting = context.RegionalSettings.FirstOrDefault(t => t.ID == ID);
                if (objRegionalSetting != null)
                {
                    objRegionalSetting.DayLightStartTime = DayLightStartTime;
                    objRegionalSetting.DayLightEndTime = DayLightEndTime;
                    context.SaveChanges();
                }
            }

        }
        public List<RegionalSetting> GetAllregionalSettings()
        {
            List<RegionalSetting> lstAllSettings = new List<RegionalSetting>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstAllSettings = context.RegionalSettings.ToList();
            }
            return lstAllSettings;

        }
        public List<RegionalSetting> GetAllregionalSettings(long[] RoomIds)
        {
            List<RegionalSetting> lstAllSettings = new List<RegionalSetting>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstAllSettings = context.RegionalSettings.Where(t => RoomIds.Contains(t.RoomId)).ToList();
            }
            return lstAllSettings;
        }
        #endregion
    }
}
