using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class OrderDetailsDALPart1 : eTurnsBaseDAL
    {
        public OrderMasterDTO Update_And_Return_Order_NoOfItem_And_OrderCost(Guid OrderGuid)
        {
            Int64 CompanyID = 0;
            Int64 RoomID = 0;
            int NoOfItem = 0;
            double OrderPrice = 0;
            double OrderCost = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<OrderDetail> obj = context.OrderDetails.Where(x => (x.OrderGUID ?? Guid.Empty) == OrderGuid && (x.IsDeleted ?? false) == false && (x.IsArchived ?? false) == false).ToList();
                if (obj != null && obj.Count > 0)
                {
                    RoomID = obj[0].Room.GetValueOrDefault(0);
                    CompanyID = obj[0].CompanyID.GetValueOrDefault(0);
                    NoOfItem = obj.Count;
                    foreach (var item in obj)
                    {
                        if (item.ItemGUID.HasValue)
                        {
                            double? Itemcost = new ItemMasterDAL(base.DataBaseName).CalculateAndGetItemCost(item.ItemGUID.Value, (double)((context.ItemMasters.Where(x => x.GUID == (item.ItemGUID ?? Guid.Empty)).FirstOrDefault()).Cost ?? 0), RoomID, CompanyID);
                            OrderPrice = OrderPrice + (Itemcost.GetValueOrDefault(0) * Convert.ToDouble(item.RequestedQuantity ?? 0));

                            double? ItemPrice = new ItemMasterDAL(base.DataBaseName).CalculateAndGetItemCost(item.ItemGUID.Value, (double)((context.ItemMasters.Where(x => x.GUID == (item.ItemGUID ?? Guid.Empty)).FirstOrDefault()).SellPrice ?? 0), RoomID, CompanyID);
                            OrderCost = OrderCost + (ItemPrice.GetValueOrDefault(0) * Convert.ToDouble(item.RequestedQuantity ?? 0));
                        }
                    }
                }
            }

            OrderMasterDAL objOMDAL = new OrderMasterDAL(base.DataBaseName);
            OrderMasterDTO objOrderDTO = objOMDAL.GetRecord(OrderGuid, RoomID, CompanyID);
            objOrderDTO.NoOfLineItems = NoOfItem;
            objOrderDTO.OrderCost = OrderPrice;
            objOrderDTO.OrderPrice = OrderCost;

            return objOrderDTO;

        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<OrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM OrderDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new OrderDetailsDTO
                        {
                            ID = u.ID,
                            OrderGUID = u.OrderGUID,
                            ItemGUID = u.ItemGUID,
                            Bin = u.Bin,
                            RequestedQuantity = u.RequestedQuantity,
                            RequiredDate = u.RequiredDate,
                            ReceivedQuantity = u.ReceivedQuantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            ASNNumber = u.ASNNumber,
                            ApprovedQuantity = u.ApprovedQuantity ?? 0,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            OrderLineItemExtendedCost = u.OrderLineItemExtendedCost.GetValueOrDefault(0),
                            OrderLineItemExtendedPrice = u.OrderLineItemExtendedPrice.GetValueOrDefault(0)
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderDetailsDTO GetRecord(Guid Guid, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, null, Guid, null, null).FirstOrDefault();
        }


        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<OrderDetailsDTO> GetOrderLineItemHistory(Int64 OrderMasterHistoryID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, ";
                qry += " IM.[ID] as ItemID,IM.[GUID] as ItemGUID,IM.ItemNumber as ItemNumber,IM.MinimumQuantity, IM.MaximumQuantity,IM.GLAccountID as GLAccount,";
                qry += " IM.Cost, IM.Markup ,IM.SellPrice,IM.ManufacturerID as Manufacturer,IM.ManufacturerNumber,IM.SupplierID as Supplier,IM.SupplierPartNo,";
                qry += " IM.Description,IM.UOMID ,IM.OnOrderQuantity,IM.InTransitquantity,IM.[UDF1] as ItemUDF1,IM.[UDF2] as ItemUDF2,";
                qry += " IM.[UDF3] as ItemUDF3, IM.[UDF4] as ItemUDF4,IM.[UDF5] as ItemUDF5,UM.Unit as UOM, IM.ItemType ";
                qry += " FROM OrderDetails_History A LEFT OUTER  JOIN ItemMaster IM on IM.ID= A.ItemID";
                qry += " LEFT OUTER  JOIN UnitMaster UM on UM.ID= IM.UOMID LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID ";
                qry += " LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ";
                qry += "  WHERE A.OrderMasterHistroyID = " + OrderMasterHistoryID.ToString() + " ";

                return (from u in context.ExecuteStoreQuery<OrderDetailsDTO>(qry)
                        select new OrderDetailsDTO
                        {
                            ID = u.ID,
                            OrderGUID = u.OrderGUID,
                            ItemGUID = u.ItemGUID,
                            Bin = u.Bin,
                            RequestedQuantity = u.RequestedQuantity,
                            RequiredDate = u.RequiredDate,
                            ReceivedQuantity = u.ReceivedQuantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            Action = u.Action,
                            HistoryID = u.HistoryID,
                            IsHistory = true,
                            //ItemDetail = new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.GetValueOrDefault(Guid.Empty).ToString(), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)),
                            ApprovedQuantity = u.ApprovedQuantity ?? 0,
                            ASNNumber = u.ASNNumber,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            OrderLineItemExtendedCost = u.OrderLineItemExtendedCost.GetValueOrDefault(0),
                            OrderLineItemExtendedPrice = u.OrderLineItemExtendedPrice.GetValueOrDefault(0)
                        }).AsParallel().ToList();
            }

        }

        public List<OrderDetailsDTO> GetOrderedRecord(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 SupplierID = 0)
        {
            return DB_GetCachedData(RoomID, CompanyID, IsDeleted, IsArchived, null, null, SupplierID, OrderGuid).ToList();
        }

        private IEnumerable<OrderDetailsDTO> DB_GetCachedData(Int64? RoomID, Int64? CompanyID, bool? IsDeleted, bool? IsArchived, Int64? ID, Guid? DetailGuid, Int64? SupplierID, Guid? OrderGUID)
        {



            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@CompnayID", (CompanyID.GetValueOrDefault(0) > 0) ? CompanyID.GetValueOrDefault(0) :(object)DBNull.Value),
                        new SqlParameter("@RoomID", (RoomID.GetValueOrDefault(0) > 0) ? RoomID.GetValueOrDefault(0) :(object)DBNull.Value),
                        new SqlParameter("@IsDeleted",(IsDeleted == null ? (object)DBNull.Value : IsDeleted.GetValueOrDefault(false))),
                        new SqlParameter("@IsArchived",(IsArchived  == null ? (object)DBNull.Value : IsArchived.GetValueOrDefault(false))),
                        new SqlParameter("@ID",  (ID.GetValueOrDefault(0) > 0) ? ID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@GUID", (DetailGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty) ? DetailGuid.GetValueOrDefault(Guid.Empty) : (object)DBNull.Value),
                        new SqlParameter("@SupplierID", (SupplierID.GetValueOrDefault(0) > 0) ? SupplierID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@OrderGuid", (OrderGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty) ? OrderGUID.GetValueOrDefault(Guid.Empty) : (object)DBNull.Value)

                    };


                string strCommand = " EXEC [Orddtl_GetOrderDetailData] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@ID,@GUID,@SupplierID,@OrderGuid ";


                context.CommandTimeout = 150;
                IEnumerable<OrderDetailsDTO> obj = (from u in context.ExecuteStoreQuery<OrderDetailsDTO>(strCommand, params1)
                                                    select new OrderDetailsDTO
                                                    {
                                                        ID = u.ID,
                                                        ApprovedQuantity = u.ApprovedQuantity,
                                                        ASNNumber = u.ASNNumber,
                                                        Bin = u.Bin,
                                                        BinName = u.BinName,
                                                        IsEDISent = u.IsEDISent,
                                                        ItemGUID = u.ItemGUID,
                                                        OrderGUID = u.OrderGUID,
                                                        ReceivedQuantity = u.ReceivedQuantity,
                                                        RequestedQuantity = u.RequestedQuantity.GetValueOrDefault(0),
                                                        RequiredDate = u.RequiredDate,
                                                        Created = u.Created,
                                                        LastUpdated = u.LastUpdated,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        Room = u.Room,
                                                        IsDeleted = u.IsDeleted,
                                                        IsArchived = u.IsArchived,
                                                        CompanyID = u.CompanyID,
                                                        GUID = u.GUID,
                                                        CreatedByName = u.CreatedByName,
                                                        UpdatedByName = u.UpdatedByName,
                                                        RoomName = u.RoomName,
                                                        InTransitQuantity = u.InTransitQuantity,
                                                        IsEDIRequired = u.IsEDIRequired,
                                                        LastEDIDate = u.LastEDIDate,
                                                        TotalRecords = u.TotalRecords,

                                                        Action = string.Empty,
                                                        HistoryID = 0,
                                                        IsHistory = false,

                                                        AverageCost = u.AverageCost,
                                                        //ItemDetail = null,
                                                        AverageUsage = u.AverageUsage,
                                                        Category = u.Category,
                                                        CategoryID = u.CategoryID,
                                                        Consignment = u.Consignment,
                                                        Cost = u.Cost,
                                                        CriticalQuantity = u.CriticalQuantity,
                                                        DateCodeTracking = u.DateCodeTracking,
                                                        DefaultLocation = u.DefaultLocation,
                                                        DefaultLocationName = u.DefaultLocationName,
                                                        DefaultPullQuantity = u.DefaultPullQuantity,
                                                        DefaultReorderQuantity = u.DefaultReorderQuantity,
                                                        ExtendedCost = u.ExtendedCost,
                                                        GLAccount = u.GLAccount,
                                                        GLAccountID = u.GLAccountID,
                                                        ImagePath = u.ImagePath,
                                                        IsBuildBreak = u.IsBuildBreak,
                                                        IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                                                        IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                                        IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                                        IsPurchase = u.IsPurchase,
                                                        IsTransfer = u.IsTransfer,
                                                        ItemDescription = u.ItemDescription,
                                                        ItemID = u.ItemID,
                                                        ItemInTransitQuantity = u.ItemInTransitQuantity,
                                                        ItemNumber = u.ItemNumber,
                                                        ItemType = u.ItemType,
                                                        ItemUDF1 = u.ItemUDF1,
                                                        ItemUDF2 = u.ItemUDF2,
                                                        ItemUDF3 = u.ItemUDF3,
                                                        ItemUDF4 = u.ItemUDF4,
                                                        ItemUDF5 = u.ItemUDF5,
                                                        InventoryClassification = u.InventoryClassification,
                                                        ItemUniqueNumber = u.ItemUniqueNumber,
                                                        WeightPerPiece = u.WeightPerPiece,
                                                        UPC = u.UPC,
                                                        UOMID = u.UOMID,
                                                        UNSPSC = u.UNSPSC,

                                                        LeadTimeInDays = u.LeadTimeInDays,
                                                        Link1 = u.Link1,
                                                        Link2 = u.Link2,
                                                        LongDescription = u.LongDescription,
                                                        LotNumberTracking = u.LotNumberTracking,
                                                        Manufacturer = u.Manufacturer,
                                                        ManufacturerID = u.ManufacturerID,
                                                        ManufacturerNumber = u.ManufacturerNumber,
                                                        Markup = u.Markup,
                                                        MaximumQuantity = u.MaximumQuantity,
                                                        MinimumQuantity = u.MinimumQuantity,
                                                        OnHandQuantity = u.OnHandQuantity,
                                                        OnOrderQuantity = u.OnOrderQuantity,
                                                        OnReturnQuantity = u.OnReturnQuantity,
                                                        OnTransferQuantity = u.OnTransferQuantity,
                                                        PackingQuantity = u.PackingQuantity,
                                                        PricePerTerm = u.PricePerTerm,
                                                        RequisitionedQuantity = u.RequisitionedQuantity,
                                                        SellPrice = u.SellPrice,
                                                        SerialNumberTracking = u.SerialNumberTracking,
                                                        StagedQuantity = u.StagedQuantity,
                                                        SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                        SupplierID = u.SupplierID,
                                                        SupplierName = u.SupplierName,
                                                        SupplierPartNo = u.SupplierPartNo,
                                                        Taxable = u.Taxable,
                                                        Unit = u.Unit,
                                                        Trend = u.Trend,
                                                        Turns = u.Turns,
                                                        ItemBlanketPO = u.ItemBlanketPO,
                                                        ODPackSlipNumbers = u.ODPackSlipNumbers,
                                                        ReceivedOn = (u.ReceivedOn == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(u.ReceivedOn)),
                                                        ReceivedOnWeb = (u.ReceivedOnWeb == null ? DateTimeUtility.DateTimeNow : Convert.ToDateTime(u.ReceivedOnWeb)),
                                                        AddedFrom = (u.AddedFrom == null ? "Web" : Convert.ToString(u.AddedFrom)),
                                                        EditedFrom = (u.EditedFrom == null ? "Web" : Convert.ToString(u.EditedFrom)),
                                                        IsCloseItem = u.IsCloseItem,
                                                        CostUOM = u.CostUOM,
                                                        OrderUOM = u.OrderUOM,
                                                        ControlNumber = u.ControlNumber,
                                                        LineNumber = u.LineNumber,
                                                        IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                                                        Comment = u.Comment,
                                                        CostUOMValue = u.CostUOMValue,
                                                        OrderUOMValue = u.OrderUOMValue,
                                                        OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                                                        UDF1 = u.UDF1,
                                                        UDF2 = u.UDF2,
                                                        UDF3 = u.UDF3,
                                                        UDF4 = u.UDF4,
                                                        UDF5 = u.UDF5,
                                                        OrderCost = u.OrderCost,
                                                        OrderLineItemExtendedCost = u.OrderLineItemExtendedCost.GetValueOrDefault(0),
                                                        OrderLineItemExtendedPrice = u.OrderLineItemExtendedPrice.GetValueOrDefault(0),
                                                        RequestedQuantityUOM = u.RequestedQuantityUOM.GetValueOrDefault(0),
                                                        ApprovedQuantityUOM = u.ApprovedQuantityUOM.GetValueOrDefault(0),
                                                        ReceivedQuantityUOM = u.ReceivedQuantityUOM.GetValueOrDefault(0),
                                                        InTransitQuantityUOM = u.InTransitQuantityUOM.GetValueOrDefault(0),
                                                        OrderPrice = u.OrderPrice,
                                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                    }).AsParallel().ToList();

                return obj;
            }


        }


        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<OrderDetailsDTO> GetHistoryRecordsListByOrderID(string OrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<OrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM OrderDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.OrderGUID= '" + OrderGUID + "'")
                        select new OrderDetailsDTO
                        {
                            ID = u.ID,
                            OrderGUID = u.OrderGUID,
                            ItemGUID = u.ItemGUID,
                            Bin = u.Bin,
                            RequestedQuantity = u.RequestedQuantity,
                            RequiredDate = u.RequiredDate,
                            ReceivedQuantity = u.ReceivedQuantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            IsEDISent = u.IsEDISent,
                            Action = u.Action,
                            HistoryID = u.HistoryID,
                            IsHistory = true,
                            BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetBinByID(u.Bin.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).BinNumber : string.Empty,
                            //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, u.Bin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,
                            //ItemDetail = u.ItemGUID.HasValue ? new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.Value.ToString(), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)) : null,
                            ApprovedQuantity = u.ApprovedQuantity ?? 0,
                            ASNNumber = u.ASNNumber,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            OrderLineItemExtendedCost = u.OrderLineItemExtendedCost.GetValueOrDefault(0),
                            OrderLineItemExtendedPrice = u.OrderLineItemExtendedPrice.GetValueOrDefault(0)
                        }).ToList();
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<OrderDetailsDTO> GetOrderedRecord(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 SupplierID = 0)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, null, null, null, OrderGuid).ToList();
        }

        private void CreateItemMasterCache(ItemMasterDTO oItemMasterDTO)
        {
            //Get Cached-Media
            IEnumerable<ItemMasterDTO> ObjCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + oItemMasterDTO.CompanyID.ToString());
            if (ObjCache != null)
            {
                List<ItemMasterDTO> objTemp = ObjCache.ToList();
                if (objTemp.Any(x => x.ID == oItemMasterDTO.ID))
                {
                    objTemp.RemoveAll(i => i.ID == oItemMasterDTO.ID);
                }

                ObjCache = objTemp.AsEnumerable();

                List<ItemMasterDTO> tempC = new List<ItemMasterDTO>();
                tempC.Add(oItemMasterDTO);
                IEnumerable<ItemMasterDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable()).AsEnumerable();
                CacheHelper<IEnumerable<ItemMasterDTO>>.AddCacheItem("Cached_ItemMaster_" + oItemMasterDTO.CompanyID.ToString(), NewCache);
            }
        }



        public IEnumerable<OrderDetailsDTO> GetOrderDetailDataWithItemBin(Int64 RoomID, Int64 CompanyID, Guid OrderGUID, Guid ItemGuid, int BinID)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<OrderDetailsDTO> obj = (from u in context.ExecuteStoreQuery<OrderDetailsDTO>(@"SELECT A.*, 
                    B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,OM.OrderStatus,
                    E.ItemNumber,E.Description,E.LongDescription,E.SerialNumberTracking, CM.Category as CategoryName,SM.SupplierName,SM.ID as SupplierID,Om.OrderType ,
                    E.UDF1 AS ItemUDF1,E.UDF2 AS ItemUDF2,E.UDF3 AS ItemUDF3,E.UDF4 AS ItemUDF4,E.UDF5 AS ItemUDF5,RS.CurrencyDecimalDigits,RS.NumberDecimalDigits 
                                                                FROM OrderDetails A 
                                                                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                LEFT OUTER JOIN Room D on A.Room = D.ID  
                                                                LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID 
                                                                Left Outer join CategoryMaster CM on CM.ID = e.CategoryID  
                                                                left Outer join SupplierMaster SM on SM.ID = e.SupplierID 
                                                        left outer join OrderMaster OM on oM.GUID = A.OrderGUID   
                                                                left outer join RegionalSetting RS on A.Room = RS.RoomID
                                                                WHERE A.Isdeleted=0 and A.CompanyID = " + CompanyID.ToString() + " and A.Room =" + RoomID.ToString() + " and A.OrderGUID = '" + OrderGUID.ToString() + "' and A.ItemGuid= '" + ItemGuid.ToString() + "' and A.Bin=" + BinID.ToString())
                                                    select new OrderDetailsDTO
                                                    {
                                                        ID = u.ID,
                                                        OrderGUID = u.OrderGUID,
                                                        ItemGUID = u.ItemGUID,
                                                        Bin = u.Bin,
                                                        RequestedQuantity = u.RequestedQuantity,
                                                        RequiredDate = u.RequiredDate,
                                                        ReceivedQuantity = u.ReceivedQuantity,
                                                        Created = u.Created,
                                                        LastUpdated = u.LastUpdated,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        Room = u.Room,
                                                        IsDeleted = u.IsDeleted,
                                                        IsArchived = u.IsArchived,
                                                        CompanyID = u.CompanyID,
                                                        GUID = u.GUID,
                                                        CreatedByName = u.CreatedByName,
                                                        UpdatedByName = u.UpdatedByName,
                                                        RoomName = u.RoomName,
                                                        IsEDISent = u.IsEDISent,
                                                        Action = u.Action,
                                                        HistoryID = u.HistoryID,
                                                        IsHistory = true,
                                                        BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetBinByID(u.Bin.GetValueOrDefault(0), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).BinNumber : string.Empty,
                                                        //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0), false, false,Guid.Empty, u.Bin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,
                                                        //ItemDetail = u.ItemGUID.HasValue ? new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.Value.ToString(), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)) : null,
                                                        ApprovedQuantity = u.ApprovedQuantity ?? 0,
                                                        ASNNumber = u.ASNNumber,
                                                        AddedFrom = u.AddedFrom,
                                                        EditedFrom = u.EditedFrom,
                                                        ReceivedOn = u.ReceivedOn,
                                                        ReceivedOnWeb = u.ReceivedOnWeb,
                                                        SupplierID = u.SupplierID,
                                                        SupplierName = u.SupplierName,
                                                        LongDescription = u.LongDescription,
                                                        ItemNumber = u.ItemNumber,
                                                        SerialNumberTracking = u.SerialNumberTracking,
                                                        ItemUDF1 = u.ItemUDF1,
                                                        ItemUDF2 = u.ItemUDF2,
                                                        ItemUDF3 = u.ItemUDF3,
                                                        ItemUDF4 = u.ItemUDF4,
                                                        ItemUDF5 = u.ItemUDF5,
                                                        UDF1 = u.UDF1,
                                                        UDF2 = u.UDF2,
                                                        UDF3 = u.UDF3,
                                                        UDF4 = u.UDF4,
                                                        UDF5 = u.UDF5,
                                                        OrderLineItemExtendedCost = u.OrderLineItemExtendedCost.GetValueOrDefault(0),
                                                        OrderLineItemExtendedPrice = u.OrderLineItemExtendedPrice.GetValueOrDefault(0)
                                                    }).AsParallel().ToList();
                if (obj != null && obj.Count() > 0)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }

        }



        public List<KeyValDTO> GetPendingAndClosedOrderItems(string OrderStatusIn, string RoomID, string CompanyID, DateTime? StartDate, DateTime? EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string query = " SELECT DISTINCT Convert(VARCHAR(100),ODV.ItemGUID) as [key],ODV.ItemNumber[value] FROM OrderDetails_View ODV  WHERE ISNULL(ODV.IsDeleted,0) = 0 AND ISNULL(ODV.IsArchived,0) = 0 AND ISNULL(ODV.IsCloseItem,0) = 0 ";

                if (!string.IsNullOrEmpty(OrderStatusIn))
                {
                    query += " And ODV.OrderStatus In (" + OrderStatusIn + ") ";
                }

                if (!string.IsNullOrEmpty(RoomID))
                {
                    query += " And ODV.Room In (" + RoomID + ") ";
                }

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    query += " And ODV.CompanyID In (" + CompanyID + ") ";
                }

                if (StartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    query += " AND (SELECT MIN(Convert(DateTime,Received)) FROM ReceivedOrderTransferDetail ROTD WHERE ROTD.OrderDetailGUID = ODV.[GUID] AND ISNULL(ROTD.IsDeleted,0) =0 AND ISNULL(ROTD.ISArchived,0)=0) >= Convert(DateTime,'" + StartDate.GetValueOrDefault(DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                }
                if (EndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
                {
                    query += " AND (SELECT MAX(Convert(DateTime,Received)) FROM ReceivedOrderTransferDetail ROTD WHERE ROTD.OrderDetailGUID = ODV.[GUID] AND ISNULL(ROTD.IsDeleted,0) =0 AND ISNULL(ROTD.ISArchived,0)=0) <= Convert(DateTime,'" + EndDate.GetValueOrDefault(DateTime.MinValue).ToString("yyyy-MM-dd  HH:mm:ss") + "')";
                }

                List<KeyValDTO> obj = (from u in context.ExecuteStoreQuery<KeyValDTO>(query)
                                       select new KeyValDTO
                                       {
                                           key = u.key,
                                           value = u.value,

                                       }).ToList();

                return obj;
            }

        }


        private void CreateItemLocationDetailsCache(ItemLocationDetail oItemLocationDetail)
        {
            ItemLocationDetailsDTO objDTO = new ItemLocationDetailsDTO();

            objDTO.ID = oItemLocationDetail.ID;
            objDTO.BinID = oItemLocationDetail.BinID;
            objDTO.CustomerOwnedQuantity = oItemLocationDetail.CustomerOwnedQuantity;
            objDTO.ConsignedQuantity = oItemLocationDetail.ConsignedQuantity;
            objDTO.MeasurementID = oItemLocationDetail.MeasurementID;
            objDTO.LotNumber = oItemLocationDetail.LotNumber;
            objDTO.SerialNumber = oItemLocationDetail.SerialNumber;
            objDTO.Expiration = oItemLocationDetail.Expiration;
            objDTO.Received = oItemLocationDetail.Received;
            objDTO.ExpirationDate = oItemLocationDetail.ExpirationDate;
            objDTO.ReceivedDate = oItemLocationDetail.ReceivedDate;
            objDTO.Cost = oItemLocationDetail.Cost;
            objDTO.eVMISensorPort = oItemLocationDetail.eVMISensorPort;
            objDTO.eVMISensorID = oItemLocationDetail.eVMISensorID;
            objDTO.UDF1 = oItemLocationDetail.UDF1;
            objDTO.UDF2 = oItemLocationDetail.UDF2;
            objDTO.UDF3 = oItemLocationDetail.UDF3;
            objDTO.UDF4 = oItemLocationDetail.UDF4;
            objDTO.UDF5 = oItemLocationDetail.UDF5;
            objDTO.GUID = oItemLocationDetail.GUID;
            objDTO.ItemGUID = oItemLocationDetail.ItemGUID;
            objDTO.Created = oItemLocationDetail.Created;
            objDTO.Updated = oItemLocationDetail.Updated;
            objDTO.CreatedBy = oItemLocationDetail.CreatedBy;
            objDTO.LastUpdatedBy = oItemLocationDetail.LastUpdatedBy;
            objDTO.IsDeleted = oItemLocationDetail.IsDeleted;
            objDTO.IsArchived = oItemLocationDetail.IsArchived;
            objDTO.CompanyID = oItemLocationDetail.CompanyID;

            objDTO.Room = oItemLocationDetail.Room;
            objDTO.KitDetailGUID = oItemLocationDetail.KitDetailGUID;
            objDTO.TransferDetailGUID = oItemLocationDetail.TransferDetailGUID;
            objDTO.OrderDetailGUID = oItemLocationDetail.OrderDetailGUID;
            objDTO.IsConsignedSerialLot = oItemLocationDetail.IsConsignedSerialLot;
            objDTO.InitialQuantity = oItemLocationDetail.InitialQuantity;
            objDTO.IsWebEdit = oItemLocationDetail.IsWebEdit;
            objDTO.IsPDAEdit = oItemLocationDetail.IsPDAEdit;
            objDTO.RefWebSelfGUID = oItemLocationDetail.RefWebSelfGUID;
            objDTO.RefPDASelfGUID = oItemLocationDetail.RefPDASelfGUID;
            objDTO.InitialQuantityWeb = oItemLocationDetail.InitialQuantityWeb;
            objDTO.InitialQuantityPDA = oItemLocationDetail.InitialQuantityPDA;
            objDTO.InsertedFrom = oItemLocationDetail.InsertedFrom;
            objDTO.ReceivedOnWeb = oItemLocationDetail.ReceivedOnWeb;
            objDTO.ReceivedOn = oItemLocationDetail.ReceivedOn;
            objDTO.AddedFrom = oItemLocationDetail.AddedFrom;
            objDTO.EditedFrom = oItemLocationDetail.EditedFrom;

            IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString());
            if (ObjCache != null)
            {
                objDTO.SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(objDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.BinID.GetValueOrDefault(0));

                List<ItemLocationDetailsDTO> objTemp = ObjCache.ToList();
                if (objTemp.Any(x => x.ID == objDTO.ID))
                {
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                }

                ObjCache = objTemp.AsEnumerable();

                List<ItemLocationDetailsDTO> tempC = new List<ItemLocationDetailsDTO>();
                tempC.Add(objDTO);
                IEnumerable<ItemLocationDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AppendToCacheItem("Cached_ItemLocationDetails_" + objDTO.CompanyID.ToString(), NewCache);
            }
        }

        /// <summary>
        /// Get Paged Records from the OrderDetails Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<OrderDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Int64 SupplierID = 0)
        {
            return DB_GetCachedData(RoomID, CompanyId, null, null, null, null, SupplierID, null);
        }


        public double GetOrderdQtyOfItem(Guid ItmGUID)
        {
            //double retValReq = 0;
            double retValApprv = 0;
            double retValRec = 0;
            double retVal = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {


                List<OrderDetail> obj = (from x in context.OrderDetails
                                         join m in context.OrderMasters on (x.OrderGUID ?? Guid.Empty) equals m.GUID
                                         where (x.ItemGUID ?? Guid.Empty) == ItmGUID && (m.StagingID ?? 0) <= 0
                                         && ((x.RequestedQuantity ?? 0) - (x.ReceivedQuantity ?? 0)) > 0
                                         && (m.StagingID ?? 0) <= 0
                                         && m.OrderStatus >= (int)OrderStatus.Approved
                                         && m.OrderStatus < (int)OrderStatus.Closed
                                         select x).ToList();

                List<OrderDetail> objList = new List<OrderDetail>();
                OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);

                retValApprv = obj.Where(t => (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && (t.ApprovedQuantity ?? 0) > 0).Sum(t => (t.ApprovedQuantity ?? 0));
                retValRec = obj.Where(t => (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && (t.ReceivedQuantity ?? 0) > 0).Sum(t => (t.ReceivedQuantity ?? 0));

                retVal = retValApprv - retValRec;
                if (retVal <= 0)
                    retVal = 0;
            }
            return retVal;

        }

        public bool InsertAuditTrialOnOrderChange(Guid OrderGUID, Guid OrderDetailGUID, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    string strCommand = "EXEC AT_InsertItemOrderHistory ";
                    strCommand += "" + OrderGUID.ToString() + "";
                    strCommand += ",'" + OrderDetailGUID.ToString() + "'";
                    strCommand += "," + ItemGUID.ToString() + " ";
                    context.ExecuteStoreCommand(strCommand);
                    return true;
                }
                catch (Exception Ex)
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public OrderDetailsDTO GetRecord(Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetCachedData(RoomID, CompanyID, null, null, ID, null, null, null).FirstOrDefault();
        }

        /// <summary>
        /// DeleteRecords
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <param name="RoomId"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public bool DB_DeleteRecords(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    string strCommand = "EXEC Orddtl_DeleteOrderDetail ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";

                    int intReturn = context.ExecuteStoreQuery<int>(strCommand).FirstOrDefault();

                    string[] arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIDs != null && arrIDs.Length > 0)
                    {
                        DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
                        ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                        foreach (var item in arrIDs)
                        {
                            OrderDetailsDTO DetailDTO = GetOrderDetailByIDPlain(Int64.Parse(item), RoomId, CompanyID);

                            try
                            {
                                DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, null, null);
                                DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, null, null);
                                ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                                objItemDTO.LastUpdatedBy = userid;
                                objItemDTO.IsOnlyFromItemUI = false;
                                itmDAL.Edit(objItemDTO);
                            }
                            catch (Exception)
                            {

                            }


                        }
                    }

                    if (intReturn > 0)
                        return true;

                }
            }
            return false;
        }
    }
}
