using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;
using System.Data;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DAL;
using System.Data.SqlClient;
using eTurns.DTO.Resources;
using System.Web;

namespace eTurns.DAL
{
    public partial class BOMItemMasterDAL : eTurnsBaseDAL
    {

        public IEnumerable<ItemManufacturerDetailsDTO> GetCachedDataManuBOM(Int64 RoomID, Int64 CompanyID) 
        {
            //Get Cached-Media
            IEnumerable<ItemManufacturerDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.GetCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemManufacturerDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemManufacturerDetailsDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM ItemManufacturerDetails A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                                   select new ItemManufacturerDetailsDTO
                                                                   {
                                                                       ID = u.ID,

                                                                       ManufacturerID = u.ManufacturerID,
                                                                       Created = u.Created,
                                                                       Updated = u.Updated,
                                                                       CreatedBy = u.CreatedBy,
                                                                       LastUpdatedBy = u.LastUpdatedBy,
                                                                       IsDeleted = u.IsDeleted,
                                                                       IsArchived = u.IsArchived,
                                                                       IsDefault = u.IsDefault,
                                                                       ManufacturerName = u.ManufacturerName,
                                                                       ManufacturerNumber = u.ManufacturerNumber,
                                                                       Room = u.Room,
                                                                       CompanyID = u.CompanyID,
                                                                       CreatedByName = u.CreatedByName,
                                                                       UpdatedByName = u.UpdatedByName,
                                                                       RoomName = u.RoomName,
                                                                       ItemGUID = u.ItemGUID,
                                                                       GUID = u.GUID,
                                                                   }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.AddCacheItem("Cached_ItemManufacturerDetails_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == null);
        }

        public IEnumerable<ItemManufacturerDetailsDTO> GetAllRecordsManuBOM(Int64 RoomID, Int64 CompanyId) 
        {
            return GetCachedDataManuBOM(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<ItemSupplierDetailsDTO> GetAllRecordsSuppBOM(Int64 RoomID, Int64 CompanyId)
        {
            return GetCachedDataSuppBOM(RoomID, CompanyId).OrderBy("ID DESC");
        }

        public IEnumerable<ItemSupplierDetailsDTO> GetCachedDataSuppBOM(Int64 RoomID, Int64 CompanyID) 
        {
            //Get Cached-Media
            IEnumerable<ItemSupplierDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.GetCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString());
            if (ObjCache == null)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<ItemSupplierDetailsDTO> obj = (from u in context.ExecuteStoreQuery<ItemSupplierDetailsDTO>(@"
                SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName 
                FROM ItemSupplierDetails A 
                LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                LEFT OUTER JOIN Room D on A.Room = D.ID 
                WHERE A.IsDeleted != 1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                               select new ItemSupplierDetailsDTO
                                                               {
                                                                   ID = u.ID,
                                                                   ItemGUID = u.ItemGUID,
                                                                   SupplierID = u.SupplierID,
                                                                   Created = u.Created,
                                                                   Updated = u.Updated,
                                                                   CreatedBy = u.CreatedBy,
                                                                   LastUpdatedBy = u.LastUpdatedBy,
                                                                   IsDeleted = u.IsDeleted,
                                                                   IsArchived = u.IsArchived,
                                                                   IsDefault = u.IsDefault,
                                                                   BlanketPOID = u.BlanketPOID,
                                                                   SupplierName = u.SupplierName,
                                                                   SupplierNumber = u.SupplierNumber,
                                                                   Room = u.Room,
                                                                   CompanyID = u.CompanyID,
                                                                   CreatedByName = u.CreatedByName,
                                                                   UpdatedByName = u.UpdatedByName,
                                                                   RoomName = u.RoomName,
                                                                   GUID = u.GUID,
                                                               }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.AddCacheItem("Cached_ItemSupplierDetails_" + CompanyID.ToString(), obj);
                }
            }

            return ObjCache.Where(t => t.Room == null);
        }

        public IEnumerable<BOMItemDTO> GetCachedData(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<BOMItemDTO> ObjCache;
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            if (IsArchived == false && IsDeleted == false)
            {
                #region "Both False"
                ObjCache = GetCachedData(RoomID, CompanyID);
                ObjCache = ObjCache.Where(t => t.IsDeleted == false && t.IsArchived == false);
                #endregion
            }
            else
            {

                #region "Conditional"
                string sSQL = "";
                if (IsArchived && IsDeleted)
                {
                    sSQL += "A.IsDeleted = 1 AND A.IsArchived = 1";
                }
                else if (IsArchived)
                {
                    sSQL += "A.IsArchived = 1";
                }
                else if (IsDeleted)
                {
                    sSQL += "A.IsDeleted =1";
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ObjCache = (from u in context.ExecuteStoreQuery<BOMItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', 
                                    C.UserName AS UpdatedByName, D.RoomName,M.Manufacturer as ManufacturerName,S.SupplierName, 
                                    C1.Category AS CategoryName,U1.Unit,G1.GLAccount, B1.BinNumber as DefaultLocationName,I1.InventoryClassification  As InventoryClassificationName  
                                    FROM ItemMaster A 
                                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                                    LEFT OUTER join ManufacturerMaster M on m.id = A.ManufacturerID 
                                    left outer join SupplierMaster S on S.id = A.SupplierID 
                                    LEFT OUTER join CategoryMaster C1 on C1.id = A.CategoryID
                                    LEFT OUTER join UnitMaster U1 on U1.id = A.UOMID
                                    LEFT OUTER join GLAccountMaster G1 on G1.id = A.GLAccountID
                                    LEFT OUTER join BinMaster B1 on B1.id = A.DefaultLocation
                                    Left Outer join InventoryClassificationMaster I1 On A.InventoryClassification  = I1.ID 
                                                WHERE A.CompanyID = " + CompanyID.ToString() + " AND " + sSQL + " ORDER by ID DESC")
                                select new BOMItemDTO
                                {
                                    ID = u.ID,
                                    ItemNumber = u.ItemNumber,
                                    ManufacturerID = u.ManufacturerID,
                                    ManufacturerNumber = u.ManufacturerNumber,
                                    ManufacturerName = u.ManufacturerName,
                                    SupplierID = u.SupplierID,
                                    SupplierPartNo = u.SupplierPartNo,
                                    SupplierName = u.SupplierName,
                                    UPC = u.UPC,
                                    UNSPSC = u.UNSPSC,
                                    Description = u.Description,
                                    LongDescription = u.LongDescription,
                                    CategoryID = u.CategoryID,
                                    GLAccountID = u.GLAccountID,
                                    UOMID = u.UOMID,

                                    PricePerTerm = u.PricePerTerm,
                                    CostUOMID = u.CostUOMID,
                                    DefaultReorderQuantity = u.DefaultReorderQuantity,
                                    DefaultPullQuantity = u.DefaultPullQuantity,
                                    Cost = u.Cost,
                                    Markup = u.Markup,
                                    SellPrice = u.SellPrice,
                                    ExtendedCost = u.ExtendedCost,
                                    AverageCost = u.AverageCost,



                                    LeadTimeInDays = u.LeadTimeInDays,
                                    Link1 = u.Link1,
                                    Link2 = u.Link2,
                                    Trend = u.Trend,
                                    Taxable = u.Taxable,
                                    Consignment = u.Consignment,
                                    StagedQuantity = u.StagedQuantity,

                                    InTransitquantity = u.InTransitquantity,
                                    OnOrderQuantity = u.OnOrderQuantity,
                                    OnReturnQuantity = u.OnReturnQuantity,
                                    OnTransferQuantity = u.OnTransferQuantity,
                                    SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                    RequisitionedQuantity = u.RequisitionedQuantity,

                                    PackingQuantity = u.PackingQuantity,
                                    AverageUsage = u.AverageUsage,
                                    Turns = u.Turns,
                                    OnHandQuantity = u.OnHandQuantity,
                                    CriticalQuantity = u.CriticalQuantity,
                                    MinimumQuantity = u.MinimumQuantity,
                                    MaximumQuantity = u.MaximumQuantity,
                                    WeightPerPiece = u.WeightPerPiece,

                                    ItemUniqueNumber = u.ItemUniqueNumber,
                                    //TransferOrPurchase = u.TransferOrPurchase,
                                    IsPurchase = u.IsPurchase,
                                    IsTransfer = u.IsTransfer,
                                    DefaultLocation = u.DefaultLocation,
                                    DefaultLocationName = u.DefaultLocationName,
                                    InventoryClassification = u.InventoryClassification,
                                    SerialNumberTracking = u.SerialNumberTracking,
                                    LotNumberTracking = u.LotNumberTracking,
                                    DateCodeTracking = u.DateCodeTracking,
                                    ItemType = u.ItemType,
                                    ImagePath = u.ImagePath,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    GUID = u.GUID,
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
                                    IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                    CategoryName = u.CategoryName,
                                    Unit = u.Unit,
                                    GLAccount = u.GLAccount,
                                    ItemTypeName = u.ItemTypeName,
                                    IsItemLevelMinMaxQtyRequired = (u.IsItemLevelMinMaxQtyRequired.HasValue ? u.IsItemLevelMinMaxQtyRequired : false),
                                    IsBuildBreak = u.IsBuildBreak,
                                    BondedInventory = u.BondedInventory,
                                    IsEnforceDefaultReorderQuantity = (u.IsEnforceDefaultReorderQuantity.HasValue ? u.IsEnforceDefaultReorderQuantity : false),
                                    ItemLocations = objLocationDAL.GetAllRecords(RoomID, CompanyID, u.GUID, null, "ID ASC").ToList(),

                                    InventoryClassificationName = u.InventoryClassificationName,
                                    ImageType = u.ImageType,
                                    ItemLink2ImageType = u.ItemLink2ImageType,
                                    ItemImageExternalURL = u.ItemImageExternalURL,
                                    ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                    ItemDocExternalURL = u.ItemDocExternalURL,
                                    IsActive = u.IsActive,
                                }).AsParallel().ToList();
                    //ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("Cached_ItemMaster_" + CompanyID.ToString(), obj);
                }
                #endregion
            }
            return ObjCache.Where(t => t.Room == null);
        }

        public IEnumerable<BOMItemDTO> GetCachedData(Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<BOMItemDTO> ObjCache;
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            #region "Both False"
            ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("Cached_BOMItemMaster_" + CompanyID.ToString());
            //objReturn = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("Cached_ItemMaster_" + CompanyID.ToString());
            if (ObjCache == null || ObjCache.Count() <= 0)
            {

                IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<BOMItemDTO> obj = (from u in context.ExecuteStoreQuery<BOMItemDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', 
                                    C.UserName AS UpdatedByName, D.RoomName,M.Manufacturer as ManufacturerName,S.SupplierName, 
                                    C1.Category AS CategoryName,U1.Unit,G1.GLAccount , 
                                    B1.BinNumber as DefaultLocationName, I1.InventoryClassification As InventoryClassificationName 
                                    FROM ItemMaster A 
                                    LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID 
                                    LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID 
                                    LEFT OUTER JOIN Room D on A.Room = D.ID 
                                    LEFT OUTER join ManufacturerMaster M on M.id = A.ManufacturerID 
                                    left outer join SupplierMaster S on S.id = A.SupplierID 
                                    LEFT OUTER join CategoryMaster C1 on C1.id = A.CategoryID
                                    LEFT OUTER join UnitMaster U1 on U1.id = A.UOMID
                                    LEFT OUTER join GLAccountMaster G1 on G1.id = A.GLAccountID
                                    LEFT OUTER join BinMaster B1 on B1.id = A.DefaultLocation
                                    Left Outer join InventoryClassificationMaster I1 On A.InventoryClassification  = I1.ID 
                                    WHERE A.CompanyID = " + CompanyID.ToString() + " and A.IsBOMItem=1 ORDER BY ID DESC")
                                                   select new BOMItemDTO
                                                   {
                                                       ID = u.ID,
                                                       ItemNumber = u.ItemNumber,
                                                       ManufacturerID = u.ManufacturerID,
                                                       ManufacturerNumber = u.ManufacturerNumber,
                                                       ManufacturerName = u.ManufacturerName,
                                                       SupplierID = u.SupplierID,
                                                       SupplierPartNo = u.SupplierPartNo,
                                                       SupplierName = u.SupplierName,
                                                       UPC = u.UPC,
                                                       UNSPSC = u.UNSPSC,
                                                       Description = u.Description,
                                                       LongDescription = u.LongDescription,
                                                       CategoryID = u.CategoryID,
                                                       GLAccountID = u.GLAccountID,
                                                       UOMID = u.UOMID,

                                                       PricePerTerm = u.PricePerTerm,
                                                       CostUOMID = u.CostUOMID,
                                                       DefaultReorderQuantity = u.DefaultReorderQuantity,
                                                       DefaultPullQuantity = u.DefaultPullQuantity,
                                                       Cost = u.Cost,
                                                       Markup = u.Markup,
                                                       SellPrice = u.SellPrice,
                                                       ExtendedCost = u.ExtendedCost,
                                                       AverageCost = u.AverageCost,

                                                       LeadTimeInDays = u.LeadTimeInDays,
                                                       Link1 = u.Link1,
                                                       Link2 = u.Link2,
                                                       Trend = u.Trend,
                                                       Taxable = u.Taxable,
                                                       Consignment = u.Consignment,
                                                       StagedQuantity = u.StagedQuantity,
                                                       InTransitquantity = u.InTransitquantity,
                                                       OnOrderQuantity = u.OnOrderQuantity,
                                                       OnReturnQuantity = u.OnReturnQuantity,
                                                       OnTransferQuantity = u.OnTransferQuantity,
                                                       SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                                                       RequisitionedQuantity = u.RequisitionedQuantity,
                                                       PackingQuantity = u.PackingQuantity,
                                                       AverageUsage = u.AverageUsage,
                                                       Turns = u.Turns,
                                                       OnHandQuantity = u.OnHandQuantity,
                                                       CriticalQuantity = u.CriticalQuantity,
                                                       MinimumQuantity = u.MinimumQuantity,
                                                       MaximumQuantity = u.MaximumQuantity,
                                                       WeightPerPiece = u.WeightPerPiece,




                                                       ItemUniqueNumber = u.ItemUniqueNumber,
                                                       //TransferOrPurchase = u.TransferOrPurchase,
                                                       IsPurchase = u.IsPurchase,
                                                       IsTransfer = u.IsTransfer,
                                                       DefaultLocation = u.DefaultLocation,
                                                       DefaultLocationName = u.DefaultLocationName,
                                                       InventoryClassification = u.InventoryClassification,
                                                       SerialNumberTracking = u.SerialNumberTracking,
                                                       LotNumberTracking = u.LotNumberTracking,
                                                       DateCodeTracking = u.DateCodeTracking,
                                                       ItemType = u.ItemType,
                                                       ImagePath = u.ImagePath,
                                                       UDF1 = u.UDF1,
                                                       UDF2 = u.UDF2,
                                                       UDF3 = u.UDF3,
                                                       UDF4 = u.UDF4,
                                                       UDF5 = u.UDF5,
                                                       GUID = u.GUID,
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
                                                       IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                                                       ItemTypeName = u.ItemTypeName,
                                                       CategoryName = u.CategoryName,
                                                       Unit = u.Unit,
                                                       GLAccount = u.GLAccount,

                                                       InventoryClassificationName = u.InventoryClassificationName,
                                                       AddedFrom = u.AddedFrom,
                                                       EditedFrom = u.EditedFrom,
                                                       ImageType = u.ImageType,
                                                       ItemLink2ImageType = u.ItemLink2ImageType,
                                                       ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                                                       ItemImageExternalURL = u.ItemImageExternalURL,
                                                       ItemDocExternalURL = u.ItemDocExternalURL,
                                                       IsActive = u.IsActive,
                                                   }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("Cached_BOMItemMaster_" + CompanyID.ToString(), obj);
                    //return objReturn;
                }
            }
            #endregion
            return ObjCache;
        }

        public List<BOMItemDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<BOMItemDTO> lstSuppliers = GetAllItems();
            if (String.IsNullOrWhiteSpace(SearchTerm))
            {
                //Get Cached-Media
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                lstSuppliers = lstSuppliers.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.SupplierName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.ManufacturerName))));
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();
            }
            else
            {
                lstSuppliers = lstSuppliers.Where(
                    t => t.ID.ToString().Contains(SearchTerm) ||
                    t.ItemNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.Description.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    Convert.ToString(t.SellPrice).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    Convert.ToString(t.PackingQantity).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.UPC.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.SupplierName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.SupplierPartNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.ManufacturerPartNumber.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    t.ManufacturerName.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    );
                TotalCount = lstSuppliers.Count();
                return lstSuppliers.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows).ToList();

            }

        }

        public IEnumerable<BOMItemDTO> GetAllItems()
        {
            bool IsLatest = false;
            IEnumerable<BOMItemDTO> ObjCache = null;

            using (var dbcntx = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IsLatest = dbcntx.SupplierCatalogTrackers.First().IsLatest;

                if (IsLatest)
                {
                    ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.GetCacheItem("SupplierCatalog");
                    if (ObjCache == null)
                    {

                        IEnumerable<BOMItemDTO> obj = (from scm in dbcntx.SupplierCatalogs
                                                       select new BOMItemDTO
                                                       {
                                                           ID = scm.SupplierCatalogItemID,
                                                           ItemNumber = scm.ItemNumber,
                                                           Description = scm.Description,
                                                           UPC = scm.UPC,
                                                           SellPrice = scm.SellPrice,
                                                           ImagePath = scm.ImagePath,
                                                           SupplierName = scm.SupplierName,
                                                           SupplierPartNumber = scm.SupplierPartNumber,
                                                           ManufacturerName = scm.ManufacturerName,
                                                           ManufacturerPartNumber = scm.ManufacturerPartNumber,
                                                           PackingQantity = scm.PackingQuantity
                                                       }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("SupplierCatalog", obj);
                        dbcntx.SupplierCatalogTrackers.First().IsLatest = true;
                        dbcntx.SaveChanges();
                    }
                }
                else
                {

                    IEnumerable<BOMItemDTO> obj = (from scm in dbcntx.SupplierCatalogs
                                                   select new BOMItemDTO
                                                   {
                                                       ID = scm.SupplierCatalogItemID,
                                                       ItemNumber = scm.ItemNumber,
                                                       Description = scm.Description,
                                                       UPC = scm.UPC,
                                                       SellPrice = scm.SellPrice,
                                                       ImagePath = scm.ImagePath,
                                                       SupplierName = scm.SupplierName,
                                                       SupplierPartNumber = scm.SupplierPartNumber,
                                                       ManufacturerName = scm.ManufacturerName,
                                                       ManufacturerPartNumber = scm.ManufacturerPartNumber,
                                                       PackingQantity = scm.PackingQuantity
                                                   }).AsParallel().ToList();
                    ObjCache = CacheHelper<IEnumerable<BOMItemDTO>>.AddCacheItem("SupplierCatalog", obj);
                    dbcntx.SupplierCatalogTrackers.First().IsLatest = true;
                    dbcntx.SaveChanges();
                }
            }
            return ObjCache;
        }
        

        public BOMItemDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            //return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).Where(t => t.GUID == Guid.Parse(GUID)).SingleOrDefault();
            return GetAllBOMItems(CompanyID).Where(t => t.GUID == Guid.Parse(GUID)).SingleOrDefault();
        }

        public BOMItemDTO GetRecordByGUID(string GUID, Int64 CompanyID)
        {
            return GetAllBOMItems(CompanyID).Where(t => t.GUID == Guid.Parse(GUID)).SingleOrDefault();
        }


    }
}
