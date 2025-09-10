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
    public partial class CostUOMMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public CostUOMMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CostUOMMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]
        public List<NarrowSearchDTO> GetBOMCostUOMListListNarrowSearch(long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetBOMCostUOMListListNarrowSearch] @CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<NarrowSearchDTO> GetCostUOMListListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetCostUOMListListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetBOMCostUOMsByCompanyID(long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetBOMCostUOMsByCompanyID] @CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetBOMCostUOMsByIDs(string CostUOMIDs, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMIDs", CostUOMIDs ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetBOMCostUOMsByIDs] @CostUOMIDs,@CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetBOMCostUOMsByName(string CostUOMName, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMName", CostUOMName ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetBOMCostUOMsByName] @CostUOMName, @CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetBOMCostUOMsByNameSearch(string CostUOMName, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMName", CostUOMName ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetBOMCostUOMsByNameSearch] @CostUOMName,@CompanyID", params1).ToList();
            }
        }
        public CostUOMMasterDTO GetCostUOMByID(long CostUOMID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMID", CostUOMID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMByID] @CostUOMID", params1).FirstOrDefault();
            }
        }
        public List<CostUOMMasterDTO> GetCostUOMsByIDs(string CostUOMIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMIDs", CostUOMIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMsByIDs] @CostUOMIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetCostUOMsByName(string CostUOMName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMName", CostUOMName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMsByName] @CostUOMName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetCostUOMsByNameSearch(string CostUOMName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMName", CostUOMName ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMsByNameSearch] @CostUOMName,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<CostUOMMasterDTO> GetCostUOMsByRoomID(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMsByRoomID] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public CostUOMMasterDTO GetCostUOMByName(string CostUOMName, long RoomID, long CompanyID)
        {
            return GetCostUOMsByName(CostUOMName, RoomID, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public CostUOMMasterDTO GetBOMCostUOMByName(string CostUOMName, long CompanyID)
        {
            return GetBOMCostUOMsByName(CostUOMName, CompanyID).OrderByDescending(t => t.ID).FirstOrDefault();
        }
        public List<CostUOMMasterDTO> GetPagedCostUOMRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, bool IsForBom, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedBy = "";
            string UpdatedBy = "";
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
                    CreatedBy = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedBy = FieldsPara[1].TrimEnd(',');
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
                    new SqlParameter("@CreatedBy", CreatedBy),
                    new SqlParameter("@LastUpdatedBy", UpdatedBy),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@IsForBom", IsForBom)

                };
                List<CostUOMMasterDTO> lstcats = context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetPagedCostUOMMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@UpdatedFrom,@UpdatedTo,@CreatedBy,@LastUpdatedBy,@IsDeleted,@IsArchived,@CompanyID,@Room,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }

        }
        public Int64 Insert(CostUOMMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CostUOMMaster obj = new CostUOMMaster();
                obj.ID = 0;
                obj.CostUOM = objDTO.CostUOM;
                obj.CostUOMValue = objDTO.CostUOMValue;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.IsForBOM = objDTO.isForBOM;
                if ((objDTO.isForBOM))
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                context.CostUOMMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;


                //if (objDTO.ID > 0)
                //{
                //    //Get Cached-Media
                //    IEnumerable<CostUOMMasterDTO> ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString());
                //    if (ObjCache != null)
                //    {
                //        List<CostUOMMasterDTO> tempC = new List<CostUOMMasterDTO>();
                //        tempC.Add(objDTO);

                //        IEnumerable<CostUOMMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //        CacheHelper<IEnumerable<CostUOMMasterDTO>>.AppendToCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //    }
                //}

                return obj.ID;
            }

        }
        public bool Edit(CostUOMMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CostUOMMaster obj = new CostUOMMaster();
                obj.ID = objDTO.ID;
                obj.CostUOM = objDTO.CostUOM;
                obj.CostUOMValue = objDTO.CostUOMValue;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = objDTO.GUID;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsForBOM = objDTO.isForBOM;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.CostUOMMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.Database.CommandTimeout = 3600;
                context.SaveChanges();

                if ((objDTO.isForBOM))
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }

                //Get Cached-Media
                //IEnumerable<CostUOMMasterDTO> ObjCache = CacheHelper<IEnumerable<CostUOMMasterDTO>>.GetCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString());
                //if (ObjCache != null)
                //{
                //    List<CostUOMMasterDTO> objTemp = ObjCache.ToList();
                //    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                //    ObjCache = objTemp.AsEnumerable();

                //    List<CostUOMMasterDTO> tempC = new List<CostUOMMasterDTO>();
                //    tempC.Add(objDTO);
                //    IEnumerable<CostUOMMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                //    CacheHelper<IEnumerable<CostUOMMasterDTO>>.AppendToCacheItem("Cached_CostUOMMaster_" + objDTO.CompanyID.ToString(), NewCache);
                //}


                return true;
            }
        }
        public void InsertDefaultCostUOMByNameAndValue(string CostUOM, int CostUOMValue, Int64? CreatedUserID, Int64 RoomID, Int64? CompanyID, bool? IsForBOM = false)
        {
            CostUOMMasterDTO objDTO = new CostUOMMasterDTO();
            objDTO.ID = 0;
            objDTO.CostUOM = CostUOM;
            objDTO.CostUOMValue = CostUOMValue;
            objDTO.GUID = Guid.NewGuid();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = CreatedUserID;
            objDTO.LastUpdatedBy = CreatedUserID;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;
            objDTO.CompanyID = CompanyID;
            objDTO.Room = RoomID;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.AddedFrom = "Web";
            objDTO.EditedFrom = "Web";
            objDTO.isForBOM = IsForBOM ?? false;
            Insert(objDTO);
        }

        public Int64 CheckCostUOMwithOrder(Int64 CostUOMID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CostUOMUID", CostUOMID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("EXEC [dbo].[CheckCostUOMWithOrder] @CostUOMUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<CostUOMMasterDTO> GetCostUOMMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CostUOMMasterDTO>("exec [GetCostUOMMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion
    }
}


