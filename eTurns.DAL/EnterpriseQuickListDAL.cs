using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class EnterpriseQuickListDAL: eTurnsBaseDAL
    {
        public EnterpriseQuickListDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        /// <summary>
        /// This method is used to the Enterprise QuickList .
        /// </summary>
        /// <param name="transferGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool InsertEnterpriseQuickList(long UserId,long EnterpriseId,string EnterpriseQLName,string RoomIds, DataTable EnterpriseQLDetailTbl)
        {
            bool result = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "InsertEnterpriseQL", UserId, EnterpriseId, EnterpriseQLName, RoomIds, EnterpriseQLDetailTbl);

                if (dsBins.Tables.Count > 0)
                { 
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (dt.Columns.Contains("ReturnValue"))
                            {
                                long ReturnValue = 0;
                                if (long.TryParse(Convert.ToString(dt.Rows[0]["ReturnValue"]),out ReturnValue));                                
                                result = ReturnValue > 0;                                 
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        public bool EditEnterpriseQuickList(Guid Guid,long UserId,string EnterpriseQLName,string RoomIds, DataTable EnterpriseQLDetailTbl)
        {
            bool result = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "EditEnterpriseQuickList", Guid, EnterpriseQLName, UserId, RoomIds, EnterpriseQLDetailTbl);

                if (dsBins.Tables.Count > 0)
                { 
                    if (dsBins != null && dsBins.Tables.Count > 0)
                    {
                        DataTable dt = dsBins.Tables[0];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (dt.Columns.Contains("ReturnValue"))
                            {
                                long ReturnValue = 0;
                                long.TryParse(Convert.ToString(dt.Rows[0]["ReturnValue"]), out ReturnValue);
                                result = ReturnValue > 0;                                 
                            }
                        }
                    }
                }
            }

            return result;
        }

        public EnterpriseQLDTO GetEnterpriseQuickListByGuidNormal(Guid Guid)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@Guid", Guid)                                                
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseQLDTO>("exec GetEnterpriseQuickListByGuidNormal @Guid ", params1).FirstOrDefault();
            }
        }

        public List<EnterpriseQLDetailDTO> GetEnterpriseQLDetailByQLMGuidPlain(Guid QuickListMasterGuid)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@QLMasterGUID", QuickListMasterGuid)                                                
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<EnterpriseQLDetailDTO>("exec GetEnterpriseQLDetailByQLMGuidPlain @QLMasterGUID ", params1).ToList();
            }
        }

        public IEnumerable<EnterpriseQLDTO> GetPagedEnterpriseQuickList(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName,
            bool IsDeleted,bool IsArchived, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            string Creaters = string.Empty;
            string Updators = string.Empty;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;

            if (!string.IsNullOrWhiteSpace(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                //SearchTerm = string.Empty;
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
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@StartRowIndex", StartRowIndex),
                    new SqlParameter("@MaxRows", MaxRows),
                    new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                    new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),                    
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@IsArchived", IsArchived),
                    new SqlParameter("@Creaters", Creaters ?? (object)DBNull.Value), 
                    new SqlParameter("@Updators", Updators ?? (object)DBNull.Value), 
                    new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? (object)DBNull.Value), 
                    new SqlParameter("@CreatedDateTo", CreatedDateTo ?? (object)DBNull.Value), 
                    new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? (object)DBNull.Value), 
                    new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? (object)DBNull.Value)
                };

                List<EnterpriseQLDTO> enterpriseQLs = context.Database.SqlQuery<EnterpriseQLDTO>("exec [GetPagedEnterpriseQuickList] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName," +
                    "@IsDeleted,@IsArchived,@Creaters,@Updators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo", sqlParams).ToList();

                TotalCount = 0;

                if (enterpriseQLs != null && enterpriseQLs.Any() && enterpriseQLs.Count > 0)
                {
                    TotalCount = enterpriseQLs.First().TotalRecords;
                }

                return enterpriseQLs;
            }

        }
        
        public bool IsEnterpriseQLDuplicate(long Id, string EnterpriseQLName)
        { 
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ID", Id),
                                                new SqlParameter("@Name", EnterpriseQLName) 
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec CheckEnterpriseQLDuplication @ID,@Name ", params1).FirstOrDefault();
            }
        }

        public List<RPT_EnterpriseQL> GetEnterpriseQLForReportRangeData(long[]  RoomIDs, bool IsRunWithReportConnection)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (IsRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            var tmpRoomIds = string.Empty;
            if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
            { 
                tmpRoomIds = string.Join(",",RoomIDs);
            }

            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomIDs", tmpRoomIds)                                                
                                            };

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                return context.Database.SqlQuery<RPT_EnterpriseQL>("exec RPT_GetEnterpriseQLFor_RangeData @RoomIDs ", params1).ToList();
            }
        }       
     
        public List<NarrowSearchDTO> GetEnterpriseQLNarrowSearch(bool IsDeleted, string NarrowSearchKey)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { 
                                                    new SqlParameter("@IsDeleted", IsDeleted), 
                                                    new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? string.Empty) 
                                                 };

                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetEnterpriseQLNarrowSearch] @IsDeleted,@NarrowSearchKey", params1).ToList();
            }
        }
        public List<EnterpriseQLExportDTO> GetAllEnterpriseQuickListWiseLineItem(string ids)
        {
            List<EnterpriseQLExportDTO> obj = new List<EnterpriseQLExportDTO>();
            string[] arrids = ids.Split(',');
            List<Guid> lstids = new List<Guid>();
            foreach (string guiditem in arrids)
            {
                Guid temp;
                if (Guid.TryParse(guiditem, out temp))
                {
                    lstids.Add(temp);
                }
            }
            Guid[] arrguids = new Guid[] { };
            arrguids = lstids.ToArray();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {
                    new SqlParameter("@SearchTerm", string.Join(",",arrguids) ?? (object)DBNull.Value)
                };

                obj = context.Database.SqlQuery<EnterpriseQLExportDTO>("exec [GetEnterpriseQuickList] @SearchTerm", sqlParams).ToList();
            }
            return obj;
        }

        public EnterpriseQLRoomCompany GetCompanyIdsAndRoomIdsByEnterpriseQLGuids(string Guids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guids", Guids)  };
                return context.Database.SqlQuery<EnterpriseQLRoomCompany>("exec GetCompanyIdsAndRoomIdsByEnterpriseQLGuids @Guids", params1).FirstOrDefault();
            }
        }

        #region  #6582. Instock time out related changes
        public List<RPT_EnterpriseQL> GetEnterpriseQLForReportWiseRangeData(long[] RoomIDs, bool IsRunWithReportConnection)
        {
            string _strConnectionString = base.DataBaseEntityConnectionString;
            if (IsRunWithReportConnection)
            {
                _strConnectionString = base.DataBaseEntityConnectionStringForReport;
            }

            var tmpRoomIds = string.Empty;
            if (RoomIDs != null && RoomIDs.Any() && RoomIDs.Count() > 0)
            {
                tmpRoomIds = string.Join(",", RoomIDs);
            }

            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomIDs", tmpRoomIds)
                                            };

            using (var context = new eTurnsEntities(_strConnectionString))
            {
                return context.Database.SqlQuery<RPT_EnterpriseQL>("exec RPT_GetEnterpriseReportWiseQLFor_RangeData @RoomIDs ", params1).ToList();
            }
        }


        #endregion
    }
}
