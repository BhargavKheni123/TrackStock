using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurns.DTO.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using System.Web;

namespace eTurns.DAL
{
    public partial class ItemMasterDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public ItemMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public IEnumerable<ItemMasterDTO> GetAllRecordsOnlyImages()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("EXEC [GetAllItemsImageDetails] ").ToList();
            }
        }

        //public ItemMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [GetRecordMethods]

        public Guid? GetItemGuIDOnlyByItemNumber(string ItemNumber, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objitem = (from im in context.ItemMasters
                                      where (im.IsDeleted ?? false) == false && (im.IsArchived ?? false) == false && im.Room == RoomID && im.ItemNumber == ItemNumber
                                      select im).FirstOrDefault();
                if (objitem != null)
                {
                    return objitem.GUID;
                }
                return null;
            }
        }

        public ItemMasterDTO GetItemWithMasterTableJoins(long? ID, Guid? ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID ?? (object)DBNull.Value), new SqlParameter("@GUID", ItemGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemByID] @ID,@GUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public int checkItemInUsed(Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@GUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("exec [checkIsItemInUsed] @GUID", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetItemWithoutJoins(long? ID, Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ID", ID ?? (object)DBNull.Value), new SqlParameter("@GUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetPlainItemByID] @ID,@GUID", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetRecordByItemNumber(string ItemNumber, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByNameWithInfo @ItemNumber,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public ItemMasterDTO GetItemByItemNumberPlain(string ItemNumber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByItemNumberPlain @ItemNumber,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public ItemMasterDTO GetItemByItemNumberNormal(string ItemNumber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemNumber", ItemNumber ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByItemNumberNormal @ItemNumber,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetRecordByItemGUID(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGUIDWithInfo @ItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public bool UpdateItemAsPerIDList(string ItemIds, Int64 RoomID, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //var params1 = new SqlParameter[] { new SqlParameter("@ItemIds", ItemIds ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                context.Database.ExecuteSqlCommand("exec UpdateItemAsPerIDList '" + ItemIds + "'," + CompanyID + "," + RoomID + "");
                return true;
            }
        }

        public ItemMasterDTO GetItemBySupplierPartNumberPlain(string ItemNumber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SupplierPartNo", ItemNumber ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemBySupplierPartNumberPlain @SupplierPartNo,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetItemBySupplierPartNumberNormal(string ItemNumber, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@SupplierPartNo", ItemNumber ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemBySupplierPartNumberNormal @SupplierPartNo,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetRecordByUniqueNo(string UniqueNo, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@UniqueNo", UniqueNo ?? string.Empty), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByUPCWithInfo @UniqueNo,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public List<BinMasterDTO> GetItemLocationsByItemNumberList(List<string> ItemNumber, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<BinMasterDTO> lstItemLocation = (from BM in context.BinMasters
                                                      join I in context.ItemMasters on BM.ItemGUID equals I.GUID
                                                      where ItemNumber.Contains(I.ItemNumber)
                                                            && (I.IsDeleted == null || I.IsDeleted == false) && (I.IsArchived == null || I.IsArchived == false)
                                                            && BM.IsDeleted == false && (BM.IsArchived == null || BM.IsArchived == false)
                                                            && I.Room == RoomId && I.CompanyID == CompanyId
                                                      select new BinMasterDTO()
                                                      {
                                                          ItemNumber = I.ItemNumber,
                                                          BinNumber = BM.BinNumber,
                                                          IsDeleted = BM.IsDeleted,
                                                          IsDefault = (BM.IsDefault == null ? false : BM.IsDefault.Value)
                                                      }).ToList();
                return lstItemLocation;
            }
        }

        public int GetItemCountFromRoom(long RoomID, long CompanyID, List<long> UserSupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@SupplierIds", (UserSupplierIds != null && UserSupplierIds.Any()) ? string.Join(",",UserSupplierIds) : (object) DBNull.Value )
                                                };
                return context.Database.SqlQuery<int>("exec GetItemCountRoomWise @RoomID,@CompanyID,@SupplierIds", params1).FirstOrDefault();
            }
        }

        #endregion

        #region [GetAllRecordsMethod]

        public List<ItemMasterDTO> GetAllItemsWithJoins(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived, string ItemTypes)
        {

            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@ItemTypes", (ItemTypes ?? string.Empty)) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetAllItemsWithJoins @RoomID,@CompanyID,@IsArchived,@IsDeleted,@ItemTypes", params1).ToList();
            }
        }

        public List<ItemMasterDTO> GetAllItemsWithoutJoins(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived, string ItemTypes)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@ItemTypes", (ItemTypes ?? string.Empty)) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetAllItemsWithoutJoins @RoomID,@CompanyID,@IsArchived,@IsDeleted,@ItemTypes", params1).ToList();
            }
        }

        public List<ItemMasterDTO> GetAllItemsPlainForTransfer(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                             };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetAllItemsPlainForTransfer] @RoomID,@CompanyID,@IsArchived,@IsDeleted", params1).ToList();
            }
        }

        #endregion

        #region [Class Methods]
        public IEnumerable<ItemMasterDTO> GetAllRecords(Int64 RoomID, Int64 CompanyId)
        {
            return GetAllItemsWithJoins(RoomID, CompanyId, false, false, null).OrderBy("ID DESC");
        }

        public IEnumerable<ItemMasterDTO> GetAllItemsPlain(long RoomId, long CompanyId)
        {
            return GetAllItemsWithoutJoins(RoomId, CompanyId, false, false, null).OrderBy("ID DESC");
        }

        public List<ItemMasterDTO> GetAllRecordsForImport(bool IsBuildBreak, Int32 ItemType, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Int64 SupplierID = 0)
        {
            List<ItemMasterDTO> ObjCache;
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);

            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(base.DataBaseName);
            IEnumerable<BarcodeMasterDTO> lstBarcodeDTO = objBarcodeMasterDAL.GetAllRecords(RoomID, CompanyID);
            Int64? MonthlyAverageUsage = 30;

            #region "Conditional"
            if (SupplierID > 0)
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                    {
                        MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                    }
                    if (MonthlyAverageUsage == null)
                    {
                        MonthlyAverageUsage = 30;
                    }
                    ObjCache = (from im in context.ItemMasters
                                join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                                from im_sm in im_sm_join.DefaultIfEmpty()

                                join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
                                from im_mm in im_mm_join.DefaultIfEmpty()

                                join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
                                from im_rm in im_rm_join.DefaultIfEmpty()

                                join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
                                from im_cuomm in im_cuomm_join.DefaultIfEmpty()

                                join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
                                from im_UMC in im_UMC_join.DefaultIfEmpty()

                                join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
                                from im_UMU in im_UMU_join.DefaultIfEmpty()

                                join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
                                from im_CM in im_CM_join.DefaultIfEmpty()

                                join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
                                from im_UNM in im_UNM_join.DefaultIfEmpty()

                                join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
                                from im_gla in im_gla_join.DefaultIfEmpty()

                                join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                                from im_icm in im_icm_join.DefaultIfEmpty()

                                join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
                                from im_bm in im_bm_join.DefaultIfEmpty()

                                where im.CompanyID == CompanyID && im.Room == RoomID
                                && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
                                && im.SupplierID == SupplierID

                                select new ItemMasterDTO
                                {
                                    ID = im.ID,
                                    ItemNumber = im.ItemNumber,
                                    ManufacturerID = im.ManufacturerID,
                                    ManufacturerNumber = im.ManufacturerNumber,
                                    ManufacturerName = im_mm.Manufacturer,
                                    SupplierID = im.SupplierID,
                                    SupplierPartNo = im.SupplierPartNo,
                                    SupplierName = im_sm.SupplierName,
                                    UPC = im.UPC,
                                    UNSPSC = im.UNSPSC,
                                    Description = im.Description,
                                    LongDescription = im.LongDescription,
                                    CategoryID = im.CategoryID,
                                    GLAccountID = im.GLAccountID,
                                    UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
                                    PricePerTerm = im.PricePerTerm,
                                    CostUOMID = im.CostUOMID,
                                    DefaultReorderQuantity = im.DefaultReorderQuantity,
                                    DefaultPullQuantity = im.DefaultPullQuantity,
                                    Cost = im.Cost,
                                    Markup = im.Markup,
                                    SellPrice = im.SellPrice,
                                    ExtendedCost = im.ExtendedCost,
                                    AverageCost = im.AverageCost,
                                    PerItemCost = im.PerItemCost,
                                    LeadTimeInDays = im.LeadTimeInDays,
                                    Link1 = im.Link1,
                                    Link2 = im.Link2,
                                    Trend = im.Trend,
                                    Taxable = im.Taxable,
                                    Consignment = im.Consignment,
                                    StagedQuantity = im.StagedQuantity,
                                    InTransitquantity = im.InTransitquantity,
                                    OnOrderQuantity = im.OnOrderQuantity,
                                    OnReturnQuantity = im.OnReturnQuantity,
                                    OnTransferQuantity = im.OnTransferQuantity,
                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                    SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                                    RequisitionedQuantity = im.RequisitionedQuantity,
                                    PackingQuantity = im.PackingQuantity,
                                    AverageUsage = im.AverageUsage,
                                    Turns = im.Turns,
                                    OnHandQuantity = im.OnHandQuantity,
                                    CriticalQuantity = im.CriticalQuantity,
                                    MinimumQuantity = im.MinimumQuantity,
                                    MaximumQuantity = im.MaximumQuantity,
                                    WeightPerPiece = im.WeightPerPiece,
                                    ItemUniqueNumber = im.ItemUniqueNumber,
                                    IsPurchase = (im.IsPurchase == true ? true : false),
                                    IsTransfer = (im.IsTransfer == true ? true : false),
                                    DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
                                    DefaultLocationName = im_bm.BinNumber,
                                    InventoryClassification = im.InventoryClassification,
                                    SerialNumberTracking = im.SerialNumberTracking,
                                    LotNumberTracking = im.LotNumberTracking,
                                    DateCodeTracking = im.DateCodeTracking,
                                    ItemType = im.ItemType,
                                    ImagePath = im.ImagePath,
                                    UDF1 = im.UDF1,
                                    UDF2 = im.UDF2,
                                    UDF3 = im.UDF3,
                                    UDF4 = im.UDF4,
                                    UDF5 = im.UDF5,
                                    ItemUDF1 = im.UDF1,
                                    ItemUDF2 = im.UDF2,
                                    ItemUDF3 = im.UDF3,
                                    ItemUDF4 = im.UDF4,
                                    ItemUDF5 = im.UDF5,
                                    UDF6 = im.UDF6,
                                    UDF7 = im.UDF7,
                                    UDF8 = im.UDF8,
                                    UDF9 = im.UDF9,
                                    UDF10 = im.UDF10,
                                    ItemUDF6 = im.UDF6,
                                    ItemUDF7 = im.UDF7,
                                    ItemUDF8 = im.UDF8,
                                    ItemUDF9 = im.UDF9,
                                    ItemUDF10 = im.UDF10,
                                    GUID = im.GUID,
                                    Created = im.Created,
                                    Updated = im.Updated,
                                    CreatedBy = im.CreatedBy,
                                    LastUpdatedBy = im.LastUpdatedBy,
                                    IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                                    IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                                    CompanyID = im.CompanyID,
                                    Room = im.Room,
                                    CreatedByName = im_UMC.UserName,
                                    UpdatedByName = im_UMU.UserName,
                                    RoomName = im_rm.RoomName,
                                    IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                                    CategoryName = im_CM.Category,
                                    Unit = im_UNM.Unit,
                                    GLAccount = im_gla.GLAccount,
                                    ItemImageExternalURL = im.ItemImageExternalURL,

                                    ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                                    ItemLink2ImageType = im.ItemLink2ImageType,
                                    //ItemLink2ImageType =im.ItemLink2ImageType,
                                    IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                                    IsBuildBreak = im.IsBuildBreak,
                                    BondedInventory = im.BondedInventory,
                                    IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                                    PullQtyScanOverride = im.PullQtyScanOverride,
                                    TrendingSetting = im.TrendingSetting,
                                    IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                                    LastCost = im.LastCost,
                                    IsActive = im.IsActive,
                                    IsOrderable = im.IsOrderable,
                                    IsAllowOrderCostuom = im.IsAllowOrderCostuom,
                                    MonthlyAverageUsage = Convert.ToInt64(im.AverageUsage.GetValueOrDefault(0) * MonthlyAverageUsage),
                                    OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,
                                    ItemLocations = (from A in context.ItemLocationDetails
                                                     join B in context.BinMasters on A.BinID equals B.ID into A_B_join
                                                     from A_B in A_B_join.DefaultIfEmpty()
                                                     join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
                                                     from A_C in A_C_join.DefaultIfEmpty()
                                                     join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
                                                     from A_D in A_D_join.DefaultIfEmpty()
                                                     where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
                                                     select new ItemLocationDetailsDTO
                                                     {
                                                         ID = A.ID,
                                                         BinID = A.BinID,
                                                         CustomerOwnedQuantity = A.CustomerOwnedQuantity,
                                                         ConsignedQuantity = A.ConsignedQuantity,
                                                         MeasurementID = A.MeasurementID,
                                                         LotNumber = A.LotNumber,
                                                         SerialNumber = A.SerialNumber,
                                                         ExpirationDate = A.ExpirationDate,
                                                         ReceivedDate = A.ReceivedDate,
                                                         Expiration = A.Expiration,
                                                         Received = A.Received,
                                                         Cost = A.Cost,
                                                         GUID = A.GUID,
                                                         ItemGUID = A.ItemGUID,
                                                         Created = A.Created,
                                                         Updated = A.Updated,
                                                         CreatedBy = A.CreatedBy,
                                                         LastUpdatedBy = A.LastUpdatedBy,
                                                         IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
                                                         IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
                                                         CompanyID = A.CompanyID,
                                                         Room = A.Room,
                                                         CreatedByName = A_C.UserName,
                                                         UpdatedByName = A_D.UserName,
                                                         BinNumber = A_B.BinNumber,
                                                         ItemNumber = im.ItemNumber,
                                                         SerialNumberTracking = im.SerialNumberTracking,
                                                         LotNumberTracking = im.LotNumberTracking,
                                                         DateCodeTracking = im.DateCodeTracking,
                                                         OrderDetailGUID = A.OrderDetailGUID,
                                                         TransferDetailGUID = A.TransferDetailGUID,
                                                         KitDetailGUID = A.KitDetailGUID,
                                                         CriticalQuantity = A_B.CriticalQuantity,
                                                         MinimumQuantity = A_B.MinimumQuantity,
                                                         MaximumQuantity = A_B.MaximumQuantity,
                                                         SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
                                                     }).AsEnumerable<ItemLocationDetailsDTO>(),
                                    //AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, im.GUID, "Item Master"),
                                    InventoryClassificationName = im_icm.InventoryClassification,
                                    IsBOMItem = im.IsBOMItem,
                                    RefBomId = im.RefBomId,
                                    CostUOMName = im_cuomm.CostUOM,
                                    IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
                                    OnQuotedQuantity = im.OnQuotedQuantity ?? 0
                                }).AsParallel().ToList();
                }
            }
            else
            {

                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                    {
                        MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                    }
                    if (MonthlyAverageUsage == null)
                    {
                        MonthlyAverageUsage = 30;
                    }
                    ObjCache = (from im in context.ItemMasters
                                join sm in context.SupplierMasters on im.SupplierID equals sm.ID into im_sm_join
                                from im_sm in im_sm_join.DefaultIfEmpty()

                                join mm in context.ManufacturerMasters on im.ManufacturerID equals mm.ID into im_mm_join
                                from im_mm in im_mm_join.DefaultIfEmpty()

                                join rm in context.Rooms on im.Room equals rm.ID into im_rm_join
                                from im_rm in im_rm_join.DefaultIfEmpty()

                                join cuomm in context.CostUOMMasters on im.CostUOMID equals cuomm.ID into im_cuomm_join
                                from im_cuomm in im_cuomm_join.DefaultIfEmpty()

                                join um in context.UserMasters on im.CreatedBy equals um.ID into im_UMC_join
                                from im_UMC in im_UMC_join.DefaultIfEmpty()

                                join umU in context.UserMasters on im.LastUpdatedBy equals umU.ID into im_UMU_join
                                from im_UMU in im_UMU_join.DefaultIfEmpty()

                                join cm in context.CategoryMasters on im.CategoryID equals cm.ID into im_CM_join
                                from im_CM in im_CM_join.DefaultIfEmpty()

                                join unm in context.UnitMasters on im.UOMID equals unm.ID into im_UNM_join
                                from im_UNM in im_UNM_join.DefaultIfEmpty()

                                join gla in context.GLAccountMasters on im.GLAccountID equals gla.ID into im_gla_join
                                from im_gla in im_gla_join.DefaultIfEmpty()

                                join icm in context.InventoryClassificationMasters on im.InventoryClassification equals icm.ID into im_icm_join
                                from im_icm in im_icm_join.DefaultIfEmpty()

                                join bm in context.BinMasters on im.DefaultLocation equals bm.ID into im_bm_join
                                from im_bm in im_bm_join.DefaultIfEmpty()

                                where im.CompanyID == CompanyID && im.Room == RoomID
                                && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived


                                select new ItemMasterDTO
                                {
                                    ID = im.ID,
                                    ItemNumber = im.ItemNumber,
                                    ManufacturerID = im.ManufacturerID,
                                    ManufacturerNumber = im.ManufacturerNumber,
                                    ManufacturerName = im_mm.Manufacturer,
                                    SupplierID = im.SupplierID,
                                    SupplierPartNo = im.SupplierPartNo,
                                    SupplierName = im_sm.SupplierName,
                                    UPC = im.UPC,
                                    UNSPSC = im.UNSPSC,
                                    Description = im.Description,
                                    LongDescription = im.LongDescription,
                                    CategoryID = im.CategoryID,
                                    GLAccountID = im.GLAccountID,
                                    UOMID = im_cuomm == null ? 0 : im_cuomm.ID,
                                    PricePerTerm = im.PricePerTerm,
                                    CostUOMID = im.CostUOMID,
                                    DefaultReorderQuantity = im.DefaultReorderQuantity,
                                    DefaultPullQuantity = im.DefaultPullQuantity,
                                    Cost = im.Cost,
                                    Markup = im.Markup,
                                    SellPrice = im.SellPrice,
                                    ExtendedCost = im.ExtendedCost,
                                    AverageCost = im.AverageCost,
                                    PerItemCost = im.PerItemCost,
                                    LeadTimeInDays = im.LeadTimeInDays,
                                    Link1 = im.Link1,
                                    Link2 = im.Link2,
                                    ItemLink2ImageType = im.ItemLink2ImageType,
                                    ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                                    Trend = im.Trend,
                                    Taxable = im.Taxable,
                                    Consignment = im.Consignment,
                                    StagedQuantity = im.StagedQuantity,
                                    InTransitquantity = im.InTransitquantity,
                                    OnOrderQuantity = im.OnOrderQuantity,
                                    OnReturnQuantity = im.OnReturnQuantity,
                                    OnTransferQuantity = im.OnTransferQuantity,
                                    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                                    SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                                    RequisitionedQuantity = im.RequisitionedQuantity,
                                    PackingQuantity = im.PackingQuantity,
                                    AverageUsage = im.AverageUsage,
                                    Turns = im.Turns,
                                    OnHandQuantity = im.OnHandQuantity,
                                    CriticalQuantity = im.CriticalQuantity,
                                    MinimumQuantity = im.MinimumQuantity,
                                    MaximumQuantity = im.MaximumQuantity,
                                    WeightPerPiece = im.WeightPerPiece,
                                    ItemUniqueNumber = im.ItemUniqueNumber,
                                    IsPurchase = (im.IsPurchase == true ? true : false),
                                    IsTransfer = (im.IsTransfer == true ? true : false),
                                    DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
                                    DefaultLocationName = im_bm.BinNumber,
                                    InventoryClassification = im.InventoryClassification,
                                    SerialNumberTracking = im.SerialNumberTracking,
                                    LotNumberTracking = im.LotNumberTracking,
                                    DateCodeTracking = im.DateCodeTracking,
                                    ItemType = im.ItemType,
                                    ImagePath = im.ImagePath,
                                    UDF1 = im.UDF1,
                                    UDF2 = im.UDF2,
                                    UDF3 = im.UDF3,
                                    UDF4 = im.UDF4,
                                    UDF5 = im.UDF5,
                                    ItemUDF1 = im.UDF1,
                                    ItemUDF2 = im.UDF2,
                                    ItemUDF3 = im.UDF3,
                                    ItemUDF4 = im.UDF4,
                                    ItemUDF5 = im.UDF5,
                                    UDF6 = im.UDF6,
                                    UDF7 = im.UDF7,
                                    UDF8 = im.UDF8,
                                    UDF9 = im.UDF9,
                                    UDF10 = im.UDF10,
                                    ItemUDF6 = im.UDF6,
                                    ItemUDF7 = im.UDF7,
                                    ItemUDF8 = im.UDF8,
                                    ItemUDF9 = im.UDF9,
                                    ItemUDF10 = im.UDF10,
                                    GUID = im.GUID,
                                    Created = im.Created,
                                    Updated = im.Updated,
                                    CreatedBy = im.CreatedBy,
                                    LastUpdatedBy = im.LastUpdatedBy,
                                    IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                                    IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                                    CompanyID = im.CompanyID,
                                    Room = im.Room,
                                    CreatedByName = im_UMC.UserName,
                                    UpdatedByName = im_UMU.UserName,
                                    RoomName = im_rm.RoomName,
                                    IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                                    CategoryName = im_CM.Category,
                                    Unit = im_UNM.Unit,
                                    GLAccount = im_gla.GLAccount,
                                    IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                                    IsBuildBreak = im.IsBuildBreak,
                                    BondedInventory = im.BondedInventory,
                                    PullQtyScanOverride = im.PullQtyScanOverride,
                                    TrendingSetting = im.TrendingSetting,
                                    IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                                    IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                                    LastCost = im.LastCost,
                                    IsActive = im.IsActive,
                                    IsOrderable = im.IsOrderable,
                                    IsAllowOrderCostuom = im.IsAllowOrderCostuom,
                                    MonthlyAverageUsage = Convert.ToInt64(im.AverageUsage.GetValueOrDefault(0) * MonthlyAverageUsage),
                                    OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,
                                    ItemLocations = (from A in context.ItemLocationDetails
                                                     join B in context.BinMasters on A.BinID equals B.ID into A_B_join
                                                     from A_B in A_B_join.DefaultIfEmpty()
                                                     join C in context.UserMasters on A.CreatedBy equals C.ID into A_C_join
                                                     from A_C in A_C_join.DefaultIfEmpty()
                                                     join D in context.UserMasters on A.LastUpdatedBy equals D.ID into A_D_join
                                                     from A_D in A_D_join.DefaultIfEmpty()
                                                     where A.ItemGUID == im.GUID && (A.IsDeleted ?? false) == false && (A.IsArchived ?? false) == false
                                                     select new ItemLocationDetailsDTO
                                                     {
                                                         ID = A.ID,
                                                         BinID = A.BinID,
                                                         CustomerOwnedQuantity = A.CustomerOwnedQuantity,
                                                         ConsignedQuantity = A.ConsignedQuantity,
                                                         MeasurementID = A.MeasurementID,
                                                         LotNumber = A.LotNumber,
                                                         SerialNumber = A.SerialNumber,
                                                         ExpirationDate = A.ExpirationDate,
                                                         ReceivedDate = A.ReceivedDate,
                                                         Expiration = A.Expiration,
                                                         Received = A.Received,
                                                         Cost = A.Cost,
                                                         GUID = A.GUID,
                                                         ItemGUID = A.ItemGUID,
                                                         Created = A.Created,
                                                         Updated = A.Updated,
                                                         CreatedBy = A.CreatedBy,
                                                         LastUpdatedBy = A.LastUpdatedBy,
                                                         IsDeleted = (A.IsDeleted.HasValue ? A.IsDeleted : false),
                                                         IsArchived = (A.IsArchived.HasValue ? A.IsArchived : false),
                                                         CompanyID = A.CompanyID,
                                                         Room = A.Room,
                                                         CreatedByName = A_C.UserName,
                                                         UpdatedByName = A_D.UserName,
                                                         BinNumber = A_B.BinNumber,
                                                         ItemNumber = im.ItemNumber,
                                                         SerialNumberTracking = im.SerialNumberTracking,
                                                         LotNumberTracking = im.LotNumberTracking,
                                                         DateCodeTracking = im.DateCodeTracking,
                                                         OrderDetailGUID = A.OrderDetailGUID,
                                                         TransferDetailGUID = A.TransferDetailGUID,
                                                         KitDetailGUID = A.KitDetailGUID,
                                                         CriticalQuantity = A_B.CriticalQuantity,
                                                         MinimumQuantity = A_B.MinimumQuantity,
                                                         MaximumQuantity = A_B.MaximumQuantity,
                                                         SuggestedOrderQuantity = context.CartItems.Where(t => t.ItemGUID == im.GUID && t.BinId == A_B.ID && t.IsDeleted == false && t.IsArchived == false).Select(t => t.Quantity).Sum(),
                                                     }).AsEnumerable<ItemLocationDetailsDTO>(),
                                    // AppendedBarcodeString = objBarcodeMasterDAL.GetComaSaperatedBarcodes(lstBarcodeDTO, im.GUID, "Item Master"),
                                    InventoryClassificationName = im_icm.InventoryClassification,
                                    IsBOMItem = im.IsBOMItem,
                                    RefBomId = im.RefBomId,
                                    CostUOMName = im_cuomm.CostUOM,
                                    IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
                                    OnQuotedQuantity = im.OnQuotedQuantity ?? 0
                                }).AsParallel().ToList();
                }
            }
            #endregion
            return ObjCache;
        }

        public Int64 GetRecordForDropDown(string GUID, Int64 RoomID, Int64 CompanyID)
        {
            ItemMasterDTO objDTO = new ItemMasterDTO();
            ItemMasterDAL ItemDAL = new ItemMasterDAL(base.DataBaseName);
            Guid itemGUID = Guid.Empty;
            if (Guid.TryParse(GUID, out itemGUID))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    objDTO = ItemDAL.GetItemWithoutJoins(null, itemGUID);
                    if (objDTO != null)
                    {
                        return objDTO.DefaultLocation.GetValueOrDefault(0);
                    }
                    else
                    {
                        return 0;
                    }
                }
                //.FirstOrDefault().DefaultLocation.GetValueOrDefault(0);
            }
            else
            {
                return 0;
            }
        }
        public Int64 Insert(ItemMasterDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                var oRoom = objRoomDAL.GetRoomByIDPlain(objDTO.Room.GetValueOrDefault(0));//context.Rooms.Where(x => x.ID == objDTO.Room && x.CompanyID == objDTO.CompanyID).FirstOrDefault();

                if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                    && !objDTO.Consignment
                    && (objDTO.OnHandQuantity == null || objDTO.OnHandQuantity <= 0))
                {
                    //objDTO.Cost = 0;
                    //objDTO.SellPrice = 0;
                }

                ItemMaster obj = new ItemMaster();
                obj.ID = 0;
                obj.ItemNumber = objDTO.ItemNumber;

                if (objDTO.ManufacturerID > 0)
                    obj.ManufacturerID = objDTO.ManufacturerID;

                obj.ManufacturerNumber = objDTO.ManufacturerNumber;

                if (objDTO.ItemType != 4)
                {
                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;
                    obj.SupplierPartNo = objDTO.SupplierPartNo;
                }
                obj.UPC = objDTO.UPC;
                obj.UNSPSC = objDTO.UNSPSC;
                obj.Description = objDTO.Description;
                obj.LongDescription = objDTO.LongDescription;
                if (objDTO.CategoryID > 0)
                    obj.CategoryID = objDTO.CategoryID;

                obj.GLAccountID = objDTO.GLAccountID;

                if (objDTO.UOMID > 0)
                    obj.UOMID = objDTO.UOMID;

                obj.PricePerTerm = objDTO.PricePerTerm;
                obj.CostUOMID = objDTO.CostUOMID;
                obj.OrderUOMID = objDTO.OrderUOMID;
                obj.CategoryID = objDTO.CategoryID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.QtyToMeetDemand = (objDTO.QtyToMeetDemand == null ? 0 : objDTO.QtyToMeetDemand.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                //obj.ExtendedCost = objDTO.ExtendedCost;
                obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                obj.Link1 = objDTO.Link1;
                obj.Link2 = objDTO.Link2;
                obj.Trend = objDTO.Trend;
                obj.IsAutoInventoryClassification = objDTO.IsAutoInventoryClassification;
                obj.Taxable = objDTO.Taxable;
                obj.Consignment = objDTO.Consignment;
                obj.StagedQuantity = objDTO.StagedQuantity;
                obj.InTransitquantity = objDTO.InTransitquantity;
                obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                obj.OnOrderInTransitQuantity = objDTO.OnOrderInTransitQuantity;
                obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.SuggestedTransferQuantity = objDTO.SuggestedTransferQuantity;
                obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                obj.PackingQuantity = objDTO.PackingQuantity;
                obj.AverageUsage = objDTO.AverageUsage;
                obj.Turns = objDTO.Turns;
                obj.OnHandQuantity = objDTO.OnHandQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity ?? 0;
                obj.MinimumQuantity = objDTO.MinimumQuantity ?? 0;
                obj.MaximumQuantity = objDTO.MaximumQuantity ?? 0;
                obj.WeightPerPiece = objDTO.WeightPerPiece;
                //obj.ItemUniqueNumber = objCommonDAL.UniqueItemId();    //objDTO.ItemUniqueNumber;
                obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                obj.IsTransfer = objDTO.IsTransfer;
                obj.IsPurchase = objDTO.IsPurchase;

                // if (objDTO.DefaultLocation > 0)
                obj.DefaultLocation = objDTO.DefaultLocation;

                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                if (string.IsNullOrWhiteSpace(objDTO.ImageType))
                {
                    objDTO.ImageType = "ExternalImage";

                }
                obj.ImageType = objDTO.ImageType;

                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                if (string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                {
                    objDTO.ItemLink2ImageType = "InternalLink";

                }
                obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                obj.ImagePath = objDTO.ImagePath;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                if (objDTO.GUID != Guid.Empty)
                {
                    obj.GUID = objDTO.GUID;
                }
                else
                {
                    obj.GUID = Guid.NewGuid();
                }
                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                obj.BondedInventory = objDTO.BondedInventory;
                obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                obj.TrendingSetting = objDTO.TrendingSetting;
                obj.ItemType = objDTO.ItemType;
                if (string.IsNullOrWhiteSpace(objDTO.WhatWhereAction))
                {
                    objDTO.WhatWhereAction = "Item";
                }

                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.IsPackslipMandatoryAtReceive = objDTO.IsPackslipMandatoryAtReceive;

                obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                obj.ReceivedOnWeb = (objDTO.ReceivedOnWeb.HasValue ? Convert.ToDateTime(objDTO.ReceivedOnWeb) : DateTimeUtility.DateTimeNow);
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.pricesaveddate = DateTime.UtcNow;
                obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;
                obj.IsActive = objDTO.IsActive;
                //if (objDTO.IsActive == null)
                //{
                //    obj.IsActive = true;
                //}
                //else
                //{
                //    obj.IsActive = objDTO.IsActive;
                //}
                obj.IsOrderable = objDTO.IsOrderable;
                obj.ItemIsActiveDate = objDTO.ItemIsActiveDate;
                obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                obj.WeightVariance = objDTO.WeightVariance;
                obj.OnQuotedQuantity = objDTO.OnQuotedQuantity ?? 0;
                obj.eLabelKey = objDTO.eLabelKey;
                obj.EnrichedProductData = objDTO.EnrichedProductData;
                obj.EnhancedDescription = objDTO.EnhancedDescription;
                obj.POItemLineNumber = objDTO.POItemLineNumber;
                context.ItemMasters.Add(obj);


                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;
                //string strUpdateOnHand = "EXEC [dbo].[AutoCartEntryonInventoryUpDown] '" + obj.GUID.ToString() + "', " + obj.CreatedBy;
                //context.Database.ExecuteSqlCommand(strUpdateOnHand);

                objDTO = FillWithExtraDetail(objDTO);
                //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0);
                //objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(obj.GUID);
                return obj.ID;
            }

        }
        public ItemMasterDTO FillWithExtraDetail(ItemMasterDTO objDTO)
        {
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RoomDTO ObjRoomTemp = null;
            if (objDTO.Room.GetValueOrDefault(0) > 0)
            {
                ObjRoomTemp = objRoomDAL.GetRoomByIDPlain(objDTO.Room.GetValueOrDefault(0));
            }

            ManufacturerMasterDAL objItemManDAL = new ManufacturerMasterDAL(base.DataBaseName);
            ManufacturerMasterDTO ObjItemManCache = null;

            if (objDTO.ManufacturerID.HasValue && objDTO.ManufacturerID.Value > 0)
            {
                ObjItemManCache = objItemManDAL.GetManufacturerByIDPlain(objDTO.ManufacturerID.Value, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            }

            SupplierMasterDAL objItemSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO ObjItemSuppTemp = null;

            if (objDTO.SupplierID.GetValueOrDefault(0) > 0)
            {
                ObjItemSuppTemp = objItemSupDAL.GetSupplierByIDPlain(objDTO.SupplierID.GetValueOrDefault(0));
            }

            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);

            string createdByUserName = objUserDAL.GetUserNameByUserId(objDTO.CreatedBy.GetValueOrDefault(0));
            string updatedByUserName = objUserDAL.GetUserNameByUserId(objDTO.LastUpdatedBy.GetValueOrDefault(0));

            UnitMasterDAL objUnitDAL = new UnitMasterDAL(base.DataBaseName);
            UnitMasterDTO objUnitTemp = null;
            if (objDTO.UOMID.GetValueOrDefault(0) > 0)
            {
                objUnitTemp = objUnitDAL.GetUnitByIDPlain(objDTO.UOMID ?? 0);
            }

            CategoryMasterDAL objCategoryDAL = new CategoryMasterDAL(base.DataBaseName);
            CategoryMasterDTO objCategoryCache = null;
            if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
            {
                objCategoryCache = objCategoryDAL.GetCategoryByIdPlain(objDTO.CategoryID.GetValueOrDefault(0));
            }
            GLAccountMasterDAL objGLAccountDAL = new GLAccountMasterDAL(base.DataBaseName);
            GLAccountMasterDTO objGLAccountCaChe = null;
            if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
            {
                objGLAccountCaChe = objGLAccountDAL.GetGLAccountByIdPlain(objDTO.GLAccountID ?? 0);
            }
            objDTO.RoomName = ObjRoomTemp.RoomName;

            if (ObjItemManCache != null)
                objDTO.ManufacturerName = ObjItemManCache.Manufacturer;

            if (ObjItemSuppTemp != null)
                objDTO.SupplierName = ObjItemSuppTemp.SupplierName;

            if (!string.IsNullOrEmpty(createdByUserName))
                objDTO.CreatedByName = createdByUserName;

            if (!string.IsNullOrEmpty(updatedByUserName))
                objDTO.UpdatedByName = updatedByUserName;

            if (objUnitTemp != null && objUnitTemp.ID > 0)
                objDTO.Unit = objUnitTemp.Unit;

            if (objCategoryCache != null)
                objDTO.CategoryName = objCategoryCache.Category;

            if (objGLAccountCaChe != null)
                objDTO.GLAccount = objGLAccountCaChe.GLAccount;

            ItemOrderInfoDTO objItemOrderInfoDTO = GetItemOnOrderQuantity(objDTO.GUID, objDTO.Room ?? 0, objDTO.CompanyID ?? 0);

            if (objItemOrderInfoDTO != null)
            {
                objDTO.OnOrderQuantity = objItemOrderInfoDTO.OnOrderQuantity;
                objDTO.OnOrderInTransitQuantity = objItemOrderInfoDTO.OnOrderInTransitQuantity;
            }

            return objDTO;
        }

        public BOMItemMasterMain FillWithExtraDetailImport(BOMItemMasterMain objDTO)
        {
            RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
            RoomDTO ObjRoomTemp = null;
            if (objDTO.Room.GetValueOrDefault(0) > 0)
            {
                ObjRoomTemp = objRoomDAL.GetRoomByIDPlain(objDTO.Room.GetValueOrDefault(0));
            }

            ManufacturerMasterDAL objItemManDAL = new ManufacturerMasterDAL(base.DataBaseName);
            ManufacturerMasterDTO ObjItemManCache = null;

            if (objDTO.ManufacturerID.HasValue && objDTO.ManufacturerID.Value > 0)
            {
                ObjItemManCache = objItemManDAL.GetManufacturerByIDPlain(objDTO.ManufacturerID.Value, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
            }

            SupplierMasterDAL objItemSupDAL = new SupplierMasterDAL(base.DataBaseName);
            SupplierMasterDTO ObjItemSuppTemp = null;

            if (objDTO.SupplierID.GetValueOrDefault(0) > 0)
            {
                ObjItemSuppTemp = objItemSupDAL.GetSupplierByIDPlain(objDTO.SupplierID.GetValueOrDefault(0));
            }

            UserMasterDAL objUserDAL = new UserMasterDAL(base.DataBaseName);
            string createdByUserName = objUserDAL.GetUserNameByUserId(objDTO.CreatedBy.GetValueOrDefault(0));
            string updatedByUserName = objUserDAL.GetUserNameByUserId(objDTO.LastUpdatedBy.GetValueOrDefault(0));

            UnitMasterDAL objUnitDAL = new UnitMasterDAL(base.DataBaseName);
            UnitMasterDTO objUnitTemp = null;
            if (objDTO.UOMID > 0)
            {
                objUnitTemp = objUnitDAL.GetUnitByIDPlain(objDTO.UOMID);
            }

            CategoryMasterDAL objCategoryDAL = new CategoryMasterDAL(base.DataBaseName);
            CategoryMasterDTO objCategoryCache = null;
            if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
            {
                objCategoryCache = objCategoryDAL.GetCategoryByIdPlain(objDTO.CategoryID.GetValueOrDefault(0));
            }
            GLAccountMasterDAL objGLAccountDAL = new GLAccountMasterDAL(base.DataBaseName);
            GLAccountMasterDTO objGLAccountCaChe = null;
            if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
            {
                objGLAccountCaChe = objGLAccountDAL.GetGLAccountByIdPlain(objDTO.GLAccountID ?? 0);
            }
            objDTO.RoomName = ObjRoomTemp.RoomName;

            if (ObjItemManCache != null)
                objDTO.ManufacturerName = ObjItemManCache.Manufacturer;

            if (ObjItemSuppTemp != null)
                objDTO.SupplierName = ObjItemSuppTemp.SupplierName;

            if (!string.IsNullOrEmpty(createdByUserName))
                objDTO.CreatedByName = createdByUserName;

            if (!string.IsNullOrEmpty(updatedByUserName))
                objDTO.UpdatedByName = updatedByUserName;

            if (objUnitTemp != null)
                objDTO.Unit = objUnitTemp.Unit;

            if (objCategoryCache != null)
                objDTO.CategoryName = objCategoryCache.Category;

            if (objGLAccountCaChe != null)
                objDTO.GLAccount = objGLAccountCaChe.GLAccount;

            return objDTO;
        }

        public bool Edit(ItemMasterDTO objDTO, long SessionUserId, long EnterpriseId, bool IsFromItemPage = false,bool IsAutoSOTLater = false,bool IgnoreAutoSOT = false)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            RequisitionDetailsDAL objReqDetailDAL = new RequisitionDetailsDAL(DataBaseName);
            OrderUOMMasterDAL objOUMDAL = new OrderUOMMasterDAL(DataBaseName);

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = context.ItemMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();

                if (obj != null)
                {
                    double? OldItemCost = obj.Cost;
                    //obj.ID = objDTO.ID;
                    obj.ItemNumber = objDTO.ItemNumber;

                    if (objDTO.ManufacturerID > 0 || IsFromItemPage)
                        obj.ManufacturerID = objDTO.ManufacturerID;

                    obj.ManufacturerNumber = objDTO.ManufacturerNumber;

                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;

                    obj.SupplierPartNo = objDTO.SupplierPartNo;
                    obj.UPC = objDTO.UPC;
                    obj.UNSPSC = objDTO.UNSPSC;
                    obj.Description = objDTO.Description;
                    obj.LongDescription = objDTO.LongDescription;

                    if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                        obj.CategoryID = objDTO.CategoryID;

                    //if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                    obj.GLAccountID = objDTO.GLAccountID;

                    if (objDTO.UOMID > 0)
                        obj.UOMID = objDTO.UOMID;

                    obj.PricePerTerm = objDTO.PricePerTerm;
                    obj.CostUOMID = objDTO.CostUOMID;
                    //obj.OrderUOMID = objDTO.OrderUOMID;
                    obj.OrderUOMID = objOUMDAL.GetOrderUomIdByCostUomId(Convert.ToInt64(objDTO.CostUOMID), objDTO);
                    obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                    obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                    obj.Cost = objDTO.Cost;
                    obj.Markup = objDTO.Markup;
                    if (obj.SellPrice != objDTO.SellPrice)
                    {
                        obj.pricesaveddate = DateTime.UtcNow;
                    }
                    obj.SellPrice = objDTO.SellPrice;

                    //obj.ExtendedCost = objDTO.ExtendedCost;
                    obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                    obj.Link1 = objDTO.Link1;
                    if (!string.IsNullOrEmpty(objDTO.Link2))
                    {
                        obj.Link2 = objDTO.Link2;
                    }
                    obj.ItemType = objDTO.ItemType;
                    obj.Trend = objDTO.Trend;
                    obj.IsAutoInventoryClassification = objDTO.IsAutoInventoryClassification;
                    obj.Taxable = objDTO.Taxable;
                    obj.Consignment = objDTO.Consignment;
                    obj.StagedQuantity = objDTO.StagedQuantity;
                    obj.InTransitquantity = objDTO.InTransitquantity;
                    obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                    obj.OnOrderInTransitQuantity = objDTO.OnOrderInTransitQuantity;
                    obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                    obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                    obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                    obj.SuggestedTransferQuantity = objDTO.SuggestedTransferQuantity;
                    //obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                    obj.RequisitionedQuantity = objReqDetailDAL.GetItemCurrentOnRequisitionQty(obj.GUID);
                    obj.PackingQuantity = objDTO.PackingQuantity;
                    obj.AverageUsage = objDTO.AverageUsage;
                    obj.Turns = objDTO.Turns;
                    if (!IsFromItemPage)
                        obj.OnHandQuantity = objDTO.OnHandQuantity;
                    obj.IsActive = objDTO.IsActive;
                    obj.IsOrderable = objDTO.IsOrderable;
                    obj.CriticalQuantity = objDTO.CriticalQuantity ?? 0;
                    obj.MinimumQuantity = objDTO.MinimumQuantity ?? 0;
                    obj.MaximumQuantity = objDTO.MaximumQuantity ?? 0;
                    obj.WeightPerPiece = objDTO.WeightPerPiece;
                    if (string.IsNullOrEmpty(obj.ItemUniqueNumber))
                    {
                        if (!string.IsNullOrEmpty(objDTO.ItemUniqueNumber))
                            obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                        else
                        {
                            //obj.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                            obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                        }
                    }
                    //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                    obj.IsPurchase = objDTO.IsPurchase;
                    obj.IsTransfer = objDTO.IsTransfer;
                    obj.DefaultLocation = objDTO.DefaultLocation;
                    obj.InventoryClassification = objDTO.InventoryClassification;
                    obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                    obj.LotNumberTracking = objDTO.LotNumberTracking;
                    obj.DateCodeTracking = objDTO.DateCodeTracking;
                    if (!string.IsNullOrWhiteSpace(objDTO.ImageType))
                    {
                        obj.ImageType = objDTO.ImageType;
                    }
                    obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                    if (!string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                    {
                        obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                    }
                    if (!string.IsNullOrEmpty(objDTO.ImagePath))
                    {
                        obj.ImagePath = objDTO.ImagePath;
                    }

                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.UDF6 = objDTO.UDF6;
                    obj.UDF7 = objDTO.UDF7;
                    obj.UDF8 = objDTO.UDF8;
                    obj.UDF9 = objDTO.UDF9;
                    obj.UDF10 = objDTO.UDF10;
                    obj.GUID = objDTO.GUID;
                    //obj.Created = objDTO.Created;
                    //obj.Updated = objDTO.Updated;
                    if (IsFromItemPage)
                    {
                        obj.Updated = objDTO.Updated;
                        obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    }
                    //obj.CreatedBy = objDTO.CreatedBy;

                    //obj.IsDeleted = false;
                    //obj.IsArchived = false;
                    obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.Room = objDTO.Room;
                    obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                    obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                    obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                    obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                    // Get ext cost based on Item Location details START
                    CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                    //if (!obj.Consignment)
                    //{
                    //    obj.Cost = ObjCostDTO.Cost;
                    //}
                    //else
                    //{
                    //    obj.Cost = objDTO.Cost;
                    //}
                    //RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    //var oRoom = objRoomDAL.GetRoomByIDPlain(obj.Room.GetValueOrDefault());
                    //Room oRoom = context.Rooms.Where(x => x.ID == obj.Room && x.CompanyID == obj.CompanyID).FirstOrDefault();
                    //if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !obj.Consignment)
                    //{
                    //    //obj.Cost = objDTO.Cost = OldItemCost;
                    //}
                    //else if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !obj.Consignment && objDTO.Cost != OldItemCost)
                    //{
                    //    //new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                    //}
                    //else if (obj.Consignment && objDTO.Cost != OldItemCost)
                    //{
                    //    new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                    //}

                    obj.ExtendedCost = ObjCostDTO.ExtCost;
                    obj.AverageCost = ObjCostDTO.AvgCost;
                    obj.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                    objDTO.AverageCost = ObjCostDTO.AvgCost;
                    objDTO.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.Cost = obj.Cost;

                    //if ((obj.Markup ?? 0) > 0)
                    //{
                    //    obj.SellPrice = (obj.Cost ?? 0) + (((obj.Cost ?? 0) * (obj.Markup ?? 0)) / 100);
                    //}
                    //else
                    //{
                    //    obj.SellPrice = obj.Cost;
                    //}
                    objDTO.SellPrice = obj.SellPrice;
                    // Get ext cost based on Item Location details END
                    obj.BondedInventory = objDTO.BondedInventory;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Item";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.IsBOMItem = objDTO.IsBOMItem;
                    obj.RefBomId = objDTO.RefBomId;
                    obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                    obj.TrendingSetting = objDTO.TrendingSetting;
                    obj.IsPackslipMandatoryAtReceive = objDTO.IsPackslipMandatoryAtReceive;
                    obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                    obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                    obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;

                    if (objDTO.IsOnlyFromItemUI) //Only Updated When Item Updated From Item Detail Page
                    {
                        obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                        if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                            obj.EditedFrom = objDTO.EditedFrom;
                        else
                            obj.EditedFrom = "Web";
                    }
                    obj.QtyToMeetDemand = (objDTO.QtyToMeetDemand == null ? 0 : objDTO.QtyToMeetDemand.Value);
                    obj.ItemIsActiveDate = objDTO.ItemIsActiveDate;
                    obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                    obj.WeightVariance = objDTO.WeightVariance;
                    obj.OnQuotedQuantity = objDTO.OnQuotedQuantity ?? 0;
                    obj.eLabelKey = objDTO.eLabelKey;
                    obj.EnrichedProductData = objDTO.EnrichedProductData;
                    if (objDTO.EnhancedDescription != null)
                    {
                        obj.EnhancedDescription = objDTO.EnhancedDescription;
                    }
                    obj.POItemLineNumber = objDTO.POItemLineNumber;
                    //obj.ReceivedOnWeb = Convert.ToDateTime(objDTO.ReceivedOnWeb);
                    //obj.AddedFrom = objDTO.AddedFrom;
                    //context.ItemMasters.Attach(obj);
                    //context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    new DashboardDAL(base.DataBaseName).SetItemsAutoClassification(obj.GUID, obj.Room ?? 0, obj.CompanyID ?? 0, obj.LastUpdatedBy ?? 0, 1);
                    objDTO = FillWithExtraDetail(objDTO);
                    //UpdateON_OrderFlagOnSolum (EnterpriseId, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), objDTO.GUID, objDTO.OnOrderQuantity.GetValueOrDefault(0), objDTO.SupplierPartNo);
                    string DefaultLocationName = objDTO.DefaultLocationName;
                    if (objDTO.DefaultLocation.GetValueOrDefault(0) > 0)
                    {
                        DefaultLocationName = new BinMasterDAL(base.DataBaseName).GetBinByID(Int64.Parse(Convert.ToString(objDTO.DefaultLocation)), objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0)).BinNumber;
                    }
                    AddUpdateSolumnProduct(obj.SupplierPartNo, obj.ItemNumber, obj.GUID, obj.Description, obj.MinimumQuantity.ToString(), obj.MaximumQuantity.ToString(), obj.DefaultReorderQuantity, obj.CostUOMID, DefaultLocationName, obj.OnOrderQuantity, String.Empty, EnterpriseId, objDTO.CompanyID.GetValueOrDefault(0), objDTO.Room.GetValueOrDefault(0), objDTO.eLabelKey);
                }
            }

            //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), "web", "ItemMasterDAL >> EDIT");
            if (!IsAutoSOTLater)
            {
                if (!IgnoreAutoSOT)
                {
                    new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), "web", "Inventory >> Modified Item", SessionUserId);
                }
                objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(objDTO.GUID);
                objDTO.SuggestedTransferQuantity = GetSuggestedTransferQty(objDTO.GUID);

                /* update QtyToMeetDemand */
                if (objDTO.ItemType == 3 && objDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                {
                    new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), SessionUserId);
                }
            }
            else
            {
                if (!IgnoreAutoSOT)
                {
                    bool UpdateQtyToMeedDemandrequired = false;
                    if (objDTO.ItemType == 3 && objDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                    {
                        UpdateQtyToMeedDemandrequired = true;
                    }
                    new CartItemDAL(base.DataBaseName).AddAutosotForImport(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), base.DataBaseName, UpdateQtyToMeedDemandrequired, SessionUserId);
                }
                else
                {
                    objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(objDTO.GUID);
                    objDTO.SuggestedTransferQuantity = GetSuggestedTransferQty(objDTO.GUID);

                    /* update QtyToMeetDemand */
                    if (objDTO.ItemType == 3 && objDTO.IsBuildBreak.GetValueOrDefault(false) == true)
                    {
                        new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), SessionUserId);
                    }
                }
            }
            /* update QtyToMeetDemand */

            //if (objDTO.SuggestedOrderQuantity.GetValueOrDefault(0) > 0)
            //    SendMailForSuggestedOrder(objDTO.ItemNumber, objDTO.OnHandQuantity.GetValueOrDefault(0), objDTO.CriticalQuantity, objDTO.MinimumQuantity, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), "Critical", objDTO.RoomName);

            return true;
        }
        public void UpdateON_OrderFlagOnSolum(long EnterpriseId, long CompanyId, long RoomId, Guid ItemGUID, double OnOrderQuantity, string supplierPartNo)
        {

            try
            {
                var defaultSupplierPartNo = supplierPartNo;

                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                var cartList = objCartItemDAL.GetCartsByItemGUIDPlain(ItemGUID, RoomId, CompanyId);
                bool manualCartEntry = (cartList != null && cartList.Where(x => x.IsDeleted == false && x.IsAutoMatedEntry == false).Any());

                if (!string.IsNullOrEmpty(defaultSupplierPartNo) && !string.IsNullOrWhiteSpace(defaultSupplierPartNo))
                {
                    var quickBookDBName = "eTurnsQuickBook";
                    var quickBookDBNameFromConfig = Convert.ToString(ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);

                    if (!string.IsNullOrWhiteSpace(quickBookDBNameFromConfig) && !string.IsNullOrEmpty(quickBookDBNameFromConfig))
                    {
                        quickBookDBName = quickBookDBNameFromConfig;
                    }

                    SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                    var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterpriseId, CompanyId, RoomId);

                    if (solumStore != null && solumStore.ID > 0)
                    {
                        string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";

                        if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                        {
                            solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                        }

                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                        var requestURL = "v2/common/config/article/info?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + defaultSupplierPartNo.Trim();
                        HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                        string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                        //CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                        var article = JsonConvert.DeserializeObject<ArticleInfoDTO>(respstr);

                        if (article != null && !string.IsNullOrEmpty(article.responseMessage) && !string.IsNullOrWhiteSpace(article.responseMessage) && article.responseMessage.ToLower() == "success")
                        {
                            var articles = article.articleList;
                            if (articles != null && articles.Any() && articles.Count() > 0 && articles[0].data != null)
                            {
                                string ON_ORDER_Flag = manualCartEntry || OnOrderQuantity > 0 ? "1" : "0";

                                if (ON_ORDER_Flag != articles[0].data.ON_ORDER)
                                {
                                    articles[0].data.ON_ORDER = ON_ORDER_Flag;
                                    if (ON_ORDER_Flag == "1")
                                    {
                                        articles[0].data.Shipped = "0";
                                        articles[0].data.BackOrdered = "0";
                                    }

                                    //var articleData = new SolumArticle();
                                    List<Article> articleList = new List<Article>();
                                    var item = new Article();
                                    //item.companyCode = solumStore.CompanyName;
                                    //item.stationCode = articles[0].stationCode;
                                    item.articleId = defaultSupplierPartNo;
                                    //item.articleName = objDTO.ItemNumber;
                                    item.data = articles[0].data;
                                    articleList.Add(item);
                                    //articleData.dataList = articleList;

                                    var postURL = "v2/common/articles?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode;
                                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleList.ToArray()), Encoding.UTF8);
                                    //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                    HttpResponseMessage postResponseMessage = client.PostAsync(postURL, httpContent).Result;
                                    //string response = postResponseMessage.Content.ReadAsStringAsync().Result;
                                    //var authenticationResponse = JsonConvert.DeserializeObject<SolumArticlePostResponse>(response);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                //CommonFunctions.SaveLogInTextFile(" Error on ItemSave >> solum article info exception: " + ex.Message ?? string.Empty + " : " + System.DateTime.UtcNow);
            }
        }
        public bool EditItemSVC(ItemMasterDTO objDTO, long SessionUserId)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            objDTO.Updated = DateTimeUtility.DateTimeNow;

            RequisitionDetailsDAL objReqDetailDAL = new RequisitionDetailsDAL(DataBaseName);
            OrderUOMMasterDAL objOUMDAL = new OrderUOMMasterDAL(DataBaseName);
            int itemType = 0;
            Guid itemGuid = Guid.Empty;
            bool isBuildBreak = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = context.ItemMasters.Where(x => x.GUID == objDTO.GUID).FirstOrDefault();

                if (obj != null)
                {
                    double? OldItemCost = obj.Cost;

                    itemType = obj.ItemType;
                    itemGuid = obj.GUID;
                    isBuildBreak = obj.IsBuildBreak.GetValueOrDefault(false);
                    obj.ItemNumber = objDTO.ItemNumber;
                    obj.ManufacturerID = objDTO.ManufacturerID;
                    obj.ManufacturerNumber = objDTO.ManufacturerNumber;

                    if (objDTO.SupplierID > 0)
                        obj.SupplierID = objDTO.SupplierID;

                    obj.SupplierPartNo = objDTO.SupplierPartNo;
                    obj.UPC = objDTO.UPC;
                    obj.UNSPSC = objDTO.UNSPSC;
                    obj.Description = objDTO.Description;
                    obj.LongDescription = objDTO.LongDescription;

                    //if (objDTO.CategoryID.GetValueOrDefault(0) > 0)
                    obj.CategoryID = objDTO.CategoryID;

                    //if (objDTO.GLAccountID.GetValueOrDefault(0) > 0)
                    obj.GLAccountID = objDTO.GLAccountID;

                    if (objDTO.UOMID > 0)
                        obj.UOMID = objDTO.UOMID;

                    //obj.PricePerTerm = objDTO.PricePerTerm;
                    obj.CostUOMID = objDTO.CostUOMID;
                    //obj.OrderUOMID = objDTO.OrderUOMID;
                    obj.OrderUOMID = objOUMDAL.GetOrderUomIdByCostUomId(Convert.ToInt64(objDTO.CostUOMID), objDTO);
                    obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                    obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                    obj.Cost = objDTO.Cost;
                    obj.Markup = objDTO.Markup;

                    if (obj.SellPrice != objDTO.SellPrice)
                    {
                        obj.pricesaveddate = DateTime.UtcNow;
                    }

                    obj.SellPrice = objDTO.SellPrice;

                    //obj.ExtendedCost = objDTO.ExtendedCost;
                    obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                    //obj.Link1 = objDTO.Link1;
                    if (!string.IsNullOrEmpty(objDTO.Link2) && !string.IsNullOrWhiteSpace(objDTO.Link2))
                    {
                        obj.Link2 = objDTO.Link2;
                    }
                    //obj.ItemType = objDTO.ItemType;
                    obj.Trend = objDTO.Trend;
                    obj.IsAutoInventoryClassification = objDTO.IsAutoInventoryClassification;
                    obj.Taxable = objDTO.Taxable;
                    obj.Consignment = objDTO.Consignment;
                    //obj.StagedQuantity = objDTO.StagedQuantity;
                    //obj.InTransitquantity = objDTO.InTransitquantity;
                    //obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                    //obj.OnOrderInTransitQuantity = objDTO.OnOrderInTransitQuantity;
                    //obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                    //obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                    //obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                    //obj.SuggestedTransferQuantity = objDTO.SuggestedTransferQuantity;
                    //obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                    //obj.RequisitionedQuantity = objReqDetailDAL.GetItemCurrentOnRequisitionQty(obj.GUID);
                    //obj.PackingQuantity = objDTO.PackingQuantity;
                    //obj.AverageUsage = objDTO.AverageUsage;
                    //obj.Turns = objDTO.Turns;
                    //obj.OnHandQuantity = objDTO.OnHandQuantity;
                    obj.IsActive = objDTO.IsActive;
                    obj.IsOrderable = objDTO.IsOrderable;
                    obj.CriticalQuantity = objDTO.CriticalQuantity ?? 0;
                    obj.MinimumQuantity = objDTO.MinimumQuantity ?? 0;
                    obj.MaximumQuantity = objDTO.MaximumQuantity ?? 0;
                    obj.WeightPerPiece = objDTO.WeightPerPiece;

                    //if (string.IsNullOrEmpty(obj.ItemUniqueNumber))
                    //{
                    //    if (!string.IsNullOrEmpty(objDTO.ItemUniqueNumber))
                    //        obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                    //    else
                    //    {
                    //        obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                    //    }
                    //}
                    //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                    obj.IsPurchase = objDTO.IsPurchase;
                    obj.IsTransfer = objDTO.IsTransfer;
                    obj.DefaultLocation = objDTO.DefaultLocation;
                    obj.InventoryClassification = objDTO.InventoryClassification;
                    //obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                    //obj.LotNumberTracking = objDTO.LotNumberTracking;
                    //obj.DateCodeTracking = objDTO.DateCodeTracking;

                    //if (!string.IsNullOrWhiteSpace(objDTO.ImageType))
                    //{
                    obj.ImageType = (string.IsNullOrEmpty(objDTO.ImageType) || string.IsNullOrWhiteSpace(objDTO.ImageType))
                                    ? "ExternalImage" : objDTO.ImageType;
                    //}
                    obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;

                    //if (!string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                    //{
                    obj.ItemLink2ImageType = (string.IsNullOrEmpty(objDTO.ItemLink2ImageType) || string.IsNullOrWhiteSpace(objDTO.ItemLink2ImageType))
                                             ? "InternalLink" : objDTO.ItemLink2ImageType;
                    //}

                    if (!string.IsNullOrEmpty(objDTO.ImagePath) && !string.IsNullOrWhiteSpace(objDTO.ImagePath))
                    {
                        obj.ImagePath = objDTO.ImagePath;
                    }

                    obj.UDF1 = objDTO.UDF1;
                    obj.UDF2 = objDTO.UDF2;
                    obj.UDF3 = objDTO.UDF3;
                    obj.UDF4 = objDTO.UDF4;
                    obj.UDF5 = objDTO.UDF5;
                    obj.UDF6 = objDTO.UDF6;
                    obj.UDF7 = objDTO.UDF7;
                    obj.UDF8 = objDTO.UDF8;
                    obj.UDF9 = objDTO.UDF9;
                    obj.UDF10 = objDTO.UDF10;
                    //obj.GUID = objDTO.GUID;
                    //obj.Created = objDTO.Created;
                    //obj.Updated = objDTO.Updated;

                    //if (IsFromItemPage)
                    //{
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    //}

                    //obj.CreatedBy = objDTO.CreatedBy;
                    //obj.IsDeleted = false;
                    //obj.IsArchived = false;
                    //obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                    //obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                    //obj.CompanyID = objDTO.CompanyID;
                    //obj.Room = objDTO.Room;
                    //obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                    //obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                    obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                    //obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                    // Get ext cost based on Item Location details START
                    CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                    //if (!obj.Consignment)
                    //{
                    //    obj.Cost = ObjCostDTO.Cost;
                    //}
                    //else
                    //{
                    //    obj.Cost = objDTO.Cost;
                    //}
                    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    var oRoom = objRoomDAL.GetRoomByIDPlain(obj.Room.GetValueOrDefault());
                    //Room oRoom = context.Rooms.Where(x => x.ID == obj.Room && x.CompanyID == obj.CompanyID).FirstOrDefault();
                    if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !obj.Consignment)
                    {
                        //obj.Cost = objDTO.Cost = OldItemCost;
                    }
                    else if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !obj.Consignment && objDTO.Cost != OldItemCost)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                    }
                    //else if (obj.Consignment && objDTO.Cost != OldItemCost)
                    //{
                    //    new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                    //}

                    obj.ExtendedCost = ObjCostDTO.ExtCost;
                    obj.AverageCost = ObjCostDTO.AvgCost;
                    obj.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                    objDTO.AverageCost = ObjCostDTO.AvgCost;
                    objDTO.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.Cost = obj.Cost;

                    //if ((obj.Markup ?? 0) > 0)
                    //{
                    //    obj.SellPrice = (obj.Cost ?? 0) + (((obj.Cost ?? 0) * (obj.Markup ?? 0)) / 100);
                    //}
                    //else
                    //{
                    //    obj.SellPrice = obj.Cost;
                    //}
                    objDTO.SellPrice = obj.SellPrice;
                    // Get ext cost based on Item Location details END
                    //obj.BondedInventory = objDTO.BondedInventory;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Edit From SVC";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    //obj.IsBOMItem = objDTO.IsBOMItem;
                    //obj.RefBomId = objDTO.RefBomId;
                    obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                    obj.TrendingSetting = objDTO.TrendingSetting;
                    obj.IsPackslipMandatoryAtReceive = objDTO.IsPackslipMandatoryAtReceive;
                    obj.ItemImageExternalURL = objDTO.ItemImageExternalURL;
                    obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                    //obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;
                    obj.ItemDocExternalURL = objDTO.ItemDocExternalURL;

                    //if (objDTO.IsOnlyFromItemUI) //Only Updated When Item Updated From Item Detail Page
                    //{
                    //    obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        obj.EditedFrom = objDTO.EditedFrom;
                    else
                        obj.EditedFrom = "SVC";
                    //}
                    //obj.QtyToMeetDemand = (objDTO.QtyToMeetDemand == null ? 0 : objDTO.QtyToMeetDemand.Value);
                    obj.ItemIsActiveDate = objDTO.ItemIsActiveDate;
                    //obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                    obj.WeightVariance = objDTO.WeightVariance;
                    obj.EnhancedDescription = objDTO.EnhancedDescription;
                    obj.POItemLineNumber = objDTO.POItemLineNumber;
                    //obj.ReceivedOnWeb = Convert.ToDateTime(objDTO.ReceivedOnWeb);
                    //obj.AddedFrom = objDTO.AddedFrom;
                    //context.ItemMasters.Attach(obj);

                    //context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                    context.SaveChanges();

                    new DashboardDAL(base.DataBaseName).SetItemsAutoClassification(obj.GUID, obj.Room ?? 0, obj.CompanyID ?? 0, obj.LastUpdatedBy ?? 0, 1);
                    //objDTO = FillWithExtraDetail(objDTO);    
                }



            }

            if (itemType > 0)
            {
                //new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), "web", "ItemMasterDAL >> EDIT");
                new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), "web", "Inventory >> Modified Item", SessionUserId);

                //objDTO.SuggestedOrderQuantity = GetSuggestedOrderQty(objDTO.GUID);
                //objDTO.SuggestedTransferQuantity = GetSuggestedTransferQty(objDTO.GUID);

                /* update QtyToMeetDemand */

                if (itemType == 3 && isBuildBreak == true)
                {
                    new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(itemGuid, (objDTO.LastUpdatedBy ?? 0), SessionUserId);
                }
            }


            return true;
        }

        public bool EditItemOnHandAndDefaultLocationSVC(ItemMasterDTO objDTO, long SessionUserId)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            RequisitionDetailsDAL objReqDetailDAL = new RequisitionDetailsDAL(DataBaseName);
            OrderUOMMasterDAL objOUMDAL = new OrderUOMMasterDAL(DataBaseName);
            int itemType = 0;
            Guid itemGuid = Guid.Empty;
            bool isBuildBreak = false;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = context.ItemMasters.Where(x => x.GUID == objDTO.GUID).FirstOrDefault();

                if (obj != null)
                {
                    double? OldItemCost = obj.Cost;

                    itemType = obj.ItemType;
                    itemGuid = obj.GUID;
                    isBuildBreak = obj.IsBuildBreak.GetValueOrDefault(false);
                    obj.DefaultLocation = objDTO.DefaultLocation;
                    obj.Updated = objDTO.Updated;
                    obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                    // Get ext cost based on Item Location details START
                    CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                    RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                    var oRoom = objRoomDAL.GetRoomByIDPlain(obj.Room.GetValueOrDefault());

                    if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !obj.Consignment)
                    {
                        //obj.Cost = objDTO.Cost = OldItemCost;
                    }
                    else if (oRoom != null && oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !obj.Consignment && objDTO.Cost != OldItemCost)
                    {
                        new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                    }

                    obj.ExtendedCost = ObjCostDTO.ExtCost;
                    obj.AverageCost = ObjCostDTO.AvgCost;
                    obj.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                    objDTO.AverageCost = ObjCostDTO.AvgCost;
                    objDTO.PerItemCost = ObjCostDTO.PerItemCost;

                    objDTO.Cost = obj.Cost;
                    objDTO.SellPrice = obj.SellPrice;
                    // Get ext cost based on Item Location details END
                    //obj.BondedInventory = objDTO.BondedInventory;

                    if (string.IsNullOrEmpty(objDTO.WhatWhereAction))
                        objDTO.WhatWhereAction = "Edit From SVC";

                    obj.WhatWhereAction = objDTO.WhatWhereAction;

                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        obj.EditedFrom = objDTO.EditedFrom;
                    else
                        obj.EditedFrom = "SVC";

                    context.SaveChanges();
                    new DashboardDAL(base.DataBaseName).SetItemsAutoClassification(obj.GUID, obj.Room ?? 0, obj.CompanyID ?? 0, obj.LastUpdatedBy ?? 0, 1);
                }

            }

            new CartItemDAL(base.DataBaseName).AutoCartUpdateByCode(objDTO.GUID, (objDTO.LastUpdatedBy ?? 0), "web", "Inventory >> Modified Item", SessionUserId);
            /* update QtyToMeetDemand */
            if (itemType == 3 && isBuildBreak == true)
            {
                new KitDetailDAL(base.DataBaseName).UpdateQtyToMeedDemand(itemGuid, (objDTO.LastUpdatedBy ?? 0), SessionUserId);
            }
            /* update QtyToMeetDemand */

            return true;
        }

        public void UpdateCostMarkupSellPrice(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, long SessionUserId, long EnterpriseId, string FromWhere = "")
        {
            ItemMasterDAL objItem = new ItemMasterDAL(base.DataBaseName);
            ItemMasterDTO ItemDTO = objItem.GetItemWithoutJoins(null, ItemGUID);
            CostDTO ObjCostDTO = GetExtCostAndAvgCost(ItemGUID, RoomID, CompanyID, ItemDTO.WhatWhereAction);
            if (ObjCostDTO != null)
            {
                ItemDTO.Cost = ObjCostDTO.Cost;
                ItemDTO.SellPrice = ObjCostDTO.SellPrice;
                ItemDTO.Markup = ObjCostDTO.Markup;

                ItemDTO.PerItemCost = ObjCostDTO.PerItemCost;

                if (!string.IsNullOrEmpty(FromWhere))
                    ItemDTO.WhatWhereAction = FromWhere;
                objItem.Edit(ItemDTO, SessionUserId, EnterpriseId);
            }
        }

        public bool EditMultiple(ItemMasterDTO objDTO)
        {
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            RequisitionDetailsDAL objReqDetailDAL = new RequisitionDetailsDAL(DataBaseName);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //ItemMaster obj = new ItemMaster();
                ItemMaster obj = context.ItemMasters.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                double? OldItemCost = obj.Cost;
                //obj.ID = objDTO.ID;
                obj.ItemNumber = objDTO.ItemNumber;
                obj.ManufacturerID = objDTO.ManufacturerID;
                obj.ManufacturerNumber = objDTO.ManufacturerNumber;
                obj.SupplierID = objDTO.SupplierID;
                obj.SupplierPartNo = objDTO.SupplierPartNo;
                obj.UPC = objDTO.UPC;
                obj.UNSPSC = objDTO.UNSPSC;
                obj.Description = objDTO.Description;
                obj.LongDescription = objDTO.LongDescription;
                obj.CategoryID = objDTO.CategoryID;
                obj.GLAccountID = objDTO.GLAccountID;
                obj.UOMID = objDTO.UOMID;
                obj.PricePerTerm = objDTO.PricePerTerm;
                obj.CostUOMID = objDTO.CostUOMID;
                obj.DefaultReorderQuantity = (objDTO.DefaultReorderQuantity == null ? 0 : objDTO.DefaultReorderQuantity.Value);
                obj.DefaultPullQuantity = (objDTO.DefaultPullQuantity == null ? 0 : objDTO.DefaultPullQuantity.Value);
                obj.Cost = objDTO.Cost;
                obj.Markup = objDTO.Markup;
                obj.SellPrice = objDTO.SellPrice;
                //obj.ExtendedCost = objDTO.ExtendedCost;
                obj.LeadTimeInDays = objDTO.LeadTimeInDays;
                obj.Link1 = objDTO.Link1;
                obj.Link2 = objDTO.Link2;
                obj.ItemLink2ExternalURL = objDTO.ItemLink2ExternalURL;
                obj.ItemLink2ImageType = objDTO.ItemLink2ImageType;

                obj.Trend = objDTO.Trend;
                obj.Taxable = objDTO.Taxable;
                obj.IsAutoInventoryClassification = objDTO.IsAutoInventoryClassification;
                obj.Consignment = objDTO.Consignment;
                obj.StagedQuantity = objDTO.StagedQuantity;
                obj.InTransitquantity = objDTO.InTransitquantity;
                obj.OnOrderQuantity = objDTO.OnOrderQuantity;
                obj.OnOrderInTransitQuantity = objDTO.OnOrderInTransitQuantity;
                obj.OnReturnQuantity = objDTO.OnReturnQuantity;
                obj.OnTransferQuantity = objDTO.OnTransferQuantity;
                obj.SuggestedOrderQuantity = objDTO.SuggestedOrderQuantity;
                obj.SuggestedTransferQuantity = objDTO.SuggestedTransferQuantity;
                //obj.RequisitionedQuantity = objDTO.RequisitionedQuantity;
                obj.RequisitionedQuantity = objReqDetailDAL.GetItemCurrentOnRequisitionQty(obj.GUID);
                obj.PackingQuantity = objDTO.PackingQuantity;
                obj.AverageUsage = objDTO.AverageUsage;
                obj.Turns = objDTO.Turns;
                obj.OnHandQuantity = objDTO.OnHandQuantity;
                obj.CriticalQuantity = objDTO.CriticalQuantity ?? 0;
                obj.MinimumQuantity = objDTO.MinimumQuantity ?? 0;
                obj.MaximumQuantity = objDTO.MaximumQuantity ?? 0;
                obj.WeightPerPiece = objDTO.WeightPerPiece;
                //obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                if (!string.IsNullOrEmpty(objDTO.ItemUniqueNumber))
                    obj.ItemUniqueNumber = objDTO.ItemUniqueNumber;
                else
                {
                    //   obj.ItemUniqueNumber = objCommonDAL.UniqueItemId();
                    obj.ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0));
                }

                //obj.TransferOrPurchase = objDTO.TransferOrPurchase;
                obj.IsPurchase = objDTO.IsPurchase;
                obj.IsTransfer = objDTO.IsTransfer;
                obj.DefaultLocation = objDTO.DefaultLocation;
                obj.InventoryClassification = objDTO.InventoryClassification;
                obj.SerialNumberTracking = objDTO.SerialNumberTracking;
                obj.LotNumberTracking = objDTO.LotNumberTracking;
                obj.DateCodeTracking = objDTO.DateCodeTracking;
                obj.ItemType = objDTO.ItemType;
                obj.ImagePath = objDTO.ImagePath;
                obj.UDF1 = objDTO.UDF1;
                obj.UDF2 = objDTO.UDF2;
                obj.UDF3 = objDTO.UDF3;
                obj.UDF4 = objDTO.UDF4;
                obj.UDF5 = objDTO.UDF5;
                obj.UDF6 = objDTO.UDF6;
                obj.UDF7 = objDTO.UDF7;
                obj.UDF8 = objDTO.UDF8;
                obj.UDF9 = objDTO.UDF9;
                obj.UDF10 = objDTO.UDF10;
                obj.GUID = objDTO.GUID;

                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                //obj.IsDeleted = false;
                //obj.IsArchived = false;
                obj.IsDeleted = objDTO.IsDeleted.HasValue ? (bool)objDTO.IsDeleted : false;
                obj.IsArchived = objDTO.IsArchived.HasValue ? (bool)objDTO.IsArchived : false;
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                obj.IsLotSerialExpiryCost = objDTO.IsLotSerialExpiryCost;
                obj.IsItemLevelMinMaxQtyRequired = (objDTO.IsItemLevelMinMaxQtyRequired.HasValue ? objDTO.IsItemLevelMinMaxQtyRequired : false);
                obj.IsEnforceDefaultReorderQuantity = (objDTO.IsEnforceDefaultReorderQuantity.HasValue ? objDTO.IsEnforceDefaultReorderQuantity : false);
                obj.IsBuildBreak = (objDTO.IsBuildBreak.HasValue ? objDTO.IsBuildBreak : false);
                // Get ext cost based on Item Location details START
                CostDTO ObjCostDTO = GetExtCostAndAvgCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.WhatWhereAction);
                obj.ExtendedCost = ObjCostDTO.ExtCost;
                obj.AverageCost = ObjCostDTO.AvgCost;
                obj.PerItemCost = ObjCostDTO.PerItemCost;

                objDTO.ExtendedCost = ObjCostDTO.ExtCost;
                objDTO.AverageCost = ObjCostDTO.AvgCost;
                objDTO.PerItemCost = ObjCostDTO.PerItemCost;

                obj.PullQtyScanOverride = objDTO.PullQtyScanOverride;
                obj.TrendingSetting = objDTO.TrendingSetting;

                //Room oRoom = context.Rooms.Where(x => x.ID == obj.Room && x.CompanyID == obj.CompanyID).FirstOrDefault();
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                var oRoom = objRoomDAL.GetRoomByIDPlain(obj.Room.GetValueOrDefault());

                if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString() && !obj.Consignment)
                {
                    obj.Cost = objDTO.Cost = OldItemCost;
                }
                else if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !obj.Consignment
                    && objDTO.Cost != OldItemCost)
                {
                    //new ItemMasterDAL(base.DataBaseName).UpdatePastReceiptCostByItemCost(objDTO.GUID, objDTO.Room.GetValueOrDefault(0), objDTO.CompanyID.GetValueOrDefault(0), objDTO.Cost.GetValueOrDefault(0));
                }

                if ((obj.Markup ?? 0) > 0)
                    obj.SellPrice = (obj.Cost ?? 0) + (((obj.Cost ?? 0) * (obj.Markup ?? 0)) / 100);
                else
                    obj.SellPrice = obj.Cost;
                objDTO.SellPrice = obj.SellPrice;
                obj.IsActive = obj.IsActive;
                obj.IsOrderable = obj.IsOrderable;
                // Get ext cost based on Item Location details END
                obj.BondedInventory = objDTO.BondedInventory;
                if (objDTO.IsOnlyFromItemUI) //Only Updated When Item Updated From Item Detail Page
                {
                    obj.ReceivedOn = (objDTO.ReceivedOn.HasValue ? Convert.ToDateTime(objDTO.ReceivedOn) : DateTimeUtility.DateTimeNow);
                    if (!string.IsNullOrEmpty(objDTO.EditedFrom))
                        obj.EditedFrom = objDTO.EditedFrom;
                    else
                        obj.EditedFrom = "Web";
                }
                obj.IsAllowOrderCostuom = objDTO.IsAllowOrderCostuom;
                obj.OnQuotedQuantity = objDTO.OnQuotedQuantity ?? 0;
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                objDTO = FillWithExtraDetail(objDTO);

                return true;
            }
        }

        public double GetSuggestedOrderQty(Guid ItemGUIDId)
        {
            double TotalSuggestedQty = new CartItemDAL(base.DataBaseName).GetSuggestedQtyByReplenishType(ItemGUIDId, 0, 0, "Purchase");
            return TotalSuggestedQty;
        }

        public double GetSuggestedTransferQty(Guid ItemGUIDId)
        {
            double TotalSuggestedQty = new CartItemDAL(base.DataBaseName).GetSuggestedQtyByReplenishType(ItemGUIDId, 0, 0, "Transfer");
            return TotalSuggestedQty;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsNew(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, string callFrom = "")
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemLocations = null;
            string InventoryClassification = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string OnHandQuantity = null;
            string ItemTrackingType = null;
            string IsActive = null;
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, null, null, null, null, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                {
                    ItemLocations = FieldsPara[55].TrimEnd(',');
                    //if (callFrom == "ItemMaster")
                    //{
                    //    HttpContext.Current.Session["NSItemLocation"] = ItemLocations;
                    //}
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[87]))
                {
                    InventoryClassification = FieldsPara[87].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[89]))
                {
                    //if (FieldsPara[89] == "1")
                    //{
                    //    OnHandQuantity = ("Out of Stock");
                    //}
                    //else if (FieldsPara[89] == "2")
                    //{
                    //    OnHandQuantity = ("Below Critical");
                    //}
                    //else if (FieldsPara[89] == "3")
                    //{
                    //    OnHandQuantity = ("Below Min");
                    //}
                    //else if (FieldsPara[89] == "4")
                    //{
                    //    OnHandQuantity = ("Above Max");
                    //}

                    OnHandQuantity = FieldsPara[89].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    string[] arrReplenishTypes = FieldsPara[92].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    string[] arrReplenishTypes = FieldsPara[93].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    string[] arrReplenishTypes = FieldsPara[94].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    string[] arrReplenishTypes = FieldsPara[95].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    string[] arrReplenishTypes = FieldsPara[96].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[48].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[49].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[97]))
                {
                    IsActive = FieldsPara[97].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[88]))
                {
                    ItemTrackingType = FieldsPara[88].TrimEnd(',');
                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, ItemLocations, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = (row.Field<bool?>("Trend").HasValue ? row.Field<bool>("Trend") : false),// row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = (row.Field<bool?>("IsAutoInventoryClassification").HasValue ? row.Field<bool>("IsAutoInventoryClassification") : false),// row.Field<bool>("Trend"),
                        Taxable = (row.Field<bool?>("Taxable").HasValue ? row.Field<bool>("Taxable") : false), //row.Field<bool>("Taxable"),
                        Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),//   row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = (row.Field<bool?>("IsPurchase").HasValue ? row.Field<bool>("IsPurchase") : false), //row.Field<bool>("IsPurchase"),
                        IsTransfer = (row.Field<bool?>("IsTransfer").HasValue ? row.Field<bool>("IsTransfer") : false), //row.Field<bool>("IsTransfer"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = (row.Field<bool?>("SerialNumberTracking").HasValue ? row.Field<bool>("SerialNumberTracking") : false),
                        LotNumberTracking = (row.Field<bool?>("LotNumberTracking").HasValue ? row.Field<bool>("LotNumberTracking") : false),
                        DateCodeTracking = (row.Field<bool?>("DateCodeTracking").HasValue ? row.Field<bool>("DateCodeTracking") : false),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        //for item grid display purpose - CART, PUll  
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),

                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = (row.Field<bool?>("IsBuildBreak").HasValue ? row.Field<bool>("IsBuildBreak") : false),
                        // row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = (row.Field<bool?>("IsBOMItem").HasValue ? row.Field<bool>("IsBOMItem") : false),
                        //row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = (row.Field<bool?>("PullQtyScanOverride").HasValue ? row.Field<bool>("PullQtyScanOverride") : false),
                        // row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        IsPackslipMandatoryAtReceive = row.Field<bool>("IsPackslipMandatoryAtReceive"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        BPONumber = row.Field<string>("BPONumber"),
                        TrasnferedDate = row.Field<DateTime?>("trasnfereddate"),
                        CountedDate = row.Field<DateTime?>("counteddate"),
                        OrderedDate = row.Field<DateTime?>("ordereddate"),
                        PulledDate = row.Field<DateTime?>("pulleddate"),
                        PriceSavedDate = row.Field<DateTime?>("PriceSavedDate"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        ItemDocExternalURL = row.Field<string>("ItemDocExternalURL"),
                        ImageType = row.Field<string>("ImageType"),
                        QtyToMeetDemand = row.Field<double?>("QtyToMeetDemand"),
                        DefaultLocationGUID = row.Field<Guid?>("DefaultLocationGUID"),
                        OutTransferQuantity = row.Field<double?>("OutTransferQuantity"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity"),
                        eLabelKey = row.Field<string>("eLabelKey"),
                        EnrichedProductData = row.Field<string>("EnrichedProductData"),
                        EnhancedDescription = row.Field<string>("EnhancedDescription")
                    }).ToList();
                }
            }
            return lstItems;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsNew_ChnageLog(Guid ItemGuid, Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                    //  Cost = FieldsPara[15].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[48].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[49].Contains("10_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[49].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_ChangeLog", ItemGuid, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        HistoryID = row.Field<long>("HistoryID"),
                        ID = row.Field<long>("ID"),
                        Action = row.Field<string>("Action"),
                        WhatWhereAction = row.Field<string>("WhatWhereAction"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = (row.Field<double?>("PricePerTerm").HasValue ? row.Field<double?>("PricePerTerm") : 0),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        //Cost = row.Field<double?>("Cost"),
                        //Markup = row.Field<double?>("Markup"),
                        //SellPrice = row.Field<double?>("SellPrice"),
                        //ExtendedCost = row.Field<double?>("ExtendedCost"),
                        //AverageCost = row.Field<double?>("AverageCost"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        Cost = (row.Field<double?>("Cost").HasValue ? row.Field<double?>("Cost") : 0),
                        Markup = (row.Field<double?>("Markup").HasValue ? row.Field<double?>("Markup") : 0),
                        SellPrice = (row.Field<double?>("SellPrice").HasValue ? row.Field<double?>("SellPrice") : 0),
                        ExtendedCost = (row.Field<double?>("ExtendedCost").HasValue ? row.Field<double?>("ExtendedCost") : 0),
                        AverageCost = (row.Field<double?>("AverageCost").HasValue ? row.Field<double?>("AverageCost") : 0),
                        PerItemCost = (row.Field<double?>("PerItemCost").HasValue ? row.Field<double?>("PerItemCost") : 0),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = (row.Field<bool?>("Trend").HasValue ? row.Field<bool>("Trend") : false),// row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = (row.Field<bool?>("IsAutoInventoryClassification").HasValue ? row.Field<bool>("IsAutoInventoryClassification") : false),// row.Field<bool>("Trend"),
                        Taxable = (row.Field<bool?>("Taxable").HasValue ? row.Field<bool>("Taxable") : false), //row.Field<bool>("Taxable"),
                        Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),//   row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        // InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = (row.Field<bool?>("IsPurchase").HasValue ? row.Field<bool>("IsPurchase") : false), //row.Field<bool>("IsPurchase"),
                        IsTransfer = (row.Field<bool?>("IsTransfer").HasValue ? row.Field<bool>("IsTransfer") : false), //row.Field<bool>("IsTransfer"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = (row.Field<bool?>("SerialNumberTracking").HasValue ? row.Field<bool>("SerialNumberTracking") : false),
                        LotNumberTracking = (row.Field<bool?>("LotNumberTracking").HasValue ? row.Field<bool>("LotNumberTracking") : false),
                        DateCodeTracking = (row.Field<bool?>("DateCodeTracking").HasValue ? row.Field<bool>("DateCodeTracking") : false),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = (row.Field<bool?>("IsBuildBreak").HasValue ? row.Field<bool>("IsBuildBreak") : false),
                        // row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = (row.Field<bool?>("IsBOMItem").HasValue ? row.Field<bool>("IsBOMItem") : false),
                        //row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = (row.Field<bool?>("PullQtyScanOverride").HasValue ? row.Field<bool>("PullQtyScanOverride") : false),
                        // row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        IsPackslipMandatoryAtReceive = row.Field<bool>("IsPackslipMandatoryAtReceive"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        QtyToMeetDemand = row.Field<double?>("QtyToMeetDemand"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        HistoryOn = row.Field<DateTime?>("HistoryDate").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("HistoryDate")) : row.Field<DateTime?>("HistoryDate"),
                        OrderedDate = row.Field<DateTime?>("OrderedDate").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("OrderedDate")) : row.Field<DateTime?>("OrderedDate"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity"),
                    }).ToList();
                }
            }
            return lstItems;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, string CalledFor, List<long> SupplierIds, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, Int32? QuickListTypeValue = null)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string OnHandQuantity = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;

            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            if (sortColumnName.Equals("null desc") || sortColumnName.Equals("null asc"))
            {
                sortColumnName = string.Empty;
            }
            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, CalledFor, null, null, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, QuickListTypeValue);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }


                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    if (FieldsPara[21] == "1")
                    {
                        OnHandQuantity = ("Out of Stock");
                    }
                    else if (FieldsPara[21] == "2")
                    {
                        OnHandQuantity = ("Below Critical");
                    }
                    else if (FieldsPara[21] == "3")
                    {
                        OnHandQuantity = ("Below Min");
                    }
                    else if (FieldsPara[21] == "4")
                    {
                        OnHandQuantity = ("Above Max");
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }

                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, CalledFor, null, OnHandQuantity, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, QuickListTypeValue);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, CalledFor, null, OnHandQuantity, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, QuickListTypeValue);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = row.Field<bool?>("Trend") ?? false,
                        IsAutoInventoryClassification = row.Field<bool?>("IsAutoInventoryClassification") ?? false,
                        Taxable = row.Field<bool?>("Taxable") ?? false,
                        Consignment = row.Field<bool?>("Consignment") ?? false,
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool?>("SerialNumberTracking") ?? false,
                        LotNumberTracking = row.Field<bool?>("LotNumberTracking") ?? false,
                        DateCodeTracking = row.Field<bool?>("DateCodeTracking") ?? false,
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        ImageType = row.Field<string>("ImageType"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        //Created = row.Field<DateTime?>("Created"),
                        //Updated = row.Field<DateTime?>("Updated"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool?>("IsBOMItem") ?? false,
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool?>("PullQtyScanOverride") ?? false,
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity"),
                    }).ToList();
                }
            }
            return lstItems;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, bool ItemsHaveQuantityOnly, List<long> SupplierIds)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string OnHandQuantity = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, null, ItemsHaveQuantityOnly, OnHandQuantity, null, null, null, null, null, null, null, null, null, null);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                    //  Cost = FieldsPara[15].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                    UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    if (FieldsPara[21] == "1")
                    {
                        OnHandQuantity = ("Out of Stock");
                    }
                    else if (FieldsPara[21] == "2")
                    {
                        OnHandQuantity = ("Below Critical");
                    }
                    else if (FieldsPara[21] == "3")
                    {
                        OnHandQuantity = ("Below Min");
                    }
                    else if (FieldsPara[21] == "4")
                    {
                        OnHandQuantity = ("Above Max");
                    }
                }

                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, null, ItemsHaveQuantityOnly, OnHandQuantity, null, null, null, null, null, null, null, null, null, null);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemsForModel", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemGUIDs, null, ItemsHaveQuantityOnly, OnHandQuantity, null, null, null, null, null, null, null, null, null, null);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        Trend = row.Field<bool?>("Trend") ?? false,
                        IsAutoInventoryClassification = row.Field<bool?>("IsAutoInventoryClassification") ?? false,
                        Taxable = row.Field<bool?>("Taxable") ?? false,
                        Consignment = row.Field<bool?>("Consignment") ?? false,
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool?>("SerialNumberTracking") ?? false,
                        LotNumberTracking = row.Field<bool?>("LotNumberTracking") ?? false,
                        DateCodeTracking = row.Field<bool?>("DateCodeTracking") ?? false,
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        //Created = row.Field<DateTime?>("Created"),
                        // Updated = row.Field<DateTime?>("Updated"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),

                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool?>("IsBOMItem") ?? false,
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool?>("PullQtyScanOverride") ?? false,
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")
                    }).ToList();
                }
            }
            return lstItems;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string ItemGUIDs, string ItemType, string QuickListType, List<long> SupplierIds, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, string ActionType = "", bool isQuickListRequired = true, bool IsAllowConsignedCredit = true, bool IsAllowConsigeItem = true, OrderType? OrdType = OrderType.Order, Int64? StagHeaderID = null, MoveType? moveType = null)
        {
            string DateFormat = "MM/dd/yyyy";

            //Get Cached-Media
            IEnumerable<ItemMasterDTO> ObjCache;
            if (ItemType == "item")
                ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null).Where(x => x.ItemType != 4 && x.IsActive == true);
            else if (ItemType == "kit")
                ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null).Where(x => x.ItemType != 3 && x.IsActive == true && x.IsOrderable == true);
            else if (ItemType == "newcart")
                ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null).Where(x => x.ItemType != 4 && x.IsActive == true);
            else
                ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null).Where(x => x.IsActive == true);

            if (ActionType == "Pull")
            {
                if (!String.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {
                    string[] stringSeparators12 = new string[] { "[###]" };
                    string[] Fields12 = SearchTerm.Split(stringSeparators12, StringSplitOptions.None);
                    if (!Fields12[1].Split('@')[22].Split(',').ToList().Contains("4"))
                    {
                        ObjCache = ObjCache.Where(x => x.OnHandQuantity.GetValueOrDefault(0) > 0 || x.StagedQuantity.GetValueOrDefault(0) > 0);
                    }
                }
                else
                {
                    ObjCache = ObjCache.Where(x => x.OnHandQuantity.GetValueOrDefault(0) > 0 || x.StagedQuantity.GetValueOrDefault(0) > 0);
                }
            }

            if (isQuickListRequired == false && ActionType != "MoveMTR")
            {
                ObjCache = GetAllItemsWithJoins(RoomID, CompanyID, IsArchived, IsDeleted, null).Where(x => x.ItemType != 2);
            }

            if (ActionType == "Credit")
            {
                PullMasterDAL pullMasterDAL = new PullMasterDAL(base.DataBaseName);
                var arrItemGuids = pullMasterDAL.GetItemGuidsByPullActionType(RoomID, CompanyID, "pull");
                ObjCache = ObjCache.Where(x => arrItemGuids.Contains(x.GUID));
            }

            if (!IsAllowConsignedCredit && ActionType == "Credit")
            {
                ObjCache = ObjCache.Where(x => x.Consignment == false);
            }

            if (!string.IsNullOrEmpty(ItemGUIDs))
            {
                string[] arrGUIDs = ItemGUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<ItemMasterDTO> obj = (from x in ObjCache
                                                  where !arrGUIDs.Contains(x.GUID.ToString())
                                                  select x).AsEnumerable();
                ObjCache = obj;
            }

            if (IsAllowConsigeItem == false)
            {
                ObjCache = ObjCache.Where(x => x.Consignment == false);
            }

            if (isQuickListRequired)
            {
                // QuickList Data Binding for Item PopUP /////////////////////////////////START
                QuickListDAL objQLDAL = new QuickListDAL(base.DataBaseName);
                List<QuickListMasterDTO> QuickListDATA = objQLDAL.GetAllRecords(RoomID, CompanyID, IsArchived, IsDeleted, QuickListType).ToList();
                List<ItemMasterDTO> TempItemQLData = new List<ItemMasterDTO>();

                foreach (QuickListMasterDTO item in QuickListDATA)
                {
                    if (objQLDAL.GetQuickListItemsRecords(RoomID, CompanyID, item.GUID.ToString(), SupplierIds).Sum(x => x.Quantity) > 0)
                    {
                        ItemMasterDTO tempItem = new ItemMasterDTO();
                        tempItem.QuickListGUID = item.GUID.ToString();
                        tempItem.QuickListName = item.Name;
                        tempItem.ItemNumber = item.Name;
                        tempItem.ItemType = 2;
                        tempItem.GUID = item.GUID;
                        //Can not convert int to long
                        tempItem.SupplierID = 0;//SupplierID;
                        tempItem.OnHandQuantity = 0;
                        tempItem.CriticalQuantity = 0;
                        tempItem.MinimumQuantity = 0;
                        tempItem.MaximumQuantity = 0;
                        tempItem.Created = item.Created;
                        tempItem.Updated = item.LastUpdated;
                        tempItem.CreatedByName = item.CreatedByName;
                        tempItem.UpdatedByName = item.UpdatedByName;
                        tempItem.RoomName = item.RoomName;
                        tempItem.ID = item.ID;
                        TempItemQLData.Add(tempItem);
                    }
                }
                if (TempItemQLData.Count > 0)
                    ObjCache = ObjCache.Concat(TempItemQLData.AsEnumerable());
            }


            //////////////////////// Filter for Stage Location Selection ////////////////START
            if (!String.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators12 = new string[] { "[###]" };
                string[] Fields13 = SearchTerm.Split(stringSeparators12, StringSplitOptions.None);
                if (Fields13[0].Split(',')[26].Split(',').ToList().Contains("MSID") && Fields13[1].Split('@')[26].Split(',').ToList()[0] != "")
                {
                    Guid tempMSGUID = Guid.Parse(Fields13[1].Split('@')[26].Split(',').ToList()[0]);
                    MaterialStagingDetailDAL objMSDtlDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    string[] arrGUIDs = Fields13[1].Split('@')[26].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //var TempMSDtlData = objMSDtlDAL.GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted).ToList();
                    var TempMSDtlData = objMSDtlDAL.GetMSDetailByRoomCompanyItemGUID(RoomID, CompanyID, string.Empty, IsArchived, IsDeleted, false).ToList();
                    var MSDtlData = (from x in TempMSDtlData
                                     where arrGUIDs.Contains(x.MaterialStagingGUID.ToString())
                                     select x).ToList();

                    ObjCache = (from x in ObjCache
                                join y in MSDtlData on x.GUID equals y.ItemGUID
                                select x).Distinct().ToList();
                }
            }
            //////////////////////// Filter for Stage Location Selection ////////////////END

            if (SupplierIds != null && SupplierIds.Any())
            {
                ObjCache = ObjCache.Where(x => SupplierIds.Contains(x.SupplierID.GetValueOrDefault(0)) || x.ItemType == 2);
            }

            if (OrdType != null && OrdType == OrderType.RuturnOrder)
            {
                if (StagHeaderID.GetValueOrDefault(0) > 0)
                {
                    ObjCache = ObjCache.Where(x => x.StagedQuantity.GetValueOrDefault(0) > 0);
                }
                else
                {
                    ObjCache = ObjCache.Where(x => x.OnHandQuantity.GetValueOrDefault(0) > 0);
                }
            }

            if (moveType != null)
            {
                if (moveType == MoveType.InvToInv || moveType == MoveType.InvToStag)
                {
                    ObjCache = ObjCache.Where(x => x.OnHandQuantity.GetValueOrDefault(0) > 0);
                }
                else if (moveType == MoveType.StagToInv || moveType == MoveType.StagToStag)
                {
                    ObjCache = ObjCache.Where(x => x.StagedQuantity.GetValueOrDefault(0) > 0);
                }
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

                if (Fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(Fields[2]))
                    {
                        SearchTerm = Fields[2];
                    }
                    else
                    {
                        SearchTerm = string.Empty;
                    }
                }
                else
                {
                    SearchTerm = string.Empty;
                }

                ObjCache = ObjCache.Where(t =>
                              ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains((t.CreatedBy ?? 0).ToString())))
                           && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains((t.LastUpdatedBy ?? 0).ToString())))
                           && ((Fields[1].Split('@')[2] == "") || (t.Created.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Created.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                           && ((Fields[1].Split('@')[3] == "") || (t.Updated.Value >= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone) && t.Updated.Value <= TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(Fields[1].Split('@')[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone)))
                           && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                           && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                           && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                           && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                           && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                           && ((Fields[1].Split('@')[9] == "") || (Fields[1].Split('@')[9].Split(',').ToList().Contains(t.SupplierID.ToString())))
                           && ((Fields[1].Split('@')[10] == "") || (Fields[1].Split('@')[10].Split(',').ToList().Contains(t.ManufacturerID.ToString())))
                           && ((Fields[1].Split('@')[11] == "") || (Fields[1].Split('@')[11].Split(',').ToList().Contains(t.CategoryID.ToString())))
                            //&& ((Fields[1].Split('@')[15] == "") || ((t.Cost == null ? 0 : t.Cost) >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0])))
                            && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 1 ? t.OnHandQuantity.GetValueOrDefault(0) <= 0 && t.ItemType != 4 : true))
                            && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 2 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.CriticalQuantity : true))
                            && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 3 ? t.OnHandQuantity.GetValueOrDefault(0) <= t.MinimumQuantity : true))
                            && ((Fields[1].Split('@')[21] == "") || (Convert.ToInt32(Fields[1].Split('@')[21].Split(',')[0]) == 4 ? t.OnHandQuantity.GetValueOrDefault(0) >= t.MaximumQuantity : true))
                           && ((Fields[1].Split('@')[22] == "") || (Fields[1].Split('@')[22].Split(',').ToList().Contains(t.ItemType.ToString())))
                              && ((Fields[1].Split('@')[4] == "") || (Fields[1].Split('@')[4].Split(',').ToList().Contains(t.UDF1)))
                           && ((Fields[1].Split('@')[5] == "") || (Fields[1].Split('@')[5].Split(',').ToList().Contains(t.UDF2)))
                           && ((Fields[1].Split('@')[6] == "") || (Fields[1].Split('@')[6].Split(',').ToList().Contains(t.UDF3)))
                           && ((Fields[1].Split('@')[7] == "") || (Fields[1].Split('@')[7].Split(',').ToList().Contains(t.UDF4)))
                           && ((Fields[1].Split('@')[8] == "") || (Fields[1].Split('@')[8].Split(',').ToList().Contains(t.UDF5)))
                           && ((Fields[1].Split('@')[45] == "") || (Fields[1].Split('@')[45].Split(',').ToList().Contains(t.UDF6)))
                           && ((Fields[1].Split('@')[46] == "") || (Fields[1].Split('@')[46].Split(',').ToList().Contains(t.UDF7)))
                           && ((Fields[1].Split('@')[47] == "") || (Fields[1].Split('@')[47].Split(',').ToList().Contains(t.UDF8)))
                           && ((Fields[1].Split('@')[48] == "") || (Fields[1].Split('@')[48].Split(',').ToList().Contains(t.UDF9)))
                           && ((Fields[1].Split('@')[49] == "") || (Fields[1].Split('@')[49].Split(',').ToList().Contains(t.UDF10)))

                           && (SearchTerm == "" || (t.ID.ToString().Contains(SearchTerm) ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderInTransitQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.AverageCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.AverageUsage.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BinNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BondedInventory) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.CategoryName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Cost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultLocationName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultPullQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultReorderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ExtendedCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.InTransitquantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemTypeName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemUniqueNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.LeadTimeInDays) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link1) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link2) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Markup.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MaximumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MinimumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MonthValue) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnHandQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PackingQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PricePerTerm.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.QuickListName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.RequisitionedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SellPrice.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.StagedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Turns) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UNSPSC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UPC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.WeightPerPiece) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Unit) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Created.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Updated.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Trend ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsAutoInventoryClassification ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Taxable ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Consignment ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false) ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsTransfer ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsPurchase ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.InventoryClassification.GetValueOrDefault(0) == 0 ? "No" : "Yes").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SerialNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LotNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.DateCodeTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsLotSerialExpiryCost ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ImagePath ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AddedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.EditedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0) ||
                        (t.IsAllowOrderCostuom ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||

                        (t.UDF6 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF7 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF8 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF9 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF10 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                        )
                           );

                if (!string.IsNullOrEmpty(Fields[1].Split('@')[15]))
                {
                    if (Fields[1].Split('@')[15].Contains("100_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                              ((Fields[1].Split('@')[15] == "") || ((t.Cost == null ? 0 : t.Cost) >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]))));
                    }
                    else if (Fields[1].Split('@')[15].Contains("10_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[15] == "") || ((t.Cost == null ? 0 : t.Cost) <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]))));
                    }
                    else
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[15] == "") || ((t.Cost == null ? 0 : t.Cost) >= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[0]) && (t.Cost == null ? 0 : t.Cost) <= Convert.ToDouble(Fields[1].Split('@')[15].Split('_')[1]))));
                    }
                }
                if (!string.IsNullOrEmpty(Fields[1].Split('@')[31]))
                {
                    if (Fields[1].Split('@')[31].Contains("100_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                              ((Fields[1].Split('@')[31] == "") || ((t.AverageUsage == null ? 0 : t.AverageUsage) >= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0]))));
                    }
                    else if (Fields[1].Split('@')[31].Contains("0.4_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[31] == "") || ((t.AverageUsage == null ? 0 : t.AverageUsage) <= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0]))));
                    }
                    else
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[31] == "") || ((t.AverageUsage == null ? 0 : t.AverageUsage) >= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[0]) && (t.AverageUsage == null ? 0 : t.AverageUsage) <= Convert.ToDouble(Fields[1].Split('@')[31].Split('_')[1]))));
                    }
                }
                if (!string.IsNullOrEmpty(Fields[1].Split('@')[32]))
                {
                    if (Fields[1].Split('@')[32].Contains("20_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                              ((Fields[1].Split('@')[32] == "") || ((t.Turns == null ? 0 : t.Turns) >= Convert.ToDouble(Fields[1].Split('@')[32].Split('_')[0]))));
                    }
                    else if (Fields[1].Split('@')[32].Contains("1_1000"))
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[32] == "") || ((t.Turns == null ? 0 : t.Turns) <= Convert.ToDouble(Fields[1].Split('@')[32].Split('_')[0]))));
                    }
                    else
                    {
                        ObjCache = ObjCache.Where(t =>
                             ((Fields[1].Split('@')[32] == "") || ((t.Turns == null ? 0 : t.Turns) >= Convert.ToDouble(Fields[1].Split('@')[32].Split('_')[0]) && (t.Turns == null ? 0 : t.Turns) <= Convert.ToDouble(Fields[1].Split('@')[32].Split('_')[1]))));
                    }
                }

                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else
            {
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderInTransitQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||

                        (Convert.ToString(t.AverageCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.AverageUsage.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BinNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BondedInventory) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.CategoryName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Cost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultLocationName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultPullQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultReorderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ExtendedCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.InTransitquantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemTypeName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemUniqueNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.LeadTimeInDays) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link1) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link2) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Markup.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MaximumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MinimumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MonthValue) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnHandQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PackingQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PricePerTerm.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.QuickListName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.RequisitionedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SellPrice.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.StagedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Turns) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UNSPSC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UPC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.WeightPerPiece) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Unit) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Created.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Updated.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Trend ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsAutoInventoryClassification ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Taxable ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Consignment ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false) ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsTransfer ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsPurchase ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.InventoryClassification.GetValueOrDefault(0) == 0 ? "No" : "Yes").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SerialNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LotNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.DateCodeTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsLotSerialExpiryCost ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ImagePath ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AddedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.EditedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsAllowOrderCostuom ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF6 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF7 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF8 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF9 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF10 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.ItemNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UPC ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Description ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LongDescription ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SupplierPartNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ManufacturerNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemUniqueNumber ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ItemTypeName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.GLAccount ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.CreatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UpdatedByName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnOrderInTransitQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.AverageCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.AverageUsage.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BinNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.BondedInventory) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.CategoryName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Cost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultLocationName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultPullQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.DefaultReorderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ExtendedCost.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.InTransitquantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemTypeName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.ItemUniqueNumber) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.LeadTimeInDays) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link1) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Link2) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Markup.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MaximumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MinimumQuantity.ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.MonthValue) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnHandQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.OnTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PackingQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.PricePerTerm.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.QuickListName) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.RequisitionedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SellPrice.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.StagedQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedOrderQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.SuggestedTransferQuantity.GetValueOrDefault(0).ToString()) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Turns) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UNSPSC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.UPC) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.WeightPerPiece) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Unit) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Created.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (Convert.ToString(t.Updated.GetValueOrDefault(DateTime.MinValue).ToString(DateFormat)) ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Trend ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsAutoInventoryClassification ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Taxable ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Consignment ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsItemLevelMinMaxQtyRequired.GetValueOrDefault(false) ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsTransfer ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsPurchase ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.InventoryClassification.GetValueOrDefault(0) == 0 ? "No" : "Yes").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.SerialNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.LotNumberTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.DateCodeTracking ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsLotSerialExpiryCost ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ImagePath ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AppendedBarcodeString ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.AddedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.EditedFrom ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.IsAllowOrderCostuom ? "Yes" : "No").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF6 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF7 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF8 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF9 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF10 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }
        }

        public List<ItemMasterDTO> GetItemsToAddTransferDetail_Paging(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID,
                                                                    Int64 ReplinishRoomID, int TransferRequestType, List<long> SupplierIds, long LogInUserId, string ExclueItemGUIDs, string ExclueBinIDs, string StagingGuids, string RoomDateFormat, TimeZoneInfo CurrentTimeZone)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string Cost = null;
            string Cost1 = null;

            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {

                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }
            }

            dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetItemsToAddTransferDetail_Paging", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType,
                                       ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5,
                                       RoomID, CompanyID, strSupplierIds, LogInUserId, ExclueItemGUIDs, StagingGuids, StockStatus, ExclueBinIDs, TransferRequestType, ReplinishRoomID, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);

            if (dsItems != null && dsItems.Tables.Count > 0)
            {
                DataTable dtItems = dsItems.Tables[0];

                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Trend = row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                        Taxable = row.Field<bool>("Taxable"),
                        Consignment = row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImageType") == "ImagePath" ? row.Field<string>("ImagePath") : row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ItemImageExternalURL") : "",
                        Link2 = row.Field<string>("ItemLink2ImageType") == "InternalLink" ? row.Field<string>("Link2") : row.Field<string>("ImageType") == "ExternalURL" ? row.Field<string>("ItemLink2ExternalURL") : "",
                        ImageType = row.Field<string>("ImageType"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        xmlItemLocations = row.Field<string>("ItemLocations"),
                        QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                        QuickListName = row.Field<string>("QuickListName"),
                        AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                        EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")

                    }).ToList();
                }
            }

            return lstItems;
        }

        public bool UpdateSupplierDetails(long CompanyID, long RoomID, long UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                string strUpdateOnHand = "EXEC [dbo].[UpdateItemSupplierRecordsForIsDefualt] " + CompanyID + ", " + RoomID + ", " + UserID;
                strUpdateOnHand += "   EXEC [dbo].[UpdateItemManufacturerRecordsForIsDefualt] " + CompanyID + ", " + RoomID + ", " + UserID;
                context.Database.ExecuteSqlCommand(strUpdateOnHand);
                return true;
            }
        }

        public Guid? GetGuidByItemNumber(string ItemNumber, long RoomID, long CompanyID)
        {
            Guid? RetGUID = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster obj = new ItemMaster();
                obj = (from u in context.ItemMasters
                       where u.ItemNumber == ItemNumber && u.Room == RoomID && u.CompanyID == CompanyID && u.IsDeleted == false && u.IsArchived == false
                       select u).FirstOrDefault();
                if (obj != null)
                {
                    RetGUID = obj.GUID;
                }
            }
            return RetGUID;
        }

        public List<ExportItemLocationDetailsDTO> GetItemLocationDetailsQtyExport(string ItemGUIDs, long RoomID, long CompanyID)
        {
            List<Guid> arrids = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                foreach (string item in ItemGUIDs.Split(','))
                {
                    Guid temp = Guid.Empty;
                    if (Guid.TryParse(item, out temp))
                    {
                        arrids.Add(temp);
                    }
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", ItemGUIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ExportItemLocationDetailsDTO>("exec GetItemLocationDetailsQtyExport @ItemGUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ExportItemLocationDetailsDTO> GetItemBinLocationDetailsQtyExport(string ItemGUIDs, long RoomID, long CompanyID, string BinIDs)
        {
            //List<Guid> arrids = new List<Guid>();
            //if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            //{
            //    foreach (string item in ItemGUIDs.Split(','))
            //    {
            //        Guid temp = Guid.Empty;
            //        if (Guid.TryParse(item, out temp))
            //        {
            //            arrids.Add(temp);
            //        }
            //    }
            //}

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", ItemGUIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@BinIDs", BinIDs) };
                return context.Database.SqlQuery<ExportItemLocationDetailsDTO>("exec GetItemBinLocationDetailsQtyExport @ItemGUIDs,@RoomID,@CompanyID,@BinIDs", params1).ToList();
            }
        }

        public List<ExportItemLocationDetailsDTO> InventoryCountItemLocationExport(string ItemGUIDs, long RoomID, long CompanyID)
        {
            List<Guid> arrids = new List<Guid>();

            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                foreach (string item in ItemGUIDs.Split(','))
                {
                    Guid temp = Guid.Empty;
                    if (Guid.TryParse(item, out temp))
                    {
                        arrids.Add(temp);
                    }
                }
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUIDs", ItemGUIDs), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ExportItemLocationDetailsDTO>("exec InventoryCountItemLocationExport @ItemGUIDs,@RoomID,@CompanyID", params1).ToList();
            }
        }

        public List<ItemLocationDetailsDTO> GetItemLocationDetailsExport(string ItemGUIDs, long RoomID, long CompanyID)
        {
            List<Guid> arrids = new List<Guid>();

            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                foreach (string item in ItemGUIDs.Split(','))
                {
                    Guid temp = Guid.Empty;
                    if (Guid.TryParse(item, out temp))
                    {
                        arrids.Add(temp);
                    }
                }
            }

            List<ItemLocationDetailsDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                obj = (from bm in context.BinMasters
                       join im in context.ItemMasters on new { igid = bm.ItemGUID ?? Guid.Empty } equals new { igid = im.GUID }
                       join ilevmi in context.ItemLocationeVMISetups on new { bnevid = bm.ID, bm.ItemGUID, IsDeleted = false } equals new { bnevid = (ilevmi.BinID ?? 0), ilevmi.ItemGUID, IsDeleted = (ilevmi.IsDeleted ?? false) } into bn_ilevmi_join
                       from bn_ilevmi in bn_ilevmi_join.DefaultIfEmpty()
                       where (ItemGUIDs == null || ItemGUIDs == "" || arrids.Contains(im.GUID)) && bm.IsDeleted == false && bm.IsArchived == false
                       && bm.Room == RoomID && bm.CompanyID == CompanyID && im.IsDeleted == false && im.IsArchived == false
                       select new ItemLocationDetailsDTO
                       {
                           BinNumber = bm.BinNumber != null ? bm.BinNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           ItemNumber = im.ItemNumber != null ? im.ItemNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                           CriticalQuantity = bm.CriticalQuantity ?? 0,
                           MinimumQuantity = bm.MinimumQuantity ?? 0,
                           MaximumQuantity = bm.MaximumQuantity ?? 0,
                           eVMISensorPortstr = bn_ilevmi.eVMISensorPort != null ? bn_ilevmi.eVMISensorPort.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           eVMISensorIDdbl = bn_ilevmi.eVMISensorID,
                           IsDefault = bm.IsDefault ?? false,
                           GUID = bm.GUID,
                           ItemGUID = bm.ItemGUID,
                           IsDeleted = bm.IsDeleted,
                           IsStagingLocation = bm.IsStagingLocation,
                           DefaultPullQuantity = bm.DefaultPullQuantity,
                           IsEnforceDefaultPullQuantity = bm.IsEnforceDefaultPullQuantity,
                           DefaultReorderQuantity = bm.DefaultReorderQuantity,
                           IsEnforceDefaultReorderQuantity = bm.IsEnforceDefaultReorderQuantity,
                           BinUDF1 = bm.BinUDF1,
                           BinUDF2 = bm.BinUDF2,
                           BinUDF3 = bm.BinUDF3,
                           BinUDF4 = bm.BinUDF4,
                           BinUDF5 = bm.BinUDF5
                       }).ToList();
            }
            return obj;
        }

        public List<ItemLocationDetailsDTO> GetItemLocationDetailsExport(string ItemGUIDs, long RoomID, long CompanyID, string BinIDs)
        {
            List<Guid> arrids = new List<Guid>();
            List<Int64> arrBinids = new List<Int64>();
            if (!string.IsNullOrWhiteSpace(ItemGUIDs))
            {
                foreach (string item in ItemGUIDs.Split(','))
                {
                    Guid temp = Guid.Empty;
                    if (Guid.TryParse(item, out temp))
                    {
                        arrids.Add(temp);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(BinIDs))
            {
                foreach (string item in BinIDs.Split(','))
                {
                    Int64 temp = 0;
                    if (Int64.TryParse(item, out temp))
                    {
                        arrBinids.Add(temp);
                    }
                }
            }

            List<ItemLocationDetailsDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                obj = (from bm in context.BinMasters
                       join im in context.ItemMasters on new { igid = bm.ItemGUID ?? Guid.Empty } equals new { igid = im.GUID }
                       join ilevmi in context.ItemLocationeVMISetups on new { bnevid = bm.ID, bm.ItemGUID, IsDeleted = false } equals new { bnevid = (ilevmi.BinID ?? 0), ilevmi.ItemGUID, IsDeleted = (ilevmi.IsDeleted ?? false) } into bn_ilevmi_join
                       from bn_ilevmi in bn_ilevmi_join.DefaultIfEmpty()
                       where (ItemGUIDs == null || ItemGUIDs == "" || arrids.Contains(im.GUID)) && bm.IsDeleted == false && bm.IsArchived == false
                       && bm.Room == RoomID && bm.CompanyID == CompanyID && im.IsDeleted == false && im.IsArchived == false &&
                       (BinIDs == null || BinIDs == "" || arrBinids.Contains(bm.ID))
                       select new ItemLocationDetailsDTO
                       {
                           BinNumber = bm.BinNumber != null ? bm.BinNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           ItemNumber = im.ItemNumber != null ? im.ItemNumber.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           IsItemLevelMinMax = im.IsItemLevelMinMaxQtyRequired ?? false,
                           CriticalQuantity = bm.CriticalQuantity ?? 0,
                           MinimumQuantity = bm.MinimumQuantity ?? 0,
                           MaximumQuantity = bm.MaximumQuantity ?? 0,
                           eVMISensorPortstr = bn_ilevmi.eVMISensorPort != null ? bn_ilevmi.eVMISensorPort.Replace("[|EmptyStagingBin|]", "") : string.Empty,
                           eVMISensorIDdbl = bn_ilevmi.eVMISensorID,
                           IsDefault = bm.IsDefault ?? false,
                           GUID = bm.GUID,
                           ItemGUID = bm.ItemGUID,
                           IsDeleted = bm.IsDeleted,
                           IsStagingLocation = bm.IsStagingLocation,
                           DefaultPullQuantity = bm.DefaultPullQuantity,
                           IsEnforceDefaultPullQuantity = bm.IsEnforceDefaultPullQuantity,
                           DefaultReorderQuantity = bm.DefaultReorderQuantity,
                           IsEnforceDefaultReorderQuantity = bm.IsEnforceDefaultReorderQuantity,
                           BinUDF1 = bm.BinUDF1,
                           BinUDF2 = bm.BinUDF2,
                           BinUDF3 = bm.BinUDF3,
                           BinUDF4 = bm.BinUDF4,
                           BinUDF5 = bm.BinUDF5

                       }).ToList();
            }
            return obj;
        }
        

        public List<ItemMasterDTO> GetPagedItemsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId, string ItemPopupFor, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, bool IsQLInclude = false, string ExclueItemGUIDs = null, string InclueItemGUIDs = null, string ExclueBinMasterGUIDs = "", long? OrderSupplierId = null)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();

            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string ItemBins = null;

            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;

            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;

            string Cost = null;
            string Cost1 = null;

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OrderSupplierId, ItemBins);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[51]))
                {
                    ItemBins = FieldsPara[51].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }

                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OrderSupplierId, ItemBins);
            }
            else
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OrderSupplierId, ItemBins);
            }

            if (dsItems != null && dsItems.Tables.Count > 0)
            {
                DataTable dtItems = dsItems.Tables[0];
                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        ItemDefaultReorderQuantity = row.Field<double?>("ItemDefaultReorderQuantity"),
                        ItemDefaultPullQuantity = row.Field<double?>("ItemDefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        // Link2 = row.Field<string>("Link2"),
                        Trend = row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                        Taxable = row.Field<bool>("Taxable"),
                        Consignment = row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        Link2 = row.Field<string>("ItemLink2ImageType") == "InternalLink" ? row.Field<string>("Link2") : row.Field<string>("ItemLink2ImageType") == "ExternalURL" ? row.Field<string>("ItemLink2ExternalURL") : "",
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ImageType = row.Field<string>("ImageType"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        xmlItemLocations = row.Field<string>("ItemLocations"),
                        QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                        QuickListName = row.Field<string>("QuickListName"),
                        AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                        EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        OrderUOMID = row.Field<long?>("OrderUOMID"),
                        OrderUOMName = row.Field<string>("OrderUOM"),
                        OrderUOMValue = row.Field<int?>("OrderUOMValue"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        OrderItemCost = row.Field<double?>("Cost"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")

                    }).ToList();
                }
            }

            return lstItems;
        }

        public List<ItemMasterDTO> GetPagedItemsForModelMS(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId, string ItemPopupFor, string RoomDateFormat, TimeZoneInfo CurrentTimeZone, bool IsQLInclude = false, string ExclueItemGUIDs = null, string InclueItemGUIDs = null, string ExclueBinMasterGUIDs = "")
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string Cost = null;
            string Cost1 = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;

            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, null, null, null, null, null, null, null, null, null, null, null);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }

                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, null, null);
            }
            else
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForAddPage", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, ExclueBinMasterGUIDs, null, null, null, null, null, null, null, null, null, null, null);
            }

            if (dsItems != null && dsItems.Tables.Count > 0)
            {
                DataTable dtItems = dsItems.Tables[0];
                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        // Link2 = row.Field<string>("Link2"),
                        Trend = row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                        Taxable = row.Field<bool>("Taxable"),
                        Consignment = row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName") + "(" + Convert.ToString(row.Field<double?>("DefaultLocationQTY") ?? 0) + ")",
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        Link2 = row.Field<string>("ItemLink2ImageType") == "InternalLink" ? row.Field<string>("Link2") : row.Field<string>("ItemLink2ImageType") == "ExternalURL" ? row.Field<string>("ItemLink2ExternalURL") : "",
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ImageType = row.Field<string>("ImageType"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        xmlItemLocations = row.Field<string>("ItemLocations"),
                        QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                        QuickListName = row.Field<string>("QuickListName"),
                        AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                        EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")
                    }).ToList();
                }
            }

            return lstItems;
        }

        public List<ItemMasterDTO> GetPulledItemsForModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId, string ItemPopupFor, string RoomDateFormat, TimeZoneInfo CurrentTimeZone,
                                                         bool IsQLInclude = false, string ExclueItemGUIDs = null, string InclueItemGUIDs = null, int CurrencyDecimalDigits = 0)
        {
            bool isallowNegetive = false;
            string SPname = "GetPagedItemsForAddPull";
            if (ItemPopupFor.ToLower().Equals("pull"))
            {
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                if (objRoomDTO != null && objRoomDTO.AllowPullBeyondAvailableQty == true)
                {
                    isallowNegetive = true;
                    SPname = "GetPagedItemsForAddNegativePull";
                }
            }

            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string StagingBinNumbers = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string Cost = null;
            string Cost1 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;

            string supplierIdsStr = string.Empty;
            if (SupplierIds != null && SupplierIds.Any())
            {
                supplierIdsStr = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, SPname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, supplierIdsStr, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, StagingBinNumbers, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[43]))
                {
                    StagingBinNumbers = FieldsPara[43].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]);
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]);
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, SPname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, supplierIdsStr, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, StagingBinNumbers, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }
            else
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, SPname, StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, supplierIdsStr, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, StagingBinNumbers, UDF6, UDF7, UDF8, UDF9, UDF10, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo);
            }

            if (dsItems != null && dsItems.Tables.Count > 0)
            {

                DataTable dtItems = dsItems.Tables[0];
                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    if (isallowNegetive == true)
                    {
                        lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                        {
                            ID = row.Field<long>("ID"),
                            ItemNumber = row.Field<string>("ItemNumber"),
                            ManufacturerID = row.Field<long?>("ManufacturerID"),
                            ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                            ManufacturerName = row.Field<string>("ManufacturerName"),
                            SupplierID = row.Field<long?>("SupplierID"),
                            SupplierPartNo = row.Field<string>("SupplierPartNo"),
                            SupplierName = row.Field<string>("SupplierName"),
                            UPC = row.Field<string>("UPC"),
                            UNSPSC = row.Field<string>("UNSPSC"),
                            Description = row.Field<string>("Description"),
                            LongDescription = row.Field<string>("LongDescription"),
                            CategoryID = row.Field<long?>("CategoryID"),
                            GLAccountID = row.Field<long?>("GLAccountID"),
                            UOMID = row.Field<long?>("UOMID"),
                            PricePerTerm = row.Field<double?>("PricePerTerm"),
                            CostUOMID = row.Field<long?>("CostUOMID"),
                            DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                            DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                            ItemDefaultPullQuantity = row.Field<double?>("ItemDefaultPullQuantity"),
                            Cost = row.Field<double?>("Cost"),
                            Markup = row.Field<double?>("Markup"),
                            SellPrice = row.Field<double?>("SellPrice"),
                            ExtendedCost = row.Field<double?>("ExtendedCost"),
                            AverageCost = row.Field<double?>("AverageCost"),
                            PerItemCost = row.Field<double?>("PerItemCost"),
                            LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                            Link1 = row.Field<string>("Link1"),
                            Link2 = row.Field<string>("Link2"),
                            Trend = row.Field<bool>("Trend"),
                            IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                            Taxable = row.Field<bool>("Taxable"),
                            Consignment = row.Field<bool>("Consignment"),
                            StagedQuantity = row.Field<double?>("StagedQuantity"),
                            InTransitquantity = row.Field<double?>("InTransitquantity"),
                            OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                            OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                            OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                            SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                            SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                            RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                            PackingQuantity = row.Field<double?>("PackingQuantity"),
                            AverageUsage = row.Field<double?>("AverageUsage"),
                            Turns = row.Field<double?>("Turns"),
                            OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                            CriticalQuantity = row.Field<double>("CriticalQuantity"),
                            MinimumQuantity = row.Field<double>("MinimumQuantity"),
                            MaximumQuantity = row.Field<double>("MaximumQuantity"),
                            WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                            ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                            IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                            IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                            DefaultLocation = row.Field<long?>("DefaultLocationID"),
                            DefaultLocationName =
                            (row.Field<bool>("SerialNumberTracking") == true ||
                                row.Field<bool>("LotNumberTracking") == true ||
                                row.Field<bool>("DateCodeTracking") == true)
                            ?
                            ((row.Field<string>("DefaultLocationName") != null && !string.IsNullOrEmpty((row.Field<string>("DefaultLocationName") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty)))
                                                ||
                                                (row.Field<double?>("ItemLocationStageQty") > 0)
                                                ) ? row.Field<double?>("ItemLocationOHQty") > 0 ? (row.Field<string>("DefaultLocationName") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty) + " (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", row.Field<double>("ItemLocationOHQty")) + ")" : Math.Round(Convert.ToDecimal(row.Field<double?>("ItemLocationStageQty")), CurrencyDecimalDigits) > 0 ? row.Field<string>("DefaultLocationName").Replace("[|EmptyStagingBin|]", string.Empty) + " [Staging] (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", row.Field<double>("ItemLocationStageQty")) + ")" : "(" + string.Format("{0:F" + CurrencyDecimalDigits + "}", 0) + ")" : null //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                            :
                            (
                            ((row.Field<string>("DefaultLocationName1") != null && !string.IsNullOrEmpty((row.Field<string>("DefaultLocationName1") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty)))
                                                    ) ?
                                                        (row.Field<double?>("ItemLocationOHQty") != null && row.Field<double?>("ItemLocationOHQty") > 0)
                                                        ? (row.Field<string>("DefaultLocationName1") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty) + " (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", row.Field<double?>("ItemLocationOHQty")) + ")"
                                                        : Math.Round(Convert.ToDecimal(row.Field<double?>("ItemLocationOHQty")), CurrencyDecimalDigits) <= 0
                                                        ? row.Field<string>("DefaultLocationName1").Replace("[|EmptyStagingBin|]", string.Empty) + " (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", (row.Field<double?>("ItemLocationOHQty") <= 0 ? row.Field<double?>("ItemLocationOHQty") : 0)) + ")"
                                                        : (row.Field<string>("DefaultLocationName1") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty) + "(" + string.Format("{0:F" + CurrencyDecimalDigits + "}", 0) + ")"
                                                        : null
                            ),
                            InventoryClassification = row.Field<int?>("InventoryClassification"),
                            SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                            LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                            DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                            ItemType = row.Field<int>("ItemType"),
                            ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                            ImagePath = row.Field<string>("ImagePath"),
                            UDF1 = row.Field<string>("UDF1"),
                            UDF2 = row.Field<string>("UDF2"),
                            UDF3 = row.Field<string>("UDF3"),
                            UDF4 = row.Field<string>("UDF4"),
                            UDF5 = row.Field<string>("UDF5"),
                            ItemUDF1 = row.Field<string>("UDF1"),
                            ItemUDF2 = row.Field<string>("UDF2"),
                            ItemUDF3 = row.Field<string>("UDF3"),
                            ItemUDF4 = row.Field<string>("UDF4"),
                            ItemUDF5 = row.Field<string>("UDF5"),
                            UDF6 = row.Field<string>("UDF6"),
                            UDF7 = row.Field<string>("UDF7"),
                            UDF8 = row.Field<string>("UDF8"),
                            UDF10 = row.Field<string>("UDF10"),
                            UDF9 = row.Field<string>("UDF9"),
                            ItemUDF6 = row.Field<string>("UDF6"),
                            ItemUDF7 = row.Field<string>("UDF7"),
                            ItemUDF8 = row.Field<string>("UDF8"),
                            ItemUDF9 = row.Field<string>("UDF9"),
                            ItemUDF10 = row.Field<string>("UDF10"),
                            GUID = row.Field<Guid>("GUID"),
                            Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                            Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                            CreatedBy = row.Field<long?>("CreatedBy"),
                            LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                            IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                            IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                            CompanyID = row.Field<long?>("CompanyID"),
                            Room = row.Field<long?>("Room"),
                            CreatedByName = row.Field<string>("CreatedByName"),
                            UpdatedByName = row.Field<string>("UpdatedByName"),
                            RoomName = row.Field<string>("RoomName"),
                            IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                            CategoryName = row.Field<string>("CategoryName"),
                            Unit = row.Field<string>("Unit"),
                            GLAccount = row.Field<string>("GLAccount"),
                            IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                            IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                            BondedInventory = row.Field<string>("BondedInventory"),
                            IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                            InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                            IsBOMItem = row.Field<bool>("IsBOMItem"),
                            RefBomId = row.Field<long?>("RefBomId"),
                            CostUOMName = row.Field<string>("CostUOMName"),
                            PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                            TrendingSetting = row.Field<byte?>("TrendingSetting"),
                            xmlItemLocations = row.Field<string>("ItemLocations"),
                            QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                            QuickListName = row.Field<string>("QuickListName"),
                            AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                            EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                            ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                            ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                            OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                            ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                            ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                            IsActive = row.Field<bool>("IsActive"),
                            MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                            ImageType = row.Field<string>("ImageType"),
                            ItemBlanketPO = row.Field<string>("ItemBlanketPO"),
                            IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                            IsOrderable = row.Field<bool>("IsOrderable"),
                            OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")
                        }).ToList();
                    }
                    else
                    {
                        lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                        {
                            ID = row.Field<long>("ID"),
                            ItemNumber = row.Field<string>("ItemNumber"),
                            ManufacturerID = row.Field<long?>("ManufacturerID"),
                            ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                            ManufacturerName = row.Field<string>("ManufacturerName"),
                            SupplierID = row.Field<long?>("SupplierID"),
                            SupplierPartNo = row.Field<string>("SupplierPartNo"),
                            SupplierName = row.Field<string>("SupplierName"),
                            UPC = row.Field<string>("UPC"),
                            UNSPSC = row.Field<string>("UNSPSC"),
                            Description = row.Field<string>("Description"),
                            LongDescription = row.Field<string>("LongDescription"),
                            CategoryID = row.Field<long?>("CategoryID"),
                            GLAccountID = row.Field<long?>("GLAccountID"),
                            UOMID = row.Field<long?>("UOMID"),
                            PricePerTerm = row.Field<double?>("PricePerTerm"),
                            CostUOMID = row.Field<long?>("CostUOMID"),
                            DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                            DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                            ItemDefaultPullQuantity = row.Field<double?>("ItemDefaultPullQuantity"),
                            Cost = row.Field<double?>("Cost"),
                            Markup = row.Field<double?>("Markup"),
                            SellPrice = row.Field<double?>("SellPrice"),
                            ExtendedCost = row.Field<double?>("ExtendedCost"),
                            AverageCost = row.Field<double?>("AverageCost"),
                            PerItemCost = row.Field<double?>("PerItemCost"),
                            LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                            Link1 = row.Field<string>("Link1"),
                            Link2 = row.Field<string>("Link2"),
                            Trend = row.Field<bool>("Trend"),
                            IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                            Taxable = row.Field<bool>("Taxable"),
                            Consignment = row.Field<bool>("Consignment"),
                            StagedQuantity = row.Field<double?>("StagedQuantity"),
                            InTransitquantity = row.Field<double?>("InTransitquantity"),
                            OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                            OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                            OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                            SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                            SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                            RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                            PackingQuantity = row.Field<double?>("PackingQuantity"),
                            AverageUsage = row.Field<double?>("AverageUsage"),
                            Turns = row.Field<double?>("Turns"),
                            OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                            CriticalQuantity = row.Field<double>("CriticalQuantity"),
                            MinimumQuantity = row.Field<double>("MinimumQuantity"),
                            MaximumQuantity = row.Field<double>("MaximumQuantity"),
                            WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                            ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                            IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                            IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                            DefaultLocation = row.Field<long?>("DefaultLocationID"),
                            DefaultLocationName = ((row.Field<string>("DefaultLocationName") != null && !string.IsNullOrEmpty((row.Field<string>("DefaultLocationName") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty)))
                                                ||
                                                (row.Field<double?>("ItemLocationStageQty") > 0)
                                                ) ? row.Field<double?>("ItemLocationOHQty") > 0 ? (row.Field<string>("DefaultLocationName") ?? string.Empty).Replace("[|EmptyStagingBin|]", string.Empty) + " (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", row.Field<double>("ItemLocationOHQty")) + ")" : Math.Round(Convert.ToDecimal(row.Field<double?>("ItemLocationStageQty")), CurrencyDecimalDigits) > 0 ? row.Field<string>("DefaultLocationName").Replace("[|EmptyStagingBin|]", string.Empty) + " [Staging] (" + string.Format("{0:F" + CurrencyDecimalDigits + "}", row.Field<double>("ItemLocationStageQty")) + ")" : "(" + string.Format("{0:F" + CurrencyDecimalDigits + "}", 0) + ")" : null, //string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity))
                            InventoryClassification = row.Field<int?>("InventoryClassification"),
                            SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                            LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                            DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                            ItemType = row.Field<int>("ItemType"),
                            ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                            ImagePath = row.Field<string>("ImagePath"),
                            UDF1 = row.Field<string>("UDF1"),
                            UDF2 = row.Field<string>("UDF2"),
                            UDF3 = row.Field<string>("UDF3"),
                            UDF4 = row.Field<string>("UDF4"),
                            UDF5 = row.Field<string>("UDF5"),
                            ItemUDF1 = row.Field<string>("UDF1"),
                            ItemUDF2 = row.Field<string>("UDF2"),
                            ItemUDF3 = row.Field<string>("UDF3"),
                            ItemUDF4 = row.Field<string>("UDF4"),
                            ItemUDF5 = row.Field<string>("UDF5"),
                            UDF6 = row.Field<string>("UDF6"),
                            UDF7 = row.Field<string>("UDF7"),
                            UDF8 = row.Field<string>("UDF8"),
                            UDF10 = row.Field<string>("UDF10"),
                            UDF9 = row.Field<string>("UDF9"),
                            ItemUDF6 = row.Field<string>("UDF6"),
                            ItemUDF7 = row.Field<string>("UDF7"),
                            ItemUDF8 = row.Field<string>("UDF8"),
                            ItemUDF9 = row.Field<string>("UDF9"),
                            ItemUDF10 = row.Field<string>("UDF10"),
                            GUID = row.Field<Guid>("GUID"),
                            Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                            Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                            CreatedBy = row.Field<long?>("CreatedBy"),
                            LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                            IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                            IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                            CompanyID = row.Field<long?>("CompanyID"),
                            Room = row.Field<long?>("Room"),
                            CreatedByName = row.Field<string>("CreatedByName"),
                            UpdatedByName = row.Field<string>("UpdatedByName"),
                            RoomName = row.Field<string>("RoomName"),
                            IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                            CategoryName = row.Field<string>("CategoryName"),
                            Unit = row.Field<string>("Unit"),
                            GLAccount = row.Field<string>("GLAccount"),
                            IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                            IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                            BondedInventory = row.Field<string>("BondedInventory"),
                            IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                            InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                            IsBOMItem = row.Field<bool>("IsBOMItem"),
                            RefBomId = row.Field<long?>("RefBomId"),
                            CostUOMName = row.Field<string>("CostUOMName"),
                            PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                            TrendingSetting = row.Field<byte?>("TrendingSetting"),
                            xmlItemLocations = row.Field<string>("ItemLocations"),
                            QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                            QuickListName = row.Field<string>("QuickListName"),
                            AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                            EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                            ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                            ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                            OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                            ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                            ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                            IsActive = row.Field<bool>("IsActive"),
                            MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                            ImageType = row.Field<string>("ImageType"),
                            ItemBlanketPO = row.Field<string>("ItemBlanketPO"),
                            IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                            IsOrderable = row.Field<bool>("IsOrderable"),
                            OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")

                        }).ToList();
                    }
                }
            }

            return lstItems;
        }

        public List<ItemMasterDTO> GetPagedItemsForCreditModel(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId, string ItemPopupFor, string RoomDateFormat, TimeZoneInfo CurrentTimeZone,
                                                         bool IsQLInclude = false, string ExclueItemGUIDs = null, string InclueItemGUIDs = null)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string StagingBinNumbers = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string Cost = null;
            string Cost1 = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPage_CreditHistory", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[43]))
                {
                    StagingBinNumbers = FieldsPara[43].TrimEnd(',');
                }
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPage_CreditHistory", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }
            else
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPage_CreditHistory", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, ItemPopupFor, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }

            if (dsItems != null && dsItems.Tables.Count > 0)
            {
                DataTable dtItems = dsItems.Tables[0];

                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        //Link2 = row.Field<string>("Link2"),
                        Trend = row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                        Taxable = row.Field<bool>("Taxable"),
                        Consignment = row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImageType") == "ImagePath" ? row.Field<string>("ImagePath") : row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ItemImageExternalURL") : "",
                        Link2 = row.Field<string>("ItemLink2ImageType") == "InternalLink" ? row.Field<string>("Link2") : row.Field<string>("ItemLink2ImageType") == "ExternalURL" ? row.Field<string>("ItemLink2ExternalURL") : "",
                        ImageType = row.Field<string>("ImageType"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF1"),
                        UDF7 = row.Field<string>("UDF2"),
                        UDF8 = row.Field<string>("UDF3"),
                        UDF9 = row.Field<string>("UDF4"),
                        UDF10 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        xmlItemLocations = row.Field<string>("ItemLocations"),
                        QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                        QuickListName = row.Field<string>("QuickListName"),
                        AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                        EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")

                    }).ToList();
                }
            }

            return lstItems;
        }
        public double getOnOrderInTransitQty(Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double OnOrderInTransitQuantity = 0;

                if (context.OrderDetails.Join(context.OrderMasters, od => od.OrderGUID, om => om.GUID, (od, om) => new { od, om }).Where(t => t.om.IsDeleted == false && (t.od.IsDeleted ?? false) == false && (t.od.IsCloseItem ?? false) == false && new int[] { 4, 5, 6, 7 }.Contains(t.om.OrderStatus) && t.od.ItemGUID == ItemGUID && t.om.OrderType == 1).Any())
                {
                    OnOrderInTransitQuantity = context.OrderDetails.Join(context.OrderMasters, od => od.OrderGUID, om => om.GUID, (od, om) => new { od, om }).Where(t => t.om.IsDeleted == false && (t.od.IsDeleted ?? false) == false && (t.od.IsCloseItem ?? false) == false && new int[] { 4, 5, 6, 7 }.Contains(t.om.OrderStatus) && t.od.ItemGUID == ItemGUID && t.om.OrderType == 1).Sum(t => ((t.od.ApprovedQuantity ?? (t.od.RequestedQuantity ?? 0))) - (t.od.ReceivedQuantity ?? 0));
                }
                return OnOrderInTransitQuantity;
            }
        }
        public List<Guid> GetAllItemsId(long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Where(t => t.Room == RoomId && t.CompanyID == CompanyId && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Select(t => t.GUID).ToList();
            }

        }
        public List<ItemMasterDTO> GetAllItemsRecordsForViewReport(long[] CompanyIds, long[] RoomIds, bool applydatefilter, string StartDate, string EndDate)
        {
            List<ItemMasterDTO> obj = null;
            DateTime sDate = new DateTime();
            DateTime eDate = new DateTime();
            if (applydatefilter)
            {
                sDate = Convert.ToDateTime(StartDate);

                eDate = Convert.ToDateTime(EndDate);
            }
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ItemMasters
                           //where u.IsDeleted == false && u.IsArchived == false && CompanyIds.Contains(u.CompanyID ?? 0) && RoomIds.Contains(u.Room ?? 0) && (applydatefilter ? (EntityFunctions.TruncateTime(u.Created) >= EntityFunctions.TruncateTime(StartDate) && EntityFunctions.TruncateTime(u.Created) <= EntityFunctions.TruncateTime(EndDate)) : true)
                       where u.IsDeleted == false && u.IsArchived == false && CompanyIds.Contains(u.CompanyID ?? 0) && RoomIds.Contains(u.Room ?? 0)
                       && (applydatefilter ? (u.Created >= sDate && u.Created <= eDate) : true)
                       select new ItemMasterDTO
                       {
                           GUID = u.GUID,
                           ItemNumber = u.ItemNumber,
                           Created = u.Created,
                           Room = u.Room,
                           CompanyID = u.CategoryID
                       }).AsParallel().ToList();
            }
            return obj;

        }

        public ItemMaster UpdateItemWeight(Guid ItemGUID, double WeighterPiece, bool UpdateAndGet)
        {
            ItemMaster objItem = new ItemMaster();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                if (objItem != null)
                {
                    if (UpdateAndGet)
                    {
                        objItem.WeightPerPiece = WeighterPiece;
                        context.SaveChanges();
                    }
                }
            }
            return objItem;
        }


        public List<ItemMasterDTO> GetRecordsOnlyItemsFields(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int64? MonthlyAverageUsage = 30;
                if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                {
                    MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                }
                if (MonthlyAverageUsage == null)
                {
                    MonthlyAverageUsage = 30;
                }

                return (from im in context.ItemMasters
                        where im.CompanyID == CompanyID && im.Room == RoomID
                        && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
                        select new ItemMasterDTO
                        {
                            ID = im.ID,
                            ItemNumber = im.ItemNumber,
                            ManufacturerID = im.ManufacturerID,
                            ManufacturerNumber = im.ManufacturerNumber,
                            SupplierID = im.SupplierID,
                            SupplierPartNo = im.SupplierPartNo,
                            UPC = im.UPC,
                            UNSPSC = im.UNSPSC,
                            Description = im.Description,
                            LongDescription = im.LongDescription,
                            CategoryID = im.CategoryID,
                            GLAccountID = im.GLAccountID,
                            UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
                            PricePerTerm = im.PricePerTerm,
                            CostUOMID = im.CostUOMID,
                            DefaultReorderQuantity = im.DefaultReorderQuantity,
                            DefaultPullQuantity = im.DefaultPullQuantity,
                            Cost = im.Cost,
                            Markup = im.Markup,
                            SellPrice = im.SellPrice,
                            ExtendedCost = im.ExtendedCost,
                            AverageCost = im.AverageCost,
                            PerItemCost = im.PerItemCost,
                            LeadTimeInDays = im.LeadTimeInDays,
                            Link1 = im.Link1,
                            Link2 = im.Link2,
                            Trend = im.Trend,
                            Taxable = im.Taxable,
                            Consignment = im.Consignment,
                            StagedQuantity = im.StagedQuantity,
                            InTransitquantity = im.InTransitquantity,
                            OnOrderQuantity = im.OnOrderQuantity,
                            OnReturnQuantity = im.OnReturnQuantity,
                            OnTransferQuantity = im.OnTransferQuantity,
                            SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                            SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                            RequisitionedQuantity = im.RequisitionedQuantity,
                            PackingQuantity = im.PackingQuantity,
                            AverageUsage = im.AverageUsage,
                            Turns = im.Turns,
                            OnHandQuantity = im.OnHandQuantity,
                            CriticalQuantity = im.CriticalQuantity,
                            MinimumQuantity = im.MinimumQuantity,
                            MaximumQuantity = im.MaximumQuantity,
                            WeightPerPiece = im.WeightPerPiece,
                            ItemUniqueNumber = im.ItemUniqueNumber,
                            IsPurchase = (im.IsPurchase == true ? true : false),
                            IsTransfer = (im.IsTransfer == true ? true : false),
                            DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
                            InventoryClassification = im.InventoryClassification,
                            SerialNumberTracking = im.SerialNumberTracking,
                            LotNumberTracking = im.LotNumberTracking,
                            DateCodeTracking = im.DateCodeTracking,
                            ItemType = im.ItemType,
                            ImagePath = im.ImagePath,
                            UDF1 = im.UDF1,
                            UDF2 = im.UDF2,
                            UDF3 = im.UDF3,
                            UDF4 = im.UDF4,
                            UDF5 = im.UDF5,
                            UDF6 = im.UDF6,
                            UDF7 = im.UDF7,
                            UDF8 = im.UDF8,
                            UDF9 = im.UDF9,
                            UDF10 = im.UDF10,
                            GUID = im.GUID,
                            Created = im.Created,
                            Updated = im.Updated,
                            CreatedBy = im.CreatedBy,
                            LastUpdatedBy = im.LastUpdatedBy,
                            IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                            IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                            CompanyID = im.CompanyID,
                            Room = im.Room,
                            IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                            IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                            IsBuildBreak = im.IsBuildBreak,
                            BondedInventory = im.BondedInventory,
                            PullQtyScanOverride = im.PullQtyScanOverride,
                            TrendingSetting = im.TrendingSetting,
                            IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                            IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                            IsBOMItem = im.IsBOMItem,
                            RefBomId = im.RefBomId,
                            LastCost = im.LastCost,
                            ImageType = im.ImageType,
                            ItemImageExternalURL = im.ItemImageExternalURL,
                            ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                            ItemLink2ImageType = im.ItemLink2ImageType,
                            OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,
                            IsActive = im.IsActive,
                            MonthlyAverageUsage = ((long)(im.AverageUsage.HasValue ? (long)im.AverageUsage : (long)0) * (long)MonthlyAverageUsage),
                            IsAllowOrderCostuom = im.IsAllowOrderCostuom,
                            IsOrderable = im.IsOrderable,
                            OnQuotedQuantity = im.OnQuotedQuantity ?? 0
                        }).ToList();
            }
        }

        public ItemMasterDTO GetRecordOnlyItemsFields(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemMasterDTO obj = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int64? MonthlyAverageUsage = 30;
                if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                {
                    MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                }
                if (MonthlyAverageUsage == null)
                {
                    MonthlyAverageUsage = 30;
                }
                obj = (from im in context.ItemMasters
                       where im.CompanyID == CompanyID && im.Room == RoomID
                       && im.IsDeleted == IsDeleted && im.IsArchived == IsArchived
                       && im.GUID == new Guid(GUID)

                       select new ItemMasterDTO
                       {
                           ID = im.ID,
                           ItemNumber = im.ItemNumber,
                           ManufacturerID = im.ManufacturerID,
                           ManufacturerNumber = im.ManufacturerNumber,
                           SupplierID = im.SupplierID,
                           SupplierPartNo = im.SupplierPartNo,
                           UPC = im.UPC,
                           UNSPSC = im.UNSPSC,
                           Description = im.Description,
                           LongDescription = im.LongDescription,
                           CategoryID = im.CategoryID,
                           GLAccountID = im.GLAccountID,
                           UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
                           PricePerTerm = im.PricePerTerm,
                           CostUOMID = im.CostUOMID,
                           DefaultReorderQuantity = im.DefaultReorderQuantity,
                           DefaultPullQuantity = im.DefaultPullQuantity,
                           Cost = im.Cost,
                           Markup = im.Markup,
                           SellPrice = im.SellPrice,
                           ExtendedCost = im.ExtendedCost,
                           AverageCost = im.AverageCost,
                           PerItemCost = im.PerItemCost,
                           LeadTimeInDays = im.LeadTimeInDays,
                           Link1 = im.Link1,
                           Link2 = im.Link2,
                           Trend = im.Trend,
                           Taxable = im.Taxable,
                           Consignment = im.Consignment,
                           StagedQuantity = im.StagedQuantity,
                           InTransitquantity = im.InTransitquantity,
                           OnOrderQuantity = im.OnOrderQuantity,
                           OnReturnQuantity = im.OnReturnQuantity,
                           OnTransferQuantity = im.OnTransferQuantity,
                           SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                           SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                           RequisitionedQuantity = im.RequisitionedQuantity,
                           PackingQuantity = im.PackingQuantity,
                           AverageUsage = im.AverageUsage,
                           Turns = im.Turns,
                           OnHandQuantity = im.OnHandQuantity,
                           CriticalQuantity = im.CriticalQuantity,
                           MinimumQuantity = im.MinimumQuantity,
                           MaximumQuantity = im.MaximumQuantity,
                           WeightPerPiece = im.WeightPerPiece,
                           ItemUniqueNumber = im.ItemUniqueNumber,
                           IsPurchase = (im.IsPurchase == true ? true : false),
                           IsTransfer = (im.IsTransfer == true ? true : false),
                           DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
                           InventoryClassification = im.InventoryClassification,
                           SerialNumberTracking = im.SerialNumberTracking,
                           LotNumberTracking = im.LotNumberTracking,
                           DateCodeTracking = im.DateCodeTracking,
                           ItemType = im.ItemType,
                           ImagePath = im.ImagePath,
                           UDF1 = im.UDF1,
                           UDF2 = im.UDF2,
                           UDF3 = im.UDF3,
                           UDF4 = im.UDF4,
                           UDF5 = im.UDF5,
                           UDF6 = im.UDF6,
                           UDF7 = im.UDF7,
                           UDF8 = im.UDF8,
                           UDF9 = im.UDF9,
                           UDF10 = im.UDF10,
                           GUID = im.GUID,
                           Created = im.Created,
                           Updated = im.Updated,
                           CreatedBy = im.CreatedBy,
                           LastUpdatedBy = im.LastUpdatedBy,
                           IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                           IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                           CompanyID = im.CompanyID,
                           Room = im.Room,
                           IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                           IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                           IsBuildBreak = im.IsBuildBreak,
                           BondedInventory = im.BondedInventory,
                           PullQtyScanOverride = im.PullQtyScanOverride,
                           TrendingSetting = im.TrendingSetting,
                           IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                           IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                           IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
                           IsBOMItem = im.IsBOMItem,
                           RefBomId = im.RefBomId,
                           LastCost = im.LastCost,
                           ImageType = im.ImageType,
                           ItemImageExternalURL = im.ItemImageExternalURL,
                           ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                           ItemLink2ImageType = im.ItemLink2ImageType,
                           //CreatedDate = CommonUtility.ConvertDateByTimeZone(im.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                           //UpdatedDate = CommonUtility.ConvertDateByTimeZone(im.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true)
                           OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,
                           IsActive = im.IsActive,
                           MonthlyAverageUsage = ((long)(im.AverageUsage.HasValue ? (long)im.AverageUsage : (long)0) * (long)MonthlyAverageUsage),
                           IsAllowOrderCostuom = im.IsAllowOrderCostuom,
                           IsOrderable = im.IsOrderable,
                           OnQuotedQuantity = im.OnQuotedQuantity ?? 0
                       }).FirstOrDefault();
            }
            return obj;

        }

        public ItemMasterDTO GetRecordOnlyItemsFields(string GUID, Int64 RoomID, Int64 CompanyID)
        {
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(base.DataBaseName);
            ItemMasterDTO obj = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int64? MonthlyAverageUsage = 30;
                if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                {
                    MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                }
                if (MonthlyAverageUsage == null)
                {
                    MonthlyAverageUsage = 30;
                }
                obj = (from im in context.ItemMasters
                       where im.CompanyID == CompanyID && im.Room == RoomID
                       && im.GUID == new Guid(GUID)

                       select new ItemMasterDTO
                       {
                           ID = im.ID,
                           ItemNumber = im.ItemNumber,
                           ManufacturerID = im.ManufacturerID,
                           ManufacturerNumber = im.ManufacturerNumber,
                           SupplierID = im.SupplierID,
                           SupplierPartNo = im.SupplierPartNo,
                           UPC = im.UPC,
                           UNSPSC = im.UNSPSC,
                           Description = im.Description,
                           LongDescription = im.LongDescription,
                           CategoryID = im.CategoryID,
                           GLAccountID = im.GLAccountID,
                           UOMID = im.UOMID == null ? 0 : (Int64)(im.UOMID),
                           PricePerTerm = im.PricePerTerm,
                           CostUOMID = im.CostUOMID,
                           DefaultReorderQuantity = im.DefaultReorderQuantity,
                           DefaultPullQuantity = im.DefaultPullQuantity,
                           Cost = im.Cost,
                           Markup = im.Markup,
                           SellPrice = im.SellPrice,
                           ExtendedCost = im.ExtendedCost,
                           AverageCost = im.AverageCost,
                           PerItemCost = im.PerItemCost,
                           LeadTimeInDays = im.LeadTimeInDays,
                           Link1 = im.Link1,
                           Link2 = im.Link2,
                           Trend = im.Trend,
                           Taxable = im.Taxable,
                           Consignment = im.Consignment,
                           StagedQuantity = im.StagedQuantity,
                           InTransitquantity = im.InTransitquantity,
                           OnOrderQuantity = im.OnOrderQuantity,
                           OnReturnQuantity = im.OnReturnQuantity,
                           OnTransferQuantity = im.OnTransferQuantity,
                           SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                           SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                           RequisitionedQuantity = im.RequisitionedQuantity,
                           PackingQuantity = im.PackingQuantity,
                           AverageUsage = im.AverageUsage,
                           Turns = im.Turns,
                           OnHandQuantity = im.OnHandQuantity,
                           CriticalQuantity = im.CriticalQuantity,
                           MinimumQuantity = im.MinimumQuantity,
                           MaximumQuantity = im.MaximumQuantity,
                           WeightPerPiece = im.WeightPerPiece,
                           ItemUniqueNumber = im.ItemUniqueNumber,
                           IsPurchase = (im.IsPurchase == true ? true : false),
                           IsTransfer = (im.IsTransfer == true ? true : false),
                           DefaultLocation = im.DefaultLocation == null ? 0 : (Int64)(im.DefaultLocation),
                           InventoryClassification = im.InventoryClassification,
                           SerialNumberTracking = im.SerialNumberTracking,
                           LotNumberTracking = im.LotNumberTracking,
                           DateCodeTracking = im.DateCodeTracking,
                           ItemType = im.ItemType,
                           ImagePath = im.ImagePath,
                           UDF1 = im.UDF1,
                           UDF2 = im.UDF2,
                           UDF3 = im.UDF3,
                           UDF4 = im.UDF4,
                           UDF5 = im.UDF5,
                           ItemUDF1 = im.UDF1,
                           ItemUDF2 = im.UDF2,
                           ItemUDF3 = im.UDF3,
                           ItemUDF4 = im.UDF4,
                           ItemUDF5 = im.UDF5,

                           UDF6 = im.UDF6,
                           UDF7 = im.UDF7,
                           UDF8 = im.UDF8,
                           UDF9 = im.UDF9,
                           UDF10 = im.UDF10,
                           ItemUDF6 = im.UDF6,
                           ItemUDF7 = im.UDF7,
                           ItemUDF8 = im.UDF8,
                           ItemUDF9 = im.UDF9,
                           ItemUDF10 = im.UDF10,

                           GUID = im.GUID,
                           Created = im.Created,
                           Updated = im.Updated,
                           CreatedBy = im.CreatedBy,
                           LastUpdatedBy = im.LastUpdatedBy,
                           IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                           IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                           CompanyID = im.CompanyID,
                           Room = im.Room,
                           IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                           IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                           IsBuildBreak = im.IsBuildBreak,
                           BondedInventory = im.BondedInventory,
                           PullQtyScanOverride = im.PullQtyScanOverride,
                           TrendingSetting = im.TrendingSetting,
                           IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                           IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                           IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
                           IsBOMItem = im.IsBOMItem,
                           RefBomId = im.RefBomId,
                           LastCost = im.LastCost,
                           ImageType = im.ImageType,
                           ItemImageExternalURL = im.ItemImageExternalURL,
                           ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                           ItemLink2ImageType = im.ItemLink2ImageType,
                           OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,
                           IsActive = im.IsActive,
                           MonthlyAverageUsage = ((long)(im.AverageUsage.HasValue ? (long)im.AverageUsage : (long)0) * (long)MonthlyAverageUsage),
                           ItemIsActiveDate = im.ItemIsActiveDate,
                           IsAllowOrderCostuom = im.IsAllowOrderCostuom,
                           IsOrderable = im.IsOrderable,
                           OnQuotedQuantity = im.OnQuotedQuantity ?? 0
                       }).SingleOrDefault();
            }

            return obj;
        }


        public CostDTO GetExtCostAndAvgCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string Action = "others")
        {
            CostDTO objCostDTO = new CostDTO();

            double AverageCost = 0;
            double ExtendedCost = 0;
            double LastCost = 0;
            double MarkUp = 0;
            double SellPrice = 0;
            double PerItemCost = 0;
            // read with nolock
            using (var txn = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }
            ))
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                    if (objItem != null)
                    {
                        ExtendedCost = objItem.ExtendedCost ?? 0;
                        MarkUp = objItem.Markup ?? 0;
                        SellPrice = objItem.SellPrice ?? 0;
                        LastCost = objItem.Cost ?? 0;
                        AverageCost = objItem.AverageCost ?? 0;
                        PerItemCost = objItem.PerItemCost ?? 0;
                        RoomDAL roomDAL = new RoomDAL(base.DataBaseName);
                        var objRoom = roomDAL.GetRoomByIDPlain(objItem.Room.GetValueOrDefault(0));
                        CostUOMMaster objCostUom = context.CostUOMMasters.FirstOrDefault(t => t.ID == objItem.CostUOMID);
                        //UnitMaster objUnitMaster = context.UnitMasters.FirstOrDefault(t => t.ID == objItem.UOMID);

                        if (objItem.Consignment)
                        {
                            // For last cost and sell price
                            LastCost = objItem.Cost ?? 0;
                            SellPrice = (objItem.Cost ?? 0) + (((objItem.Cost ?? 0) * (objItem.Markup ?? 0)) / 100);
                            // for extended and average cost when consigned items ..
                            IQueryable<ItemLocationDetail> Ilq = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && t.IsDeleted == false).OrderByDescending(t => t.ReceivedDate).ThenByDescending(t => t.ID);

                            if (objCostUom != null && (objCostUom.CostUOMValue ?? 0) > 0)
                            {
                                double CostUomval = objCostUom.CostUOMValue ?? 0;
                                double Extcost = 0;
                                IQueryable<ItemLocationDetail> ilqe = Ilq.Where(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)) != 0);
                                double totalItemquantity = 0;
                                if (ilqe.Any())
                                {
                                    totalItemquantity = ilqe.Sum(x => (x.ConsignedQuantity ?? 0)) + ilqe.Sum(x => (x.CustomerOwnedQuantity ?? 0));
                                }
                                if (totalItemquantity > 0)
                                {
                                    if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.AverageCost).ToString()) // Average cost
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * (totalItemquantity);
                                    }
                                    else if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.LastCost).ToString()) // Average cost
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * (totalItemquantity);
                                    }
                                    else // default as last cost 
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * (totalItemquantity);
                                    }
                                    ExtendedCost = Extcost;

                                    AverageCost = (Extcost / totalItemquantity) * CostUomval;
                                }
                                else
                                {
                                    ExtendedCost = 0;
                                    AverageCost = 0;
                                    Extcost = 0;
                                }
                            }
                            else
                            {
                                double CostUomval = 1;
                                double Extcost = 0;
                                double totalItemquantity = 0;
                                IQueryable<ItemLocationDetail> ilqe = Ilq.Where(t => ((t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0)) != 0);
                                if (ilqe.Any())
                                {
                                    totalItemquantity = ilqe.Sum(x => (x.ConsignedQuantity ?? 0)) + ilqe.Sum(x => (x.CustomerOwnedQuantity ?? 0));
                                }
                                if (totalItemquantity > 0)
                                {
                                    if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.AverageCost).ToString()) // Average cost
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    else if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.LastCost).ToString()) // Average cost
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    else // default as last cost 
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    ExtendedCost = Extcost;

                                    AverageCost = (Extcost / totalItemquantity) * CostUomval;
                                }
                                else
                                {
                                    ExtendedCost = 0;
                                    AverageCost = 0;
                                    Extcost = 0;
                                }
                            }
                        }
                        else
                        {
                            // For last cost and sell price

                            IQueryable<ItemLocationDetail> Ilq = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && t.IsDeleted == false).OrderByDescending(t => t.ReceivedDate).ThenByDescending(t => t.ID);
                            if (Ilq.Any())
                            {
                                LastCost = Ilq.First().Cost ?? 0;
                                if ((objItem.Markup ?? 0) > 0)
                                {
                                    SellPrice = (objItem.Cost ?? 0) + (((objItem.Cost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                else
                                {
                                    SellPrice = objItem.Cost ?? 0;
                                }
                            }
                            else
                            {
                                LastCost = 0;
                                SellPrice = 0;
                            }

                            // For extended cost and average cost
                            if (objCostUom != null && (objCostUom.CostUOMValue ?? 0) > 0)
                            {
                                double CostUomval = objCostUom.CostUOMValue ?? 0;
                                double Extcost = 0;
                                IQueryable<ItemLocationDetail> ilqe = Ilq.Where(t => (t.CustomerOwnedQuantity ?? 0) != 0);
                                double totalItemquantity = 0;
                                if (ilqe.Any())
                                {
                                    totalItemquantity = ilqe.Sum(x => (x.ConsignedQuantity ?? 0)) + ilqe.Sum(x => (x.CustomerOwnedQuantity ?? 0));
                                }
                                if (totalItemquantity > 0)
                                {
                                    if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.AverageCost).ToString()) // Average cost
                                    {
                                        foreach (var ritem in ilqe)
                                        {
                                            Extcost = Extcost + ((ritem.Cost ?? 0) / CostUomval) * ((ritem.CustomerOwnedQuantity ?? 0) + (ritem.ConsignedQuantity ?? 0));
                                        }
                                    }
                                    else if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.LastCost).ToString()) // Average cost
                                    {
                                        // last cost may have to consider the last cost for all receipt to calculate ext cost.so as of now in both cases average cost is same.
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    else // default as last cost 
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    ExtendedCost = Extcost;

                                    AverageCost = (Extcost / totalItemquantity) * CostUomval;
                                }
                                else
                                {
                                    ExtendedCost = 0;
                                    AverageCost = 0;
                                    Extcost = 0;
                                }
                            }
                            else
                            {
                                double CostUomval = 1;
                                double Extcost = 0;
                                IQueryable<ItemLocationDetail> ilqe = Ilq.Where(t => (t.CustomerOwnedQuantity ?? 0) != 0);
                                double totalItemquantity = 0;
                                if (ilqe.Any())
                                {
                                    totalItemquantity = ilqe.Sum(x => (x.ConsignedQuantity ?? 0)) + ilqe.Sum(x => (x.CustomerOwnedQuantity ?? 0));
                                }
                                if (totalItemquantity > 0)
                                {
                                    if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.AverageCost).ToString()) // Average cost
                                    {
                                        foreach (var ritem in ilqe)
                                        {
                                            Extcost = Extcost + ((ritem.Cost ?? 0) / CostUomval) * ((ritem.CustomerOwnedQuantity ?? 0) + (ritem.ConsignedQuantity ?? 0));
                                        }
                                    }
                                    else if (objRoom.MethodOfValuingInventory == Convert.ToInt32(InventoryValuationMethod.LastCost).ToString()) // Average cost
                                    {
                                        // last cost may have to consider the last cost for all receipt to calculate ext cost.so as of now in both cases average cost is same.
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    else // default as last cost 
                                    {
                                        Extcost = ((objItem.Cost ?? 0) / CostUomval) * totalItemquantity;
                                    }
                                    ExtendedCost = Extcost;

                                    AverageCost = (Extcost / totalItemquantity) * CostUomval;
                                }
                                else
                                {
                                    ExtendedCost = 0;
                                    AverageCost = 0;
                                    Extcost = 0;
                                }
                            }
                            // Average cost

                        }

                        if (objCostUom != null && (objCostUom.CostUOMValue ?? 0) > 0)
                        {
                            double CostUomval = objCostUom.CostUOMValue ?? 0;
                            PerItemCost = (objItem.Cost ?? 0) / CostUomval;
                        }
                        else
                        {
                            double CostUomval = 1;
                            PerItemCost = (objItem.Cost ?? 0) / CostUomval;
                        }
                    }
                }
                txn.Complete();
            }

            objCostDTO.AvgCost = AverageCost;
            objCostDTO.Cost = LastCost;
            objCostDTO.ExtCost = ExtendedCost;
            objCostDTO.Markup = MarkUp;
            objCostDTO.SellPrice = SellPrice;
            objCostDTO.PerItemCost = PerItemCost;

            return objCostDTO;
        }


        public CostDTO GetAndUpdateExtCostAndAvgCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            CostDTO objCostDTO = GetExtCostAndAvgCost(ItemGUID, RoomID, CompanyID);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                if (objItem != null)
                {
                    objItem.AverageCost = objCostDTO.AvgCost;
                    objItem.ExtendedCost = objCostDTO.ExtCost;
                    objItem.PerItemCost = objCostDTO.PerItemCost;
                    //objItem.Cost = objCostDTO.Cost;
                    //objItem.LastCost = objCostDTO.Cost ?? 0;
                    context.SaveChanges();
                }
            }
            return objCostDTO;
        }

        public void UpdateItemCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, string EditFrom, long SessionUserId, long EnterpriseId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster oItemMaster = context.ItemMasters.Where(x => x.GUID == ItemGUID && x.CompanyID == CompanyID && x.Room == RoomID).FirstOrDefault();
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                var oRoom = objRoomDAL.GetRoomByIDPlain(RoomID);

                if (oRoom.MethodOfValuingInventory == ((int)InventoryValuationMethod.LastCost).ToString() && !oItemMaster.Consignment)
                {
                    var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Itemguid", ItemGUID),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@Cost", oItemMaster.Cost.GetValueOrDefault(0)),
                                                    new SqlParameter("@EditedFrom", EditFrom)
                                                };
                    context.Database.ExecuteSqlCommand("exec [UpdateItemCost] @Itemguid,@CompanyID,@RoomID,@Cost,@EditedFrom", params1);
                    ItemMasterDTO oItemDTO = GetItemWithoutJoins(null, ItemGUID);
                    Edit(oItemDTO, SessionUserId, EnterpriseId);
                }
            }
        }

        public double? CalculateAndGetItemCost(Guid ItemGUID, double? itemCost, Int64 RoomID, Int64 CompanyID)
        {
            double? newItemCost = null;
            int costUOMValue = 1;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                costUOMValue = (from im in context.ItemMasters
                                join uom in context.CostUOMMasters on im.CostUOMID equals uom.ID
                                where im.CompanyID == CompanyID && im.Room == RoomID && im.GUID == ItemGUID
                                select uom.CostUOMValue).FirstOrDefault().GetValueOrDefault(0);
            }

            if (costUOMValue == 0)
                costUOMValue = 1;

            if (itemCost.HasValue)
                newItemCost = itemCost.Value / costUOMValue;

            return newItemCost;
        }

        public void UpdatePastReceiptCostByItemCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, double ItemCost)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlHelper.ExecuteNonQuery(Connectionstring, "UpdatePastReceiptCostByItemCost", CompanyID, RoomID, ItemGUID, ItemCost);
        }

        public void UpdateItemAndPastReceiptCostByReceiveCost(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, bool IsLastCost, string EditedFrom, DateTime ReceivedOn)
        {
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            SqlHelper.ExecuteNonQuery(Connectionstring, "UpdateItemAndPastReceiptCostByReceiveCost", CompanyID, RoomID, ItemGUID, IsLastCost, ReceivedOn, EditedFrom);

        }

        public IEnumerable<ItemMasterDTO> GetItemsByArray(List<Guid> ItemGUIDs, long RoomId, long CompanyId)
        {
            //IEnumerable<ItemMasterDTO> ObjCache = null;
            #region "Conditional"

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //Int64? MonthlyAverageUsage = 30;
                string itemGuids = string.Empty;

                if (ItemGUIDs.Any())
                {
                    itemGuids = string.Join(",", ItemGUIDs);
                }

                //foreach (Guid item in ItemGUIDs)
                //{
                //    if (!string.IsNullOrWhiteSpace(Guids))
                //    {
                //        Guids = Guids + "," + Convert.ToString(item);
                //    }
                //    else
                //    {
                //        Guids = Convert.ToString(item);
                //    }
                //}

                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuids", itemGuids ?? (object)DBNull.Value) ,
                                                   new SqlParameter("@RoomId",RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId)
                };
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemByGuIDs] @ItemGuids,@RoomId,@CompanyId ", params1).ToList();
                //select new ItemMasterDTO
                //{

                //    ID = im.ID,
                //    ItemNumber = im.ItemNumber,
                //    ManufacturerID = im.ManufacturerID,
                //    ManufacturerNumber = im.ManufacturerNumber,
                //    ManufacturerName = im.ManufacturerName,
                //    SupplierID = im.SupplierID,
                //    SupplierPartNo = im.SupplierPartNo,
                //    SupplierName = im.SupplierName,
                //    UPC = im.UPC,
                //    UNSPSC = im.UNSPSC,
                //    Description = im.Description,
                //    LongDescription = im.LongDescription,
                //    CategoryID = im.CategoryID,
                //    GLAccountID = im.GLAccountID,
                //    UOMID = im.UOMID,
                //    PricePerTerm = im.PricePerTerm,
                //    CostUOMID = im.CostUOMID,
                //    DefaultReorderQuantity = im.DefaultReorderQuantity,
                //    DefaultPullQuantity = im.DefaultPullQuantity,
                //    Cost = im.Cost,
                //    Markup = im.Markup,
                //    SellPrice = im.SellPrice,
                //    ExtendedCost = im.ExtendedCost,
                //    AverageCost = im.AverageCost,
                //    PerItemCost = im.PerItemCost,
                //    LeadTimeInDays = im.LeadTimeInDays,
                //    Link1 = im.Link1,
                //    Link2 = im.Link2,
                //    Trend = im.Trend,
                //    Taxable = im.Taxable,
                //    Consignment = im.Consignment,
                //    StagedQuantity = im.StagedQuantity,
                //    InTransitquantity = im.InTransitquantity,
                //    OnOrderQuantity = im.OnOrderQuantity,
                //    OnReturnQuantity = im.OnReturnQuantity,
                //    OnTransferQuantity = im.OnTransferQuantity,
                //    SuggestedOrderQuantity = im.SuggestedOrderQuantity,
                //    SuggestedTransferQuantity = im.SuggestedTransferQuantity,
                //    RequisitionedQuantity = im.RequisitionedQuantity,
                //    PackingQuantity = im.PackingQuantity,
                //    AverageUsage = im.AverageUsage,
                //    Turns = im.Turns,
                //    OnHandQuantity = im.OnHandQuantity,
                //    CriticalQuantity = im.CriticalQuantity,
                //    MinimumQuantity = im.MinimumQuantity,
                //    MaximumQuantity = im.MaximumQuantity,
                //    WeightPerPiece = im.WeightPerPiece,
                //    ItemUniqueNumber = im.ItemUniqueNumber,
                //    IsPurchase = (im.IsPurchase == true ? true : false),
                //    IsTransfer = (im.IsTransfer == true ? true : false),
                //    DefaultLocation = im.DefaultLocation ?? 0,
                //    DefaultLocationName = im.DefaultLocationName,
                //    InventoryClassification = im.InventoryClassification,
                //    SerialNumberTracking = im.SerialNumberTracking,
                //    LotNumberTracking = im.LotNumberTracking,
                //    DateCodeTracking = im.DateCodeTracking,
                //    ItemType = im.ItemType,
                //    ImagePath = im.ImagePath,
                //    UDF1 = im.UDF1,
                //    UDF2 = im.UDF2,
                //    UDF3 = im.UDF3,
                //    UDF4 = im.UDF4,
                //    UDF5 = im.UDF5,
                //    ItemUDF1 = im.ItemUDF1,
                //    ItemUDF2 = im.ItemUDF2,
                //    ItemUDF3 = im.ItemUDF3,
                //    ItemUDF4 = im.ItemUDF4,
                //    ItemUDF5 = im.ItemUDF5,
                //    UDF6 = im.UDF6,
                //    UDF7 = im.UDF7,
                //    UDF8 = im.UDF8,
                //    UDF9 = im.UDF9,
                //    UDF10 = im.UDF10,
                //    ItemUDF6 = im.ItemUDF6,
                //    ItemUDF7 = im.ItemUDF7,
                //    ItemUDF8 = im.ItemUDF8,
                //    ItemUDF9 = im.ItemUDF9,
                //    ItemUDF10 = im.ItemUDF10,
                //    GUID = im.GUID,
                //    Created = im.Created,
                //    Updated = im.Updated,
                //    CreatedBy = im.CreatedBy,
                //    LastUpdatedBy = im.LastUpdatedBy,
                //    IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                //    IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                //    CompanyID = im.CompanyID,
                //    Room = im.Room,
                //    CreatedByName = im.CreatedByName,
                //    UpdatedByName = im.UpdatedByName,
                //    RoomName = im.RoomName,
                //    IsLotSerialExpiryCost = im.IsLotSerialExpiryCost,
                //    CategoryName = im.CategoryName,
                //    Unit = im.Unit,
                //    GLAccount = im.GLAccount,
                //    IsItemLevelMinMaxQtyRequired = (im.IsItemLevelMinMaxQtyRequired.HasValue ? im.IsItemLevelMinMaxQtyRequired : false),
                //    IsBuildBreak = im.IsBuildBreak,
                //    BondedInventory = im.BondedInventory,
                //    PullQtyScanOverride = im.PullQtyScanOverride,
                //    TrendingSetting = im.TrendingSetting,
                //    IsAutoInventoryClassification = im.IsAutoInventoryClassification,
                //    IsEnforceDefaultReorderQuantity = (im.IsEnforceDefaultReorderQuantity.HasValue ? im.IsEnforceDefaultReorderQuantity : false),
                //    LastCost = im.LastCost,
                //    ItemImageExternalURL = im.ItemImageExternalURL,
                //    ItemLink2ExternalURL = im.ItemLink2ExternalURL,
                //    ItemLink2ImageType = im.ItemLink2ImageType,
                //    ImageType = im.ImageType,
                //    ItemDocExternalURL = im.ItemDocExternalURL,
                //    IsActive = im.IsActive,
                //    IsAllowOrderCostuom=im.IsAllowOrderCostuom,
                //    MonthlyAverageUsage = ((long)(im.AverageUsage.HasValue ? (long)im.AverageUsage : (long)0) * (long)MonthlyAverageUsage),
                //    OnOrderInTransitQuantity = im.OnOrderInTransitQuantity,

                //    InventoryClassificationName = im.InventoryClassificationName,
                //    IsBOMItem = im.IsBOMItem,
                //    RefBomId = im.RefBomId,
                //    CostUOMName = im.CostUOMName,
                //    IsPackslipMandatoryAtReceive = im.IsPackslipMandatoryAtReceive,
                //    BlanketOrderNumber = im.BlanketOrderNumber,                                
                //    OutTransferQuantity = im.OutTransferQuantity,
                //    SuggestedReturnQuantity = im.SuggestedReturnQuantity
                //}).AsParallel().ToList();

                //if (ObjCache != null && ObjCache.Count() > 0)
                //{
                //    Int64 RoomID = ObjCache.FirstOrDefault().Room ?? 0;
                //    Int64 CompanyID = ObjCache.FirstOrDefault().CompanyID ?? 0;
                //    if ((from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).Any())
                //    {
                //        MonthlyAverageUsage = (from dp in context.DashboardParameters where dp.RoomId == RoomID && dp.CompanyId == CompanyID select dp).FirstOrDefault().MonthlyAverageUsage ?? 30;
                //    }
                //    if (MonthlyAverageUsage == null)
                //    {
                //        MonthlyAverageUsage = 30;
                //    }
                //    ObjCache.ToList().ForEach(t => t.MonthlyAverageUsage = Convert.ToInt64(t.AverageUsage.GetValueOrDefault(0) * MonthlyAverageUsage));
                //}
            }

            #endregion
            //return ObjCache;

        }

        public IEnumerable<ItemMasterDTO> GetItemsBinByArray(List<Guid> ItemGUIDs, long RoomId, long CompanyId, string BinIDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string itemGuids = string.Empty;

                if (ItemGUIDs.Any())
                {
                    itemGuids = string.Join(",", ItemGUIDs);
                }

                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuids", itemGuids ?? (object)DBNull.Value) ,
                                                   new SqlParameter("@RoomId",RoomId),
                                                   new SqlParameter("@CompanyId", CompanyId),
                                                   new SqlParameter("@BinIDs",BinIDs)
                };
                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemBinByGuIDs] @ItemGuids,@RoomId,@CompanyId,@BinIDs", params1).ToList();
            }
        }

        public IEnumerable<ItemMasterDTO> GetItemsByArrayForImport(List<Guid> ItemGUIDs, long RoomId, long CompanyId, List<long> UserSupplierIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string itemGuids = string.Empty;

                if (ItemGUIDs.Any())
                {
                    itemGuids = string.Join(",", ItemGUIDs);
                }

                var userSupplierIds = string.Empty;

                if (UserSupplierIds.Any())
                {
                    userSupplierIds = string.Join(",", UserSupplierIds);
                }

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ItemGuids", itemGuids ?? (object)DBNull.Value),
                                                    new SqlParameter("@RoomId",RoomId),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@UserSupplierIds", userSupplierIds ?? (object)DBNull.Value),
                                                 };

                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemsByGuIDsForImport] @ItemGuids,@RoomId,@CompanyId,@UserSupplierIds ", params1).ToList();
            }
        }

        public IEnumerable<ItemMasterDTO> GetItemsBinByArrayForImport(List<Guid> ItemGUIDs, long RoomId, long CompanyId, List<long> UserSupplierIds, string BinIDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string itemGuids = string.Empty;

                if (ItemGUIDs.Any())
                {
                    itemGuids = string.Join(",", ItemGUIDs);
                }

                var userSupplierIds = string.Empty;

                if (UserSupplierIds.Any())
                {
                    userSupplierIds = string.Join(",", UserSupplierIds);
                }

                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ItemGuids", itemGuids ?? (object)DBNull.Value),
                                                    new SqlParameter("@RoomId",RoomId),
                                                    new SqlParameter("@CompanyId", CompanyId),
                                                    new SqlParameter("@UserSupplierIds", userSupplierIds ?? (object)DBNull.Value),
                                                    new SqlParameter("@BinIDs", BinIDs ?? (object)DBNull.Value),
                                                 };

                return context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemsBinByGuIDsForImport] @ItemGuids,@RoomId,@CompanyId,@UserSupplierIds,@BinIDs", params1).ToList();
            }
        }

        public bool updateLink2Name(long Id, string fileName, bool forBOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", Id),
                                                    new SqlParameter("@Link2", fileName),
                                                    new SqlParameter("@IsForBOM", forBOM)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemLink2ById] @Id,@Link2,@IsForBOM", params1);
                return true;
            }
        }

        public string GetItemSupplierDetails(Guid ItemGUID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID) };
                return context.Database.SqlQuery<string>("exec [GetItemSupplierBlanketPOByItemGuid] @ItemGUID ", params1).FirstOrDefault();
            }
        }

        public bool EditDate(Guid ItemGUID, string DateUpdateField)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ItemMaster obj = context.ItemMasters.Where(x => x.GUID == ItemGUID).FirstOrDefault();
                    switch (DateUpdateField)
                    {
                        case "EditPulledDate":

                            if (obj != null)
                            {
                                obj.pulleddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                        case "EditOrderedDate":
                            if (obj != null)
                            {
                                obj.ordereddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }

                            break;
                        case "EditCountedDate":
                            if (obj != null)
                            {
                                obj.counteddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                        case "EditTransferedDate":
                            if (obj != null)
                            {
                                obj.trasnfereddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditDateAndOnOrderInTransitQuantity(Guid ItemGUID, string DateUpdateField, double OnOrderInTransitQuantity)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ItemMaster obj = context.ItemMasters.Where(x => x.GUID == ItemGUID).FirstOrDefault();
                    switch (DateUpdateField)
                    {
                        case "EditPulledDate":

                            if (obj != null)
                            {
                                obj.pulleddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                        case "EditOrderedDate":
                            if (obj != null)
                            {
                                obj.ordereddate = DateTime.UtcNow;
                                obj.OnOrderInTransitQuantity = OnOrderInTransitQuantity;
                                context.SaveChanges();
                            }

                            break;
                        case "EditCountedDate":
                            if (obj != null)
                            {
                                obj.counteddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                        case "EditTransferedDate":
                            if (obj != null)
                            {
                                obj.trasnfereddate = DateTime.UtcNow;
                                context.SaveChanges();
                            }
                            break;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<ItemMasterDTO> GetActiveItemsLimitedFieldsByRoom(long RoomID, long CompanyID)
        {
            IEnumerable<ItemMasterDTO> ObjCache = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from im in context.ItemMasters
                            where im.CompanyID == CompanyID && im.Room == RoomID && (im.IsDeleted ?? false) == false && (im.IsArchived ?? false) == false
                            select new ItemMasterDTO
                            {
                                ID = im.ID,
                                ItemNumber = im.ItemNumber,
                                GUID = im.GUID,
                                ItemUniqueNumber = im.ItemUniqueNumber
                            }).AsParallel().ToList();
            }

            return ObjCache;
        }

        public bool updateImagePath(long Id, string fileName, bool forBOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@Id", Id),
                                                    new SqlParameter("@ImagePath", fileName),
                                                    new SqlParameter("@IsForBOM", forBOM)
                                                };

                context.Database.ExecuteSqlCommand("EXEC [UpdateItemImagePathById] @Id,@ImagePath,@IsForBOM", params1);
                return true;
            }
        }

        public string GetnonConsigneditemsbeforeDelete(string Guids)
        {
            string returnGUIDs = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGuids", Guids) };
                var lstBins = context.Database.SqlQuery<Guid>("exec [GetNonConsignedItemsByItemGuids] @ItemGuids ", params1).ToList();

                if (lstBins.Any())
                {
                    returnGUIDs = string.Join(",", lstBins);
                }
                return returnGUIDs;
            }
        }

        public void CleanDeletedItemRefs(long RoomID, long CompanyID)
        {
            string returnGUIDs = string.Empty;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC CleanDeletedItemRefs " + RoomID + "," + CompanyID);
            }
        }

        public SupplierMasterDTO GetRoomDefaultSupplier(long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.SupplierMasters
                        join rm in context.Rooms on new { supid = u.ID } equals new { supid = rm.DefaultSupplierID ?? 0 }
                        where rm.ID == RoomID
                        select new SupplierMasterDTO
                        {
                            ID = u.ID,
                            SupplierName = u.SupplierName,
                            SupplierColor = u.SupplierColor,
                            Description = u.Description,
                            //AccountNo = u.AccountNo,
                            ReceiverID = u.ReceiverID,
                            Address = u.Address,
                            City = u.City,
                            State = u.State,
                            ZipCode = u.ZipCode,
                            Country = u.Country,
                            Contact = u.Contact,
                            Phone = u.Phone,
                            Fax = u.Fax,
                            Email = u.Email,
                            IsEmailPOInBody = u.IsEmailPOInBody,
                            IsEmailPOInPDF = u.IsEmailPOInPDF,
                            IsEmailPOInCSV = u.IsEmailPOInCSV,
                            IsEmailPOInX12 = u.IsEmailPOInX12,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            GUID = u.GUID,
                            IsDeleted = u.IsDeleted,
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            BranchNumber = u.BranchNumber,
                            MaximumOrderSize = u.MaximumOrderSize,
                            IsSendtoVendor = u.IsSendtoVendor,
                            IsVendorReturnAsn = u.IsVendorReturnAsn,
                            IsSupplierReceivesKitComponents = u.IsSupplierReceivesKitComponents,
                            POAutoSequence = u.POAutoSequence,
                            ScheduleType = u.ScheduleType,
                            Days = u.Days,
                            WeekDays = u.WeekDays,
                            MonthDays = u.MonthDays,
                            ScheduleTime = u.ScheduleTime,
                            IsAutoGenerate = u.IsAutoGenerate,
                            IsAutoGenerateSubmit = u.IsAutoGenerateSubmit,
                            isForBOM = u.isForBOM,
                            RefBomId = u.RefBomId,
                            NextOrderNo = u.NextOrderNo,
                            PullPurchaseNumberType = u.PullPurchaseNumberType,
                            LastPullPurchaseNumberUsed = u.LastPullPurchaseNumberUsed,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb,
                            SupplierImage = u.SupplierImage,
                            ImageType = u.ImageType,
                            ImageExternalURL = u.ImageExternalURL
                        }).FirstOrDefault();

            }
        }

        public BinMasterDTO GetRoomDefaultBin(long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.BinMasters
                        join rm in context.Rooms on new { supid = u.ID } equals new { supid = rm.DefaultBinID ?? 0 }
                        where rm.ID == RoomID
                        select new BinMasterDTO
                        {
                            ID = u.ID,
                            BinNumber = u.BinNumber,
                            Created = u.Created,
                            LastUpdated = u.LastUpdated,
                            IsStagingLocation = u.IsStagingLocation,
                            IsStagingHeader = u.IsStagingHeader,
                            MaterialStagingGUID = u.MaterialStagingGUID,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            Room = u.Room,
                            CompanyID = u.CompanyID,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived ?? false,
                            GUID = u.GUID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            ItemGUID = u.ItemGUID,
                            MinimumQuantity = u.MinimumQuantity,
                            MaximumQuantity = u.MaximumQuantity,
                            CriticalQuantity = u.CriticalQuantity,
                            SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                            IsDefault = u.IsDefault,
                            AddedFrom = u.AddedFrom,
                            EditedFrom = u.EditedFrom,
                            ReceivedOn = u.ReceivedOn,
                            ReceivedOnWeb = u.ReceivedOnWeb
                        }).FirstOrDefault();
            }
        }


        #endregion

        public bool UpdateOnHandQuantity(Guid ItemGuid, double OnHandQuantity)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster ObjItemMaster = (from I in context.ItemMasters
                                            where I.GUID == ItemGuid
                                            select I).FirstOrDefault();

                if (ObjItemMaster != null)
                {
                    ObjItemMaster.OnHandQuantity = OnHandQuantity;
                    context.SaveChanges();
                }

                return true;
            }
        }

        public int GetNegativeQuantityItemCountByRoomId(long RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var lstItemQty = (from ILD in context.ItemLocationDetails
                                  join I in context.ItemMasters on ILD.ItemGUID equals I.GUID
                                  join B in context.BinMasters on new { ItemGUID = ILD.ItemGUID, BinId = (long)ILD.BinID } equals new { ItemGUID = B.ItemGUID, BinId = B.ID }
                                  where I.Room == RoomId
                                        && (ILD.IsDeleted == null || ILD.IsDeleted == false)
                                        && (I.IsDeleted == null || I.IsDeleted == false)
                                        && (B.IsDeleted == false)
                                  select new
                                  {
                                      ItemGUID = I.GUID,
                                      BinId = ILD.BinID,
                                      CustomerOwnedQuantity = (ILD.CustomerOwnedQuantity == null ? 0 : ILD.CustomerOwnedQuantity),
                                      ConsignedQuantity = (ILD.ConsignedQuantity == null ? 0 : ILD.ConsignedQuantity)
                                  }).ToList();

                int NegativeQtyItmCount = (from I in lstItemQty
                                           group I by new { I.ItemGUID, I.BinId } into G
                                           select new
                                           {
                                               ItemGUID = G.Key.ItemGUID,
                                               BinId = G.Key.BinId,
                                               TotalQty = G.Sum(x => x.CustomerOwnedQuantity + x.ConsignedQuantity)
                                           }).Where(x => x.TotalQty < 0).Count();

                return NegativeQtyItmCount;
            }
        }

        public List<string> DeleteItemData(string ItemGuid, long UserID, string WhatWhereAction)
        {
            List<string> DeletedBinIds = null;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                DeletedBinIds = context.Database.SqlQuery<string>("EXEC USP_DeleteItemData {0}, {1}, {2}", ItemGuid, UserID, WhatWhereAction).ToList();
            }
            return DeletedBinIds;
        }

        public double? GetItemPriceByRoomModuleSettings(long CompanyId, long RoomId, long ModuleId, Guid ItemGuid, bool ConsiderCostUOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                //----------------------------------------------------------------------------------------
                //
                int? PriseSelectionOption = 0;
                PriseSelectionOption = (from RMS in context.RoomModuleSettings
                                        join M in context.ModuleMasters on RMS.ModuleId equals M.ID
                                        where RMS.CompanyId == CompanyId
                                               && RMS.RoomId == RoomId
                                               && RMS.ModuleId == ModuleId
                                        select RMS.PriseSelectionOption).FirstOrDefault();

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                    PriseSelectionOption = 1;


                var Result = (from IM in context.ItemMasters
                              join cuom in context.CostUOMMasters
                              on new { CostUOMID = (IM.CostUOMID == null ? 0 : (long)IM.CostUOMID), IsDeleted = false, IsArchived = false }
                              equals new { CostUOMID = cuom.ID, IsDeleted = (cuom.IsDeleted == null ? false : (bool)cuom.IsDeleted), IsArchived = (cuom.IsArchived == null ? false : (bool)cuom.IsArchived) } into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where IM.GUID == ItemGuid
                                    && IM.CompanyID == CompanyId
                                    && IM.Room == RoomId
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                                    && (IM.IsArchived == null || IM.IsArchived == false)
                              select new
                              {
                                  Cost = (IM.Cost == null ? 0 : IM.Cost),
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice),
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).FirstOrDefault();

                if (PriseSelectionOption == 1)
                {
                    if (ConsiderCostUOM == true)
                        return Result.SellPrice / Result.CostUOMValue;
                    else
                        return Result.SellPrice;
                }
                else
                {
                    if (ConsiderCostUOM == true)
                        return Result.Cost / Result.CostUOMValue;
                    else
                        return Result.Cost;
                }
            }
        }

        public List<ItemMasterDTO> GetAllItemByBOMRefID(long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemMasterDTO> objItemList = new List<ItemMasterDTO>();
                objItemList = (from im in context.ItemMasters
                               where im.CompanyID == CompanyID && im.IsBOMItem == false && im.RefBomId != null
                               select new ItemMasterDTO
                               {
                                   ID = im.ID,
                                   ItemNumber = im.ItemNumber,
                                   GUID = im.GUID,
                                   Room = im.Room,
                                   CompanyID = im.CompanyID,
                                   RefBomId = im.RefBomId
                               }).ToList();

                return objItemList.Where(x => x.RefBomId.GetValueOrDefault(0) > 0).ToList();
            }
        }

        public List<ItemMasterDTO> GetAllItemByBOMRefID(long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<ItemMasterDTO> objItemList = new List<ItemMasterDTO>();
                objItemList = (from im in context.ItemMasters
                               where im.CompanyID == CompanyID && im.Room == RoomID && im.IsBOMItem == false && im.RefBomId != null && im.IsDeleted == false && im.IsArchived == false
                               select new ItemMasterDTO
                               {
                                   ID = im.ID,
                                   ItemNumber = im.ItemNumber,
                                   GUID = im.GUID,
                                   Room = im.Room,
                                   CompanyID = im.CompanyID,
                                   RefBomId = im.RefBomId
                               }).ToList();

                return objItemList.Where(x => x.RefBomId.GetValueOrDefault(0) > 0).ToList();
            }
        }

        public string GetItemsByReportRange(string _range, string _rangeFieldID, string _rangeData, string RoomIDs, string CompanyIDs, bool _isSelectAllRangeData = false)
        {

            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_rangeFieldID))
                return string.Empty;

            string _dataGuids = string.Empty;


            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@range",_range)
                                    , new SqlParameter("@rangeData",_rangeData)
                                    , new SqlParameter("@rangeFieldID",_rangeFieldID)
                                    , new SqlParameter("@roomid",RoomIDs)
                                    , new SqlParameter("@companyid",CompanyIDs)
                                    , new SqlParameter("@IsSelectAllRangeData",_isSelectAllRangeData)};

                string qry = "exec [Schl_GetItemByReportRange] @range, @rangeData,@rangeFieldID, @roomid, @companyid, @IsSelectAllRangeData";
                List<Guid> lstGuids = context.Database.SqlQuery<Guid>(qry, params1).ToList();
                if (lstGuids != null && lstGuids.Any())
                {
                    _dataGuids = string.Join(",", lstGuids.Select(t => t).ToArray());
                }

                return _dataGuids;
            }
        }

        #region [for service]        

        public IEnumerable<ItemMasterDTO> GetItemListRoomWise(Int64 RoomID, Int64 CompanyID)
        {
            IEnumerable<ItemMasterDTO> ObjCache;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ObjCache = (from im in context.ItemMasters
                            where im.CompanyID == CompanyID && im.Room == RoomID
                            && im.SerialNumberTracking == false
                            && im.LotNumberTracking == false && im.DateCodeTracking == false && (im.IsArchived ?? false) == false && (im.IsDeleted ?? false) == false
                            orderby im.ID descending

                            select new ItemMasterDTO
                            {
                                ID = im.ID,
                                ItemNumber = im.ItemNumber,
                                WeightPerPiece = im.WeightPerPiece,
                                GUID = im.GUID,
                                IsDeleted = (im.IsDeleted.HasValue ? im.IsDeleted : false),
                                IsArchived = (im.IsArchived.HasValue ? im.IsArchived : false),
                                CompanyID = im.CompanyID,
                                Room = im.Room
                            }).AsParallel().ToList();
            }

            //            using (var context = new eTurnsEntities(ConnectionString))
            //            {
            //                ObjCache = (from u in context.Database.SqlQuery<ItemMasterDTO>(@"SELECT A.ItemNumber,A.Room,A.CompanyID,A.IsDeleted,A.IsArchived,A.ID,A.GUID,A.WeightPerPiece
            //                                            FROM ItemMaster A 
            //                                            WHERE A.CompanyID = " + CompanyID.ToString() + "AND A.Room = " + RoomID.ToString() + " And A.SerialNumberTracking = 0 And A.LotNumberTracking = 0 And A.DateCodeTracking = 0 ORDER BY ID DESC")
            //                            select new ItemMasterDTO
            //                            {
            //                                ID = u.ID,
            //                                ItemNumber = u.ItemNumber,
            //                                WeightPerPiece = u.WeightPerPiece,
            //                                GUID = u.GUID,
            //                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
            //                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
            //                                CompanyID = u.CompanyID,
            //                                Room = u.Room
            //                            }).AsParallel().ToList();
            //            }
            return ObjCache; //.Where(t => t.Room == RoomID);
        }

        public string CheckItemExistsForInActive(Guid ItemGuid, long RoomId, long CompanyId, ref bool Result, long EnterpriseId, string CultureCode)
        {
            string ErrorMessage = string.Empty;
            string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];
            var itemMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResItemMaster", CultureCode, EnterpriseId, CompanyId);
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                string msgConnectionNotAvailable = ResourceRead.GetResourceValueByKeyAndFullFilePath("ConnectionNotAvailable", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                return msgConnectionNotAvailable;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            DataSet dsBins = SqlHelper.ExecuteDataset(EturnsConnection, "CheckBeforeItemInActive", ItemGuid, RoomId, CompanyId);
            if (dsBins != null && dsBins.Tables.Count > 0)
            {
                DataTable dt = dsBins.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dt.Columns.Contains("OrderCount"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["OrderCount"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgOrderAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("OrderAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgOrderAlreadyExistForItem + " ";
                            }
                        }
                        if (dt.Columns.Contains("QuoteCount"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["QuoteCount"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgMsgItemInUsedOfQuotes = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgItemInUsedOfQuotes", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgMsgItemInUsedOfQuotes;
                            }
                        }
                        if (dt.Columns.Contains("RequisitionCount"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["RequisitionCount"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgRequisitionAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("RequisitionAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgRequisitionAlreadyExistForItem + " ";
                            }
                        }
                        if (dt.Columns.Contains("TransferCount"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["TransferCount"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgTransferAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("TransferAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgTransferAlreadyExistForItem + " ";
                            }
                        }
                        if (dt.Columns.Contains("WorkOrderCount"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["WorkOrderCount"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgWorkorderAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("WorkorderAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgWorkorderAlreadyExistForItem + " ";
                            }
                        }
                        if (dt.Columns.Contains("KitMoveInOut"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["KitMoveInOut"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                Result = false;
                                string msgKitAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("KitAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgKitAlreadyExistForItem + " ";
                            }
                        }
                        if (dt.Columns.Contains("CountQuantity"))
                        {
                            Int64 tmpdbl = 0;
                            Int64.TryParse(Convert.ToString(dr["CountQuantity"]), out tmpdbl);
                            if (tmpdbl > 0)
                            {
                                string msgCountAlreadyExistForItem = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountAlreadyExistForItem", itemMasterResourceFilePath, EnterpriseId, CompanyId, RoomId, "ResItemMaster", CultureCode);
                                ErrorMessage += msgCountAlreadyExistForItem + " ";
                                Result = false;
                            }
                        }
                    }
                }
            }

            return ErrorMessage;
        }

        #endregion

        public List<ItemMasterDTO> GetPagedItemsForCreditPageForCreditRule(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool CanSeeConsignItems, bool CanOrderConsignItems, bool CanUseConsignedQuantity, long LoggedInUserId, string RoomDateFormat, TimeZoneInfo CurrentTimeZone,
                                                         bool IsQLInclude = false, string ExclueItemGUIDs = null, string InclueItemGUIDs = null)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            DataSet dsItems = new DataSet();

            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }
            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string StagingIds = null;
            string StagingBinNumbers = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string Cost = null;
            string Cost1 = null;
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPageForCreditRule", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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
                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[26]))
                {
                    StagingIds = FieldsPara[26].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[23]))
                {
                    ItemSuppliers = Convert.ToString(FieldsPara[23]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[43]))
                {
                    StagingBinNumbers = FieldsPara[43].TrimEnd(',');
                }
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPageForCreditRule", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }
            else
            {
                dsItems = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForCreditPageForCreditRule", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, ItemCreaters, ItemUpdators, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, CanSeeConsignItems, CanOrderConsignItems, CanUseConsignedQuantity, LoggedInUserId, IsQLInclude, ExclueItemGUIDs, InclueItemGUIDs, StagingIds, StockStatus, UDF6, UDF7, UDF8, UDF9, UDF10, StagingBinNumbers);
            }

            if (dsItems != null && dsItems.Tables.Count > 0)
            {
                DataTable dtItems = dsItems.Tables[0];

                if (dtItems.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtItems.Rows[0]["TotalRecords"]);
                    lstItems = dtItems.AsEnumerable().Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        //Link2 = row.Field<string>("Link2"),
                        Trend = row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = row.Field<bool>("IsAutoInventoryClassification"),
                        Taxable = row.Field<bool>("Taxable"),
                        Consignment = row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = row.Field<bool?>("IsPurchase") ?? false,
                        IsTransfer = row.Field<bool?>("IsTransfer") ?? false,
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = row.Field<bool>("SerialNumberTracking"),
                        LotNumberTracking = row.Field<bool>("LotNumberTracking"),
                        DateCodeTracking = row.Field<bool>("DateCodeTracking"),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImageType") == "ImagePath" ? row.Field<string>("ImagePath") : row.Field<string>("ImageType") == "ExternalImage" ? row.Field<string>("ItemImageExternalURL") : "",
                        Link2 = row.Field<string>("ItemLink2ImageType") == "InternalLink" ? row.Field<string>("Link2") : row.Field<string>("ItemLink2ImageType") == "ExternalURL" ? row.Field<string>("ItemLink2ExternalURL") : "",
                        ImageType = row.Field<string>("ImageType"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF1"),
                        UDF7 = row.Field<string>("UDF2"),
                        UDF8 = row.Field<string>("UDF3"),
                        UDF9 = row.Field<string>("UDF4"),
                        UDF10 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        xmlItemLocations = row.Field<string>("ItemLocations"),
                        QuickListGUID = Convert.ToString(row.Field<Guid?>("QuickListGUID")),
                        QuickListName = row.Field<string>("QuickListName"),
                        AddedFrom = Convert.ToString(row.Field<string>("AddedFrom")),
                        EditedFrom = Convert.ToString(row.Field<string>("EditedFrom")),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity")
                    }).ToList();
                }
            }
            return lstItems;
        }

        public ItemOrderInfoDTO GetItemOnOrderQuantity(Guid ItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemOrderInfoDTO>("EXEC [dbo].[Get_Item_OnOrderQuantity] @ItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }

        }

        public ItemMasterDTO GetItemByGuidPlain(Guid ItemGuid, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", ItemGuid),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidPlain @ItemGUID,@RoomId,@CompanyId", params1).FirstOrDefault();
            }
        }

        public ItemMasterDTO GetItemByGuidNormal(Guid ItemGuid, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", ItemGuid),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidNormal @ItemGUID,@RoomId,@CompanyId ", params1).FirstOrDefault();
            }
        }

        public List<ItemMasterDTO> GetItemByGuidsPlain(string ItemGuids, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUIDs", ItemGuids ?? string.Empty),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidsPlain @ItemGUIDs,@RoomId,@CompanyId", params1).ToList();
            }
        }

        public List<ItemMasterDTO> GetItemByGuidsNormal(string ItemGuids, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUIDs", ItemGuids ?? string.Empty),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidsNormal @ItemGUIDs,@RoomId,@CompanyId", params1).ToList();
            }
        }

        #region 4919 - Bin level MinMax item did not send stockout scheduled report

        public List<ItemStockOutMailLogDTO> GetItemStockOuteMailtoSend(long EnterPriceID, string EnterpriseDBName, long CompanyId, long RoomId, long MasterNotificationID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string eTurnsMasterDBName = DbConnectionHelper.GetETurnsMasterDBName();
                DateTime LastRunDate = DateTime.UtcNow;
                ItemStockOutMailLogDTO objReportMailLog = new ItemStockOutMailLogDTO();

                var paramsmaster1 = new SqlParameter[] {  new SqlParameter("@EnterPriceID", EnterPriceID),
                                                          new SqlParameter("@CompanyId", CompanyId),
                                                         new SqlParameter("@RoomId", RoomId),
                                                         new SqlParameter("@MasterNotificationID", MasterNotificationID)};
                objReportMailLog = context.Database.SqlQuery<ItemStockOutMailLogDTO>("EXEC [" + eTurnsMasterDBName + "].[dbo].[GetLastRunDateFromItemStockOutMailSendHistory] @EnterPriceID,@CompanyId,@RoomId,@MasterNotificationID", paramsmaster1).FirstOrDefault();
                if (objReportMailLog != null && objReportMailLog.StockoutDate != null)
                {
                    LastRunDate = objReportMailLog.StockoutDate;
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompanyId", (CompanyId > 0 ? CompanyId : (object)DBNull.Value))
                                                  , new SqlParameter("@RoomId", (RoomId > 0 ? RoomId : (object)DBNull.Value))
                                                  , new SqlParameter("@LastRunDate", LastRunDate)
                                                };
                return context.Database.SqlQuery<ItemStockOutMailLogDTO>("EXEC [" + EnterpriseDBName + "].[dbo].[GetItemStockOuteMailtoSend] @CompanyId,@RoomId,@LastRunDate", params1).ToList();
            }
        }

        public List<ItemStockOutMailLogDTO> GetItemStockOuteMailtoSendByItemGuid(long EnterPriceID, string EnterpriseDBName, long CompanyId, long RoomId, long MasterNotificationID, Guid ItemGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string eTurnsMasterDBName = DbConnectionHelper.GetETurnsMasterDBName();
                DateTime LastRunDate = DateTime.UtcNow;
                ItemStockOutMailLogDTO objReportMailLog = new ItemStockOutMailLogDTO();

                var paramsmaster1 = new SqlParameter[] {  new SqlParameter("@EnterPriceID", EnterPriceID),
                                                          new SqlParameter("@CompanyId", CompanyId),
                                                         new SqlParameter("@RoomId", RoomId),
                                                         new SqlParameter("@MasterNotificationID", MasterNotificationID)};
                objReportMailLog = context.Database.SqlQuery<ItemStockOutMailLogDTO>("EXEC [" + eTurnsMasterDBName + "].[dbo].[GetLastRunDateFromItemStockOutMailSendHistory] @EnterPriceID,@CompanyId,@RoomId,@MasterNotificationID", paramsmaster1).FirstOrDefault();
                if (objReportMailLog != null && objReportMailLog.StockoutDate != null)
                {
                    LastRunDate = objReportMailLog.StockoutDate;
                }

                var params1 = new SqlParameter[] { new SqlParameter("@CompanyId", (CompanyId > 0 ? CompanyId : (object)DBNull.Value))
                                                  , new SqlParameter("@RoomId", (RoomId > 0 ? RoomId : (object)DBNull.Value))
                                                  , new SqlParameter("@ItemGuid", ItemGuid)
                                                  , new SqlParameter("@LastRunDate", LastRunDate)
                                                };
                return context.Database.SqlQuery<ItemStockOutMailLogDTO>("EXEC [" + EnterpriseDBName + "].[dbo].[GetItemStockOuteMailtoSendByItemGuid] @CompanyId,@RoomId,@ItemGuid,@LastRunDate", params1).ToList();
            }
        }

        #endregion

        #region WI-4986

        public double? CalculateAndGetItemSellPrice(Guid ItemGUID, double? ItemSellPrice, Int64 RoomID, Int64 CompanyID)
        {
            double? newItemSellPrice = null;
            int costUOMValue = 1;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                costUOMValue = (from im in context.ItemMasters
                                join uom in context.CostUOMMasters on im.CostUOMID equals uom.ID
                                where im.CompanyID == CompanyID && im.Room == RoomID && im.GUID == ItemGUID
                                select uom.CostUOMValue).FirstOrDefault().GetValueOrDefault(0);
            }

            if (costUOMValue == 0)
                costUOMValue = 1;

            if (ItemSellPrice.HasValue)
                newItemSellPrice = ItemSellPrice.Value / costUOMValue;

            return newItemSellPrice;
        }

        public double? GetItemCostByRoomModuleSettings(long CompanyId, long RoomId, long ModuleId, Guid ItemGuid, bool ConsiderCostUOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var Result = (from IM in context.ItemMasters
                              join cuom in context.CostUOMMasters
                              on new { CostUOMID = (IM.CostUOMID == null ? 0 : (long)IM.CostUOMID), IsDeleted = false, IsArchived = false }
                              equals new { CostUOMID = cuom.ID, IsDeleted = (cuom.IsDeleted == null ? false : (bool)cuom.IsDeleted), IsArchived = (cuom.IsArchived == null ? false : (bool)cuom.IsArchived) } into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where IM.GUID == ItemGuid
                                    && IM.CompanyID == CompanyId
                                    && IM.Room == RoomId
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                                    && (IM.IsArchived == null || IM.IsArchived == false)
                              select new
                              {
                                  Cost = (IM.Cost == null ? 0 : IM.Cost),
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice),
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).FirstOrDefault();

                if (Result != null)
                {
                    if (ConsiderCostUOM == true)
                        return Result.Cost / (Result.CostUOMValue ?? 1);
                    else
                        return Result.Cost;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double? GetItemSellPriceByRoomModuleSettings(long CompanyId, long RoomId, long ModuleId, Guid ItemGuid, bool ConsiderCostUOM)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var Result = (from IM in context.ItemMasters
                              join cuom in context.CostUOMMasters
                              on new { CostUOMID = (IM.CostUOMID == null ? 0 : (long)IM.CostUOMID), IsDeleted = false, IsArchived = false }
                              equals new { CostUOMID = cuom.ID, IsDeleted = (cuom.IsDeleted == null ? false : (bool)cuom.IsDeleted), IsArchived = (cuom.IsArchived == null ? false : (bool)cuom.IsArchived) } into cuom_join
                              from CUOM in cuom_join.DefaultIfEmpty()
                              where IM.GUID == ItemGuid
                                    && IM.CompanyID == CompanyId
                                    && IM.Room == RoomId
                                    && (IM.IsDeleted == null || IM.IsDeleted == false)
                                    && (IM.IsArchived == null || IM.IsArchived == false)
                              select new
                              {
                                  Cost = (IM.Cost == null ? 0 : IM.Cost),
                                  SellPrice = (IM.SellPrice == null ? 0 : IM.SellPrice),
                                  CostUOMValue = (CUOM == null || CUOM.CostUOMValue == null || CUOM.CostUOMValue == 0 ? 1 : CUOM.CostUOMValue)
                              }).FirstOrDefault();
                if (Result != null)
                {
                    if (ConsiderCostUOM == true)
                        return Result.SellPrice / Result.CostUOMValue;
                    else
                        return Result.SellPrice;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// This method is used to get the count of
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public long GetCountOfAutoClassificationItemsInRoom(long CompanyId, long RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Where(e => e.IsAutoInventoryClassification == true && (e.CompanyID ?? 0) == CompanyId
                                             && (e.Room ?? 0) == RoomId && (e.IsDeleted ?? false) == false).Count();
            }
        }

        public long getItemStockOutHistory(long CompanyId, long RoomId, string ItemGuids, DateTime FromDate)
        {
            var param = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomId),
                                                new SqlParameter("@CompanyID", CompanyId),
                                                new SqlParameter("@FromDate", FromDate.ToString("yyyy-MM-dd")),
                                                new SqlParameter("@ItemGuids", ItemGuids)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<long>("exec [GetItemStockOutHistoryPlain] @RoomID,@CompanyID,@FromDate,@ItemGuids", param).FirstOrDefault();
            }
        }

        #region Item List Narrow Search

        public List<NarrowSearchDTO> GetItemsListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemsListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount", params1).ToList();
            }
        }

        public List<NarrowSearchDTO> GetItemsBinListNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemsBinListNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount", params1).ToList();
            }
        }

        #region Items list for popup Narrow Search

        public List<NarrowSearchDTO> GetItemPopupNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, string ItemPopupFor, int? QuicklistType = null)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@ItemPopupFor", ItemPopupFor.ToLower() ?? (object)DBNull.Value),
                                                new SqlParameter("@QuicklistType", QuicklistType ?? (object)DBNull.Value)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemPopupNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@ItemPopupFor,@QuicklistType", params1).ToList();
            }
        }

        #endregion

        #region Items list for popup Narrow Search for Transfer

        public List<NarrowSearchDTO> GetItemsForTransferPopupNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, Int64 ParentID)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@ParentID", ParentID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemsForTransferPopupNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@ParentID", params1).ToList();
            }
        }

        #endregion

        #region Items list for popup Narrow Search for Count

        public List<NarrowSearchDTO> GetItemCountPopupNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, bool ShowStagingLocation, Guid CountGUID)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@ShowStagingLocation", ShowStagingLocation),
                                                new SqlParameter("@CountGUID", CountGUID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemCountPopupNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@ShowStagingLocation,@CountGUID", params1).ToList();
            }
        }

        #endregion

        #region Items list for popup Narrow Search for Move Material

        public List<NarrowSearchDTO> GetItemMoveMTRPopupNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, int moveTypeValue)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@MoveType", moveTypeValue)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetItemMoveMTRPopupNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@MoveType", params1).ToList();
            }
        }

        #endregion

        #region Items list for popup Narrow Search for Project Spend

        public List<NarrowSearchDTO> GetProjectSpendItemPopupNarrowSearch(long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, string NarrowSearchKey, List<long> UserSupplierIds, bool UserConsignmentAllowed, int LoadDataCount, Int64 ParentID)
        {
            string strSupplierIds = string.Empty;

            if (UserSupplierIds != null && UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", UserSupplierIds);
            }
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@NarrowSearchKey", NarrowSearchKey ?? (object)DBNull.Value),
                                                new SqlParameter("@SupplierIds", strSupplierIds),
                                                new SqlParameter("@UserConsignmentAllowed", UserConsignmentAllowed),
                                                new SqlParameter("@LoadDataCount", LoadDataCount),
                                                new SqlParameter("@ProjectSpendID", ParentID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<NarrowSearchDTO>("exec [GetProjectSpendItemPopupNarrowSearch] @RoomID,@CompanyID,@IsArchived,@IsDeleted,@NarrowSearchKey,@SupplierIds,@UserConsignmentAllowed,@LoadDataCount,@ProjectSpendID", params1).ToList();
            }
        }

        #endregion

        public IEnumerable<ItemMasterDTO> GetPagedItemsForCartNew(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName,
                                            long RoomID, long CompanyID, bool IsArchived, bool IsDeleted, List<long> SupplierIds, bool IsAllowConsigeItem,
                                            string CurrentTimeZone, string RoomDateFormat)
        {
            var cartItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            StartRowIndex = int.Parse(Math.Ceiling((double)(StartRowIndex / MaxRows)).ToString());
            string ItemSuppliers = null;
            string StockStatus = null;
            string ItemCreaters = null;
            string ItemUpdators = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string Manufacturers = null;
            string ItemCategory = null;
            string ItemType = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string Cost = null;
            string Cost1 = null;
            string AverageUsage = null;
            string AverageUsage1 = null;
            string Turns = null;
            string Turns1 = null;
            string strUserSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strUserSupplierIds = string.Join(",", SupplierIds);
            }

            if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');

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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    ItemCreaters = FieldsPara[0].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    ItemUpdators = FieldsPara[1].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), TimeZoneInfo.FromSerializedString(CurrentTimeZone)).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + "','";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + "','";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + "','";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + "','";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + "','";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + "','";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + "','";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + "','";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + "','";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + "','";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    StockStatus = Convert.ToString(FieldsPara[21]).TrimEnd();
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]);
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[31].Contains("100_1000"))
                    {
                        AverageUsage = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else if (FieldsPara[31].Contains("0.4_1000"))
                    {
                        AverageUsage = (Fields[1].Split('@')[31].Split('_')[0]);
                    }
                    else
                    {
                        AverageUsage = (Fields[1].Split('@')[31].Split('_')[0]);
                        AverageUsage1 = (Fields[1].Split('@')[31].Split('_')[1]);
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[32].Contains("20_1000"))
                    {
                        Turns = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else if (FieldsPara[32].Contains("1_1000"))
                    {
                        Turns = (Fields[1].Split('@')[32].Split('_')[0]);
                    }
                    else
                    {
                        Turns = (Fields[1].Split('@')[32].Split('_')[0]);
                        Turns1 = (Fields[1].Split('@')[32].Split('_')[1]);
                    }
                }
            }

            var params1 = new SqlParameter[] {
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@IsDeleted", IsDeleted),
                                                new SqlParameter("@IsArchived", IsArchived),
                                                new SqlParameter("@StartRowIndex", StartRowIndex),
                                                new SqlParameter("@MaxRows", MaxRows),
                                                new SqlParameter("@sortColumnName", sortColumnName),
                                                new SqlParameter("@SearchTerm", SearchTerm ?? string.Empty),
                                                new SqlParameter("@UserSupplierIds", strUserSupplierIds),
                                                new SqlParameter("@IsAllowConsigeItem", IsAllowConsigeItem),
                                                new SqlParameter("@SelectedSupplierIds", ItemSuppliers ?? string.Empty),
                                                new SqlParameter("@SelectedManufacturerIds", Manufacturers ?? string.Empty),
                                                new SqlParameter("@SelectedCategoryIds", ItemCategory ?? string.Empty),
                                                new SqlParameter("@Cost", Cost ?? string.Empty),
                                                new SqlParameter("@Cost1", Cost1 ?? string.Empty),
                                                new SqlParameter("@StockStatus", StockStatus ?? string.Empty),
                                                new SqlParameter("@AverageUsage", AverageUsage ?? string.Empty),
                                                new SqlParameter("@AverageUsage1", AverageUsage1 ?? string.Empty),
                                                new SqlParameter("@Turns", Turns ?? string.Empty),
                                                new SqlParameter("@Turns1", Turns1 ?? string.Empty),
                                                new SqlParameter("@ItemType", ItemType ?? string.Empty),
                                                new SqlParameter("@ItemCreaters", ItemCreaters ?? string.Empty),
                                                new SqlParameter("@ItemUpdators", ItemUpdators ?? string.Empty),
                                                new SqlParameter("@CreatedDateFrom", CreatedDateFrom ?? string.Empty),
                                                new SqlParameter("@CreatedDateTo", CreatedDateTo ?? string.Empty),
                                                new SqlParameter("@UpdatedDateFrom", UpdatedDateFrom ?? string.Empty),
                                                new SqlParameter("@UpdatedDateTo", UpdatedDateTo ?? string.Empty),
                                                new SqlParameter("@UDF1", UDF1 ?? string.Empty),
                                                new SqlParameter("@UDF2", UDF2 ?? string.Empty),
                                                new SqlParameter("@UDF3", UDF3 ?? string.Empty),
                                                new SqlParameter("@UDF4", UDF4 ?? string.Empty),
                                                new SqlParameter("@UDF5", UDF5 ?? string.Empty),
                                                new SqlParameter("@UDF6", UDF6 ?? string.Empty),
                                                new SqlParameter("@UDF7", UDF7 ?? string.Empty),
                                                new SqlParameter("@UDF8", UDF8 ?? string.Empty),
                                                new SqlParameter("@UDF9", UDF9 ?? string.Empty),
                                                new SqlParameter("@UDF10", UDF10 ?? string.Empty),
                                                };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                cartItems = context.Database.SqlQuery<ItemMasterDTO>("exec GetPagedItemsForCartNew @CompanyID,@RoomID,@IsDeleted,@IsArchived,@StartRowIndex,@MaxRows,@sortColumnName,@SearchTerm,@UserSupplierIds,@IsAllowConsigeItem,@SelectedSupplierIds,@SelectedManufacturerIds,@SelectedCategoryIds,@Cost,@Cost1,@StockStatus,@AverageUsage,@AverageUsage1,@Turns,@Turns1,@ItemType,@ItemCreaters,@ItemUpdators,@CreatedDateFrom,@CreatedDateTo,@UpdatedDateFrom,@UpdatedDateTo,@UDF1,@UDF2,@UDF3,@UDF4,@UDF5,@UDF6,@UDF7,@UDF8,@UDF9,@UDF10", params1).ToList();
                if (cartItems != null && cartItems.Count() > 0)
                {
                    TotalCount = cartItems.ElementAt(0).TotalRecords;
                }
            }

            return cartItems;
        }
        #endregion

        #region WI-5451 Add lot, serial and expiration fields to the pull import templates

        public List<ItemMasterDTO> GetItemByItemNumbers(List<string> lstItemNumbers, long CompanyID, long RoomID)
        {
            List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string ItemNumbers = string.Empty;

                foreach (string item in lstItemNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(ItemNumbers))
                    {
                        ItemNumbers = ItemNumbers + "," + Convert.ToString(item);
                    }
                    else
                    {
                        ItemNumbers = Convert.ToString(item);
                    }
                }

                var params1 = new SqlParameter[] { new SqlParameter("@ItemNumbers", ItemNumbers),
                                                   new SqlParameter("@RoomID", RoomID),
                                                   new SqlParameter("@CompanyID", CompanyID)};

                lstItemMaster = context.Database.SqlQuery<ItemMasterDTO>("exec [GetItemByItemNumbers] @ItemNumbers,@RoomID,@CompanyID", params1).ToList();
            }
            return lstItemMaster;
        }

        #endregion

        public List<ItemMasterDTO> GetAllItemsWithJoinsByIDs(long RoomID, long CompanyID, bool IsDeleted, bool IsArchived, string ItemIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@IsArchived", IsArchived),
                                               new SqlParameter("@IsDeleted", IsDeleted),
                                               new SqlParameter("@ItemIDs", ItemIDs) };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetAllItemsWithJoinsByItemIDs @RoomID,@CompanyID,@IsArchived,@IsDeleted,@ItemIDs", params1).ToList();
            }
        }

        public bool UpdateItemCostAndUOMOnly(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, double Cost, long CostUOMID, string WhatwhereAction, string EditedFrom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == ItemGUID);
                if (objItem != null)
                {
                    objItem.Cost = Cost;
                    objItem.CostUOMID = CostUOMID;
                    objItem.EditedFrom = EditedFrom;
                    objItem.WhatWhereAction = WhatwhereAction;
                    context.SaveChanges();
                }
            }

            return true;
        }

        public IEnumerable<ItemMasterDTO> GetItemBinPagedRecord(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, string RoomDateFormat, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, string callFrom = "")
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemLocations = null;
            string InventoryClassification = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string OnHandQuantity = null;
            string ItemTrackingType = null;
            string IsActive = null;
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, null, null, null, null, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                {
                    ItemLocations = FieldsPara[55].TrimEnd(',');
                    //if (callFrom == "ItemMaster")
                    //{
                    //    HttpContext.Current.Session["NSItemLocation"] = ItemLocations;
                    //}
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[87]))
                {
                    InventoryClassification = FieldsPara[87].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[89]))
                {
                    //if (FieldsPara[89] == "1")
                    //{
                    //    OnHandQuantity = ("Out of Stock");
                    //}
                    //else if (FieldsPara[89] == "2")
                    //{
                    //    OnHandQuantity = ("Below Critical");
                    //}
                    //else if (FieldsPara[89] == "3")
                    //{
                    //    OnHandQuantity = ("Below Min");
                    //}
                    //else if (FieldsPara[89] == "4")
                    //{
                    //    OnHandQuantity = ("Above Max");
                    //}

                    OnHandQuantity = FieldsPara[89].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    string[] arrReplenishTypes = FieldsPara[92].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    string[] arrReplenishTypes = FieldsPara[93].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    string[] arrReplenishTypes = FieldsPara[94].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    string[] arrReplenishTypes = FieldsPara[95].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    string[] arrReplenishTypes = FieldsPara[96].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[48].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[49].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[97]))
                {
                    IsActive = FieldsPara[97].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[88]))
                {
                    ItemTrackingType = FieldsPara[88].TrimEnd(',');
                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, ItemLocations, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItemBins", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = (row.Field<bool?>("Trend").HasValue ? row.Field<bool>("Trend") : false),// row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = (row.Field<bool?>("IsAutoInventoryClassification").HasValue ? row.Field<bool>("IsAutoInventoryClassification") : false),// row.Field<bool>("Trend"),
                        Taxable = (row.Field<bool?>("Taxable").HasValue ? row.Field<bool>("Taxable") : false), //row.Field<bool>("Taxable"),
                        Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),//   row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = (row.Field<bool?>("IsPurchase").HasValue ? row.Field<bool>("IsPurchase") : false), //row.Field<bool>("IsPurchase"),
                        IsTransfer = (row.Field<bool?>("IsTransfer").HasValue ? row.Field<bool>("IsTransfer") : false), //row.Field<bool>("IsTransfer"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        //DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        BinNumber = row.Field<string>("BinNumber"),
                        BinID = row.Field<long?>("BinID"),
                        BinGUID = row.Field<Guid?>("BinGUID"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = (row.Field<bool?>("SerialNumberTracking").HasValue ? row.Field<bool>("SerialNumberTracking") : false),
                        LotNumberTracking = (row.Field<bool?>("LotNumberTracking").HasValue ? row.Field<bool>("LotNumberTracking") : false),
                        DateCodeTracking = (row.Field<bool?>("DateCodeTracking").HasValue ? row.Field<bool>("DateCodeTracking") : false),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        //for item grid display purpose - CART, PUll  
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = (row.Field<bool?>("IsBuildBreak").HasValue ? row.Field<bool>("IsBuildBreak") : false),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = (row.Field<bool?>("IsBOMItem").HasValue ? row.Field<bool>("IsBOMItem") : false),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = (row.Field<bool?>("PullQtyScanOverride").HasValue ? row.Field<bool>("PullQtyScanOverride") : false),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        IsPackslipMandatoryAtReceive = row.Field<bool>("IsPackslipMandatoryAtReceive"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        BPONumber = row.Field<string>("BPONumber"),
                        TrasnferedDate = row.Field<DateTime?>("trasnfereddate"),
                        CountedDate = row.Field<DateTime?>("counteddate"),
                        OrderedDate = row.Field<DateTime?>("ordereddate"),
                        PulledDate = row.Field<DateTime?>("pulleddate"),
                        PriceSavedDate = row.Field<DateTime?>("PriceSavedDate"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        ItemDocExternalURL = row.Field<string>("ItemDocExternalURL"),
                        ImageType = row.Field<string>("ImageType"),
                        QtyToMeetDemand = row.Field<double?>("QtyToMeetDemand"),
                        //DefaultLocationGUID = row.Field<Guid?>("DefaultLocationGUID"),
                        OutTransferQuantity = row.Field<double?>("OutTransferQuantity"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        eVMISensorPort = row.Field<string>("eVMISensorPort"),
                        eVMISensorID = row.Field<double?>("eVMISensorID"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity"),
                        BinUDF1 = row.Field<string>("BinUDF1"),
                        BinUDF2 = row.Field<string>("BinUDF2"),
                        BinUDF3 = row.Field<string>("BinUDF3"),
                        BinUDF4 = row.Field<string>("BinUDF4"),
                        BinUDF5 = row.Field<string>("BinUDF5"),
                    }).ToList();
                }
            }
            return lstItems;
        }

        public EDILineItemDetailInfo UpdateItemCostAndCUOM(EDILineItemDetailInfo objEDILineItemDetailInfo)
        {
            if (objEDILineItemDetailInfo != null)
            {
                CostUOMMasterDAL objCostUOMMasterDAL = new CostUOMMasterDAL(base.DataBaseName);

                ItemMasterDTO objItemMaster = null;
                bool DoCostChanges = false;
                bool DoCUOMChanges = false;
                double ItmCost = 0;
                bool IsproperCost = double.TryParse(objEDILineItemDetailInfo.ItemCost, out ItmCost);
                if (!string.IsNullOrWhiteSpace(objEDILineItemDetailInfo.SupplierPartNo))
                {
                    objItemMaster = GetItemBySupplierPartNumberPlain(objEDILineItemDetailInfo.SupplierPartNo, objEDILineItemDetailInfo.RoomID, objEDILineItemDetailInfo.CompanyID);
                }
                if (objItemMaster == null || objItemMaster.ID < 1)
                {
                    if (!string.IsNullOrWhiteSpace(objEDILineItemDetailInfo.ItemNumber))
                    {
                        objItemMaster = GetItemByItemNumberPlain(objEDILineItemDetailInfo.ItemNumber, objEDILineItemDetailInfo.RoomID, objEDILineItemDetailInfo.CompanyID);
                    }
                }

                if (objItemMaster != null && objItemMaster.ID > 0)
                {
                    if (IsproperCost)
                    {
                        if (objItemMaster.Cost != ItmCost)
                        {
                            DoCostChanges = true;
                            objItemMaster.Cost = ItmCost;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(objEDILineItemDetailInfo.CostUOM))
                    {
                        CostUOMMasterDTO objCostUOMMasterDTO = new CostUOMMasterDTO();
                        CostUOMMasterDTO objCostUOMMasterByName = new CostUOMMasterDTO();
                        objCostUOMMasterDTO = objCostUOMMasterDAL.GetCostUOMByID(objItemMaster.CostUOMID ?? 0);
                        if (objCostUOMMasterDTO != null)
                        {
                            if (objEDILineItemDetailInfo.CostUOM.ToLower() != objCostUOMMasterDTO.CostUOM.ToLower())
                            {
                                objCostUOMMasterByName = objCostUOMMasterDAL.GetCostUOMByName(objEDILineItemDetailInfo.CostUOM, objEDILineItemDetailInfo.RoomID, objEDILineItemDetailInfo.CompanyID);
                                if (objCostUOMMasterByName == null || objCostUOMMasterByName.ID < 1)
                                {
                                    objCostUOMMasterByName = new CostUOMMasterDTO();
                                    objCostUOMMasterByName.AddedFrom = "edi";
                                    objCostUOMMasterByName.Action = "create";
                                    objCostUOMMasterByName.CompanyID = objEDILineItemDetailInfo.CompanyID;
                                    objCostUOMMasterByName.CostUOM = objEDILineItemDetailInfo.CostUOM;
                                    objCostUOMMasterByName.CostUOMValue = 1;
                                    objCostUOMMasterByName.Created = DateTime.UtcNow;
                                    objCostUOMMasterByName.CreatedBy = 2;
                                    objCostUOMMasterByName.EditedFrom = "edi";
                                    objCostUOMMasterByName.GUID = Guid.NewGuid();
                                    objCostUOMMasterByName.ID = 0;
                                    objCostUOMMasterByName.IsArchived = false;
                                    objCostUOMMasterByName.IsDeleted = false;
                                    objCostUOMMasterByName.isForBOM = false;
                                    objCostUOMMasterByName.LastUpdatedBy = 2;
                                    objCostUOMMasterByName.ReceivedOn = DateTime.UtcNow;
                                    objCostUOMMasterByName.ReceivedOnWeb = DateTime.UtcNow;
                                    objCostUOMMasterByName.RefBomId = null;
                                    objCostUOMMasterByName.Room = objEDILineItemDetailInfo.RoomID;
                                    objCostUOMMasterByName.Updated = DateTime.UtcNow;
                                    long CostUOMID = objCostUOMMasterDAL.Insert(objCostUOMMasterByName);
                                    objItemMaster.CostUOMID = CostUOMID;
                                    DoCUOMChanges = true;
                                }
                                else
                                {
                                    objItemMaster.CostUOMID = objCostUOMMasterByName.ID;
                                    DoCUOMChanges = true;
                                }
                            }
                            else
                            {
                                DoCUOMChanges = false;
                            }
                        }
                        else
                        {
                            objCostUOMMasterByName = objCostUOMMasterDAL.GetCostUOMByName(objEDILineItemDetailInfo.CostUOM, objEDILineItemDetailInfo.RoomID, objEDILineItemDetailInfo.CompanyID);
                            if (objCostUOMMasterByName == null || objCostUOMMasterByName.ID < 1)
                            {
                                objCostUOMMasterByName = new CostUOMMasterDTO();
                                objCostUOMMasterByName.AddedFrom = "edi";
                                objCostUOMMasterByName.Action = "create";
                                objCostUOMMasterByName.CompanyID = objEDILineItemDetailInfo.CompanyID;
                                objCostUOMMasterByName.CostUOM = objEDILineItemDetailInfo.CostUOM;
                                objCostUOMMasterByName.CostUOMValue = 1;
                                objCostUOMMasterByName.Created = DateTime.UtcNow;
                                objCostUOMMasterByName.CreatedBy = 2;
                                objCostUOMMasterByName.EditedFrom = "edi";
                                objCostUOMMasterByName.GUID = Guid.NewGuid();
                                objCostUOMMasterByName.ID = 0;
                                objCostUOMMasterByName.IsArchived = false;
                                objCostUOMMasterByName.IsDeleted = false;
                                objCostUOMMasterByName.isForBOM = false;
                                objCostUOMMasterByName.LastUpdatedBy = 2;
                                objCostUOMMasterByName.ReceivedOn = DateTime.UtcNow;
                                objCostUOMMasterByName.ReceivedOnWeb = DateTime.UtcNow;
                                objCostUOMMasterByName.RefBomId = null;
                                objCostUOMMasterByName.Room = objEDILineItemDetailInfo.RoomID;
                                objCostUOMMasterByName.Updated = DateTime.UtcNow;
                                long CostUOMID = objCostUOMMasterDAL.Insert(objCostUOMMasterByName);
                                objItemMaster.CostUOMID = CostUOMID;
                                DoCUOMChanges = true;
                            }
                            else
                            {
                                objItemMaster.CostUOMID = objCostUOMMasterByName.ID;
                                DoCUOMChanges = true;
                            }
                        }

                    }
                    else
                    {
                        DoCUOMChanges = false;
                    }
                }
                else
                {
                    DoCostChanges = false;
                    DoCUOMChanges = false;
                }

                if (DoCostChanges || DoCUOMChanges)
                {
                    objItemMaster.Updated = DateTime.UtcNow;
                    objItemMaster.EditedFrom = "edi832";
                    objItemMaster.WhatWhereAction = "EDI File Cost and CUOM update";
                    //Edit(objItemMaster, 2, false);
                }

            }
            return objEDILineItemDetailInfo;
        }

        public bool RemoveItemImage(Guid ItemGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveItemImage] '" + Convert.ToString(ItemGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }

        public bool RemoveItemLink(Guid ItemGUID, string EditedFrom, Int64 UserID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                bool bResult = false;
                try
                {
                    string strQuery = "EXEC [dbo].[RemoveItemLink] '" + Convert.ToString(ItemGUID) + "','" + EditedFrom + "'," + UserID + "";
                    context.Database.ExecuteSqlCommand(strQuery);
                    bResult = true;
                }
                catch
                {
                    bResult = false;
                }
                return bResult;
            }
        }
        public long? GetMaxQtyBinIdByItemGuid(Guid ItemGUID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {
                                                    new SqlParameter("@ItemGUID", ItemGUID)
                                                };
                return context.Database.SqlQuery<long?>("exec [GetMaxQtyBinIdByItemGuid] @ItemGUID ", params1).FirstOrDefault();
            }
        }

        public long? GetDefaultBinByItemGuid(Guid ItemGuid)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Where(e => e.GUID == ItemGuid).FirstOrDefault().DefaultLocation;
            }
        }

        public long? GetDefaultBinByItemNumberAndRoomId(string ItemNumber, long RoomId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.ItemMasters.Where(e => e.ItemNumber == ItemNumber && (e.Room ?? 0) == RoomId && (e.IsDeleted ?? false) == false).FirstOrDefault().DefaultLocation;
            }
        }

        public ItemMasterDTO GetItemSupplierForOrder(Guid ItemGuid, long SupplierId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", ItemGuid),
                                                new SqlParameter("@SupplierId", SupplierId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemSupplierForOrder @ItemGUID,@SupplierId ", params1).FirstOrDefault();
            }
        }
        public List<ItemMasterDTO> GetMissingmonthEndCalcItems(long RoomID, long CompanyID, int ForMonth, int ForYear)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@RoomID", RoomID),
                                                new SqlParameter("@CompanyID", CompanyID),
                                                new SqlParameter("@ForMonth", ForMonth),
                                                new SqlParameter("@ForYear", ForYear)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetMissingmonthEndCalcItems @RoomID,@CompanyID,@ForMonth,@ForYear ", params1).ToList();
            }
        }

        public ItemMasterDTO GetItemByGuidNormalForKit(Guid ItemGuid)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", ItemGuid),
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidNormalForKit @ItemGUID ", params1).FirstOrDefault();
            }
        }
        //

        #region Quote
        public bool EditDateAndOnQuotedQuantity(Guid ItemGUID, Int64 RoomID, Int64 CompanyID, double OnQuotedQuantity)
        {
            try
            {
                var params1 = new SqlParameter[] {
                    new SqlParameter("@ItemGUID", ItemGUID) ,
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@CompanyID", CompanyID),
                    new SqlParameter("@OnQuotedQuantity", OnQuotedQuantity)
                };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    context.Database.ExecuteSqlCommand("exec [UpdateItemonQuotedQuantity] @ItemGUID,@RoomID,@CompanyID,@OnQuotedQuantity", params1);
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public ItemQuoteInfoDTO GetItemQuotedQuantity(Guid ItemGUID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemQuoteInfoDTO>("EXEC [dbo].[GetItemQuotedQuantity] @ItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }


        #endregion

        public IEnumerable<ItemMasterDTO> GetPagedItemsForMoveMaterial(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomID, long CompanyID
            , bool IsArchived, bool IsDeleted, string RoomDateFormat, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, int MoveType, bool CanSeeConsignedItem)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemLocations = null;
            string InventoryClassification = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string OnHandQuantity = null;
            string ItemTrackingType = null;
            string IsActive = null;
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForMoveMaterial", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OnHandQuantity, InventoryClassification, MoveType, CanSeeConsignedItem, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }

                //if (FieldsPara.Length > 87 && !string.IsNullOrWhiteSpace(FieldsPara[87]))
                //{
                //    InventoryClassification = FieldsPara[87].TrimEnd(',');
                //}

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (FieldsPara.Length > 21 && !string.IsNullOrWhiteSpace(FieldsPara[21]))
                {
                    OnHandQuantity = FieldsPara[21].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[45]))
                {
                    string[] arrReplenishTypes = FieldsPara[45].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[46]))
                {
                    string[] arrReplenishTypes = FieldsPara[46].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[47]))
                {
                    string[] arrReplenishTypes = FieldsPara[47].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    string[] arrReplenishTypes = FieldsPara[48].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    string[] arrReplenishTypes = FieldsPara[49].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[31]))
                {
                    if (FieldsPara[31].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[31].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[31].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[31].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[32]))
                {
                    if (FieldsPara[32].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[32].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[32].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[32].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                //if (!string.IsNullOrWhiteSpace(FieldsPara[97]))
                //{
                //    IsActive = FieldsPara[97].TrimEnd(',');
                //}
                //if (!string.IsNullOrWhiteSpace(FieldsPara[88]))
                //{
                //    ItemTrackingType = FieldsPara[88].TrimEnd(',');
                //}
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForMoveMaterial", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OnHandQuantity, InventoryClassification, MoveType, CanSeeConsignedItem, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedItemsForMoveMaterial", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, RoomID, CompanyID, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, OnHandQuantity, InventoryClassification, MoveType, CanSeeConsignedItem, UDF6, UDF7, UDF8, UDF9, UDF10);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                lstItems = DataTableHelper.ToList<ItemMasterDTO>(dsCart.Tables[0]);
                if (lstItems != null && lstItems.Count() > 0)
                {
                    TotalCount = lstItems.ElementAt(0).TotalRecords;
                }
            }

            return lstItems;
        }

        public DateTime GetDBUTCTime()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var dt = context.Database.SqlQuery<DateTime>("SELECT GETUTCDATE()").FirstOrDefault();
                return dt;
            }
        }

        public IEnumerable<ItemMasterDTO> GeteVMIItemsPagedRecord(Int64 RoomID, Int64 CompanyID, string SearchValue, int ItemStatus)
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            ItemMasterDTO objItemDTO = new ItemMasterDTO();

            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryevmiItem", RoomID, CompanyID, (string.IsNullOrEmpty(SearchValue) ? (object)DBNull.Value : SearchValue), (ItemStatus > 0 ? ItemStatus : (object)DBNull.Value));

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        BinNumber = row.Field<string>("BinNumber"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        GUID = row.Field<Guid>("GUID"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        BinID = row.Field<long?>("BinID"),
                        BinGUID = row.Field<Guid?>("BinGUID"),
                        IsActive = row.Field<bool>("IsActive"),
                        eVMISensorPort = row.Field<string>("eVMISensorPort"),
                        eVMISensorID = row.Field<double?>("eVMISensorID"),
                        IsRed = (row.Field<bool?>("IsRed").HasValue ? row.Field<bool?>("IsRed") : false),
                        IsYellow = (row.Field<bool?>("IsYellow").HasValue ? row.Field<bool?>("IsYellow") : false),
                        IsGreen = (row.Field<bool?>("IsGreen").HasValue ? row.Field<bool?>("IsGreen") : false)
                    }).ToList();
                }
            }
            return lstItems;
        }

        public IEnumerable<ItemMasterDTO> GetPagedRecordsAll(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, bool IsArchived, bool IsDeleted, string RoomDateFormat, List<long> SupplierIds, TimeZoneInfo CurrentTimeZone, string callFrom = "")
        {
            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
            TotalCount = 0;
            ItemMasterDTO objItemDTO = new ItemMasterDTO();
            string ItemSuppliers = null;
            string Manufacturers = null;
            string CreatedByName = null;
            string UpdatedByName = null;
            string ItemCategory = null;
            string Cost = null;
            string Cost1 = null;
            string ItemLocations = null;
            string InventoryClassification = null;
            string AvgUsageFrom = null;
            string AvgUsageTo = null;
            string turnsFrom = null;
            string turnsTo = null;
            string ItemType = null;
            string CreatedDateFrom = null;
            string CreatedDateTo = null;
            string UpdatedDateFrom = null;
            string UpdatedDateTo = null;
            string UDF1 = null;
            string UDF2 = null;
            string UDF3 = null;
            string UDF4 = null;
            string UDF5 = null;
            string UDF6 = null;
            string UDF7 = null;
            string UDF8 = null;
            string UDF9 = null;
            string UDF10 = null;
            string OnHandQuantity = null;
            string ItemTrackingType = null;
            string IsActive = null;
            DataSet dsCart = new DataSet();
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
            string strSupplierIds = string.Empty;

            if (SupplierIds != null && SupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SupplierIds);
            }

            if (Connectionstring == "")
            {
                return lstItems;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_All", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, null, null, null, null, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                string[] FieldsPara = Fields[1].Split('@');
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

                if (!string.IsNullOrWhiteSpace(FieldsPara[9]))
                {
                    ItemSuppliers = FieldsPara[9].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[10]))
                {
                    Manufacturers = FieldsPara[10].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[11]))
                {
                    ItemCategory = FieldsPara[11].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[22]))
                {
                    ItemType = FieldsPara[22].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                {
                    UpdatedByName = FieldsPara[1].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                {
                    CreatedByName = FieldsPara[0].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                {
                    ItemLocations = FieldsPara[55].TrimEnd(',');
                    //if (callFrom == "ItemMaster")
                    //{
                    //    HttpContext.Current.Session["NSItemLocation"] = ItemLocations;
                    //}
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[87]))
                {
                    InventoryClassification = FieldsPara[87].TrimEnd(',');
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[15]))
                {
                    if (FieldsPara[15].Contains("100_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[15].Contains("10_1000"))
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        Cost = (Fields[1].Split('@')[15].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        Cost1 = (Fields[1].Split('@')[15].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[89]))
                {
                    //if (FieldsPara[89] == "1")
                    //{
                    //    OnHandQuantity = ("Out of Stock");
                    //}
                    //else if (FieldsPara[89] == "2")
                    //{
                    //    OnHandQuantity = ("Below Critical");
                    //}
                    //else if (FieldsPara[89] == "3")
                    //{
                    //    OnHandQuantity = ("Below Min");
                    //}
                    //else if (FieldsPara[89] == "4")
                    //{
                    //    OnHandQuantity = ("Above Max");
                    //}

                    OnHandQuantity = FieldsPara[89].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                {
                    CreatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    CreatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[2].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                {
                    UpdatedDateFrom = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[0], RoomDateFormat, ResourceHelper.CurrentCult), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                    UpdatedDateTo = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(FieldsPara[3].Split(',')[1], RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399), CurrentTimeZone).ToString("dd-MM-yyyy HH:mm:ss");
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[4]))
                {
                    string[] arrReplenishTypes = FieldsPara[4].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF1 = UDF1 + supitem + ",";
                    }
                    UDF1 = UDF1.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF1 = HttpUtility.UrlDecode(UDF1);

                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[5]))
                {
                    string[] arrReplenishTypes = FieldsPara[5].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF2 = UDF2 + supitem + ",";
                    }
                    UDF2 = UDF2.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF2 = HttpUtility.UrlDecode(UDF2);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[6]))
                {
                    string[] arrReplenishTypes = FieldsPara[6].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF3 = UDF3 + supitem + ",";
                    }
                    UDF3 = UDF3.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF3 = HttpUtility.UrlDecode(UDF3);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[7]))
                {
                    string[] arrReplenishTypes = FieldsPara[7].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF4 = UDF4 + supitem + ",";
                    }
                    UDF4 = UDF4.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF4 = HttpUtility.UrlDecode(UDF4);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[8]))
                {
                    string[] arrReplenishTypes = FieldsPara[8].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF5 = UDF5 + supitem + ",";
                    }
                    UDF5 = UDF5.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF5 = HttpUtility.UrlDecode(UDF5);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[92]))
                {
                    string[] arrReplenishTypes = FieldsPara[92].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF6 = UDF6 + supitem + ",";
                    }
                    UDF6 = UDF6.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF6 = HttpUtility.UrlDecode(UDF6);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[93]))
                {
                    string[] arrReplenishTypes = FieldsPara[93].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF7 = UDF7 + supitem + ",";
                    }
                    UDF7 = UDF7.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF7 = HttpUtility.UrlDecode(UDF7);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[94]))
                {
                    string[] arrReplenishTypes = FieldsPara[94].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF8 = UDF8 + supitem + ",";
                    }
                    UDF8 = UDF8.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF8 = HttpUtility.UrlDecode(UDF8);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[95]))
                {
                    string[] arrReplenishTypes = FieldsPara[95].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF9 = UDF9 + supitem + ",";
                    }
                    UDF9 = UDF9.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF9 = HttpUtility.UrlDecode(UDF9);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[96]))
                {
                    string[] arrReplenishTypes = FieldsPara[96].Split(',');
                    foreach (string supitem in arrReplenishTypes)
                    {
                        UDF10 = UDF10 + supitem + ",";
                    }
                    UDF10 = UDF10.TrimEnd('\'').TrimEnd(',').TrimEnd('\'');
                    UDF10 = HttpUtility.UrlDecode(UDF10);
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[48]))
                {
                    if (FieldsPara[48].Contains("100_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[48].Contains("0.4_-1"))
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        AvgUsageFrom = (Fields[1].Split('@')[48].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        AvgUsageTo = (Fields[1].Split('@')[48].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }

                if (!string.IsNullOrWhiteSpace(FieldsPara[49]))
                {
                    if (FieldsPara[49].Contains("20_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else if (FieldsPara[49].Contains("1_-1"))
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                    }
                    else
                    {
                        turnsFrom = (Fields[1].Split('@')[49].Split('_')[0]); //FieldsPara[15].TrimEnd(',');
                        turnsTo = (Fields[1].Split('@')[49].Split('_')[1]); //FieldsPara[15].TrimEnd(',');
                    }
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[97]))
                {
                    IsActive = FieldsPara[97].TrimEnd(',');
                }
                if (!string.IsNullOrWhiteSpace(FieldsPara[88]))
                {
                    ItemTrackingType = FieldsPara[88].TrimEnd(',');
                }
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_All", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, ItemLocations, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }
            else
            {
                dsCart = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedInventoryItems_All", StartRowIndex, MaxRows, SearchTerm, sortColumnName, ItemCategory, Cost, Cost1, ItemType, ItemSuppliers, Manufacturers, CreatedByName, UpdatedByName, CreatedDateFrom, CreatedDateTo, UpdatedDateFrom, UpdatedDateTo, UDF1, UDF2, UDF3, UDF4, UDF5, IsDeleted, IsArchived, strSupplierIds, AvgUsageFrom, AvgUsageTo, turnsFrom, turnsTo, string.Empty, OnHandQuantity, InventoryClassification, ItemTrackingType, IsActive, UDF6, UDF7, UDF8, UDF9, UDF10);
            }

            if (dsCart != null && dsCart.Tables.Count > 0)
            {
                DataTable dtCart = dsCart.Tables[0];
                if (dtCart.Rows.Count > 0)
                {
                    TotalCount = Convert.ToInt32(dtCart.Rows[0]["TotalRecords"]);
                    lstItems = dtCart.AsEnumerable()
                    .Select(row => new ItemMasterDTO
                    {
                        ID = row.Field<long>("ID"),
                        ItemNumber = row.Field<string>("ItemNumber"),
                        ManufacturerID = row.Field<long?>("ManufacturerID"),
                        ManufacturerNumber = row.Field<string>("ManufacturerNumber"),
                        ManufacturerName = row.Field<string>("ManufacturerName"),
                        SupplierID = row.Field<long?>("SupplierID"),
                        SupplierPartNo = row.Field<string>("SupplierPartNo"),
                        SupplierName = row.Field<string>("SupplierName"),
                        UPC = row.Field<string>("UPC"),
                        UNSPSC = row.Field<string>("UNSPSC"),
                        Description = row.Field<string>("Description"),
                        LongDescription = row.Field<string>("LongDescription"),
                        CategoryID = row.Field<long?>("CategoryID"),
                        GLAccountID = row.Field<long?>("GLAccountID"),
                        UOMID = row.Field<long?>("UOMID"),
                        PricePerTerm = row.Field<double?>("PricePerTerm"),
                        CostUOMID = row.Field<long?>("CostUOMID"),
                        DefaultReorderQuantity = row.Field<double?>("DefaultReorderQuantity"),
                        DefaultPullQuantity = row.Field<double?>("DefaultPullQuantity"),
                        Cost = row.Field<double?>("Cost"),
                        Markup = row.Field<double?>("Markup"),
                        SellPrice = row.Field<double?>("SellPrice"),
                        ExtendedCost = row.Field<double?>("ExtendedCost"),
                        AverageCost = row.Field<double?>("AverageCost"),
                        PerItemCost = row.Field<double?>("PerItemCost"),
                        LeadTimeInDays = row.Field<int?>("LeadTimeInDays"),
                        Link1 = row.Field<string>("Link1"),
                        Link2 = row.Field<string>("Link2"),
                        Trend = (row.Field<bool?>("Trend").HasValue ? row.Field<bool>("Trend") : false),// row.Field<bool>("Trend"),
                        IsAutoInventoryClassification = (row.Field<bool?>("IsAutoInventoryClassification").HasValue ? row.Field<bool>("IsAutoInventoryClassification") : false),// row.Field<bool>("Trend"),
                        Taxable = (row.Field<bool?>("Taxable").HasValue ? row.Field<bool>("Taxable") : false), //row.Field<bool>("Taxable"),
                        Consignment = (row.Field<bool?>("Consignment").HasValue ? row.Field<bool>("Consignment") : false),//   row.Field<bool>("Consignment"),
                        StagedQuantity = row.Field<double?>("StagedQuantity"),
                        InTransitquantity = row.Field<double?>("InTransitquantity"),
                        OnOrderQuantity = row.Field<double?>("OnOrderQuantity"),
                        OnReturnQuantity = row.Field<double?>("OnReturnQuantity"),
                        OnTransferQuantity = row.Field<double?>("OnTransferQuantity"),
                        SuggestedOrderQuantity = row.Field<double?>("SuggestedOrderQuantity"),
                        SuggestedTransferQuantity = row.Field<double?>("SuggestedTransferQuantity"),
                        RequisitionedQuantity = row.Field<double?>("RequisitionedQuantity"),
                        PackingQuantity = row.Field<double?>("PackingQuantity"),
                        AverageUsage = row.Field<double?>("AverageUsage"),
                        Turns = row.Field<double?>("Turns"),
                        OnHandQuantity = row.Field<double?>("OnHandQuantity"),
                        CriticalQuantity = row.Field<double>("CriticalQuantity"),
                        MinimumQuantity = row.Field<double>("MinimumQuantity"),
                        MaximumQuantity = row.Field<double>("MaximumQuantity"),
                        WeightPerPiece = row.Field<double?>("WeightPerPiece"),
                        ItemUniqueNumber = row.Field<string>("ItemUniqueNumber"),
                        IsPurchase = (row.Field<bool?>("IsPurchase").HasValue ? row.Field<bool>("IsPurchase") : false), //row.Field<bool>("IsPurchase"),
                        IsTransfer = (row.Field<bool?>("IsTransfer").HasValue ? row.Field<bool>("IsTransfer") : false), //row.Field<bool>("IsTransfer"),
                        DefaultLocation = row.Field<long?>("DefaultLocation"),
                        DefaultLocationName = row.Field<string>("DefaultLocationName"),
                        InventoryClassification = row.Field<int?>("InventoryClassification"),
                        SerialNumberTracking = (row.Field<bool?>("SerialNumberTracking").HasValue ? row.Field<bool>("SerialNumberTracking") : false),
                        LotNumberTracking = (row.Field<bool?>("LotNumberTracking").HasValue ? row.Field<bool>("LotNumberTracking") : false),
                        DateCodeTracking = (row.Field<bool?>("DateCodeTracking").HasValue ? row.Field<bool>("DateCodeTracking") : false),
                        ItemType = row.Field<int>("ItemType"),
                        ImagePath = row.Field<string>("ImagePath"),
                        UDF1 = row.Field<string>("UDF1"),
                        UDF2 = row.Field<string>("UDF2"),
                        UDF3 = row.Field<string>("UDF3"),
                        UDF4 = row.Field<string>("UDF4"),
                        UDF5 = row.Field<string>("UDF5"),
                        UDF6 = row.Field<string>("UDF6"),
                        UDF7 = row.Field<string>("UDF7"),
                        UDF8 = row.Field<string>("UDF8"),
                        UDF9 = row.Field<string>("UDF9"),
                        UDF10 = row.Field<string>("UDF10"),
                        //for item grid display purpose - CART, PUll  
                        ItemUDF1 = row.Field<string>("UDF1"),
                        ItemUDF2 = row.Field<string>("UDF2"),
                        ItemUDF3 = row.Field<string>("UDF3"),
                        ItemUDF4 = row.Field<string>("UDF4"),
                        ItemUDF5 = row.Field<string>("UDF5"),
                        ItemUDF6 = row.Field<string>("UDF6"),
                        ItemUDF7 = row.Field<string>("UDF7"),
                        ItemUDF8 = row.Field<string>("UDF8"),
                        ItemUDF9 = row.Field<string>("UDF9"),
                        ItemUDF10 = row.Field<string>("UDF10"),

                        GUID = row.Field<Guid>("GUID"),
                        Created = row.Field<DateTime?>("Created").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Created")) : row.Field<DateTime?>("Created"),
                        Updated = row.Field<DateTime?>("Updated").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("Updated")) : row.Field<DateTime?>("Updated"),
                        CreatedBy = row.Field<long?>("CreatedBy"),
                        LastUpdatedBy = row.Field<long?>("LastUpdatedBy"),
                        IsDeleted = (row.Field<bool?>("IsDeleted").HasValue ? row.Field<bool?>("IsDeleted") : false),
                        IsArchived = (row.Field<bool?>("IsArchived").HasValue ? row.Field<bool?>("IsArchived") : false),
                        CompanyID = row.Field<long?>("CompanyID"),
                        Room = row.Field<long?>("Room"),
                        CreatedByName = row.Field<string>("CreatedByName"),
                        UpdatedByName = row.Field<string>("UpdatedByName"),
                        RoomName = row.Field<string>("RoomName"),
                        IsLotSerialExpiryCost = row.Field<string>("IsLotSerialExpiryCost"),
                        CategoryName = row.Field<string>("CategoryName"),
                        Unit = row.Field<string>("Unit"),
                        GLAccount = row.Field<string>("GLAccount"),
                        IsItemLevelMinMaxQtyRequired = (row.Field<bool?>("IsItemLevelMinMaxQtyRequired").HasValue ? row.Field<bool?>("IsItemLevelMinMaxQtyRequired") : false),
                        IsBuildBreak = (row.Field<bool?>("IsBuildBreak").HasValue ? row.Field<bool>("IsBuildBreak") : false),
                        // row.Field<bool?>("IsBuildBreak"),
                        BondedInventory = row.Field<string>("BondedInventory"),
                        IsEnforceDefaultReorderQuantity = (row.Field<bool?>("IsEnforceDefaultReorderQuantity").HasValue ? row.Field<bool?>("IsEnforceDefaultReorderQuantity") : false),
                        InventoryClassificationName = row.Field<string>("InventoryClassificationName"),
                        IsBOMItem = (row.Field<bool?>("IsBOMItem").HasValue ? row.Field<bool>("IsBOMItem") : false),
                        //row.Field<bool>("IsBOMItem"),
                        RefBomId = row.Field<long?>("RefBomId"),
                        CostUOMName = row.Field<string>("CostUOMName"),
                        PullQtyScanOverride = (row.Field<bool?>("PullQtyScanOverride").HasValue ? row.Field<bool>("PullQtyScanOverride") : false),
                        // row.Field<bool>("PullQtyScanOverride"),
                        TrendingSetting = row.Field<byte?>("TrendingSetting"),
                        IsPackslipMandatoryAtReceive = row.Field<bool>("IsPackslipMandatoryAtReceive"),
                        ReceivedOn = row.Field<DateTime?>("ReceivedOn").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOn")) : row.Field<DateTime?>("ReceivedOn"),
                        ReceivedOnWeb = row.Field<DateTime?>("ReceivedOnWeb").HasValue ? Convert.ToDateTime(row.Field<DateTime?>("ReceivedOnWeb")) : row.Field<DateTime?>("ReceivedOnWeb"),
                        AddedFrom = row.Field<string>("AddedFrom"),
                        EditedFrom = row.Field<string>("EditedFrom"),
                        BPONumber = row.Field<string>("BPONumber"),
                        TrasnferedDate = row.Field<DateTime?>("trasnfereddate"),
                        CountedDate = row.Field<DateTime?>("counteddate"),
                        OrderedDate = row.Field<DateTime?>("ordereddate"),
                        PulledDate = row.Field<DateTime?>("pulleddate"),
                        PriceSavedDate = row.Field<DateTime?>("PriceSavedDate"),
                        ItemImageExternalURL = row.Field<string>("ItemImageExternalURL"),
                        ItemLink2ExternalURL = row.Field<string>("ItemLink2ExternalURL"),
                        ItemLink2ImageType = row.Field<string>("ItemLink2ImageType"),
                        ItemDocExternalURL = row.Field<string>("ItemDocExternalURL"),
                        ImageType = row.Field<string>("ImageType"),
                        QtyToMeetDemand = row.Field<double?>("QtyToMeetDemand"),
                        DefaultLocationGUID = row.Field<Guid?>("DefaultLocationGUID"),
                        OutTransferQuantity = row.Field<double?>("OutTransferQuantity"),
                        OnOrderInTransitQuantity = row.Field<double?>("OnOrderInTransitQuantity"),
                        IsActive = row.Field<bool>("IsActive"),
                        MonthlyAverageUsage = row.Field<long?>("MonthlyAverageUsage"),
                        IsAllowOrderCostuom = row.Field<bool>("IsAllowOrderCostuom"),
                        SuggestedReturnQuantity = row.Field<double?>("SuggestedReturnQuantity"),
                        IsOrderable = row.Field<bool>("IsOrderable"),
                        OnQuotedQuantity = row.Field<double?>("OnQuotedQuantity"),
                    }).ToList();
                }
            }
            return lstItems;
        }


        public ItemMasterDTO GetDeleteUndeleteRecordByItemGUID(Guid? ItemGUID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetDeleteUndeleteRecordByItemGUID @ItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();

            }
        }

        public void AddUpdateSolumnProduct(string defaultSupplierPartNo, string ItemNumber, Guid ItemGUID, string ItemDescription, string MinQty, string MaxQty, double? DefaultOrderQty, long? CostUOMID, string DefaultLocation, double? OnOrderQuantity, string CostUOMField, long EnterPriceID, long CompanyID, long RoomID, string ELabelKey)
        {
            try
            {
                var quickBookDBName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterPriceID, CompanyID, RoomID);

                if ((!string.IsNullOrEmpty(defaultSupplierPartNo)) && (!string.IsNullOrWhiteSpace(defaultSupplierPartNo)) && solumStore != null && solumStore.ID > 0)
                {
                    CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                    var cartList = objCartItemDAL.GetCartsByItemGUIDPlain(ItemGUID, RoomID, CompanyID);
                    //bool manualCartEntry = (cartList != null && cartList.Where(x => x.IsDeleted == false && x.IsAutoMatedEntry == false).Any());
                    //string ON_ORDER_Flag = manualCartEntry || OnOrderQuantity > 0 ? "1" : "0";



                    #region Order details added to solum table for update flag
                    string BackOrdered = "0", Shipped = "0", ON_ORDER_Flag = "0", DisplayOrderException = "0";
                     OrderDetailsDAL orderDetailsDAL = new OrderDetailsDAL(DataBaseName);
                    ItemMasterDAL itemMasterDAL = new ItemMasterDAL(DataBaseName);
                    var OrderDetailsSolumFlags = itemMasterDAL.GetItemOrderFlags(ItemGUID);
                    if (OrderDetailsSolumFlags != null)
                    {
                        BackOrdered = OrderDetailsSolumFlags.IsBackOrder ? "1" : "0";
                        Shipped = OrderDetailsSolumFlags.IsTransit ? "1" : "0";
                        ON_ORDER_Flag = OrderDetailsSolumFlags.IsOnOrder ? "1" : "0";
                        DisplayOrderException = OrderDetailsSolumFlags.DisplayOrderException ? "1" : "0";
                    }
                    else
                    {
                        BackOrdered =  "0";
                        Shipped = "0";
                        ON_ORDER_Flag = "0";
                        DisplayOrderException = "0";

                    }
                    //orderDetailsDAL.InsertDetailToUpdateSolumFlags(OrderDetails.GUID, OrderDetails.OrderGUID.GetValueOrDefault(), ItemGUID, EnterPriceID, CompanyID, RoomID, OrderDetails.InTransitQuantity.GetValueOrDefault(0), OrderDetails.SupplierPartNo, OrderDetails.IsBackOrdered.GetValueOrDefault(false), currentOrderStatus);
                    #endregion


                    CostUOMMasterDAL costUOMMasterDAL = new CostUOMMasterDAL(DataBaseName);
                    string costUOM = string.Empty;
                    if (string.IsNullOrWhiteSpace(CostUOMField) && string.IsNullOrEmpty(CostUOMField) && costUOM != null && CostUOMID.Value > 0)
                    {
                        costUOM = costUOMMasterDAL.GetCostUOMByID(CostUOMID.Value).CostUOM.ToString();
                    }
                    else
                    {
                        costUOM = CostUOMField;
                    }
                    string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";

                    if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                    {
                        solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                    }
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                    var requestURL = "";
                    requestURL = "v2/common/config/article/info?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + defaultSupplierPartNo.Trim();
                    HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                    string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                    CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                    var article = JsonConvert.DeserializeObject<ArticleInfoDTO>(respstr);
                    string ItemIDValue = defaultSupplierPartNo;
                    if ((!string.IsNullOrWhiteSpace(ELabelKey)) && (ELabelKey != "0") && (ELabelKey != "null"))
                    {
                        ItemIDValue = ELabelKey;
                    }
                    if (article != null && !string.IsNullOrEmpty(article.responseMessage) && !string.IsNullOrWhiteSpace(article.responseMessage) && article.responseMessage.ToLower() == "success")
                    {
                        var articles = article.articleList;

                        if (articles != null && articles.Any() && articles.Count() > 0 && articles[0].data != null)
                        {
                            //if ((objDTO.OnOrderQuantity.GetValueOrDefault(0) > 0 && articles[0].data.ON_ORDER != "1") || (objDTO.OnOrderQuantity.GetValueOrDefault(0) <= 0
                            //    && articles[0].data.ON_ORDER != "0"))
                            //{
                            //    var onOrder = objDTO.OnOrderQuantity.GetValueOrDefault(0) > 0 ? "1" : "0";
                            //    articles[0].data.ON_ORDER = onOrder;

                            //var articleData = new SolumArticle();
                            if (string.IsNullOrEmpty(articles[0].data.BackOrdered))
                            {
                                articles[0].data.BackOrdered = "0";
                            }
                            if (string.IsNullOrEmpty(articles[0].data.Shipped))
                            {
                                articles[0].data.Shipped = "0";
                            }

                            bool IsSameData = articles[0].articleId == defaultSupplierPartNo
                                && articles[0].data.ITEM_ID == ItemIDValue
                                && articles[0].data.ITEM_NAME == ItemNumber
                                && articles[0].data.ITEM_DESCRIPTION == ItemDescription
                                && articles[0].data.Min == MinQty
                                && articles[0].data.Max == MaxQty
                                && articles[0].data.Default_Order_Qty == Convert.ToString(DefaultOrderQty)
                                && articles[0].data.UOM == costUOM
                                && articles[0].data.Loc == DefaultLocation
                                && articles[0].data.BARCODE == ItemNumber
                                && articles[0].data.ON_ORDER == ON_ORDER_Flag
                                && articles[0].data.BackOrdered == BackOrdered
                                && articles[0].data.Shipped == Shipped
                                && articles[0].data.Exception == DisplayOrderException;
                            if (!IsSameData)
                            {
                                List<Article> articleList = new List<Article>();
                                var item = new Article();
                                item.articleId = defaultSupplierPartNo;
                                item.data = new ArticleData()
                                {
                                    ITEM_ID = ItemIDValue,
                                    ITEM_NAME = ItemNumber,
                                    ITEM_DESCRIPTION = ItemDescription,
                                    Min = MinQty,
                                    Max = MaxQty,
                                    Default_Order_Qty = Convert.ToString(DefaultOrderQty),
                                    UOM = costUOM,
                                    Loc = DefaultLocation,
                                    BARCODE = ItemNumber,
                                    DISPLAY_PAGE_1 = null,
                                    DISPLAY_PAGE_2 = null,
                                    DISPLAY_PAGE_3 = null,
                                    DISPLAY_PAGE_4 = null,
                                    DISPLAY_PAGE_5 = null,
                                    DISPLAY_PAGE_6 = null,
                                    DISPLAY_PAGE_7 = null,
                                    NFC_DATA = null,
                                    SKU = null,
                                    ON_ORDER = ON_ORDER_Flag,
                                    Shipped = Shipped,
                                    BackOrdered = BackOrdered,
                                    Exception = DisplayOrderException
                                };
                                articleList.Add(item);
                                //articleData.dataList = articleList;

                                var postURL = "v2/common/articles?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + defaultSupplierPartNo.ToString();
                                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleList.ToArray()), Encoding.UTF8);
                                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                HttpResponseMessage postResponseMessage = client.PostAsync(postURL, httpContent).Result;
                                //string response = postResponseMessage.Content.ReadAsStringAsync().Result;
                                //var authenticationResponse = JsonConvert.DeserializeObject<SolumArticlePostResponse>(response);
                                //}

                                #region Inserting into solumn table for update the flags.
                                if(OrderDetailsSolumFlags != null && OrderDetailsSolumFlags.IsClosedOrder == false && OrderDetailsSolumFlags.OrderStatus != (int)OrderStatus.Closed)
                                {
                                     orderDetailsDAL.InsertDetailToUpdateSolumFlags(OrderDetailsSolumFlags.OrderDetailsGUID, OrderDetailsSolumFlags.OrderGUID, ItemGUID, EnterPriceID, CompanyID, RoomID, OrderDetailsSolumFlags.InTransitQuantity, defaultSupplierPartNo, OrderDetailsSolumFlags.IsBackOrder, OrderDetailsSolumFlags.OrderStatus,OrderDetailsSolumFlags.DisplayOrderException); 
                                }
                               
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        requestURL = "v2/common/articles?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode;
                        //var articleData = new SolumArticle();
                        List<Article> articleList = new List<Article>();
                        var item = new Article();
                        item.articleId = defaultSupplierPartNo;
                        item.data = new ArticleData()
                        {
                            ITEM_ID = ItemIDValue,
                            ITEM_NAME = ItemNumber,
                            ITEM_DESCRIPTION = ItemDescription,
                            Min = MinQty,
                            Max = MaxQty,
                            Default_Order_Qty = Convert.ToString(DefaultOrderQty),
                            UOM = costUOM,
                            Loc = Convert.ToString(DefaultLocation),
                            BARCODE = ItemNumber,
                            DISPLAY_PAGE_1 = null,
                            DISPLAY_PAGE_2 = null,
                            DISPLAY_PAGE_3 = null,
                            DISPLAY_PAGE_4 = null,
                            DISPLAY_PAGE_5 = null,
                            DISPLAY_PAGE_6 = null,
                            DISPLAY_PAGE_7 = null,
                            NFC_DATA = null,
                            SKU = null,
                            ON_ORDER = ON_ORDER_Flag,
                            Shipped = Shipped,
                            BackOrdered = BackOrdered,
                            Exception= DisplayOrderException
                        };

                        articleList.Add(item);
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleList.ToArray()), Encoding.UTF8);
                        //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        HttpResponseMessage postResponseMessage = client.PostAsync(requestURL, httpContent).Result;
                        //articleData.dataList = articleList;
                    }


                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveLogInTextFile(" Error on ItemSave >> solum article info exception: " + ex.Message ?? string.Empty + " : " + System.DateTime.UtcNow);
            }
        }


        #region Get All Solum Labels
        public LabelListDTO GetAllSolumLables(long EnterpriseId, long CompanyId, long RoomId, string SupplierPartNumber)
        {
            try
            {
                var quickBookDBName = "eTurnsQuickBook";
                var quickBookDBNameFromConfig = Convert.ToString(ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                if (!string.IsNullOrWhiteSpace(quickBookDBNameFromConfig) && !string.IsNullOrEmpty(quickBookDBNameFromConfig))
                {
                    quickBookDBName = quickBookDBNameFromConfig;
                }
                SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterpriseId, CompanyId, RoomId);
                if (solumStore != null && solumStore.ID > 0)
                {
                    string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                    {
                        solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                    }
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                    var requestURL = "v2/common/labels/article?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + SupplierPartNumber;
                    HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                    string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                    //CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                    var article = JsonConvert.DeserializeObject<LabelListDTO>(respstr);
                    return article;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion


        #region Verify Solum Labels
        public LabelVerification verifySolumLables(long EnterpriseId, long CompanyId, long RoomId, string labelCode)
        {
            try
            {
                var quickBookDBName = "eTurnsQuickBook";
                var quickBookDBNameFromConfig = Convert.ToString(ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                if (!string.IsNullOrWhiteSpace(quickBookDBNameFromConfig) && !string.IsNullOrEmpty(quickBookDBNameFromConfig))
                {
                    quickBookDBName = quickBookDBNameFromConfig;
                }
                SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterpriseId, CompanyId, RoomId);
                if (solumStore != null && solumStore.ID > 0)
                {
                    string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                    {
                        solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                    }
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                    var requestURL = "v1/labels/type?code=" + labelCode;
                    HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                    string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                    //CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                    var article = JsonConvert.DeserializeObject<LabelVerification>(respstr);
                    return article;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Assign / UnAssign Labels
        public bool AssignUnAssignSolumLabelsToItem(long EnterPriceID, long CompanyID, long RoomID, string SupplierPartNumber, string LablesToAssign, string LabelsToUnLink)
        {
            try
            {
                var quickBookDBName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterPriceID, CompanyID, RoomID);
                if ((!string.IsNullOrEmpty(SupplierPartNumber)) && (!string.IsNullOrWhiteSpace(SupplierPartNumber)) && solumStore != null && solumStore.ID > 0 && ((!string.IsNullOrEmpty(LablesToAssign) || (!string.IsNullOrEmpty(LabelsToUnLink)))))
                {
                    string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                    {
                        solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                    }
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                    var postURL = "v1/labels/link/" + solumStore.StationCode + "?company=" + solumStore.CompanyName;

                    #region Unlink / UnAssign
                    if (!string.IsNullOrEmpty(LabelsToUnLink))
                    {
                        foreach (var item in LabelsToUnLink.Split(','))
                        {
                            if (item != "")
                            {
                                var UnLinkUrl = "v1/labels/unlink?company=" + solumStore.CompanyName + "&labelCode=" + item;
                                HttpResponseMessage postResponseMessage = client.PostAsync(UnLinkUrl, null).Result;
                            }
                        }
                    }
                    #endregion

                    #region Link / Assign
                    if (!string.IsNullOrEmpty(LablesToAssign))
                    {
                        foreach (var item in LablesToAssign.Split(','))
                        {
                            if (item != "")
                            {
                                SolumnLabelAssignDTO solumnLabelAssignDTO = new SolumnLabelAssignDTO();
                                solumnLabelAssignDTO.articleIdList = new string[] { SupplierPartNumber };
                                solumnLabelAssignDTO.labelCode = item;
                                //creating post request
                                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(solumnLabelAssignDTO), Encoding.UTF8);
                                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                HttpResponseMessage postResponseMessage = client.PostAsync(postURL, httpContent).Result;
                            }
                        }
                    }
                    #endregion


                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Insert Assign/UnAssign History
        public decimal InsertAssignUnAssignHistory(Guid ItemGuid, string SupplierPartNumber, string AssignLabels, string UnAssignLabels, long CompanyId, long RoomId, long UserId)
        {
            try
            {
                UnAssignLabels = (UnAssignLabels == null || string.IsNullOrWhiteSpace(UnAssignLabels)) ? string.Empty : UnAssignLabels;
                AssignLabels = (AssignLabels == null || string.IsNullOrWhiteSpace(AssignLabels)) ? string.Empty : AssignLabels;
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid",ItemGuid)
                                    , new SqlParameter("@SupplierPartNumber",SupplierPartNumber)
                                    , new SqlParameter("@AssignLabels",AssignLabels)
                                    , new SqlParameter("@UnAssignLabels",UnAssignLabels)
                                    , new SqlParameter("@CompanyId",CompanyId)
                                    , new SqlParameter("@RoomId",RoomId)
                                    , new SqlParameter("@UserId",UserId)};

                    string qry = "exec [AddLabelsAssignUnAssignHistory] @ItemGuid, @SupplierPartNumber,@AssignLabels , @UnAssignLabels, @CompanyId, @RoomId, @UserId";
                    return context.Database.SqlQuery<decimal>(qry, params1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        public string UpdateFlagOnSolum(long EnterpriseId, long CompanyId, long RoomId, OrderDetailsForSolum orderDetailsForSolum)
        {
            string returnValue = "";
            try
            {
                var defaultSupplierPartNo = orderDetailsForSolum.SupplierPartNo;
                if (!string.IsNullOrEmpty(defaultSupplierPartNo) && !string.IsNullOrWhiteSpace(defaultSupplierPartNo))
                {
                    var quickBookDBName = "eTurnsQuickBook";
                    var quickBookDBNameFromConfig = Convert.ToString(ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                    if (!string.IsNullOrWhiteSpace(quickBookDBNameFromConfig) && !string.IsNullOrEmpty(quickBookDBNameFromConfig))
                    {
                        quickBookDBName = quickBookDBNameFromConfig;
                    }
                    SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                    var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(EnterpriseId, CompanyId, RoomId);
                    if (solumStore != null && solumStore.ID > 0)
                    {
                        string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                        {
                            solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                        }
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                        var requestURL = "v2/common/config/article/info?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + defaultSupplierPartNo.Trim();
                        HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                        string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                        //CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                        var article = JsonConvert.DeserializeObject<ArticleInfoDTO>(respstr);

                        if (article != null && !string.IsNullOrEmpty(article.responseMessage) && !string.IsNullOrWhiteSpace(article.responseMessage) && article.responseMessage.ToLower() == "success")
                        {
                            var articles = article.articleList;
                            if (articles != null && articles.Any() && articles.Count() > 0 && articles[0].data != null)
                            {
                                if (orderDetailsForSolum.InTransitQuantity.GetValueOrDefault(0) > 0)
                                {
                                    //WI-8129
                                    if (articles[0].data.ON_ORDER == "1" || articles[0].data.BackOrdered == "1" || articles[0].data.Exception == "1")
                                    {
                                        articles[0].data.Shipped = "1";
                                        articles[0].data.BackOrdered = "0";
                                        articles[0].data.ON_ORDER = "0";
                                        articles[0].data.Exception = "0";
                                    }
                                    else
                                    {
                                        return returnValue;
                                    }
                                }
                                else if (orderDetailsForSolum.IsBackOrdered && orderDetailsForSolum.InTransitQuantity.GetValueOrDefault(0) <= 0)
                                {
                                    //WI-8129
                                    if (articles[0].data.ON_ORDER == "1")
                                    {
                                        articles[0].data.BackOrdered = "1";
                                        articles[0].data.Shipped = "0";
                                        articles[0].data.ON_ORDER = "0";
                                        articles[0].data.Exception = "0";
                                    }else
                                    {
                                        return returnValue;
                                    }
                                }
                               
                                else if (orderDetailsForSolum.OrderStatus == (int)OrderStatus.Closed && orderDetailsForSolum.OnOrderQuantity.GetValueOrDefault(0) <= 0)
                                {
                                    articles[0].data.Shipped = "0";
                                    articles[0].data.BackOrdered = "0";
                                    articles[0].data.ON_ORDER = "0";
                                    articles[0].data.Exception = "0";
                                }
                                else if (orderDetailsForSolum.IsReopenedOrder.GetValueOrDefault(false))
                                {
                                    articles[0].data.Shipped = "0";
                                    articles[0].data.BackOrdered = "0";
                                    articles[0].data.ON_ORDER = "1";
                                    articles[0].data.Exception = "0";
                                }
                                else if (orderDetailsForSolum.DisplayOrderException)
                                {
                                    //WI-8129
                                    if (articles[0].data.ON_ORDER == "1")
                                    {
                                        articles[0].data.Shipped = "0";
                                        articles[0].data.BackOrdered = "0";
                                        articles[0].data.ON_ORDER = "0";
                                        articles[0].data.Exception = "1";
                                    }
                                    else
                                    {
                                        return returnValue;
                                    }
                                }
                                else if (orderDetailsForSolum.OnOrderQuantity.GetValueOrDefault(0) > 0)
                                {
                                    articles[0].data.Shipped = "0";
                                    articles[0].data.BackOrdered = "0";
                                    articles[0].data.ON_ORDER = "1";
                                    articles[0].data.Exception = "0";
                                }
                                else
                                {
                                    articles[0].data.Shipped = "0";
                                    articles[0].data.BackOrdered = "0";
                                    articles[0].data.ON_ORDER = "0";
                                    articles[0].data.Exception = "0";
                                }
                                //else
                                //{
                                //    articles[0].data.Exception = "0";
                                //}

                                //var articleData = new SolumArticle();
                                List<Article> articleList = new List<Article>();
                                var item = new Article();
                                //item.companyCode = solumStore.CompanyName;
                                //item.stationCode = articles[0].stationCode;
                                item.articleId = defaultSupplierPartNo;
                                //item.articleName = objDTO.ItemNumber;
                                item.data = articles[0].data;
                                articleList.Add(item);
                                //articleData.dataList = articleList;

                                var postURL = "v2/common/articles?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode;
                                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleList.ToArray()), Encoding.UTF8);
                                //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                HttpResponseMessage postResponseMessage = client.PostAsync(postURL, httpContent).Result;
                                //string response = postResponseMessage.Content.ReadAsStringAsync().Result;
                                //var authenticationResponse = JsonConvert.DeserializeObject<SolumArticlePostResponse>(response);
                            }
                        }
                    }

                }
                return returnValue;
            }
            catch (Exception ex)
            {
                return ex.Message;
                //CommonFunctions.SaveLogInTextFile(" Error on ItemSave >> solum article info exception: " + ex.Message ?? string.Empty + " : " + System.DateTime.UtcNow);
            }
        }

        public SolumFlags GetItemOrderFlags(Guid ItemGuID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGuID) };
                    return context.Database.SqlQuery<SolumFlags>("exec GetItemOrderFlags @ItemGUID", params1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        public List<UnAssignedLabelsDeletedItems> GetPendingItems(long? EnterpriseId, long? CompanyId, long? RoomId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@EnterpriseId", EnterpriseId), new SqlParameter("@RoomID", RoomId), new SqlParameter("@CompanyID", CompanyId) };
                return context.Database.SqlQuery<UnAssignedLabelsDeletedItems>("EXEC [dbo].[GetPendingLabelsItems] @EnterpriseId,@CompanyID,@RoomID",params1).ToList();
            }
        }

        public int UpdateStatusForProcessedItems(Guid NewItemGUID, bool IsStarted, bool IsCompleted)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@GUID", NewItemGUID),
                                                new SqlParameter("@IsStarted", IsStarted),
                                                new SqlParameter("@IsCompleted", IsCompleted)
                                              };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<int>("EXEC [dbo].[UpdateStatusLabelItems] @GUID, @IsStarted, @IsCompleted", params1)
                    .FirstOrDefault();
            }
        }

        public void UpdateFailDataForProcessedItems(Guid NewItemGUID, string Error)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@NewItemGUID", NewItemGUID),
                                               new SqlParameter("@Error", Error),};

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC [dbo].[UpdateFailData] @NewItemGUID, @Error", params1);
            }
        }

        public ItemMasterDTO GetItemByGuidNormalNew(Guid ItemGuid, long RoomId, long CompanyId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@ItemGUID", ItemGuid),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@CompanyId", CompanyId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ItemMasterDTO>("exec GetItemByGuidNormalNew @ItemGUID,@RoomId,@CompanyId ", params1).FirstOrDefault();
            }
        }



    }
}