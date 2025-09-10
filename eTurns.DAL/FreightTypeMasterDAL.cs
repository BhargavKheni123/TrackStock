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
    public class FreightTypeMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public FreightTypeMasterDAL(base.DataBaseName)
        //{

        //}

        public FreightTypeMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public FreightTypeMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion

        #region [Class Methods]
        public FreightTypeMasterDTO GetFreightTypeByGUID(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypeByGUID] @GUID", params1).FirstOrDefault();
            }
        }
        public FreightTypeMasterDTO GetFreightTypeByID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypeByID] @ID", params1).FirstOrDefault();
            }
        }
        public List<FreightTypeMasterDTO> GetFreightTypeByRoomID(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypeByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetFreightTypeListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetFreightTypeListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<FreightTypeMasterDTO> GetFreightTypesByGUIDs(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypesByGUIDs] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<FreightTypeMasterDTO> GetFreightTypesByIDs(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypesByIDs] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<FreightTypeMasterDTO> GetFreightTypesByName(string FreightType, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FreightType", FreightType ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypesByName] @FreightType,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<FreightTypeMasterDTO> GetFreightTypesByNameSearch(string FreightType, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@FreightType", FreightType ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<FreightTypeMasterDTO>("exec [GetFreightTypesByNameSearch] @FreightType,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public FreightTypeMasterDTO GetFreightTypeByName(string FreightType, long RoomID, long CompanyID)
        {
            return GetFreightTypesByName(FreightType, RoomID, CompanyID).OrderBy(t => t.ID).FirstOrDefault();
        }
        public IEnumerable<FreightTypeMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<FreightTypeMasterDTO> ObjCache = GetFreightTypeByRoomID(RoomID, CompanyId);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<FreightTypeMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<FreightTypeMasterDTO> ObjCache = GetCachedData(RoomID,CompanyId);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.LastUpdated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<FreightTypeMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.FreightType.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.FreightType.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
        }
        public Int64 Insert(FreightTypeMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                FreightTypeMaster obj = new FreightTypeMaster();
                obj.ID = 0;
                obj.FreightType = objDTO.FreightType;
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
                context.FreightTypeMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<FreightTypeMasterDTO> ObjCache = CacheHelper<IEnumerable<FreightTypeMasterDTO>>.GetCacheItem("Cached_FreightTypeMaster_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<FreightTypeMasterDTO> tempC = new List<FreightTypeMasterDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<FreightTypeMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<FreightTypeMasterDTO>>.AppendToCacheItem("Cached_FreightTypeMaster_" + objDTO.CompanyID.ToString(), NewCache);
                    }
                }


                return obj.ID;

            }
        }
        public bool Edit(FreightTypeMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                FreightTypeMaster obj = new FreightTypeMaster();
                obj.ID = objDTO.ID;
                obj.FreightType = objDTO.FreightType;

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

                context.FreightTypeMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<FreightTypeMasterDTO> ObjCache = CacheHelper<IEnumerable<FreightTypeMasterDTO>>.GetCacheItem("Cached_FreightTypeMaster_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<FreightTypeMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<FreightTypeMasterDTO> tempC = new List<FreightTypeMasterDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<FreightTypeMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<FreightTypeMasterDTO>>.AppendToCacheItem("Cached_FreightTypeMaster_" + objDTO.CompanyID.ToString(), NewCache);
                }

                return true;
            }

        }
        public string UpdateData(Int64 id, string value, int updateByID, string columnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strQuery = "UPDATE FreightTypeMaster SET " + columnName + " = '" + value + "', LastUpdatedBy= " + updateByID + ", LastUpdated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID= " + id + "";
                context.Database.ExecuteSqlCommand(strQuery);
                context.SaveChanges();
            }

            return value;
        }
        #endregion
    }
}
