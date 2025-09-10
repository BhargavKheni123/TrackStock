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
using System.Web;
using eTurns.DTO.Resources;

namespace eTurns.DAL
{
    public partial class RoomDAL : eTurnsBaseDAL
    {
        /// <summary>
        /// Common Method to Cache table wise data, if not Cached then insert it to Cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoomDTO> GetCachedData(Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<RoomDTO> ObjCache;
            if (IsArchived == false && IsDeleted == false)
            {
                //Get Cached-Media
                ObjCache = CacheHelper<IEnumerable<RoomDTO>>.GetCacheItem("Cached_Room_" + CompanyID.ToString());
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' AS UpdatedDate FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                                    select new RoomDTO
                                                    {
                                                        ID = u.ID,
                                                        RoomName = u.RoomName,
                                                        CompanyName = u.CompanyName,
                                                        ContactName = u.ContactName,
                                                        streetaddress = u.streetaddress,
                                                        City = u.City,
                                                        State = u.State,
                                                        PostalCode = u.PostalCode,
                                                        Country = u.Country,
                                                        Email = u.Email,
                                                        PhoneNo = u.PhoneNo,
                                                        InvoiceBranch = u.InvoiceBranch,
                                                        CustomerNumber = u.CustomerNumber,
                                                        BlanketPO = u.BlanketPO,
                                                        IsConsignment = u.IsConsignment,
                                                        IsTrending = u.IsTrending,
                                                        SourceOfTrending = u.SourceOfTrending,
                                                        TrendingFormula = u.TrendingFormula,
                                                        TrendingFormulaType = u.TrendingFormulaType,
                                                        TrendingFormulaDays = u.TrendingFormulaDays,
                                                        TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                                        TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                                        TrendingFormulaCounts = u.TrendingFormulaCounts,
                                                        SuggestedOrder = u.SuggestedOrder,
                                                        SuggestedTransfer = u.SuggestedTransfer,
                                                        ReplineshmentRoom = u.ReplineshmentRoom,
                                                        ReplenishmentType = u.ReplenishmentType,
                                                        IseVMI = u.IseVMI,
                                                        MaxOrderSize = u.MaxOrderSize,
                                                        HighPO = u.HighPO,
                                                        HighJob = u.HighJob,
                                                        HighTransfer = u.HighTransfer,
                                                        HighCount = u.HighCount,
                                                        GlobMarkupParts = u.GlobMarkupParts,
                                                        GlobMarkupLabor = u.GlobMarkupLabor,
                                                        IsTax1Parts = u.IsTax1Parts,
                                                        IsTax1Labor = u.IsTax1Labor,
                                                        Tax1name = u.Tax1name,
                                                        Tax1Rate = u.Tax1Rate,
                                                        IsTax2Parts = u.IsTax2Parts,
                                                        IsTax2Labor = u.IsTax2Labor,
                                                        tax2name = u.tax2name,
                                                        Tax2Rate = u.Tax2Rate,
                                                        IsTax2onTax1 = u.IsTax2onTax1,
                                                        GXPRConsJob = u.GXPRConsJob,
                                                        CostCenter = u.CostCenter,
                                                        UniqueID = u.UniqueID,
                                                        Created = u.Created,
                                                        Updated = u.Updated,
                                                        CreatedBy = u.CreatedBy,
                                                        LastUpdatedBy = u.LastUpdatedBy,
                                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                        CreatedByName = u.CreatedByName,
                                                        UpdatedByName = u.UpdatedByName,
                                                        MethodOfValuingInventory = u.MethodOfValuingInventory,
                                                        AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                                        AutoCreateTransferTime = u.AutoCreateTransferTime,
                                                        AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                                        IsActive = u.IsActive,
                                                        LicenseBilled = u.LicenseBilled,
                                                        NextCountNo = u.NextCountNo,
                                                        IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                                        IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                                        SlowMovingValue = u.SlowMovingValue,
                                                        FastMovingValue = u.FastMovingValue,
                                                        POAutoSequence = u.POAutoSequence,
                                                        NextOrderNo = u.NextOrderNo,
                                                        NextRequisitionNo = u.NextRequisitionNo,
                                                        NextStagingNo = u.NextStagingNo,
                                                        NextTransferNo = u.NextTransferNo,
                                                        NextWorkOrderNo = u.NextWorkOrderNo,
                                                        RoomGrouping = u.RoomGrouping,
                                                        TransferFrequencyOption = u.TransferFrequencyOption,
                                                        TransferFrequencyDays = u.TransferFrequencyDays,
                                                        TransferFrequencyMonth = u.TransferFrequencyMonth,
                                                        TransferFrequencyNumber = u.TransferFrequencyNumber,
                                                        TransferFrequencyWeek = u.TransferFrequencyWeek,
                                                        TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                                                        TrendingSampleSize = u.TrendingSampleSize,
                                                        TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,
                                                        AverageUsageTransactions = u.AverageUsageTransactions,
                                                        AverageUsageSampleSize = u.AverageUsageSampleSize,
                                                        AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                                        GUID = u.GUID,
                                                        CompanyID = u.CompanyID,
                                                        UDF1 = u.UDF1,
                                                        UDF2 = u.UDF2,
                                                        UDF3 = u.UDF3,
                                                        UDF4 = u.UDF4,
                                                        UDF5 = u.UDF5,
                                                        DefaultSupplierID = u.DefaultSupplierID,
                                                        NextAssetNo = u.NextAssetNo,
                                                        NextBinNo = u.NextBinNo,
                                                        NextKitNo = u.NextKitNo,
                                                        NextItemNo = u.NextItemNo,
                                                        NextProjectSpendNo = u.NextProjectSpendNo,
                                                        NextToolNo = u.NextToolNo,
                                                        DefaultBinID = u.DefaultBinID,
                                                        IsRoomActive = u.IsRoomActive,
                                                        RequestedXDays = u.RequestedXDays ?? 0,
                                                        RequestedYDays = u.RequestedYDays ?? 0,
                                                        InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                                        DefaultBinName = u.DefaultBinName,
                                                        BaseOfInventory = u.BaseOfInventory,
                                                        eVMIWaitCommand = u.eVMIWaitCommand,
                                                        eVMIWaitPort = u.eVMIWaitPort,
                                                        ShelfLifeleadtimeOrdRpt = u.ShelfLifeleadtimeOrdRpt,
                                                        LeadTimeOrdRpt = u.LeadTimeOrdRpt,
                                                        CreatedDate = u.CreatedDate,
                                                        UpdatedDate = u.UpdatedDate,
                                                        IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                                        LastReceivedDate = u.LastReceivedDate,
                                                        LastTrasnferedDate = u.LastTrasnferedDate,
                                                        ExtPhoneNo = u.ExtPhoneNo,
                                                        ReqAutoSequence = u.ReqAutoSequence,
                                                        AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                                        IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                                        IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                                        AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                                        PullRejectionType = u.PullRejectionType,
                                                        BillingRoomType = u.BillingRoomType,
                                                        StagingAutoSequence = u.StagingAutoSequence,
                                                        TransferAutoSequence = u.TransferAutoSequence,
                                                        WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                                        StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                                        TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                                        WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                                        WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                                        MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                                        DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                                        AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                                        PreventMaxOrderQty = u.PreventMaxOrderQty,
                                                        DefaultCountType = u.DefaultCountType,
                                                        TAOAutoSequence = u.TAOAutoSequence,
                                                        TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                                        NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                                        AllowToolOrdering = u.AllowToolOrdering,
                                                        IsWOSignatureRequired = u.IsWOSignatureRequired,
                                                        IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                        ToolCountAutoSequence = u.ToolCountAutoSequence,
                                                        NextToolCountNo = u.NextToolCountNo,

                                                        ForceSupplierFilter = u.ForceSupplierFilter
                                                    }).AsParallel().ToList();
                        ObjCache = CacheHelper<IEnumerable<RoomDTO>>.AddCacheItem("Cached_Room_" + CompanyID.ToString(), obj);
                    }
                }
            }
            else
            {
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
                    ObjCache = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName,'' AS CreatedDate,'' As UpdatedDate FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new RoomDTO
                                {
                                    ID = u.ID,
                                    RoomName = u.RoomName,
                                    CompanyName = u.CompanyName,
                                    ContactName = u.ContactName,
                                    streetaddress = u.streetaddress,
                                    City = u.City,
                                    State = u.State,
                                    PostalCode = u.PostalCode,
                                    Country = u.Country,
                                    Email = u.Email,
                                    PhoneNo = u.PhoneNo,
                                    InvoiceBranch = u.InvoiceBranch,
                                    CustomerNumber = u.CustomerNumber,
                                    BlanketPO = u.BlanketPO,
                                    IsConsignment = u.IsConsignment,
                                    IsTrending = u.IsTrending,
                                    SourceOfTrending = u.SourceOfTrending,
                                    TrendingFormula = u.TrendingFormula,
                                    TrendingFormulaType = u.TrendingFormulaType,
                                    TrendingFormulaDays = u.TrendingFormulaDays,
                                    TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                    TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                    TrendingFormulaCounts = u.TrendingFormulaCounts,
                                    //   ManualMin = u.ManualMin,
                                    SuggestedOrder = u.SuggestedOrder,
                                    SuggestedTransfer = u.SuggestedTransfer,
                                    ReplineshmentRoom = u.ReplineshmentRoom,
                                    //IsItemReplenishment = u.IsItemReplenishment,
                                    //IsBinReplenishment = u.IsBinReplenishment,
                                    ReplenishmentType = u.ReplenishmentType,
                                    IseVMI = u.IseVMI,
                                    MaxOrderSize = u.MaxOrderSize,
                                    HighPO = u.HighPO,
                                    HighJob = u.HighJob,
                                    HighTransfer = u.HighTransfer,
                                    HighCount = u.HighCount,
                                    GlobMarkupParts = u.GlobMarkupParts,
                                    GlobMarkupLabor = u.GlobMarkupLabor,
                                    IsTax1Parts = u.IsTax1Parts,
                                    IsTax1Labor = u.IsTax1Labor,
                                    Tax1name = u.Tax1name,
                                    Tax1Rate = u.Tax1Rate,
                                    IsTax2Parts = u.IsTax2Parts,
                                    IsTax2Labor = u.IsTax2Labor,
                                    tax2name = u.tax2name,
                                    Tax2Rate = u.Tax2Rate,
                                    IsTax2onTax1 = u.IsTax2onTax1,
                                    GXPRConsJob = u.GXPRConsJob,
                                    CostCenter = u.CostCenter,
                                    UniqueID = u.UniqueID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    MethodOfValuingInventory = u.MethodOfValuingInventory,
                                    AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                    AutoCreateTransferTime = u.AutoCreateTransferTime,
                                    AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                    IsActive = u.IsActive,
                                    LicenseBilled = u.LicenseBilled,
                                    NextCountNo = u.NextCountNo,
                                    POAutoSequence = u.POAutoSequence,
                                    IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                    IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                    SlowMovingValue = u.SlowMovingValue,
                                    FastMovingValue = u.FastMovingValue,
                                    NextOrderNo = u.NextOrderNo,
                                    NextRequisitionNo = u.NextRequisitionNo,
                                    NextStagingNo = u.NextStagingNo,
                                    NextTransferNo = u.NextTransferNo,
                                    NextWorkOrderNo = u.NextWorkOrderNo,
                                    RoomGrouping = u.RoomGrouping,

                                    TransferFrequencyOption = u.TransferFrequencyOption,
                                    TransferFrequencyDays = u.TransferFrequencyDays,
                                    TransferFrequencyMonth = u.TransferFrequencyMonth,
                                    TransferFrequencyNumber = u.TransferFrequencyNumber,
                                    TransferFrequencyWeek = u.TransferFrequencyWeek,
                                    TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                    TrendingSampleSize = u.TrendingSampleSize,
                                    TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                    AverageUsageTransactions = u.AverageUsageTransactions,
                                    AverageUsageSampleSize = u.AverageUsageSampleSize,
                                    AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    DefaultSupplierID = u.DefaultSupplierID,
                                    NextAssetNo = u.NextAssetNo,
                                    NextBinNo = u.NextBinNo,
                                    NextKitNo = u.NextKitNo,
                                    NextItemNo = u.NextItemNo,
                                    NextProjectSpendNo = u.NextProjectSpendNo,
                                    NextToolNo = u.NextToolNo,
                                    DefaultBinID = u.DefaultBinID,
                                    IsRoomActive = u.IsRoomActive,
                                    RequestedXDays = u.RequestedXDays ?? 0,
                                    RequestedYDays = u.RequestedYDays ?? 0,
                                    InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                    BaseOfInventory = u.BaseOfInventory,
                                    eVMIWaitCommand = u.eVMIWaitCommand,
                                    eVMIWaitPort = u.eVMIWaitPort,
                                    ShelfLifeleadtimeOrdRpt = u.ShelfLifeleadtimeOrdRpt,
                                    LeadTimeOrdRpt = u.LeadTimeOrdRpt,
                                    CreatedDate = u.CreatedDate,
                                    UpdatedDate = u.UpdatedDate,
                                    IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                    LastReceivedDate = u.LastReceivedDate,
                                    LastTrasnferedDate = u.LastTrasnferedDate,
                                    ReqAutoSequence = u.ReqAutoSequence,
                                    AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                    IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                    IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                    AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                    PullRejectionType = u.PullRejectionType,
                                    BillingRoomType = u.BillingRoomType,

                                    StagingAutoSequence = u.StagingAutoSequence,
                                    TransferAutoSequence = u.TransferAutoSequence,
                                    WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                    StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                    TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                    WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                    WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                    MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                    DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                    AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                    PreventMaxOrderQty = u.PreventMaxOrderQty,
                                    DefaultCountType = u.DefaultCountType,
                                    TAOAutoSequence = u.TAOAutoSequence,
                                    TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                    NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                    AllowToolOrdering = u.AllowToolOrdering,
                                    IsWOSignatureRequired = u.IsWOSignatureRequired,
                                    IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                    IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                    ToolCountAutoSequence = u.ToolCountAutoSequence,
                                    NextToolCountNo = u.NextToolCountNo,

                                    ForceSupplierFilter = u.ForceSupplierFilter
                                }).AsParallel().ToList();
                }

            }

            return ObjCache;//.Where(t => t.Room == RoomID);
        }


        /// <summary>
        /// Get Paged Records from the Room Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<RoomDTO> GetAllRecords(Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            return GetCachedData(CompanyID, IsArchived, IsDeleted).OrderBy("ID DESC");
        }


        public IEnumerable<RoomDTO> GetAllRoomByCompany(Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<RoomDTO> ObjCache = null;
            if (IsArchived == false && IsDeleted == false)
            {
                if (ObjCache == null)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        ObjCache = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                                    select new RoomDTO
                                    {
                                        ID = u.ID,
                                        RoomName = u.RoomName,
                                        CompanyName = u.CompanyName,
                                        ContactName = u.ContactName,
                                        streetaddress = u.streetaddress,
                                        City = u.City,
                                        State = u.State,
                                        PostalCode = u.PostalCode,
                                        Country = u.Country,
                                        Email = u.Email,
                                        PhoneNo = u.PhoneNo,
                                        InvoiceBranch = u.InvoiceBranch,
                                        CustomerNumber = u.CustomerNumber,
                                        BlanketPO = u.BlanketPO,
                                        IsConsignment = u.IsConsignment,
                                        IsTrending = u.IsTrending,
                                        SourceOfTrending = u.SourceOfTrending,
                                        TrendingFormula = u.TrendingFormula,
                                        TrendingFormulaType = u.TrendingFormulaType,
                                        TrendingFormulaDays = u.TrendingFormulaDays,
                                        TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                        TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                        TrendingFormulaCounts = u.TrendingFormulaCounts,
                                        //   ManualMin = u.ManualMin,
                                        SuggestedOrder = u.SuggestedOrder,
                                        SuggestedTransfer = u.SuggestedTransfer,
                                        ReplineshmentRoom = u.ReplineshmentRoom,
                                        //IsItemReplenishment = u.IsItemReplenishment,
                                        //IsBinReplenishment = u.IsBinReplenishment,
                                        ReplenishmentType = u.ReplenishmentType,
                                        IseVMI = u.IseVMI,
                                        MaxOrderSize = u.MaxOrderSize,
                                        HighPO = u.HighPO,
                                        HighJob = u.HighJob,
                                        HighTransfer = u.HighTransfer,
                                        HighCount = u.HighCount,
                                        GlobMarkupParts = u.GlobMarkupParts,
                                        GlobMarkupLabor = u.GlobMarkupLabor,
                                        IsTax1Parts = u.IsTax1Parts,
                                        IsTax1Labor = u.IsTax1Labor,
                                        Tax1name = u.Tax1name,
                                        Tax1Rate = u.Tax1Rate,
                                        IsTax2Parts = u.IsTax2Parts,
                                        IsTax2Labor = u.IsTax2Labor,
                                        tax2name = u.tax2name,
                                        Tax2Rate = u.Tax2Rate,
                                        IsTax2onTax1 = u.IsTax2onTax1,
                                        GXPRConsJob = u.GXPRConsJob,
                                        CostCenter = u.CostCenter,
                                        UniqueID = u.UniqueID,
                                        Created = u.Created,
                                        Updated = u.Updated,
                                        CreatedBy = u.CreatedBy,
                                        LastUpdatedBy = u.LastUpdatedBy,
                                        IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                        IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                        CreatedByName = u.CreatedByName,
                                        UpdatedByName = u.UpdatedByName,
                                        MethodOfValuingInventory = u.MethodOfValuingInventory,
                                        AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                        AutoCreateTransferTime = u.AutoCreateTransferTime,
                                        AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                        IsActive = u.IsActive,
                                        LicenseBilled = u.LicenseBilled,
                                        NextCountNo = u.NextCountNo,
                                        NextOrderNo = u.NextOrderNo,
                                        POAutoSequence = u.POAutoSequence,
                                        IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                        IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                        SlowMovingValue = u.SlowMovingValue,
                                        FastMovingValue = u.FastMovingValue,
                                        NextRequisitionNo = u.NextRequisitionNo,
                                        NextStagingNo = u.NextStagingNo,
                                        NextTransferNo = u.NextTransferNo,
                                        NextWorkOrderNo = u.NextWorkOrderNo,
                                        RoomGrouping = u.RoomGrouping,

                                        TransferFrequencyOption = u.TransferFrequencyOption,
                                        TransferFrequencyDays = u.TransferFrequencyDays,
                                        TransferFrequencyMonth = u.TransferFrequencyMonth,
                                        TransferFrequencyNumber = u.TransferFrequencyNumber,
                                        TransferFrequencyWeek = u.TransferFrequencyWeek,
                                        TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                        TrendingSampleSize = u.TrendingSampleSize,
                                        TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                        AverageUsageTransactions = u.AverageUsageTransactions,
                                        AverageUsageSampleSize = u.AverageUsageSampleSize,
                                        AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                        GUID = u.GUID,
                                        CompanyID = u.CompanyID,
                                        UDF1 = u.UDF1,
                                        UDF2 = u.UDF2,
                                        UDF3 = u.UDF3,
                                        UDF4 = u.UDF4,
                                        UDF5 = u.UDF5,
                                        DefaultSupplierID = u.DefaultSupplierID,
                                        NextAssetNo = u.NextAssetNo,
                                        NextBinNo = u.NextBinNo,
                                        NextKitNo = u.NextKitNo,
                                        NextItemNo = u.NextItemNo,
                                        NextProjectSpendNo = u.NextProjectSpendNo,
                                        NextToolNo = u.NextToolNo,
                                        InventoryConsuptionMethod = u.InventoryConsuptionMethod,
                                        DefaultBinID = u.DefaultBinID,
                                        RequestedXDays = u.RequestedXDays ?? 0,
                                        RequestedYDays = u.RequestedYDays ?? 0,
                                        IsRoomActive = u.IsRoomActive,
                                        BaseOfInventory = u.BaseOfInventory,
                                        eVMIWaitCommand = u.eVMIWaitCommand,
                                        eVMIWaitPort = u.eVMIWaitPort,
                                        IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                        LastReceivedDate = u.LastReceivedDate,
                                        LastTrasnferedDate = u.LastTrasnferedDate,
                                        ExtPhoneNo = u.ExtPhoneNo,
                                        ReqAutoSequence = u.ReqAutoSequence,
                                        AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                        IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                        IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                        AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                        PullRejectionType = u.PullRejectionType,
                                        BillingRoomType = u.BillingRoomType,

                                        StagingAutoSequence = u.StagingAutoSequence,
                                        TransferAutoSequence = u.TransferAutoSequence,
                                        WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                        StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                        TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                        WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                        WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                        MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                        DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                        AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                        PreventMaxOrderQty = u.PreventMaxOrderQty,
                                        DefaultCountType = u.DefaultCountType,
                                        TAOAutoSequence = u.TAOAutoSequence,
                                        TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                        NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                        AllowToolOrdering = u.AllowToolOrdering,
                                        IsWOSignatureRequired = u.IsWOSignatureRequired,
                                        IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                        IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                        ToolCountAutoSequence = u.ToolCountAutoSequence,
                                        NextToolCountNo = u.NextToolCountNo,

                                        ForceSupplierFilter = u.ForceSupplierFilter
                                    }).AsParallel().ToList();


                    }
                }
            }
            else
            {
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
                    ObjCache = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.CompanyID = " + CompanyID.ToString() + @" AND " + sSQL)
                                select new RoomDTO
                                {
                                    ID = u.ID,
                                    RoomName = u.RoomName,
                                    CompanyName = u.CompanyName,
                                    ContactName = u.ContactName,
                                    streetaddress = u.streetaddress,
                                    City = u.City,
                                    State = u.State,
                                    PostalCode = u.PostalCode,
                                    Country = u.Country,
                                    Email = u.Email,
                                    PhoneNo = u.PhoneNo,
                                    InvoiceBranch = u.InvoiceBranch,
                                    CustomerNumber = u.CustomerNumber,
                                    BlanketPO = u.BlanketPO,
                                    IsConsignment = u.IsConsignment,
                                    IsTrending = u.IsTrending,
                                    SourceOfTrending = u.SourceOfTrending,
                                    TrendingFormula = u.TrendingFormula,
                                    TrendingFormulaType = u.TrendingFormulaType,
                                    TrendingFormulaDays = u.TrendingFormulaDays,
                                    TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                    TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                    TrendingFormulaCounts = u.TrendingFormulaCounts,
                                    //   ManualMin = u.ManualMin,
                                    SuggestedOrder = u.SuggestedOrder,
                                    SuggestedTransfer = u.SuggestedTransfer,
                                    ReplineshmentRoom = u.ReplineshmentRoom,
                                    //IsItemReplenishment = u.IsItemReplenishment,
                                    //IsBinReplenishment = u.IsBinReplenishment,
                                    ReplenishmentType = u.ReplenishmentType,
                                    IseVMI = u.IseVMI,
                                    MaxOrderSize = u.MaxOrderSize,
                                    HighPO = u.HighPO,
                                    HighJob = u.HighJob,
                                    HighTransfer = u.HighTransfer,
                                    HighCount = u.HighCount,
                                    GlobMarkupParts = u.GlobMarkupParts,
                                    GlobMarkupLabor = u.GlobMarkupLabor,
                                    IsTax1Parts = u.IsTax1Parts,
                                    IsTax1Labor = u.IsTax1Labor,
                                    Tax1name = u.Tax1name,
                                    Tax1Rate = u.Tax1Rate,
                                    IsTax2Parts = u.IsTax2Parts,
                                    IsTax2Labor = u.IsTax2Labor,
                                    tax2name = u.tax2name,
                                    Tax2Rate = u.Tax2Rate,
                                    IsTax2onTax1 = u.IsTax2onTax1,
                                    GXPRConsJob = u.GXPRConsJob,
                                    CostCenter = u.CostCenter,
                                    UniqueID = u.UniqueID,
                                    Created = u.Created,
                                    Updated = u.Updated,
                                    CreatedBy = u.CreatedBy,
                                    LastUpdatedBy = u.LastUpdatedBy,
                                    IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                    IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                    CreatedByName = u.CreatedByName,
                                    UpdatedByName = u.UpdatedByName,
                                    MethodOfValuingInventory = u.MethodOfValuingInventory,
                                    AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                    AutoCreateTransferTime = u.AutoCreateTransferTime,
                                    AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                    IsActive = u.IsActive,
                                    LicenseBilled = u.LicenseBilled,
                                    NextCountNo = u.NextCountNo,
                                    POAutoSequence = u.POAutoSequence,
                                    IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                    IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                    SlowMovingValue = u.SlowMovingValue,
                                    FastMovingValue = u.FastMovingValue,
                                    NextOrderNo = u.NextOrderNo,
                                    NextRequisitionNo = u.NextRequisitionNo,
                                    NextStagingNo = u.NextStagingNo,
                                    NextTransferNo = u.NextTransferNo,
                                    NextWorkOrderNo = u.NextWorkOrderNo,
                                    RoomGrouping = u.RoomGrouping,

                                    TransferFrequencyOption = u.TransferFrequencyOption,
                                    TransferFrequencyDays = u.TransferFrequencyDays,
                                    TransferFrequencyMonth = u.TransferFrequencyMonth,
                                    TransferFrequencyNumber = u.TransferFrequencyNumber,
                                    TransferFrequencyWeek = u.TransferFrequencyWeek,
                                    TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                    TrendingSampleSize = u.TrendingSampleSize,
                                    TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                    AverageUsageTransactions = u.AverageUsageTransactions,
                                    AverageUsageSampleSize = u.AverageUsageSampleSize,
                                    AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                    GUID = u.GUID,
                                    CompanyID = u.CompanyID,
                                    UDF1 = u.UDF1,
                                    UDF2 = u.UDF2,
                                    UDF3 = u.UDF3,
                                    UDF4 = u.UDF4,
                                    UDF5 = u.UDF5,
                                    DefaultSupplierID = u.DefaultSupplierID,
                                    NextAssetNo = u.NextAssetNo,
                                    NextBinNo = u.NextBinNo,
                                    NextKitNo = u.NextKitNo,
                                    NextItemNo = u.NextItemNo,
                                    NextProjectSpendNo = u.NextProjectSpendNo,
                                    NextToolNo = u.NextToolNo,
                                    InventoryConsuptionMethod = u.InventoryConsuptionMethod,
                                    DefaultBinID = u.DefaultBinID,
                                    RequestedXDays = u.RequestedXDays ?? 0,
                                    RequestedYDays = u.RequestedYDays ?? 0,
                                    IsRoomActive = u.IsRoomActive,
                                    BaseOfInventory = u.BaseOfInventory,
                                    eVMIWaitCommand = u.eVMIWaitCommand,
                                    eVMIWaitPort = u.eVMIWaitPort,
                                    IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                    LastReceivedDate = u.LastReceivedDate,
                                    LastTrasnferedDate = u.LastTrasnferedDate,
                                    ReqAutoSequence = u.ReqAutoSequence,
                                    AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                    IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                    IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                    AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                    PullRejectionType = u.PullRejectionType,
                                    BillingRoomType = u.BillingRoomType,

                                    StagingAutoSequence = u.StagingAutoSequence,
                                    TransferAutoSequence = u.TransferAutoSequence,
                                    WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                    StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                    TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                    WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                    WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                    MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                    DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                    AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                    PreventMaxOrderQty = u.PreventMaxOrderQty,
                                    DefaultCountType = u.DefaultCountType,
                                    TAOAutoSequence = u.TAOAutoSequence,
                                    TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                    NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                    AllowToolOrdering = u.AllowToolOrdering,
                                    IsWOSignatureRequired = u.IsWOSignatureRequired,
                                    IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                    IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                    ToolCountAutoSequence = u.ToolCountAutoSequence,
                                    NextToolCountNo = u.NextToolCountNo,

                                    ForceSupplierFilter = u.ForceSupplierFilter
                                }).AsParallel().ToList();
                }

            }

            return ObjCache;//.Where(t => t.Room == RoomID);
        }


        /// <summary>
        /// Get Paged Records from the Room Master Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<RoomDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 CompanyId, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<RoomDTO> ObjCache = GetAllRoomByCompany(CompanyId, IsArchived, IsDeleted);

            if (String.IsNullOrEmpty(SearchTerm))
            {
                //Get Cached-Media
                //IEnumerable<RoomDTO> ObjCache = GetCachedData(CompanyId, IsArchived, IsDeleted);
                TotalCount = ObjCache.Count();
                return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);
            }
            else if (SearchTerm.Contains("[###]"))
            {
                //Get Cached-Media
                //IEnumerable<RoomDTO> ObjCache = GetCachedData(CompanyId, IsArchived, IsDeleted);

                //string[] dd = SearchTerm.Replace("STARTWITH#", "$").Split('$');
                //string[] stringSeparators = new string[] { "[###]" };

                //if (dd != null && dd.Length > 0)
                //{
                //    string[] Fields = dd[1].Split(stringSeparators, StringSplitOptions.None);
                //    // 6 counts for fields based on that prepare the search string
                //    // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo
                //    foreach (var item in Fields)
                //    {
                //        if (item.Length > 0)
                //        {
                //            if (item.Contains("CreatedBy"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.CreatedByName.ToString()));
                //            }
                //            else if (item.Contains("UpdatedBy"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UpdatedByName.ToString()));
                //            }
                //            else if (item.Contains("DateCreatedFrom"))
                //            {
                //                ObjCache = ObjCache.Where(t => t.Created.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Created.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                //            }
                //            else if (item.Contains("DateUpdatedFrom"))
                //            {
                //                ObjCache = ObjCache.Where(t => t.Updated.Value.Date >= Convert.ToDateTime(item.Split('#')[1]).Date && t.Updated.Value.Date <= Convert.ToDateTime(item.Split('#')[3]).Date);
                //            }
                //            else if (item.Contains("UDF1"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF1));
                //            }
                //            else if (item.Contains("UDF2"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF2));
                //            }
                //            else if (item.Contains("UDF3"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF3));
                //            }
                //            else if (item.Contains("UDF4"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF4));
                //            }
                //            else if (item.Contains("UDF5"))
                //            {
                //                ObjCache = ObjCache.Where(t => item.Split('#')[1].Split(',').ToList().Contains(t.UDF5));
                //            }
                //        }
                //    }
                //}
                //TotalCount = ObjCache.Count();
                //return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

                //IEnumerable<TechnicianMasterDTO> ObjCache = GetCachedData(RoomID, CompanyID);
                string[] stringSeparators = new string[] { "[###]" };
                string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                //CreatedBy,UpdatedBy,DateCreatedFrom,DateUpdatedFrom,UDF1,UDF2,UDF3,UDF4,UDF5,[###]admin,niraj$$$$$$$test2$$
                // 1 = createdBy, 2 = updatedby , 3 = CreatedFrom, 4 = CreatedTo, 5=UpdatedFrom,6=UpdatedTo

                ObjCache = ObjCache.Where(t =>
                       ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
                    && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
                    && ((Fields[1].Split('@')[2] == "") || (t.Created.Value.Date >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created.Value.Date <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
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
                //IEnumerable<RoomDTO> ObjCache = GetCachedData();
                TotalCount = ObjCache.Where
                    (
                        t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ContactName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.streetaddress ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.State ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.PostalCode ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.PhoneNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.eVMIWaitCommand.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.eVMIWaitPort.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).Count();
                return ObjCache.Where
                    (t => t.ID.ToString().Contains(SearchTerm) ||
                        (t.RoomName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.ContactName ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.streetaddress ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.City ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.State ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.PostalCode ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.Email ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.PhoneNo ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF1 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF2 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF3 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF4 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.UDF5 ?? "").IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.eVMIWaitCommand.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (t.eVMIWaitPort.ToString()).IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0
                    ).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

            }

            #region Previous Code
            #endregion
        }


        public RoomDTO GetRoomByName(Int64 CompanyId, string RoomName)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Rooms
                        join cm in context.CompanyMasters on u.CompanyID equals cm.ID into rm_cm_join
                        from rm_cm in rm_cm_join.DefaultIfEmpty()
                        where rm_cm.ID == CompanyId && u.RoomName == RoomName
                        && u.IsDeleted == false && u.IsArchived == false
                        select new RoomDTO
                        {
                            ID = u.ID,
                            RoomName = u.RoomName,
                            CompanyName = rm_cm.Name,
                            ContactName = u.ContactName,
                            streetaddress = u.StreetAddress,
                            City = u.City,
                            State = u.State,
                            PostalCode = u.PostalCode,
                            Country = u.Country,
                            Email = u.Email,
                            PhoneNo = u.PhoneNo,
                            InvoiceBranch = u.InvoiceBranch,
                            CustomerNumber = u.CustomerNumber,
                            BlanketPO = u.BlanketPO,
                            IsConsignment = u.IsConsignment,
                            IsTrending = u.IsTrending,
                            SourceOfTrending = u.SourceOfTrending,
                            TrendingFormula = u.TrendingFormula,
                            TrendingFormulaType = u.TrendingFormulaType,
                            TrendingFormulaDays = u.TrendingFormulaDays,
                            TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                            TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                            TrendingFormulaCounts = u.TrendingFormulaCounts,
                            //  ManualMin = u.ManualMin,
                            SuggestedOrder = u.SuggestedOrder,
                            SuggestedTransfer = u.SuggestedTransfer,
                            ReplineshmentRoom = u.ReplineshmentRoom,
                            //IsItemReplenishment = u.IsItemReplenishment,
                            //IsBinReplenishment = u.IsBinReplenishment ?? false,
                            ReplenishmentType = u.ReplenishmentType,
                            IseVMI = u.IseVMI,
                            MaxOrderSize = u.MaxOrderSize,
                            HighPO = u.HighPO,
                            HighJob = u.HighJob,
                            HighTransfer = u.HighTransfer,
                            HighCount = u.HighCount,
                            GlobMarkupParts = u.GlobMarkupParts,
                            GlobMarkupLabor = u.GlobMarkupLabor,
                            IsTax1Parts = u.IsTax1Parts,
                            IsTax1Labor = u.IsTax1Labor,
                            Tax1name = u.Tax1Name,
                            Tax1Rate = u.Tax1Rate,
                            IsTax2Parts = u.IsTax2Parts,
                            IsTax2Labor = u.IsTax2Labor,
                            tax2name = u.Tax2Name,
                            Tax2Rate = u.Tax2Rate,

                            GXPRConsJob = u.GXPRConsJob,
                            CostCenter = u.CostCenter,
                            UniqueID = u.UniqueID,
                            Created = u.Created ?? DateTime.MinValue,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            MethodOfValuingInventory = u.MethodOfValuingInventory,
                            AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                            AutoCreateTransferTime = u.AutoCreateTransferTime,
                            AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                            IsActive = u.IsActive,
                            LicenseBilled = u.LicenseBilled,
                            NextCountNo = u.NextCountNo,
                            POAutoSequence = u.POAutoSequence,
                            IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                            IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                            SlowMovingValue = u.SlowMovingValue,
                            FastMovingValue = u.FastMovingValue,
                            NextOrderNo = u.NextOrderNo,
                            NextRequisitionNo = u.NextRequisitionNo,
                            NextStagingNo = u.NextStagingNo,
                            NextTransferNo = u.NextTransferNo,
                            NextWorkOrderNo = u.NextWorkOrderNo,
                            RoomGrouping = u.RoomGrouping,
                            TransferFrequencyOption = u.TransferFrequencyOption,
                            TransferFrequencyDays = u.TransferFrequencyDays,
                            TransferFrequencyMonth = u.TransferFrequencyMonth,
                            TransferFrequencyNumber = u.TransferFrequencyNumber,
                            TransferFrequencyWeek = u.TransferFrequencyWeek,
                            TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                            TrendingSampleSize = u.TrendingSampleSize,
                            TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                            AverageUsageTransactions = u.AverageUsageTransactions,
                            AverageUsageSampleSize = u.AverageUsageSampleSize,
                            AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            NextAssetNo = u.NextAssetNo,
                            NextBinNo = u.NextBinNo,
                            NextKitNo = u.NextKitNo,
                            NextItemNo = u.NextItemNo,
                            NextProjectSpendNo = u.NextProjectSpendNo,
                            NextToolNo = u.NextToolNo,
                            InventoryConsuptionMethod = u.InventoryConsuptionMethod,
                            DefaultBinID = u.DefaultBinID,
                            DefaultSupplierID = u.DefaultSupplierID,
                            IsRoomActive = u.IsRoomActive,
                            RequestedXDays = u.RequestedXDays ?? 0,
                            RequestedYDays = u.RequestedYDays ?? 0,
                            IsCompanyActive = rm_cm.IsActive,
                            BaseOfInventory = u.BaseOfInventory,
                            eVMIWaitCommand = u.eVMIWaitCommand,
                            eVMIWaitPort = u.eVMIWaitPort,
                            IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                            IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                            ExtPhoneNo = u.ExtPhoneNo,
                            LastReceivedDate = u.LastReceivedDate,
                            LastTrasnferedDate = u.LastTrasnferedDate,
                            ReqAutoSequence = u.ReqAutoSequence,
                            CompanyID = u.CompanyID,
                            AllowInsertingItemOnScan = u.AllowInsertingItemOnScan ?? false,
                            DefaultBinName = u.DefaultBinName,
                            AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                            PullRejectionType = u.PullRejectionType,
                            BillingRoomType = u.BillingRoomType,

                            StagingAutoSequence = u.StagingAutoSequence,
                            TransferAutoSequence = u.TransferAutoSequence,
                            WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                            WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                            IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                            AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                            PreventMaxOrderQty = u.PreventMaxOrderQty,
                            DefaultCountType = u.DefaultCountType,
                            TAOAutoSequence = u.TAOAutoSequence,
                            TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                            NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                            AllowToolOrdering = u.AllowToolOrdering,
                            IsWOSignatureRequired = u.IsWOSignatureRequired,
                            IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                            IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                            ToolCountAutoSequence = u.ToolCountAutoSequence,
                            NextToolCountNo = u.NextToolCountNo,

                            ForceSupplierFilter = u.ForceSupplierFilter
                        }).FirstOrDefault();
            }
        }

        public long GetLanguageId(string Culturename)
        {
            long languageid = 0;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                languageid = Convert.ToInt64(context.ResourceLaguages.Where(u => u.Culture == Culturename).Select(p => p.ID).SingleOrDefault());
            }
            return languageid;
        }
        /// <summary>
        /// Delete Particullar Record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool Delete(Int64 id, Int64 userid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Room obj = context.Rooms.Single(t => t.ID == id);
                obj.IsDeleted = true;
                obj.IsArchived = false;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.LastUpdatedBy = userid;
                context.Rooms.Attach(obj);
                context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
                context.SaveChanges();

                //Get Cached-Media
                IEnumerable<RoomDTO> ObjCache = CacheHelper<IEnumerable<RoomDTO>>.GetCacheItem("Cached_Room_");
                if (ObjCache != null)
                {
                    List<RoomDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => i.ID == id);
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<RoomDTO>>.AppendToCacheItem("Cached_Room_", ObjCache);
                }
                return true;
            }
        }
        public void SaveEmailTemplateByRoom(long CompanyId, long RoomId, List<EmailTemplateDTO> lstEmailTemplateDTO, long userID, long CultureID)
        {
            if (lstEmailTemplateDTO != null)
            {
                foreach (EmailTemplateDTO item in lstEmailTemplateDTO)
                {
                    using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                    {
                        long TemplateId = 0;
                        EmailTemplateDTO objEmailTemplateDTO = new EmailTemplateDTO();
                        EmailTemplate obj = context.EmailTemplates.SingleOrDefault(x => x.TemplateName == item.TemplateName);
                        if (obj == null)
                        {

                            objEmailTemplateDTO.TemplateName = item.TemplateName;
                            objEmailTemplateDTO.RoomId = RoomId;
                            objEmailTemplateDTO.CompanyId = CompanyId;
                            objEmailTemplateDTO.CreatedBy = userID;
                            objEmailTemplateDTO.LastUpdatedBy = userID;
                            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                            TemplateId = objEmailTemplateDAL.InsertEmailTemplateMaster(objEmailTemplateDTO);
                        }
                        else
                        {
                            TemplateId = obj.ID;
                        }
                        if (TemplateId > 0)
                        {
                            EmailTemplateDetailDTO objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                            objEmailTemplateDetailDTO.EmailTempateId = TemplateId;
                            objEmailTemplateDetailDTO.MailBodyText = item.MailBodyText;

                            objEmailTemplateDetailDTO.RoomId = RoomId;
                            objEmailTemplateDetailDTO.CompanyID = CompanyId;
                            objEmailTemplateDetailDTO.CreatedBy = userID;
                            objEmailTemplateDetailDTO.LastUpdatedBy = userID;
                            objEmailTemplateDetailDTO.ResourceLaguageId = CultureID;// GetLanguageId("en-US");
                            EmailTemplateDAL objEmailTemplateDAL = new EmailTemplateDAL(base.DataBaseName);
                            objEmailTemplateDAL.SaveEmailTemplate(objEmailTemplateDetailDTO);
                        }
                    }
                }
            }
            else
            {
                return;
            }

        }

        public string RoomEmailDuplicateCheck(long ID, string EmailAddress, long CompanyID)
        {
            string msg = "";
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var qry = (from em in context.Rooms
                           where em.Email == EmailAddress && em.IsArchived == false && em.IsDeleted == false && em.ID != ID && em.CompanyID == CompanyID
                           select em);
                if (qry.Any())
                {
                    msg = "duplicate";
                }
                else
                {
                    msg = "ok";
                }
            }
            return msg;
        }



        public List<RoomDTO> GetAllRooms(long CompanyId, bool IsDeleted, bool IsArchived, List<RoomDTO> lstRoomsIds)
        {
            long[] RoomIds = new long[] { };
            if (lstRoomsIds == null)
            {
                lstRoomsIds = new List<RoomDTO>();
            }
            RoomIds = lstRoomsIds.Select(t => t.ID).ToArray();
            List<RoomDTO> lstRooms = new List<RoomDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstRooms = (from u in context.Rooms
                            join cby in context.UserMasters on u.CreatedBy equals cby.ID into u_cby_join
                            from u_cby in u_cby_join.DefaultIfEmpty()
                            join uby in context.UserMasters on u.LastUpdatedBy equals uby.ID into u_uby_join
                            from u_uby in u_uby_join.DefaultIfEmpty()
                            where RoomIds.Contains(u.ID) && u.IsDeleted == IsDeleted && u.IsArchived == IsArchived && u.CompanyID == CompanyId
                            select new RoomDTO
                            {
                                ID = u.ID,
                                RoomName = u.RoomName,
                                CompanyName = u.CompanyName,
                                ContactName = u.ContactName,
                                streetaddress = u.StreetAddress,
                                City = u.City,
                                State = u.State,
                                PostalCode = u.PostalCode,
                                Country = u.Country,
                                Email = u.Email,
                                PhoneNo = u.PhoneNo,
                                InvoiceBranch = u.InvoiceBranch,
                                CustomerNumber = u.CustomerNumber,
                                BlanketPO = u.BlanketPO,
                                IsConsignment = u.IsConsignment,
                                IsTrending = u.IsTrending,
                                SourceOfTrending = u.SourceOfTrending,
                                TrendingFormula = u.TrendingFormula,
                                TrendingFormulaType = u.TrendingFormulaType,
                                TrendingFormulaDays = u.TrendingFormulaDays,
                                TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                TrendingFormulaCounts = u.TrendingFormulaCounts,
                                SuggestedOrder = u.SuggestedOrder,
                                SuggestedTransfer = u.SuggestedTransfer,
                                ReplineshmentRoom = u.ReplineshmentRoom,
                                ReplenishmentType = u.ReplenishmentType,
                                IseVMI = u.IseVMI,
                                MaxOrderSize = u.MaxOrderSize,
                                HighPO = u.HighPO,
                                HighJob = u.HighJob,
                                HighTransfer = u.HighTransfer,
                                HighCount = u.HighCount,
                                GlobMarkupParts = u.GlobMarkupParts,
                                GlobMarkupLabor = u.GlobMarkupLabor,
                                IsTax1Parts = u.IsTax1Parts,
                                IsTax1Labor = u.IsTax1Labor,
                                Tax1name = u.Tax1Name,
                                Tax1Rate = u.Tax1Rate,
                                IsTax2Parts = u.IsTax2Parts,
                                IsTax2Labor = u.IsTax2Labor,
                                tax2name = u.Tax2Name,
                                Tax2Rate = u.Tax2Rate,
                                IsTax2onTax1 = u.IsTax2onTax1 ?? false,
                                GXPRConsJob = u.GXPRConsJob,
                                CostCenter = u.CostCenter,
                                UniqueID = u.UniqueID,
                                Created = u.Created ?? DateTime.MinValue,
                                Updated = u.Updated,
                                CreatedBy = u.CreatedBy,
                                LastUpdatedBy = u.LastUpdatedBy,
                                IsDeleted = u.IsDeleted,
                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                MethodOfValuingInventory = u.MethodOfValuingInventory,
                                AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                AutoCreateTransferTime = u.AutoCreateTransferTime,
                                AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                IsActive = u.IsActive,
                                LicenseBilled = u.LicenseBilled,
                                NextCountNo = u.NextCountNo,
                                IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                SlowMovingValue = u.SlowMovingValue,
                                FastMovingValue = u.FastMovingValue,
                                POAutoSequence = u.POAutoSequence,
                                NextOrderNo = u.NextOrderNo,
                                NextRequisitionNo = u.NextRequisitionNo,
                                NextStagingNo = u.NextStagingNo,
                                NextTransferNo = u.NextTransferNo,
                                NextWorkOrderNo = u.NextWorkOrderNo,
                                RoomGrouping = u.RoomGrouping,
                                TransferFrequencyOption = u.TransferFrequencyOption,
                                TransferFrequencyDays = u.TransferFrequencyDays,
                                TransferFrequencyMonth = u.TransferFrequencyMonth,
                                TransferFrequencyNumber = u.TransferFrequencyNumber,
                                TransferFrequencyWeek = u.TransferFrequencyWeek,
                                TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                                TrendingSampleSize = u.TrendingSampleSize,
                                TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,
                                AverageUsageTransactions = u.AverageUsageTransactions,
                                AverageUsageSampleSize = u.AverageUsageSampleSize,
                                AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                GUID = u.GUID,
                                CompanyID = u.CompanyID,
                                UDF1 = u.UDF1,
                                UDF2 = u.UDF2,
                                UDF3 = u.UDF3,
                                UDF4 = u.UDF4,
                                UDF5 = u.UDF5,
                                DefaultSupplierID = u.DefaultSupplierID,
                                NextAssetNo = u.NextAssetNo,
                                NextBinNo = u.NextBinNo,
                                NextKitNo = u.NextKitNo,
                                NextItemNo = u.NextItemNo,
                                NextProjectSpendNo = u.NextProjectSpendNo,
                                NextToolNo = u.NextToolNo,
                                DefaultBinID = u.DefaultBinID,
                                IsRoomActive = u.IsRoomActive,
                                RequestedXDays = u.RequestedXDays ?? 0,
                                RequestedYDays = u.RequestedYDays ?? 0,
                                InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                CreatedByName = u_cby.UserName,
                                UpdatedByName = u_uby.UserName,
                                BaseOfInventory = u.BaseOfInventory,
                                eVMIWaitCommand = u.eVMIWaitCommand,
                                eVMIWaitPort = u.eVMIWaitPort,
                                IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                                LastReceivedDate = u.LastReceivedDate,
                                LastTrasnferedDate = u.LastTrasnferedDate,
                                ExtPhoneNo = u.ExtPhoneNo,
                                ReqAutoSequence = u.ReqAutoSequence,
                                AllowInsertingItemOnScan = u.AllowInsertingItemOnScan ?? false,
                                IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                PullRejectionType = u.PullRejectionType,
                                BillingRoomType = u.BillingRoomType,

                                StagingAutoSequence = u.StagingAutoSequence,
                                TransferAutoSequence = u.TransferAutoSequence,
                                WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                PreventMaxOrderQty = u.PreventMaxOrderQty,
                                DefaultCountType = u.DefaultCountType,
                                TAOAutoSequence = u.TAOAutoSequence,
                                TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                AllowToolOrdering = u.AllowToolOrdering,
                                IsWOSignatureRequired = u.IsWOSignatureRequired,
                                IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                ToolCountAutoSequence = u.ToolCountAutoSequence,
                                NextToolCountNo = u.NextToolCountNo,

                                ForceSupplierFilter = u.ForceSupplierFilter
                            }).ToList();
            }
            return lstRooms;
        }

        public IEnumerable<RoomDTO> GetAllRecords(long CompanyId, string DBConnectionstring)
        {
            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
                                            select new RoomDTO
                                            {
                                                ID = u.ID,
                                                RoomName = u.RoomName,
                                                CompanyName = u.CompanyName,
                                                ContactName = u.ContactName,
                                                streetaddress = u.streetaddress,
                                                City = u.City,
                                                State = u.State,
                                                PostalCode = u.PostalCode,
                                                Country = u.Country,
                                                Email = u.Email,
                                                PhoneNo = u.PhoneNo,
                                                InvoiceBranch = u.InvoiceBranch,
                                                CustomerNumber = u.CustomerNumber,
                                                BlanketPO = u.BlanketPO,
                                                IsConsignment = u.IsConsignment,
                                                IsTrending = u.IsTrending,
                                                SourceOfTrending = u.SourceOfTrending,
                                                TrendingFormula = u.TrendingFormula,
                                                TrendingFormulaType = u.TrendingFormulaType,
                                                TrendingFormulaDays = u.TrendingFormulaDays,
                                                TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                                TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                                TrendingFormulaCounts = u.TrendingFormulaCounts,
                                                //   ManualMin = u.ManualMin,
                                                SuggestedOrder = u.SuggestedOrder,
                                                SuggestedTransfer = u.SuggestedTransfer,
                                                ReplineshmentRoom = u.ReplineshmentRoom,
                                                //IsItemReplenishment = u.IsItemReplenishment,
                                                //IsBinReplenishment = u.IsBinReplenishment,
                                                ReplenishmentType = u.ReplenishmentType,
                                                IseVMI = u.IseVMI,
                                                MaxOrderSize = u.MaxOrderSize,
                                                HighPO = u.HighPO,
                                                HighJob = u.HighJob,
                                                HighTransfer = u.HighTransfer,
                                                HighCount = u.HighCount,
                                                GlobMarkupParts = u.GlobMarkupParts,
                                                GlobMarkupLabor = u.GlobMarkupLabor,
                                                IsTax1Parts = u.IsTax1Parts,
                                                IsTax1Labor = u.IsTax1Labor,
                                                Tax1name = u.Tax1name,
                                                Tax1Rate = u.Tax1Rate,
                                                IsTax2Parts = u.IsTax2Parts,
                                                IsTax2Labor = u.IsTax2Labor,
                                                tax2name = u.tax2name,
                                                Tax2Rate = u.Tax2Rate,
                                                IsTax2onTax1 = u.IsTax2onTax1,
                                                GXPRConsJob = u.GXPRConsJob,
                                                CostCenter = u.CostCenter,
                                                UniqueID = u.UniqueID,
                                                Created = u.Created,
                                                Updated = u.Updated,
                                                CreatedBy = u.CreatedBy,
                                                LastUpdatedBy = u.LastUpdatedBy,
                                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                CreatedByName = u.CreatedByName,
                                                UpdatedByName = u.UpdatedByName,
                                                MethodOfValuingInventory = u.MethodOfValuingInventory,
                                                AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                                AutoCreateTransferTime = u.AutoCreateTransferTime,
                                                AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                                IsActive = u.IsActive,
                                                LicenseBilled = u.LicenseBilled,
                                                NextCountNo = u.NextCountNo,
                                                IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                                IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                                SlowMovingValue = u.SlowMovingValue,
                                                FastMovingValue = u.FastMovingValue,
                                                POAutoSequence = u.POAutoSequence,
                                                NextOrderNo = u.NextOrderNo,
                                                NextRequisitionNo = u.NextRequisitionNo,
                                                NextStagingNo = u.NextStagingNo,
                                                NextTransferNo = u.NextTransferNo,
                                                NextWorkOrderNo = u.NextWorkOrderNo,
                                                RoomGrouping = u.RoomGrouping,

                                                TransferFrequencyOption = u.TransferFrequencyOption,
                                                TransferFrequencyDays = u.TransferFrequencyDays,
                                                TransferFrequencyMonth = u.TransferFrequencyMonth,
                                                TransferFrequencyNumber = u.TransferFrequencyNumber,
                                                TransferFrequencyWeek = u.TransferFrequencyWeek,
                                                TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                                TrendingSampleSize = u.TrendingSampleSize,
                                                TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                                AverageUsageTransactions = u.AverageUsageTransactions,
                                                AverageUsageSampleSize = u.AverageUsageSampleSize,
                                                AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                                GUID = u.GUID,
                                                CompanyID = u.CompanyID,
                                                UDF1 = u.UDF1,
                                                UDF2 = u.UDF2,
                                                UDF3 = u.UDF3,
                                                UDF4 = u.UDF4,
                                                UDF5 = u.UDF5,
                                                DefaultSupplierID = u.DefaultSupplierID,
                                                NextAssetNo = u.NextAssetNo,
                                                NextBinNo = u.NextBinNo,
                                                NextKitNo = u.NextKitNo,
                                                NextItemNo = u.NextItemNo,
                                                NextProjectSpendNo = u.NextProjectSpendNo,
                                                NextToolNo = u.NextToolNo,
                                                DefaultBinID = u.DefaultBinID,
                                                IsRoomActive = u.IsRoomActive,
                                                RequestedXDays = u.RequestedXDays ?? 0,
                                                RequestedYDays = u.RequestedYDays ?? 0,
                                                InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                                BaseOfInventory = u.BaseOfInventory,
                                                eVMIWaitCommand = u.eVMIWaitCommand,
                                                eVMIWaitPort = u.eVMIWaitPort,
                                                IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                                LastReceivedDate = u.LastReceivedDate,
                                                LastTrasnferedDate = u.LastTrasnferedDate,
                                                ExtPhoneNo = u.ExtPhoneNo,
                                                ReqAutoSequence = u.ReqAutoSequence,
                                                AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                                IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                                IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                                AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                                PullRejectionType = u.PullRejectionType,
                                                BillingRoomType = u.BillingRoomType,

                                                StagingAutoSequence = u.StagingAutoSequence,
                                                TransferAutoSequence = u.TransferAutoSequence,
                                                WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                                StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                                TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                                WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                                WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                                DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                                AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                                PreventMaxOrderQty = u.PreventMaxOrderQty,
                                                DefaultCountType = u.DefaultCountType,
                                                TAOAutoSequence = u.TAOAutoSequence,
                                                TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                                NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                                AllowToolOrdering = u.AllowToolOrdering,
                                                IsWOSignatureRequired = u.IsWOSignatureRequired,
                                                IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                                IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                ToolCountAutoSequence = u.ToolCountAutoSequence,
                                                NextToolCountNo = u.NextToolCountNo,

                                                ForceSupplierFilter = u.ForceSupplierFilter
                                            }).AsParallel().ToList();

                return obj.ToList();
            }

        }

        /// <summary>
        /// Get Particullar Record from the Room Master by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public RoomDTO GetRecord(Int64 id, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Rooms
                        join sm in context.SupplierMasters on u.DefaultSupplierID equals sm.ID into rm_sm_join
                        from rm_sm in rm_sm_join.DefaultIfEmpty()
                        join cm in context.CompanyMasters on u.CompanyID equals cm.ID into rm_cm_join
                        from rm_cm in rm_cm_join.DefaultIfEmpty()
                        join uc in context.UserMasters on u.CreatedBy equals uc.ID into rm_uc_join
                        from rm_uc in rm_uc_join.DefaultIfEmpty()
                        join uu in context.UserMasters on u.LastUpdatedBy equals uu.ID into rm_uu_join
                        from rm_uu in rm_uu_join.DefaultIfEmpty()
                        where u.ID == id
                        select new RoomDTO
                        {
                            ID = u.ID,
                            RoomName = u.RoomName,
                            CompanyName = rm_cm.Name,
                            ContactName = u.ContactName,
                            streetaddress = u.StreetAddress,
                            City = u.City,
                            State = u.State,
                            PostalCode = u.PostalCode,
                            Country = u.Country,
                            Email = u.Email,
                            PhoneNo = u.PhoneNo,
                            InvoiceBranch = u.InvoiceBranch,
                            CustomerNumber = u.CustomerNumber,
                            BlanketPO = u.BlanketPO,
                            IsConsignment = u.IsConsignment,
                            IsTrending = u.IsTrending,
                            SourceOfTrending = u.SourceOfTrending,
                            TrendingFormula = u.TrendingFormula,
                            TrendingFormulaType = u.TrendingFormulaType,
                            TrendingFormulaDays = u.TrendingFormulaDays,
                            TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                            TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                            TrendingFormulaCounts = u.TrendingFormulaCounts,
                            //   ManualMin = u.ManualMin,
                            SuggestedOrder = u.SuggestedOrder,
                            SuggestedTransfer = u.SuggestedTransfer,
                            ReplineshmentRoom = u.ReplineshmentRoom,
                            //IsItemReplenishment = u.IsItemReplenishment,
                            //IsBinReplenishment = u.IsBinReplenishment,
                            ReplenishmentType = u.ReplenishmentType,
                            IseVMI = u.IseVMI,
                            MaxOrderSize = u.MaxOrderSize,
                            HighPO = u.HighPO,
                            HighJob = u.HighJob,
                            HighTransfer = u.HighTransfer,
                            HighCount = u.HighCount,
                            GlobMarkupParts = u.GlobMarkupParts,
                            GlobMarkupLabor = u.GlobMarkupLabor,
                            IsTax1Parts = u.IsTax1Parts,
                            IsTax1Labor = u.IsTax1Labor,
                            Tax1name = u.Tax1Name,
                            Tax1Rate = u.Tax1Rate,
                            IsTax2Parts = u.IsTax2Parts,
                            IsTax2Labor = u.IsTax2Labor,
                            tax2name = u.Tax2Name,
                            Tax2Rate = u.Tax2Rate,
                            IsTax2onTax1 = u.IsTax2onTax1 ?? false,
                            GXPRConsJob = u.GXPRConsJob,
                            CostCenter = u.CostCenter,
                            UniqueID = u.UniqueID,
                            Created = u.Created ?? DateTime.MinValue,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived ?? false,
                            CreatedByName = rm_uc.UserName,
                            UpdatedByName = rm_uu.UserName,
                            MethodOfValuingInventory = u.MethodOfValuingInventory,
                            AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                            AutoCreateTransferTime = u.AutoCreateTransferTime,
                            AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                            IsActive = u.IsActive,
                            LicenseBilled = u.LicenseBilled,
                            NextCountNo = u.NextCountNo,
                            IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                            IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                            SlowMovingValue = u.SlowMovingValue,
                            FastMovingValue = u.FastMovingValue,
                            POAutoSequence = u.POAutoSequence,
                            NextOrderNo = u.NextOrderNo,
                            NextRequisitionNo = u.NextRequisitionNo,
                            NextStagingNo = u.NextStagingNo,
                            NextTransferNo = u.NextTransferNo,
                            NextWorkOrderNo = u.NextWorkOrderNo,
                            RoomGrouping = u.RoomGrouping,

                            TransferFrequencyOption = u.TransferFrequencyOption,
                            TransferFrequencyDays = u.TransferFrequencyDays,
                            TransferFrequencyMonth = u.TransferFrequencyMonth,
                            TransferFrequencyNumber = u.TransferFrequencyNumber,
                            TransferFrequencyWeek = u.TransferFrequencyWeek,
                            TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                            TrendingSampleSize = u.TrendingSampleSize,
                            TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                            AverageUsageTransactions = u.AverageUsageTransactions,
                            AverageUsageSampleSize = u.AverageUsageSampleSize,
                            AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            DefaultSupplierID = u.DefaultSupplierID,
                            NextAssetNo = u.NextAssetNo,
                            NextBinNo = u.NextBinNo,
                            NextKitNo = u.NextKitNo,
                            NextItemNo = u.NextItemNo,
                            NextProjectSpendNo = u.NextProjectSpendNo,
                            NextToolNo = u.NextToolNo,
                            DefaultBinID = u.DefaultBinID,
                            IsRoomActive = u.IsRoomActive,
                            RequestedXDays = u.RequestedXDays ?? 0,
                            RequestedYDays = u.RequestedYDays ?? 0,
                            InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                            DefaultBinName = u.DefaultBinName,
                            DefaultSupplierName = rm_sm.SupplierName,
                            BaseOfInventory = u.BaseOfInventory,
                            eVMIWaitCommand = u.eVMIWaitCommand,
                            eVMIWaitPort = u.eVMIWaitPort,
                            CountAutoSequence = u.CountAutoSequence,
                            ShelfLifeleadtimeOrdRpt = u.ShelfLifeleadtimeOrdRpt,
                            LeadTimeOrdRpt = u.LeadTimeOrdRpt,
                            IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                            LastReceivedDate = u.LastReceivedDate,
                            LastTrasnferedDate = u.LastTrasnferedDate,
                            ExtPhoneNo = u.ExtPhoneNo,
                            ReqAutoSequence = u.ReqAutoSequence,
                            AllowInsertingItemOnScan = u.AllowInsertingItemOnScan ?? false,
                            IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                            IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                            AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                            PullRejectionType = u.PullRejectionType,
                            BillingRoomType = u.BillingRoomType,

                            StagingAutoSequence = u.StagingAutoSequence,
                            TransferAutoSequence = u.TransferAutoSequence,
                            WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                            StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                            TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                            WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                            WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                            AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                            PreventMaxOrderQty = u.PreventMaxOrderQty,
                            DefaultCountType = u.DefaultCountType,
                            TAOAutoSequence = u.TAOAutoSequence,
                            TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                            NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                            AllowToolOrdering = u.AllowToolOrdering,
                            IsWOSignatureRequired = u.IsWOSignatureRequired,
                            IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                            IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                            ToolCountAutoSequence = u.ToolCountAutoSequence,
                            NextToolCountNo = u.NextToolCountNo,

                            ForceSupplierFilter = u.ForceSupplierFilter
                        }).FirstOrDefault();
            }
        }


        public RoomDTO GetRoomByID(Int64 id, List<long> arrValidModules = null)
        {

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                RoomDTO objRoomDTO = (from u in context.Rooms
                                      join sm in context.SupplierMasters on u.DefaultSupplierID equals sm.ID into rm_sm_join
                                      from rm_sm in rm_sm_join.DefaultIfEmpty()
                                      join cm in context.CompanyMasters on u.CompanyID equals cm.ID into rm_cm_join
                                      from rm_cm in rm_cm_join.DefaultIfEmpty()
                                      join uc in context.UserMasters on u.CreatedBy equals uc.ID into rm_uc_join
                                      from rm_uc in rm_uc_join.DefaultIfEmpty()
                                      join uu in context.UserMasters on u.LastUpdatedBy equals uu.ID into rm_uu_join
                                      from rm_uu in rm_uu_join.DefaultIfEmpty()
                                      where u.ID == id
                                      select new RoomDTO
                                      {
                                          ID = u.ID,
                                          RoomName = u.RoomName,
                                          CompanyName = rm_cm.Name,
                                          ContactName = u.ContactName,
                                          streetaddress = u.StreetAddress,
                                          City = u.City,
                                          State = u.State,
                                          PostalCode = u.PostalCode,
                                          Country = u.Country,
                                          Email = u.Email,
                                          PhoneNo = u.PhoneNo,
                                          InvoiceBranch = u.InvoiceBranch,
                                          CustomerNumber = u.CustomerNumber,
                                          BlanketPO = u.BlanketPO,
                                          IsConsignment = u.IsConsignment,
                                          IsTrending = u.IsTrending,
                                          SourceOfTrending = u.SourceOfTrending,
                                          TrendingFormula = u.TrendingFormula,
                                          TrendingFormulaType = u.TrendingFormulaType,
                                          TrendingFormulaDays = u.TrendingFormulaDays,
                                          TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                          TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                          TrendingFormulaCounts = u.TrendingFormulaCounts,
                                          //   ManualMin = u.ManualMin,
                                          SuggestedOrder = u.SuggestedOrder,
                                          SuggestedTransfer = u.SuggestedTransfer,
                                          ReplineshmentRoom = u.ReplineshmentRoom,
                                          //IsItemReplenishment = u.IsItemReplenishment,
                                          //IsBinReplenishment = u.IsBinReplenishment,
                                          ReplenishmentType = u.ReplenishmentType,
                                          IseVMI = u.IseVMI,
                                          MaxOrderSize = u.MaxOrderSize,
                                          HighPO = u.HighPO,
                                          HighJob = u.HighJob,
                                          HighTransfer = u.HighTransfer,
                                          HighCount = u.HighCount,
                                          GlobMarkupParts = u.GlobMarkupParts,
                                          GlobMarkupLabor = u.GlobMarkupLabor,
                                          IsTax1Parts = u.IsTax1Parts,
                                          IsTax1Labor = u.IsTax1Labor,
                                          Tax1name = u.Tax1Name,
                                          Tax1Rate = u.Tax1Rate,
                                          IsTax2Parts = u.IsTax2Parts,
                                          IsTax2Labor = u.IsTax2Labor,
                                          tax2name = u.Tax2Name,
                                          Tax2Rate = u.Tax2Rate,
                                          IsTax2onTax1 = u.IsTax2onTax1 ?? false,
                                          GXPRConsJob = u.GXPRConsJob,
                                          CostCenter = u.CostCenter,
                                          UniqueID = u.UniqueID,
                                          Created = u.Created ?? DateTime.MinValue,
                                          Updated = u.Updated,
                                          CreatedBy = u.CreatedBy,
                                          LastUpdatedBy = u.LastUpdatedBy,
                                          IsDeleted = u.IsDeleted,
                                          IsArchived = u.IsArchived ?? false,
                                          CreatedByName = rm_uc.UserName,
                                          UpdatedByName = rm_uu.UserName,
                                          MethodOfValuingInventory = u.MethodOfValuingInventory,
                                          AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                          AutoCreateTransferTime = u.AutoCreateTransferTime,
                                          AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                          IsActive = u.IsActive,
                                          LicenseBilled = u.LicenseBilled,
                                          NextCountNo = u.NextCountNo,
                                          IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                          IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                          SlowMovingValue = u.SlowMovingValue,
                                          FastMovingValue = u.FastMovingValue,
                                          POAutoSequence = u.POAutoSequence,
                                          NextOrderNo = u.NextOrderNo,
                                          NextRequisitionNo = u.NextRequisitionNo,
                                          NextStagingNo = u.NextStagingNo,
                                          NextTransferNo = u.NextTransferNo,
                                          NextWorkOrderNo = u.NextWorkOrderNo,
                                          RoomGrouping = u.RoomGrouping,
                                          IsCompanyActive = rm_cm.IsActive,
                                          TransferFrequencyOption = u.TransferFrequencyOption,
                                          TransferFrequencyDays = u.TransferFrequencyDays,
                                          TransferFrequencyMonth = u.TransferFrequencyMonth,
                                          TransferFrequencyNumber = u.TransferFrequencyNumber,
                                          TransferFrequencyWeek = u.TransferFrequencyWeek,
                                          TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                          TrendingSampleSize = u.TrendingSampleSize,
                                          TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                          AverageUsageTransactions = u.AverageUsageTransactions,
                                          AverageUsageSampleSize = u.AverageUsageSampleSize,
                                          AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                          GUID = u.GUID,
                                          CompanyID = u.CompanyID,
                                          UDF1 = u.UDF1,
                                          UDF2 = u.UDF2,
                                          UDF3 = u.UDF3,
                                          UDF4 = u.UDF4,
                                          UDF5 = u.UDF5,
                                          DefaultSupplierID = u.DefaultSupplierID,
                                          NextAssetNo = u.NextAssetNo,
                                          NextBinNo = u.NextBinNo,
                                          NextKitNo = u.NextKitNo,
                                          NextItemNo = u.NextItemNo,
                                          NextProjectSpendNo = u.NextProjectSpendNo,
                                          NextToolNo = u.NextToolNo,
                                          DefaultBinID = u.DefaultBinID,
                                          IsRoomActive = u.IsRoomActive,
                                          RequestedXDays = u.RequestedXDays ?? 0,
                                          RequestedYDays = u.RequestedYDays ?? 0,
                                          InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                          DefaultBinName = u.DefaultBinName,
                                          DefaultSupplierName = rm_sm.SupplierName,
                                          BaseOfInventory = u.BaseOfInventory,
                                          eVMIWaitCommand = u.eVMIWaitCommand,
                                          eVMIWaitPort = u.eVMIWaitPort,
                                          CountAutoSequence = u.CountAutoSequence,
                                          ShelfLifeleadtimeOrdRpt = u.ShelfLifeleadtimeOrdRpt,
                                          LeadTimeOrdRpt = u.LeadTimeOrdRpt,
                                          PullPurchaseNumberType = u.PullPurchaseNumberType,
                                          LastPullPurchaseNumberUsed = u.LastPullPurchaseNumberUsed,
                                          IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                                          ExtPhoneNo = u.ExtPhoneNo,
                                          ReqAutoSequence = u.ReqAutoSequence,
                                          LastReceivedDate = u.LastReceivedDate,
                                          LastTrasnferedDate = u.LastTrasnferedDate,
                                          AllowInsertingItemOnScan = u.AllowInsertingItemOnScan ?? false,
                                          IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                          IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                          AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                          PullRejectionType = u.PullRejectionType,
                                          CountAutoNrFixedValue = u.CountAutoNrFixedValue,
                                          POAutoNrFixedValue = u.POAutoNrFixedValue,
                                          PullPurchaseNrFixedValue = u.PullPurchaseNrFixedValue,
                                          ReqAutoNrFixedValue = u.ReqAutoNrFixedValue,
                                          BillingRoomType = u.BillingRoomType,
                                          WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                          TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                          StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                          WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                          TransferAutoSequence = u.TransferAutoSequence,
                                          StagingAutoSequence = u.StagingAutoSequence,
                                          WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                          MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                          DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                          AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                          PreventMaxOrderQty = u.PreventMaxOrderQty,
                                          DefaultCountType = u.DefaultCountType,
                                          TAOAutoSequence = u.TAOAutoSequence,
                                          TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                          NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                          AllowToolOrdering = u.AllowToolOrdering,
                                          IsWOSignatureRequired = u.IsWOSignatureRequired,
                                          IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                          IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                          ToolCountAutoSequence = u.ToolCountAutoSequence,
                                          NextToolCountNo = u.NextToolCountNo,
                                          SuggestedReturn = u.SuggestedReturn,
                                          ForceSupplierFilter = u.ForceSupplierFilter
                                      }).FirstOrDefault();


                if (objRoomDTO != null && arrValidModules != null)
                {
                    objRoomDTO.lstRoomModleSettings = (from M in context.ModuleMasters
                                                       join rms in context.RoomModuleSettings
                                                       on new { ModuleId = (long?)M.ID, CompanyId = objRoomDTO.CompanyID, RoomId = (long?)objRoomDTO.ID }
                                                       equals new { ModuleId = rms.ModuleId, CompanyId = rms.CompanyId, RoomId = rms.RoomId } into rms_join
                                                       from RMS in rms_join.DefaultIfEmpty()
                                                       where arrValidModules.Contains(M.ID)
                                                       select new RoomModuleSettingsDTO
                                                       {
                                                           ID = (RMS == null ? 0 : RMS.ID),
                                                           CompanyId = objRoomDTO.CompanyID,
                                                           RoomId = objRoomDTO.ID,
                                                           ModuleId = M.ID,
                                                           ModuleName = M.ModuleName,
                                                           PriseSelectionOption = (RMS == null ? 1 : RMS.PriseSelectionOption),
                                                           CreatedBy = (RMS == null ? 0 : RMS.CreatedBy),
                                                           CreatedDate = (RMS == null ? null : RMS.CreatedDate),
                                                           LastUpdatedBy = (RMS == null ? 0 : RMS.LastUpdatedBy),
                                                           LastUpdatedDate = (RMS == null ? null : RMS.LastUpdatedDate)
                                                       }
                                                      ).ToList();
                }

                return objRoomDTO;
            }
        }


        public IEnumerable<RoomDTO> GetAllRecords(long CompanyId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyId.ToString())
                                            select new RoomDTO
                                            {
                                                ID = u.ID,
                                                RoomName = u.RoomName,
                                                CompanyName = u.CompanyName,
                                                ContactName = u.ContactName,
                                                streetaddress = u.streetaddress,
                                                City = u.City,
                                                State = u.State,
                                                PostalCode = u.PostalCode,
                                                Country = u.Country,
                                                Email = u.Email,
                                                PhoneNo = u.PhoneNo,
                                                InvoiceBranch = u.InvoiceBranch,
                                                CustomerNumber = u.CustomerNumber,
                                                BlanketPO = u.BlanketPO,
                                                IsConsignment = u.IsConsignment,
                                                IsTrending = u.IsTrending,
                                                SourceOfTrending = u.SourceOfTrending,
                                                TrendingFormula = u.TrendingFormula,
                                                TrendingFormulaType = u.TrendingFormulaType,
                                                TrendingFormulaDays = u.TrendingFormulaDays,
                                                TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                                TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                                TrendingFormulaCounts = u.TrendingFormulaCounts,
                                                //   ManualMin = u.ManualMin,
                                                SuggestedOrder = u.SuggestedOrder,
                                                SuggestedTransfer = u.SuggestedTransfer,
                                                ReplineshmentRoom = u.ReplineshmentRoom,
                                                //IsItemReplenishment = u.IsItemReplenishment,
                                                //IsBinReplenishment = u.IsBinReplenishment,
                                                ReplenishmentType = u.ReplenishmentType,
                                                IseVMI = u.IseVMI,
                                                MaxOrderSize = u.MaxOrderSize,
                                                HighPO = u.HighPO,
                                                HighJob = u.HighJob,
                                                HighTransfer = u.HighTransfer,
                                                HighCount = u.HighCount,
                                                GlobMarkupParts = u.GlobMarkupParts,
                                                GlobMarkupLabor = u.GlobMarkupLabor,
                                                IsTax1Parts = u.IsTax1Parts,
                                                IsTax1Labor = u.IsTax1Labor,
                                                Tax1name = u.Tax1name,
                                                Tax1Rate = u.Tax1Rate,
                                                IsTax2Parts = u.IsTax2Parts,
                                                IsTax2Labor = u.IsTax2Labor,
                                                tax2name = u.tax2name,
                                                Tax2Rate = u.Tax2Rate,
                                                IsTax2onTax1 = u.IsTax2onTax1,
                                                GXPRConsJob = u.GXPRConsJob,
                                                CostCenter = u.CostCenter,
                                                UniqueID = u.UniqueID,
                                                Created = u.Created,
                                                Updated = u.Updated,
                                                CreatedBy = u.CreatedBy,
                                                LastUpdatedBy = u.LastUpdatedBy,
                                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                CreatedByName = u.CreatedByName,
                                                UpdatedByName = u.UpdatedByName,
                                                MethodOfValuingInventory = u.MethodOfValuingInventory,
                                                AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                                AutoCreateTransferTime = u.AutoCreateTransferTime,
                                                AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                                IsActive = u.IsActive,
                                                LicenseBilled = u.LicenseBilled,
                                                NextCountNo = u.NextCountNo,
                                                IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                                IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                                SlowMovingValue = u.SlowMovingValue,
                                                FastMovingValue = u.FastMovingValue,
                                                POAutoSequence = u.POAutoSequence,
                                                NextOrderNo = u.NextOrderNo,
                                                NextRequisitionNo = u.NextRequisitionNo,
                                                NextStagingNo = u.NextStagingNo,
                                                NextTransferNo = u.NextTransferNo,
                                                NextWorkOrderNo = u.NextWorkOrderNo,
                                                RoomGrouping = u.RoomGrouping,

                                                TransferFrequencyOption = u.TransferFrequencyOption,
                                                TransferFrequencyDays = u.TransferFrequencyDays,
                                                TransferFrequencyMonth = u.TransferFrequencyMonth,
                                                TransferFrequencyNumber = u.TransferFrequencyNumber,
                                                TransferFrequencyWeek = u.TransferFrequencyWeek,
                                                TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                                TrendingSampleSize = u.TrendingSampleSize,
                                                TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                                AverageUsageTransactions = u.AverageUsageTransactions,
                                                AverageUsageSampleSize = u.AverageUsageSampleSize,
                                                AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                                GUID = u.GUID,
                                                CompanyID = u.CompanyID,
                                                UDF1 = u.UDF1,
                                                UDF2 = u.UDF2,
                                                UDF3 = u.UDF3,
                                                UDF4 = u.UDF4,
                                                UDF5 = u.UDF5,
                                                DefaultSupplierID = u.DefaultSupplierID,
                                                NextAssetNo = u.NextAssetNo,
                                                NextBinNo = u.NextBinNo,
                                                NextKitNo = u.NextKitNo,
                                                NextItemNo = u.NextItemNo,
                                                NextProjectSpendNo = u.NextProjectSpendNo,
                                                NextToolNo = u.NextToolNo,
                                                DefaultBinID = u.DefaultBinID,
                                                IsRoomActive = u.IsRoomActive,
                                                RequestedXDays = u.RequestedXDays ?? 0,
                                                RequestedYDays = u.RequestedYDays ?? 0,
                                                InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                                BaseOfInventory = u.BaseOfInventory,
                                                eVMIWaitCommand = u.eVMIWaitCommand,
                                                eVMIWaitPort = u.eVMIWaitPort,
                                                IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                                LastReceivedDate = u.LastReceivedDate,
                                                LastTrasnferedDate = u.LastTrasnferedDate,
                                                ExtPhoneNo = u.ExtPhoneNo,
                                                ReqAutoSequence = u.ReqAutoSequence,
                                                AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                                IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                                IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                                AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                                PullRejectionType = u.PullRejectionType,
                                                BillingRoomType = u.BillingRoomType,

                                                StagingAutoSequence = u.StagingAutoSequence,
                                                TransferAutoSequence = u.TransferAutoSequence,
                                                WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                                StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                                TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                                WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                                WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                                DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                                AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                                PreventMaxOrderQty = u.PreventMaxOrderQty,
                                                DefaultCountType = u.DefaultCountType,
                                                TAOAutoSequence = u.TAOAutoSequence,
                                                TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                                NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                                AllowToolOrdering = u.AllowToolOrdering,
                                                IsWOSignatureRequired = u.IsWOSignatureRequired,
                                                IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                                IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                ToolCountAutoSequence = u.ToolCountAutoSequence,
                                                NextToolCountNo = u.NextToolCountNo,

                                                ForceSupplierFilter = u.ForceSupplierFilter
                                            }).AsParallel().ToList();

                return obj.ToList();
            }
        }

        public RoomDTO GetRecord(Int64 id, Int64 CompanyID, string DBConnectionstring)
        {

            using (var context = new eTurnsEntities(DBConnectionstring))
            {
                return (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 AND A.CompanyID = " + CompanyID.ToString())
                        select new RoomDTO
                        {
                            ID = u.ID,
                            RoomName = u.RoomName,
                            CompanyName = u.CompanyName,
                            ContactName = u.ContactName,
                            streetaddress = u.streetaddress,
                            City = u.City,
                            State = u.State,
                            PostalCode = u.PostalCode,
                            Country = u.Country,
                            Email = u.Email,
                            PhoneNo = u.PhoneNo,
                            InvoiceBranch = u.InvoiceBranch,
                            CustomerNumber = u.CustomerNumber,
                            BlanketPO = u.BlanketPO,
                            IsConsignment = u.IsConsignment,
                            IsTrending = u.IsTrending,
                            SourceOfTrending = u.SourceOfTrending,
                            TrendingFormula = u.TrendingFormula,
                            TrendingFormulaType = u.TrendingFormulaType,
                            TrendingFormulaDays = u.TrendingFormulaDays,
                            TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                            TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                            TrendingFormulaCounts = u.TrendingFormulaCounts,
                            //   ManualMin = u.ManualMin,
                            SuggestedOrder = u.SuggestedOrder,
                            SuggestedTransfer = u.SuggestedTransfer,
                            ReplineshmentRoom = u.ReplineshmentRoom,
                            //IsItemReplenishment = u.IsItemReplenishment,
                            //IsBinReplenishment = u.IsBinReplenishment,
                            ReplenishmentType = u.ReplenishmentType,
                            IseVMI = u.IseVMI,
                            MaxOrderSize = u.MaxOrderSize,
                            HighPO = u.HighPO,
                            HighJob = u.HighJob,
                            HighTransfer = u.HighTransfer,
                            HighCount = u.HighCount,
                            GlobMarkupParts = u.GlobMarkupParts,
                            GlobMarkupLabor = u.GlobMarkupLabor,
                            IsTax1Parts = u.IsTax1Parts,
                            IsTax1Labor = u.IsTax1Labor,
                            Tax1name = u.Tax1name,
                            Tax1Rate = u.Tax1Rate,
                            IsTax2Parts = u.IsTax2Parts,
                            IsTax2Labor = u.IsTax2Labor,
                            tax2name = u.tax2name,
                            Tax2Rate = u.Tax2Rate,
                            IsTax2onTax1 = u.IsTax2onTax1,
                            GXPRConsJob = u.GXPRConsJob,
                            CostCenter = u.CostCenter,
                            UniqueID = u.UniqueID,
                            Created = u.Created,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                            IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                            CreatedByName = u.CreatedByName,
                            UpdatedByName = u.UpdatedByName,
                            MethodOfValuingInventory = u.MethodOfValuingInventory,
                            AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                            AutoCreateTransferTime = u.AutoCreateTransferTime,
                            AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                            IsActive = u.IsActive,
                            LicenseBilled = u.LicenseBilled,
                            NextCountNo = u.NextCountNo,
                            IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                            IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                            SlowMovingValue = u.SlowMovingValue,
                            FastMovingValue = u.FastMovingValue,
                            POAutoSequence = u.POAutoSequence,
                            NextOrderNo = u.NextOrderNo,
                            NextRequisitionNo = u.NextRequisitionNo,
                            NextStagingNo = u.NextStagingNo,
                            NextTransferNo = u.NextTransferNo,
                            NextWorkOrderNo = u.NextWorkOrderNo,
                            RoomGrouping = u.RoomGrouping,

                            TransferFrequencyOption = u.TransferFrequencyOption,
                            TransferFrequencyDays = u.TransferFrequencyDays,
                            TransferFrequencyMonth = u.TransferFrequencyMonth,
                            TransferFrequencyNumber = u.TransferFrequencyNumber,
                            TransferFrequencyWeek = u.TransferFrequencyWeek,
                            TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                            TrendingSampleSize = u.TrendingSampleSize,
                            TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                            AverageUsageTransactions = u.AverageUsageTransactions,
                            AverageUsageSampleSize = u.AverageUsageSampleSize,
                            AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                            GUID = u.GUID,
                            CompanyID = u.CompanyID,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            DefaultSupplierID = u.DefaultSupplierID,
                            NextAssetNo = u.NextAssetNo,
                            NextBinNo = u.NextBinNo,
                            NextKitNo = u.NextKitNo,
                            NextItemNo = u.NextItemNo,
                            NextProjectSpendNo = u.NextProjectSpendNo,
                            NextToolNo = u.NextToolNo,
                            DefaultBinID = u.DefaultBinID,
                            IsRoomActive = u.IsRoomActive,
                            RequestedXDays = u.RequestedXDays ?? 0,
                            RequestedYDays = u.RequestedYDays ?? 0,
                            InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                            BaseOfInventory = u.BaseOfInventory,
                            eVMIWaitCommand = u.eVMIWaitCommand,
                            eVMIWaitPort = u.eVMIWaitPort,
                            IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                            ExtPhoneNo = u.ExtPhoneNo,
                            ReqAutoSequence = u.ReqAutoSequence,
                            AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                            IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                            IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                            AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                            PullRejectionType = u.PullRejectionType,
                            BillingRoomType = u.BillingRoomType,

                            StagingAutoSequence = u.StagingAutoSequence,
                            TransferAutoSequence = u.TransferAutoSequence,
                            WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                            WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                            AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                            PreventMaxOrderQty = u.PreventMaxOrderQty,
                            DefaultCountType = u.DefaultCountType,
                            TAOAutoSequence = u.TAOAutoSequence,
                            TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                            NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                            AllowToolOrdering = u.AllowToolOrdering,
                            IsWOSignatureRequired = u.IsWOSignatureRequired,
                            IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                            IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                            ToolCountAutoSequence = u.ToolCountAutoSequence,
                            NextToolCountNo = u.NextToolCountNo,

                            ForceSupplierFilter = u.ForceSupplierFilter
                        }).FirstOrDefault();

            }



        }
        public IEnumerable<RoomDTO> GetAllRoomData(bool IsArchived, bool IsDeleted)
        {


            IEnumerable<RoomDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1")
                       select new RoomDTO
                       {
                           ID = u.ID,
                           RoomName = u.RoomName,
                           CompanyName = u.CompanyName,
                           ContactName = u.ContactName,
                           streetaddress = u.streetaddress,
                           City = u.City,
                           State = u.State,
                           PostalCode = u.PostalCode,
                           Country = u.Country,
                           Email = u.Email,
                           PhoneNo = u.PhoneNo,
                           InvoiceBranch = u.InvoiceBranch,
                           CustomerNumber = u.CustomerNumber,
                           BlanketPO = u.BlanketPO,
                           IsConsignment = u.IsConsignment,
                           IsTrending = u.IsTrending,
                           SourceOfTrending = u.SourceOfTrending,
                           TrendingFormula = u.TrendingFormula,
                           TrendingFormulaType = u.TrendingFormulaType,
                           TrendingFormulaDays = u.TrendingFormulaDays,
                           TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                           TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                           TrendingFormulaCounts = u.TrendingFormulaCounts,
                           SuggestedOrder = u.SuggestedOrder,
                           SuggestedTransfer = u.SuggestedTransfer,
                           ReplineshmentRoom = u.ReplineshmentRoom,
                           ReplenishmentType = u.ReplenishmentType,
                           IseVMI = u.IseVMI,
                           MaxOrderSize = u.MaxOrderSize,
                           HighPO = u.HighPO,
                           HighJob = u.HighJob,
                           HighTransfer = u.HighTransfer,
                           HighCount = u.HighCount,
                           GlobMarkupParts = u.GlobMarkupParts,
                           GlobMarkupLabor = u.GlobMarkupLabor,
                           IsTax1Parts = u.IsTax1Parts,
                           IsTax1Labor = u.IsTax1Labor,
                           Tax1name = u.Tax1name,
                           Tax1Rate = u.Tax1Rate,
                           IsTax2Parts = u.IsTax2Parts,
                           IsTax2Labor = u.IsTax2Labor,
                           tax2name = u.tax2name,
                           Tax2Rate = u.Tax2Rate,
                           IsTax2onTax1 = u.IsTax2onTax1,
                           GXPRConsJob = u.GXPRConsJob,
                           CostCenter = u.CostCenter,
                           UniqueID = u.UniqueID,
                           Created = u.Created,
                           Updated = u.Updated,
                           CreatedBy = u.CreatedBy,
                           LastUpdatedBy = u.LastUpdatedBy,
                           IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                           IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                           CreatedByName = u.CreatedByName,
                           UpdatedByName = u.UpdatedByName,
                           MethodOfValuingInventory = u.MethodOfValuingInventory,
                           AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                           AutoCreateTransferTime = u.AutoCreateTransferTime,
                           AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                           IsActive = u.IsActive,
                           LicenseBilled = u.LicenseBilled,
                           NextCountNo = u.NextCountNo,
                           IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                           IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                           SlowMovingValue = u.SlowMovingValue,
                           FastMovingValue = u.FastMovingValue,
                           POAutoSequence = u.POAutoSequence,
                           NextOrderNo = u.NextOrderNo,
                           NextRequisitionNo = u.NextRequisitionNo,
                           NextStagingNo = u.NextStagingNo,
                           NextTransferNo = u.NextTransferNo,
                           NextWorkOrderNo = u.NextWorkOrderNo,
                           RoomGrouping = u.RoomGrouping,
                           TransferFrequencyOption = u.TransferFrequencyOption,
                           TransferFrequencyDays = u.TransferFrequencyDays,
                           TransferFrequencyMonth = u.TransferFrequencyMonth,
                           TransferFrequencyNumber = u.TransferFrequencyNumber,
                           TransferFrequencyWeek = u.TransferFrequencyWeek,
                           TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                           TrendingSampleSize = u.TrendingSampleSize,
                           TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,
                           AverageUsageTransactions = u.AverageUsageTransactions,
                           AverageUsageSampleSize = u.AverageUsageSampleSize,
                           AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                           GUID = u.GUID,
                           CompanyID = u.CompanyID,
                           UDF1 = u.UDF1,
                           UDF2 = u.UDF2,
                           UDF3 = u.UDF3,
                           UDF4 = u.UDF4,
                           UDF5 = u.UDF5,
                           DefaultSupplierID = u.DefaultSupplierID,
                           NextAssetNo = u.NextAssetNo,
                           NextBinNo = u.NextBinNo,
                           NextKitNo = u.NextKitNo,
                           NextItemNo = u.NextItemNo,
                           NextProjectSpendNo = u.NextProjectSpendNo,
                           NextToolNo = u.NextToolNo,
                           DefaultBinID = u.DefaultBinID,
                           IsRoomActive = u.IsRoomActive,
                           RequestedXDays = u.RequestedXDays ?? 0,
                           RequestedYDays = u.RequestedYDays ?? 0,
                           InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                           DefaultBinName = u.DefaultBinName,
                           BaseOfInventory = u.BaseOfInventory,
                           eVMIWaitCommand = u.eVMIWaitCommand,
                           eVMIWaitPort = u.eVMIWaitPort,
                           IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                           LastReceivedDate = u.LastReceivedDate,
                           LastTrasnferedDate = u.LastTrasnferedDate,
                           ExtPhoneNo = u.ExtPhoneNo,
                           ReqAutoSequence = u.ReqAutoSequence,
                           AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                           IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                           IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                           AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                           PullRejectionType = u.PullRejectionType,
                           BillingRoomType = u.BillingRoomType,

                           StagingAutoSequence = u.StagingAutoSequence,
                           TransferAutoSequence = u.TransferAutoSequence,
                           WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                           WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                           MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                           DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                           AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                           PreventMaxOrderQty = u.PreventMaxOrderQty,
                           DefaultCountType = u.DefaultCountType,
                           TAOAutoSequence = u.TAOAutoSequence,
                           TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                           NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                           AllowToolOrdering = u.AllowToolOrdering,
                           IsWOSignatureRequired = u.IsWOSignatureRequired,
                           IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                           IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                           ToolCountAutoSequence = u.ToolCountAutoSequence,
                           NextToolCountNo = u.NextToolCountNo,

                           ForceSupplierFilter = u.ForceSupplierFilter
                       }).AsParallel().ToList();

            }



            return obj;
        }


        public RoomDTO GetRoomByCompanyIDAndRoomID(Int64 CompanyId, long RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return (from u in context.Rooms
                        join cm in context.CompanyMasters on u.CompanyID equals cm.ID into rm_cm_join
                        from rm_cm in rm_cm_join.DefaultIfEmpty()
                        where rm_cm.ID == CompanyId && u.ID == RoomID
                        && u.IsDeleted == false && u.IsArchived == false
                        select new RoomDTO
                        {
                            ID = u.ID,
                            RoomName = u.RoomName,
                            CompanyName = rm_cm.Name,
                            ContactName = u.ContactName,
                            streetaddress = u.StreetAddress,
                            City = u.City,
                            State = u.State,
                            PostalCode = u.PostalCode,
                            Country = u.Country,
                            Email = u.Email,
                            PhoneNo = u.PhoneNo,
                            InvoiceBranch = u.InvoiceBranch,
                            CustomerNumber = u.CustomerNumber,
                            BlanketPO = u.BlanketPO,
                            IsConsignment = u.IsConsignment,
                            IsTrending = u.IsTrending,
                            SourceOfTrending = u.SourceOfTrending,
                            TrendingFormula = u.TrendingFormula,
                            TrendingFormulaType = u.TrendingFormulaType,
                            TrendingFormulaDays = u.TrendingFormulaDays,
                            TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                            TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                            TrendingFormulaCounts = u.TrendingFormulaCounts,
                            //  ManualMin = u.ManualMin,
                            SuggestedOrder = u.SuggestedOrder,
                            SuggestedTransfer = u.SuggestedTransfer,
                            ReplineshmentRoom = u.ReplineshmentRoom,
                            //IsItemReplenishment = u.IsItemReplenishment,
                            //IsBinReplenishment = u.IsBinReplenishment ?? false,
                            ReplenishmentType = u.ReplenishmentType,
                            IseVMI = u.IseVMI,
                            MaxOrderSize = u.MaxOrderSize,
                            HighPO = u.HighPO,
                            HighJob = u.HighJob,
                            HighTransfer = u.HighTransfer,
                            HighCount = u.HighCount,
                            GlobMarkupParts = u.GlobMarkupParts,
                            GlobMarkupLabor = u.GlobMarkupLabor,
                            IsTax1Parts = u.IsTax1Parts,
                            IsTax1Labor = u.IsTax1Labor,
                            Tax1name = u.Tax1Name,
                            Tax1Rate = u.Tax1Rate,
                            IsTax2Parts = u.IsTax2Parts,
                            IsTax2Labor = u.IsTax2Labor,
                            tax2name = u.Tax2Name,
                            Tax2Rate = u.Tax2Rate,

                            GXPRConsJob = u.GXPRConsJob,
                            CostCenter = u.CostCenter,
                            UniqueID = u.UniqueID,
                            Created = u.Created ?? DateTime.MinValue,
                            Updated = u.Updated,
                            CreatedBy = u.CreatedBy,
                            LastUpdatedBy = u.LastUpdatedBy,
                            IsDeleted = u.IsDeleted,
                            IsArchived = u.IsArchived,
                            MethodOfValuingInventory = u.MethodOfValuingInventory,
                            AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                            AutoCreateTransferTime = u.AutoCreateTransferTime,
                            AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                            IsActive = u.IsActive,
                            LicenseBilled = u.LicenseBilled,
                            NextCountNo = u.NextCountNo,
                            POAutoSequence = u.POAutoSequence,
                            IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                            IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                            SlowMovingValue = u.SlowMovingValue,
                            FastMovingValue = u.FastMovingValue,
                            NextOrderNo = u.NextOrderNo,
                            NextRequisitionNo = u.NextRequisitionNo,
                            NextStagingNo = u.NextStagingNo,
                            NextTransferNo = u.NextTransferNo,
                            NextWorkOrderNo = u.NextWorkOrderNo,
                            RoomGrouping = u.RoomGrouping,
                            TransferFrequencyOption = u.TransferFrequencyOption,
                            TransferFrequencyDays = u.TransferFrequencyDays,
                            TransferFrequencyMonth = u.TransferFrequencyMonth,
                            TransferFrequencyNumber = u.TransferFrequencyNumber,
                            TransferFrequencyWeek = u.TransferFrequencyWeek,
                            TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                            TrendingSampleSize = u.TrendingSampleSize,
                            TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                            AverageUsageTransactions = u.AverageUsageTransactions,
                            AverageUsageSampleSize = u.AverageUsageSampleSize,
                            AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                            UDF1 = u.UDF1,
                            UDF2 = u.UDF2,
                            UDF3 = u.UDF3,
                            UDF4 = u.UDF4,
                            UDF5 = u.UDF5,
                            NextAssetNo = u.NextAssetNo,
                            NextBinNo = u.NextBinNo,
                            NextKitNo = u.NextKitNo,
                            NextItemNo = u.NextItemNo,
                            NextProjectSpendNo = u.NextProjectSpendNo,
                            NextToolNo = u.NextToolNo,
                            InventoryConsuptionMethod = u.InventoryConsuptionMethod,
                            DefaultBinID = u.DefaultBinID,
                            DefaultSupplierID = u.DefaultSupplierID,
                            IsRoomActive = u.IsRoomActive,
                            RequestedXDays = u.RequestedXDays ?? 0,
                            RequestedYDays = u.RequestedYDays ?? 0,
                            IsCompanyActive = rm_cm.IsActive,
                            BaseOfInventory = u.BaseOfInventory,
                            eVMIWaitCommand = u.eVMIWaitCommand,
                            eVMIWaitPort = u.eVMIWaitPort,
                            IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                            IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate ?? true,
                            IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                            ExtPhoneNo = u.ExtPhoneNo,
                            LastReceivedDate = u.LastReceivedDate,
                            LastTrasnferedDate = u.LastTrasnferedDate,
                            ReqAutoSequence = u.ReqAutoSequence,
                            CompanyID = u.CompanyID,
                            AllowInsertingItemOnScan = u.AllowInsertingItemOnScan ?? false,
                            DefaultBinName = u.DefaultBinName,
                            AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                            PullRejectionType = u.PullRejectionType,
                            BillingRoomType = u.BillingRoomType,

                            StagingAutoSequence = u.StagingAutoSequence,
                            TransferAutoSequence = u.TransferAutoSequence,
                            WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                            WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                            MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                            DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                            AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                            PreventMaxOrderQty = u.PreventMaxOrderQty,
                            DefaultCountType = u.DefaultCountType,
                            TAOAutoSequence = u.TAOAutoSequence,
                            TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                            NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                            AllowToolOrdering = u.AllowToolOrdering,
                            IsWOSignatureRequired = u.IsWOSignatureRequired,
                            IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                            IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                            ToolCountAutoSequence = u.ToolCountAutoSequence,
                            NextToolCountNo = u.NextToolCountNo,

                            ForceSupplierFilter = u.ForceSupplierFilter
                        }).FirstOrDefault();
            }
        }


        public IEnumerable<RoomDTO> GetAllRecords()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 ")
                                            select new RoomDTO
                                            {
                                                ID = u.ID,
                                                RoomName = u.RoomName,
                                                CompanyName = u.CompanyName,
                                                ContactName = u.ContactName,
                                                streetaddress = u.streetaddress,
                                                City = u.City,
                                                State = u.State,
                                                PostalCode = u.PostalCode,
                                                Country = u.Country,
                                                Email = u.Email,
                                                PhoneNo = u.PhoneNo,
                                                InvoiceBranch = u.InvoiceBranch,
                                                CustomerNumber = u.CustomerNumber,
                                                BlanketPO = u.BlanketPO,
                                                IsConsignment = u.IsConsignment,
                                                IsTrending = u.IsTrending,
                                                SourceOfTrending = u.SourceOfTrending,
                                                TrendingFormula = u.TrendingFormula,
                                                TrendingFormulaType = u.TrendingFormulaType,
                                                TrendingFormulaDays = u.TrendingFormulaDays,
                                                TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                                TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                                TrendingFormulaCounts = u.TrendingFormulaCounts,
                                                //   ManualMin = u.ManualMin,
                                                SuggestedOrder = u.SuggestedOrder,
                                                SuggestedTransfer = u.SuggestedTransfer,
                                                ReplineshmentRoom = u.ReplineshmentRoom,
                                                //IsItemReplenishment = u.IsItemReplenishment,
                                                //IsBinReplenishment = u.IsBinReplenishment,
                                                ReplenishmentType = u.ReplenishmentType,
                                                IseVMI = u.IseVMI,
                                                MaxOrderSize = u.MaxOrderSize,
                                                HighPO = u.HighPO,
                                                HighJob = u.HighJob,
                                                HighTransfer = u.HighTransfer,
                                                HighCount = u.HighCount,
                                                GlobMarkupParts = u.GlobMarkupParts,
                                                GlobMarkupLabor = u.GlobMarkupLabor,
                                                IsTax1Parts = u.IsTax1Parts,
                                                IsTax1Labor = u.IsTax1Labor,
                                                Tax1name = u.Tax1name,
                                                Tax1Rate = u.Tax1Rate,
                                                IsTax2Parts = u.IsTax2Parts,
                                                IsTax2Labor = u.IsTax2Labor,
                                                tax2name = u.tax2name,
                                                Tax2Rate = u.Tax2Rate,
                                                IsTax2onTax1 = u.IsTax2onTax1,
                                                GXPRConsJob = u.GXPRConsJob,
                                                CostCenter = u.CostCenter,
                                                UniqueID = u.UniqueID,
                                                Created = u.Created,
                                                Updated = u.Updated,
                                                CreatedBy = u.CreatedBy,
                                                LastUpdatedBy = u.LastUpdatedBy,
                                                IsDeleted = (u.IsDeleted.HasValue ? u.IsDeleted : false),
                                                IsArchived = (u.IsArchived.HasValue ? u.IsArchived : false),
                                                CreatedByName = u.CreatedByName,
                                                UpdatedByName = u.UpdatedByName,
                                                MethodOfValuingInventory = u.MethodOfValuingInventory,
                                                AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                                AutoCreateTransferTime = u.AutoCreateTransferTime,
                                                AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                                IsActive = u.IsActive,
                                                LicenseBilled = u.LicenseBilled,
                                                NextCountNo = u.NextCountNo,
                                                IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                                IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                                SlowMovingValue = u.SlowMovingValue,
                                                FastMovingValue = u.FastMovingValue,
                                                POAutoSequence = u.POAutoSequence,
                                                NextOrderNo = u.NextOrderNo,
                                                NextRequisitionNo = u.NextRequisitionNo,
                                                NextStagingNo = u.NextStagingNo,
                                                NextTransferNo = u.NextTransferNo,
                                                NextWorkOrderNo = u.NextWorkOrderNo,
                                                RoomGrouping = u.RoomGrouping,

                                                TransferFrequencyOption = u.TransferFrequencyOption,
                                                TransferFrequencyDays = u.TransferFrequencyDays,
                                                TransferFrequencyMonth = u.TransferFrequencyMonth,
                                                TransferFrequencyNumber = u.TransferFrequencyNumber,
                                                TransferFrequencyWeek = u.TransferFrequencyWeek,
                                                TransferFrequencyMainOption = u.TransferFrequencyMainOption,

                                                TrendingSampleSize = u.TrendingSampleSize,
                                                TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                                AverageUsageTransactions = u.AverageUsageTransactions,
                                                AverageUsageSampleSize = u.AverageUsageSampleSize,
                                                AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                                GUID = u.GUID,
                                                CompanyID = u.CompanyID,
                                                UDF1 = u.UDF1,
                                                UDF2 = u.UDF2,
                                                UDF3 = u.UDF3,
                                                UDF4 = u.UDF4,
                                                UDF5 = u.UDF5,
                                                DefaultSupplierID = u.DefaultSupplierID,
                                                NextAssetNo = u.NextAssetNo,
                                                NextBinNo = u.NextBinNo,
                                                NextKitNo = u.NextKitNo,
                                                NextItemNo = u.NextItemNo,
                                                NextProjectSpendNo = u.NextProjectSpendNo,
                                                NextToolNo = u.NextToolNo,
                                                DefaultBinID = u.DefaultBinID,
                                                IsRoomActive = u.IsRoomActive,
                                                RequestedXDays = u.RequestedXDays ?? 0,
                                                RequestedYDays = u.RequestedYDays ?? 0,
                                                InventoryConsuptionMethod = u.InventoryConsuptionMethod == null ? "" : u.InventoryConsuptionMethod,
                                                BaseOfInventory = u.BaseOfInventory,
                                                eVMIWaitCommand = u.eVMIWaitCommand,
                                                eVMIWaitPort = u.eVMIWaitPort,
                                                IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                                ExtPhoneNo = u.ExtPhoneNo,
                                                ReqAutoSequence = u.ReqAutoSequence,
                                                LastReceivedDate = u.LastReceivedDate,
                                                LastTrasnferedDate = u.LastTrasnferedDate,
                                                AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                                IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                                IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                                AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                                PullRejectionType = u.PullRejectionType,
                                                BillingRoomType = u.BillingRoomType,
                                                WorkOrderAutoNrFixedValue = u.WorkOrderAutoNrFixedValue,
                                                TransferAutoNrFixedValue = u.TransferAutoNrFixedValue,
                                                StagingAutoNrFixedValue = u.StagingAutoNrFixedValue,
                                                WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                                TransferAutoSequence = u.TransferAutoSequence,
                                                StagingAutoSequence = u.StagingAutoSequence,
                                                WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                                MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                                DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                                AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                                PreventMaxOrderQty = u.PreventMaxOrderQty,
                                                DefaultCountType = u.DefaultCountType,
                                                TAOAutoSequence = u.TAOAutoSequence,
                                                TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                                NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                                AllowToolOrdering = u.AllowToolOrdering,
                                                IsWOSignatureRequired = u.IsWOSignatureRequired,
                                                IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                                IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                                                ToolCountAutoSequence = u.ToolCountAutoSequence,
                                                NextToolCountNo = u.NextToolCountNo,

                                                ForceSupplierFilter = u.ForceSupplierFilter
                                            }).AsParallel().ToList();

                return obj.ToList();
            }
        }

        public List<RoomDTO> GetRoomsByRoomIdsNormal(string RoomIds)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomIds", RoomIds) };
                return context.ExecuteStoreQuery<RoomDTO>("exec [GetRoomsByRoomIdsNormal] @RoomIds", params1).ToList();
            }
        }

        public List<RoomDTO> GetRoomByCompanyList(string CompanyIDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@CompanyIDs", CompanyIDs ?? (object)DBNull.Value) };

                var rooms = context.ExecuteStoreQuery<RoomDTO>("exec [GetRoomByCompanyList] @CompanyIDs", params1).ToList();

                List<RoomDTO> lstAllRooms = (from u in rooms
                                             select new RoomDTO
                                             {
                                                 ID = u.ID,
                                                 RoomName = u.RoomName,
                                                 CompanyName = u.CompanyName,
                                                 ContactName = u.ContactName,
                                                 streetaddress = u.streetaddress,
                                                 City = u.City,
                                                 State = u.State,
                                                 PostalCode = u.PostalCode,
                                                 Country = u.Country,
                                                 Email = u.Email,
                                                 PhoneNo = u.PhoneNo,
                                                 InvoiceBranch = u.InvoiceBranch,
                                                 CustomerNumber = u.CustomerNumber,
                                                 BlanketPO = u.BlanketPO,
                                                 IsConsignment = u.IsConsignment,
                                                 IsTrending = u.IsTrending,
                                                 SourceOfTrending = u.SourceOfTrending,
                                                 TrendingFormula = u.TrendingFormula,
                                                 TrendingFormulaType = u.TrendingFormulaType,
                                                 TrendingFormulaDays = u.TrendingFormulaDays,
                                                 TrendingFormulaOverDays = u.TrendingFormulaOverDays,
                                                 TrendingFormulaAvgDays = u.TrendingFormulaAvgDays,
                                                 TrendingFormulaCounts = u.TrendingFormulaCounts,
                                                 //  ManualMin = u.ManualMin,
                                                 SuggestedOrder = u.SuggestedOrder,
                                                 SuggestedTransfer = u.SuggestedTransfer,
                                                 ReplineshmentRoom = u.ReplineshmentRoom,
                                                 //IsItemReplenishment = u.IsItemReplenishment,
                                                 //IsBinReplenishment = u.IsBinReplenishment ?? false,
                                                 ReplenishmentType = u.ReplenishmentType,
                                                 IseVMI = u.IseVMI,
                                                 MaxOrderSize = u.MaxOrderSize,
                                                 HighPO = u.HighPO,
                                                 HighJob = u.HighJob,
                                                 HighTransfer = u.HighTransfer,
                                                 HighCount = u.HighCount,
                                                 GlobMarkupParts = u.GlobMarkupParts,
                                                 GlobMarkupLabor = u.GlobMarkupLabor,
                                                 IsTax1Parts = u.IsTax1Parts,
                                                 IsTax1Labor = u.IsTax1Labor,
                                                 Tax1name = u.Tax1name,
                                                 Tax1Rate = u.Tax1Rate,
                                                 IsTax2Parts = u.IsTax2Parts,
                                                 IsTax2Labor = u.IsTax2Labor,
                                                 tax2name = u.tax2name,
                                                 Tax2Rate = u.Tax2Rate,

                                                 GXPRConsJob = u.GXPRConsJob,
                                                 CostCenter = u.CostCenter,
                                                 UniqueID = u.UniqueID,
                                                 Created = u.Created,
                                                 Updated = u.Updated,
                                                 CreatedBy = u.CreatedBy,
                                                 LastUpdatedBy = u.LastUpdatedBy,
                                                 IsDeleted = u.IsDeleted,
                                                 IsArchived = u.IsArchived,
                                                 MethodOfValuingInventory = u.MethodOfValuingInventory,
                                                 AutoCreateTransferFrequency = u.AutoCreateTransferFrequency,
                                                 AutoCreateTransferTime = u.AutoCreateTransferTime,
                                                 AutoCreateTransferSubmit = u.AutoCreateTransferSubmit,
                                                 IsActive = u.IsActive,
                                                 LicenseBilled = u.LicenseBilled,
                                                 NextCountNo = u.NextCountNo,
                                                 POAutoSequence = u.POAutoSequence,
                                                 IsProjectSpendMandatory = u.IsProjectSpendMandatory,
                                                 IsAverageUsageBasedOnPull = u.IsAverageUsageBasedOnPull,
                                                 SlowMovingValue = u.SlowMovingValue,
                                                 FastMovingValue = u.FastMovingValue,
                                                 NextOrderNo = u.NextOrderNo,
                                                 NextRequisitionNo = u.NextRequisitionNo,
                                                 NextStagingNo = u.NextStagingNo,
                                                 NextTransferNo = u.NextTransferNo,
                                                 NextWorkOrderNo = u.NextWorkOrderNo,
                                                 RoomGrouping = u.RoomGrouping,
                                                 TransferFrequencyOption = u.TransferFrequencyOption,
                                                 TransferFrequencyDays = u.TransferFrequencyDays,
                                                 TransferFrequencyMonth = u.TransferFrequencyMonth,
                                                 TransferFrequencyNumber = u.TransferFrequencyNumber,
                                                 TransferFrequencyWeek = u.TransferFrequencyWeek,
                                                 TransferFrequencyMainOption = u.TransferFrequencyMainOption,
                                                 TrendingSampleSize = u.TrendingSampleSize,
                                                 TrendingSampleSizeDivisor = u.TrendingSampleSizeDivisor,

                                                 AverageUsageTransactions = u.AverageUsageTransactions,
                                                 AverageUsageSampleSize = u.AverageUsageSampleSize,
                                                 AverageUsageSampleSizeDivisor = u.AverageUsageSampleSizeDivisor,
                                                 UDF1 = u.UDF1,
                                                 UDF2 = u.UDF2,
                                                 UDF3 = u.UDF3,
                                                 UDF4 = u.UDF4,
                                                 UDF5 = u.UDF5,
                                                 NextAssetNo = u.NextAssetNo,
                                                 NextBinNo = u.NextBinNo,
                                                 NextKitNo = u.NextKitNo,
                                                 NextItemNo = u.NextItemNo,
                                                 NextProjectSpendNo = u.NextProjectSpendNo,
                                                 NextToolNo = u.NextToolNo,
                                                 InventoryConsuptionMethod = u.InventoryConsuptionMethod,
                                                 DefaultBinID = u.DefaultBinID,
                                                 DefaultSupplierID = u.DefaultSupplierID,
                                                 IsRoomActive = u.IsRoomActive,
                                                 RequestedXDays = u.RequestedXDays ?? 0,
                                                 RequestedYDays = u.RequestedYDays ?? 0,
                                                 IsCompanyActive = u.IsCompanyActive,
                                                 BaseOfInventory = u.BaseOfInventory,
                                                 eVMIWaitCommand = u.eVMIWaitCommand,
                                                 eVMIWaitPort = u.eVMIWaitPort,
                                                 IsAllowOrderDuplicate = u.IsAllowOrderDuplicate,
                                                 IsAllowRequisitionDuplicate = u.IsAllowRequisitionDuplicate,
                                                 ExtPhoneNo = u.ExtPhoneNo,
                                                 LastReceivedDate = u.LastReceivedDate,
                                                 LastTrasnferedDate = u.LastTrasnferedDate,
                                                 ReqAutoSequence = u.ReqAutoSequence,
                                                 CompanyID = u.CompanyID,
                                                 AllowInsertingItemOnScan = u.AllowInsertingItemOnScan,
                                                 DefaultBinName = u.DefaultBinName,
                                                 AllowPullBeyondAvailableQty = u.AllowPullBeyondAvailableQty,
                                                 PullRejectionType = u.PullRejectionType,
                                                 BillingRoomType = u.BillingRoomType,

                                                 StagingAutoSequence = u.StagingAutoSequence,
                                                 TransferAutoSequence = u.TransferAutoSequence,
                                                 WorkOrderAutoSequence = u.WorkOrderAutoSequence,
                                                 WarnUserOnAssigningNonDefaultBin = u.WarnUserOnAssigningNonDefaultBin,
                                                 MaintenanceDueNoticeDays = u.MaintenanceDueNoticeDays,
                                                 DefaultRequisitionRequiredDays = u.DefaultRequisitionRequiredDays,
                                                 IsAllowWorkOrdersDuplicate = u.IsAllowWorkOrdersDuplicate,
                                                 AttachingWOWithRequisition = u.AttachingWOWithRequisition,
                                                 PreventMaxOrderQty = u.PreventMaxOrderQty,
                                                 DefaultCountType = u.DefaultCountType,
                                                 TAOAutoSequence = u.TAOAutoSequence,
                                                 TAOAutoNrFixedValue = u.TAOAutoNrFixedValue,
                                                 NextToolAssetOrderNo = u.NextToolAssetOrderNo,
                                                 AllowToolOrdering = u.AllowToolOrdering,
                                                 IsWOSignatureRequired = u.IsWOSignatureRequired,
                                                 IsIgnoreCreditRule = u.IsIgnoreCreditRule,
                                                 IsAllowOrderCostuom = u.IsAllowOrderCostuom,

                                                 ForceSupplierFilter = u.ForceSupplierFilter
                                             }).ToList();

                return lstAllRooms;
            }
        }



        public IEnumerable<RoomDTO> GetAllCompanyRoomNameByIDs(string IDs)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"
                                            SELECT A.ID,A.RoomName,C.Name AS CompanyName,A.CompanyID 
                                            FROM Room A LEFT OUTER  JOIN CompanyMaster C on A.CompanyID= C.ID 
                                            WHERE A.ID IN (SELECt SplitValue FROM dbo.split('" + IDs + @"', ','))")
                                            select new RoomDTO
                                            {
                                                ID = u.ID,
                                                RoomName = u.RoomName,
                                                CompanyName = u.CompanyName,
                                                CompanyID = u.CompanyID
                                            }).AsParallel().ToList();

                return obj.ToList();
            }
        }


        public IEnumerable<RoomDTO> GetAllRecordsIdName()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                IEnumerable<RoomDTO> obj = (from u in context.ExecuteStoreQuery<RoomDTO>(@"SELECT A.*, B.UserName AS 'CreatedByName', C.UserName AS UpdatedByName, D.RoomName FROM Room A LEFT OUTER  JOIN UserMaster B on A.CreatedBy = B.ID LEFT OUTER JOIN UserMaster C on A.LastUpdatedBy = C.ID LEFT OUTER JOIN Room D on A.Room = D.ID WHERE A.IsDeleted!=1 AND A.IsArchived != 1 ")
                                            select new RoomDTO
                                            {
                                                ID = u.ID,
                                                RoomName = u.RoomName,
                                                CompanyName = u.CompanyName,
                                                CompanyID = u.CompanyID
                                            }).AsParallel().ToList();

                return obj.ToList();
            }
        }


        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecords(string IDs, Int64 userid, Int64 CompanyId)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = "";
                foreach (var item in IDs.Split(','))
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        strQuery += "UPDATE Room SET Updated = '" + DateTime.Now.ToString() + "' , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1 WHERE ID =" + item.ToString() + ";";
                    }
                }
                context.ExecuteStoreCommand(strQuery);

                //Get Cached-Media
                IEnumerable<RoomDTO> ObjCache = CacheHelper<IEnumerable<RoomDTO>>.GetCacheItem("Cached_Room_" + CompanyId.ToString());
                if (ObjCache != null)
                {
                    List<RoomDTO> objTemp = ObjCache.ToList();
                    objTemp.RemoveAll(i => IDs.Split(',').Contains(i.ID.ToString()));
                    ObjCache = objTemp.AsEnumerable();
                    CacheHelper<IEnumerable<RoomDTO>>.AppendToCacheItem("Cached_Room_" + CompanyId.ToString(), ObjCache);
                }

                return true;
            }
        }
    }
}
