using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class UnitMasterDAL : eTurnsBaseDAL
    {

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                MessageDTO objMSG = new MessageDTO();

                UnitMaster obj = context.UnitMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                obj.EditedFrom = "Web";
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                context.UnitMasters.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();


                //Get Cached-Media
                IEnumerable<UnitMasterDTO> ObjCache = CacheHelper<IEnumerable<UnitMasterDTO>>.GetCacheItem("Cached_UnitMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<UnitMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<UnitMasterDTO>>.AppendToCacheItem("Cached_UnitMaster_" + obj.CompanyID.ToString(), ObjCache);
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
                        strQuery += "UPDATE UnitMaster SET Updated = '" + DateTime.Now.ToString() + "' ,LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1,EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);


                //Get Cached-Media
                IEnumerable<UnitMasterDTO> ObjCache = CacheHelper<IEnumerable<UnitMasterDTO>>.GetCacheItem("Cached_UnitMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<UnitMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<UnitMasterDTO>>.AppendToCacheItem("Cached_UnitMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }




        public UnitMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.ID == id).FirstOrDefault();

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<UnitMasterDTO>("SELECT A.* , B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UnitMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new UnitMasterDTO
            //            {
            //                Unit = u.Unit,
            //                Created = u.Created,
            //                CreatedBy = u.CreatedBy,
            //                ID = u.ID,
            //                LastUpdatedBy = u.LastUpdatedBy,
            //                Room = u.Room,
            //                Updated = u.Updated,
            //                Description = u.Description,
            //                EngineModel = u.EngineModel,
            //                EngineSerialNo = u.EngineSerialNo,
            //                GUID = u.GUID,
            //                Make = u.Make,
            //                MarkupLabour = u.MarkupLabour,
            //                MarkupParts = u.MarkupParts,
            //                Model = u.Model,
            //                Odometer = u.Odometer,
            //                OdometerUpdate = u.OdometerUpdate,
            //                OpHours = u.OpHours,
            //                OpHoursUpdate = u.OpHoursUpdate,
            //                Plate = u.Plate,
            //                SerialNo = u.SerialNo,
            //                Year = u.Year,
            //                IsArchived = u.IsArchived,
            //                IsDeleted = u.IsDeleted,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                RoomName = u.RoomName,
            //                CompanyID = u.CompanyID,
            //                UDF1 = u.UDF1,
            //                UDF2 = u.UDF2,
            //                UDF3 = u.UDF3,
            //                UDF4 = u.UDF4,
            //                UDF5 = u.UDF5
            //            }).SingleOrDefault();
            //}
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool? IsForBom)
        {

            IEnumerable<UnitMasterDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<UnitMasterDTO>>.GetCacheItem("Cached_UnitMaster_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<UnitMasterDTO> obj = (from u in context.ExecuteStoreQuery<UnitMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UnitMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                          select new UnitMasterDTO
                                                          {
                                                              Unit = u.Unit,
                                                              Created = u.Created,
                                                              CreatedBy = u.CreatedBy,
                                                              ID = u.ID,
                                                              LastUpdatedBy = u.LastUpdatedBy,
                                                              Room = u.Room,
                                                              Updated = u.Updated,
                                                              Description = u.Description,
                                                              //EngineModel = u.EngineModel,
                                                              //EngineSerialNo = u.EngineSerialNo,
                                                              GUID = u.GUID,
                                                              //Make = u.Make,
                                                              //MarkupLabour = u.MarkupLabour,
                                                              //MarkupParts = u.MarkupParts,
                                                              //Model = u.Model,
                                                              //Odometer = u.Odometer,
                                                              //OdometerUpdate = u.OdometerUpdate,
                                                              //OpHours = u.OpHours,
                                                              //OpHoursUpdate = u.OpHoursUpdate,
                                                              //Plate = u.Plate,
                                                              //SerialNo = u.SerialNo,
                                                              //Year = u.Year,
                                                              IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                              IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                              CreatedByName = u.CreatedByName,
                                                              UpdatedByName = u.UpdatedByName,
                                                              RoomName = u.RoomName,
                                                              CompanyID = u.CompanyID,
                                                              isForBOM = u.isForBOM,
                                                              RefBomId = u.RefBomId,
                                                              AddedFrom = u.AddedFrom,
                                                              EditedFrom = u.EditedFrom,
                                                              ReceivedOn = u.ReceivedOn,
                                                              ReceivedOnWeb = u.ReceivedOnWeb,
                                                          }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<UnitMasterDTO>>.AddCacheItem("Cached_UnitMaster_" + CompanyID.ToString(), obj);
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
                    ObjCache = (from u in context.ExecuteStoreQuery<UnitMasterDTO>(@"SELECT A.* , B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM UnitMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new UnitMasterDTO
                                {
                                    Unit = u.Unit,
                                    Created = u.Created,
                                    CreatedBy = u.CreatedBy,
                                    ID = u.ID,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    Updated = u.Updated,
                                    Description = u.Description,
                                    //EngineModel = u.EngineModel,
                                    //EngineSerialNo = u.EngineSerialNo,
                                    //GUID = u.GUID,
                                    //Make = u.Make,
                                    //MarkupLabour = u.MarkupLabour,
                                    //MarkupParts = u.MarkupParts,
                                    //Model = u.Model,
                                    //Odometer = u.Odometer,
                                    //OdometerUpdate = u.OdometerUpdate,
                                    //OpHours = u.OpHours,
                                    //OpHoursUpdate = u.OpHoursUpdate,
                                    //Plate = u.Plate,
                                    //SerialNo = u.SerialNo,
                                    //Year = u.Year,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    CompanyID = u.CompanyID,
                                    isForBOM = u.isForBOM,
                                    RefBomId = u.RefBomId,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
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
                return ObjCache.Where(t => t.Room == RoomID && t.isForBOM == false);
            }
        }


        public IEnumerable<UnitMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            IEnumerable<UnitMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom);
            if (IsForBom)
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == true);
            }
            else
            {
                ObjCache = ObjCache.Where(t => t.isForBOM == false);
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<UnitMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<UnitMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);

                //string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                //string[] stringSeparators = new string[] { "[###]" };

                //if (dd != null && dd.Length > 0)
                //{
                //    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                //    // 6 counts for fields based on that prepare the search string
                //    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                //    foreach (var item in Fields)
                //    {
                //        if (item.Length > 0)
                //        {
                //            if (item.Contains("CreatedBy"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                //            }
                //            else if (item.Contains("UpdatedBy"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                //            }
                //            else if (item.Contains("DateCreatedFrom"))
                //            {
                //                ObjCache = ObjCache.Where(t => t.Created.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                //            }
                //            else if (item.Contains("DateUpdatedFrom"))
                //            {
                //                ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                //            }
                //            else if (item.Contains("UDF1"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF1));
                //            }
                //            else if (item.Contains("UDF2"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF2));
                //            }
                //            else if (item.Contains("UDF3"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF3));
                //            }
                //            else if (item.Contains("UDF4"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF4));
                //            }
                //            else if (item.Contains("UDF5"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF5));
                //            }
                //        }
                //    }
                //}
                //TotalCount = ObjCache.Count();
                //return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

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
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').Select(long.Parse).ToList().Contains(t.CreatedBy.GetValueOrDefault())))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').Select(long.Parse).ToList().Contains(t.LastUpdatedBy.GetValueOrDefault(0))))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"]))) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])))))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').Select(HttpUtility.UrlDecode).ToList().Contains(t.UDF5)))
                    && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        (t.Unit ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Year.HasValue ? t.Year.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.SerialNo.HasValue ? t.SerialNo.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Model ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Plate ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
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
                //IEnumerable<UnitMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.Unit ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Year.HasValue ? t.Year.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.SerialNo.HasValue ? t.SerialNo.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Model ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Plate ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.Unit ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Year.HasValue ? t.Year.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.SerialNo.HasValue ? t.SerialNo.Value.ToString() : "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Model ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        //(t.Plate ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

        }



        public IEnumerable<UnitMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsForBom).OrderBy("ID DESC");
        }

        public UnitMasterDTO GetRecord(string Unit, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom)
        {
            return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsForBom).Where(t => t.Unit.ToLower() == Unit.ToLower()).FirstOrDefault();

        }

        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE UnitMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID=" + id;
                context.ExecuteStoreCommand(strQuery);
                return value;
            }
        }

        public UnitMasterDTO GetUnitsByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom), new SqlParameter("@Name", Name ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UnitMasterDTO>("exec [GetUnitByNamePlain] @RoomID,@CompanyID,@IsForBom,@Name", params1).FirstOrDefault();
            }
        }
    }
}
