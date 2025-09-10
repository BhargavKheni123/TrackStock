using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class PullTransactionDAL : eTurnsBaseDAL
    {
        public List<ItemLocationDetailsDTO> GetItemLocationsDetails(Guid ItemGUID, long RoomID, long BinID, double PullQuantity, string InventoryConsuptionMethod)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationDetailsDTO> lstItemLocations = new List<ItemLocationDetailsDTO>();
                if (!string.IsNullOrWhiteSpace(InventoryConsuptionMethod))
                {
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate descending
                                                select new ItemLocationDetailsDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    eVMISensorID = string.Empty,
                                                    eVMISensorIDdbl = 0,
                                                    eVMISensorPort = 0,
                                                    eVMISensorPortstr = string.Empty,
                                                    HistoryID = 0,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    mode = string.Empty,
                                                    PackSlipNumber = string.Empty,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    RoomName = string.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    UpdatedByName = string.Empty,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    CompanyID = il.CompanyID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    Created = il.Created,
                                                    CreatedBy = il.CreatedBy,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    IsArchived = il.IsArchived,
                                                    IsDeleted = il.IsDeleted,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LastUpdatedBy = il.LastUpdatedBy,
                                                    LotNumber = il.LotNumber,
                                                    MeasurementID = il.MeasurementID,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    Updated = il.Updated
                                                }).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate ascending
                                                select new ItemLocationDetailsDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    eVMISensorID = string.Empty,
                                                    eVMISensorIDdbl = 0,
                                                    eVMISensorPort = 0,
                                                    eVMISensorPortstr = string.Empty,
                                                    HistoryID = 0,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    mode = string.Empty,
                                                    PackSlipNumber = string.Empty,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    RoomName = string.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    UpdatedByName = string.Empty,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    CompanyID = il.CompanyID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    Created = il.Created,
                                                    CreatedBy = il.CreatedBy,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    IsArchived = il.IsArchived,
                                                    IsDeleted = il.IsDeleted,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LastUpdatedBy = il.LastUpdatedBy,
                                                    LotNumber = il.LotNumber,
                                                    MeasurementID = il.MeasurementID,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    Updated = il.Updated
                                                }).ToList();
                            break;
                        default:
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate ascending
                                                select new ItemLocationDetailsDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    eVMISensorID = string.Empty,
                                                    eVMISensorIDdbl = 0,
                                                    eVMISensorPort = 0,
                                                    eVMISensorPortstr = string.Empty,
                                                    HistoryID = 0,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    mode = string.Empty,
                                                    PackSlipNumber = string.Empty,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    RoomName = string.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    UpdatedByName = string.Empty,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    CompanyID = il.CompanyID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    Created = il.Created,
                                                    CreatedBy = il.CreatedBy,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    IsArchived = il.IsArchived,
                                                    IsDeleted = il.IsDeleted,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LastUpdatedBy = il.LastUpdatedBy,
                                                    LotNumber = il.LotNumber,
                                                    MeasurementID = il.MeasurementID,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    Updated = il.Updated
                                                }).ToList();
                            break;

                    }

                }

                return lstItemLocations;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsLotSerials(Guid ItemGUID, long RoomID, long BinID, double PullQuantity)
        {
            string InventoryConsuptionMethod = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                Room objRoomDTO = context.Rooms.FirstOrDefault(t => t.ID == RoomID);
                if (objRoomDTO != null)
                {
                    InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                }
                List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
                if (!string.IsNullOrWhiteSpace(InventoryConsuptionMethod))
                {
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate descending
                                                select new ItemLocationLotSerialDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LotNumber = il.LotNumber,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                                    LotSerialQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                                    LotOrSerailNumber = im.LotNumberTracking ? il.LotNumber : im.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                                    PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0)
                                                }).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate ascending
                                                select new ItemLocationLotSerialDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LotNumber = il.LotNumber,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                                    LotSerialQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                                    LotOrSerailNumber = im.LotNumberTracking ? il.LotNumber : im.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                                    PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0)
                                                }).ToList();
                            break;
                        default:
                            lstItemLocations = (from il in context.ItemLocationDetails
                                                join im in context.ItemMasters on il.ItemGUID equals im.GUID
                                                join bm in context.BinMasters on il.BinID equals bm.ID
                                                where il.ItemGUID == ItemGUID && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                                                orderby il.ReceivedDate ascending
                                                select new ItemLocationLotSerialDTO
                                                {
                                                    BinNumber = bm.BinNumber,
                                                    CriticalQuantity = im.CriticalQuantity,
                                                    DateCodeTracking = im.DateCodeTracking,
                                                    IsCreditPull = false,
                                                    IsDefault = bm.IsDefault ?? false,
                                                    IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                                                    ItemNumber = im.ItemNumber,
                                                    ItemType = im.ItemType,
                                                    LotNumberTracking = im.LotNumberTracking,
                                                    Markup = im.Markup,
                                                    MaximumQuantity = im.MaximumQuantity,
                                                    MinimumQuantity = 0,
                                                    ProjectSpentGUID = Guid.Empty,
                                                    SellPrice = 0,
                                                    SerialNumberTracking = im.SerialNumberTracking,
                                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                                    WorkOrderGUID = Guid.Empty,
                                                    BinID = il.BinID,
                                                    ConsignedQuantity = il.ConsignedQuantity,
                                                    Cost = il.Cost,
                                                    CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                                    Expiration = il.Expiration,
                                                    ExpirationDate = il.ExpirationDate,
                                                    GUID = il.GUID,
                                                    ID = il.ID,
                                                    ItemGUID = il.ItemGUID,
                                                    KitDetailGUID = il.KitDetailGUID,
                                                    LotNumber = il.LotNumber,
                                                    OrderDetailGUID = il.OrderDetailGUID,
                                                    Received = il.Received,
                                                    ReceivedDate = il.ReceivedDate,
                                                    Room = il.Room,
                                                    SerialNumber = il.SerialNumber,
                                                    TransferDetailGUID = il.TransferDetailGUID,
                                                    IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                                    LotSerialQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                                    LotOrSerailNumber = im.LotNumberTracking ? il.LotNumber : im.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                                    PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0)
                                                }).ToList();
                            break;

                    }

                }

                return lstItemLocations;
            }
        }

        private void CreatePullDetailsCache(PullDetail oPullDetail)
        {
            PullDetailsDTO objDTO = new PullDetailsDTO();

            objDTO.ID = oPullDetail.ID;
            objDTO.PULLGUID = oPullDetail.PULLGUID;
            objDTO.ItemGUID = oPullDetail.ItemGUID;
            objDTO.ProjectSpendGUID = oPullDetail.ProjectSpendGUID;
            objDTO.ItemCost = oPullDetail.ItemCost;
            objDTO.CustomerOwnedQuantity = oPullDetail.CustomerOwnedQuantity;
            objDTO.ConsignedQuantity = oPullDetail.ConsignedQuantity;
            objDTO.PoolQuantity = oPullDetail.PoolQuantity;
            objDTO.SerialNumber = oPullDetail.SerialNumber;
            objDTO.LotNumber = oPullDetail.LotNumber;
            objDTO.Expiration = oPullDetail.Expiration;
            objDTO.Received = oPullDetail.Received;
            objDTO.BinID = oPullDetail.BinID;
            objDTO.Created = oPullDetail.Created;
            objDTO.Updated = oPullDetail.Updated;
            objDTO.CreatedBy = oPullDetail.CreatedBy;
            objDTO.LastUpdatedBy = oPullDetail.LastUpdatedBy;
            objDTO.IsDeleted = oPullDetail.IsDeleted;
            objDTO.IsArchived = oPullDetail.IsArchived;
            objDTO.CompanyID = oPullDetail.CompanyID;
            objDTO.Room = oPullDetail.Room;
            objDTO.PullCredit = oPullDetail.PullCredit;
            objDTO.ItemLocationDetailGUID = oPullDetail.ItemLocationDetailGUID;
            objDTO.GUID = oPullDetail.GUID;
            objDTO.MaterialStagingPullDetailGUID = oPullDetail.MaterialStagingPullDetailGUID;
            objDTO.ItemOnhandQty = oPullDetail.ItemOnhandQty;
            objDTO.IsAddedFromPDA = oPullDetail.IsAddedFromPDA;
            objDTO.IsProcessedAfterSync = oPullDetail.IsProcessedAfterSync;
            objDTO.ReceivedOnWeb = oPullDetail.ReceivedOnWeb;
            objDTO.ReceivedOn = oPullDetail.ReceivedOn;
            objDTO.AddedFrom = oPullDetail.AddedFrom;
            objDTO.EditedFrom = oPullDetail.EditedFrom;
            objDTO.CreditCustomerOwnedQuantity = oPullDetail.CreditCustomerOwnedQuantity;
            objDTO.CreditConsignedQuantity = oPullDetail.CreditConsignedQuantity;

            IEnumerable<PullDetailsDTO> ObjCache = CacheHelper<IEnumerable<PullDetailsDTO>>.GetCacheItem("Cached_PullDetails_" + objDTO.CompanyID.ToString());
            if (ObjCache != null)
            {
                List<PullDetailsDTO> objTemp = ObjCache.ToList();
                if (objTemp.Any(x => x.ID == objDTO.ID))
                {
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                }

                ObjCache = objTemp.AsEnumerable();

                List<PullDetailsDTO> tempC = new List<PullDetailsDTO>();
                tempC.Add(objDTO);
                IEnumerable<PullDetailsDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                CacheHelper<IEnumerable<PullDetailsDTO>>.AppendToCacheItem("Cached_PullDetails_" + objDTO.CompanyID.ToString(), NewCache);
            }
        }

        public void CreateItemLocationDetailsCache(ItemLocationDetail oItemLocationDetail)
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

        public void CreateMSPullDetailsCache(MaterialStagingPullDetail oItemLocationDetail)
        {
            MaterialStagingPullDetailDTO objDTO = new MaterialStagingPullDetailDTO();

            objDTO.ID = oItemLocationDetail.ID;
            objDTO.BinID = oItemLocationDetail.BinID;
            objDTO.CustomerOwnedQuantity = oItemLocationDetail.CustomerOwnedQuantity;
            objDTO.ConsignedQuantity = oItemLocationDetail.ConsignedQuantity;

            objDTO.LotNumber = oItemLocationDetail.LotNumber;
            objDTO.SerialNumber = oItemLocationDetail.SerialNumber;
            objDTO.Expiration = oItemLocationDetail.Expiration;
            objDTO.Received = oItemLocationDetail.Received;
            objDTO.ItemCost = oItemLocationDetail.ItemCost;
            objDTO.MaterialStagingGUID = oItemLocationDetail.MaterialStagingGUID;
            objDTO.MaterialStagingdtlGUID = oItemLocationDetail.MaterialStagingdtlGUID;

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

            objDTO.OrderDetailGUID = oItemLocationDetail.OrderDetailGUID;

            objDTO.ReceivedOnWeb = oItemLocationDetail.ReceivedOnWeb;
            objDTO.ReceivedOn = oItemLocationDetail.ReceivedOn;
            objDTO.AddedFrom = oItemLocationDetail.AddedFrom;
            objDTO.EditedFrom = oItemLocationDetail.EditedFrom;

            IEnumerable<MaterialStagingPullDetailDTO> ObjCache = CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.GetCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString());
            if (ObjCache != null)
            {
                List<MaterialStagingPullDetailDTO> objTemp = ObjCache.ToList();
                if (objTemp.Any(x => x.ID == objDTO.ID))
                {
                    objTemp.RemoveAll(i => i.ID == objDTO.ID);
                }

                ObjCache = objTemp.AsEnumerable();

                List<MaterialStagingPullDetailDTO> tempC = new List<MaterialStagingPullDetailDTO>();
                tempC.Add(objDTO);
                IEnumerable<MaterialStagingPullDetailDTO> NewCache = ObjCache.Concat(tempC.AsEnumerable());
                CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.AppendToCacheItem("Cached_MaterialStagingPullDetail_" + objDTO.CompanyID.ToString(), NewCache);
            }
        }

        public ItemLocationLotSerialDTO GetLotSerialDetails(Guid ItemGUID, string LotOrSerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from il in context.ItemLocationDetails
                        join im in context.ItemMasters on il.ItemGUID equals im.GUID
                        join bm in context.BinMasters on il.BinID equals bm.ID
                        where il.ItemGUID == ItemGUID && (il.SerialNumber == LotOrSerialNumber || il.LotNumber == LotOrSerialNumber) && (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) > 0) && (il.IsDeleted ?? false) == false && (il.IsArchived ?? false) == false
                        select new ItemLocationLotSerialDTO
                        {
                            BinNumber = bm.BinNumber,
                            CriticalQuantity = im.CriticalQuantity,
                            DateCodeTracking = im.DateCodeTracking,
                            IsCreditPull = false,
                            IsDefault = bm.IsDefault ?? false,
                            IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                            ItemNumber = im.ItemNumber,
                            ItemType = im.ItemType,
                            LotNumberTracking = im.LotNumberTracking,
                            Markup = im.Markup,
                            MaximumQuantity = im.MaximumQuantity,
                            MinimumQuantity = 0,
                            ProjectSpentGUID = Guid.Empty,
                            SellPrice = 0,
                            SerialNumberTracking = im.SerialNumberTracking,
                            SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                            WorkOrderGUID = Guid.Empty,
                            BinID = il.BinID,
                            ConsignedQuantity = il.ConsignedQuantity,
                            Cost = il.Cost,
                            CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                            Expiration = il.Expiration,
                            ExpirationDate = il.ExpirationDate,
                            GUID = il.GUID,
                            ID = il.ID,
                            ItemGUID = il.ItemGUID,
                            KitDetailGUID = il.KitDetailGUID,
                            LotNumber = il.LotNumber,
                            OrderDetailGUID = il.OrderDetailGUID,
                            Received = il.Received,
                            ReceivedDate = il.ReceivedDate,
                            Room = il.Room,
                            SerialNumber = il.SerialNumber,
                            TransferDetailGUID = il.TransferDetailGUID,
                            IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                            LotSerialQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                            LotOrSerailNumber = im.LotNumberTracking ? il.LotNumber : im.SerialNumberTracking ? il.SerialNumber : string.Empty,
                            PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0)


                        }).FirstOrDefault();
            }
        }

        /// <summary>
        /// PullItemQuantity_Old As PerIrfan was Made
        /// </summary>
        /// <param name="objItemPullInfo"></param>
        /// <param name="PullCredit"></param>
        /// <returns></returns>
        public ItemPullInfo PullItemQuantity_OldIrf(ItemPullInfo objItemPullInfo, string PullCredit = "Pull")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);

                PullMaster objPullMaster = new PullMaster();
                objPullMaster.ActionType = PullCredit;
                objPullMaster.Billing = null;
                objPullMaster.BinID = objItemPullInfo.BinID;
                objPullMaster.CompanyID = objItemPullInfo.CompanyId;
                objPullMaster.ConsignedQuantity = 0;
                objPullMaster.CustomerOwnedQuantity = 0;
                objPullMaster.GUID = Guid.NewGuid();
                objPullMaster.IsArchived = false;
                objPullMaster.IsDeleted = false;
                objPullMaster.IsEDIRequired = null;
                objPullMaster.IsEDISent = null;
                objPullMaster.ItemGUID = objItemPullInfo.ItemGUID;
                objPullMaster.LastEDIDate = null;
                objPullMaster.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                objPullMaster.PoolQuantity = objItemPullInfo.PullQuantity;
                objPullMaster.ProjectSpendGUID = objItemPullInfo.ProjectSpendGUID;
                objPullMaster.PULLCost = 0;
                objPullMaster.PullCredit = PullCredit;
                objPullMaster.RequisitionDetailGUID = null;
                objPullMaster.Room = objItemPullInfo.RoomId;
                objPullMaster.UDF1 = objItemPullInfo.UDF1;
                objPullMaster.UDF2 = objItemPullInfo.UDF2;
                objPullMaster.UDF3 = objItemPullInfo.UDF3;
                objPullMaster.UDF4 = objItemPullInfo.UDF4;
                objPullMaster.UDF5 = objItemPullInfo.UDF5;
                objPullMaster.Updated = DateTimeUtility.DateTimeNow;
                objPullMaster.WhatWhereAction = PullCredit;
                objPullMaster.WorkOrderDetailGUID = objItemPullInfo.WorkOrderDetailGUID;

                objPullMaster.Created = DateTimeUtility.DateTimeNow;
                objPullMaster.CreatedBy = objItemPullInfo.CreatedBy;
                objPullMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objPullMaster.AddedFrom = "Web";
                objPullMaster.EditedFrom = "Web";
                objPullMaster.CountLineItemGuid = objItemPullInfo.CountLineItemGuid;
                objPullMaster.PullOrderNumber = objItemPullInfo.PullOrderNumber;
                context.PullMasters.AddObject(objPullMaster);
                context.SaveChanges();
                objItemPullInfo.PullGUID = objPullMaster.GUID;

                objItemPullInfo.lstItemPullDetails.ForEach(t =>
                {
                    string InventoryConsuptionMethod = string.Empty;
                    Room objRoomDTO = context.Rooms.FirstOrDefault(x => x.ID == objItemPullInfo.RoomId);
                    if (objRoomDTO != null && !string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod))
                    {
                        InventoryConsuptionMethod = objRoomDTO.InventoryConsuptionMethod;
                    }

                    if (string.IsNullOrEmpty(InventoryConsuptionMethod))
                        InventoryConsuptionMethod = "";

                    ItemLocationDetail objItemLocationDetail = null;
                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            objItemLocationDetail = context.ItemLocationDetails.OrderByDescending(x => x.ReceivedDate).FirstOrDefault(x => (
                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                            || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                            || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false);
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).OrderBy(x => x.Created).ToList();
                            break;
                        default:
                            objItemLocationDetail = context.ItemLocationDetails.OrderBy(x => x.ReceivedDate).FirstOrDefault(x => (
                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                            || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                            || (!t.LotNumberTracking && !t.SerialNumberTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false);
                            break;
                    }


                    if (lstItemLocations != null)
                    {
                        foreach (var item in lstItemLocations)
                        {
                            objItemLocationDetail = item;
                            if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) >= (t.ConsignedTobePulled + t.CustomerOwnedTobePulled))
                            {
                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = t.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;
                                objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = t.Cost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = null;
                                objPullDetail.PoolQuantity = t.CustomerOwnedTobePulled + t.ConsignedTobePulled;
                                objPullDetail.ProjectSpendGUID = objItemPullInfo.ProjectSpendGUID;
                                objPullDetail.PullCredit = PullCredit;
                                objPullDetail.PULLGUID = objItemPullInfo.PullGUID;

                                if (!string.IsNullOrWhiteSpace(objItemLocationDetail.Received))
                                    objPullDetail.Received = objItemLocationDetail.Received;
                                else if (objItemLocationDetail.ReceivedDate.HasValue)
                                    objPullDetail.Received = objItemLocationDetail.ReceivedDate.Value.ToString("MM/dd/yyyy");

                                objPullDetail.Room = objItemPullInfo.RoomId;
                                objPullDetail.SerialNumber = t.SerialNumberTracking ? t.LotOrSerailNumber : string.Empty;

                                if (objItem.DateCodeTracking && !string.IsNullOrWhiteSpace(objItemLocationDetail.Expiration))
                                    objPullDetail.Expiration = objItemLocationDetail.Expiration;
                                else if (objItem.DateCodeTracking && objItemLocationDetail.ExpirationDate.HasValue)
                                    objPullDetail.Expiration = objItemLocationDetail.ExpirationDate.Value.ToString("MM/dd/yyyy");

                                objPullDetail.Updated = DateTimeUtility.DateTimeNow;
                                objPullDetail.Created = DateTimeUtility.DateTimeNow;
                                objPullDetail.CreatedBy = objItemPullInfo.CreatedBy;
                                objPullDetail.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objPullDetail.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objPullDetail.AddedFrom = "Web";
                                objPullDetail.EditedFrom = "Web";
                                objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, objItem.SellPrice, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                objPullDetail.ItemCost = itemCost;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));

                                context.PullDetails.AddObject(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;

                            }
                        }
                    }
                });

                objPullMaster.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                objPullMaster.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;

                if (objItem.Consignment)
                {
                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, objItem.SupplierID ?? 0, objItem.GUID, null, false);
                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                    {
                        objPullMaster.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                    }
                    else
                    {
                        DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, 0);
                        objPullMaster.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                    }
                }

                if (objItemPullInfo.ProjectSpendGUID.HasValue && objItemPullInfo.ProjectSpendGUID != Guid.Empty)
                {
                    ProjectMaster objProjectMaster = context.ProjectMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ProjectSpendGUID);
                    if (objProjectMaster != null)
                    {
                        ProjectSpendItem objProjectSpendItem = context.ProjectSpendItems.FirstOrDefault(t => t.ProjectGUID == objItemPullInfo.ProjectSpendGUID && t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false);
                        if (objProjectSpendItem == null)
                        {
                            objProjectSpendItem = new ProjectSpendItem();
                            objProjectSpendItem.CompanyID = objItemPullInfo.CompanyId;
                            objProjectSpendItem.Created = DateTimeUtility.DateTimeNow;
                            objProjectSpendItem.CreatedBy = objItemPullInfo.CreatedBy;
                            objProjectSpendItem.DollarLimitAmount = 0;
                            objProjectSpendItem.DollarUsedAmount = Convert.ToDecimal(objPullMaster.PULLCost);
                            objProjectSpendItem.GUID = Guid.NewGuid();
                            objProjectSpendItem.IsArchived = false;
                            objProjectSpendItem.IsDeleted = false;
                            objProjectSpendItem.ItemGUID = objItemPullInfo.ItemGUID;
                            objProjectSpendItem.LastUpdated = DateTimeUtility.DateTimeNow;
                            objProjectSpendItem.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                            objProjectSpendItem.ProjectGUID = objItemPullInfo.ProjectSpendGUID;
                            objProjectSpendItem.QuantityLimit = 0;
                            objProjectSpendItem.QuantityUsed = objItemPullInfo.PullQuantity;
                            objProjectSpendItem.Room = objItemPullInfo.RoomId;
                            objProjectSpendItem.UDF1 = string.Empty;
                            objProjectSpendItem.UDF2 = string.Empty;
                            objProjectSpendItem.UDF3 = string.Empty;
                            objProjectSpendItem.UDF4 = string.Empty;
                            objProjectSpendItem.UDF5 = string.Empty;

                            objProjectSpendItem.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objProjectSpendItem.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objProjectSpendItem.AddedFrom = "Web";
                            objProjectSpendItem.EditedFrom = "Web";

                            context.ProjectSpendItems.AddObject(objProjectSpendItem);
                        }
                        else
                        {
                            objProjectSpendItem.DollarUsedAmount = objProjectSpendItem.DollarLimitAmount + Convert.ToDecimal(objPullMaster.PULLCost);
                            objProjectSpendItem.QuantityUsed = objProjectSpendItem.QuantityUsed + objItemPullInfo.PullQuantity;
                        }
                        objProjectMaster.DollarUsedAmount = objProjectMaster.DollarLimitAmount + Convert.ToDecimal(objPullMaster.PULLCost);
                    }
                }
                context.SaveChanges();

                ItemLocationQTY objItemLocationQTY = context.ItemLocationQTies.FirstOrDefault(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID);
                if (objItemLocationQTY == null)
                {
                    objItemLocationQTY = new ItemLocationQTY();
                    objItemLocationQTY.BinID = objItemPullInfo.BinID;
                    objItemLocationQTY.CompanyID = objItemPullInfo.CompanyId;
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Created = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.CreatedBy = objItemPullInfo.CreatedBy;
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.GUID = Guid.NewGuid();
                    objItemLocationQTY.ItemGUID = objItemPullInfo.ItemGUID;
                    objItemLocationQTY.LastUpdated = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                    objItemLocationQTY.Room = objItemPullInfo.RoomId;

                    objItemLocationQTY.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemLocationQTY.AddedFrom = "Web";
                    objItemLocationQTY.EditedFrom = "Web";

                    context.ItemLocationQTies.AddObject(objItemLocationQTY);
                }
                else
                {
                    objItemLocationQTY.CustomerOwnedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    objItemLocationQTY.ConsignedQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.ConsignedQuantity ?? 0));
                    objItemLocationQTY.Quantity = (objItemLocationQTY.ConsignedQuantity ?? 0) + (objItemLocationQTY.CustomerOwnedQuantity ?? 0);
                }
                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();

                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                CostDTO objCostDTO = objDashboardDAL.GetAvgExtendedCost(objItem.GUID);
                objItem.ExtendedCost = objCostDTO.ExtCost;
                objItem.AverageCost = objCostDTO.AvgCost;

                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);

                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "Pull >> Pull Quantity");
                objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, null, null);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, null, null);

                context.SaveChanges();

                //Get Cached-Media Create PullMaster Cache
                CreatePullMasterCache(objPullMaster);

                ItemMasterDTO oItemDTO = objItemMasterDAL.GetItemWithMasterTableJoins(objItem.ID, null, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0));
                CreateItemMasterCache(oItemDTO);

                if (objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objPullMaster.Room.GetValueOrDefault(0), objPullMaster.CompanyID.GetValueOrDefault(0));
                }

                return objItemPullInfo;
            }
        }

        public void CreateItemMasterCache(ItemMasterDTO oItemMasterDTO)
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
    }
}
