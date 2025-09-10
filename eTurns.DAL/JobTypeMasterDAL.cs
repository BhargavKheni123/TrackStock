using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class JobTypeMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public JobTypeMasterDAL(base.DataBaseName)
        //{

        //}

        public JobTypeMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public JobTypeMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion
        #region [Class Methods]
        public List<NarrowSearchDTO> GetJobTypeListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetJobTypeListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public JobTypeMasterDTO GetJobTypeByIDNormal(Int64 ID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ID", ID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<JobTypeMasterDTO>("exec [GetJobTypeByIDNormal] @RoomID,@CompanyID,@ID", params1).FirstOrDefault();
            }
        }
        public List<JobTypeMasterDTO> GetJobTypeByRoomNormal(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<JobTypeMasterDTO>("exec [GetJobTypeByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public int GetJobTypeByJobTypeByIDNormal(long RoomID, long CompanyID, Int64 ID, string JobType)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ID", ID), new SqlParameter("@JobType", JobType) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<Int32>("exec [GetJobTypeByJobTypeByIDNormal] @RoomID,@CompanyID,@ID,@JobType", params1).FirstOrDefault();
            }
        }

        public List<JobTypeMasterDTO> GetPagedJobTypeMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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
                    new SqlParameter("@LastUpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@LastUpdatedTo", UpdatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),
                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5)
                };

                List<JobTypeMasterDTO> lstcats = context.Database.SqlQuery<JobTypeMasterDTO>("exec [GetPagedJobTypeMaster] @StartRowIndex ,@MaxRows ,@SearchTerm ,@sortColumnName,@CreatedFrom ,@CreatedTo ,@LastUpdatedFrom ,@LastUpdatedTo ,@CreatedBy ,@LastUpdatedBy ,@Room ,@IsDeleted ,@IsArchived ,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1).ToList();
                TotalCount = 0;

                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords ?? 0;
                }

                return lstcats;
            }
        }

        public Int64 Insert(JobTypeMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                JobTypeMaster obj = new JobTypeMaster();
                obj.ID = 0;
                obj.JobType = objDTO.JobType;
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
                context.JobTypeMasters.Add(obj);
                context.SaveChanges();
                return obj.ID;
            }
        }

        public bool Edit(JobTypeMasterDTO objDTO)
        {
            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                JobTypeMaster obj = new JobTypeMaster();
                obj.ID = objDTO.ID;
                obj.JobType = objDTO.JobType;

                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.LastUpdated = objDTO.LastUpdated;
                obj.Created = objDTO.Created;
                //obj.CreatedBy = objDTO.CreatedBy;
                obj.GUID = objDTO.GUID;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;

                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;

                context.JobTypeMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public string UpdateJobTypeMaster(Int64 id, string value, int updateByID, string columnName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                          new SqlParameter("@LastUpdatedBy", updateByID)
                         ,new SqlParameter("@ID", id)
                         ,new SqlParameter("@ColumnName", columnName)
                         ,new SqlParameter("@Value",value)
                         };
                string strQuery = @"EXEC UpdateJobTypeMaster @LastUpdatedBy,@ID,@ColumnName,@Value";
                context.Database.ExecuteSqlCommand(strQuery, params1);
            }
            return value;
        }

        public string DuplicateCheck(string Name, string ActionMode, Int64 ID)
        {
            string Msg = "duplicate";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string WhereCond = "";

                if (ActionMode == "add")
                    WhereCond = " JobType = '" + Name + "'";
                else
                    WhereCond = " JobType = '" + Name + "' and ID = " + ID + "";

                //var data = context.ChkDuplicate("JobTypeMaster", "ID", WhereCond);
                var params1 = new SqlParameter[] { new SqlParameter("@TableName", "JobTypeMaster"), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                var data = (from u in context.Database.SqlQuery<Nullable<global::System.Int32>>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 


                foreach (var item in data)
                {
                    if (item.Value == 0 && ActionMode == "add")
                        Msg = "ok";
                    else if (item.Value == 0 && ActionMode == "edit")
                    {
                        WhereCond = " JobType = '" + Name + "'";
                        //data = context.ChkDuplicate("JobTypeMaster", "ID", WhereCond);
                        params1 = new SqlParameter[] { new SqlParameter("@TableName", "JobTypeMaster"), new SqlParameter("@PkID", "ID"), new SqlParameter("@WhereCond", WhereCond) };
                        data = (from u in context.Database.SqlQuery<Nullable<global::System.Int32>>("exec [ChkDuplicate] @TableName,@PkID,@WhereCond", params1) select u).ToList(); //A.IsDeleted!=1 AND A.IsArchived != 1 AND 

                        foreach (var item1 in data)
                        {
                            if (item1.Value == 0)
                                Msg = "ok";
                        }
                    }
                    else
                    {
                        if (ActionMode == "edit" && item.Value == 1)
                            Msg = "ok";
                    }
                }

            }
            return Msg;

        }
        #endregion
    }
}
