using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;
using System.Web;

namespace eTurns.DAL
{
    public partial class LocationMasterDAL : eTurnsBaseDAL
    {
        public LocationMasterDTO GetLocationByID(long LocationID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@LocationID", LocationID) };
                LocationMasterDTO obj = context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationsByID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@LocationID", params1).ToList().FirstOrDefault();
                return obj;
            }

        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocationMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<LocationMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<LocationMasterDTO>>.GetCacheItem("Cached_LocationMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<LocationMasterDTO> obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM LocationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND isnull(A.Location,'') != '' AND A.CompanyID = " + CompanyID.ToString())
                                                              select new LocationMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Location = u.Location,
                                                                  Created = u.Created,
                                                                  LastUpdated = u.LastUpdated,
                                                                  CreatedBy = u.CreatedBy,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  Room = u.Room,
                                                                  RoomName = u.RoomName,
                                                                  GUID = u.GUID,
                                                                  CompanyID = u.CompanyID,
                                                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                  UDF1 = u.UDF1,
                                                                  UDF2 = u.UDF2,
                                                                  UDF3 = u.UDF3,
                                                                  UDF4 = u.UDF4,
                                                                  UDF5 = u.UDF5
                                                              }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<LocationMasterDTO>>.AddCacheItem("Cached_LocationMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<LocationMasterDTO>(@"SELECT A.ID,A.Location, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.LastUpdated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5, A.IsDeleted, A.IsArchived, A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM LocationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID  WHERE A.CompanyID = " + CompanyID.ToString() + @" AND A.Room = " + RoomID.ToString() + " AND " + sSQL)
                                select new LocationMasterDTO
                                {
                                    ID = u.ID,
                                    Location = u.Location,
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    GUID = u.GUID,
                                    Room = u.Room,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5
                                }).AsParallel().ToList();
                }

            }

            return ObjCache.ToList().Where(t => t.Room == RoomID);
        }


        /// <summary>
        /// Update Data - Grid Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE LocationMaster SET " + columnName + " = '" + value + "', LastUpdated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;
        }

        public IEnumerable<LocationMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");


        }

        public IEnumerable<LocationMasterDTO> GetAllLocationStartWith(Int64 RoomId, Int64 CompanyID, bool IsArchived, bool IsDeleted, string NameStartWith)
        {
            return GetAllRecords(RoomId, CompanyID, IsArchived, IsDeleted).ToList().Where(c => (!string.IsNullOrWhiteSpace(c.Location)) && c.Location.ToLower().Contains(NameStartWith)).ToList();
        }


        /// <summary>
        /// Get Paged Records from the Bin Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<LocationMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<LocationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<LocationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<LocationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
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
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').Select(long.Parse).ToList().Contains(t.CreatedBy.GetValueOrDefault(0))))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').Select(long.Parse).ToList().Contains(t.LastUpdatedBy.GetValueOrDefault(0))))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                       && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.Location.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //------------------------------------------------------------------------------------------------
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0)));

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<LocationMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Location.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.Location.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

        }


        public IEnumerable<LocationMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string DBConnectionstring)
        {
            IEnumerable<LocationMasterDTO> ObjCache = null;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media

                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(DBConnectionstring))
                    {
                        IEnumerable<LocationMasterDTO> obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM LocationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                              select new LocationMasterDTO
                                                              {
                                                                  ID = u.ID,
                                                                  Location = u.Location,
                                                                  Created = u.Created,
                                                                  LastUpdated = u.LastUpdated,
                                                                  CreatedBy = u.CreatedBy,
                                                                  LastUpdatedBy = u.LastUpdatedBy,
                                                                  CreatedByName = u.CreatedByName,
                                                                  UpdatedByName = u.UpdatedByName,
                                                                  Room = u.Room,
                                                                  RoomName = u.RoomName,
                                                                  GUID = u.GUID,
                                                                  CompanyID = u.CompanyID,
                                                                  IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                  IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                  UDF1 = u.UDF1,
                                                                  UDF2 = u.UDF2,
                                                                  UDF3 = u.UDF3,
                                                                  UDF4 = u.UDF4,
                                                                  UDF5 = u.UDF5
                                                              }).AsParallel().ToList();
                        ObjCache = obj;

                    }
                }
            }



            return ObjCache.ToList().Where(t => t.Room == RoomID);
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
                LocationMaster obj = context.LocationMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;

                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.LocationMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<LocationMasterDTO> ObjCache = CacheHelper<IEnumerable<LocationMasterDTO>>.GetCacheItem("Cached_LocationMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<LocationMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<LocationMasterDTO>>.AppendToCacheItem("Cached_LocationMaster_" + obj.CompanyID.ToString(), ObjCache);
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
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE LocationMaster SET LastUpdated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<LocationMasterDTO> ObjCache = CacheHelper<IEnumerable<LocationMasterDTO>>.GetCacheItem("Cached_LocationMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<LocationMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<LocationMasterDTO>>.AppendToCacheItem("Cached_LocationMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }

        public LocationMasterDTO GetLocationsByName(string LocationName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (LocationName == null)
                {
                    LocationName = string.Empty;
                }
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@LocationName", LocationName) };
                LocationMasterDTO obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationsByName] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@LocationName", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                         select new LocationMasterDTO
                                         {
                                             ID = u.ID,
                                             Location = u.Location,
                                             Created = u.Created,
                                             LastUpdated = u.LastUpdated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u.CreatedByName,
                                             UpdatedByName = u.UpdatedByName,
                                             Room = u.Room,
                                             RoomName = u.RoomName,
                                             GUID = u.GUID,
                                             CompanyID = u.CompanyID,
                                             IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                             IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }

        public List<LocationMasterDTO> GetAllLocation(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string Location)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@Location", (Location ?? string.Empty).Trim()) };
                List<LocationMasterDTO> obj = context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationRoomData] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@Location", params1).ToList();
                return obj;
            }
        }

        public LocationMasterDTO GetLocationUsingSerial(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGUID, string SerialNumber)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@SerialNumber", SerialNumber) };

                LocationMasterDTO obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationUsingSerial] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ToolGUID,@SerialNumber", params1)
                                         select new LocationMasterDTO
                                         {
                                             ID = u.ID,
                                             Location = u.Location,
                                             Created = u.Created,
                                             LastUpdated = u.LastUpdated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u.CreatedByName,
                                             UpdatedByName = u.UpdatedByName,
                                             Room = u.Room,
                                             RoomName = u.RoomName,
                                             GUID = u.GUID,
                                             CompanyID = u.CompanyID,
                                             IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                             IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }

        public IEnumerable<LocationMasterDTO> GetLocationForRoomWise(Int64 RoomID, Int64 CompanyID, bool IsDeleted, bool IsArchived)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };

                IEnumerable<LocationMasterDTO> obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>("EXEC GetAllLocationRoomWise @CompanyID,@RoomID,@IsDeleted,@IsArchived", params1)
                                                      select new LocationMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          Location = u.Location,
                                                          Created = u.Created,
                                                          LastUpdated = u.LastUpdated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Room = u.Room,
                                                          RoomName = u.RoomName,
                                                          GUID = u.GUID,
                                                          CompanyID = u.CompanyID,
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5
                                                      }).AsParallel().ToList();
                return obj;
            }
        }

        public IEnumerable<LocationMasterDTO> GetLocationRoomData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };

                IEnumerable<LocationMasterDTO> obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationRoomData] @RoomID,@CompanyID,@IsDeleted,@IsArchived", params1)
                                                      select new LocationMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          Location = u.Location,
                                                          Created = u.Created,
                                                          LastUpdated = u.LastUpdated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          CreatedByName = u.CreatedByName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          Room = u.Room,
                                                          RoomName = u.RoomName,
                                                          GUID = u.GUID,
                                                          CompanyID = u.CompanyID,
                                                          IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                          IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5
                                                      }).AsParallel().ToList();
                return obj;
            }

        }

        public LocationMasterDTO GetLocationsByGUID(Guid LocationGuid, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@Guid", LocationGuid) };
                LocationMasterDTO obj = (from u in context.ExecuteStoreQuery<LocationMasterDTO>("exec [GetLocationsByGUID] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@Guid", params1) //A.IsDeleted!=1 AND A.IsArchived != 1 AND 
                                         select new LocationMasterDTO
                                         {
                                             ID = u.ID,
                                             Location = u.Location,
                                             Created = u.Created,
                                             LastUpdated = u.LastUpdated,
                                             CreatedBy = u.CreatedBy,
                                             LastUpdatedBy = u.LastUpdatedBy,
                                             CreatedByName = u.CreatedByName,
                                             UpdatedByName = u.UpdatedByName,
                                             Room = u.Room,
                                             RoomName = u.RoomName,
                                             GUID = u.GUID,
                                             CompanyID = u.CompanyID,
                                             IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                             IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                             UDF1 = u.UDF1,
                                             UDF2 = u.UDF2,
                                             UDF3 = u.UDF3,
                                             UDF4 = u.UDF4,
                                             UDF5 = u.UDF5
                                         }).AsParallel().FirstOrDefault();
                return obj;
            }

        }


        public LocationMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<LocationMasterDTO>(@"SELECT A.ID,A.Location, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.LastUpdated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM LocationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new LocationMasterDTO
            //            {
            //                ID = u.ID,
            //                Location = u.Location,
            //                Created = u.Created,
            //                LastUpdated = u.LastUpdated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName,
            //                LastUpdatedBy = u.LastUpdatedBy,
            //                CreatedBy = u.CreatedBy,
            //                Room = u.Room,
            //                GUID = u.GUID,
            //                CompanyID = u.CompanyID,
            //                UDF1 = u.UDF1,
            //                UDF2 = u.UDF2,
            //                UDF3 = u.UDF3,
            //                UDF4 = u.UDF4,
            //                UDF5 = u.UDF5
            //            }).SingleOrDefault();
            //}
        }

        public IEnumerable<LocationMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string DBConnectionstring)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, DBConnectionstring).OrderBy("ID DESC");

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<LocationMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM LocationMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString())
            //            select new LocationMasterDTO
            //            {
            //                ID = u.ID,
            //                Location = u.Location,
            //                Created = u.Created,
            //                LastUpdated = u.LastUpdated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName
            //            }).AsParallel().ToList();

            //}
        }
    }
}
