using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace eTurns.DAL
{
    public partial class BinMasterDAL : eTurnsBaseDAL
    {
        public List<BinMasterDTO> GetBinsByBinNumberGenOrStage(Guid? itemGUID, string BinNumber, long? RoomID, long? CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@itemGUID", itemGUID ?? (object)DBNull.Value), new SqlParameter("@BinNumber", BinNumber ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyId", CompanyId ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinsByBinNumberGenOrStage] @itemGUID,@BinNumber,@RoomID,@CompanyId", params1).ToList().ToList();
            }
        }
        public List<BinMasterDTO> GetBinMasterbyGUIds(long? RoomID, long? CompanyID, string GUIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@GUIDs", GUIDs ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterbyGUIds] @RoomID,@CompanyID,@GUIDs", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetBinMasterbyIds(long? RoomID, long? CompanyID, string Ids)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@Ids", Ids ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterbyIds] @RoomID,@CompanyID,@Ids", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetAllBins(long? RoomID, long? CompanyID, bool? IsDeleted, bool? IsArchived, string BinNumber, bool? Staginglocation)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@IsDeleted", IsDeleted ?? (object)DBNull.Value), new SqlParameter("@IsArchived", IsArchived ?? (object)DBNull.Value), new SqlParameter("@BinNumber", BinNumber ?? (object)DBNull.Value), new SqlParameter("@Staginglocation", Staginglocation ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetAllBins] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@BinNumber,@Staginglocation", params1).ToList();
            }
        }
        public BinMasterDTO GetBinMasterByBinID(long BinID, long RoomID, long CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinID", BinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByBinID] @BinID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public List<BinMasterDTO> GetBinMasterByBinGUID(Guid? BinGUID, long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinGUID", BinGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetBinMasterByBinGUID] @BinGUID,@RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetInventoryBinMasterByRoom(long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryBinMasterByRoom] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetStagingBinMasterByRoom(long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetStagingBinMasterByRoom] @RoomID,@CompanyID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetInventoryBinByItem(long? RoomID, long? CompanyID, Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryBinByItem] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetStagingBinByItem(long RoomID, long CompanyID, Guid ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetStagingBinByItem] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetInventoryBinByItemWithQuantity(long? RoomID, long? CompanyID, Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryBinByItemWithQuantity] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }
        public List<BinMasterDTO> GetStagingBinByItemWithQuantity(long? RoomID, long? CompanyID, Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetStagingBinByItemWithQuantity] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }
        public BinMasterDTO GetInventoryBinByIDWithQuantity(long? BinID, Guid? ItemGUID, long? RoomID, long? CompanyID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinID", BinID ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryBinByIDWithQuantity] @BinID,@ItemGUID,@RoomID,@CompanyID", params1).FirstOrDefault();
            }
        }
        public BinMasterDTO GetStagingBinByIDWithQuantity(long? BinID, long? RoomID, long? CompanyID, Guid? ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinID", BinID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetStagingBinByIDWithQuantity] @BinID,@RoomID,@CompanyID,@ItemGUID", params1).FirstOrDefault();
            }
        }
        public BinMasterDTO GetItemBin(Guid ItemGUID, string BinName, long RoomID, long CompanyID, long UserID, bool StagingLocation, string EditedFrom = "", bool IsFromService = false, Guid? materialStagingGUID = null)
        {
            if (!string.IsNullOrWhiteSpace(BinName))
            {
                long binID = 0;
                BinMasterDTO objBinMasterDTO = GetBinByName(BinName, RoomID, CompanyID, StagingLocation);
                if (objBinMasterDTO == null)
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = null;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    objBinMasterDTO = InsertBin(objBinMasterDTO);
                    binID = objBinMasterDTO.ID;

                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = binID;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.ItemGUID = ItemGUID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    objBinMasterDTO = InsertBin(objBinMasterDTO);
                }
                else
                {
                    binID = objBinMasterDTO.ID;
                    objBinMasterDTO = GetItemBinByParentBin(ItemGUID, objBinMasterDTO.ID, RoomID, CompanyID, StagingLocation);
                    if (objBinMasterDTO == null)
                    {
                        objBinMasterDTO = new BinMasterDTO();
                        objBinMasterDTO.ID = 0;
                        objBinMasterDTO.BinNumber = BinName;
                        objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.CreatedBy = UserID;
                        objBinMasterDTO.LastUpdatedBy = UserID;
                        objBinMasterDTO.ParentBinId = binID;
                        objBinMasterDTO.Room = RoomID;
                        objBinMasterDTO.CompanyID = CompanyID;
                        objBinMasterDTO.ItemGUID = ItemGUID;
                        objBinMasterDTO.IsStagingLocation = StagingLocation;
                        objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                        {
                            objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                        }
                        objBinMasterDTO = InsertBin(objBinMasterDTO);
                    }
                }
                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                return objBinMasterDTO;
            }
            else
            {
                return null;
            }
        }

        public BinMasterDTO GetItemBinPlain(Guid ItemGUID, string BinName, long RoomID, long CompanyID, long UserID, bool StagingLocation, string EditedFrom = "", bool IsFromService = false, Guid? materialStagingGUID = null)
        {
            if (!string.IsNullOrWhiteSpace(BinName))
            {
                long binID = 0;
                BinMasterDTO objBinMasterDTO = GetBinByBinNumberPlain(BinName, RoomID, CompanyID, StagingLocation);
                if (objBinMasterDTO == null)
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = null;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                   
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    
                    objBinMasterDTO = InsertBin(objBinMasterDTO);
                    binID = objBinMasterDTO.ID;

                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = binID;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.ItemGUID = ItemGUID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    objBinMasterDTO = InsertBin(objBinMasterDTO);
                }
                else
                {
                    binID = objBinMasterDTO.ID;
                    objBinMasterDTO = GetItemBinByParentBinPlain(ItemGUID, objBinMasterDTO.ID, RoomID, CompanyID, StagingLocation);
                    if (objBinMasterDTO == null)
                    {
                        objBinMasterDTO = new BinMasterDTO();
                        objBinMasterDTO.ID = 0;
                        objBinMasterDTO.BinNumber = BinName;
                        objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.CreatedBy = UserID;
                        objBinMasterDTO.LastUpdatedBy = UserID;
                        objBinMasterDTO.ParentBinId = binID;
                        objBinMasterDTO.Room = RoomID;
                        objBinMasterDTO.CompanyID = CompanyID;
                        objBinMasterDTO.ItemGUID = ItemGUID;
                        objBinMasterDTO.IsStagingLocation = StagingLocation;
                        objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        
                        if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                        {
                            objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                        }
                        objBinMasterDTO = InsertBin(objBinMasterDTO);
                    }
                }
                CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                return objBinMasterDTO;
            }
            else
            {
                return null;
            }
        }

        public BinMasterDTO GetItemDefaultBin(Guid ItemGuid, long RoomID, long CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyId", CompanyId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetItemDefaultBin] @ItemGuid,@RoomID,@CompanyId", params1).FirstOrDefault();
            }
        }
        public BinMasterDTO getInventoryBinByItemAndBinNumber(string BinName, Guid? ItemGUID, long? RoomID, long? CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinName", BinName ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyId", CompanyId ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [getInventoryBinByItemAndBinNumber] @BinName,@ItemGUID,@RoomID,@CompanyId", params1).FirstOrDefault();
            }
        }
        public BinMasterDTO getStagingBinByItemAndBinNumber(string BinName, Guid? ItemGUID, long? RoomID, long? CompanyId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@BinName", BinName ?? (object)DBNull.Value), new SqlParameter("@ItemGUID", ItemGUID ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID ?? (object)DBNull.Value), new SqlParameter("@CompanyId", CompanyId ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [getStagingBinByItemAndBinNumber] @BinName,@ItemGUID,@RoomID,@CompanyId", params1).FirstOrDefault();
            }
        }

        public List<BinMasterDTO> GetInventoryAndStagingBinsByItem(long RoomID, long CompanyID, Guid ItemGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUID", ItemGUID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryAndStagingBinsByItem] @RoomID,@CompanyID,@ItemGUID", params1).ToList();
            }
        }

        public List<BinMasterDTO> GetInventoryAndStagingBinsByItemGUIDs(long RoomID, long CompanyID, string ItemGUIDs)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@ItemGUIDs", ItemGUIDs ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<BinMasterDTO>("exec [GetInventoryAndStagingBinsByItemGUIDs] @RoomID,@CompanyID,@ItemGUIDs", params1).ToList();
            }
        }

        public IEnumerable<BinMasterDTO> GetItemLocation(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid? ItemGuid, Int64? BinId, bool? Isdefault, bool? IsStagingLocation)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchived", IsArchived), new SqlParameter("@ItemGuid", ItemGuid), new SqlParameter("@BinId", BinId) };
                List<BinMasterDTO> obj = context.Database.SqlQuery<BinMasterDTO>("exec [GetItemLocation] @RoomID,@CompanyID,@IsDeleted,@IsArchived,@ItemGuid,@BinId", params1).ToList();
                if (obj.Any())
                {
                    if (Isdefault != null)
                    {
                        if (Isdefault == true)
                        {
                            obj = obj.Where(b => b.IsDefault == true).ToList();
                        }
                        else
                        {
                            obj = obj.Where(b => b.IsDefault == false).ToList();
                        }
                    }
                    if (IsStagingLocation != null)
                    {
                        if (IsStagingLocation == true)
                        {
                            obj = obj.Where(b => b.IsStagingLocation == true).ToList();
                        }
                        else
                        {
                            obj = obj.Where(b => b.IsStagingLocation == false).ToList();
                        }
                    }
                }
                return obj;
            }



        }

        public BinMasterDTO GetOrInsertItemBinFromPullHistoryImport(Guid ItemGUID, string BinName, long RoomID, long CompanyID, long UserID, bool StagingLocation, DateTime Created, string EditedFrom = "", bool IsFromService = false, Guid? materialStagingGUID = null)
        {
            if (!string.IsNullOrWhiteSpace(BinName))
            {
                long binID = 0;
                BinMasterDTO objBinMasterDTO = GetBinByName(BinName, RoomID, CompanyID, StagingLocation);
                if (objBinMasterDTO == null)
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = Created;
                    objBinMasterDTO.LastUpdated = Created;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = null;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = Created;
                    objBinMasterDTO.ReceivedOnWeb = Created;
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    objBinMasterDTO = InsertBinFromPullHistoryImport(objBinMasterDTO);
                    binID = objBinMasterDTO.ID;

                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO.ID = 0;
                    objBinMasterDTO.BinNumber = BinName;
                    objBinMasterDTO.Created = Created;
                    objBinMasterDTO.LastUpdated = Created;
                    objBinMasterDTO.CreatedBy = UserID;
                    objBinMasterDTO.LastUpdatedBy = UserID;
                    objBinMasterDTO.ParentBinId = binID;
                    objBinMasterDTO.Room = RoomID;
                    objBinMasterDTO.CompanyID = CompanyID;
                    objBinMasterDTO.ItemGUID = ItemGUID;
                    objBinMasterDTO.IsStagingLocation = StagingLocation;
                    objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                    objBinMasterDTO.ReceivedOn = Created;
                    objBinMasterDTO.ReceivedOnWeb = Created;
                    if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                    {
                        objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                    }
                    objBinMasterDTO = InsertBinFromPullHistoryImport(objBinMasterDTO);
                }
                else
                {
                    binID = objBinMasterDTO.ID;
                    objBinMasterDTO = GetItemBinByParentBin(ItemGUID, objBinMasterDTO.ID, RoomID, CompanyID, StagingLocation);
                    if (objBinMasterDTO == null)
                    {
                        objBinMasterDTO = new BinMasterDTO();
                        objBinMasterDTO.ID = 0;
                        objBinMasterDTO.BinNumber = BinName;
                        objBinMasterDTO.Created = Created;
                        objBinMasterDTO.LastUpdated = Created;
                        objBinMasterDTO.CreatedBy = UserID;
                        objBinMasterDTO.LastUpdatedBy = UserID;
                        objBinMasterDTO.ParentBinId = binID;
                        objBinMasterDTO.Room = RoomID;
                        objBinMasterDTO.CompanyID = CompanyID;
                        objBinMasterDTO.ItemGUID = ItemGUID;
                        objBinMasterDTO.IsStagingLocation = StagingLocation;
                        objBinMasterDTO.AddedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.EditedFrom = (IsFromService ? (!string.IsNullOrWhiteSpace(EditedFrom) ? EditedFrom : "Web svc") : "Web");
                        objBinMasterDTO.ReceivedOn = Created;
                        objBinMasterDTO.ReceivedOnWeb = Created;
                        if (StagingLocation && materialStagingGUID.HasValue && materialStagingGUID.Value != Guid.Empty)
                        {
                            objBinMasterDTO.MaterialStagingGUID = materialStagingGUID;
                        }
                        objBinMasterDTO = InsertBinFromPullHistoryImport(objBinMasterDTO);
                    }
                }
                return objBinMasterDTO;
            }
            else
            {
                return null;
            }
        }
    }
}
