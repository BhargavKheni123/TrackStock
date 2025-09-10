using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class TransferDetailDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TransferDetailDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
        {
            return DB_GetCachedData(CompanyID, RoomID, null, null, null, null, null, SupplierIds);
        }

        /// <summary>
        /// Get Paged Records from the TransferDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<TransferDetailDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, List<long> SuppierIds)
        {
            return DB_GetCachedData(CompanyId, RoomID, IsDeleted, IsArchived, null, null, null, SuppierIds);
        }

        public IEnumerable<TransferDetailDTO> GetOnTransferQtyDetail(Int64 RoomID, Int64 CompanyId, bool? IsArchived, bool? IsDeleted, List<long> SupplierIds, Guid ItemGUID, Int64? BinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strSupplierIds = string.Empty;

                if (SupplierIds != null && SupplierIds.Any())
                {
                    strSupplierIds = string.Join(",", SupplierIds);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompnayID", CompanyId),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value),
                                                   new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value),
                                                   new SqlParameter("@SupplierIds", strSupplierIds),
                                                   new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID ?? (object)DBNull.Value) };

                IEnumerable<TransferDetailDTO> obj = (from u in context.ExecuteStoreQuery<TransferDetailDTO>("EXEC [Trnsfrdtl_GetTransferDetailDataByItemGUID] @CompnayID,@RoomID,@IsDeleted,@IsArchived,@SupplierIds,@ItemGUID,@BinID", params1)
                                                      select new TransferDetailDTO
                                                      {
                                                          ID = u.ID,
                                                          TransferGUID = u.TransferGUID,
                                                          ItemGUID = u.ItemGUID,
                                                          Bin = u.Bin,
                                                          RequestedQuantity = u.RequestedQuantity,
                                                          RequiredDate = u.RequiredDate,
                                                          ReceivedQuantity = u.ReceivedQuantity,
                                                          FulFillQuantity = u.FulFillQuantity,
                                                          ApprovedQuantity = u.ApprovedQuantity,
                                                          Created = u.Created,
                                                          LastUpdated = u.LastUpdated,
                                                          CreatedBy = u.CreatedBy,
                                                          LastUpdatedBy = u.LastUpdatedBy,
                                                          Room = u.Room,
                                                          IsDeleted = u.IsDeleted,
                                                          IsArchived = u.IsArchived,
                                                          CompanyID = u.CompanyID,
                                                          GUID = u.GUID,
                                                          IntransitQuantity = u.IntransitQuantity,
                                                          ShippedQuantity = u.ShippedQuantity,
                                                          Action = string.Empty,
                                                          HistoryID = 0,
                                                          IsHistory = false,
                                                          AverageCost = u.AverageCost,
                                                          BinName = u.BinName,
                                                          Cost = u.Cost,
                                                          CreatedByName = u.CreatedByName,
                                                          Description = u.Description,
                                                          ItemID = u.ItemID,
                                                          ItemNumber = u.ItemNumber,
                                                          ManufacturerName = u.ManufacturerName,
                                                          ManufacturerNumber = u.ManufacturerNumber,
                                                          OnHandQuantity = u.OnHandQuantity,
                                                          OnOrderQuantity = u.OnOrderQuantity,
                                                          OnTransferQuantity = u.OnTransferQuantity,
                                                          RoomName = u.RoomName,
                                                          SupplierName = u.SupplierName,
                                                          SupplierPartNo = u.SupplierPartNo,
                                                          UpdatedByName = u.UpdatedByName,
                                                          DefaultLocation = u.DefaultLocation,
                                                          SerialNumberTracking = u.SerialNumberTracking,
                                                          LotNumberTracking = u.LotNumberTracking,
                                                          DateCodeTracking = u.DateCodeTracking,
                                                          ReceivedOn = u.ReceivedOn,
                                                          ReceivedOnWeb = u.ReceivedOnWeb,
                                                          AddedFrom = u.AddedFrom,
                                                          EditedFrom = u.EditedFrom,
                                                          TotalRecords = 0,
                                                          IsLotSelectionRequire = u.IsLotSelectionRequire,
                                                          ReturnedQuantity = u.ReturnedQuantity,
                                                          StagedQuantity = u.StagedQuantity,
                                                          DestinationBin = u.DestinationBin
                                                      }).AsParallel().ToList();
                return obj;

            }

        }

        /// <summary>
        /// Get Particullar Record from the TransferDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferDetailDTO GetRecord(Guid Guid, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(RoomID, CompanyID, null, Guid);
            //return GetCachedData(RoomID, CompanyID, 0).SingleOrDefault(t => t.GUID == Guid);
        }

        /// <summary>
        /// Get Particullar Record from the TransferDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferDetailDTO GetRecord(Int64 ID, Int64 RoomID, Int64 CompanyID)
        {
            return DB_GetRecord(RoomID, CompanyID, ID, null);
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferDetailDTO> GetTransferedRecord(Guid TransferGUID, Int64 RoomID, Int64 CompanyID, List<long> SupplierIds)
        {
            return DB_GetCachedData(CompanyID, RoomID, false, false, null, null, TransferGUID, SupplierIds).ToList();
        }

        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public List<TransferDetailDTO> GetTransferLineItemHistory(Int64 TransferMasterHistoryID)
        {
            //Get Cached-Media
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string qry = "SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName, ";
                qry += " IM.[ID] as ItemID,IM.[GUID] as ItemGUID,IM.ItemNumber as ItemNumber,IM.MinimumQuantity, IM.MaximumQuantity,IM.GLAccountID as GLAccount,";
                qry += " IM.Cost, IM.Markup ,IM.SellPrice,IM.ManufacturerID as Manufacturer,IM.ManufacturerNumber,IM.SupplierID as Supplier,IM.SupplierPartNo,";
                qry += " IM.Description,IM.UOMID ,IM.OnOrderQuantity,IM.InTransitquantity,IM.[UDF1] as ItemUDF1,IM.[UDF2] as ItemUDF2,";
                qry += " IM.[UDF3] as ItemUDF3, IM.[UDF4] as ItemUDF4,IM.[UDF5] as ItemUDF5,UM.Unit as UOM, IM.ItemType ";
                qry += " FROM TransferDetail_History A LEFT OUTER  JOIN ItemMaster IM on IM.ID= A.ItemID";
                qry += " LEFT OUTER  JOIN UnitMaster UM on UM.ID= IM.UOMID LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID ";
                qry += " LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID ";
                qry += "  WHERE A.TransferMasterHistoryID = " + TransferMasterHistoryID.ToString() + " ";

                return (from u in context.ExecuteStoreQuery<TransferDetailDTO>(qry)
                        select new TransferDetailDTO
                        {
                            ID = u.ID,
                            TransferGUID = u.TransferGUID,

                            ItemGUID = u.ItemGUID,
                            Bin = u.Bin,
                            RequestedQuantity = u.RequestedQuantity,
                            RequiredDate = u.RequiredDate,
                            ReceivedQuantity = u.ReceivedQuantity,
                            ApprovedQuantity = u.ApprovedQuantity,
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
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ControlNumber = u.ControlNumber
                            //ItemDetail = new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.ToString(), u.Room, u.CompanyID)

                        }).AsParallel().ToList();
            }

        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public List<TransferDetailDTO> GetHistoryRecordsListByOrderID(Guid TransferGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<TransferDetailDTO> objLstTransferDetail = (from u in context.ExecuteStoreQuery<TransferDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                                                                                 FROM TransferDetail_History A  LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                                                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                                                                                                LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                 WHERE A.TransferGUID='" + TransferGUID.ToString() + "'")
                                                                select new TransferDetailDTO
                                                                {
                                                                    ID = u.ID,
                                                                    TransferGUID = u.TransferGUID,
                                                                    ItemGUID = u.ItemGUID,
                                                                    Bin = u.Bin,
                                                                    RequestedQuantity = u.RequestedQuantity,
                                                                    RequiredDate = u.RequiredDate,
                                                                    ReceivedQuantity = u.ReceivedQuantity,
                                                                    ApprovedQuantity = u.ApprovedQuantity,
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
                                                                    ReceivedOn = u.ReceivedOn,
                                                                    ReceivedOnWeb = u.ReceivedOnWeb,
                                                                    AddedFrom = u.AddedFrom,
                                                                    EditedFrom = u.EditedFrom,
                                                                    BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetBinByID(u.Bin.GetValueOrDefault(0), u.Room, u.CompanyID).BinNumber : string.Empty,
                                                                    //BinName = u.Bin.GetValueOrDefault(0) > 0 ? new BinMasterDAL(base.DataBaseName).GetItemLocation( u.Room, u.CompanyID, false, false,Guid.Empty, u.Bin.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber : string.Empty,
                                                                    ControlNumber = u.ControlNumber
                                                                    //ItemDetail = new ItemMasterDAL(base.DataBaseName).GetRecord(u.ItemGUID.ToString(), u.Room, u.CompanyID),
                                                                }).ToList();

                foreach (var item in objLstTransferDetail)
                {
                    ItemMasterDTO ItemDetail = new ItemMasterDAL(base.DataBaseName).GetItemWithMasterTableJoins(null, item.ItemGUID, item.Room, item.CompanyID);

                    item.ItemID = ItemDetail.ID;
                    item.ItemNumber = ItemDetail.ItemNumber;
                    item.SupplierName = ItemDetail.SupplierName;
                    item.SupplierPartNo = ItemDetail.SupplierPartNo;
                    item.ManufacturerName = ItemDetail.ManufacturerName;
                    item.ManufacturerNumber = ItemDetail.ManufacturerNumber;
                    item.OnHandQuantity = ItemDetail.OnHandQuantity;
                    item.AverageCost = ItemDetail.AverageCost;
                    item.Cost = ItemDetail.Cost;
                    item.Description = ItemDetail.Description;
                    item.OnTransferQuantity = item.OnTransferQuantity;
                    item.OnOrderQuantity = item.OnOrderQuantity;
                    item.DefaultLocation = item.DefaultLocation;

                }
                return objLstTransferDetail;
            }
        }

        /// <summary>
        /// Get Particullar Record from the OrderDetails by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public TransferDetailDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<TransferDetailDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM TransferDetail_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new TransferDetailDTO
                        {
                            ID = u.ID,
                            TransferGUID = u.TransferGUID,

                            ItemGUID = u.ItemGUID,
                            Bin = u.Bin,
                            RequestedQuantity = u.RequestedQuantity,
                            RequiredDate = u.RequiredDate,
                            ReceivedQuantity = u.ReceivedQuantity,
                            ApprovedQuantity = u.ApprovedQuantity,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            GUID = u.GUID,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ControlNumber = u.ControlNumber
                        }).SingleOrDefault();
            }
        }
    }
}
