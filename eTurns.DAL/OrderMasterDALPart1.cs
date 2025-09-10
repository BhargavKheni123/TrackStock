using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;
using System.Globalization;


namespace eTurns.DAL
{
    public partial class OrderMasterDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        private IEnumerable<OrderMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, OrderType ordType)
        {
            return DB_GetCachedData(CompanyID, RoomID, null, null, null, null, null, ordType);
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        public IEnumerable<OrderMasterDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, OrderType OrdType)
        {
            return DB_GetCachedData(CompanyID, RoomID, IsDeleted, IsArchived, null, null, null, OrdType);
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
        public IEnumerable<OrderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, OrderType OrdType, Int64 SupplierID = 0)
        {
            return DB_GetCachedData(CompanyId, RoomID, null, null, null, null, SupplierID, OrdType);

        }

        /// <summary>
        /// Get List of Pending Orders
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<OrderMasterDTO> GetPendingOrderList(Int64 RoomID, Int64 CompanyID, OrderType OrdType)
        {
            IEnumerable<OrderMasterDTO> objOrders = GetAllRecords(RoomID, CompanyID, false, false, OrdType);
            objOrders = (from x in objOrders
                         where (x.OrderStatus == (int)OrderStatus.Transmitted
                                 || x.OrderStatus == (int)OrderStatus.TransmittedIncomplete
                                 || x.OrderStatus == (int)OrderStatus.TransmittedInCompletePastDue
                                 || x.OrderStatus == (int)OrderStatus.TransmittedPastDue
                                 ) && x.RequiredDate < DateTime.Now
                         orderby x.OrderNumber
                         select x).AsEnumerable<OrderMasterDTO>();

            return objOrders;
        }

        public bool IsOrderInApprovalLimit(OrderMasterDTO order, List<OrderDetailsDTO> lstOrdDetailDTO, Int64 UserID)
        {
            double UserApprovalLimit = 5000;
            double UserUsedLimit = 2500;
            double OrderCost = 0;
            foreach (OrderDetailsDTO objOrderDetail in lstOrdDetailDTO)
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                objItemMasterDTO = objItemMasterDAL.GetRecordByItemGUIDLight((objOrderDetail.ItemGUID ?? Guid.Empty), order.Room.GetValueOrDefault(0), order.CompanyID.GetValueOrDefault(0));
                CostUOMMasterDTO costUOM = new CostUOMMasterDAL(base.DataBaseName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                if (costUOM == null)
                {
                    costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };
                }
                if (objItemMasterDTO != null && objOrderDetail.ApprovedQuantity != null
                            && objOrderDetail.ApprovedQuantity > 0)
                {
                    OrderCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * objOrderDetail.ApprovedQuantity.GetValueOrDefault(0))
                            / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                }
            }

            if (OrderCost <= (UserApprovalLimit - UserUsedLimit))
                return true;

            return false;
        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID, string DBConnection)
        {
            OrderMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            if (id > 0)
            {
                using (var context = new eTurnsEntities(DBConnection))
                {
                    objOrder = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(@"SELECT A.*
                                                                                                ,B.UserName AS 'CreatedByName'
		                                                                                        ,C.UserName AS UpdatedByName
		                                                                                        ,D.RoomName 
		                                                                                        ,ISNULL(S.SupplierName,'') AS SupplierName
		                                                                                        ,ISNULL(SH.ShipVia,'') AS ShipViaName
		                                                                                        ,ISNULL(CM.Customer,'') AS CustomerName
		                                                                                        ,ISNULL(BM.BinNumber,'') AS StagingName	
                                                                                                ,ISNULL(VM.Vender,'') AS ShippingVendorName				
                                                                                                ,Convert(Bit,ISNULL((SELECT TOP 1 ISNULL(OD.ReceivedQuantity,0) FROM OrderDetails OD WHERE OD.OrderGUID = A.GUID AND OD.ReceivedQuantity >0 ),0))  AS OrderIsInReceive
                                                                                        FROM OrderMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                       LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                                                                       LEFT OUTER JOIN SupplierMaster S on A.Supplier= S.ID 
					                                                                                       LEFT OUTER JOIN ShipViaMaster SH on A.ShipVia= SH.ID 
                                                                                                           LEFT OUTER JOIN VenderMaster VM on  A.ShippingVendor = VM.ID 
					                                                                                       LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID= CM.GUID 
					                                                                                       LEFT OUTER JOIN BinMaster BM on A.StagingID = BM.ID
					                                                                                       LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.ID = " + id)
                                select new OrderMasterDTO
                                {
                                    ID = u.ID,
                                    OrderNumber = u.OrderNumber,
                                    ReleaseNumber = u.ReleaseNumber,
                                    ShipVia = u.ShipVia,
                                    Supplier = u.Supplier,
                                    Comment = u.Comment,
                                    RequiredDate = u.RequiredDate,
                                    OrderStatus = u.OrderStatus,
                                    CustomerID = u.CustomerID,
                                    CustomerGUID = u.CustomerGUID,
                                    PackSlipNumber = u.PackSlipNumber,
                                    ShippingTrackNumber = u.ShippingTrackNumber,
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    RejectionReason = u.RejectionReason,
                                    Action = string.Empty,
                                    HistoryID = 0,
                                    IsHistory = false,
                                    StagingID = u.StagingID,
                                    CustomerAddress = u.CustomerAddress,
                                    OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                    SupplierName = u.SupplierName,
                                    ShipViaName = u.ShipViaName,
                                    CustomerName = u.CustomerName,
                                    StagingName = u.StagingName,
                                    OrderIsInReceive = u.OrderIsInReceive,
                                    OrderCost = u.OrderCost,
                                    NoOfLineItems = u.NoOfLineItems,
                                    BlanketOrderNumberID = u.BlanketOrderNumberID,
                                    ShippingVendorName = u.ShippingVendorName,
                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    IsEDIOrder = u.IsEDIOrder,
                                    OrderPrice = u.OrderPrice
                                }).SingleOrDefault();

                }
            }
            return objOrder;

        }

        public OrderMasterDTO GetRecord(Guid OrdGuid, Int64 RoomID, Int64 CompanyID, string DBConnection)
        {
            OrderMasterDTO objOrder = null;// GetCachedData(RoomID, CompanyID, false, false).SingleOrDefault(t => t.ID == id);
            if (OrdGuid != Guid.Empty)
            {
                using (var context = new eTurnsEntities(DBConnection))
                {
                    objOrder = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(@"SELECT A.*
                                                                                                ,B.UserName AS 'CreatedByName'
		                                                                                        ,C.UserName AS UpdatedByName
		                                                                                        ,D.RoomName 
		                                                                                        ,ISNULL(S.SupplierName,'') AS SupplierName
		                                                                                        ,ISNULL(SH.ShipVia,'') AS ShipViaName
		                                                                                        ,ISNULL(CM.Customer,'') AS CustomerName
		                                                                                        ,ISNULL(BM.BinNumber,'') AS StagingName	
                                                                                                ,ISNULL(VM.Vender,'') AS ShippingVendorName				
                                                                                                ,Convert(Bit,ISNULL((SELECT TOP 1 ISNULL(OD.ReceivedQuantity,0) FROM OrderDetails OD WHERE OD.OrderGUID = A.GUID AND OD.ReceivedQuantity >0 ),0))  AS OrderIsInReceive
                                                                                        FROM OrderMaster A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
					                                                                                       LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
					                                                                                       LEFT OUTER JOIN SupplierMaster S on A.Supplier= S.ID 
					                                                                                       LEFT OUTER JOIN ShipViaMaster SH on A.ShipVia= SH.ID 
                                                                                                           LEFT OUTER JOIN VenderMaster VM on  A.ShippingVendor = VM.ID 
					                                                                                       LEFT OUTER JOIN CustomerMaster CM on A.CustomerGUID= CM.GUID 
					                                                                                       LEFT OUTER JOIN BinMaster BM on A.StagingID = BM.ID
					                                                                                       LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.GUID = '" + OrdGuid.ToString() + "'")
                                select new OrderMasterDTO
                                {
                                    ID = u.ID,
                                    OrderNumber = u.OrderNumber,
                                    ReleaseNumber = u.ReleaseNumber,
                                    ShipVia = u.ShipVia,
                                    Supplier = u.Supplier,
                                    Comment = u.Comment,
                                    RequiredDate = u.RequiredDate,
                                    OrderStatus = u.OrderStatus,
                                    CustomerID = u.CustomerID,
                                    CustomerGUID = u.CustomerGUID,
                                    PackSlipNumber = u.PackSlipNumber,
                                    ShippingTrackNumber = u.ShippingTrackNumber,
                                    Created = u.Created,
                                    LastUpdated = u.LastUpdated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    Room = u.Room,
                                    IsDeleted = u.IsDeleted,
                                    IsArchived = u.IsArchived,
                                    CompanyID = u.CompanyID,
                                    GUID = u.GUID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    RejectionReason = u.RejectionReason,
                                    Action = string.Empty,
                                    HistoryID = 0,
                                    IsHistory = false,
                                    StagingID = u.StagingID,
                                    CustomerAddress = u.CustomerAddress,
                                    OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                    SupplierName = u.SupplierName,
                                    ShipViaName = u.ShipViaName,
                                    CustomerName = u.CustomerName,
                                    StagingName = u.StagingName,
                                    OrderIsInReceive = u.OrderIsInReceive,
                                    OrderCost = u.OrderCost,
                                    NoOfLineItems = u.NoOfLineItems,
                                    BlanketOrderNumberID = u.BlanketOrderNumberID,
                                    ShippingVendorName = u.ShippingVendorName,
                                    MaterialStagingGUID = u.MaterialStagingGUID,
                                    AddedFrom = u.AddedFrom,
                                    EditedFrom = u.EditedFrom,
                                    ReceivedOn = u.ReceivedOn,
                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                    IsEDIOrder = u.IsEDIOrder,
                                    OrderPrice = u.OrderPrice
                                }).FirstOrDefault();

                }
            }
            return objOrder;

        }

        /// <summary>
        /// GetOnlyReceivedOrderIDs
        /// </summary>
        /// <returns></returns>
        public List<Int64> GetOnlyReceivedOrderIDs()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Int64> lstReceivedIDs = (from u in context.ExecuteStoreQuery<Int64>(@"SELECT DISTINCT OrderID 
                                                                                            FROM OrderDetails 
                                                                                            WHERE ID IN (SELECT DISTINCT OrderDetailID 
                                                                                                         FROM ReceiveOrderDetails 
                                                                                                         WHERE ISNULL(IsDeleted,0) = 0 and ISNULL(IsArchived,0) = 0 
                                                                                                         Union 
                                                                                                         SELECT DISTINCT OrderDetailID 
                                                                                                         FROM ItemLocationDetails 
                                                                                                         WHERE OrderDetailID IS NOT NULL 
                                                                                                                AND ISNULL(IsDeleted,0) = 0 
                                                                                                                AND ISNULL(IsArchived,0) = 0)
                                                                                            AND ISNULL(IsDeleted,0) = 0 
                                                                                            AND ISNULL(IsArchived,0) = 0 ")
                                              select u).ToList();

                return lstReceivedIDs;
            }
        }

        public int GetTotalOrderCountbyOrderName(string OrderNUmber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ExecuteStoreQuery<int>("Select cast(isnull(Count(ID),0) as int) from OrderMaster where isnull(isdeleted,0)=0 and isnull(isarchived,0)=0 AND Room=" + RoomID + " AND CompanyID=" + CompanyID + " AND OrderNumber='" + (OrderNUmber ?? string.Empty).Trim() + "'").First();
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
        public IEnumerable<OrderMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, OrderType OrdType, Int64 SupplierID = 0)
        {
            return DB_GetCachedData(CompanyId, RoomID, IsDeleted, IsArchived, null, null, SupplierID, OrdType);
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        private IEnumerable<OrderMasterDTO> DB_GetCachedData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? GuID, Int64? SupplierID, OrderType OrdType)
        {

            string strCommand = "EXEC Ord_GetOrderMasterData ";

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

            if (SupplierID.HasValue && SupplierID.Value > 0)
                strCommand += ", " + SupplierID.Value.ToString();
            else
                strCommand += ", " + "null";

            strCommand += ", " + Convert.ToString((int)OrdType);

            IEnumerable<OrderMasterDTO> obj = ExecuteQuery(strCommand);
            return obj;

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
        private IEnumerable<OrderMasterDTO> DB_GetPagedRecords_Archive(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 SupplierID, OrderType OrdType, string RoomDateFormat, int HaveLineItem = 0)
        {
            IsArchived = false;

            string strQuer = "";
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
                        // FromRequiredDate = DateTime.Now.AddDays(14);
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
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                    //FromdDate = Convert.ToDateTime(FieldsPara[5].Split(',')[0]).Date;
                    //ToDate = Convert.ToDateTime(FieldsPara[5].Split(',')[1]).Date;

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,Created) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105)) ";

                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {

                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
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

            //if (!string.IsNullOrEmpty(NarrowSearchQry))
            //    SearchTerm = "";


            string strSupplier = "null";
            if (SupplierID > 0)
                strSupplier = SupplierID.ToString();


            string OrderType = Convert.ToString((int)OrdType);


            //EXEC [Ord_GetOrderMasterPagedData] 201180000,4,0,0,1,10, 'ID DESC', '', '', '5'
            strQuer = @"EXEC [Ord_GetOrderMasterPagedData_Archive] " + CompanyID + "," + RoomID + "," + IsDeleted + "," + IsArchived + "," + StartRowIndex + "," + MaxRows + ",'" + sortColumnName + "','" + SearchTerm + "','" + NarrowSearchQry + "','" + StatusQuery + "'," + strSupplier + "," + OrderType + "," + HaveLineItem;

            //DataSet ds = new DataSet();
            //string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            //SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            //ds = SqlHelper.ExecuteDataset(EturnsConnection, "Ord_GetOrderMasterPagedData_Archive", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, NarrowSearchQry, StatusQuery, SupplierID, OrderType, HaveLineItem);

            //IEnumerable<OrderMasterDTO> obj = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);
            //using (var context = new eTurnsEntities())
            //{

            //}
            IEnumerable<OrderMasterDTO> obj = ExecuteQuery(strQuer);

            TotalCount = 0;
            if (obj != null && obj.Count() > 0)
            {
                TotalCount = obj.ElementAt(0).TotalRecords;
            }
            return obj;


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
        private IEnumerable<OrderMasterDTO> DB_GetPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, long SupplierID, OrderType OrdType, string RoomDateFormat, int HaveLineItem = 0)
        {
            string strQuer = "";
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
                        // FromRequiredDate = DateTime.Now.AddDays(14);
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
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));

                    if (!string.IsNullOrEmpty(NarrowSearchQry))
                        NarrowSearchQry += " AND ";

                    NarrowSearchQry += @" (Convert(Datetime,Created) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105)) ";
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[0], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));
                    ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[6].Split(',')[1], Convert.ToString(HttpContext.Current.Session["RoomDateFormat"]), ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(Convert.ToString(HttpContext.Current.Session["CurrentTimeZone"])));

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

            string strSupplier = "null";

            if (SupplierID > 0)
                strSupplier = SupplierID.ToString();

            string OrderType = Convert.ToString((int)OrdType);
            strQuer = @"EXEC [Ord_GetOrderMasterPagedData] " + CompanyID + "," + RoomID + "," + IsDeleted + "," + IsArchived + "," + StartRowIndex + "," + MaxRows + ",'" + sortColumnName + "','" + SearchTerm + "','" + NarrowSearchQry + "','" + StatusQuery + "'," + strSupplier + "," + OrderType + "," + HaveLineItem;
            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "Ord_GetOrderMasterPagedData", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, NarrowSearchQry, StatusQuery, SupplierID, OrderType, HaveLineItem);
            IEnumerable<OrderMasterDTO> obj = DataTableHelper.ToList<OrderMasterDTO>(ds.Tables[0]);
            TotalCount = 0;

            if (obj != null && obj.Count() > 0)
            {
                TotalCount = obj.ElementAt(0).TotalRecords;
            }

            return obj;

        }

        public void UpdateItemInTransitQtyForOrderAndReceive(string ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            string strCommand = "EXEC UpdateItemInTransitQtyForOrderAndReceive ";
            strCommand += "'" + ItemGUID + "'";
            strCommand += "," + RoomID + " ";
            strCommand += "," + CompanyID + " ";
            OrderMasterDTO objDTO = new OrderMasterDTO();
            objDTO = ExecuteQuery(strCommand).FirstOrDefault();

        }

        private IEnumerable<OrderMasterDTO> ExecuteQuery(string query, string DBConnectionstring)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                context.CommandTimeout = 120;
                IEnumerable<OrderMasterDTO> obj = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(query)
                                                   select new OrderMasterDTO
                                                   {
                                                       ID = u.ID,
                                                       OrderNumber = u.OrderNumber,
                                                       ReleaseNumber = u.ReleaseNumber,
                                                       ShipVia = u.ShipVia,
                                                       Supplier = u.Supplier,
                                                       Comment = u.Comment,
                                                       RequiredDate = u.RequiredDate,
                                                       OrderStatus = u.OrderStatus,
                                                       CustomerID = u.CustomerID,
                                                       CustomerGUID = u.CustomerGUID,
                                                       PackSlipNumber = u.PackSlipNumber,
                                                       ShippingTrackNumber = u.ShippingTrackNumber,
                                                       Created = u.Created,
                                                       LastUpdated = u.LastUpdated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       GUID = u.GUID,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       CreatedByName = u.CreatedByName,
                                                       UpdatedByName = u.UpdatedByName,
                                                       RoomName = u.RoomName,
                                                       RejectionReason = u.RejectionReason,
                                                       Action = string.Empty,
                                                       HistoryID = 0,
                                                       IsHistory = false,
                                                       StagingID = u.StagingID,
                                                       CustomerAddress = u.CustomerAddress,
                                                       OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                                       SupplierName = u.SupplierName,
                                                       ShipViaName = u.ShipViaName,
                                                       CustomerName = u.CustomerName,
                                                       StagingName = u.StagingName,
                                                       OrderIsInReceive = u.OrderIsInReceive,
                                                       AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                                                       OrderCost = u.OrderCost,
                                                       NoOfLineItems = u.NoOfLineItems,
                                                       AccountNumber = u.AccountNumber,
                                                       ChangeOrderRevisionNo = u.ChangeOrderRevisionNo,
                                                       OrderDate = u.OrderDate,
                                                       OrderType = u.OrderType.GetValueOrDefault(0),
                                                       ShippingVendor = u.ShippingVendor,
                                                       TotalRecords = u.TotalRecords,
                                                       BlanketOrderNumberID = u.BlanketOrderNumberID,
                                                       ShippingVendorName = u.ShippingVendorName,
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       Indicator = u.Indicator,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       IsEDIOrder = u.IsEDIOrder,
                                                   }).AsParallel().ToList();

                return obj;
            }

        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderMasterDTO GetRecord(Int64 id, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(CompanyID, RoomID, id);
        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        private OrderMasterDTO DB_GetRecord(Int64? CompanyID, Int64? RoomID, Int64 ID)
        {


            string strCommand = "EXEC Ord_GetOrderMasterData ";

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

            OrderMasterDTO obj = ExecuteQuery(strCommand).FirstOrDefault();
            return obj;

        }

        /// <summary>
        /// Insert Order Master
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        private OrderMasterDTO DB_InsertOrderMaster(OrderMasterDTO objDTO)
        {
            AutoSequenceDAL objAutoSeqDAL = null;
            try
            {
                string strCommand = "EXEC Ord_InsertOrderMaster ";
                strCommand += "'" + objDTO.OrderNumber + "'";
                strCommand += ",'" + objDTO.ReleaseNumber + "'";
                strCommand += "," + objDTO.Supplier.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.CreatedBy.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.Room.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.CompanyID.GetValueOrDefault(0) + " ";
                strCommand += "," + objDTO.OrderStatus + " ";

                if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.StagingID.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";

                if (objDTO.CustomerID.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.CustomerID.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";




                if (objDTO.ShipVia.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.ShipVia.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";

                if (objDTO.OrderType.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.OrderType.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";

                if (objDTO.ShippingVendor.GetValueOrDefault(0) > 0)
                    strCommand += "," + objDTO.ShippingVendor.GetValueOrDefault(0) + " ";
                else
                    strCommand += ", null";

                strCommand += ",'" + objDTO.RequiredDate.ToString("yyyy-MM-dd") + "' ";
                strCommand += ",'" + objDTO.OrderDate.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd") + "' ";

                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.Comment) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@PackSlipNumber) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@ShippingTrackNumber) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF1) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF2) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF3) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF4) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.@UDF5) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.CustomerAddress) + "'";
                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.AccountNumber) + "'";

                if (objDTO.BlanketOrderNumberID.HasValue)
                {
                    strCommand += "," + objDTO.BlanketOrderNumberID.GetValueOrDefault(0);
                }
                else
                {
                    strCommand += ",null";
                }
                if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                    objDTO.WhatWhereAction = "Order";

                strCommand += ",'" + ConvertStringForSQLPerameter(objDTO.WhatWhereAction) + "'";

                if (string.IsNullOrEmpty(objDTO.OrderNumber_ForSorting))
                    objDTO.OrderNumber_ForSorting = objDTO.OrderNumber;

                strCommand += ",'" + CommonDAL.GetSortingString(objDTO.OrderNumber) + "'";

                if (objDTO.MaterialStagingGUID != null)
                    strCommand += ",'" + objDTO.MaterialStagingGUID.Value + "'";
                else
                    strCommand += ", null";

                if (objDTO.CustomerGUID != null)
                    strCommand += ",'" + objDTO.CustomerGUID.Value + "' ";
                else
                    strCommand += ", null";

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

                if (objDTO.SupplierAccountGuid != null)
                    strCommand += ",@SupplierAccountGuid='" + objDTO.SupplierAccountGuid.Value + "' ";
                else
                    strCommand += ",@SupplierAccountGuid= null";

                objDTO = ExecuteQuery(strCommand).FirstOrDefault();

                objAutoSeqDAL = new AutoSequenceDAL(base.DataBaseName);
                objAutoSeqDAL.UpdateNextOrderNumber(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Supplier.GetValueOrDefault(0), objDTO.OrderNumber);

                return objDTO;
            }
            finally
            {
                objAutoSeqDAL = null;
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderMaster by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderMasterDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(CompanyID, RoomID, GUID);
        }

        private OrderMasterDTO DB_GetRecord(Int64? CompanyID, Int64? RoomID, Guid Guid)
        {
            string strCommand = "EXEC Ord_GetOrderMasterData ";

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

            OrderMasterDTO obj = ExecuteQuery(strCommand).FirstOrDefault();
            return obj;

        }

        /// <summary>
        /// GetPendingOrderList
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="OrdType"></param>
        /// <returns></returns>
        public IEnumerable<OrderMasterDTO> GetPendingOrderList(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, OrderType OrdType)
        {
            string strCommand = "EXEC Ord_GetOrderMasterData ";

            strCommand += CompanyID.ToString();
            strCommand += ", " + RoomID.ToString();
            strCommand += ", " + (IsDeleted ? "1" : "0");
            strCommand += ", " + (IsArchived ? "1" : "0");
            strCommand += ", " + "null";
            strCommand += ", " + "null";
            strCommand += ", " + "null";
            strCommand += ", " + ((int)OrdType).ToString();

            IEnumerable<OrderMasterDTO> obj = ExecuteQuery(strCommand);
            return obj;
        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<OrderMasterDTO> ExecuteQuery(string query)
        {
            IEnumerable<OrderMasterDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.CommandTimeout = 120;
                    obj = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(query)
                           select new OrderMasterDTO
                           {
                               ID = u.ID,
                               OrderNumber = u.OrderNumber,
                               ReleaseNumber = u.ReleaseNumber,
                               ShipVia = u.ShipVia,
                               Supplier = u.Supplier,
                               Comment = u.Comment,
                               RequiredDate = u.RequiredDate,
                               RequiredDateStr = Convert.ToString(u.RequiredDate),
                               OrderStatus = u.OrderStatus,
                               CustomerID = u.CustomerID,
                               CustomerGUID = u.CustomerGUID,
                               PackSlipNumber = u.PackSlipNumber,
                               ShippingTrackNumber = u.ShippingTrackNumber,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               IsDeleted = u.IsDeleted,
                               IsArchived = u.IsArchived,
                               CompanyID = u.CompanyID,
                               GUID = u.GUID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               RejectionReason = u.RejectionReason,
                               Action = string.Empty,
                               HistoryID = 0,
                               IsHistory = false,
                               StagingID = u.StagingID,
                               CustomerAddress = u.CustomerAddress,
                               OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                               SupplierName = u.SupplierName,
                               ShipViaName = u.ShipViaName,
                               CustomerName = u.CustomerName,
                               StagingName = u.StagingName,
                               OrderIsInReceive = u.OrderIsInReceive,
                               AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                               OrderCost = u.OrderCost,
                               NoOfLineItems = u.NoOfLineItems,
                               AccountNumber = u.AccountNumber,
                               ChangeOrderRevisionNo = u.ChangeOrderRevisionNo,
                               OrderDate = u.OrderDate,
                               OrderType = u.OrderType.GetValueOrDefault(0),
                               ShippingVendor = u.ShippingVendor,
                               TotalRecords = u.TotalRecords,
                               BlanketOrderNumberID = u.BlanketOrderNumberID,
                               ShippingVendorName = u.ShippingVendorName,
                               WhatWhereAction = u.WhatWhereAction,
                               Indicator = u.Indicator,
                               OMPackSlipNumbers = u.OMPackSlipNumbers,
                               InCompleteItemCount = u.InCompleteItemCount,
                               MaterialStagingGUID = u.MaterialStagingGUID,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               IsEDIOrder = u.IsEDIOrder,
                               SupplierAccountGuid = u.SupplierAccountGuid,
                               SupplierAccountAddress = u.SupplierAccountAddress,
                               SupplierAccountCity = u.SupplierAccountCity,
                               SupplierAccountDetailWithFullAddress = u.SupplierAccountDetailWithFullAddress,
                               SupplierAccountName = u.SupplierAccountName,
                               SupplierAccountNumber = u.SupplierAccountNumber,
                               SupplierAccountNumberName = u.SupplierAccountNumberName,
                               SupplierAccountState = u.SupplierAccountState,
                               SupplierAccountZipcode = u.SupplierAccountZipcode,
                               OrderPrice = u.OrderPrice


                           }).AsEnumerable<OrderMasterDTO>().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(OrderMasterDTO objDTO, String dbConnString)
        {
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;
            OrderMaster obj;

            using (var context = new eTurnsEntities(dbConnString))
            {
                obj = context.OrderMasters.FirstOrDefault(t => t.ID == objDTO.ID);

                if (obj != null)
                {
                    obj.ID = objDTO.ID;
                    obj.OrderNumber = objDTO.OrderNumber;
                    obj.ReleaseNumber = objDTO.ReleaseNumber;

                    if (objDTO.ShipVia.GetValueOrDefault(0) > 0)
                        obj.ShipVia = objDTO.ShipVia;

                    if (objDTO.Supplier.GetValueOrDefault(0) > 0)
                        obj.Supplier = objDTO.Supplier;

                    if (objDTO.StagingID.GetValueOrDefault(0) > 0)
                        obj.StagingID = objDTO.StagingID;

                    if (objDTO.MaterialStagingGUID != null)
                        obj.MaterialStagingGUID = objDTO.MaterialStagingGUID;

                    if (objDTO.CustomerID.GetValueOrDefault(0) > 0)
                        obj.CustomerID = objDTO.CustomerID;
                    else
                        obj.CustomerID = null;

                    if (objDTO.CustomerGUID != null)
                        obj.CustomerGUID = objDTO.CustomerGUID;
                    else
                        obj.CustomerGUID = null;

                    obj.Comment = objDTO.Comment;
                    obj.RequiredDate = objDTO.RequiredDate;
                    obj.OrderStatus = objDTO.OrderStatus;
                    obj.PackSlipNumber = objDTO.PackSlipNumber;
                    obj.ShippingTrackNumber = objDTO.ShippingTrackNumber;
                    obj.Created = objDTO.Created;
                    obj.LastUpdated = objDTO.LastUpdated;
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
                    obj.RejectionReason = objDTO.RejectionReason;
                    obj.NoOfLineItems = objDTO.NoOfLineItems;
                    obj.OrderCost = objDTO.OrderCost;
                    obj.OrderPrice = objDTO.OrderPrice;
                    obj.IsEDIOrder = objDTO.IsEDIOrder;
                    obj.CustomerAddress = objDTO.CustomerAddress;
                    if (objDTO.OrderDate != null)
                        obj.OrderDate = objDTO.OrderDate;
                    else
                        obj.OrderDate = DateTime.Now;

                    context.SaveChanges();
                }

                return true;
            }
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderMasterDTO> GetOrderedCachedData(Int64 RoomID, Int64 CompanyID, Guid ItemGUID, OrderType OrdType)
        {
            IEnumerable<OrderMasterDTO> ObjCache = null;
            ObjCache = CacheHelper<IEnumerable<OrderMasterDTO>>.GetCacheItem("ItemsCached_OrderMaster_" + CompanyID.ToString());
            //if (ObjCache == null || ObjCache.Count() <= 0)
            //{
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string qry = " SELECT OM.ID FROM OrderMaster OM ";
                qry += "  LEFT OUTER  JOIN OrderDetails OD on OD.OrderGUID= OM.GUID ";
                qry += " WHERE OM.CompanyID = '" + CompanyID + "' and OD.ItemGUID='" + ItemGUID + "'  and OM.IsDeleted!=1 and OM.IsArchived!=1  and OD.IsDeleted!=1 and OD.IsArchived!=1 AND OM.OrderType = " + Convert.ToString((int)OrdType);

                List<Int64> OrderIds = (from u in context.ExecuteStoreQuery<Int64>(qry)
                                        select u).ToList<Int64>();

                IEnumerable<OrderMasterDTO> obj = (from y in GetAllRecords(RoomID, CompanyID, OrdType)
                                                   where OrderIds.Contains(y.ID)
                                                   select y);

                ObjCache = CacheHelper<IEnumerable<OrderMasterDTO>>.AddCacheItem("ItemsCached_OrderMaster_" + CompanyID.ToString(), obj);
            }
            return ObjCache.Where(t => t.Room == RoomID);
        }

        public IEnumerable<OrderMasterDTO> GetOrderedPagedRecords(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, Guid ItemGUID, int OrderStatus, OrderType OrdType)
        {

            //Get Cached-Media
            IEnumerable<OrderMasterDTO> ObjOrderedCache = GetOrderedCachedData(RoomID, CompanyID, ItemGUID, OrdType);
            IEnumerable<OrderMasterDTO> ObjOrderedGlobalCache = ObjOrderedCache;
            ObjOrderedCache = ObjOrderedCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));
            if (OrderStatus == 1)
            {
                ObjOrderedCache = ObjOrderedCache.Where(t => t.OrderStatus > 3 && t.OrderStatus < 8);
            }
            else if (OrderStatus == 2)
            {
                ObjOrderedCache = ObjOrderedCache.Where(t => t.OrderStatus == 8);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjOrderedCache.Count();
                return ObjOrderedCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<OrderMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                ObjOrderedCache = ObjOrderedCache.Where(t =>
                       ((Fields[0].Split('~')[0] == "") || (Fields[0].Split('~')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[0].Split('~')[1] == "") || (Fields[0].Split('~')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[0].Split('~')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[0].Split('~')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[0].Split('~')[2].Split(',')[1]).Date))
                    && ((Fields[0].Split('~')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[0].Split('~')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[0].Split('~')[3].Split(',')[1]).Date))
                    && ((Fields[0].Split('~')[4] == "") || (Fields[0].Split('~')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[0].Split('~')[5] == "") || (Fields[0].Split('~')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[0].Split('~')[6] == "") || (Fields[0].Split('~')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[0].Split('~')[7] == "") || (Fields[0].Split('~')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[0].Split('~')[8] == "") || (Fields[0].Split('~')[8].Split(',').ToList().Contains(t.UDF5)))
                     && ((Fields[0].Split('~')[12] == "") || (Fields[0].Split('~')[12].Split(',').ToList().Contains(t.Supplier.ToString())))
                    && ((Fields[0].Split('~')[13] == "") || (Fields[0].Split('~')[13].Split(',').ToList().Contains(t.OrderStatus.ToString())))

                    );

                TotalCount = ObjOrderedCache.Count();
                return ObjOrderedCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<OrderMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjOrderedCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjOrderedCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public void UpdateOrderComment(string comment, Int64 OrderId, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderMaster obj = context.OrderMasters.FirstOrDefault(x => x.ID == OrderId);
                obj.Comment = comment;
                obj.LastUpdatedBy = UserID;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.EditedFrom = "Web";
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<OrderMasterDTO> ExecuteQuery(string query, SqlParameter[] sqlParas)
        {
            IEnumerable<OrderMasterDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.CommandTimeout = 120;
                    obj = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(query, sqlParas)
                           select new OrderMasterDTO
                           {
                               ID = u.ID,
                               OrderNumber = u.OrderNumber,
                               ReleaseNumber = u.ReleaseNumber,
                               ShipVia = u.ShipVia,
                               Supplier = u.Supplier,
                               Comment = u.Comment,
                               RequiredDate = u.RequiredDate,
                               RequiredDateStr = Convert.ToString(u.RequiredDate),
                               OrderStatus = u.OrderStatus,
                               CustomerID = u.CustomerID,
                               CustomerGUID = u.CustomerGUID,
                               PackSlipNumber = u.PackSlipNumber,
                               ShippingTrackNumber = u.ShippingTrackNumber,
                               Created = u.Created,
                               LastUpdated = u.LastUpdated,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               IsDeleted = u.IsDeleted,
                               IsArchived = u.IsArchived,
                               CompanyID = u.CompanyID,
                               GUID = u.GUID,
                               UDF1 = u.UDF1,
                               UDF2 = u.UDF2,
                               UDF3 = u.UDF3,
                               UDF4 = u.UDF4,
                               UDF5 = u.UDF5,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               RejectionReason = u.RejectionReason,
                               Action = string.Empty,
                               HistoryID = 0,
                               IsHistory = false,
                               StagingID = u.StagingID,
                               CustomerAddress = u.CustomerAddress,
                               OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                               SupplierName = u.SupplierName,
                               ShipViaName = u.ShipViaName,
                               CustomerName = u.CustomerName,
                               StagingName = u.StagingName,
                               OrderIsInReceive = u.OrderIsInReceive,
                               AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                               OrderCost = u.OrderCost,
                               NoOfLineItems = u.NoOfLineItems,
                               AccountNumber = u.AccountNumber,
                               ChangeOrderRevisionNo = u.ChangeOrderRevisionNo,
                               OrderDate = u.OrderDate,
                               OrderType = u.OrderType.GetValueOrDefault(0),
                               ShippingVendor = u.ShippingVendor,
                               TotalRecords = u.TotalRecords,
                               BlanketOrderNumberID = u.BlanketOrderNumberID,
                               ShippingVendorName = u.ShippingVendorName,
                               WhatWhereAction = u.WhatWhereAction,
                               Indicator = u.Indicator,
                               OMPackSlipNumbers = u.OMPackSlipNumbers,
                               InCompleteItemCount = u.InCompleteItemCount,
                               MaterialStagingGUID = u.MaterialStagingGUID,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               IsEDIOrder = u.IsEDIOrder,
                               SupplierAccountGuid = u.SupplierAccountGuid,
                               OrderPrice = u.OrderPrice
                           }).AsEnumerable<OrderMasterDTO>().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }

        /// <summary>
        /// ConvertStringForSQLPerameter
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertStringForSQLPerameter(string str)
        {
            string strReturn = str;
            if (!string.IsNullOrEmpty(strReturn))
                strReturn = strReturn.Replace("'", "''");

            return strReturn;

        }

        /// <summary>
        /// GetCachedData
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <returns></returns>
        public IEnumerable<OrderMasterDTO> DB_GetChangeOrderData(Int64? CompanyID, Int64? RoomID, bool? IsDeleted, bool? IsArchived, Int64? OrderID, Guid? OrderGuid, Int64? SupplierID, OrderType? OrdType, Guid? ChangeOrderGuid, Int64? ChangeOrderID)
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

            if (SupplierID.HasValue && SupplierID.Value > 0)
                strCommand += ", " + SupplierID.Value.ToString();
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

            IEnumerable<OrderMasterDTO> obj = ExecuteQueryChangeOrder(strCommand);
            return obj;

        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<OrderMasterDTO> ExecuteQueryChangeOrder(string query)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.CommandTimeout = 120;
                IEnumerable<OrderMasterDTO> obj = (from u in context.ExecuteStoreQuery<OrderMasterDTO>(query)
                                                   select new OrderMasterDTO
                                                   {
                                                       ID = u.ID,
                                                       OrderNumber = u.OrderNumber,
                                                       ReleaseNumber = u.ReleaseNumber,
                                                       ShipVia = u.ShipVia,
                                                       Supplier = u.Supplier,
                                                       Comment = u.Comment,
                                                       RequiredDate = u.RequiredDate,
                                                       OrderStatus = u.OrderStatus,
                                                       CustomerID = u.CustomerID,
                                                       CustomerGUID = u.CustomerGUID,
                                                       PackSlipNumber = u.PackSlipNumber,
                                                       ShippingTrackNumber = u.ShippingTrackNumber,
                                                       Created = u.Created,
                                                       LastUpdated = u.LastUpdated,
                                                       CreatedBy = u.CreatedBy,
                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                       Room = u.Room,
                                                       IsDeleted = u.IsDeleted,
                                                       IsArchived = u.IsArchived,
                                                       CompanyID = u.CompanyID,
                                                       GUID = u.GUID,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       CreatedByName = u.CreatedByName,
                                                       UpdatedByName = u.UpdatedByName,
                                                       RoomName = u.RoomName,
                                                       RejectionReason = u.RejectionReason,
                                                       Action = string.Empty,
                                                       HistoryID = 0,
                                                       IsHistory = false,
                                                       StagingID = u.StagingID,
                                                       CustomerAddress = u.CustomerAddress,
                                                       OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                                       SupplierName = u.SupplierName,
                                                       ShipViaName = u.ShipViaName,
                                                       CustomerName = u.CustomerName,
                                                       StagingName = u.StagingName,
                                                       OrderIsInReceive = u.OrderIsInReceive,
                                                       AppendedBarcodeString = string.Empty,// u.AppendedBarcodeString,
                                                       OrderCost = u.OrderCost,
                                                       NoOfLineItems = u.NoOfLineItems,
                                                       AccountNumber = u.AccountNumber,
                                                       ChangeOrderRevisionNo = u.ChangeOrderRevisionNo,
                                                       OrderDate = u.OrderDate,
                                                       OrderType = u.OrderType.GetValueOrDefault(0),
                                                       ShippingVendor = u.ShippingVendor,
                                                       TotalRecords = u.TotalRecords,
                                                       BlanketOrderNumberID = u.BlanketOrderNumberID,
                                                       ShippingVendorName = u.ShippingVendorName,
                                                       WhatWhereAction = u.WhatWhereAction,
                                                       Indicator = u.Indicator,
                                                       ChangeOrderCreated = u.ChangeOrderCreated,
                                                       ChangeOrderCreatedBy = u.ChangeOrderCreatedBy,
                                                       ChangeOrderCreatedByName = u.ChangeOrderCreatedByName,
                                                       ChangeOrderGUID = u.ChangeOrderGUID,
                                                       ChangeOrderID = u.ChangeOrderID,
                                                       ChangeOrderLastUpdated = u.ChangeOrderLastUpdated,
                                                       ChangeOrderLastUpdatedBy = u.ChangeOrderLastUpdatedBy,
                                                       ChangeOrderUpdatedByName = u.ChangeOrderUpdatedByName,
                                                       MaterialStagingGUID = u.MaterialStagingGUID,
                                                       ReceivedOn = u.ReceivedOn,
                                                       ReceivedOnWeb = u.ReceivedOnWeb,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       IsEDIOrder = u.IsEDIOrder,
                                                       OrderPrice = u.OrderPrice
                                                   }).AsParallel().ToList();

                return obj;
            }

        }

        /// <summary>
        /// Temp Update Order number for sorting field
        /// </summary>
        public void TempFillOrderNumberSorting()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.CommandTimeout = 120;
                List<OrderMasterDTO> objItem = (from u in context.ExecuteStoreQuery<OrderMasterDTO>("SELECT ID,CompanyID,Room,OrderNumber FROM OrderMaster")
                                                select new OrderMasterDTO
                                                {
                                                    ID = u.ID,
                                                    OrderNumber = u.OrderNumber,
                                                    CompanyID = u.CompanyID,
                                                    Room = u.Room,
                                                }).ToList();

                foreach (var item in objItem)
                {
                    OrderMaster obj = context.OrderMasters.FirstOrDefault(x => x.ID == item.ID && x.Room == item.Room && x.CompanyID == item.CompanyID);
                    obj.OrderNumber_ForSorting = CommonDAL.GetSortingString(obj.OrderNumber);
                    context.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
                    context.OrderMasters.Attach(obj);
                    context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                    context.SaveChanges();
                }
            }
        }

        public IEnumerable<OrderMasterDTO> GetAllReportRecords()
        {
            IEnumerable<OrderMasterDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.OrderMasters
                       where u.IsDeleted == false && u.IsArchived == false
                       select new OrderMasterDTO
                       {
                           GUID = u.GUID,
                           OrderNumber = u.OrderNumber,
                           OrderStatus = u.OrderStatus,
                           Created = u.Created,
                           Room = u.Room,
                           Supplier = u.Supplier,
                           OrderType = u.OrderType,
                           LastUpdated = u.LastUpdated,
                       }).AsParallel().ToList().OrderBy(x => x.OrderNumber);
            }
            return obj;

        }
    }
}
