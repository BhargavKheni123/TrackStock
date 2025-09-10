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
    public partial class TransferMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        //public TransferMasterDAL(base.DataBaseName)
        //{

        //}

        public TransferMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public TransferMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public TransferMasterDTO GetLatestTransferByRoomPlain(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)
                    
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetLatestTransferByRoomPlain] @RoomId,@CompanyId", params1).FirstOrDefault();
            }
        }

        public List<TransferMasterDTO> GetAllTransfersByRoomNormal(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetAllTransfersByRoomNormal] @RoomId,@CompanyId", params1).ToList();
            }
        }

        public List<TransferMasterDTO> GetTransfersByIdsNormal(long RoomId, long CompanyId, long[] Ids)
        {
            var transferIds = "";

            if (Ids != null && Ids.Any() && Ids.Count() > 0)
            {
                transferIds = string.Join(",", Ids);
            }
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId),
                                                   new SqlParameter("@Ids", transferIds)
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransfersByIdsNormal] @RoomId,@CompanyId,@Ids", params1).ToList();
            }
        }

        public List<TransferMasterDTO> GetTransfersByRoomTrfTypeAndStatusPlain(long RoomId, long CompanyId,int RequestType, int TransferStatus)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId),
                                                   new SqlParameter("@RequestType", RequestType),
                                                   new SqlParameter("@TransferStatus", TransferStatus)
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransfersByRoomTrfTypeAndStatusPlain] @RoomId,@CompanyId,@RequestType,@TransferStatus", params1).ToList();
            }
        }

        public IEnumerable<TransferMasterDTO> GetPagedTransferDataForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, string TransferStatusValue = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                TotalCount = 0;
                string strQuer = "";
                string StatusQuery = "";
                string RequestTypeQry = "";
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
                {
                    int idx = SearchTerm.IndexOf("[^]");
                    StatusQuery = SearchTerm.Remove(0, idx + 3);

                    if (!string.IsNullOrEmpty(StatusQuery))
                    {
                        if (StatusQuery.ToLower().Contains("receivable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "0";
                        }
                        else if (StatusQuery.ToLower().Contains("transferable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "1";
                        }
                        else if (StatusQuery.ToLower().Contains("changetransefer"))
                        {
                            StatusQuery = "4";
                        }
                        else if (StatusQuery.ToLower().Contains("requested"))
                        {
                            StatusQuery = "1";
                            RequestTypeQry = "1";
                        }
                    }
                    SearchTerm = SearchTerm.Substring(0, idx);
                }

                sortColumnName = sortColumnName.Replace("RequiredDateString", " RequiredDate");
                strQuer = @"EXEC [GetPagedTransferDataForDashboard] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@TransferStatusIn,@RequestType,@TransferStatus";
                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@CompnayID", CompanyID));
                sqlParas.Add(new SqlParameter("@RoomID", RoomID));
                sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
                sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
                sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
                sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));
                sqlParas.Add(new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value));
                sqlParas.Add(new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value));
                sqlParas.Add(new SqlParameter("@TransferStatusIn", StatusQuery ?? (object)DBNull.Value));
                sqlParas.Add(new SqlParameter("@RequestType", RequestTypeQry ?? (object)DBNull.Value));
                sqlParas.Add(new SqlParameter("@TransferStatus", TransferStatusValue ?? (object)DBNull.Value));

                var params1 = sqlParas.ToArray();

                var transfers = context.Database.SqlQuery<TransferMasterDTO>(strQuer, params1).ToList();
                if (transfers != null && transfers.Count() > 0)
                {
                    TotalCount = transfers.ElementAt(0).TotalRecords;
                }
                return transfers;
            }
        }


        /// <summary>
        /// Get List of Pending Orders
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public List<TransferMasterDTO> GetPendingTransfersByRoomNormal(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@CompanyId", CompanyID)
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetPendingTransfersByRoomNormal] @RoomId,@CompanyId", params1).ToList();
            }
            
        }

        public TransferMasterDTO GetTransferByIdPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferByIdPlain] @Id", params1).FirstOrDefault();
            }
        }
        
        public TransferMasterDTO GetTransferByIdNormal(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferByIdNormal] @Id", params1).FirstOrDefault();
            }
        }

        public TransferMasterDTO GetTransferByIdFull(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferByIdFull] @Id", params1).FirstOrDefault();
            }
        }

        public TransferMasterDTO GetTransferHistoryByIdFull(long HistoryId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryId) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferHistoryByIdFull] @HistoryID", params1).FirstOrDefault();
            }
        }

        public TransferMasterDTO GetTransferHistoryByIdPlain(long HistoryId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryId) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferHistoryByIdPlain] @HistoryID", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Insert Record in the DataBase Transfer
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(TransferMasterDTO objDTO, bool IsCallFromSVC)
        {
            return DB_Insert(objDTO, IsCallFromSVC);
        }

        /// <summary>
        /// Insert Record in the DataBase Transfer
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public TransferMasterDTO InsertTransfer(TransferMasterDTO objDTO)
        {
            return DB_InsertTransfer(objDTO);
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(TransferMasterDTO objDTO)
        {

            return DB_Edit(objDTO);
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID, long SessionUserId,long EnterpriseId)
        {
            return DB_DeleteTransferMasterRecords(IDs, userid, SessionUserId,EnterpriseId);
        }

        #region From DB

        public IEnumerable<CommonDTO> DB_GetTransferNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Status, string MainFilter)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string requestType = "null";

                if (!string.IsNullOrEmpty(Status))
                {
                    if (Status.ToLower() == "receivable")
                    {
                        Status = "4,5,6,7,8";
                        requestType = "0";
                    }

                    else if (Status.ToLower() == "transferable")
                    {
                        Status = "4,5,6,7,8";
                        requestType = "1";
                    }

                    else if (Status.ToLower() == "changetransefer")
                    {
                        Status = "4";
                    }

                    else if (Status.ToLower() == "requested")
                    {
                        Status = "1";
                        requestType = "1";
                    }
                }
                else
                {
                    requestType = "null";
                }


                string strCommand = "EXEC [Trnsfr_GetTransferMasterNarrowSearchData] ";
                strCommand += CompanyID.ToString();
                strCommand += ", " + RoomID.ToString();
                strCommand += ", " + (IsDeleted ? "1" : "0");
                strCommand += ", " + (IsArchived ? "1" : "0");
                strCommand += ", '" + Status + "'";
                strCommand += ", '" + MainFilter + "'";
                strCommand += ", " + requestType + "";

                IEnumerable<CommonDTO> obj = (from u in context.Database.SqlQuery<CommonDTO>(strCommand)
                                              select new CommonDTO
                                              {
                                                  ID = u.ID,
                                                  ControlID = "",
                                                  Count = u.Count,
                                                  PageName = u.PageName,
                                                  Text = u.Text
                                              }).AsParallel().ToList();
                return obj;
            }
        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferMasterDTO> GetPagedTransfers(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, string CurrentTimeZone, string MainFilter = "")
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strBinIDs = string.Empty;
                string strQuer = "";
                string StatusQuery = "";
                string RequestTypeQry = "null";
                string UDF1 = null;
                string UDF2 = null;
                string UDF3 = null;
                string UDF4 = null;
                string UDF5 = null;
                string ItemCreaters = null;
                string ItemUpdators = null;
                string CreatedDateFrom = null;
                string CreatedDateTo = null;
                string UpdatedDateFrom = null;
                string UpdatedDateTo = null;
                string RequiredDateFromMoreThan3Week = null;
                string RequiredDateToMoreThan3Week = null;
                string TransferStatus = null;
                string RequiredDateFrom2to3Weeks = null;
                string RequiredDateTo2to3Weeks = null;
                string RequiredDateFromNextWeek = null;
                string RequiredDateToNextWeek = null;
                string RequiredDateFromThisWeek = null;
                string RequiredDateTothisWeek = null;
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
                {
                    int idx = SearchTerm.IndexOf("[^]");

                    StatusQuery = SearchTerm.Remove(0, idx + 3);
                    if (!string.IsNullOrEmpty(StatusQuery))
                    {
                        if (StatusQuery.ToLower().Contains("receivable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "0";
                        }
                        else if (StatusQuery.ToLower().Contains("transferable"))
                        {
                            StatusQuery = "4,5,6,7,8";
                            RequestTypeQry = "1";
                        }
                        else if (StatusQuery.ToLower().Contains("changetransefer"))
                        {
                            StatusQuery = "4";
                        }
                        else if (StatusQuery.ToLower().Contains("requested"))
                        {
                            StatusQuery = "1";
                            RequestTypeQry = "1";
                        }
                    }
                    SearchTerm = SearchTerm.Substring(0, idx);

                }

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string[] FieldsPara = Fields[0].Split('~');
                    DateTime FromdDate = DateTime.Now;
                    DateTime ToDate = DateTime.Now;

                    if (Fields[0].Split('~')[3] != "")
                    {
                        if (Fields[0].Split('~')[3].Contains("1"))//  > 3 weeks 
                        {
                            FromdDate = DateTime.Now.AddDays(21);
                            ToDate = FromdDate.AddDays(999);
                            RequiredDateFromMoreThan3Week = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateToMoreThan3Week = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("2"))// 2-3 weeks
                        {
                            FromdDate = DateTime.Now.AddDays(14);
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFrom2to3Weeks = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateTo2to3Weeks = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("3"))// Next weeks
                        {
                            FromdDate = DateTime.Now.AddDays(7);
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFromNextWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateToNextWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                        if (Fields[0].Split('~')[3].Contains("4"))// This weeks
                        {
                            FromdDate = DateTime.Now;
                            ToDate = FromdDate.AddDays(7);
                            RequiredDateFromThisWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FromdDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                            RequiredDateTothisWeek = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ToDate.ToString(RoomDateFormat), RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {
                        ItemCreaters = FieldsPara[0];//.TrimEnd(',');
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        ItemUpdators = FieldsPara[1];//.TrimEnd(',');
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        TransferStatus = FieldsPara[2];
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                    {
                        CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                    {
                        UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                        UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                    {
                        string[] arrReplenishTypes = FieldsPara[6].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF1 = UDF1 + HttpUtility.UrlDecode(supitem).Replace("'", "''") + "','";
                        }
                        UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF1 = HttpUtility.UrlDecode(UDF1);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                    {
                        string[] arrReplenishTypes = FieldsPara[7].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF2 = UDF2 + HttpUtility.UrlDecode(supitem).Replace("'", "''") + "','";
                        }
                        UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF2 = HttpUtility.UrlDecode(UDF2);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                    {
                        string[] arrReplenishTypes = FieldsPara[8].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF3 = UDF3 + HttpUtility.UrlDecode(supitem).Replace("'", "''") + "','";
                        }
                        UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF3 = HttpUtility.UrlDecode(UDF3);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                    {
                        string[] arrReplenishTypes = FieldsPara[9].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF4 = UDF4 + HttpUtility.UrlDecode(supitem).Replace("'", "''") + "','";
                        }
                        UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF4 = HttpUtility.UrlDecode(UDF4);
                    }

                    if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                    {
                        string[] arrReplenishTypes = FieldsPara[10].Split(',');
                        foreach (string supitem in arrReplenishTypes)
                        {
                            UDF5 = UDF5 + HttpUtility.UrlDecode(supitem).Replace("'", "''") + "','";
                        }
                        UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                        UDF5 = HttpUtility.UrlDecode(UDF5);
                    }


                    if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                    {
                        strBinIDs = FieldsPara[11];
                    }

                    if (Fields.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(Fields[1]))
                            SearchTerm = Fields[1];
                        else
                            SearchTerm = string.Empty;
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }


                if (!string.IsNullOrEmpty(MainFilter) && MainFilter.ToLower() == "true")
                    MainFilter = "1,4";
                sortColumnName = sortColumnName.Replace("RequestTypeName", " RequestType");

                var spName = IsArchived ? "GetTransferMasterPagedData_Archive" : "Trnsfr_GetTransferMasterPagedData";
                strQuer = @"EXEC ["+ spName + "] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@TransferStatusIn,@RequestType,@MainFilter,@TransferStatus,@BinIDs,@ItemCreaters,@ItemUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@RequiredDateFromMoreThan3Week,@RequiredDateToMoreThan3Week,@RequiredDateFrom2to3Weeks,@RequiredDateTo2to3Weeks,@RequiredDateFromNextWeek,@RequiredDateToNextWeek,@RequiredDateFromThisWeek,@RequiredDateTothisWeek,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5";

                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@CompnayID", CompanyID));
                sqlParas.Add(new SqlParameter("@RoomID", RoomID));
                sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
                sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
                sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
                sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));

                if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName != "null")
                    sqlParas.Add(new SqlParameter("@sortColumnName", sortColumnName));
                else
                    sqlParas.Add(new SqlParameter("@sortColumnName", DBNull.Value));

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm != "null")
                    sqlParas.Add(new SqlParameter("@SearchTerm", SearchTerm));
                else
                    sqlParas.Add(new SqlParameter("@SearchTerm", DBNull.Value));

                if (!string.IsNullOrEmpty(StatusQuery) && StatusQuery != "null")
                    sqlParas.Add(new SqlParameter("@TransferStatusIn", StatusQuery));
                else
                    sqlParas.Add(new SqlParameter("@TransferStatusIn", DBNull.Value));

                if (!string.IsNullOrEmpty(RequestTypeQry) && RequestTypeQry != "null")
                    sqlParas.Add(new SqlParameter("@RequestType", RequestTypeQry));
                else
                    sqlParas.Add(new SqlParameter("@RequestType", DBNull.Value));

                if (!string.IsNullOrEmpty(MainFilter) && MainFilter != "null")
                    sqlParas.Add(new SqlParameter("@MainFilter", MainFilter));
                else
                    sqlParas.Add(new SqlParameter("@MainFilter", DBNull.Value));

                sqlParas.Add(new SqlParameter("@TransferStatus", TransferStatus ?? string.Empty));

                if (!string.IsNullOrEmpty(strBinIDs) && strBinIDs != "null")
                    sqlParas.Add(new SqlParameter("@BinIDs", strBinIDs));
                else
                    sqlParas.Add(new SqlParameter("@BinIDs", DBNull.Value));

                sqlParas.Add(new SqlParameter("@ItemCreaters", ItemCreaters ?? string.Empty));
                sqlParas.Add(new SqlParameter("@ItemUpdators", ItemUpdators ?? string.Empty));
                sqlParas.Add(new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? string.Empty));
                sqlParas.Add(new SqlParameter("@CreatedDateTo", CreatedDateTo ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromMoreThan3Week", RequiredDateFromMoreThan3Week ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateToMoreThan3Week", RequiredDateToMoreThan3Week ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFrom2to3Weeks", RequiredDateFrom2to3Weeks ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateTo2to3Weeks", RequiredDateTo2to3Weeks ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromNextWeek", RequiredDateFromNextWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateToNextWeek", RequiredDateToNextWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateFromThisWeek", RequiredDateFromThisWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@RequiredDateTothisWeek", RequiredDateTothisWeek ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF1", UDF1 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF2", UDF2 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF3", UDF3 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF4", UDF4 ?? string.Empty));
                sqlParas.Add(new SqlParameter("@UDF5", UDF5 ?? string.Empty));

                var params1 = sqlParas.ToArray();

                IEnumerable<TransferMasterDTO> obj = (from u in context.Database.SqlQuery<TransferMasterDTO>(strQuer, params1)
                                                      select new TransferMasterDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferNumber = u.TransferNumber,
                                                          ReplenishingRoomID = u.ReplenishingRoomID,
                                                          StagingID = u.StagingID,
                                                          Comment = u.Comment,
                                                          RequestType = u.RequestType,
                                                          RequireDate = u.RequireDate,
                                                          UDF1 = u.UDF1,
                                                          UDF2 = u.UDF2,
                                                          UDF3 = u.UDF3,
                                                          UDF4 = u.UDF4,
                                                          UDF5 = u.UDF5,
                                                          Created = u.Created,
                                                          Updated = u.Updated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          RoomID = u.RoomID,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          RequestingRoomID = u.RequestingRoomID,
                                                          RefTransferGUID = u.RefTransferGUID,
                                                          TransferStatus = u.TransferStatus,
                                                          CreatedByName = u.CreatedByName,
                                                          RoomName = u.RoomName,
                                                          UpdatedByName = u.UpdatedByName,
                                                          StagingName = u.StagingName,
                                                          ReplenishingRoomName = u.ReplenishingRoomName,
                                                          RequestingRoomName = u.RequestingRoomName,
                                                          RefTransferNumber = u.RefTransferNumber,
                                                          RequestTypeName = Enum.Parse(typeof(RequestType), u.RequestType.ToString()).ToString(),
                                                          TransferStatusName = ResTransfer.GetTransferStatusText(Enum.Parse(typeof(TransferStatus), u.TransferStatus.ToString()).ToString()),
                                                          RejectionReason = u.RejectionReason,
                                                          TransferIsInReceive = u.TransferIsInReceive,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsAbleToDelete = false,
                                                          IsHistory = false,
                                                          IsOnlyStatusUpdate = false,
                                                          IsRecordNotEditable = false,
                                                          TransferDetailList = null,
                                                          AppendedBarcodeString = u.AppendedBarcodeString,
                                                          TotalRecords = u.TotalRecords,
                                                          NoOfItems = u.NoOfItems,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          TotalCost = u.TotalCost
                                                      }).AsParallel().ToList();

                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }
                return obj;
            }

        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DB_DeleteTransferMasterRecords(string IDs, Int64 userid, long SessionUserId, long EnterpriseId)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            var tmpsupplierIds = new List<long>();

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                TransferDetailDAL trDetailDAL = new TransferDetailDAL(base.DataBaseName);
                string strIDs = "";
                string[] strGUIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var item in strGUIDs)
                {
                    strIDs += item + ",";
                }

                strIDs = strIDs.TrimEnd(',');

                strQuery += "Exec Trnsfr_DeleteTransferMaster " + userid.ToString() + ", '" + strIDs + "'";
                
                if (userid > 0 && !string.IsNullOrWhiteSpace(strQuery))
                {
                    context.Database.ExecuteSqlCommand(strQuery);
                
                    foreach (string trmGuid in strGUIDs)
                    {
                        IEnumerable<TransferDetailDTO> objTDetailDTO = trDetailDAL.DB_GetCachedData(null, null, null, null, null, null, Guid.Parse(trmGuid), tmpsupplierIds);
                        
                        foreach (var item in objTDetailDTO)
                        {
                            ItemMasterDTO objITemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                            objItemDAL.Edit(objITemDTO, SessionUserId,EnterpriseId);
                        }
                    }

                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Used to Insert Transfer (guid will be pass as param in case of IsCallFromSVC is true)
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="IsCallFromSVC">true in case of being called from web service</param>
        /// <returns></returns>
        public Int64 DB_Insert(TransferMasterDTO objDTO, bool IsCallFromSVC)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommnad = @"EXEC [InsertTransfer] @TransferNumber,@RequireDate,@ReplenishingRoomID,@RequestingRoomID,@Comment,@StagingID,@TransferStatus,@RequestType,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@CreatedBy,@LastUpdatedBy,@CompanyID,@RoomID,@RejectionReason,@WhatWhereAction,@GUID,@id,@AddedFrom,@EditedFrom,@ReceivedOn,@ReceivedOnWeb";
                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@TransferNumber", objDTO.TransferNumber));
                sqlParas.Add(new SqlParameter("@RequireDate", objDTO.RequireDate));
                sqlParas.Add(new SqlParameter("@ReplenishingRoomID", objDTO.ReplenishingRoomID));
                sqlParas.Add(new SqlParameter("@RequestingRoomID", objDTO.RequestingRoomID));

                if (!string.IsNullOrEmpty(objDTO.Comment))
                    sqlParas.Add(new SqlParameter("@Comment", objDTO.Comment));
                else
                    sqlParas.Add(new SqlParameter("@Comment", DBNull.Value));

                if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                    sqlParas.Add(new SqlParameter("@StagingID", objDTO.StagingID));
                else
                    sqlParas.Add(new SqlParameter("@StagingID", DBNull.Value));

                sqlParas.Add(new SqlParameter("@TransferStatus", objDTO.TransferStatus));
                sqlParas.Add(new SqlParameter("@RequestType", objDTO.RequestType));


                if (!string.IsNullOrEmpty(objDTO.UDF1))
                    sqlParas.Add(new SqlParameter("@UDF1", objDTO.UDF1));
                else
                    sqlParas.Add(new SqlParameter("@UDF1", DBNull.Value));


                if (!string.IsNullOrEmpty(objDTO.UDF2))
                    sqlParas.Add(new SqlParameter("@UDF2", objDTO.UDF2));
                else
                    sqlParas.Add(new SqlParameter("@UDF2", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF3))
                    sqlParas.Add(new SqlParameter("@UDF3", objDTO.UDF3));
                else
                    sqlParas.Add(new SqlParameter("@UDF3", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF4))
                    sqlParas.Add(new SqlParameter("@UDF4", objDTO.UDF4));
                else
                    sqlParas.Add(new SqlParameter("@UDF4", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF5))
                    sqlParas.Add(new SqlParameter("@UDF5", objDTO.UDF5));
                else
                    sqlParas.Add(new SqlParameter("@UDF5", DBNull.Value));

                sqlParas.Add(new SqlParameter("@CreatedBy", objDTO.CreatedBy));
                sqlParas.Add(new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy));
                sqlParas.Add(new SqlParameter("@CompanyID", objDTO.CompanyID));
                sqlParas.Add(new SqlParameter("@RoomID", objDTO.RoomID));

                if (!string.IsNullOrEmpty(objDTO.RejectionReason))
                    sqlParas.Add(new SqlParameter("@RejectionReason", objDTO.RejectionReason));
                else
                    sqlParas.Add(new SqlParameter("@RejectionReason", DBNull.Value));

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Transfer";

                sqlParas.Add(new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction));

                if (IsCallFromSVC)
                {
                    sqlParas.Add(new SqlParameter("@GUID", objDTO.GUID));
                }
                else
                {
                    sqlParas.Add(new SqlParameter("@GUID", DBNull.Value));
                }
                
                sqlParas.Add(new SqlParameter("@id", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                    sqlParas.Add(new SqlParameter("@AddedFrom", objDTO.AddedFrom));
                else
                    sqlParas.Add(new SqlParameter("@AddedFrom", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    sqlParas.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));
                else
                    sqlParas.Add(new SqlParameter("@EditedFrom", DBNull.Value));

                sqlParas.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                sqlParas.Add(new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow)));

                var parameter = sqlParas.ToArray();

                if (!string.IsNullOrEmpty(strCommnad))
                {
                    TransferMasterDTO obj = (from u in context.Database.SqlQuery<TransferMasterDTO>(strCommnad, parameter)
                                             select new TransferMasterDTO
                                             {
                                                 ID = u.ID,
                                                 GUID = u.GUID,
                                             }).AsParallel().FirstOrDefault();

                    objDTO.ID = obj.ID;
                    objDTO.GUID = obj.GUID;

                    new AutoSequenceDAL(base.DataBaseName).UpdateNextTransferNumber(objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);

                    //if (objDTO.RequestType == 0)
                    //{
                    //    new AutoSequenceDAL(base.DataBaseName).UpdateNextTransferNumber(objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);
                    //    //new AutoSequenceDAL(base.DataBaseName).UpdateRoomDetailForNextAutoNumberByModule("NextTransferNo", objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);
                    //}
                    return obj.ID;
                }

                return 0;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase Transfer
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public TransferMasterDTO DB_InsertTransfer(TransferMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<SqlParameter> lstSQLPara = new List<SqlParameter>();
                lstSQLPara.Add(new SqlParameter("@TransferNumber", objDTO.TransferNumber));
                lstSQLPara.Add(new SqlParameter("@RequireDate", objDTO.RequireDate));
                lstSQLPara.Add(new SqlParameter("@ReplenishingRoomID", objDTO.ReplenishingRoomID));
                lstSQLPara.Add(new SqlParameter("@RequestingRoomID", objDTO.RequestingRoomID));
                
                if (!string.IsNullOrEmpty(objDTO.Comment))
                {
                    lstSQLPara.Add(new SqlParameter("@Comment", objDTO.Comment));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@Comment", DBNull.Value));
                }

                if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@StagingID", objDTO.StagingID));                    
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@StagingID", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@TransferStatus", objDTO.TransferStatus));
                lstSQLPara.Add(new SqlParameter("@RequestType", objDTO.RequestType));

                if (!string.IsNullOrEmpty(objDTO.UDF1))
                {
                    lstSQLPara.Add(new SqlParameter("@UDF1", objDTO.UDF1));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@UDF1", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.UDF2))
                {
                    lstSQLPara.Add(new SqlParameter("@UDF2", objDTO.UDF2));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@UDF2", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.UDF3))
                {
                    lstSQLPara.Add(new SqlParameter("@UDF3", objDTO.UDF3));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@UDF3", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.UDF4))
                {
                    lstSQLPara.Add(new SqlParameter("@UDF4", objDTO.UDF4));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@UDF4", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.UDF5))
                {
                    lstSQLPara.Add(new SqlParameter("@UDF5", objDTO.UDF5));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@UDF5", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@CreatedBy", objDTO.CreatedBy));
                lstSQLPara.Add(new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID));
                lstSQLPara.Add(new SqlParameter("@RoomID", objDTO.RoomID));

                if (!string.IsNullOrEmpty(objDTO.RejectionReason))
                {
                    lstSQLPara.Add(new SqlParameter("@RejectionReason", objDTO.RejectionReason));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@RejectionReason", DBNull.Value));
                }

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Transfer";

                lstSQLPara.Add(new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction));
                lstSQLPara.Add(new SqlParameter("@GUID", DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@id", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.AddedFrom))
                {
                    lstSQLPara.Add(new SqlParameter("@AddedFrom", objDTO.AddedFrom));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@AddedFrom", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                {
                    lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));
                }
                else
                {
                    lstSQLPara.Add(new SqlParameter("@EditedFrom", DBNull.Value));
                }

                lstSQLPara.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                lstSQLPara.Add(new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                
                objDTO = context.Database.SqlQuery<TransferMasterDTO>("EXEC [Trnsfr_InsertTransferMasterData] @TransferNumber,@RequireDate,@ReplenishingRoomID,@RequestingRoomID,@Comment,@StagingID,@TransferStatus,@RequestType,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@CreatedBy,@LastUpdatedBy,@CompanyID,@RoomID,@RejectionReason,@WhatWhereAction,@GUID,@id,@AddedFrom,@EditedFrom,@ReceivedOn,@ReceivedOnWeb", lstSQLPara.ToArray()).FirstOrDefault();

                if (objDTO.RequestType == 0)
                {
                    new AutoSequenceDAL(base.DataBaseName).UpdateNextTransferNumber(objDTO.RoomID, objDTO.CompanyID, objDTO.TransferNumber);
                }

                return objDTO;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool DB_Edit(TransferMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommnad = @"EXEC [Trnsfr_UpdateTransferMasterData] @TransferNumber,@RequireDate,@ReplenishingRoomID,@Comment,@StagingID,@TransferStatus,@RefTransferGUID,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@LastUpdatedBy,@RejectionReason,@GUID,@id,@WhatWhereAction,@EditedFrom,@ReceivedOn";
                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@TransferNumber", objDTO.TransferNumber));
                sqlParas.Add(new SqlParameter("@RequireDate", objDTO.RequireDate));
                sqlParas.Add(new SqlParameter("@ReplenishingRoomID", objDTO.ReplenishingRoomID));

                if (!string.IsNullOrEmpty(objDTO.Comment))
                    sqlParas.Add(new SqlParameter("@Comment", objDTO.Comment));
                else
                    sqlParas.Add(new SqlParameter("@Comment", DBNull.Value));

                if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                    sqlParas.Add(new SqlParameter("@StagingID", objDTO.StagingID));
                else
                    sqlParas.Add(new SqlParameter("@StagingID", DBNull.Value));

                sqlParas.Add(new SqlParameter("@TransferStatus", objDTO.TransferStatus));

                if (objDTO.RefTransferGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    sqlParas.Add(new SqlParameter("@RefTransferGUID", objDTO.RefTransferGUID));
                else
                    sqlParas.Add(new SqlParameter("@RefTransferGUID", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF1))
                    sqlParas.Add(new SqlParameter("@UDF1", objDTO.UDF1));
                else
                    sqlParas.Add(new SqlParameter("@UDF1", DBNull.Value));


                if (!string.IsNullOrEmpty(objDTO.UDF2))
                    sqlParas.Add(new SqlParameter("@UDF2", objDTO.UDF2));
                else
                    sqlParas.Add(new SqlParameter("@UDF2", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF3))
                    sqlParas.Add(new SqlParameter("@UDF3", objDTO.UDF3));
                else
                    sqlParas.Add(new SqlParameter("@UDF3", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF4))
                    sqlParas.Add(new SqlParameter("@UDF4", objDTO.UDF4));
                else
                    sqlParas.Add(new SqlParameter("@UDF4", DBNull.Value));

                if (!string.IsNullOrEmpty(objDTO.UDF5))
                    sqlParas.Add(new SqlParameter("@UDF5", objDTO.UDF5));
                else
                    sqlParas.Add(new SqlParameter("@UDF5", DBNull.Value));

                sqlParas.Add(new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy));

                if (!string.IsNullOrEmpty(objDTO.RejectionReason))
                    sqlParas.Add(new SqlParameter("@RejectionReason", objDTO.RejectionReason));
                else
                    sqlParas.Add(new SqlParameter("@RejectionReason", DBNull.Value));

                //for ID and GUID
                sqlParas.Add(new SqlParameter("@GUID", objDTO.GUID));
                sqlParas.Add(new SqlParameter("@id", objDTO.ID));

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Transfer";

                sqlParas.Add(new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction));

                if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                    sqlParas.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));
                else
                    sqlParas.Add(new SqlParameter("@EditedFrom", DBNull.Value));

                if (objDTO.ReceivedOn.HasValue)
                    sqlParas.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.Value));
                else
                    sqlParas.Add(new SqlParameter("@ReceivedOn", DBNull.Value));


                var parameter = sqlParas.ToArray();

                if (!string.IsNullOrEmpty(strCommnad))
                {
                    int cnt = context.Database.ExecuteSqlCommand(strCommnad, parameter);
                    if (cnt > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// This method is used to return intransit items to replenish room when user(requesting room user or replenish room user) performs close transfer.
        /// </summary>
        /// <param name="transferGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ReturnIntransitItemsResult ReturnInTransitItemsToReplenishRoomOnCloseTransfer(Guid TransferGuid, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@TransferGUID", TransferGuid),
                                                   new SqlParameter("@UserId", UserId)
                };
                return context.Database.SqlQuery<ReturnIntransitItemsResult>("exec [Trnsfr_ReturnIntransitQuantityToReplenishRoomOnTransferClose] @TransferGUID,@UserId", params1).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Transfers For Dashboard Chart
        /// </summary>
        /// <param name="StartRowIndex"></param>
        /// <param name="MaxRows"></param>
        /// <param name="SortColumnName"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="TransferStatusValue"></param>
        /// <returns></returns>

        public IEnumerable<TransferMasterDTO> GetTransfersForDashboardChart(int StartRowIndex, int MaxRows, string SortColumnName,
                                              long RoomId, long CompanyId, bool IsArchived, bool IsDeleted, string TransferStatusValue)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
                SortColumnName = SortColumnName.Replace("RequestTypeName", " RequestType");

                List<SqlParameter> sqlParas = new List<SqlParameter>();
                sqlParas.Add(new SqlParameter("@CompnayID", CompanyId));
                sqlParas.Add(new SqlParameter("@RoomID", RoomId));
                sqlParas.Add(new SqlParameter("@IsDeleted", IsDeleted));
                sqlParas.Add(new SqlParameter("@IsArchived", IsArchived));
                sqlParas.Add(new SqlParameter("@StartRowIndex", StartRowIndex));
                sqlParas.Add(new SqlParameter("@MaxRows", MaxRows));

                if (!string.IsNullOrEmpty(SortColumnName) && SortColumnName != "null")
                    sqlParas.Add(new SqlParameter("@sortColumnName", SortColumnName));
                else
                    sqlParas.Add(new SqlParameter("@sortColumnName", DBNull.Value));

                if (!string.IsNullOrEmpty(TransferStatusValue) && TransferStatusValue != "null")
                    sqlParas.Add(new SqlParameter("@TransferStatus", TransferStatusValue));
                else
                    sqlParas.Add(new SqlParameter("@TransferStatus", DBNull.Value));

                return context.Database.SqlQuery<TransferMasterDTO>("EXEC [GetTransferForDashboardChart] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@TransferStatus", sqlParas.ToArray()).ToList();
            }
        }
        public List<TransferMasterDTO> GetTransferMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferMasterChangeLog] @ID", params1).ToList();
            }
        }
        #endregion

        public TransferMasterDTO GetTransferByGuidPlain(Guid Guid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@GUID", Guid) };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferByGuidPlain] @GUID", params1).FirstOrDefault();
            }
        }

        public TransferMasterDTO GetTransferByRefTransferGuidPlain(long RoomId, long CompanyId,Guid RefTransferGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@CompnayID", CompanyId),
                                                    new SqlParameter("@RoomID", RoomId),
                                                    new SqlParameter("@RefTransferGUID", RefTransferGUID)
                                                 };
                return context.Database.SqlQuery<TransferMasterDTO>("exec [GetTransferByRefTransferGuidPlain] @CompnayID,@RoomID,@RefTransferGUID", params1).FirstOrDefault();
            }
        }

        //public bool CloseUnsubmittedTransferByGuids(string Guids, long UserId, string WhatWhereAction , string EditedFrom)
        //{
        //    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {
        //        var params1 = new SqlParameter[] { new SqlParameter("@Guids", Guids),
        //                                           new SqlParameter("@UserId", UserId),
        //                                           new SqlParameter("@WhatWhereAction", WhatWhereAction),
        //                                           new SqlParameter("@EditedFrom", EditedFrom) };

        //        context.Database.ExecuteSqlCommand("EXEC [CloseUnsubmittedTransferByGuids] @Guids,@UserId,@WhatWhereAction,@EditedFrom", params1);
        //        return true;
        //    }
        //}
    }
}
