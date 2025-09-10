using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Linq;
namespace eTurns.DAL
{
    public partial class UDFDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public UDFDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public UDFDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion

        #region [New Methods]
        public UDFDTO GetUDFByIDNormal(long UDFID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFID", UDFID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFByIDNormal] @UDFID", params1).FirstOrDefault();
            }
        }
        public UDFDTO GetUDFByIDPlain(long UDFID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFID", UDFID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFByIDPlain] @UDFID", params1).FirstOrDefault();
            }
        }
        public UDFDTO GetUDFByUDFColumnNamePlain(string UDFColumnName, string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFColumnName", UDFColumnName ?? (object)DBNull.Value), 
                                               new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID) 
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFsByUDFColumnNamePlain] @UDFColumnName,@UDFTableName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public bool GetUDFRequiredStatus(string UDFColumnName, string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFColumnName", UDFColumnName ?? (object)DBNull.Value),
                                               new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<bool>("exec [GetUDFRequiredStatus] @UDFColumnName,@UDFTableName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<UDFDTO> GetUDFsByUDFTableNamePlain(string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value), 
                                               new SqlParameter("@RoomID", RoomID), 
                                               new SqlParameter("@CompanyID", CompanyID) 
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFsByUDFTableNamePlain] @UDFTableName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<UDFDTO> GetRequiredUDFsByUDFTableNamePlain(string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetRequiredUDFsByUDFTableNamePlain] @UDFTableName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<UDFDTO> GetNonDeletedUDFsByUDFTableNamePlain(string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetNonDeletedUDFsByUDFTableNamePlain] @UDFTableName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<UDFDTO> GetUDFsByUDFTableNameNormal(string UDFTableName, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFsByUDFTableNameNormal] @UDFTableName,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public int GetNonDeletedUDFCountByUDFTableName(string UDFTableName, long RoomID, long CompanyID)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomID = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyID = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [GetNonDeletedUDFCountByUDFTableName] @UDFTableName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        #endregion


        #region [Class Methods]        

        public List<UDFDTO> GetPagedUDFsByUDFTableNamePlain(int StartRowIndex, int MaxRows, out int TotalCount, string sortColumnName, long CompanyId,string UDFTableName, long RoomId)
        {
            List<UDFDTO> udfs = new List<UDFDTO>();
            TotalCount = 0;

            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomId = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyId = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            var params1 = new SqlParameter[]
                        {
                            new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value)
                              , new SqlParameter("@RoomID", RoomId)
                              , new SqlParameter("@CompanyID", CompanyId)
                              , new SqlParameter("@StartRowIndex", StartRowIndex)
                              , new SqlParameter("@MaxRows", MaxRows)
                              //, new SqlParameter("@SearchTerm", searchStr ?? string.Empty)
                              , new SqlParameter("@sortColumnName", sortColumnName)                             
                        };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                udfs = context.Database.SqlQuery<UDFDTO>("exec GetPagedUDFsByUDFTableNamePlain @UDFTableName,@RoomID,@CompanyID,@StartRowIndex,@MaxRows,@sortColumnName", params1).ToList();

                if (udfs != null && udfs.Any() && udfs.Count > 0)
                {
                    TotalCount = udfs.First().TotalRecords.GetValueOrDefault(0);
                }
            }

            return udfs;
        }

        public List<UDFDTO> GetDataCompanyTableNameWise(long CompanyID, string UDFTableName, long RoomId)
        {
            List<UDFDTO> udfs = new List<UDFDTO>();
            
            if (UDFTableName == "BOMItemMaster")
            {
                RoomId = 0;
            }
            else if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            if (CompanyID > 0 && (!string.IsNullOrWhiteSpace(UDFTableName)) && RoomId > 0)
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), 
                                                   new SqlParameter("@RoomID", RoomId), 
                                                   new SqlParameter("@UDFTableName", UDFTableName) 
                                                 };

                string strUDFQry = @"EXEC GetUDFByTableNameNormal @CompanyID,@RoomID,@UDFTableName";
                
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    udfs = context.Database.SqlQuery<UDFDTO>(strUDFQry, params1).ToList();
                }
            }
            else
            {
                udfs = GetUDFsByUDFTableNameNormal(UDFTableName, RoomId, CompanyID);
            }

            return udfs;
        }

        public List<UDFDTO> GetPagedUDFsByUDFTableNameNormal(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, string UDFTableName,
                                                            long RoomId, long CompanyId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            if (UDFTableName == null)
                UDFTableName = "";

            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName == "BOMItemMaster")
                RoomId = 0;
            if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                CompanyId = 0;

            if (UDFTableName == "ItemCountList")
            {
                UDFTableName = "ItemMaster";
            }

            List<UDFDTO> udfs = new List<UDFDTO>();
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            TotalCount = 0;
            string searchStr = "";

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            
            {               
                string[] stringSeparators = new string[] { "[###]" };
                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split(stringSeparators, StringSplitOptions.None);

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    foreach (var item in Fields)
                    {
                        if (item.Length > 0)
                        {
                            if (item.Contains("CreatedBy"))
                            {
                                UserCreaters = item.Split('#')[1];
                            }
                            else if (item.Contains("UpdatedBy"))
                            {
                                UserUpdators = item.Split('#')[1];
                            }
                            else if (item.Contains("DateCreatedFrom"))
                            {
                                CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[1], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                                CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[3], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[1], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                                UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[3], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                            }
                        }
                    }
                    if (dd.Length > 2)
                    {
                        if (!string.IsNullOrEmpty(dd[2]))
                            searchStr = dd[2];
                        else
                            searchStr = string.Empty;
                    }
                }
            }
            else
            {
                searchStr = SearchTerm;
            }

            var params1 = new SqlParameter[]
                        {
                            new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value)
                              , new SqlParameter("@RoomID", RoomId)
                              , new SqlParameter("@CompanyID", CompanyId)
                              , new SqlParameter("@StartRowIndex", StartRowIndex)
                              , new SqlParameter("@MaxRows", MaxRows)
                              , new SqlParameter("@SearchTerm", searchStr ?? string.Empty)
                              , new SqlParameter("@sortColumnName", sortColumnName)
                              , new SqlParameter("@UserCreaters", UserCreaters)
                              , new SqlParameter("@UserUpdators", UserUpdators)
                              , new SqlParameter("@CreatedDateFrom", CreatedDateFrom)
                              , new SqlParameter("@CreatedDateTo", CreatedDateTo)
                              , new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom)
                              , new SqlParameter("@UpdatedDateTo", UpdatedDateTo)
                        };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                udfs = context.Database.SqlQuery<UDFDTO>("exec GetPagedUDFsByUDFTableNameNormal @UDFTableName,@RoomID,@CompanyID,@StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@UserCreaters,@UserUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo", params1).ToList();

                if (udfs != null && udfs.Any() && udfs.Count > 0)
                {
                    TotalCount = udfs.First().TotalRecords.GetValueOrDefault(0);
                }
            }

            return udfs;
        }
        
        public List<string> GetReportResourceFilesByPageResourceFile(string PageResourceFile)
        {
                var params1 = new SqlParameter[] { new SqlParameter("@PageResourceFile", PageResourceFile ?? (object)DBNull.Value) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<string>("exec [GetReportResourceFilesByPageResourceFile] @PageResourceFile", params1).ToList();
                }
        }
        public Dictionary<int, string> getReportResourceFileNameWithPrefix(string PageResourceFile)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    Dictionary<int, string> valuePairs = new Dictionary<int, string>();
                    var params1 = new SqlParameter[] { new SqlParameter("@ResFileName", PageResourceFile ?? (object)DBNull.Value) };
                    var spresult = context.Database.SqlQuery<UDFPrefixResult>("exec [SP_ReportResourceFileNameWithPrefix] @ResFileName", params1).ToList();
                    foreach (var item in spresult)
                    {
                        valuePairs.Add(item.Id, item.ReportResourceFile + "$" + item.Prefix);
                    }
                    return valuePairs;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public Dictionary<int, string> getReportResourceFileName(string PageResourceFile)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    Dictionary<int, string> valuePairs = new Dictionary<int, string>();
                    var params1 = new SqlParameter[] { new SqlParameter("@ResFileName", PageResourceFile ?? (object)DBNull.Value) };
                    var spresult = context.Database.SqlQuery<UDFPrefixResult>("exec [SP_ReportResourceFileNameWithPrefix] @ResFileName", params1).ToList();
                    foreach (var item in spresult)
                    {
                        valuePairs.Add(item.Id, item.ReportResourceFile + "$" + item.Prefix);
                    }
                    return valuePairs;
                }
            }
            catch
            {
                return null;
            }
        }
        public List<string> GetPageResourceFilesByReportResourceFile(string ReportResourceFile)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ReportResourceFile", ReportResourceFile ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<string>("exec [GetPageResourceFilesByReportResourceFile] @ReportResourceFile", params1).ToList();
            }
        }
        
        public IEnumerable<UDFDTO> GetUDFsByRoomPlain(long RoomId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId)  };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFsByRoomPlain] @RoomId", params1).ToList();
            }
        }

        public List<UDFDTO> GetNonDeletedPOs(long CompanyID, long RoomID)
        {
            var pos = GetNonDeletedUDFsByUDFTableNamePlain("PullPoMaster", RoomID, CompanyID);

            if (pos != null && pos.Any() && pos.Count() > 0)
            {
                pos.ForEach(x =>
                {
                    x.UDFDefaultValue = string.Empty;
                });
            }
            return pos;
        }

        public UDFDTO GetSinglePullPOPlain(long CompanyId, long RoomId)
        {
            string UDFTableName = "PullPoMaster";
            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@RoomID", RoomId),
                                               new SqlParameter("@CompanyID", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetSinglePullPOPlain] @UDFTableName,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public IEnumerable<UDFDTO> GetAllUDFTableNameByRoom(long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID) , new SqlParameter("@CompanyID", CompanyID) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetAllUDFTableNameByRoom] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<UDFDTO> GetPagedUDFsByUDFTableNameForEturns(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, string UDFTableName,string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<UDFDTO> udfs = new List<UDFDTO>();
            string UserCreaters = string.Empty;
            string UserUpdators = string.Empty;
            string CreatedDateFrom = string.Empty;
            string CreatedDateTo = string.Empty;
            string UpdatedDateFrom = string.Empty;
            string UpdatedDateTo = string.Empty;
            TotalCount = 0;
            string searchStr = "";

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                string[] stringSeparators = new string[] { "[###]" };

                if (dd != null && dd.Length > 0)
                {
                    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                    // 6 counts for fields based on that prepare the search string
                    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                    foreach (var item in Fields)
                    {
                        if (item.Length > 0)
                        {
                            if (item.Contains("CreatedBy"))
                            {
                                UserCreaters = item.Split('#')[1];
                            }
                            else if (item.Contains("UpdatedBy"))
                            {
                                UserUpdators = item.Split('#')[1];
                            }
                            else if (item.Contains("DateCreatedFrom"))
                            {
                                CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[1], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                                CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[3], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                            }
                            else if (item.Contains("DateUpdatedFrom"))
                            {
                                UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[1], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                                UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.Split('#')[3], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                            }                            
                        }
                    }
                }
            }
            else
            {
                searchStr = SearchTerm;
            }

            long RoomId = 0;
            long CompanyId = 0;

            var params1 = new SqlParameter[]
                        {
                            new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value)
                              , new SqlParameter("@RoomID", RoomId)
                              , new SqlParameter("@CompanyID", CompanyId)
                              , new SqlParameter("@StartRowIndex", StartRowIndex)
                              , new SqlParameter("@MaxRows", MaxRows)
                              , new SqlParameter("@SearchTerm", searchStr ?? string.Empty)
                              , new SqlParameter("@sortColumnName", sortColumnName)
                              , new SqlParameter("@UserCreaters", UserCreaters)
                              , new SqlParameter("@UserUpdators", UserUpdators)
                              , new SqlParameter("@CreatedDateFrom", CreatedDateFrom)
                              , new SqlParameter("@CreatedDateTo", CreatedDateTo)
                              , new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom)
                              , new SqlParameter("@UpdatedDateTo", UpdatedDateTo)                             
                        };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                udfs =  context.Database.SqlQuery<UDFDTO>("exec GetPagedUDFsByUDFTableNameForEturns @UDFTableName,@RoomID,@CompanyID,@StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@UserCreaters,@UserUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo", params1).ToList();

                if (udfs != null && udfs.Any() && udfs.Count > 0)
                {
                    TotalCount = udfs.First().TotalRecords.GetValueOrDefault(0);
                }
            }

            return udfs;
        }

        public Int64 Insert(UDFDTO objDTO)
        {
            if (objDTO.UDFTableName.ToLower() == "companymaster" || objDTO.UDFTableName.ToLower() == "room" || objDTO.UDFTableName.ToLower() == "usermaster")
                objDTO.Room = 0;
            if (objDTO.UDFTableName.ToLower() == "companymaster" || objDTO.UDFTableName.ToLower() == "usermaster")
                objDTO.CompanyID = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = 0;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();
                obj.Room = objDTO.Room;
                obj.UDFMaxLength = objDTO.UDFMaxLength ?? 200;
                context.UDFs.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }
        }
        public bool Edit(UDFDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            if (objDTO.UDFTableName.ToLower() == "companymaster" || objDTO.UDFTableName.ToLower() == "room" || objDTO.UDFTableName.ToLower() == "usermaster")
                objDTO.Room = 0;
            if (objDTO.UDFTableName.ToLower() == "companymaster" || objDTO.UDFTableName.ToLower() == "usermaster")
                objDTO.CompanyID = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = context.UDFs.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                obj.ID = objDTO.ID;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;

                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                if (objDTO.CreatedBy != null && objDTO.Created != null)
                {
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.Created = objDTO.Created;
                }
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = objDTO.Updated;

                obj.IsDeleted = objDTO.IsDeleted;
                obj.IsEncryption = objDTO.IsEncryption ?? false;
                if (obj.IsDeleted)
                    obj.UDFDefaultValue = string.Empty;

                obj.UDFMaxLength = objDTO.UDFMaxLength ?? 200;
                obj.GUID = Guid.NewGuid();
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }
        
        public bool DeleteRecords(string IDs, long UserId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrWhiteSpace(IDs))
                {
                    var params1 = new SqlParameter[] {
                                                        new SqlParameter("@UserID", UserId),
                                                        new SqlParameter("@IDs", IDs)
                                                        };

                    string strQuery = @"EXEC DeleteUDFByIDs @UserID,@IDs";
                    context.Database.ExecuteSqlCommand(strQuery, params1);
                    return true;
                }
                return false;                
            }
        }
        
        public bool InsertDefaultUDFs(string UDFTableName, Int64 CompanyID, Int64 UserID, Int64 RoomID, Int64 ResourcePageId = 0, int iUDFMaxLength = 200)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "room" || UDFTableName.ToLower() == "usermaster" || UDFTableName.ToLower().Contains("bom"))
                    RoomID = 0;
                if (UDFTableName.ToLower() == "companymaster" || UDFTableName.ToLower() == "usermaster")
                    CompanyID = 0;
                //string sSQL = "";
                //string DateNow = DateTimeUtility.DateTimeNow.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String)))); //System.DateTime.Now.ToString(Convert.ToString((new System.Configuration.AppSettingsReader()).GetValue("FormatDateTimetoStore", typeof(System.String))));
                //string UDFColumnName;
                int noofudf = 5;
                UDFModule objUDFModule = GetUDFModule(UDFTableName);

                if (objUDFModule != null)
                {
                    if (objUDFModule.NoOfUdfs > 0)
                    {
                        noofudf = objUDFModule.NoOfUdfs;
                    }
                }

                bool isIncludeInNarrowSearch = true;
                if (UDFTableName.ToLower() == "orderdetails")
                {
                    isIncludeInNarrowSearch = false;
                }

                var eTurnsDBName = CommonDAL.GeteTurnsDatabase();
                var SqlParam = new SqlParameter[] {
                                                          new SqlParameter("@UDFTableName", UDFTableName),                                                          
                                                          new SqlParameter("@UserID", UserID),
                                                          new SqlParameter("@UDFMaxLength", iUDFMaxLength),
                                                          new SqlParameter("@eTurnsDBName", eTurnsDBName),
                                                          new SqlParameter("@NoOfUDF", noofudf) ,
                                                          new SqlParameter("@IsIncludeInNarrowSearch", isIncludeInNarrowSearch) ,
                                                          new SqlParameter("@CompanyID", CompanyID) ,
                                                          new SqlParameter("@RoomID", RoomID) ,
                                                          new SqlParameter("@ResourcePageId", ResourcePageId) ,
                                                   };
                var iResult = context.Database.ExecuteSqlCommand("exec [InsertDefaultUDFs] @UDFTableName,@UserID,@UDFMaxLength,@eTurnsDBName,@NoOfUDF,@IsIncludeInNarrowSearch,@CompanyID,@RoomID,@ResourcePageId", SqlParam);

                return true;
            }
        }

        public bool InsertUserMasterUDFs(string UDFTableName, Int64 CompanyID, Int64 UserID, Int64 RoomID, Int64 ResourcePageId = 0, int iUDFMaxLength = 200)
        {
                using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ResourcePageId", ResourcePageId), new SqlParameter("@UserId", UserID), new SqlParameter("@UDFMaxLength", iUDFMaxLength) };
                    context.Database.SqlQuery<ItemLocationQTYDTO>("exec [InsertUserMasterUDFs] @UDFTableName,@CompanyID,@RoomID,@ResourcePageId,@UserId,@UDFMaxLength", params1).ToList();
                }
                return true;
        }

        public UDFDTO GetUDFByCompanyAndColumnNamePlain(string UDFColumnName, string UDFTableName, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFColumnName", UDFColumnName ?? (object)DBNull.Value),
                                               new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFByCompanyAndColumnNamePlain] @UDFColumnName,@UDFTableName,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<UDFDTO> GetUDFsByCompanyAndTableNamePlain(long CompanyID, string UDFTableName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", UDFTableName ?? (object)DBNull.Value),
                                               new SqlParameter("@CompanyID", CompanyID)
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFDTO>("exec [GetUDFsByCompanyAndTableNamePlain] @UDFTableName,@CompanyID", params1).ToList();
            }
        }

        public int BulkInsertForUDFData(List<UDFBulkItem> lstUDF, string TableName, long CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    var udfTable = new DataTable();
                    udfTable.Columns.Add("UDFID", typeof(Int64));
                    udfTable.Columns.Add("UDFOption", typeof(string));
                    udfTable.Columns.Add("UDFColumnName", typeof(string));

                    foreach (var item in lstUDF)
                    {
                        udfTable.Rows.Add(item.UDFID, item.UDFOption, item.UDFColumnName);
                    }

                    var parameters = new[]
                    {
                        new SqlParameter("@TableName", TableName),
                        new SqlParameter("@CompanyId", CompanyId),
                        new SqlParameter("@UDFList", SqlDbType.Structured)
                        {
                            TypeName = "UDFList_BulkType",
                            Value = udfTable
                        }
                    };
                    var result = context.Database.SqlQuery<int>("EXEC BulkInsertForUDFList @TableName, @CompanyId, @UDFList", parameters).FirstOrDefault();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public UDFModule GetUDFModule(string Name)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UDFModuleName", Name )  };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFModule>("exec [GetUDFModuleByModuleNamePlain] @UDFModuleName", params1).FirstOrDefault();
            }
        }
        
        public UDFDTO GetPullPORecord(long CompanyID, long RoomID, Int64 UserId, int iUDFMaxLength = 200)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFDTO obj = GetSinglePullPOPlain(CompanyID, RoomID);

                if (obj == null)
                {
                    UDFDTO objUdfdto = new UDFDTO();
                    objUdfdto.CompanyID = CompanyID;
                    objUdfdto.Created = DateTimeUtility.DateTimeNow;
                    objUdfdto.CreatedBy = UserId;
                    objUdfdto.GUID = Guid.NewGuid();
                    objUdfdto.IsArchived = false;
                    objUdfdto.IsDeleted = false;
                    objUdfdto.LastUpdatedBy = UserId;
                    objUdfdto.Room = RoomID;
                    objUdfdto.UDFColumnName = "UDF1";
                    objUdfdto.UDFControlType = "Dropdown Editable";
                    objUdfdto.UDFDefaultValue = string.Empty;
                    objUdfdto.UDFIsRequired = false;
                    objUdfdto.UDFIsSearchable = false;
                    objUdfdto.UDFTableName = "PullPoMaster";
                    objUdfdto.Updated = DateTimeUtility.DateTimeNow;
                    objUdfdto.IsEncryption = false;
                    objUdfdto.UDFMaxLength = iUDFMaxLength;
                    return UDFInsert(objUdfdto);

                }
                return obj;
            }

        }

        public UDFDTO UDFInsert(UDFDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDF obj = new UDF();
                obj.ID = 0;
                obj.CompanyID = objDTO.CompanyID;
                obj.UDFTableName = objDTO.UDFTableName;
                obj.UDFColumnName = objDTO.UDFColumnName;
                obj.UDFControlType = objDTO.UDFControlType;
                obj.UDFDefaultValue = objDTO.UDFDefaultValue;
                obj.UDFOptionsCSV = objDTO.UDFOptionsCSV;
                obj.UDFIsRequired = objDTO.UDFIsRequired;
                obj.UDFIsSearchable = objDTO.UDFIsSearchable;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.UDFMaxLength = objDTO.UDFMaxLength;
                obj.GUID = Guid.NewGuid();
                obj.Room = objDTO.Room;
                context.UDFs.Add(obj);
                context.SaveChanges();
                return GetSinglePullPOPlain(objDTO.CompanyID, objDTO.Room.GetValueOrDefault(0));
            }
        }
        
        public IEnumerable<ExportUDFDTO> GetUDFExport(Int64 RoomID, Int64 CompanyID, string UDFTableName, bool? IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ExportUDFDTO> obj = null;
                var params1 = new SqlParameter[] { };
                List<ExportUDFDTO> lstresult;
                if (IsDeleted != null)
                {
                    params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@ModuleName", UDFTableName), new SqlParameter("@Isdeleted", IsDeleted) };
                    lstresult = context.Database.SqlQuery<ExportUDFDTO>("exec GetUDFData @RoomId,@CompanyId,@ModuleName,@Isdeleted", params1).ToList();
                }
                else
                {
                    params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@ModuleName", UDFTableName), new SqlParameter("@Isdeleted", DBNull.Value) };
                    lstresult = context.Database.SqlQuery<ExportUDFDTO>("exec GetUDFData @RoomId,@CompanyId,@ModuleName,@Isdeleted", params1).ToList();
                }
                CommonDAL objCommon = new CommonDAL(base.DataBaseName);
                obj = (from u in lstresult
                       select new ExportUDFDTO
                       {
                           ModuleName = u.ModuleName,
                           UDFColumnName = u.UDFColumnName,
                           UDFName = string.Empty,
                           ControlType = u.ControlType,
                           UDFDefaultValue = u.UDFDefaultValue,
                           OptionName = (u.IsEncryption ?? false) == false ? u.OptionName : objCommon.GetDecryptValue(u.OptionName),
                           IsRequired = u.IsRequired,
                           IsDeleted = u.IsDeleted,
                           IncludeInNarrowSearch = u.IncludeInNarrowSearch,
                           UDFMaxLength = u.UDFMaxLength
                       }).ToList();
                return obj;
            }
        }


        public bool UDFOptionInsert(CartItemDTO objCartItemDTO)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@UDFTableName", "cartitemlist")
                        , new SqlParameter("@LastUpdatedBy", objCartItemDTO.LastUpdatedBy)
                        , new SqlParameter("@RoomId", objCartItemDTO.Room)
                        , new SqlParameter("@CompanyId", objCartItemDTO.CompanyID)
                        , new SqlParameter("@UDF1", objCartItemDTO.UDF1)
                        , new SqlParameter("@UDF2", objCartItemDTO.UDF2)
                        , new SqlParameter("@UDF3", objCartItemDTO.UDF3 ?? (object)DBNull.Value)
                        , new SqlParameter("@UDF4", objCartItemDTO.UDF4 ?? (object)DBNull.Value)
                        , new SqlParameter("@UDF5", objCartItemDTO.UDF5 ?? (object)DBNull.Value)
                    };
                    context.Database.ExecuteSqlCommand("exec [SetUDFAndUDFOptions] @UDFTableName,@LastUpdatedBy,@RoomId,@CompanyId,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5", params1);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        public bool InsertDefaultUDFsByRoom(long UserID, long CompanyId, int iUDFMaxLength = 200)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var eTurnsDBName = DbConnectionHelper.GeteTurnsDBName();
                    var SqlParam = new SqlParameter[] {
                                                          new SqlParameter("@UDFTableName", "Room"),
                                                          new SqlParameter("@UserID", UserID),
                                                          new SqlParameter("@UDFMaxLength", iUDFMaxLength),
                                                          new SqlParameter("@eTurnsDBName", eTurnsDBName),
                                                          new SqlParameter("@NoOfUDF",5),
                                                          new SqlParameter("@IsIncludeInNarrowSearch",false),
                                                          new SqlParameter("@CompanyID",CompanyId),
                                                          new SqlParameter("@RoomID",Convert.ToInt16(0)),
                                                                                                             };
                    var iResult = context.Database.ExecuteSqlCommand("exec [InsertDefaultUDFsByRoom] @UDFTableName,@UserID,@UDFMaxLength,@eTurnsDBName,@NoOfUDF,@IsIncludeInNarrowSearch,@CompanyID,@RoomID", SqlParam);
                    if (iResult > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
    }
}
