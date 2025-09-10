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
    public partial class CategoryMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public CategoryMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CategoryMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public CategoryMasterDTO GetCategoryByIdPlain(long CategoryId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryID", CategoryId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryByIdPlain] @CategoryID", params1).FirstOrDefault();
            }
        }

        public CategoryMasterDTO GetCategoryByCatID(long CategoryID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryID", CategoryID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryByCatID] @CategoryID", params1).FirstOrDefault();
            }
        }
        public CategoryMasterDTO GetCategoryByID(long CategoryID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryID", CategoryID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryByID] @CategoryID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public CategoryMasterDTO GetBOMCategoryByID(long CategoryID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryID", CategoryID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetBOMCategoryByID] @CategoryID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<CategoryMasterDTO> GetPagedCategoryMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64? Room, Int64? CompanyID, Boolean? IsArchived, Boolean? IsDeleted, string RoomDateFormat, bool IsForBom, TimeZoneInfo CurrentTimeZone)
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
            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (Fields[2] != null)
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
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
            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@Room", Room),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@IsForBom", IsForBom)
                };

                List<CategoryMasterDTO> lstcats = context.Database.SqlQuery<CategoryMasterDTO>("exec [GetPagedCategoryMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@UpdatedFrom,@UpdatedTo,@CreatedBy,@LastUpdatedBy,@Room,@IsDeleted,@IsArchived,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }
        public List<CategoryMasterDTO> GetCategoriesByRoomID(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoriesByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CategoryMasterDTO> GetCategoriesByCompanyBOM(long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoriesByCompanyBOM] @CompanyID", params1).ToList();
            }
        }
        public List<CategoryMasterDTO> GetCategoryByNameByRoomID(string CategoryName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryName", CategoryName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryByNameByRoomID] @CategoryName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public CategoryMasterDTO GetSingleCategoryByNameByRoomID(string CategoryName, long RoomID, long CompanyID)
        {
            return GetCategoryByNameByRoomID(CategoryName, RoomID, CompanyID).OrderBy(t => t.Created).FirstOrDefault();
        }
        public List<CategoryMasterDTO> GetCategoryByNameByCompanyIDBOM(string CategoryName, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryName", CategoryName ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryByNameByCompanyIDBOM] @CategoryName,@CompanyID", params1).ToList();
            }
        }
        public CategoryMasterDTO GetSingleCategoryByNameByCompanyIDBOM(string CategoryName, long CompanyID)
        {
            return GetCategoryByNameByCompanyIDBOM(CategoryName, CompanyID).OrderBy(t => t.ID).FirstOrDefault();
        }
        public Int64 Insert(CategoryMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CategoryMaster obj = new CategoryMaster();
                obj.ID = 0;
                obj.Category = objDTO.Category;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CategoryColor = objDTO.CategoryColor;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.CategoryMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<CategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<CategoryMasterDTO> tempC = new List<CategoryMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<CategoryMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<CategoryMasterDTO>>.AppendToCacheItem("Cached_CategoryMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }
        }
        public bool Edit(CategoryMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CategoryMaster obj = new CategoryMaster();
                obj.ID = objDTO.ID;
                obj.Category = objDTO.Category;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CategoryColor = objDTO.CategoryColor;

                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.CategoryMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<CategoryMasterDTO> ObjCache = CacheHelper<IEnumerable<CategoryMasterDTO>>.GetCacheItem("Cached_CategoryMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<CategoryMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<CategoryMasterDTO> tempC = new List<CategoryMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<CategoryMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<CategoryMasterDTO>>.AppendToCacheItem("Cached_CategoryMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}

                return true;
            }
        }
        public List<NarrowSearchDTO> GetCategoryListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetCategoryListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetBOMCategoryListNarrowSearch(long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetBOMCategoryListNarrowSearch] @CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<CategoryMasterDTO> GetCategoryListSearch(long RoomID, long CompanyID, string SearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryName", SearchKey ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryListSearch] @CategoryName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CategoryMasterDTO> GetBOMCategoryListSearch(long CompanyID, string SearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CategoryName", SearchKey ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetBOMCategoryListSearch] @CategoryName,@CompanyID", params1).ToList();
            }
        }

        public List<CategoryMasterDTO> GetCategoriesByIdsByRoomID(long RoomID, long CompanyID, string csvCatIds)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@csvCatIds", csvCatIds ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoriesByIdsByRoomID] @csvCatIds,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CategoryMasterDTO> GetCategoryMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CategoryMasterDTO>("exec [GetCategoryMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion

    }
}
