using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace eTurns.DAL
{
    public class ToolsOdometerDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public ToolsOdometerDAL(base.DataBaseName)
        //{

        //}

        public ToolsOdometerDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolsOdometerDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ToolsOdometerDTO> GetCachedData(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";

            if (AssetGUID != null)
            {
                sSQL += " A.AssetGUID = '" + AssetGUID.Value + "'";
            }
            else if (ToolGUID != null)
            {
                sSQL += " A.ToolGUID = '" + ToolGUID.Value + "'";
            }
            sSQL += " AND ";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsOdometerDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                            FROM ToolsOdometer A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
                        select new ToolsOdometerDTO
                        {
                            ID = u.ID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            EntryDate = u.EntryDate,
                            Odometer = u.Odometer,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                        }).AsParallel().ToList();
            }
        }

        /// <summary>
        /// Get Paged Records from the ToolsOdometer Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolsOdometerDTO> GetAllRecords(Guid? AssetGUID, Guid? ToolGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(AssetGUID, ToolGUID, RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");
        }

        ///// <summary>
        ///// Get Paged Records from the ToolsOdometer Table
        ///// </summary>
        ///// <param name="StartRowIndex">StartRowIndex</param>
        ///// <param name="MaxRows">MaxRows</param>
        ///// <param name="TotalCount">TotalCount</param>
        ///// <param name="SearchTerm">SearchTerm</param>
        ///// <param name="sortColumnName">sortColumnName</param>
        ///// <returns></returns>
        //public IEnumerable<ToolsOdometerDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        //{
        //    //Get Cached-Media
        //    IEnumerable<ToolsOdometerDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //    IEnumerable<ToolsOdometerDTO> ObjGlobalCache = ObjCache;
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
        //        //IEnumerable<ToolsOdometerDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.Created.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.Updated.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
        //            && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
        //            && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
        //            && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
        //            && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
        //            );

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
        //    }
        //    else
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<ToolsOdometerDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).Count();
        //        return ObjCache.Where
        //            (t => t.ID.ToString().Contains(SearchTerm) ||
        //                (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
        //                (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //}


        /// <summary>
        /// Get Particullar Record from the ToolsOdometer by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolsOdometerDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            string sSQL = "";
            if (IsArchived == false && IsDeleted == false)
            {
                sSQL += " A.IsDeleted != 1 AND A.IsArchived != 1 ";
            }
            else if (IsArchived && IsDeleted)
            {
                sSQL += " A.IsDeleted = 1 AND A.IsArchived = 1 ";
            }
            else if (IsArchived)
            {
                sSQL += " A.IsArchived = 1 ";
            }
            else if (IsDeleted)
            {
                sSQL += " A.IsDeleted =1 ";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsOdometerDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                            FROM ToolsOdometer A 
                            LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                            LEFT OUTER JOIN Room D on A.Room = D.ID 
                            WHERE A.GUID = '" + GUID.ToString() + "' AND A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND " + sSQL)
                        select new ToolsOdometerDTO
                        {
                            ID = u.ID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            EntryDate = u.EntryDate,
                            Odometer = u.Odometer,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                        }).SingleOrDefault();
            }


        }


        /// <summary>
        /// Get Particullar Record from the ToolsOdometer by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolsOdometerDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Database.SqlQuery<ToolsOdometerDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolsOdometer_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ToolsOdometerDTO
                        {
                            ID = u.ID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            EntryDate = u.EntryDate,
                            Odometer = u.Odometer,
                            Created = u.Created,
                            CreatedBy = u.CreatedBy,
                            Updated = u.Updated,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsArchived = u.IsArchived,
                            IsDeleted = u.IsDeleted,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase ToolsOdometer
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ToolsOdometerDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsOdometer obj = new ToolsOdometer();
                obj.ID = 0;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.EntryDate = objDTO.EntryDate;
                obj.Odometer = objDTO.Odometer;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                objDTO.GUID = Guid.NewGuid();
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                context.ToolsOdometers.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                try
                {
                    //Execute Odometer logic for Approximate schedule date and maintenance entry.
                    if (objDTO.AssetGUID != null)
                    {
                        context.Database.ExecuteSqlCommand(@"EXEC [dbo].[Job_ScheduleMaintenanceForOdometer] '" + objDTO.GUID.ToString() + "','" + objDTO.AssetGUID.ToString() + "',null," + objDTO.Room.ToString() + "," + objDTO.CompanyID);
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand(@"EXEC [dbo].[Job_ScheduleMaintenanceForOdometer] '" + objDTO.GUID.ToString() + "',null,'" + objDTO.ToolGUID.ToString() + "'," + objDTO.Room.ToString() + "," + objDTO.CompanyID);
                    }
                }
                catch
                {
                    throw;
                }
                return obj.ID;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ToolsOdometerDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolsOdometer obj = new ToolsOdometer();
                obj.ID = objDTO.ID;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.AssetGUID = objDTO.AssetGUID;
                obj.EntryDate = objDTO.EntryDate;
                obj.Odometer = objDTO.Odometer;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.Updated = objDTO.Updated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = objDTO.GUID;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                context.ToolsOdometers.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();



                //Execute Odometer logic for Approximate schedule date and maintenance entry.
                if (objDTO.AssetGUID != null)
                {
                    context.Database.ExecuteSqlCommand("EXEC [dbo].[Job_ScheduleMaintenanceForOdometer] '" + objDTO.GUID.ToString() + "','" + objDTO.AssetGUID + "',null,'" + objDTO.Room.ToString() + "','" + objDTO.CompanyID + "'");
                }
                else
                {
                    context.Database.ExecuteSqlCommand("EXEC [dbo].[Job_ScheduleMaintenanceForOdometer] '" + objDTO.GUID.ToString() + "',null,'" + objDTO.ToolGUID + "','" + objDTO.Room.ToString() + "','" + objDTO.CompanyID + "'");
                }



                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ToolsOdometer SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE GUID ='" + item.ToString() + "';";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);
                return true;
            }
        }

    }
}


