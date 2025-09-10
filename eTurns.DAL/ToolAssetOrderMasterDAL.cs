using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public partial class ToolAssetOrderMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]
        public ToolAssetOrderMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        //public ToolAssetOrderMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion

        #region Call DB SPs By following Functions

        public IEnumerable<ToolAssetOrderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, ToolAssetOrderType OrdType)
        {
            return DB_GetCachedData(CompanyId, RoomID, null, null, null, null, OrdType);

        }
        public IEnumerable<ToolAssetOrderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, ToolAssetOrderType OrdType)
        {
            return DB_GetCachedData(CompanyId, RoomID, IsDeleted, IsArchived, null, null, OrdType);
        }
        public IEnumerable<ToolAssetOrderMasterDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, ToolAssetOrderType OrdType, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string MainFilter = "False", int HaveLineItem = 0)
        {
            return DB_GetPagedRecords(StartRowIndex, MaxRows, out TotalCount, SearchTerm, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, OrdType, RoomDateFormat, CurrentTimeZone, HaveLineItem);
        }
        public ToolAssetOrderMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(CompanyID, RoomID, id);

        }
        public ToolAssetOrderMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(CompanyID, RoomID, GUID);
        }
        public ToolAssetOrderMasterDTO InsertOrder(ToolAssetOrderMasterDTO objDTO)
        {
            return DB_InsertOrder(objDTO);

        }
        public bool Edit(ToolAssetOrderMasterDTO objDTO)
        {
            DB_UpdateOrderMaster(objDTO);
            return true;
        }
        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            return DB_DeleteOrderMaster(IDs, userid, RoomID, CompanyID);

        }
        public void UpdateOrderComment(string comment, Int64 OrderId, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetOrderMaster obj = context.ToolAssetOrderMasters.FirstOrDefault(x => x.ID == OrderId);
                obj.Comment = comment;

                obj.LastUpdatedBy = UserID;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.SaveChanges();
            }
        }
        public void UpdateOrderComment(string comment, string PackslipNumber, Int64 OrderId, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetOrderMaster obj = context.ToolAssetOrderMasters.FirstOrDefault(x => x.ID == OrderId);
                obj.Comment = comment;

                obj.PackSlipNumber = PackslipNumber;

                obj.LastUpdatedBy = UserID;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.SaveChanges();
            }
        }
        private IEnumerable<ToolAssetOrderMasterDTO> DB_GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, ToolAssetOrderType OrdType, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, int HaveLineItem = 0)
        {
            string StatusQuery = "";
            string NarrowSearchQry = "";

            if (MaxRows < 1)
            {
                MaxRows = 10;
            }

            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[^]"))
            {
                int idx = SearchTerm.IndexOf("[^]");
                StatusQuery = SearchTerm.Remove(0, idx + 3);
                SearchTerm = SearchTerm.Substring(0, idx);
            }

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                string[] FieldsPara = Fields[0].Split('~');
                DateTime FromdDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;

                if (Fields[0].Split('~')[4] != "")
                {
                    string RDateCondi = string.Empty;

                    if (Fields[0].Split('~')[4].Contains("1"))//  > 3 weeks 
                    {
                        FromdDate = DateTime.Now.AddDays(21);
                        ToDate = FromdDate.AddDays(999);

                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }
                    if (Fields[0].Split('~')[4].Contains("2"))// 2-3 weeks
                    {
                        FromdDate = DateTime.Now.AddDays(14);
                        ToDate = DateTime.Now.AddDays(21); //FromdDate.AddDays(21);
                        if (!string.IsNullOrWhiteSpace(RDateCondi))
                        {
                            RDateCondi += " Or ";
                        }
                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (Fields[0].Split('~')[4].Contains("3"))// Next weeks
                    {
                        FromdDate = DateTime.Now.AddDays(7);
                        ToDate = DateTime.Now.AddDays(14); // FromdDate.AddDays(14);

                        if (!string.IsNullOrWhiteSpace(RDateCondi))
                        {
                            RDateCondi += " Or ";
                        }
                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (Fields[0].Split('~')[4].Contains("4"))// This weeks
                    {
                        ToDate = FromdDate.AddDays(7);
                        if (!string.IsNullOrWhiteSpace(RDateCondi))
                        {
                            RDateCondi += " Or ";
                        }
                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    if (!string.IsNullOrWhiteSpace(RDateCondi))
                    {
                        RDateCondi = "(" + RDateCondi + ")";
                    }
                    NarrowSearchQry = RDateCondi;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " CreatedBy IN (" + FieldsPara[0] + ")";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " LastUpdatedBy IN (" + FieldsPara[1] + ")";
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " Supplier IN (" + FieldsPara[2] + ")";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " OrderStatus IN (" + FieldsPara[3] + ")";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,Created) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105)) ";

                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,LastUpdated) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105)) ";

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string UDF1 = string.Empty;
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";
                    NarrowSearchQry += " UDF1 IN ('" + UDF1 + "')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    string UDF2 = string.Empty;
                    string[] arrReplenishTypes = FieldsPara[9].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";
                    NarrowSearchQry += " UDF2 IN ('" + UDF2 + "')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    string UDF3 = string.Empty;
                    string[] arrReplenishTypes = FieldsPara[10].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";
                    NarrowSearchQry += " UDF3 IN ('" + UDF3 + "')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    string UDF4 = string.Empty;
                    string[] arrReplenishTypes = FieldsPara[11].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";
                    NarrowSearchQry += " UDF4 IN ('" + UDF4 + "')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[12]))
                {
                    string UDF5 = string.Empty;
                    string[] arrReplenishTypes = FieldsPara[12].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";
                    NarrowSearchQry += " UDF5 IN ('" + UDF5 + "')";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[13]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " ShippingVendor IN (" + FieldsPara[13] + ")";
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

            string OrderType = Convert.ToString((int)OrdType);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID )
                        ,new SqlParameter("@RoomID", RoomID )
                        , new SqlParameter("@IsDeleted", IsDeleted)
                        , new SqlParameter("@IsArchived", IsArchived)
                        , new SqlParameter("@StartRowIndex", StartRowIndex)
                        , new SqlParameter("@MaxRows", MaxRows)
                        , new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value)
                        , new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value)
                        , new SqlParameter("@NarrowSearchQry", NarrowSearchQry ?? (object)DBNull.Value)
                        , new SqlParameter("@StatusQuery", StatusQuery ?? (object)DBNull.Value)
                        , new SqlParameter("@OrderType", OrderType ?? (object)DBNull.Value)
                        , new SqlParameter("@HaveLineItem", HaveLineItem)
                };
                List<ToolAssetOrderMasterDTO> objToolAssetOrderMasterDTO =
               context.Database.SqlQuery<ToolAssetOrderMasterDTO>("exec [GetToolAssetOrderMasterPagedData] @CompanyID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@NarrowSearchQry,@StatusQuery,@OrderType,@HaveLineItem", params1).ToList();
                TotalCount = 0;
                if (objToolAssetOrderMasterDTO != null && objToolAssetOrderMasterDTO.Count() > 0)
                {
                    TotalCount = objToolAssetOrderMasterDTO.FirstOrDefault().TotalRecords;
                }
                return objToolAssetOrderMasterDTO;
            }

        }
        private IEnumerable<ToolAssetOrderMasterDTO> DB_GetCachedData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, ToolAssetOrderType OrdType)
        {
            string strCommand = "EXEC GetToolAssetOrderMasterData ";

            if (CompanyID.HasValue)
                strCommand += CompanyID.Value.ToString();
            else
                strCommand += "null";
            if (RoomID.HasValue)
                strCommand += ", " + RoomID.Value.ToString();
            else
                strCommand += ", " + "null";

            if (IsDeleted.HasValue)
                strCommand += ", " + (IsDeleted.Value ? "1" : "0");
            else
                strCommand += ", " + "null";

            if (IsArchived.HasValue)
                strCommand += ", " + (IsArchived.Value ? "1" : "0");
            else
                strCommand += ", " + "null";

            if (ID.HasValue)
                strCommand += ", " + ID.Value.ToString();
            else
                strCommand += ", " + "null";

            if (GuID.HasValue)
                strCommand += ", '" + GuID.Value.ToString() + "'";
            else
                strCommand += ", " + "null";

            strCommand += ", " + Convert.ToString((int)OrdType);
            IEnumerable<ToolAssetOrderMasterDTO> obj = ExecuteQuery(strCommand);
            return obj;

        }
        private ToolAssetOrderMasterDTO DB_GetRecord(Int64? CompanyID, Int64? RoomID, Int64 ID)
        {
            string strCommand = "EXEC GetToolAssetOrderMasterData ";

            if (CompanyID.HasValue)
                strCommand += CompanyID.Value.ToString();
            else
                strCommand += "null";
            if (RoomID.HasValue)
                strCommand += ", " + RoomID.Value.ToString();
            else
                strCommand += ", " + "null";

            strCommand += ", " + "null";
            strCommand += ", " + "null";
            strCommand += ", " + ID.ToString();
            strCommand += ", " + "null";

            ToolAssetOrderMasterDTO obj = ExecuteQuery(strCommand).FirstOrDefault();
            return obj;

        }
        private ToolAssetOrderMasterDTO DB_GetRecord(Int64? CompanyID, Int64? RoomID, Guid Guid)
        {
            string strCommand = "EXEC GetToolAssetOrderMasterData ";

            if (CompanyID.HasValue)
                strCommand += CompanyID.Value.ToString();
            else
                strCommand += "null";
            if (RoomID.HasValue)
                strCommand += ", " + RoomID.Value.ToString();
            else
                strCommand += ", " + "null";

            strCommand += ", " + "null";
            strCommand += ", " + "null";
            strCommand += ", " + "null";
            strCommand += ", '" + Guid.ToString() + "'";
            ToolAssetOrderMasterDTO obj = ExecuteQuery(strCommand).FirstOrDefault();
            return obj;

        }
        private ToolAssetOrderMasterDTO DB_InsertOrderMaster(ToolAssetOrderMasterDTO objDTO)
        {
            try
            {
                string strCommand = "EXEC InsertToolAssetOrderMaster ";
                strCommand += "'" + objDTO.ToolAssetOrderNumber + "'";
                strCommand += ",'" + objDTO.ReleaseNumber + "'";
                strCommand += "," + objDTO.CreatedBy.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.RoomID.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.CompanyID.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.OrderStatus + " ";

                if (objDTO.OrderType.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.OrderType.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";

                strCommand += ",'" + objDTO.RequiredDate.ToString("yyyy-MM-dd") + "' ";
                strCommand += ",'" + objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd") + "' ";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.Comment) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@PackSlipNumber) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF1) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF2) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF3) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF4) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF5) + "'";

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Order";

                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.WhatWhereAction) + "'";

                if (string.IsNullOrEmpty(objDTO.OrderNumber_ForSorting))
                    objDTO.OrderNumber_ForSorting = objDTO.ToolAssetOrderNumber;

                strCommand += ",'" + CommonDAL.GetSortingString(objDTO.ToolAssetOrderNumber) + "'";
                strCommand += ",'" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";
                strCommand += ",'" + objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

                if (!string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    strCommand += ",'" + objDTO.AddedFrom + "' ";
                else
                    strCommand += ",'Web'";

                if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    strCommand += ",'" + objDTO.EditedFrom + "' ";
                else
                    strCommand += ",'Web'";

                if (objDTO.IsEDIOrder.GetValueOrDefault(false))
                    strCommand += ",1";
                else
                    strCommand += ",0";

                strCommand += ",@OrderGuid='" + objDTO.GUID + "'";
                objDTO = ExecuteQuery(strCommand).FirstOrDefault();
                return objDTO;
            }
            finally
            {

            }
        }
        private object DBNullValueorStringIfNotNull(string value)
        {
            object o;
            if (value == null)
            {
                o = DBNull.Value;
            }
            else
            {
                o = value;
            }
            return o;
        }
        private ToolAssetOrderMasterDTO DB_InsertOrder(ToolAssetOrderMasterDTO objDTO)
        {
            AutoSequenceDAL objAutoSeqDAL = null;
            try
            {
                string strCommand = @"EXEC [InsertToolAssetOrderMaster] @OrderNumber,@ReleaseNumber ,@CreatedBy,@RoomID,@CompanyID,@OrderStatus,
                                                                    @OrderType,@RequiredDate,@OrderDate,@Comment,@PackSlipNumber,
                                                                   @UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@WhatWhereAction,@OrderNumber_ForSorting,
                                                                    @ReceivedOn,@ReceivedOnWeb,@AddedFrom,@EditedFrom,@IsEDIOrder,@OrderGuid,@EditedOnAction";



                List<SqlParameter> lstSQLPara = new List<SqlParameter>();

                lstSQLPara.Add(new SqlParameter("@OrderNumber", objDTO.ToolAssetOrderNumber));
                lstSQLPara.Add(new SqlParameter("@ReleaseNumber", objDTO.ReleaseNumber));
                lstSQLPara.Add(new SqlParameter("@CreatedBy", objDTO.CreatedBy.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@RoomID", objDTO.RoomID.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@OrderStatus", objDTO.OrderStatus));

                if (objDTO.OrderType.GetValueOrDefault(0) > 0)
                    lstSQLPara.Add(new SqlParameter("@OrderType", objDTO.OrderType));
                else
                    lstSQLPara.Add(new SqlParameter("@OrderType", DBNull.Value));

                lstSQLPara.Add(new SqlParameter("@RequiredDate", objDTO.RequiredDate.ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@OrderDate", objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@Comment", DBNullValueorStringIfNotNull(objDTO.Comment)));
                lstSQLPara.Add(new SqlParameter("@PackSlipNumber", DBNullValueorStringIfNotNull(objDTO.PackSlipNumber)));
                lstSQLPara.Add(new SqlParameter("@UDF1", DBNullValueorStringIfNotNull(objDTO.UDF1)));
                lstSQLPara.Add(new SqlParameter("@UDF2", DBNullValueorStringIfNotNull(objDTO.UDF2)));
                lstSQLPara.Add(new SqlParameter("@UDF3", DBNullValueorStringIfNotNull(objDTO.UDF3)));
                lstSQLPara.Add(new SqlParameter("@UDF4", DBNullValueorStringIfNotNull(objDTO.UDF4)));
                lstSQLPara.Add(new SqlParameter("@UDF5", DBNullValueorStringIfNotNull(objDTO.UDF5)));
                lstSQLPara.Add(new SqlParameter("@WhatWhereAction", DBNullValueorStringIfNotNull(objDTO.WhatWhereAction)));

                if (string.IsNullOrEmpty(objDTO.OrderNumber_ForSorting))
                    objDTO.OrderNumber_ForSorting = objDTO.ToolAssetOrderNumber;

                lstSQLPara.Add(new SqlParameter("@OrderNumber_ForSorting", DBNullValueorStringIfNotNull(CommonDAL.GetSortingString(objDTO.ToolAssetOrderNumber.Replace("'", "")))));
                lstSQLPara.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                lstSQLPara.Add(new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow)));

                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    objDTO.AddedFrom = "Web";

                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    objDTO.EditedFrom = "Web";

                lstSQLPara.Add(new SqlParameter("@AddedFrom", objDTO.AddedFrom));
                lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));

                if (objDTO.IsEDIOrder.GetValueOrDefault(false))
                    lstSQLPara.Add(new SqlParameter("@IsEDIOrder", "1"));
                else
                    lstSQLPara.Add(new SqlParameter("@IsEDIOrder", "0"));

                lstSQLPara.Add(new SqlParameter("@OrderGuid", objDTO.GUID));
                lstSQLPara.Add(new SqlParameter("@EditedOnAction", objDTO.EditedOnAction));
                objDTO = ExecuteQuery(strCommand, lstSQLPara.ToArray()).FirstOrDefault();
                objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                objAutoSeqDAL.UpdateNextToolAssetOrderNumber(objDTO.RoomID.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.ToolAssetOrderNumber);

                return objDTO;
            }
            finally
            {
                objAutoSeqDAL = null;
            }
        }
        private ToolAssetOrderMasterDTO DB_UpdateOrderMaster(ToolAssetOrderMasterDTO objDTO)
        {
            string strCommand = "EXEC UpdateToolAssetOrderMaster ";
            strCommand += "" + objDTO.ID + "";
            strCommand += ",'" + objDTO.GUID + "'";
            strCommand += "," + objDTO.LastUpdatedBy.GetValueOrDefault(0) + " ";
            strCommand += "," + objDTO.RoomID.GetValueOrDefault(0) + " ";
            strCommand += "," + objDTO.CompanyID.GetValueOrDefault(0) + " ";
            strCommand += "," + objDTO.OrderStatus + " ";
            strCommand += ",'" + objDTO.RequiredDate.ToString("yyyy-MM-dd") + "' ";
            strCommand += ",'" + objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd") + "' ";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.Comment) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@PackSlipNumber) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF1) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF2) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF3) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF4) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF5) + "'";
            strCommand += "," + objDTO.OrderCost.GetValueOrDefault(0) + " ";
            strCommand += "," + objDTO.NoOfLineItems.GetValueOrDefault(0) + " ";
            if (objDTO.ChangeOrderRevisionNo.GetValueOrDefault(0) > 0)
                strCommand += "," + objDTO.ChangeOrderRevisionNo.GetValueOrDefault(0) + " ";
            else
                strCommand += ",null";

            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.RejectionReason) + "'";

            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.ReleaseNumber) + "'";
            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.ToolAssetOrderNumber) + "'";

            if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                objDTO.WhatWhereAction = "ToolAssetOrder";

            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.WhatWhereAction) + "'";
            strCommand += ",'" + objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

            if (!string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                strCommand += ",'" + objDTO.AddedFrom + "' ";
            else
                strCommand += ",'Web'";
            strCommand += ",'" + objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow) + "' ";

            if (!string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                strCommand += ",'" + objDTO.EditedFrom + "' ";
            else
                strCommand += ",'Web'";

            if (objDTO.IsEDIOrder.GetValueOrDefault(false))
                strCommand += ",1";
            else
                strCommand += ",0";

            if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                objDTO.EditedOnAction = "Update Order Master.";

            strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.WhatWhereAction) + "'";
            objDTO = ExecuteQuery(strCommand).FirstOrDefault();
            return objDTO;
        }
        public ToolAssetOrderMasterDTO DB_TransmitOrder(ToolAssetOrderMasterDTO objDTO)
        {
            string strCommand = @"EXEC TransmitToolAssetOrder @OrderGUID,@RoomID,@CompanyID,@EditedFrom,@WhatWhereAction	";
            List<SqlParameter> lstSQLPara = new List<SqlParameter>();

            lstSQLPara.Add(new SqlParameter("@OrderGUID", objDTO.GUID));
            lstSQLPara.Add(new SqlParameter("@RoomID", objDTO.RoomID.GetValueOrDefault(0)));
            lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)));
            lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));
            lstSQLPara.Add(new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction));

            objDTO = ExecuteQuery(strCommand, lstSQLPara.ToArray()).FirstOrDefault();

            return objDTO;
        }
        private bool DB_DeleteOrderMaster(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strCommand = "EXEC DeleteToolAssetOrderMaster ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomID;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";
                    context.Database.CommandTimeout = 1200;
                    int intReturn = context.Database.SqlQuery<int>(strCommand).FirstOrDefault();

                    if (intReturn > 0)
                        return true;
                }
            }
            return false;

        }
        public IEnumerable<CommonDTO> DB_GetOrderNarrowSearchData(Int64 CompanyID, Int64 RoomID, bool IsArchived, bool IsDeleted, string Status, ToolAssetOrderType OrdType)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC [GetToolAssetOrderMasterNarrowSearchData] ";
                strCommand += CompanyID.ToString();
                strCommand += ", " + RoomID.ToString();
                strCommand += ", " + (IsDeleted ? "1" : "0");
                strCommand += ", " + (IsArchived ? "1" : "0");
                strCommand += ", '" + Status + "'";


                strCommand += ", " + ((int)OrdType).ToString();
                context.Database.CommandTimeout = 120;
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
        private IEnumerable<ToolAssetOrderMasterDTO> ExecuteQuery(string query)
        {
            IEnumerable<ToolAssetOrderMasterDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.CommandTimeout = 120;
                    obj = (from u in context.Database.SqlQuery<ToolAssetOrderMasterDTO>(query)
                           select new ToolAssetOrderMasterDTO
                           {
                               ID = u.ID
                                ,
                               ToolAssetOrderNumber = u.ToolAssetOrderNumber
                                ,
                               ReleaseNumber = u.ReleaseNumber
                                ,
                               RejectionReason = u.RejectionReason
                                ,
                               Comment = u.Comment
                                ,
                               RequiredDate = u.RequiredDate
                                ,
                               OrderStatus = u.OrderStatus
                                ,
                               PackSlipNumber = u.PackSlipNumber
                                ,
                               Created = u.Created
                                ,
                               LastUpdated = u.LastUpdated
                                ,
                               CreatedBy = u.CreatedBy
                                ,
                               LastUpdatedBy = u.LastUpdatedBy
                                ,
                               RoomID = u.RoomID
                                ,
                               IsDeleted = u.IsDeleted
                                ,
                               IsArchived = u.IsArchived
                                ,
                               CompanyID = u.CompanyID
                                ,
                               GUID = u.GUID
                                ,
                               UDF1 = u.UDF1
                                ,
                               UDF2 = u.UDF2
                                ,
                               UDF3 = u.UDF3
                                ,
                               UDF4 = u.UDF4
                                ,
                               UDF5 = u.UDF5
                                ,
                               OrderCost = u.OrderCost
                                ,
                               NoOfLineItems = u.NoOfLineItems
                                ,
                               OrderDate = u.OrderDate
                                ,
                               ChangeOrderRevisionNo = u.ChangeOrderRevisionNo
                                ,
                               WhatWhereAction = u.WhatWhereAction
                                ,
                               OrderNumber_ForSorting = u.OrderNumber_ForSorting
                                ,
                               ReceivedOnWeb = u.ReceivedOnWeb
                                ,
                               ReceivedOn = u.ReceivedOn
                                ,
                               AddedFrom = u.AddedFrom
                                ,
                               EditedFrom = u.EditedFrom
                                ,
                               IsEDIOrder = u.IsEDIOrder
                                ,
                               EditedOnAction = u.EditedOnAction
                                ,
                               CreatedByName = u.CreatedByName
                                ,
                               UpdatedByName = u.UpdatedByName,


                               RoomName = u.RoomName
                                ,
                               OrderIsInReceive = u.OrderIsInReceive
                                ,
                               TotalRecords = u.TotalRecords
                                ,
                               Indicator = u.Indicator
                                ,
                               OMPackSlipNumbers = u.OMPackSlipNumbers,
                               RequiredDateStr = Convert.ToString(u.RequiredDate),
                               Action = string.Empty,
                               HistoryID = Convert.ToInt64(0),
                               IsHistory = false,
                               OrderStatusText = ResToolAssetOrder.GetOrderStatusText(((eTurns.DTO.ToolAssetOrderStatus)u.OrderStatus).ToString()),
                               AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                               OrderType = u.OrderType.GetValueOrDefault(0),
                           }).AsEnumerable<ToolAssetOrderMasterDTO>().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }
        private IEnumerable<ToolAssetOrderMasterDTO> ExecuteQuery(string query, SqlParameter[] sqlParas)
        {
            IEnumerable<ToolAssetOrderMasterDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.CommandTimeout = 120;
                    obj = (from u in context.Database.SqlQuery<ToolAssetOrderMasterDTO>(query, sqlParas)
                           select new ToolAssetOrderMasterDTO
                           {
                               ID = u.ID
                                ,
                               ToolAssetOrderNumber = u.ToolAssetOrderNumber
                                ,
                               ReleaseNumber = u.ReleaseNumber
                                ,
                               RejectionReason = u.RejectionReason
                                ,
                               Comment = u.Comment
                                ,
                               RequiredDate = u.RequiredDate
                                ,
                               OrderStatus = u.OrderStatus
                                ,
                               PackSlipNumber = u.PackSlipNumber
                                ,
                               Created = u.Created
                                ,
                               LastUpdated = u.LastUpdated
                                ,
                               CreatedBy = u.CreatedBy
                                ,
                               LastUpdatedBy = u.LastUpdatedBy
                                ,
                               RoomID = u.RoomID
                                ,
                               IsDeleted = u.IsDeleted
                                ,
                               IsArchived = u.IsArchived
                                ,
                               CompanyID = u.CompanyID
                                ,
                               GUID = u.GUID
                                ,
                               UDF1 = u.UDF1
                                ,
                               UDF2 = u.UDF2
                                ,
                               UDF3 = u.UDF3
                                ,
                               UDF4 = u.UDF4
                                ,
                               UDF5 = u.UDF5
                                ,
                               OrderCost = u.OrderCost
                                ,
                               NoOfLineItems = u.NoOfLineItems
                                ,
                               OrderDate = u.OrderDate
                                ,
                               ChangeOrderRevisionNo = u.ChangeOrderRevisionNo
                                ,
                               WhatWhereAction = u.WhatWhereAction
                                ,
                               OrderNumber_ForSorting = u.OrderNumber_ForSorting
                                ,
                               ReceivedOnWeb = u.ReceivedOnWeb
                                ,
                               ReceivedOn = u.ReceivedOn
                                ,
                               AddedFrom = u.AddedFrom
                                ,
                               EditedFrom = u.EditedFrom
                                ,
                               IsEDIOrder = u.IsEDIOrder
                                ,
                               EditedOnAction = u.EditedOnAction
                                ,
                               CreatedByName = u.CreatedByName
                                ,
                               UpdatedByName = u.UpdatedByName,


                               RoomName = u.RoomName
                                ,
                               OrderIsInReceive = u.OrderIsInReceive
                                ,
                               TotalRecords = u.TotalRecords
                                ,
                               Indicator = u.Indicator
                                ,
                               OMPackSlipNumbers = u.OMPackSlipNumbers,
                               RequiredDateStr = Convert.ToString(u.RequiredDate),
                               Action = string.Empty,
                               HistoryID = 0,
                               IsHistory = false,
                               OrderStatusText = ResToolAssetOrder.GetOrderStatusText(((eTurns.DTO.ToolAssetOrderStatus)u.OrderStatus).ToString()),
                               AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                               OrderType = u.OrderType.GetValueOrDefault(0),
                           }).AsEnumerable<ToolAssetOrderMasterDTO>().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }
        private string ConvertStringForSQLPerameter(string str)
        {
            string strReturn = str;
            if (!string.IsNullOrEmpty(strReturn))
                strReturn = strReturn.Replace("'", "''");

            return strReturn;

        }
        public IEnumerable<ToolAssetOrderMasterDTO> DB_GetChangeOrderData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? OrderID, Guid? OrderGuid, OrderType? OrdType, Guid? ChangeOrderGuid, Int64? ChangeOrderID)
        {
            string strCommand = "EXEC Ord_GetChangeOrderMasterData ";

            if (CompanyID.HasValue)
                strCommand += CompanyID.Value.ToString();
            else
                strCommand += "null";
            if (RoomID.HasValue)
                strCommand += ", " + RoomID.Value.ToString();
            else
                strCommand += ", " + "null";

            if (IsDeleted.HasValue)
                strCommand += ", " + (IsDeleted.Value ? "1" : "0");
            else
                strCommand += ", " + "null";

            if (IsArchived.HasValue)
                strCommand += ", " + (IsArchived.Value ? "1" : "0");
            else
                strCommand += ", " + "null";

            if (OrderID.HasValue)
                strCommand += ", " + OrderID.Value.ToString();
            else
                strCommand += ", " + "null";

            if (OrderGuid.HasValue)
                strCommand += ", '" + OrderGuid.Value.ToString() + "'";
            else
                strCommand += ", " + "null";



            if (OrdType.HasValue)
            {
                strCommand += ", " + Convert.ToString((int)OrdType.Value);
            }
            else
            {
                strCommand += ", " + "null";
            }

            if (ChangeOrderGuid.HasValue)
                strCommand += ", '" + ChangeOrderGuid.Value.ToString() + "'";
            else
                strCommand += ", " + "null";

            if (ChangeOrderID.HasValue)
                strCommand += ", " + ChangeOrderID.Value.ToString();
            else
                strCommand += ", " + "null";

            IEnumerable<ToolAssetOrderMasterDTO> obj = ExecuteQueryChangeOrder(strCommand);
            return obj;

        }
        private IEnumerable<ToolAssetOrderMasterDTO> ExecuteQueryChangeOrder(string query)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 120;
                IEnumerable<ToolAssetOrderMasterDTO> obj = (from u in context.Database.SqlQuery<ToolAssetOrderMasterDTO>(query)
                                                            select new ToolAssetOrderMasterDTO
                                                            {
                                                                ID = u.ID
                                         ,
                                                                ToolAssetOrderNumber = u.ToolAssetOrderNumber
                                         ,
                                                                ReleaseNumber = u.ReleaseNumber
                                         ,
                                                                RejectionReason = u.RejectionReason
                                         ,
                                                                Comment = u.Comment
                                         ,
                                                                RequiredDate = u.RequiredDate
                                         ,
                                                                OrderStatus = u.OrderStatus
                                         ,
                                                                PackSlipNumber = u.PackSlipNumber
                                         ,
                                                                Created = u.Created
                                         ,
                                                                LastUpdated = u.LastUpdated
                                         ,
                                                                CreatedBy = u.CreatedBy
                                         ,
                                                                LastUpdatedBy = u.LastUpdatedBy
                                         ,
                                                                RoomID = u.RoomID
                                         ,
                                                                IsDeleted = u.IsDeleted
                                         ,
                                                                IsArchived = u.IsArchived
                                         ,
                                                                CompanyID = u.CompanyID
                                         ,
                                                                GUID = u.GUID
                                         ,
                                                                UDF1 = u.UDF1
                                         ,
                                                                UDF2 = u.UDF2
                                         ,
                                                                UDF3 = u.UDF3
                                         ,
                                                                UDF4 = u.UDF4
                                         ,
                                                                UDF5 = u.UDF5
                                         ,
                                                                OrderCost = u.OrderCost
                                         ,
                                                                NoOfLineItems = u.NoOfLineItems
                                         ,
                                                                OrderDate = u.OrderDate
                                         ,
                                                                ChangeOrderRevisionNo = u.ChangeOrderRevisionNo
                                         ,
                                                                WhatWhereAction = u.WhatWhereAction
                                         ,
                                                                OrderNumber_ForSorting = u.OrderNumber_ForSorting
                                         ,
                                                                ReceivedOnWeb = u.ReceivedOnWeb
                                         ,
                                                                ReceivedOn = u.ReceivedOn
                                         ,
                                                                AddedFrom = u.AddedFrom
                                         ,
                                                                EditedFrom = u.EditedFrom
                                         ,
                                                                IsEDIOrder = u.IsEDIOrder
                                         ,
                                                                EditedOnAction = u.EditedOnAction
                                         ,
                                                                CreatedByName = u.CreatedByName
                                         ,
                                                                UpdatedByName = u.UpdatedByName,


                                                                RoomName = u.RoomName
                                         ,
                                                                OrderIsInReceive = u.OrderIsInReceive
                                         ,
                                                                TotalRecords = u.TotalRecords
                                         ,
                                                                Indicator = u.Indicator
                                         ,
                                                                OMPackSlipNumbers = u.OMPackSlipNumbers,
                                                                RequiredDateStr = Convert.ToString(u.RequiredDate),
                                                                Action = string.Empty,
                                                                HistoryID = 0,
                                                                IsHistory = false,
                                                                OrderStatusText = ResToolAssetOrder.GetOrderStatusText(((eTurns.DTO.ToolAssetOrderStatus)u.OrderStatus).ToString()),
                                                                AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                                                                OrderType = u.OrderType.GetValueOrDefault(0),
                                                            }).AsParallel().ToList();

                return obj;
            }

        }
        
        
        public IEnumerable<ToolAssetOrderMasterDTO> GetAllReportRecords()
        {
            IEnumerable<ToolAssetOrderMasterDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ToolAssetOrderMasters
                       where u.IsDeleted == false && u.IsArchived == false
                       select new ToolAssetOrderMasterDTO
                       {
                           GUID = u.GUID,
                           ToolAssetOrderNumber = u.ToolAssetOrderNumber,
                           OrderStatus = u.OrderStatus,
                           Created = u.Created,
                           RoomID = u.RoomID,

                           OrderType = u.OrderType,
                           LastUpdated = u.LastUpdated,
                       }).AsParallel().ToList();
            }
            return obj;

        }
        public bool IsOrderNumberDuplicate(string OrderNumber, Int64? OrderId, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int OrderCount = (from x in context.ToolAssetOrderMasters
                                  where x.IsDeleted == false && (x.IsArchived ?? false) == false
                                        && x.ID != OrderId
                                        && x.ToolAssetOrderNumber.Trim().ToUpper() == OrderNumber.Trim().ToUpper()
                                        && x.RoomID == RoomID && x.CompanyID == CompanyID
                                  select x.ID).Count();

                if (OrderCount > 0)
                    return true;

                return false;
            }
        }

        public bool IsOrderNumberDuplicate(string OrderNumber, Guid OrderGUId, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int OrderCount = (from x in context.ToolAssetOrderMasters
                                  where x.IsDeleted == false && (x.IsArchived ?? false) == false
                                        && x.GUID != OrderGUId
                                        && x.ToolAssetOrderNumber.Trim().ToUpper() == OrderNumber.Trim().ToUpper()
                                        && x.RoomID == RoomID && x.CompanyID == CompanyID
                                  select x.ID).Count();

                if (OrderCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        
        public IEnumerable<ToolAssetOrderMasterDTO> GetOrdersByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, int HaveLineItem = 0)
        {
            return DB_GetOrdersByDateRange(StartRowIndex, MaxRows, out TotalCount, RoomID, CompanyID, IsArchived, IsDeleted, FromDate, ToDate, HaveLineItem);
        }

        private IEnumerable<ToolAssetOrderMasterDTO> DB_GetOrdersByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate, int HaveLineItem = 0)
        {

            FromDate = FromDate.Date;
            ToDate = ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            CultureInfo CurrentCult = CultureInfo.CreateSpecificCulture("en-US");
            TimeZoneInfo roomTimeZone = TimeZoneInfo.Utc;
            string RoomDateFormat = "M/d/yyyy";

            eTurnsRegionInfo RegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, 0);
            if (RegionInfo != null)
            {
                RoomDateFormat = RegionInfo.ShortDatePattern;
                roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(RegionInfo.TimeZoneName);
                CurrentCult = CultureInfo.CreateSpecificCulture(RegionInfo.CultureCode); // new CultureInfo(RegionInfo.CultureCode ?? "en-US");
            }

            string StatusQuery = "";
            string NarrowSearchQry = "";
            if (MaxRows < 1)
            {
                MaxRows = 10;
            }
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
            if (FromDate == null || FromDate == DateTime.MinValue)
                FromDate = DateTime.Now;

            if (ToDate == null || ToDate == DateTime.MinValue)
                ToDate = DateTime.Now;

            string CreatedDateFrom = null;
            string CreatedDateTo = null;



            CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(FromDate, DateTimeKind.Unspecified), roomTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
            CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(ToDate, DateTimeKind.Unspecified), roomTimeZone).ToString("dd-MM-yyyy HH:mm:ss");


            NarrowSearchQry += @" (Convert(Date,Created,105) Between Convert(Date,'" + CreatedDateFrom + "',105) AND Convert(Date,'" + CreatedDateTo + "',105)) ";

            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetToolAssetOrderMasterPagedData", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, null, null, NarrowSearchQry, StatusQuery, 1, 0);

            IEnumerable<ToolAssetOrderMasterDTO> obj = null;

            if (ds != null && ds.Tables.Count > 0)
            {
                obj = DataTableHelper.ToList<ToolAssetOrderMasterDTO>(ds.Tables[0]);

                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }
                else
                {
                    TotalCount = 0;
                }
                return obj;
            }
            else
            {
                TotalCount = 0;
                return obj;
            }
        }

        #endregion

    }
}


