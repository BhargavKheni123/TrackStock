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
    public partial class OrderMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public OrderMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public OrderMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region Call DB SPs By following Functions

        public long GetOrderCountByOrderStatus(long RoomId, long CompanyId, int Status)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@Status", Status) };
                return context.Database.SqlQuery<long>("exec [GetOrderCountByOrderStatus] @RoomID,@CompanyID,@Status", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetLatestOrderByRoomIdPlain(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId)};

                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetLatestOrderByRoomIdPlain] @RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public int GetMaximumReleaseNoByOrderNumber(long RoomId, long CompanyId, string OrderNumber, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderNumber", OrderNumber),
                                                   new SqlParameter("@OrderType", (int)OrdType)
                                                };
                var maximumReleaseNo = context.Database.SqlQuery<int?>("exec [GetMaximumReleaseNoByOrderNumber] @RoomID,@CompanyID,@OrderNumber,@OrderType", params1).FirstOrDefault();
                return maximumReleaseNo ?? 0;
            }
        }

        public int GetCountOfOrderByOrderNumber(long RoomId, long CompanyId, string OrderNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@OrderNumber", OrderNumber) };
                return context.Database.SqlQuery<int>("exec [GetCountOfOrderByOrderNumber] @RoomID,@CompanyID,@OrderNumber", params1).FirstOrDefault();
            }
        }

        public IEnumerable<OrderMasterDTO> GetAllOrdersByRoomNormal(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId)};

                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetAllOrdersByRoomNormal] @RoomID,@CompanyID", params1).ToList();
            }
        }

        public IEnumerable<OrderMasterDTO> GetOrdersByOrderTypeAndIdsNormal(long RoomId, long CompanyId, OrderType OrdType, string Ids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderType", (int)OrdType),
                                                   new SqlParameter("@IDs", Ids)
                                                 };

                return context.Database.SqlQuery<OrderMasterDTO>("EXEC [GetOrdersByOrderTypeAndIdsNormal] @RoomID,@CompanyID,@OrderType,@IDs", params1).ToList();
            }
        }

        public IEnumerable<OrderMasterDTO> GetOrderMasterPagedDataNormal(out IEnumerable<OrderMasterDTO> SupplierList, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, List<long> SupplierIds, string SelectedSupplierIds)
        {
            List<OrderMasterDTO> orders = new List<OrderMasterDTO>();
            string StatusQuery = "";
            TotalCount = 0;

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

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
            {
                strSelectedSupplierIds = SelectedSupplierIds;
            }

            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetOrderMasterPagedDataNormal", CompanyID, RoomID, StartRowIndex, MaxRows, sortColumnName, SearchTerm, StatusQuery, strSupplierIds, strSelectedSupplierIds);

            if (ds != null && ds.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);

                if (ds.Tables.Count > 1)
                {
                    orders = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[1]);

                    if (orders != null && orders.Count() > 0)
                    {
                        TotalCount = orders.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
            }

            return orders;
        }

        public IEnumerable<OrderMasterDTO> GetOrderMasterPagedDataNormalForDashboard(out IEnumerable<OrderMasterDTO> SupplierList, int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, OrderType OrdType, string SelectedSupplierIds, int HaveLineItem, List<long> UserSupplierIds)
        {
            string StatusQuery = "";

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

            string strSelectedSupplierIds = string.Empty;

            if (!string.IsNullOrEmpty(SelectedSupplierIds) && !string.IsNullOrWhiteSpace(SelectedSupplierIds))
                strSelectedSupplierIds = SelectedSupplierIds;

            string strUserSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", UserSupplierIds);
            }

            string OrderType = Convert.ToString((int)OrdType);
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetOrderMasterPagedDataNormalForDashboard", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, StatusQuery, strSelectedSupplierIds, OrderType, HaveLineItem, strUserSupplierIds);
            TotalCount = 0;
            List<OrderMasterDTO> orders = new List<OrderMasterDTO>();

            if (ds != null && ds.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);

                if (ds.Tables.Count > 1)
                {
                    orders = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[1]);

                    if (orders != null && orders.Count() > 0)
                    {
                        TotalCount = orders.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
            }

            return orders;
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
        public IEnumerable<OrderMasterDTO> GetPagedOrderMasterDataFull(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, OrderType OrdType, string RoomDateFormat, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, int HaveLineItem = 0)
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
                //DateTime FromdDate =  DateTime.Now;
                //DateTime ToDate = DateTime.Now;
                DateTime FromdDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone);
                DateTime ToDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone);

                if (Fields[0].Split('~')[4] != "")
                {
                    string RDateCondi = string.Empty;

                    if (Fields[0].Split('~')[4].Contains("1"))//  > 3 weeks 
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(21);
                        ToDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(999);

                        //RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                        //            " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";

                        RDateCondi += @"( Convert(Date,RequiredDate) > Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (Fields[0].Split('~')[4].Contains("2"))// 2-3 weeks
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(15);
                        ToDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(21); //FromdDate.AddDays(21);
                        if (!string.IsNullOrWhiteSpace(RDateCondi))
                        {
                            RDateCondi += " Or ";
                        }
                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (Fields[0].Split('~')[4].Contains("3"))// Next weeks
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(8);
                        ToDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone).AddDays(14); // FromdDate.AddDays(14);
                        if (!string.IsNullOrWhiteSpace(RDateCondi))
                        {
                            RDateCondi += " Or ";
                        }
                        RDateCondi += @"( Convert(Date,RequiredDate) Between Convert(Date,'" + FromdDate.ToString("yyyy-MM-dd") + "') " +
                                    " AND Convert(Date,'" + ToDate.ToString("yyyy-MM-dd") + "') )";
                    }

                    if (Fields[0].Split('~')[4].Contains("4"))// This weeks
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentTimeZone);
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
                        UDF1 = UDF1 + HttpUtility.UrlDecode(supitem) + "','";

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
                        UDF2 = UDF2 + HttpUtility.UrlDecode(supitem) + "','";
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
                        UDF3 = UDF3 + HttpUtility.UrlDecode(supitem) + "','";
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
                        UDF4 = UDF4 + HttpUtility.UrlDecode(supitem) + "','";
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
                        UDF5 = UDF5 + HttpUtility.UrlDecode(supitem) + "','";
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[14]))
                {
                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += " EXISTS (SELECT Item FROM dbo.tblSplitString('" + FieldsPara[14] + "', ',', 1, 1) WHERE Item = PackSlipNumber )";
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

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            string OrderType = Convert.ToString((int)OrdType);
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string spName = IsArchived ? "GetOrderMasterPagedData_Archive" : "Ord_GetOrderMasterPagedData";
            ds = SqlHelper.ExecuteDataset(EturnsConnection, spName, CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, NarrowSearchQry, StatusQuery, strSupplierIds, OrderType, HaveLineItem);
            IEnumerable<OrderMasterDTO> obj = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);
            TotalCount = 0;

            if (obj != null && obj.Count() > 0)
            {
                TotalCount = obj.ElementAt(0).TotalRecords;
            }

            return obj;
        }

        public OrderMasterDTO GetOrderByIdPlain(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByIdPlain] @Id", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetOrderByIdNormal(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByIdNormal] @Id", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetOrderByIdFull(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByIdFull] @Id", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetArchivedOrderByIdFull(long Id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id) };
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetArchivedOrderByIdFull] @Id", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetOrderByGuidPlain(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                var order = context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByGuidPlain] @Guid", params1).FirstOrDefault();

                if (order != null)
                {
                    order.OrderStatusText = ResOrder.GetOrderStatusText(((OrderStatus)order.OrderStatus).ToString());
                }

                return order;
            }
        }

        public OrderMasterDTO GetOrderByGuidNormal(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByGuidNormal] @Guid", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetOrderByGuidNormalForChangeOrder(Guid GUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Guid", GUID) };
                var order = context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrderByGuidNormalForChangeOrder] @Guid", params1).FirstOrDefault();

                if (order != null)
                {
                    order.OrderStatusText = ResOrder.GetOrderStatusText(((OrderStatus)order.OrderStatus).ToString());
                }

                return order;
            }
        }

        /// <summary>
        /// Insert Record in the DataBase OrderMaster
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public OrderMasterDTO InsertOrder(OrderMasterDTO objDTO, long SessionUserId)
        {
            return DB_InsertOrder(objDTO, SessionUserId);
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(OrderMasterDTO objDTO)
        {
            DB_UpdateOrderMaster(objDTO);
            return true;
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteOrdersByOrderIds(string IDs, long userid, long RoomID, long CompanyID, long SessionUserId, long EnterpriceId)
        {
            return DB_DeleteOrderMaster(IDs, userid, RoomID, CompanyID, SessionUserId, EnterpriceId);
        }

        #endregion

        public IEnumerable<OrderMasterDTO> GetPendingOrderListNormal(long RoomId, long CompanyId, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompnayID", CompanyId),
                                                   new SqlParameter("@OrderType", (int)OrdType)
                                                };

                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetPendingOrderListNormal] @RoomID,@CompnayID,@OrderType", params1).ToList();
            }
        }

        public IEnumerable<OrderMasterDTO> GetPendingOrderListBySupplierIdsNormal(long RoomId, long CompanyId, OrderType OrdType, string SupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompnayID", CompanyId),
                                                   new SqlParameter("@OrderType", (int)OrdType),
                                                   new SqlParameter("@SupplierIds", SupplierIds)
                };

                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetPendingOrderListBySupplierIdsNormal] @RoomID,@CompnayID,@OrderType,@SupplierIds", params1).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderMasterDTO GetOrderHistoryByHistoryIDFull(long HistoryID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderMasterDTO>("exec GetOrderHistoryByHistoryIDFull @HistoryID", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO GetOrderHistoryByHistoryIDNormal(long HistoryID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@HistoryID", HistoryID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderMasterDTO>("EXEC GetOrderHistoryByHistoryIDNormal @HistoryID", params1).FirstOrDefault();
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
        public IEnumerable<OrderMasterDTO> GetOrderedPagedRecordsByItemGuidNormal(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, Guid ItemGUID, int OrderStatus, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@StartRowIndex", StartRowIndex),
                                                   new SqlParameter("@MaxRows", MaxRows),
                                                   new SqlParameter("@SearchTerm", SearchTerm ?? (object)DBNull.Value),
                                                   new SqlParameter("@sortColumnName", sortColumnName ?? (object)DBNull.Value),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID),
                                                   new SqlParameter("@OrderType", (int)OrdType),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@OrderStatus", OrderStatus),
                };

                var orders = context.Database.SqlQuery<OrderMasterDTO>("EXEC [GetOrderedPagedRecordsByItemGuidNormal] @StartRowIndex,@MaxRows,@SearchTerm,@sortColumnName,@RoomID,@CompanyID,@OrderType,@ItemGUID,@OrderStatus", params1).ToList();
                TotalCount = 0;

                if (orders != null && orders.Count > 0)
                {
                    orders.ForEach(z => z.OrderStatusText = ResOrder.GetOrderStatusText(((OrderStatus)z.OrderStatus).ToString()));
                    TotalCount = orders.First().TotalRecords;
                }

                return orders;
            }
        }

        public void UpdateOrderComment(string comment, string PackslipNumber, string ShipmentTrackingNumber, long OrderId, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderMaster obj = context.OrderMasters.FirstOrDefault(x => x.ID == OrderId);
                obj.Comment = comment;
                obj.ShippingTrackNumber = ShipmentTrackingNumber;
                obj.PackSlipNumber = PackslipNumber;
                obj.LastUpdatedBy = UserID;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.SaveChanges();
            }
        }
        #region DB Section

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

        private object DBNullValueorInt64IfNotNull(Int64? value)
        {
            object o;
            if (!value.HasValue)
            {
                o = DBNull.Value;
            }
            else
            {
                o = value;
            }
            return o;
        }

        /// <summary>
        /// Insert Order Master
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private OrderMasterDTO DB_InsertOrder(OrderMasterDTO objDTO, long SessionUserId)
        {
            AutoSequenceDAL objAutoSeqDAL = null;
            try
            {
                List<SqlParameter> lstSQLPara = new List<SqlParameter>();
                lstSQLPara.Add(new SqlParameter("@OrderNumber", objDTO.OrderNumber));
                lstSQLPara.Add(new SqlParameter("@SalesOrderNumber", DBNullValueorStringIfNotNull(objDTO.SalesOrderNumber)));
                lstSQLPara.Add(new SqlParameter("@ReleaseNumber", objDTO.ReleaseNumber));
                lstSQLPara.Add(new SqlParameter("@Supplier", objDTO.Supplier.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@CreatedBy", objDTO.CreatedBy.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@Room", objDTO.Room.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@OrderStatus", objDTO.OrderStatus));
                lstSQLPara.Add(new SqlParameter("@StagingID", DBNullValueorInt64IfNotNull(objDTO.StagingID)));
                lstSQLPara.Add(new SqlParameter("@CustomerID", DBNullValueorInt64IfNotNull(objDTO.CustomerID)));
                lstSQLPara.Add(new SqlParameter("@ShipVia", DBNullValueorInt64IfNotNull(objDTO.ShipVia)));

                if (objDTO.OrderType.GetValueOrDefault(0) > 0)
                    lstSQLPara.Add(new SqlParameter("@OrderType", objDTO.OrderType));
                else
                    lstSQLPara.Add(new SqlParameter("@OrderType", DBNull.Value));

                lstSQLPara.Add(new SqlParameter("@ShippingVendor", DBNullValueorInt64IfNotNull(objDTO.ShippingVendor)));
                lstSQLPara.Add(new SqlParameter("@RequiredDate", objDTO.RequiredDate.ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@OrderDate", objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@Comment", DBNullValueorStringIfNotNull(objDTO.Comment)));
                lstSQLPara.Add(new SqlParameter("@PackSlipNumber", DBNullValueorStringIfNotNull(objDTO.PackSlipNumber)));
                lstSQLPara.Add(new SqlParameter("@ShippingTrackNumber", DBNullValueorStringIfNotNull(objDTO.ShippingTrackNumber)));
                lstSQLPara.Add(new SqlParameter("@UDF1", DBNullValueorStringIfNotNull(objDTO.UDF1)));
                lstSQLPara.Add(new SqlParameter("@UDF2", DBNullValueorStringIfNotNull(objDTO.UDF2)));
                lstSQLPara.Add(new SqlParameter("@UDF3", DBNullValueorStringIfNotNull(objDTO.UDF3)));
                lstSQLPara.Add(new SqlParameter("@UDF4", DBNullValueorStringIfNotNull(objDTO.UDF4)));
                lstSQLPara.Add(new SqlParameter("@UDF5", DBNullValueorStringIfNotNull(objDTO.UDF5)));
                lstSQLPara.Add(new SqlParameter("@CustomerAddress", DBNullValueorStringIfNotNull(objDTO.CustomerAddress)));
                lstSQLPara.Add(new SqlParameter("@AccountNumber", DBNullValueorStringIfNotNull(objDTO.AccountNumber)));
                lstSQLPara.Add(new SqlParameter("@BlanketOrderNumberID", DBNullValueorInt64IfNotNull(objDTO.BlanketOrderNumberID)));
                lstSQLPara.Add(new SqlParameter("@WhatWhereAction", DBNullValueorStringIfNotNull(objDTO.WhatWhereAction)));

                if (string.IsNullOrEmpty(objDTO.OrderNumber_ForSorting))
                    objDTO.OrderNumber_ForSorting = objDTO.OrderNumber;

                lstSQLPara.Add(new SqlParameter("@OrderNumber_ForSorting", DBNullValueorStringIfNotNull(CommonDAL.GetSortingString(objDTO.OrderNumber.Replace("'", "")))));

                if (objDTO.MaterialStagingGUID != null)
                    lstSQLPara.Add(new SqlParameter("@MaterialStagingGUID", objDTO.MaterialStagingGUID.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@MaterialStagingGUID", DBNull.Value));

                if (objDTO.CustomerGUID != null)
                    lstSQLPara.Add(new SqlParameter("@CustomerGUID", objDTO.CustomerGUID.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@CustomerGUID", DBNull.Value));

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

                if (objDTO.SupplierAccountGuid != null)
                    lstSQLPara.Add(new SqlParameter("@SupplierAccountGuid", objDTO.SupplierAccountGuid.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@SupplierAccountGuid", DBNull.Value));

                if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@RequesterID", objDTO.RequesterID.Value));
                }
                else
                    lstSQLPara.Add(new SqlParameter("@RequesterID", DBNull.Value));

                if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@ApproverID", objDTO.ApproverID.Value));
                }
                else
                    lstSQLPara.Add(new SqlParameter("@ApproverID", DBNull.Value));

                lstSQLPara.Add(new SqlParameter("@SalesOrder", DBNullValueorStringIfNotNull(objDTO.SalesOrder)));
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objDTO = context.Database.SqlQuery<OrderMasterDTO>("EXEC [Ord_InsertOrderMaster] @OrderNumber,@SalesOrderNumber,@ReleaseNumber,@Supplier,@CreatedBy,@Room,@CompanyID,@OrderStatus,@StagingID,@CustomerID,@ShipVia,@OrderType,@ShippingVendor,@RequiredDate,@OrderDate,@Comment,@PackSlipNumber,@ShippingTrackNumber,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@CustomerAddress,@AccountNumber,@BlanketOrderNumberID ,@WhatWhereAction,@OrderNumber_ForSorting,@MaterialStagingGUID,@CustomerGUID,@ReceivedOn,@ReceivedOnWeb,@AddedFrom,@EditedFrom,@IsEDIOrder,@OrderGuid,@SupplierAccountGuid,@RequesterID,@ApproverID,@SalesOrder", lstSQLPara.ToArray()).FirstOrDefault();
                }

                objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                objAutoSeqDAL.UpdateNextOrderNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Supplier.GetValueOrDefault(0), objDTO.OrderNumber, SessionUserId, objDTO.ReleaseNumber);

                return objDTO;
            }
            finally
            {
                objAutoSeqDAL = null;
            }
        }

        /// <summary>
        /// Insert Order Master
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private OrderMasterDTO DB_UpdateOrderMaster(OrderMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var OrderPrice = 0;

                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Order";

                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    objDTO.EditedFrom = "Web";

                List<SqlParameter> lstSQLPara = new List<SqlParameter>();
                lstSQLPara.Add(new SqlParameter("@ID", objDTO.ID));
                lstSQLPara.Add(new SqlParameter("@GUID", objDTO.GUID));
                lstSQLPara.Add(new SqlParameter("@Supplier", objDTO.Supplier.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@LastUpdatedBy", objDTO.LastUpdatedBy.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@Room", objDTO.Room.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@OrderStatus", objDTO.OrderStatus));
                lstSQLPara.Add(new SqlParameter("@StagingID", DBNullValueorInt64IfNotNull(objDTO.StagingID)));
                lstSQLPara.Add(new SqlParameter("@CustomerID", DBNullValueorInt64IfNotNull(objDTO.CustomerID)));
                lstSQLPara.Add(new SqlParameter("@ShipVia", DBNullValueorInt64IfNotNull(objDTO.ShipVia)));
                lstSQLPara.Add(new SqlParameter("@ShippingVendor", DBNullValueorInt64IfNotNull(objDTO.ShippingVendor)));
                lstSQLPara.Add(new SqlParameter("@RequiredDate", objDTO.RequiredDate.ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@OrderDate", objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd")));
                lstSQLPara.Add(new SqlParameter("@Comment", DBNullValueorStringIfNotNull(objDTO.Comment)));
                lstSQLPara.Add(new SqlParameter("@PackSlipNumber", DBNullValueorStringIfNotNull(objDTO.PackSlipNumber)));
                lstSQLPara.Add(new SqlParameter("@ShippingTrackNumber", DBNullValueorStringIfNotNull(objDTO.ShippingTrackNumber)));
                lstSQLPara.Add(new SqlParameter("@UDF1", DBNullValueorStringIfNotNull(objDTO.UDF1)));
                lstSQLPara.Add(new SqlParameter("@UDF2", DBNullValueorStringIfNotNull(objDTO.UDF2)));
                lstSQLPara.Add(new SqlParameter("@UDF3", DBNullValueorStringIfNotNull(objDTO.UDF3)));
                lstSQLPara.Add(new SqlParameter("@UDF4", DBNullValueorStringIfNotNull(objDTO.UDF4)));
                lstSQLPara.Add(new SqlParameter("@UDF5", DBNullValueorStringIfNotNull(objDTO.UDF5)));
                lstSQLPara.Add(new SqlParameter("@CustomerAddress", DBNullValueorStringIfNotNull(objDTO.CustomerAddress)));
                lstSQLPara.Add(new SqlParameter("@AccountNumber", DBNullValueorStringIfNotNull(objDTO.AccountNumber)));
                lstSQLPara.Add(new SqlParameter("@OrderCost", objDTO.OrderCost.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@NoOfLineItems", objDTO.NoOfLineItems.GetValueOrDefault(0)));
                lstSQLPara.Add(new SqlParameter("@ChangeOrderRevisionNo", DBNullValueorInt64IfNotNull(objDTO.ChangeOrderRevisionNo)));
                lstSQLPara.Add(new SqlParameter("@RejectionReason", DBNullValueorStringIfNotNull(objDTO.RejectionReason)));
                lstSQLPara.Add(new SqlParameter("@BlanketOrderNumberID", DBNullValueorInt64IfNotNull(objDTO.BlanketOrderNumberID)));
                lstSQLPara.Add(new SqlParameter("@ReleaseNumber", objDTO.ReleaseNumber));
                lstSQLPara.Add(new SqlParameter("@OrderNumber", objDTO.OrderNumber));
                lstSQLPara.Add(new SqlParameter("@WhatWhereAction", DBNullValueorStringIfNotNull(objDTO.WhatWhereAction)));
                lstSQLPara.Add(new SqlParameter("@SalesOrderNumber", DBNullValueorStringIfNotNull(objDTO.SalesOrderNumber)));

                if (objDTO.MaterialStagingGUID != null)
                    lstSQLPara.Add(new SqlParameter("@MaterialStagingGUID", objDTO.MaterialStagingGUID.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@MaterialStagingGUID", DBNull.Value));

                if (objDTO.CustomerGUID != null)
                    lstSQLPara.Add(new SqlParameter("@CustomerGUID", objDTO.CustomerGUID.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@CustomerGUID", DBNull.Value));

                lstSQLPara.Add(new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)));
                lstSQLPara.Add(new SqlParameter("@EditedFrom", objDTO.EditedFrom));

                if (objDTO.IsEDIOrder.GetValueOrDefault(false))
                    lstSQLPara.Add(new SqlParameter("@IsEDIOrder", "1"));
                else
                    lstSQLPara.Add(new SqlParameter("@IsEDIOrder", "0"));

                if (objDTO.SupplierAccountGuid != null)
                    lstSQLPara.Add(new SqlParameter("@SupplierAccountGuid", objDTO.SupplierAccountGuid.Value));
                else
                    lstSQLPara.Add(new SqlParameter("@SupplierAccountGuid", DBNull.Value));

                lstSQLPara.Add(new SqlParameter("@OrderPrice", OrderPrice));

                if (objDTO.RequesterID.GetValueOrDefault(0) > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@RequesterID", objDTO.RequesterID.Value));
                }
                else
                    lstSQLPara.Add(new SqlParameter("@RequesterID", DBNull.Value));

                if (objDTO.ApproverID.GetValueOrDefault(0) > 0)
                {
                    lstSQLPara.Add(new SqlParameter("@ApproverID", objDTO.ApproverID.Value));
                }
                else
                    lstSQLPara.Add(new SqlParameter("@ApproverID", DBNull.Value));
                lstSQLPara.Add(new SqlParameter("@SalesOrder", DBNullValueorStringIfNotNull(objDTO.SalesOrder)));

                //objDTO = context.Database.SqlQuery<OrderMasterDTO>("EXEC [Ord_UpdateOrderMaster] @ID,@GUID,@Supplier,@LastUpdatedBy,@Room,@CompanyID,@OrderStatus,@StagingID,@CustomerID,@ShipVia,@ShippingVendor,@RequiredDate,@OrderDate,@Comment,@PackSlipNumber,@ShippingTrackNumber,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@CustomerAddress,@AccountNumber,@OrderCost,@NoOfLineItems,@ChangeOrderRevisionNo,@RejectionReason,@BlanketOrderNumberID,@ReleaseNumber,@OrderNumber,@WhatWhereAction,@SalesOrderNumber,@MaterialStagingGUID,@CustomerGUID,@ReceivedOn,@EditedFrom,@IsEDIOrder,@SupplierAccountGuid,@OrderPrice,@RequesterID,@ApproverID,@SalesOrder", lstSQLPara.ToArray()).FirstOrDefault();
                var sqlparams = lstSQLPara.ToArray();
                string strCommand = "EXEC [Ord_UpdateOrderMaster] @ID,@GUID,@Supplier,@LastUpdatedBy,@Room,@CompanyID,@OrderStatus,@StagingID,@CustomerID,@ShipVia,@ShippingVendor,@RequiredDate,@OrderDate,@Comment,@PackSlipNumber,@ShippingTrackNumber,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@CustomerAddress,@AccountNumber,@OrderCost,@NoOfLineItems,@ChangeOrderRevisionNo,@RejectionReason,@BlanketOrderNumberID,@ReleaseNumber,@OrderNumber,@WhatWhereAction,@SalesOrderNumber,@MaterialStagingGUID,@CustomerGUID,@ReceivedOn,@EditedFrom,@IsEDIOrder,@SupplierAccountGuid,@OrderPrice,@RequesterID,@ApproverID,@SalesOrder";
                context.Database.ExecuteSqlCommand(strCommand, sqlparams);
                return null;
            }

        }

        public OrderMasterDTO DB_TransmitOrder(OrderMasterDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderGUID", objDTO.GUID),
                                                   new SqlParameter("@RoomID", objDTO.Room.GetValueOrDefault(0)),
                                                   new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)),
                                                   new SqlParameter("@EditedFrom", objDTO.EditedFrom),
                                                   new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction)
                                                 };
                return context.Database.SqlQuery<OrderMasterDTO>("EXEC [Ord_TransmitOrder] @OrderGUID,@RoomID,@CompanyID,@EditedFrom,@WhatWhereAction", params1).FirstOrDefault();
            }
        }

        public OrderMasterDTO CheckOrderExistAndValid(string OrderNumber, string releaseno, long RoomID, long CompanyID, int OrderType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (!string.IsNullOrEmpty(OrderNumber) && !string.IsNullOrEmpty(releaseno))
                {
                    var obj = (from x in context.OrderMasters
                               where x.OrderNumber == OrderNumber.Trim()
                                 && x.ReleaseNumber == releaseno.Trim()
                                 && x.IsDeleted == false
                                 && (x.IsArchived ?? false) == false
                                 && x.Room == RoomID
                                 && x.CompanyID == CompanyID
                                 && x.OrderType == OrderType
                               select new OrderMasterDTO
                               {
                                   OrderNumber = x.OrderNumber,
                                   CompanyID = x.CompanyID,
                                   Room = x.Room,
                                   ReleaseNumber = x.ReleaseNumber,
                                   GUID = x.GUID,
                                   OrderStatus = x.OrderStatus,
                                   OrderType = x.OrderType,
                                   ID = x.ID,
                                   ShippingTrackNumber = x.ShippingTrackNumber
                               }).FirstOrDefault();

                    return obj;
                }
            }
            return null;
        }

        public IEnumerable<OrderMasterDTO> GetOrdersByIdsPlain(string Ids)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Ids", Ids) };

                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrdersByIdsPlain] @Ids", params1).ToList();
            }
        }

        /// <summary>
        /// Insert Order Master
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private bool DB_DeleteOrderMaster(string IDs, long userid, long RoomID, long CompanyID, long SessionUserId, long EnterpriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@IDs", strIDs),
                                                       new SqlParameter("@UserID", userid),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@CompanyID", CompanyID),
                                                       new SqlParameter("@ReceivedOn", DateTimeUtility.DateTimeNow),
                                                       new SqlParameter("@EditedFrom", "Web"),
                                                     };
                    int intReturn = context.Database.SqlQuery<int>("exec [Ord_DeleteOrderMaster] @IDs,@UserID,@RoomID,@CompanyID,@ReceivedOn,@EditedFrom", params1).FirstOrDefault(); ;
                    DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
                    ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                    var orders = GetOrdersByIdsPlain(strIDs);

                    foreach (var item in orders)
                    {
                        List<OrderDetailsDTO> lstDetails = new OrderDetailsDAL(base.DataBaseName).GetOrderDetailByOrderGUIDPlain(item.GUID, RoomID, CompanyID);

                        foreach (var detailitem in lstDetails)
                        {
                            try
                            {
                                DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(RoomID, CompanyID, detailitem.ItemGUID.GetValueOrDefault(Guid.Empty), item.LastUpdatedBy ?? 0, null, null);
                                DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(RoomID, CompanyID, detailitem.ItemGUID.GetValueOrDefault(Guid.Empty), item.LastUpdatedBy ?? 0, SessionUserId, null, null);
                            }
                            catch
                            {
                            }

                            ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, detailitem.ItemGUID.GetValueOrDefault(Guid.Empty));
                            objItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            objItemDTO.IsOnlyFromItemUI = false;
                            itmDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);
                        }
                    }

                    if (intReturn > 0)
                        return true;
                }
            }
            return false;

        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        public IEnumerable<CommonDTO> DB_GetOrderNarrowSearchData(long CompanyID, long RoomID, bool IsArchived, bool IsDeleted, string Status, List<long> SupplierIds, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }
                var sqlParams = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyID),
                                                       new SqlParameter("@RoomID", RoomID),
                                                       new SqlParameter("@IsDeleted", IsDeleted),
                                                       new SqlParameter("@IsArchived", IsArchived),
                                                       new SqlParameter("@StatusQry", Status),
                                                       new SqlParameter("@SupplierIds", strSupplierIds),
                                                       new SqlParameter("@OrderType", (int)OrdType),
                                                     };

                return context.Database.SqlQuery<CommonDTO>("EXEC [Ord_GetOrderMasterNarrowSearchData] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@StatusQry,@SupplierIds,@OrderType", sqlParams).ToList();
            }
        }

        public IEnumerable<OrderMasterDTO> GetChangeOrderDataNormal(long CompanyId, long RoomId, Guid OrderGuid, List<long> SupplierIds, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }
                var sqlParams = new SqlParameter[] {   new SqlParameter("@CompnayID", CompanyId),
                                                       new SqlParameter("@RoomID", RoomId),
                                                       new SqlParameter("@OrderGUID", OrderGuid),
                                                       new SqlParameter("@OrderType", (int)OrdType),
                                                       new SqlParameter("@SupplierIds", strSupplierIds)
                                                   };

                var changeOrders = context.Database.SqlQuery<OrderMasterDTO>("EXEC [GetChangeOrderDataNormal] @CompnayID,@RoomID,@OrderGUID,@OrderType,@SupplierIds", sqlParams).ToList();

                if (changeOrders != null && changeOrders.Count > 0)
                {
                    changeOrders.ForEach(z => z.OrderStatusText = ResOrder.GetOrderStatusText(((OrderStatus)z.OrderStatus).ToString()));
                }
                return changeOrders;
            }
        }

        public OrderMasterDTO GetChangeOrderDataPlain(long CompanyId, long RoomId, List<long> SupplierIds, Guid ChangeOrderGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }
                var sqlParams = new SqlParameter[] {   new SqlParameter("@CompnayID", CompanyId),
                                                       new SqlParameter("@RoomID", RoomId),
                                                       new SqlParameter("@ChangeOrderGuid", ChangeOrderGuid),
                                                       new SqlParameter("@SupplierIds", strSupplierIds)
                                                   };

                return context.Database.SqlQuery<OrderMasterDTO>("EXEC [GetChangeOrderDataPlain] @CompnayID,@RoomID,@ChangeOrderGuid,@SupplierIds", sqlParams).FirstOrDefault();
            }
        }

        /// <summary>
        /// Update Order Status Nightly
        /// </summary>
        public void UpdateOrderStatusNightly(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.CommandTimeout = 120;
                //string query = "EXEC ORD_UpdateOrderStatus_Nigthly " + RoomID + "," + CompanyID + "";
                //context.Database.SqlQuery<string>(query);

                var sqlParams = new SqlParameter[] {    new SqlParameter("@RoomID", RoomID),
                                                        new SqlParameter("@CompnayID", CompanyID)
                                                   };

                string retVal = context.Database.SqlQuery<string>("EXEC [ORD_UpdateOrderStatus_Nigthly] @RoomID,@CompnayID", sqlParams).FirstOrDefault();
            }
        }

        #endregion

        public IEnumerable<KeyValDTO> GetClosedOrdersForReportByRoomAndSupplier(long RoomId, List<long> SupplierIds, bool IsApplyDateFilter, string StartDate, string EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var sqlParams = new SqlParameter[] {   new SqlParameter("@RoomId", RoomId),
                                                       new SqlParameter("@SupplierIds", strSupplierIds),
                                                       new SqlParameter("@IsApplyDateFilter", IsApplyDateFilter),
                                                       new SqlParameter("@StartDate", StartDate),
                                                       new SqlParameter("@EndDate", EndDate)
                                                   };

                return context.Database.SqlQuery<KeyValDTO>("EXEC [GetClosedOrdersForReportByRoomAndSupplier] @RoomId,@SupplierIds,@IsApplyDateFilter,@StartDate,@EndDate", sqlParams).ToList();
            }
        }

        public IEnumerable<KeyValDTO> GetClosedOrdersForReportByRoom(long RoomId, bool IsApplyDateFilter, string StartDate, string EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {   new SqlParameter("@RoomId", RoomId),
                                                       new SqlParameter("@IsApplyDateFilter", IsApplyDateFilter),
                                                       new SqlParameter("@StartDate", StartDate),
                                                       new SqlParameter("@EndDate", EndDate)
                                                   };

                return context.Database.SqlQuery<KeyValDTO>("EXEC [GetClosedOrdersForReportByRoom] @RoomId,@IsApplyDateFilter,@StartDate,@EndDate", sqlParams).ToList();
            }
        }

        public IEnumerable<KeyValDTO> GetReturnOrdersForReportByRoomAndSupplier(long RoomId, string OrderStatuses, List<long> SupplierIds, bool IsApplyDateFilter, string StartDate, string EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var sqlParams = new SqlParameter[] {   new SqlParameter("@RoomId", RoomId),
                                                       new SqlParameter("@OrderStatuses", OrderStatuses),
                                                       new SqlParameter("@SupplierIds", strSupplierIds),
                                                       new SqlParameter("@IsApplyDateFilter", IsApplyDateFilter),
                                                       new SqlParameter("@StartDate", StartDate),
                                                       new SqlParameter("@EndDate", EndDate)
                                                   };

                return context.Database.SqlQuery<KeyValDTO>("EXEC [GetReturnOrdersForReportByRoomAndSupplier] @RoomId,@OrderStatuses,@SupplierIds,@IsApplyDateFilter,@StartDate,@EndDate", sqlParams).ToList();
            }
        }

        public IEnumerable<KeyValDTO> GetReturnOrdersForReportByRoom(long RoomId, string OrderStatuses, bool IsApplyDateFilter, string StartDate, string EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var sqlParams = new SqlParameter[] {   new SqlParameter("@RoomId", RoomId),
                                                       new SqlParameter("@OrderStatuses", OrderStatuses),
                                                       new SqlParameter("@IsApplyDateFilter", IsApplyDateFilter),
                                                       new SqlParameter("@StartDate", StartDate),
                                                       new SqlParameter("@EndDate", EndDate)
                                                   };

                return context.Database.SqlQuery<KeyValDTO>("EXEC [GetReturnOrdersForReportByRoom] @RoomId,@OrderStatuses,@IsApplyDateFilter,@StartDate,@EndDate", sqlParams).ToList();
            }
        }


        public void UpdateOrderStatus(Guid? OrderGUID, long? ID, int OrderStatus)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderMaster ORD = context.OrderMasters.FirstOrDefault(t => t.ID == ID || t.GUID == OrderGUID);

                if (ORD != null)
                {
                    ORD.OrderStatus = OrderStatus;
                    context.SaveChanges();
                }
            }
        }

        public void UpdateOrderStatusbyGUID(Guid? OrderGUID, int OrderStatus)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderMaster ORD = context.OrderMasters.FirstOrDefault(t => t.GUID == OrderGUID);

                if (ORD != null)
                {
                    ORD.OrderStatus = OrderStatus;
                    context.SaveChanges();
                }
            }
        }

        public bool IsOrderNumberDuplicateById(string OrderNumber, long OrderId, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int OrderCount = (from x in context.OrderMasters
                                  where x.IsDeleted == false && (x.IsArchived ?? false) == false
                                        && x.ID != OrderId
                                        && x.OrderNumber.Trim().ToUpper() == OrderNumber.Trim().ToUpper()
                                        && x.Room == RoomID && x.CompanyID == CompanyID
                                  select x.ID).Count();

                return (OrderCount > 0);
            }
        }
        public bool IsOrderReleaseDuplicate(string OrderNumber, string ReleaseNo, long OrderId, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int OrderCount = (from x in context.OrderMasters
                                  where x.IsDeleted == false && (x.IsArchived ?? false) == false
                                        && x.ID != OrderId
                                        && x.OrderNumber.Trim().ToUpper() == OrderNumber.Trim().ToUpper()
                                        && x.Room == RoomID && x.CompanyID == CompanyID && x.ReleaseNumber.ToUpper() == ReleaseNo.ToUpper()
                                  select x.ID).Count();

                return (OrderCount > 0);
            }
        }

        public bool IsOrderNumberDuplicateByGuid(string OrderNumber, Guid OrderGUId, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int OrderCount = (from x in context.OrderMasters
                                  where x.IsDeleted == false && (x.IsArchived ?? false) == false
                                        && x.GUID != OrderGUId
                                        && x.OrderNumber.Trim().ToUpper() == OrderNumber.Trim().ToUpper()
                                        && x.Room == RoomID && x.CompanyID == CompanyID
                                  select x.ID).Count();

                return (OrderCount > 0);
            }
        }

        public int GetNextReleaseNumber(string OrderNumber, Guid? OrderGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@OrderNumber", OrderNumber ?? (object)DBNull.Value),
                                                new SqlParameter("@OrderGUID", OrderGUID ?? (object)DBNull.Value),
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("EXEC [dbo].[csp_GenerateAndGetOrderReleaseNumber] @OrderNumber,@OrderGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public List<DashboardBottomAndTopSpendDTO> GetDashboardTopAndBottomSpend(int StartRowIndex, int MaxRows, out int TotalRecords, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, int MaxItemsInGraph, string TabName)
        {
            List<eTurns.DTO.DashboardBottomAndTopSpendDTO> lstBTSpend = new List<eTurns.DTO.DashboardBottomAndTopSpendDTO>();
            TotalRecords = 0;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet ds = new DataSet();
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "Ord_GetDashboardTopAndBottomSpend", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, MaxItemsInGraph, TabName);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                lstBTSpend = (from so in dt.AsEnumerable()
                              select new DashboardBottomAndTopSpendDTO
                              {
                                  ItemNumber = so.Field<string>("ItemNumber"),
                                  TotalRecords = so.Field<int>("TotalRecords"),
                                  ItemGUID = so.Field<Guid>("ItemGUID"),
                                  OrderCost = so.Field<double>("OrderCost"),
                                  SupplierName = so.Field<string>("SupplierName"),
                                  SupplierID = so.Field<long>("SupplierID")
                              }).ToList();
            }
            return lstBTSpend;
        }

        public List<DashboardBottomAndTopSpendDTO> GetDashboardTopAndBottomSpendForGrid(int StartRowIndex, int MaxRows, out int TotalRecords, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, string TabName, string SupplierIds, out IEnumerable<DashboardBottomAndTopSpendDTO> SupplierList, List<long> UserSupplierIds)
        {
            string strSupplier = string.Empty;
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            if (!string.IsNullOrEmpty(SupplierIds) && !string.IsNullOrWhiteSpace(SupplierIds))
                strSupplier = SupplierIds;

            string strUserSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", UserSupplierIds);
            }

            List<DashboardBottomAndTopSpendDTO> lstBTSpend = new List<DashboardBottomAndTopSpendDTO>();
            TotalRecords = 0;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            DataSet ds = new DataSet();
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetDashboardTopAndBottomSpendForGrid", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomID, CompanyID, TabName, strSupplier, strUserSupplierIds);
            TotalRecords = 0;

            if (ds != null && ds.Tables.Count > 0)
            {
                SupplierList = DataTableHelper.ToList<DashboardBottomAndTopSpendDTO>(ds.Tables[0]);

                if (ds.Tables.Count > 1)
                {
                    lstBTSpend = DataTableHelper.ToList<DashboardBottomAndTopSpendDTO>(ds.Tables[1]);

                    if (lstBTSpend != null && lstBTSpend.Count() > 0)
                    {
                        TotalRecords = lstBTSpend.ElementAt(0).TotalRecords;
                    }
                }
            }
            else
            {
                SupplierList = null;
            }

            return lstBTSpend;
        }

        #region Get Order record by Date Range

        public IEnumerable<OrderMasterDTO> GetOrdersByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate)
        {
            return DB_GetOrdersByDateRange(StartRowIndex, MaxRows, out TotalCount, RoomID, CompanyID, IsArchived, IsDeleted, FromDate, ToDate);
        }

        private IEnumerable<OrderMasterDTO> DB_GetOrdersByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, DateTime FromDate, DateTime ToDate)
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
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "Ord_GetOrderMasterPagedData", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, null, null, NarrowSearchQry, StatusQuery, null, 1, 0);
            IEnumerable<OrderMasterDTO> obj = null;

            if (ds != null && ds.Tables.Count > 0)
            {
                obj = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);
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

        /// <summary>
        /// This method is used to get the previous status of the order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public int? GetOrderPreviousStatus(long OrderId)
        {
            int? orderStatus = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@OrderID", OrderId) };
                orderStatus = context.Database.SqlQuery<int?>("EXEC [dbo].[GetOrderPreviousStatus] @OrderID", params1).FirstOrDefault();
            }

            return orderStatus;
        }
        public List<OrderMasterDTO> GetOrdersMasterChangeLog(string IDs)
        {
            IDs = IDs.Replace(",", "");
            var params1 = new SqlParameter[] { new SqlParameter("@ID", IDs) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrdersMasterChangeLog] @ID", params1).ToList();
            }
        }

        public void Ord_UpdateItemCostBasedonOrderDetailCost(long UserID, string EditedFrom, long RoomID, long CompanyID, DataTable DT)
        {
            try
            {
                SqlConnection ChildDbConnection = new SqlConnection(base.DataBaseConnectionString);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "Ord_UpdateItemCostBasedonOrderDetailCost", UserID, EditedFrom, RoomID, CompanyID, DT);
            }
            catch
            {

            }
        }
        public List<(Guid ItemGUID, string ItemNumber, bool IsValid)> ValidateOrderItemsQtyOnSupplierBlanketPO(Guid OrderGUID, List<OrderDetailsDTO> lstOfItems, long RoomID, long CompanyID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var itemTable = new DataTable();
                    itemTable.Columns.Add("ItemGUID", typeof(Guid));
                    itemTable.Columns.Add("ApprovedQuantity", typeof(double));

                    foreach (var item in lstOfItems)
                    {
                        if (item.ItemGUID != Guid.Empty)
                        {
                            itemTable.Rows.Add(item.ItemGUID, item.ApprovedQuantity.GetValueOrDefault(0));
                        }
                    }

                    var parameters = new[]
                    {
                        new SqlParameter("@OrderGUID", OrderGUID),
                        new SqlParameter("@Room", RoomID),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@ItemList", SqlDbType.Structured)
                        {
                            TypeName = "ItemApproval_BulkType",
                            Value = itemTable
                        }
                    };
                    var result = context.Database.SqlQuery<ItemValidationResult>("EXEC ValidateItemQuantityLimit @OrderGUID, @Room, @CompanyID, @ItemList", parameters).ToList();

                    return result.Select(r => (r.ItemGUID, r.ItemNumber, r.IsValid)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<(Guid ItemGUID, string ItemNumber, bool IsValid)> ValidateOrderItemsCostOnSupplierBlanketPO(Guid OrderGUID, List<OrderDetailsDTO> lstOfItems, long RoomID, long CompanyID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var itemTable = new DataTable();
                    itemTable.Columns.Add("ItemGUID", typeof(Guid));
                    itemTable.Columns.Add("OrderLineItemExtendedCost", typeof(double));

                    foreach (var item in lstOfItems)
                    {
                        if (item.ItemGUID != Guid.Empty)
                        {
                            itemTable.Rows.Add(item.ItemGUID, item.OrderLineItemExtendedCost.GetValueOrDefault(0));
                        }
                    }

                    var parameters = new[]
                    {
                        new SqlParameter("@OrderGUID", OrderGUID),
                        new SqlParameter("@Room", RoomID),
                        new SqlParameter("@CompanyID", CompanyID),
                        new SqlParameter("@ItemList", SqlDbType.Structured)
                        {
                            TypeName = "ItemCost_BulkType",
                            Value = itemTable
                        }
                    };
                    var result = context.Database.SqlQuery<ItemValidationResult>("EXEC ValidateItemCostLimit @OrderGUID, @Room, @CompanyID, @ItemList", parameters).ToList();

                    return result.Select(r => (r.ItemGUID, r.ItemNumber, r.IsValid)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public class ItemValidationResult
        {
            public Guid ItemGUID { get; set; }
            public string ItemNumber { get; set; }
            public bool IsValid { get; set; }            
        }

        public Int64 GetMaxReleaseNoByAllOrderNumber(long RoomId, long CompanyId, string OrderNumber, OrderType OrdType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomId),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderNumber", OrderNumber),
                                                   new SqlParameter("@OrderType", (int)OrdType)
                                                };
                var maximumReleaseNo = context.Database.SqlQuery<Int64?>("exec [GetMaxReleaseNoByAllOrderNumber] @RoomID,@CompanyID,@OrderNumber,@OrderType", params1).FirstOrDefault();
                return maximumReleaseNo ?? 0;
            }
        }
        public List<OrderMasterDTO> GetOrdersToBeClosed()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<OrderMasterDTO>("exec [GetOrdersToBeClosed]").ToList();
            }
        }
    }
}


