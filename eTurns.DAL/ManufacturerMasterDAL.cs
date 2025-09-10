using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ManufacturerMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ManufacturerMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ManufacturerMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        #region [Class Methods]
        public ManufacturerMasterDTO GetManufacturerByNameNormal(string ManufacturerName, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {
            ManufacturerName = ManufacturerName == null ? ManufacturerName : ManufacturerName.Replace("'", "''");
            var params1 = new SqlParameter[] {new SqlParameter("@RoomID", RoomID),new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@ManufacturerName", ManufacturerName ?? string.Empty)
                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec GetManufacturerByNameNormal @RoomID,@CompanyID,@IsForBom,@ManufacturerName", params1).FirstOrDefault();
            }

        }
        public List<ManufacturerMasterDTO> GetManufacturerByNameSearch(string Name, Int64 RoomID, Int64 CompanyID, int? MaxRows, bool? IsForBom)
        {

            var params1 = new SqlParameter[] {new SqlParameter("@RoomID", RoomID),new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@Name", Name ?? string.Empty)
                ,new SqlParameter("@MaxRows", MaxRows ?? -999)

                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec GetManufacturerByNameSearch @RoomID,@CompanyID,@IsForBom,@Name,@MaxRows", params1).ToList();
            }

        }
        public ManufacturerMasterDTO GetManufacturerByIDNormal(Int64? ID, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {

            var params1 = new SqlParameter[] {
                 new SqlParameter("@RoomID", RoomID)
                ,new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@ID", ID  ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec GetManufacturerByIDNormal @RoomID,@CompanyID,@IsForBom,@ID", params1).FirstOrDefault();
            }
        }
        public ManufacturerMasterDTO GetManufacturerByIDPlain(long ID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] {
                 new SqlParameter("@RoomID", RoomID)
                ,new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@ID", ID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec GetManufacturerByIDPlain @RoomID,@CompanyID,@ID", params1).FirstOrDefault();
            }
        }

        public List<ManufacturerMasterDTO> GetManufacturerByRoomNormal(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool? IsForBom, string OrderBy = null)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID)
                    , new SqlParameter("@CompanyId", CompanyId)
                    , new SqlParameter("@Isdeleted", IsDeleted)
                    , new SqlParameter("@IsArchived", IsArchived)
                    ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                    ,new SqlParameter("@OrderBy", OrderBy?? (object)DBNull.Value)

                };
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec GetManufacturerByRoomNormal @RoomId,@CompanyId,@Isdeleted,@IsArchived,@IsForBom,@OrderBy", params1).ToList();
            }
        }
        public List<ManufacturerMasterDTO> GetPagedManufacturerMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    UDF1 = FieldsPara[4].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    UDF2 = FieldsPara[5].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    UDF3 = FieldsPara[6].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    UDF4 = FieldsPara[7].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    UDF5 = FieldsPara[8].TrimEnd(',');
                }

            }
            else
            {
                SearchTerm = "";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // var supplierIds = (SupplierIds != null && SupplierIds.Any()) ? string.Join(",", SupplierIds) : string.Empty;

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

                List<ManufacturerMasterDTO> lstcats = context.Database.SqlQuery<ManufacturerMasterDTO>("exec [GetPagedManufacturerMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@IsForBom", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }
        public Int64 GetORInsertBlankManuFacID(long RoomID, long CompanyID, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ManufacturerMaster objMan = context.ManufacturerMasters.Where(t => t.Manufacturer == "" && t.IsDeleted == false && t.Room == RoomID && t.CompanyID == CompanyID).FirstOrDefault();
                if (objMan == null)
                {
                    objMan = new ManufacturerMaster();
                    objMan.AddedFrom = "web";
                    objMan.CompanyID = CompanyID;
                    objMan.Created = DateTimeUtility.DateTimeNow;
                    objMan.CreatedBy = UserID;
                    objMan.LastUpdatedBy = UserID;
                    objMan.EditedFrom = "web";
                    objMan.GUID = Guid.NewGuid();
                    objMan.ID = 0;
                    objMan.IsArchived = false;
                    objMan.IsDeleted = false;
                    objMan.isForBOM = false;
                    objMan.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objMan.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objMan.Room = RoomID;
                    objMan.Updated = DateTimeUtility.DateTimeNow;
                    objMan.Manufacturer = "";
                    context.ManufacturerMasters.Add(objMan);
                    context.SaveChanges();
                }
                return objMan.ID;
            }
        }
        public Int64 Insert(ManufacturerMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ManufacturerMaster obj = new ManufacturerMaster();
                obj.ID = 0;
                obj.Manufacturer = objDTO.Manufacturer;
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
                obj.GUID = Guid.NewGuid();
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;

                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                context.ManufacturerMasters.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;


                return obj.ID;
            }
        }
        public bool Edit(ManufacturerMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ManufacturerMaster obj = new ManufacturerMaster();
                obj.ID = objDTO.ID;
                obj.Manufacturer = objDTO.Manufacturer;

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
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    objDTO.RoomName = string.Empty;
                    obj.Room = 0;
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                if (objDTO.IsOnlyFromItemUI)
                {
                    obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                    obj.ReceivedOn = objDTO.ReceivedOn;
                }
                obj.isForBOM = objDTO.isForBOM;
                obj.RefBomId = objDTO.RefBomId;

                context.ManufacturerMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media


                return true;
            }
        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "UPDATE ManufacturerMaster SET " + columnName + " = '" + value + "', Updated = DATETIME ('" + System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))) + "'),EditedFrom='Web',ReceivedOn='" + DateTimeUtility.DateTimeNow + "' WHERE ID=" + id;
                context.Database.ExecuteSqlCommand(strQuery);
            }
            return value;
        }
        public List<NarrowSearchDTO> GetMFListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetManufacturerListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }
        public List<ManufacturerMasterDTO> GetManufacturerMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ManufacturerMasterDTO>("exec [GetManufacturerMasterChangeLog] @ID,@dbName", params1).ToList();
            }
        }
        #endregion
    }

}
