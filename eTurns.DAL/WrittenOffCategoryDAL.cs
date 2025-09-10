using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public partial class WrittenOffCategoryDAL : eTurnsBaseDAL
    {
        public WrittenOffCategoryDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public List<WrittenOfCategoryDTO> GetPagedWrittenOffCategoryMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsForBom, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string CreatedByName = "";
            string UpdatedByName = "";
            string CreatedDateFrom = "";
            string CreatedDateTo = "";
            string UpdatedDateFrom = "";
            string UpdatedDateTo = "";
            //string UDF1 = "";
            //string UDF2 = "";
            //string UDF3 = "";
            //string UDF4 = "";
            //string UDF5 = "";

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

                //if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                //{
                //    UDF1 = FieldsPara[4].TrimEnd(',');
                //}
                //if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                //{
                //    UDF2 = FieldsPara[5].TrimEnd(',');
                //}
                //if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                //{
                //    UDF3 = FieldsPara[6].TrimEnd(',');
                //}
                //if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                //{
                //    UDF4 = FieldsPara[7].TrimEnd(',');
                //}
                //if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                //{
                //    UDF5 = FieldsPara[8].TrimEnd(',');
                //}

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
                    //new SqlParameter("@UDF1", UDF1),
                    //new SqlParameter("@UDF2", UDF2),
                    //new SqlParameter("@UDF3", UDF3),
                    //new SqlParameter("@UDF4", UDF4),
                    //new SqlParameter("@UDF5", UDF5),
                    new SqlParameter("@IsForBom", IsForBom)
                };

                List<WrittenOfCategoryDTO> lstcats = context.Database.SqlQuery<WrittenOfCategoryDTO>("exec [GetPagedWrittenOfCategoryMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsArchived,@IsDeleted,@CompanyID,@IsForBom", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }

        public List<NarrowSearchDTO> GetWrittenOffCategoryListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, bool IsForBOM)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value), new SqlParameter("@IsForBOM", IsForBOM) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetWrittenOffCategoryListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@IsForBOM", params1).ToList();
            }
        }

        public void InsertDefaultEntry(string nameofcategory,long? createdby,long? roomid,long? companyid)
        {
            WrittenOfCategoryDTO dto = new WrittenOfCategoryDTO();
            dto.GUID=new Guid();
            dto.WrittenOffCategory = nameofcategory;
            dto.CreatedBy = createdby;
            dto.LastUpdatedBy = createdby;
            dto.Room = roomid;
            dto.CompanyID = companyid;
            dto.ReceivedOn = DateTimeUtility.DateTimeNow;
            dto.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            Insert(dto);
        }
        public Int64 Insert(WrittenOfCategoryDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;

            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolWrittenOffCategory obj = new ToolWrittenOffCategory();
                obj.ID = 0;
                obj.WrittenOffCategory = objDTO.WrittenOffCategory;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = Guid.NewGuid();
                obj.AddedFrom = (objDTO.AddedFrom == null ? "Web" : objDTO.AddedFrom);
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;

                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    obj.Room = 0;
                }
                context.ToolWrittenOffCategories.Add(obj);
                context.SaveChanges();

                objDTO.ID = obj.ID;


                return obj.ID;
            }
        }

        public WrittenOfCategoryDTO GetWrittenOffCategoryByNameNormal(string WrittenOffCategoryName, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {
            WrittenOffCategoryName = WrittenOffCategoryName == null ? WrittenOffCategoryName : WrittenOffCategoryName.Replace("'", "''");
            var params1 = new SqlParameter[] {new SqlParameter("@RoomID", RoomID),new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@WrittenOffCategoryName", WrittenOffCategoryName ?? string.Empty)
                };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WrittenOfCategoryDTO>("exec GetWrittenOffCategoryByNameNormal @RoomID,@CompanyID,@IsForBom,@WrittenOffCategoryName", params1).FirstOrDefault();
            }

        }

        public bool Edit(WrittenOfCategoryDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ToolWrittenOffCategory obj = new ToolWrittenOffCategory();
                obj.ID = objDTO.ID;
                obj.WrittenOffCategory = objDTO.WrittenOffCategory;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.Updated = objDTO.Updated;
                obj.Created = objDTO.Created;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                obj.GUID = objDTO.GUID.Value;
                //obj.UDF1 = objDTO.UDF1;
                //obj.UDF2 = objDTO.UDF2;
                //obj.UDF3 = objDTO.UDF3;
                //obj.UDF4 = objDTO.UDF4;
                //obj.UDF5 = objDTO.UDF5;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    obj.Room = 0;
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                //if (objDTO.IsOnlyFromItemUI)
                //{
                //    obj.EditedFrom = (objDTO.EditedFrom == null ? "Web" : objDTO.EditedFrom);
                //}
                //obj.isForBOM = objDTO.isForBOM;
                //obj.RefBomId = objDTO.RefBomId;

                context.ToolWrittenOffCategories.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //Get Cached-Media


                return true;
            }
        }

        public WrittenOfCategoryDTO GetWrittenOffCategoryByIDNormal(Int64? ID, Int64 RoomID, Int64 CompanyID, bool? IsForBom)
        {

            var params1 = new SqlParameter[] {
                 new SqlParameter("@RoomID", RoomID)
                ,new SqlParameter("@CompanyID", CompanyID)
                ,new SqlParameter("@IsForBom", IsForBom ?? (object)DBNull.Value)
                ,new SqlParameter("@ID", ID  ?? (object)DBNull.Value) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WrittenOfCategoryDTO>("exec GetWrittenOffCategoryByIDNormal @RoomID,@CompanyID,@IsForBom,@ID", params1).FirstOrDefault();
            }
        }

        public List<WrittenOfCategoryDTO> WrittenOffCategoryHistoryChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs), new SqlParameter("@dbName", DataBaseName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<WrittenOfCategoryDTO>("exec [GetWrittenOffCategoryChangeLog] @ID,@dbName", params1).ToList();
            }
        }


    }
}
