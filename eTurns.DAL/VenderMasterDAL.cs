using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class VenderMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public VenderMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public VenderMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        #region [Class Methods]

        public VenderMasterDTO GetVenderByNameNormal(string VenderName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderName", VenderName), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByNameNormal] @VenderName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public VenderMasterDTO GetVenderByNamePlain(string VenderName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderName", VenderName), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByNamePlain] @VenderName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public VenderMasterDTO GetVenderByGuidNormal(Guid VenderGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderGUID", VenderGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByGuidNormal] @VenderGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public VenderMasterDTO GetVenderByGuidPlain(Guid VenderGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderGUID", VenderGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByGuidPlain] @VenderGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public VenderMasterDTO GetVenderByIDNormal(long VenderID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderID", VenderID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByIDNormal] @VenderID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public VenderMasterDTO GetVenderByIDPlain(long VenderID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderID", VenderID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByIDPlain] @VenderID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<VenderMasterDTO> GetVenderByRoomIDNormal(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByRoomIDNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<VenderMasterDTO> GetVenderByRoomIDPlain(long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderByRoomIDPlain] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<VenderMasterDTO> GetVenderListSearch(long RoomID, long CompanyID, string SearchKey)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@VenderName", SearchKey ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderListSearch] @VenderName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<VenderMasterDTO> GetPagedVenderMaster(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
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
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),

                    new SqlParameter("@CreatedFrom", CreatedDateFrom),
                    new SqlParameter("@CreatedTo", CreatedDateTo),
                    new SqlParameter("@CreatedBy", CreatedByName),
                    new SqlParameter("@UpdatedFrom", UpdatedDateFrom),
                    new SqlParameter("@UpdatedTo", UpdatedDateTo),
                    new SqlParameter("@LastUpdatedBy", UpdatedByName),

                    new SqlParameter("@Room", RoomID),
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@CompanyID", CompanyID),

                    new SqlParameter("@UDF1", UDF1),
                    new SqlParameter("@UDF2", UDF2),
                    new SqlParameter("@UDF3", UDF3),
                    new SqlParameter("@UDF4", UDF4),
                    new SqlParameter("@UDF5", UDF5),

                };
                List<VenderMasterDTO> lstcats = context.Database.SqlQuery<VenderMasterDTO>("exec [GetPagedVenderMaster] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@CreatedFrom,@CreatedTo,@CreatedBy,@UpdatedFrom,@UpdatedTo,@LastUpdatedBy,@Room,@IsDeleted,@IsArchived,@CompanyID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1).ToList();
                TotalCount = 0;
                if (lstcats != null && lstcats.Count > 0)
                {
                    TotalCount = lstcats.First().TotalRecords;
                }

                return lstcats;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase VenderMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(VenderMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                VenderMaster obj = new VenderMaster();
                obj.ID = 0;
                obj.Vender = objDTO.Vender;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                context.VenderMasters.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(VenderMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                VenderMaster obj = new VenderMaster();
                obj.ID = objDTO.ID;
                obj.Vender = objDTO.Vender;
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = objDTO.GUID;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                context.VenderMasters.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public bool DeleteVenderMasterRecords(string IDs, long UserId)
        {
            if (!string.IsNullOrWhiteSpace(IDs))
            {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string vIDs = (IDs ?? string.Empty).TrimEnd(',');
                    if (!string.IsNullOrWhiteSpace(vIDs))
                    {
                        var params1 = new SqlParameter[] {
                                                        new SqlParameter("@UserID", UserId),
                                                        new SqlParameter("@VenderIDs", IDs)
                                                        };

                        string strQuery = @"EXEC DeleteVenderMasterByIDs @UserID,@VenderIDs";
                        context.Database.ExecuteSqlCommand(strQuery, params1);

                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// This method is used to get the data for the narrow search for the Vendor list
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="NarrowSearchKey"></param>
        /// <returns></returns>
        public List<NarrowSearchDTO> GetVendorListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) };
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetVendorListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<VenderMasterDTO> GetVenderMasterChangeLog(string IDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<VenderMasterDTO>("exec [GetVenderMasterChangeLog] @ID", params1).ToList();
            }
        }
        #endregion

    }
}


