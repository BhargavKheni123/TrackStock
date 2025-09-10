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
    public partial class GLAccountMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public GLAccountMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public GLAccountMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class methods]       
        public GLAccountMasterDTO GetGLAccountByGUID(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountByGUID] @GUID", params1).FirstOrDefault();
            }
        }

        public GLAccountMasterDTO GetGLAccountByIdPlain(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountByIdPlain] @ID", params1).FirstOrDefault();
            }
        }

        public GLAccountMasterDTO GetGLAccountByID(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountByID] @ID", params1).FirstOrDefault();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountByNameSearch(string Name, long RoomID, long CompanyID, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Name", Name ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountByNameSearch] @Name,@RoomID,@CompanyID,@IsForBOM", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetGLAccountListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetGLAccountListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountMasterByName(long? RoomID, long? CompanyID, bool? IsDeleted, bool? IsArchived, bool? IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value), new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value), new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value), new SqlParameter("@Name", Name ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountMasterByName] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@IsForBom,@Name", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountMasterChangeLog(long ID, string dbName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@dbName", dbName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountMasterUDF(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived, bool IsForBom, bool UDF1, bool UDF2, bool UDF3, bool UDF4, bool UDF5)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsForBom", IsForBom), new SqlParameter("@UDF1", UDF1), new SqlParameter("@UDF2", UDF2), new SqlParameter("@UDF3", UDF3), new SqlParameter("@UDF4", UDF4), new SqlParameter("@UDF5", UDF5) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountMasterUDF] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@IsForBom,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountsByGUIDs(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountsByGUIDs] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountsByIDs(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountsByIDs] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetGLAccountsByName(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom), new SqlParameter("@Name", Name ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountsByName] @RoomID,@CompanyID,@IsForBom,@Name", params1).ToList();
            }
        }
        public GLAccountMasterDTO GetGLAccountByName(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            return GetGLAccountsByName(RoomID, CompanyID, IsForBom, Name).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public List<GLAccountMasterDTO> GetGLAccountsByRoom(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountsByRoom] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> UpdateGLAccountMaster(string ColumnName, string Value, long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ColumnName", ColumnName ?? (object)DBNull.Value), new SqlParameter("@Value", Value ?? (object)DBNull.Value), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [UpdateGLAccountMaster] @ColumnName,@Value,@ID", params1).ToList();
            }
        }
        public List<GLAccountMasterDTO> GetPagedGLAccountMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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
            string UDF6 = "";
            string UDF7 = "";
            string UDF8 = "";
            string UDF9 = "";
            string UDF10 = "";

            if ((SearchTerm ?? string.Empty).Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    //Updated = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
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
                    new SqlParameter("@SearchTerm", SearchTerm),
                    new SqlParameter("@sortColumnName", sortColumnName),

                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),

                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@UDF6", UDF6),
                    new SqlParameter("@UDF7", UDF7),
                    new SqlParameter("@UDF8", UDF8),
                    new SqlParameter("@UDF9", UDF9),
                    new SqlParameter("@UDF10", UDF10),
                    new SqlParameter("@IsForBom", IsForBom)
                };
                List<GLAccountMasterDTO> lstcats = context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetPagedGLAccountMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@UDF6,@UDF7,@UDF8,@UDF9,@UDF10,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }
                return lstcats;
            }

        }
        public Int64 Insert(GLAccountMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                GLAccountMaster obj = new GLAccountMaster();
                obj.ID = 0;
                obj.GLAccount = objDTO.GLAccount;
                obj.Description = objDTO.Description;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
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
                context.GLAccountMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;
                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<GLAccountMasterDTO> ObjCache = CacheHelper<IEnumerable<GLAccountMasterDTO>>.GetCacheItem("Cached_GLAccountMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<GLAccountMasterDTO> tempC = new List<GLAccountMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<GLAccountMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<GLAccountMasterDTO>>.AppendToCacheItem("Cached_GLAccountMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }
        }
        public bool Edit(GLAccountMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                GLAccountMaster obj = new GLAccountMaster();
                obj.ID = objDTO.ID;
                obj.GLAccount = objDTO.GLAccount;
                obj.Description = objDTO.Description;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
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
                context.GLAccountMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media
                //IEnumerable<GLAccountMasterDTO> ObjCache = CacheHelper<IEnumerable<GLAccountMasterDTO>>.GetCacheItem("Cached_GLAccountMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<GLAccountMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<GLAccountMasterDTO> tempC = new List<GLAccountMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<GLAccountMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<GLAccountMasterDTO>>.AppendToCacheItem("Cached_GLAccountMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}

                return true;
            }
        }
        public string UpdateData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE GLAccountMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "') WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }

            return value;
        }
        public List<GLAccountMasterDTO> GetGLAccountMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<GLAccountMasterDTO>("exec [GetGLAccountMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion
    }
}
