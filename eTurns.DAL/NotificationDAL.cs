using eTurns.DTO;
using eTurns.DTO.Resources;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using eTurns.DTO.Helper;
namespace eTurns.DAL
{
    public class NotificationDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public NotificationDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public NotificationDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        string SubReportResFile = string.Empty;
        string ReportBasePath = System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"];
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
        XNamespace nsrd = XNamespace.Get("http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
        Dictionary<string, string> rptPara = null;
        string rdlPath = string.Empty;
        string strSubTablix = string.Empty;
        string DBconectstring = string.Empty;

        public void SaveEmailScheduleSetup(NotificationDTO objDTO)
        {
            short[] smodes = new short[] { 1, 2, 3, 4 };
            long[] EmailTemplateIds = GetArrayfromCSV(objDTO.EmailTemplates);
            long[] ReportIds = GetArrayfromCSV(objDTO.Reports);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ResourceLaguage> lstlangs = context.ResourceLaguages.ToList();
                if (EmailTemplateIds != null && EmailTemplateIds.Count() > 0)
                {
                    if (objDTO.SchedulerParams.ScheduleID == 0)
                    {
                        objDTO.SchedulerParams.LoadSheduleFor = Convert.ToInt16(objDTO.ScheduleFor);
                        objDTO.SchedulerParams.IsScheduleActive = true;
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    else
                    {
                        //objDTO.SchedulerParams = objSupplierMasterDAL.GetRoomScheduleByID(objDTO.SchedulerParams.ScheduleID);
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);

                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy);
                    if (!string.IsNullOrEmpty(objDTO.ScheduleRunTime))
                    {
                        if (objDTO.SchedulerParams.ScheduleMode > 0 && objDTO.SchedulerParams.ScheduleMode < 4)
                        {
                            string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                            objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                            objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                            //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                        }

                    }
                    if (objDTO.SchedulerParams.ScheduleMode < 1 || objDTO.SchedulerParams.ScheduleMode > 4)
                    {
                        string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                        objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    if (objDTO.SchedulerParams.ScheduleMode == 4)
                    {
                        objDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objDTO.SchedulerParams.HourlyAtWhatMinute, 0);

                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    foreach (var item in EmailTemplateIds)
                    {
                        Notification objNotification = new Notification();
                        objNotification.AttachmentReportIDs = objDTO.AttachmentReportIDs;
                        objNotification.AttachmentTypes = objDTO.AttachmentTypes;
                        objNotification.Created = DateTimeUtility.DateTimeNow;
                        objNotification.CreatedBy = objDTO.CreatedBy;
                        objNotification.EmailAddress = objDTO.EmailAddress;
                        objNotification.EmailTemplateID = item;
                        objNotification.ID = 0;
                        objNotification.NextRunDate = null;
                        objNotification.ReportID = null;
                        objNotification.RoomScheduleID = objDTO.SchedulerParams.ScheduleID;
                        objNotification.ScheduleFor = 6;
                        objNotification.SupplierIds = objDTO.SupplierIds;
                        objNotification.Updated = DateTimeUtility.DateTimeNow;
                        objNotification.UpdatedBy = objDTO.UpdatedBy;
                        objNotification.RoomId = objDTO.RoomID;
                        objNotification.CompanyId = objDTO.CompanyID;
                        objNotification.IsActive = objDTO.IsActive;
                        objNotification.IsDeleted = objDTO.IsDeleted;
                        objNotification.ReportDataSelectionType = objDTO.ReportDataSelectionType;
                        objNotification.ReportDataSince = objDTO.ReportDataSince;
                        objNotification.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                        objNotification.FTPId = objDTO.FTPId;
                        objNotification.NotificationMode = objDTO.NotificationMode;
                        objNotification.SendEmptyEmail = objDTO.SendEmptyEmail;
                        objNotification.HideHeader = objDTO.HideHeader;
                        objNotification.ShowSignature = objDTO.ShowSignature;
                        objNotification.SortSequence = objDTO.SortSequence;
                        objNotification.XMLValue = objDTO.XMLValue;

                        objNotification.CompanyIDs = objDTO.CompanyIds;
                        objNotification.RoomIDs = objDTO.RoomIds;
                        objNotification.Status = objDTO.Status;
                        objNotification.Range = objDTO.Range;
                        if (!string.IsNullOrWhiteSpace(objDTO.ScheduleRunTime))
                        {
                            objNotification.ScheduleTime = Convert.ToDateTime(objDTO.ScheduleRunTime).TimeOfDay;
                        }
                        if (objDTO.ScheduleRunDateTime == DateTime.MinValue)
                        {
                            objDTO.ScheduleRunDateTime = DateTime.UtcNow.Date;
                        }
                        objNotification.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                        context.Notifications.Add(objNotification);
                        context.SaveChanges();
                        objDTO.ID = objNotification.ID;
                        if (objDTO.lstEmailTemplateDtls != null && objDTO.lstEmailTemplateDtls.Count() > 0)
                        {

                            foreach (var item1 in objDTO.lstEmailTemplateDtls)
                            {
                                ResourceLaguage objLang = lstlangs.FirstOrDefault(t => t.Culture == item1.CultureCode);
                                EmailTemplateDetail objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(t => t.EmailTemplateId == item && t.RoomId == objDTO.RoomID && t.CompanyID == objDTO.CompanyID && t.NotificationID == objDTO.ID && t.ResourceLaguageId == objLang.ID);
                                if (objEmailTemplateDetail != null)
                                {
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                }
                                else
                                {
                                    objEmailTemplateDetail = new EmailTemplateDetail();
                                    objEmailTemplateDetail.CompanyID = objDTO.CompanyID;
                                    objEmailTemplateDetail.Created = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.CreatedBy = objDTO.CreatedBy;
                                    objEmailTemplateDetail.EmailTemplateId = item;
                                    objEmailTemplateDetail.ID = 0;
                                    objEmailTemplateDetail.LastUpdatedBy = objDTO.UpdatedBy;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.NotificationID = null;
                                    objEmailTemplateDetail.ResourceLaguageId = objLang.ID;
                                    objEmailTemplateDetail.RoomId = objDTO.RoomID;
                                    objEmailTemplateDetail.Updated = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.NotificationID = objDTO.ID;
                                    context.EmailTemplateDetails.Add(objEmailTemplateDetail);

                                }
                            }
                            context.SaveChanges();
                        }
                        if (objDTO.SchedulerParams.ScheduleMode != 0 && objDTO.SchedulerParams.ScheduleMode != 5)
                        {
                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + objNotification.ID + "");
                        }
                        if (objDTO.SchedulerParams.RecalcSchedule)
                        {
                            IQueryable<Notification> lstAllNoticationForthisSchedule = context.Notifications.Where(t => t.RoomScheduleID == objDTO.SchedulerParams.ScheduleID);
                            if (lstAllNoticationForthisSchedule != null && lstAllNoticationForthisSchedule.Any())
                            {
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {

                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            ntfc.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                            ntfc.NextRunDate = null;
                                        }


                                    }
                                }
                                context.SaveChanges();
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {
                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + ntfc.ID + "");
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                if (ReportIds != null && ReportIds.Count() > 0)
                {
                    if (objDTO.SchedulerParams.ScheduleID == 0)
                    {
                        objDTO.SchedulerParams.LoadSheduleFor = Convert.ToInt16(objDTO.ScheduleFor);
                        objDTO.SchedulerParams.IsScheduleActive = true;
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    else
                    {
                        //objDTO.SchedulerParams = objSupplierMasterDAL.GetRoomScheduleByID(objDTO.SchedulerParams.ScheduleID);
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy);
                    if (!string.IsNullOrEmpty(objDTO.ScheduleRunTime))
                    {
                        if (objDTO.SchedulerParams.ScheduleMode > 0 && objDTO.SchedulerParams.ScheduleMode < 4)
                        {
                            string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                            objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                            objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                            //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                        }

                    }
                    if (objDTO.SchedulerParams.ScheduleMode < 1 || objDTO.SchedulerParams.ScheduleMode > 4)
                    {
                        string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                        objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    if (objDTO.SchedulerParams.ScheduleMode == 4)
                    {
                        objDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objDTO.SchedulerParams.HourlyAtWhatMinute, 0);

                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    foreach (var item in ReportIds)
                    {
                        Notification objNotification = new Notification();
                        objNotification.AttachmentReportIDs = objDTO.AttachmentReportIDs;
                        objNotification.AttachmentTypes = objDTO.AttachmentTypes;
                        objNotification.Created = DateTimeUtility.DateTimeNow;
                        objNotification.CreatedBy = objDTO.CreatedBy;
                        objNotification.EmailAddress = objDTO.EmailAddress;
                        objNotification.EmailTemplateID = 3;
                        objNotification.ID = 0;
                        objNotification.NextRunDate = null;
                        objNotification.ReportID = item;
                        objNotification.RoomScheduleID = objDTO.SchedulerParams.ScheduleID;
                        objNotification.ScheduleFor = 5;
                        objNotification.SupplierIds = objDTO.SupplierIds;
                        objNotification.Updated = DateTimeUtility.DateTimeNow;
                        objNotification.UpdatedBy = objDTO.UpdatedBy;
                        objNotification.RoomId = objDTO.RoomID;
                        objNotification.CompanyId = objDTO.CompanyID;
                        objNotification.IsActive = objDTO.IsActive;
                        objNotification.IsDeleted = objDTO.IsDeleted;
                        objNotification.ReportDataSelectionType = objDTO.ReportDataSelectionType;
                        objNotification.ReportDataSince = objDTO.ReportDataSince;
                        objNotification.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                        objNotification.FTPId = objDTO.FTPId;
                        objNotification.NotificationMode = objDTO.NotificationMode;
                        objNotification.SendEmptyEmail = objDTO.SendEmptyEmail;
                        objNotification.HideHeader = objDTO.HideHeader;
                        objNotification.ShowSignature = objDTO.ShowSignature;
                        objNotification.SortSequence = objDTO.SortSequence;
                        objNotification.XMLValue = objDTO.XMLValue;
                        objNotification.CompanyIDs = objDTO.CompanyIds;
                        objNotification.RoomIDs = objDTO.RoomIds;
                        objNotification.Status = objDTO.Status;
                        objNotification.Range = objDTO.Range;
                        if (!string.IsNullOrWhiteSpace(objDTO.ScheduleRunTime))
                        {
                            objNotification.ScheduleTime = Convert.ToDateTime(objDTO.ScheduleRunTime).TimeOfDay;
                        }
                        context.Notifications.Add(objNotification);
                        context.SaveChanges();
                        objDTO.ID = objNotification.ID;
                        if (objDTO.lstEmailTemplateDtls != null && objDTO.lstEmailTemplateDtls.Count() > 0)
                        {

                            foreach (var item1 in objDTO.lstEmailTemplateDtls)
                            {
                                ResourceLaguage objLang = lstlangs.FirstOrDefault(t => t.Culture == item1.CultureCode);
                                //EmailTemplateDetail objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(t => t.NotificationID == item && t.ResourceLaguageId == objLang.ID && t.RoomId == objDTO.RoomID && t.CompanyID == objDTO.CompanyID);
                                EmailTemplateDetail objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(t => t.EmailTemplateId == 3 && t.NotificationID == objNotification.ID && t.ResourceLaguageId == objLang.ID && t.RoomId == objDTO.RoomID && t.CompanyID == objDTO.CompanyID);
                                if (objEmailTemplateDetail != null)
                                {
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                }
                                else
                                {
                                    objEmailTemplateDetail = new EmailTemplateDetail();
                                    objEmailTemplateDetail.CompanyID = objDTO.CompanyID;
                                    objEmailTemplateDetail.Created = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.CreatedBy = objDTO.CreatedBy;
                                    objEmailTemplateDetail.EmailTemplateId = 3;
                                    objEmailTemplateDetail.ID = 0;
                                    objEmailTemplateDetail.LastUpdatedBy = objDTO.UpdatedBy;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.NotificationID = objDTO.ID;
                                    objEmailTemplateDetail.ResourceLaguageId = objLang.ID;
                                    objEmailTemplateDetail.RoomId = objDTO.RoomID;
                                    objEmailTemplateDetail.Updated = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.NotificationID = objDTO.ID;
                                    context.EmailTemplateDetails.Add(objEmailTemplateDetail);

                                }
                            }
                            context.SaveChanges();
                        }

                        if (objDTO.SchedulerParams.ScheduleMode != 0 && objDTO.SchedulerParams.ScheduleMode != 5)
                        {
                            if (objDTO.ReportDataSelectionType == 1)
                            {
                                ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(base.DataBaseName);
                                ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
                                try
                                {
                                    objReportMailLogDTO.Id = 0;
                                    objReportMailLogDTO.ReportID = item;
                                    objReportMailLogDTO.ScheduleID = objDTO.SchedulerParams.ScheduleID;
                                    objReportMailLogDTO.CompanyID = objDTO.CompanyID;
                                    objReportMailLogDTO.RoomID = objDTO.RoomID;
                                    objReportMailLogDTO.SendDate = DateTime.UtcNow;
                                    objReportMailLogDTO.SendEmailAddress = "";
                                    objReportMailLogDTO.NotificationID = objDTO.ID;
                                    objReportMailLogDTO.AttachmentCount = null;

                                    objReportMasterDAL.InsertMailLog(objReportMailLogDTO);
                                }
                                finally
                                {
                                    objReportMasterDAL = null;
                                    objReportMailLogDTO = null;
                                }
                            }

                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + objNotification.ID + "");
                        }
                        if (objDTO.SchedulerParams.RecalcSchedule)
                        {
                            IQueryable<Notification> lstAllNoticationForthisSchedule = context.Notifications.Where(t => t.RoomScheduleID == objDTO.SchedulerParams.ScheduleID);
                            if (lstAllNoticationForthisSchedule != null && lstAllNoticationForthisSchedule.Any())
                            {
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {

                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            ntfc.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                            ntfc.NextRunDate = null;
                                        }


                                    }
                                }
                                context.SaveChanges();
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {
                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + ntfc.ID + "");
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            }
        }

        public List<NotificationDTO> GetNotificationByReportID(long CompanyID, long RoomID, long ReportID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ReportID", ReportID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
                lstNotifications = (from u in context.Database.SqlQuery<NotificationDTO>("EXEC GetNotificationByReportID @CompanyID, @RoomID, @ReportID", params1)
                                    select new NotificationDTO
                                    {
                                        AttachmentReportIDs = u.AttachmentReportIDs,
                                        AttachmentTypes = u.AttachmentTypes,
                                        CompanyID = u.CompanyID,
                                        Created = u.Created,
                                        CreatedBy = u.CreatedBy,
                                        EmailAddress = u.EmailAddress,
                                        EmailTemplateID = u.EmailTemplateID,
                                        ID = u.ID,
                                        IsActive = u.IsActive,
                                        IsDeleted = u.IsDeleted,
                                        ReportID = u.ReportID,
                                        RoomID = u.RoomID,
                                        RoomScheduleID = u.RoomScheduleID,
                                        ScheduleFor = u.ScheduleFor,
                                        ScheduleMode = u.ScheduleMode,
                                        ScheduleName = u.ScheduleName,
                                        SupplierIds = u.SupplierIds,
                                        TemplateName = u.TemplateName,
                                        Updated = u.Updated,
                                        UpdatedBy = u.UpdatedBy,
                                        EmailSubject = u.EmailSubject,
                                        NotificationMode = u.NotificationMode,
                                        FTPId = u.FTPId,
                                        SendEmptyEmail = u.SendEmptyEmail,
                                        HideHeader = u.HideHeader,
                                        ShowSignature = u.ShowSignature,
                                        SortSequence = u.SortSequence,
                                        XMLValue = u.XMLValue,

                                    }).ToList();

                return lstNotifications;
            }
        }


        public NotificationDTO GetEmailTemplateDetails(long TemplateID)
        {
            NotificationDTO objNotificationDTO = new NotificationDTO();
            return objNotificationDTO;
        }

        public long[] GetArrayfromCSV(string CSV)
        {
            if (!string.IsNullOrWhiteSpace(CSV))
            {

                return ToIntArray(CSV.Split(','));
            }
            else
            {
                return new long[] { };
            }
        }

        private long[] ToIntArray(string[] strArray)
        {
            return Array.ConvertAll<string, long>(strArray, delegate (string intParameter)
            {
                long retval = 0;
                long.TryParse((intParameter ?? string.Empty).ToString(), out retval);
                return retval;
            });
        }

        public List<SchedulerDTO> GetAllSchedules(long RoomID, long CompanyID, string ScheduleName)
        {
            List<SchedulerDTO> lstSchedules = new List<SchedulerDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from rs in context.RoomSchedules
                                where rs.RoomId == RoomID && rs.CompanyId == CompanyID && (rs.ScheduleName ?? string.Empty) != string.Empty && ((ScheduleName ?? string.Empty) == string.Empty || (rs.ScheduleName ?? string.Empty).ToLower().Contains((ScheduleName ?? string.Empty).ToLower()))
                                select new SchedulerDTO()
                                {
                                    ScheduleID = rs.ScheduleID,
                                    SupplierId = rs.SupplierId ?? 0,
                                    ScheduleMode = rs.ScheduleMode,
                                    DailyRecurringType = rs.DailyRecurringType,
                                    DailyRecurringDays = rs.DailyRecurringDays,
                                    WeeklyRecurringWeeks = rs.WeeklyRecurringWeeks,
                                    WeeklyOnMonday = rs.WeeklyOnMonday,
                                    WeeklyOnTuesday = rs.WeeklyOnTuesday,
                                    WeeklyOnWednesday = rs.WeeklyOnWednesday,
                                    WeeklyOnThursday = rs.WeeklyOnThursday,
                                    WeeklyOnFriday = rs.WeeklyOnFriday,
                                    WeeklyOnSaturday = rs.WeeklyOnSaturday,
                                    WeeklyOnSunday = rs.WeeklyOnSunday,
                                    MonthlyRecurringType = rs.MonthlyRecurringType,
                                    MonthlyDateOfMonth = rs.MonthlyDateOfMonth,
                                    MonthlyRecurringMonths = rs.MonthlyRecurringMonths,
                                    MonthlyDayOfMonth = rs.MonthlyDayOfMonth,
                                    SubmissionMethod = rs.SubmissionMethod,
                                    ScheduleRunDateTime = rs.ScheduleRunTime,
                                    LoadSheduleFor = rs.ScheduleFor,
                                    RoomId = rs.RoomId ?? 0,
                                    CreatedBy = rs.CreatedBy ?? 0,
                                    Created = rs.Created,
                                    LastUpdatedBy = rs.LastUpdatedBy ?? 0,
                                    Updated = rs.Updated,
                                    CompanyId = rs.CompanyId ?? 0,
                                    IsScheduleActive = rs.IsScheduleActive,
                                    NextRunDate = rs.NextRunDate,
                                    AssetToolID = rs.AssetToolID,
                                    ReportID = rs.ReportID,
                                    ReportDataSelectionType = rs.ReportDataSelectionType,
                                    ReportDataSince = rs.ReportDataSince,
                                    HourlyRecurringHours = rs.HourlyRecurringHours ?? 0,
                                    HourlyAtWhatMinute = rs.HourlyAtWhatMinute ?? 0,
                                    ScheduledBy = rs.ScheduledBy,
                                    IsDeleted = rs.IsDeleted,
                                    ScheduleName = rs.ScheduleName,
                                    BinNumber = rs.ScheduleName

                                }).ToList();
            }
            return lstSchedules;
        }

        public SchedulerDTO GetScheduleByName(long RoomID, long CompanyID, string ScheduleName)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from rs in context.RoomSchedules
                                   join NM in context.Notifications on rs.ScheduleID equals NM.RoomScheduleID
                                   where rs.IsDeleted == false && NM.IsDeleted == false && rs.RoomId == RoomID && rs.CompanyId == CompanyID && (rs.ScheduleName ?? string.Empty) != string.Empty && (rs.ScheduleName ?? string.Empty).ToLower() == (ScheduleName ?? string.Empty).ToLower()
                                   select new SchedulerDTO()
                                   {
                                       ScheduleID = rs.ScheduleID,
                                       SupplierId = rs.SupplierId ?? 0,
                                       ScheduleMode = rs.ScheduleMode,
                                       DailyRecurringType = rs.DailyRecurringType,
                                       DailyRecurringDays = rs.DailyRecurringDays,
                                       WeeklyRecurringWeeks = rs.WeeklyRecurringWeeks,
                                       WeeklyOnMonday = rs.WeeklyOnMonday,
                                       WeeklyOnTuesday = rs.WeeklyOnTuesday,
                                       WeeklyOnWednesday = rs.WeeklyOnWednesday,
                                       WeeklyOnThursday = rs.WeeklyOnThursday,
                                       WeeklyOnFriday = rs.WeeklyOnFriday,
                                       WeeklyOnSaturday = rs.WeeklyOnSaturday,
                                       WeeklyOnSunday = rs.WeeklyOnSunday,
                                       MonthlyRecurringType = rs.MonthlyRecurringType,
                                       MonthlyDateOfMonth = rs.MonthlyDateOfMonth,
                                       MonthlyRecurringMonths = rs.MonthlyRecurringMonths,
                                       MonthlyDayOfMonth = rs.MonthlyDayOfMonth,
                                       SubmissionMethod = rs.SubmissionMethod,
                                       ScheduleRunDateTime = rs.ScheduleRunTime,
                                       LoadSheduleFor = rs.ScheduleFor,
                                       RoomId = rs.RoomId ?? 0,
                                       CreatedBy = rs.CreatedBy ?? 0,
                                       Created = rs.Created,
                                       LastUpdatedBy = rs.LastUpdatedBy ?? 0,
                                       Updated = rs.Updated,
                                       CompanyId = rs.CompanyId ?? 0,
                                       IsScheduleActive = rs.IsScheduleActive,
                                       NextRunDate = rs.NextRunDate,
                                       AssetToolID = rs.AssetToolID,
                                       ReportID = rs.ReportID,
                                       ReportDataSelectionType = rs.ReportDataSelectionType,
                                       ReportDataSince = rs.ReportDataSince,
                                       HourlyRecurringHours = rs.HourlyRecurringHours ?? 0,
                                       HourlyAtWhatMinute = rs.HourlyAtWhatMinute ?? 0,
                                       ScheduledBy = rs.ScheduledBy,
                                       IsDeleted = rs.IsDeleted,
                                       ScheduleName = rs.ScheduleName,
                                       BinNumber = rs.ScheduleName

                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                }
            }
            return objSchedulerDTO;
        }

        public string IsDuplicateSchedule(long ID, string ScheduleName, long RoomId, long CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.RoomSchedules
                           join noti in context.Notifications on em.ScheduleID equals noti.RoomScheduleID
                           where em.ScheduleName == ScheduleName && em.IsDeleted == false && em.ScheduleID != ID && em.CompanyId == CompanyID && em.RoomId == RoomId
                           && noti.IsDeleted == false && noti.RoomId == RoomId && noti.CompanyId == CompanyID
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;

        }

        public List<NotificationDTO> GetPagedNotifications(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long UserID, string CurrentCulture)
        {
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            string NotificationCreaters = null;
            string NotificationUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string notificationreports = null;
            string notificationtemplates = null;
            string notificationschedules = null;
            string notificationtypes = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            TotalCount = 0;

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
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    NotificationCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    NotificationUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[52]))
                {
                    notificationreports = FieldsPara[52].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[51]))
                {
                    notificationtemplates = FieldsPara[51].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[53]))
                {
                    notificationschedules = FieldsPara[53].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[54]))
                {
                    notificationtypes = FieldsPara[54].TrimEnd(',');
                }

            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //List<getpagednotifications_Result> lstPullsfromDB = context.getpagednotifications(StartRowIndex, MaxRows, SearchTerm, sortColumnName, notificationtypes, notificationreports, notificationtemplates, notificationschedules, NotificationCreaters, NotificationUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, UserID, CurrentCulture).ToList();

                //lstNotifications = (from u in lstPullsfromDB
                var params1 = new SqlParameter[] { new SqlParameter("@startrowindex", StartRowIndex), new SqlParameter("@maxrows", MaxRows), new SqlParameter("@searchterm", SearchTerm ?? (object)DBNull.Value)
                ,new SqlParameter("@sortcolumnname", sortColumnName ?? (object)DBNull.Value), new SqlParameter("@notificationtypes", notificationtypes ?? (object)DBNull.Value), new SqlParameter("@notificationreports", notificationreports ?? (object)DBNull.Value) ,new SqlParameter("@notificationtemplates", notificationtemplates ?? (object)DBNull.Value), new SqlParameter("@notificationschedules", notificationschedules ?? (object)DBNull.Value), new SqlParameter("@notificationcreaters", NotificationCreaters ?? (object)DBNull.Value),new SqlParameter("@notificationupdators",NotificationUpdators ?? (object)DBNull.Value),new SqlParameter("@createddatefrom", CreatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@createddateto", CreatedDateTo ?? (object)DBNull.Value), new SqlParameter("@updateddatefrom", UpdatedDateFrom ?? (object)DBNull.Value) ,new SqlParameter("@updateddateto", UpdatedDateTo ?? (object)DBNull.Value), new SqlParameter("@udf1", UDF1 ?? (object)DBNull.Value), new SqlParameter("@udf2", UDF2 ?? (object)DBNull.Value)
                ,new SqlParameter("@udf3", UDF3 ?? (object)DBNull.Value), new SqlParameter("@udf4", UDF4 ?? (object)DBNull.Value),new SqlParameter("@udf5", UDF5 ?? (object)DBNull.Value), new SqlParameter("@isdeleted", IsDeleted  ), new SqlParameter("@isarchived", IsArchived  ), new SqlParameter("@roomid", RoomID  ), new SqlParameter("@companyid", CompanyID  ), new SqlParameter("@userid", UserID  ), new SqlParameter("@currentculture", CurrentCulture ?? (object)DBNull.Value)
                };
                lstNotifications = (from u in context.Database.SqlQuery<NotificationDTO>("exec [GetPagedNotifications] @startrowindex,@maxrows,@searchterm,@sortcolumnname,@notificationtypes,@notificationreports,@notificationtemplates,@notificationschedules,@notificationcreaters,@notificationupdators,@createddatefrom,@createddateto,@updateddatefrom,@updateddateto,@udf1,@udf2,@udf3,@udf4,@udf5,@isdeleted,@isarchived,@roomid,@companyid,@userid,@currentculture", params1)
                                    select new NotificationDTO
                                    {
                                        AttachmentReportIDs = u.AttachmentReportIDs,
                                        AttachmentTypes = u.AttachmentTypes,
                                        CompanyID = u.CompanyID,
                                        Created = u.Created,
                                        CreatedBy = u.CreatedBy,
                                        CreatedByName = u.CreatedByName,
                                        EmailAddress = u.EmailAddress,
                                        EmailTemplateID = u.EmailTemplateID,
                                        ID = u.ID,
                                        IsActive = u.IsActive,
                                        IsDeleted = u.IsDeleted,
                                        NextRunDate = u.NextRunDate,
                                        ReportID = u.ReportID,
                                        RoomID = u.RoomID,
                                        RoomScheduleID = u.RoomScheduleID,
                                        ScheduleFor = u.ScheduleFor,
                                        ScheduleMode = u.ScheduleMode,
                                        ScheduleName = u.ScheduleName,
                                        SupplierIds = u.SupplierIds,
                                        TemplateName = u.TemplateName,
                                        Updated = u.Updated,
                                        UpdatedBy = u.UpdatedBy,
                                        UpdatedByName = u.UpdatedByName,
                                        ReportName = u.ReportName,
                                        EmailSubject = u.EmailSubject,
                                        NotificationMode = u.NotificationMode,
                                        FTPId = u.FTPId,
                                        SendEmptyEmail = u.SendEmptyEmail,
                                        HideHeader = u.HideHeader,
                                        ShowSignature = u.ShowSignature,
                                        SortSequence = u.SortSequence,
                                        XMLValue = u.XMLValue,
                                        TotalRecords = u.TotalRecords,
                                        ResourceKey = u.ResourceKey,
                                        ResourceKeyName = u.ResourceKeyName
                                    }).ToList();
                if (lstNotifications != null && lstNotifications.Count() > 0)
                {

                    TotalCount = lstNotifications.First().TotalRecords ?? 0;
                }
            }
            return lstNotifications;

        }
        public List<NotificationDTO> GetPagedNotifications_ChangeLog(Int32 NotificationID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, long UserID, string CurrentCulture)
        {
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            string NotificationCreaters = null;
            string NotificationUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string notificationreports = null;
            string notificationtemplates = null;
            string notificationschedules = null;
            string notificationtypes = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            TotalCount = 0;

            DataSet dsNotification_History = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstNotifications;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsNotification_History = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedNotifications_ChangeLog", NotificationID, StartRowIndex, MaxRows, SearchTerm, sortColumnName, notificationtypes, notificationreports, notificationtemplates, notificationschedules, NotificationCreaters, NotificationUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, UserID, CurrentCulture);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                SearchTerm = string.Empty;
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    NotificationCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    NotificationUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[52]))
                {
                    notificationreports = FieldsPara[52].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[51]))
                {
                    notificationtemplates = FieldsPara[51].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[53]))
                {
                    notificationschedules = FieldsPara[53].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[54]))
                {
                    notificationtypes = FieldsPara[54].TrimEnd(',');
                }
                dsNotification_History = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedNotifications_ChangeLog", NotificationID, StartRowIndex, MaxRows, SearchTerm, sortColumnName, notificationtypes, notificationreports, notificationtemplates, notificationschedules, NotificationCreaters, NotificationUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, UserID, CurrentCulture);
            }
            else
            {
                dsNotification_History = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedNotifications_ChangeLog", NotificationID, StartRowIndex, MaxRows, SearchTerm, sortColumnName, notificationtypes, notificationreports, notificationtemplates, notificationschedules, NotificationCreaters, NotificationUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, UserID, CurrentCulture);
            }
            if (dsNotification_History != null && dsNotification_History.Tables.Count > 0)
            {
                DataTable dtNotification_History = dsNotification_History.Tables[0];
                if (dtNotification_History.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtNotification_History.Rows[0]["TotalRecords"]);
                    lstNotifications = dtNotification_History.AsEnumerable()
                    .Select(row => new NotificationDTO
                    {
                        HistoryID = row.Field<int>("HistoryID"),
                        Action = row.Field<string>("Action"),
                        AttachmentTypes = row.Field<string>("AttachmentTypes"),
                        CompanyID = row.Field<long>("CompanyId"),
                        Created = row.Field<DateTime>("Created"),
                        CreatedBy = row.Field<long>("CreatedBy"),
                        CreatedByName = row.Field<string>("createdbyname"),

                        EmailAddress = row.Field<string>("EmailAddress"),
                        EmailTemplateID = row.Field<long?>("EmailTemplateID"),
                        ID = row.Field<long>("ID"),
                        IsActive = row.Field<bool>("IsActive"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        NextRunDate = row.Field<DateTime?>("NextRunDate"),
                        ReportID = row.Field<long?>("ReportID"),
                        RoomID = row.Field<long>("RoomId"),
                        RoomScheduleID = row.Field<long>("RoomScheduleID"),

                        ScheduleFor = row.Field<int>("ScheduleFor"),
                        ScheduleMode = row.Field<short?>("schedulemode"),
                        ScheduleName = row.Field<string>("schedulename"),
                        SupplierIds = row.Field<string>("SupplierIds"),
                        TemplateName = row.Field<string>("templatename"),
                        Updated = row.Field<DateTime>("Updated"),
                        UpdatedBy = row.Field<long>("UpdatedBy"),
                        UpdatedByName = row.Field<string>("updatedbyname"),
                        ReportName = row.Field<string>("reportname"),

                        EmailSubject = row.Field<string>("mailsubject"),
                        NotificationMode = row.Field<int>("NotificationMode"),
                        FTPId = row.Field<long?>("FTPId"),
                        SendEmptyEmail = row.Field<bool?>("SendEmptyEmail") ?? false,
                        HideHeader = row.Field<bool>("HideHeader"),
                        ShowSignature = row.Field<bool?>("ShowSignature") ?? false,
                        SortSequence = row.Field<string>("SortSequence"),
                        XMLValue = row.Field<string>("XMLValue"),
                        ResourceKey = row.Field<string>("ResourceKey"),
                        ResourceKeyName = row.Field<string>("ResourceKeyName")
                    }).ToList();
                }
            }
            return lstNotifications;
        }
        public List<NotificationDTO> GetAllSchedulesByTimePeriod(DateTime FromDate, DateTime ToDate)
        {
            short[] arrmodes = new short[] { 1, 2, 3, 4 };
            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.Notifications
                                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                from nm_tm in nm_tm_join.DefaultIfEmpty()
                                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                from sm_tm in sm_tm_join.DefaultIfEmpty()
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where arrmodes.Contains(sm_tm.ScheduleMode) && nm.IsDeleted == false && nm.IsActive == true && nm.NextRunDate >= FromDate && nm.NextRunDate <= ToDate && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                                select new NotificationDTO
                                {
                                    AttachmentReportIDs = nm.AttachmentReportIDs,
                                    AttachmentTypes = nm.AttachmentTypes,
                                    CompanyID = nm.CompanyId,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    EmailAddress = nm.EmailAddress,
                                    EmailTemplateID = nm.EmailTemplateID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsDeleted = nm.IsDeleted,
                                    NextRunDate = nm.NextRunDate,
                                    ReportID = nm.ReportID,
                                    RoomID = nm.RoomId,
                                    RoomScheduleID = nm.RoomScheduleID,
                                    ScheduleFor = nm.ScheduleFor,
                                    ScheduleName = sm_tm.ScheduleName,
                                    SupplierIds = nm.SupplierIds,
                                    TemplateName = nm_tm.TemplateName,
                                    Updated = nm.Updated,
                                    UpdatedBy = nm.UpdatedBy,
                                    ReportName = nm_rpt.ReportName,
                                    ReportDataSince = nm.ReportDataSince,
                                    ReportDataSelectionType = nm.ReportDataSelectionType,
                                    RoomName = rm.RoomName,
                                    CompanyName = cm.Name,
                                    NotificationMode = nm.NotificationMode,
                                    FTPId = nm.FTPId,
                                    ScheduleTime = nm.ScheduleTime,
                                    SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                    HideHeader = nm.HideHeader,
                                    ShowSignature = nm.ShowSignature,
                                    SortSequence = nm.SortSequence,
                                    XMLValue = nm.XMLValue,
                                    CompanyIds = nm.CompanyIDs,
                                    RoomIds = nm.RoomIDs,
                                    Status = nm.Status,
                                    Range = nm.Range
                                }).ToList();

                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport

                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == rs.CultureCode && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = rs.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }

                    });
                }
            }

            return lstSchedules;
        }

        public void UpdateNextRunDateOfNotification(long ScheduleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "EXEC GetNextReportRunTimeNotification " + ScheduleID;
                context.Database.ExecuteSqlCommand(qry);
            }
        }

        public NotificationDTO GetNotifiactionByID(long ID)
        {
            NotificationDTO objNotificationDTO = new NotificationDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objNotificationDTO = (from nm in context.Notifications
                                      join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                      from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                      join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                      from nm_tm in nm_tm_join.DefaultIfEmpty()
                                      join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                      from sm_tm in sm_tm_join.DefaultIfEmpty()
                                      join rm in context.Rooms on nm.RoomId equals rm.ID
                                      join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                      where nm.ID == ID
                                      select new NotificationDTO
                                      {
                                          AttachmentReportIDs = nm.AttachmentReportIDs,
                                          AttachmentTypes = nm.AttachmentTypes,
                                          CompanyID = nm.CompanyId,
                                          Created = nm.Created,
                                          CreatedBy = nm.CreatedBy,
                                          EmailAddress = nm.EmailAddress,
                                          EmailTemplateID = nm.EmailTemplateID,
                                          ID = nm.ID,
                                          IsActive = nm.IsActive,
                                          IsDeleted = nm.IsDeleted,
                                          NextRunDate = nm.NextRunDate,
                                          ReportID = nm.ReportID,
                                          RoomID = nm.RoomId,
                                          RoomScheduleID = nm.RoomScheduleID,
                                          ScheduleFor = nm.ScheduleFor,
                                          ScheduleName = sm_tm.ScheduleName,
                                          SupplierIds = nm.SupplierIds,
                                          TemplateName = nm_tm.TemplateName,
                                          Updated = nm.Updated,
                                          UpdatedBy = nm.UpdatedBy,
                                          ReportName = nm_rpt.ReportName,
                                          ReportDataSince = nm.ReportDataSince,
                                          ReportDataSelectionType = nm.ReportDataSelectionType,
                                          ScheduleRunDateTime = nm.ScheduleRunTime ?? DateTime.MinValue,
                                          NotificationMode = nm.NotificationMode,
                                          FTPId = nm.FTPId,
                                          ScheduleTime = nm.ScheduleTime,
                                          SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                          HideHeader = nm.HideHeader,
                                          ShowSignature = nm.ShowSignature,
                                          SortSequence = nm.SortSequence,
                                          XMLValue = nm.XMLValue,
                                          CompanyIds = nm.CompanyIDs,
                                          RoomIds = nm.RoomIDs,
                                          Status = nm.Status,
                                          Range = nm.Range,

                                      }).FirstOrDefault();


                if (objNotificationDTO != null)
                {

                    if (objNotificationDTO.ReportID > 0)
                    {


                        objNotificationDTO.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                              where nm_rpt.ID == objNotificationDTO.ReportID
                                                              select new ReportBuilderDTO
                                                              {
                                                                  ChildReportName = nm_rpt.SubReportFileName,
                                                                  CompanyID = nm_rpt.CompanyID,
                                                                  Created = nm_rpt.CreatedOn,
                                                                  CreatedBy = nm_rpt.CreatedBy,
                                                                  Days = nm_rpt.Days,
                                                                  FromDate = nm_rpt.FromDate,
                                                                  GroupName = nm_rpt.GroupName,
                                                                  ID = nm_rpt.ID,
                                                                  IsBaseReport = nm_rpt.IsBaseReport,
                                                                  IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                                  IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                                  IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                                  IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                                  IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                                  IsPrivate = nm_rpt.IsPrivate,
                                                                  LastUpdatedBy = nm_rpt.UpdatedBy,
                                                                  MasterReportResFile = nm_rpt.MasterReportResFile,
                                                                  ModuleName = nm_rpt.ModuleName,
                                                                  ParentID = nm_rpt.ParentID,
                                                                  PrivateUserID = nm_rpt.PrivateUserID,
                                                                  ReportFileName = nm_rpt.ReportFileName,
                                                                  ReportName = nm_rpt.ReportName,
                                                                  ReportType = nm_rpt.ReportType,
                                                                  RoomID = nm_rpt.RoomID,
                                                                  SortColumns = nm_rpt.SortColumns,
                                                                  SubReportFileName = nm_rpt.SubReportFileName,
                                                                  SubReportResFile = nm_rpt.SubReportResFile,
                                                                  ToDate = nm_rpt.ToDate,
                                                                  ToEmailAddress = nm_rpt.ToEmailAddress,
                                                                  Updated = nm_rpt.UpdatedON,
                                                                  IsNotEditable = nm_rpt.IsNotEditable,
                                                                  //IsSupplierRequired = nm_rpt.IsSupplierRequired

                                                              }).FirstOrDefault();


                        if (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ParentID > 0)
                        {
                            ReportMasterDAL objDAL = new ReportMasterDAL(base.DataBaseName);
                            ReportMasterDTO objDTO = objDAL.GetParentReportSigleRecord(objNotificationDTO.ReportMasterDTO.ID, 0, "Report");
                            if (objDTO != null && objDTO.ID > 0)
                            {
                                objNotificationDTO.ReportMasterDTO.ParentReportName = objDTO.ReportName;
                                objNotificationDTO.ParentReportName = objDTO.ReportName;
                            }
                            else
                            {
                                objNotificationDTO.ReportMasterDTO.ParentReportName = objNotificationDTO.ReportMasterDTO.ReportName;
                                objNotificationDTO.ParentReportName = objNotificationDTO.ReportName;
                            }
                        }
                        else if (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ParentID == 0)
                        {
                            objNotificationDTO.ReportMasterDTO.ParentReportName = objNotificationDTO.ReportMasterDTO.ReportName;
                            objNotificationDTO.ParentReportName = objNotificationDTO.ReportName;
                        }

                    }
                    long attachedreportID = !string.IsNullOrWhiteSpace(objNotificationDTO.AttachmentReportIDs) ? Convert.ToInt64(objNotificationDTO.AttachmentReportIDs.Trim().Split(',').First()) : 0;
                    if (attachedreportID > 0)
                    {
                        objNotificationDTO.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                                      where nm_rpt.ID == attachedreportID
                                                                      select new ReportBuilderDTO
                                                                      {
                                                                          ChildReportName = nm_rpt.SubReportFileName,
                                                                          CompanyID = nm_rpt.CompanyID,
                                                                          Created = nm_rpt.CreatedOn,
                                                                          CreatedBy = nm_rpt.CreatedBy,
                                                                          Days = nm_rpt.Days,
                                                                          FromDate = nm_rpt.FromDate,
                                                                          GroupName = nm_rpt.GroupName,
                                                                          ID = nm_rpt.ID,
                                                                          IsBaseReport = nm_rpt.IsBaseReport,
                                                                          IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                                          IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                                          IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                                          IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                                          IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                                          IsPrivate = nm_rpt.IsPrivate,
                                                                          LastUpdatedBy = nm_rpt.UpdatedBy,
                                                                          MasterReportResFile = nm_rpt.MasterReportResFile,
                                                                          ModuleName = nm_rpt.ModuleName,
                                                                          ParentID = nm_rpt.ParentID,
                                                                          PrivateUserID = nm_rpt.PrivateUserID,
                                                                          ReportFileName = nm_rpt.ReportFileName,
                                                                          ReportName = nm_rpt.ReportName,
                                                                          ReportType = nm_rpt.ReportType,
                                                                          RoomID = nm_rpt.RoomID,
                                                                          SortColumns = nm_rpt.SortColumns,
                                                                          SubReportFileName = nm_rpt.SubReportFileName,
                                                                          SubReportResFile = nm_rpt.SubReportResFile,
                                                                          ToDate = nm_rpt.ToDate,
                                                                          ToEmailAddress = nm_rpt.ToEmailAddress,
                                                                          Updated = nm_rpt.UpdatedON,
                                                                          //IsSupplierRequired = nm_rpt.IsSupplierRequired
                                                                      }).FirstOrDefault();
                    }
                    if (objNotificationDTO.RoomScheduleID > 0)
                    {


                        objNotificationDTO.SchedulerParams = (from sm_tm in context.RoomSchedules
                                                              where sm_tm.ScheduleID == objNotificationDTO.RoomScheduleID
                                                              select new SchedulerDTO
                                                              {
                                                                  AssetToolID = sm_tm.AssetToolID,
                                                                  CompanyId = sm_tm.CompanyId ?? 0,
                                                                  Created = sm_tm.Created,
                                                                  CreatedBy = sm_tm.CreatedBy ?? 0,
                                                                  DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                                  DailyRecurringType = sm_tm.DailyRecurringType,
                                                                  HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                                  HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                                  IsDeleted = sm_tm.IsDeleted,
                                                                  IsScheduleActive = sm_tm.IsScheduleActive,
                                                                  LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                                  MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                                  MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                                  MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                                  MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                                  NextRunDate = sm_tm.NextRunDate,
                                                                  ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                                  ReportDataSince = sm_tm.ReportDataSince,
                                                                  ReportID = sm_tm.ReportID,
                                                                  RoomId = sm_tm.RoomId ?? 0,
                                                                  ScheduledBy = sm_tm.ScheduledBy,
                                                                  LoadSheduleFor = sm_tm.ScheduleFor,
                                                                  ScheduleID = sm_tm.ScheduleID,
                                                                  ScheduleMode = sm_tm.ScheduleMode,
                                                                  ScheduleName = sm_tm.ScheduleName,
                                                                  ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                                  SubmissionMethod = sm_tm.SubmissionMethod,
                                                                  SupplierId = sm_tm.SupplierId ?? 0,
                                                                  Updated = sm_tm.Updated,
                                                                  WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                                  WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                                  WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                                  WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                                  WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                                  WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                                  WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                                  WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                                              }).FirstOrDefault();
                    }
                    if (objNotificationDTO.EmailTemplateID > 0)
                    {


                        objNotificationDTO.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                                  where nm_tm.ID == objNotificationDTO.EmailTemplateID
                                                                  select new EmailTemplateDTO
                                                                  {
                                                                      CompanyId = nm_tm.CompanyId,
                                                                      Created = nm_tm.Created,
                                                                      CreatedBy = nm_tm.CreatedBy,
                                                                      ID = nm_tm.ID,
                                                                      LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                                      RoomId = nm_tm.RoomId,
                                                                      TemplateName = nm_tm.TemplateName,
                                                                      Updated = nm_tm.Updated,
                                                                      //IsSupplierRequired = nm_tm.IsSupplierRequired,
                                                                      lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                                              join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                                              join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                                              where etd.RoomId == objNotificationDTO.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == rs.CultureCode && etd.NotificationID == objNotificationDTO.ID //(objNotificationDTO.ScheduleFor == 5 ? etd.NotificationID == objNotificationDTO.ID : true)
                                                                                              select new EmailTemplateDetailDTO()
                                                                                              {
                                                                                                  CompanyID = etd.CompanyID,
                                                                                                  Created = etd.Created,
                                                                                                  CreatedBy = etd.CreatedBy,
                                                                                                  CultureCode = rs.CultureCode,
                                                                                                  EmailTempateId = etd.EmailTemplateId,
                                                                                                  ID = etd.ID,
                                                                                                  LastUpdatedBy = etd.LastUpdatedBy,
                                                                                                  MailBodyText = etd.MailBodyText,
                                                                                                  MailSubject = etd.MailSubject,
                                                                                                  NotificationID = etd.NotificationID,
                                                                                                  ResourceLaguageId = etd.ResourceLaguageId,
                                                                                                  RoomId = etd.RoomId,
                                                                                                  Updated = etd.Updated
                                                                                              }).AsEnumerable()
                                                                  }).FirstOrDefault();
                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    objNotificationDTO.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, objNotificationDTO.CreatedBy);
                    if (objNotificationDTO.objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName))
                    {
                        objNotificationDTO.ScheduleRunTime = (DateTimeUtility.ConvertDateByTimeZonedt(objNotificationDTO.ScheduleRunDateTime, TimeZoneInfo.FindSystemTimeZoneById(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName), null, null) ?? DateTime.MinValue).ToString("HH:mm");
                    }
                    else
                    {
                        objNotificationDTO.ScheduleRunTime = objNotificationDTO.ScheduleRunDateTime.ToString("HH:mm");
                    }
                    if ((objNotificationDTO.FTPId ?? 0) > 0)
                    {
                        objNotificationDTO.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(objNotificationDTO.FTPId ?? 0, objNotificationDTO.RoomID, objNotificationDTO.CompanyID);
                    }

                }
            }
            return objNotificationDTO;
        }

        public void UpdateEmailScheduleSetup(NotificationDTO objDTO)
        {
            short[] smodes = new short[] { 1, 2, 3, 4 };
            bool UpdateSchedule = false;
            long[] EmailTemplateIds = GetArrayfromCSV(objDTO.EmailTemplates);
            long[] ReportIds = GetArrayfromCSV(objDTO.Reports);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(base.DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ResourceLaguage> lstlangs = context.ResourceLaguages.ToList();
                if (EmailTemplateIds != null && EmailTemplateIds.Count() > 0)
                {
                    if (objDTO.SchedulerParams.ScheduleID == 0)
                    {
                        objDTO.SchedulerParams.LoadSheduleFor = Convert.ToInt16(objDTO.ScheduleFor);
                        objDTO.SchedulerParams.IsScheduleActive = true;
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(objDTO.SchedulerParams.ScheduleRunTime))
                        {
                            objDTO.SchedulerParams.ScheduleRunTime = objDTO.ScheduleRunTime;
                        }

                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                        //objDTO.SchedulerParams = objSupplierMasterDAL.GetRoomScheduleByID(objDTO.SchedulerParams.ScheduleID);
                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy);
                    if (!string.IsNullOrEmpty(objDTO.ScheduleRunTime))
                    {
                        if (objDTO.SchedulerParams.ScheduleMode > 0 && objDTO.SchedulerParams.ScheduleMode < 4)
                        {
                            string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                            objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                            objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                            //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                        }

                    }
                    if (objDTO.SchedulerParams.ScheduleMode < 1 || objDTO.SchedulerParams.ScheduleMode > 4)
                    {
                        string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                        objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    if (objDTO.SchedulerParams.ScheduleMode == 4)
                    {
                        objDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objDTO.SchedulerParams.HourlyAtWhatMinute, 0);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    foreach (var item in EmailTemplateIds)
                    {
                        Notification objNotification = context.Notifications.FirstOrDefault(t => t.ID == objDTO.ID);
                        if (objNotification != null)
                        {
                            if (objNotification.RoomScheduleID != objDTO.SchedulerParams.ScheduleID || (!objNotification.IsActive && (objNotification.IsActive != objDTO.IsActive)))
                            {
                                UpdateSchedule = true;
                            }
                            else if (objDTO.SchedulerParams.RecalcSchedule)
                            {
                                UpdateSchedule = true;

                            }
                            objNotification.AttachmentReportIDs = objDTO.AttachmentReportIDs;
                            objNotification.AttachmentTypes = objDTO.AttachmentTypes;
                            objNotification.EmailAddress = objDTO.EmailAddress;
                            objNotification.RoomScheduleID = objDTO.SchedulerParams.ScheduleID;
                            objNotification.SupplierIds = objDTO.SupplierIds;
                            objNotification.Updated = DateTimeUtility.DateTimeNow;
                            objNotification.UpdatedBy = objDTO.UpdatedBy;
                            objNotification.IsActive = objDTO.IsActive;
                            objNotification.ReportDataSelectionType = objDTO.ReportDataSelectionType;
                            objNotification.ReportDataSince = objDTO.ReportDataSince;
                            objNotification.NotificationMode = objDTO.NotificationMode;
                            objNotification.FTPId = objDTO.FTPId;
                            objNotification.SendEmptyEmail = objDTO.SendEmptyEmail;
                            objNotification.HideHeader = objDTO.HideHeader;
                            objNotification.ShowSignature = objDTO.ShowSignature;
                            objNotification.SortSequence = objDTO.SortSequence;
                            objNotification.XMLValue = objDTO.XMLValue;


                            objNotification.CompanyIDs = objDTO.CompanyIds;
                            objNotification.RoomIDs = objDTO.RoomIds;
                            objNotification.Status = objDTO.Status;
                            objNotification.Range = objDTO.Range;
                            if (!string.IsNullOrWhiteSpace(objDTO.ScheduleRunTime))
                            {
                                objNotification.ScheduleTime = Convert.ToDateTime(objDTO.ScheduleRunTime).TimeOfDay;
                            }
                            if (UpdateSchedule && smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                            {
                                objNotification.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                objNotification.NextRunDate = null;
                            }
                        }
                        context.SaveChanges();
                        if (objDTO.lstEmailTemplateDtls != null && objDTO.lstEmailTemplateDtls.Count() > 0)
                        {

                            foreach (var item1 in objDTO.lstEmailTemplateDtls)
                            {
                                ResourceLaguage objLang = lstlangs.FirstOrDefault(t => t.Culture == item1.CultureCode);
                                EmailTemplateDetail objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(t => t.EmailTemplateId == item && t.RoomId == objDTO.RoomID && t.CompanyID == objDTO.CompanyID && t.NotificationID == objDTO.ID && t.ResourceLaguageId == objLang.ID);
                                if (objEmailTemplateDetail != null)
                                {
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                }
                                else
                                {
                                    objEmailTemplateDetail = new EmailTemplateDetail();
                                    objEmailTemplateDetail.CompanyID = objDTO.CompanyID;
                                    objEmailTemplateDetail.Created = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.CreatedBy = objDTO.CreatedBy;
                                    objEmailTemplateDetail.EmailTemplateId = item;
                                    objEmailTemplateDetail.ID = 0;
                                    objEmailTemplateDetail.LastUpdatedBy = objDTO.UpdatedBy;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.NotificationID = null;
                                    objEmailTemplateDetail.ResourceLaguageId = objLang.ID;
                                    objEmailTemplateDetail.RoomId = objDTO.RoomID;
                                    objEmailTemplateDetail.Updated = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.NotificationID = objDTO.ID;
                                    context.EmailTemplateDetails.Add(objEmailTemplateDetail);

                                }
                            }
                            context.SaveChanges();
                        }
                        if (UpdateSchedule)
                        {
                            if (objDTO.SchedulerParams.ScheduleMode != 0 && objDTO.SchedulerParams.ScheduleMode != 5)
                            {
                                //GenetareNextRunDate(objNotification.ID, 0);
                                context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + objNotification.ID + "");
                            }
                        }
                        if (objDTO.SchedulerParams.RecalcSchedule)
                        {
                            IQueryable<Notification> lstAllNoticationForthisSchedule = context.Notifications.Where(t => t.RoomScheduleID == objDTO.SchedulerParams.ScheduleID);
                            if (lstAllNoticationForthisSchedule != null && lstAllNoticationForthisSchedule.Any())
                            {
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {

                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            ntfc.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                            ntfc.NextRunDate = null;
                                        }


                                    }
                                }
                                context.SaveChanges();
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {
                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            //GenetareNextRunDate(objNotification.ID, 0);
                                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + ntfc.ID + "");
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                if (ReportIds != null && ReportIds.Count() > 0)
                {
                    if (objDTO.SchedulerParams.ScheduleID == 0)
                    {
                        objDTO.SchedulerParams.LoadSheduleFor = Convert.ToInt16(objDTO.ScheduleFor);
                        objDTO.SchedulerParams.IsScheduleActive = true;
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    else
                    {
                        //objDTO.SchedulerParams = objSupplierMasterDAL.GetRoomScheduleByID(objDTO.SchedulerParams.ScheduleID);
                        objDTO.SchedulerParams = objSupplierMasterDAL.SaveSchedule(objDTO.SchedulerParams);
                    }
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy);
                    if (!string.IsNullOrEmpty(objDTO.ScheduleRunTime))
                    {
                        if (objDTO.SchedulerParams.ScheduleMode > 0 && objDTO.SchedulerParams.ScheduleMode < 4)
                        {
                            string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                            objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                            objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                            //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                        }

                    }
                    if (objDTO.SchedulerParams.ScheduleMode < 1 || objDTO.SchedulerParams.ScheduleMode > 4)
                    {
                        string strtmp = datetimetoConsider.ToShortDateString() + " " + objDTO.SchedulerParams.ScheduleRunTime;
                        objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    if (objDTO.SchedulerParams.ScheduleMode == 4)
                    {
                        objDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objDTO.SchedulerParams.HourlyAtWhatMinute, 0);
                        objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                    }
                    foreach (var item in ReportIds)
                    {
                        Notification objNotification = context.Notifications.FirstOrDefault(t => t.ID == objDTO.ID);
                        if (objNotification != null)
                        {
                            if (objNotification.RoomScheduleID != objDTO.SchedulerParams.ScheduleID || (!objNotification.IsActive && (objNotification.IsActive != objDTO.IsActive)))
                            {
                                UpdateSchedule = true;
                            }
                            else if (objDTO.SchedulerParams.RecalcSchedule)
                            {
                                UpdateSchedule = true;

                            }
                            objNotification.AttachmentReportIDs = objDTO.AttachmentReportIDs;
                            objNotification.AttachmentTypes = objDTO.AttachmentTypes;
                            objNotification.EmailAddress = objDTO.EmailAddress;
                            objNotification.RoomScheduleID = objDTO.SchedulerParams.ScheduleID;
                            objNotification.SupplierIds = objDTO.SupplierIds;
                            objNotification.Updated = DateTimeUtility.DateTimeNow;
                            objNotification.UpdatedBy = objDTO.UpdatedBy;
                            objNotification.ReportDataSelectionType = objDTO.ReportDataSelectionType;
                            objNotification.ReportDataSince = objDTO.ReportDataSince;
                            objNotification.IsActive = objDTO.IsActive;
                            objNotification.FTPId = objDTO.FTPId;
                            objNotification.NotificationMode = objDTO.NotificationMode;
                            objNotification.SendEmptyEmail = objDTO.SendEmptyEmail;
                            objNotification.HideHeader = objDTO.HideHeader;
                            objNotification.ShowSignature = objDTO.ShowSignature;
                            objNotification.SortSequence = objDTO.SortSequence;
                            objNotification.XMLValue = objDTO.XMLValue;
                            objNotification.CompanyIDs = objDTO.CompanyIds;
                            objNotification.RoomIDs = objDTO.RoomIds;
                            objNotification.Status = objDTO.Status;
                            objNotification.Range = objDTO.Range;
                            if (!string.IsNullOrWhiteSpace(objDTO.ScheduleRunTime))
                            {
                                objNotification.ScheduleTime = Convert.ToDateTime(objDTO.ScheduleRunTime).TimeOfDay;
                            }
                            if (UpdateSchedule && smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                            {
                                objNotification.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                objNotification.NextRunDate = null;
                            }
                        }
                        context.SaveChanges();
                        if (objDTO.lstEmailTemplateDtls != null && objDTO.lstEmailTemplateDtls.Count() > 0)
                        {

                            foreach (var item1 in objDTO.lstEmailTemplateDtls)
                            {
                                ResourceLaguage objLang = lstlangs.FirstOrDefault(t => t.Culture == item1.CultureCode);
                                EmailTemplateDetail objEmailTemplateDetail = context.EmailTemplateDetails.FirstOrDefault(t => t.EmailTemplateId == 3 && t.NotificationID == objNotification.ID && t.ResourceLaguageId == objLang.ID && t.RoomId == objDTO.RoomID && t.CompanyID == objDTO.CompanyID);
                                if (objEmailTemplateDetail != null)
                                {
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                }
                                else
                                {
                                    objEmailTemplateDetail = new EmailTemplateDetail();
                                    objEmailTemplateDetail.CompanyID = objDTO.CompanyID;
                                    objEmailTemplateDetail.Created = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.CreatedBy = objDTO.CreatedBy;
                                    objEmailTemplateDetail.EmailTemplateId = 3;
                                    objEmailTemplateDetail.ID = 0;
                                    objEmailTemplateDetail.LastUpdatedBy = objDTO.UpdatedBy;
                                    objEmailTemplateDetail.MailBodyText = item1.MailBodyText;
                                    objEmailTemplateDetail.MailSubject = item1.MailSubject;
                                    objEmailTemplateDetail.NotificationID = objNotification.ID;
                                    objEmailTemplateDetail.ResourceLaguageId = objLang.ID;
                                    objEmailTemplateDetail.RoomId = objDTO.RoomID;
                                    objEmailTemplateDetail.Updated = DateTimeUtility.DateTimeNow;
                                    objEmailTemplateDetail.NotificationID = objDTO.ID;
                                    context.EmailTemplateDetails.Add(objEmailTemplateDetail);

                                }
                            }
                            context.SaveChanges();
                        }
                        if (UpdateSchedule)
                        {
                            if (objDTO.SchedulerParams.ScheduleMode != 0 && objDTO.SchedulerParams.ScheduleMode != 5)
                            {
                                //GenetareNextRunDate(objNotification.ID, 0);
                                context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + objNotification.ID + "");
                            }
                        }
                        if (objDTO.SchedulerParams.RecalcSchedule)
                        {
                            IQueryable<Notification> lstAllNoticationForthisSchedule = context.Notifications.Where(t => t.RoomScheduleID == objDTO.SchedulerParams.ScheduleID);
                            if (lstAllNoticationForthisSchedule != null && lstAllNoticationForthisSchedule.Any())
                            {
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {

                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            ntfc.ScheduleRunTime = objDTO.ScheduleRunDateTime;
                                            ntfc.NextRunDate = null;
                                        }


                                    }
                                }
                                context.SaveChanges();
                                foreach (var ntfc in lstAllNoticationForthisSchedule)
                                {
                                    if (objNotification.ID != ntfc.ID)
                                    {
                                        if (smodes.Contains(objDTO.SchedulerParams.ScheduleMode))
                                        {
                                            //GenetareNextRunDate(objNotification.ID, 0);
                                            context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + ntfc.ID + "");
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            }
        }

        public List<NotificationDTO> GetAllSchedulesByEmailTemplate(long EmailTemplateID, long RoomID, long CompanyID, string Culture)
        {

            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.Notifications
                                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                from nm_tm in nm_tm_join.DefaultIfEmpty()
                                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                from sm_tm in sm_tm_join.DefaultIfEmpty()
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where nm.RoomId == RoomID && nm.CompanyId == CompanyID && nm.IsDeleted == false && nm.IsActive == true && nm.EmailTemplateID == EmailTemplateID && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                                select new NotificationDTO
                                {
                                    AttachmentReportIDs = nm.AttachmentReportIDs,
                                    AttachmentTypes = nm.AttachmentTypes,
                                    CompanyID = nm.CompanyId,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    EmailAddress = nm.EmailAddress,
                                    EmailTemplateID = nm.EmailTemplateID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsDeleted = nm.IsDeleted,
                                    NextRunDate = nm.NextRunDate,
                                    ReportID = nm.ReportID,
                                    RoomID = nm.RoomId,
                                    RoomScheduleID = nm.RoomScheduleID,
                                    ScheduleFor = nm.ScheduleFor,
                                    ScheduleName = sm_tm.ScheduleName,
                                    SupplierIds = nm.SupplierIds,
                                    TemplateName = nm_tm.TemplateName,
                                    Updated = nm.Updated,
                                    UpdatedBy = nm.UpdatedBy,
                                    ReportName = nm_rpt.ReportName,
                                    ReportDataSince = nm.ReportDataSince,
                                    ReportDataSelectionType = nm.ReportDataSelectionType,
                                    RoomName = rm.RoomName,
                                    CompanyName = cm.Name,
                                    FTPId = nm.FTPId,
                                    NotificationMode = nm.NotificationMode,
                                    ScheduleTime = nm.ScheduleTime,
                                    SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                    HideHeader = nm.HideHeader,
                                    ShowSignature = nm.ShowSignature,
                                    SortSequence = nm.SortSequence,
                                    XMLValue = nm.XMLValue,
                                    CompanyIds = nm.CompanyIDs,
                                    RoomIds = nm.RoomIDs,
                                    Status = nm.Status,
                                    Range = nm.Range
                                }).ToList();

                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == Culture && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = rs.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }
                    });
                }
            }

            return lstSchedules;
        }
        public List<NotificationDTO> GetAllSchedulesByReportId(long ReportID, long RoomID, long CompanyID, string Culture)
        {

            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.Notifications
                                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                from nm_tm in nm_tm_join.DefaultIfEmpty()
                                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                from sm_tm in sm_tm_join.DefaultIfEmpty()
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where nm.RoomId == RoomID && nm.CompanyId == CompanyID && nm.IsDeleted == false && nm.IsActive == true && nm.ReportID == ReportID && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                                select new NotificationDTO
                                {
                                    AttachmentReportIDs = nm.AttachmentReportIDs,
                                    AttachmentTypes = nm.AttachmentTypes,
                                    CompanyID = nm.CompanyId,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    EmailAddress = nm.EmailAddress,
                                    EmailTemplateID = nm.EmailTemplateID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsDeleted = nm.IsDeleted,
                                    NextRunDate = nm.NextRunDate,
                                    ReportID = nm.ReportID,
                                    RoomID = nm.RoomId,
                                    RoomScheduleID = nm.RoomScheduleID,
                                    ScheduleFor = nm.ScheduleFor,
                                    ScheduleName = sm_tm.ScheduleName,
                                    SupplierIds = nm.SupplierIds,
                                    TemplateName = nm_tm.TemplateName,
                                    Updated = nm.Updated,
                                    UpdatedBy = nm.UpdatedBy,
                                    ReportName = nm_rpt.ReportName,
                                    ReportDataSince = nm.ReportDataSince,
                                    ReportDataSelectionType = nm.ReportDataSelectionType,
                                    RoomName = rm.RoomName,
                                    CompanyName = cm.Name,
                                    FTPId = nm.FTPId,
                                    NotificationMode = nm.NotificationMode,
                                    ScheduleTime = nm.ScheduleTime,
                                    SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                    HideHeader = nm.HideHeader,
                                    ShowSignature = nm.ShowSignature,
                                    SortSequence = nm.SortSequence,
                                    XMLValue = nm.XMLValue,
                                    CompanyIds = nm.CompanyIDs,
                                    RoomIds = nm.RoomIDs,
                                    Status = nm.Status,
                                    Range = nm.Range
                                }).ToList();

                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == Culture && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = rs.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }
                    });
                }
            }

            return lstSchedules;
        }

        public List<NotificationDTO> GetAllSchedulesByReportName(string ReportName, long RoomID, long CompanyID, string Culture)
        {

            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.Notifications
                                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                from nm_tm in nm_tm_join.DefaultIfEmpty()
                                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                from sm_tm in sm_tm_join.DefaultIfEmpty()
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where nm.RoomId == RoomID && nm.CompanyId == CompanyID && nm.IsDeleted == false && nm.IsActive == true 
                                && nm_rpt.ReportName == ReportName && rm.IsRoomActive == true && rm.IsDeleted == false 
                                && (cm.IsDeleted ?? false) == false
                                select new NotificationDTO
                                {
                                    AttachmentReportIDs = nm.AttachmentReportIDs,
                                    AttachmentTypes = nm.AttachmentTypes,
                                    CompanyID = nm.CompanyId,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    EmailAddress = nm.EmailAddress,
                                    EmailTemplateID = nm.EmailTemplateID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsDeleted = nm.IsDeleted,
                                    NextRunDate = nm.NextRunDate,
                                    ReportID = nm.ReportID,
                                    RoomID = nm.RoomId,
                                    RoomScheduleID = nm.RoomScheduleID,
                                    ScheduleFor = nm.ScheduleFor,
                                    ScheduleName = sm_tm.ScheduleName,
                                    SupplierIds = nm.SupplierIds,
                                    TemplateName = nm_tm.TemplateName,
                                    Updated = nm.Updated,
                                    UpdatedBy = nm.UpdatedBy,
                                    ReportName = nm_rpt.ReportName,
                                    ReportDataSince = nm.ReportDataSince,
                                    ReportDataSelectionType = nm.ReportDataSelectionType,
                                    RoomName = rm.RoomName,
                                    CompanyName = cm.Name,
                                    FTPId = nm.FTPId,
                                    NotificationMode = nm.NotificationMode,
                                    ScheduleTime = nm.ScheduleTime,
                                    SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                    HideHeader = nm.HideHeader,
                                    ShowSignature = nm.ShowSignature,
                                    SortSequence = nm.SortSequence,
                                    XMLValue = nm.XMLValue,
                                    CompanyIds = nm.CompanyIDs,
                                    RoomIds = nm.RoomIDs,
                                    Status = nm.Status,
                                    Range = nm.Range
                                }).ToList();

                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == Culture && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = rs.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }
                    });
                }
            }

            return lstSchedules;
        }

        public List<NotificationDTO> GetAllSchedulesByEmailTemplateQuoteandOrder(long EmailTemplateID, long RoomID, long CompanyID, string Culture)
        {

            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID),
                    new SqlParameter("@CompanyId", CompanyID),
                    new SqlParameter("@EmailTemplateID", EmailTemplateID)};
                lstSchedules = context.Database.SqlQuery<NotificationDTO>("EXEC [GetAllSchedulesByEmailTemplateQuoteandOrder] @RoomId,@CompanyId,@EmailTemplateID", params1).ToList();


                //lstSchedules = (from nm in context.Notifications
                //                //join nn in objNewNotificationDTO.AsEnumerable() on nm.ID equals nn.ID 
                //                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                //                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                //                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                //                from nm_tm in nm_tm_join.DefaultIfEmpty()
                //                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                //                from sm_tm in sm_tm_join.DefaultIfEmpty()
                //                join rm in context.Rooms on nm.RoomId equals rm.ID
                //                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                //                where objNewNotificationDTO.AsEnumerable().Any(y => y.ID == nm.ID)&& rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                //                select new NotificationDTO
                //                {
                //                    AttachmentReportIDs = nm.AttachmentReportIDs,
                //                    AttachmentTypes = nm.AttachmentTypes,
                //                    CompanyID = nm.CompanyId,
                //                    Created = nm.Created,
                //                    CreatedBy = nm.CreatedBy,
                //                    EmailAddress = nm.EmailAddress,
                //                    EmailTemplateID = nm.EmailTemplateID,
                //                    ID = nm.ID,
                //                    IsActive = nm.IsActive,
                //                    IsDeleted = nm.IsDeleted,
                //                    NextRunDate = nm.NextRunDate,
                //                    ReportID = nm.ReportID,
                //                    RoomID = nm.RoomId,
                //                    RoomScheduleID = nm.RoomScheduleID,
                //                    ScheduleFor = nm.ScheduleFor,
                //                    ScheduleName = sm_tm.ScheduleName,
                //                    SupplierIds = nm.SupplierIds,
                //                    TemplateName = nm_tm.TemplateName,
                //                    Updated = nm.Updated,
                //                    UpdatedBy = nm.UpdatedBy,
                //                    ReportName = nm_rpt.ReportName,
                //                    ReportDataSince = nm.ReportDataSince,
                //                    ReportDataSelectionType = nm.ReportDataSelectionType,
                //                    RoomName = rm.RoomName,
                //                    CompanyName = cm.Name,
                //                    FTPId = nm.FTPId,
                //                    NotificationMode = nm.NotificationMode,
                //                    ScheduleTime = nm.ScheduleTime,
                //                    SendEmptyEmail = nm.SendEmptyEmail??false,
                //                    HideHeader = nm.HideHeader,
                //                    ShowSignature = nm.ShowSignature,
                //                    SortSequence = nm.SortSequence,
                //                    XMLValue = nm.XMLValue,
                //                    CompanyIds = nm.CompanyIDs,
                //                    RoomIds = nm.RoomIDs,
                //                    Status = nm.Status,
                //                    Range = nm.Range
                //                }).ToList();


                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                                 //join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == t.CultureCode && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = t.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }
                    });
                }
            }

            return lstSchedules;
        }
        public bool DeleteRecords(string IDs, int userid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE [Notification] SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

        public void UpdateAllScheduleAfterTZChange(TimeZoneInfo OldTimeZone, TimeZoneInfo NewTimeZone, long RoomID, long CompanyID)
        {
            short[] smodes = new short[] { 1, 2, 3, 4 };
            short[] stype = new short[] { 1, 2, 7, 8, 11 };
            List<long> ScheduleIds = new List<long>();
            List<long> eVMIScheduleIds = new List<long>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IQueryable<RoomSchedule> lstSchedules = context.RoomSchedules.Where(t => t.RoomId == RoomID && t.CompanyId == CompanyID && stype.Contains(t.ScheduleFor) && smodes.Contains(t.ScheduleMode) && t.IsScheduleActive == true && t.IsDeleted == false);
                if (lstSchedules.Any())
                {
                    foreach (var rs in lstSchedules)
                    {
                        eVMISetup objeVMISetup = context.eVMISetups.Where(t => t.Room == rs.RoomId && t.CompanyID == rs.CompanyId && t.IsDeleted == false).FirstOrDefault();
                        DateTime _scruntime = DateTime.SpecifyKind(rs.ScheduleRunTime, DateTimeKind.Unspecified);
                        DateTime _scruntimeold = TimeZoneInfo.ConvertTimeFromUtc(_scruntime, OldTimeZone);
                        //DateTime _scruntimeNew = TimeZoneInfo.ConvertTimeFromUtc(_scruntime, NewTimeZone);
                        DateTime datetimetoConsider = System.TimeZoneInfo.ConvertTime(DateTime.Now, System.TimeZoneInfo.Local, NewTimeZone);
                        if (rs.ScheduleMode != 4)
                        {
                            string strtmp = datetimetoConsider.ToShortDateString() + " " + _scruntimeold.ToString("HH:mm");
                            rs.ScheduleRunTime = Convert.ToDateTime(strtmp);
                            rs.ScheduleRunTime = TimeZoneInfo.ConvertTimeToUtc(rs.ScheduleRunTime, NewTimeZone);
                        }
                        else
                        {
                            rs.ScheduleRunTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, rs.HourlyAtWhatMinute ?? 0, 0);
                            rs.ScheduleRunTime = TimeZoneInfo.ConvertTimeToUtc(rs.ScheduleRunTime, NewTimeZone);
                        }

                        rs.NextRunDate = null;
                        if (objeVMISetup != null)
                        {
                            if (objeVMISetup.PollType == 2)
                            {
                                TimeSpan ts = Convert.ToDateTime("01:00").TimeOfDay;
                                DateTime dteVMIPoll = GeteVMINextPollDateTime(objeVMISetup, datetimetoConsider, out ts);
                                rs.ScheduleRunTime = TimeZoneInfo.ConvertTimeToUtc(dteVMIPoll, NewTimeZone);
                                rs.ScheduleTime = ts;
                            }
                        }
                        //string strtmp = _scruntimeold.ToShortDateString() + " " + _scruntimeold.SchedulerParams.ScheduleRunTime;
                        //objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                        //objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                        if (rs.ScheduleFor == (short)eVMIScheduleFor.eVMISchedule)
                        {
                           eVMIScheduleIds.Add(rs.ScheduleID);
                        }
                        else 
                        {
                            ScheduleIds.Add(rs.ScheduleID);
                        }
                        
                    }
                    context.SaveChanges();
                }

                if (ScheduleIds.Count() > 0)
                {
                    foreach (var item in ScheduleIds)
                    {
                        context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + item + "");
                        //context.Database.ExecuteSqlCommand("EXEC GetNexteVMIRunTime " + item + "");
                    }                    
                }

                if (eVMIScheduleIds != null && eVMIScheduleIds.Any() && eVMIScheduleIds.Count() > 0)
                {
                    foreach (var item in eVMIScheduleIds)
                    {
                        //context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + item + "");
                        context.Database.ExecuteSqlCommand("EXEC GetNexteVMIRunTime " + item + "");
                    }
                }

                int[] sitype = new int[] { 5, 6 };
                ScheduleIds = new List<long>();

                IQueryable<Notification> lstNotifications = (from nm in context.Notifications
                                                             join rs in context.RoomSchedules on nm.RoomScheduleID equals rs.ScheduleID
                                                             where nm.RoomId == RoomID && nm.CompanyId == CompanyID && sitype.Contains(nm.ScheduleFor) && nm.IsActive == true && nm.IsDeleted == false && smodes.Contains(rs.ScheduleMode)
                                                             select nm);

                //context.Notifications.Where(t => t.RoomId == RoomID && t.CompanyId == CompanyID && sitype.Contains(t.ScheduleFor) && t.IsActive == true && t.IsDeleted == false);

                if (lstNotifications.Any())
                {
                    foreach (var rs in lstNotifications)
                    {
                        if (rs.ScheduleRunTime != null)
                        {

                            RoomSchedule rms = context.RoomSchedules.FirstOrDefault(t => t.ScheduleID == rs.RoomScheduleID);
                            DateTime _scruntime = DateTime.SpecifyKind(rs.ScheduleRunTime.Value, DateTimeKind.Unspecified);
                            DateTime _scruntimeold = TimeZoneInfo.ConvertTimeFromUtc(_scruntime, OldTimeZone);
                            DateTime datetimetoConsider = System.TimeZoneInfo.ConvertTime(DateTime.Now, System.TimeZoneInfo.Local, NewTimeZone);
                            if (rms.ScheduleMode != 4)
                            {
                                string strtmp = datetimetoConsider.ToShortDateString() + " " + _scruntimeold.ToString("HH:mm");
                                rs.ScheduleRunTime = Convert.ToDateTime(strtmp);
                                rs.ScheduleRunTime = TimeZoneInfo.ConvertTimeToUtc(rs.ScheduleRunTime.Value, NewTimeZone);
                            }
                            else
                            {
                                rs.ScheduleRunTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, rms.HourlyAtWhatMinute ?? 0, 0);
                                rs.ScheduleRunTime = TimeZoneInfo.ConvertTimeToUtc(rs.ScheduleRunTime.Value, NewTimeZone);
                            }

                            rs.NextRunDate = null;
                            //string strtmp = _scruntimeold.ToShortDateString() + " " + _scruntimeold.SchedulerParams.ScheduleRunTime;
                            //objDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                            //objDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objDTO.ScheduleRunDateTime, objDTO.RoomID, objDTO.CompanyID, objDTO.CreatedBy) ?? objDTO.SchedulerParams.ScheduleRunDateTime;
                            ScheduleIds.Add(rs.ID);
                        }
                    }
                    context.SaveChanges();
                }

                if (ScheduleIds.Count() > 0)
                {
                    foreach (var item in ScheduleIds)
                    {
                        context.Database.ExecuteSqlCommand("EXEC GetNextReportRunTimeNotification " + item + "");
                    }
                }

            }
        }

        public DateTime GeteVMINextPollDateTime(eVMISetup objeVMISetup, DateTime datetimetoConsider, out TimeSpan TS)
        {
            //DateTime dtOnlyConsider = datetimetoConsider.ToShortDateString();
            TimeSpan TimetoConsider = datetimetoConsider.TimeOfDay;
            DateTime? datetimetoConsider_New = null;
            TS = Convert.ToDateTime("01:00").TimeOfDay;
            List<TimeSpan> lstPollTime = new List<TimeSpan>();
            if (objeVMISetup.PollTime1 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime1);
            if (objeVMISetup.PollTime2 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime2);
            if (objeVMISetup.PollTime3 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime3);
            if (objeVMISetup.PollTime4 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime4);
            if (objeVMISetup.PollTime5 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime5);
            if (objeVMISetup.PollTime6 != null)
                lstPollTime.Add((TimeSpan)objeVMISetup.PollTime6);

            if (lstPollTime != null && lstPollTime.Count > 0)
            {
                lstPollTime = lstPollTime.OrderBy(x => x).ToList();
                foreach (TimeSpan item in lstPollTime)
                {
                    if (TimetoConsider > item)
                    {
                        string strtmp = datetimetoConsider.ToShortDateString() + " " + item;
                        datetimetoConsider_New = Convert.ToDateTime(strtmp);
                        TS = item;
                        break;
                    }
                }
                if (datetimetoConsider_New == null)
                {
                    datetimetoConsider_New = Convert.ToDateTime(datetimetoConsider.ToShortDateString() + " " + lstPollTime.FirstOrDefault());
                    TS = lstPollTime.FirstOrDefault();
                }
            }
            if (datetimetoConsider_New != null)
                datetimetoConsider = Convert.ToDateTime(datetimetoConsider_New);

            return datetimetoConsider;
        }

        public List<NotificationDTO> GetAllDeadSchedules()
        {
            long[] Schedulemodes = new long[] { 1, 2, 3, 4 };
            DateTime dtpast = DateTime.UtcNow.AddMinutes(-30);
            List<NotificationDTO> lstNotifications = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstNotifications = (from u in context.Notifications
                                    join rs in context.RoomSchedules on u.RoomScheduleID equals rs.ScheduleID
                                    where u.IsDeleted == false && u.IsActive == true && (u.NextRunDate <= dtpast || u.NextRunDate == null) && Schedulemodes.Contains(rs.ScheduleMode)
                                    select new NotificationDTO
                                    {
                                        AttachmentReportIDs = u.AttachmentReportIDs,
                                        AttachmentTypes = u.AttachmentTypes,
                                        CompanyID = u.CompanyId,
                                        Created = u.Created,
                                        CreatedBy = u.CreatedBy,
                                        EmailAddress = u.EmailAddress,
                                        EmailTemplateID = u.EmailTemplateID,
                                        ID = u.ID,
                                        IsActive = u.IsActive,
                                        IsDeleted = u.IsDeleted,
                                        NextRunDate = u.NextRunDate,
                                        ReportID = u.ReportID,
                                        RoomID = u.RoomId,
                                        RoomScheduleID = u.RoomScheduleID,
                                        ScheduleFor = u.ScheduleFor,
                                        SupplierIds = u.SupplierIds,
                                        Updated = u.Updated,
                                        UpdatedBy = u.UpdatedBy,
                                        NotificationMode = u.NotificationMode,
                                        FTPId = u.FTPId,
                                        SendEmptyEmail = u.SendEmptyEmail ?? false,
                                        HideHeader = u.HideHeader,
                                        ShowSignature = u.ShowSignature,
                                        SortSequence = u.SortSequence,
                                        XMLValue = u.XMLValue,
                                        CompanyIds = u.CompanyIDs,
                                        RoomIds = u.RoomIDs,
                                        Status = u.Status,
                                        Range = u.Range
                                    }).ToList();
            }
            return lstNotifications;
        }

        public void DisableNotification(long NOtificationID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Notification objNotes = context.Notifications.FirstOrDefault(t => t.ID == NOtificationID);
                if (objNotes != null)
                {
                    objNotes.IsActive = false;
                    context.SaveChanges();
                }
            }
        }

        public void GenetareNextRunDate(long NotificationID, long RoomScheduleID)
        {
            NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
            NotificationDTO objNotificationDTO = new NotificationDTO();
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            DateTime CalcNextRunDate = DateTime.UtcNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (NotificationID > 0)
                {
                    objNotificationDTO = GetNotifiactionByID(NotificationID);
                    if (objNotificationDTO != null && objNotificationDTO.SchedulerParams != null && new short[] { 1, 2, 3, 4 }.Contains(objNotificationDTO.SchedulerParams.ScheduleMode))
                    {

                        switch (objNotificationDTO.SchedulerParams.ScheduleMode)
                        {
                            case 1: // Daily
                                if (objNotificationDTO.SchedulerParams.DailyRecurringType == 1)
                                {
                                    if (objNotificationDTO.NextRunDate == null)
                                    {
                                        DateTime CurrentDateTime = DateTimeUtility.GetCurrentDatetimeByTimeZone(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                                        DateTime SelectedTime = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, objNotificationDTO.ScheduleTime.Value.Hours, objNotificationDTO.ScheduleTime.Value.Minutes, 0);
                                        DateTime SelectedTimeUTC = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, SelectedTime);
                                        if (SelectedTimeUTC < DateTime.UtcNow)
                                        {
                                            SelectedTimeUTC = SelectedTimeUTC.AddDays(objNotificationDTO.SchedulerParams.DailyRecurringDays);
                                        }
                                        CalcNextRunDate = SelectedTimeUTC;

                                    }
                                    else
                                    {
                                        CalcNextRunDate = objNotificationDTO.NextRunDate.Value.AddDays(objNotificationDTO.SchedulerParams.DailyRecurringDays);
                                    }
                                }
                                else if (objNotificationDTO.SchedulerParams.DailyRecurringType == 2)
                                {
                                    if (objNotificationDTO.NextRunDate == null)
                                    {
                                        DateTime CurrentDateTime = DateTimeUtility.GetCurrentDatetimeByTimeZone(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                                        DateTime SelectedTime = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, objNotificationDTO.ScheduleTime.Value.Hours, objNotificationDTO.ScheduleTime.Value.Minutes, 0);
                                        DateTime SelectedTimeUTC = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, SelectedTime);
                                        if (SelectedTimeUTC.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            SelectedTimeUTC = SelectedTimeUTC.AddDays(2);
                                        }
                                        else if (SelectedTimeUTC.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            SelectedTimeUTC = SelectedTimeUTC.AddDays(1);
                                        }
                                        CalcNextRunDate = SelectedTimeUTC;
                                    }
                                    else
                                    {
                                        CalcNextRunDate = objNotificationDTO.NextRunDate.Value.AddDays(1);
                                        if (CalcNextRunDate.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            CalcNextRunDate = CalcNextRunDate.AddDays(2);
                                        }
                                        else if (CalcNextRunDate.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            CalcNextRunDate = CalcNextRunDate.AddDays(1);
                                        }
                                    }
                                }
                                break;
                            case 2: // Weekly
                                if (objNotificationDTO.NextRunDate == null)
                                {
                                    DateTime CurrentDateTime = DateTimeUtility.GetCurrentDatetimeByTimeZone(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                                    DateTime SelectedTime = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, objNotificationDTO.ScheduleTime.Value.Hours, objNotificationDTO.ScheduleTime.Value.Minutes, 0);
                                    DateTime SelectedTimeUTC = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, SelectedTime);
                                    DateTime FirstOccurencedateToime;
                                    int delta = DayOfWeek.Monday - SelectedTime.DayOfWeek;
                                    DateTime mondayDate = SelectedTime.AddDays(delta);
                                    List<DateTime> lstdayofWeek = new List<DateTime>();
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnMonday)
                                    {
                                        lstdayofWeek.Add(mondayDate);
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnTuesday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(1));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnWednesday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(2));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnThursday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(3));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnFriday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(4));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnSaturday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(5));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnSunday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(6));
                                    }
                                    if (lstdayofWeek.Any(t => t >= CurrentDateTime))
                                    {
                                        FirstOccurencedateToime = lstdayofWeek.Where(t => t >= CurrentDateTime).FirstOrDefault();
                                    }
                                    else
                                    {
                                        FirstOccurencedateToime = lstdayofWeek.Where(t => t.AddDays(7) >= CurrentDateTime).FirstOrDefault().AddDays(7);
                                    }

                                    CalcNextRunDate = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, FirstOccurencedateToime);
                                }
                                else
                                {
                                    DateTime SelectedTime = DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, objNotificationDTO.NextRunDate.Value);
                                    DateTime FirstOccurencedateToime;
                                    int delta = DayOfWeek.Monday - SelectedTime.DayOfWeek;
                                    DateTime mondayDate = SelectedTime.AddDays(delta);
                                    List<DateTime> lstdayofWeek = new List<DateTime>();
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnMonday)
                                    {
                                        lstdayofWeek.Add(mondayDate);
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnTuesday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(1));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnWednesday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(2));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnThursday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(3));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnFriday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(4));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnSaturday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(5));
                                    }
                                    if (objNotificationDTO.SchedulerParams.WeeklyOnSunday)
                                    {
                                        lstdayofWeek.Add(mondayDate.AddDays(6));
                                    }
                                    if (lstdayofWeek.Any(t => t >= objNotificationDTO.NextRunDate.Value))
                                    {
                                        FirstOccurencedateToime = lstdayofWeek.Where(t => t >= objNotificationDTO.NextRunDate.Value).FirstOrDefault();
                                    }
                                    else
                                    {
                                        FirstOccurencedateToime = lstdayofWeek.Where(t => t.AddDays(7) >= objNotificationDTO.NextRunDate.Value).FirstOrDefault().AddDays(7);
                                    }

                                    CalcNextRunDate = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, FirstOccurencedateToime);
                                }
                                break;
                            case 3: // Monthly
                                break;
                            case 4: // Hourly
                                if (objNotificationDTO.NextRunDate == null)
                                {
                                    DateTime CurrentDateTime = DateTimeUtility.GetCurrentDatetimeByTimeZone(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName);
                                    DateTime SelectedTime = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, CurrentDateTime.Day, CurrentDateTime.Hour, objNotificationDTO.SchedulerParams.HourlyAtWhatMinute, 0);
                                    DateTime SelectedTimeUTC = DateTimeUtility.ConvertDateToUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, SelectedTime);
                                    if (SelectedTimeUTC < DateTime.UtcNow)
                                    {
                                        SelectedTimeUTC = SelectedTimeUTC.AddHours(1);
                                    }
                                    CalcNextRunDate = SelectedTimeUTC;

                                }
                                else
                                {
                                    CalcNextRunDate = objNotificationDTO.NextRunDate.Value.AddHours(objNotificationDTO.SchedulerParams.HourlyRecurringHours);
                                }
                                break;

                        }
                        Notification obNotification = context.Notifications.FirstOrDefault(t => t.ID == objNotificationDTO.ID);
                        if (obNotification != null)
                        {
                            obNotification.NextRunDate = CalcNextRunDate;
                            context.SaveChanges();
                        }
                    }


                }
                if (RoomScheduleID > 0)
                {

                }
            }
        }

        public void UpdateNotificationNextRunDate(long ID, DateTime? NextRunDate, double? MinutesToAdd)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Notification objNotification = context.Notifications.FirstOrDefault(t => t.ID == ID);
                DateTime? nestrunfDateToupdate = null;
                if (NextRunDate.HasValue)
                {
                    nestrunfDateToupdate = NextRunDate;
                }
                else
                {
                    if (objNotification.NextRunDate.HasValue)
                    {
                        nestrunfDateToupdate = objNotification.NextRunDate.Value.AddMinutes(MinutesToAdd ?? 5);
                    }
                }
                if (objNotification != null)
                {
                    objNotification.NextRunDate = nestrunfDateToupdate;
                    context.SaveChanges();
                }
            }
        }

        public void UpdateNotificationNextRunDate(long ID, DateTime? NextRunDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Notification objNotification = context.Notifications.FirstOrDefault(t => t.ID == ID);
                if (objNotification != null)
                {
                    objNotification.NextRunDate = NextRunDate;
                    context.SaveChanges();
                }
            }
        }
        public List<KeyValDTO> GetReportToken(long ReportID)
        {
            List<KeyValDTO> objKeyValDTO = new List<KeyValDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objKeyValDTO = (from u in context.EmailTokenDetails
                                join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                where u.ReportID == ReportID
                                select new KeyValDTO
                                {
                                    key = rs.Token,
                                    value = rs.Token
                                }).ToList();
                var CommonToken = (from u in context.EmailTokenDetails
                                   join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                   where (u.ReportID ?? 0) == 0 && (u.TemplateID ?? 0) == 0
                                   select new KeyValDTO
                                   {
                                       key = rs.Token,
                                       value = rs.Token
                                   }).ToList();
                objKeyValDTO = objKeyValDTO.Union(CommonToken).OrderBy(i => i.value).Distinct().ToList();
                //objKeyValDTO = objKeyValDTO.AsEnumerable().Distinct().ToList();
                return objKeyValDTO;

            }

        }
        public List<KeyValDTO> GetAlertToken(long TemplateID)
        {
            List<KeyValDTO> objKeyValDTO = new List<KeyValDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objKeyValDTO = (from u in context.EmailTokenDetails
                                join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                where u.TemplateID == TemplateID
                                select new KeyValDTO
                                {
                                    key = rs.Token,
                                    value = rs.Token
                                }).ToList();
                var CommonToken = (from u in context.EmailTokenDetails
                                   join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                   where (u.ReportID ?? 0) == 0 && (u.TemplateID ?? 0) == 0
                                   select new KeyValDTO
                                   {
                                       key = rs.Token,
                                       value = rs.Token
                                   }).ToList();
                objKeyValDTO = objKeyValDTO.Union(CommonToken).OrderBy(i => i.value).Distinct().ToList();
                //objKeyValDTO = objKeyValDTO.AsEnumerable().Distinct().ToList();
                return objKeyValDTO;

            }

        }

        public List<KeyValDTO> GetAlertToken(long TemplateID, long ReportID)
        {
            List<KeyValDTO> objKeyValDTO = new List<KeyValDTO>();
            List<KeyValDTO> objKeyValReportDTO = new List<KeyValDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objKeyValDTO = (from u in context.EmailTokenDetails
                                join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                where u.TemplateID == TemplateID
                                select new KeyValDTO
                                {
                                    key = rs.Token,
                                    value = rs.Token
                                }).ToList();

                objKeyValReportDTO = (from u in context.EmailTokenDetails
                                      join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                      where u.ReportID == ReportID
                                      select new KeyValDTO
                                      {
                                          key = rs.Token,
                                          value = rs.Token
                                      }).ToList();

                objKeyValDTO = objKeyValDTO.Union(objKeyValReportDTO).OrderBy(i => i.value).Distinct().ToList();

                var CommonToken = (from u in context.EmailTokenDetails
                                   join rs in context.EmailTokenMasters on u.TokenID equals rs.Id
                                   where (u.ReportID ?? 0) == 0 && (u.TemplateID ?? 0) == 0
                                   select new KeyValDTO
                                   {
                                       key = rs.Token,
                                       value = rs.Token
                                   }).ToList();
                objKeyValDTO = objKeyValDTO.Union(CommonToken).OrderBy(i => i.value).Distinct().ToList();
                //objKeyValDTO = objKeyValDTO.AsEnumerable().Distinct().ToList();
                return objKeyValDTO;

            }

        }


        public List<NotificationDTO> GetNotificationsByIds(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationDTO>("EXEC GetNotificationByIDs @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<ReportBuilderDTO> GetReportsByIDs(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportBuilderDTO>("EXEC [GetReportsByIDs] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<SchedulerDTO> GetRoomSchedulesByIDs(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SchedulerDTO>("EXEC [GetRoomSchedulesByIDs] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<EmailTemplateDTO> GetEmailTemplatesByIDs(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EmailTemplateDTO>("EXEC [GetEmailTemplatesByIDs] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<EmailTemplateDetailDTO> GetEmailTemplateDetailsByNotIds(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EmailTemplateDetailDTO>("EXEC [GetEmailTemplateDetailsByNotIds] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<eTurnsRegionInfo> GetRegionSettingsById(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eTurnsRegionInfo>("EXEC [GetRegionSettingsById] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<FTPMasterDTO> GetFTPDetailsByIds(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FTPMasterDTO>("EXEC [GetFTPDetailsByIds] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }

        public List<CycleCountSettingDTO> GetCountSettingsByIds(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CycleCountSettingDTO>("EXEC GetCountSettingsByIds @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }

        public List<RoomScheduleDetailDTO> GetSchedulesByIds(string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", Ids ?? (object)DBNull.Value), new SqlParameter("@RoomID", (object)DBNull.Value), new SqlParameter("@CompanyID", (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<RoomScheduleDetailDTO>("EXEC [GetRoomSchedulesByIDs] @IDs, @RoomID, @CompanyID", params1).ToList();
            }
        }
        public List<DailyScheduleRunHistoryDTO> InsertDailyScheduleRunHistory(DailyScheduleRunHistoryDTO objDailyScheduleRunHistoryDTO, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] {
new SqlParameter("@RecGUID", objDailyScheduleRunHistoryDTO.RecGUID ),
new SqlParameter("@EnterpriseID", objDailyScheduleRunHistoryDTO.EnterpriseID ?? (object)DBNull.Value),
new SqlParameter("@CompanyID", objDailyScheduleRunHistoryDTO.CompanyID ?? (object)DBNull.Value),
new SqlParameter("@RoomID", objDailyScheduleRunHistoryDTO.RoomID ?? (object)DBNull.Value),
new SqlParameter("@ScheduleID", objDailyScheduleRunHistoryDTO.ScheduleID ?? (object)DBNull.Value),
new SqlParameter("@MasterScheduleID", objDailyScheduleRunHistoryDTO.MasterScheduleID ?? (object)DBNull.Value),
new SqlParameter("@IsTurnsCalcStarted", objDailyScheduleRunHistoryDTO.IsTurnsCalcStarted ),
new SqlParameter("@IsOrderNightlyCalcStarted", objDailyScheduleRunHistoryDTO.IsOrderNightlyCalcStarted ),
new SqlParameter("@IsDailyItemLocStarted", objDailyScheduleRunHistoryDTO.IsDailyItemLocStarted ),
new SqlParameter("@TurnsCalcStartedTime", objDailyScheduleRunHistoryDTO.TurnsCalcStartedTime ?? (object)DBNull.Value),
new SqlParameter("@OrderNightlyCalcStartedTime", objDailyScheduleRunHistoryDTO.OrderNightlyCalcStartedTime ?? (object)DBNull.Value),
new SqlParameter("@DailyItemLocStartedTime", objDailyScheduleRunHistoryDTO.DailyItemLocStartedTime ?? (object)DBNull.Value),
new SqlParameter("@IsTurnsCalcCompleted", objDailyScheduleRunHistoryDTO.IsTurnsCalcCompleted ),
new SqlParameter("@IsOrderNightlyCalcCompleted", objDailyScheduleRunHistoryDTO.IsOrderNightlyCalcCompleted ),
new SqlParameter("@IsDailyItemLocCompleted", objDailyScheduleRunHistoryDTO.IsDailyItemLocCompleted ),
new SqlParameter("@TurnsCalcCompletedTime", objDailyScheduleRunHistoryDTO.TurnsCalcCompletedTime ?? (object)DBNull.Value),
new SqlParameter("@OrderNightlyCalcCompletedTime", objDailyScheduleRunHistoryDTO.OrderNightlyCalcCompletedTime ?? (object)DBNull.Value),
new SqlParameter("@DailyItemLocCompletedTime", objDailyScheduleRunHistoryDTO.DailyItemLocCompletedTime ?? (object)DBNull.Value),
new SqlParameter("@NextRunDate", objDailyScheduleRunHistoryDTO.NextRunDate ?? (object)DBNull.Value),
new SqlParameter("@Created", objDailyScheduleRunHistoryDTO.Created),
new SqlParameter("@LastUpdated", objDailyScheduleRunHistoryDTO.LastUpdated ) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DailyScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[InsertDailyScheduleRunHistory] @RecGUID,@EnterpriseID,@CompanyID,@RoomID,@ScheduleID,@MasterScheduleID,@IsTurnsCalcStarted,@IsOrderNightlyCalcStarted,@IsDailyItemLocStarted,@TurnsCalcStartedTime,@OrderNightlyCalcStartedTime,@DailyItemLocStartedTime,@IsTurnsCalcCompleted,@IsOrderNightlyCalcCompleted,@IsDailyItemLocCompleted,@TurnsCalcCompletedTime,@OrderNightlyCalcCompletedTime,@DailyItemLocCompletedTime,@NextRunDate,@Created,@LastUpdated", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> InsertOrderScheduleRunHistory(OrderScheduleRunHistoryDTO objOrderScheduleRunHistoryDTO, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] {
new SqlParameter("@RecGUID", objOrderScheduleRunHistoryDTO.RecGUID ),
new SqlParameter("@EnterpriseID", objOrderScheduleRunHistoryDTO.EnterpriseID ?? (object)DBNull.Value),
new SqlParameter("@CompanyID", objOrderScheduleRunHistoryDTO.CompanyID ?? (object)DBNull.Value),
new SqlParameter("@RoomID", objOrderScheduleRunHistoryDTO.RoomID ?? (object)DBNull.Value),
new SqlParameter("@ScheduleID", objOrderScheduleRunHistoryDTO.ScheduleID ?? (object)DBNull.Value),
new SqlParameter("@MasterScheduleID", objOrderScheduleRunHistoryDTO.MasterScheduleID ?? (object)DBNull.Value),
new SqlParameter("@NextRunDate", objOrderScheduleRunHistoryDTO.NextRunDate ?? (object)DBNull.Value),
new SqlParameter("@Created", objOrderScheduleRunHistoryDTO.Created),
new SqlParameter("@LastUpdated", objOrderScheduleRunHistoryDTO.LastUpdated ),
new SqlParameter("@IsGeneralScheduleStarted", objOrderScheduleRunHistoryDTO.IsGeneralScheduleStarted),
new SqlParameter("@IsGeneralScheduleCompleted", objOrderScheduleRunHistoryDTO.IsGeneralScheduleCompleted ),
new SqlParameter("@GeneralScheduleStartedTime", objOrderScheduleRunHistoryDTO.GeneralScheduleStartedTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleCompletedTime", objOrderScheduleRunHistoryDTO.GeneralScheduleCompletedTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleExceptionTime", objOrderScheduleRunHistoryDTO.GeneralScheduleExceptionTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleException", objOrderScheduleRunHistoryDTO.GeneralScheduleException ?? (object)DBNull.Value),
new SqlParameter("@ScheduleFor", objOrderScheduleRunHistoryDTO.ScheduleFor ?? (object)DBNull.Value),

 };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[InsertOrderScheduleRunHistory] @RecGUID,@EnterpriseID,@CompanyID,@RoomID,@ScheduleID,@MasterScheduleID,@NextRunDate,@Created,@LastUpdated,@IsGeneralScheduleStarted,@IsGeneralScheduleCompleted,@GeneralScheduleStartedTime,@GeneralScheduleCompletedTime,@GeneralScheduleExceptionTime,@GeneralScheduleException,@ScheduleFor", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> InserteVMIScheduleRunHistory(eVMIScheduleRunHistoryDTO objOrderScheduleRunHistoryDTO, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] {
new SqlParameter("@RecGUID", objOrderScheduleRunHistoryDTO.RecGUID ),
new SqlParameter("@EnterpriseID", objOrderScheduleRunHistoryDTO.EnterpriseID ?? (object)DBNull.Value),
new SqlParameter("@CompanyID", objOrderScheduleRunHistoryDTO.CompanyID ?? (object)DBNull.Value),
new SqlParameter("@RoomID", objOrderScheduleRunHistoryDTO.RoomID ?? (object)DBNull.Value),
new SqlParameter("@eVMISetupID", objOrderScheduleRunHistoryDTO.eVMISetupID ?? (object)DBNull.Value),
new SqlParameter("@MastereVMISetupID", objOrderScheduleRunHistoryDTO.MastereVMISetupID ?? (object)DBNull.Value),
new SqlParameter("@NextPollDate", objOrderScheduleRunHistoryDTO.NextPollDate ?? (object)DBNull.Value),
new SqlParameter("@Created", objOrderScheduleRunHistoryDTO.Created),
new SqlParameter("@LastUpdated", objOrderScheduleRunHistoryDTO.LastUpdated ),
new SqlParameter("@IsGeneralScheduleStarted", objOrderScheduleRunHistoryDTO.IsGeneralScheduleStarted),
new SqlParameter("@IsGeneralScheduleCompleted", objOrderScheduleRunHistoryDTO.IsGeneralScheduleCompleted ),
new SqlParameter("@GeneralScheduleStartedTime", objOrderScheduleRunHistoryDTO.GeneralScheduleStartedTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleCompletedTime", objOrderScheduleRunHistoryDTO.GeneralScheduleCompletedTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleExceptionTime", objOrderScheduleRunHistoryDTO.GeneralScheduleExceptionTime ?? (object)DBNull.Value),
new SqlParameter("@GeneralScheduleException", objOrderScheduleRunHistoryDTO.GeneralScheduleException ?? (object)DBNull.Value),
new SqlParameter("@ScheduleFor", objOrderScheduleRunHistoryDTO.ScheduleFor ?? (object)DBNull.Value),

 };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[InserteVMIScheduleRunHistory] @RecGUID,@EnterpriseID,@CompanyID,@RoomID,@eVMISetupID,@MastereVMISetupID,@NextPollDate,@Created,@LastUpdated,@IsGeneralScheduleStarted,@IsGeneralScheduleCompleted,@GeneralScheduleStartedTime,@GeneralScheduleCompletedTime,@GeneralScheduleExceptionTime,@GeneralScheduleException,@ScheduleFor", params1).ToList();
            }
        }
        public List<DailyScheduleRunHistoryDTO> GetRecordToRunSchedule(int OperationType, int NumberOfInstance, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DailyScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordToRunSchedule] @OperationType,@NumberOfInstance", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> GetRecordToRunOrderSchedule(int OperationType, int NumberOfInstance, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordToRunOrderSchedule] @OperationType,@NumberOfInstance", params1).ToList();
            }
        }
        public List<eVMIScheduleRunHistoryDTO> GetRecordToRuneVMISchedule(int OperationType, int NumberOfInstance, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eVMIScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordToRuneVMISchedule] @OperationType,@NumberOfInstance", params1).ToList();
            }
        }
        public List<DailyScheduleRunHistoryDTO> SetCompletedDailyScheduleRunHistory(int OperationType, long ID, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DailyScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[SetCompletedDailyScheduleRunHistory] @OperationType,@ID", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> SetCompletedOrderScheduleRunHistory(int OperationType, long ID, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[SetCompletedOrderScheduleRunHistory] @OperationType,@ID", params1).ToList();
            }
        }
        public List<eVMIScheduleRunHistoryDTO> SetCompletedeVMIScheduleRunHistory(int OperationType, long ID, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eVMIScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[SetCompletedeVMIScheduleRunHistory] @OperationType,@ID", params1).ToList();
            }
        }

        public List<DailyScheduleRunHistoryDTO> UpdateErrorForSchedule(int OperationType, long ID, string ExceptionDetails, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID), new SqlParameter("@ExceptionDetails", (ExceptionDetails ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DailyScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[UpdateErrorForSchedule] @OperationType,@ID,@ExceptionDetails", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> UpdateErrorForOrderSchedule(int OperationType, long ID, string ExceptionDetails, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID), new SqlParameter("@ExceptionDetails", (ExceptionDetails ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[UpdateErrorForOrderSchedule] @OperationType,@ID,@ExceptionDetails", params1).ToList();
            }
        }

        public List<ReportAlertScheduleRunHistoryDTO> InsertAlertCountScheduleHistory(ReportAlertScheduleRunHistoryDTO objReportAlertScheduleRunHistoryDTO, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] {
                new SqlParameter("@RecGUID", objReportAlertScheduleRunHistoryDTO.RecGUID ),
new SqlParameter("@ScheduleFor", objReportAlertScheduleRunHistoryDTO.ScheduleFor?? (object)DBNull.Value ),
new SqlParameter("@AlertReportScheduleAttempt", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleAttempt?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleAttempt", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleAttempt?? (object)DBNull.Value ),
new SqlParameter("@NextRunDate", objReportAlertScheduleRunHistoryDTO.NextRunDate?? (object)DBNull.Value ),
new SqlParameter("@Created", objReportAlertScheduleRunHistoryDTO.Created ),
new SqlParameter("@LastUpdated", objReportAlertScheduleRunHistoryDTO.LastUpdated ),
new SqlParameter("@AlertReportScheduleStartedTime", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleStartedTime?? (object)DBNull.Value ),
new SqlParameter("@AlertReportScheduleCompletedTime", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleCompletedTime?? (object)DBNull.Value ),
new SqlParameter("@AlertReportScheduleExceptionTime", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleExceptionTime?? (object)DBNull.Value ),
new SqlParameter("@AlertReportScheduleAttemptTime", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleAttemptTime?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleStartedTime", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleStartedTime?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleCompletedTime", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleCompletedTime?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleExceptionTime", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleExceptionTime?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleAttemptTime", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleAttemptTime?? (object)DBNull.Value ),
new SqlParameter("@IsAlertReportScheduleStarted", objReportAlertScheduleRunHistoryDTO.IsAlertReportScheduleStarted ),
new SqlParameter("@IsAlertReportScheduleCompleted", objReportAlertScheduleRunHistoryDTO.IsAlertReportScheduleCompleted ),
new SqlParameter("@IsCycleCountScheduleStarted", objReportAlertScheduleRunHistoryDTO.IsCycleCountScheduleStarted ),
new SqlParameter("@IsCycleCountScheduleCompleted", objReportAlertScheduleRunHistoryDTO.IsCycleCountScheduleCompleted ),
new SqlParameter("@ID", objReportAlertScheduleRunHistoryDTO.ID ),
new SqlParameter("@EnterpriseID", objReportAlertScheduleRunHistoryDTO.EnterpriseID?? (object)DBNull.Value ),
new SqlParameter("@CompanyID", objReportAlertScheduleRunHistoryDTO.CompanyID?? (object)DBNull.Value ),
new SqlParameter("@RoomID", objReportAlertScheduleRunHistoryDTO.RoomID?? (object)DBNull.Value ),
new SqlParameter("@ScheduleID", objReportAlertScheduleRunHistoryDTO.ScheduleID?? (object)DBNull.Value ),
new SqlParameter("@NotificationID", objReportAlertScheduleRunHistoryDTO.NotificationID?? (object)DBNull.Value ),
new SqlParameter("@MasterScheduleID", objReportAlertScheduleRunHistoryDTO.MasterScheduleID?? (object)DBNull.Value ),
new SqlParameter("@MasterNotificationID", objReportAlertScheduleRunHistoryDTO.MasterNotificationID?? (object)DBNull.Value ),
new SqlParameter("@CycleCountSettingID", objReportAlertScheduleRunHistoryDTO.CycleCountSettingID?? (object)DBNull.Value ),
new SqlParameter("@MasterCycleCountSettingID", objReportAlertScheduleRunHistoryDTO.MasterCycleCountSettingID?? (object)DBNull.Value ),
new SqlParameter("@AlertReportScheduleException", objReportAlertScheduleRunHistoryDTO.AlertReportScheduleException?? (object)DBNull.Value ),
new SqlParameter("@CycleCountScheduleException", objReportAlertScheduleRunHistoryDTO.CycleCountScheduleException?? (object)DBNull.Value ),
new SqlParameter("@ExternalFilter", objReportAlertScheduleRunHistoryDTO.DataGuids?? (object)DBNull.Value )
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportAlertScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[InsertAlertCountScheduleHistory] @RecGUID,@EnterpriseID,@CompanyID,@RoomID,@ScheduleID,@NotificationID,@MasterScheduleID,@MasterNotificationID,@NextRunDate,@Created,@LastUpdated,@ScheduleFor,@IsAlertReportScheduleStarted,@IsAlertReportScheduleCompleted,@AlertReportScheduleStartedTime,@AlertReportScheduleCompletedTime,@AlertReportScheduleExceptionTime,@AlertReportScheduleException,@AlertReportScheduleAttempt,@AlertReportScheduleAttemptTime,@IsCycleCountScheduleStarted,@IsCycleCountScheduleCompleted,@CycleCountScheduleStartedTime,@CycleCountScheduleCompletedTime,@CycleCountScheduleExceptionTime,@CycleCountScheduleException,@CycleCountScheduleAttempt,@CycleCountScheduleAttemptTime,@CycleCountSettingID,@MasterCycleCountSettingID,@ExternalFilter", params1).ToList();
            }
        }

        public List<ReportAlertScheduleRunHistoryDTO> GetRecordsAlertCountSchedule(int OperationType, int NumberOfInstance, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { 
                new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance)
            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportAlertScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordsAlertCountSchedule] @OperationType,@NumberOfInstance", params1).ToList();
            }
        }

        public List<ReportAlertScheduleRunHistoryDTO> SetCompletedAlertCountScheduleRunHistory(int OperationType, long ID, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportAlertScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[SetCompletedAlertCountScheduleRunHistory] @OperationType,@ID", params1).ToList();
            }
        }

        public List<ReportAlertScheduleRunHistoryDTO> UpdateErrorForAlertReportSchedule(int OperationType, long ID, string ExceptionDetails, string ScheduleDBName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@ID", ID), new SqlParameter("@ExceptionDetails", (ExceptionDetails ?? (object)DBNull.Value)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportAlertScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[UpdateErrorForAlertReportSchedule] @OperationType,@ID,@ExceptionDetails", params1).ToList();
            }
        }

        public void CleanDailyScheduleRunHistory(string ScheduleDBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[CleanDailyScheduleRunHistory]");
            }
        }

        public void CleanOrderScheduleRunHistory(string ScheduleDBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[CleanOrderScheduleRunHistory]");
            }
        }
        public void CleanReportAlertScheduleRunHistory(string ScheduleDBName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [" + ScheduleDBName + "].[dbo].[CleanReportAlertScheduleRunHistory]");
            }
        }
        public List<ReportAlertScheduleRunHistoryDTO> GetRecordsAlertCountScheduleTest(int OperationType, int NumberOfInstance, string ScheduleDBName, long EnterpriseID, long MasterNotificationID, long NotificationID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance), new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@MasterNotificationID", MasterNotificationID), new SqlParameter("@NotificationID", NotificationID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ReportAlertScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordsAlertCountScheduleTest] @OperationType,@NumberOfInstance,@EnterpriseID,@MasterNotificationID,@NotificationID", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> GetRecordToRunOrderScheduleTest(int OperationType, int NumberOfInstance, string ScheduleDBName, long EnterpriseID, long MasterScheduleID, long ScheduleID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance), new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@MasterScheduleID", MasterScheduleID), new SqlParameter("@ScheduleID", ScheduleID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordToRunOrderScheduleTest] @OperationType,@NumberOfInstance,@EnterpriseID,@MasterScheduleID,@ScheduleID", params1).ToList();
            }
        }
        public List<OrderScheduleRunHistoryDTO> GetRecordToRunOrderScheduleDummyData(int OperationType, int NumberOfInstance, string ScheduleDBName, long EnterpriseID, long CompanyID, long RoomID, long MasterScheduleID, long ScheduleID, DateTime NextRunDate, short ScheduleFor)
        {
            OrderScheduleRunHistoryDTO objOrderScheduleRunHistoryDTO = new OrderScheduleRunHistoryDTO();
            objOrderScheduleRunHistoryDTO.EnterpriseID = EnterpriseID;
            objOrderScheduleRunHistoryDTO.CompanyID = CompanyID;
            objOrderScheduleRunHistoryDTO.RoomID = RoomID;
            objOrderScheduleRunHistoryDTO.ScheduleID = ScheduleID;
            objOrderScheduleRunHistoryDTO.MasterScheduleID = MasterScheduleID;
            objOrderScheduleRunHistoryDTO.NextRunDate = NextRunDate;
            objOrderScheduleRunHistoryDTO.Created = DateTime.Now;
            objOrderScheduleRunHistoryDTO.LastUpdated = DateTime.Now;
            objOrderScheduleRunHistoryDTO.ScheduleFor = ScheduleFor;
            List<OrderScheduleRunHistoryDTO> lst = new List<OrderScheduleRunHistoryDTO>();
            lst.Add(objOrderScheduleRunHistoryDTO);
            return lst;
        }
        public List<DailyScheduleRunHistoryDTO> GetRecordToRunScheduleTest(int OperationType, int NumberOfInstance, string ScheduleDBName, long EnterpriseID, long MasterScheduleID, long ScheduleID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OperationType", OperationType), new SqlParameter("@NumberOfInstance", NumberOfInstance), new SqlParameter("@EnterpriseID", EnterpriseID), new SqlParameter("@MasterScheduleID", MasterScheduleID), new SqlParameter("@ScheduleID", ScheduleID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<DailyScheduleRunHistoryDTO>("EXEC [" + ScheduleDBName + "].[dbo].[GetRecordToRunScheduleTest] @OperationType,@NumberOfInstance,@EnterpriseID,@MasterScheduleID,@ScheduleID", params1).ToList();
            }
        }

        #region [New Notification]

        public List<NotificationDTO> GetCurrentNotificationListByEventName(string eventName, long RoomID, long CompanyID, long UserID)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                return null;

            List<NotificationDTO> lstNotificationsImidiate = new List<NotificationDTO>();

            var params1 = new SqlParameter[] { new SqlParameter("@EventName", eventName), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@UserID", UserID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstNotificationsImidiate = context.Database.SqlQuery<NotificationDTO>("EXEC [dbo].[GetCurrentNotificationListByEventName] @EventName,@RoomID,@CompanyID,@UserID", params1).ToList();
                if (lstNotificationsImidiate != null && lstNotificationsImidiate.Count > 0)
                {
                    lstNotificationsImidiate = lstNotificationsImidiate.Where(x => IsActionCodeAvail(x.XMLValue ?? string.Empty, eventName) == true).ToList();
                }
                return lstNotificationsImidiate;
            }


        }

        public void SendMailForImmediate(List<NotificationDTO> lst, long RoomID, long CompanyID, long UserID, long EnterpriseID, string eTurnsScheduleDBName, string DataGuids = "")
        {
            EnterpriseDAL objEntDAL = new EnterpriseDAL(base.DataBaseName);
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            objEnterpriseDTO = objEntDAL.GetEnterprise(EnterpriseID);
            List<NotificationDTO> newNotificationList = new List<NotificationDTO>();
            string NotificationIDs = string.Join(",", lst.Select(t => t.ID).ToArray());
            List<NotificationMasterDTO> lstMasterNotification = GetMasterNotificationListSchedules(NotificationIDs, objEnterpriseDTO.EnterpriseDBName, objEnterpriseDTO.ID);

            if (lstMasterNotification != null && lstMasterNotification.Count > 0)
            {
                newNotificationList = GetNotificationsForEnterprise(NotificationIDs, objEnterpriseDTO.EnterpriseDBName);
                ReportAlertScheduleRunHistoryDTO objReportAlertScheduleRunHistoryDTO = new ReportAlertScheduleRunHistoryDTO();

                foreach (NotificationDTO itemNotification in newNotificationList)
                {
                    try
                    {
                        // SendReportNotification(itemNotification, objEnterpriseDTO);
                        NotificationMasterDTO Scheduleitem = lstMasterNotification.Where(t => t.EnterpriseId == objEnterpriseDTO.ID && t.NotificationID == itemNotification.ID).FirstOrDefault();

                        if (Scheduleitem != null && Scheduleitem.ID > 0)
                        {
                            objReportAlertScheduleRunHistoryDTO = new ReportAlertScheduleRunHistoryDTO();
                            objReportAlertScheduleRunHistoryDTO.EnterpriseID = Scheduleitem.EnterpriseId;
                            objReportAlertScheduleRunHistoryDTO.CompanyID = Scheduleitem.CompanyId;
                            objReportAlertScheduleRunHistoryDTO.RoomID = Scheduleitem.RoomId;
                            objReportAlertScheduleRunHistoryDTO.Created = DateTime.UtcNow;
                            objReportAlertScheduleRunHistoryDTO.LastUpdated = DateTime.UtcNow;
                            objReportAlertScheduleRunHistoryDTO.NextRunDate = Scheduleitem.NextRunDate;
                            objReportAlertScheduleRunHistoryDTO.RecGUID = Guid.NewGuid();
                            objReportAlertScheduleRunHistoryDTO.NotificationID = itemNotification.ID;
                            objReportAlertScheduleRunHistoryDTO.MasterNotificationID = Scheduleitem.ID;
                            objReportAlertScheduleRunHistoryDTO.ScheduleID = itemNotification.RoomScheduleID;
                            objReportAlertScheduleRunHistoryDTO.MasterScheduleID = itemNotification.RoomScheduleID;
                            objReportAlertScheduleRunHistoryDTO.ScheduleFor = itemNotification.ScheduleFor;
                            objReportAlertScheduleRunHistoryDTO.DataGuids = DataGuids;

                            InsertAlertCountScheduleHistory(objReportAlertScheduleRunHistoryDTO, eTurnsScheduleDBName);
                        }
                    }
                    catch (Exception ex)
                    {
                        SaveNotificationError(itemNotification, objEnterpriseDTO, UserID, ex);
                    }
                }
            }
        }

        public bool IsActionCodeAvail(string XMLValue, string EventName)
        {
            if (string.IsNullOrWhiteSpace(EventName) || string.IsNullOrEmpty(EventName))
                return false;

            string ActionCodes = string.Empty;
            bool isActionCodeMatchedEventName = false;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/ActionCodes");
            
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["Code" + i].InnerText))
                        {
                            //if (!string.IsNullOrWhiteSpace(ActionCodes))
                            //{
                            //    ActionCodes += "," + node["Code" + i].InnerText;
                            //}
                            //else
                            //{
                            //    ActionCodes += node["Code" + i].InnerText;
                            //}

                            if (node["Code" + i].InnerText.ToLower() == EventName.ToLower())
                            {
                                isActionCodeMatchedEventName = true;
                                break;
                            }
                        }
                    }

                    if (isActionCodeMatchedEventName)
                    {
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(EventName) && !string.IsNullOrEmpty(EventName) && isActionCodeMatchedEventName ) //if (!string.IsNullOrWhiteSpace(EventName) && (ActionCodes ?? string.Empty).ToLower().Contains(EventName.ToLower()))
            {
                return true;
            }

            return false;
        }


        #region [Send Email Method]

        public void SendReportNotification(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO)
        {

            List<eMailAttachmentDTO> lstAttachments = new List<eMailAttachmentDTO>();
            string EmailSubject = string.Empty;
            string EmailBody = string.Empty;
            string ToEmailAddersses = string.Empty;
            if (objNotificationDTO != null)
            {
                ToEmailAddersses = objNotificationDTO.EmailAddress;
                if (objNotificationDTO.EmailTemplateDetail != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls != null && objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.Count() > 0)
                {
                    EmailSubject = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First().MailSubject;
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, -1);
                    string DateTimeFormat = "MM/dd/yyyy";
                    DateTime TZDateTimeNow = DateTime.UtcNow;
                    if (objeTurnsRegionInfo != null)
                    {
                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                    }
                    if (EmailSubject != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        EmailSubject = Regex.Replace(EmailSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            EmailSubject = Regex.Replace(EmailSubject, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomName))
                        {
                            EmailSubject = Regex.Replace(EmailSubject, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }

                        EmailSubject = Regex.Replace(EmailSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);


                    }
                    EmailBody = objNotificationDTO.EmailTemplateDetail.lstEmailTemplateDtls.First().MailBodyText;
                    if (EmailBody != null && objNotificationDTO != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        EmailBody = Regex.Replace(EmailBody, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.CompanyName))
                        {
                            EmailBody = Regex.Replace(EmailBody, "@@COMPANYNAME@@", objNotificationDTO.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(objNotificationDTO.RoomName))
                        {
                            EmailBody = Regex.Replace(EmailBody, "@@ROOMNAME@@", objNotificationDTO.RoomName, RegexOptions.IgnoreCase);
                        }
                        EmailBody = Regex.Replace(EmailBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }
                    //EmailBody=SendMailForPendingOrders(objNotificationDTO, objEnterpriseDTO, objNotificationDTO.ReportMasterDTO.ReportName, EmailBody);
                }
            }
            lstAttachments = GenerateBytesBasedOnAttachment(objNotificationDTO, objEnterpriseDTO, ref EmailBody);
            // if (lstAttachments.Count() > 0 || objNotificationDTO.SendEmptyEmail)
            {
                if (objNotificationDTO.NotificationMode == 1)
                {
                    CreateReportMail(lstAttachments, EmailSubject, EmailBody, ToEmailAddersses, objNotificationDTO, objEnterpriseDTO);
                }
                else if (objNotificationDTO.NotificationMode == 2)
                {
                    if (lstAttachments.Count > 0)
                    {
                        //  SaveAttachMentsTosFTP(objNotificationDTO, objEnterpriseDTO, lstAttachments);
                        CreateReportMail(lstAttachments, EmailSubject, EmailBody, Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FailedEmailTo"]), objNotificationDTO, objEnterpriseDTO);
                    }
                }
            }
        }

        #region [Private Methods]

        public List<NotificationMasterDTO> GetMasterNotificationListSchedules(string NotificationIDs, string enterpriseDBName, long enterpriseID)
        {
            string eTurnsMasterDBName = (Convert.ToString(ConfigurationManager.AppSettings["MasterDBName"]) ?? "eTurnsMaster");
            List<NotificationMasterDTO> masterNotificationList = new List<NotificationMasterDTO>();
            //
            var params1 = new SqlParameter[] { new SqlParameter("@NotificationIDs", NotificationIDs), new SqlParameter("@EnterpriseID", enterpriseID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationMasterDTO>("EXEC [" + eTurnsMasterDBName + "].[dbo].[GetMasterNotificationByMainID] @NotificationIDs,@EnterpriseID", params1).ToList();
            }

        }


        public List<NotificationDTO> GetNotificationsForEnterprise(string Ids, string EnterpriseDBName)
        {
            List<NotificationDTO> lstNotificationsNew = new List<NotificationDTO>();
            NotificationDAL objNotificationDAL = new NotificationDAL(EnterpriseDBName);
            List<NotificationDTO> lstNotifications = objNotificationDAL.GetNotificationsByIds(Ids);
            List<EmailTemplateDTO> lstEmailTemplates = new List<EmailTemplateDTO>();
            List<ReportBuilderDTO> lstReports = new List<ReportBuilderDTO>();
            List<FTPMasterDTO> Ftps = new List<FTPMasterDTO>();
            List<eTurnsRegionInfo> lsteTurnsRegionInfo = new List<eTurnsRegionInfo>();
            List<EmailTemplateDetailDTO> lstEmailTemplateDetailDTO = new List<EmailTemplateDetailDTO>();
            List<SchedulerDTO> lstSchedules = new List<SchedulerDTO>();
            if (lstNotifications != null && lstNotifications.Count() > 0)
            {
                string emailTemplateIds = string.Join(",", lstNotifications.Where(t => (t.EmailTemplateID ?? 0) > 0).Select(t => (t.EmailTemplateID ?? 0)).ToArray());
                string reportId = string.Join(",", lstNotifications.Where(t => (t.ReportID ?? 0) > 0).Select(t => (t.ReportID ?? 0)).ToArray());
                string AttachmentIds = string.Join(",", lstNotifications.Where(t => (t.AttachmentReportIDs ?? string.Empty) != string.Empty).Select(t => (t.AttachmentReportIDs ?? string.Empty)).ToArray());
                string reportsandattachments = (reportId ?? string.Empty) + "," + (AttachmentIds ?? string.Empty);
                string RoomIds = string.Join(",", lstNotifications.Select(t => (t.RoomID)).ToArray());
                string ftpIds = string.Join(",", lstNotifications.Where(t => (t.FTPId ?? 0) > 0).Select(t => (t.FTPId ?? 0)).ToArray());
                long attachedreportID = 0;
                reportsandattachments = reportsandattachments.TrimEnd(new char[] { ',' }).TrimStart(new char[] { ',' });
                string ScheduleIds = string.Join(",", lstNotifications.Select(t => (t.RoomScheduleID)).ToArray());
                if (!string.IsNullOrWhiteSpace(emailTemplateIds))
                {
                    lstEmailTemplates = objNotificationDAL.GetEmailTemplatesByIDs(emailTemplateIds);
                }
                if (!string.IsNullOrWhiteSpace(reportsandattachments))
                {
                    lstReports = objNotificationDAL.GetReportsByIDs(reportsandattachments);
                }
                if (!string.IsNullOrWhiteSpace(RoomIds))
                {
                    lsteTurnsRegionInfo = objNotificationDAL.GetRegionSettingsById(RoomIds);
                }
                if (!string.IsNullOrWhiteSpace(ftpIds))
                {
                    Ftps = objNotificationDAL.GetFTPDetailsByIds(ftpIds);
                }
                if (!string.IsNullOrWhiteSpace(ScheduleIds))
                {
                    lstSchedules = objNotificationDAL.GetRoomSchedulesByIDs(ScheduleIds);
                }
                lstEmailTemplateDetailDTO = objNotificationDAL.GetEmailTemplateDetailsByNotIds(Ids);
                EmailTemplateDTO objEmailTemplateDTO = null;
                List<EmailTemplateDetailDTO> lstEmailTemplateDtls = null;
                foreach (var notification in lstNotifications)
                {
                    NotificationDTO objNOt = new NotificationDTO();
                    attachedreportID = !string.IsNullOrWhiteSpace(notification.AttachmentReportIDs) ? Convert.ToInt64(notification.AttachmentReportIDs.Trim().Split(',').First()) : 0;
                    notification.ReportMasterDTO = lstReports.FirstOrDefault(t => t.ID == notification.ReportID);
                    notification.AttachedReportMasterDTO = lstReports.FirstOrDefault(t => t.ID == attachedreportID);
                    notification.FtpDetails = Ftps.FirstOrDefault(t => t.ID == notification.FTPId);
                    notification.SchedulerParams = lstSchedules.FirstOrDefault(t => t.ScheduleID == notification.RoomScheduleID);
                    notification.objeTurnsRegionInfo = lsteTurnsRegionInfo.FirstOrDefault(t => t.RoomId == notification.RoomID);
                    notification.EmailTemplateDetail = new EmailTemplateDTO();
                    objEmailTemplateDTO = new EmailTemplateDTO();
                    //objEmailTemplateDTO = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID);
                    if (lstEmailTemplates.Any(t => t.ID == notification.EmailTemplateID))
                    {
                        objEmailTemplateDTO.CompanyId = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).CompanyId;
                        objEmailTemplateDTO.Created = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).Created;
                        objEmailTemplateDTO.CreatedBy = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).CreatedBy;
                        objEmailTemplateDTO.CreatedDate = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).CreatedDate;
                        objEmailTemplateDTO.ID = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).ID;
                        objEmailTemplateDTO.IdWithValue = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).IdWithValue;
                        objEmailTemplateDTO.LastUpdatedBy = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).LastUpdatedBy;
                        objEmailTemplateDTO.MailBodyText = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).MailBodyText;
                        objEmailTemplateDTO.MailSubject = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).MailSubject;
                        objEmailTemplateDTO.RoomId = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).RoomId;
                        objEmailTemplateDTO.TemplateName = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).TemplateName;
                        objEmailTemplateDTO.Updated = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).Updated;
                        objEmailTemplateDTO.UpdatedDate = lstEmailTemplates.FirstOrDefault(t => t.ID == notification.EmailTemplateID).UpdatedDate;
                        objEmailTemplateDTO.lstEmailTemplateDtls = new List<EmailTemplateDetailDTO>();
                    }
                    if (objEmailTemplateDTO != null)
                    {
                        lstEmailTemplateDtls = new List<EmailTemplateDetailDTO>();
                        lstEmailTemplateDtls = lstEmailTemplateDetailDTO.Where(t => t.NotificationID == notification.ID && t.EmailTempateId == objEmailTemplateDTO.ID && t.CultureCode == notification.objeTurnsRegionInfo.CultureCode).ToList();
                        objEmailTemplateDTO.lstEmailTemplateDtls = new List<EmailTemplateDetailDTO>();
                        objEmailTemplateDTO.lstEmailTemplateDtls = lstEmailTemplateDtls;
                    }
                    notification.EmailTemplateDetail = objEmailTemplateDTO;
                    objNOt = notification;
                    lstNotificationsNew.Add(objNOt);
                }
            }
            return lstNotificationsNew;
        }

        private void SaveNotificationError(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO, long UserID, Exception Ex)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            ReportSchedulerErrorDTO objReportSchedulerError = new ReportSchedulerErrorDTO();
            objReportSchedulerError.CompanyID = objNotificationDTO.CompanyID;
            objReportSchedulerError.EnterpriseID = objEnterpriseDTO.ID;
            objReportSchedulerError.Exception = Convert.ToString(Ex);
            objReportSchedulerError.ID = 0;
            objReportSchedulerError.NotificationID = objNotificationDTO.ID;
            objReportSchedulerError.RoomID = objNotificationDTO.RoomID;
            objReportSchedulerError.UserID = UserID;
            objReportSchedulerError.ScheduleFor = objNotificationDTO.ScheduleFor;
            objCommonDAL.SaveNotificationError(objReportSchedulerError);
        }

        private List<eMailAttachmentDTO> GenerateBytesBasedOnAttachment(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterprise, ref string Emailbody)
        {
            List<eMailAttachmentDTO> bytes = new List<eMailAttachmentDTO>();
            long ParentID = 0;
            ReportMailLogDTO objReportMailLogDTO = null;
            ReportMasterDAL objDAL = null;
            string strEmail = string.Empty;
            string MasterReportResFile = string.Empty;
            IEnumerable<XElement> lstTablix = null;
            IEnumerable<XElement> lstUpdateTablix = null;
            IEnumerable<XElement> lstReportPara = null;
            List<ReportParameter> rpt = null;
            SqlConnection myConnection = null;
            SqlCommand cmd = null;
            SqlDataAdapter sqla = null;
            DataTable dt = null;
            IEnumerable<XElement> lstQueryPara = null;
            SqlParameter slpar = null;
            XElement objReportPara = null;
            ReportViewer ReportViewer1 = null;
            XDocument docSub = null;
            ReportDataSource rds = null;
            XDocument doc = null;
            RegionSettingDAL rsDAL = null;
            eTurnsRegionInfo rsInfo = null;
            eTurns.DAL.AlertMail amDAL = null;
            //Dictionary<string, string> LocalDictRptPara = null;
            try
            {
                if (objNotificationDTO != null)
                {
                    objDAL = new ReportMasterDAL(objEnterprise.EnterpriseDBName);
                    rsDAL = new RegionSettingDAL(objEnterprise.EnterpriseDBName);
                    rsInfo = rsDAL.GetRegionSettingsById(objNotificationDTO.RoomID, objNotificationDTO.CompanyID, 0);
                    amDAL = new DAL.AlertMail();
                    //Get mail logo 

                    /* TO DO : remove*/
                    objReportMailLogDTO = objDAL.GetReportMaillog(Convert.ToInt64(objNotificationDTO.ID), false, null);


                    strEmail = Convert.ToString(objNotificationDTO.EmailAddress);

                    /* TO DO : attchemtn maaster Report DTO*/
                    if (objNotificationDTO.ReportMasterDTO != null)
                    {
                        //Set values from ReportBuilder Objects
                        MasterReportResFile = objNotificationDTO.ReportMasterDTO.MasterReportResFile;
                        SubReportResFile = MasterReportResFile;
                        string Reportname = objNotificationDTO.ReportMasterDTO.ReportName;
                        string MasterReportname = objNotificationDTO.ReportMasterDTO.ReportFileName;
                        string SubReportname = objNotificationDTO.ReportMasterDTO.SubReportFileName;
                        string mainGuid = "RPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string subGuid = "SubRPT_" + Guid.NewGuid().ToString().Replace("-", "_");
                        string ReportPath = string.Empty;
                        bool hasSubReport = false;
                        string strCulture = "en-US";
                        if (rsInfo != null && !string.IsNullOrEmpty(rsInfo.CultureCode))
                            strCulture = rsInfo.CultureCode;

                        if (!string.IsNullOrEmpty(objNotificationDTO.CultureCode))
                            strCulture = objNotificationDTO.CultureCode;

                        ParentID = objNotificationDTO.ReportMasterDTO.ParentID ?? 0;

                        if (objNotificationDTO.AttachedReportMasterDTO.ParentID > 0)
                        {
                            if (objNotificationDTO.AttachedReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                            {
                                ReportPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                            else
                            {
                                ReportPath = CopyFiletoTempForAlert(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"]) + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                            }
                        }
                        else
                        {
                            ReportPath = CopyFiletoTempForAlert(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["RDLCBaseFilePath"]) + @"\" + objEnterprise.ID + @"\BaseReport" + @"\\" + MasterReportname, mainGuid, ReportBasePath + @"\Temp");
                        }



                        doc = XDocument.Load(ReportPath);
                        doc = amDAL.AddFormatToTaxbox(doc, rsInfo);
                        doc.Save(ReportPath);
                        doc = XDocument.Load(ReportPath);

                        lstTablix = doc.Descendants(ns + "Tablix");

                        if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                        {
                            hasSubReport = true;
                        }

                        //Hardik Code Started

                        eTurns.DAL.AlertMail objAlertMail = new eTurns.DAL.AlertMail();
                        if (objNotificationDTO != null && (objNotificationDTO.HideHeader))
                        {
                            if (!hasSubReport && !objNotificationDTO.ReportMasterDTO.IsNotEditable.GetValueOrDefault(false)
                                             && (objNotificationDTO.ReportMasterDTO.ReportType == 3 || objNotificationDTO.ReportMasterDTO.ReportType == 1)
                                             && (objNotificationDTO.HideHeader))
                            {
                                doc = objAlertMail.GetAdditionalHeaderRow(doc, objNotificationDTO.ReportMasterDTO, objNotificationDTO.CompanyName, objNotificationDTO.RoomName, EnterpriseDBName: objEnterprise.EnterpriseDBName);
                                doc.Save(ReportPath);
                                doc = XDocument.Load(ReportPath);

                            }
                        }

                        if (objNotificationDTO != null && (objNotificationDTO.ShowSignature))
                        {
                            doc = objAlertMail.GetFooterForSignature(doc, objNotificationDTO.ReportMasterDTO);
                            doc.Save(ReportPath);
                            doc = XDocument.Load(ReportPath);
                        }
                        if (objNotificationDTO != null && objNotificationDTO.SortSequence != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.SortSequence)))
                        {
                            objNotificationDTO.ReportMasterDTO.SortColumns = objNotificationDTO.SortSequence;
                        }
                        //Hardik Code ended


                        string strTablix = string.Empty;
                        if (lstTablix != null && lstTablix.ToList().Count > 0)
                        {
                            strTablix = lstTablix.ToList()[0].ToString();
                        }
                        lstUpdateTablix = UpdateResourceNew(lstTablix, MasterReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                        //lstUpdateTablix = UpdateResource(lstTablix, MasterReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);

                        rptPara = SetGetPDFReportParaDictionary(objEnterprise, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, objReportMailLogDTO, objNotificationDTO);


                        if (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ReportType == 3 && !hasSubReport && rptPara != null && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
                        {
                            string SortFields = rptPara["SortFields"];

                            string[] arrSortFields = SortFields.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrSortFields != null && arrSortFields.Length > 0)
                            {
                                string firstSortFields = arrSortFields[0].Replace(" asc", "").Replace(" desc", "").Replace(" ASC", "").Replace(" DESC", "");
                                XElement xRowHira = doc.Descendants(ns + "TablixRowHierarchy").FirstOrDefault();
                                XElement xGroup = xRowHira.Descendants(ns + "Group").FirstOrDefault();
                                XElement xGroupExpression = xGroup.Descendants(ns + "GroupExpression").FirstOrDefault();
                                if (xGroupExpression != null)
                                    xGroupExpression.Value = "=Fields!" + firstSortFields + ".Value";
                            }
                        }


                        doc.Save(ReportPath);
                        lstReportPara = doc.Descendants(ns + "ReportParameter");
                        rpt = new List<ReportParameter>();

                        /* TO DO : remove mail log ref*/



                        if (lstReportPara != null && lstReportPara.Count() > 0)
                        {
                            foreach (var item in lstReportPara)
                            {
                                ReportParameter rpara = new ReportParameter();
                                rpara.Name = item.Attribute("Name").Value;
                                if (!string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value))
                                    rpara.Values.Add(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value);

                                rpt.Add(rpara);
                            }
                        }



                        DBconectstring = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", objEnterprise.EnterpriseDBName);
                        string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
                        myConnection = new SqlConnection();
                        cmd = new SqlCommand();
                        sqla = new SqlDataAdapter();
                        dt = new DataTable();
                        myConnection.ConnectionString = DbConnectionHelper.GetOledbConnection("GeneralReadOnly", objEnterprise.EnterpriseDBName);
                        cmd.Connection = myConnection;
                        cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                        cmd.CommandType = CommandType.Text;
                        if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                        {
                            cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                        }

                        lstQueryPara = doc.Descendants(ns + "QueryParameter");

                        if (lstQueryPara != null && lstQueryPara.Count() > 0)
                        {
                            foreach (var item in lstQueryPara)
                            {
                                slpar = new SqlParameter();
                                slpar.ParameterName = item.Attribute("Name").Value;//
                                if (!(hasSubReport && slpar.ParameterName.ToLower().Replace("@", "") == "sortfields") && !(string.IsNullOrEmpty(rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value)))
                                    slpar.Value = rptPara.FirstOrDefault(x => x.Key.Replace("@", "") == item.Attribute("Name").Value.Replace("@", "")).Value;
                                else
                                    slpar.Value = DBNull.Value;

                                objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));

                                if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                                {
                                    slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                                }

                                cmd.Parameters.Add(slpar);
                            }
                        }

                        if (cmd.CommandText == "RPT_GetAuditTrail_Data" || cmd.CommandText == "RPT_GetItemAuditTrail_Trans")
                        {

                            Call_Sub_SP_ForDML(cmd, objEnterprise.EnterpriseDBName);
                        }

                        cmd.CommandTimeout = 600;
                        sqla = new SqlDataAdapter(cmd);
                        sqla.Fill(dt);
                        if (Emailbody.ToLower().Contains("@@table@@"))
                        {
                            Emailbody = NewEmailBody(dt, Emailbody, doc, MasterReportResFile, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID, strCulture);
                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (objNotificationDTO != null && (objNotificationDTO.XMLValue) != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.XMLValue)))
                            {
                                // List<ReportBuilderDTO> lstReportList = new ReportMasterDAL(objEnterprise.EnterpriseDBName).GetReportList();

                                if ((objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition"))
                                || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition")))
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("status") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        string wherecondition = string.Empty;

                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/Status");
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["Status" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(wherecondition))
                                                        {
                                                            wherecondition += "or RequisitionStatus in('" + node["Status" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            wherecondition = " RequisitionStatus in('" + node["Status" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            WoWhereCondition += "or WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            WoWhereCondition = " WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                                        string OrderWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(OrderWhereCondition))
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                OrderWhereCondition += "or OrderStatus in('UnSubmitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Submitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Approved') ";
                                                                OrderWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                OrderWhereCondition += "or OrderStatus in('Past Due') ";
                                                                OrderWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                OrderWhereCondition += "or OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                OrderWhereCondition = " OrderStatus in('UnSubmitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Submitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Approved') ";
                                                                OrderWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                OrderWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                OrderWhereCondition += "or OrderStatus in('Past Due') ";
                                                                OrderWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                OrderWhereCondition = " OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // query example = "id = 10"

                                        dv.RowFilter = wherecondition; // query example = "id = 10"
                                        dv.RowFilter = WoWhereCondition;
                                        dv.RowFilter = OrderWhereCondition;
                                        dt = dv.ToTable();
                                    }

                                }
                                if (objNotificationDTO != null &&
                                       (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "order"))
                                       || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "order"))
                                       )
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("orderstatus") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                WoWhereCondition += "or OrderStatus in('UnSubmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Submitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Approved') ";
                                                                WoWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                WoWhereCondition += "or OrderStatus in('Past Due') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                WoWhereCondition += "or OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (node["OrderStatus" + i].InnerText.ToLower().Equals("open"))
                                                            {
                                                                WoWhereCondition = " OrderStatus in('UnSubmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Submitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Approved') ";
                                                                WoWhereCondition += "or OrderStatus in('Transmitted') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete') ";
                                                                WoWhereCondition += "or OrderStatus in('Past Due') ";
                                                                WoWhereCondition += "or OrderStatus in('Incomplete Past Due') ";
                                                            }
                                                            else if (node["OrderStatus" + i].InnerText.ToLower().Equals("close"))
                                                            {
                                                                WoWhereCondition = " OrderStatus in('Closed') ";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            dv.RowFilter = WoWhereCondition;
                                            dt = dv.ToTable();
                                        }
                                    }

                                }
                                if (objNotificationDTO != null &&
                                       (objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))
                                       || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))
                                       )
                                {

                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("wostatus") >= 0)
                                    {

                                        DataView dv = new DataView(dt);
                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                                        string WoWhereCondition = string.Empty;
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                        {
                                                            WoWhereCondition += "or WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                        else
                                                        {
                                                            WoWhereCondition = " WOStatus in('" + node["WOStatus" + i].InnerText + "') ";
                                                        }
                                                    }
                                                }
                                            }
                                            dv.RowFilter = WoWhereCondition;
                                            dt = dv.ToTable();

                                        }
                                        // query example = "id = 10"

                                    }

                                }
                                if ((objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull"))
                                || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull")))
                                {
                                    if (objNotificationDTO.XMLValue.ToLower().IndexOf("quantitytype") >= 0)
                                    {
                                        DataView dv = new DataView(dt);
                                        string wherecondition = string.Empty;

                                        XmlDocument xmldoc = new XmlDocument();
                                        xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                                        XmlNodeList nodeList = xmldoc.SelectNodes("/Data/QuantityType");

                                        string WoWhereCondition = "";
                                        if (nodeList != null && nodeList.Count > 0)
                                        {
                                            foreach (XmlNode node in nodeList)
                                            {
                                                for (int i = 1; i <= node.ChildNodes.Count; i++)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(node["Type" + i].InnerText))
                                                    {
                                                        if (node["Type" + i].InnerText == "1")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                            {
                                                                WoWhereCondition += "or CustomerOwnedQuantity > 0 ";
                                                            }
                                                            else
                                                            {
                                                                WoWhereCondition += "CustomerOwnedQuantity > 0 ";
                                                            }
                                                        }
                                                        if (node["Type" + i].InnerText == "2")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                                            {
                                                                WoWhereCondition += " or ConsignedQuantity > 0 ";
                                                            }
                                                            else
                                                            {
                                                                WoWhereCondition += " ConsignedQuantity > 0 ";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(WoWhereCondition))
                                        {
                                            WoWhereCondition = " (" + WoWhereCondition + ")";
                                        }

                                        dv.RowFilter = wherecondition; // query example = "id = 10"
                                        dv.RowFilter = WoWhereCondition;
                                        dt = dv.ToTable();
                                    }

                                }
                            }
                        }
                        if (dt != null && dt.Rows.Count > 0)
                        {



                            ReportViewer1 = new ReportViewer();
                            ReportViewer1.Reset();
                            ReportViewer1.LocalReport.DataSources.Clear();

                            if (doc.Descendants(ns + "Subreport") != null && doc.Descendants(ns + "Subreport").Count() > 0)
                            {
                                hasSubReport = true;
                                if (objNotificationDTO.ReportMasterDTO.ParentID > 0)
                                {
                                    if (objNotificationDTO.ReportMasterDTO.ISEnterpriseReport.GetValueOrDefault(false))
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\EnterpriseReport" + @"\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                    }
                                    else
                                    {
                                        rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\" + objNotificationDTO.CompanyID + @"\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                    }
                                }
                                else
                                {

                                    rdlPath = CopyFiletoTempForAlert(ReportBasePath + @"\" + objEnterprise.ID + @"\BaseReport" + @"\\" + SubReportname, subGuid, ReportBasePath + @"\Temp");
                                }
                                doc.Descendants(ns + "Tablix").Descendants(ns + "Subreport").FirstOrDefault().Attribute("Name").Value = Convert.ToString(subGuid);
                                doc.Descendants(ns + "Tablix").Descendants(ns + "ReportName").FirstOrDefault().Value = Convert.ToString(subGuid);
                                doc.Save(ReportPath);

                                docSub = XDocument.Load(rdlPath);
                                //lstSubTablix = docSub.Descendants(ns + "Tablix");

                                IEnumerable<XElement> lstSubTablixNew = docSub.Descendants(ns + "Tablix");
                                IEnumerable<XElement> lstUpdateSubTablixNew = UpdateResourceNew(lstSubTablixNew, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);

                                docSub.Save(rdlPath);


                                if (lstSubTablixNew != null && lstSubTablixNew.ToList().Count > 0)
                                {
                                    strSubTablix = lstSubTablixNew.ToList()[0].ToString();
                                }

                                docSub = amDAL.AddFormatToTaxbox(docSub, rsInfo);
                                docSub.Save(rdlPath);
                                docSub = XDocument.Load(rdlPath);

                                //lstUpdateSubTablix = UpdateResource(lstSubTablix, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID);
                                //lstUpdateSubTablix = UpdateResourceNew(lstSubTablix, SubReportResFile, strCulture, objEnterprise.ID, objNotificationDTO.CompanyID, objNotificationDTO.RoomID);
                                //docSub.Save(rdlPath);

                                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LoadSubreport);

                                ReportViewer1.LocalReport.EnableExternalImages = true;
                                ReportViewer1.LocalReport.EnableHyperlinks = true;
                                ReportViewer1.LocalReport.Refresh();
                            }
                            if (!hasSubReport && rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
                            {
                                string SortFields = rptPara["SortFields"].TrimEnd(',');

                                if (!string.IsNullOrEmpty(SortFields))
                                {
                                    dt.DefaultView.Sort = SortFields;
                                    dt = dt.DefaultView.ToTable();

                                }
                            }

                            ReportViewer1.LocalReport.EnableExternalImages = true;
                            ReportViewer1.LocalReport.ReportPath = ReportPath;
                            rds = new ReportDataSource();
                            rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
                            rds.Value = dt;

                            ReportViewer1.LocalReport.DataSources.Add(rds);
                            ReportViewer1.LocalReport.SetParameters(rpt);
                            ReportViewer1.ZoomMode = ZoomMode.Percent;
                            ReportViewer1.LocalReport.Refresh();

                            if (objNotificationDTO.AttachmentTypes == "1")
                            {
                                //Generate byte for PDF File Only
                                Warning[] warnings;
                                string[] streamIds;
                                string mimeType = "application/pdf";
                                string encoding = "utf-8";
                                string extension = "pdf";
                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 0;
                                objAttachment.AttachedFileName = Reportname + ".pdf";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                                //bytes.Add(tempByte);
                            }
                            else if (objNotificationDTO.AttachmentTypes == "2")
                            {
                                //Generate byte for EXCEL file
                                Warning[] warnings2;
                                string[] streamIds2;
                                string mimeType2 = "application/vnd.ms-excel";
                                string encoding2 = "utf-8";
                                string extension2 = "xls";

                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("Excel", null, out mimeType2, out encoding2, out extension2, out streamIds2, out warnings2);

                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 0;
                                objAttachment.AttachedFileName = Reportname + ".xls";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType2;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                            }
                            else if (objNotificationDTO.AttachmentTypes == "1,2")
                            {
                                //Generate Excel and PDF files bytes
                                Warning[] warnings;
                                string[] streamIds;
                                string mimeType = "application/pdf";
                                string encoding = "utf-8";
                                string extension = "pdf";
                                byte[] tempByte = null;
                                tempByte = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                                eMailAttachmentDTO objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 1;
                                objAttachment.AttachedFileName = Reportname + ".pdf";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);

                                warnings = null;
                                mimeType = "application/vnd.ms-excel";
                                streamIds = null;
                                extension = "xls";
                                tempByte = null;

                                tempByte = ReportViewer1.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);


                                objAttachment = new eMailAttachmentDTO();
                                objAttachment.ID = 1;
                                objAttachment.AttachedFileName = Reportname + ".xls";
                                objAttachment.eMailToSendID = 0;
                                objAttachment.MimeType = mimeType;
                                objAttachment.FileData = tempByte;
                                bytes.Add(objAttachment);
                            }
                        }

                    }


                }
            }
            catch
            {
                throw;
            }
            finally
            {

                objReportMailLogDTO = null;
                objDAL = null;
                doc = null;
                lstTablix = null;
                lstUpdateTablix = null;
                lstReportPara = null;
                rpt = null;
                if (myConnection != null)
                    myConnection.Dispose();

                if (cmd != null)
                    cmd.Dispose();
                if (sqla != null)
                    sqla.Dispose();

                if (dt != null)
                    dt.Dispose();

                if (sqla != null)
                    sqla.Dispose();

                sqla = null;
                myConnection = null;
                cmd = null;
                dt = null;
                lstQueryPara = null;
                slpar = null;
                objReportPara = null;
                docSub = null;
                rds = null;
                if (ReportViewer1 != null)
                {
                    ReportViewer1.Dispose();
                }

            }
            return bytes;
        }

        private string CopyFiletoTempForAlert(string strfile, string reportname, string ReportTempPath)
        {
            try
            {
                string ReportRetPath = string.Empty;

                if (!System.IO.Directory.Exists(ReportTempPath))
                {
                    System.IO.Directory.CreateDirectory(ReportTempPath);
                }
                ReportRetPath = ReportTempPath + @"\" + reportname + ".rdlc";
                //System.IO.File.Copy(strfile, ReportRetPath);
                System.IO.File.Create(ReportRetPath).Dispose();
                System.IO.File.WriteAllText(ReportRetPath, System.IO.File.ReadAllText(strfile));


                return ReportRetPath;
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {

            }
        }

        private void Call_Sub_SP_ForDML(SqlCommand cmd, string databasename)
        {
            ReportMasterDAL objDAL = new ReportMasterDAL(databasename);
            if (cmd.CommandText == "RPT_GetAuditTrail_Data")
            {
                string _dataGuids = string.Empty;
                if (cmd.Parameters.Contains("@DataGuids"))
                {
                    _dataGuids = (cmd.Parameters["@DataGuids"].Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(_dataGuids))
                    {
                        objDAL.Call_AT_AT_Calc_Qty(_dataGuids);
                    }
                }
            }
            else if (cmd.CommandText == "RPT_GetItemAuditTrail_Trans")
            {
                string _dataGuids = string.Empty;
                if (cmd.Parameters.Contains("@DataGuids"))
                {
                    _dataGuids = (cmd.Parameters["@DataGuids"].Value ?? string.Empty).ToString();
                    if (!string.IsNullOrEmpty(_dataGuids))
                    {
                        objDAL.Call_AT_AuditTrail_CalculateQty(_dataGuids);
                    }
                }
            }
        }

        private string GetResourceFileFullPath(string fileName, string Culture, Int64 EntID, Int64 CompanyID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        private string GetResourceFileFullPathForUDF(string fileName, string Culture, Int64 EntID, Int64 CompanyID, Int64 RoomID)
        {
            string path = ResourceBaseFilePath;
            path += "\\" + EntID + "\\" + CompanyID + "\\" + RoomID + "\\" + fileName;
            if (Culture != "en-US")
            {
                path += "." + Culture;
            }
            path += ".resx";

            return path;
        }

        private IEnumerable<XElement> UpdateResourceNew(IEnumerable<XElement> lstTablix, string ResFile, string strCulture, Int64 EntID, Int64 CompanyID, Int64 RoomID)
        {
            //make changes to resolved WI-1123
            string ResourceFilePath = GetResourceFileFullPath(ResFile, strCulture, EntID, CompanyID);
            string ResourceFilePathForUDF = GetResourceFileFullPathForUDF(ResFile, strCulture, EntID, CompanyID, RoomID);
            IEnumerable<XElement> lstTableCell = null;
            try
            {
                foreach (XElement Table in lstTablix)
                {
                    lstTableCell = Table.Descendants(ns + "TablixCell");
                    foreach (XElement Cell in lstTableCell)
                    {
                        if (Cell.Descendants(ns + "Value").Any())
                        {
                            //Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResFile, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                            //Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValue(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value);
                            Cell.Descendants(ns + "Value").FirstOrDefault().Value = GetResourceValueNew(ResourceFilePath, Cell.Descendants(ns + "Value").FirstOrDefault().Value, ResourceFilePathForUDF);
                        }
                    }
                }

                return lstTablix;
            }
            finally
            {
                lstTableCell = null;
            }
        }

        private string GetResourceValue(string ResourceFilePath, string Key)
        {
            string KeyVal = string.Empty;
            KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePath);
            //KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            return KeyVal;
        }

        private string GetResourceValueNew(string ResourceFilePath, string Key, string ResourceFilePathForUDF = "")
        {
            string KeyVal = string.Empty;
            if (Key.Trim().ToLower().Contains("udf"))
            {
                KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePathForUDF);
            }
            else
            {
                KeyVal = ResourceHelper.GetResourceValueByKeyAndFullFilePath(Key, ResourceFilePath);
            }
            //KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
            return KeyVal;
        }

        private Dictionary<string, string> SetGetPDFReportParaDictionary(EnterpriseDTO EntDTO, long CompanyID, long RoomID, ReportMailLogDTO objReportMailLogDTO, NotificationDTO objNotificationDTO)
        {
            Dictionary<string, string> dictionary = null;
            try
            {

                dictionary = new Dictionary<string, string>();
                DateTime startDate = DateTime.MinValue;
                DateTime EndDate = DateTime.MinValue;
                CompanyMasterDTO objCompDTO = new CompanyMasterDAL(EntDTO.EnterpriseDBName).GetCompanyByID(CompanyID);
                ItemMasterDAL objItemDAL = new ItemMasterDAL(EntDTO.EnterpriseDBName);

                //For Room Report and company report, RoomId will not added
                List<ReportBuilderDTO> lstReportList = new ReportMasterDAL(EntDTO.EnterpriseDBName).GetReportList();
                bool AddRoomIDFilter = true;

                if (objNotificationDTO != null && objNotificationDTO.ReportName.Trim().ToLower() == "room")
                {
                    AddRoomIDFilter = false;
                }

                if (objNotificationDTO != null && objNotificationDTO.ReportName.Trim().ToLower() == "company")
                {
                    AddRoomIDFilter = false;
                }
                if (lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "room").Any() && AddRoomIDFilter)
                {
                    ReportBuilderDTO objRoomReportDTO = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "room").FirstOrDefault();
                    ReportBuilderDTO CurrentReport = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == objNotificationDTO.ReportName.Trim().ToLower()).FirstOrDefault();
                    if (CurrentReport != null && objRoomReportDTO != null && CurrentReport.ParentID == objRoomReportDTO.ID)
                    {
                        AddRoomIDFilter = false;
                    }
                }
                if (lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "company").Any() && AddRoomIDFilter)
                {
                    ReportBuilderDTO objRoomReportDTO = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == "company").FirstOrDefault();
                    ReportBuilderDTO CurrentReport = lstReportList.ToList().Where(r => r.ReportName.Trim().ToLower() == objNotificationDTO.ReportName.Trim().ToLower()).FirstOrDefault();
                    if (CurrentReport != null && objRoomReportDTO != null && CurrentReport.ParentID == objRoomReportDTO.ID)
                    {
                        AddRoomIDFilter = false;
                    }
                }


                dictionary.Add("ConnectionString", DbConnectionHelper.GeteTurnsSQLConnectionString(EntDTO.EnterpriseDBName, DbConnectionType.GeneralReadOnly.ToString("F")));

                if (objNotificationDTO != null && AddRoomIDFilter)
                {
                    //dictionary.Add("CompanyIDs", Convert.ToString(CompanyID));
                    //dictionary.Add("RoomIDs", Convert.ToString(RoomID));
                    dictionary.Add("CompanyIDs", Convert.ToString(objNotificationDTO.CompanyIds));
                    dictionary.Add("RoomIDs", Convert.ToString(objNotificationDTO.RoomIds));
                }
                CommonDAL objCommonDAL = new CommonDAL(EntDTO.EnterpriseDBName);
                if (objCommonDAL.HasSpecialDomain(EntDTO.EnterpriseDBName, EntDTO.ID))
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + EntDTO.ID + "\\" + EntDTO.EnterpriseLogo));
                }
                dictionary.Add("CompanyLogoURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + CompanyID + "\\" + objCompDTO.CompanyLogo));

                dictionary.Add("UserID", null);

                dictionary.Add("BarcodeURL", System.Configuration.ConfigurationManager.AppSettings["LocalHostName"] + "/Barcode/GetBarcodeByKey?barcodekey=");
                if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 2)
                {
                    startDate = DateTime.UtcNow.AddDays(-(Convert.ToDouble(objNotificationDTO.ReportDataSince)));
                    EndDate = objNotificationDTO.NextRunDate ?? DateTime.UtcNow;
                    dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                }
                else if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 1)
                {
                    if (objReportMailLogDTO != null)
                    {
                        startDate = objReportMailLogDTO.SendDate;
                        EndDate = (objNotificationDTO.NextRunDate ?? DateTime.UtcNow).AddSeconds(-1);

                        dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                        dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                        dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                    }
                    else
                    {
                        dictionary.Add("StartDate", null);
                        dictionary.Add("EndDate", null);
                    }

                }
                else if (objNotificationDTO != null && objNotificationDTO.ReportDataSelectionType == 3)
                {

                    DateTime current = (DateTime.UtcNow);
                    startDate = new DateTime(current.Year, current.Month, 1);
                    EndDate = (objNotificationDTO.NextRunDate ?? DateTime.UtcNow).AddSeconds(-1);

                    dictionary.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    dictionary.Add("EndDate", EndDate.ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigStartDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, startDate).ToString("yyyy-MM-dd  HH:mm:ss"));
                    dictionary.Add("OrigEndDate", DateTimeUtility.ConvertDateFromUTC(objNotificationDTO.objeTurnsRegionInfo.TimeZoneName, EndDate).ToString("yyyy-MM-dd  HH:mm:ss"));

                }
                if (objNotificationDTO != null)
                {
                    if (objNotificationDTO.ReportMasterDTO != null)
                    {
                        if (!string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.SortColumns))
                        {
                            dictionary.Add("SortFields", objNotificationDTO.ReportMasterDTO.SortColumns.TrimEnd(','));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(objNotificationDTO.SupplierIds) && AddRoomIDFilter)
                    dictionary.Add("SupplierIDs", objNotificationDTO.SupplierIds);
                else
                    dictionary.Add("SupplierIDs", string.Empty);
                if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems"))
               || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems")))
                {
                    string RequisitionStatus = string.Empty;
                    string DaysUntilItemExpires = string.Empty;
                    string DaysToApproveOrder = string.Empty;
                    string ProjectExpirationDate = string.Empty;

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OnlyExpiredItems");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        RequisitionStatus = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/DaysUntilItemExpires");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        DaysUntilItemExpires = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/DaysToApproveOrder");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        DaysToApproveOrder = nodeList[0].InnerText;
                    }
                    nodeList = xmldoc.SelectNodes("/Data/ProjectExpirationDate");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        ProjectExpirationDate = nodeList[0].InnerText;
                    }
                    dictionary.Add("OnlyExirationItems", RequisitionStatus);
                    if (string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        dictionary.Add("DaysToApproveOrder", DaysUntilItemExpires);
                        dictionary.Add("DaysUntilItemExpires", DaysToApproveOrder);
                    }
                    else
                    {
                        dictionary.Add("DaysToApproveOrder", string.Empty);
                        dictionary.Add("DaysUntilItemExpires", string.Empty);

                    }
                    dictionary.Add("ProjectedExpirationDate", ProjectExpirationDate);
                    if (!string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        objNotificationDTO.ProjectExpirationDate = FnCommon.ConvertDateByTimeZone(Convert.ToDateTime(ProjectExpirationDate), true).Split(' ')[0];// ProjectExpirationDate.ToString(SessionHelper.RoomDateFormat);
                    }
                    // objNotificationDTO.ProjectExpirationDate = ProjectExpirationDate;

                }
                else if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "tool"))
               || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "tool")))
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("OnlyAvailableTools").FirstOrDefault() != null && xDoc.Descendants("OnlyAvailableTools").FirstOrDefault().Value == "Yes")
                        dictionary.Add("@OnlyAvailable", "HasQty");
                }
                else if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "countmaster")))
                {
                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");
                    if (xDoc.Descendants("CountAppliedFilter").FirstOrDefault() != null)
                        dictionary.Add("@AppliedFilter", xDoc.Descendants("CountAppliedFilter").FirstOrDefault().Value);
                }

                else if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "instockbybin"
                                                                                                                                                             || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "instockbyactivity"
                                                                                                                                                             || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "instockbybinmargin"
                                                                                                                                                             || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "instockwithqoh")))
                {
                    string _QOHFilter = string.Empty;
                    // string _MonthlyAverageUsage = "30";
                    // string _OnlyExpirationItems = "No";
                    string _DataGuids = string.Empty;
                    string _range = "";
                    string _rangeData = string.Empty;
                    string _itemGuids = string.Empty;
                    string _binIDs = string.Empty;
                    string _supplierIDs = string.Empty;
                    string _categoryIDs = string.Empty;
                    bool _isSelectAllRangeData = false;
                    string _FieldColumnID = string.Empty;


                    XDocument xDoc = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");

                    if (xDoc.Descendants("ReportRange").FirstOrDefault() != null)
                    {
                        _range = xDoc.Descendants("ReportRange").FirstOrDefault().Value;

                        if (xDoc.Descendants("IsSelectAllRangeData").FirstOrDefault() != null)
                        {
                            bool.TryParse(xDoc.Descendants("IsSelectAllRangeData").FirstOrDefault().Value, out _isSelectAllRangeData);
                        }

                        if (!string.IsNullOrEmpty(_range) && xDoc.Descendants("RangeData").FirstOrDefault() != null || _isSelectAllRangeData)
                        {
                            /*
                            switch (_range)
                            {
                                case "ItemNumber":
                                    _itemGuids = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    if (_isSelectAllRangeData)
                                    {
                                        _itemGuids = objItemDAL.GetItemsByReportRange(_range, _itemGuids, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeData);
                                    }
                                    _DataGuids = _itemGuids;
                                    break;
                                case "SupplierPartNo":
                                    _supplierIDs = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuids = objItemDAL.GetItemsByReportRange(_range, _supplierIDs, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeData);
                                    break;
                                case "Supplier":
                                    _supplierIDs = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuids = objItemDAL.GetItemsByReportRange(_range, _supplierIDs, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeData);
                                    break;
                                case "Bin":
                                    _binIDs = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuids = objItemDAL.GetItemsByReportRange(_range, _binIDs, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeData);
                                    break;
                                case "Category":
                                    _categoryIDs = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuids = objItemDAL.GetItemsByReportRange(_range, _categoryIDs, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeData);
                                    break;
                            }
                            */

                            dictionary.Add("@DataGuids", _DataGuids ?? string.Empty);
                        }
                    }

                    if (xDoc.Descendants("FilterQOH").FirstOrDefault() != null)
                    {

                        _QOHFilter = FilterInStockQOH(objNotificationDTO.XMLValue);
                        //dictionary.Add("@QOHFilter", xDoc.Descendants("FilterQOH").FirstOrDefault().Value);
                        dictionary.Add("@QOHFilter", _QOHFilter);
                    }

                    if (xDoc.Descendants("MonthlyAverageUsage").FirstOrDefault() != null)
                        dictionary.Add("@MonthlyUsage", xDoc.Descendants("MonthlyAverageUsage").FirstOrDefault().Value);



                    if (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "instockbybin")
                    {
                        if (xDoc.Descendants("OnlyExpirationItems").FirstOrDefault() != null)
                            dictionary.Add("@OnlyExirationItems", xDoc.Descendants("OnlyExpirationItems").FirstOrDefault().Value);
                    }
                }

                else if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull")))
                {


                    PullMasterDAL objPullDAL = new PullMasterDAL(EntDTO.EnterpriseDBName);
                    string _DataGuidsPull = string.Empty;
                    string _itemGuidsPull = string.Empty;
                    string _rangePull = "";
                    string _rangeDataPull = string.Empty;
                    string _pullGuids = string.Empty;
                    string _binIDsPull = string.Empty;
                    string _supplierIDsPull = string.Empty;
                    string _categoryIDsPull = string.Empty;
                    string _manufacturerIDsPull = string.Empty;
                    bool _isSelectAllRangeDataPull = false;
                    string _FieldColumnID = string.Empty;


                    XDocument xDocPull = XDocument.Parse("<Data>" + objNotificationDTO.XMLValue + "</Data>");

                    if (xDocPull.Descendants("ReportRange").FirstOrDefault() != null)
                    {
                        _rangePull = xDocPull.Descendants("ReportRange").FirstOrDefault().Value;

                        if (xDocPull.Descendants("IsSelectAllRangeData").FirstOrDefault() != null)
                        {
                            bool.TryParse(xDocPull.Descendants("IsSelectAllRangeData").FirstOrDefault().Value, out _isSelectAllRangeDataPull);
                        }


                        if (!string.IsNullOrEmpty(_rangePull) && (xDocPull.Descendants("RangeData").FirstOrDefault() != null || _isSelectAllRangeDataPull))
                        {

                            _rangeDataPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                            _DataGuidsPull = objPullDAL.GetPullGuidsByReportRange(_rangePull, _FieldColumnID, _rangeDataPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);

                            /*
                            switch (_rangePull)
                            {
                                case "ItemNumber":
                                    _itemGuidsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _itemGuidsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "SupplierPartNo":
                                    _supplierIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _supplierIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "SupplierName":
                                    _supplierIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _supplierIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "PullBin":
                                    _binIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _binIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "CategoryName":
                                    _categoryIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _categoryIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "ManufacturerName":
                                    _manufacturerIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _manufacturerIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                                case "ManufacturerNumber":
                                    _manufacturerIDsPull = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                                    _DataGuidsPull = objPullDAL.GetPullByReportRange(_rangePull, _manufacturerIDsPull, objNotificationDTO.RoomIds, objNotificationDTO.CompanyIds, _isSelectAllRangeDataPull);
                                    break;
                            }
                            */

                            dictionary.Add("@DataGuids", _DataGuidsPull ?? string.Empty);
                        }
                    }
                }

                return dictionary;
            }
            finally
            {
                dictionary = null;
            }

        }

        private string FilterInStockQOH(string XMLValue)
        {
            string FilterQOH = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/FilterQOH");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["FQOH" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(FilterQOH))
                            {
                                FilterQOH += "," + node["FQOH" + i].InnerText;
                            }
                            else
                            {
                                FilterQOH += node["FQOH" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return FilterQOH;
        }

        private string FilterInStockReportRangeData(string XMLValue)
        {
            string ReportRangeData = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/RangeData");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["DataId" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(ReportRangeData))
                            {
                                ReportRangeData += "," + node["DataId" + i].InnerText;
                            }
                            else
                            {
                                ReportRangeData += node["DataId" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return ReportRangeData;
        }

        protected void LoadSubreport(object sender, SubreportProcessingEventArgs e)
        {
            XDocument doc = null;
            SqlConnection myConnection = null;
            SqlCommand cmd = null;
            SqlDataAdapter sqla = null;
            DataTable dt = null;
            IEnumerable<XElement> lstReportPara = null;
            List<ReportParameter> rpt = null;
            ReportParameter rpara = null;
            IEnumerable<XElement> lstQueryPara = null;
            SqlParameter slpar = null;
            XElement objReportPara = null;
            ReportDataSource rds = null;

            try
            {
                string rdlPath = string.Empty;
                rdlPath = ReportBasePath + @"/Temp" + @"\\" + e.ReportPath + ".rdlc";
                doc = XDocument.Load(rdlPath);

                string connString = doc.Descendants(ns + "ConnectString").FirstOrDefault().Value;
                myConnection = new SqlConnection();
                cmd = new SqlCommand();
                sqla = new SqlDataAdapter();
                dt = new DataTable();
                myConnection.ConnectionString = DBconectstring;

                cmd.Connection = myConnection;
                cmd.CommandText = doc.Descendants(ns + "CommandText").FirstOrDefault().Value;
                cmd.CommandType = CommandType.Text;
                if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                {
                    cmd.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                }

                lstReportPara = doc.Descendants(ns + "ReportParameter");
                rpt = new List<ReportParameter>();

                if (lstReportPara != null && lstReportPara.Count() > 0)
                {
                    foreach (var item in lstReportPara)
                    {
                        rpara = new ReportParameter();
                        rpara.Name = item.Attribute("Name").Value;
                        rpara.Values.Add(e.Parameters[rpara.Name].Values[0]);
                        rpt.Add(rpara);
                    }
                }

                lstQueryPara = doc.Descendants(ns + "QueryParameter");

                if (lstQueryPara != null && lstQueryPara.Count() > 0)
                {
                    foreach (var item in lstQueryPara)
                    {
                        slpar = new SqlParameter();
                        slpar.ParameterName = item.Attribute("Name").Value;//
                        slpar.Value = e.Parameters[slpar.ParameterName.Replace("@", "")].Values[0];
                        objReportPara = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slpar.ParameterName.Replace("@", ""));
                        if (objReportPara.Descendants(ns + "DataType") != null && objReportPara.Descendants(ns + "DataType").Count() > 0)
                        {
                            slpar.DbType = (DbType)Enum.Parse(typeof(DbType), objReportPara.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                        }
                        if (cmd.Parameters.IndexOf(slpar.ParameterName) < 0)
                            cmd.Parameters.Add(slpar);
                    }
                }
                sqla = new SqlDataAdapter(cmd);
                cmd.CommandTimeout = 600;
                sqla.Fill(dt);
                if (rptPara.ContainsKey("SortFields") && !string.IsNullOrEmpty(rptPara["SortFields"]))
                {
                    string SortFields = rptPara["SortFields"];

                    if (!string.IsNullOrEmpty(SortFields))
                    {
                        dt.DefaultView.Sort = SortFields;
                        dt = dt.DefaultView.ToTable();

                    }
                }
                rds = new ReportDataSource();
                rds.Name = doc.Descendants(ns + "DataSet").FirstOrDefault().FirstAttribute.Value;
                rds.Value = dt;
                e.DataSources.Add(rds);

                #region WI-3336 For Work Order Attachement 

                if (e.DataSourceNames.Count > 1)
                {
                    if (e.DataSourceNames[1].ToLower().Equals("datasetworkorderattachments"))
                    {
                        SqlCommand cmdWOA = new SqlCommand();
                        SqlDataAdapter sqlaWOA = new SqlDataAdapter();
                        DataTable dtWOA = new DataTable();

                        cmdWOA.Connection = myConnection;
                        cmdWOA.CommandText = "RPT_GetWorkOrderAttachments";
                        cmdWOA.CommandType = CommandType.Text;
                        if (doc.Descendants(ns + "CommandType").FirstOrDefault() != null)
                        {
                            cmdWOA.CommandType = (CommandType)Enum.Parse(typeof(CommandType), doc.Descendants(ns + "CommandType").FirstOrDefault().Value == null ? "Text" : doc.Descendants(ns + "CommandType").FirstOrDefault().Value, true);
                        }

                        IEnumerable<XElement> lstQueryParaWOA = doc.Descendants(ns + "QueryParameter");

                        if (lstQueryParaWOA != null && lstQueryParaWOA.Count() > 0)
                        {
                            foreach (var item in lstQueryParaWOA)
                            {
                                if (item.Attribute("Name").Value.ToLower().Equals("@workorderguid"))
                                {
                                    SqlParameter slparWOA = new SqlParameter();
                                    slparWOA.ParameterName = item.Attribute("Name").Value;
                                    slparWOA.Value = e.Parameters[slparWOA.ParameterName.Replace("@", "")].Values[0];
                                    XElement objReportParaWO = lstReportPara.FirstOrDefault(x => x.Attribute("Name").Value == slparWOA.ParameterName.Replace("@", ""));
                                    if (objReportParaWO.Descendants(ns + "DataType") != null && objReportParaWO.Descendants(ns + "DataType").Count() > 0)
                                    {
                                        slparWOA.DbType = (DbType)Enum.Parse(typeof(DbType), objReportParaWO.Descendants(ns + "DataType").FirstOrDefault().Value, true);
                                    }
                                    if (cmdWOA.Parameters.IndexOf(slparWOA.ParameterName) < 0)
                                        cmdWOA.Parameters.Add(slparWOA);
                                }
                            }
                        }

                        cmdWOA.CommandTimeout = 7200;
                        sqlaWOA = new SqlDataAdapter(cmdWOA);

                        sqlaWOA.Fill(dtWOA);

                        ReportDataSource rds2 = new ReportDataSource();
                        rds2.Name = "DataSetWorkOrderAttachments";
                        rds2.Value = dtWOA;
                        e.DataSources.Add(rds2);
                    }
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                doc = null;
                lstReportPara = null;
                rpt = null;
                rpara = null;
                lstQueryPara = null;
                objReportPara = null;
                rds = null;
                slpar = null;

                if (myConnection != null)
                    myConnection.Dispose();

                if (cmd != null)
                    cmd.Dispose();
                if (sqla != null)
                    sqla.Dispose();

                if (dt != null)
                    dt.Dispose();

                myConnection = null;
                cmd = null;
                sqla = null;
                dt = null;



            }


        }

        private string NewEmailBody(DataTable dt, string EmailBody, XDocument doc, string ReportResFile, long EnterpriseId, long CompanyId, long RoomId, string CultureCode)
        {
            XDocument doc1 = new XDocument(doc);
            string UpdatedEmailbody = string.Empty;
            string headerSequence = string.Empty;
            try
            {
                bool hasTotalField = true;
                List<XElement> lstColumns = doc1.Descendants(ns + "TablixColumn").ToList();
                string DatasetFields = string.Empty;
                List<XElement> lstReportField = doc1.Descendants(ns + "TablixRow").ToList();
                List<XElement> lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                List<XElement> lstDSFields = doc1.Descendants(ns + "Field").ToList();
                List<XElement> lstTablixRowGrouping = doc1.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "GroupExpression").ToList();
                List<XElement> lstGroupCSS = doc1.Descendants(ns + "Tablix").Descendants(ns + "TablixRowHierarchy").Descendants(ns + "TablixMember").ToList()[0].Descendants(ns + "TablixHeader").ToList();
                List<KeyValSelectDTO> lstGrouplist = new List<KeyValSelectDTO>();
                List<KeyValSelectDTO> lstRemainingGrlist = new List<KeyValSelectDTO>();
                if (lstDSFields.Descendants(ns + "DataField").FirstOrDefault(x => x.Value == "Total") == null)
                {
                    hasTotalField = false;
                }

                #region New Code to remove CostDecimalPoint,QuantityDecimalPoint,RoomInfo,ComapnyInfo,Int64,Guid

                if (lstDSFields != null && lstDSFields.Count > 0)
                {
                    List<XElement> IEnuDSFields = IEnuDSFields = lstDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Total"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CostDecimalPoint"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("QuantityDecimalPoint"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("RoomInfo"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CompanyInfo"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("CurrentDateTime"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("ID"))).ToList();
                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Guid"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Rate"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Rate"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax1Name"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => !(x.Attribute("Name").Value.Contains("Tax2Name"))).ToList();



                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Int64"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                        IEnuDSFields = IEnuDSFields.Where(x => x.Descendants(nsrd + "TypeName").FirstOrDefault() != null && !(x.Descendants(nsrd + "TypeName").FirstOrDefault().Value.Contains("System.Guid"))).ToList();

                    if (IEnuDSFields != null && IEnuDSFields.Count > 0)
                    {
                        lstDSFields.Remove();
                        lstDSFields = IEnuDSFields.OrderBy(e => e.Attribute("Name").Value).ToList();
                    }
                }
                #endregion

                if (lstReportField != null && lstReportField.Count > 0)
                {

                    List<XElement> lstFieldTop = lstReportField[0].Descendants(ns + "TablixCell").ToList();
                    lstFieldBottom = lstReportField[1].Descendants(ns + "TablixCell").ToList();
                    int cellcount = lstFieldBottom.Count;
                    if (!hasTotalField)
                    {
                        cellcount = cellcount - 1;
                    }
                    string tablixcelltop = string.Empty;
                    string tablixcellBottom = string.Empty;
                    int rowGroupCount = lstTablixRowGrouping.Count;
                    int tdcount = 0;
                    UpdatedEmailbody += "<table style='margin-left: 0px; width: 99%;'  border='1' cellpadding='0' cellspacing ='0' > ";
                    UpdatedEmailbody += "<thead><tr role='row'>";
                    for (int i = 0; i <= rowGroupCount - 1; i++)
                    {

                        string strcolumnwidth = lstGroupCSS[i].Descendants(ns + "Size").FirstOrDefault().Value;
                        string fontstyle = string.Empty;
                        string fontfamily = string.Empty;
                        string fontweight = string.Empty;
                        string Textalign = string.Empty;

                        if (lstGroupCSS[i].Descendants(ns + "FontStyle").Any())
                        {
                            fontstyle = lstGroupCSS[i].Descendants(ns + "FontStyle").FirstOrDefault().Value;
                        }
                        if (lstGroupCSS[i].Descendants(ns + "FontFamily").Any())
                        {
                            fontfamily = lstGroupCSS[i].Descendants(ns + "FontFamily").FirstOrDefault().Value;
                        }
                        if (lstGroupCSS[i].Descendants(ns + "FontWeight").Any())
                        {
                            fontweight = lstGroupCSS[i].Descendants(ns + "FontWeight").FirstOrDefault().Value;
                        }
                        if (lstGroupCSS[i].Descendants(ns + "TextAlign").Any())
                        {
                            Textalign = lstGroupCSS[i].Descendants(ns + "TextAlign").FirstOrDefault().Value;
                        }
                        KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();

                        tablixcellBottom = Convert.ToString(lstTablixRowGrouping[i].Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        string Header = GetField(tablixcellBottom, ReportResFile);
                        if (!string.IsNullOrWhiteSpace(headerSequence))
                        {
                            //  headerSequence += "," + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        else
                        {
                            // headerSequence = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        // UpdatedEmailbody += "<td id='tddropline_" + tdcount + "'><span class='ChangeText'  " + GetSpanStyle(fontstyle, fontweight) + ">" + Header + "</span></td>";
                        objKeyValDTO.key = tablixcellBottom;
                        objKeyValDTO.value = GetField(tablixcellBottom, ReportResFile);
                        objKeyValDTO.IsSelect = true;
                        lstGrouplist.Add(objKeyValDTO);
                        //tablixcellBottom = Convert.ToString(lstFieldBottom[i].Descendants(ns + "Value").FirstOrDefault().Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        //strHeaderFields += "<td id='tddropline_" + i + "' onmousemove='doResize(this,event)'  onmouseover='doResize(this,event)' onmouseout='doneResizing()' " + GetTDStyleWithWidth(Textalign, ConvertInchToPx(strcolumnwidth)) + ";'><div class='divLineDrag'>&nbsp;</div><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + GetField(tablixcelltop, ReportResFile) + "</span><input type='hidden' value='" + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "") + "' id='hdnlineitem_" + (i + 1) + "' /><img id='imgdelete' style='float: right;' alt='Remove' src='../../Content/images/deletereport_icon.png'  onclick='RemovelineItem(this)'></img></td>";
                        tdcount += 1;
                    }
                    for (int i = 0; i <= cellcount - 1; i++)
                    {

                        string strcolumnwidth = lstColumns[i].Value;

                        string fontstyle = string.Empty;
                        string fontweight = string.Empty;
                        string Textalign = string.Empty;
                        string fontfamily = string.Empty;

                        if (lstFieldTop[i].Descendants(ns + "FontStyle").Any())
                        {
                            fontstyle = lstFieldTop[i].Descendants(ns + "FontStyle").FirstOrDefault().Value;
                        }
                        if (lstFieldTop[i].Descendants(ns + "FontFamily").Any())
                        {
                            fontfamily = lstFieldTop[i].Descendants(ns + "FontFamily").FirstOrDefault().Value;
                        }

                        if (lstFieldTop[i].Descendants(ns + "FontWeight").Any())
                        {
                            fontweight = lstFieldTop[i].Descendants(ns + "FontWeight").FirstOrDefault().Value;
                        }
                        if (lstFieldTop[i].Descendants(ns + "TextAlign").Any())
                        {
                            Textalign = lstFieldTop[i].Descendants(ns + "TextAlign").FirstOrDefault().Value;
                        }
                        if (lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault() != null)
                        {
                            tablixcelltop = Convert.ToString(lstFieldTop[i].Descendants(ns + "Value").FirstOrDefault().Value);
                            tablixcellBottom = Convert.ToString(lstFieldBottom[i].Descendants(ns + "Value").FirstOrDefault().Value).Replace("=Fields!", "").Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        string Header = GetField(tablixcelltop, ReportResFile);
                        if (!string.IsNullOrWhiteSpace(headerSequence))
                        {
                            headerSequence += "," + tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        else
                        {
                            headerSequence = tablixcellBottom.Replace("=Parameters!BarcodeURL.Value+Fields!", "").Replace(".Value", "");
                        }
                        UpdatedEmailbody += "<td id='tddropline_" + tdcount + "' style='text-align: left;' ><span class='ChangeText' " + GetSpanStyle(fontstyle, fontweight) + " >" + Header + "</span></td>";
                        tdcount += 1;
                        //if (lstReportGroupMasterDTO.Any(x => x.FieldName == tablixcellBottom))

                        DataColumnCollection columns = dt.Columns;
                        //if (lstReportGroupMasterDTO.Any(x => x.FieldName == DatasetFields))
                        if (columns.Contains(DatasetFields))
                        {
                            KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();

                        }
                        else
                        {
                            KeyValSelectDTO objKeyValDTO = new KeyValSelectDTO();
                            objKeyValDTO.key = tablixcellBottom;
                            objKeyValDTO.value = GetField(tablixcellBottom, ReportResFile);
                            objKeyValDTO.IsSelect = false;
                            if (lstGrouplist.FindIndex(x => x.key == objKeyValDTO.key && x.value == objKeyValDTO.value) < 0)
                                lstRemainingGrlist.Add(objKeyValDTO);
                        }
                    }
                    UpdatedEmailbody += "</tr></thead>";
                    UpdatedEmailbody += "<tbody>";
                    string AlterNativeRowStyle = "Style='background:#DBD9D9;'";
                    for (int cnt = 0; cnt < dt.Rows.Count; cnt++)
                    {
                        string RowStyle = string.Empty;
                        if (cnt % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        else
                        {

                        }
                        UpdatedEmailbody += @"<tr " + RowStyle + @" >";
                        string[] Columns = headerSequence.Split(',');
                        foreach (string column in Columns)
                        {
                            string Value = Convert.ToString(dt.Rows[cnt][column]);
                            UpdatedEmailbody += " <td>" + Value + "</td>";
                        }
                        UpdatedEmailbody += "</tr>";
                    }
                    if (dt.Rows.Count == 0)
                    {
                        string resourceBaseFilePath = ConfigurationManager.AppSettings["ResourceBaseFilePath"];
                        var commonResourceFilePath = ResourceRead.GetResourceFileFullPath(resourceBaseFilePath, "ResCommon", CultureCode, EnterpriseId, CompanyId);
                        string msgNoDataFound = ResourceRead.GetResourceValueByKeyAndFullFilePath("msgNoDataFound", commonResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResCommon", CultureCode);
                        string[] Columns = headerSequence.Split(',');
                        string Str = msgNoDataFound;
                        string RowStyle = string.Empty;
                        UpdatedEmailbody += @"<tr " + RowStyle + @" >
                        <td colspan=" + Columns.Length + @">
                            " + Str + @"
                        </td>
                    </tr>";
                    }
                    UpdatedEmailbody += "</tbody>";
                    UpdatedEmailbody += "</table>";

                }
                EmailBody = EmailBody.Replace("@@table@@", UpdatedEmailbody).Replace("@@TABLE@@", UpdatedEmailbody).Replace("@@Table@@", UpdatedEmailbody);
            }
            catch (Exception)
            {
                return EmailBody;
            }
            return EmailBody;
        }

        public string GetField(string Key, string FileName)
        {
            try
            {
                string KeyVal = string.Empty;

                if (Key.ToLower().Contains("udf"))
                {
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
                }
                else
                {
                    KeyVal = ResourceHelper.GetResourceValue(Key, FileName);

                }
                return KeyVal;
            }
            catch (Exception)
            {
                return Key;
            }

        }

        public string GetSpanStyle(string fontstyle, string fontweight)
        {
            try
            {
                string retStyle = string.Empty;
                retStyle = "style='";
                if (!string.IsNullOrEmpty(fontstyle))
                {
                    retStyle += "font-style:" + fontstyle + "; ";
                }
                if (!string.IsNullOrEmpty(fontweight))
                {
                    retStyle += "font-weight:" + fontweight + "; ";
                }
                retStyle += "'";
                return retStyle;
            }
            catch
            {
                return "style=''";
            }
        }

        private void CreateReportMail(List<eMailAttachmentDTO> objeMailAttchList, string MailSubject, string MessageBody, string ToEmailAddress, NotificationDTO objRoomScheduleDetailDTO, EnterpriseDTO objEnterpriseDTO)
        {
            eMailMasterDAL objUtils = null;
            try
            {
                //string StrSubject = Reportname + " Report.";
                //if (string.IsNullOrEmpty(ToEmailAddress))
                //    return;
                //ToEmailAddress = "niraj_dave@semaphore-software.com";

                string strCCAddress = "";

                if ((objeMailAttchList != null && objeMailAttchList.Count() > 0) || objRoomScheduleDetailDTO.SendEmptyEmail)
                {
                    objUtils = new eMailMasterDAL(base.DataBaseName);
                    eTurnsRegionInfo objeTurnsRegionInfo = null;
                    if (objRoomScheduleDetailDTO != null)
                    {
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(objEnterpriseDTO.EnterpriseDBName);
                        objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objRoomScheduleDetailDTO.RoomID, objRoomScheduleDetailDTO.CompanyID, -1);

                        string DateTimeFormat = "MM/dd/yyyy";
                        DateTime TZDateTimeNow = DateTime.UtcNow;
                        if (objeTurnsRegionInfo != null)
                        {
                            DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                            TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                        }
                        if (MailSubject != null)
                        {
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                            MailSubject = Regex.Replace(MailSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.CompanyName))
                            {
                                MailSubject = Regex.Replace(MailSubject, "@@COMPANYNAME@@", objRoomScheduleDetailDTO.CompanyName, RegexOptions.IgnoreCase);
                            }
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.RoomName))
                            {
                                MailSubject = Regex.Replace(MailSubject, "@@ROOMNAME@@", objRoomScheduleDetailDTO.RoomName, RegexOptions.IgnoreCase);
                            }

                            MailSubject = Regex.Replace(MailSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                        }

                        if (MessageBody != null)
                        {
                            string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                            //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                            MessageBody = Regex.Replace(MessageBody, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.CompanyName))
                            {
                                MessageBody = Regex.Replace(MessageBody, "@@COMPANYNAME@@", objRoomScheduleDetailDTO.CompanyName, RegexOptions.IgnoreCase);
                            }
                            if (!string.IsNullOrWhiteSpace(objRoomScheduleDetailDTO.RoomName))
                            {
                                MessageBody = Regex.Replace(MessageBody, "@@ROOMNAME@@", objRoomScheduleDetailDTO.RoomName, RegexOptions.IgnoreCase);
                            }
                        }
                        MessageBody = Regex.Replace(MessageBody, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);

                    }

                    objUtils.eMailToSend(ToEmailAddress, strCCAddress, MailSubject, MessageBody, objEnterpriseDTO.ID, objRoomScheduleDetailDTO.CompanyID, objRoomScheduleDetailDTO.RoomID, 0, objeMailAttchList);
                }
                AddToReportMailLog(objEnterpriseDTO, objRoomScheduleDetailDTO, ToEmailAddress, objeMailAttchList);
            }
            finally
            {
                objUtils = null;
            }
        }

        public void AddToReportMailLog(EnterpriseDTO objEnterpriseDTO, NotificationDTO objRoomScheduleDetailDTO, string ToEmailAddress, List<eMailAttachmentDTO> objeMailAttchList)
        {
            ReportMasterDAL objReportMasterDAL = new ReportMasterDAL(objEnterpriseDTO.EnterpriseDBName);
            ReportMailLogDTO objReportMailLogDTO = new ReportMailLogDTO();
            try
            {
                objReportMailLogDTO.Id = 0;
                objReportMailLogDTO.ReportID = objRoomScheduleDetailDTO.ReportID ?? 0;
                objReportMailLogDTO.ScheduleID = objRoomScheduleDetailDTO.RoomScheduleID;
                objReportMailLogDTO.CompanyID = objRoomScheduleDetailDTO.CompanyID;
                objReportMailLogDTO.RoomID = objRoomScheduleDetailDTO.RoomID;
                objReportMailLogDTO.SendDate = objRoomScheduleDetailDTO.NextRunDate ?? DateTime.UtcNow;
                objReportMailLogDTO.SendEmailAddress = ToEmailAddress;
                objReportMailLogDTO.NotificationID = objRoomScheduleDetailDTO.ID;
                if (objeMailAttchList != null)
                {
                    objReportMailLogDTO.AttachmentCount = objeMailAttchList.Count;
                }
                objReportMasterDAL.InsertMailLog(objReportMailLogDTO);
            }
            finally
            {
                objReportMasterDAL = null;
                objReportMailLogDTO = null;
            }
        }


        private string ConvertImageToPNG(string BasePath, string InnerPath)
        {
            string returnImagePath = string.Empty;
            System.Drawing.Image bmpImageToConvert = null;
            System.Drawing.Image bmpNewImage = null;
            Graphics gfxNewImage = null;
            string[] arrPath = null;
            try
            {
                string path = BasePath + InnerPath;
                if (!string.IsNullOrEmpty(InnerPath))
                {
                    arrPath = InnerPath.Split(new string[1] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrPath != null && arrPath.Length > 0)
                    {
                        if (arrPath[arrPath.Length - 1].ToLower().Contains(".png"))
                        {
                            return InnerPath;
                        }
                        else
                        {
                            string strNewFileName = arrPath[arrPath.Length - 1];
                            if (strNewFileName.LastIndexOf(".") > 0)
                            {
                                strNewFileName = strNewFileName.Substring(0, strNewFileName.LastIndexOf("."));

                                for (int i = 0; i < arrPath.Length - 1; i++)
                                {
                                    if (i > 0)
                                        returnImagePath += "\\";

                                    returnImagePath += arrPath[i];
                                }

                                returnImagePath += "\\" + strNewFileName + ".png";
                            }
                        }
                    }
                }

                if (!System.IO.File.Exists(BasePath + returnImagePath) && System.IO.File.Exists(path))
                {
                    if (path.LastIndexOf(".svg") == (path.Length - 4))
                    {
                        var svgDocument = Svg.SvgDocument.Open(path);
                        using (var smallBitmap = svgDocument.Draw())
                        {
                            var width = smallBitmap.Width;
                            var height = smallBitmap.Height;
                            //if (width != 135)// I resize my bitmap
                            //{
                            //    width = 135;
                            //    height = 135 / smallBitmap.Width * height;
                            //}
                            using (var bitmap = svgDocument.Draw(width, height))//I render again
                            {
                                bitmap.Save(BasePath + returnImagePath, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                    else
                    {
                        bmpImageToConvert = System.Drawing.Image.FromFile(path);
                        bmpNewImage = new Bitmap(135, 75);
                        gfxNewImage = Graphics.FromImage(bmpNewImage);
                        gfxNewImage.DrawImage(bmpImageToConvert, new System.Drawing.Rectangle(0, 0, bmpNewImage.Width, bmpNewImage.Height), 0, 0, bmpImageToConvert.Width, bmpImageToConvert.Height, GraphicsUnit.Pixel);
                        gfxNewImage.Dispose();
                        bmpImageToConvert.Dispose();

                        bmpNewImage.Save(BasePath + returnImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                else if (!System.IO.File.Exists(BasePath + returnImagePath))
                {
                    string NoImagePath = BasePath + "\\Uploads\\EnterpriseLogos\\";
                    NoImagePath += "NoEntImage.png";

                    if (!System.IO.File.Exists(NoImagePath))
                    {
                        bmpNewImage = new Bitmap(135, 75);
                        System.Drawing.Font f = new System.Drawing.Font("Verdana", 12);
                        gfxNewImage = Graphics.FromImage(bmpNewImage);
                        gfxNewImage.DrawString(" ", f, Brushes.Black, new RectangleF(0, 0, 135, 75));
                        gfxNewImage.Dispose();
                        bmpNewImage.Save(NoImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    returnImagePath = NoImagePath;
                }
                return "/" + returnImagePath.Replace(BasePath, "").Replace("\\", "/");
            }
            finally
            {
                returnImagePath = string.Empty;
                bmpImageToConvert = null;
                bmpNewImage = null;
                gfxNewImage = null;
                arrPath = null;

            }
        }


        public void SaveAttachMentsTosFTP(NotificationDTO objNotificationDTO, EnterpriseDTO objEnterpriseDTO, List<eMailAttachmentDTO> lstAttachments)
        {
            //string FTPReportSavePath = Convert.ToString(ConfigurationManager.AppSettings["FTPReportSavePath"]);
            //List<string> ZipFilePaths = SFTPHelper.AddFileToTemplLocation(objNotificationDTO, objEnterpriseDTO, lstAttachments);
            //FTPMasterDTO objFTP = objNotificationDTO.FtpDetails;
            //if (ZipFilePaths != null && ZipFilePaths.Count > 0)
            //{
            //    foreach (string strpath in ZipFilePaths)
            //    {
            //        SFTPHelper.UploadFileToSFTPServer(objFTP.ServerAddress, objFTP.Port, objFTP.UserName, objFTP.Password, FTPReportSavePath, strpath);
            //    }
            //}
        }

        #endregion

        #endregion

        #endregion

        public void UpdateItemStockOuteMailtoSend(long EnterPriceID, long CompanyId, long RoomId, long MasterNotificationID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string eTurnsMasterDBName = DbConnectionHelper.GetETurnsMasterDBName();
                var params1 = new SqlParameter[] {  new SqlParameter("@EnterPriceID", EnterPriceID),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@MasterNotificationID", MasterNotificationID)};
                context.Database.SqlQuery<object>("EXEC [" + eTurnsMasterDBName + "].[dbo].[InsertorUpdateItemStockOutMailSendHistory] @EnterPriceID,@CompanyId,@RoomId,@MasterNotificationID", params1).ToList();
            }
        }

        public List<NotificationDTO> GetAllSchedulesByEmailTemplateByID(long EmailTemplateID, long RoomID, long CompanyID, string Culture, long MasterNotificationID)
        {

            List<NotificationDTO> lstSchedules = new List<NotificationDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.Notifications
                                join rpt in context.ReportMasters on nm.ReportID equals rpt.ID into nm_rpt_join
                                from nm_rpt in nm_rpt_join.DefaultIfEmpty()
                                join tm in context.EmailTemplates on nm.EmailTemplateID equals tm.ID into nm_tm_join
                                from nm_tm in nm_tm_join.DefaultIfEmpty()
                                join sm in context.RoomSchedules on nm.RoomScheduleID equals sm.ScheduleID into sm_tm_join
                                from sm_tm in sm_tm_join.DefaultIfEmpty()
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where nm.RoomId == RoomID && nm.CompanyId == CompanyID && nm.IsDeleted == false && nm.IsActive == true && nm.EmailTemplateID == EmailTemplateID && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                                && nm.ID == MasterNotificationID
                                select new NotificationDTO
                                {
                                    AttachmentReportIDs = nm.AttachmentReportIDs,
                                    AttachmentTypes = nm.AttachmentTypes,
                                    CompanyID = nm.CompanyId,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    EmailAddress = nm.EmailAddress,
                                    EmailTemplateID = nm.EmailTemplateID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsDeleted = nm.IsDeleted,
                                    NextRunDate = nm.NextRunDate,
                                    ReportID = nm.ReportID,
                                    RoomID = nm.RoomId,
                                    RoomScheduleID = nm.RoomScheduleID,
                                    ScheduleFor = nm.ScheduleFor,
                                    ScheduleName = sm_tm.ScheduleName,
                                    SupplierIds = nm.SupplierIds,
                                    TemplateName = nm_tm.TemplateName,
                                    Updated = nm.Updated,
                                    UpdatedBy = nm.UpdatedBy,
                                    ReportName = nm_rpt.ReportName,
                                    ReportDataSince = nm.ReportDataSince,
                                    ReportDataSelectionType = nm.ReportDataSelectionType,
                                    RoomName = rm.RoomName,
                                    CompanyName = cm.Name,
                                    FTPId = nm.FTPId,
                                    NotificationMode = nm.NotificationMode,
                                    ScheduleTime = nm.ScheduleTime,
                                    SendEmptyEmail = nm.SendEmptyEmail ?? false,
                                    HideHeader = nm.HideHeader,
                                    ShowSignature = nm.ShowSignature,
                                    SortSequence = nm.SortSequence,
                                    XMLValue = nm.XMLValue,
                                    CompanyIds = nm.CompanyIDs,
                                    RoomIds = nm.RoomIDs,
                                    Status = nm.Status,
                                    Range = nm.Range
                                }).ToList();

                if (lstSchedules != null)
                {
                    lstSchedules.ForEach(t =>
                    {
                        t.ReportMasterDTO = (from nm_rpt in context.ReportMasters
                                             where nm_rpt.ID == t.ReportID
                                             select new ReportBuilderDTO
                                             {
                                                 ChildReportName = nm_rpt.SubReportFileName,
                                                 CompanyID = nm_rpt.CompanyID,
                                                 Created = nm_rpt.CreatedOn,
                                                 CreatedBy = nm_rpt.CreatedBy,
                                                 Days = nm_rpt.Days,
                                                 FromDate = nm_rpt.FromDate,
                                                 GroupName = nm_rpt.GroupName,
                                                 ID = nm_rpt.ID,
                                                 IsBaseReport = nm_rpt.IsBaseReport,
                                                 IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                 IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                 IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                 IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                 IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                 IsPrivate = nm_rpt.IsPrivate,
                                                 LastUpdatedBy = nm_rpt.UpdatedBy,
                                                 MasterReportResFile = nm_rpt.MasterReportResFile,
                                                 ModuleName = nm_rpt.ModuleName,
                                                 ParentID = nm_rpt.ParentID,
                                                 PrivateUserID = nm_rpt.PrivateUserID,
                                                 ReportFileName = nm_rpt.ReportFileName,
                                                 ReportName = nm_rpt.ReportName,
                                                 ReportType = nm_rpt.ReportType,
                                                 RoomID = nm_rpt.RoomID,
                                                 SortColumns = nm_rpt.SortColumns,
                                                 SubReportFileName = nm_rpt.SubReportFileName,
                                                 SubReportResFile = nm_rpt.SubReportResFile,
                                                 ToDate = nm_rpt.ToDate,
                                                 ToEmailAddress = nm_rpt.ToEmailAddress,
                                                 Updated = nm_rpt.UpdatedON,
                                                 ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                             }).FirstOrDefault();

                        long attachedreportID = !string.IsNullOrWhiteSpace(t.AttachmentReportIDs) ? Convert.ToInt64(t.AttachmentReportIDs.Trim().Split(',').First()) : 0;

                        t.AttachedReportMasterDTO = (from nm_rpt in context.ReportMasters
                                                     where nm_rpt.ID == attachedreportID
                                                     select new ReportBuilderDTO
                                                     {
                                                         ChildReportName = nm_rpt.SubReportFileName,
                                                         CompanyID = nm_rpt.CompanyID,
                                                         Created = nm_rpt.CreatedOn,
                                                         CreatedBy = nm_rpt.CreatedBy,
                                                         Days = nm_rpt.Days,
                                                         FromDate = nm_rpt.FromDate,
                                                         GroupName = nm_rpt.GroupName,
                                                         ID = nm_rpt.ID,
                                                         IsBaseReport = nm_rpt.IsBaseReport,
                                                         IsIncludeDateRange = nm_rpt.IsIncludeDateRange,
                                                         IsIncludeGrandTotal = nm_rpt.IsIncludeGrandTotal,
                                                         IsIncludeGroup = nm_rpt.IsIncludeGroup,
                                                         IsIncludeSubTotal = nm_rpt.IsIncludeSubTotal,
                                                         IsIncludeTotal = nm_rpt.IsIncludeTotal,
                                                         IsPrivate = nm_rpt.IsPrivate,
                                                         LastUpdatedBy = nm_rpt.UpdatedBy,
                                                         MasterReportResFile = nm_rpt.MasterReportResFile,
                                                         ModuleName = nm_rpt.ModuleName,
                                                         ParentID = nm_rpt.ParentID,
                                                         PrivateUserID = nm_rpt.PrivateUserID,
                                                         ReportFileName = nm_rpt.ReportFileName,
                                                         ReportName = nm_rpt.ReportName,
                                                         ReportType = nm_rpt.ReportType,
                                                         RoomID = nm_rpt.RoomID,
                                                         SortColumns = nm_rpt.SortColumns,
                                                         SubReportFileName = nm_rpt.SubReportFileName,
                                                         SubReportResFile = nm_rpt.SubReportResFile,
                                                         ToDate = nm_rpt.ToDate,
                                                         ToEmailAddress = nm_rpt.ToEmailAddress,
                                                         Updated = nm_rpt.UpdatedON,
                                                         ISEnterpriseReport = nm_rpt.ISEnterpriseReport
                                                     }).FirstOrDefault();

                        t.SchedulerParams = (from sm_tm in context.RoomSchedules
                                             where sm_tm.ScheduleID == t.RoomScheduleID
                                             select new SchedulerDTO
                                             {
                                                 AssetToolID = sm_tm.AssetToolID,
                                                 CompanyId = sm_tm.CompanyId ?? 0,
                                                 Created = sm_tm.Created,
                                                 CreatedBy = sm_tm.CreatedBy ?? 0,
                                                 DailyRecurringDays = sm_tm.DailyRecurringDays,
                                                 DailyRecurringType = sm_tm.DailyRecurringType,
                                                 HourlyAtWhatMinute = sm_tm.HourlyAtWhatMinute ?? 0,
                                                 HourlyRecurringHours = sm_tm.HourlyRecurringHours ?? 0,
                                                 IsDeleted = sm_tm.IsDeleted,
                                                 IsScheduleActive = sm_tm.IsScheduleActive,
                                                 LastUpdatedBy = sm_tm.LastUpdatedBy ?? 0,
                                                 MonthlyDateOfMonth = sm_tm.MonthlyDateOfMonth,
                                                 MonthlyDayOfMonth = sm_tm.MonthlyDayOfMonth,
                                                 MonthlyRecurringMonths = sm_tm.MonthlyRecurringMonths,
                                                 MonthlyRecurringType = sm_tm.MonthlyRecurringType,
                                                 NextRunDate = sm_tm.NextRunDate,
                                                 ReportDataSelectionType = sm_tm.ReportDataSelectionType,
                                                 ReportDataSince = sm_tm.ReportDataSince,
                                                 ReportID = sm_tm.ReportID,
                                                 RoomId = sm_tm.RoomId ?? 0,
                                                 ScheduledBy = sm_tm.ScheduledBy,
                                                 LoadSheduleFor = sm_tm.ScheduleFor,
                                                 ScheduleID = sm_tm.ScheduleID,
                                                 ScheduleMode = sm_tm.ScheduleMode,
                                                 ScheduleName = sm_tm.ScheduleName,
                                                 ScheduleRunDateTime = sm_tm.ScheduleRunTime,
                                                 SubmissionMethod = sm_tm.SubmissionMethod,
                                                 SupplierId = sm_tm.SupplierId ?? 0,
                                                 Updated = sm_tm.Updated,
                                                 WeeklyOnFriday = sm_tm.WeeklyOnFriday,
                                                 WeeklyOnMonday = sm_tm.WeeklyOnMonday,
                                                 WeeklyOnSaturday = sm_tm.WeeklyOnSaturday,
                                                 WeeklyOnSunday = sm_tm.WeeklyOnSunday,
                                                 WeeklyOnThursday = sm_tm.WeeklyOnThursday,
                                                 WeeklyOnTuesday = sm_tm.WeeklyOnTuesday,
                                                 WeeklyOnWednesday = sm_tm.WeeklyOnWednesday,
                                                 WeeklyRecurringWeeks = sm_tm.WeeklyRecurringWeeks,
                                             }).FirstOrDefault();

                        t.EmailTemplateDetail = (from nm_tm in context.EmailTemplates
                                                 where nm_tm.ID == t.EmailTemplateID
                                                 select new EmailTemplateDTO
                                                 {
                                                     CompanyId = nm_tm.CompanyId,
                                                     Created = nm_tm.Created,
                                                     CreatedBy = nm_tm.CreatedBy,
                                                     ID = nm_tm.ID,
                                                     LastUpdatedBy = nm_tm.LastUpdatedBy,
                                                     RoomId = nm_tm.RoomId,
                                                     TemplateName = nm_tm.TemplateName,
                                                     Updated = nm_tm.Updated,
                                                     lstEmailTemplateDtls = (from etd in context.EmailTemplateDetails
                                                                             join rl in context.ResourceLaguages on etd.ResourceLaguageId equals rl.ID
                                                                             join rs in context.RegionalSettings on etd.RoomId equals rs.RoomId
                                                                             where etd.RoomId == t.RoomID && etd.EmailTemplateId == nm_tm.ID && rl.Culture == Culture && etd.NotificationID == t.ID //(t.ScheduleFor == 5 ? etd.NotificationID == t.ID : true)
                                                                             select new EmailTemplateDetailDTO()
                                                                             {
                                                                                 CompanyID = etd.CompanyID,
                                                                                 Created = etd.Created,
                                                                                 CreatedBy = etd.CreatedBy,
                                                                                 CultureCode = rs.CultureCode,
                                                                                 EmailTempateId = etd.EmailTemplateId,
                                                                                 ID = etd.ID,
                                                                                 LastUpdatedBy = etd.LastUpdatedBy,
                                                                                 MailBodyText = etd.MailBodyText,
                                                                                 MailSubject = etd.MailSubject,
                                                                                 NotificationID = etd.NotificationID,
                                                                                 ResourceLaguageId = etd.ResourceLaguageId,
                                                                                 RoomId = etd.RoomId,
                                                                                 Updated = etd.Updated
                                                                             }).AsEnumerable()
                                                 }).FirstOrDefault();
                        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        t.objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(t.RoomID, t.CompanyID, t.CreatedBy);
                        if ((t.FTPId ?? 0) > 0)
                        {
                            t.FtpDetails = new SFTPDAL(base.DataBaseName).GetFtpByID(t.FTPId ?? 0, t.RoomID, t.CompanyID);
                        }
                    });
                }
            }

            return lstSchedules;
        }
        public List<NotificationDTO> GetNotificationMasterChangeLog(string IDs)
        {
            //IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationDTO>("exec [GetNotificationMasterChangeLog] @ID", params1).ToList();
            }
        }
        public SchedulerDTO GetRoomSchedulesBySupplierID(long SupplierID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierID", SupplierID),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SchedulerDTO>("EXEC [GetRoomSchedulesBySupplierID] @SupplierID, @RoomID, @CompanyID", params1).FirstOrDefault();
            }
        }

        public List<NotificationDTO> GetNotificationByIDsNormal(string IDs, string CurrentCulture)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@currentculture", CurrentCulture) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NotificationDTO>("exec [GetNotificationByIDsNormal] @ID,@currentculture", params1).ToList();
            }
        }

        public SchedulerDTO GetScheduleByRoomScheduleFor(long RoomID, long CompanyID, int ScheduleFor)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from rs in context.RoomSchedules
                                   where rs.IsDeleted == false && rs.RoomId == RoomID && rs.CompanyId == CompanyID
                                   && rs.ScheduleFor == ScheduleFor
                                   select new SchedulerDTO()
                                   {
                                       ScheduleID = rs.ScheduleID,
                                       SupplierId = rs.SupplierId ?? 0,
                                       ScheduleMode = rs.ScheduleMode,
                                       DailyRecurringType = rs.DailyRecurringType,
                                       DailyRecurringDays = rs.DailyRecurringDays,
                                       WeeklyRecurringWeeks = rs.WeeklyRecurringWeeks,
                                       WeeklyOnMonday = rs.WeeklyOnMonday,
                                       WeeklyOnTuesday = rs.WeeklyOnTuesday,
                                       WeeklyOnWednesday = rs.WeeklyOnWednesday,
                                       WeeklyOnThursday = rs.WeeklyOnThursday,
                                       WeeklyOnFriday = rs.WeeklyOnFriday,
                                       WeeklyOnSaturday = rs.WeeklyOnSaturday,
                                       WeeklyOnSunday = rs.WeeklyOnSunday,
                                       MonthlyRecurringType = rs.MonthlyRecurringType,
                                       MonthlyDateOfMonth = rs.MonthlyDateOfMonth,
                                       MonthlyRecurringMonths = rs.MonthlyRecurringMonths,
                                       MonthlyDayOfMonth = rs.MonthlyDayOfMonth,
                                       SubmissionMethod = rs.SubmissionMethod,
                                       ScheduleRunDateTime = rs.ScheduleRunTime,
                                       LoadSheduleFor = rs.ScheduleFor,
                                       RoomId = rs.RoomId ?? 0,
                                       CreatedBy = rs.CreatedBy ?? 0,
                                       Created = rs.Created,
                                       LastUpdatedBy = rs.LastUpdatedBy ?? 0,
                                       Updated = rs.Updated,
                                       CompanyId = rs.CompanyId ?? 0,
                                       IsScheduleActive = rs.IsScheduleActive,
                                       NextRunDate = rs.NextRunDate,
                                       AssetToolID = rs.AssetToolID,
                                       ReportID = rs.ReportID,
                                       ReportDataSelectionType = rs.ReportDataSelectionType,
                                       ReportDataSince = rs.ReportDataSince,
                                       HourlyRecurringHours = rs.HourlyRecurringHours ?? 0,
                                       HourlyAtWhatMinute = rs.HourlyAtWhatMinute ?? 0,
                                       ScheduledBy = rs.ScheduledBy,
                                       IsDeleted = rs.IsDeleted,
                                       ScheduleName = rs.ScheduleName,
                                       BinNumber = rs.ScheduleName,
                                       ScheduleTime = rs.ScheduleTime

                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                }
            }
            return objSchedulerDTO;
        }

        public void DeleteAllNotificationsByRoomID(long RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId) };
                    context.Database.SqlQuery<object>("EXEC [SP_DeleteAllNotificationsByRoomID] @RoomId", params1).ToList();
                }
                catch (Exception ex)
                {
                }
            }
        }

    }

}

