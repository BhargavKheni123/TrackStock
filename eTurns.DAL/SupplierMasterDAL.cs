using Dynamite.Data.Extensions;
using Dynamite.Extensions;
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
    public partial class SupplierMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public SupplierMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public SupplierMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]


        public Int64 Insert(SupplierMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierMaster obj = new SupplierMaster();

                obj.ID = 0;
                obj.SupplierName = objDTO.SupplierName;
                obj.SupplierColor = objDTO.SupplierColor;
                obj.Description = objDTO.Description;
                //obj.AccountNo = objDTO.AccountNo;
                obj.ReceiverID = objDTO.ReceiverID;
                obj.Address = objDTO.Address;
                obj.City = objDTO.City;
                obj.State = objDTO.State;
                obj.ZipCode = objDTO.ZipCode;
                obj.Country = objDTO.Country;
                obj.Contact = objDTO.Contact;
                obj.Phone = objDTO.Phone;
                obj.Fax = objDTO.Fax;
                obj.Email = objDTO.Email;
                obj.IsEmailPOInBody = objDTO.IsEmailPOInBody;
                obj.IsEmailPOInPDF = objDTO.IsEmailPOInPDF;
                obj.IsEmailPOInCSV = objDTO.IsEmailPOInCSV;
                obj.IsEmailPOInX12 = objDTO.IsEmailPOInX12;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                obj.BranchNumber = objDTO.BranchNumber;
                obj.MaximumOrderSize = objDTO.MaximumOrderSize;
                obj.IsSendtoVendor = objDTO.IsSendtoVendor;
                obj.IsVendorReturnAsn = objDTO.IsVendorReturnAsn;
                obj.IsSupplierReceivesKitComponents = objDTO.IsSupplierReceivesKitComponents;
                obj.POAutoSequence = objDTO.POAutoSequence;
                obj.NextOrderNo = objDTO.NextOrderNo;
                obj.ScheduleType = objDTO.ScheduleType;
                obj.Days = objDTO.Days;
                obj.WeekDays = objDTO.WeekDays;
                obj.MonthDays = objDTO.MonthDays;
                obj.ScheduleTime = objDTO.ScheduleTime;
                obj.IsAutoGenerate = objDTO.IsAutoGenerate;
                obj.IsAutoGenerateSubmit = objDTO.IsAutoGenerateSubmit;
                obj.isForBOM = objDTO.isForBOM;
                obj.PullPurchaseNumberType = objDTO.PullPurchaseNumberType;
                obj.LastPullPurchaseNumberUsed = objDTO.LastPullPurchaseNumberUsed;
                obj.RefBomId = objDTO.RefBomId;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.DefaultOrderRequiredDays = objDTO.DefaultOrderRequiredDays;
                obj.POAutoNrFixedValue = objDTO.POAutoNrFixedValue;
                obj.PullPurchaseNrFixedValue = objDTO.PullPurchaseNrFixedValue;

                obj.ImageExternalURL = objDTO.ImageExternalURL;
                obj.ImageType = objDTO.ImageType;
                obj.SupplierImage = objDTO.SupplierImage;
                obj.IsOrderReleaseNumberEditable = objDTO.IsOrderReleaseNumberEditable;
                obj.POAutoNrReleaseNumber = objDTO.POAutoNrReleaseNumber;
                obj.QuoteAutoSequence = objDTO.QuoteAutoSequence;
                obj.NextQuoteNo = objDTO.NextQuoteNo;
                obj.QuoteAutoNrFixedValue = objDTO.QuoteAutoNrFixedValue;
                obj.QuoteAutoNrReleaseNumber = objDTO.QuoteAutoNrReleaseNumber;

                context.SupplierMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }
        }

        /// <summary>
        /// Edit Supplier master Record
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(SupplierMasterDTO objDTO, long SessionUserId)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            bool Iskitcomponnentchanged = false;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                SupplierMaster obj = context.SupplierMasters.Where(s => s.ID == objDTO.ID).FirstOrDefault();
                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.SupplierName = objDTO.SupplierName;
                    obj.SupplierColor = objDTO.SupplierColor;
                    obj.Description = objDTO.Description;
                    //obj.AccountNo = objDTO.AccountNo;
                    obj.ReceiverID = objDTO.ReceiverID;
                    obj.Address = objDTO.Address;
                    obj.City = objDTO.City;
                    obj.State = objDTO.State;
                    obj.ZipCode = objDTO.ZipCode;
                    obj.Country = objDTO.Country;
                    obj.Contact = objDTO.Contact;
                    obj.Phone = objDTO.Phone;
                    obj.Fax = objDTO.Fax;
                    obj.Email = objDTO.Email;
                    obj.IsEmailPOInBody = objDTO.IsEmailPOInBody;
                    obj.IsEmailPOInPDF = objDTO.IsEmailPOInPDF;
                    obj.IsEmailPOInCSV = objDTO.IsEmailPOInCSV;
                    obj.IsEmailPOInX12 = objDTO.IsEmailPOInX12;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.LastUpdated = objDTO.LastUpdated;
                    obj.Created = objDTO.Created;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.GUID = objDTO.GUID;
                    obj.IsDeleted = (bool)objDTO.IsDeleted;
                    obj.IsArchived = (bool)objDTO.IsArchived;
                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.UDF6 = objDTO.UDF6;
                    obj.UDF7 = objDTO.UDF7;
                    obj.UDF8 = objDTO.UDF8;
                    obj.UDF9 = objDTO.UDF9;
                    obj.UDF10 = objDTO.UDF10;
                    obj.BranchNumber = objDTO.BranchNumber;
                    obj.MaximumOrderSize = objDTO.MaximumOrderSize;
                    obj.IsSendtoVendor = objDTO.IsSendtoVendor;
                    obj.IsVendorReturnAsn = objDTO.IsVendorReturnAsn;
                    if (obj.IsSupplierReceivesKitComponents != objDTO.IsSupplierReceivesKitComponents)
                    {
                        Iskitcomponnentchanged = true;
                    }
                    obj.IsSupplierReceivesKitComponents = objDTO.IsSupplierReceivesKitComponents;
                    obj.POAutoSequence = objDTO.POAutoSequence;
                    obj.ScheduleType = objDTO.ScheduleType;
                    obj.Days = objDTO.Days;
                    obj.WeekDays = objDTO.WeekDays;
                    obj.MonthDays = objDTO.MonthDays;
                    obj.ScheduleTime = objDTO.ScheduleTime;
                    obj.IsAutoGenerate = objDTO.IsAutoGenerate;
                    obj.IsAutoGenerateSubmit = objDTO.IsAutoGenerateSubmit;
                    obj.NextOrderNo = objDTO.NextOrderNo;
                    obj.PullPurchaseNumberType = objDTO.PullPurchaseNumberType;
                    obj.LastPullPurchaseNumberUsed = objDTO.LastPullPurchaseNumberUsed;
                    obj.ImageType = objDTO.ImageType;
                    obj.ImageExternalURL = objDTO.ImageExternalURL;
                    if (objDTO.isForBOM)
                    {
                        objDTO.Room = 0;
                        objDTO.RoomName = string.Empty;
                        obj.Room = 0;
                    }
                    obj.AddedFrom = objDTO.AddedFrom;
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        obj.EditedFrom = objDTO.EditedFrom;
                        obj.ReceivedOn = objDTO.ReceivedOn;
                    }
                    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                    obj.isForBOM = objDTO.isForBOM;
                    obj.RefBomId = objDTO.RefBomId;
                    obj.DefaultOrderRequiredDays = objDTO.DefaultOrderRequiredDays;
                    obj.POAutoNrFixedValue = objDTO.POAutoNrFixedValue;
                    obj.PullPurchaseNrFixedValue = objDTO.PullPurchaseNrFixedValue;
                    obj.IsOrderReleaseNumberEditable = objDTO.IsOrderReleaseNumberEditable;
                    obj.POAutoNrReleaseNumber = objDTO.POAutoNrReleaseNumber;
                    obj.QuoteAutoSequence = objDTO.QuoteAutoSequence;
                    obj.NextQuoteNo = objDTO.NextQuoteNo;
                    obj.QuoteAutoNrFixedValue = objDTO.QuoteAutoNrFixedValue;
                    obj.QuoteAutoNrReleaseNumber = objDTO.QuoteAutoNrReleaseNumber;
                    //context.SupplierMasters.Attach(obj);
                    //context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                    context.SaveChanges();
                    if (Iskitcomponnentchanged)
                    {
                        CartItemDAL objCartItem = new CartItemDAL(base.DataBaseName);
                        //code
                        List<Guid> objGuidList = context.ItemMasters.Where(t => t.SupplierID == obj.ID && t.IsDeleted == false && t.Room == obj.Room).Select(t => t.GUID).ToList();
                        foreach (Guid currentGuid in objGuidList)
                        {

                            //objCartItem.AutoCartUpdateByCode(currentGuid, obj.LastUpdatedBy ?? 0, "Web", "Supplier Edit");
                            objCartItem.AutoCartUpdateByCode(currentGuid, obj.LastUpdatedBy ?? 0, "Web", "Supplier >> Modified Supplier", SessionUserId);
                        }

                    }
                }

                return true;
            }
        }

        public string UpdateSupplierData(Int64 id, string value, string columnName, long UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@ColumnName", columnName), new SqlParameter("@ColumnValue", value), new SqlParameter("@UserID", UserID) };
                context.Database.ExecuteSqlCommand("exec [UpdateSupplierMasterData] @ID,@ColumnName,@ColumnValue,@UserID", params1);
            }

            return value;
        }

        public SchedulerDTO SaveSupplierSchedule(SchedulerDTO objSchedulerDTO)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy);

            if (!string.IsNullOrEmpty(objSchedulerDTO.ScheduleRunTime))
            {
                if (objSchedulerDTO.ScheduleMode > 0 && objSchedulerDTO.ScheduleMode < 4)
                {
                    string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                    objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                    objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                }
            }

            if (objSchedulerDTO.ScheduleMode < 1 || objSchedulerDTO.ScheduleMode > 4)
            {
                string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }

            if (objSchedulerDTO.ScheduleMode == 4)
            {
                objSchedulerDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objSchedulerDTO.HourlyAtWhatMinute, 0);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }

            RoomSchedule objRoomSchedule = new RoomSchedule();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objSchedulerDTO.ScheduleID < 1)
                {
                    objRoomSchedule = new RoomSchedule();
                    objRoomSchedule.CompanyId = objSchedulerDTO.CompanyId;
                    objRoomSchedule.Created = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.CreatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                    objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                    objRoomSchedule.ScheduleID = 0;
                    objRoomSchedule.LastUpdatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                    objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                    objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                    objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                    objRoomSchedule.RoomId = objSchedulerDTO.RoomId;
                    objRoomSchedule.ScheduleFor = objSchedulerDTO.LoadSheduleFor;
                    objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;

                    if (objSchedulerDTO.ScheduleRunDateTime == DateTime.MinValue)
                    {
                        objSchedulerDTO.ScheduleRunDateTime = DateTime.Now.Date;
                    }

                    objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                    objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                    objRoomSchedule.SupplierId = objSchedulerDTO.SupplierId;
                    objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                    objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                    objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                    objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                    objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                    objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                    objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                    objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                    objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                    objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                    objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                    objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                    objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                    objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                    objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;
                    objRoomSchedule.ScheduledBy = objSchedulerDTO.ScheduledBy;
                    objRoomSchedule.UserID = objSchedulerDTO.UserID;

                    if (!string.IsNullOrWhiteSpace(objSchedulerDTO.ScheduleRunTime))
                    {
                        objRoomSchedule.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;
                    }

                    context.RoomSchedules.Add(objRoomSchedule);
                    context.SaveChanges();
                    objSchedulerDTO.ScheduleID = objRoomSchedule.ScheduleID;
                    if (objSchedulerDTO.ScheduleMode != 0 && objSchedulerDTO.ScheduleMode != 5 && objSchedulerDTO.ScheduleMode != 6)
                    {
                        context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                    }
                }
                else
                {
                    bool ReclacSchedule = false;
                    bool SetPullBilling = false;
                    objRoomSchedule = context.RoomSchedules.FirstOrDefault(t => t.ScheduleID == objSchedulerDTO.ScheduleID);

                    if (objRoomSchedule != null)
                    {
                        if (new int[] { 1, 2, 3, 4 }.Contains(objRoomSchedule.ScheduleMode) && objSchedulerDTO.ScheduleMode == 5 && objRoomSchedule.ScheduleFor == 7)
                        {
                            SetPullBilling = true;
                        }

                        if (objRoomSchedule.ScheduleMode != objSchedulerDTO.ScheduleMode)
                        {
                            ReclacSchedule = true;
                        }
                        else if (objRoomSchedule.IsScheduleActive != objSchedulerDTO.IsScheduleActive)
                        {
                            ReclacSchedule = true;
                        }
                        else if ((objRoomSchedule.ScheduleRunTime.Hour != objSchedulerDTO.ScheduleRunDateTime.Hour) || (objRoomSchedule.ScheduleRunTime.Minute != objSchedulerDTO.ScheduleRunDateTime.Minute))
                        {
                            ReclacSchedule = true;
                        }
                        else
                        {
                            switch (objSchedulerDTO.ScheduleMode)
                            {
                                case 1:
                                    if ((objRoomSchedule.DailyRecurringType != objSchedulerDTO.DailyRecurringType))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.DailyRecurringType == 1 && (objRoomSchedule.DailyRecurringDays != objSchedulerDTO.DailyRecurringDays))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 2:
                                    if ((objRoomSchedule.WeeklyRecurringWeeks != objSchedulerDTO.WeeklyRecurringWeeks) || (objRoomSchedule.WeeklyOnSunday != objSchedulerDTO.WeeklyOnSunday) || (objRoomSchedule.WeeklyOnMonday != objSchedulerDTO.WeeklyOnMonday) || (objRoomSchedule.WeeklyOnTuesday != objSchedulerDTO.WeeklyOnTuesday) || (objRoomSchedule.WeeklyOnWednesday != objSchedulerDTO.WeeklyOnWednesday) || (objRoomSchedule.WeeklyOnThursday != objSchedulerDTO.WeeklyOnThursday) || (objRoomSchedule.WeeklyOnFriday != objSchedulerDTO.WeeklyOnFriday) || (objRoomSchedule.WeeklyOnSaturday != objSchedulerDTO.WeeklyOnSaturday))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 3:
                                    if (objRoomSchedule.MonthlyRecurringType != objSchedulerDTO.MonthlyRecurringType)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringMonths != objSchedulerDTO.MonthlyRecurringMonths)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 1 && (objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 2 && ((objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth) || (objRoomSchedule.MonthlyDayOfMonth != objSchedulerDTO.MonthlyDayOfMonth)))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 4:
                                    if ((objRoomSchedule.HourlyRecurringHours != objSchedulerDTO.HourlyRecurringHours) || (objRoomSchedule.HourlyAtWhatMinute != objSchedulerDTO.HourlyAtWhatMinute))
                                    {
                                        ReclacSchedule = true;
                                    }

                                    break;
                            }
                        }

                        if (ReclacSchedule)
                        {
                            objRoomSchedule.NextRunDate = null;
                        }

                        objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                        objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                        objRoomSchedule.LastUpdatedBy = objSchedulerDTO.LastUpdatedBy;
                        objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                        objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                        objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                        objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                        objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;
                        objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                        objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                        objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                        objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                        objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                        objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                        objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                        objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                        objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                        objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                        objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                        objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                        objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                        objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                        objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                        objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                        objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;

                        if (!string.IsNullOrWhiteSpace(objSchedulerDTO.ScheduleRunTime))
                        {
                            objRoomSchedule.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;
                        }

                        objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                        context.SaveChanges();
                    }

                    if (ReclacSchedule)
                    {
                        if (objSchedulerDTO.ScheduleMode != 0 && objSchedulerDTO.ScheduleMode != 5 && objSchedulerDTO.ScheduleMode != 6)
                        {
                            context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                        }
                    }

                    if (SetPullBilling)
                    {
                        PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                        objPullMasterDAL.ProcessPullForBilling(objRoomSchedule.SupplierId ?? 0, objRoomSchedule.RoomId ?? 0, objRoomSchedule.CompanyId ?? 0, DateTime.Now, DateTime.Now, objRoomSchedule.LastUpdatedBy ?? 0, "SupplierMasterDAL>>SaveSupplierSchedule");
                    }
                }
            }

            return objSchedulerDTO;
        }

        public SchedulerDTO SaveSchedule(SchedulerDTO objSchedulerDTO)
        {
            bool ReclacSchedule = false;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy);
            if (!string.IsNullOrEmpty(objSchedulerDTO.ScheduleRunTime))
            {
                if (objSchedulerDTO.ScheduleMode > 0 && objSchedulerDTO.ScheduleMode < 4)
                {
                    string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                    objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                    objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                    //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                }
            }

            if (objSchedulerDTO.ScheduleMode < 1 || objSchedulerDTO.ScheduleMode > 4)
            {
                string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }

            if (objSchedulerDTO.ScheduleMode == 4)
            {
                objSchedulerDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objSchedulerDTO.HourlyAtWhatMinute, 0);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }

            RoomSchedule objRoomSchedule = new RoomSchedule();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objSchedulerDTO.ScheduleID < 1)
                {
                    objRoomSchedule = new RoomSchedule();
                    objRoomSchedule.CompanyId = objSchedulerDTO.CompanyId;
                    objRoomSchedule.Created = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.CreatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                    objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                    objRoomSchedule.ScheduleID = 0;
                    objRoomSchedule.LastUpdatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                    objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                    objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                    objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                    objRoomSchedule.RoomId = objSchedulerDTO.RoomId;
                    objRoomSchedule.ScheduleFor = objSchedulerDTO.LoadSheduleFor;
                    objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;
                    if (objSchedulerDTO.ScheduleRunDateTime == DateTime.MinValue)
                    {
                        objSchedulerDTO.ScheduleRunDateTime = DateTime.UtcNow.Date;
                    }
                    objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                    //                    new RegionSettingDAL(base.DataBaseName).ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                    objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                    objRoomSchedule.SupplierId = objSchedulerDTO.SupplierId;
                    objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                    objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                    objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                    objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                    objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                    objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                    objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                    objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                    objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                    objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                    objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                    objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                    objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                    objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                    objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;
                    objRoomSchedule.ScheduledBy = objSchedulerDTO.ScheduledBy;
                    objRoomSchedule.ScheduleName = objSchedulerDTO.ScheduleName;
                    if (objRoomSchedule.ScheduleFor == (int)eVMIScheduleFor.eVMISchedule)
                    {
                        objRoomSchedule.ScheduleTime = objSchedulerDTO.ScheduleTime;
                        objRoomSchedule.NextRunDate = objSchedulerDTO.NextRunDate;
                    }
                    //RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ExecuitionDate > DateTime.Now && t.ScheduleID == objRoomSchedule.ScheduleID).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    //if (objRoomScheduleDetail != null)
                    //{
                    //    objRoomSchedule.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    //}

                    context.RoomSchedules.Add(objRoomSchedule);
                    context.SaveChanges();
                    objSchedulerDTO.ScheduleID = objRoomSchedule.ScheduleID;
                    //context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                }
                else
                {

                    objRoomSchedule = context.RoomSchedules.FirstOrDefault(t => t.ScheduleID == objSchedulerDTO.ScheduleID);
                    if (objRoomSchedule != null)
                    {

                        if (objRoomSchedule.ScheduleMode != objSchedulerDTO.ScheduleMode)
                        {
                            ReclacSchedule = true;
                        }
                        else if ((objRoomSchedule.ScheduleRunTime.Hour != objSchedulerDTO.ScheduleRunDateTime.Hour) || (objRoomSchedule.ScheduleRunTime.Minute != objSchedulerDTO.ScheduleRunDateTime.Minute))
                        {
                            ReclacSchedule = true;
                        }
                        else
                        {
                            switch (objSchedulerDTO.ScheduleMode)
                            {
                                case 1:
                                    if ((objRoomSchedule.DailyRecurringType != objSchedulerDTO.DailyRecurringType))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.DailyRecurringType == 1 && (objRoomSchedule.DailyRecurringDays != objSchedulerDTO.DailyRecurringDays))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 2:
                                    if ((objRoomSchedule.WeeklyRecurringWeeks != objSchedulerDTO.WeeklyRecurringWeeks) || (objRoomSchedule.WeeklyOnSunday != objSchedulerDTO.WeeklyOnSunday) || (objRoomSchedule.WeeklyOnMonday != objSchedulerDTO.WeeklyOnMonday) || (objRoomSchedule.WeeklyOnTuesday != objSchedulerDTO.WeeklyOnTuesday) || (objRoomSchedule.WeeklyOnWednesday != objSchedulerDTO.WeeklyOnWednesday) || (objRoomSchedule.WeeklyOnThursday != objSchedulerDTO.WeeklyOnThursday) || (objRoomSchedule.WeeklyOnFriday != objSchedulerDTO.WeeklyOnFriday) || (objRoomSchedule.WeeklyOnSaturday != objSchedulerDTO.WeeklyOnSaturday))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 3:
                                    if (objRoomSchedule.MonthlyRecurringType != objSchedulerDTO.MonthlyRecurringType)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringMonths != objSchedulerDTO.MonthlyRecurringMonths)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 1 && (objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 2 && ((objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth) || (objRoomSchedule.MonthlyDayOfMonth != objSchedulerDTO.MonthlyDayOfMonth)))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 4:
                                    if ((objRoomSchedule.HourlyRecurringHours != objSchedulerDTO.HourlyRecurringHours) || (objRoomSchedule.HourlyAtWhatMinute != objSchedulerDTO.HourlyAtWhatMinute))
                                    {
                                        ReclacSchedule = true;
                                    }

                                    break;

                            }
                        }
                        if (ReclacSchedule)
                        {
                            objRoomSchedule.NextRunDate = null;
                        }
                        objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                        objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                        objRoomSchedule.LastUpdatedBy = objSchedulerDTO.LastUpdatedBy;
                        objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                        objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                        objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                        objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                        objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;
                        objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                        objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                        objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                        objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                        objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                        objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                        objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                        objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                        objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                        objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                        objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                        objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                        objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                        objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                        objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                        objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                        objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;
                        //objRoomSchedule.NextRunDate = GetScheduleNextRunDate(objSchedulerDTO);
                        objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                        objRoomSchedule.ScheduleName = objSchedulerDTO.ScheduleName;
                        //objRoomSchedule.ScheduledBy = objSchedulerDTO.ScheduledBy;
                        if (objRoomSchedule.ScheduleFor == (int)eVMIScheduleFor.eVMISchedule)
                        {
                            objRoomSchedule.ScheduleTime = objSchedulerDTO.ScheduleTime;
                            objRoomSchedule.NextRunDate = objSchedulerDTO.NextRunDate;
                        }
                        context.SaveChanges();
                    }



                    //if (ReclacSchedule)
                    //{
                    //    context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                    //}
                }



                //if (objSchedulerDTO.LoadSheduleFor != 5)
                //{
                //    context.Database.ExecuteSqlCommand("EXEC GENERATESCHEDULEFORORDERTRANSFER " + objRoomSchedule.ScheduleID + "");
                //}
            }
            //GenerateSupplierSchedule(objSchedulerDTO);
            objSchedulerDTO.RecalcSchedule = ReclacSchedule;
            return objSchedulerDTO;
        }

        public SchedulerDTO SaveRoomDailySchedule(SchedulerDTO objSchedulerDTO)
        {
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy);
            if (!string.IsNullOrEmpty(objSchedulerDTO.ScheduleRunTime))
            {
                if (objSchedulerDTO.ScheduleMode > 0 && objSchedulerDTO.ScheduleMode < 4)
                {
                    string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                    objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                    objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                    //objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                }

            }
            if (objSchedulerDTO.ScheduleMode < 1 || objSchedulerDTO.ScheduleMode > 4)
            {
                string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }
            if (objSchedulerDTO.ScheduleMode == 4)
            {
                objSchedulerDTO.ScheduleRunDateTime = new DateTime(datetimetoConsider.Year, datetimetoConsider.Month, datetimetoConsider.Day, datetimetoConsider.Hour, objSchedulerDTO.HourlyAtWhatMinute, 0);
                objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
            }
            RoomSchedule objRoomSchedule = new RoomSchedule();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objSchedulerDTO.ScheduleID < 1)
                {
                    objRoomSchedule = new RoomSchedule();
                    objRoomSchedule.CompanyId = objSchedulerDTO.CompanyId;
                    objRoomSchedule.Created = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.CreatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                    objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                    objRoomSchedule.ScheduleID = 0;
                    objRoomSchedule.LastUpdatedBy = objSchedulerDTO.CreatedBy;
                    objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                    objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                    objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                    objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                    objRoomSchedule.RoomId = objSchedulerDTO.RoomId;
                    objRoomSchedule.ScheduleFor = objSchedulerDTO.LoadSheduleFor;
                    objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;
                    if (objSchedulerDTO.ScheduleRunDateTime == DateTime.MinValue)
                    {
                        objSchedulerDTO.ScheduleRunDateTime = DateTime.Now.Date;
                    }
                    objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                    //                    new RegionSettingDAL(base.DataBaseName).ConvertLocalDateTimeToUTCDateTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
                    objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                    objRoomSchedule.SupplierId = objSchedulerDTO.SupplierId;
                    objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                    objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                    objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                    objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                    objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                    objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                    objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                    objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                    objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                    objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                    objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                    objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                    objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                    objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                    objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                    objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;
                    objRoomSchedule.ScheduledBy = objSchedulerDTO.ScheduledBy;
                    if (!string.IsNullOrWhiteSpace(objSchedulerDTO.ScheduleRunTime))
                    {
                        objRoomSchedule.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;
                    }
                    //RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ExecuitionDate > DateTime.Now && t.ScheduleID == objRoomSchedule.ScheduleID).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    //if (objRoomScheduleDetail != null)
                    //{
                    //    objRoomSchedule.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    //}

                    context.RoomSchedules.Add(objRoomSchedule);
                    context.SaveChanges();
                    objSchedulerDTO.ScheduleID = objRoomSchedule.ScheduleID;
                    if (objSchedulerDTO.ScheduleMode != 0 && objSchedulerDTO.ScheduleMode != 5 && objSchedulerDTO.ScheduleMode != 6)
                    {
                        context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                    }
                }
                else
                {
                    bool ReclacSchedule = false;
                    bool SetPullBilling = false;
                    objRoomSchedule = context.RoomSchedules.FirstOrDefault(t => t.ScheduleID == objSchedulerDTO.ScheduleID);

                    if (objRoomSchedule != null)
                    {

                        if (new int[] { 1, 2, 3, 4 }.Contains(objRoomSchedule.ScheduleMode) && objSchedulerDTO.ScheduleMode == 5 && objRoomSchedule.ScheduleFor == 7)
                        {
                            SetPullBilling = true;
                        }

                        if (objRoomSchedule.ScheduleMode != objSchedulerDTO.ScheduleMode)
                        {
                            ReclacSchedule = true;
                        }
                        else if (objRoomSchedule.IsScheduleActive != objSchedulerDTO.IsScheduleActive)
                        {
                            ReclacSchedule = true;
                        }
                        else if ((objRoomSchedule.ScheduleRunTime.Hour != objSchedulerDTO.ScheduleRunDateTime.Hour) || (objRoomSchedule.ScheduleRunTime.Minute != objSchedulerDTO.ScheduleRunDateTime.Minute))
                        {
                            ReclacSchedule = true;
                        }
                        else
                        {
                            switch (objSchedulerDTO.ScheduleMode)
                            {
                                case 1:
                                    if ((objRoomSchedule.DailyRecurringType != objSchedulerDTO.DailyRecurringType))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.DailyRecurringType == 1 && (objRoomSchedule.DailyRecurringDays != objSchedulerDTO.DailyRecurringDays))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 2:
                                    if ((objRoomSchedule.WeeklyRecurringWeeks != objSchedulerDTO.WeeklyRecurringWeeks) || (objRoomSchedule.WeeklyOnSunday != objSchedulerDTO.WeeklyOnSunday) || (objRoomSchedule.WeeklyOnMonday != objSchedulerDTO.WeeklyOnMonday) || (objRoomSchedule.WeeklyOnTuesday != objSchedulerDTO.WeeklyOnTuesday) || (objRoomSchedule.WeeklyOnWednesday != objSchedulerDTO.WeeklyOnWednesday) || (objRoomSchedule.WeeklyOnThursday != objSchedulerDTO.WeeklyOnThursday) || (objRoomSchedule.WeeklyOnFriday != objSchedulerDTO.WeeklyOnFriday) || (objRoomSchedule.WeeklyOnSaturday != objSchedulerDTO.WeeklyOnSaturday))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 3:
                                    if (objRoomSchedule.MonthlyRecurringType != objSchedulerDTO.MonthlyRecurringType)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringMonths != objSchedulerDTO.MonthlyRecurringMonths)
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 1 && (objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    else if (objRoomSchedule.MonthlyRecurringType == 2 && ((objRoomSchedule.MonthlyDateOfMonth != objSchedulerDTO.MonthlyDateOfMonth) || (objRoomSchedule.MonthlyDayOfMonth != objSchedulerDTO.MonthlyDayOfMonth)))
                                    {
                                        ReclacSchedule = true;
                                    }
                                    break;
                                case 4:
                                    if ((objRoomSchedule.HourlyRecurringHours != objSchedulerDTO.HourlyRecurringHours) || (objRoomSchedule.HourlyAtWhatMinute != objSchedulerDTO.HourlyAtWhatMinute))
                                    {
                                        ReclacSchedule = true;
                                    }

                                    break;

                            }
                        }
                        if (ReclacSchedule)
                        {
                            objRoomSchedule.NextRunDate = null;
                        }
                        objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                        objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                        objRoomSchedule.LastUpdatedBy = objSchedulerDTO.LastUpdatedBy;
                        objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                        objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                        objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                        objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                        objRoomSchedule.ScheduleMode = objSchedulerDTO.ScheduleMode;
                        objRoomSchedule.SubmissionMethod = objSchedulerDTO.SubmissionMethod;
                        objRoomSchedule.WeeklyOnFriday = objSchedulerDTO.WeeklyOnFriday;
                        objRoomSchedule.WeeklyOnMonday = objSchedulerDTO.WeeklyOnMonday;
                        objRoomSchedule.WeeklyOnSaturday = objSchedulerDTO.WeeklyOnSaturday;
                        objRoomSchedule.WeeklyOnSunday = objSchedulerDTO.WeeklyOnSunday;
                        objRoomSchedule.WeeklyOnThursday = objSchedulerDTO.WeeklyOnThursday;
                        objRoomSchedule.WeeklyOnTuesday = objSchedulerDTO.WeeklyOnTuesday;
                        objRoomSchedule.WeeklyOnWednesday = objSchedulerDTO.WeeklyOnWednesday;
                        objRoomSchedule.WeeklyRecurringWeeks = objSchedulerDTO.WeeklyRecurringWeeks;
                        objRoomSchedule.IsScheduleActive = objSchedulerDTO.IsScheduleActive;
                        objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                        objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                        objRoomSchedule.ReportID = objSchedulerDTO.ReportID;
                        objRoomSchedule.ReportDataSelectionType = objSchedulerDTO.ReportDataSelectionType;
                        objRoomSchedule.ReportDataSince = objSchedulerDTO.ReportDataSince;
                        objRoomSchedule.HourlyAtWhatMinute = objSchedulerDTO.HourlyAtWhatMinute;
                        objRoomSchedule.HourlyRecurringHours = objSchedulerDTO.HourlyRecurringHours;
                        if (!string.IsNullOrWhiteSpace(objSchedulerDTO.ScheduleRunTime))
                        {
                            objRoomSchedule.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;
                        }
                        //objRoomSchedule.NextRunDate = GetScheduleNextRunDate(objSchedulerDTO);
                        objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                        //objRoomSchedule.ScheduledBy = objSchedulerDTO.ScheduledBy;
                        context.SaveChanges();
                    }



                    if (ReclacSchedule)
                    {
                        if (objSchedulerDTO.ScheduleMode != 0 && objSchedulerDTO.ScheduleMode != 5 && objSchedulerDTO.ScheduleMode != 6)
                        {
                            context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                        }
                    }
                    if (SetPullBilling)
                    {
                        PullMasterDAL objPullMasterDAL = new PullMasterDAL(base.DataBaseName);
                        objPullMasterDAL.ProcessPullForBilling(objRoomSchedule.SupplierId ?? 0, objRoomSchedule.RoomId ?? 0, objRoomSchedule.CompanyId ?? 0, DateTime.Now, DateTime.Now, objRoomSchedule.LastUpdatedBy ?? 0, "SupplierMasterDAL>>SaveRoomDailySchedule");


                    }
                }



                //if (objSchedulerDTO.LoadSheduleFor != 5)
                //{
                //    context.Database.ExecuteSqlCommand("EXEC GENERATESCHEDULEFORORDERTRANSFER " + objRoomSchedule.ScheduleID + "");
                //}
            }
            //GenerateSupplierSchedule(objSchedulerDTO);
            return objSchedulerDTO;
        }

        public void UpdatedLastUsedNumber(long UsedNumber, long SupplierID, long RoomID, string UpdateTO)
        {
            if (!string.IsNullOrWhiteSpace(UpdateTO))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    switch (UpdateTO)
                    {
                        case "room":
                            Room objRoom = context.Rooms.FirstOrDefault(t => t.ID == RoomID);
                            if (objRoom != null)
                            {
                                objRoom.LastPullPurchaseNumberUsed = Convert.ToString(UsedNumber);
                                context.SaveChanges();
                            }
                            break;
                        case "supplier":
                            SupplierMaster objSupplierMaster = context.SupplierMasters.FirstOrDefault(t => t.ID == SupplierID);
                            if (objSupplierMaster != null)
                            {
                                objSupplierMaster.LastPullPurchaseNumberUsed = Convert.ToString(UsedNumber);
                                context.SaveChanges();
                            }
                            break;
                    }
                }
            }

        }

        public SchedulerDTO GetRoomSchedule(long SupplierId, long RoomId, int ScheduleFor)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from ss in context.RoomSchedules
                                   where ss.SupplierId == SupplierId && ss.RoomId == RoomId && ss.ScheduleFor == ScheduleFor
                                   select new SchedulerDTO
                                   {
                                       CompanyId = ss.CompanyId ?? 0,
                                       CreatedBy = ss.CreatedBy ?? 0,
                                       DailyRecurringDays = ss.DailyRecurringDays,
                                       DailyRecurringType = ss.DailyRecurringType,
                                       ScheduleID = ss.ScheduleID,
                                       IsScheduleActive = ss.IsScheduleActive,
                                       LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                       LoadSheduleFor = ss.ScheduleFor,
                                       MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                       MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                       MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                       MonthlyRecurringType = ss.MonthlyRecurringType,
                                       RoomId = ss.RoomId ?? 0,
                                       RoomName = string.Empty,
                                       ScheduleMode = ss.ScheduleMode,
                                       //ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                       ScheduleRunDateTime = ss.ScheduleRunTime,
                                       SubmissionMethod = ss.SubmissionMethod,
                                       SupplierId = ss.SupplierId ?? 0,
                                       SupplierName = string.Empty,
                                       WeeklyOnFriday = ss.WeeklyOnFriday,
                                       WeeklyOnMonday = ss.WeeklyOnMonday,
                                       WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                       WeeklyOnSunday = ss.WeeklyOnSunday,
                                       WeeklyOnThursday = ss.WeeklyOnThursday,
                                       WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                       WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                       WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                       WeeklySelectedDays = string.Empty,
                                       AssetToolID = ss.AssetToolID,
                                       HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                       HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                       NextRunDate = ss.NextRunDate,
                                       ScheduleTime = ss.ScheduleTime
                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                    RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    if (objRoomScheduleDetail != null)
                    {
                        objSchedulerDTO.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    }
                }
            }


            return objSchedulerDTO;
        }

        public QuoteSchedulerDTO GetRoomScheduleForQuote(long SupplierId, long RoomId, int ScheduleFor)
        {
            try
            {
                QuoteSchedulerDTO objSchedulerDTO = new QuoteSchedulerDTO();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objSchedulerDTO = (from ss in context.RoomSchedules
                                       where ss.SupplierId == SupplierId && ss.RoomId == RoomId && ss.ScheduleFor == ScheduleFor
                                       select new QuoteSchedulerDTO
                                       {
                                           Quote_CompanyId = ss.CompanyId ?? 0,
                                           Quote_CreatedBy = ss.CreatedBy ?? 0,
                                           Quote_DailyRecurringDays = ss.DailyRecurringDays,
                                           Quote_DailyRecurringType = ss.DailyRecurringType,
                                           Quote_ScheduleID = ss.ScheduleID,
                                           Quote_IsScheduleActive = ss.IsScheduleActive,
                                           Quote_LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                           Quote_LoadSheduleFor = ss.ScheduleFor,
                                           Quote_MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                           Quote_MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                           Quote_MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                           Quote_MonthlyRecurringType = ss.MonthlyRecurringType,
                                           Quote_RoomId = ss.RoomId ?? 0,
                                           Quote_RoomName = string.Empty,
                                           Quote_ScheduleMode = ss.ScheduleMode,
                                           //Quote_ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                           Quote_ScheduleRunDateTime = ss.ScheduleRunTime,
                                           Quote_SubmissionMethod = ss.SubmissionMethod,
                                           Quote_SupplierId = ss.SupplierId ?? 0,
                                           Quote_SupplierName = string.Empty,
                                           Quote_WeeklyOnFriday = ss.WeeklyOnFriday,
                                           Quote_WeeklyOnMonday = ss.WeeklyOnMonday,
                                           Quote_WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                           Quote_WeeklyOnSunday = ss.WeeklyOnSunday,
                                           Quote_WeeklyOnThursday = ss.WeeklyOnThursday,
                                           Quote_WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                           Quote_WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                           Quote_WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                           Quote_WeeklySelectedDays = string.Empty,
                                           Quote_AssetToolID = ss.AssetToolID,
                                           Quote_HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                           Quote_HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                           Quote_NextRunDate = ss.NextRunDate,
                                           Quote_ScheduleTime = ss.ScheduleTime
                                       }).FirstOrDefault();
                    if (objSchedulerDTO != null)
                    {
                        objSchedulerDTO.Quote_ScheduleRunTime = objSchedulerDTO.Quote_ScheduleRunDateTime.ToString("HH:mm");
                        RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.Quote_ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                        if (objRoomScheduleDetail != null)
                        {
                            objSchedulerDTO.Quote_NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                        }
                    }
                }


                return objSchedulerDTO;
            }
            catch (Exception ex)
            {
                return new QuoteSchedulerDTO();
            }

        }
        public PullSchedulerDTO GetRoomScheduleForPull(long SupplierId, long RoomId, int ScheduleFor)
        {
            PullSchedulerDTO objSchedulerDTO = new PullSchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from ss in context.RoomSchedules
                                   where ss.SupplierId == SupplierId && ss.RoomId == RoomId && ss.ScheduleFor == ScheduleFor
                                   select new PullSchedulerDTO
                                   {
                                       Pull_CompanyId = ss.CompanyId ?? 0,
                                       Pull_CreatedBy = ss.CreatedBy ?? 0,
                                       Pull_DailyRecurringDays = ss.DailyRecurringDays,
                                       Pull_DailyRecurringType = ss.DailyRecurringType,
                                       Pull_ScheduleID = ss.ScheduleID,
                                       Pull_IsScheduleActive = ss.IsScheduleActive,
                                       Pull_LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                       Pull_LoadSheduleFor = ss.ScheduleFor,
                                       Pull_MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                       Pull_MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                       Pull_MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                       Pull_MonthlyRecurringType = ss.MonthlyRecurringType,
                                       Pull_RoomId = ss.RoomId ?? 0,
                                       Pull_RoomName = string.Empty,
                                       Pull_ScheduleMode = ss.ScheduleMode,
                                       //Pull_ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                       Pull_ScheduleRunDateTime = ss.ScheduleRunTime,
                                       Pull_SubmissionMethod = ss.SubmissionMethod,
                                       Pull_SupplierId = ss.SupplierId ?? 0,
                                       Pull_SupplierName = string.Empty,
                                       Pull_WeeklyOnFriday = ss.WeeklyOnFriday,
                                       Pull_WeeklyOnMonday = ss.WeeklyOnMonday,
                                       Pull_WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                       Pull_WeeklyOnSunday = ss.WeeklyOnSunday,
                                       Pull_WeeklyOnThursday = ss.WeeklyOnThursday,
                                       Pull_WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                       Pull_WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                       Pull_WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                       Pull_WeeklySelectedDays = string.Empty,
                                       Pull_AssetToolID = ss.AssetToolID,
                                       Pull_HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                       Pull_HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                       Pull_NextRunDate = ss.NextRunDate,
                                       Pull_ScheduleTime = ss.ScheduleTime
                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.Pull_ScheduleRunTime = objSchedulerDTO.Pull_ScheduleRunDateTime.ToString("HH:mm");
                    RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.Pull_ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    if (objRoomScheduleDetail != null)
                    {
                        objSchedulerDTO.Pull_NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    }
                }
            }


            return objSchedulerDTO;
        }

        public SchedulerDTO GetRoomScheduleForDailyMidNight(long RoomId, int ScheduleFor)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from ss in context.RoomSchedules
                                   where ss.RoomId == RoomId && ss.ScheduleFor == ScheduleFor
                                   select new SchedulerDTO
                                   {
                                       CompanyId = ss.CompanyId ?? 0,
                                       CreatedBy = ss.CreatedBy ?? 0,
                                       DailyRecurringDays = ss.DailyRecurringDays,
                                       DailyRecurringType = ss.DailyRecurringType,
                                       ScheduleID = ss.ScheduleID,
                                       IsScheduleActive = ss.IsScheduleActive,
                                       LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                       LoadSheduleFor = ss.ScheduleFor,
                                       MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                       MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                       MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                       MonthlyRecurringType = ss.MonthlyRecurringType,
                                       RoomId = ss.RoomId ?? 0,
                                       RoomName = string.Empty,
                                       ScheduleMode = ss.ScheduleMode,
                                       //ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                       ScheduleRunDateTime = ss.ScheduleRunTime,
                                       SubmissionMethod = ss.SubmissionMethod,
                                       SupplierId = ss.SupplierId ?? 0,
                                       SupplierName = string.Empty,
                                       WeeklyOnFriday = ss.WeeklyOnFriday,
                                       WeeklyOnMonday = ss.WeeklyOnMonday,
                                       WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                       WeeklyOnSunday = ss.WeeklyOnSunday,
                                       WeeklyOnThursday = ss.WeeklyOnThursday,
                                       WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                       WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                       WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                       WeeklySelectedDays = string.Empty,
                                       AssetToolID = ss.AssetToolID,
                                       HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                       HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                       NextRunDate = ss.NextRunDate,
                                       ScheduleTime = ss.ScheduleTime
                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                    RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    if (objRoomScheduleDetail != null)
                    {
                        objSchedulerDTO.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    }
                }
            }


            return objSchedulerDTO;
        }

        public SchedulerDTO GetRoomScheduleForAsset(Guid AssetToolGuid, long RoomId, int ScheduleFor)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from ss in context.RoomSchedules
                                   where ss.AssetToolID == AssetToolGuid && ss.RoomId == RoomId
                                   select new SchedulerDTO
                                   {
                                       CompanyId = ss.CompanyId ?? 0,
                                       CreatedBy = ss.CreatedBy ?? 0,
                                       DailyRecurringDays = ss.DailyRecurringDays,
                                       DailyRecurringType = ss.DailyRecurringType,
                                       ScheduleID = ss.ScheduleID,
                                       IsScheduleActive = ss.IsScheduleActive,
                                       LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                       LoadSheduleFor = ss.ScheduleFor,
                                       MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                       MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                       MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                       MonthlyRecurringType = ss.MonthlyRecurringType,
                                       RoomId = ss.RoomId ?? 0,
                                       RoomName = string.Empty,
                                       ScheduleMode = ss.ScheduleMode,
                                       //ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                       ScheduleRunDateTime = ss.ScheduleRunTime,
                                       SubmissionMethod = ss.SubmissionMethod,
                                       SupplierId = ss.SupplierId ?? 0,
                                       SupplierName = string.Empty,
                                       WeeklyOnFriday = ss.WeeklyOnFriday,
                                       WeeklyOnMonday = ss.WeeklyOnMonday,
                                       WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                       WeeklyOnSunday = ss.WeeklyOnSunday,
                                       WeeklyOnThursday = ss.WeeklyOnThursday,
                                       WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                       WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                       WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                       WeeklySelectedDays = string.Empty,
                                       AssetToolID = ss.AssetToolID
                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                    RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    if (objRoomScheduleDetail != null)
                    {
                        objSchedulerDTO.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    }
                }
            }


            return objSchedulerDTO;
        }

        public int GetSupplierMaxOrderSize(Int64 SupplierID, Int64 RoomID, Int64 CompanyID, SupplierMasterDTO objSuppMast)
        {
            int maxOrdSize = -1;
            if (objSuppMast != null && objSuppMast.ID > 0)
            {
                if (objSuppMast.MaximumOrderSize.GetValueOrDefault(0) > 0 || objSuppMast.MaxOrderSize.GetValueOrDefault(0) > 0)
                {
                    maxOrdSize = (int)objSuppMast.MaximumOrderSize.GetValueOrDefault(0);
                    if (maxOrdSize <= 0)
                        maxOrdSize = (int)objSuppMast.MaxOrderSize.GetValueOrDefault(0);
                }
            }
            else
            {
                SupplierMasterDTO objDTO = GetSupplierByIDPlain(SupplierID);
                if (objDTO.MaximumOrderSize.GetValueOrDefault(0) > 0 || objDTO.MaxOrderSize.GetValueOrDefault(0) > 0)
                {
                    maxOrdSize = (int)objDTO.MaximumOrderSize.GetValueOrDefault(0);
                    if (maxOrdSize <= 0)
                        maxOrdSize = (int)objDTO.MaxOrderSize.GetValueOrDefault(0);
                }
            }
            return maxOrdSize;
        }

        public List<RoomScheduleDetailDTO> GetAllSchedulesByTimePeriod(DateTime FromDate, DateTime ToDate, short[] arrScheduleFor)
        {
            List<RoomScheduleDetailDTO> lstSchedules = new List<RoomScheduleDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from rs in context.RoomSchedules
                                join rm in context.Rooms on rs.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                join sm in context.SupplierMasters on rs.SupplierId equals sm.ID into rs_sm_join
                                from rs_sm in rs_sm_join.DefaultIfEmpty()
                                join regset in context.RegionalSettings on rs.RoomId equals regset.RoomId into rs_regset_join
                                from rs_regset in rs_regset_join.DefaultIfEmpty()
                                where (rs.IsScheduleActive == true && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false && ((rs_sm.IsDeleted == false && rs.SupplierId > 0) || rs.SupplierId == 0)) && ((rs.NextRunDate >= FromDate && rs.NextRunDate <= ToDate) || rs.ScheduleMode == 5) && arrScheduleFor.Contains(rs.ScheduleFor)
                                select new RoomScheduleDetailDTO
                                {
                                    CompanyId = rs.CompanyId ?? 0,
                                    ExecuitionDate = rs.NextRunDate ?? DateTime.MinValue,
                                    ID = rs.ScheduleID,
                                    IsScheduleActive = rs.IsScheduleActive,
                                    LoadSheduleFor = rs.ScheduleFor,
                                    RoomId = rs.RoomId ?? 0,
                                    RoomName = rm.RoomName,
                                    ScheduleID = rs.ScheduleID,
                                    SupplierId = rs.SupplierId ?? 0,
                                    SupplierName = rs_sm.SupplierName,
                                    SubmissionMethod = rs.SubmissionMethod,
                                    NextRunDate = rs.NextRunDate,
                                    TimeZoneName = rs_regset.TimeZoneName,
                                }).ToList();
            }

            return lstSchedules;
        }

        public void UpdateNextRunDateOfSchedule(long ScheduleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "EXEC RPT_GetNextReportRunTime_nd " + ScheduleID;
                context.Database.ExecuteSqlCommand(qry);
            }

        }

        public string SupplierDuplicateCheck(long ID, string SupplierName, long RoomID, long CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.SupplierMasters
                           where em.SupplierName.ToLower().Trim() == SupplierName.ToLower().Trim() && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.Room == RoomID && em.CompanyID == CompanyID
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

        public SchedulerDTO GetRoomScheduleByID(long ScheduleID)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objSchedulerDTO = (from ss in context.RoomSchedules
                                   where ss.ScheduleID == ScheduleID
                                   select new SchedulerDTO
                                   {
                                       CompanyId = ss.CompanyId ?? 0,
                                       CreatedBy = ss.CreatedBy ?? 0,
                                       DailyRecurringDays = ss.DailyRecurringDays,
                                       DailyRecurringType = ss.DailyRecurringType,
                                       ScheduleID = ss.ScheduleID,
                                       IsScheduleActive = ss.IsScheduleActive,
                                       LastUpdatedBy = ss.LastUpdatedBy ?? 0,
                                       LoadSheduleFor = ss.ScheduleFor,
                                       MonthlyDateOfMonth = ss.MonthlyDateOfMonth,
                                       MonthlyDayOfMonth = ss.MonthlyDayOfMonth,
                                       MonthlyRecurringMonths = ss.MonthlyRecurringMonths,
                                       MonthlyRecurringType = ss.MonthlyRecurringType,
                                       RoomId = ss.RoomId ?? 0,
                                       RoomName = string.Empty,
                                       ScheduleMode = ss.ScheduleMode,
                                       //ScheduleRunTime = ss.ScheduleRunTime.ToShortTimeString(),
                                       ScheduleRunDateTime = ss.ScheduleRunTime,
                                       SubmissionMethod = ss.SubmissionMethod,
                                       SupplierId = ss.SupplierId ?? 0,
                                       SupplierName = string.Empty,
                                       WeeklyOnFriday = ss.WeeklyOnFriday,
                                       WeeklyOnMonday = ss.WeeklyOnMonday,
                                       WeeklyOnSaturday = ss.WeeklyOnSaturday,
                                       WeeklyOnSunday = ss.WeeklyOnSunday,
                                       WeeklyOnThursday = ss.WeeklyOnThursday,
                                       WeeklyOnTuesday = ss.WeeklyOnTuesday,
                                       WeeklyOnWednesday = ss.WeeklyOnWednesday,
                                       WeeklyRecurringWeeks = ss.WeeklyRecurringWeeks,
                                       WeeklySelectedDays = string.Empty,
                                       AssetToolID = ss.AssetToolID,
                                       HourlyAtWhatMinute = ss.HourlyAtWhatMinute ?? 0,
                                       HourlyRecurringHours = ss.HourlyRecurringHours ?? 0,
                                       NextRunDate = ss.NextRunDate,
                                       ScheduleName = ss.ScheduleName
                                   }).FirstOrDefault();
                if (objSchedulerDTO != null)
                {
                    objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                    RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == objSchedulerDTO.ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    if (objRoomScheduleDetail != null)
                    {
                        objSchedulerDTO.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    }

                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy);

                    if (objeTurnsRegionInfo != null && !string.IsNullOrWhiteSpace(objeTurnsRegionInfo.TimeZoneName))
                    {
                        objSchedulerDTO.ScheduleRunTime = (DateTimeUtility.ConvertDateByTimeZonedt(objSchedulerDTO.ScheduleRunDateTime, TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName), null, null) ?? DateTime.MinValue).ToString("HH:mm");
                    }
                    else
                    {
                        objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime.ToString("HH:mm");
                    }
                }

            }


            return objSchedulerDTO;
        }

        public List<SchedulerDTO> GetAllDeadSchedules()
        {
            long[] Schedulemodes = new long[] { 1, 2, 3, 4 };
            short[] ScheduleFors = new short[] { 1, 2, 3, 4, 7, 8 };
            DateTime dtpast = DateTime.UtcNow.AddMinutes(-30);
            List<SchedulerDTO> lstNotifications = new List<SchedulerDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstNotifications = (from u in context.RoomSchedules
                                    where u.IsDeleted == false && u.IsScheduleActive == true && (u.NextRunDate <= dtpast || u.NextRunDate == null) && Schedulemodes.Contains(u.ScheduleMode) && ScheduleFors.Contains(u.ScheduleFor)
                                    select new SchedulerDTO
                                    {
                                        CompanyId = u.CompanyId ?? 0,
                                        Created = u.Created,
                                        CreatedBy = u.CreatedBy ?? 0,
                                        ScheduleID = u.ScheduleID,
                                        IsScheduleActive = u.IsScheduleActive,
                                        IsDeleted = u.IsDeleted,
                                        NextRunDate = u.NextRunDate,
                                        ReportID = u.ReportID,
                                        RoomId = u.RoomId ?? 0,
                                        LoadSheduleFor = u.ScheduleFor,
                                        SupplierId = u.SupplierId ?? 0,
                                        Updated = u.Updated,
                                        ScheduleRunDateTime = u.ScheduleRunTime
                                    }).ToList();
            }
            return lstNotifications;
        }
        public void DisableSchedule(long ScheduleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomSchedule objNotes = context.RoomSchedules.FirstOrDefault(t => t.ScheduleID == ScheduleID);
                if (objNotes != null)
                {
                    objNotes.IsScheduleActive = false;
                    context.SaveChanges();
                }
            }
        }

        public SupplierMasterDTO GetSupplierByGUIDPlain(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByGUIDPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public SupplierMasterDTO GetSupplierByGUIDNormal(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByGUIDNormal] @GUID", params1).FirstOrDefault();
            }
        }

        public SupplierMasterDTO GetSupplierByIDPlain(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByIDPlain] @ID", params1).FirstOrDefault();
            }
        }

        public SupplierMasterDTO GetSupplierByIDNormal(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByIDNormal] @ID", params1).FirstOrDefault();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByGUIDsNormal(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByGUIDsNormal] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<SupplierMasterDTO> GetSupplierByIDsNormal(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByIDsNormal] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByGUIDsPlain(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByGUIDsPlain] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByNAMEsPlain(string supplierNames, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@SupplierNames", supplierNames ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByNAMEsPlain] @SupplierNames,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByIDsPlain(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByIDsPlain] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByRoomPlain(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByRoomPlain] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByRoomPlainWithCmpRoom(long RoomID, long CompanyID, string SupplierIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@SupplierIDs", SupplierIDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByRoomPlainWithCmpRoom] @RoomID,@CompanyID,@SupplierIDs", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByRoomNormal(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByRoomNormal] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByRoomsIdsNormal(string RoomIDs, long UserId)
        {
            if ((RoomIDs ?? string.Empty) != string.Empty)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[]
                                                    {
                                                        new SqlParameter("@RoomIDs", RoomIDs) ,
                                                        new SqlParameter("@UserId", UserId)
                                                    };
                    return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByRoomIDsNormal] @RoomIDs,@UserId", params1).ToList();
                }
            }
            else
            {
                return new List<SupplierMasterDTO>();
            }
        }

        public List<SupplierMasterDTO> GetSuppliersByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom), new SqlParameter("@Name", Name ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByNamePlain] @RoomID,@CompanyID,@IsForBom,@Name", params1).ToList();
            }
        }
        public SupplierMasterDTO GetSupplierByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            return GetSuppliersByNamePlain(RoomID, CompanyID, IsForBom, Name).OrderByDescending(t => t.ID).FirstOrDefault();
        }

        public List<NarrowSearchDTO> GetSupplierListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetSupplierListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByNameSearch(string Name, long RoomID, long CompanyID, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Name", Name ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierByNameSearch] @Name,@RoomID,@CompanyID,@IsForBOM", params1).ToList();
            }
        }

        public List<SupplierMasterDTO> GetExportSupplierListByIDsFull(string ids, Int64 RoomID, Int64 CompanyID)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet dsSMFull = SqlHelper.ExecuteDataset(EturnsConnection, "GetExportSupplierListByIDsFull", ids, RoomID, CompanyID);

            DataTable dtSM = dsSMFull.Tables[0];
            DataTable dtSAD = dsSMFull.Tables[1];
            DataTable dtSBPOD = dsSMFull.Tables[2];

            List<SupplierMasterDTO> lstSM = CommonUtilityHelper.ConvertDataTable<SupplierMasterDTO>(dtSM);
            List<SupplierAccountDetailsDTO> lstSAD = CommonUtilityHelper.ConvertDataTable<SupplierAccountDetailsDTO>(dtSAD);
            List<SupplierBlanketPODetailsDTO> lstSBPOD = CommonUtilityHelper.ConvertDataTable<SupplierBlanketPODetailsDTO>(dtSBPOD);

            if (lstSM == null)
                lstSM = new List<SupplierMasterDTO>();
            if (lstSAD == null)
                lstSAD = new List<SupplierAccountDetailsDTO>();
            if (lstSBPOD == null)
                lstSBPOD = new List<SupplierBlanketPODetailsDTO>();

            List<SupplierMasterDTO> lstSupplierMasterDTO = new List<SupplierMasterDTO>();


            lstSupplierMasterDTO = (from u in lstSM
                                    select new SupplierMasterDTO
                                    {
                                        ID = u.ID,
                                        SupplierName = u.SupplierName != null ? (u.SupplierName) : string.Empty,
                                        SupplierColor = u.SupplierColor != null ? (u.SupplierColor) : string.Empty,
                                        Description = u.Description != null ? (u.Description) : string.Empty,
                                        //AccountNo = u.AccountNo,
                                        ReceiverID = u.ReceiverID,
                                        BranchNumber = u.BranchNumber != null ? (u.BranchNumber) : string.Empty,
                                        MaximumOrderSize = u.MaximumOrderSize,
                                        Address = u.Address,
                                        City = u.City,
                                        State = u.State,
                                        ZipCode = u.ZipCode,
                                        Country = u.Country,
                                        Contact = u.Contact != null ? (u.Contact) : string.Empty,
                                        Phone = u.Phone,
                                        Fax = u.Fax,
                                        Email = u.Email,
                                        IsEmailPOInBody = u.IsEmailPOInBody,
                                        IsEmailPOInPDF = u.IsEmailPOInPDF,
                                        IsEmailPOInCSV = u.IsEmailPOInCSV,
                                        IsEmailPOInX12 = u.IsEmailPOInX12,
                                        IsSendtoVendor = u.IsSendtoVendor,
                                        IsVendorReturnAsn = u.IsVendorReturnAsn,
                                        IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                                        POAutoSequence = u.POAutoSequence,
                                        Created = u.Created,
                                        LastUpdated = u.LastUpdated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        Room = u.Room,
                                        GUID = u.GUID,
                                        IsDeleted = u.IsDeleted,
                                        IsArchived = u.IsArchived,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        UDF6 = u.UDF6,
                                        UDF7 = u.UDF7,
                                        UDF8 = u.UDF8,
                                        UDF9 = u.UDF9,
                                        UDF10 = u.UDF10,
                                        SupplierImage = u.SupplierImage,
                                        ImageExternalURL = u.ImageExternalURL,
                                        POAutoNrReleaseNumber = u.POAutoNrReleaseNumber,
                                        SupplierAccountDetails = (from A in lstSAD
                                                                  where A.SupplierID == u.ID && !A.IsDeleted && !A.IsArchived
                                                                  && A.Room == RoomID && A.CompanyID == CompanyID
                                                                  select new SupplierAccountDetailsDTO
                                                                  {
                                                                      ID = A.ID,
                                                                      SupplierID = A.SupplierID,
                                                                      AccountNo = A.AccountNo,
                                                                      AccountName = A.AccountName != null ? (A.AccountName) : string.Empty,
                                                                      Address = A.Address,
                                                                      City = A.City,
                                                                      State = A.State,
                                                                      ZipCode = A.ZipCode,
                                                                      IsDefault = A.IsDefault,
                                                                      Country = A.Country,
                                                                      ShipToID = A.ShipToID

                                                                  }).ToList(),
                                        SupplierBlanketPODetails = (from A in lstSBPOD
                                                                    where A.SupplierID == u.ID && !(A.IsDeleted ?? false) && !(A.IsArchived ?? false)
                                                                    && A.Room == RoomID && A.CompanyID == CompanyID
                                                                    select new SupplierBlanketPODetailsDTO
                                                                    {
                                                                        ID = A.ID,
                                                                        SupplierID = A.SupplierID,
                                                                        BlanketPO = A.BlanketPO,
                                                                        StartDate = A.StartDate,
                                                                        Enddate = A.Enddate,
                                                                        MaxLimit = A.MaxLimit,
                                                                        IsNotExceed = A.IsNotExceed,
                                                                        MaxLimitQty = A.MaxLimitQty,
                                                                        IsNotExceedQty = A.IsNotExceedQty,
                                                                        OrderUsed=A.OrderUsed,
                                                                        TotalOrder=A.TotalOrder
                                                                    }).ToList(),

                                    }).ToList();


            return lstSupplierMasterDTO;
        }

        public List<SupplierMasterDTO> GetRecordByNameAndColor(string SupplierName, string SupplierColor, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SupplierName", SupplierName ?? string.Empty), new SqlParameter("@SupplierColor", SupplierColor ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<SupplierMasterDTO>("exec GetRecordByNameAndColorList @SupplierName,@SupplierColor,@RoomID,@CompanyID", params1).ToList<SupplierMasterDTO>();
            }
        }

        public List<SchedulerDTO> GetRoomSchedulesForUser(long UserID, short ScheduleFor)
        {
            List<SchedulerDTO> objSchedulerDTO = new List<SchedulerDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserID),
                                                    new SqlParameter("@ScheduleFor", ScheduleFor)
                                                 };
                objSchedulerDTO = context.Database.SqlQuery<SchedulerDTO>("exec GetRoomSchedulesForUser @UserID,@ScheduleFor", params1).ToList();

                if (objSchedulerDTO != null && objSchedulerDTO.Count > 0)
                {
                    objSchedulerDTO.ForEach(usi =>
                    {
                        usi.ScheduleRunTime = usi.ScheduleRunDateTime.ToString("HH:mm");
                        RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ScheduleID == usi.ScheduleID && t.ExecuitionDate > DateTime.Now).OrderBy(t => t.ExecuitionDate).FirstOrDefault();

                        if (objRoomScheduleDetail != null)
                        {
                            usi.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                        }
                    });
                }
            }

            return objSchedulerDTO;
        }

        public void DeleteAllSchedulesForUser(long UserID, short ScheduleFor)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserID),
                                                    new SqlParameter("@ScheduleFor", ScheduleFor)
                                                 };
                context.Database.ExecuteSqlCommand("exec [DeleteAllSchedulesForUser] @UserID,@ScheduleFor", params1);
            }
        }

        public void DeleteAllSchedulesForUserForRoom(long UserID, short ScheduleFor, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@UserID", UserID),
                                                    new SqlParameter("@ScheduleFor", ScheduleFor),
                                                    new SqlParameter("@CompanyId", CompanyID),
                                                    new SqlParameter("@RoomId", RoomID)

                                                 };
                context.Database.ExecuteSqlCommand("exec [DeleteAllSchedulesForUserByRoom] @UserID,@ScheduleFor,@CompanyId,@RoomId", params1);
            }
        }

        public List<SupplierMasterDTO> GetPagedSupplierMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds, bool IsArchived, bool IsDeleted, bool IsForBom, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            string UDF1 = "";
            string UDF2 = "";
            string UDF3 = "";
            string UDF4 = "";
            string UDF5 = "";
            string UDF6 = "";
            string UDF7 = "";
            string UDF8 = "";
            string UDF9 = "";
            string UDF10 = "";

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    string[] arrReplenishTypes = FieldsPara[92].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    string[] arrReplenishTypes = FieldsPara[93].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    string[] arrReplenishTypes = FieldsPara[94].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    string[] arrReplenishTypes = FieldsPara[95].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    string[] arrReplenishTypes = FieldsPara[96].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var supplierIds = (SupplierIds != null && SupplierIds.Any()) ? string.Join(",", SupplierIds) : string.Empty;

                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@SupplierIDs", supplierIds),
                    new SqlParameter("@IsForBom", IsForBom),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@UDF6", UDF6),
                    new SqlParameter("@UDF7", UDF7),
                    new SqlParameter("@UDF8", UDF8),
                    new SqlParameter("@UDF9", UDF9),
                    new SqlParameter("@UDF10", UDF10)
                };

                List<SupplierMasterDTO> lstcats = context.Database.SqlQuery<SupplierMasterDTO>("exec [GetPagedSupplierMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@SupplierIDs,@IsForBom,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@UDF6,@UDF7,@UDF8,@UDF9,@UDF10", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords;
                }

                return lstcats;
            }
        }
        public List<SupplierMasterDTO> GetSupplierMasterChangeLog(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetSupplierMasterChangeLog] @ID", params1).ToList();
            }
        }

        public bool RemoveSupplierImage(Guid SupplierGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveSupplierImage] '" + Convert.ToString(SupplierGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }
        #endregion

        #region [IDisposable interface]

        //bool disposed = false;
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposed)
        //        return;

        //    if (disposing)
        //    {
        //        // Free any other managed objects here. 
        //        //
        //    }

        //    // Free any unmanaged objects here. 
        //    //
        //    disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        #endregion

        public List<SupplierMasterDTO> GetNonDeletedSupplierByIDsNormal(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<SupplierMasterDTO>("exec [GetNonDeletedSupplierByIDsNormal] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
    }
}
