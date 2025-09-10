using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace eTurns.DAL
{
    public partial class PullTransactionDAL : eTurnsBaseDAL
    {
        #region [Class constructor]

        public PullTransactionDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public PullTransactionDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<ItemLocationLotSerialDTO> GetItemLocationsLotSerials(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, bool PullQuantityLimit)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit)};

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetLotSerialsByItem] @ItemGUID,@BinID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        BinNumber = il.BinNumber,
                                        CriticalQuantity = il.CriticalQuantity,
                                        DateCodeTracking = il.DateCodeTracking,
                                        IsCreditPull = false,
                                        IsDefault = il.IsDefault,
                                        IsItemLevelMinMax = il.IsItemLevelMinMax,
                                        ItemNumber = il.ItemNumber,
                                        ItemType = il.ItemType,
                                        LotNumberTracking = il.LotNumberTracking,
                                        Markup = il.Markup,
                                        MaximumQuantity = il.MaximumQuantity,
                                        MinimumQuantity = 0,
                                        ProjectSpentGUID = Guid.Empty,
                                        SellPrice = 0,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        SuggestedOrderQuantity = il.SuggestedOrderQuantity,
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
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                        CumulativeTotalQuantity = il.CumulativeTotalQuantity,
                                        ItemLocationDetailGUID = il.ItemLocationDetailGUID
                                    }).ToList();
                return lstItemLocations;
            }

        }

        public List<ItemLocationLotSerialDTO> GetStageLocationsByItemGuidAndBinId(Guid ItemGUID, long RoomID, long CompanyID, long BinID)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID)};

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("exec [GetStageLocationsByItemGuidAndBinId] @ItemGUID,@BinID,@CompanyId,@RoomId", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        BinNumber = il.BinNumber,
                                        CriticalQuantity = il.CriticalQuantity,
                                        DateCodeTracking = il.DateCodeTracking,
                                        IsCreditPull = false,
                                        IsDefault = il.IsDefault,
                                        IsItemLevelMinMax = il.IsItemLevelMinMax,
                                        ItemNumber = il.ItemNumber,
                                        ItemType = il.ItemType,
                                        LotNumberTracking = il.LotNumberTracking,
                                        Markup = il.Markup,
                                        MaximumQuantity = il.MaximumQuantity,
                                        MinimumQuantity = 0,
                                        ProjectSpentGUID = Guid.Empty,
                                        SellPrice = 0,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        SuggestedOrderQuantity = il.SuggestedOrderQuantity,
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
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity ?? 0) > 0 ? (il.CustomerOwnedQuantity ?? 0) : (il.ConsignedQuantity ?? 0),
                                        CumulativeTotalQuantity = il.CumulativeTotalQuantity,
                                        ItemLocationDetailGUID = il.ItemLocationDetailGUID
                                    }).ToList();
                return lstItemLocations;
            }

        }

        public List<PullMasterDTO> GetPullWithDetails(List<PullMasterDTO> lstPullInfo, long RoomId, long CompanyId)
        {
            List<UDFDTO> objPullUDFDTO = new List<UDFDTO>();
            List<UDFDTO> objToolCheckoutUDFDTO = new List<UDFDTO>();
            UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);
            objPullUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("PullMaster", RoomId, CompanyId).ToList();
            objToolCheckoutUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("ToolCheckInOutHistory", RoomId, CompanyId).ToList();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);

                if (lstPullInfo != null)
                {
                    lstPullInfo.ForEach(t =>
                    {
                        ItemMaster objItem = context.ItemMasters.FirstOrDefault(p => p.GUID == t.ItemGUID);
                        ProjectMaster objProject = new ProjectMaster();
                        BinMaster objBin = new BinMaster();

                        if (t.ProjectSpendGUID.HasValue && t.ProjectSpendGUID != Guid.Empty)
                        {
                            objProject = context.ProjectMasters.FirstOrDefault(p => p.GUID == t.ProjectSpendGUID);
                            if (objProject == null)
                            {
                                objProject = new ProjectMaster();
                            }
                        }
                        if (t.BinID.HasValue && (t.BinID ?? 0) > 0)
                        {
                            objBin = context.BinMasters.FirstOrDefault(p => p.ID == t.BinID);
                            if (objBin == null)
                            {
                                objBin = new BinMaster();
                            }
                        }

                        if (objItem != null)
                        {
                            t.SerialNumberTracking = objItem.SerialNumberTracking;
                            t.LotNumberTracking = objItem.LotNumberTracking;
                            t.DateCodeTracking = objItem.DateCodeTracking;
                            t.ItemType = objItem.ItemType;
                            t.Consignment = objItem.Consignment;
                            t.ItemNumber = objItem.ItemNumber;
                            t.ProjectName = objProject.ProjectSpendName;
                            t.BinNumber = objBin.BinNumber;
                            t.InventoryConsuptionMethod = objRoom.InventoryConsuptionMethod;
                            if (objPullUDFDTO != null && objPullUDFDTO.Count > 0)
                            {                                
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF1Deleted = true;
                                }
                                else if(objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.UDFControlType != null).Any())
                                    t.isPullUDF1Deleted = false;
                                
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF2Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.UDFControlType != null).Any())
                                    t.isPullUDF2Deleted = false;
                                
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF3Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.UDFControlType != null).Any())
                                    t.isPullUDF3Deleted = false;
                               
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF4Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.UDFControlType != null).Any())
                                    t.isPullUDF4Deleted = false;
                                
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF5Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.UDFControlType != null).Any())
                                    t.isPullUDF5Deleted = false;
                            }
                            if (objToolCheckoutUDFDTO != null && objToolCheckoutUDFDTO.Count > 0)
                            {                                
                                if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == true).Any())
                                {
                                    t.isToolUDF1Deleted = true;
                                }
                                else if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.UDFControlType != null).Any())
                                    t.isToolUDF1Deleted = false;
                                
                                if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF2Deleted = true;
                                }
                                else if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.UDFControlType != null).Any())
                                    t.isPullUDF2Deleted = false;
                                
                                if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF3Deleted = true;
                                }
                                else if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.UDFControlType != null).Any())
                                    t.isPullUDF3Deleted = false;
                               
                                if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                                {
                                    t.isToolUDF4Deleted = true;
                                }
                                else if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.UDFControlType != null).Any())
                                    t.isToolUDF4Deleted = false;
                                
                                if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == true).Any())
                                {
                                    t.isToolUDF5Deleted = true;
                                }
                                else if (objToolCheckoutUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.UDFControlType != null).Any())
                                    t.isToolUDF5Deleted = false;
                            }
                        }
                    });
                }
                if (lstPullInfo == null)
                {
                    lstPullInfo = new List<PullMasterDTO>();
                }

                return lstPullInfo;
            }
        }

        public ItemPullInfo ValidatePullProjectSpend(ItemPullInfo objItemPullInfo)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectMaster objProjectMaster = context.ProjectMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ProjectSpendGUID);

                if (objProjectMaster != null && (objProjectMaster.DollarLimitAmount ?? 0) > 0)
                {
                    if (((objProjectMaster.DollarUsedAmount ?? 0) + Convert.ToDecimal(objItemPullInfo.PullCost)) > (objProjectMaster.DollarLimitAmount ?? 0))
                    {
                        if (!objItemPullInfo.CanOverrideProjectLimits)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "2", ErrorMessage = ResPullMaster.msgProjectSpendLimit });
                        }
                        ProjectSpendItem objProjectSpendItem = context.ProjectSpendItems.FirstOrDefault(t => t.ProjectGUID == objItemPullInfo.ProjectSpendGUID && t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false);

                        if (objProjectSpendItem != null)
                        {
                            if ((objProjectSpendItem.QuantityLimit ?? 0) > 0)
                            {
                                if (((objProjectSpendItem.QuantityUsed ?? 0) + objItemPullInfo.PullQuantity) > (objProjectSpendItem.QuantityLimit ?? 0))
                                {
                                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "3", ErrorMessage = ResPullMaster.msgProjectSpendItemQtyLimit });
                                }
                            }

                            if ((objProjectSpendItem.DollarLimitAmount ?? 0) > 0)
                            {
                                if (((objProjectSpendItem.DollarUsedAmount ?? 0) + Convert.ToDecimal(objItemPullInfo.PullCost)) > (objProjectSpendItem.DollarLimitAmount ?? 0))
                                {
                                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "4", ErrorMessage = ResPullMaster.msgProjectSpendItemamountLimit });
                                }
                            }
                        }
                    }
                }
            }
            return objItemPullInfo;
        }

        public ItemPullInfo ValidatePullProjectSpendForImport(ItemPullInfo objItemPullInfo)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ProjectMaster objProjectMaster = context.ProjectMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ProjectSpendGUID);

                if (objProjectMaster != null && (objProjectMaster.DollarLimitAmount ?? 0) > 0)
                {
                    if (((objProjectMaster.DollarUsedAmount ?? 0) + Convert.ToDecimal(objItemPullInfo.PullCost)) > (objProjectMaster.DollarLimitAmount ?? 0))
                    {
                        if (!objItemPullInfo.CanOverrideProjectLimits)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "4", ErrorMessage = ResPullMaster.msgProjectSpendLimit });
                        }
                        ProjectSpendItem objProjectSpendItem = context.ProjectSpendItems.FirstOrDefault(t => t.ProjectGUID == objItemPullInfo.ProjectSpendGUID && t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false);

                        if (objProjectSpendItem != null)
                        {
                            if ((objProjectSpendItem.QuantityLimit ?? 0) > 0)
                            {
                                if (((objProjectSpendItem.QuantityUsed ?? 0) + objItemPullInfo.PullQuantity) > (objProjectSpendItem.QuantityLimit ?? 0))
                                {
                                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "4", ErrorMessage = ResPullMaster.msgProjectSpendItemQtyLimit });
                                }
                            }

                            if ((objProjectSpendItem.DollarLimitAmount ?? 0) > 0)
                            {
                                if (((objProjectSpendItem.DollarUsedAmount ?? 0) + Convert.ToDecimal(objItemPullInfo.PullCost)) > (objProjectSpendItem.DollarLimitAmount ?? 0))
                                {
                                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "4", ErrorMessage = ResPullMaster.msgProjectSpendItemamountLimit });
                                }
                            }
                        }
                    }
                }
            }
            return objItemPullInfo;
        }

        public ItemPullInfo PullItemQuantity(ItemPullInfo objItemPullInfo, long ModuleId, long SessionUserId,long EnterpriceId, string PullCredit = "Pull",bool AllowEditItemSellPriceonWorkOrderPull = false, bool isFromWorkOrder = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }

                BinMasterDTO objLocDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                {
                    return PullItemStageQuantity(objItemPullInfo, ModuleId, SessionUserId,EnterpriceId, PullCredit,AllowEditItemSellPriceonWorkOrderPull,isFromWorkOrder);
                }

                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
                CostUOMMaster objCostUOM = context.CostUOMMasters.FirstOrDefault(t => t.ID == objItem.CostUOMID);

                //---------------------------------------------------------------------
                //
                double? ItemPullCost = 0;
                double? ItemPullPrice = 0;

                if (((objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                        || (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                     ) && ModuleId != 0)
                {
                    int? PriseSelectionOption = 0;
                    RoomModuleSettingsDTO objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings((long)objItem.CompanyID, (long)objItem.Room, ModuleId);

                    if (objRoomModuleSettingsDTO == null || objRoomModuleSettingsDTO.PriseSelectionOption == null || (objRoomModuleSettingsDTO.PriseSelectionOption != 1 && objRoomModuleSettingsDTO.PriseSelectionOption != 2))
                        PriseSelectionOption = 1;
                    else
                        PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
                    
                    if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                    {
                        ItemPullCost = ( objItemPullInfo.PullCost / (objItemPullInfo.PullQuantity > 0 ? objItemPullInfo.PullQuantity : 1));
                        objItemPullInfo.PullCost = ItemPullCost.GetValueOrDefault(0);
                        if (objItem.Markup.GetValueOrDefault(0) > 0
                            && objItemPullInfo.PullCost > 0)
                        {
                            ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(objItemPullInfo.PullCost)) / (Convert.ToDecimal(objItem.Markup) + Convert.ToDecimal(100)));
                        }
                        else
                        { ItemPullPrice = objItemPullInfo.PullCost; }
                    }
                    else
                    {
                        ItemPullCost = objItem.SellPrice;
                        ItemPullPrice = objItem.Cost;
                    }
                }
                else
                {
                    if (objItem != null && objItem.ItemType == 4)
                    {
                        ItemPullCost = objItem.Cost;
                    }
                    else
                    {
                        ItemPullCost = objItem.SellPrice;
                    }
                }
                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                {
                    if (objItem.Markup.GetValueOrDefault(0) > 0 && objItemPullInfo.PullCost > 0)
                    {
                        ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(objItemPullInfo.PullCost)) / (Convert.ToDecimal(objItem.Markup) + Convert.ToDecimal(100)));
                    }
                    else
                    { ItemPullPrice = objItemPullInfo.PullCost; }
                }
                else
                {
                    ItemPullPrice = objItem.Cost;
                }
                //---------------------------------------------------------------------
                //
                PullMaster objPullMaster = new PullMaster();
                objPullMaster.ActionType = PullCredit;
                objPullMaster.Billing = null;
                objPullMaster.BinID = objItemPullInfo.BinID;
                objPullMaster.CompanyID = objItemPullInfo.CompanyId;
                objPullMaster.ConsignedQuantity = 0;
                objPullMaster.CustomerOwnedQuantity = 0;
                if (objItemPullInfo != null && objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    objPullMaster.GUID = objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty); 
                else
                    objPullMaster.GUID = Guid.NewGuid();
                objPullMaster.IsArchived = false;
                objPullMaster.IsDeleted = false;
                objPullMaster.IsEDIRequired = null;
                objPullMaster.IsEDISent = null;
                objPullMaster.ItemGUID = objItemPullInfo.ItemGUID;
                objPullMaster.LastEDIDate = null;
                objPullMaster.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                objPullMaster.PoolQuantity = objItemPullInfo.PullQuantity;
                objPullMaster.ProjectSpendGUID = (objItemPullInfo.ProjectSpendGUID == Guid.Empty ? null : objItemPullInfo.ProjectSpendGUID);
                objPullMaster.PULLCost = 0;
                objPullMaster.PullPrice = 0;
                objPullMaster.PullCredit = PullCredit;
                objPullMaster.RequisitionDetailGUID = (objItemPullInfo.RequisitionDetailsGUID == Guid.Empty ? null : objItemPullInfo.RequisitionDetailsGUID);
                objPullMaster.Room = objItemPullInfo.RoomId;
                objPullMaster.UDF1 = objItemPullInfo.UDF1;
                objPullMaster.UDF2 = objItemPullInfo.UDF2;
                objPullMaster.UDF3 = objItemPullInfo.UDF3;
                objPullMaster.UDF4 = objItemPullInfo.UDF4;
                objPullMaster.UDF5 = objItemPullInfo.UDF5;
                objPullMaster.Updated = DateTimeUtility.DateTimeNow;
                objPullMaster.WhatWhereAction = PullCredit;
                objPullMaster.WorkOrderDetailGUID = (objItemPullInfo.WorkOrderDetailGUID == Guid.Empty ? null : objItemPullInfo.WorkOrderDetailGUID);
                objPullMaster.Created = DateTimeUtility.DateTimeNow;
                objPullMaster.CreatedBy = objItemPullInfo.CreatedBy;
                objPullMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objPullMaster.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                objPullMaster.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";
                objPullMaster.CountLineItemGuid = objItemPullInfo.CountLineItemGuid;
                objPullMaster.PullOrderNumber = objItemPullInfo.PullOrderNumber;
                objPullMaster.SupplierAccountGuid = objItemPullInfo.SupplierAccountGuid;

                if (objItem != null && objItem.ID > 0)
                {
                    objPullMaster.ItemCost = objItem.Cost.GetValueOrDefault(0);
                    objPullMaster.ItemSellPrice = objItem.SellPrice.GetValueOrDefault(0);
                    objPullMaster.ItemAverageCost = objItem.AverageCost.GetValueOrDefault(0);
                    objPullMaster.ItemMarkup = objItem.Markup.GetValueOrDefault(0);
                }
                if (objCostUOM != null && objCostUOM.ID > 0)
                {
                    objPullMaster.ItemCostUOMValue = objCostUOM.CostUOMValue.GetValueOrDefault(0);
                }

                context.PullMasters.Add(objPullMaster);
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

                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                              || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                                || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                               || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
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
                                objPullDetail.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.Cost + (((objItemLocationDetail.Cost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                                {
                                    itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ItemPullCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                    itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, ItemPullPrice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                }
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));

                                context.PullDetails.Add(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {
                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = t.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;

                                //-----------------------SET CUSTOMER OWNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.CustomerOwnedQuantity != null && objItemLocationDetail.CustomerOwnedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) < t.CustomerOwnedTobePulled)
                                    {
                                        objPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                                        t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objPullDetail.CustomerOwnedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objItemLocationDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity - t.CustomerOwnedTobePulled;
                                        t.CustomerOwnedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.CustomerOwnedQuantity = 0;
                                }

                                //-----------------------SET CONSIGNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.ConsignedQuantity != null && objItemLocationDetail.ConsignedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.ConsignedQuantity ?? 0) < t.ConsignedTobePulled)
                                    {
                                        objPullDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                        objItemLocationDetail.ConsignedQuantity = 0;
                                        t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objPullDetail.ConsignedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                        objItemLocationDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity - t.ConsignedTobePulled;
                                        t.ConsignedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.ConsignedQuantity = 0;
                                }

                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = t.Cost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = null;
                                objPullDetail.PoolQuantity = (objPullDetail.CustomerOwnedQuantity ?? 0) + (objPullDetail.ConsignedQuantity ?? 0);
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
                                objPullDetail.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.Cost + (((objItemLocationDetail.Cost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.Cost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                                {
                                    itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ItemPullCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                    itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, ItemPullPrice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                }
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                context.PullDetails.Add(objPullDetail);

                                if (objItemLocationDetail.CustomerOwnedQuantity < 0)
                                    objItemLocationDetail.CustomerOwnedQuantity = 0;

                                if (objItemLocationDetail.ConsignedQuantity < 0)
                                    objItemLocationDetail.ConsignedQuantity = 0;

                            }
                        }
                    }

                });

                objPullMaster.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                objPullMaster.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;

                if (objItem.Consignment && string.IsNullOrEmpty(objItemPullInfo.PullOrderNumber))
                {
                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, objItem.SupplierID ?? 0, objItem.GUID,EnterpriceId, null, false);
                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                    {
                        objPullMaster.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                    }
                    else
                    {
                        bool isAutoNumberGenerated = true;
                        if (objAutoOrderNumberGenerate != null && objAutoOrderNumberGenerate.IsBlank)
                        {
                            NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
                            SchedulerDTO objSchedulerDTO = objNotificationDAL.GetRoomSchedulesBySupplierID(objItem.SupplierID.GetValueOrDefault(0), objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0);
                            if (objSchedulerDTO != null && objSchedulerDTO.ScheduleMode == 6)
                            {
                                if (objPullMaster.PullOrderNumber == null)
                                    objPullMaster.PullOrderNumber = "";
                                isAutoNumberGenerated = false;
                            }
                        }
                        if (isAutoNumberGenerated)
                        {
                            DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, 0);
                            objPullMaster.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                        }
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
                            objProjectSpendItem.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                            objProjectSpendItem.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";

                            context.ProjectSpendItems.Add(objProjectSpendItem);
                        }
                        else
                        {
                            objProjectSpendItem.DollarUsedAmount = (objProjectSpendItem.DollarLimitAmount == null ? 0 : objProjectSpendItem.DollarLimitAmount) + Convert.ToDecimal((objPullMaster.PULLCost == null ? 0 : objPullMaster.PULLCost));
                            objProjectSpendItem.QuantityUsed = (objProjectSpendItem.QuantityUsed == null ? 0 : objProjectSpendItem.QuantityUsed) + (objItemPullInfo.PullQuantity);
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
                    objItemLocationQTY.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                    objItemLocationQTY.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";

                    context.ItemLocationQTies.Add(objItemLocationQTY);
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

                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "Consume >> Pull Quantity", SessionUserId);
                objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, null, null);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, SessionUserId, null, null);

                context.SaveChanges();

                UpdateCumulativeOnHand(objPullMaster);

                if (objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objPullMaster.Room.GetValueOrDefault(0), objPullMaster.CompanyID.GetValueOrDefault(0));
                }

                if (objPullMaster.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
                    RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRequisitionDetailsByGUIDPlain(objPullMaster.RequisitionDetailGUID.Value);
                    objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objPullMaster.PoolQuantity;
                    objReqDDAL.Edit(objReqDTO, SessionUserId);
                }

                return objItemPullInfo;
            }
        }

        public ItemPullInfo PullItemStageQuantity(ItemPullInfo objItemPullInfo, long ModuleId, long SessionUserId,long EnterpriseId, string PullCredit = "Pull", bool AllowEditItemSellPriceonWorkOrderPull = false, bool isFromWorkOrder = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }

                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
                CostUOMMaster objCostUOM = context.CostUOMMasters.FirstOrDefault(t => t.ID == objItem.CostUOMID);
                //---------------------------------------------------------------------
                //
                double? ItemPullCost = 0;
                double? ItemPullPrice = 0;
                if (((objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                        || (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                     ) && ModuleId != 0)
                {
                    int? PriseSelectionOption = 0;
                    RoomModuleSettingsDTO objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings((long)objItem.CompanyID, (long)objItem.Room, ModuleId);
                    if (objRoomModuleSettingsDTO == null || objRoomModuleSettingsDTO.PriseSelectionOption == null || (objRoomModuleSettingsDTO.PriseSelectionOption != 1 && objRoomModuleSettingsDTO.PriseSelectionOption != 2))
                        PriseSelectionOption = 1;
                    else
                        PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;
                    if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                    {
                        ItemPullCost = (objItemPullInfo.PullCost / (objItemPullInfo.PullQuantity > 0 ? objItemPullInfo.PullQuantity : 1));
                        objItemPullInfo.PullCost = ItemPullCost.GetValueOrDefault(0);
                        if (objItem.Markup.GetValueOrDefault(0) > 0
                            && objItemPullInfo.PullCost > 0)
                        {
                            ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(objItemPullInfo.PullCost)) / (Convert.ToDecimal(objItem.Markup) + Convert.ToDecimal(100)));
                        }
                        else
                        { ItemPullPrice = objItemPullInfo.PullCost; }
                    }
                    else
                    {
                        ItemPullCost = objItem.SellPrice;
                        ItemPullPrice = objItem.Cost;
                    }
                }
                else
                {
                    if (objItem != null && objItem.ItemType == 4)
                    {
                        ItemPullCost = objItem.Cost;
                    }
                    else
                    {
                        ItemPullCost = objItem.SellPrice;
                    }
                }
                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                        && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                {
                    if (objItem.Markup.GetValueOrDefault(0) > 0 && objItemPullInfo.PullCost > 0)
                    {
                        ItemPullPrice = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(objItemPullInfo.PullCost)) / (Convert.ToDecimal(objItem.Markup) + Convert.ToDecimal(100)));
                    }
                    else
                    { ItemPullPrice = objItemPullInfo.PullCost; }
                }
                else
                {
                    ItemPullPrice = objItem.Cost;
                }
                //---------------------------------------------------------------------
                //
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
                objPullMaster.PullPrice = 0;
                objPullMaster.PullCredit = PullCredit;
                objPullMaster.RequisitionDetailGUID = objItemPullInfo.RequisitionDetailsGUID;
                objPullMaster.Room = objItemPullInfo.RoomId;
                objPullMaster.UDF1 = objItemPullInfo.UDF1;
                objPullMaster.UDF2 = objItemPullInfo.UDF2;
                objPullMaster.UDF3 = objItemPullInfo.UDF3;
                objPullMaster.UDF4 = objItemPullInfo.UDF4;
                objPullMaster.UDF5 = objItemPullInfo.UDF5;
                objPullMaster.Updated = DateTimeUtility.DateTimeNow;
                objPullMaster.WhatWhereAction = PullCredit;
                objPullMaster.WorkOrderDetailGUID = (objItemPullInfo.WorkOrderDetailGUID == Guid.Empty ? null : objItemPullInfo.WorkOrderDetailGUID);
                objPullMaster.Created = DateTimeUtility.DateTimeNow;
                objPullMaster.CreatedBy = objItemPullInfo.CreatedBy;
                objPullMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objPullMaster.AddedFrom = "Web";
                objPullMaster.EditedFrom = "Web";
                objPullMaster.CountLineItemGuid = objItemPullInfo.CountLineItemGuid;
                objPullMaster.PullOrderNumber = objItemPullInfo.PullOrderNumber;
                objPullMaster.SupplierAccountGuid = objItemPullInfo.SupplierAccountGuid;

                if (objItem != null && objItem.ID > 0)
                {
                    objPullMaster.ItemCost = objItem.Cost.GetValueOrDefault(0);
                    objPullMaster.ItemSellPrice = objItem.SellPrice.GetValueOrDefault(0);
                    objPullMaster.ItemAverageCost = objItem.AverageCost.GetValueOrDefault(0);
                    objPullMaster.ItemMarkup = objItem.Markup.GetValueOrDefault(0);
                }
                if (objCostUOM != null && objCostUOM.ID > 0)
                {
                    objPullMaster.ItemCostUOMValue = objCostUOM.CostUOMValue.GetValueOrDefault(0);
                }

                context.PullMasters.Add(objPullMaster);
                context.SaveChanges();

                objItemPullInfo.PullGUID = objPullMaster.GUID;
                List<Guid> lstMSPDGuids = new List<Guid>();

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

                    List<MaterialStagingPullDetail> lstItemLocations = null;

                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                              || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                                || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                               || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
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
                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = objItemPullInfo.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;
                                objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = objItemLocationDetail.ItemCost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = objItemLocationDetail.GUID;
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
                                objPullDetail.ItemLocationDetailGUID = null;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.ItemCost + (((objItemLocationDetail.ItemCost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.ItemCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                                {
                                    itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ItemPullCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                    itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, ItemPullPrice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                }
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                context.PullDetails.Add(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;
                                objItemLocationDetail.PoolQuantity = (objItemLocationDetail.PoolQuantity ?? 0) - t.PullQuantity;

                                if (objItemLocationDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    lstMSPDGuids.Add(objItemLocationDetail.GUID);
                                }
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {

                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = t.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;

                                //-----------------------SET CUSTOMER OWNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.CustomerOwnedQuantity != null && objItemLocationDetail.CustomerOwnedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) < t.CustomerOwnedTobePulled)
                                    {
                                        objPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                                        t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objPullDetail.CustomerOwnedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objItemLocationDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity - t.CustomerOwnedTobePulled;
                                        t.CustomerOwnedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.CustomerOwnedQuantity = 0;
                                }

                                //-----------------------SET CONSIGNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.ConsignedQuantity != null && objItemLocationDetail.ConsignedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.ConsignedQuantity ?? 0) < t.ConsignedTobePulled)
                                    {
                                        objPullDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                        objItemLocationDetail.ConsignedQuantity = 0;
                                        t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objPullDetail.ConsignedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                        objItemLocationDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity - t.ConsignedTobePulled;
                                        t.ConsignedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.ConsignedQuantity = 0;
                                }

                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = objItemLocationDetail.ItemCost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = objItemLocationDetail.GUID;
                                objPullDetail.PoolQuantity = (objPullDetail.CustomerOwnedQuantity ?? 0) + (objPullDetail.ConsignedQuantity ?? 0);
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
                                objPullDetail.ItemLocationDetailGUID = null;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.ItemCost + (((objItemLocationDetail.ItemCost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.ItemCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                if ((PullCredit.ToLower() == "pull" || PullCredit.ToLower() == "ms pull")
                                    && isFromWorkOrder && AllowEditItemSellPriceonWorkOrderPull)
                                {
                                    itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ItemPullCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                    itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, ItemPullPrice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                }
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                context.PullDetails.Add(objPullDetail);

                                if (objItemLocationDetail.CustomerOwnedQuantity < 0)
                                    objItemLocationDetail.CustomerOwnedQuantity = 0;

                                if (objItemLocationDetail.ConsignedQuantity < 0)
                                    objItemLocationDetail.ConsignedQuantity = 0;

                                if (objItemLocationDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    lstMSPDGuids.Add(objItemLocationDetail.GUID);
                                }
                            }
                        }
                    }

                });

                objPullMaster.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                objPullMaster.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;

                if (objItem.Consignment && string.IsNullOrEmpty(objItemPullInfo.PullOrderNumber))
                {
                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, objItem.SupplierID ?? 0, objItem.GUID, EnterpriseId, null, false);

                    if (objAutoOrderNumberGenerate != null && !string.IsNullOrWhiteSpace(objAutoOrderNumberGenerate.OrderNumber))
                    {
                        objPullMaster.PullOrderNumber = objAutoOrderNumberGenerate.OrderNumber;
                    }
                    else
                    {
                        bool isAutoNumberGenerated = true;
                        if (objAutoOrderNumberGenerate != null && objAutoOrderNumberGenerate.IsBlank)
                        {
                            NotificationDAL objNotificationDAL = new NotificationDAL(base.DataBaseName);
                            SchedulerDTO objSchedulerDTO = objNotificationDAL.GetRoomSchedulesBySupplierID(objItem.SupplierID.GetValueOrDefault(0), objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0);
                            if (objSchedulerDTO != null && objSchedulerDTO.ScheduleMode == 6)
                            {
                                if (objPullMaster.PullOrderNumber == null)
                                    objPullMaster.PullOrderNumber = "";
                                isAutoNumberGenerated = false;
                            }
                        }
                        if (isAutoNumberGenerated)
                        {
                            DateTime datetimetoConsider = new RegionSettingDAL(base.DataBaseName).GetCurrentDatetimebyTimeZone(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, 0);
                            objPullMaster.PullOrderNumber = datetimetoConsider.ToString("yyyyMMdd");
                        }
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

                            context.ProjectSpendItems.Add(objProjectSpendItem);
                        }
                        else
                        {
                            objProjectSpendItem.DollarUsedAmount = (objProjectSpendItem.DollarLimitAmount == null ? 0 : objProjectSpendItem.DollarLimitAmount) + Convert.ToDecimal((objPullMaster.PULLCost == null ? 0 : objPullMaster.PULLCost));
                            objProjectSpendItem.QuantityUsed = (objProjectSpendItem.QuantityUsed == null ? 0 : objProjectSpendItem.QuantityUsed) + (objItemPullInfo.PullQuantity);
                        }

                        objProjectMaster.DollarUsedAmount = objProjectMaster.DollarLimitAmount + Convert.ToDecimal(objPullMaster.PULLCost);
                    }
                }
                context.SaveChanges();

                if (lstMSPDGuids != null && lstMSPDGuids.Count() > 0)
                {
                    MaterialStagingPullDetailDAL mspdDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    MaterialStagingDetailDAL msdDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    // Write code for Update Staging Detail

                    foreach (var item in lstMSPDGuids)
                    {
                        MaterialStagingPullDetailDTO mspdDTO = mspdDAL.GetRecord(item, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        //Get records with out caching - by hetal
                        //MaterialStagingDetailDTO msdDTO = msdDAL.GetRecordwithoutCaching(mspdDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        MaterialStagingDetailDTO msdDTO = msdDAL.GetMaterialStagingDetailByGUID(mspdDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                        if (msdDTO != null)
                        {
                            List<MaterialStagingPullDetailDTO> lstMSPDDTO = mspdDAL.GetMsPullDetailsByMsDetailsId(msdDTO.GUID);
                            msdDTO.Quantity = lstMSPDDTO.Sum(x => (x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0));
                            msdDTO.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                            msdDTO.Updated = DateTimeUtility.DateTimeNow;
                            msdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            msdDAL.Edit(msdDTO);
                        }
                    }
                }

                objItem.StagedQuantity = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();

                UpdateCumulativeOnHand(objPullMaster);

                if (objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objPullMaster.Room.GetValueOrDefault(0), objPullMaster.CompanyID.GetValueOrDefault(0));
                }

                if (objPullMaster.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
                    RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRequisitionDetailsByGUIDPlain(objPullMaster.RequisitionDetailGUID.Value);
                    objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objPullMaster.PoolQuantity;
                    objReqDDAL.Edit(objReqDTO, SessionUserId);
                }

                return objItemPullInfo;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForPull(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForPull] @ItemGUID,@BinID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();

                return lstItemLocations;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForNegativePull(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForNegativePull] @ItemGUID,@BinID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CumulativeTotalQuantity == null ? 0 : (double)il.CumulativeTotalQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();

                return lstItemLocations;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForRequisition(Guid ItemGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation, Guid MaterialStagingGUID)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation),
                                                   new SqlParameter("@MaterialStagingGUID", MaterialStagingGUID)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForRequisition] @ItemGUID,@BinID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation,@MaterialStagingGUID", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, true, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();
                return lstItemLocations;
            }
        }

        private void UpdateCumulativeOnHand(PullMaster oPullMaster)
        {
            PullMasterViewDTO oPull = new PullMasterViewDTO();
            oPull.ID = oPullMaster.ID;
            oPull.ProjectSpendGUID = oPullMaster.ProjectSpendGUID;
            oPull.UOI = oPullMaster.UOI;
            oPull.CustomerOwnedQuantity = oPullMaster.CustomerOwnedQuantity;
            oPull.ConsignedQuantity = oPullMaster.ConsignedQuantity;
            oPull.PoolQuantity = oPullMaster.PoolQuantity;
            oPull.PullCost = oPullMaster.PULLCost;
            oPull.CreditCustomerOwnedQuantity = oPullMaster.CreditCustomerOwnedQuantity;
            oPull.CreditConsignedQuantity = oPullMaster.CreditConsignedQuantity;
            oPull.BinID = oPullMaster.BinID;
            oPull.UDF1 = oPullMaster.UDF1;
            oPull.UDF2 = oPullMaster.UDF2;
            oPull.UDF3 = oPullMaster.UDF3;
            oPull.UDF4 = oPullMaster.UDF4;
            oPull.UDF5 = oPullMaster.UDF5;
            oPull.GUID = oPullMaster.GUID;
            oPull.ItemGUID = oPullMaster.ItemGUID;
            oPull.Created = oPullMaster.Created;
            oPull.Updated = oPullMaster.Updated;
            oPull.CreatedBy = oPullMaster.CreatedBy;
            oPull.LastUpdatedBy = oPullMaster.LastUpdatedBy;
            oPull.IsDeleted = oPullMaster.IsDeleted;
            oPull.IsArchived = oPullMaster.IsArchived;
            oPull.CompanyID = oPullMaster.CompanyID;
            oPull.Room = oPullMaster.Room;
            oPull.ActionType = oPullMaster.ActionType;
            oPull.PullCredit = oPullMaster.PullCredit;
            oPull.RequisitionDetailGUID = oPullMaster.RequisitionDetailGUID;
            oPull.Billing = oPullMaster.Billing;
            oPull.WorkOrderDetailGUID = oPullMaster.WorkOrderDetailGUID;
            oPull.CountLineItemGuid = oPullMaster.CountLineItemGuid;
            oPull.IsEDISent = oPullMaster.IsEDISent;
            oPull.WhatWhereAction = oPullMaster.WhatWhereAction;
            oPull.ItemOnhandQty = oPullMaster.ItemOnhandQty;
            oPull.IsAddedFromPDA = oPullMaster.IsAddedFromPDA;
            oPull.IsProcessedAfterSync = oPullMaster.IsProcessedAfterSync;
            oPull.ItemStageQty = oPullMaster.ItemStageQty;
            oPull.ItemLocationOnHandQty = oPullMaster.ItemLocationOnhandQty;
            oPull.ItemStageLocationQty = oPullMaster.ItemStageLocationQty;
            oPull.PullOrderNumber = oPullMaster.PullOrderNumber;
            oPull.ReceivedOnWeb = oPullMaster.ReceivedOnWeb;
            oPull.ReceivedOn = oPullMaster.ReceivedOn;
            oPull.AddedFrom = oPullMaster.AddedFrom;
            oPull.EditedFrom = oPullMaster.EditedFrom;

            PullMasterDAL objPullMaster = new PullMasterDAL(base.DataBaseName);
            objPullMaster.UpdateCumulativeOnHand(oPull);
        }

        #region WI-4693-Allow specified rooms to ignore credit rules

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForPullForMoreCredit(Guid ItemGUID, long RoomID, long CompanyID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForPullForMoreCredit] @ItemGUID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();
                return lstItemLocations;
            }
        }

        public List<PullMasterDTO> GetPullWithDetailsForSerialLot(List<PullMasterDTO> lstPullInfo, long RoomId, long CompanyId, long UserID, bool IsStaging)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);
                BinMasterDAL binDAL = new BinMasterDAL(base.DataBaseName);
                List<UDFDTO> objPullUDFDTO = new List<UDFDTO>();                
                UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);
                objPullUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("PullMaster", RoomId, CompanyId).ToList();

                if (lstPullInfo != null)
                {
                    lstPullInfo.ForEach(t =>
                    {
                        ItemMaster objItem = context.ItemMasters.FirstOrDefault(p => p.GUID == t.ItemGUID);
                        ProjectMaster objProject = new ProjectMaster();
                        BinMaster objBin = new BinMaster();
                        long BinID = t.BinID.GetValueOrDefault(0);

                        if (t.ProjectSpendGUID.HasValue && t.ProjectSpendGUID != Guid.Empty)
                        {
                            objProject = context.ProjectMasters.FirstOrDefault(p => p.GUID == t.ProjectSpendGUID);
                            if (objProject == null)
                            {
                                objProject = new ProjectMaster();
                            }
                        }

                        if (!string.IsNullOrEmpty(t.BinNumber))
                        {
                            BinID = binDAL.GetOrInsertBinIDByName(t.ItemGUID.GetValueOrDefault(Guid.Empty), t.BinNumber, UserID, RoomId, CompanyId, IsStaging).GetValueOrDefault(0);
                        }
                        else if (t.BinID.HasValue && (t.BinID ?? 0) > 0)
                        {
                            objBin = context.BinMasters.FirstOrDefault(p => p.ID == t.BinID);
                            if (objBin == null)
                            {
                                objBin = new BinMaster();
                            }
                        }

                        if (objItem != null)
                        {
                            t.SerialNumberTracking = objItem.SerialNumberTracking;
                            t.LotNumberTracking = objItem.LotNumberTracking;
                            t.DateCodeTracking = objItem.DateCodeTracking;
                            t.ItemType = objItem.ItemType;
                            t.Consignment = objItem.Consignment;
                            t.ItemNumber = objItem.ItemNumber;
                            t.ProjectName = objProject.ProjectSpendName;
                            t.BinID = BinID;
                            t.InventoryConsuptionMethod = objRoom.InventoryConsuptionMethod;
                            if (objPullUDFDTO != null && objPullUDFDTO.Count > 0)
                            {
                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF1Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.UDFControlType != null).Any())
                                    t.isPullUDF1Deleted = false;

                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF2Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.UDFControlType != null).Any())
                                    t.isPullUDF2Deleted = false;

                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF3Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.UDFControlType != null).Any())
                                    t.isPullUDF3Deleted = false;

                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF4Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.UDFControlType != null).Any())
                                    t.isPullUDF4Deleted = false;

                                if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                                {
                                    t.isPullUDF5Deleted = true;
                                }
                                else if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.UDFControlType != null).Any())
                                    t.isPullUDF5Deleted = false;
                            }
                        }

                    });
                }

                if (lstPullInfo == null)
                {
                    lstPullInfo = new List<PullMasterDTO>();
                }

                return lstPullInfo;
            }
        }

        public bool ValidateSerialNumberForCredit(Guid ItemGUID, string SerialNumber, long CompanyID, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGUID", ItemGUID),
                                                    new SqlParameter("@SerialNumber", SerialNumber),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@RoomID", RoomID)
                                                };
                int ExistCount = 0;
                ExistCount = context.Database.SqlQuery<int>("EXEC [ValidateSerialNumberForCredit] @ItemGUID,@SerialNumber,@CompanyID,@RoomID", params1).FirstOrDefault();
                return !(ExistCount > 0);
            }
        }

        public bool ValidateSerialNumberForCreditCount(Guid ItemGUID, string SerialNumber, long CompanyID, long RoomID, double ConsignDiff, double CustDiff)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] {  new SqlParameter("@ItemGUID", ItemGUID),
                                                    new SqlParameter("@SerialNumber", SerialNumber),
                                                    new SqlParameter("@CompanyID", CompanyID),
                                                    new SqlParameter("@RoomID", RoomID),
                                                    new SqlParameter("@ConsignDiff", ConsignDiff),
                                                    new SqlParameter("@CustDiff", CustDiff),
                                                };
                int ExistCount = 0;
                ExistCount = context.Database.SqlQuery<int>("EXEC [ValidateSerialNumberForCreditCount] @ItemGUID,@SerialNumber,@CompanyID,@RoomID,@ConsignDiff,@CustDiff", params1).FirstOrDefault();
                if (ExistCount > 0)
                    return false;
                else
                    return true;
            }
        }

        public bool ValidateLotDateCodeForCredit(Guid ItemGUID, string LotNumber, DateTime ExpirationDate, long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                               new SqlParameter("@LotNumber", LotNumber),
                                               new SqlParameter("@ExpirationDate", ExpirationDate),
                                               new SqlParameter("@CompanyID", CompanyID),
                                               new SqlParameter("@RoomID", RoomID)
                                            };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                int ExistCount = 0;
                ExistCount = context.Database.SqlQuery<int>("exec [ValidateLotDateCodeForCredit] @ItemGUID,@LotNumber,@ExpirationDate,@CompanyID,@RoomID", params1).FirstOrDefault();
                return !(ExistCount > 0);
            }
        }

        #endregion

        #region 3055 Edit Pull Quantity

        public PullMasterViewDTO GetPullWithDetailsForEdit(PullMasterViewDTO PullInfo, long RoomId, long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<UDFDTO> objPullUDFDTO = new List<UDFDTO>();
                UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);
                objPullUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("PullMaster", RoomId, CompanyId).ToList();

                Room objRoom = context.Rooms.FirstOrDefault(p => p.ID == RoomId);

                ItemMaster objItem = context.ItemMasters.FirstOrDefault(p => p.GUID == PullInfo.ItemGUID);
                ProjectMaster objProject = new ProjectMaster();
                BinMaster objBin = new BinMaster();

                if (PullInfo.ProjectSpendGUID.HasValue && PullInfo.ProjectSpendGUID != Guid.Empty)
                {
                    objProject = context.ProjectMasters.FirstOrDefault(p => p.GUID == PullInfo.ProjectSpendGUID);
                    if (objProject == null)
                    {
                        objProject = new ProjectMaster();
                    }
                }
                if (PullInfo.BinID.HasValue && (PullInfo.BinID ?? 0) > 0)
                {
                    objBin = context.BinMasters.FirstOrDefault(p => p.ID == PullInfo.BinID);
                    if (objBin == null)
                    {
                        objBin = new BinMaster();
                    }
                }

                if (objItem != null)
                {
                    PullInfo.SerialNumberTracking = objItem.SerialNumberTracking;
                    PullInfo.LotNumberTracking = objItem.LotNumberTracking;
                    PullInfo.DateCodeTracking = objItem.DateCodeTracking;
                    PullInfo.ItemType = objItem.ItemType;
                    PullInfo.Consignment = objItem.Consignment;
                    PullInfo.ItemNumber = objItem.ItemNumber;
                    PullInfo.ProjectName = objProject.ProjectSpendName;
                    PullInfo.BinNumber = objBin.BinNumber;
                    PullInfo.InventoryConsuptionMethod = objRoom.InventoryConsuptionMethod;
                    if (objPullUDFDTO != null && objPullUDFDTO.Count > 0)
                    {
                        PullInfo.isPullUDF1Deleted = false;
                        if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == true).Any())
                        {
                            PullInfo.isPullUDF1Deleted = true;
                        }
                        PullInfo.isPullUDF2Deleted = false;
                        if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == true).Any())
                        {
                            PullInfo.isPullUDF2Deleted = true;
                        }
                        PullInfo.isPullUDF3Deleted = false;
                        if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == true).Any())
                        {
                            PullInfo.isPullUDF3Deleted = true;
                        }
                        PullInfo.isPullUDF4Deleted = false;
                        if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == true).Any())
                        {
                            PullInfo.isPullUDF4Deleted = true;
                        }
                        PullInfo.isPullUDF5Deleted = false;
                        if (objPullUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == true).Any())
                        {
                            PullInfo.isPullUDF5Deleted = true;
                        }
                    }
                }

                if (PullInfo == null)
                {
                    PullInfo = new PullMasterViewDTO();
                }

                return PullInfo;
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForPullEdit(Guid ItemGUID,Guid PullGUID, long RoomID, long CompanyID, long BinID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@PullGUID", PullGUID),
                                                   new SqlParameter("@BinID", BinID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForPullEdit] @ItemGUID,@PullGUID,@BinID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();

                return lstItemLocations;
            }
        }

        public ItemPullInfo PullItemEditQuantity(ItemPullInfo objItemPullInfo,ItemMasterDTO objItem, double ItemCost,double ItemPrice, long ModuleId, long SessionUserId,string strWhatWhereAction, long EnterpriseId,string PullCredit = "Pull")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                PullDetailsDAL objPullDtlDal = new PullDetailsDAL(base.DataBaseName);
                PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                ItemLocationDetailsDAL itemDAL = new ItemLocationDetailsDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }

                #region take Pull Master data from pull guid

                PullMasterDAL objpullMasterDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
                PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

                objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty));                
                objnewPullMasterData = objoldPullMasterData;

                //need to update oter fileds

                #endregion

                #region take Pull details data from pull guid

                List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();
                lstloldPullDetailsDTO = objPullDtlDal.GetPullDetailsByPullGuid(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId).ToList();

                #endregion

                BinMasterDTO objLocDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                {
                    return PullItemStageEditQuantity(objItemPullInfo, ModuleId, SessionUserId, EnterpriseId, PullCredit);
                }

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

                    List<ItemLocationDetail> lstItemLocations = null;
                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                              || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                                || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.ItemLocationDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                               || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.BinID == t.BinID
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
                                objPullDetail.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                objPullDetail.ItemCost = ItemCost;
                                objPullDetail.ItemPrice = ItemPrice;

                                context.PullDetails.Add(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {
                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = t.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;

                                //-----------------------SET CUSTOMER OWNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.CustomerOwnedQuantity != null && objItemLocationDetail.CustomerOwnedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) < t.CustomerOwnedTobePulled)
                                    {
                                        objPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                                        t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objPullDetail.CustomerOwnedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objItemLocationDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity - t.CustomerOwnedTobePulled;
                                        t.CustomerOwnedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.CustomerOwnedQuantity = 0;
                                }

                                //-----------------------SET CONSIGNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.ConsignedQuantity != null && objItemLocationDetail.ConsignedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.ConsignedQuantity ?? 0) < t.ConsignedTobePulled)
                                    {
                                        objPullDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                        objItemLocationDetail.ConsignedQuantity = 0;
                                        t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objPullDetail.ConsignedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                        objItemLocationDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity - t.ConsignedTobePulled;
                                        t.ConsignedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.ConsignedQuantity = 0;
                                }

                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = t.Cost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = null;
                                objPullDetail.PoolQuantity = (objPullDetail.CustomerOwnedQuantity ?? 0) + (objPullDetail.ConsignedQuantity ?? 0);
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
                                objPullDetail.AddedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.EditedFrom = objItemPullInfo.EditedFrom ?? "Web";
                                objPullDetail.ItemLocationDetailGUID = objItemLocationDetail.GUID;

                                objPullDetail.ItemCost = ItemCost;
                                objPullDetail.ItemPrice = ItemPrice;

                                context.PullDetails.Add(objPullDetail);

                                if (objItemLocationDetail.CustomerOwnedQuantity < 0)
                                    objItemLocationDetail.CustomerOwnedQuantity = 0;

                                if (objItemLocationDetail.ConsignedQuantity < 0)
                                    objItemLocationDetail.ConsignedQuantity = 0;

                            }
                        }
                    }

                });

                context.SaveChanges();
                
                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
                lstLocDTO.ID = 0;
                lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                lstLocDTO.GUID = Guid.NewGuid();
                lstLocDTO.ItemGUID = objItem.GUID;
                lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                lstLocDTO.CreatedBy = SessionUserId;
                lstLocDTO.LastUpdatedBy = SessionUserId;
                lstLocDTO.Room = objItemPullInfo.RoomId;
                lstLocDTO.CompanyID = objItemPullInfo.CompanyId;
                lstLocDTO.AddedFrom = "Web";
                lstLocDTO.EditedFrom = "Web";
                lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId,EnterpriseId);

                #region item on hand quantity update

                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                objItem.WhatWhereAction = objnewPullMasterData.PullCredit;

                objItemDAL.Edit(objItem, SessionUserId,EnterpriseId);

                #endregion

                #region "Project Spend Quantity Update"              

                List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(objnewPullMasterData.GUID, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                if (lstPullDtl != null && lstPullDtl.Count > 0)
                {
                    double OldPullCost = objnewPullMasterData.PullCost ?? 0;
                    double OldPullQuantity = objnewPullMasterData.PoolQuantity ?? 0;

                    objnewPullMasterData.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.PoolQuantity = (
                                            lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                +
                                            lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                            );
                    objnewPullMasterData.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                    objnewPullMasterData.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                    objnewPullMasterData.WhatWhereAction = strWhatWhereAction;

                    objnewPullMasterData.ItemOnhandQty = objItem.OnHandQuantity;
                    objnewPullMasterData.ItemStageQty = objItem.StagedQuantity;
                    objnewPullMasterData.ItemLocationOnHandQty = 0;

                    ItemLocationQTYDTO objItemLocationQuantity = itemDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), SessionUserId);
                    if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                    {
                        objnewPullMasterData.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                    }

                    objPullDal.EditForPullQty(objnewPullMasterData);
                    objPullDal.InsertPullEditHistory(objnewPullMasterData.GUID, objnewPullMasterData.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, strWhatWhereAction);
                    // objPullDal.Edit(objnewPullMasterData);

                    double DiffPullCost = (OldPullCost - (objnewPullMasterData.PullCost ?? 0));
                    double DiffPoolQuantity = (OldPullQuantity - (objnewPullMasterData.PoolQuantity ?? 0));

                    if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        objpullMasterDAL.UpdateProjectSpendWithCostEditPull(objItem, objnewPullMasterData, DiffPullCost, DiffPoolQuantity, objnewPullMasterData.ProjectSpendGUID.Value, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                    }
                }

                #endregion

                #region "Update Ext Cost And Avg Cost"
                objItemDAL.GetAndUpdateExtCostAndAvgCost(objItem.GUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                #endregion

                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "Consume >> Pull Quantity", SessionUserId);
                objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, null, null);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, SessionUserId, null, null);

                //if (objItem != null && objItem.GUID != Guid.Empty)
                //{
                //    objItemDAL.EditDate(objItem.GUID, "EditPulledDate");
                //}

                if (objnewPullMasterData != null && objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(objnewPullMasterData.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                }

                objpullMasterDAL.UpdateCumulativeOnHand(objnewPullMasterData);

                return objItemPullInfo;
            }
        }

        public ItemPullInfo PullItemStageEditQuantity(ItemPullInfo objItemPullInfo, long ModuleId, long SessionUserId,long EnterpriseId, string PullCredit = "Pull")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }

                ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);
                CostUOMMaster objCostUOM = context.CostUOMMasters.FirstOrDefault(t => t.ID == objItem.CostUOMID);

                //---------------------------------------------------------------------
                //
                double? ItemPullCost = 0;
                double? ItemPullPrice = 0;
                if (((objItemPullInfo.RequisitionDetailsGUID != null && objItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                        || (objItemPullInfo.WorkOrderDetailGUID != null && objItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                     ) && ModuleId != 0)
                {
                    int? PriseSelectionOption = 0;
                    RoomModuleSettingsDTO objRoomModuleSettingsDTO = objRoomDAL.GetRoomModuleSettings((long)objItem.CompanyID, (long)objItem.Room, ModuleId);
                    if (objRoomModuleSettingsDTO == null || objRoomModuleSettingsDTO.PriseSelectionOption == null || (objRoomModuleSettingsDTO.PriseSelectionOption != 1 && objRoomModuleSettingsDTO.PriseSelectionOption != 2))
                        PriseSelectionOption = 1;
                    else
                        PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                    ItemPullCost = objItem.SellPrice;
                    ItemPullPrice = objItem.Cost;
                }
                else
                {
                    if (objItem != null && objItem.ItemType == 4)
                    {
                        ItemPullCost = objItem.Cost;
                    }
                    else
                    {
                        ItemPullCost = objItem.SellPrice;
                    }
                }

                ItemPullPrice = objItem.Cost;
                //---------------------------------------------------------------------
                //
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
                objPullMaster.PullPrice = 0;
                objPullMaster.PullCredit = PullCredit;
                objPullMaster.RequisitionDetailGUID = objItemPullInfo.RequisitionDetailsGUID;
                objPullMaster.Room = objItemPullInfo.RoomId;
                objPullMaster.UDF1 = objItemPullInfo.UDF1;
                objPullMaster.UDF2 = objItemPullInfo.UDF2;
                objPullMaster.UDF3 = objItemPullInfo.UDF3;
                objPullMaster.UDF4 = objItemPullInfo.UDF4;
                objPullMaster.UDF5 = objItemPullInfo.UDF5;
                objPullMaster.Updated = DateTimeUtility.DateTimeNow;
                objPullMaster.WhatWhereAction = PullCredit;
                objPullMaster.WorkOrderDetailGUID = (objItemPullInfo.WorkOrderDetailGUID == Guid.Empty ? null : objItemPullInfo.WorkOrderDetailGUID);
                objPullMaster.Created = DateTimeUtility.DateTimeNow;
                objPullMaster.CreatedBy = objItemPullInfo.CreatedBy;
                objPullMaster.ReceivedOn = DateTimeUtility.DateTimeNow;
                objPullMaster.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objPullMaster.AddedFrom = "Web";
                objPullMaster.EditedFrom = "Web";
                objPullMaster.CountLineItemGuid = objItemPullInfo.CountLineItemGuid;
                objPullMaster.PullOrderNumber = objItemPullInfo.PullOrderNumber;
                objPullMaster.SupplierAccountGuid = objItemPullInfo.SupplierAccountGuid;

                if (objItem != null && objItem.ID > 0)
                {
                    objPullMaster.ItemCost = objItem.Cost.GetValueOrDefault(0);
                    objPullMaster.ItemSellPrice = objItem.SellPrice.GetValueOrDefault(0);
                    objPullMaster.ItemAverageCost = objItem.AverageCost.GetValueOrDefault(0);
                    objPullMaster.ItemMarkup = objItem.Markup.GetValueOrDefault(0);
                }
                if (objCostUOM != null && objCostUOM.ID > 0)
                {
                    objPullMaster.ItemCostUOMValue = objCostUOM.CostUOMValue.GetValueOrDefault(0);
                }

                context.PullMasters.Add(objPullMaster);
                context.SaveChanges();

                objItemPullInfo.PullGUID = objPullMaster.GUID;
                List<Guid> lstMSPDGuids = new List<Guid>();

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

                    List<MaterialStagingPullDetail> lstItemLocations = null;

                    switch (InventoryConsuptionMethod.ToLower())
                    {
                        case "lifo":
                        case "lifooverride":
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                          (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                              || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                              || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                          || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
                                          && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderByDescending(x => x.ReceivedDate).ToList();
                            break;
                        case "fifo":
                        case "fifooverride":
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                            (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                                || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                                || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                            || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
                                            && (((x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0)) > 0) && (x.IsDeleted ?? false) == false).ToList();
                            lstItemLocations = lstItemLocations.OrderBy(x => x.ReceivedDate).ToList();
                            break;
                        default:
                            lstItemLocations = context.MaterialStagingPullDetails.Where(x => (
                                           (x.LotNumber == t.LotOrSerailNumber && t.LotNumberTracking)
                                               || (x.SerialNumber == t.LotOrSerailNumber && t.SerialNumberTracking)
                                               || (x.ExpirationDate == t.ExpirationDate && t.DateCodeTracking)
                                           || (!t.LotNumberTracking && !t.SerialNumberTracking && !t.DateCodeTracking)) && x.ItemGUID == t.ItemGUID && x.StagingBinId == objItemPullInfo.BinID
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
                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = objItemPullInfo.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;
                                objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = objItemLocationDetail.ItemCost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = objItemLocationDetail.GUID;
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
                                objPullDetail.ItemLocationDetailGUID = null;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.ItemCost + (((objItemLocationDetail.ItemCost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.ItemCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                context.PullDetails.Add(objPullDetail);

                                objItemLocationDetail.CustomerOwnedQuantity = (objItemLocationDetail.CustomerOwnedQuantity ?? 0) - t.CustomerOwnedTobePulled;
                                objItemLocationDetail.ConsignedQuantity = (objItemLocationDetail.ConsignedQuantity ?? 0) - t.ConsignedTobePulled;
                                objItemLocationDetail.PoolQuantity = (objItemLocationDetail.PoolQuantity ?? 0) - t.PullQuantity;

                                if (objItemLocationDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    lstMSPDGuids.Add(objItemLocationDetail.GUID);
                                }
                                break;
                            }
                            else if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) + (objItemLocationDetail.ConsignedQuantity ?? 0) > 0)
                            {

                                PullDetail objPullDetail = new PullDetail();
                                objPullDetail.BinID = t.BinID;
                                objPullDetail.CompanyID = objItemPullInfo.CompanyId;

                                //-----------------------SET CUSTOMER OWNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.CustomerOwnedQuantity != null && objItemLocationDetail.CustomerOwnedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.CustomerOwnedQuantity ?? 0) < t.CustomerOwnedTobePulled)
                                    {
                                        objPullDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity;
                                        objItemLocationDetail.CustomerOwnedQuantity = 0;
                                        t.CustomerOwnedTobePulled = (t.CustomerOwnedTobePulled) - (objPullDetail.CustomerOwnedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.CustomerOwnedQuantity = t.CustomerOwnedTobePulled;
                                        objItemLocationDetail.CustomerOwnedQuantity = objItemLocationDetail.CustomerOwnedQuantity - t.CustomerOwnedTobePulled;
                                        t.CustomerOwnedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.CustomerOwnedQuantity = 0;
                                }

                                //-----------------------SET CONSIGNED QUANTITY-----------------------
                                //
                                if (objItemLocationDetail.ConsignedQuantity != null && objItemLocationDetail.ConsignedQuantity > 0)
                                {
                                    if ((objItemLocationDetail.ConsignedQuantity ?? 0) < t.ConsignedTobePulled)
                                    {
                                        objPullDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity;
                                        objItemLocationDetail.ConsignedQuantity = 0;
                                        t.ConsignedTobePulled = (t.ConsignedTobePulled) - (objPullDetail.ConsignedQuantity ?? 0);
                                    }
                                    else
                                    {
                                        objPullDetail.ConsignedQuantity = t.ConsignedTobePulled;
                                        objItemLocationDetail.ConsignedQuantity = objItemLocationDetail.ConsignedQuantity - t.ConsignedTobePulled;
                                        t.ConsignedTobePulled = 0;
                                    }
                                }
                                else
                                {
                                    objPullDetail.ConsignedQuantity = 0;
                                }

                                objPullDetail.GUID = Guid.NewGuid();
                                objPullDetail.IsArchived = false;
                                objPullDetail.IsDeleted = false;
                                objPullDetail.ItemCost = objItemLocationDetail.ItemCost;
                                objPullDetail.ItemGUID = objItemPullInfo.ItemGUID;
                                objPullDetail.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                                objPullDetail.LotNumber = t.LotNumberTracking ? t.LotOrSerailNumber : string.Empty;
                                objPullDetail.MaterialStagingPullDetailGUID = objItemLocationDetail.GUID;
                                objPullDetail.PoolQuantity = (objPullDetail.CustomerOwnedQuantity ?? 0) + (objPullDetail.ConsignedQuantity ?? 0);
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
                                objPullDetail.ItemLocationDetailGUID = null;

                                #region For Rool Level Average Cost and Customerowned Item

                                double? ilodSellprice = 0;

                                if (objRoomDTO.MethodOfValuingInventory == ((int)InventoryValuationMethod.AverageCost).ToString()
                                    && objPullDetail.ConsignedQuantity == 0)
                                {
                                    ilodSellprice = objItemLocationDetail.ItemCost + (((objItemLocationDetail.ItemCost ?? 0) * (objItem.Markup ?? 0)) / 100);
                                }
                                #endregion

                                double? itemCost = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullCost(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullCost, ilodSellprice, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                double? itemSellprice = new PullMasterDAL(base.DataBaseName).CalculateAndGetPullPrice(objRoomDTO.MethodOfValuingInventory, objPullDetail.ConsignedQuantity > 0, ItemPullPrice, objItemLocationDetail.ItemCost, objItem.CostUOMID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                                objPullDetail.ItemCost = itemCost;
                                objPullDetail.ItemPrice = itemSellprice;
                                objPullMaster.PULLCost = (objPullMaster.PULLCost.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemCost.GetValueOrDefault(0));
                                objPullMaster.PullPrice = (objPullMaster.PullPrice.GetValueOrDefault(0)) + (objPullDetail.PoolQuantity.GetValueOrDefault(0) * itemSellprice.GetValueOrDefault(0));
                                context.PullDetails.Add(objPullDetail);

                                if (objItemLocationDetail.CustomerOwnedQuantity < 0)
                                    objItemLocationDetail.CustomerOwnedQuantity = 0;

                                if (objItemLocationDetail.ConsignedQuantity < 0)
                                    objItemLocationDetail.ConsignedQuantity = 0;

                                if (objItemLocationDetail.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                                {
                                    lstMSPDGuids.Add(objItemLocationDetail.GUID);
                                }
                            }
                        }
                    }

                });

                objPullMaster.CustomerOwnedQuantity = objItemPullInfo.TotalCustomerOwnedTobePulled;
                objPullMaster.ConsignedQuantity = objItemPullInfo.TotalConsignedTobePulled;

                if (objItem.Consignment && string.IsNullOrEmpty(objItemPullInfo.PullOrderNumber))
                {
                    AutoOrderNumberGenerate objAutoOrderNumberGenerate = new AutoSequenceDAL(base.DataBaseName).GetNextPullOrderNumber(objPullMaster.Room ?? 0, objPullMaster.CompanyID ?? 0, objItem.SupplierID ?? 0, objItem.GUID, EnterpriseId, null, false);

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

                            context.ProjectSpendItems.Add(objProjectSpendItem);
                        }
                        else
                        {
                            objProjectSpendItem.DollarUsedAmount = (objProjectSpendItem.DollarLimitAmount == null ? 0 : objProjectSpendItem.DollarLimitAmount) + Convert.ToDecimal((objPullMaster.PULLCost == null ? 0 : objPullMaster.PULLCost));
                            objProjectSpendItem.QuantityUsed = (objProjectSpendItem.QuantityUsed == null ? 0 : objProjectSpendItem.QuantityUsed) + (objItemPullInfo.PullQuantity);
                        }

                        objProjectMaster.DollarUsedAmount = objProjectMaster.DollarLimitAmount + Convert.ToDecimal(objPullMaster.PULLCost);
                    }
                }
                context.SaveChanges();

                if (lstMSPDGuids != null && lstMSPDGuids.Count() > 0)
                {
                    MaterialStagingPullDetailDAL mspdDAL = new MaterialStagingPullDetailDAL(base.DataBaseName);
                    MaterialStagingDetailDAL msdDAL = new MaterialStagingDetailDAL(base.DataBaseName);
                    // Write code for Update Staging Detail

                    foreach (var item in lstMSPDGuids)
                    {
                        MaterialStagingPullDetailDTO mspdDTO = mspdDAL.GetRecord(item, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        //Get records with out caching - by hetal
                        //MaterialStagingDetailDTO msdDTO = msdDAL.GetRecordwithoutCaching(mspdDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                        MaterialStagingDetailDTO msdDTO = msdDAL.GetMaterialStagingDetailByGUID(mspdDTO.MaterialStagingdtlGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                        if (msdDTO != null)
                        {
                            List<MaterialStagingPullDetailDTO> lstMSPDDTO = mspdDAL.GetMsPullDetailsByMsDetailsId(msdDTO.GUID);
                            msdDTO.Quantity = lstMSPDDTO.Sum(x => (x.CustomerOwnedQuantity ?? 0) + (x.ConsignedQuantity ?? 0));
                            msdDTO.LastUpdatedBy = objItemPullInfo.LastUpdatedBy;
                            msdDTO.Updated = DateTimeUtility.DateTimeNow;
                            msdDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            msdDAL.Edit(msdDTO);
                        }
                    }
                }

                objItem.StagedQuantity = context.MaterialStagingPullDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                context.SaveChanges();

                UpdateCumulativeOnHand(objPullMaster);

                if (objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(base.DataBaseName);
                    objWOLDAL.UpdateWOItemAndTotalCost(objPullMaster.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), objPullMaster.Room.GetValueOrDefault(0), objPullMaster.CompanyID.GetValueOrDefault(0));
                }

                if (objPullMaster.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(base.DataBaseName);
                    RequisitionDetailsDTO objReqDTO = objReqDDAL.GetRequisitionDetailsByGUIDPlain(objPullMaster.RequisitionDetailGUID.Value);
                    objReqDTO.QuantityPulled = objReqDTO.QuantityPulled.GetValueOrDefault(0) + objPullMaster.PoolQuantity;
                    objReqDDAL.Edit(objReqDTO, SessionUserId);
                }

                return objItemPullInfo;
            }
        }

        public ItemPullInfo PullItemEditQuantityForRevertExe(ItemPullInfo objItemPullInfo, ItemMasterDTO objItem, double ItemCost, double ItemPrice, long ModuleId, long SessionUserId,string strWhatWhereAction,long EnterpriseId, string PullCredit = "Pull")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                PullDetailsDAL objPullDtlDal = new PullDetailsDAL(base.DataBaseName);
                PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                ItemLocationDetailsDAL itemDAL = new ItemLocationDetailsDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                if (objItemPullInfo == null || objItemPullInfo.lstItemPullDetails == null)
                {
                    return objItemPullInfo;
                }

                #region take Pull Master data from pull guid

                PullMasterDAL objpullMasterDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
                PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

                objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty));
                objoldPullMasterData.WhatWhereAction = strWhatWhereAction;
                objnewPullMasterData = objoldPullMasterData;

                //need to update oter fileds

                #endregion

                #region take Pull details data from pull guid

                List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();
                lstloldPullDetailsDTO = objPullDtlDal.GetPullDetailsByPullGuid(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty), objItemPullInfo.RoomId, objItemPullInfo.CompanyId).ToList();

                #endregion

                BinMasterDTO objLocDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                {
                    return PullItemStageEditQuantity(objItemPullInfo, ModuleId, SessionUserId,EnterpriseId,PullCredit);
                }

                #region for Pull  

                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
                lstLocDTO.ID = 0;
                lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                lstLocDTO.GUID = Guid.NewGuid();
                lstLocDTO.ItemGUID = objItem.GUID;
                lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                lstLocDTO.CreatedBy = SessionUserId;
                lstLocDTO.LastUpdatedBy = SessionUserId;
                lstLocDTO.Room = objItemPullInfo.RoomId;
                lstLocDTO.CompanyID = objItemPullInfo.CompanyId;
                lstLocDTO.AddedFrom = "Web";
                lstLocDTO.EditedFrom = "Web";
                lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId,EnterpriseId);

                #region item on hand quantity update

                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                objItem.WhatWhereAction = objnewPullMasterData.PullCredit;

                objItemDAL.Edit(objItem, SessionUserId, EnterpriseId);

                #endregion

                #region "Project Spend Quantity Update"              

                List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(objnewPullMasterData.GUID, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                if (lstPullDtl != null && lstPullDtl.Count > 0)
                {
                    double OldPullCost = objnewPullMasterData.PullCost ?? 0;
                    double OldPullQuantity = objnewPullMasterData.PoolQuantity ?? 0;

                    objnewPullMasterData.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.PoolQuantity = (
                                            lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                +
                                            lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                            );
                    objnewPullMasterData.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                    objnewPullMasterData.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                    objnewPullMasterData.WhatWhereAction = strWhatWhereAction;
                    objnewPullMasterData.ItemOnhandQty = objItem.OnHandQuantity;
                    objnewPullMasterData.ItemStageQty = objItem.StagedQuantity;
                    objnewPullMasterData.ItemLocationOnHandQty = 0;

                    ItemLocationQTYDTO objItemLocationQuantity = itemDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, objItem.GUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, SessionUserId);
                    if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                    {
                        objnewPullMasterData.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                    }

                    objPullDal.EditForPullQty(objnewPullMasterData);
                    objPullDal.InsertPullEditHistory(objnewPullMasterData.GUID, objnewPullMasterData.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, strWhatWhereAction);
                    //objPullDal.Edit(objnewPullMasterData);

                    double DiffPullCost = (OldPullCost - (objnewPullMasterData.PullCost ?? 0));
                    double DiffPoolQuantity = (OldPullQuantity - (objnewPullMasterData.PoolQuantity ?? 0));

                    if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        objpullMasterDAL.UpdateProjectSpendWithCostEditPull(objItem, objnewPullMasterData, DiffPullCost, DiffPoolQuantity, objnewPullMasterData.ProjectSpendGUID.Value, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                    }
                }

                #endregion

                #region "Update Ext Cost And Avg Cost"
                objItemDAL.GetAndUpdateExtCostAndAvgCost(objItem.GUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                #endregion

                objCartItemDAL.AutoCartUpdateByCode(objItem.GUID, objItemPullInfo.CreatedBy, "web", "Consume >> Pull Quantity", SessionUserId);
                objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, null, null);
                objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.ItemGUID, objItemPullInfo.CreatedBy, SessionUserId, null, null);

                objpullMasterDAL.UpdateCumulativeOnHand(objnewPullMasterData);

                return objItemPullInfo;

                #endregion
            }
        }

        public ItemInfoToCredit PullItemEditCreditQuantityForRevertExe(ItemInfoToCredit objItemPullInfo, ItemMasterDTO objItem, double ItemCost, double ItemPrice, long ModuleId,
            long SessionUserId, string strWhatWhereAction,long EnterpriseId, string PullCredit = "Credit")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                CartItemDAL objCartItemDAL = new CartItemDAL(base.DataBaseName);
                RoomDAL objRoomDAL = new RoomDAL(base.DataBaseName);
                ItemLocationQTYDAL objLocQTY = new ItemLocationQTYDAL(base.DataBaseName);
                ItemMasterDAL objItemDAL = new ItemMasterDAL(base.DataBaseName);
                PullDetailsDAL objPullDtlDal = new PullDetailsDAL(base.DataBaseName);
                PullMasterDAL objPullDal = new PullMasterDAL(base.DataBaseName);
                DashboardDAL objDashboardDAL = new DashboardDAL(base.DataBaseName);
                ItemLocationDetailsDAL itemDAL = new ItemLocationDetailsDAL(base.DataBaseName);

                //---------------------------------------------------------------------
                //
                //if (objItemPullInfo == null || objItemPullInfo.PrevPullsToCredit == null)
                //{
                //    return objItemPullInfo;
                //}

                #region take Pull Master data from pull guid

                PullMasterDAL objpullMasterDAL = new PullMasterDAL(base.DataBaseName);
                PullMasterViewDTO objoldPullMasterData = new PullMasterViewDTO();
                PullMasterViewDTO objnewPullMasterData = new PullMasterViewDTO();

                objoldPullMasterData = objpullMasterDAL.GetPullByGuidPlain(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty));                
                objnewPullMasterData = objoldPullMasterData;

                //need to update oter fileds

                #endregion

                #region take Pull details data from pull guid

                List<PullDetailsDTO> lstloldPullDetailsDTO = new List<PullDetailsDTO>();
                lstloldPullDetailsDTO = objPullDtlDal.GetPullDetailsByPullGuid(objItemPullInfo.PullGUID.GetValueOrDefault(Guid.Empty), objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0)).ToList();

                #endregion

                BinMasterDTO objLocDTO = new BinMasterDAL(base.DataBaseName).GetBinByID(objoldPullMasterData.BinID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0));

                if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                {
                    //return PullItemStageEditQuantity(objItemPullInfo, ModuleId, SessionUserId, PullCredit);
                }

                #region for credit

                List<ItemLocationQTYDTO> lstUpdate = new List<ItemLocationQTYDTO>();
                ItemLocationQTYDTO lstLocDTO = new ItemLocationQTYDTO();
                lstLocDTO.ID = 0;
                lstLocDTO.BinID = objnewPullMasterData.BinID.GetValueOrDefault(0);
                lstLocDTO.Quantity = objnewPullMasterData.PoolQuantity.GetValueOrDefault();
                lstLocDTO.CustomerOwnedQuantity = objnewPullMasterData.CustomerOwnedQuantity;
                lstLocDTO.ConsignedQuantity = objnewPullMasterData.ConsignedQuantity;
                lstLocDTO.LotNumber = objnewPullMasterData.LotNumber;
                lstLocDTO.GUID = Guid.NewGuid();
                lstLocDTO.ItemGUID = objItem.GUID;
                lstLocDTO.Created = DateTimeUtility.DateTimeNow;
                lstLocDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                lstLocDTO.CreatedBy = SessionUserId;
                lstLocDTO.LastUpdatedBy = SessionUserId;
                lstLocDTO.Room = objItem.Room.GetValueOrDefault(0);
                lstLocDTO.CompanyID = objItem.CompanyID.GetValueOrDefault(0);
                lstLocDTO.AddedFrom = "Web";
                lstLocDTO.EditedFrom = "Web";
                lstLocDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                lstLocDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                lstUpdate.Add(lstLocDTO);
                objLocQTY.Save(lstUpdate, SessionUserId,EnterpriseId);

                #region item on hand quantity update

                objItem.OnHandQuantity = context.ItemLocationDetails.Where(t => t.ItemGUID == objItem.GUID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
                objItem.WhatWhereAction = objnewPullMasterData.PullCredit;

                objItemDAL.Edit(objItem, SessionUserId,EnterpriseId);

                #endregion

                #region "Project Spend Quantity Update"              

                List<PullDetailsDTO> lstPullDtl = objPullDtlDal.GetPullDetailsByPullGuidPlain(objnewPullMasterData.GUID, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                if (lstPullDtl != null && lstPullDtl.Count > 0)
                {
                    double OldPullCost = objnewPullMasterData.PullCost ?? 0;
                    double OldPullQuantity = objnewPullMasterData.PoolQuantity ?? 0;

                    objnewPullMasterData.CustomerOwnedQuantity = lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.ConsignedQuantity = lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                    objnewPullMasterData.PoolQuantity = (
                                            lstPullDtl.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0))
                                                +
                                            lstPullDtl.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0))
                                            );
                    objnewPullMasterData.PullCost = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemCost.GetValueOrDefault(0));
                    objnewPullMasterData.PullPrice = lstPullDtl.Sum(x => x.PoolQuantity.GetValueOrDefault(0) * x.ItemPrice.GetValueOrDefault(0));

                    objnewPullMasterData.WhatWhereAction = strWhatWhereAction;

                    objnewPullMasterData.ItemOnhandQty = objItem.OnHandQuantity;
                    objnewPullMasterData.ItemStageQty = objItem.StagedQuantity;
                    objnewPullMasterData.ItemLocationOnHandQty = 0;

                    ItemLocationQTYDTO objItemLocationQuantity = itemDAL.GetItemQtyByLocation(objoldPullMasterData.BinID ?? 0, objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), SessionUserId);
                    if (objItemLocationQuantity != null && objItemLocationQuantity.BinID > 0)
                    {
                        objnewPullMasterData.ItemLocationOnHandQty = objItemLocationQuantity.CustomerOwnedQuantity.GetValueOrDefault(0) + objItemLocationQuantity.ConsignedQuantity.GetValueOrDefault(0);
                    }

                    objPullDal.EditForPullQty(objnewPullMasterData);
                    objPullDal.InsertPullEditHistory(objnewPullMasterData.GUID, objnewPullMasterData.PoolQuantity.GetValueOrDefault(0), OldPullQuantity, strWhatWhereAction);
                    
                    double DiffPullCost = (OldPullCost - (objnewPullMasterData.PullCost ?? 0));
                    double DiffPoolQuantity = (OldPullQuantity - (objnewPullMasterData.PoolQuantity ?? 0));

                    if (objnewPullMasterData.ProjectSpendGUID != null && objnewPullMasterData.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    {
                        objpullMasterDAL.UpdateProjectSpendWithCostEditCredit(objItem, objnewPullMasterData, DiffPullCost, DiffPoolQuantity, objnewPullMasterData.ProjectSpendGUID.Value, objnewPullMasterData.Room.GetValueOrDefault(0), objnewPullMasterData.CompanyID.GetValueOrDefault(0));
                    }
                }

                #endregion

                #region "Update Ext Cost And Avg Cost"
                objItemDAL.GetAndUpdateExtCostAndAvgCost(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0));
                #endregion

                objpullMasterDAL.UpdateCumulativeOnHand(objnewPullMasterData);

                return objItemPullInfo;

                #endregion
            }
        }

        public List<ItemLocationLotSerialDTO> GetItemLocationsWithLotSerialsForCreditEdit(Guid ItemGUID, Guid PullGUID, long RoomID, long CompanyID, double PullQuantity, bool PullQuantityLimit, string SerialOrLotNumber, bool IsStagginLocation)
        {
            List<ItemLocationLotSerialDTO> lstItemLocations = new List<ItemLocationLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ItemGUID", ItemGUID),
                                                   new SqlParameter("@PullGUID", PullGUID),
                                                   new SqlParameter("@PullQuantity", PullQuantity) ,
                                                   new SqlParameter("@CompanyId", CompanyID) ,
                                                   new SqlParameter("@RoomId", RoomID),
                                                   new SqlParameter("@PullQtyLimits", PullQuantityLimit),
                                                   new SqlParameter("@SerialOrLotNumber", SerialOrLotNumber),
                                                   new SqlParameter("@IsStaggingLocation", IsStagginLocation)
                                                };

                lstItemLocations = (from il in context.Database.SqlQuery<ItemLocationLotSerialDTO>("EXEC [GetItemLocationsWithLotSerialsForCreditEdit] @ItemGUID,@PullGUID,@PullQuantity,@CompanyId,@RoomId,@PullQtyLimits,@SerialOrLotNumber,@IsStaggingLocation", params1)
                                    select new ItemLocationLotSerialDTO
                                    {
                                        ItemGUID = il.ItemGUID,
                                        IsCreditPull = false,
                                        BinNumber = il.BinNumber,
                                        DateCodeTracking = il.DateCodeTracking,
                                        SerialNumberTracking = il.SerialNumberTracking,
                                        LotNumberTracking = il.LotNumberTracking,
                                        BinID = il.BinID,
                                        ID = il.BinID.Value,
                                        ConsignedQuantity = il.ConsignedQuantity,
                                        CustomerOwnedQuantity = il.CustomerOwnedQuantity,
                                        SerialNumber = (!string.IsNullOrWhiteSpace(il.SerialNumber)) ? il.SerialNumber.Trim() : string.Empty,
                                        LotNumber = (!string.IsNullOrWhiteSpace(il.LotNumber)) ? il.LotNumber.Trim() : string.Empty,
                                        Received = !il.ReceivedDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ReceivedDate, true, true),
                                        ReceivedDate = il.ReceivedDate,
                                        Expiration = !il.ExpirationDate.HasValue ? "" : FnCommon.ConvertDateByTimeZone(il.ExpirationDate, false, true),
                                        ExpirationDate = il.ExpirationDate,
                                        IsConsignedLotSerial = (il.CustomerOwnedQuantity ?? 0) > 0 ? false : true,
                                        LotSerialQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        LotOrSerailNumber = il.LotNumberTracking ? il.LotNumber : il.SerialNumberTracking ? il.SerialNumber : string.Empty,
                                        PullQuantity = (il.CustomerOwnedQuantity == null ? 0 : (double)il.CustomerOwnedQuantity) + (il.ConsignedQuantity == null ? 0 : (double)il.ConsignedQuantity),
                                        SerialLotExpirationcombin = il.SerialLotExpirationcombin,
                                        strExpirationDate = il.strExpirationDate
                                    }).ToList();
                return lstItemLocations;
            }
        }

        #endregion

        #endregion
    }
}
