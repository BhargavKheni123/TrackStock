using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class InventoryClassificationMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public InventoryClassificationMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public InventoryClassificationMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public InventoryClassificationMasterDTO GetInventoryClassificationByGUIDPlain(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByGUIDPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public InventoryClassificationMasterDTO GetInventoryClassificationByGUIDNormal(Guid GUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", GUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByGUIDNormal] @GUID", params1).FirstOrDefault();
            }
        }

        public InventoryClassificationMasterDTO GetInventoryClassificationByIDPlain(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByIDPlain] @ID", params1).FirstOrDefault();
            }
        }

        public InventoryClassificationMasterDTO GetInventoryClassificationByIDNormal(long ID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByIDNormal] @ID", params1).FirstOrDefault();
            }
        }

        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByGUIDsNormal(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByGUIDsNormal] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByIDsNormal(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByIDsNormal] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByGUIDsPlain(string GUIDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByGUIDsPlain] @GUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByIDsPlain(string IDs, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IDs", IDs ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByIDsPlain] @IDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByRoomPlain(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByRoomPlain] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }

        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByRoomNormal(long RoomID, long CompanyID, bool IsForBom)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByRoomNormal] @RoomID,@CompanyID,@IsForBom", params1).ToList();
            }
        }


        public List<InventoryClassificationMasterDTO> GetInventoryClassificationsByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBom", IsForBom), new SqlParameter("@Name", Name ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByNamePlain] @RoomID,@CompanyID,@IsForBom,@Name", params1).ToList();
            }
        }
        public InventoryClassificationMasterDTO GetInventoryClassificationByNamePlain(long RoomID, long CompanyID, bool IsForBom, string Name)
        {
            return GetInventoryClassificationsByNamePlain(RoomID, CompanyID, IsForBom, Name).OrderByDescending(t => t.ID).FirstOrDefault();
        }

        public List<InventoryClassificationMasterDTO> GetInventoryClassificationByNameSearch(string Name, long RoomID, long CompanyID, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@Name", Name ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationByNameSearch] @Name,@RoomID,@CompanyID,@IsForBOM", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetInventoryClassificationListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetInventoryClassificationListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }


        public Int32 Insert(InventoryClassificationMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryClassificationMaster obj = new InventoryClassificationMaster();
                obj.ID = 0;
                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.BaseOfInventory = objDTO.BaseOfInventory;
                obj.RangeStart = objDTO.RangeStart;
                obj.RangeEnd = objDTO.RangeEnd;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.GUID = Guid.NewGuid();
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted; ;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;

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

                context.InventoryClassificationMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;



                return obj.ID;
            }

        }
        public bool Edit(InventoryClassificationMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                InventoryClassificationMaster obj = new InventoryClassificationMaster();
                obj.ID = objDTO.ID;
                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.BaseOfInventory = objDTO.BaseOfInventory;
                obj.RangeStart = objDTO.RangeStart;
                obj.RangeEnd = objDTO.RangeEnd;
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

                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;

                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;

                context.InventoryClassificationMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();



                return true;
            }
        }

        public List<InventoryClassificationMasterDTO> GetPagedInventoryClassificationMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
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
                    new SqlParameter("@IsForBom", IsForBom)
                };
                List<InventoryClassificationMasterDTO> lstcats = context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetPagedInventoryClassificationMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords;
                }
                return lstcats;
            }

        }
        public List<InventoryClassificationMasterDTO> GetInventoryClassificationMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<InventoryClassificationMasterDTO>("exec [GetInventoryClassificationMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }

        #endregion

    }
}


