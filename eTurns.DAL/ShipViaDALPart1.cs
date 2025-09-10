using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data.Objects;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class ShipViaDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ShipViaDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ShipViaDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = null;// CacheHelper<IEnumerable<ShipViaDTO>>.GetCacheItem("Cached_ShipVia_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<ShipViaDTO> obj = (from u in context.ExecuteStoreQuery<ShipViaDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ShipViaMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                       select new ShipViaDTO
                                                       {
                                                           ID = u.ID,
                                                           ShipVia = u.ShipVia,
                                                           Created = u.Created,
                                                           Updated = u.Updated,
                                                           CreatedByName = u.CreatedByName,
                                                           UpdatedByName = u.UpdatedByName,
                                                           RoomName = u.RoomName,
                                                           Room = u.Room,
                                                           CreatedBy = u.CreatedBy,
                                                           LastUpdatedBy = u.LastUpdatedBy,
                                                           GUID = u.GUID,
                                                           CompanyID = u.CompanyID,
                                                           IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                           IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                           UDF1 = u.UDF1,
                                                           UDF2 = u.UDF2,
                                                           UDF3 = u.UDF3,
                                                           UDF4 = u.UDF4,
                                                           UDF5 = u.UDF5,
                                                           ReceivedOn = u.ReceivedOn,
                                                           ReceivedOnWeb = u.ReceivedOnWeb,
                                                           AddedFrom = u.AddedFrom,
                                                           EditedFrom = u.EditedFrom,
                                                       }).AsParallel().ToList();
                        ObjCache = obj;// CacheHelper<IEnumerable<ShipViaDTO>>.AddCacheItem("Cached_ShipVia_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<ShipViaDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ShipViaMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new ShipViaDTO
                                {
                                    ID = u.ID,
                                    ShipVia = u.ShipVia,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    Room = u.Room,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                }).AsParallel().ToList();
                }

            }



            return ObjCache.Where(t => t.Room == RoomID);
        }

        public IEnumerable<ShipViaDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted).OrderBy("ID DESC");

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<ShipViaDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ShipViaMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString())
            //            select new ShipViaDTO
            //            {
            //                ID = u.ID,
            //                ShipVia = u.ShipVia,
            //                Created = u.Created,
            //                Updated = u.Updated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName,
            //                Room = u.Room
            //            }).AsParallel().ToList();

            //}
        }

        public IEnumerable<ShipViaDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ShipViaDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<ShipViaDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<ShipViaDTO> ObjCache = GetCachedData(RoomID, CompanyId);
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
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        t.ShipVia.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                //IEnumerable<ShipViaDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ShipVia.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.ShipVia.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

        }


        public ShipViaDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).SingleOrDefault(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<ShipViaDTO>(@"SELECT A.ID,A.ShipVia, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM ShipViaMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new ShipViaDTO
            //            {
            //                ID = u.ID,
            //                ShipVia = u.ShipVia,
            //                Created = u.Created,
            //                Updated = u.Updated,
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

        public ShipViaDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).SingleOrDefault(t => t.ID == id);
            ShipViaDTO dto = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            dto = GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            if (dto == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    dto = (from u in context.ExecuteStoreQuery<ShipViaDTO>(@"SELECT A.ID,A.ShipVia, A.Created, A.CreatedBy,A.Room, A.LastUpdatedBy, A.Updated,A.UDF1,A.UDF2,A.UDF3,A.UDF4,A.UDF5,A.CompanyID, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM ShipViaMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ID=" + id.ToString())
                           select new ShipViaDTO
                           {
                               ID = u.ID,
                               ShipVia = u.ShipVia,
                               Created = u.Created,
                               Updated = u.Updated,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               Room = u.Room,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               GUID = u.GUID,
                               CompanyID = u.CompanyID,
                               IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                               IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                           }).SingleOrDefault();
                }
            }

            return dto;
        }

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ShipViaMaster obj = new ShipViaMaster();
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.ShipViaMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<ShipViaDTO> ObjCache = CacheHelper<IEnumerable<ShipViaDTO>>.GetCacheItem("Cached_ShipVia_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<ShipViaDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ShipViaDTO>>.AppendToCacheItem("Cached_ShipVia_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE ShipViaMaster SET " + columnName + " = '" + value + "', ReceivedOn=GETUTCDATE(),EditedFrom='Web', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
            }
            return value;



        }

        public List<ShipViaDTO> getShipViasByName(string Name, long RoomID, long CompanyID)
        {
            List<ShipViaDTO> lstShipVia = new List<ShipViaDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstShipVia = (from u in context.ShipViaMasters
                              where u.Room == RoomID && u.CompanyID == CompanyID && u.ShipVia == Name && u.IsDeleted == false
                              select new ShipViaDTO()
                              {
                                  ID = u.ID,
                                  ShipVia = u.ShipVia,
                                  Created = u.Created,
                                  Updated = u.Updated,
                                  Room = u.Room,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  GUID = u.GUID,
                                  CompanyID = u.CompanyID,
                                  UDF1 = u.UDF1,
                                  UDF2 = u.UDF2,
                                  UDF3 = u.UDF3,
                                  UDF4 = u.UDF4,
                                  UDF5 = u.UDF5,
                                  ReceivedOn = u.ReceivedOn,
                                  ReceivedOnWeb = u.ReceivedOnWeb,
                                  AddedFrom = u.AddedFrom,
                                  EditedFrom = u.EditedFrom,
                              }).ToList();
            }
            return lstShipVia;


        }



        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE ShipViaMaster SET Updated = '" + DateTime.Now.ToString() + "' ,LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<ShipViaDTO> ObjCache = CacheHelper<IEnumerable<ShipViaDTO>>.GetCacheItem("Cached_ShipVia_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<ShipViaDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<ShipViaDTO>>.AppendToCacheItem("Cached_ShipVia_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }

    }
}
