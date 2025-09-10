using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;


namespace eTurns.DAL
{
    public partial class ToolAssetOrderDetailsDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolAssetOrderDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolAssetOrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolAssetOrderDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ToolAssetOrderDetailsDTO
                        {
                            ID = u.ID,
                            ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            ToolBinID = u.ToolBinID,
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
                        }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<ToolAssetOrderDetailsDTO> GetHistoryRecordsListByOrderID(string OrderGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ToolAssetOrderDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ToolAssetOrderDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.OrderGUID= '" + OrderGUID + "'")
                        select new ToolAssetOrderDetailsDTO
                        {
                            ID = u.ID,
                            ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            ToolBinID = u.ToolBinID,
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
                            ToolLocationName = u.ToolBinID.GetValueOrDefault(0) > 0 ? new LocationMasterDAL(base.DataBaseName).GetLocationByGuidPlain(u.ToolLocationGuid.GetValueOrDefault(Guid.Empty), u.Room.GetValueOrDefault(0), u.CompanyID.GetValueOrDefault(0)).Location : string.Empty,
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
                        }).ToList();
            }
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<ToolAssetOrderDetailsDTO> GetOrderLineItemHistory(Int64 OrderMasterHistoryID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, ";
                qry += " IM.[ID] as ItemID,IM.[GUID] as ItemGUID,IM.ItemNumber as ItemNumber,IM.MinimumQuantity, IM.MaximumQuantity,IM.GLAccountID as GLAccount,";
                qry += " IM.Cost, IM.Markup ,IM.SellPrice,IM.ManufacturerID as Manufacturer,IM.ManufacturerNumber,IM.SupplierID as Supplier,IM.SupplierPartNo,";
                qry += " IM.Description,IM.UOMID ,IM.OnOrderQuantity,IM.InTransitquantity,IM.[UDF1] as ItemUDF1,IM.[UDF2] as ItemUDF2,";
                qry += " IM.[UDF3] as ItemUDF3, IM.[UDF4] as ItemUDF4,IM.[UDF5] as ItemUDF5,UM.Unit as UOM, IM.ItemType,IM.ToolTypeTracking ";
                qry += " FROM ToolAssetOrderDetails_History A LEFT OUTER  JOIN ToolMaster IM on IM.GUID= A.ToolGuid";
                qry += "   LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID ";
                qry += " LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ";
                qry += "  WHERE A.HistroyID = " + OrderMasterHistoryID.ToString() + " ";

                return (from u in context.ExecuteStoreQuery<ToolAssetOrderDetailsDTO>(qry)
                        select new ToolAssetOrderDetailsDTO
                        {
                            ID = u.ID,
                            ToolAssetOrderGUID = u.ToolAssetOrderGUID,
                            ToolGUID = u.ToolGUID,
                            AssetGUID = u.AssetGUID,
                            ToolBinID = u.ToolBinID,
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
                            IsSerialDateCode = (u.ToolTypeTracking != null && u.ToolTypeTracking.Contains("2")) ? true : false
                        }).AsParallel().ToList();
            }

        }

        public ToolAssetOrderStatus UpdateOrderStatusByReceiveNew(Guid OrderGuid, Int64 RoomID, Int64 CompanyID, Int64 UserID, bool IsOnlyFromUI = false)
        {
            ToolAssetOrderMasterDAL ordMasterDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);

            ToolAssetOrderMasterDTO ordMasterDTO = ordMasterDAL.GetRecord(OrderGuid, RoomID, CompanyID);

            List<ReceivedOrderTransferDetailDTO> lstAllReceived = new List<ReceivedOrderTransferDetailDTO>();
            if (ordMasterDTO.OrderStatus > (int)ToolAssetOrderStatus.Approved && ordMasterDTO.OrderStatus <= (int)ToolAssetOrderStatus.Closed)
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ToolAssetOrderDetail> lstOrdDetails = context.ToolAssetOrderDetails.Where(x => x.ToolAssetOrderGUID == OrderGuid && !(x.IsCloseTool ?? false));
                    if (lstOrdDetails != null && lstOrdDetails.Count() > 0)
                    {
                        IEnumerable<ToolAssetOrderDetail> lstIncompleteOrdDetails = lstOrdDetails.Where(x => (x.ApprovedQuantity ?? 0) > (x.ReceivedQuantity ?? 0));

                        if (lstIncompleteOrdDetails.Count() > 0)
                        {
                            DateTime MaxReceiveDate = DateTime.Now;
                            DateTime MaxRequiredDate = DateTime.Now;

                            IEnumerable<Guid> lstOrderDetailsGuid = lstOrdDetails.Select(x => x.GUID);
                            MaxRequiredDate = lstIncompleteOrdDetails.Select(x => (x.RequiredDate ?? DateTime.MinValue)).Max();

                            string strOrderDetailGuids = string.Join(",", lstOrderDetailsGuid.ToArray());
                            DateTime? rcvdate = context.ExecuteStoreQuery<DateTime?>(@"SELECT Max(ReceivedDate) FROM ReceivedOrderTransferDetail 
                                                                                       WHERE ReceivedDate IS NOT NULL AND ISNULL(IsDeleted,0)=0 AND ISNULL(IsArchived,0)=0 
                                                                                       AND OrderDetailGUID IN (SELECT SplitValue FROM dbo.split('" + strOrderDetailGuids + @"',','))").FirstOrDefault();
                            if (rcvdate.HasValue)
                                MaxReceiveDate = rcvdate.GetValueOrDefault(DateTime.MinValue);

                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";

                            if (MaxRequiredDate < MaxReceiveDate && ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.TransmittedInCompletePastDue)
                            {
                                ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedInCompletePastDue;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                            else if (MaxRequiredDate > MaxReceiveDate && ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.TransmittedIncomplete)
                            {
                                ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.TransmittedIncomplete;
                                ordMasterDAL.Edit(ordMasterDTO);
                            }
                        }
                        else if (ordMasterDTO.OrderStatus != (int)ToolAssetOrderStatus.Closed)
                        {
                            ordMasterDTO.OrderStatus = (int)ToolAssetOrderStatus.Closed;
                            ordMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.LastUpdatedBy = UserID;
                            ordMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            ordMasterDTO.EditedFrom = "";
                            if (IsOnlyFromUI)
                                ordMasterDTO.EditedFrom = "Web";

                            ordMasterDAL.Edit(ordMasterDTO);
                        }
                    }
                }
            }
            return (ToolAssetOrderStatus)ordMasterDTO.OrderStatus;
        }

        public List<KeyValDTO> GetPendingAndClosedOrderItems(string OrderStatusIn, string RoomID, string CompanyID, DateTime? StartDate, DateTime? EndDate)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string query = " SELECT DISTINCT Convert(VARCHAR(100),ODV.ItemGUID) as [key],ODV.ItemNumber[value] FROM ToolAssetOrderDetails_View ODV  WHERE ISNULL(ODV.IsDeleted,0) = 0 AND ISNULL(ODV.IsArchived,0) = 0 AND ISNULL(ODV.IsCloseItem,0) = 0 ";

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
    }
}
