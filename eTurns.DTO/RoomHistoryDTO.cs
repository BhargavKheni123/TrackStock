using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace eTurns.DTO
{
    public class RoomHistoryDTO
    {
        [Key]
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResRoomMaster))]
        public string RoomName { get; set; }

        [Display(Name = "CompanyName", ResourceType = typeof(ResRoomMaster))]
        public string CompanyName { get; set; }

        [Display(Name = "ContactName", ResourceType = typeof(ResRoomMaster))]
        public string ContactName { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        public string streetaddress { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResCommon))]
        public string City { get; set; }

        [Display(Name = "State", ResourceType = typeof(ResCommon))]
        public string State { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResCommon))]
        public string PostalCode { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        public string Country { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        public string Email { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
        public string PhoneNo { get; set; }

        [Display(Name = "InvoiceBranch", ResourceType = typeof(ResRoomMaster))]
        public string InvoiceBranch { get; set; }

        [Display(Name = "CustomerNumber", ResourceType = typeof(ResRoomMaster))]
        public string CustomerNumber { get; set; }

        [Display(Name = "BillingRoomType", ResourceType = typeof(ResRoomMaster))]
        public int? BillingRoomType { get; set; }

        [Display(Name = "BlanketPO", ResourceType = typeof(ResRoomMaster))]
        public string BlanketPO { get; set; }

        [Display(Name = "IsConsignment", ResourceType = typeof(ResRoomMaster))]
        public bool IsConsignment { get; set; }

        [Display(Name = "SuggestedOrder", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedOrder { get; set; }

        [Display(Name = "SuggestedTransfer", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedTransfer { get; set; }

        [Display(Name = "ReplineshmentRoom", ResourceType = typeof(ResRoomMaster))]
        public int? ReplineshmentRoom { get; set; }

        [Display(Name = "ReplenishmentType", ResourceType = typeof(ResRoomMaster))]
        public String ReplenishmentType { get; set; }

        [Display(Name = "IseVMI", ResourceType = typeof(ResRoomMaster))]
        public bool IseVMI { get; set; }

        [Display(Name = "MaxOrderSize", ResourceType = typeof(ResRoomMaster))]
        public double? MaxOrderSize { get; set; }

        [Display(Name = "HighPO", ResourceType = typeof(ResRoomMaster))]
        public string HighPO { get; set; }

        [Display(Name = "HighJob", ResourceType = typeof(ResRoomMaster))]
        public string HighJob { get; set; }

        [Display(Name = "HighTransfer", ResourceType = typeof(ResRoomMaster))]
        public string HighTransfer { get; set; }

        [Display(Name = "HighCount", ResourceType = typeof(ResRoomMaster))]
        public string HighCount { get; set; }

        [Display(Name = "GlobalMarkupParts", ResourceType = typeof(ResRoomMaster))]
        public double? GlobMarkupParts { get; set; }

        [Display(Name = "GlobalMarkupLabor", ResourceType = typeof(ResRoomMaster))]
        public double? GlobMarkupLabor { get; set; }

        [Display(Name = "Tax1Parts", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax1Parts { get; set; }

        [Display(Name = "Tax1Labor", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax1Labor { get; set; }

        [Display(Name = "Tax1name", ResourceType = typeof(ResRoomMaster))]
        public string Tax1name { get; set; }

        [Display(Name = "Tax1percent", ResourceType = typeof(ResRoomMaster))]
        public double? Tax1Rate { get; set; }

        [Display(Name = "Tax2Parts", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax2Parts { get; set; }

        [Display(Name = "Tax2Labor", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax2Labor { get; set; }

        [Display(Name = "Tax2name", ResourceType = typeof(ResRoomMaster))]
        public string tax2name { get; set; }

        [Display(Name = "Tax2percent", ResourceType = typeof(ResRoomMaster))]
        public double? Tax2Rate { get; set; }

        [Display(Name = "Tax2onTax1", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax2onTax1 { get; set; }

        [Display(Name = "IsTrending", ResourceType = typeof(ResRoomMaster))]
        public bool IsTrending { get; set; }

        [Display(Name = "TrendingFormula", ResourceType = typeof(ResRoomMaster))]
        public string TrendingFormula { get; set; }

        [Display(Name = "TrendingFormulaType", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingFormulaType { get; set; }

        [Display(Name = "TrendingFormulaDays", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingFormulaDays { get; set; }

        [Display(Name = "TrendingFormulaOverDays", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingFormulaOverDays { get; set; }

        [Display(Name = "TrendingFormulaAvgDays", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingFormulaAvgDays { get; set; }

        [Display(Name = "TrendingFormulaCounts", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingFormulaCounts { get; set; }

        [Display(Name = "GXPRConsJob", ResourceType = typeof(ResRoomMaster))]
        public string GXPRConsJob { get; set; }

        [Display(Name = "CostCenter", ResourceType = typeof(ResRoomMaster))]
        public string CostCenter { get; set; }

        [Display(Name = "UniqueID", ResourceType = typeof(ResRoomMaster))]
        public string UniqueID { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "ActiveOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> ActiveOn { get; set; }
        public string ActiveOnDateStr { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "ValuingInventoryMethod", ResourceType = typeof(ResRoomMaster))]
        public string MethodOfValuingInventory { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResRoomMaster))]
        public bool IsActive { get; set; }

        [Display(Name = "LicenseBilled", ResourceType = typeof(ResRoomMaster))]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LicenseBilled { get; set; }

        [Display(Name = "LicenseBilled", ResourceType = typeof(ResRoomMaster))]
        public System.String LicenseBilledStri { get; set; }

        [Display(Name = "NextCountNo", ResourceType = typeof(ResRoomMaster))]
        public string NextCountNo { get; set; }

        [Display(Name = "POAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> POAutoSequence { get; set; }

        [Display(Name = "CountAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> CountAutoSequence { get; set; }

        [Display(Name = "NextOrderNo", ResourceType = typeof(ResRoomMaster))]
        public string NextOrderNo { get; set; }

        [Display(Name = "NextRequisitionNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextRequisitionNo { get; set; }

        [Display(Name = "NextStagingNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextStagingNo { get; set; }

        [Display(Name = "NextTransferNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextTransferNo { get; set; }

        [Display(Name = "NextWorkOrderNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextWorkOrderNo { get; set; }

        [Display(Name = "RoomGrouping", ResourceType = typeof(ResRoomMaster))]
        public string RoomGrouping { get; set; }

        [Display(Name = "TransferFrequency", ResourceType = typeof(ResRoomMaster))]
        public string AutoCreateTransferFrequency { get; set; }

        [Display(Name = "TransferTime", ResourceType = typeof(ResRoomMaster))]
        public string AutoCreateTransferTime { get; set; }

        [Display(Name = "TransferSubmit", ResourceType = typeof(ResRoomMaster))]
        public bool AutoCreateTransferSubmit { get; set; }

        [Display(Name = "TransferFrequencyOption", ResourceType = typeof(ResRoomMaster))]
        public int? TransferFrequencyOption { get; set; }

        [Display(Name = "TransferFrequencyDays", ResourceType = typeof(ResRoomMaster))]
        public string TransferFrequencyDays { get; set; }

        [Display(Name = "TransferFrequencyMonth", ResourceType = typeof(ResRoomMaster))]
        public int? TransferFrequencyMonth { get; set; }

        [Display(Name = "TransferFrequencyNumber", ResourceType = typeof(ResRoomMaster))]
        public int? TransferFrequencyNumber { get; set; }

        [Display(Name = "TransferFrequencyWeek", ResourceType = typeof(ResRoomMaster))]
        public int? TransferFrequencyWeek { get; set; }

        [Display(Name = "TransferFrequencyMainOption", ResourceType = typeof(ResRoomMaster))]
        public int? TransferFrequencyMainOption { get; set; }

        [Display(Name = "TrendingSampleSize", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingSampleSize { get; set; }

        [Display(Name = "TrendingSampleSizeDivisor", ResourceType = typeof(ResRoomMaster))]
        public int? TrendingSampleSizeDivisor { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "SourceOfTrending", ResourceType = typeof(ResRoomMaster))]
        public int? SourceOfTrending { get; set; }

        [Display(Name = "AverageUsageTransactions", ResourceType = typeof(ResRoomMaster))]
        public int? AverageUsageTransactions { get; set; }

        [Display(Name = "AverageUsageSampleSize", ResourceType = typeof(ResRoomMaster))]
        public int? AverageUsageSampleSize { get; set; }

        [Display(Name = "AverageUsageSampleSizeDivisor", ResourceType = typeof(ResRoomMaster))]
        public int? AverageUsageSampleSizeDivisor { get; set; }

        [Display(Name = "UDF1")]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2")]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3")]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4")]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5")]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6")]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7")]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8")]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9")]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10")]
        public string UDF10 { get; set; }
        [Display(Name = "DefaultSupplier", ResourceType = typeof(ResRoomMaster))]
        public Nullable<Int64> DefaultSupplierID { get; set; }
        public string DefaultSupplierName { get; set; }

        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }

        [Display(Name = "NextAssetNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextAssetNo { get; set; }

        [Display(Name = "NextBinNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextBinNo { get; set; }

        [Display(Name = "NextKitNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextKitNo { get; set; }

        [Display(Name = "NextItemNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextItemNo { get; set; }

        [Display(Name = "NextProjectSpendNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextProjectSpendNo { get; set; }

        [Display(Name = "NextToolNo", ResourceType = typeof(ResRoomMaster))]
        public Int64? NextToolNo { get; set; }

        [Display(Name = "InventoryConsuptionMethod", ResourceType = typeof(ResRoomMaster))]
        public string InventoryConsuptionMethod { get; set; }

        [Display(Name = "DefaultBinID", ResourceType = typeof(ResRoomMaster))]
        public Nullable<Int64> DefaultBinID { get; set; }

        [Display(Name = "DefaultBinID", ResourceType = typeof(ResRoomMaster))]
        public string DefaultBinName { get; set; }

        [Display(Name = "DefaultBinID", ResourceType = typeof(ResRoomMaster))]
        public string DefaultLocationName { get; set; }

        [Display(Name = "IsRoomActive", ResourceType = typeof(ResRoomMaster))]
        public bool IsRoomActive { get; set; }

        public bool IsRoomInactive { get; set; }
        public bool IsCompanyActive { get; set; }

        [Display(Name = "IsProjectSpendMandatory", ResourceType = typeof(ResRoomMaster))]
        public bool IsProjectSpendMandatory { get; set; }

        [Display(Name = "IsAverageUsageBasedOnPull", ResourceType = typeof(ResRoomMaster))]
        public bool IsAverageUsageBasedOnPull { get; set; }

        [Display(Name = "SlowMovingValue", ResourceType = typeof(ResRoomMaster))]
        public double SlowMovingValue { get; set; }

        [Display(Name = "FastMovingValue", ResourceType = typeof(ResRoomMaster))]
        public double FastMovingValue { get; set; }
        public string ReplineshmentRoomName { get; set; }
        public short RoomActiveStatus { get; set; }
        public short ReclassifyAllItems { get; set; }
        public int RoomNameChange { get; set; }
        public int SOSettingChanged { get; set; }
        public int STSettingChanged { get; set; }
        public int SRSettingChanged { get; set; }

        [Display(Name = "RequestedXDays", ResourceType = typeof(ResRoomMaster))]
        public Int32? RequestedXDays { get; set; }

        [Display(Name = "RequestedYDays", ResourceType = typeof(ResRoomMaster))]
        public Int32? RequestedYDays { get; set; }

        [Display(Name = "LastPullDate", ResourceType = typeof(ResRoomMaster))]
        public Nullable<DateTime> LastPullDate { get; set; }

        [Display(Name = "LastOrderDate", ResourceType = typeof(ResRoomMaster))]
        public Nullable<DateTime> LastOrderDate { get; set; }

        [Display(Name = "BaseOfInventory", ResourceType = typeof(ResRoomMaster))]
        public int? BaseOfInventory { get; set; }

        [Display(Name = "eVMIWaitCommand", ResourceType = typeof(ResRoomMaster))]
        public Int64 eVMIWaitCommand { get; set; }

        [Display(Name = "eVMIWaitPort", ResourceType = typeof(ResRoomMaster))]
        public Int64 eVMIWaitPort { get; set; }

        [Display(Name = "ShelfLifeleadtimeOrdRpt", ResourceType = typeof(ResRoomMaster))]
        public int? ShelfLifeleadtimeOrdRpt { get; set; }

        [Display(Name = "LeadTimeOrdRpt", ResourceType = typeof(ResRoomMaster))]
        public int? LeadTimeOrdRpt { get; set; }
        public int? MaintenanceDueNoticeDays { get; set; }

        public Nullable<System.Int32> NoOfItems { get; set; }
        public string ReportAppIntent { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate) && HttpContext.Current != null)
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true, true);
                }
                return _createdDate;
            }
            set { this._createdDate = value; }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedDate) && Updated != null && HttpContext.Current != null)
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }
        private string _ActiveOn;
        public string ActiveOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ActiveOn) && ActiveOn != null && HttpContext.Current != null)
                {
                    _ActiveOn = FnCommon.ConvertDateByTimeZone(ActiveOn, true, true);
                }
                return _ActiveOn;
            }
            set { this._ActiveOn = value; }
        }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "PullPurchaseNumberType", ResourceType = typeof(ResRoomMaster))]
        public int? PullPurchaseNumberType { get; set; }

        [Display(Name = "LastPullPurchaseNumberUsed", ResourceType = typeof(ResRoomMaster))]
        public string LastPullPurchaseNumberUsed { get; set; }

        [Display(Name = "RequisitionAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> ReqAutoSequence { get; set; }

        private string _LastOrderDateStr;
        public string LastOrderDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastOrderDateStr) && LastOrderDate != null && HttpContext.Current != null)
                {
                    _LastOrderDateStr = FnCommon.ConvertDateByTimeZone(LastOrderDate, true, true);
                }
                return _LastOrderDateStr;
            }
            set { this._LastOrderDateStr = value; }
        }
        private string _LicenseBilledStr;
        public string LicenseBilledStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LicenseBilledStr) && LicenseBilled != null && HttpContext.Current != null)
                {
                    _LicenseBilledStr = FnCommon.ConvertDateByTimeZone(LicenseBilled, true, true);
                }
                return _LicenseBilledStr;
            }
            set { this._LicenseBilledStr = value; }
        }
        private string _LastPullDateStr;
        public string LastPullDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastPullDateStr) && LastPullDate != null && HttpContext.Current != null)
                {
                    _LastPullDateStr = FnCommon.ConvertDateByTimeZone(LastPullDate, true, true);
                }
                return _LastPullDateStr;
            }
            set { this._LastPullDateStr = value; }
        }

        public DateTime? LastSyncDateTime { get; set; }
        public string PDABuildVersion { get; set; }
        public string LastSyncUserName { get; set; }

        [Display(Name = "IsAllowRequisitionDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowRequisitionDuplicate { get; set; }
        public Int64? RoomId { get; set; }
        public string ExtPhoneNo { get; set; }

        public Nullable<DateTime> LastReceivedDate { get; set; }
        public Nullable<DateTime> LastTrasnferedDate { get; set; }

        private string _LastTrasnferedDateStr;
        public string LastTrasnferedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastTrasnferedDateStr) && LastTrasnferedDate != null && HttpContext.Current != null)
                {
                    _LastTrasnferedDateStr = FnCommon.ConvertDateByTimeZone(LastTrasnferedDate, true, true);
                }
                return _LastTrasnferedDateStr;
            }
            set { this._LastTrasnferedDateStr = value; }
        }
        private string _LastReceivedDateStr;
        public string LastReceivedDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastReceivedDateStr) && LastReceivedDate != null && HttpContext.Current != null)
                {
                    _LastReceivedDateStr = FnCommon.ConvertDateByTimeZone(LastReceivedDate, true, true);
                }
                return _LastReceivedDateStr;
            }
            set { this._LastReceivedDateStr = value; }
        }

        public int? intTimeZoneOffSet { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime? dtServiceRunTime { get; set; }
        public bool? IsServiceExecuted { get; set; }
        public string EnterpriseDBName { get; set; }
        public bool? CalculateDaily { get; set; }
        public bool? CalculateMonthly { get; set; }
        [Display(Name = "AllowInsertingItemOnScan", ResourceType = typeof(ResRoomMaster))]
        public bool AllowInsertingItemOnScan { get; set; }
        [Display(Name = "AllowAutoReceiveFromEDI", ResourceType = typeof(ResRoomMaster))]
        public bool AllowAutoReceiveFromEDI { get; set; }

        [Display(Name = "IsAllowOrderDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowOrderDuplicate { get; set; }

        [Display(Name = "IsAllowWorkOrdersDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowWorkOrdersDuplicate { get; set; }

        [Display(Name = "AllowPullBeyondAvailableQty", ResourceType = typeof(ResRoomMaster))]
        [System.ComponentModel.DefaultValue(true)]
        public bool AllowPullBeyondAvailableQty { get; set; }

        [Display(Name = "PullRejectionType", ResourceType = typeof(ResRoomMaster))]
        [System.ComponentModel.DefaultValue(1)]
        public int PullRejectionType { get; set; }

        public List<RoomModuleSettingsDTO> lstRoomModleSettings { get; set; }

        [Display(Name = "CountAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string CountAutoNrFixedValue { get; set; }

        [Display(Name = "POAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string POAutoNrFixedValue { get; set; }

        [Display(Name = "PullPurchaseNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string PullPurchaseNrFixedValue { get; set; }

        [Display(Name = "ReqAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string ReqAutoNrFixedValue { get; set; }

        [Display(Name = "StagingAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> StagingAutoSequence { get; set; }

        [Display(Name = "TransferAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> TransferAutoSequence { get; set; }

        [Display(Name = "WorkOrderAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> WorkOrderAutoSequence { get; set; }

        [Display(Name = "StagingAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string StagingAutoNrFixedValue { get; set; }

        [Display(Name = "TransferAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string TransferAutoNrFixedValue { get; set; }

        [Display(Name = "WorkOrderAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string WorkOrderAutoNrFixedValue { get; set; }

        [Display(Name = "WarnUserOnAssigningNonDefaultBin", ResourceType = typeof(ResRoomMaster))]
        public bool WarnUserOnAssigningNonDefaultBin { get; set; }

        [Display(Name = "DefaultRequisitionRequiredDays", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> DefaultRequisitionRequiredDays { get; set; }

        [Display(Name = "AttachingWOWithRequisition", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> AttachingWOWithRequisition { get; set; }

        [Display(Name = "PreventMaxOrderQty", ResourceType = typeof(ResRoomMaster))]
        public int PreventMaxOrderQty { get; set; }

        [Display(Name = "DefaultCountType", ResourceType = typeof(ResRoomMaster))]
        public string DefaultCountType { get; set; }


        [Display(Name = "TAOAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> TAOAutoSequence { get; set; }
        [Display(Name = "TAOAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string TAOAutoNrFixedValue { get; set; }
        [Display(Name = "NextToolAssetOrderNo", ResourceType = typeof(ResRoomMaster))]
        public string NextToolAssetOrderNo { get; set; }

        [Display(Name = "AllowToolOrdering", ResourceType = typeof(ResRoomMaster))]
        public bool AllowToolOrdering { get; set; }

        [Display(Name = "IsWOSignatureRequired", ResourceType = typeof(ResRoomMaster))]
        public bool IsWOSignatureRequired { get; set; }

        [Display(Name = "IsIgnoreCreditRule", ResourceType = typeof(ResRoomMaster))]
        public bool IsIgnoreCreditRule { get; set; }

        [Display(Name = "IsAllowOrderCostuom", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowOrderCostuom { get; set; }

        [Display(Name = "ToolCountAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> ToolCountAutoSequence { get; set; }

        [Display(Name = "NextToolCountNo", ResourceType = typeof(ResRoomMaster))]
        public string NextToolCountNo { get; set; }

        [Display(Name = "ToolCountAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string ToolCountAutoNrFixedValue { get; set; }

        private string _LastSyncDateTimeStr;
        public string LastSyncDateTimeStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastSyncDateTimeStr) && LastSyncDateTime != null && HttpContext.Current != null)
                {
                    _LastSyncDateTimeStr = FnCommon.ConvertDateByTimeZone(LastSyncDateTime, true, true);//LastSyncDateTime.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
                }
                return _LastSyncDateTimeStr;
            }
            set { this._LastSyncDateTimeStr = value; }
        }

        private string _BillingRoomTypeStr;
        public string BillingRoomTypeStr
        {
            get
            {
                _BillingRoomTypeStr = string.Empty;
                if (BillingRoomType.HasValue && BillingRoomType.Value > 0)
                {
                    switch (BillingRoomType.Value)
                    {
                        case 1:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_AssetOnly;
                            break;
                        case 2:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_eVMI;
                            break;
                        case 3:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_Manage;
                            break;
                        case 4:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_Replenish;
                            break;
                        case 5:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_RFID;
                            break;
                        case 6:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_ToolandAssetOnly;
                            break;
                        case 7:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_ToolOnly;
                            break;
                        case 8:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_Truck;
                            break;
                        case 9:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_Optimize;
                            break;
                        case 10:
                            _BillingRoomTypeStr = ResRoomMaster.TestBilling1;
                            break;
                        case 11:
                            _BillingRoomTypeStr = ResRoomMaster.TestBilling2;
                            break;
                        case 12:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_BillingTools;
                            break;
                        case 13:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_ManageLite;
                            break;
                        case 14:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_TrackStockReplenish;
                            break;
                        case 15:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_TrackStockManageLite;
                            break;
                        case 16:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_TrackStockManage;
                            break;
                        case 17:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_TrackStockOptimize;
                            break;
                        case 18:
                            _BillingRoomTypeStr = ResRoomMaster.BillingRoomType_TrackStockTruck;
                            break;
                    }
                }
                return _BillingRoomTypeStr;
            }
            set { this._BillingRoomTypeStr = value; }
        }

        private string _CountAutoSequenceStr;
        public string CountAutoSequenceStr
        {
            get
            {
                _CountAutoSequenceStr = string.Empty;

                if (CountAutoSequence.HasValue && CountAutoSequence.Value > 0)
                {
                    switch (CountAutoSequence.Value)
                    {
                        case 3:
                            _CountAutoSequenceStr = "Incrementing by Count#";
                            break;
                        case 5:
                            _CountAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _CountAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                    }
                }
                return _CountAutoSequenceStr;
            }
            set { this._CountAutoSequenceStr = value; }
        }

        private string _POAutoSequenceStr;
        public string POAutoSequenceStr
        {
            get
            {
                _POAutoSequenceStr = string.Empty;

                if (POAutoSequence.HasValue)
                {
                    switch (POAutoSequence.Value)
                    {
                        case 0:
                            _POAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _POAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _POAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _POAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _POAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _POAutoSequenceStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                        case 8:
                            _POAutoSequenceStr = ResSupplierMaster.optDateIncrementingFixed;
                            break;
                    }
                }
                return _POAutoSequenceStr;
            }
            set { this._POAutoSequenceStr = value; }
        }

        private string _PullPurchaseNumberTypeStr;
        public string PullPurchaseNumberTypeStr
        {
            get
            {
                _PullPurchaseNumberTypeStr = string.Empty;

                if (PullPurchaseNumberType.HasValue && PullPurchaseNumberType.Value > 0)
                {
                    switch (PullPurchaseNumberType.Value)
                    {
                        case 1:
                            _PullPurchaseNumberTypeStr = ResSupplierMaster.optFixed;
                            break;
                        case 3:
                            _PullPurchaseNumberTypeStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 5:
                            _PullPurchaseNumberTypeStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _PullPurchaseNumberTypeStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _PullPurchaseNumberTypeStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                    }
                }
                return _PullPurchaseNumberTypeStr;
            }
            set { this._PullPurchaseNumberTypeStr = value; }
        }

        private string _ReqAutoSequenceStr;
        public string ReqAutoSequenceStr
        {
            get
            {
                _ReqAutoSequenceStr = string.Empty;

                if (ReqAutoSequence.HasValue)
                {
                    switch (ReqAutoSequence.Value)
                    {
                        case 0:
                            _ReqAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _ReqAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _ReqAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _ReqAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _ReqAutoSequenceStr = ResSupplierMaster.optDate;
                            break;

                    }
                }
                return _ReqAutoSequenceStr;
            }
            set { this._ReqAutoSequenceStr = value; }
        }

        private string _StagingAutoSequenceStr;
        public string StagingAutoSequenceStr
        {
            get
            {
                _StagingAutoSequenceStr = string.Empty;

                if (StagingAutoSequence.HasValue)
                {
                    switch (StagingAutoSequence.Value)
                    {
                        case 0:
                            _StagingAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _StagingAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _StagingAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _StagingAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _StagingAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _StagingAutoSequenceStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                        case 8:
                            _StagingAutoSequenceStr = ResSupplierMaster.optDateIncrementingFixed;
                            break;

                    }
                }
                return _StagingAutoSequenceStr;
            }
            set { this._StagingAutoSequenceStr = value; }
        }

        private string _TransferAutoSequenceStr;
        public string TransferAutoSequenceStr
        {
            get
            {
                _TransferAutoSequenceStr = string.Empty;

                if (TransferAutoSequence.HasValue)
                {
                    switch (TransferAutoSequence.Value)
                    {
                        case 0:
                            _TransferAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _TransferAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _TransferAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _TransferAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _TransferAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _TransferAutoSequenceStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                        case 8:
                            _TransferAutoSequenceStr = ResSupplierMaster.optDateIncrementingFixed;
                            break;

                    }
                }
                return _TransferAutoSequenceStr;

            }
            set { this._TransferAutoSequenceStr = value; }
        }

        private string _WorkOrderAutoSequenceStr;
        public string WorkOrderAutoSequenceStr
        {
            get
            {
                _WorkOrderAutoSequenceStr = string.Empty;

                if (WorkOrderAutoSequence.HasValue)
                {
                    switch (WorkOrderAutoSequence.Value)
                    {
                        case 0:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                        case 8:
                            _WorkOrderAutoSequenceStr = ResSupplierMaster.optDateIncrementingFixed;
                            break;

                    }
                }
                return _WorkOrderAutoSequenceStr;

            }
            set { this._WorkOrderAutoSequenceStr = value; }
        }

        private string _TAOAutoSequenceStr;
        public string TAOAutoSequenceStr
        {
            get
            {
                _TAOAutoSequenceStr = string.Empty;

                if (TAOAutoSequence.HasValue)
                {
                    switch (TAOAutoSequence.Value)
                    {
                        case 0:
                            _TAOAutoSequenceStr = ResSupplierMaster.optBlank;
                            break;
                        case 3:
                            _TAOAutoSequenceStr = ResSupplierMaster.optIncreamentingbyOrder;
                            break;
                        case 4:
                            _TAOAutoSequenceStr = ResSupplierMaster.optIncreamentingbyDay;
                            break;
                        case 5:
                            _TAOAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _TAOAutoSequenceStr = ResSupplierMaster.optDate;
                            break;
                        case 7:
                            _TAOAutoSequenceStr = ResSupplierMaster.optFixedIncrementing;
                            break;
                        case 8:
                            _TAOAutoSequenceStr = ResSupplierMaster.optDateIncrementingFixed;
                            break;

                    }
                }
                return _TAOAutoSequenceStr;
            }
            set { this._TAOAutoSequenceStr = value; }
        }

        private string _ToolCountAutoSequenceStr;
        public string ToolCountAutoSequenceStr
        {
            get
            {
                _ToolCountAutoSequenceStr = string.Empty;

                if (ToolCountAutoSequence.HasValue && ToolCountAutoSequence.Value > 0)
                {
                    switch (ToolCountAutoSequence.Value)
                    {

                        case 3:
                            _ToolCountAutoSequenceStr = "Incrementing by Count#";
                            break;
                        case 5:
                            _ToolCountAutoSequenceStr = ResSupplierMaster.optDateIncrementing;
                            break;
                        case 6:
                            _ToolCountAutoSequenceStr = ResSupplierMaster.optDate;
                            break;

                    }
                }
                return _ToolCountAutoSequenceStr;
            }
            set { this._ToolCountAutoSequenceStr = value; }
        }

        private string _PullRejectionTypeStr;
        public string PullRejectionTypeStr
        {
            get
            {
                _PullRejectionTypeStr = string.Empty;

                if (PullRejectionType > 0)
                {
                    switch (PullRejectionType)
                    {
                        case 1:
                            _PullRejectionTypeStr = ResRoomMaster.RejDeletePull;
                            break;
                        case 2:
                            _PullRejectionTypeStr = ResRoomMaster.RejAdjustmentPull;
                            break;
                    }
                }
                return _PullRejectionTypeStr;
            }
            set { this._PullRejectionTypeStr = value; }
        }

        private string _ReplenishmentTypeStr;
        public string ReplenishmentTypeStr
        {
            get
            {
                _ReplenishmentTypeStr = string.Empty;

                if (!string.IsNullOrEmpty(ReplenishmentType))
                {
                    switch (ReplenishmentType)
                    {
                        case "1":
                            _ReplenishmentTypeStr = ResRoomMaster.ReplanishmentType_Itemreplenish;
                            break;
                        case "2":
                            _ReplenishmentTypeStr = ResRoomMaster.ReplanishmentType_Locationreplenish;
                            break;
                        case "3":
                            _ReplenishmentTypeStr = ResRoomMaster.ReplanishmentType_Both;
                            break;
                    }
                }
                return _ReplenishmentTypeStr;
            }
            set { this._ReplenishmentTypeStr = value; }
        }

        private string _MethodOfValuingInventoryStr;
        public string MethodOfValuingInventoryStr
        {
            get
            {
                _MethodOfValuingInventoryStr = string.Empty;

                if (!string.IsNullOrEmpty(MethodOfValuingInventory))
                {
                    switch (MethodOfValuingInventory)
                    {
                        case "3":
                            _MethodOfValuingInventoryStr = "Average cost";
                            break;
                        case "4":
                            _MethodOfValuingInventoryStr = "Last cost";
                            break;
                    }
                }
                return _MethodOfValuingInventoryStr;
            }
            set { this._MethodOfValuingInventoryStr = value; }
        }

        private string _BaseOfInventoryStr;
        public string BaseOfInventoryStr
        {
            get
            {
                _BaseOfInventoryStr = string.Empty;

                if (BaseOfInventory.HasValue && BaseOfInventory.Value > 0)
                {
                    switch (BaseOfInventory)
                    {
                        case 1:
                            _BaseOfInventoryStr = "Cost";
                            break;
                        case 2:
                            _BaseOfInventoryStr = "Turn";
                            break;
                    }
                }
                return _BaseOfInventoryStr;
            }
            set { this._BaseOfInventoryStr = value; }
        }

        private string _AttachingWOWithRequisitionStr;
        public string AttachingWOWithRequisitionStr
        {
            get
            {
                _AttachingWOWithRequisitionStr = string.Empty;

                if (AttachingWOWithRequisition.HasValue && AttachingWOWithRequisition.Value > 0)
                {
                    switch (AttachingWOWithRequisition)
                    {
                        case 1:
                            _AttachingWOWithRequisitionStr = ResRoomMaster.AttachedWOReq_NewWO;
                            break;
                        case 2:
                            _AttachingWOWithRequisitionStr = ResRoomMaster.AttachedWOReq_ExistingWO;
                            break;
                        case 3:
                            _AttachingWOWithRequisitionStr = ResRoomMaster.Mixed;
                            break;
                    }
                }
                return _AttachingWOWithRequisitionStr;
            }
            set { this._AttachingWOWithRequisitionStr = value; }
        }

        private string _PreventMaxOrderQtyStr;
        public string PreventMaxOrderQtyStr
        {
            get
            {
                _PreventMaxOrderQtyStr = string.Empty;

                if (PreventMaxOrderQty > 0)
                {
                    switch (PreventMaxOrderQty)
                    {
                        case 1:
                            _PreventMaxOrderQtyStr = ResRoomMaster.PreventMaxOrderQty_None;
                            break;
                        case 2:
                            _PreventMaxOrderQtyStr = ResRoomMaster.PreventMaxOrderQty_Onorder;
                            break;
                    }
                }
                return _PreventMaxOrderQtyStr;
            }
            set { this._PreventMaxOrderQtyStr = value; }
        }

        private string _DefaultCountTypeStr;
        public string DefaultCountTypeStr
        {
            get
            {
                _DefaultCountTypeStr = string.Empty;

                if (!string.IsNullOrEmpty(DefaultCountType))
                {
                    switch (DefaultCountType)
                    {
                        case "A":
                            _DefaultCountTypeStr = "Adjustment";
                            break;
                        case "M":
                            _DefaultCountTypeStr = "Manual";
                            break;
                    }
                }
                return _DefaultCountTypeStr;
            }
            set { this._DefaultCountTypeStr = value; }
        }

        [Display(Name = "ForceSupplierFilter", ResourceType = typeof(ResRoomMaster))]
        public bool ForceSupplierFilter { get; set; }

        [Display(Name = "SuggestedReturn", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedReturn { get; set; }

        [Display(Name = "IsAllowQuoteDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowQuoteDuplicate { get; set; }

        [Display(Name = "QuoteAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> QuoteAutoSequence { get; set; }

        [Display(Name = "NextQuoteNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextQuoteNo { get; set; }

        [Display(Name = "QuoteAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string QuoteAutoNrFixedValue { get; set; }

        [Display(Name = "AllowOrderCloseAfterDays", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 1000, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> AllowOrderCloseAfterDays { get; set; }
        public bool IsELabel { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UserName", ResourceType = typeof(ResUserMaster))]
        [RegularExpression(@"[a-zA-Z0-9 ]+", ErrorMessage = "Special Charactor is not allowed.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ResUserMaster))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResUserMaster))]
        public string Password { get; set; }

        [Display(Name = "CompanyCode", ResourceType = typeof(ResRoomMaster))]
        public string CompanyCode { get; set; }
        [Display(Name = "StoreCode", ResourceType = typeof(ResRoomMaster))]
        public string StoreCode { get; set; }
    }
}
