using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class OrderDetailsDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
        #region [Class Constructor]

        //public OrderDetailsDAL(base.DataBaseName)
        //{

        //}

        public OrderDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public OrderDetailsDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion


        #region [Class Methods]

        public OrderDetailsDTO GetOrderDetailByGuidPlain(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByGuidPlain] @GUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@GUID", OrderDetailGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }
        public List<PreReceivOrderDetailDTO> GetOrderDetailTrackingByGuidPlain(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailTrackingByGuidPlain] @OrderDetailId,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailId", OrderDetailGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<PreReceivOrderDetailDTO>(stryQry, params1).ToList();

            }
        }
        public List<PreReceivOrderDetailDTO> GetOrderDetailTrackingList(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailTrackingList] @RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<PreReceivOrderDetailDTO>(stryQry, params1).ToList();

            }
        }
        public OrderDetailsDTO GetOrderDetailByGuidNormal(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByGuidNormal] @GUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@GUID", OrderDetailGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public OrderDetailsDTO GetOrderDetailByGuidFull(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByGuidFull] @GUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@GUID", OrderDetailGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailByRoomPlain(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByRoomPlain] @RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailByItemGUID(long RoomID, long CompanyID, Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByItemGUID] @RoomID,@CompanyID,@ItemGUID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@ItemGUID", ItemGUID),
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public OrderDetailsDTO GetOrderDetailByIDFull(long ID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByIDFull] @ID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ID", ID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public OrderDetailsDTO GetOrderDetailByIDNormal(long ID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByIDNormal] @ID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ID", ID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public OrderDetailsDTO GetOrderDetailByIDPlain(long ID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByIDPlain] @ID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ID", ID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailByOrderGUIDPlain(Guid OrderGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByOrderGUIDPlain] @OrderGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(Guid OrderGUID, long RoomID, long CompanyID, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain] @IsDeleted,@OrderGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@IsDeleted", IsDeleted),
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailByOrderGUIDFull(Guid OrderGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByOrderGUIDFull] @OrderGUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailByOrderGUIDFullWithSupplierFilter(Guid OrderGUID, long RoomID, long CompanyID, bool IsDeleted, List<long> SupplierIds)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByOrderGUIDFullWithSupFltr] @OrderGUID,@RoomID,@CompanyID,@IsDeleted,@SupplierIds";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@SupplierIds",strSupplierIds ?? string.Empty),
                    new SqlParameter("@IsDeleted ",IsDeleted )

                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetArchivedOrderDetailByOrderGUIDFullWithSupplierFilter(Guid OrderGUID, long RoomID, long CompanyID, List<long> SupplierIds)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetArchivedOrderDetailByOrderGUIDFullWithSupFltr] @OrderGUID,@RoomID,@CompanyID,@SupplierIds";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderGUID", OrderGUID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@SupplierIds",strSupplierIds ?? string.Empty)//,
                    //new SqlParameter("@IsDeleted ",IsDeleted )

                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }

        public List<OrderDetailsDTO> GetOrderDetailHistoryByOrderGUIDNormal(Guid OrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailHistoryByOrderGUIDNormal] @OrderGUID";
                var params1 = new SqlParameter[] { new SqlParameter("@OrderGUID", OrderGUID) };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).ToList();

            }
        }


        public double GetOrderdQtyOfItemBinWise(Int64 RoomId, Int64 CompanyId, Guid ItmGUID, Int64 binId)
        {
            double onOrderQty = 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<OrderDetail> obj = (from x in context.OrderDetails
                                         join m in context.OrderMasters on (x.OrderGUID ?? Guid.Empty) equals m.GUID
                                         where (x.IsDeleted ?? false).Equals(false)
                                         && (x.IsArchived ?? false).Equals(false)
                                         && (x.IsCloseItem ?? false).Equals(false)
                                         && (m.IsDeleted.Equals(false))
                                         && (m.IsArchived ?? false).Equals(false)
                                         && m.OrderStatus < (int)OrderStatus.Closed
                                         && (m.OrderType ?? (int)OrderType.Order) == (int)OrderType.Order
                                         && x.Room == RoomId
                                         && x.CompanyID == CompanyId
                                         && (x.RequestedQuantity ?? 0) > 0
                                         && (m.StagingID ?? 0) <= 0
                                         && (x.ItemGUID ?? Guid.Empty) == ItmGUID
                                         && (x.Bin ?? 0) == binId
                                         select x).ToList();


                onOrderQty = obj.Sum(t => (t.ApprovedQuantity ?? 0) > 0 ? t.ApprovedQuantity.GetValueOrDefault(0) : t.RequestedQuantity.GetValueOrDefault(0) - t.ReceivedQuantity.GetValueOrDefault(0));

                if (onOrderQty <= 0)
                    onOrderQty = 0;
            }
            return onOrderQty;
        }


        public IEnumerable<OrderLineItemDetailDTO> GetOrderDetailExport(Int64 RoomID, Int64 CompanyID, int OrderType)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@OrderType", OrderType) };
                return context.Database.SqlQuery<OrderLineItemDetailDTO>("exec [GetOrderDetailExport] @RoomID,@CompanyID,@OrderType", params1).ToList();
            }
        }


        public IEnumerable<OrderDetailsDTO> GetOrderDetailsByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderGuid)
        {
            return DB_GetOrderDetailsByDateRange(StartRowIndex, MaxRows, out TotalCount, RoomID, CompanyID, IsArchived, IsDeleted, OrderGuid);
        }

        private IEnumerable<OrderDetailsDTO> DB_GetOrderDetailsByDateRange(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string OrderGuid)
        {
            if (MaxRows < 1)
            {
                MaxRows = 10;
            }
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());

            DataSet ds = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            ds = SqlHelper.ExecuteDataset(EturnsConnection, "Orddtl_GetOrderDetailPagedData", CompanyID, RoomID, IsDeleted, IsArchived, StartRowIndex, MaxRows, null, null, null, OrderGuid);

            IEnumerable<OrderDetailsDTO> obj = null;

            if (ds != null && ds.Tables.Count > 0)
            {
                obj = DataTableHelper.ToList<OrderDetailsDTO>(ds.Tables[0]);

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

        public IEnumerable<OrderDetailsDTO> GetChangeOrderDetailData(Int64? RoomID, Int64? CompanyID, List<long> SupplierIds, Guid ChangeOrderGUID)
        {
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@CompnayID", (CompanyID.GetValueOrDefault(0) > 0) ? CompanyID.GetValueOrDefault(0) :(object)DBNull.Value),
                        new SqlParameter("@RoomID", (RoomID.GetValueOrDefault(0) > 0) ? RoomID.GetValueOrDefault(0) :(object)DBNull.Value),
                        new SqlParameter("@SupplierIds", strSupplierIds),
                        new SqlParameter("@ChangeOrderGuid", (ChangeOrderGUID != Guid.Empty) ? ChangeOrderGUID : (object)DBNull.Value)

                    };


                string strCommand = " EXEC [GetChangeOrderDetailData] @CompnayID,@RoomID,@SupplierIds,@ChangeOrderGuid ";


                context.Database.CommandTimeout = 150;
                return context.Database.SqlQuery<OrderDetailsDTO>(strCommand, params1).ToList();

            }

        }



        public List<OrderDetailsDTO> GetOrderDetailDataWithItemBinByDetailGUID(Int64 RoomID, Int64 CompanyID, Guid OrderGUID, Guid ItemGuid, int BinID, Guid DetailGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@OrderGUID", OrderGUID),
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@BinID", BinID),
                                                new SqlParameter("@DetailGUID", DetailGUID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<OrderDetailsDTO> obj = context.Database.SqlQuery<OrderDetailsDTO>("exec [Orddtl_GetOrderDetailDataWithItemBinByDetailGUID] @RoomID,@CompanyID,@OrderGUID,@ItemGuid,@BinID,@DetailGUID", params1).ToList();
                if (obj != null && obj.Count > 0)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }

        }

        public int GetOrderDetailsCountWithItemBinByDetailGUID(long RoomID, long CompanyID, Guid OrderGUID, Guid ItemGuid, long BinID, Guid DetailGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@OrderGUID", OrderGUID),
                                                new SqlParameter("@ItemGuid", ItemGuid),
                                                new SqlParameter("@BinID", BinID),
                                                new SqlParameter("@DetailGUID", DetailGUID)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [GetOrderDetailsCountWithItemBinByDetailGUID] @RoomID,@CompanyID,@OrderGUID,@ItemGuid,@BinID,@DetailGUID", params1).FirstOrDefault();
            }
        }


        public ItemToReturnDTO ReturntemQuantity(ItemToReturnDTO objItemReturnInfo, long SessionUserId, long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objItemReturnInfo == null || objItemReturnInfo.lstItemPullDetails == null)
                {
                    return objItemReturnInfo;
                }
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemReturnInfo.ItemGUID);
                objItemReturnInfo.lstItemPullDetails.ForEach(t =>
                {
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objItemReturnInfo.RoomId);
                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    //ItemLocationDetail objItemLocationDetail = null;
                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                           && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                    }


                    if (lstItemLocations != null)
                    {
                        foreach (var objItemLocationDetail in lstItemLocations)
                        {
                            if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) >= (t.ConsignedTobePulled + t.CustomerOwnedTobePulled))
                            {
                                ReceivedOrderTransferDetail objROTDDetail = new ReceivedOrderTransferDetail();
                                objROTDDetail.BinID = t.BinID;
                                objROTDDetail.CompanyID = objItemReturnInfo.CompanyId;
                                objROTDDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                objROTDDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                objROTDDetail.GUID = Guid.NewGuid();
                                objROTDDetail.IsArchived = false;
                                objROTDDetail.IsDeleted = false;
                                objROTDDetail.Cost = t.Cost;
                                objROTDDetail.ItemGUID = objItemReturnInfo.ItemGUID;
                                objROTDDetail.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                                objROTDDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objROTDDetail.OrderDetailGUID = objItemReturnInfo.OrderDetailGUID;
                                objROTDDetail.ItemLocationDetailGUID = t.GUID;
                                objROTDDetail.ReceivedDate = t.ReceivedDate;
                                objROTDDetail.ExpirationDate = t.ExpirationDate;

                                if (!string.IsNullOrWhiteSpace(objItemLocationDetail.Received))
                                    objROTDDetail.Received = objItemLocationDetail.Received;
                                else if (objItemLocationDetail.ReceivedDate.HasValue)
                                    objROTDDetail.Received = objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy");

                                objROTDDetail.Room = objItemReturnInfo.RoomId;
                                objROTDDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                if (objItem.DateCodeTracking && !string.IsNullOrWhiteSpace(objItemLocationDetail.Expiration))
                                    objROTDDetail.Expiration = objItemLocationDetail.Expiration;
                                else if (objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue)
                                    objROTDDetail.Expiration = objItemLocationDetail.ExpirationDate.Value.ToString("MM/dd/yyyy");

                                objROTDDetail.Updated = DateTimeUtility.DateTimeNow;
                                objROTDDetail.Created = DateTimeUtility.DateTimeNow;
                                objROTDDetail.CreatedBy = objItemReturnInfo.CreatedBy;
                                objROTDDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objROTDDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objROTDDetail.AddedFrom = "Web";
                                objROTDDetail.EditedFrom = "Web";
                                objROTDDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;


                                context.ReceivedOrderTransferDetails.Add(objROTDDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;


                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {

                                ReceivedOrderTransferDetail objROTDDetail = new ReceivedOrderTransferDetail();
                                objROTDDetail.BinID = t.BinID;
                                objROTDDetail.CompanyID = objItemReturnInfo.CompanyId;
                                objROTDDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                objROTDDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objItemLocationDetail.CustomerOwnedQuantity ?? 0);
                                t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objItemLocationDetail.ConsignedQuantity ?? 0);
                                objROTDDetail.GUID = Guid.NewGuid();
                                objROTDDetail.IsArchived = false;
                                objROTDDetail.IsDeleted = false;
                                objROTDDetail.Cost = t.Cost;
                                objROTDDetail.ItemGUID = objItemReturnInfo.ItemGUID;
                                objROTDDetail.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                                objROTDDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objROTDDetail.OrderDetailGUID = objItemReturnInfo.OrderDetailGUID;
                                objROTDDetail.ItemLocationDetailGUID = t.GUID;
                                objROTDDetail.ReceivedDate = t.ReceivedDate;
                                objROTDDetail.ExpirationDate = t.ExpirationDate;



                                if (!string.IsNullOrWhiteSpace(objItemLocationDetail.Received))
                                    objROTDDetail.Received = objItemLocationDetail.Received;
                                else if (objItemLocationDetail.ReceivedDate.HasValue)
                                    objROTDDetail.Received = objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy");

                                objROTDDetail.Room = objItemReturnInfo.RoomId;
                                objROTDDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                if (objItem.DateCodeTracking && !string.IsNullOrWhiteSpace(objItemLocationDetail.Expiration))
                                    objROTDDetail.Expiration = objItemLocationDetail.Expiration;
                                else if (objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue)
                                    objROTDDetail.Expiration = objItemLocationDetail.ExpirationDate.Value.ToString("MM/dd/yyyy");

                                objROTDDetail.Updated = DateTimeUtility.DateTimeNow;
                                objROTDDetail.Created = DateTimeUtility.DateTimeNow;
                                objROTDDetail.CreatedBy = objItemReturnInfo.CreatedBy;
                                objROTDDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objROTDDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objROTDDetail.AddedFrom = "Web";
                                objROTDDetail.EditedFrom = "Web";
                                objROTDDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;
                                context.ReceivedOrderTransferDetails.Add(objROTDDetail);
                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;


                            }
                        }
                    }

                });

                context.SaveChanges();

                ItemLocationQTY objItemLocationQTY = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID);
                if (objItemLocationQTY == null)
                {
                    objItemLocationQTY = new ItemLocationQTY();
                    objItemLocationQTY.BinID = objItemReturnInfo.BinID;
                    objItemLocationQTY.CompanyID = objItemReturnInfo.CompanyId;
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Created = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.CreatedBy = objItemReturnInfo.CreatedBy;
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.GUID = Guid.NewGuid();
                    objItemLocationQTY.ItemGUID = objItemReturnInfo.ItemGUID;
                    objItemLocationQTY.LastUpdated = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                    objItemLocationQTY.Room = objItemReturnInfo.RoomId;

                    objItemLocationQTY.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.AddedFrom = "Web";
                    objItemLocationQTY.EditedFrom = "Web";

                    context.ItemLocationQTies.Add(objItemLocationQTY);
                }
                else
                {
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && t.BinID == objItemReturnInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                }
                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemReturnInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();

                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                CostDTO objCostDTO = objDashboardDAL.GetAvgExtendedCost(objItem.GUID);
                objItem.ExtendedCost = objCostDTO.ExtCost;
                objItem.AverageCost = objCostDTO.AvgCost;

                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);

                //objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "PullTransationDAL >> PullitemQuantity_OLD");
                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemReturnInfo.LastUpdatedBy, "web", "Consume >> Return Quantity to Item", SessionUserId);
                objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, objItemReturnInfo.ItemGUID, objItemReturnInfo.CreatedBy, null, null);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId, objItemReturnInfo.ItemGUID, objItemReturnInfo.CreatedBy, SessionUserId, null, null);

                context.SaveChanges();

                OrderDetailsDTO ordDetailDTO = GetOrderDetailByGuidPlain(objItemReturnInfo.OrderDetailGUID, objItemReturnInfo.RoomId, objItemReturnInfo.CompanyId);
                ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objItemReturnInfo.QtyToReturn;
                ordDetailDTO.LastUpdatedBy = objItemReturnInfo.LastUpdatedBy;
                ordDetailDTO.EditedFrom = "Web-ReturnOrder";
                ordDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                Edit(ordDetailDTO, SessionUserId, EnterpriseId);
                UpdateOrderStatusByReceive(ordDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), ordDetailDTO.Room.GetValueOrDefault(0), ordDetailDTO.CompanyID.GetValueOrDefault(0), ordDetailDTO.LastUpdatedBy.GetValueOrDefault(0), true);

                return objItemReturnInfo;
            }
        }


        public ResponseMessage StagingOrderReturnQuantity(OrderReturnQuantityDetail objReturnQty, long RoomID, long CompanyID, long UserID, long MaterialStagingID, long SessionUserId,
                                                          long EnterpriseID, string CultureCode)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ResponseMessage response = new ResponseMessage();
            MaterialStagingDAL objMSDAL = new MaterialStagingDAL(base.DataBaseName);
            MaterialStagingDTO MSDTO = objMSDAL.GetRecord(MaterialStagingID, RoomID, CompanyID);
            Guid MaterialStagingGUID = MSDTO.GUID;
            var orderMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", CultureCode, EnterpriseID, CompanyID);
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objReturnQty.ItemGUID);

            if (ItemDTO.StagedQuantity < objReturnQty.ReturnQuantity)
            {
                string msgNotEnoughQtyOnHandQtyIsLessThanReturnQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQtyOnHandQtyIsLessThanReturnQty", orderMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResOrder", CultureCode);
                response.IsSuccess = false;
                response.Message = msgNotEnoughQtyOnHandQtyIsLessThanReturnQty;
                return response;
            }
            else
            {
                string msg = "";
                ResponseMessage ResponseMsg = new CommonDAL(base.DataBaseName).CheckQuantityByStagingLocation(MaterialStagingGUID, objReturnQty.LocationID, objReturnQty.ItemGUID, objReturnQty.ReturnQuantity, RoomID, CompanyID, EnterpriseID, UserID);
                if (!ResponseMsg.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = msg;
                    return response;
                }
            }

            BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objReturnQty.LocationID, RoomID, CompanyID);
            //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objReturnQty.LocationID,null,null).FirstOrDefault();

            MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(base.DataBaseName);
            MaterialStagingPullDetailDAL objLocationDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
            ReceivedOrderTransferDetailDAL objRecvOrdDetailDAL = new ReceivedOrderTransferDetailDAL(base.DataBaseName);
            //MaterialStagingDetailDTO objMSDetailDTO = objMSDetailDAL.GetAllRecords(RoomID, CompanyID).Where(x => x.MaterialStagingGUID == MaterialStagingGUID && x.ItemGUID == objReturnQty.ItemGUID && x.StagingBinID == objReturnQty.LocationID && x.IsDeleted == false && x.IsArchived == false).FirstOrDefault();
            MaterialStagingDetailDTO objMSDetailDTO = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MaterialStagingGUID), Convert.ToString(objReturnQty.ItemGUID), objReturnQty.LocationID, RoomID, CompanyID, false, false).FirstOrDefault();

            #region "ItemLocation Deduction"
            if (ItemDTO.SerialNumberTracking)
            {
                #region "Serial logic"
                //List<ItemLocationDetailsDTO> ObjItemLocation = null;
                List<MaterialStagingPullDetailDTO> ObjItemLocation = null;
                //ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MaterialStagingGUID && x.StagingBinId == objReturnQty.LocationID && x.ItemGUID == objReturnQty.ItemGUID && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false).Take((int)objReturnQty.ReturnQuantity).ToList();
                ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Convert.ToString(MaterialStagingGUID), objReturnQty.LocationID, Convert.ToString(objReturnQty.ItemGUID), false, false, string.Empty).Take((int)objReturnQty.ReturnQuantity).ToList();

                foreach (var itemoil in ObjItemLocation)
                {
                    double loopCurrentTakenCustomer = 0;
                    double loopCurrentTakenConsignment = 0;

                    if (ItemDTO.Consignment)
                    {
                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                        {
                            loopCurrentTakenConsignment = 1;
                            itemoil.ConsignedQuantity = 0;
                        }
                        else
                        {
                            loopCurrentTakenCustomer = 1;
                            itemoil.CustomerOwnedQuantity = 0;
                        }
                    }
                    else
                    {
                        loopCurrentTakenCustomer = 1;
                        itemoil.CustomerOwnedQuantity = 0;
                    }
                    itemoil.OrderDetailGUID = objReturnQty.OrderDetailGUID;
                    objMSDetailDTO.Quantity = objMSDetailDTO.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                    objLocationDAL.Edit(itemoil);

                    ReceivedOrderTransferDetailDTO objReceiveOrdTrnfDetailDTO = new ReceivedOrderTransferDetailDTO()
                    {
                        Action = string.Empty,
                        BinNumber = itemoil.BinNumber,
                        Cost = itemoil.ItemCost,
                        CreatedByName = string.Empty,
                        DateCodeTracking = itemoil.DateCodeTracking,
                        Expiration = itemoil.Expiration,
                        GUID = Guid.NewGuid(),
                        HistoryID = 0,
                        ID = 0,
                        IsCreditPull = true,
                        ItemLocationDetailGUID = itemoil.GUID,
                        ItemNumber = itemoil.ItemNumber,
                        LotNumber = (!string.IsNullOrWhiteSpace(itemoil.LotNumber)) ? itemoil.LotNumber.Trim() : string.Empty,
                        LotNumberTracking = itemoil.LotNumberTracking,
                        mode = "ReturnOrder",
                        Received = itemoil.Received,
                        RoomName = itemoil.RoomName,
                        BinID = objReturnQty.LocationID,
                        ItemGUID = objReturnQty.ItemGUID,
                        OrderDetailGUID = objReturnQty.OrderDetailGUID,
                        UpdatedByName = itemoil.UpdatedByName,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CreatedBy = UserID,
                        LastUpdatedBy = UserID,
                        CompanyID = CompanyID,
                        Room = RoomID,
                        IsArchived = false,
                        IsDeleted = false,
                        SerialNumberTracking = itemoil.SerialNumberTracking,
                        SerialNumber = (!string.IsNullOrWhiteSpace(itemoil.SerialNumber)) ? itemoil.SerialNumber.Trim() : string.Empty,
                        CustomerOwnedQuantity = loopCurrentTakenCustomer,
                        ConsignedQuantity = loopCurrentTakenConsignment,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };

                    objRecvOrdDetailDAL.Insert(objReceiveOrdTrnfDetailDTO);
                }
                #endregion
            }
            else
            {
                #region "Lot and other type logic"
                //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objReturnQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == objReturnQty.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(objReturnQty.LocationID, RoomID, CompanyID, objReturnQty.ItemGUID, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                //List<MaterialStagingPullDetailDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MaterialStagingGUID && x.StagingBinId == objReturnQty.LocationID && x.ItemGUID == objReturnQty.ItemGUID && x.IsDeleted.GetValueOrDefault(false) == false && x.IsArchived.GetValueOrDefault(false) == false).Take((int)objReturnQty.ReturnQuantity).ToList();
                List<MaterialStagingPullDetailDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Convert.ToString(MaterialStagingGUID), objReturnQty.LocationID, Convert.ToString(objReturnQty.ItemGUID), false, false, string.Empty).Take((int)objReturnQty.ReturnQuantity).ToList();

                Double takenQunatity = 0;
                foreach (var itemoil in ObjItemLocation)
                {
                    Double loopCurrentTakenCustomer = 0;
                    Double loopCurrentTakenConsignment = 0;
                    if (takenQunatity == objReturnQty.ReturnQuantity)
                    {
                        break;
                    }
                    itemoil.OrderDetailGUID = objReturnQty.OrderDetailGUID;
                    if (ItemDTO.Consignment)
                    {
                        #region "Consignment Pull"
                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenConsignment = objReturnQty.ReturnQuantity - takenQunatity;
                            itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity = (objReturnQty.ReturnQuantity - takenQunatity);
                        }
                        else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenCustomer = (objReturnQty.ReturnQuantity - takenQunatity);
                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity = objReturnQty.ReturnQuantity - takenQunatity;
                        }
                        else
                        {
                            takenQunatity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity - takenQunatity;
                            if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                            {
                                loopCurrentTakenConsignment = (objReturnQty.ReturnQuantity - takenQunatity);
                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                                takenQunatity += objReturnQty.ReturnQuantity - takenQunatity;
                            }
                            else
                            {
                                loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                itemoil.ConsignedQuantity = 0;
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region "Customer own Pull"
                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenCustomer = (objReturnQty.ReturnQuantity - takenQunatity);
                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity += (objReturnQty.ReturnQuantity - takenQunatity);
                        }
                        else
                        {
                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;

                        }
                        #endregion
                    }
                    objLocationDAL.Edit(itemoil);
                    objMSDetailDTO.Quantity = objMSDetailDTO.Quantity - (loopCurrentTakenCustomer + loopCurrentTakenConsignment);
                    ReceivedOrderTransferDetailDTO objReceiveOrdTrnfDetailDTO = new ReceivedOrderTransferDetailDTO()
                    {
                        Action = string.Empty,
                        BinNumber = itemoil.BinNumber,
                        Cost = itemoil.ItemCost,
                        CreatedByName = string.Empty,

                        DateCodeTracking = itemoil.DateCodeTracking,

                        Expiration = itemoil.Expiration,

                        GUID = Guid.NewGuid(),
                        HistoryID = 0,
                        ID = 0,
                        IsCreditPull = true,
                        ItemLocationDetailGUID = itemoil.GUID,
                        ItemNumber = itemoil.ItemNumber,

                        LotNumber = (!string.IsNullOrWhiteSpace(itemoil.LotNumber)) ? itemoil.LotNumber.Trim() : string.Empty,
                        LotNumberTracking = itemoil.LotNumberTracking,

                        mode = "ReturnOrder",
                        Received = itemoil.Received,
                        RoomName = itemoil.RoomName,
                        BinID = objReturnQty.LocationID,
                        ItemGUID = objReturnQty.ItemGUID,
                        OrderDetailGUID = objReturnQty.OrderDetailGUID,
                        UpdatedByName = itemoil.UpdatedByName,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CreatedBy = UserID,
                        LastUpdatedBy = UserID,
                        CompanyID = CompanyID,
                        Room = RoomID,
                        IsArchived = false,
                        IsDeleted = false,
                        SerialNumberTracking = itemoil.SerialNumberTracking,
                        SerialNumber = (!string.IsNullOrWhiteSpace(itemoil.SerialNumber)) ? itemoil.SerialNumber.Trim() : string.Empty,
                        CustomerOwnedQuantity = loopCurrentTakenCustomer,
                        ConsignedQuantity = loopCurrentTakenConsignment,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };

                    objRecvOrdDetailDAL.Insert(objReceiveOrdTrnfDetailDTO);
                }
                #endregion
            }
            #endregion

            #region "Saving Location QTY data"

            ItemDTO.StagedQuantity = ItemDTO.StagedQuantity.GetValueOrDefault(0) - objReturnQty.ReturnQuantity;
            ItemDTO.LastUpdatedBy = UserID;
            ItemDTO.WhatWhereAction = "Return Order";
            objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);
            objMSDetailDAL.Edit(objMSDetailDTO);

            #endregion

            #region "Order Detail table"

            OrderDetailsDTO ordDetailDTO = GetOrderDetailByGuidPlain(objReturnQty.OrderDetailGUID, RoomID, CompanyID);
            ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objReturnQty.ReturnQuantity;
            ordDetailDTO.LastUpdatedBy = UserID;
            Edit(ordDetailDTO, SessionUserId, EnterpriseID);
            UpdateOrderStatusByReceive(ordDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), ordDetailDTO.Room.GetValueOrDefault(0), ordDetailDTO.CompanyID.GetValueOrDefault(0), ordDetailDTO.LastUpdatedBy.GetValueOrDefault(0));

            response.IsSuccess = true;
            string msgQuantityReturnedSuccessfully = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityReturnedSuccessfully", orderMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResOrder", CultureCode);
            response.Message = msgQuantityReturnedSuccessfully;

            #endregion

            return response;
        }


        public bool UpdateOrderStatusByReceive(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, bool IsOnlyFromUI = false, bool IsRestrictOrderStatusUpdate = false)
        {
            OrderMasterDAL ordMasterDAL = new OrderMasterDAL(base.DataBaseName);
            OrderMasterDTO ordMasterDTO = ordMasterDAL.GetOrderByGuidPlain(OrderGuid);
            if (ordMasterDTO.OrderStatus > (int)OrderStatus.Approved && ordMasterDTO.OrderStatus <= (int)OrderStatus.Closed)
            {
                List<OrderDetailsDTO> lstOrdDetailRecords = GetOrderDetailByOrderGUIDFull(OrderGuid, RoomID, CompanyID);
                if (lstOrdDetailRecords != null && lstOrdDetailRecords.Count > 0)
                {
                    bool IsAllItemNotReceived = false;

                    foreach (var item in lstOrdDetailRecords)
                    {
                        if ((item.ApprovedQuantity.GetValueOrDefault(0) > item.ReceivedQuantity.GetValueOrDefault(0) 
                            && !item.IsCloseItem.GetValueOrDefault(false)) || IsRestrictOrderStatusUpdate)
                        {
                            IsAllItemNotReceived = true;
                            break;
                        }
                    }

                    if (IsAllItemNotReceived)
                    {
                        List<ReceivedOrderTransferDetailDTO> lstAllROTD = new List<ReceivedOrderTransferDetailDTO>();
                        foreach (var item in lstOrdDetailRecords)
                        {
                            IEnumerable<ReceivedOrderTransferDetailDTO> lstROTD = new ReceivedOrderTransferDetailDAL(base.DataBaseName).GetROTDByOrderDetailGUIDFull(item.GUID, RoomID, CompanyID).OrderByDescending(x => x.ID).ToList();
                            lstAllROTD.AddRange(lstROTD);
                        }
                        List<DateTime> lstReceivedDates = new List<DateTime>();
                        if (lstAllROTD != null && lstAllROTD.Count > 0)
                        {
                            for (int i = 0; i < lstAllROTD.Count; i++)
                            {
                                if (lstAllROTD[i].ReceivedDate.HasValue)
                                {
                                    lstReceivedDates.Add(lstAllROTD[i].ReceivedDate.Value);
                                }
                            }
                        }
                        DateTime MaxReceiveDate = DateTime.Now;

                        if (lstReceivedDates != null && lstReceivedDates.Count > 0)
                            MaxReceiveDate = lstReceivedDates.Max();

                        bool IsRequeredDateIsPast = false;

                        foreach (var item in lstOrdDetailRecords)
                        {
                            if (item.RequiredDate.GetValueOrDefault(DateTime.MinValue) < MaxReceiveDate && !item.IsCloseItem.GetValueOrDefault(false))
                            {
                                IsRequeredDateIsPast = true;
                                break;
                            }
                        }

                        if (IsRequeredDateIsPast)
                        {
                            ordMasterDTO.OrderStatus = (int)OrderStatus.TransmittedInCompletePastDue;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);
                        }
                        else
                        {
                            ordMasterDTO.OrderStatus = (int)OrderStatus.TransmittedIncomplete;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);

                        }

                    }
                    else
                    {
                        if (ordMasterDTO.OrderStatus != (int)OrderStatus.Closed)
                        {
                            ordMasterDTO.OrderStatus = (int)OrderStatus.Closed;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            if (IsOnlyFromUI)
                            {
                                ordMasterDTO.EditedFrom = "Web";
                                ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            }
                            ordMasterDAL.Edit(ordMasterDTO);
                        }

                    }
                }
            }
            return true;
        }

        public OrderStatus UpdateOrderStatusByReceiveNew(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, bool IsOnlyFromUI = false, string EditedFrom = "", bool IsFromService = false, string ShippingTrackNumber = "")
        {
            OrderMasterDAL ordMasterDAL = new OrderMasterDAL(base.DataBaseName);
            OrderMasterDTO ordMasterDTO = ordMasterDAL.GetOrderByGuidPlain(OrderGuid);
            List<ReceivedOrderTransferDetailDTO> lstAllReceived = new List<ReceivedOrderTransferDetailDTO>();
            if (ordMasterDTO.OrderStatus > (int)OrderStatus.Approved && ordMasterDTO.OrderStatus <= (int)OrderStatus.Closed)
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<OrderDetail> lstOrdDetails = context.OrderDetails.Where(x => x.OrderGUID == OrderGuid && !(x.IsCloseItem ?? false));
                    if (lstOrdDetails != null && lstOrdDetails.Count() > 0)
                    {
                        IEnumerable<OrderDetail> lstIncompleteOrdDetails = lstOrdDetails.Where(x => (x.ApprovedQuantity ?? 0) > (x.ReceivedQuantity ?? 0));

                        if (lstIncompleteOrdDetails.Count() > 0)
                        {
                            DateTime MaxReceiveDate = DateTime.Now;
                            DateTime MaxRequiredDate = DateTime.Now;

                            IEnumerable<Guid> lstOrderDetailsGuid = lstOrdDetails.Select(x => x.GUID);
                            MaxRequiredDate = lstIncompleteOrdDetails.Select(x => (x.RequiredDate ?? DateTime.MinValue)).Max();
                            string strOrderDetailGuids = string.Join(",", lstOrderDetailsGuid.ToArray());

                            var params1 = new SqlParameter[] { new SqlParameter("@OrderDetailGUIDs", strOrderDetailGuids) };

                            /*
                            DateTime? rcvdate = context.Database.SqlQuery<DateTime?>(@"SELECT Max(ReceivedDate) FROM ReceivedOrderTransferDetail 
                                                                                       WHERE ReceivedDate IS NOT NULL AND ISNULL(IsDeleted,0)=0 AND ISNULL(IsArchived,0)=0 
                                                                                       AND OrderDetailGUID IN (SELECT SplitValue FROM dbo.split('" + strOrderDetailGuids + @"',','))").FirstOrDefault();
                            */

                            string strQRY = "EXEC GetMaxReceiveDateFromOrderDetail @OrderDetailGUIDs";
                            DateTime? rcvdate = context.Database.SqlQuery<DateTime?>(strQRY, params1).FirstOrDefault();

                            if (rcvdate.HasValue)
                                MaxReceiveDate = rcvdate.GetValueOrDefault(DateTime.MinValue);

                            if (!string.IsNullOrWhiteSpace(ShippingTrackNumber))
                            {
                                ordMasterDTO.ShippingTrackNumber = ShippingTrackNumber;
                            }

                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";

                            if (MaxRequiredDate < MaxReceiveDate && ordMasterDTO.OrderStatus != (int)OrderStatus.TransmittedInCompletePastDue)
                            {
                                ordMasterDTO.OrderStatus = (int)OrderStatus.TransmittedInCompletePastDue;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                            else if (MaxRequiredDate > MaxReceiveDate && ordMasterDTO.OrderStatus != (int)OrderStatus.TransmittedIncomplete)
                            {
                                ordMasterDTO.OrderStatus = (int)OrderStatus.TransmittedIncomplete;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                        }
                        else if (ordMasterDTO.OrderStatus != (int)OrderStatus.Closed)
                        {

                            if (!string.IsNullOrWhiteSpace(ShippingTrackNumber))
                            {
                                ordMasterDTO.ShippingTrackNumber = ShippingTrackNumber;
                            }

                            ordMasterDTO.OrderStatus = (int)OrderStatus.Closed;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";
                            if (IsFromService)
                                ordMasterDTO.EditedFrom = EditedFrom;
                            ordMasterDAL.Edit(ordMasterDTO);
                        }
                    }
                }
            }
            return (OrderStatus)ordMasterDTO.OrderStatus;
        }


        public ResponseMessage OrderReturnQuantity(OrderReturnQuantityDetail objReturnQty, long RoomID, long CompanyID, long UserID, long SessionUserId, long EnterpriseID,
                                                   string CultureCode, bool isFromUI = false)
        {
            ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
            ResponseMessage response = new ResponseMessage();
            ItemMasterDTO ItemDTO = objItemDAL.GetItemWithoutJoins(null, objReturnQty.ItemGUID);
            var orderMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", CultureCode, EnterpriseID, CompanyID);

            if (ItemDTO.OnHandQuantity < objReturnQty.ReturnQuantity)
            {
                string msgNotEnoughQtyOnHandQtyIsLessThanReturnQty = ResourceRead.GetResourceValueByKeyAndFullFilePath("NotEnoughQtyOnHandQtyIsLessThanReturnQty", orderMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResOrder", CultureCode);
                response.IsSuccess = false;
                response.Message = msgNotEnoughQtyOnHandQtyIsLessThanReturnQty;
                return response;
            }
            else
            {
                //bool IsQtyAvailable = true;
                string msg = "";
                ResponseMessage ResponseMsg = new CommonDAL(base.DataBaseName).CheckQuantityByLocation(objReturnQty.LocationID, objReturnQty.ItemGUID, objReturnQty.ReturnQuantity, RoomID, CompanyID, EnterpriseID, UserID);
                if (!ResponseMsg.IsSuccess)
                {
                    response.IsSuccess = false;
                    response.Message = msg;
                    return response;
                }
            }


            //ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetAllRecords(RoomID, CompanyID).Where(x => x.BinID == objReturnQty.LocationID && x.ItemGUID == objReturnQty.ItemGUID).FirstOrDefault();
            ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDAL(base.DataBaseName).GetItemLocationQTY(RoomID, CompanyID, objReturnQty.LocationID, Convert.ToString(objReturnQty.ItemGUID)).FirstOrDefault();
            BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objReturnQty.LocationID, RoomID, CompanyID);
            //BinMasterDTO objBINDTO = new BinMasterDAL(base.DataBaseName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objReturnQty.LocationID,null,null).FirstOrDefault();
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            double CosigneQty = 0;
            double CustomerQty = 0;
            ReceivedOrderTransferDetailDAL objRecvOrdDetailDAL = new ReceivedOrderTransferDetailDAL(base.DataBaseName);

            #region "ItemLocation Deduction"
            if (ItemDTO.SerialNumberTracking)
            {
                #region "Serial logic"
                List<ItemLocationDetailsDTO> ObjItemLocation = null;
                ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(objReturnQty.LocationID, RoomID, CompanyID, objReturnQty.ItemGUID, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").Take((int)objReturnQty.ReturnQuantity).ToList();

                foreach (var itemoil in ObjItemLocation)
                {
                    double loopCurrentTakenCustomer = 0;
                    double loopCurrentTakenConsignment = 0;

                    if (ItemDTO.Consignment)
                    {
                        //loopCurrentTaken = 1;

                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                        {
                            loopCurrentTakenConsignment = 1;
                            itemoil.ConsignedQuantity = 0;
                        }
                        else
                        {
                            loopCurrentTakenCustomer = 1;
                            itemoil.CustomerOwnedQuantity = 0;
                        }
                    }
                    else
                    {
                        loopCurrentTakenCustomer = 1;
                        itemoil.CustomerOwnedQuantity = 0;
                    }
                    itemoil.OrderDetailGUID = objReturnQty.OrderDetailGUID;
                    if (isFromUI)
                    {
                        itemoil.EditedFrom = "Web";
                        itemoil.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    objLocationDAL.Edit(itemoil);

                    ReceivedOrderTransferDetailDTO objReceiveOrdTrnfDetailDTO = new ReceivedOrderTransferDetailDTO()
                    {
                        Action = string.Empty,
                        BinNumber = itemoil.BinNumber,
                        Cost = itemoil.Cost,
                        CreatedByName = string.Empty,
                        CriticalQuantity = itemoil.CriticalQuantity,
                        DateCodeTracking = itemoil.DateCodeTracking,
                        eVMISensorID = itemoil.eVMISensorID,
                        eVMISensorPort = itemoil.eVMISensorPort,
                        Expiration = itemoil.Expiration,
                        ExpirationDate = itemoil.ExpirationDate,
                        GUID = Guid.NewGuid(),
                        HistoryID = 0,
                        ID = 0,
                        IsCreditPull = true,
                        ItemLocationDetailGUID = itemoil.GUID,
                        ItemNumber = itemoil.ItemNumber,
                        ItemType = itemoil.ItemType,
                        KitDetailGUID = itemoil.KitDetailGUID,
                        LotNumber = (!string.IsNullOrWhiteSpace(itemoil.LotNumber)) ? itemoil.LotNumber.Trim() : string.Empty,
                        LotNumberTracking = itemoil.LotNumberTracking,
                        MaximumQuantity = itemoil.MaximumQuantity,
                        MeasurementID = itemoil.MeasurementID,
                        MinimumQuantity = itemoil.MinimumQuantity,
                        mode = "ReturnOrder",
                        Received = itemoil.Received,
                        ReceivedDate = itemoil.ReceivedDate,
                        RoomName = itemoil.RoomName,
                        TransferDetailGUID = itemoil.TransferDetailGUID,
                        UDF1 = itemoil.UDF1,
                        UDF2 = itemoil.UDF2,
                        UDF3 = itemoil.UDF3,
                        UDF4 = itemoil.UDF4,
                        UDF5 = itemoil.UDF5,
                        UDF6 = itemoil.UDF6,
                        UDF7 = itemoil.UDF7,
                        UDF8 = itemoil.UDF8,
                        UDF9 = itemoil.UDF9,
                        UDF10 = itemoil.UDF10,
                        BinID = objReturnQty.LocationID,
                        ItemGUID = objReturnQty.ItemGUID,
                        OrderDetailGUID = objReturnQty.OrderDetailGUID,
                        UpdatedByName = itemoil.UpdatedByName,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CreatedBy = UserID,
                        LastUpdatedBy = UserID,
                        CompanyID = CompanyID,
                        Room = RoomID,
                        IsArchived = false,
                        IsDeleted = false,
                        SerialNumberTracking = itemoil.SerialNumberTracking,
                        SerialNumber = (!string.IsNullOrWhiteSpace(itemoil.SerialNumber)) ? itemoil.SerialNumber.Trim() : string.Empty,
                        CustomerOwnedQuantity = loopCurrentTakenCustomer,
                        ConsignedQuantity = loopCurrentTakenConsignment,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow
                    };

                    objRecvOrdDetailDAL.Insert(objReceiveOrdTrnfDetailDTO);
                }
                #endregion
            }
            else
            {
                #region "Lot and other type logic"
                //List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecords(RoomID, CompanyID, Guid.Parse(objReturnQty.ItemGUID), null, "ID ASC").Where(x => x.BinID == objReturnQty.LocationID).OrderBy("CustomerOwnedQuantity DESC").ToList();
                List<ItemLocationDetailsDTO> ObjItemLocation = objLocationDAL.GetAllRecordsAvailableAt(objReturnQty.LocationID, RoomID, CompanyID, objReturnQty.ItemGUID, null, "CustomerOwnedQuantity DESC, ConsignedQuantity DESC, ID ASC").ToList();
                Double takenQunatity = 0;
                foreach (var itemoil in ObjItemLocation)
                {
                    Double loopCurrentTakenCustomer = 0;
                    Double loopCurrentTakenConsignment = 0;
                    if (takenQunatity == objReturnQty.ReturnQuantity)
                    {
                        break;
                    }
                    itemoil.OrderDetailGUID = objReturnQty.OrderDetailGUID;
                    if (ItemDTO.Consignment)
                    {
                        #region "Consignment Pull"
                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) == 0 && itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenConsignment = objReturnQty.ReturnQuantity - takenQunatity;
                            itemoil.ConsignedQuantity = (Double)itemoil.ConsignedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity += (objReturnQty.ReturnQuantity - takenQunatity);
                        }
                        else if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenCustomer = (objReturnQty.ReturnQuantity - takenQunatity);
                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity += objReturnQty.ReturnQuantity - takenQunatity;
                        }
                        else
                        {

                            if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                            {
                                takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                                itemoil.CustomerOwnedQuantity = 0;
                            }

                            if (itemoil.ConsignedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                            {
                                loopCurrentTakenConsignment = (objReturnQty.ReturnQuantity - takenQunatity);
                                itemoil.ConsignedQuantity = itemoil.ConsignedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                                takenQunatity += objReturnQty.ReturnQuantity - takenQunatity;
                            }
                            else
                            {
                                loopCurrentTakenConsignment = itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                takenQunatity += itemoil.ConsignedQuantity.GetValueOrDefault(0);
                                itemoil.ConsignedQuantity = 0;
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region "Customer own Pull"
                        if (itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) >= (objReturnQty.ReturnQuantity - takenQunatity))
                        {
                            loopCurrentTakenCustomer = (objReturnQty.ReturnQuantity - takenQunatity);
                            itemoil.CustomerOwnedQuantity = (Double)itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - (objReturnQty.ReturnQuantity - takenQunatity);
                            takenQunatity += (objReturnQty.ReturnQuantity - takenQunatity);
                        }
                        else
                        {
                            loopCurrentTakenCustomer = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            takenQunatity += itemoil.CustomerOwnedQuantity.GetValueOrDefault(0);
                            itemoil.CustomerOwnedQuantity = itemoil.CustomerOwnedQuantity.GetValueOrDefault(0) - takenQunatity;

                        }
                        #endregion
                    }
                    if (isFromUI)
                    {
                        itemoil.EditedFrom = "Web";
                        itemoil.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    objLocationDAL.Edit(itemoil);

                    ReceivedOrderTransferDetailDTO objReceiveOrdTrnfDetailDTO = new ReceivedOrderTransferDetailDTO()
                    {
                        Action = string.Empty,
                        BinNumber = itemoil.BinNumber,
                        Cost = itemoil.Cost,
                        CreatedByName = string.Empty,
                        CriticalQuantity = itemoil.CriticalQuantity,
                        DateCodeTracking = itemoil.DateCodeTracking,
                        eVMISensorID = itemoil.eVMISensorID,
                        eVMISensorPort = itemoil.eVMISensorPort,
                        Expiration = itemoil.Expiration,
                        ExpirationDate = itemoil.ExpirationDate,
                        GUID = Guid.NewGuid(),
                        HistoryID = 0,
                        ID = 0,
                        IsCreditPull = true,
                        ItemLocationDetailGUID = itemoil.GUID,
                        ItemNumber = itemoil.ItemNumber,
                        ItemType = itemoil.ItemType,
                        KitDetailGUID = itemoil.KitDetailGUID,
                        LotNumber = (!string.IsNullOrWhiteSpace(itemoil.LotNumber)) ? itemoil.LotNumber.Trim() : string.Empty,
                        LotNumberTracking = itemoil.LotNumberTracking,
                        MaximumQuantity = itemoil.MaximumQuantity,
                        MeasurementID = itemoil.MeasurementID,
                        MinimumQuantity = itemoil.MinimumQuantity,
                        mode = "ReturnOrder",
                        Received = itemoil.Received,
                        ReceivedDate = itemoil.ReceivedDate,
                        RoomName = itemoil.RoomName,
                        TransferDetailGUID = itemoil.TransferDetailGUID,
                        UDF1 = itemoil.UDF1,
                        UDF2 = itemoil.UDF2,
                        UDF3 = itemoil.UDF3,
                        UDF4 = itemoil.UDF4,
                        UDF5 = itemoil.UDF5,
                        UDF6 = itemoil.UDF6,
                        UDF7 = itemoil.UDF7,
                        UDF8 = itemoil.UDF8,
                        UDF9 = itemoil.UDF9,
                        UDF10 = itemoil.UDF10,
                        BinID = objReturnQty.LocationID,
                        ItemGUID = objReturnQty.ItemGUID,
                        OrderDetailGUID = objReturnQty.OrderDetailGUID,
                        UpdatedByName = itemoil.UpdatedByName,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CreatedBy = UserID,
                        LastUpdatedBy = UserID,
                        CompanyID = CompanyID,
                        Room = RoomID,
                        IsArchived = false,
                        IsDeleted = false,
                        SerialNumberTracking = itemoil.SerialNumberTracking,
                        SerialNumber = (!string.IsNullOrWhiteSpace(itemoil.SerialNumber)) ? itemoil.SerialNumber.Trim() : string.Empty,
                        CustomerOwnedQuantity = loopCurrentTakenCustomer,
                        ConsignedQuantity = loopCurrentTakenConsignment,
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                    };

                    objRecvOrdDetailDAL.Insert(objReceiveOrdTrnfDetailDTO);
                }
                #endregion
            }
            #endregion

            #region "ItemLocation Quantity Deduction"

            ItemDTO.OnHandQuantity = ItemDTO.OnHandQuantity - objReturnQty.ReturnQuantity;
            if (ItemDTO.Consignment)
            {
                //Both's sum we have available.
                if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) == 0)
                {
                    CosigneQty = objReturnQty.ReturnQuantity;
                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity.GetValueOrDefault(0) - objReturnQty.ReturnQuantity;
                    lstLocDTO.Quantity = lstLocDTO.Quantity - objReturnQty.ReturnQuantity;
                }
                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) >= objReturnQty.ReturnQuantity)
                {
                    CustomerQty = objReturnQty.ReturnQuantity;
                    lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objReturnQty.ReturnQuantity;
                    lstLocDTO.Quantity = lstLocDTO.Quantity - objReturnQty.ReturnQuantity;
                }
                else if (lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) < objReturnQty.ReturnQuantity)
                {
                    Double cstqty = objReturnQty.ReturnQuantity - (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); // -((Double)objDTO.TempPullQTY - cstqty);
                    Double consqty = cstqty;// objReturnQty.Quantity - cstqty;

                    lstLocDTO.ConsignedQuantity = lstLocDTO.ConsignedQuantity - consqty;
                    CustomerQty = (Double)lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0); ;
                    CosigneQty = consqty;
                    lstLocDTO.CustomerOwnedQuantity = 0;
                    lstLocDTO.Quantity = lstLocDTO.Quantity - (CustomerQty + CosigneQty);
                }
            }
            else
            {
                lstLocDTO.CustomerOwnedQuantity = lstLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0) - objReturnQty.ReturnQuantity;
                lstLocDTO.Quantity = lstLocDTO.Quantity - objReturnQty.ReturnQuantity;
                CustomerQty = objReturnQty.ReturnQuantity;
            }
            #endregion

            #region "Saving Location QTY data"
            ItemDTO.LastUpdatedBy = UserID;
            ItemDTO.WhatWhereAction = "Return Order";
            ItemDTO.IsOnlyFromItemUI = isFromUI;
            if (isFromUI)
            {
                ItemDTO.EditedFrom = "Web";
                ItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            }
            objItemDAL.Edit(ItemDTO, SessionUserId, EnterpriseID);
            ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
            List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
            lstUpdate.Add(lstLocDTO);
            objLocQTY.Save(lstUpdate, SessionUserId, EnterpriseID);

            #endregion

            #region "Update ItemMaster and Order Detail table"


            OrderDetailsDTO ordDetailDTO = GetOrderDetailByGuidPlain(objReturnQty.OrderDetailGUID, RoomID, CompanyID);
            ordDetailDTO.ReceivedQuantity = ordDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + objReturnQty.ReturnQuantity;
            ordDetailDTO.ReceivedQuantityUOM = ordDetailDTO.ReceivedQuantity;
            ordDetailDTO.LastUpdatedBy = UserID;
            if (isFromUI)
            {
                ordDetailDTO.EditedFrom = "Web";
                ordDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            }
            Edit(ordDetailDTO, SessionUserId, EnterpriseID);
            UpdateOrderStatusByReceive(ordDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), ordDetailDTO.Room.GetValueOrDefault(0), ordDetailDTO.CompanyID.GetValueOrDefault(0), ordDetailDTO.LastUpdatedBy.GetValueOrDefault(0), isFromUI);

            response.IsSuccess = true;
            string msgQuantityReturnedSuccessfully = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityReturnedSuccessfully", orderMasterResourceFilePath, EnterpriseID, CompanyID, RoomID, "ResOrder", CultureCode);
            response.Message = msgQuantityReturnedSuccessfully;

            #endregion

            return response;
        }



        public bool CloseOrderDetailItem(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID, long SessionUserId, long EnterpriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@IDs", IDs),
                    new SqlParameter("@UserID", userid),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                     new SqlParameter("@EditedFrom", "Web"),
                       new SqlParameter("@IsCloseItem", "1")
                };

                    string strCommand = "EXEC Orddtl_CloseOrderDetailItem @IDs,@UserID,@RoomID,@CompanyID,@EditedFrom,@IsCloseItem";


                    /*
                    string strCommand = "EXEC Orddtl_CloseOrderDetailItem ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";
                    strCommand += ",1";
                    */

                    int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();

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
                                DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, SessionUserId, null, null);
                                ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                                objItemDTO.LastUpdatedBy = userid;
                                objItemDTO.IsOnlyFromItemUI = false;
                                itmDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);
                                UpdateOrderStatusByReceive(DetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), RoomId, CompanyID, userid);
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


        public bool UnCloseOrderDetailItem(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID, long SessionUserId, long EntepriseId)
        {
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@IDs", IDs),
                    new SqlParameter("@UserID", userid),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                     new SqlParameter("@EditedFrom", "Web"),
                       new SqlParameter("@IsCloseItem", "0")
                };

                    string strCommand = "EXEC Orddtl_CloseOrderDetailItem @IDs,@UserID,@RoomID,@CompanyID,@EditedFrom,@IsCloseItem";

                    /*
                    string strCommand = "EXEC Orddtl_CloseOrderDetailItem ";
                    strCommand += "null";
                    strCommand += ",null";
                    strCommand += ",'" + strIDs + "'";
                    strCommand += ",null";
                    strCommand += "," + userid;
                    strCommand += "," + RoomId;
                    strCommand += "," + CompanyID;
                    strCommand += ",'" + DateTimeUtility.DateTimeNow + "'";
                    strCommand += ",'Web'";
                    strCommand += ",0";
                    */

                    int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();

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
                                DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, SessionUserId, null, null);
                                ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                                objItemDTO.LastUpdatedBy = userid;
                                objItemDTO.IsOnlyFromItemUI = false;
                                itmDAL.Edit(objItemDTO, SessionUserId, EntepriseId);
                                UpdateOrderStatusByReceive(DetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), RoomId, CompanyID, userid, false, true);
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

        public bool DeleteRecords(string IDs, Int64 userid, Int64 RoomId, Int64 CompanyID, long SessionUserId, long EnterpriseId)
        {
            // return DB_DeleteRecords(IDs, userid, RoomId, CompanyID);
            string[] strArrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrIDs != null && strArrIDs.Length > 0)
            {
                string strIDs = string.Join(",", strArrIDs);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@IDs", IDs),
                    new SqlParameter("@UserID", userid),
                    new SqlParameter("@RoomID", RoomId),
                    new SqlParameter("@CompanyID", CompanyID),
                     new SqlParameter("@EditedFrom", "Web")
                };

                    string strCommand = "EXEC Orddtl_DeleteOrderDetail @IDs,@UserID,@RoomID,@CompanyID,@EditedFrom";

                    int intReturn = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();

                    //string[] arrIDs = IDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //if (arrIDs != null && arrIDs.Length > 0)
                    //{
                    //    DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
                    //    ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
                    //    foreach (var item in arrIDs)
                    //    {
                    //        OrderDetailsDTO DetailDTO = GetOrderDetailByIDPlain(Int64.Parse(item), RoomId, CompanyID);
                    //        try
                    //        {
                    //            DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, null, null);
                    //            DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), userid, SessionUserId, null, null);
                    //            ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
                    //            objItemDTO.LastUpdatedBy = userid;
                    //            objItemDTO.IsOnlyFromItemUI = false;
                    //            itmDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);
                    //        }
                    //        catch (Exception)
                    //        {

                    //        }
                    //    }
                    //}

                    if (intReturn > 0)
                        return true;
                }
            }
            return false;
        }


        public bool Edit(OrderDetailsDTO objDTO, long SessionUserId, long EnterpriseId)
        {

            OrderDetailsDTO DetailDTO = UpdateOrderDetail(objDTO);

            //try
            //{
            //    DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
            //    DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, null, null);
            //    DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.LastUpdatedBy ?? 0, SessionUserId, null, null);
            //}
            //catch (Exception)
            //{
            //    //Log Exception
            //}


            //ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
            //ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
            //objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            //objItemDTO.WhatWhereAction = "Order";
            //objItemDTO.IsOnlyFromItemUI = false;
            //itmDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);


            //return DetailDTO;
            return true;
        }


        public OrderDetailsDTO UpdateOrderDetail(OrderDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@DetailID",  objDTO.ID),
                        new SqlParameter("@DetailGUID", (objDTO.GUID != Guid.Empty) ? objDTO.GUID : (object)DBNull.Value),
                        new SqlParameter("@OrderGUID", objDTO.OrderGUID ),
                        new SqlParameter("@ItemGUID", objDTO.ItemGUID  ?? (object)DBNull.Value),
                        new SqlParameter("@Bin", objDTO.Bin  ?? (object)DBNull.Value),
                        new SqlParameter("@RequestedQuantity", objDTO.RequestedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantity", objDTO.ApprovedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@RequiredDate", objDTO.RequiredDate.GetValueOrDefault(DateTime.Now)),
                        new SqlParameter("@ReceivedQuantity", objDTO.ReceivedQuantity  ?? (object)DBNull.Value),
                        new SqlParameter("@LastUpdatedBy",  objDTO.LastUpdatedBy.GetValueOrDefault(0)),
                        new SqlParameter("@Room", objDTO.Room.GetValueOrDefault(0)),
                        new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)),
                        new SqlParameter("@IsEDISENT",objDTO.IsEDISent.GetValueOrDefault(false)),
                        new SqlParameter("@InTransitQuantity", objDTO.InTransitQuantity.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantity.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ASNNumber", objDTO.ASNNumber ?? (object)DBNull.Value),
                        new SqlParameter("@ReceivedOn", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)),
                        new SqlParameter("@EditedFrom", (!string.IsNullOrWhiteSpace(objDTO.EditedFrom)) ? objDTO.EditedFrom : "Web" ),
                        new SqlParameter("@Comment", objDTO.Comment ?? (object)DBNull.Value),
                        new SqlParameter("@LastEDIDate", objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? objDTO.LastEDIDate.GetValueOrDefault(DateTime.MinValue) : (object)DBNull.Value),
                        new SqlParameter("@IsCloseItem", objDTO.IsCloseItem.GetValueOrDefault(false)),
                        new SqlParameter("@UDF1", objDTO.UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", objDTO.UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", objDTO.UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", objDTO.UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", objDTO.UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@OrderLineItemExtendedCost", objDTO.OrderLineItemExtendedCost.GetValueOrDefault(0)),
                        new SqlParameter("@OrderLineItemExtendedPrice", objDTO.OrderLineItemExtendedPrice.GetValueOrDefault(0)),
                        new SqlParameter("@RequestedQuantityUOM", objDTO.RequestedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantityUOM", objDTO.ApprovedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@ReceivedQuantityUOM", objDTO.ReceivedQuantityUOM  ?? (object)DBNull.Value),
                        new SqlParameter("@InTransitQuantityUOM", objDTO.InTransitQuantityUOM.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantityUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ItemCost", objDTO.ItemCost.GetValueOrDefault(0)),
                        new SqlParameter("@SupplierId", objDTO.SupplierID.GetValueOrDefault(0) > 0 ? objDTO.SupplierID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierPartNo", objDTO.SupplierPartNo ?? (object)DBNull.Value),
                        new SqlParameter("@ItemSellPrice", objDTO.ItemSellPrice.GetValueOrDefault(0)),
                        new SqlParameter("@ItemMarkup", objDTO.ItemMarkup.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOMValue",(objDTO.ItemCostUOMValue == null || objDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0 ? 1 : objDTO.ItemCostUOMValue.GetValueOrDefault(1))),
                        new SqlParameter("@IsBackOrdered", objDTO.IsBackOrdered ?? (object)DBNull.Value),
                        new SqlParameter("@POItemLineNumber", objDTO.POItemLineNumber ?? (object)DBNull.Value),
                        new SqlParameter("@OrderLineException", objDTO.OrderLineException ?? (object)DBNull.Value),
                        new SqlParameter("@OrderLineExceptionDesc", objDTO.OrderLineExceptionDesc ?? (object)DBNull.Value)
                    };


                string strCommand = " EXEC [Orddtl_UpdateOrderDetailData] @DetailID,@DetailGUID,@OrderGUID,@ItemGUID,@Bin,@RequestedQuantity,@ApprovedQuantity,@RequiredDate,@ReceivedQuantity,@LastUpdatedBy,@Room,@CompanyID,@IsEDISENT,@InTransitQuantity,@ASNNumber,@ReceivedOn,@EditedFrom,@Comment,@LastEDIDate,@IsCloseItem,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@OrderLineItemExtendedCost,@OrderLineItemExtendedPrice,@RequestedQuantityUOM,@ApprovedQuantityUOM,@ReceivedQuantityUOM,@InTransitQuantityUOM,@ItemCost,@SupplierId,@SupplierPartNo,@ItemSellPrice,@ItemMarkup,@ItemCostUOMValue,@IsBackOrdered,@POItemLineNumber,@OrderLineException,@OrderLineExceptionDesc";

                context.Database.CommandTimeout = 7200;
                //return context.Database.SqlQuery<OrderDetailsDTO>(strCommand, params1).FirstOrDefault();
                context.Database.ExecuteSqlCommand(strCommand, params1);
                return null;

            }
        }

        public void UpdateOrderTrackingDetailData(long OrdeTrackingDetailID, long? CompanyID, long? RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@OrdeTrackingDetailID",  OrdeTrackingDetailID),
                        new SqlParameter("@RoomID",  RoomID),
                        new SqlParameter("@CompanyID",  CompanyID),
                    };

                string strCommand = " EXEC [UpdateOrderTrackingDetailData] @OrdeTrackingDetailID,@RoomID,@CompanyID";

                context.Database.ExecuteSqlCommand(strCommand, params1);
            }
        }

        public OrderDetailsDTO Insert(OrderDetailsDTO objDTO, long SessionUserId, long EnterpriseId)
        {
            OrderDetailsDTO DetailDTO = InsertOrderDetail(objDTO);
            //try
            //{
            //    DashboardDAL objDashbordDal = new DashboardDAL(base.DataBaseName);
            //    DashboardAnalysisInfo objDashbordTurns = objDashbordDal.UpdateTurnsByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.CreatedBy ?? 0, null, null);
            //    DashboardAnalysisInfo objDashbordAvgUsg = objDashbordDal.UpdateAvgUsageByItemGUIDAfterTxn(DetailDTO.Room.GetValueOrDefault(0), DetailDTO.CompanyID.GetValueOrDefault(0), DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objDTO.CreatedBy ?? 0, SessionUserId, null, null);
            //}
            //catch (Exception)
            //{

            //}


            //ItemMasterDAL itmDAL = new ItemMasterDAL(base.DataBaseName);
            //ItemMasterDTO objItemDTO = itmDAL.GetItemWithoutJoins(null, DetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty));
            //objItemDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
            //objItemDTO.WhatWhereAction = "Order";
            //itmDAL.Edit(objItemDTO, SessionUserId, EnterpriseId);


            return DetailDTO;

        }


        public OrderDetailsDTO InsertOrderDetail(OrderDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                      {
                        new SqlParameter("@OrderGUID", objDTO.OrderGUID.GetValueOrDefault(Guid.Empty) ),
                        new SqlParameter("@ItemGUID", objDTO.ItemGUID.GetValueOrDefault(Guid.Empty)),
                        new SqlParameter("@Bin", objDTO.Bin.GetValueOrDefault(0) > 0  ? objDTO.Bin.GetValueOrDefault(0): (object)DBNull.Value),
                        new SqlParameter("@RequestedQuantity", objDTO.RequestedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantity", objDTO.ApprovedQuantity.GetValueOrDefault(0)),
                        new SqlParameter("@RequiredDate", objDTO.RequiredDate.GetValueOrDefault(DateTime.Now)),
                        new SqlParameter("@CreatedBy",  objDTO.CreatedBy.GetValueOrDefault(0)),
                        new SqlParameter("@Room", objDTO.Room.GetValueOrDefault(0)),
                        new SqlParameter("@CompanyID", objDTO.CompanyID.GetValueOrDefault(0)),
                        new SqlParameter("@InTransitQuantity", objDTO.InTransitQuantity.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantity.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ASNNumber", objDTO.ASNNumber ?? (object)DBNull.Value),
                        new SqlParameter("@IsEDISent",objDTO.IsEDISent.GetValueOrDefault(false)),
                        new SqlParameter("@ReceivedOnWeb", objDTO.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow)),
                        new SqlParameter("@ReceivedON", objDTO.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow)),
                        new SqlParameter("@AddedFrom", (!string.IsNullOrWhiteSpace(objDTO.AddedFrom)) ? objDTO.AddedFrom : "Web" ),
                        new SqlParameter("@EditedFrom", (!string.IsNullOrWhiteSpace(objDTO.EditedFrom)) ? objDTO.EditedFrom : "Web" ),
                        new SqlParameter("@LineNumber", objDTO.LineNumber ?? (object)DBNull.Value),
                        new SqlParameter("@ControlNumber", objDTO.ControlNumber ?? (object)DBNull.Value),
                        new SqlParameter("@Comment", objDTO.Comment ?? (object)DBNull.Value),
                        new SqlParameter("@UDF1", objDTO.UDF1 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF2", objDTO.UDF2 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF3", objDTO.UDF3 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF4", objDTO.UDF4 ?? (object)DBNull.Value),
                        new SqlParameter("@UDF5", objDTO.UDF5 ?? (object)DBNull.Value),
                        new SqlParameter("@DetailGUID", objDTO.GUID != Guid.Empty ?  objDTO.GUID : (object)DBNull.Value),
                        new SqlParameter("@OrderLineItemExtendedCost", objDTO.OrderLineItemExtendedCost.GetValueOrDefault(0)),
                        new SqlParameter("@OrderLineItemExtendedPrice", objDTO.OrderLineItemExtendedPrice.GetValueOrDefault(0)),
                        new SqlParameter("@IsCloseItem",objDTO.IsCloseItem.GetValueOrDefault(false)),
                        new SqlParameter("@RequestedQuantityUOM", objDTO.RequestedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@ApprovedQuantityUOM", objDTO.ApprovedQuantityUOM.GetValueOrDefault(0)),
                        new SqlParameter("@InTransitQuantityUOM", objDTO.InTransitQuantityUOM.GetValueOrDefault(0) > 0 ? objDTO.InTransitQuantityUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@ItemCost", objDTO.ItemCost.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOM", objDTO.ItemCostUOM.GetValueOrDefault(0) > 0 ? objDTO.ItemCostUOM.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierId", objDTO.SupplierID.GetValueOrDefault(0) > 0 ? objDTO.SupplierID.GetValueOrDefault(0) : (object)DBNull.Value),
                        new SqlParameter("@SupplierPartNo", objDTO.SupplierPartNo ?? (object)DBNull.Value),
                        new SqlParameter("@ItemSellPrice", objDTO.ItemSellPrice.GetValueOrDefault(0)),
                        new SqlParameter("@ItemMarkup", objDTO.ItemMarkup.GetValueOrDefault(0)),
                        new SqlParameter("@ItemCostUOMValue", (objDTO.ItemCostUOMValue == null || objDTO.ItemCostUOMValue.GetValueOrDefault(0) <= 0 ? 1 : objDTO.ItemCostUOMValue.GetValueOrDefault(1))),
                        new SqlParameter("@IsBackOrdered", objDTO.IsBackOrdered ?? (object)DBNull.Value),
                        new SqlParameter("@POItemLineNumber", objDTO.POItemLineNumber ?? (object)DBNull.Value),
                        new SqlParameter("@OrderLineException", objDTO.OrderLineException ?? (object)DBNull.Value),
                        new SqlParameter("@OrderLineExceptionDesc", objDTO.OrderLineExceptionDesc ?? (object)DBNull.Value)
                      };


                string strCommand = " EXEC [Orddtl_InsertOrderDetailData] @OrderGUID,@ItemGUID,@Bin,@RequestedQuantity,@ApprovedQuantity,@RequiredDate,@CreatedBy,@Room,@CompanyID,@InTransitQuantity,@ASNNumber,@IsEDISent,@ReceivedOnWeb,@ReceivedON,@AddedFrom,@EditedFrom,@LineNumber,@ControlNumber,@Comment,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@DetailGUID,@OrderLineItemExtendedCost,@OrderLineItemExtendedPrice,@IsCloseItem,@RequestedQuantityUOM,@ApprovedQuantityUOM,@InTransitQuantityUOM,@ItemCost,@ItemCostUOM,@SupplierId,@SupplierPartNo,@ItemSellPrice,@ItemMarkup,@ItemCostUOMValue,@IsBackOrdered,@POItemLineNumber,@OrderLineException,@OrderLineExceptionDesc";
                context.Database.CommandTimeout = 7200;
                return context.Database.SqlQuery<OrderDetailsDTO>(strCommand, params1).FirstOrDefault();

            }
        }

        public int InsertOrderDetailTracking(List<OrderDetailTrackingDTO> objDTO)
        {
            int result = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                foreach (OrderDetailTrackingDTO item in objDTO)
                {
                    try
                    {
                        var params1 = new SqlParameter[]
                     {
                          new SqlParameter("@Room", item.Room.GetValueOrDefault(0)),
                          new SqlParameter("@CompanyID", item.CompanyID.GetValueOrDefault(0)),
                          new SqlParameter("@OrderDetailID", item.OrderDetailID != Guid.Empty ?  item.OrderDetailID : (object)DBNull.Value),
                          new SqlParameter("@SerialNumber", item.SerialNumber ?? (object)DBNull.Value),
                          new SqlParameter("@LotNumber", item.LotNumber ?? (object)DBNull.Value),
                          new SqlParameter("@ExpirationDate", item.ExpirationDate.HasValue ? (object)item.ExpirationDate : DBNull.Value),
                          new SqlParameter("@Quantity", item.Quantity.GetValueOrDefault(0)),
                          new SqlParameter("@CreatedBy",  item.CreatedBy.GetValueOrDefault(0)),
                          new SqlParameter("@PackSlipNumber",  item.PackSlipNumber ?? (object)DBNull.Value)
                     };

                        string strCommand = " EXEC [Orddtl_InsertOrderDetailTrackingData] @Room,@CompanyID,@OrderDetailID,@SerialNumber,@LotNumber,@ExpirationDate,@Quantity,@CreatedBy,@PackSlipNumber";
                        result = context.Database.SqlQuery<int>(strCommand, params1).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            return result;
        }


        public bool UpdateLineItemBin(OrderDetailsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderDetail objOD = context.OrderDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                objOD.Bin = objDTO.Bin;
                objOD.LastUpdated = DateTimeUtility.DateTimeNow;
                objOD.LastUpdatedBy = objDTO.LastUpdatedBy;
                objOD.ReceivedOn = DateTimeUtility.DateTimeNow;
                objOD.EditedFrom = "Web-BinSave";
                context.SaveChanges();
                return true;
            }
        }

        public bool UpdateLineComment(OrderDetailsDTO objDTO, Int64 UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new List<SqlParameter>() {
                new SqlParameter("@ID",objDTO.ID),
                new SqlParameter("@GUID",objDTO.GUID),
                new SqlParameter("@Comment",objDTO.Comment.ToDBNull()),
                new SqlParameter("@UDF1",objDTO.UDF1.ToDBNull()),
                new SqlParameter("@UDF2",objDTO.UDF2.ToDBNull()),
                new SqlParameter("@UDF3",objDTO.UDF3.ToDBNull()),
                new SqlParameter("@UDF4",objDTO.UDF4.ToDBNull()),
                new SqlParameter("@UDF5",objDTO.UDF5.ToDBNull()),
                new SqlParameter("@LastUpdatedBy",UserID),
                new SqlParameter("@LastUpdated", DateTimeUtility.DateTimeNow),
                new SqlParameter("@EditedFrom","Web"),
                };
                string strCommand = " EXEC [uspUpdateOrderDetailComment] @ID, @GUID ,@Comment, @UDF1,@UDF2 , @UDF3 , @UDF4 , @UDF5 , @LastUpdatedBy ,@LastUpdated ,@EditedFrom ";
                context.Database.SqlQuery<OrderDetailsDTO>(strCommand, params1.ToArray()).FirstOrDefault();


                //OrderDetail objODNew = null;
                //if (objDTO.ID > 0)
                //{
                //    objODNew = context.OrderDetails.FirstOrDefault(x => x.ID == objDTO.ID);
                //}
                //else
                //{
                //    objODNew = context.OrderDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                //}
                //if (objODNew != null)
                //{
                //    objODNew.Comment = objDTO.Comment;
                //    objODNew.UDF1 = objDTO.UDF1;
                //    objODNew.UDF2 = objDTO.UDF2;
                //    objODNew.UDF3 = objDTO.UDF3;
                //    objODNew.UDF4 = objDTO.UDF4;
                //    objODNew.UDF5 = objDTO.UDF5;
                //    objODNew.LastUpdatedBy = UserID;
                //    objODNew.LastUpdated = DateTimeUtility.DateTimeNow;
                //    objODNew.EditedFrom = "Web";
                //    context.SaveChanges();
                //}

                return true;
            }
        }

        public bool UpdateReqDateandCommenttoOrderLineItems(OrderDetailsDTO objDTO, Int64 UserID, bool isCommentUpdate, bool isReqDateUpdate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                OrderDetail objODNew = null;
                if (objDTO.ID > 0)
                {
                    objODNew = context.OrderDetails.FirstOrDefault(x => x.ID == objDTO.ID);
                }
                else
                {
                    objODNew = context.OrderDetails.FirstOrDefault(x => x.GUID == objDTO.GUID);
                }
                if (objODNew != null)
                {

                    if (isReqDateUpdate)
                        objODNew.RequiredDate = objDTO.RequiredDate;
                    if (isCommentUpdate)
                        objODNew.Comment = objDTO.Comment;
                    objODNew.LastUpdatedBy = UserID;
                    objODNew.LastUpdated = DateTimeUtility.DateTimeNow;
                    objODNew.EditedFrom = "Web";
                    context.SaveChanges();
                }
                return true;
            }
        }

        #endregion

        #region WI-6215

        public DataTable GetOrderDetailTableFromList(List<OrderDetailsDTO> lstOrderDetails)
        {
            DataTable ReturnDT = new DataTable("OrderDetailWithCost");
            try
            {
                DataColumn[] arrColumns = new DataColumn[]
                {
                    new DataColumn() { AllowDBNull=true,ColumnName="OrderGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="OrderDetailGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="ItemGuid",DataType=typeof(Guid)},
                    new DataColumn() { AllowDBNull=true,ColumnName="OrderDetailItemCost",DataType=typeof(double)}
                };
                ReturnDT.Columns.AddRange(arrColumns);

                if (lstOrderDetails != null && lstOrderDetails.Count > 0)
                {
                    foreach (var item in lstOrderDetails)
                    {
                        DataRow row = ReturnDT.NewRow();
                        row["OrderGuid"] = item.OrderGUID;
                        row["OrderDetailGuid"] = item.GUID;
                        row["ItemGuid"] = item.ItemGUID;
                        //row["OrderDetailItemCost"] = item.ItemCost.HasValue ? item.ItemCost.Value : DBNull.Value;
                        row["OrderDetailItemCost"] = item.ItemCost ?? 0;

                        ReturnDT.Rows.Add(row);
                    }
                }

                return ReturnDT;
            }
            catch
            {
                return ReturnDT;
            }
        }

        #endregion

        #region expand inner grid for return order

        public ReceivableItemDTO GetReturnItemsDetailsByOrderDetailGuid(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetReturnItemsDetailsByOrderDetailGuid] @CompanyID,@RoomID,@OrderDetailGUID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@OrderDetailGUID", OrderDetailGuid)
                };
                return context.Database.SqlQuery<ReceivableItemDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        #endregion
        public List<BackOrderedDetails> GetReturnItemsDetailsByOrderDetailGuid(string OrderDetailsGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetBackOrderedDetails] @OrderDetailsGUID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailsGUID", OrderDetailsGUID)
                };
                return context.Database.SqlQuery<BackOrderedDetails>(stryQry, params1).ToList();

            }
        }

        public bool Savebackordered(string OrderDetailGuid, string ItemGUID, double? BackOrderedQuantity, DateTime? ExpectedDate, long userid)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailsGuid", OrderDetailGuid),
                    new SqlParameter("@ItemGUID", ItemGUID),
                    new SqlParameter("@BackOrderedQuantity", BackOrderedQuantity.Value),
                    new SqlParameter("@ExpectedDate", ExpectedDate.Value),
                    new SqlParameter("@userid", userid),
                };
                    string stryQry = "EXEC [SaveBackOrderedDetails] @OrderDetailsGUID,@ItemGUID,@BackOrderedQuantity,@ExpectedDate,@userid";
                    context.Database.SqlQuery<int>(stryQry, params1).FirstOrDefault();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public List<OrderDetailsForSolum> getOrderDetailsForSolum(long comapnyid, long roomid)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@CompanyID", comapnyid),
                    new SqlParameter("@RoomID", roomid),
                };
                    string stryQry = "EXEC [SP_GETLimitOrderDetails] @CompanyID,@RoomID";
                    return context.Database.SqlQuery<OrderDetailsForSolum>(stryQry, params1).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<OrderDetailsForSolum> UpdateOrderDetailsForSolums(Int64 OrderDetailsForSolumID, string returnValue)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                    new SqlParameter("@OrderDetailsForSolumID", OrderDetailsForSolumID),
                    new SqlParameter("@ReturnValue", returnValue),
                };
                    string stryQry = "EXEC [UpdateOrderDetailsForSolums] @OrderDetailsForSolumID,@ReturnValue";
                    return context.Database.SqlQuery<OrderDetailsForSolum>(stryQry, params1).ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void InsertDetailToUpdateSolumFlags(Guid? OrderDetailGUID, Guid? OrderGUID, Guid ItemGUID, long EnterpriseId, long companyid, long roomid, double? InTransitQuantitySolum, string SupplierPartNoSolum, bool? IsBackOrderedSolum, int? OrderStatus, bool? DisaplyOrderException)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] {
                        new SqlParameter("@OrderDetailGUID", OrderDetailGUID),
                        new SqlParameter("@OrderGUID", OrderGUID),
                        new SqlParameter("@ItemGUID", ItemGUID),
                        new SqlParameter("@EnterpriseId", EnterpriseId),
                        new SqlParameter("@CompanyIDSolum", companyid),
                        new SqlParameter("@RoomSolum", roomid),
                        new SqlParameter("@InTransitQuantitySolum", InTransitQuantitySolum),
                        new SqlParameter("@SupplierPartNoSolum", SupplierPartNoSolum),
                        new SqlParameter("@IsBackOrderedSolum", IsBackOrderedSolum),
                        new SqlParameter("@IsDeleted", false),
                        new SqlParameter("@DisaplyOrderException", DisaplyOrderException),
                        new SqlParameter("@OrderStatus", OrderStatus),
                };
                    string stryQry = "EXEC [InsertOrderDetailsForSolum] @OrderDetailGUID,@OrderGUID,@ItemGUID,@EnterpriseId,@CompanyIDSolum,@RoomSolum,@InTransitQuantitySolum,@SupplierPartNoSolum,@IsBackOrderedSolum,@IsDeleted,@DisaplyOrderException,@OrderStatus";
                    context.Database.ExecuteSqlCommand(stryQry, params1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<PostProcessOrderDetails> GetPostProcessData()
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    return context.Database.SqlQuery<PostProcessOrderDetails>("EXEC [dbo].[GetDataForPostProcess]").ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void InsertFailData(Guid NewItemGUID, string Error)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@NewItemGUID", NewItemGUID),
                                               new SqlParameter("@Error", Error),};

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[UpdateFailDetails] @NewItemGUID, @Error", params1);
            }
        }

        public void UpdateStatusForOrderDetailsPostProcess(Guid GUID, bool IsStarted, bool IsCompleted)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@GUID", GUID),
                                                new SqlParameter("@IsStarted", IsStarted),
                                                new SqlParameter("@IsCompleted", IsCompleted)
                                              };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[UpdateStatusForOrderDetailPostProcess] @GUID, @IsStarted,@IsCompleted ", params1);
            }
        }

        public OrderDetailsDTO GetOrderDetailByGuidPlainArchieved(Guid OrderDetailGuid, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string stryQry = "EXEC [GetOrderDetailByGuidPlainArchieved] @GUID,@RoomID,@CompanyID";
                var params1 = new SqlParameter[] {
                    new SqlParameter("@GUID", OrderDetailGuid),
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID)
                };
                return context.Database.SqlQuery<OrderDetailsDTO>(stryQry, params1).FirstOrDefault();

            }
        }

        public void UpdateOrderUsedTotalValueBPO(Guid GUID, double OrderQuantityUsed, double OrderDollarUsed, string OperationType)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", GUID),
                                                new SqlParameter("@OrderQuantityUsed", OrderQuantityUsed),
                                                new SqlParameter("@OrderDollarUsed", OrderDollarUsed),
                                                new SqlParameter("@OperationType", OperationType)
                                              };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[UpdateOrderUsedTotalValue] @ItemGUID, @OrderQuantityUsed,@OrderDollarUsed,@OperationType ", params1);
            }
        }


    }
}


