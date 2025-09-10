using eTurns.DTO;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace eTurns.DAL
{
    public class eVMISetupDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public eVMISetupDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public eVMISetupDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion
        public eVMISetupDTO GetRecord(Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Room", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<eVMISetupDTO>("exec [GetEVMIByRoom] @Room,@CompanyID", params1).FirstOrDefault();
            }
        }
        public bool Insert(eVMISetupDTO objDTO)
        {

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                eVMISetup obj = new eVMISetup();
                obj.ID = 0;
                obj.PollType = objDTO.PollType;
                obj.PollInterval = objDTO.PollInterval;
                obj.PollTime1 = objDTO.PollTime1;
                obj.PollTime2 = objDTO.PollTime2;
                obj.PollTime3 = objDTO.PollTime3;
                obj.PollTime4 = objDTO.PollTime4;
                obj.PollTime5 = objDTO.PollTime5;
                obj.PollTime6 = objDTO.PollTime6;
                obj.ErrorEmailAddresses = objDTO.ErrorEmailAddresses;
                obj.IsSuggstedOrder = objDTO.IsSuggstedOrder;
                obj.IsCriticalOrders = objDTO.IsCriticalOrders;
                obj.IsBadPolls = objDTO.IsBadPolls;
                obj.InactivityReportStart = objDTO.InactivityReportStart;
                obj.InactivityReportEnd = objDTO.InactivityReportEnd;
                obj.InactivityonSaturday = objDTO.InactivityonSaturday;
                obj.InactivityonSunday = objDTO.InactivityonSunday;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.NextPollDate = objDTO.NextPollDate;
                obj.CountType = objDTO.CountType;
                obj.PollAllBetweenTime = objDTO.PollAllBetweenTime;
                obj.ShelfID = objDTO.ShelfID;
                context.eVMISetups.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<eVMISetupDTO> ObjCache = CacheHelper<IEnumerable<eVMISetupDTO>>.GetCacheItem("Cached_eVMISetup_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<eVMISetupDTO> tempC = new List<eVMISetupDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<eVMISetupDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<eVMISetupDTO>>.AppendToCacheItem("Cached_eVMISetup_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return true;
            }

        }
        public bool Edit(eVMISetupDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                eVMISetup obj = new eVMISetup();
                obj.ID = objDTO.ID;
                obj.PollType = objDTO.PollType;
                obj.PollInterval = objDTO.PollInterval;
                obj.PollTime1 = objDTO.PollTime1;
                obj.PollTime2 = objDTO.PollTime2;
                obj.PollTime3 = objDTO.PollTime3;
                obj.PollTime4 = objDTO.PollTime4;
                obj.PollTime5 = objDTO.PollTime5;
                obj.PollTime6 = objDTO.PollTime6;
                obj.ErrorEmailAddresses = objDTO.ErrorEmailAddresses;
                obj.IsSuggstedOrder = objDTO.IsSuggstedOrder;
                obj.IsCriticalOrders = objDTO.IsCriticalOrders;
                obj.IsBadPolls = objDTO.IsBadPolls;
                obj.InactivityReportStart = objDTO.InactivityReportStart;
                obj.InactivityReportEnd = objDTO.InactivityReportEnd;
                obj.InactivityonSaturday = objDTO.InactivityonSaturday;
                obj.InactivityonSunday = objDTO.InactivityonSunday;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.NextPollDate = objDTO.NextPollDate;
                obj.CountType = objDTO.CountType;
                obj.PollAllBetweenTime = objDTO.PollAllBetweenTime;
                obj.ShelfID = objDTO.ShelfID;
                context.eVMISetups.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<eVMISetupDTO> ObjCache = CacheHelper<IEnumerable<eVMISetupDTO>>.GetCacheItem("Cached_eVMISetup_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<eVMISetupDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<eVMISetupDTO> tempC = new List<eVMISetupDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<eVMISetupDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<eVMISetupDTO>>.AppendToCacheItem("Cached_eVMISetup_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }

        public eVMISetupDTO SaveeVMISetup(eVMISetupDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objDTO.ID > 0)
                {
                    eVMISetup objeVMISetup = context.eVMISetups.FirstOrDefault(t => t.ID == objDTO.ID);
                    if (objeVMISetup != null)
                    {
                        objeVMISetup.Updated = DateTime.UtcNow;
                        objeVMISetup.LastUpdatedBy = objDTO.LastUpdatedBy;
                        objeVMISetup.PollTime1 = objDTO.PollTime1;
                        objeVMISetup.PollTime2 = objDTO.PollTime2;
                        objeVMISetup.PollTime3 = objDTO.PollTime3;
                        objeVMISetup.PollTime4 = objDTO.PollTime4;
                        objeVMISetup.PollTime5 = objDTO.PollTime5;
                        objeVMISetup.PollTime6 = objDTO.PollTime6;
                        objeVMISetup.CountType = objDTO.CountType;
                        objeVMISetup.ScheduleTime = objDTO.ScheduleTime;
                        objeVMISetup.RoomScheduleID = objDTO.RoomScheduleID;
                        objeVMISetup.PollType = objDTO.PollType;
                        objeVMISetup.IsScheduleActive = objDTO.IsScheduleActive;
                        context.SaveChanges();
                    }
                }
                else
                {
                    eVMISetup objeVMISetup = new eVMISetup();
                    objeVMISetup.ID = objDTO.ID;
                    objeVMISetup.PollType = objDTO.PollType;
                    objeVMISetup.PollInterval = objDTO.PollInterval;
                    objeVMISetup.PollTime1 = objDTO.PollTime1;
                    objeVMISetup.PollTime2 = objDTO.PollTime2;
                    objeVMISetup.PollTime3 = objDTO.PollTime3;
                    objeVMISetup.PollTime4 = objDTO.PollTime4;
                    objeVMISetup.PollTime5 = objDTO.PollTime5;
                    objeVMISetup.PollTime6 = objDTO.PollTime6;
                    objeVMISetup.ErrorEmailAddresses = objDTO.ErrorEmailAddresses;
                    objeVMISetup.IsSuggstedOrder = objDTO.IsSuggstedOrder;
                    objeVMISetup.IsCriticalOrders = objDTO.IsCriticalOrders;
                    objeVMISetup.IsBadPolls = objDTO.IsBadPolls;
                    objeVMISetup.InactivityReportStart = objDTO.InactivityReportStart;
                    objeVMISetup.InactivityReportEnd = objDTO.InactivityReportEnd;
                    objeVMISetup.InactivityonSaturday = objDTO.InactivityonSaturday;
                    objeVMISetup.InactivityonSunday = objDTO.InactivityonSunday;
                    objeVMISetup.GUID = Guid.NewGuid();
                    objeVMISetup.Created = objDTO.Created;
                    objeVMISetup.Updated = objDTO.Updated;
                    objeVMISetup.CreatedBy = objDTO.CreatedBy;
                    objeVMISetup.LastUpdatedBy = objDTO.LastUpdatedBy;
                    objeVMISetup.IsDeleted = objDTO.IsDeleted;
                    objeVMISetup.IsArchived = objDTO.IsArchived;
                    objeVMISetup.CompanyID = objDTO.CompanyID;
                    objeVMISetup.Room = objDTO.Room;
                    objeVMISetup.NextPollDate = objDTO.NextPollDate;
                    objeVMISetup.CountType = objDTO.CountType;
                    objeVMISetup.PollAllBetweenTime = objDTO.PollAllBetweenTime;
                    objeVMISetup.ShelfID = objDTO.ShelfID;
                    objeVMISetup.ScheduleTime = objDTO.ScheduleTime;
                    objeVMISetup.RoomScheduleID = objDTO.RoomScheduleID;
                    objeVMISetup.IsScheduleActive = objDTO.IsScheduleActive;
                    context.eVMISetups.Add(objeVMISetup);
                    context.SaveChanges();
                    objDTO.ID = objeVMISetup.ID;
                }
            }
            return objDTO;
        }
        //public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        //{
        //    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        string strQuery = "";
        //        foreach (var item in IDs.Split(','))
        //        {
        //            if (!string.IsNullOrEmpty(item.Trim()))
        //            {
        //                strQuery += "UPDATE eVMISetup SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
        //            }
        //        }
        //        context.Database.ExecuteSqlCommand(strQuery);


        //        //Get Cached-Media
        //        IEnumerable<eVMISetupDTO> ObjCache = CacheHelper<IEnumerable<eVMISetupDTO>>.GetCacheItem("Cached_eVMISetup_" + CompanyID.ToString());
        //        if (ObjCache != null)
        //        {
        //            List<eVMISetupDTO> objTemp = ObjCache.ToList();
        //            objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
        //            ObjCache = objTemp.AsEnumerable();
        //            CacheHelper<IEnumerable<eVMISetupDTO>>.AppendToCacheItem("Cached_eVMISetup_" + CompanyID.ToString(), ObjCache);
        //        }

        //        return true;
        //    }
        //}
        //public eVMISetupDTO GetHistoryRecord(Int64 id)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        return (from u in context.Database.SqlQuery<eVMISetupDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM eVMISetup_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
        //                select new eVMISetupDTO
        //                {
        //                    ID = u.ID,
        //                    PollType = u.PollType,
        //                    PollInterval = u.PollInterval,
        //                    PollTime1 = u.PollTime1,
        //                    PollTime2 = u.PollTime2,
        //                    PollTime3 = u.PollTime3,
        //                    PollTime4 = u.PollTime4,
        //                    PollTime5 = u.PollTime5,
        //                    PollTime6 = u.PollTime6,
        //                    ErrorEmailAddresses = u.ErrorEmailAddresses,
        //                    IsSuggstedOrder = u.IsSuggstedOrder,
        //                    IsCriticalOrders = u.IsCriticalOrders,
        //                    IsBadPolls = u.IsBadPolls,
        //                    InactivityReportStart = u.InactivityReportStart,
        //                    InactivityReportEnd = u.InactivityReportEnd,
        //                    InactivityonSaturday = u.InactivityonSaturday,
        //                    InactivityonSunday = u.InactivityonSunday,
        //                    GUID = u.GUID,
        //                    Created = u.Created,
        //                    Updated = u.Updated,
        //                    CreatedBy = u.CreatedBy,
        //                    LastUpdatedBy = u.LastUpdatedBy,
        //                    IsDeleted = u.IsDeleted,
        //                    IsArchived = u.IsArchived,
        //                    CompanyID = u.CompanyID,
        //                    Room = u.Room,
        //                }).SingleOrDefault();
        //    }
        //}
        //public IEnumerable<eVMISetupDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        //{
        //    return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        //}
        //public IEnumerable<eVMISetupDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    //Get Cached-Media
        //    IEnumerable<eVMISetupDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //    IEnumerable<eVMISetupDTO> ObjGlobalCache = ObjCache;
        //    ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

        //    if (IsArchived && IsDeleted)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }
        //    else if (IsArchived)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }
        //    else if (IsDeleted)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<eVMISetupDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            );

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<eVMISetupDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm)).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}
        //public eVMISetupDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        //{
        //    return GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.ID == id);
        //}

        #region [ForService]

        //public IEnumerable<eVMISetupDTO> GetCachedDataForService(Int64 RoomID, Int64 CompanyID, string ConnectionString)
        //{
        //    //Get Cached-Media
        //    IEnumerable<eVMISetupDTO> ObjCache;
        //    using (var context = new eTurnsEntities(ConnectionString))
        //    {
        //        ObjCache = (from u in context.Database.SqlQuery<eVMISetupDTO>(@"
        //                    SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
        //                    FROM eVMISetup A 
        //                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
        //                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
        //                    LEFT OUTER JOIN Room D on A.Room = D.ID 
        //                    WHERE A.CompanyID = " + CompanyID.ToString())
        //                    select new eVMISetupDTO
        //                    {
        //                        ID = u.ID,
        //                        PollType = u.PollType.GetValueOrDefault(1),
        //                        PollInterval = u.PollInterval,
        //                        PollTime1 = u.PollTime1,
        //                        PollTime2 = u.PollTime2,
        //                        PollTime3 = u.PollTime3,
        //                        PollTime4 = u.PollTime4,
        //                        PollTime5 = u.PollTime5,
        //                        PollTime6 = u.PollTime6,
        //                        ErrorEmailAddresses = u.ErrorEmailAddresses,
        //                        IsSuggstedOrder = u.IsSuggstedOrder,
        //                        IsCriticalOrders = u.IsCriticalOrders,
        //                        IsBadPolls = u.IsBadPolls,
        //                        InactivityReportStart = u.InactivityReportStart,
        //                        InactivityReportEnd = u.InactivityReportEnd,
        //                        InactivityonSaturday = u.InactivityonSaturday,
        //                        InactivityonSunday = u.InactivityonSunday,
        //                        GUID = u.GUID,
        //                        Created = u.Created,
        //                        Updated = u.Updated,
        //                        CreatedBy = u.CreatedBy,
        //                        LastUpdatedBy = u.LastUpdatedBy,
        //                        IsDeleted = u.IsDeleted,
        //                        IsArchived = u.IsArchived,
        //                        CompanyID = u.CompanyID,
        //                        Room = u.Room,
        //                        CreatedByName = u.CreatedByName,
        //                        UpdatedByName = u.UpdatedByName,
        //                        RoomName = u.RoomName,
        //                    }).AsParallel().ToList();
        //    }
        //    return ObjCache.Where(x => x.Room == RoomID);
        //}

        //public eVMISetupDTO GetRecordForService(Int64 RoomID, Int64 CompanyID, string ConnectionString)
        //{
        //    return GetCachedDataForService(RoomID, CompanyID, ConnectionString).FirstOrDefault();
        //}
        #endregion

        public List<eVMISetupDTO> GetAlleVMIRecord(int PollType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@PollType", PollType) };
                return context.Database.SqlQuery<eVMISetupDTO>("exec [GetAlleVMIRecord] @PollType", params1).ToList();
            }
        }

        public List<eVMISetupDTO> GetAlleVMIRecordForeVMIRoom()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<eVMISetupDTO>("exec [GetAlleVMIRecordForeVMIRoom]").ToList();
            }
        }

        public void UpdateNextPollDate(eVMISetupDTO objeVMISetupDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ID", objeVMISetupDTO.ID),
                                                   new SqlParameter("@NextPollDate", objeVMISetupDTO.NextPollDate) };

                context.Database.ExecuteSqlCommand("Exec UpdateNextPollDate @ID,@NextPollDate", params1);
            }
        }

        public void UpdateNextRoomSchedulePollDate(Int64 RoomScheduleID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomScheduleID", RoomScheduleID) };

                context.Database.ExecuteSqlCommand("Exec [GetNexteVMIRunTime] @RoomScheduleID", params1);
            }
        }

        public List<EVMIMissedPollDTO> GetEVMIMissedPolls()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var list = context.Database.SqlQuery<EVMIMissedPollDTO>("Exec GetEVMIMissedPolls ").ToList();
                return list;
            }
        }
        public SchedulerDTO SaveeVMISchedule(SchedulerDTO objSchedulerDTO)
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
                        //context.Database.ExecuteSqlCommand("EXEC SetNexteVMIRunTime " + objRoomSchedule.ScheduleID + "");
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
                            //context.Database.ExecuteSqlCommand("EXEC RPT_GetNextReportRunTime_nd " + objRoomSchedule.ScheduleID + "");
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
        public void SetNexteVMIRunTime(long eVMISetupID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@eVMISetupID", eVMISetupID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec dbo.SetNexteVMIRunTime @eVMISetupID", params1);
            }
        }
    }
}


