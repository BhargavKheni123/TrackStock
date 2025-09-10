using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using System.Data.Objects;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class SupplierMasterDAL : eTurnsBaseDAL
    {

        public SupplierMasterDTO GetRecord(string SupplierName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.SupplierName.ToLower().Trim() == SupplierName.ToLower().Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Get Particullar Record from the supplier Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public SupplierMasterDTO GetRecord(Int64? id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.ID == id).SingleOrDefault();

            //            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //            {

            //                return (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT 
            //                             A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID 
            //                        FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
            //                                              LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
            //                                              LEFT OUTER JOIN Room D on A.Room = D.ID 
            //                        WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //                        select new SupplierMasterDTO
            //                        {
            //                            ID = u.ID,
            //                            SupplierName = u.SupplierName,
            //                            Description = u.Description,
            //                            AccountNo = u.AccountNo,
            //                            ReceiverID = u.ReceiverID,
            //                            Address = u.Address,
            //                            City = u.City,
            //                            State = u.State,
            //                            ZipCode = u.ZipCode,
            //                            Country = u.Country,
            //                            Contact = u.Contact,
            //                            Phone = u.Phone,
            //                            Fax = u.Fax,
            //                            Email = u.Email,
            //                            IsEmailPOInBody = u.IsEmailPOInBody,
            //                            IsEmailPOInPDF = u.IsEmailPOInPDF,
            //                            IsEmailPOInCSV = u.IsEmailPOInCSV,
            //                            IsEmailPOInX12 = u.IsEmailPOInX12,
            //                            Created = u.Created,
            //                            LastUpdated = u.LastUpdated,
            //                            CreatedBy = u.CreatedBy,
            //                            LastUpdatedBy = u.LastUpdatedBy,
            //                            Room = u.Room,
            //                            GUID = u.GUID,
            //                            IsDeleted = u.IsDeleted,
            //                            IsArchived = u.IsArchived,
            //                            CreatedByName = u.CreatedByName,
            //                            UpdatedByName = u.UpdatedByName,
            //                            RoomName = u.RoomName,
            //                            CompanyID = u.CompanyID,
            //                            UDF1 = u.UDF1,
            //                            UDF2 = u.UDF2,
            //                            UDF3 = u.UDF3,
            //                            UDF4 = u.UDF4,
            //                            UDF5 = u.UDF5
            //                        }).SingleOrDefault();
            //            }
        }

        /// <summary>
        /// Delete Particullar Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                MessageDTO objMSG = new MessageDTO();

                SupplierMaster obj = context.SupplierMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.SupplierMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<SupplierMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierMasterDTO>>.GetCacheItem("Cached_SupplierMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<SupplierMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<SupplierMasterDTO>>.AppendToCacheItem("Cached_SupplierMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }



        public IEnumerable<SupplierMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom).OrderBy("ID DESC");
        }


        public IEnumerable<SupplierMasterDTO> GetAllRecordsOnlyImages()
        {
            IEnumerable<SupplierMasterDTO> ObjCache = null;
            //Get Cached-Media


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<SupplierMasterDTO> obj = (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT A.*
                        FROM SupplierMaster A 
                        where  Isnull(A.SupplierImage,'') != ''
                        ")
                                                      select new SupplierMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          Room = u.Room,
                                                          GUID = u.GUID,
                                                          CompanyID = u.CompanyID,
                                                          SupplierImage = u.SupplierImage,
                                                          ImageType = u.ImageType,

                                                      }).AsParallel().ToList();
                ObjCache = obj;
                return obj;
            }




        }

        public SupplierMasterDTO GetRecord(Int64? id, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {
            SupplierMasterDTO dto = null;
            dto = GetCachedData(RoomID, CompanyID, false, false, IsForBom).Where(t => t.ID == id).SingleOrDefault();
            if (dto == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    dto = (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT 
                                             A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID 
                                        FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                              LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                              LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        WHERE  A.ID=" + id.ToString())
                           select new SupplierMasterDTO
                           {
                               ID = u.ID,
                               SupplierName = u.SupplierName,
                               SupplierColor = u.SupplierColor,
                               Description = u.Description,
                               //AccountNo = u.AccountNo,
                               ReceiverID = u.ReceiverID,
                               Address = u.Address,
                               City = u.City,
                               State = u.State,
                               ZipCode = u.ZipCode,
                               Country = u.Country,
                               Contact = u.Contact,
                               Phone = u.Phone,
                               Fax = u.Fax,
                               Email = u.Email,
                               IsEmailPOInBody = u.IsEmailPOInBody,
                               IsEmailPOInPDF = u.IsEmailPOInPDF,
                               IsEmailPOInCSV = u.IsEmailPOInCSV,
                               IsEmailPOInX12 = u.IsEmailPOInX12,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               GUID = u.GUID,
                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               CompanyID = u.CompanyID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               BranchNumber = u.BranchNumber,
                               MaximumOrderSize = u.MaximumOrderSize,
                               IsSendtoVendor = u.IsSendtoVendor,
                               IsVendorReturnAsn = u.IsVendorReturnAsn,
                               IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                               POAutoSequence = u.POAutoSequence,
                               ScheduleType = u.ScheduleType,
                               Days = u.Days,
                               WeekDays = u.WeekDays,
                               MonthDays = u.MonthDays,
                               ScheduleTime = u.ScheduleTime,
                               IsAutoGenerate = u.IsAutoGenerate,
                               IsAutoGenerateSubmit = u.IsAutoGenerateSubmit,
                               isForBOM = u.isForBOM,
                               RefBomId = u.RefBomId,
                               NextOrderNo = u.NextOrderNo,
                               MaxOrderSize = u.MaxOrderSize,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb
                           }).SingleOrDefault();
                }
            }
            return dto;
        }


        public IEnumerable<SupplierMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            IEnumerable<SupplierMasterDTO> ObjCache = null;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                //ObjCache = CacheHelper<IEnumerable<SupplierMasterDTO>>.GetCacheItem("Cached_SupplierMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        ObjCache = (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                    select new SupplierMasterDTO
                                    {
                                        ID = u.ID,
                                        SupplierName = u.SupplierName,
                                        SupplierColor = u.SupplierColor,
                                        Description = u.Description,
                                        //AccountNo = u.AccountNo,
                                        ReceiverID = u.ReceiverID,
                                        Address = u.Address,
                                        City = u.City,
                                        State = u.State,
                                        ZipCode = u.ZipCode,
                                        Country = u.Country,
                                        Contact = u.Contact,
                                        Phone = u.Phone,
                                        Fax = u.Fax,
                                        Email = u.Email,
                                        IsEmailPOInBody = u.IsEmailPOInBody,
                                        IsEmailPOInPDF = u.IsEmailPOInPDF,
                                        IsEmailPOInCSV = u.IsEmailPOInCSV,
                                        IsEmailPOInX12 = u.IsEmailPOInX12,
                                        Created = u.Created,
                                        LastUpdated = u.LastUpdated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        Room = u.Room,
                                        GUID = u.GUID,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        RoomName = u.RoomName,
                                        CompanyID = u.CompanyID,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        BranchNumber = u.BranchNumber,
                                        MaximumOrderSize = u.MaximumOrderSize,
                                        IsSendtoVendor = u.IsSendtoVendor,
                                        IsVendorReturnAsn = u.IsVendorReturnAsn,
                                        IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                                        POAutoSequence = u.POAutoSequence,
                                        ScheduleType = u.ScheduleType,
                                        Days = u.Days,
                                        WeekDays = u.WeekDays,
                                        MonthDays = u.MonthDays,
                                        ScheduleTime = u.ScheduleTime,
                                        IsAutoGenerate = u.IsAutoGenerate,
                                        IsAutoGenerateSubmit = u.IsAutoGenerateSubmit,
                                        isForBOM = u.isForBOM,
                                        RefBomId = u.RefBomId,
                                        NextOrderNo = u.NextOrderNo,
                                        MaxOrderSize = u.MaxOrderSize,
                                        PullPurchaseNumberType = u.PullPurchaseNumberType,
                                        LastPullPurchaseNumberUsed = u.LastPullPurchaseNumberUsed,
                                        AddedFrom = u.AddedFrom,
                                        EditedFrom = u.EditedFrom,
                                        ReceivedOn = u.ReceivedOn,
                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                        SupplierImage = u.SupplierImage,
                                        ImageType = u.ImageType,
                                        ImageExternalURL = u.ImageExternalURL,
                                        DefaultOrderRequiredDays = u.DefaultOrderRequiredDays,
                                        POAutoNrFixedValue = u.POAutoNrFixedValue,
                                        PullPurchaseNrFixedValue = u.PullPurchaseNrFixedValue
                                    }).AsParallel().ToList();

                    }
                }
            }
            else
            {
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new SupplierMasterDTO
                                {
                                    ID = u.ID,
                                    SupplierName = u.SupplierName,
                                    SupplierColor = u.SupplierColor,
                                    Description = u.Description,
                                    //AccountNo = u.AccountNo,
                                    ReceiverID = u.ReceiverID,
                                    Address = u.Address,
                                    City = u.City,
                                    State = u.State,
                                    ZipCode = u.ZipCode,
                                    Country = u.Country,
                                    Contact = u.Contact,
                                    Phone = u.Phone,
                                    Fax = u.Fax,
                                    Email = u.Email,
                                    IsEmailPOInBody = u.IsEmailPOInBody,
                                    IsEmailPOInPDF = u.IsEmailPOInPDF,
                                    IsEmailPOInCSV = u.IsEmailPOInCSV,
                                    IsEmailPOInX12 = u.IsEmailPOInX12,
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    GUID = u.GUID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    CompanyID = u.CompanyID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    BranchNumber = u.BranchNumber,
                                    MaximumOrderSize = u.MaximumOrderSize,
                                    IsSendtoVendor = u.IsSendtoVendor,
                                    IsVendorReturnAsn = u.IsVendorReturnAsn,
                                    IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                                    POAutoSequence = u.POAutoSequence,
                                    ScheduleType = u.ScheduleType,
                                    Days = u.Days,
                                    WeekDays = u.WeekDays,
                                    MonthDays = u.MonthDays,
                                    ScheduleTime = u.ScheduleTime,
                                    IsAutoGenerate = u.IsAutoGenerate,
                                    IsAutoGenerateSubmit = u.IsAutoGenerateSubmit,
                                    isForBOM = u.isForBOM,
                                    RefBomId = u.RefBomId,
                                    NextOrderNo = u.NextOrderNo,
                                    MaxOrderSize = u.MaxOrderSize,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    SupplierImage = u.SupplierImage,
                                    ImageType = u.ImageType,
                                    ImageExternalURL = u.ImageExternalURL
                                }).AsParallel().ToList();
                }
            }

            if (IsForBom == null)
            {
                return ObjCache;
            }
            else if (IsForBom == true)
            {
                return ObjCache.Where(t => t.Room == 0);
            }
            else
            {
                return ObjCache.Where(t => t.Room == RoomID);
            }
        }

        public SchedulerDTO SaveSupplierScheduleAsset(SchedulerDTO objSchedulerDTO)
        {
            if (!string.IsNullOrEmpty(objSchedulerDTO.ScheduleRunTime))
            {
                string strtmp = DateTime.Now.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
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
                    //RoomScheduleDetail objRoomScheduleDetail = context.RoomScheduleDetails.Where(t => t.ExecuitionDate > DateTime.Now && t.ScheduleID == objRoomSchedule.ScheduleID).OrderBy(t => t.ExecuitionDate).FirstOrDefault();
                    //if (objRoomScheduleDetail != null)
                    //{
                    //    objRoomSchedule.NextRunDate = objRoomScheduleDetail.ExecuitionDate;
                    //}
                    context.RoomSchedules.AddObject(objRoomSchedule);
                    context.SaveChanges();
                    objSchedulerDTO.ScheduleID = objRoomSchedule.ScheduleID;
                }
                else
                {
                    objRoomSchedule = context.RoomSchedules.FirstOrDefault(t => t.AssetToolID == objSchedulerDTO.AssetToolID && t.RoomId == objSchedulerDTO.RoomId);
                    if (objRoomSchedule != null)
                    {
                        objRoomSchedule.DailyRecurringDays = objSchedulerDTO.DailyRecurringDays;
                        objRoomSchedule.DailyRecurringType = objSchedulerDTO.DailyRecurringType;
                        objRoomSchedule.LastUpdatedBy = objSchedulerDTO.LastUpdatedBy;
                        objRoomSchedule.MonthlyDateOfMonth = objSchedulerDTO.MonthlyDateOfMonth;
                        objRoomSchedule.MonthlyDayOfMonth = objSchedulerDTO.MonthlyDayOfMonth;
                        objRoomSchedule.MonthlyRecurringMonths = objSchedulerDTO.MonthlyRecurringMonths;
                        objRoomSchedule.MonthlyRecurringType = objSchedulerDTO.MonthlyRecurringType;
                        objRoomSchedule.ScheduleFor = objSchedulerDTO.LoadSheduleFor;
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
                        // objRoomSchedule.ScheduleRunTime = objSchedulerDTO.ScheduleRunDateTime;
                        objRoomSchedule.AssetToolID = objSchedulerDTO.AssetToolID;
                        //objRoomSchedule.NextRunDate = GetScheduleNextRunDate(objSchedulerDTO);
                        objRoomSchedule.Updated = DateTimeUtility.DateTimeNow;
                        context.SaveChanges();
                    }
                }
                //context.ExecuteStoreCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
                //context.ExecuteStoreCommand("EXEC GENERATESCHEDULEFORORDERTRANSFER " + objRoomSchedule.ScheduleID + "");
            }
            //GenerateSupplierSchedule(objSchedulerDTO);
            return objSchedulerDTO;
        }


        public SupplierMaster GetSupplierByName(string SupplierName, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from sm in context.SupplierMasters
                        where sm.SupplierName == SupplierName && sm.Room == RoomId && sm.CompanyID == CompanyId && sm.IsDeleted == false && sm.IsArchived == false
                        select sm).FirstOrDefault();
            }
        }

        public List<SupplierMasterDTO> GetSupplierByRoomsIds(string RoomIDs, string CompanyIDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!String.IsNullOrWhiteSpace(RoomIDs) && !string.IsNullOrWhiteSpace(CompanyIDs))
                {
                    return (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT 
                                             A.*, D.RoomName
                                        FROM SupplierMaster A       
                                        LEFT OUTER JOIN Room D on A.Room = D.ID                                                       
                                        WHERE  A.Room in (" + RoomIDs + ") And  A.CompanyID in (" + CompanyIDs + ") and isnull(A.IsDeleted,0)=0 Order by A.SupplierName ASC")
                            select new SupplierMasterDTO
                            {
                                ID = u.ID,
                                SupplierName = u.SupplierName,
                                RoomName = u.RoomName,
                                LastPullPurchaseNumberUsed = u.LastPullPurchaseNumberUsed,
                                PullPurchaseNrFixedValue = u.PullPurchaseNrFixedValue,
                                PullPurchaseNumberType = u.PullPurchaseNumberType,

                            }).ToList();
                }
                else
                {
                    return new List<SupplierMasterDTO>();
                }
            }
        }


        private DateTime? GetScheduleNextRunDate(SchedulerDTO objSchedulerDTO)
        {
            DateTime? NextRunDate = null;
            switch (objSchedulerDTO.ScheduleMode)
            {
                case 0:
                    NextRunDate = null;
                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3:
                    break;
                default:
                    NextRunDate = null;
                    break;
            }
            return NextRunDate;
        }

        public SupplierMasterDTO GetSupplierByID(Int64? id, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {
            SupplierMasterDTO dto = null;

            if (dto == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    dto = (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT 
                                             A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID 
                                        FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                              LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                              LEFT OUTER JOIN Room D on A.Room = D.ID 
                                        WHERE  A.ID=" + id.ToString())
                           select new SupplierMasterDTO
                           {
                               ID = u.ID,
                               SupplierName = u.SupplierName,
                               SupplierColor = u.SupplierColor,
                               Description = u.Description,
                               //AccountNo = u.AccountNo,
                               ReceiverID = u.ReceiverID,
                               Address = u.Address,
                               City = u.City,
                               State = u.State,
                               ZipCode = u.ZipCode,
                               Country = u.Country,
                               Contact = u.Contact,
                               Phone = u.Phone,
                               Fax = u.Fax,
                               Email = u.Email,
                               IsEmailPOInBody = u.IsEmailPOInBody,
                               IsEmailPOInPDF = u.IsEmailPOInPDF,
                               IsEmailPOInCSV = u.IsEmailPOInCSV,
                               IsEmailPOInX12 = u.IsEmailPOInX12,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               GUID = u.GUID,
                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               CompanyID = u.CompanyID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               BranchNumber = u.BranchNumber,
                               MaximumOrderSize = u.MaximumOrderSize,
                               IsSendtoVendor = u.IsSendtoVendor,
                               IsVendorReturnAsn = u.IsVendorReturnAsn,
                               IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                               POAutoSequence = u.POAutoSequence,
                               ScheduleType = u.ScheduleType,
                               Days = u.Days,
                               WeekDays = u.WeekDays,
                               MonthDays = u.MonthDays,
                               ScheduleTime = u.ScheduleTime,
                               IsAutoGenerate = u.IsAutoGenerate,
                               IsAutoGenerateSubmit = u.IsAutoGenerateSubmit,
                               isForBOM = u.isForBOM,
                               RefBomId = u.RefBomId,
                               NextOrderNo = u.NextOrderNo,
                               MaxOrderSize = u.MaxOrderSize,
                               PullPurchaseNumberType = u.PullPurchaseNumberType,
                               LastPullPurchaseNumberUsed = u.LastPullPurchaseNumberUsed,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               DefaultOrderRequiredDays = u.DefaultOrderRequiredDays,
                               POAutoNrFixedValue = u.POAutoNrFixedValue,
                               PullPurchaseNrFixedValue = u.PullPurchaseNrFixedValue
                           }).FirstOrDefault();
                }
            }
            return dto;
        }

        private void GenerateSupplierSchedule(SchedulerDTO objSchedulerDTO)
        {
            switch (objSchedulerDTO.ScheduleMode)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3:
                    break;
                default:

                    break;
            }

        }

        public List<SupplierMasterDTO> GetAllRecordsForExport(string ids, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom, string SortNameString)
        {
            IEnumerable<SupplierMasterDTO> DataFromDB = GetAllRecords(RoomID, CompanyID, false, false, false).OrderBy(SortNameString);

            List<SupplierMasterDTO> lstSupplierMasterDTO = new List<SupplierMasterDTO>();
            string[] arrid = ids.Split(',');
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //  if (!string.IsNullOrEmpty(ids))
                {
                    lstSupplierMasterDTO = (from u in DataFromDB
                                                // where (string.IsNullOrWhiteSpace(ids)) || arrid.Contains(u.ID.ToString())
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
                                                SupplierImage = u.SupplierImage,
                                                ImageExternalURL = u.ImageExternalURL,
                                                SupplierAccountDetails = (from A in context.SupplierAccountDetails
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
                                                SupplierBlanketPODetails = (from A in context.SupplierBlanketPODetails
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
                                                                                IsNotExceedQty = A.IsNotExceedQty
                                                                            }).ToList(),

                                            }).ToList();
                }
            }
            return lstSupplierMasterDTO;
        }

        public bool updateImageName(long Id, string fileName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierMaster objSupplierMaster = context.SupplierMasters.FirstOrDefault(t => t.ID == Id);
                if (objSupplierMaster != null)
                {
                    objSupplierMaster.SupplierImage = fileName;
                    context.SaveChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE SupplierMaster SET LastUpdated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<SupplierMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierMasterDTO>>.GetCacheItem("Cached_SupplierMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<SupplierMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<SupplierMasterDTO>>.AppendToCacheItem("Cached_SupplierMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }


        public void SetDefaultAccFalse(List<long> AccountID, long RoomId, long CompanyId, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierAccountDetail objSupplierAccountDetails = (from A in context.SupplierAccountDetails
                                                                   where AccountID.Contains(A.ID) && !A.IsDeleted && !A.IsArchived
                                                                   && A.Room == RoomId && A.CompanyID == CompanyId
                                                                   select A).FirstOrDefault();
                objSupplierAccountDetails.IsDefault = false;
                objSupplierAccountDetails.Updated = DateTime.UtcNow;
                if (UserID != 0)
                    objSupplierAccountDetails.LastUpdatedBy = UserID;
                context.SaveChanges();
            }
        }


        public List<SupplierAccountDetailsDTO> GetSupplierAccountDetails(long SupplierID, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SupplierAccountDetailsDTO> lstSupplierAccountDetailsDTO = (from A in context.SupplierAccountDetails
                                                                                where A.SupplierID == SupplierID && !A.IsDeleted && !A.IsArchived
                                                                                && A.Room == RoomId && A.CompanyID == CompanyId
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
                                                                                    ShipToID = A.ShipToID,

                                                                                }).ToList();
                return lstSupplierAccountDetailsDTO;
            }
        }


        #region [for service]

        //public SupplierMasterDTO GetRecordForService(Int64? id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ConnectionString)
        //{
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        return (from u in context.ExecuteStoreQuery<SupplierMasterDTO>(@"SELECT 
        //                                 A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID 
        //                            FROM SupplierMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
        //                                                  LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                                                  LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                            WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
        //                select new SupplierMasterDTO
        //                {
        //                    ID = u.ID,
        //                    SupplierName = u.SupplierName,
        //                    Description = u.Description,
        //                    ReceiverID = u.ReceiverID,
        //                    Address = u.Address,
        //                    City = u.City,
        //                    State = u.State,
        //                    ZipCode = u.ZipCode,
        //                    Country = u.Country,
        //                    Contact = u.Contact,
        //                    Phone = u.Phone,
        //                    Fax = u.Fax,
        //                    Email = u.Email,
        //                    IsEmailPOInBody = u.IsEmailPOInBody,
        //                    IsEmailPOInPDF = u.IsEmailPOInPDF,
        //                    IsEmailPOInCSV = u.IsEmailPOInCSV,
        //                    IsEmailPOInX12 = u.IsEmailPOInX12,
        //                    Created = u.Created,
        //                    LastUpdated = u.LastUpdated,
        //                    CreatedBy = u.CreatedBy,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    Room = u.Room,
        //                    GUID = u.GUID,
        //                    IsDeleted = u.IsDeleted,
        //                    IsArchived = u.IsArchived,
        //                    CreatedByName = u.CreatedByName,
        //                    UpdatedByName = u.UpdatedByName,
        //                    RoomName = u.RoomName,
        //                    CompanyID = u.CompanyID,
        //                    UDF1 = u.UDF1,
        //                    UDF2 = u.UDF2,
        //                    UDF3 = u.UDF3,
        //                    UDF4 = u.UDF4,
        //                    UDF5 = u.UDF5,
        //                    BranchNumber = u.BranchNumber
        //                }).SingleOrDefault();
        //    }
        //}

        #endregion


        public IEnumerable<SupplierMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, Int64 SupplierID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            IEnumerable<SupplierMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom);
            if (IsForBom)
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == true && t.Room.GetValueOrDefault(0) == 0);
            }

            if (SupplierID > 0)
            {
                ObjCache = ObjCache.Where(t => t.ID == SupplierID).ToList();
            }

            //else
            //{
            //    ObjCache = ObjCache.Where(t => t.isForBOM == false);
            //}
            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<SupplierMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<SupplierMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
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
                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains((t.CreatedBy ?? 0).ToString())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains((t.LastUpdatedBy ?? 0).ToString())))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.LastUpdated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.SupplierName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.AccountNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<SupplierMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.SupplierName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.AccountNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.SupplierName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.AccountNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Address ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Contact ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Country ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }




    }
}
