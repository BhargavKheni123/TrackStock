using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class ReceiveOrderDetailsDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ReceiveOrderDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ReceiveOrderDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Insert Record in the DataBase ReceiveOrderDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 Insert(ReceiveOrderDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ReceiveOrderDetail obj = new ReceiveOrderDetail();
                obj.ID = 0;
                obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                if (objDTO.ReceiveBin.GetValueOrDefault(0) > 0)
                    obj.ReceiveBin = objDTO.ReceiveBin;
                else
                    obj.ReceiveBin = null;

                obj.ReceiveDate = objDTO.ReceiveDate;
                obj.ReceiveQuantity = objDTO.ReceiveQuantity;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.LastUpdated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = false;
                obj.IsArchived = false;
                obj.CompanyID = objDTO.CompanyID;
                obj.GUID = Guid.NewGuid();
                context.ReceiveOrderDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }

        }

        /// <summary>
        /// Insert Record in the DataBase ReceiveOrderDetail
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public Int64 InsertRecords(List<ReceiveOrderDetailsDTO> lstDTO, long SessionUserId,long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (var objDTO in lstDTO)
                {
                    ReceiveOrderDetail obj = new ReceiveOrderDetail();
                    obj.ID = 0;
                    obj.OrderDetailGUID = objDTO.OrderDetailGUID;
                    obj.ReceiveBin = objDTO.ReceiveBin;
                    obj.ReceiveDate = objDTO.ReceiveDate;
                    obj.ReceiveQuantity = objDTO.ReceiveQuantity;
                    obj.Created = DateTimeUtility.DateTimeNow;
                    obj.LastUpdated = DateTimeUtility.DateTimeNow;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    obj.Room = objDTO.Room;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.IsArchived = objDTO.IsArchived;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.GUID = Guid.NewGuid();
                    context.ReceiveOrderDetails.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;

                    OrderDetailsDAL OrdDetail = new OrderDetailsDAL(base.DataBaseName);
                    OrderDetailsDTO ordDetailDTO = OrdDetail.GetOrderDetailByGuidPlain(objDTO.OrderDetailGUID.GetValueOrDefault(Guid.Empty), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));

                    if (ordDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {


                        if (objDTO.ReceiveBin.GetValueOrDefault(0) > 0)
                        {
                            ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
                            ItemMasterDTO itemDTO = itemDAL.GetItemWithoutJoins(null, ordDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                            itemDTO.StagedQuantity = itemDTO.StagedQuantity.GetValueOrDefault(0) + objDTO.ReceiveQuantity;
                            itemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                            itemDTO.Updated = DateTimeUtility.DateTimeNow;
                            itemDTO.WhatWhereAction = "Receive";
                            itemDAL.Edit(itemDTO, SessionUserId,EnterpriseId);
                        }

                        ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objDTO.ReceiveQuantity;
                        ordDetailDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                        ordDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        OrdDetail.Edit(ordDetailDTO, SessionUserId,EnterpriseId);

                    }
                }
            }

            return 1;

        }

        ///// <summary>
        ///// Delete Multiple Records
        ///// </summary>
        ///// <param name="IDs"></param>
        ///// <param name="userid"></param>
        ///// <returns></returns>
        //public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyID)
        //{
        //    using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
        //    {

        //        string strQuery = "";
        //        foreach (string item in IDs.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            Int64 id = Int64.Parse(item);

        //            ReceiveOrderDetail obj = context.ReceiveOrderDetails.Single(t => t.ID == id);
        //            double PreviousQuentity = obj.ReceiveQuantity.GetValueOrDefault(0);


        //            if (!string.IsNullOrEmpty(item.Trim()))
        //            {
        //                strQuery += "UPDATE ReceiveOrderDetails SET LastUpdated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
        //            }
        //            context.Database.ExecuteSqlCommand(strQuery);

        //            OrderDetailsDAL OrdDetail = new OrderDetailsDAL(base.DataBaseName);
        //            OrderDetailsDTO ordDetailDTO = OrdDetail.GetRecord(obj.OrderDetailGUID.GetValueOrDefault(Guid.Empty), obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0));
        //            if (PreviousQuentity > 0 && ordDetailDTO != null && ordDetailDTO.ItemGUID != Guid.Empty)
        //            {
        //                //CommonDAL commonDAL = new CommonDAL(base.DataBaseName);
        //                //commonDAL.UpdateItemQuentity(ordDetailDTO.ItemID.GetValueOrDefault(0), obj.ReceiveBin.GetValueOrDefault(0), PreviousQuentity, obj.Room.GetValueOrDefault(0), obj.CompanyID.GetValueOrDefault(0), userid, -1);

        //                if (ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) >= PreviousQuentity)
        //                {
        //                    ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) - PreviousQuentity;
        //                    ordDetailDTO.LastUpdatedBy = userid;
        //                    ordDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                    OrdDetail.Edit(ordDetailDTO);
        //                }

        //                ItemMasterDAL itemDAL = new ItemMasterDAL(base.DataBaseName);
        //                ItemMasterDTO itemDTO = itemDAL.GetItemWithoutJoins(null, ordDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
        //                if (ordDetailDTO.Bin.GetValueOrDefault(0) > 0 && itemDTO.StagedQuantity.GetValueOrDefault(0) >= PreviousQuentity)
        //                {
        //                    itemDTO.StagedQuantity = itemDTO.StagedQuantity.GetValueOrDefault(0) - PreviousQuentity;
        //                    itemDTO.LastUpdatedBy = userid;
        //                    itemDTO.Updated = DateTimeUtility.DateTimeNow;
        //                    itemDTO.WhatWhereAction = "Receive";
        //                    itemDAL.Edit(itemDTO);
        //                }

        //            }

        //            //Get Cached-Media
        //            IEnumerable<ReceiveOrderDetailsDTO> ObjCache = CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.GetCacheItem("Cached_ReceiveOrderDetails_" + CompanyID.ToString());
        //            if (ObjCache != null)
        //            {
        //                List<ReceiveOrderDetailsDTO> objTemp = ObjCache.ToList();
        //                objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
        //                ObjCache = objTemp.AsEnumerable();
        //                CacheHelper<IEnumerable<ReceiveOrderDetailsDTO>>.AppendToCacheItem("Cached_ReceiveOrderDetails_" + CompanyID.ToString(), ObjCache);
        //            }
        //        }


        //        return true;
        //    }
        //}

        /// <summary>
        /// Get Paged Records from the OrderDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceiveOderLineItemsDTO> GetPagedRecordsForSession(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<ReceiveOderLineItemsDTO> objlstDTO)
        {
            //Get Cached-Media
            if (objlstDTO == null)
            {
                TotalCount = 0;
                return new List<ReceiveOderLineItemsDTO>().AsEnumerable();
            }
            IEnumerable<ReceiveOderLineItemsDTO> ObjCache = objlstDTO;
            IEnumerable<ReceiveOderLineItemsDTO> ObjGlobalCache = ObjCache;
            ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false));

            if (IsArchived && IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsArchived)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }
            else if (IsDeleted)
            {
                ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
                ObjCache = ObjCache.Concat(ObjGlobalCache);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<OrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    //&& ((Fields[1].Split('@')[3] == "") || (t.LastUpdated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<OrderDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        #region "********** Pendig LineItems ***********"


        /// <summary>
        /// Get Paged Records from the OrderDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        

        /// <summary>
        /// Get Records from the OrderDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ReceiveOrderLineItemDetailsDTO> GetLineItemsOrderRecords(Int64 RoomID, Int64 CompanyId, string ItemGUID, Int64 OrderID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), 
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyId),
                                                   new SqlParameter("@OrderID", OrderID)  };

                IEnumerable<ReceiveOrderLineItemDetailsDTO> obj = (from u in context.Database.SqlQuery<ReceiveOrderLineItemDetailsDTO>("exec [GetReceiveOrderLineItemsOrderRecords] @ItemGUID,@RoomID,@CompanyID,@OrderID", params1)
                                                                   select new ReceiveOrderLineItemDetailsDTO
                                                                   {
                                                                       GUID = u.GUID,
                                                                       ItemGUID = u.ItemGUID,
                                                                       OrderGUID = u.OrderGUID,
                                                                       //OrderID = u.OrderID,
                                                                       ItemNumber = u.ItemNumber,
                                                                       ReceivedQuantity = u.ReceivedQuantity,
                                                                       RequestedQuantity = u.RequestedQuantity,
                                                                       SupplierName = u.SupplierName,
                                                                       Supplier = u.Supplier,
                                                                       Description = u.Description,
                                                                       RequiredDate = u.RequiredDate,
                                                                       OrderNumber = u.OrderNumber,
                                                                       Room = u.Room,
                                                                       ItemType = u.ItemType,

                                                                   }).AsParallel().ToList();
                return obj;
            }
        }


        #endregion


        #region Receive New Methods

        /// <summary>
        /// GetALLReceiveList
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public IEnumerable<ReceivableItemDTO> GetALLReceiveList(Int64 RoomID, Int64 CompanyID)
        {
            return GetALLReceiveList(RoomID, CompanyID, null, null, null, null);

        }

        /// <summary>
        /// GetALLReceiveList
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="OrderGuid"></param>
        /// <param name="BinID"></param>
        /// <param name="OrderDetailGuid"></param>
        /// <returns></returns>
        public IEnumerable<ReceivableItemDTO> GetALLReceiveList(Int64 RoomID, Int64 CompanyID, Guid? ItemGuid, Guid? OrderGuid, Guid? BinID, Guid? OrderDetailGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strCommand = "EXEC RCV_GetPendingReceiveItems ";
                strCommand += CompanyID;
                strCommand += ", " + RoomID;
                if (ItemGuid.HasValue)
                    strCommand += ",'" + ItemGuid.Value + "'";
                else
                    strCommand += ",null";

                if (OrderGuid.HasValue)
                    strCommand += ",'" + OrderGuid.Value + "'";
                else
                    strCommand += ",null";

                if (BinID.HasValue)
                    strCommand += "," + BinID.Value;
                else
                    strCommand += ",null";

                if (OrderDetailGuid.HasValue)
                    strCommand += ",'" + OrderDetailGuid.Value + "'";
                else
                    strCommand += ",null";


                IEnumerable<ReceivableItemDTO> obj = (from u in context.Database.SqlQuery<ReceivableItemDTO>(strCommand)
                                                      select new ReceivableItemDTO
                                                      {
                                                          OrderDetailGUID = u.OrderDetailGUID,
                                                          OrderDetailRequiredDate = u.OrderDetailRequiredDate,
                                                          OrderGUID = u.OrderGUID,
                                                          OrderNumber = u.OrderNumber,
                                                          OrderReleaseNumber = u.OrderReleaseNumber,
                                                          OrderStatus = u.OrderStatus,
                                                          OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)u.OrderStatus).ToString()),
                                                          OrderSupplierID = u.OrderSupplierID,
                                                          OrderSupplierName = u.OrderSupplierName,
                                                          ReceiveBinID = u.ReceiveBinID,
                                                          ReceiveBinName = u.ReceiveBinName,
                                                          RequestedQuantity = u.RequestedQuantity,
                                                          ApprovedQuantity = u.ApprovedQuantity,
                                                          ItemGUID = u.ItemGUID,
                                                          ItemNumber = u.ItemNumber,
                                                          ItemDescription = u.ItemDescription,
                                                          OrderRequiredDate = u.OrderRequiredDate,
                                                          ItemType = u.ItemType,
                                                          TotalRecords = u.TotalRecords,
                                                          OrderCreated = u.OrderCreated,
                                                          OrderCreatedByID = u.OrderCreatedByID,
                                                          OrderCreatedByName = u.OrderCreatedByName,
                                                          OrderLastUpdated = u.OrderLastUpdated,
                                                          OrderLastUpdatedByID = u.OrderLastUpdatedByID,
                                                          OrderUpdatedByName = u.OrderUpdatedByName,
                                                          DateCodeTracking = u.DateCodeTracking,
                                                          LotNumberTracking = u.LotNumberTracking,
                                                          SerialNumberTracking = u.SerialNumberTracking,
                                                          ItemAverageCost = u.ItemAverageCost,
                                                          ItemConsignment = u.ItemConsignment,
                                                          ItemCost = u.ItemCost,
                                                          ItemSellPrice = u.ItemSellPrice,
                                                          ItemDefaultLocation = u.ItemDefaultLocation,
                                                          StagingID = u.StagingID,
                                                          ItemID = u.ItemID,
                                                          OrderDetailID = u.OrderDetailID,
                                                          OrderID = u.OrderID,
                                                          ReceivedQuantity = u.ReceivedQuantity,
                                                          ItemDefaultLocationName = u.ItemDefaultLocationName,
                                                          IsPurchase = u.IsPurchase,
                                                          IsTransfer = u.IsTransfer,
                                                          ItemUDF1 = u.ItemUDF1,
                                                          ItemUDF2 = u.ItemUDF2,
                                                          ItemUDF3 = u.ItemUDF3,
                                                          ItemUDF4 = u.ItemUDF4,
                                                          ItemUDF5 = u.ItemUDF5,
                                                          Manufacturer = u.Manufacturer,
                                                          ManufacturerNumber = u.ManufacturerNumber,
                                                          PackingQuantity = u.PackingQuantity,
                                                          SupplierName = u.SupplierName,
                                                          SupplierPartNumber = u.SupplierPartNumber,
                                                          UnitName = u.UnitName,
                                                          PackSlipNumber = u.PackSlipNumber,
                                                          ASNNumber = u.ASNNumber,
                                                          InTransitQuantity = u.InTransitQuantity,
                                                          ODPackSlipNumbers = u.ODPackSlipNumbers,
                                                          IsPackSlipNumberMandatory = u.IsPackSlipNumberMandatory,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          ShippingTrackNumber = u.ShippingTrackNumber,
                                                          IsCloseItem = u.IsCloseItem,
                                                          Consignment = u.Consignment,
                                                          RequestedQuantityUOM = u.RequestedQuantityUOM,
                                                          ApprovedQuantityUOM = u.ApprovedQuantityUOM,
                                                          ReceivedQuantityUOM = u.ReceivedQuantityUOM,
                                                          OrderUOMValue = u.OrderUOMValue,
                                                          IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                          OrderItemCost = u.OrderItemCost,
                                                      }).AsParallel().ToList();
                return obj;
            }

        }

        private string GetCommaSaperatedSearchPara(string val)
        {
            string[] ss = val.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string inPara = string.Empty;
            if (ss != null && ss.Length > 0)
            {
                foreach (var item in ss)
                {
                    if (!string.IsNullOrEmpty(inPara) && inPara.Length > 0)
                    {
                        inPara += ",";
                    }

                    inPara += "'" + item + "'";
                }

            }
            return inPara;
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
        public IEnumerable<ReceivableItemDTO> GetALLReceiveListByPaging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderStatusIn, List<long> SupplierIds, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            if (sortColumnName.Contains("OrderStatusText"))
            {
                sortColumnName = "OrderStatus";
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string StatusQuery = OrderStatusIn;
                string NarrowSearchQry = "";
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string[] FieldsPara = Fields[1].Split('@');

                    DateTime FromdDate = DateTime.Now;
                    DateTime ToDate = DateTime.Now;

                    if (FieldsPara.Length > 0 && !string.IsNullOrWhiteSpace(FieldsPara[0]) && GetCommaSaperatedSearchPara(FieldsPara[0]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " OrderSupplierID IN (" + FieldsPara[0] + ")";

                    }

                    if (FieldsPara.Length > 1 && FieldsPara.Length > 1 && !string.IsNullOrWhiteSpace(FieldsPara[1]) && GetCommaSaperatedSearchPara(FieldsPara[1]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " OrderNumber IN (" + GetCommaSaperatedSearchPara(FieldsPara[1]) + ")";
                    }

                    if (FieldsPara.Length > 2 && !string.IsNullOrWhiteSpace(FieldsPara[2]) && GetCommaSaperatedSearchPara(FieldsPara[2]).Count() > 0)
                    {

                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " OrderCreatedByID IN (" + GetCommaSaperatedSearchPara(FieldsPara[2]) + ")";
                    }

                    if (FieldsPara.Length > 3 && !string.IsNullOrWhiteSpace(FieldsPara[3]) && GetCommaSaperatedSearchPara(FieldsPara[3]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += " OrderLastUpdatedByID IN (" + GetCommaSaperatedSearchPara(FieldsPara[3]) + ")";
                    }

                    //if (FieldsPara.Length > 4 && FieldsPara.Length < 14 && !string.IsNullOrWhiteSpace(FieldsPara[4]) && GetCommaSaperatedSearchPara(FieldsPara[4]).Count() > 0)
                    if (FieldsPara.Length > 4 && !string.IsNullOrWhiteSpace(FieldsPara[4]) && GetCommaSaperatedSearchPara(FieldsPara[4]).Count() > 0)
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                        ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[4].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += @" Convert(Datetime,OrderCreated) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";

                    }

                    if (FieldsPara.Length > 5  && !string.IsNullOrWhiteSpace(FieldsPara[5]) && GetCommaSaperatedSearchPara(FieldsPara[5]).Count() > 0)
                    {
                        FromdDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone);
                        ToDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[5].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone);
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";

                        NarrowSearchQry += @" Convert(Datetime,OrderLastUpdated) Between Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";

                    }
                    if (FieldsPara.Length > 6 && !string.IsNullOrWhiteSpace(FieldsPara[6]) && GetCommaSaperatedSearchPara(FieldsPara[6]).Count() > 0)
                    {
                        if (FieldsPara[6].Contains("1")) //This Week
                        {
                            FromdDate = DateTime.UtcNow;
                            ToDate = DateTime.UtcNow.AddDays(-7);
                            if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";

                            NarrowSearchQry += @" Convert(Datetime,ReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }
                        if (FieldsPara[6].Contains("2")) // Previous Week
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-7);
                            ToDate = DateTime.UtcNow.AddDays(-14);
                            if (FieldsPara[6].Contains("1"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";
                            NarrowSearchQry += @" Convert(Datetime,ReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }
                        if (FieldsPara[6].Contains("3")) //  2-3 Week
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-14);
                            ToDate = DateTime.UtcNow.AddDays(-21);
                            if (FieldsPara[6].Contains("1") || FieldsPara[6].Contains("2"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";
                            NarrowSearchQry += @" Convert(Datetime,ReceivedDate) Between Convert(Datetime,'" + ToDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) AND Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }
                        if (FieldsPara[6].Contains("4")) // > 3 weeks
                        {
                            FromdDate = DateTime.UtcNow.AddDays(-21);
                            if (FieldsPara[6].Contains("1") || FieldsPara[6].Contains("2") || FieldsPara[6].Contains("3"))
                                NarrowSearchQry += " OR ";
                            else if (!string.IsNullOrEmpty(NarrowSearchQry))
                                NarrowSearchQry += " AND ";
                            NarrowSearchQry += @" Convert(Datetime,ReceivedDate) <= Convert(Datetime,'" + FromdDate.ToString("dd-MM-yyyy HH:mm:ss") + "',105) ";
                        }

                    }
                    if (FieldsPara.Length > 7 && !string.IsNullOrWhiteSpace(FieldsPara[7]) && GetCommaSaperatedSearchPara(FieldsPara[7]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF1 IN (" + GetCommaSaperatedSearchPara(FieldsPara[7]) + ")";

                    }

                    if (FieldsPara.Length > 8 && !string.IsNullOrWhiteSpace(FieldsPara[8]) && GetCommaSaperatedSearchPara(FieldsPara[8]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF2 IN (" + GetCommaSaperatedSearchPara(FieldsPara[8]) + ")";
                    }


                    if (FieldsPara.Length > 9 && !string.IsNullOrWhiteSpace(FieldsPara[9]) && GetCommaSaperatedSearchPara(FieldsPara[9]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF3 IN (" + GetCommaSaperatedSearchPara(FieldsPara[9]) + ")";

                    }

                    if (FieldsPara.Length > 10 && !string.IsNullOrWhiteSpace(FieldsPara[10]) && GetCommaSaperatedSearchPara(FieldsPara[10]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF4 IN (" + GetCommaSaperatedSearchPara(FieldsPara[10]) + ")";
                    }

                    if (FieldsPara.Length > 11 && !string.IsNullOrWhiteSpace(FieldsPara[11]) && GetCommaSaperatedSearchPara(FieldsPara[11]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF5 IN (" + GetCommaSaperatedSearchPara(FieldsPara[11]) + ")";
                    }


                    if (FieldsPara.Length > 12 && !string.IsNullOrWhiteSpace(FieldsPara[12]) && GetCommaSaperatedSearchPara(FieldsPara[12]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF6 IN (" + GetCommaSaperatedSearchPara(FieldsPara[12]) + ")";
                    }
                    if (FieldsPara.Length > 13 && !string.IsNullOrWhiteSpace(FieldsPara[13]) && GetCommaSaperatedSearchPara(FieldsPara[13]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF7 IN (" + GetCommaSaperatedSearchPara(FieldsPara[13]) + ")";
                    }
                    if (FieldsPara.Length > 14 && !string.IsNullOrWhiteSpace(FieldsPara[14]) && GetCommaSaperatedSearchPara(FieldsPara[14]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF8 IN (" + GetCommaSaperatedSearchPara(FieldsPara[14]) + ")";
                    }
                    if (FieldsPara.Length > 15 && !string.IsNullOrWhiteSpace(FieldsPara[15]) && GetCommaSaperatedSearchPara(FieldsPara[15]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF9 IN (" + GetCommaSaperatedSearchPara(FieldsPara[15]) + ")";
                    }
                    if (FieldsPara.Length > 16 && !string.IsNullOrWhiteSpace(FieldsPara[16]) && GetCommaSaperatedSearchPara(FieldsPara[16]).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NarrowSearchQry))
                            NarrowSearchQry += " AND ";
                        NarrowSearchQry += " ITEMUDF10 IN (" + GetCommaSaperatedSearchPara(FieldsPara[16]) + ")";
                    }

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

                }

                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                List<ReceivableItemDTO> obj = new List<ReceivableItemDTO>();
                DataSet ds = new DataSet();
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                ds = SqlHelper.ExecuteDataset(EturnsConnection, "RCV_GetReceivableItemsByPaging", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, NarrowSearchQry, OrderStatusIn, strSupplierIds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dtCart = ds.Tables[0];
                    if (dtCart.Rows.Count > 0)
                    {
                        obj = dtCart.AsEnumerable()
                        .Select(row => new ReceivableItemDTO
                        {
                            OrderDetailGUID = row.Field<Guid>("OrderDetailGUID"),
                            OrderDetailRequiredDate = row.Field<DateTime>("OrderDetailRequiredDate"),
                            OrderGUID = row.Field<Guid>("OrderGUID"),
                            OrderNumber = row.Field<string>("OrderNumber"),
                            OrderReleaseNumber = row.Field<string>("OrderReleaseNumber"),
                            OrderStatus = row.Field<int>("OrderStatus"),
                            OrderStatusText = ResOrder.GetOrderStatusText(((eTurns.DTO.OrderStatus)row.Field<int>("OrderStatus")).ToString()),
                            OrderSupplierID = row.Field<Int64?>("OrderSupplierID"),
                            OrderSupplierName = row.Field<string>("OrderSupplierName"),
                            ReceiveBinID = row.Field<Int64?>("ReceiveBinID"),
                            ReceiveBinName = row.Field<string>("ReceiveBinName"),
                            RequestedQuantity = row.Field<double>("RequestedQuantity"),
                            ApprovedQuantity = row.Field<double>("ApprovedQuantity"),
                            ItemGUID = row.Field<Guid>("ItemGUID"),
                            ItemNumber = row.Field<string>("ItemNumber"),
                            ItemDescription = row.Field<string>("ItemDescription"),
                            OrderRequiredDate = row.Field<DateTime>("OrderRequiredDate"),
                            ItemType = row.Field<int>("ItemType"),
                            TotalRecords = row.Field<int>("TotalRecords"),
                            OrderCreated = row.Field<DateTime>("OrderCreated"),
                            OrderCreatedByID = row.Field<Int64>("OrderCreatedByID"),
                            OrderCreatedByName = row.Field<string>("OrderCreatedByName"),
                            OrderLastUpdated = row.Field<DateTime>("OrderLastUpdated"),
                            OrderLastUpdatedByID = row.Field<Int64>("OrderLastUpdatedByID"),
                            OrderUpdatedByName = row.Field<string>("OrderUpdatedByName"),
                            DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                            LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                            SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                            ItemAverageCost = row.Field<double>("ItemAverageCost"),
                            ItemConsignment = row.Field<bool>("ItemConsignment"),
                            ItemCost = row.Field<double>("ItemCost"),
                            ItemSellPrice = row.Field<double>("ItemSellPrice"),
                            ItemDefaultLocation = row.Field<Int64>("ItemDefaultLocation"),
                            StagingID = row.Field<Int64>("StagingID"),
                            ItemID = row.Field<Int64>("ItemID"),
                            OrderDetailID = row.Field<Int64>("OrderDetailID"),
                            OrderID = row.Field<Int64>("OrderID"),
                            ReceivedQuantity = row.Field<double>("ReceivedQuantity"),
                            ItemDefaultLocationName = row.Field<string>("ItemDefaultLocationName"),
                            IsPurchase = row.Field<bool>("IsPurchase"),
                            IsTransfer = row.Field<bool>("IsTransfer"),
                            ItemUDF1 = row.Field<string>("ItemUDF1"),
                            ItemUDF2 = row.Field<string>("ItemUDF2"),
                            ItemUDF3 = row.Field<string>("ItemUDF3"),
                            ItemUDF4 = row.Field<string>("ItemUDF4"),
                            ItemUDF5 = row.Field<string>("ItemUDF5"),
                            ItemUDF6 = row.Field<string>("ItemUDF6"),
                            ItemUDF7 = row.Field<string>("ItemUDF7"),
                            ItemUDF8 = row.Field<string>("ItemUDF8"),
                            ItemUDF9 = row.Field<string>("ItemUDF9"),
                            ItemUDF10 = row.Field<string>("ItemUDF10"),
                            Manufacturer = row.Field<string>("Manufacturer"),
                            ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                            PackingQuantity = row.Field<double>("PackingQuantity"),
                            SupplierName = row.Field<string>("SupplierName"),
                            SupplierPartNumber = row.Field<string>("SupplierPartNumber"),
                            UnitName = row.Field<string>("UnitName"),
                            PackSlipNumber = row.Field<string>("PackSlipNumber"),
                            ASNNumber = row.Field<string>("ASNNumber"),
                            InTransitQuantity = row.Field<double?>("InTransitQuantity"),
                            ODPackSlipNumbers = row.Field<string>("ODPackSlipNumbers"),
                            IsPackSlipNumberMandatory = row.Field<bool>("IsPackSlipNumberMandatory"),
                            ShippingTrackNumber = row.Field<string>("ShippingTrackNumber"),
                            StagingName = row.Field<string>("StagingName"),
                            //ItemSupplierID = row.Field<Int64?>("ItemSupplierID"),
                            AddedFrom = row.Field<string>("AddedFrom"),
                            EditedFrom = row.Field<string>("EditedFrom"),
                            ReceivedOn = row.Field<DateTime>("ReceivedOn"),
                            ReceivedOnWeb = row.Field<DateTime>("ReceivedOnWeb"),
                            IsCloseItem = row.Field<bool?>("IsCloseItem"),
                            IsEDIOrder = row.Field<bool?>("IsEDIOrder"),
                            CostUOM = row.Field<string>("CostUOM"),
                            OrderUOM = row.Field<string>("OrderUOM"),
                            OrderUOMValue = row.Field<int?>("OrderUOMValue"),
                            OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                            ItemMinimumQuantity = row.Field<double>("ItemMinimumQuantity"),
                            ItemMaximumQuantity = row.Field<double>("ItemMaximumQuantity"),
                            IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                            BinOnHandQTY = row.Field<double?>("BinOnHandQTY"),
                            OrderItemCost = row.Field<double?>("OrderItemCost"),
                            OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                            UDF1 = row.Field<string>("UDF1"),
                            UDF2 = row.Field<string>("UDF2"),
                            UDF3 = row.Field<string>("UDF3"),
                            UDF4 = row.Field<string>("UDF4"),
                            UDF5 = row.Field<string>("UDF5"),
                            AttachmentFileNames = getReceiveFileAttachement(row.Field<Guid>("OrderDetailGUID")),
                            IsBackOrdered = row.Field<bool>("IsBackOrdered"),
                            OrderLineException = row.Field<bool?>("OrderLineException"),
                            OrderLineExceptionDesc = row.Field<string>("OrderLineExceptionDesc")
                        }).ToList();
                    }
                }
                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }

                return obj;
            }
        }

        public List<FileAttachmentReceiveList> getReceiveFileAttachement(Guid orderDetailGuid)
        {
            ReceiveFileDetailsDAL receiveFileDetailsDAL = new ReceiveFileDetailsDAL(base.DataBaseName);
            List<FileAttachmentReceiveList> filenames = new List<FileAttachmentReceiveList>();
            List<ReceiveFileDetailDTO> objOrderImageDetail = receiveFileDetailsDAL.GetReceiveFileByGuidPlain(orderDetailGuid).ToList();
            FileAttachmentReceiveList objFileReceive = null;
            foreach (var item in objOrderImageDetail)
            {
                objFileReceive = new FileAttachmentReceiveList();
                objFileReceive.FileGUID = item.GUID;
                objFileReceive.FileName = item.FileName;
                filenames.Add(objFileReceive);
                objFileReceive = null;
            }
            return filenames;
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
        public IEnumerable<ReceivableItemDTO> GetALLReceiveListByPagingForDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string OrderStatusIn, string SupplierIds, out List<ReceivableItemDTO> SupplierList, List<long> UserSupplierIds)
        {
            if (!string.IsNullOrEmpty(sortColumnName) && sortColumnName.Contains("OrderStatusText"))
            {
                sortColumnName = "OrderStatus";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string StatusQuery = OrderStatusIn;
                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
                string strSelectedSupplierIds = string.Empty;

                if (!string.IsNullOrEmpty(SupplierIds) && !string.IsNullOrWhiteSpace(SupplierIds))
                    strSelectedSupplierIds = SupplierIds;

                string strUserSupplierIds = string.Empty;

                if (UserSupplierIds != null && UserSupplierIds.Any())
                {
                    strUserSupplierIds = string.Join(",", UserSupplierIds);
                }

                List<ReceivableItemDTO> obj = new List<ReceivableItemDTO>();
                DataSet ds = new DataSet();
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetReceivableItemsByPagingForDashboard", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, SearchTerm, OrderStatusIn, strSelectedSupplierIds, strUserSupplierIds);
                List<ReceivableItemDTO> receivable = new List<ReceivableItemDTO>();
                TotalCount = 0;

                if (ds != null && ds.Tables.Count > 0)
                {
                    SupplierList = DataTableHelper.ToList<ReceivableItemDTO>(ds.Tables[0]);

                    if (ds.Tables.Count > 1)
                    {
                        receivable = DataTableHelper.ToList<ReceivableItemDTO>(ds.Tables[1]);

                        if (receivable != null && receivable.Count() > 0)
                        {
                            TotalCount = receivable.ElementAt(0).TotalRecords;
                        }
                    }
                }
                else
                {
                    SupplierList = null;
                }

                return receivable;
            }
        }

        public IEnumerable<OrderDetailsDTO> CreateDirectReceiveOrder(string OrderNumber, Guid ItemGuid, double ReqQty, Int64 LocationID, DateTime ReceiveDate, double ReceiveQty, Int64 RoomID, Int64 CompanyID, Int64 UserID, string OrderNumberForSorting, string EditedFrom, DateTime ReceivedOn, string addedFrom, DateTime ReceivedOnWeb, string PackSlipNumber, string ShippingTrackNumber, long SessionUserId, Int64? SupplierID = 0)
        {
            string strCommand = "EXEC RCV_CreateDirectReceiveOrder @ordernumber,@itemguid,@reqqty,@locationid,@receivedate,@receiveqty,@userid,@roomid,@companyid,@ordershort,@AddedFrom,@ReceivedOnWeb,@EditedFrom,@ReceivedOn,@PackSlipNumber,@ShippingTrackNumber,@Supplier";

            SqlParameter OrderNumberParam = new SqlParameter() { ParameterName = "ordernumber", Value = OrderNumber, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ItemGuidParam = new SqlParameter() { ParameterName = "itemguid", Value = ItemGuid, SqlDbType = SqlDbType.UniqueIdentifier };
            SqlParameter ReqQtyParam = new SqlParameter() { ParameterName = "reqqty", Value = ReqQty, SqlDbType = SqlDbType.Float };
            SqlParameter LocationIDParam = new SqlParameter() { ParameterName = "locationid", Value = LocationID, SqlDbType = SqlDbType.BigInt };
            SqlParameter ReceiveDateParam = new SqlParameter() { ParameterName = "receivedate", Value = ReceiveDate.ToString("yyyy-MM-dd"), SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceiveQtyParam = new SqlParameter() { ParameterName = "receiveqty", Value = ReceiveQty, SqlDbType = SqlDbType.Float };
            SqlParameter UserIDParam = new SqlParameter() { ParameterName = "userid", Value = UserID, SqlDbType = SqlDbType.BigInt };
            SqlParameter RoomIDParam = new SqlParameter() { ParameterName = "roomid", Value = RoomID, SqlDbType = SqlDbType.BigInt };
            SqlParameter CompanyIDParam = new SqlParameter() { ParameterName = "companyid", Value = CompanyID, SqlDbType = SqlDbType.BigInt };
            SqlParameter OrderShortParam = new SqlParameter() { ParameterName = "ordershort", Value = CommonDAL.GetSortingString(OrderNumber), SqlDbType = SqlDbType.NVarChar };
            SqlParameter AddedFromParam = new SqlParameter() { ParameterName = "AddedFrom", Value = addedFrom, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceivedOnWebParam = new SqlParameter() { ParameterName = "ReceivedOnWeb", Value = ReceivedOnWeb, SqlDbType = SqlDbType.DateTime };
            SqlParameter EditedFromParam = new SqlParameter() { ParameterName = "EditedFrom", Value = EditedFrom, SqlDbType = SqlDbType.NVarChar };
            SqlParameter ReceivedOnParam = new SqlParameter() { ParameterName = "ReceivedOn", Value = ReceivedOn, SqlDbType = SqlDbType.DateTime };
            SqlParameter PackSlipNumberParam;

            if (!string.IsNullOrEmpty(PackSlipNumber))
            {
                PackSlipNumberParam = new SqlParameter() { ParameterName = "PackSlipNumber", Value = PackSlipNumber, SqlDbType = SqlDbType.NVarChar };
            }
            else
            {
                PackSlipNumberParam = new SqlParameter() { ParameterName = "PackSlipNumber", Value = (object)DBNull.Value, SqlDbType = SqlDbType.NVarChar };
            }
            SqlParameter ShippingTrackNumberParam;
            if (!string.IsNullOrEmpty(ShippingTrackNumber))
            {
                ShippingTrackNumberParam = new SqlParameter() { ParameterName = "ShippingTrackNumber", Value = ShippingTrackNumber, SqlDbType = SqlDbType.NVarChar };
            }
            else
            {
                ShippingTrackNumberParam = new SqlParameter() { ParameterName = "ShippingTrackNumber", Value = (object)DBNull.Value, SqlDbType = SqlDbType.NVarChar };
            }

            SqlParameter SupplierIDParam;
            SupplierIDParam = new SqlParameter() { ParameterName = "Supplier", Value = SupplierID, SqlDbType = SqlDbType.BigInt };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<Guid> OrderGuid = (from u in context.Database.SqlQuery<Guid>(strCommand, OrderNumberParam, ItemGuidParam, ReqQtyParam, LocationIDParam, ReceiveDateParam, ReceiveQtyParam, UserIDParam, RoomIDParam, CompanyIDParam, OrderShortParam, AddedFromParam, ReceivedOnWebParam, EditedFromParam, ReceivedOnParam, PackSlipNumberParam, ShippingTrackNumberParam, SupplierIDParam)
                                               select u);

                Guid OrdGuid = OrderGuid.FirstOrDefault();
                OrderMasterDAL objOrderDAL = new OrderMasterDAL(base.DataBaseName);
                OrderMasterDTO objOrderDTO = objOrderDAL.GetOrderByGuidPlain(OrdGuid);

                new AutoSequenceDAL(base.DataBaseName).UpdateNextOrderNumber(RoomID, CompanyID, objOrderDTO.Supplier.GetValueOrDefault(0), objOrderDTO.OrderNumber, SessionUserId, objOrderDTO.ReleaseNumber);

                OrderDetailsDAL objOrderDetailDAL = new OrderDetailsDAL(base.DataBaseName);
                var tmpsupplierIds = new List<long>();
                List<OrderDetailsDTO> obj = objOrderDetailDAL.GetOrderDetailByOrderGUIDFull(OrdGuid, RoomID, CompanyID);

                return obj;
            }

        }

        public IEnumerable<ReceivableItemDTO> GetReceiveListForDashboardChart(out IEnumerable<ReceivableItemDTO> SupplierList, int StartRowIndex, int MaxRows, out int TotalCount, string sortColumnName, long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string OrderStatusIn, List<long> SupplierIds, string SelectedSupplierIds)
        {
            if (sortColumnName.Contains("OrderStatusText"))
            {
                sortColumnName = "OrderStatus";
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string StatusQuery = OrderStatusIn;
                TotalCount = 0;

                if (MaxRows < 1)
                {
                    MaxRows = 10;
                }

                StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
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

                List<ReceivableItemDTO> receives = new List<ReceivableItemDTO>();
                DataSet ds = new DataSet();
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
                ds = SqlHelper.ExecuteDataset(EturnsConnection, "GetReceiveListForDashboardChart", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, sortColumnName, OrderStatusIn, strSupplierIds, strSelectedSupplierIds);

                if (ds != null && ds.Tables.Count > 0)
                {
                    SupplierList = DataTableHelper.ToList<ReceivableItemDTO>(ds.Tables[0]);

                    if (ds.Tables.Count > 1)
                    {
                        receives = DataTableHelper.ToList<ReceivableItemDTO>(ds.Tables[1]);

                        if (receives != null && receives.Count() > 0)
                        {
                            TotalCount = receives.ElementAt(0).TotalRecords;
                        }
                    }
                }
                else
                {
                    SupplierList = null;
                }

                return receives;
            }
        }

        public List<ReceiveFileDetailDTO> getReceiveFileByRoom(long CompanyID,long RoomID)
        {
            try
            {
                ReceiveFileDetailsDAL receiveFileDetailsDAL = new ReceiveFileDetailsDAL(base.DataBaseName);
                return receiveFileDetailsDAL.GetReceiveFileByRoom(CompanyID, RoomID);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}


