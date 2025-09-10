using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class SupplierBlanketPOMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public SupplierBlanketPOMasterDAL(base.DataBaseName)
        //{

        //}

        public SupplierBlanketPOMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public SupplierBlanketPOMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SupplierBlanketPOMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.GetCacheItem("Cached_SupplierBlanketPOMaster_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<SupplierBlanketPOMasterDTO> obj = (from u in context.Database.SqlQuery<SupplierBlanketPOMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM SupplierBlanketPOMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString())
                                                                   select new SupplierBlanketPOMasterDTO
                                                                   {
                                                                       ID = u.ID,
                                                                       BlanketPO = u.BlanketPO,
                                                                       SupplierID = u.SupplierID,
                                                                       GUID = u.GUID,
                                                                       HighPO = u.HighPO,
                                                                       Created = u.Created,
                                                                       LastUpdated = u.LastUpdated,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       RoomName = u.RoomName,
                                                                       Room = u.Room,
                                                                       CreatedBy = u.CreatedBy,
                                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                                       CompanyID = u.CompanyID,
                                                                       IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                       IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                       UDF1 = u.UDF1,
                                                                       UDF2 = u.UDF2,
                                                                       UDF3 = u.UDF3,
                                                                       UDF4 = u.UDF4,
                                                                       UDF5 = u.UDF5
                                                                   }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.AddCacheItem("Cached_SupplierBlanketPOMaster_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == RoomID);
        }

        public IEnumerable<SupplierBlanketPOMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedData(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<SupplierBlanketPOMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
            IEnumerable<SupplierBlanketPOMasterDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("STARTWITH#"))
            {
                //Get Cached-Media
                //IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);

                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    foreach (var item in Fields)
                    {
                        if (item.Length > 0)
                        {
                            if (item.Contains("CreatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                            }
                            else if (item.Contains("UpdatedBy"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                            }
                            else if (item.Contains("DateCreatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Created.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.LastUpdated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                            else if (item.Contains("UDF1"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF1));
                            }
                            else if (item.Contains("UDF2"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF2));
                            }
                            else if (item.Contains("UDF3"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF3));
                            }
                            else if (item.Contains("UDF4"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF4));
                            }
                            else if (item.Contains("UDF5"))
                            {
                                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF5));
                            }
                        }
                    }
                }
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = GetCachedData(RoomID, CompanyId);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.BlanketPO.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.BlanketPO.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
            #region Previous Code

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    string strSortinitializer = "";

            //    if (sortColumnName.ToUpper().Contains("CREATEDBYNAME"))
            //    {
            //        strSortinitializer = "B";
            //        sortColumnName = "UserName";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("UPDATEDBYNAME"))
            //    {
            //        strSortinitializer = "C";
            //        sortColumnName = "UserName";
            //    }
            //    else if (sortColumnName.ToUpper().Contains("ROOMNAME"))
            //    {
            //        strSortinitializer = "D";
            //    }
            //    else
            //    {
            //        strSortinitializer = "A";
            //    }

            //    strSortinitializer = strSortinitializer + "." + sortColumnName;


            //    if (String.IsNullOrEmpty(SearchTerm))
            //    {
            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM TechnicianMaster WHERE IsDeleted!=1 AND  CompanyID = " + CompanyId.ToString() + @" AND  Room = " + RoomID.ToString() + @" AND IsArchived != 1").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<SupplierBlanketPOMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM TechnicianMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 ORDER BY " + strSortinitializer)
            //                select new SupplierBlanketPOMasterDTO
            //                {
            //                    ID = u.ID,
            //                    BlanketPO = u.BlanketPO,
            //                    SupplierID = u.SupplierID,
            //                    HighPO = u.HighPO,
            //                    Created = u.Created,
            //                    LastUpdated = u.LastUpdated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }

            //    else
            //    {


            //        TotalCount = Convert.ToInt32(context.Database.SqlQuery<Int32>(@"SELECT Count(ID) FROM TechnicianMaster WHERE IsDeleted!=1 AND  CompanyID = " + CompanyId.ToString() + @" AND  Room = " + RoomID.ToString() + @" AND IsArchived != 1 AND ((Technician like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);

            //        return (from u in context.Database.SqlQuery<SupplierBlanketPOMasterDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM TechnicianMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.CompanyID = " + CompanyId.ToString() + @" AND A.Room = " + RoomID.ToString() + @" AND A.IsArchived != 1 AND ((A.Technician like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
            //                select new SupplierBlanketPOMasterDTO
            //                {
            //                    ID = u.ID,
            //                    BlanketPO = u.BlanketPO,
            //                    SupplierID = u.SupplierID,
            //                    HighPO = u.HighPO,
            //                    Created = u.Created,
            //                    LastUpdated = u.LastUpdated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    RoomName = u.RoomName,
            //                    UDF1 = u.UDF1,
            //                    UDF2 = u.UDF2,
            //                    UDF3 = u.UDF3,
            //                    UDF4 = u.UDF4,
            //                    UDF5 = u.UDF5
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }

            //} 
            #endregion
        }

        public SupplierBlanketPOMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(RoomID, CompanyID).Single(t => t.ID == id);

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.Database.SqlQuery<SupplierBlanketPOMasterDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, A.GUID FROM TechnicianMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.ID=" + id.ToString())
            //            select new SupplierBlanketPOMasterDTO
            //            {
            //                ID = u.ID,
            //                BlanketPO = u.BlanketPO,
            //                SupplierID = u.SupplierID,
            //                IsArchived = u.IsArchived,
            //                GUID = u.GUID,
            //                HighPO = u.HighPO,
            //                Created = u.Created,
            //                LastUpdated = u.LastUpdated,
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

        public Int64 Insert(SupplierBlanketPOMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPOMaster obj = new SupplierBlanketPOMaster();
                obj.ID = 0;
                obj.BlanketPO = objDTO.BlanketPO;
                obj.SupplierID = objDTO.SupplierID;
                obj.HighPO = objDTO.HighPO;
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
                context.SupplierBlanketPOMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                if (objDTO.ID > 0)
                {
                    //Get Cached-Media
                    IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.GetCacheItem("Cached_SupplierBlanketPOMaster_" + objDTO.CompanyID.ToString());
                    if (ObjCache != null)
                    {
                        List<SupplierBlanketPOMasterDTO> tempC = new List<SupplierBlanketPOMasterDTO>();
                        tempC.Add(objDTO);

                        IEnumerable<SupplierBlanketPOMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                        CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.AppendToCacheItem("Cached_SupplierBlanketPOMaster_" + objDTO.CompanyID.ToString(), NewCache);
                    }
                }

                return obj.ID;
            }
        }

        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPOMaster obj = context.SupplierBlanketPOMasters.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.SupplierBlanketPOMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.GetCacheItem("Cached_SupplierBlanketPOMaster_" + obj.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<SupplierBlanketPOMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.AppendToCacheItem("Cached_SupplierBlanketPOMaster_" + obj.CompanyID.ToString(), ObjCache);
                }

                return true;
            }
        }

        public bool Edit(SupplierBlanketPOMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                SupplierBlanketPOMaster obj = new SupplierBlanketPOMaster();
                obj.ID = objDTO.ID;
                obj.BlanketPO = objDTO.BlanketPO;
                obj.SupplierID = objDTO.SupplierID;
                obj.HighPO = objDTO.HighPO;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                context.SupplierBlanketPOMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.GetCacheItem("Cached_SupplierBlanketPOMaster_" + objDTO.CompanyID.ToString());
                if (ObjCache != null)
                {
                    List<SupplierBlanketPOMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                    ObjCache = objTemp.AsEnumerable();

                    List<SupplierBlanketPOMasterDTO> tempC = new List<SupplierBlanketPOMasterDTO>();
                    tempC.Add(objDTO);
                    IEnumerable<SupplierBlanketPOMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                    CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.AppendToCacheItem("Cached_SupplierBlanketPOMaster_" + objDTO.CompanyID.ToString(), NewCache);
                }

                return true;
            }
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
                        strQuery += "UPDATE SupplierBlanketPOMaster SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.Database.ExecuteSqlCommand(strQuery);

                //Get Cached-Media
                IEnumerable<SupplierBlanketPOMasterDTO> ObjCache = CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.GetCacheItem("Cached_SupplierBlanketPOMaster_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<SupplierBlanketPOMasterDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<SupplierBlanketPOMasterDTO>>.AppendToCacheItem("Cached_SupplierBlanketPOMaster_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }


        }

        public string UpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE SupplierBlanketPOMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;

        }


    }
}
