using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace eTurns.DTO
{
    [Serializable]
    public class RoomDTO
    {
        [Key]
        [Display(Name = "ID", ResourceType = typeof(ResCommon))]
        public Int64 ID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "RoomName", ResourceType = typeof(ResRoomMaster))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string RoomName { get; set; }

        //[Display(Name = "Company ID")]
        //public int? CompanyID { get; set; }

        [Display(Name = "CompanyName", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string CompanyName { get; set; }

        [Display(Name = "ContactName", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string ContactName { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string streetaddress { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string City { get; set; }

        [Display(Name = "State", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string State { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string PostalCode { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ResCommon))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Country { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression(@"[a-zA-Z0-9.!#$%&'*+-/=?\^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+", ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ResMessage))]
        public string Email { get; set; }

        [Display(Name = "Phone", ResourceType = typeof(ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "InvalidPhoneValue", ErrorMessageResourceType = typeof(ResRoomMaster))]
        public string PhoneNo { get; set; }

        [Display(Name = "InvoiceBranch", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string InvoiceBranch { get; set; }

        [Display(Name = "CustomerNumber", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string CustomerNumber { get; set; }

        [Display(Name = "BillingRoomType", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int? BillingRoomType { get; set; }

        [Display(Name = "BlanketPO", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string BlanketPO { get; set; }

        [Display(Name = "IsConsignment", ResourceType = typeof(ResRoomMaster))]
        public bool IsConsignment { get; set; }

        //[Display(Name = "Manual Min")]
        //public bool ManualMin { get; set; }

        //public Int64 ManualMinValue { get; set; }
        [Display(Name = "SuggestedOrder", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedOrder { get; set; }

        [Display(Name = "SuggestedTransfer", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedTransfer { get; set; }

        [Display(Name = "ReplineshmentRoom", ResourceType = typeof(ResRoomMaster))]
        public int? ReplineshmentRoom { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ReplenishmentType", ResourceType = typeof(ResRoomMaster))]
        public String ReplenishmentType { get; set; }

        //[Display(Name = "IsItemReplenishment", ResourceType = typeof(ResRoomMaster))]
        //public bool IsItemReplenishment { get; set; }

        //[Display(Name = "IsBinReplenishment", ResourceType = typeof(ResRoomMaster))]
        //public bool IsBinReplenishment { get; set; }

        [Display(Name = "IseVMI", ResourceType = typeof(ResRoomMaster))]
        public bool IseVMI { get; set; }

        [Display(Name = "MaxOrderSize", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? MaxOrderSize { get; set; }

        [Display(Name = "HighPO", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string HighPO { get; set; }

        [Display(Name = "HighJob", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string HighJob { get; set; }

        [Display(Name = "HighTransfer", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string HighTransfer { get; set; }

        [Display(Name = "HighCount", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [RegularExpression("([0-9]+)", ErrorMessage = "Enter proper value.")]
        public string HighCount { get; set; }

        [Display(Name = "GlobalMarkupParts", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? GlobMarkupParts { get; set; }

        [Display(Name = "GlobalMarkupLabor", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? GlobMarkupLabor { get; set; }

        [Display(Name = "Tax1Parts", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax1Parts { get; set; }

        [Display(Name = "Tax1Labor", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax1Labor { get; set; }

        [Display(Name = "Tax1name", ResourceType = typeof(ResRoomMaster))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Tax1name { get; set; }

        [Display(Name = "Tax1percent", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? Tax1Rate { get; set; }

        [Display(Name = "Tax2Parts", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax2Parts { get; set; }

        [Display(Name = "Tax2Labor", ResourceType = typeof(ResRoomMaster))]
        public bool IsTax2Labor { get; set; }

        [Display(Name = "Tax2name", ResourceType = typeof(ResRoomMaster))]
        [StringLength(127, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string tax2name { get; set; }

        [Display(Name = "Tax2percent", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? Tax2Rate { get; set; }

        [Display(Name = "Tax2onTax1", ResourceType = typeof(ResRoomMaster))]
        public bool? IsTax2onTax1 { get; set; }

        [Display(Name = "IsTrending", ResourceType = typeof(ResRoomMaster))]
        public bool IsTrending { get; set; }



        [Display(Name = "TrendingFormula", ResourceType = typeof(ResRoomMaster))]
        [StringLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
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
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string GXPRConsJob { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "CostCenter", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string CostCenter { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UniqueID", ResourceType = typeof(ResRoomMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string UniqueID { get; set; }



        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        [Display(Name = "ActiveOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> ActiveOn { get; set; }
        public string ActiveOnDateStr { get; set; }


        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        //[Display(Name = "Room")]
        //public Nullable<Int64> Room { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }

        //Added
        //[Display(Name = "Room")]
        //public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }


        [Display(Name = "ValuingInventoryMethod", ResourceType = typeof(ResRoomMaster))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string MethodOfValuingInventory { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResRoomMaster))]
        public bool IsActive { get; set; }

        [Display(Name = "LicenseBilled", ResourceType = typeof(ResRoomMaster))]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LicenseBilled { get; set; }

        [Display(Name = "LicenseBilled", ResourceType = typeof(ResRoomMaster))]
        public System.String LicenseBilledStri { get; set; }

        [Display(Name = "NextCountNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextCountNo { get; set; }

        //POAutoSequence
        [Display(Name = "POAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> POAutoSequence { get; set; }

        [Display(Name = "CountAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> CountAutoSequence { get; set; }

        [Display(Name = "NextOrderNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextOrderNo { get; set; }

        [Display(Name = "NextRequisitionNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextRequisitionNo { get; set; }

        [Display(Name = "NextStagingNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextStagingNo { get; set; }

        [Display(Name = "NextTransferNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextTransferNo { get; set; }

        [Display(Name = "NextWorkOrderNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextWorkOrderNo { get; set; }

        [Display(Name = "RoomGrouping", ResourceType = typeof(ResRoomMaster))]
        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string RoomGrouping { get; set; }

        [Display(Name = "TransferFrequency", ResourceType = typeof(ResRoomMaster))]
        [StringLength(64, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
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
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int? TrendingSampleSize { get; set; }

        [Display(Name = "TrendingSampleSizeDivisor", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public int? TrendingSampleSizeDivisor { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "SourceOfTrending", ResourceType = typeof(ResRoomMaster))]
        public int? SourceOfTrending { get; set; }

        [Display(Name = "AverageUsageTransactions", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int? AverageUsageTransactions { get; set; }

        [Display(Name = "AverageUsageSampleSize", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int? AverageUsageSampleSize { get; set; }

        [Display(Name = "AverageUsageSampleSizeDivisor", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
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
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public long? DefaultSupplierID { get; set; }

        [Display(Name = "DefaultSupplier", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string DefaultSupplierName { get; set; }

        public long EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }

        [Display(Name = "NextAssetNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextAssetNo { get; set; }

        [Display(Name = "NextBinNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextBinNo { get; set; }

        [Display(Name = "NextKitNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextKitNo { get; set; }

        [Display(Name = "NextItemNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextItemNo { get; set; }

        [Display(Name = "NextProjectSpendNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextProjectSpendNo { get; set; }

        [Display(Name = "NextToolNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64? NextToolNo { get; set; }

        //[Display(Name = "InventoryConsuptionMethod", ResourceType = typeof(ResRoomMaster))]
        [Display(Name = "InventoryConsuptionMethod", ResourceType = typeof(ResRoomMaster))]
        public string InventoryConsuptionMethod { get; set; }

        [Display(Name = "DefaultBinID", ResourceType = typeof(ResRoomMaster))]
        public Nullable<Int64> DefaultBinID { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
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
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double SlowMovingValue { get; set; }

        [Display(Name = "FastMovingValue", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double FastMovingValue { get; set; }
        public string ReplineshmentRoomName { get; set; }
        public short RoomActiveStatus { get; set; }
        public short ReclassifyAllItems { get; set; }
        public int RoomNameChange { get; set; }
        public int SOSettingChanged { get; set; }
        public int STSettingChanged { get; set; }
        public int SRSettingChanged { get; set; }

        [Display(Name = "RequestedXDays", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int32? RequestedXDays { get; set; }

        [Display(Name = "RequestedYDays", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Int32? RequestedYDays { get; set; }

        [Display(Name = "LastPullDate", ResourceType = typeof(ResRoomMaster))]
        public Nullable<DateTime> LastPullDate { get; set; }

        [Display(Name = "LastOrderDate", ResourceType = typeof(ResRoomMaster))]
        public Nullable<DateTime> LastOrderDate { get; set; }

        [Display(Name = "BaseOfInventory", ResourceType = typeof(ResRoomMaster))]
        public int? BaseOfInventory { get; set; }

        [Display(Name = "eVMIWaitCommand", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64 eVMIWaitCommand { get; set; }

        [Display(Name = "eVMIWaitPort", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Int64 eVMIWaitPort { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ShelfLifeleadtimeOrdRpt", ResourceType = typeof(ResRoomMaster))]
        public int? ShelfLifeleadtimeOrdRpt { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "LeadTimeOrdRpt", ResourceType = typeof(ResRoomMaster))]
        public int? LeadTimeOrdRpt { get; set; }
        public int? MaintenanceDueNoticeDays { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate) && HttpContext.Current != null)
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true, true);// Created.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true, true);//Updated.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _ActiveOn = FnCommon.ConvertDateByTimeZone(ActiveOn, true, true);//ActiveOn.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));//FnCommon.ConvertDateByTimeZone(ActiveOn, true);
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

        //ReqAutoSequence
        [Display(Name = "RequisitionAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> ReqAutoSequence { get; set; }

        private string _LastOrderDateStr;
        public string LastOrderDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_LastOrderDateStr) && LastOrderDate != null && HttpContext.Current != null)
                {
                    _LastOrderDateStr = FnCommon.ConvertDateByTimeZone(LastOrderDate, true, true);// LastOrderDate.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _LicenseBilledStr = FnCommon.ConvertDateByTimeZone(LicenseBilled, true, true);//LicenseBilled.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _LastPullDateStr = FnCommon.ConvertDateByTimeZone(LastPullDate, true, true);//LastPullDate.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
                }
                return _LastPullDateStr;
            }
            set { this._LastPullDateStr = value; }
        }


        public DateTime? LastSyncDateTime { get; set; }
        public string PDABuildVersion { get; set; }
        public string LastSyncUserName { get; set; }

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
        [Display(Name = "IsAllowRequisitionDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool? IsAllowRequisitionDuplicate { get; set; }
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
                    _LastTrasnferedDateStr = FnCommon.ConvertDateByTimeZone(LastTrasnferedDate, true, true);//LastTrasnferedDate.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
                    _LastReceivedDateStr = FnCommon.ConvertDateByTimeZone(LastReceivedDate, true, true);// LastReceivedDate.Value.ToString(Convert.ToString(HttpContext.Current.Session["DateTimeFormat"]));
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
        public bool? AllowInsertingItemOnScan { get; set; }

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
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 1000, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> DefaultRequisitionRequiredDays { get; set; }

        [Display(Name = "AttachingWOWithRequisition", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> AttachingWOWithRequisition { get; set; }

        [Display(Name = "PreventMaxOrderQty", ResourceType = typeof(ResRoomMaster))]
        public int PreventMaxOrderQty { get; set; }

        [Display(Name = "DefaultCountType", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string DefaultCountType { get; set; }


        [Display(Name = "TAOAutoSequence", ResourceType = typeof(ResSupplierMaster))]
        public Nullable<System.Int32> TAOAutoSequence { get; set; }
        [Display(Name = "TAOAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string TAOAutoNrFixedValue { get; set; }
        [Display(Name = "NextToolAssetOrderNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
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
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextToolCountNo { get; set; }

        [Display(Name = "ToolCountAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string ToolCountAutoNrFixedValue { get; set; }

        [Display(Name = "ForceSupplierFilter", ResourceType = typeof(ResRoomMaster))]
        public bool ForceSupplierFilter { get; set; }

        [Display(Name = "SuggestedReturn", ResourceType = typeof(ResRoomMaster))]
        public bool SuggestedReturn { get; set; }

        public Nullable<System.Int32> NoOfItems { get; set; }

        public string ReportAppIntent { get; set; }

        public string DefaultCountTypeName { get; set; }
        public string PreventMaxOrderQtyName { get; set; }
        public string AttachingWOWithRequisitionName { get; set; }
        public string BaseOfInventoryName { get; set; }
        public string MethodOfValuingInventoryName { get; set; }
        public string ReplenishmentTypeName { get; set; }
        public string PullRejectionTypeName { get; set; }
        public string ToolCountAutoSequenceName { get; set; }
        public string TAOAutoSequenceName { get; set; }
        public string WorkOrderAutoSequenceName { get; set; }
        public string TransferAutoSequenceName { get; set; }
        public string StagingAutoSequenceName { get; set; }
        public string ReqAutoSequenceName { get; set; }
        public string PullPurchaseNumberTypeName { get; set; }
        public string POAutoSequenceName { get; set; }
        public string CountAutoSequenceName { get; set; }
        public string BillingRoomTypeName { get; set; }

        [Display(Name = "IsOrderReleaseNumberEditable", ResourceType = typeof(ResRoomMaster))]
        public bool IsOrderReleaseNumberEditable { get; set; }

        [Display(Name = "IsAllowQuoteDuplicate", ResourceType = typeof(ResRoomMaster))]
        public bool IsAllowQuoteDuplicate { get; set; }

        [Display(Name = "QuoteAutoSequence", ResourceType = typeof(ResRoomMaster))]
        public Nullable<System.Int32> QuoteAutoSequence { get; set; }

        [Display(Name = "NextQuoteNo", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression("([0-9]+)", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public string NextQuoteNo { get; set; }

        [Display(Name = "QuoteAutoNrFixedValue", ResourceType = typeof(ResRoomMaster))]
        public string QuoteAutoNrFixedValue { get; set; }

        public string QuoteAutoSequenceName { get; set; }
        [Display(Name = "DoGroupSupplierQuoteToOrder", ResourceType = typeof(ResRoomMaster))]
        public bool DoGroupSupplierQuoteToOrder { get; set; }
        [Display(Name = "DoSendQuotetoVendor", ResourceType = typeof(ResRoomMaster))]
        public bool DoSendQuotetoVendor { get; set; }
        [Display(Name = "AllowABIntegration", ResourceType = typeof(ResRoomMaster))]
        public bool AllowABIntegration { get; set; }

        [Display(Name = "AllowOrderCloseAfterDays", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"\d+", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(1, 1000, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> AllowOrderCloseAfterDays { get; set; }

        [Display(Name = "IsELabel", ResourceType = typeof(ResRoomMaster))] 
        public bool IsELabel { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "UserName", ResourceType = typeof(ResRoomMaster))]
        public string UserName { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ResRoomMaster))]
        [RegularExpression(@"^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\d]){1,})(?=(.*[\W]){1,})(?!.*\s).{8,}$", ErrorMessageResourceName = "errPasswordRuleBreak", ErrorMessageResourceType = typeof(ResUserMaster))]
        public string Password { get; set; }

        [Display(Name = "CompanyCode", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string CompanyCode { get; set; }
        [Display(Name = "StoreCode", ResourceType = typeof(ResRoomMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string StoreCode { get; set; }
        public eVMISiteSettings SensorBinRoomSettings { get; set; }
        public DateTime? TrialStartDate { get; set; }
        [Display(Name = "AllowAutoReceiveFromEDI", ResourceType = typeof(ResRoomMaster))]
        public bool? AllowAutoReceiveFromEDI { get; set; }
    }

    public class RoleDetailsRoom
    {
        public long ID { get; set; }
        public long CompanyID { get; set; }
        public long EnterpriseId { get; set; }
        public bool? IsDeleted { get; set; }
        public string RoomName { get; set; }
        public bool IsRoomActive { get; set; }
        public string CompanyName { get; set; }
    }

    public class RoomModuleSettingsDTO
    {
        public long ID { get; set; }
        public long? CompanyId { get; set; }
        public long? RoomId { get; set; }
        public long? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int? PriseSelectionOption { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public string resourcekey { get; set; }
        public string ModuleNameFromResource
        {
            get
            {
                if (!string.IsNullOrEmpty(resourcekey))
                    return ResourceRead.GetResourceValue(resourcekey, "ResModuleName");
                else
                    return ModuleName;
            }
        }
    }

    public class DeleteStatusDTO
    {
        public long RowNum { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class UnDeleteStatusDTO
    {
        public long RowNum { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class ModuleDeleteDTO
    {
        public string CommonMessage { get; set; }
        public List<DeleteStatusDTO> SuccessItems { get; set; }
        public List<DeleteStatusDTO> FailureItems { get; set; }
    }

    public class ModuleUnDeleteDTO
    {
        public string CommonMessage { get; set; }
        public List<UnDeleteStatusDTO> SuccessItems { get; set; }
        public List<UnDeleteStatusDTO> FailureItems { get; set; }
    }

    public class ResRoomMaster
    {

        private static string resourceFile = typeof(ResRoomMaster).Name;

        /// <summary>
        ///   Looks up a localized string similar to Created On.
        /// </summary>
        public static string eVMIWaitCommand
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMIWaitCommand", resourceFile);
            }
        }
        public static string AllowInsertingItemOnScan
        {
            get
            {
                return ResourceRead.GetResourceValue("AllowInsertingItemOnScan", resourceFile);
            }
        }
        public static string AllowAutoReceiveFromEDI
        {
            get
            {
                return ResourceRead.GetResourceValue("AllowAutoReceiveFromEDI", resourceFile);
            }
        }

        public static string ExtPhoneNo
        {
            get
            {
                return ResourceRead.GetResourceValue("ExtPhoneNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Created On.
        /// </summary>
        public static string eVMIWaitPort
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMIWaitPort", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Created On.
        /// </summary>
        public static string LastPullDate
        {
            get
            {
                return ResourceRead.GetResourceValue("LastPullDate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created On.
        /// </summary>
        public static string LastOrderDate
        {
            get
            {
                return ResourceRead.GetResourceValue("LastOrderDate", resourceFile);
            }
        }


        public static string IsRoomActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsRoomActive", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Average Usage Sample Size.
        /// </summary>
        public static string AverageUsageSampleSize
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsageSampleSize", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Average Usage Sample Size Divisor.
        /// </summary>
        public static string AverageUsageSampleSizeDivisor
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsageSampleSizeDivisor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Average Usage Transactions.
        /// </summary>
        public static string AverageUsageTransactions
        {
            get
            {
                return ResourceRead.GetResourceValue("AverageUsageTransactions", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Blanket PO.
        /// </summary>
        public static string BlanketPO
        {
            get
            {
                return ResourceRead.GetResourceValue("BlanketPO", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Company Name.
        /// </summary>
        public static string CompanyName
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Contact Name.
        /// </summary>
        public static string ContactName
        {
            get
            {
                return ResourceRead.GetResourceValue("ContactName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cost Center.
        /// </summary>
        public static string CostCenter
        {
            get
            {
                return ResourceRead.GetResourceValue("CostCenter", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Customer Number.
        /// </summary>
        public static string CustomerNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerNumber", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Frequency Option.
        /// </summary>
        public static string FrequencyOption
        {
            get
            {
                return ResourceRead.GetResourceValue("FrequencyOption", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Global Markup Labor.
        /// </summary>
        public static string GlobalMarkupLabor
        {
            get
            {
                return ResourceRead.GetResourceValue("GlobalMarkupLabor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Global Markup Parts.
        /// </summary>
        public static string GlobalMarkupParts
        {
            get
            {
                return ResourceRead.GetResourceValue("GlobalMarkupParts", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GXPR Consignment Job.
        /// </summary>
        public static string GXPRConsJob
        {
            get
            {
                return ResourceRead.GetResourceValue("GXPRConsJob", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to High Count.
        /// </summary>
        public static string HighCount
        {
            get
            {
                return ResourceRead.GetResourceValue("HighCount", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to High Job.
        /// </summary>
        public static string HighJob
        {
            get
            {
                return ResourceRead.GetResourceValue("HighJob", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to High PO.
        /// </summary>
        public static string HighPO
        {
            get
            {
                return ResourceRead.GetResourceValue("HighPO", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to High Transfer.
        /// </summary>
        public static string HighTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("HighTransfer", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enter proper value..
        /// </summary>
        public static string IsActive
        {
            get
            {
                return ResourceRead.GetResourceValue("IsActive", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Invoice Branch.
        /// </summary>
        public static string InvoiceBranch
        {
            get
            {
                return ResourceRead.GetResourceValue("InvoiceBranch", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinReplenishment.
        /// </summary>
        public static string IsBinReplenishment
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBinReplenishment", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Consignment.
        /// </summary>
        public static string IsConsignment
        {
            get
            {
                return ResourceRead.GetResourceValue("IsConsignment", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMI.
        /// </summary>
        public static string IseVMI
        {
            get
            {
                return ResourceRead.GetResourceValue("IseVMI", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Item Replenishment.
        /// </summary>
        public static string IsItemReplenishment
        {
            get
            {
                return ResourceRead.GetResourceValue("IsItemReplenishment", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Last date billed.
        /// </summary>
        public static string LicenseBilled
        {
            get
            {
                return ResourceRead.GetResourceValue("LicenseBilled", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max length is {1}..
        /// </summary>
        public static string MaxLength
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxLength", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Max Order Size.
        /// </summary>
        public static string MaxOrderSize
        {
            get
            {
                return ResourceRead.GetResourceValue("MaxOrderSize", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next Count Number.
        /// </summary>
        public static string NextCountNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextCountNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next Order Number.
        /// </summary>
        public static string NextOrderNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextOrderNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next Requisition Number.
        /// </summary>
        public static string NextRequisitionNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextRequisitionNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextTransferNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextTransferNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next WorkOrder Number.
        /// </summary>
        public static string NextWorkOrderNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextWorkOrderNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Rooms.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Rooms.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Replineshment Room.
        /// </summary>
        public static string ReplineshmentRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplineshmentRoom", resourceFile);
            }
        }


        public static string ReplenishmentType
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplenishmentType", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to The field {0} is required..
        /// </summary>
        public static string Required
        {
            get
            {
                return ResourceRead.GetResourceValue("Required", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room Grouping.
        /// </summary>
        public static string RoomGrouping
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomGrouping", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room Name.
        /// </summary>
        public static string RoomName
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Source Of Trending.
        /// </summary>
        public static string SourceOfTrending
        {
            get
            {
                return ResourceRead.GetResourceValue("SourceOfTrending", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suggested Order.
        /// </summary>
        public static string SuggestedOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedOrder", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Suggested Transfer.
        /// </summary>
        public static string SuggestedTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedTransfer", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax1 Labor.
        /// </summary>
        public static string Tax1Labor
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax1Labor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax1name.
        /// </summary>
        public static string Tax1name
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax1name", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax1 Parts.
        /// </summary>
        public static string Tax1Parts
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax1Parts", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax1Rate.
        /// </summary>
        public static string Tax1Rate
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax1Rate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2 Labor.
        /// </summary>
        public static string Tax2Labor
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2Labor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2 name.
        /// </summary>
        public static string Tax2name
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2name", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2 on Tax1.
        /// </summary>
        public static string Tax2onTax1
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2onTax1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2 Parts.
        /// </summary>
        public static string Tax2Parts
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2Parts", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2 Rate.
        /// </summary>
        public static string Tax2Rate
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2Rate", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency.
        /// </summary>
        public static string TransferFrequency
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequency", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Days.
        /// </summary>
        public static string TransferFrequencyDays
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Main Option.
        /// </summary>
        public static string TransferFrequencyMainOption
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyMainOption", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Option.
        /// </summary>
        public static string TransferFrequencyOption
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyOption", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Month.
        /// </summary>
        public static string TransferFrequencyMonth
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyMonth", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Number.
        /// </summary>
        public static string TransferFrequencyNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyNumber", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Frequency Week.
        /// </summary>
        public static string TransferFrequencyWeek
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferFrequencyWeek", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Submit.
        /// </summary>
        public static string TransferSubmit
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferSubmit", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Transfer Time.
        /// </summary>
        public static string TransferTime
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferTime", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trending.
        /// </summary>
        public static string Trending
        {
            get
            {
                return ResourceRead.GetResourceValue("Trending", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to TrendingFormula.
        /// </summary>
        public static string TrendingFormula
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormula", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Avg Days.
        /// </summary>
        public static string TrendingFormulaAvgDays
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormulaAvgDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Counts.
        /// </summary>
        public static string TrendingFormulaCounts
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormulaCounts", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Days.
        /// </summary>
        public static string TrendingFormulaDays
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormulaDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Over Days.
        /// </summary>
        public static string TrendingFormulaOverDays
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormulaOverDays", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Type.
        /// </summary>
        public static string TrendingFormulaType
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingFormulaType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Type.
        /// </summary>
        public static string BaseOfInventory
        {
            get
            {
                return ResourceRead.GetResourceValue("BaseOfInventory", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Trending Sample Size.
        /// </summary>
        public static string TrendingSampleSize
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSampleSize", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Trending Sample Size Divisor.
        /// </summary>
        public static string TrendingSampleSizeDivisor
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingSampleSizeDivisor", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UniqueID.
        /// </summary>
        public static string UniqueID
        {
            get
            {
                return ResourceRead.GetResourceValue("UniqueID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Valuing Inventory Method.
        /// </summary>
        public static string ValuingInventoryMethod
        {
            get
            {
                return ResourceRead.GetResourceValue("ValuingInventoryMethod", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax1percent.
        /// </summary>
        public static string Tax1percent
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax1percent", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tax2percent.
        /// </summary>
        public static string Tax2percent
        {
            get
            {
                return ResourceRead.GetResourceValue("Tax2percent", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Value1.
        /// </summary>
        public static string Value1
        {
            get
            {
                return ResourceRead.GetResourceValue("Value1", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Value2.
        /// </summary>
        public static string Value2
        {
            get
            {
                return ResourceRead.GetResourceValue("Value2", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Value3.
        /// </summary>
        public static string Value3
        {
            get
            {
                return ResourceRead.GetResourceValue("Value3", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsTrending.
        /// </summary>
        public static string IsTrending
        {
            get
            {
                return ResourceRead.GetResourceValue("IsTrending", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NextStagingNo.
        /// </summary>
        public static string NextStagingNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextStagingNo", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AutoTransferSetting.
        /// </summary>
        public static string AutoTransferSetting
        {
            get
            {
                return ResourceRead.GetResourceValue("AutoTransferSetting", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextAssetNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextAssetNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextBinNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextBinNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextKitNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextKitNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextItemNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextItemNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextProjectSpendNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextProjectSpendNo", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Next Transfer Number.
        /// </summary>
        public static string NextToolNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextToolNo", resourceFile);
            }
        }

        public static string InventoryConsuptionMethod
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryConsuptionMethod", resourceFile);
            }
        }

        public static string POAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("POAutoSequence", resourceFile);
            }
        }

        public static string CountAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("CountAutoSequence", resourceFile);
            }
        }

        public static string IsProjectSpendMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("IsProjectSpendMandatory", resourceFile);
            }
        }
        public static string IsAverageUsageBasedOnPull
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAverageUsageBasedOnPull", resourceFile);
            }
        }
        public static string SlowMovingValue
        {
            get
            {
                return ResourceRead.GetResourceValue("SlowMovingValue", resourceFile);
            }
        }
        public static string FastMovingValue
        {
            get
            {
                return ResourceRead.GetResourceValue("FastMovingValue", resourceFile);
            }
        }
        public static string DefaultBinID
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultBinID", resourceFile);
            }
        }

        public static string ShelfLifeleadtimeOrdRpt
        {
            get
            {
                return ResourceRead.GetResourceValue("ShelfLifeleadtimeOrdRpt", resourceFile);
            }
        }

        public static string LeadTimeOrdRpt
        {
            get
            {
                return ResourceRead.GetResourceValue("LeadTimeOrdRpt", resourceFile);
            }
        }

        public static string MaintenanceDueNoticeDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDueNoticeDays", resourceFile);
            }
        }
        public static string PullPurchaseNumberType
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNumberType", resourceFile);
            }
        }
        public static string LastPullPurchaseNumberUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("LastPullPurchaseNumberUsed", resourceFile);
            }
        }
        public static string errmsgBlankFixedLastPullPurchaseNumberUsed
        {
            get
            {
                return ResourceRead.GetResourceValue("errmsgBlankFixedLastPullPurchaseNumberUsed", resourceFile);
            }
        }
        public static string InvalidPhoneValue
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidPhoneValue", resourceFile);
            }
        }
        public static string errConsignedItemsAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errConsignedItemsAvailable", resourceFile);
            }
        }
        public static string errSupplierWithEditableReleaseAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errSupplierWithEditableReleaseAvailable", resourceFile);
            }
        }
        public static string errOrderCostuomItemsAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errOrderCostuomItemsAvailable", resourceFile);
            }
        }
        public static string errDuplicateRequisitionAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errDuplicateRequisitionAvailable", resourceFile);
            }
        }

        public static string LastSyncDateTime
        {
            get
            {
                return ResourceRead.GetResourceValue("LastSyncDateTime", resourceFile);
            }
        }

        public static string PDABuildVersion
        {
            get
            {
                return ResourceRead.GetResourceValue("PDABuildVersion", resourceFile);
            }
        }

        public static string LastSyncUserName
        {
            get
            {
                return ResourceRead.GetResourceValue("LastSyncUserName", resourceFile);
            }
        }
        public static string TrasnferedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("TrasnferedDate", resourceFile);
            }
        }

        public static string ReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedDate", resourceFile);
            }
        }

        public static string IsAllowOrderDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowOrderDuplicate", resourceFile);
            }
        }

        public static string IsAllowWorkOrdersDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowWorkOrdersDuplicate", resourceFile);
            }
        }

        public static string errDuplicateOrderAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errDuplicateOrderAvailable", resourceFile);
            }
        }

        public static string errDuplicateWorkOrderAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errDuplicateWorkOrderAvailable", resourceFile);
            }
        }

        public static string AllowPullBeyondAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("AllowPullBeyondAvailableQty", resourceFile);
            }
        }

        public static string PullRejectionType
        {
            get
            {
                return ResourceRead.GetResourceValue("PullRejectionType", resourceFile);
            }
        }

        public static string RejDeletePull
        {
            get
            {
                return ResourceRead.GetResourceValue("RejDeletePull", resourceFile);
            }
        }

        public static string RejAdjustmentPull
        {
            get
            {
                return ResourceRead.GetResourceValue("RejAdjustmentPull", resourceFile);
            }
        }

        public static string CountAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("CountAutoNrFixedValue", resourceFile);
            }
        }

        public static string POAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("POAutoNrFixedValue", resourceFile);
            }
        }

        public static string PullPurchaseNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("PullPurchaseNrFixedValue", resourceFile);
            }
        }

        public static string ReqAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqAutoNrFixedValue", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType.
        /// </summary>
        public static string BillingRoomType
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_AssetOnly.
        /// </summary>
        public static string BillingRoomType_AssetOnly
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_AssetOnly", resourceFile);
            }
        }

        public static string BillingRoomType_Optimize
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_Optimize", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_eVMI.
        /// </summary>
        public static string BillingRoomType_eVMI
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_eVMI", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_Manage.
        /// </summary>
        public static string BillingRoomType_Manage
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_Manage", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_Replenish.
        /// </summary>
        public static string BillingRoomType_Replenish
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_Replenish", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_RFID.
        /// </summary>
        public static string BillingRoomType_RFID
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_RFID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_ToolandAssetOnly.
        /// </summary>
        public static string BillingRoomType_ToolandAssetOnly
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_ToolandAssetOnly", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_ToolOnly.
        /// </summary>
        public static string BillingRoomType_ToolOnly
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_ToolOnly", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string BillingRoomType_Truck.
        /// </summary>
        public static string BillingRoomType_Truck
        {
            get
            {
                return ResourceRead.GetResourceValue("BillingRoomType_Truck", resourceFile);
            }
        }

        public static string DefaultSupplier
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultSupplier", resourceFile);
            }
        }

        public static string ModuleName
        {
            get
            {
                return ResourceRead.GetResourceValue("ModuleName", resourceFile);
            }
        }
        public static string CalculationPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("CalculationPrice", resourceFile);
            }
        }

        public static string SellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("SellPrice", resourceFile);
            }
        }

        public static string LastCost
        {
            get
            {
                return ResourceRead.GetResourceValue("LastCost", resourceFile);
            }
        }

        public static string GeneralDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("GeneralDetails", resourceFile);
            }
        }

        public static string ContactDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ContactDetails", resourceFile);
            }
        }

        public static string InvoicingDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("InvoicingDetails", resourceFile);
            }
        }

        public static string TrendingDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("TrendingDetails", resourceFile);
            }
        }

        public static string ReplenishingDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplenishingDetails", resourceFile);
            }
        }
        public static string TaxationDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("TaxationDetails", resourceFile);
            }
        }
        public static string OtherDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("OtherDetails", resourceFile);
            }
        }
        public static string RoomModuleSettings
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomModuleSettings", resourceFile);
            }
        }

        //IsAllowRequisitionDuplicate
        public static string IsAllowRequisitionDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowRequisitionDuplicate", resourceFile);
            }
        }

        public static string StagingAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingAutoSequence", resourceFile);
            }
        }

        public static string TransferAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferAutoSequence", resourceFile);
            }
        }

        public static string WorkOrderAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrderAutoSequence", resourceFile);
            }
        }

        public static string StagingAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingAutoNrFixedValue", resourceFile);
            }
        }
        public static string TransferAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("TransferAutoNrFixedValue", resourceFile);
            }
        }

        public static string WorkOrderAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("WorkOrderAutoNrFixedValue", resourceFile);
            }
        }

        //WarnUserOnAssigningNonDefaultBin
        public static string WarnUserOnAssigningNonDefaultBin
        {
            get
            {
                return ResourceRead.GetResourceValue("WarnUserOnAssigningNonDefaultBin", resourceFile);
            }
        }

        public static string RequestedXDays
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedXDays", resourceFile);
            }
        }

        public static string RequestedYDays
        {
            get
            {
                return ResourceRead.GetResourceValue("RequestedYDays", resourceFile);
            }
        }

        //ReplanishmentType_Itemreplenish
        //ReplanishmentType_Locationreplenish
        //ReplanishmentType_Both
        public static string ReplanishmentType_Itemreplenish
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplanishmentType_Itemreplenish", resourceFile);
            }
        }

        public static string ReplanishmentType_Locationreplenish
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplanishmentType_Locationreplenish", resourceFile);
            }
        }

        public static string ReplanishmentType_Both
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplanishmentType_Both", resourceFile);
            }
        }

        //DefaultRequisitionRequiredDays
        public static string DefaultRequisitionRequiredDays
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultRequisitionRequiredDays", resourceFile);
            }
        }

        //AttachingWOWithRequisition
        public static string AttachingWOWithRequisition
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachingWOWithRequisition", resourceFile);
            }
        }

        //AttachedWOReq_NewWO
        public static string AttachedWOReq_NewWO
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachedWOReq_NewWO", resourceFile);
            }
        }

        //AttachedWOReq_ExistingWO
        public static string AttachedWOReq_ExistingWO
        {
            get
            {
                return ResourceRead.GetResourceValue("AttachedWOReq_ExistingWO", resourceFile);
            }
        }

        public static string PreventMaxOrderQty
        {
            get
            {
                return ResourceRead.GetResourceValue("PreventMaxOrderQty", resourceFile);
            }
        }

        public static string PreventMaxOrderQty_None
        {
            get
            {
                return ResourceRead.GetResourceValue("PreventMaxOrderQty_None", resourceFile);
            }
        }

        public static string PreventMaxOrderQty_Onorder
        {
            get
            {
                return ResourceRead.GetResourceValue("PreventMaxOrderQty_Onorder", resourceFile);
            }
        }

        public static string DefaultCountType
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultCountType", resourceFile);
            }
        }
        public static string TAOAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("TAOAutoNrFixedValue", resourceFile);
            }
        }
        public static string NextToolAssetOrderNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextToolAssetOrderNo", resourceFile);
            }
        }
        public static string AllowToolOrdering
        {
            get
            {
                return ResourceRead.GetResourceValue("AllowToolOrdering", resourceFile);
            }
        }

        public static string IsWOSignatureRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsWOSignatureRequired", resourceFile);
            }
        }

        public static string IsIgnoreCreditRule
        {
            get
            {
                return ResourceRead.GetResourceValue("IsIgnoreCreditRule", resourceFile);
            }
        }

        public static string Enterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("Enterprise", resourceFile);
            }
        }
        public static string IsAllowOrderCostuom
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowOrderCostuom", resourceFile);
            }
        }
        public static string ToolCountAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCountAutoSequence", resourceFile);
            }
        }


        public static string NextToolCountNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextToolCountNo", resourceFile);
            }
        }
        public static string ToolCountAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCountAutoNrFixedValue", resourceFile);
            }
        }
        public static string SuggestedReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("SuggestedReturn", resourceFile);
            }
        }
        public static string ForceSupplierFilter
        {
            get
            {
                return ResourceRead.GetResourceValue("ForceSupplierFilter", resourceFile);
            }
        }

        public static string NoOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfItems", resourceFile);
            }
        }

        public static string WeightDecimalPoints
        {
            get
            {
                return ResourceRead.GetResourceValue("WeightDecimalPoints", resourceFile);
            }
        }

        public static string TransactionNumberFormat
        {
            get
            {
                return ResourceRead.GetResourceValue("TransactionNumberFormat", resourceFile);
            }
        }
        public static string ReportAppIntent
        {
            get
            {
                return ResourceRead.GetResourceValue("ReportAppIntent", resourceFile);
            }
        }

        public static string Mixed
        {
            get
            {
                return ResourceRead.GetResourceValue("Mixed", resourceFile);
            }
        }
        public static string IsOrderReleaseNumberEditable
        {
            get
            {
                return ResourceRead.GetResourceValue("IsOrderReleaseNumberEditable", resourceFile);
            }
        }
        public static string QuoteAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteAutoSequence", resourceFile);
            }
        }
        public static string NextQuoteNo
        {
            get
            {
                return ResourceRead.GetResourceValue("NextQuoteNo", resourceFile);
            }
        }
        public static string IsAllowQuoteDuplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAllowQuoteDuplicate", resourceFile);
            }
        }
        public static string QuoteAutoNrFixedValue
        {
            get
            {
                return ResourceRead.GetResourceValue("QuoteAutoNrFixedValue", resourceFile);
            }
        }
        public static string errDuplicateQuoteAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errDuplicateQuoteAvailable", resourceFile);
            }
        }
        public static string DoGroupSupplierQuoteToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("DoGroupSupplierQuoteToOrder", resourceFile);
            }
        }

        public static string DoSendQuotetoVendor
        {
            get
            {
                return ResourceRead.GetResourceValue("DoSendQuotetoVendor", resourceFile);
            }
        }
        public static string MsgPriceSelectionMissing
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPriceSelectionMissing", resourceFile);
            }
        }

        public static string MsgSetTransferAutoSequence
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSetTransferAutoSequence", resourceFile);
            }
        }

        public static string IncrementingByCountNo
        {
            get
            {
                return ResourceRead.GetResourceValue("IncrementingByCountNo", resourceFile);
            }
        }

        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", resourceFile);
            }
        }

        public static string Turn
        {
            get
            {
                return ResourceRead.GetResourceValue("Turn", resourceFile);
            }
        }

        public static string Adjustment
        {
            get
            {
                return ResourceRead.GetResourceValue("Adjustment", resourceFile);
            }
        }

        public static string Manual
        {
            get
            {
                return ResourceRead.GetResourceValue("Manual", resourceFile);
            }
        }

        public static string MsgTaxValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTaxValidation", resourceFile);
            }
        }
        public static string MsgTaxPercentRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTaxPercentRequired", resourceFile);
            }
        }
        public static string MsgTaxTwoRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTaxTwoRequired", resourceFile);
            }
        }
        public static string MsgTaxTwoPercentRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTaxTwoPercentRequired", resourceFile);
            }
        }
        public static string MsgRoomRulesNotificationPartial
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomRulesNotificationPartial", resourceFile);
            }
        }
        public static string MsgSaveRecordsRulesNotification
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSaveRecordsRulesNotification", resourceFile);
            }
        }
        public static string MsgRequiredColumnDeleteValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRequiredColumnDeleteValidation", resourceFile);
            }
        }
        public static string MsgProvideCorrectRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProvideCorrectRoom", resourceFile);
            }
        }
        public static string MsgRoomNameRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomNameRequired", resourceFile);
            }
        }
        public static string MsgRoomisInactive
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomisInactive", resourceFile);
            }
        }
        public static string MsgRoomIsNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomIsNotExist", resourceFile);
            }
        }
        public static string MsgRoomDoesNotExistinEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomDoesNotExistinEnterprise", resourceFile);
            }
        }
        public static string MsgRoomDoesNotActiveinEnterprise
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRoomDoesNotActiveinEnterprise", resourceFile);
            }
        }
        public static string AllowABIntegration
        {
            get
            {
                return ResourceRead.GetResourceValue("AllowABIntegration", resourceFile);
            }
        }

        public static string BillingRoomType_TrackStockReplenish { get { return ResourceRead.GetResourceValue("BillingRoomType_TrackStockReplenish", resourceFile); } }
        public static string BillingRoomType_TrackStockManageLite { get { return ResourceRead.GetResourceValue("BillingRoomType_TrackStockManageLite", resourceFile); } }
        public static string BillingRoomType_TrackStockManage { get { return ResourceRead.GetResourceValue("BillingRoomType_TrackStockManage", resourceFile); } }
        public static string BillingRoomType_TrackStockOptimize { get { return ResourceRead.GetResourceValue("BillingRoomType_TrackStockOptimize", resourceFile); } }
        public static string BillingRoomType_TrackStockTruck { get { return ResourceRead.GetResourceValue("BillingRoomType_TrackStockTruck", resourceFile); } }
        public static string BillingRoomType_ManageLite { get { return ResourceRead.GetResourceValue("BillingRoomType_ManageLite", resourceFile); } }
        public static string BillingRoomType_BillingTools { get { return ResourceRead.GetResourceValue("BillingRoomType_BillingTools", resourceFile); } }
        public static string TestBilling1 { get { return ResourceRead.GetResourceValue("TestBilling1", resourceFile); } }
        public static string TestBilling2 { get { return ResourceRead.GetResourceValue("TestBilling2", resourceFile); } }
        public static string AllowOrderCloseAfterDays { get { return ResourceRead.GetResourceValue("AllowOrderCloseAfterDays", resourceFile); } }
        public static string BillingRoomTypeName { get { return ResourceRead.GetResourceValue("BillingRoomTypeName", resourceFile); } }
        public static string TakeReferenceFromBillingRoomType { get { return ResourceRead.GetResourceValue("TakeReferenceFromBillingRoomType", resourceFile); } }
        public static string IsELabel { get { return ResourceRead.GetResourceValue("IsELabel", resourceFile); } }
        public static string UserName { get { return ResourceRead.GetResourceValue("UserName", resourceFile); } }
        public static string Password { get { return ResourceRead.GetResourceValue("Password", resourceFile); } }
        public static string CompanyCode { get { return ResourceRead.GetResourceValue("CompanyCode", resourceFile); } }
        public static string StoreCode { get { return ResourceRead.GetResourceValue("StoreCode", resourceFile); } }
        public static string ELabelPasswordRequiredOnELabelDetailsChange { get { return ResourceRead.GetResourceValue("ELabelPasswordRequiredOnELabelDetailsChange", resourceFile); } }
        public static string InvalidELabelUsernameAndPassword { get { return ResourceRead.GetResourceValue("InvalidELabelUsernameAndPassword", resourceFile); } }
        public static string InvalidCompanyCodeOrStoreCode { get { return ResourceRead.GetResourceValue("InvalidCompanyCodeOrStoreCode", resourceFile); } }
        public static string CompanyCodeAndStoreCodeRequired { get { return ResourceRead.GetResourceValue("CompanyCodeAndStoreCodeRequired", resourceFile); } }
        public static string InvalidELabelDetails { get { return ResourceRead.GetResourceValue("InvalidELabelDetails", resourceFile); } }
        public static string AreYouSureELabelDetailWillBeDeleted { get { return ResourceRead.GetResourceValue("AreYouSureELabelDetailWillBeDeleted", resourceFile); } }
    }
    public class RPT_RoomMasterDTO
    {
        public Int64 Id { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string InvoiceBranch { get; set; }
        public string CustomerNumber { get; set; }
        public string BlanketPO { get; set; }
        public bool? IsConsignment { get; set; }
        public bool? IsTax1Parts { get; set; }
        public bool? IsTax1Labor { get; set; }
        public string Tax1Name { get; set; }
        public double? Tax1Rate { get; set; }
        public bool? IsTax2Parts { get; set; }
        public bool? IsTax2Labor { get; set; }
        public string Tax2Name { get; set; }
        public double? Tax2Rate { get; set; }
        public int? ReplineshmentRoom { get; set; }
        public bool? IsTrending { get; set; }
        public int? SourceOfTrending { get; set; }
        public string TrendingFormula { get; set; }
        public int? TrendingFormulaType { get; set; }
        public int? TrendingFormulaDays { get; set; }
        public int? TrendingFormulaOverDays { get; set; }
        public bool? SuggestedOrder { get; set; }
        public bool? SuggestedTransfer { get; set; }
        public string AverageUsageFormula { get; set; }
        public string MethodOfValuingInventory { get; set; }
        public string AutoCreateTransferFrequency { get; set; }
        public string AutoCreateTransferTime { get; set; }
        public bool? AutoCreateTransferSubmit { get; set; }
        public string IsActive { get; set; }
        public string LicenseBilled { get; set; }
        public string NextCountNo { get; set; }
        public string NextOrderNo { get; set; }
        public Int64? NextRequisitionNo { get; set; }
        public Int64? NextStagingNo { get; set; }
        public Int64? NextTransferNo { get; set; }
        public Int64? NextWorkOrderNo { get; set; }
        public string RoomGrouping { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public Int64? CreatedBy { get; set; }
        public Int64? LastUpdatedBy { get; set; }
        public Int64? Room { get; set; }

        public bool? IsDeleted { get; set; }
        public double? MaxOrderSize { get; set; }
        public string HighPO { get; set; }
        public string HighJob { get; set; }
        public string HighTransfer { get; set; }
        public string HighCount { get; set; }
        public double? GlobMarkupParts { get; set; }
        public double? GlobMarkupLabor { get; set; }
        public string UniqueID { get; set; }
        public string CostCenter { get; set; }
        public string GXPRConsJob { get; set; }
        public bool? IsArchived { get; set; }
        public Guid? GUID { get; set; }
        public bool? IsTax2onTax1 { get; set; }
        public int? TrendingFormulaAvgDays { get; set; }
        public int? TrendingFormulaCounts { get; set; }
        public int? TransferFrequencyOption { get; set; }
        public string TransferFrequencyDays { get; set; }
        public int? TransferFrequencyMonth { get; set; }
        public int? TransferFrequencyNumber { get; set; }
        public int? TransferFrequencyWeek { get; set; }
        public int? TransferFrequencyMainOption { get; set; }
        public int? TrendingSampleSize { get; set; }
        public int? TrendingSampleSizeDivisor { get; set; }
        public int? AverageUsageSampleSize { get; set; }
        public int? AverageUsageSampleSizeDivisor { get; set; }
        public int? AverageUsageTransactions { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }
        public Int64? CompanyID { get; set; }
        public Int64? DefaultSupplierID { get; set; }
        public Int64? NextAssetNo { get; set; }
        public Int64? NextBinNo { get; set; }
        public Int64? NextKitNo { get; set; }
        public Int64? NextItemNo { get; set; }
        public Int64? NextProjectSpendNo { get; set; }
        public Int64? NextToolNo { get; set; }
        public string InventoryConsuptionMethod { get; set; }
        public string ReplenishmentType { get; set; }
        public Int64? DefaultBinID { get; set; }
        public string IsRoomActive { get; set; }
        public bool? IsProjectSpendMandatory { get; set; }
        public int? POAutoSequence { get; set; }
        public bool? IsAverageUsageBasedOnPull { get; set; }
        public double? SlowMovingValue { get; set; }
        public double? FastMovingValue { get; set; }
        public int? RequestedXDays { get; set; }
        public int? RequestedYDays { get; set; }
        public string DefaultBinName { get; set; }
        public int? BaseOfInventory { get; set; }
        public Int64? eVMIWaitCommand { get; set; }
        public Int64? eVMIWaitPort { get; set; }
        public int? CountAutoSequence { get; set; }
        public int? ShelfLifeleadtimeOrdRpt { get; set; }
        public int? LeadTimeOrdRpt { get; set; }
        public int? PullPurchaseNumberType { get; set; }
        public string LastPullPurchaseNumberUsed { get; set; }
        public string SupplierName { get; set; }
        public string RoomCreatedByName { get; set; }
        public string RoomUpdatedByName { get; set; }
        public string RoomCompanyName { get; set; }
        public Int64? RoomCompanyId { get; set; }
        public string RoomCurrentDateTime { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string LastOrderDate { get; set; }
        public int? BillingRoomType { get; set; }

        public int? StagingAutoSequence { get; set; }

        public int? TransferAutoSequence { get; set; }

        public int? WorkOrderAutoSequence { get; set; }

        public int? ToolCountAutoSequence { get; set; }
        public string NextToolCountNo { get; set; }
        public bool? SuggestedReturn { get; set; }
    }
    public class RPT_CompanyMasterDTO
    {
        public Int64 Id { get; set; }
        public Int64 EnterpriseID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string CompanyLogo { get; set; }
        public string ContactEmail { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public Guid GUID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public bool IsActive { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> LastUpdatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }

    public class RoomCompanyDTO
    {
        public Int64 RoomID { get; set; }
        public string RoomName { get; set; }
        public Int64 CompanyID { get; set; }
        public string CompanyName { get; set; }
    }

    public class RoomResourcesTable
    {
        public string ResourceKey { get; set; }
        public string ResourceValue { get; set; }
    }

    /// <summary>
    /// This Enum will be used for PreventMaxOrderQty
    /// </summary>
    public enum PreventMaxOrderQty
    {
        None = 1,
        OnOrder = 2
    }

    /// <summary>
    /// AttachingWOWithRequisition
    /// </summary>
    public enum AttachingWOWithRequisition
    {
        New = 1,
        Existing = 2,
        Mixed = 3
    }
}
