using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Globalization;
using eTurns.DTO.Resources;
using System.Data.SqlClient;
using System.Web;


namespace eTurns.DAL
{
    public class ItemLocationDetailsDAL : eTurnsBaseDAL
    {
        public IEnumerable<ItemLocationDetailsDTO> GetCachedData(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<ItemLocationDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.GetCacheItem("Cached_ItemLocationDetails_" + CompanyID.ToString());

            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var paramA = new SqlParameter[] { new SqlParameter("@CompanyID", @CompanyID) };
                    IEnumerable<ItemLocationDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>("exec [GetItemLocDtlWithSuggestedOrderQty] @CompanyID", paramA)
                                                               select new ItemLocationDetailsDTO
                                                               {
                                                                   ID = u.ID,
                                                                   BinID = u.BinID,
                                                                   CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                                   ConsignedQuantity = u.ConsignedQuantity,
                                                                   MeasurementID = u.MeasurementID,
                                                                   LotNumber = u.LotNumber,
                                                                   SerialNumber = u.SerialNumber,
                                                                   ExpirationDate = u.ExpirationDate,
                                                                   ReceivedDate = u.ReceivedDate,
                                                                   Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yy") : u.Expiration,
                                                                   Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                                                   Cost = u.Cost,
                                                                   eVMISensorPort = u.eVMISensorPort,
                                                                   eVMISensorID = u.eVMISensorID,
                                                                   GUID = u.GUID,
                                                                   ItemGUID = u.ItemGUID,
                                                                   Created = u.Created,
                                                                   Updated = u.Updated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                                   IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                                   CompanyID = u.CompanyID,
                                                                   Room = u.Room,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   BinNumber = u.BinNumber,
                                                                   ItemNumber = u.ItemNumber,
                                                                   SerialNumberTracking = u.SerialNumberTracking,
                                                                   LotNumberTracking = u.LotNumberTracking,
                                                                   DateCodeTracking = u.DateCodeTracking,
                                                                   OrderDetailGUID = u.OrderDetailGUID,
                                                                   TransferDetailGUID = u.TransferDetailGUID,
                                                                   KitDetailGUID = u.KitDetailGUID,
                                                                   CriticalQuantity = u.CriticalQuantity,
                                                                   MinimumQuantity = u.MinimumQuantity,
                                                                   MaximumQuantity = u.MaximumQuantity,
                                                                   SuggestedOrderQuantity = u.SuggestedOrderQuantity,//(new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                                   Markup = u.Markup,
                                                                   AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                                   EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                                   ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                                   ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemLocationDetailsDTO>>.AddCacheItem("Cached_ItemLocationDetails_" + CompanyID.ToString(), obj);
                }
            }

            //IEnumerable<ItemMasterDTO> ObjItemCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyID.ToString());
            //BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
            //IEnumerable<BinMasterDTO> ObjBinCache = objBinDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            //IEnumerable<ProjectMasterDTO> ObjProjectCache = objProjectDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IEnumerable<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(0, CompanyID);
            //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            //IEnumerable<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, IsArchived, IsDeleted);
            return ObjCache.Where(t => t.Room == RoomID && t.ItemGUID == ItemGUID);
        }

        public IEnumerable<ItemLocationDetailsDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(ItemGUID, RoomID, CompanyID);
            IEnumerable<ItemLocationDetailsDTO> ObjGlobalCache = null;

            if (OrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ItemGUID == ItemGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache.Where(t => t.OrderDetailGUID == OrderDetailGUID);
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.OrderDetailGUID == OrderDetailGUID));
            }

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
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public IEnumerable<ItemLocationDetailsDTO> GetCachedDataeVMI(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*,  I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                    E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber  
                    FROM ItemLocationDetails A 
                    Left outer join BinMaster I on A.BinID = I.ID AND A.ItemGUID = I.ItemGUID 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID 
                    WHERE A.CompanyID = " + CompanyID.ToString() + " and A.Room= " + RoomID.ToString() + " and A.ItemGUID='" + ItemGUID.ToString() + "' and (isnull(A.CustomerOwnedQuantity,0) + isnull(A.ConsignedQuantity,0)) != 0")
                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
                                    GUID = u.GUID,
                                    ItemGUID = u.ItemGUID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    BinNumber = u.BinNumber,
                                    ItemNumber = u.ItemNumber,
                                    SerialNumberTracking = u.SerialNumberTracking,
                                    LotNumberTracking = u.LotNumberTracking,
                                    DateCodeTracking = u.DateCodeTracking,
                                    OrderDetailGUID = u.OrderDetailGUID,
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            //IEnumerable<ItemMasterDTO> ObjItemCache = CacheHelper<IEnumerable<ItemMasterDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyID.ToString());
            //BinMasterDAL objBinDAL = new BinMasterDAL(base.DataBaseName);
            //IEnumerable<BinMasterDTO> ObjBinCache = objBinDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //ProjectMasterDAL objProjectDAL = new ProjectMasterDAL(base.DataBaseName);
            //IEnumerable<ProjectMasterDTO> ObjProjectCache = objProjectDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted);
            //UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            //IEnumerable<UserMasterDTO> ObjUserCache = objUserDAL.GetAllRecords(0, CompanyID);
            //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            //IEnumerable<RoomDTO> ObjRoomCache = objRoomDAL.GetCachedData(CompanyID, IsArchived, IsDeleted);
            return ObjCache;
        }

        public ItemLocationDetailsDTO GetRecord(Guid GUID, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*, I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,
                    D.RoomName, E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber 
                    FROM ItemLocationDetails A 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID
                    WHERE A.GUID = '" + GUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString())
                        select new ItemLocationDetailsDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            ReceivedDate = u.ReceivedDate,
                            Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM/dd/yy") : u.Expiration,
                            Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            RoomName = u.RoomName,
                            BinNumber = u.BinNumber,
                            ItemNumber = u.ItemNumber,
                            SerialNumberTracking = u.SerialNumberTracking,
                            LotNumberTracking = u.LotNumberTracking,
                            DateCodeTracking = u.DateCodeTracking,
                            OrderDetailGUID = u.OrderDetailGUID,
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            CriticalQuantity = u.CriticalQuantity,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                            ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                        }).SingleOrDefault();

            }
        }

        public ItemLocationDetailsDTO GetHistoryRecord(Int64 id)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemLocationDetails_History A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.HistoryID=" + id.ToString())
                        select new ItemLocationDetailsDTO
                        {
                            ID = u.ID,

                            BinID = u.BinID,
                            CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                            ConsignedQuantity = u.ConsignedQuantity,
                            MeasurementID = u.MeasurementID,
                            LotNumber = u.LotNumber,
                            SerialNumber = u.SerialNumber,
                            ExpirationDate = u.ExpirationDate,
                            Cost = u.Cost,
                            eVMISensorPort = u.eVMISensorPort,
                            eVMISensorID = u.eVMISensorID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            UDF6 = u.UDF6,
                            UDF7 = u.UDF7,
                            UDF8 = u.UDF8,
                            UDF9 = u.UDF9,
                            UDF10 = u.UDF10,
                            GUID = u.GUID,
                            ItemGUID = u.ItemGUID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            OrderDetailGUID = u.OrderDetailGUID,
                            TransferDetailGUID = u.TransferDetailGUID,
                            KitDetailGUID = u.KitDetailGUID,
                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                            ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                        }).SingleOrDefault();
            }
        }

        public bool ItemManufacturerDetailsSave(List<ItemManufacturerDetailsDTO> objData)
        {
            ItemManufacturerDetailsDAL objManuDAL = new ItemManufacturerDetailsDAL(base.DataBaseName);
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // first check ItemID to detail table whether exist or not           


                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(objData[0].ItemGUID.ToString(), out ItemGuid);
                var TempData = objManuDAL.GetManufacturerByItemGuidNormal((Int64)objData[0].Room, (Int64)objData[0].CompanyID, ItemGuid, false);
                // if exists then first delete 
                if (TempData != null && TempData.Count() > 0)
                {
                    string tempDeleteQuery = @"delete ItemManufacturerDetails where ItemGUID = '" + objData[0].ItemGUID + "' and Room = " + objData[0].Room + " and CompanyID = " + objData[0].CompanyID + "";
                    context.ExecuteStoreCommand(tempDeleteQuery);
                    // cache delete code 
                    IEnumerable<ItemManufacturerDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.GetCacheItem("Cached_ItemManufacturerDetails_" + objData[0].CompanyID);
                    if (ObjCache != null)
                    {
                        List<ItemManufacturerDetailsDTO> objTemp = ObjCache.ToList();
                        objTemp.RemoveAll(i => i.ItemGUID == objData[0].ItemGUID && i.Room == objData[0].Room && i.CompanyID == objData[0].CompanyID);
                        ObjCache = objTemp.AsEnumerable();
                        CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.AppendToCacheItem("Cached_ItemManufacturerDetails_" + objData[0].CompanyID.ToString(), ObjCache);
                    }
                }
                //and then re-insert the data.
                foreach (ItemManufacturerDetailsDTO item in objData)
                {


                    objManuDAL.Insert(item);
                    if ((bool)item.IsDefault)
                    {
                        ItemMasterDAL TempItemDAL = new ItemMasterDAL(base.DataBaseName);
                        // update this record to Item Master table ....
                        ItemMasterDTO TempItemDTO = TempItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                        if (TempItemDTO != null)
                        {
                            TempItemDTO.ManufacturerID = item.ManufacturerID;
                            TempItemDTO.ManufacturerNumber = item.ManufacturerNumber;
                            TempItemDTO.CreatedBy = item.CreatedBy;
                            TempItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            TempItemDAL.EditMultiple(TempItemDTO);
                        }
                    }
                }
            }
            return true;
        }

        public bool ItemSupplierDetailsSave(List<ItemSupplierDetailsDTO> objData)
        {
            ItemSupplierDetailsDAL objSuppDAL = new ItemSupplierDetailsDAL(base.DataBaseName);
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                // first check ItemID to detail table whether exist or not             
                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(objData[0].ItemGUID.ToString(), out ItemGuid);
                var TempData = objSuppDAL.GetSuppliersByItemGuidNormal((Int64)objData[0].Room, (Int64)objData[0].CompanyID, ItemGuid);
                // if exists then first delete 
                if (TempData != null && TempData.Count() > 0)
                {
                    string tempDeleteQuery = @"delete ItemSupplierDetails where ItemGUID = '" + objData[0].ItemGUID + "' and Room = " + objData[0].Room + " and CompanyID = " + objData[0].CompanyID + "";
                    context.ExecuteStoreCommand(tempDeleteQuery);
                    // cache delete code 
                    IEnumerable<ItemSupplierDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.GetCacheItem("Cached_ItemSupplierDetails_" + objData[0].CompanyID);
                    if (ObjCache != null)
                    {
                        List<ItemSupplierDetailsDTO> objTemp = ObjCache.ToList();
                        objTemp.RemoveAll(i => i.ItemGUID == objData[0].ItemGUID && i.Room == objData[0].Room && i.CompanyID == objData[0].CompanyID);
                        ObjCache = objTemp.AsEnumerable();
                        CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.AppendToCacheItem("Cached_ItemSupplierDetails_" + objData[0].CompanyID.ToString(), ObjCache);
                    }
                }
                //and then re-insert the data.
                foreach (ItemSupplierDetailsDTO item in objData)
                {
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.AddedFrom = "Web";
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objSuppDAL.Insert(item);
                    if ((bool)item.IsDefault)
                    {
                        ItemMasterDAL TempItemDAL = new ItemMasterDAL(base.DataBaseName);
                        // update this record to Item Master table ....
                        ItemMasterDTO TempItemDTO = TempItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                        if (TempItemDTO != null)
                        {
                            TempItemDTO.SupplierID = item.SupplierID;
                            TempItemDTO.SupplierPartNo = item.SupplierNumber;
                            TempItemDTO.CreatedBy = item.CreatedBy;
                            TempItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                            TempItemDAL.EditMultiple(TempItemDTO);
                        }
                    }
                }
            }
            return true;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetRecordsByIDs(string IDs, Int64 RoomID, Int64 CompanyID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            if (ObjCache == null || ObjCache.Count() <= 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*,  I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                    E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber ,E.ID As ItemID 
                    FROM ItemLocationDetails A 
                    Left outer join BinMaster I on A.BinID = I.ID AND A.ItemGUID = I.ItemGUID 
                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID 
                    WHERE IsNull(A.IsDeleted,0) =0 ANd ISNULL(A.IsArchived,0)=0 AND A.CompanyID = " + CompanyID.ToString() + " and A.Room= " + RoomID.ToString() + @" 
                    and A.ID In (SELECT SplitValue FROM dbo.split('" + IDs.ToString() + @"' ,',')) 
                    and (isnull(A.CustomerOwnedQuantity,0) + isnull(A.ConsignedQuantity,0)) != 0 
                    Order by ItemNumber,SerialNumber,LotNumber")

                                select new ItemLocationDetailsDTO
                                {
                                    ID = u.ID,
                                    BinID = u.BinID,
                                    CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                    ConsignedQuantity = u.ConsignedQuantity,
                                    MeasurementID = u.MeasurementID,
                                    LotNumber = u.LotNumber,
                                    SerialNumber = u.SerialNumber,
                                    ExpirationDate = u.ExpirationDate,
                                    ReceivedDate = u.ReceivedDate,
                                    Expiration = u.Expiration,
                                    Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yy") : u.Received,
                                    Cost = u.Cost,
                                    eVMISensorPort = u.eVMISensorPort,
                                    eVMISensorID = u.eVMISensorID,
                                    GUID = u.GUID,
                                    ItemGUID = u.ItemGUID,
                                    ItemID = u.ItemID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CompanyID = u.CompanyID,
                                    Room = u.Room,
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    RoomName = u.RoomName,
                                    BinNumber = u.BinNumber,
                                    ItemNumber = u.ItemNumber,
                                    SerialNumberTracking = u.SerialNumberTracking,
                                    LotNumberTracking = u.LotNumberTracking,
                                    DateCodeTracking = u.DateCodeTracking,
                                    OrderDetailGUID = u.OrderDetailGUID,
                                    TransferDetailGUID = u.TransferDetailGUID,
                                    KitDetailGUID = u.KitDetailGUID,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                    AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                    EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                    ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                    ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),

                                }).AsParallel().ToList();

                }
            }

            return ObjCache;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetCachedDataBinWise(Int64 BinID, Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            return GetCachedData(ItemGUID, RoomID, CompanyID).Where(t => t.BinID == BinID);
        }

        public IEnumerable<ItemLocationDetailsDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId, Guid ItemGUID, Guid? OrderDetailGUID, string ColumnName)
        {
            // var result = GetCachedData(RoomID, CompanyId).Where(t => (t.ItemID == ItemID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            IEnumerable<ItemLocationDetailsDTO> result = null;
            if (OrderDetailGUID == null || OrderDetailGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty)
                result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => ((t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);
            else
                result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(ColumnName);

            return result;
        }

        public IEnumerable<ItemLocationDetailsDTO> GetPagedRecords_NoCache(Int64 BinID, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ItemGUID, Guid? OrderDetailGUID)
        {
            //Get Cached-Media
            IEnumerable<ItemLocationDetailsDTO> ObjCache = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*,  I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,
                    E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber,E.Markup 
                    FROM ItemLocationDetails A 
                    LEFT OUTER JOIN UserMaster B on A.CreatedBy = B.ID 
                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                    LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID 
                    WHERE A.BinID = " + BinID.ToString() + " AND A.ITEMGUID = '" + ItemGUID.ToString() + "' AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + " AND (ISNULL(A.CustomerOwnedQuantity,0) <> 0 OR ISNULL(A.ConsignedQuantity,0) <> 0 )")
                            select new ItemLocationDetailsDTO
                            {
                                ID = u.ID,
                                BinID = u.BinID,
                                CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                ConsignedQuantity = u.ConsignedQuantity,
                                MeasurementID = u.MeasurementID,
                                LotNumber = u.LotNumber,
                                SerialNumber = u.SerialNumber,
                                ExpirationDate = u.ExpirationDate,
                                ReceivedDate = u.ReceivedDate,
                                Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM-dd-yyyy") : u.Expiration,
                                Received = u.Received == null ? DateTime.Now.ToString("MM-dd-yyyy") : u.Received,
                                Cost = u.Cost,
                                Markup = u.Markup,
                                SellPrice = u.SellPrice,
                                eVMISensorPort = u.eVMISensorPort,
                                eVMISensorID = u.eVMISensorID,
                                GUID = u.GUID,
                                ItemGUID = u.ItemGUID,
                                Created = u.Created,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                CompanyID = u.CompanyID,
                                Room = u.Room,
                                CreatedByName = u.CreatedByName,
                                UpdatedByName = u.UpdatedByName,
                                RoomName = u.RoomName,
                                BinNumber = u.BinNumber,
                                ItemNumber = u.ItemNumber,
                                SerialNumberTracking = u.SerialNumberTracking,
                                LotNumberTracking = u.LotNumberTracking,
                                DateCodeTracking = u.DateCodeTracking,
                                OrderDetailGUID = u.OrderDetailGUID,
                                TransferDetailGUID = u.TransferDetailGUID,
                                KitDetailGUID = u.KitDetailGUID,
                                CriticalQuantity = u.CriticalQuantity,
                                MinimumQuantity = u.MinimumQuantity,
                                MaximumQuantity = u.MaximumQuantity,
                                SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                            }).AsParallel().ToList();
            }




            IEnumerable<ItemLocationDetailsDTO> ObjGlobalCache = null;

            if (OrderDetailGUID == null)
            {
                ObjGlobalCache = ObjCache;
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.ItemGUID == ItemGUID));
            }
            else
            {
                ObjGlobalCache = ObjCache.Where(t => t.OrderDetailGUID == OrderDetailGUID);
                ObjCache = ObjCache.Where(t => (t.IsArchived == false && t.IsDeleted == false && t.OrderDetailGUID == OrderDetailGUID));
            }

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
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.Updated.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
                    && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                    && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                    && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                    && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                    && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                    );

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                //Get Cached-Media
                //IEnumerable<ItemLocationDetailsDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                TotalCount = ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).Count();
                return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public List<ItemLocationDetailsDTO> GetRecordsByBinNumberAndLotSerial(Guid ItemGuid, string BinNumber, string LotSerialNumber, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemLocationDetailsDTO> lstItemLocationDetailsDTO = new List<ItemLocationDetailsDTO>();

                lstItemLocationDetailsDTO = (from u in context.ExecuteStoreQuery<ItemLocationDetailsDTO>(@"SELECT A.*, I.CriticalQuantity,I.MinimumQuantity, I.MaximumQuantity, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName,
                                                                                                           D.RoomName, E.ItemNumber,E.Consignment,E.ItemType,E.SerialNumberTracking,E.LotNumberTracking,E.DateCodeTracking, I.BinNumber 
                                                                                                           FROM ItemLocationDetails A 
                                                                                                           Inner Join BinMaster BM on BM.ID = A.BinID
                                                                                                           LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                                                                                           LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID 
                                                                                                           LEFT OUTER JOIN ItemMaster E on A.ItemGUID = E.GUID LEFT OUTER JOIN BinMaster I on A.BinID = I.ID  AND A.ItemGUID = I.ItemGUID
                                                                                                           WHERE A.ItemGUID = '" + ItemGuid.ToString() + @"' AND BM.BinNumber = '" + BinNumber + @"' 
                                                                                                           	     AND (ISNULL(A.LotNumber,'') = '" + LotSerialNumber + @"' OR ISNULL(A.SerialNumber,'') = '" + LotSerialNumber + @"')
                                                                                                                 AND A.Room = " + RoomID.ToString() + " AND A.CompanyID = " + CompanyID.ToString() + "")
                                             select new ItemLocationDetailsDTO
                                             {
                                                 ID = u.ID,

                                                 BinID = u.BinID,
                                                 CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                                                 ConsignedQuantity = u.ConsignedQuantity,
                                                 MeasurementID = u.MeasurementID,
                                                 LotNumber = u.LotNumber,
                                                 SerialNumber = u.SerialNumber,
                                                 ExpirationDate = u.ExpirationDate,
                                                 ReceivedDate = u.ReceivedDate,
                                                 Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                 Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                 Cost = u.Cost,
                                                 eVMISensorPort = u.eVMISensorPort,
                                                 eVMISensorID = u.eVMISensorID,
                                                 GUID = u.GUID,
                                                 ItemGUID = u.ItemGUID,
                                                 Created = u.Created,
                                                 Updated = u.Updated,
                                                 CreatedBy = u.CreatedBy,
                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                 IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                 IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                 CompanyID = u.CompanyID,
                                                 Room = u.Room,
                                                 CreatedByName = u.CreatedByName,
                                                 UpdatedByName = u.UpdatedByName,
                                                 RoomName = u.RoomName,
                                                 BinNumber = u.BinNumber,
                                                 ItemNumber = u.ItemNumber,
                                                 SerialNumberTracking = u.SerialNumberTracking,
                                                 LotNumberTracking = u.LotNumberTracking,
                                                 DateCodeTracking = u.DateCodeTracking,
                                                 OrderDetailGUID = u.OrderDetailGUID,
                                                 TransferDetailGUID = u.TransferDetailGUID,
                                                 KitDetailGUID = u.KitDetailGUID,
                                                 CriticalQuantity = u.CriticalQuantity,
                                                 MinimumQuantity = u.MinimumQuantity,
                                                 MaximumQuantity = u.MaximumQuantity,
                                                 SuggestedOrderQuantity = (new CartItemDAL(base.DataBaseName)).GetSuggestedOrderQtyForBin(u.ItemGUID.GetValueOrDefault(Guid.Empty), u.BinID.GetValueOrDefault(0)),
                                                 AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                 EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                 ReceivedOn = u.ReceivedOn.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                                 ReceivedOnWeb = u.ReceivedOnWeb.GetValueOrDefault(DateTimeUtility.DateTimeNow),
                                             }).ToList();

                return lstItemLocationDetailsDTO;
            }
        }

        public bool InsertItemLocationDetailsFromRecieve(List<ItemLocationDetailsDTO> objData, long SessionUserId)
        {
            ReceivedOrderTransferDetailDAL objROTDDAL = new ReceivedOrderTransferDetailDAL(base.DataBaseName);
            IEnumerable<ReceivedOrderTransferDetailDTO> lst;
            OrderDetailsDTO OrdDetailDTO = null;
            OrderDetailsDAL ordDetailDAL = null;

            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemLocationQTYDAL objItemQtyDAL = null;
                #region "Location Detail Save"

                foreach (ItemLocationDetailsDTO item in objData)
                {
                    if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        item.IsConsignedSerialLot = true;

                    item.InitialQuantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0);
                    item.InitialQuantityWeb = item.InitialQuantity;

                    if (string.IsNullOrEmpty(item.AddedFrom))
                        item.AddedFrom = "UnKnown";

                    if (string.IsNullOrEmpty(item.EditedFrom))
                        item.EditedFrom = "UnKnown";

                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (string.IsNullOrEmpty(item.Action))
                        item.Action = "Receive";

                    Insert(item);

                    #region "Insert QTY logic"
                    var objilq = (from x in context.ExecuteStoreQuery<ItemLocationQTYDTO>(@"SELECT BinID,ItemGuid,Room,CompanyID,Sum(ISNULL(CustomerOwnedQuantity,0)) AS CustomerOwnedQuantity
                                                                                                                            ,SUM(ISNULL(ConsignedQuantity,0)) AS ConsignedQuantity
                                                                                                                            ,Sum(ISNULL(ConsignedQuantity,0) + ISNULL(CustomerOwnedQuantity,0)) As Quantity
                                                                                                                            FROM ItemLocationDetails WHERE itemguid = '" + item.ItemGUID.GetValueOrDefault(Guid.Empty).ToString() + @"'
                                                                                                                            and ISNULL(IsDeleted,0)=0  and ISNULL(IsArchived,0) =0
                                                                                                                            Group By BinID,ItemGUID,Room,CompanyID")
                                  select new ItemLocationQTYDTO
                                  {
                                      BinID = x.BinID,
                                      ItemGUID = x.ItemGUID,
                                      ConsignedQuantity = x.ConsignedQuantity,
                                      CustomerOwnedQuantity = x.CustomerOwnedQuantity,
                                      Quantity = x.Quantity,
                                      Room = x.Room,
                                      CompanyID = x.CompanyID,
                                      LastUpdated = DateTime.Now,
                                      Created = DateTime.Now,
                                      CreatedBy = objData[0].CreatedBy,
                                      LastUpdatedBy = objData[0].LastUpdatedBy,
                                  }).ToList();



                    objItemQtyDAL = new ItemLocationQTYDAL(base.DataBaseName);
                    objItemQtyDAL.Save(objilq, SessionUserId);

                    #endregion


                    ordDetailDAL = new OrderDetailsDAL(base.DataBaseName);
                    OrdDetailDTO = ordDetailDAL.GetOrderDetailByGuidFull(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));

                    lst = objROTDDAL.GetROTDByOrderDetailGUIDPlain(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0)).OrderByDescending(x => x.ID).ToList();
                    double rcvQty = lst.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    rcvQty += lst.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));

                    OrdDetailDTO.ReceivedQuantity = rcvQty;
                    OrdDetailDTO.IsOnlyFromUI = true;
                    OrdDetailDTO.EditedFrom = "Web";
                    OrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ordDetailDAL.Edit(OrdDetailDTO, SessionUserId);


                }

                #endregion


            }
            return true;
        }
    }
}
