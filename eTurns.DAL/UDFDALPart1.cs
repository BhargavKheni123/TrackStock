using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class UDFDAL : eTurnsBaseDAL
    {
        public UDFDTO GetUDFByUDFColumnNamePlain(string UDFColumnName, string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == "BOMItemMaster")
            {
                RoomID = 0;
            }
            else if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }
            var params1 = new SqlParameter[] { new SqlParameter("@UDFColumnName", UDFColumnName ?? (object)DBNull.Value),
                                               new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<UDFDTO>("exec [GetUDFsByUDFColumnNamePlain] @UDFColumnName,@UDFTableName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public IEnumerable<UDFDTO> GetPagedRecordseTurns(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, string UDFTableName)
        {

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<UDFDTO> ObjCache = GetCachedDataeTurns().Where(t => t.UDFTableName == UDFTableName && (t.Room ?? 0) == 0);
                IEnumerable<UDFDTO> ObjCache = GetUDFsByUDFTableNameNormal(UDFTableName, 0, 0);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetUDFsByUDFTableNameNormal(UDFTableName, 0, 0);

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
                                ObjCache = ObjCache.Where(t => t.Created.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                            }
                        }
                    }
                }
                TotalCount = ObjCache.Where(t => t.UDFTableName == UDFTableName).Count();
                return ObjCache.Where(t => t.UDFTableName == UDFTableName).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                IEnumerable<UDFDTO> ObjCache = GetUDFsByUDFTableNameNormal(UDFTableName, 0, 0);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).Count();
                return ObjCache.Where(t => t.UDFTableName == UDFTableName).Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }


        }

        public IEnumerable<UDFDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyID, string UDFTableName, Int64 RoomID)
        {
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<UDFDTO> ObjCache = GetCachedData(CompanyID).Where(t => t.UDFTableName.ToLower() == UDFTableName.ToLower() && (t.Room ?? 0) == RoomID);
                IEnumerable<UDFDTO> ObjCache = GetDataCompanyTableNameWise(CompanyID, UDFTableName, RoomID);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetDataRoomWise(CompanyID, RoomID);//GetCachedData(CompanyID).Where(t => (t.Room ?? 0) == RoomID);

                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[0].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    //foreach (var item in Fields)
                    //{
                    //    if (item.Length > 0)
                    //    {
                    //        if (item.Contains("CreatedBy"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                    //        }
                    //        else if (item.Contains("UpdatedBy"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                    //        }
                    //        else if (item.Contains("DateCreatedFrom"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => t.Created.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                    //        }
                    //        else if (item.Contains("DateUpdatedFrom"))
                    //        {
                    //            ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                    //        }
                    //    }
                    //}
                    if (Fields != null && Fields.Length > 2)
                    {
                        string SerachString = Fields[2];
                        ObjCache = ObjCache.Where(t => (t.CreatedByName != null && t.CreatedByName.ToString().ToLower().Trim().Contains(SerachString.ToLower().Trim()))
                                                || (t.UpdatedByName != null && t.UpdatedByName.ToString().ToLower().Trim().Contains(SerachString.ToLower().Trim()))
                                                || (t.CreatedDate.ToString().ToLower().Trim().Contains(SerachString.ToLower().Trim()))
                                                || (t.UpdatedDate != null && t.UpdatedDate.ToString().ToLower().Trim().Contains(SerachString.ToLower().Trim())));
                    }
                }
                TotalCount = ObjCache.Where(t => t.UDFTableName.ToLower() == UDFTableName.ToLower()).Count();
                return ObjCache.Where(t => t.UDFTableName.ToLower() == UDFTableName.ToLower()).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                IEnumerable<UDFDTO> ObjCache = GetDataRoomWise(CompanyID, RoomID);//GetCachedData(CompanyID).Where(t => (t.Room ?? 0) == RoomID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).Count();
                return ObjCache.Where(t => t.UDFTableName.ToLower() == UDFTableName.ToLower()).Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        t.UDFColumnName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

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
            //    else
            //    {
            //        strSortinitializer = "A";
            //    }

            //    strSortinitializer = strSortinitializer + "." + sortColumnName;


            //    if (String.IsNullOrEmpty(SearchTerm))
            //    {
            //        //TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UDF WHERE IsDeleted!=1 AND UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " ").ToList()[0]);
            //        TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UDF WHERE UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " ").ToList()[0]);

            //        //return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.UDFTableName='" + UDFTableName + "' AND A.CompanyID=" + CompanyID + " ORDER BY " + strSortinitializer)
            //        return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName='" + UDFTableName + "' AND A.CompanyID=" + CompanyID + " ORDER BY " + strSortinitializer)
            //                select new UDFDTO
            //                {
            //                    ID = u.ID,
            //                    CompanyID = u.CompanyID,
            //                    UDFTableName = u.UDFTableName,
            //                    UDFColumnName = u.UDFColumnName,
            //                    UDFControlType = u.UDFControlType,
            //                    UDFDefaultValue = u.UDFDefaultValue,
            //                    UDFOptionsCSV = u.UDFOptionsCSV,
            //                    UDFIsRequired = u.UDFIsRequired,
            //                    UDFIsSearchable = u.UDFIsSearchable,
            //                    Created = u.Created,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    IsDeleted = u.IsDeleted
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //    else
            //    {
            //        //TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UDF WHERE IsDeleted!=1 AND UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " AND ((UDFColumnName like '%" + SearchTerm + "%') OR (UDFControlType like '%" + SearchTerm + "%') OR (UDFDefaultValue like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);
            //        TotalCount = Convert.ToInt32(context.ExecuteStoreQuery<Int32>(@"SELECT Count(ID) FROM UDF WHERE UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " AND ((UDFColumnName like '%" + SearchTerm + "%') OR (UDFControlType like '%" + SearchTerm + "%') OR (UDFDefaultValue like '%" + SearchTerm + "%') OR (ID like '%" + SearchTerm + "%'))").ToList()[0]);

            //        //return (from u in context.ExecuteStoreQuery<UDFDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.IsDeleted!=1 AND A.UDFTableName= '" + UDFTableName + "' AND ((A.UDFColumnName like '%" + SearchTerm + "%') OR (A.UDFControlType like '%" + SearchTerm + "%') OR (A.UDFDefaultValue like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
            //        return (from u in context.ExecuteStoreQuery<UDFDTO>("SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName= '" + UDFTableName + "' AND ((A.UDFColumnName like '%" + SearchTerm + "%') OR (A.UDFControlType like '%" + SearchTerm + "%') OR (A.UDFDefaultValue like '%" + SearchTerm + "%') OR (A.ID like '%" + SearchTerm + "%')) ORDER BY " + strSortinitializer)
            //                select new UDFDTO
            //                {
            //                    ID = u.ID,
            //                    CompanyID = u.CompanyID,
            //                    UDFTableName = u.UDFTableName,
            //                    UDFColumnName = u.UDFColumnName,
            //                    UDFControlType = u.UDFControlType,
            //                    UDFDefaultValue = u.UDFDefaultValue,
            //                    UDFOptionsCSV = u.UDFOptionsCSV,
            //                    UDFIsRequired = u.UDFIsRequired,
            //                    UDFIsSearchable = u.UDFIsSearchable,
            //                    Created = u.Created,
            //                    Updated = u.Updated,
            //                    CreatedByName = u.CreatedByName,
            //                    UpdatedByName = u.UpdatedByName,
            //                    IsDeleted = u.IsDeleted
            //                }).AsParallel().Skip(StartRowIndex).Take(MaxRows).ToList();
            //    }
            //}
        }

        public IEnumerable<UDFDTO> GetDataRoomWise(Int64 CompanyID, Int64 RoomID)
        {
            IEnumerable<UDFDTO> ObjCache = null;
            if (CompanyID > 0 && RoomID > 0)
            {
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + " and isnull(A.Room,0) =" + RoomID.ToString())
                                    select new UDFDTO
                                    {
                                        ID = u.ID,
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        UDFTableName = u.UDFTableName,
                                        UDFColumnName = u.UDFColumnName,
                                        UDFControlType = u.UDFControlType,
                                        UDFDefaultValue = u.UDFDefaultValue,
                                        UDFOptionsCSV = u.UDFOptionsCSV,
                                        UDFIsRequired = u.UDFIsRequired,
                                        UDFIsSearchable = u.UDFIsSearchable,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        IsDeleted = u.IsDeleted,
                                        UDFDisplayColumnName = u.UDFDisplayColumnName,
                                        UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                        IsEncryption = u.IsEncryption ?? false,
                                        UDFMaxLength = u.UDFMaxLength
                                    }).AsParallel().ToList();

                    }
                }
            }
            else
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + " and isnull(A.Room,0) =" + RoomID.ToString())
                                select new UDFDTO
                                {
                                    ID = u.ID,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    UDFTableName = u.UDFTableName,
                                    UDFColumnName = u.UDFColumnName,
                                    UDFControlType = u.UDFControlType,
                                    UDFDefaultValue = u.UDFDefaultValue,
                                    UDFOptionsCSV = u.UDFOptionsCSV,
                                    UDFIsRequired = u.UDFIsRequired,
                                    UDFIsSearchable = u.UDFIsSearchable,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    IsDeleted = u.IsDeleted,
                                    IsEncryption = u.IsEncryption ?? false,
                                    UDFMaxLength = u.UDFMaxLength
                                }).AsParallel().ToList();
                }
            }

            return ObjCache;
        }

        public UDFDTO GetDataIDWise(Int64 CompanyID, Int64 ID)
        {
            UDFDTO ObjCache = null;
            if (CompanyID > 0 && ID > 0)
            {
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + " and A.Id =" + ID.ToString())
                                    select new UDFDTO
                                    {
                                        ID = u.ID,
                                        CompanyID = u.CompanyID,
                                        Room = u.Room,
                                        UDFTableName = u.UDFTableName,
                                        UDFColumnName = u.UDFColumnName,
                                        UDFControlType = u.UDFControlType,
                                        UDFDefaultValue = u.UDFDefaultValue,
                                        UDFOptionsCSV = u.UDFOptionsCSV,
                                        UDFIsRequired = u.UDFIsRequired,
                                        UDFIsSearchable = u.UDFIsSearchable,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        IsDeleted = u.IsDeleted,
                                        UDFDisplayColumnName = u.UDFDisplayColumnName,
                                        UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                        IsEncryption = u.IsEncryption ?? false,
                                        UDFMaxLength = u.UDFMaxLength
                                    }).FirstOrDefault();

                    }
                }
            }
            else
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + " and A.ID =" + ID.ToString())
                                select new UDFDTO
                                {
                                    ID = u.ID,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    UDFTableName = u.UDFTableName,
                                    UDFColumnName = u.UDFColumnName,
                                    UDFControlType = u.UDFControlType,
                                    UDFDefaultValue = u.UDFDefaultValue,
                                    UDFOptionsCSV = u.UDFOptionsCSV,
                                    UDFIsRequired = u.UDFIsRequired,
                                    UDFIsSearchable = u.UDFIsSearchable,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    IsDeleted = u.IsDeleted,
                                    IsEncryption = u.IsEncryption ?? false,
                                    UDFMaxLength = u.UDFMaxLength
                                }).FirstOrDefault();
                }
            }

            return ObjCache;
        }

        public List<string> getReportResourceFileName(string PageResourceFile)
        {
            try
            {
                List<string> ReportResourceFileName = null;
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ReportResourceFileName = (context.ResourceFileLinks.Where(r => r.PageResourceFile == PageResourceFile).Select(r => r.ReportResourceFile).ToList());
                    return ReportResourceFileName;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> getPageResourceFile(string ReportResourceFile)
        {
            try
            {
                List<string> PageResourceFile = null;
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    PageResourceFile = context.ResourceFileLinks.Where(r => r.ReportResourceFile == ReportResourceFile).Select(p => p.PageResourceFile).ToList();
                    return PageResourceFile;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<UDFDTO> GetAllUDfData(Int64 roomid)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<UDFDTO> obj = (from u in context.ExecuteStoreQuery<UDFDTO>(@"select * from udf where UDFControlType is not null and Room != 0 and CompanyID != 0 and room =" + roomid)
                                               select new UDFDTO
                                               {
                                                   ID = u.ID,
                                                   CompanyID = u.CompanyID,
                                                   Room = u.Room,
                                                   UDFTableName = u.UDFTableName,
                                                   UDFColumnName = u.UDFColumnName,
                                                   UDFControlType = u.UDFControlType,
                                                   UDFDefaultValue = u.UDFDefaultValue,
                                                   UDFOptionsCSV = u.UDFOptionsCSV,
                                                   UDFIsRequired = u.UDFIsRequired,
                                                   UDFIsSearchable = u.UDFIsSearchable,
                                                   Created = u.Created,
                                                   Updated = u.Updated,
                                                   CreatedBy = u.CreatedBy,
                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                   CreatedByName = u.CreatedByName,
                                                   UpdatedByName = u.UpdatedByName,
                                                   IsDeleted = u.IsDeleted,
                                                   UDFDisplayColumnName = u.UDFDisplayColumnName,
                                                   UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                                   IsEncryption = u.IsEncryption ?? false,
                                                   UDFMaxLength = u.UDFMaxLength
                                               }).AsParallel().ToList();
                    return obj;

                }
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }

        public IEnumerable<UDFDTO> GetAllRecords(Int64 CompanyID, string UDFTableName, Int64 RoomID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;
            //   return GetCachedData(CompanyID).Where(t => (t.UDFTableName ?? string.Empty).Trim().ToLower() == (UDFTableName ?? string.Empty).Trim().ToLower() && (t.Room ?? 0) == RoomID).OrderBy("UDFColumnName ASC");
            return GetDataCompanyTableNameWise(CompanyID, UDFTableName, RoomID).OrderBy("UDFColumnName ASC");

            //using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            //{
            //    return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 'UpdatedByName' FROM UDF A LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName = '" + UDFTableName + "' AND A.CompanyID = " + CompanyID)
            //            select new UDFDTO
            //            {
            //                ID = u.ID,
            //                CompanyID = u.CompanyID,
            //                UDFTableName = u.UDFTableName,
            //                UDFColumnName = u.UDFColumnName,
            //                UDFControlType = u.UDFControlType,
            //                UDFDefaultValue = u.UDFDefaultValue,
            //                UDFOptionsCSV = u.UDFOptionsCSV,
            //                UDFIsRequired = u.UDFIsRequired,
            //                UDFIsSearchable = u.UDFIsSearchable,
            //                Created = u.Created,
            //                Updated = u.Updated,
            //                CreatedByName = u.CreatedByName,
            //                UpdatedByName = u.UpdatedByName,
            //                IsDeleted = u.IsDeleted
            //            }).ToList();
            //}
        }

        public IEnumerable<UDFDTO> GetAllRecordsNew(Int64 CompanyID, string UDFTableName, Int64 RoomID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@TableName", UDFTableName) };
                var ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>("exec [GetAllUDFRecords] @RoomID,@CompanyID,@TableName", params1)
                                select new UDFDTO
                                {
                                    ID = u.ID,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    UDFTableName = u.UDFTableName,
                                    UDFColumnName = u.UDFColumnName,
                                    UDFControlType = u.UDFControlType,
                                    UDFDefaultValue = u.UDFDefaultValue,
                                    UDFOptionsCSV = u.UDFOptionsCSV,
                                    UDFIsRequired = u.UDFIsRequired,
                                    UDFIsSearchable = u.UDFIsSearchable,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    IsDeleted = u.IsDeleted,
                                    UDFDisplayColumnName = u.UDFDisplayColumnName,
                                    UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                    IsEncryption = u.IsEncryption ?? false,
                                    UDFMaxLength = u.UDFMaxLength
                                }).AsParallel().ToList();
                return ObjCache;
            }


        }

        public IEnumerable<UDFDTO> GetAllUDFRecordsWithoutReqCheck(Int64 CompanyID, string UDFTableName, Int64 RoomID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@TableName", UDFTableName) };
                var ObjCache = (from u in context.ExecuteStoreQuery<UDFDTO>("exec [GetAllUDFRecordsWithoutReqCheck] @RoomID,@CompanyID,@TableName", params1)
                                select new UDFDTO
                                {
                                    ID = u.ID,
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    UDFTableName = u.UDFTableName,
                                    UDFColumnName = u.UDFColumnName,
                                    UDFControlType = u.UDFControlType,
                                    UDFDefaultValue = u.UDFDefaultValue,
                                    UDFOptionsCSV = u.UDFOptionsCSV,
                                    UDFIsRequired = u.UDFIsRequired,
                                    UDFIsSearchable = u.UDFIsSearchable,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    IsDeleted = u.IsDeleted,
                                    UDFDisplayColumnName = u.UDFDisplayColumnName,
                                    UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                    IsEncryption = u.IsEncryption ?? false,
                                    UDFMaxLength = u.UDFMaxLength
                                }).AsParallel().ToList();
                return ObjCache;
            }


        }

        public IEnumerable<UDFDTO> GetAllPORecords(Int64 CompanyID, Int64 RoomID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<UDFDTO> obj = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName 
            FROM udf A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
            WHERE A.CompanyID = " + CompanyID.ToString() + @" and A.Room=" + RoomID.ToString() + @" and A.UDFTableName='PullPoMaster'")
                                           select new UDFDTO
                                           {
                                               ID = u.ID,
                                               CompanyID = u.CompanyID,
                                               Room = u.Room,
                                               UDFColumnName = u.UDFColumnName,
                                               UDFDefaultValue = string.Empty,
                                               UDFControlType = u.UDFControlType,
                                               IsEncryption = u.IsEncryption ?? false,
                                               UDFMaxLength = u.UDFMaxLength
                                           }).AsParallel().ToList();
                return obj;
            }

        }

        public UDFDTO GetPullPORecord(Int64 CompanyID, Int64 RoomID, Int64 UserId, int iUDFMaxLength = 200)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFDTO obj = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 
                            UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" and A.Udftablename = 'PullPoMaster' and A.Room=" + RoomID.ToString())
                              select new UDFDTO
                              {
                                  ID = u.ID,
                                  CompanyID = u.CompanyID,
                                  Room = u.Room,
                                  UDFTableName = u.UDFTableName,
                                  UDFColumnName = u.UDFColumnName,
                                  UDFControlType = u.UDFControlType,
                                  UDFDefaultValue = u.UDFDefaultValue,
                                  UDFOptionsCSV = u.UDFOptionsCSV,
                                  UDFIsRequired = u.UDFIsRequired,
                                  UDFIsSearchable = u.UDFIsSearchable,
                                  Created = u.Created,
                                  Updated = u.Updated,
                                  CreatedBy = u.CreatedBy,
                                  LastUpdatedBy = u.LastUpdatedBy,
                                  CreatedByName = u.CreatedByName,
                                  UpdatedByName = u.UpdatedByName,
                                  IsDeleted = u.IsDeleted,
                                  UDFDisplayColumnName = u.UDFDisplayColumnName,
                                  UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                                  IsEncryption = u.IsEncryption ?? false,
                                  UDFMaxLength = u.UDFMaxLength
                              }).FirstOrDefault();
                if (obj == null)
                {
                    UDFDTO objUdfdto = new UDFDTO();
                    objUdfdto.CompanyID = CompanyID;
                    objUdfdto.Created = DateTimeUtility.DateTimeNow;
                    objUdfdto.CreatedBy = UserId;
                    objUdfdto.GUID = Guid.NewGuid();
                    objUdfdto.IsArchived = false;
                    objUdfdto.IsDeleted = false;
                    objUdfdto.LastUpdatedBy = UserId;
                    objUdfdto.Room = RoomID;
                    objUdfdto.UDFColumnName = "UDF1";
                    objUdfdto.UDFControlType = "Dropdown Editable";
                    objUdfdto.UDFDefaultValue = string.Empty;
                    objUdfdto.UDFIsRequired = false;
                    objUdfdto.UDFIsSearchable = false;
                    objUdfdto.UDFTableName = "PullPoMaster";
                    objUdfdto.Updated = DateTimeUtility.DateTimeNow;
                    objUdfdto.IsEncryption = false;
                    objUdfdto.UDFMaxLength = iUDFMaxLength;
                    return UDFInsert(objUdfdto);

                }
                return obj;
            }

        }

        public UDFDTO UDFInsert(UDFDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = 0;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.UDFMaxLength = objDTO.UDFMaxLength;
                obj.GUID = Guid.NewGuid();
                obj.Room = objDTO.Room;
                context.UDFs.AddObject(obj);
                context.SaveChanges();
                return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS 
                            UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                            LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.CompanyID = " + objDTO.CompanyID.ToString() + @" and A.Udftablename ='PullPoMaster' and A.Room=" + objDTO.Room.ToString())
                        select new UDFDTO
                        {
                            ID = u.ID,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            UDFTableName = u.UDFTableName,
                            UDFColumnName = u.UDFColumnName,
                            UDFControlType = u.UDFControlType,
                            UDFDefaultValue = u.UDFDefaultValue,
                            UDFOptionsCSV = u.UDFOptionsCSV,
                            UDFIsRequired = u.UDFIsRequired,
                            UDFIsSearchable = u.UDFIsSearchable,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            IsDeleted = u.IsDeleted,
                            UDFDisplayColumnName = u.UDFDisplayColumnName,
                            UDFPDADisplayColumnName = u.UDFPDADisplayColumnName,
                            IsEncryption = u.IsEncryption ?? false,
                            UDFMaxLength = u.UDFMaxLength
                        }).FirstOrDefault();
            }
        }

        public IEnumerable<UDFDTO> GetCurrentRoomAllUDF(long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.UDFs
                        where u.CompanyID == CompanyID && u.Room == RoomID && !string.IsNullOrEmpty(u.UDFTableName) && !string.IsNullOrEmpty(u.UDFControlType) && u.IsDeleted == false
                        select new UDFDTO
                        {
                            UDFTableName = u.UDFTableName
                        }).ToList();
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
                        //Update UDF to be set as Deleted
                        strQuery += "UPDATE UDF SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + "; ";
                        //Update UDFOptions to be set as Deleted
                        strQuery += "UPDATE UDFOptions SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE UDFID =" + item.ToString() + "; ";
                    }
                }
                context.ExecuteStoreCommand(strQuery);
                return true;
            }
        }

        public IEnumerable<UDFDTO> GetAllCompanyUDFRecords(Int64 CompanyID, string UDFTableName)
        {
            //return GetCachedData(CompanyID).Where(t => t.UDFTableName == UDFTableName).OrderBy("UDFColumnName ASC");
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<UDFDTO> obj = (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName = '" + UDFTableName + "' AND A.CompanyID = " + CompanyID.ToString() + " AND ISNULL(A.Room,0)=0")
                                           select new UDFDTO
                                           {
                                               ID = u.ID,
                                               CompanyID = u.CompanyID,
                                               Room = u.Room,
                                               UDFTableName = u.UDFTableName,
                                               UDFColumnName = u.UDFColumnName,
                                               UDFControlType = u.UDFControlType,
                                               UDFDefaultValue = u.UDFDefaultValue,
                                               UDFOptionsCSV = u.UDFOptionsCSV,
                                               UDFIsRequired = u.UDFIsRequired,
                                               UDFIsSearchable = u.UDFIsSearchable,
                                               Created = u.Created,
                                               Updated = u.Updated,
                                               CreatedBy = u.CreatedBy,
                                               LastUpdatedBy = u.LastUpdatedBy,
                                               CreatedByName = u.CreatedByName,
                                               UpdatedByName = u.UpdatedByName,
                                               IsDeleted = u.IsDeleted,
                                               IsEncryption = u.IsEncryption ?? false,
                                               UDFMaxLength = u.UDFMaxLength
                                           }).AsParallel().ToList();
                return obj.OrderBy("UDFColumnName ASC");
            }
        }

        public IEnumerable<UDFDTO> GetModulesUDF(string UDFTableName, Int64 RoomID, Int64 CompanyID)
        {


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<UDFDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName FROM UDF A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID WHERE A.UDFTableName='" + UDFTableName + "' AND A.Room=" + RoomID + " AND A.CompanyID = " + CompanyID.ToString())
                        select new UDFDTO
                        {
                            ID = u.ID,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            UDFTableName = u.UDFTableName,
                            UDFColumnName = u.UDFColumnName,
                            UDFControlType = u.UDFControlType,
                            UDFDefaultValue = u.UDFDefaultValue,
                            UDFOptionsCSV = u.UDFOptionsCSV,
                            UDFIsRequired = u.UDFIsRequired,
                            UDFIsSearchable = u.UDFIsSearchable,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            IsDeleted = u.IsDeleted,
                            IsEncryption = u.IsEncryption ?? false,
                            UDFMaxLength = u.UDFMaxLength
                        }).AsParallel().ToList();
            }



        }

        public UDFModule GetUDFModule(string name)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.UDFModules.FirstOrDefault(t => t.UDFModuleName == name || t.UDFModuleKeyName == name);
            }
        }

        public List<string> GetMissingUDFSetup(Int64 RoomID, Int64 CompanyID, string UDFTableName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<string> lstSetUDF = (from U in context.UDFs
                                          where U.CompanyID == CompanyID && U.Room == RoomID && U.UDFTableName.Trim().ToUpper() == UDFTableName.Trim().ToUpper()
                                                && (U.IsDeleted == false)
                                          select U.UDFColumnName).ToList();

                if (lstSetUDF != null)
                {
                    List<string> lstMissingUDFs = (new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" }).Where(x => !lstSetUDF.Contains(x)).ToList();
                    return lstMissingUDFs;
                }
                else
                {
                    List<string> lstMissingUDFs = new List<string>() { "UDF1", "UDF2", "UDF3", "UDF4", "UDF5" };
                    return lstMissingUDFs;
                }
            }
        }

        public int GetEnterpriseUDFSetUpColumnCount(string udffTableName, long companyId, long roomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.UDFs.Where(u => u.CompanyID == companyId && u.Room == roomId && u.UDFTableName == udffTableName && u.IsDeleted == false && !string.IsNullOrEmpty(u.UDFControlType)).ToList().Count();
            }
        }

        public bool InsertDefaultUDFs(string UDFTableName, Int64 CompanyID, Int64 UserID, Int64 RoomID, Int64 ResourcePageId = 0, int iUDFMaxLength = 200)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName.ToLower().Contains("bom"))
                    RoomID = 0;
                if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                    CompanyID = 0;
                string sSQL = "";
                string DateNow = DateTimeUtility.DateTimeNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))); //System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String))));
                string UDFColumnName;
                int noofudf = 5;
                UDFModule objUDFModule = GetUDFModule(UDFTableName);

                if (objUDFModule != null)
                {
                    if (objUDFModule.NoOfUdfs > 0)
                    {
                        noofudf = objUDFModule.NoOfUdfs;
                    }
                }

                string isIncludeInNarrowSearch = "1";
                if (UDFTableName.ToLower() == "orderdetails")
                {
                    isIncludeInNarrowSearch = "0";
                }

                for (int i = 1; i <= noofudf; i++)
                {
                    UDFColumnName = "UDF" + i.ToString();
                    sSQL += " IF NOT EXISTS (SELECT ID FROM " + CommonDAL.GeteTurnsDatabase() + ".dbo.UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND CompanyID=0 AND ISNULL(Room,0)=0) ";
                    sSQL += " Begin ";
                    sSQL += "     INSERT INTO " + CommonDAL.GeteTurnsDatabase() + ".dbo.UDF (CompanyID,UDFTableName, UDFColumnName, created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID,Room,UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired,UDFIsSearchable,IsEncryption,UDFMaxLength) ";
                    // sSQL += "     VALUES(" + CompanyID + ",'" + UDFTableName + "','" + UDFColumnName + "', '" + DateNow + "', '" + DateNow + "', " + UserID + ", " + UserID + ",0,'" + Guid.NewGuid() + "'," + RoomID + ")";
                    sSQL += " VALUES (0,'" + UDFTableName + "', 'UDF" + i + "', '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",0,'" + Guid.NewGuid() + "',0,null,null,null,0," + isIncludeInNarrowSearch + ",0," + iUDFMaxLength + " )";
                    sSQL += " End ";
                }
                var iResult = context.ExecuteStoreCommand(sSQL);

                sSQL = string.Empty;
                for (int i = 1; i <= noofudf; i++)
                {

                    UDFColumnName = "UDF" + i.ToString();
                    sSQL += " IF NOT EXISTS (SELECT ID FROM UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND CompanyID=0 AND ISNULL(Room,0)=0) ";
                    sSQL += " Begin ";
                    sSQL += "     INSERT INTO UDF (CompanyID,UDFTableName, UDFColumnName, created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID,Room,UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired,UDFIsSearchable,UDFMaxLength) ";
                    // sSQL += "     VALUES(" + CompanyID + ",'" + UDFTableName + "','" + UDFColumnName + "', '" + DateNow + "', '" + DateNow + "', " + UserID + ", " + UserID + ",0,'" + Guid.NewGuid() + "'," + RoomID + ")";
                    sSQL += " VALUES (0,'" + UDFTableName + "', 'UDF" + i + "', '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",0,'" + Guid.NewGuid() + "',0,null,null,null,0," + isIncludeInNarrowSearch + "," + iUDFMaxLength + " )";
                    sSQL += " End ";
                }
                iResult = context.ExecuteStoreCommand(sSQL);
                sSQL = string.Empty;
                // 5 - so the code will check from UDF1 to UDF5, if NOT exists will create them
                if (UDFTableName.Trim().ToLower() != "usermaster")
                {
                    sSQL += " IF NOT EXISTS (SELECT ID FROM UDF WHERE UDFColumnName='UDF1' AND UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " AND ISNULL(Room,0)=" + RoomID + ") ";
                    sSQL += " Begin ";
                    sSQL += "     INSERT INTO mobileresources (ResourcePageID,ResourceKey,ResourceValue, LanguageID,CompanyID,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn,Roomid)";
                    sSQL += "     select " + ResourcePageId + ",ResourceKey,ResourceValue,LanguageID," + CompanyID + "," + UserID + "," + UserID + ", '" + DateNow + "', '" + DateNow + "'," + RoomID + " from mobileresources where companyid =0 and roomid=0 and ResourcePageId=" + ResourcePageId;
                    sSQL += " End ";
                    for (int i = 1; i <= noofudf; i++)
                    {
                        UDFColumnName = "UDF" + i.ToString();
                        sSQL += " IF NOT EXISTS (SELECT ID FROM UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND CompanyID=" + CompanyID + " AND ISNULL(Room,0)=" + RoomID + ") ";
                        sSQL += " Begin ";
                        sSQL += "     INSERT INTO UDF (CompanyID,UDFTableName, UDFColumnName, created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID,Room,UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired,UDFIsSearchable,UDFMaxLength) ";
                        // sSQL += "     VALUES(" + CompanyID + ",'" + UDFTableName + "','" + UDFColumnName + "', '" + DateNow + "', '" + DateNow + "', " + UserID + ", " + UserID + ",0,'" + Guid.NewGuid() + "'," + RoomID + ")";
                        sSQL += "SELECT " + CompanyID + ",'" + UDFTableName + "', UDFColumnName, '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",IsDeleted,'" + Guid.NewGuid() + "'," + RoomID + ",UDFControlType,UDFOptionsCSV,UDFDefaultValue,UDFIsRequired," + isIncludeInNarrowSearch + "," + iUDFMaxLength + " FROM UDF where companyid =0 and room=0 and udfTableName='" + UDFTableName + "' and UDFColumnName='" + UDFColumnName + "'";
                        sSQL += "     INSERT INTO UDFoptions (UDFID,UDFOption,created, Updated,CreatedBy,LastUpdatedBy,IsDeleted,GUID)";
                        sSQL += "     select SCOPE_IDENTITY(), UDFOption, '" + DateNow + "', '" + DateNow + "'," + UserID + "," + UserID + ",IsDeleted,newid() from UDFoptions where udfid =(SELECT ID FROM UDF WHERE UDFColumnName='" + UDFColumnName + "' AND UDFTableName='" + UDFTableName + "' AND CompanyID=0 AND ISNULL(Room,0)=0)";

                        sSQL += " End ";
                    }


                    iResult = context.ExecuteStoreCommand(sSQL);
                }

                return true;
            }
        }
    }
}
