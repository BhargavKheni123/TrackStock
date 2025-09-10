using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class AssetCategoryMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public AssetCategoryMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public AssetCategoryMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]        
        public Int64 Insert(AssetCategoryMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AssetCategoryMaster obj = new AssetCategoryMaster();
                obj.ID = 0;
                obj.GUID = Guid.NewGuid();
                obj.AssetCategory = objDTO.AssetCategory;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                context.AssetCategoryMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;

            }
        }
        public bool Edit(AssetCategoryMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                AssetCategoryMaster obj = new AssetCategoryMaster();
                obj.ID = objDTO.ID;
                obj.AssetCategory = objDTO.AssetCategory;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;

                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
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
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = "Web";
                    obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                context.AssetCategoryMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }

        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strDt = DateTimeUtility.DateTimeNow.ToString("MM'/'dd'/'yyyy HH':'mm':'ss");
                string strQuery = "UPDATE AssetCategoryMaster SET " + columnName + " = '" + value + "', Updated = GETUTCDATE(),EditedFrom='Web',ReceivedOn='" + strDt + "' WHERE ID= '" + id + "'";
                context.Database.ExecuteSqlCommand(strQuery);
                context.SaveChanges();
            }

            return value;
        }
        public AssetCategoryMasterDTO GetAssetCategoryByIdOrGUID(long? ID, Guid? GUID, bool IsDeleted, bool IsArchived)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if ((ID ?? 0) > 0)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ID", ID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryByID] @ID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
                }
                else if ((GUID ?? Guid.Empty) != Guid.Empty)
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived) };
                    return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryByGUID] @GUID,@IsDeleted,@IsArchived", params1).FirstOrDefault();
                }

                return null;
            }
        }
        public AssetCategoryMasterDTO GetAssetCategoryByName(string AssetCatName, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@Name", AssetCatName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryByName] @Name,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<AssetCategoryMasterDTO> GetAssetCategoryByRoom(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }
        //public List<AssetCategoryMasterDTO> GetAssetCategoryByCompany(long CompanyID)
        //{
        //    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
        //        return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryByRoomID] @CompanyID", params1).ToList();
        //    }
        //}
        public List<AssetCategoryMasterDTO> GetPagedAssetCategory(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string Creaters = string.Empty;
            string Updators = string.Empty;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;


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
                    Creaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    Updators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
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

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex), new SqlParameter("@MaxRows", MaxRows), new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value), new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value), new SqlParameter("@Creaters", Creaters ?? (object)DBNull.Value), new SqlParameter("@Updators", Updators ?? (object)DBNull.Value), new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@CreatedDateTo", CreatedDateTo ?? (object)DBNull.Value), new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? (object)DBNull.Value), new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? (object)DBNull.Value), new SqlParameter("@UDF1", UDF1 ?? (object)DBNull.Value), new SqlParameter("@UDF2", UDF2 ?? (object)DBNull.Value), new SqlParameter("@UDF3", UDF3 ?? (object)DBNull.Value), new SqlParameter("@UDF4", UDF4 ?? (object)DBNull.Value), new SqlParameter("@UDF5", UDF5 ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyId", CompanyId) };
                List<AssetCategoryMasterDTO> lstcats = context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetPagedAssetCategory] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsDeleted,@IsArchived,@RoomID,@CompanyId", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords;
                }

                return lstcats;
            }

        }


        public List<NarrowSearchDTO> GetAssetCategoryListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetAssetCategoryListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<AssetCategoryMasterDTO> GetAssetCategoryMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", Convert.ToInt32(IDs)), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<AssetCategoryMasterDTO>("exec [GetAssetCategoryMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion
    }
}
