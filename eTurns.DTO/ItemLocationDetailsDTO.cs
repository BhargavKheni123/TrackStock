using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class ItemLocationDetailsDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int64> BinID { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //MeasurementID
        [Display(Name = "MeasurementID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int32> MeasurementID { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LotNumber { get; set; }

        //SerialNumber
        [Display(Name = "SerialNumber", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        //ExpirationDate
        [Display(Name = "ExpirationDate", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.DateTime> ReceivedDate { get; set; }


        //ExpirationDate
        [Display(Name = "ExpirationDate", ResourceType = typeof(ResItemLocationDetails))]
        public string Expiration { get; set; }

        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResItemLocationDetails))]
        public string Received { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Range(0, float.MaxValue)]
        public Nullable<System.Double> Cost { get; set; }

        //eVMISensorPort
        [Display(Name = "eVMISensorPort", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int32> eVMISensorPort { get; set; }

        //eVMISensorID
        [Display(Name = "eVMISensorID", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String eVMISensorID { get; set; }
        public string eVMISensorPortstr { get; set; }
        public double? eVMISensorIDdbl { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }
        public string ErrorMessege { get; set; }
        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<Guid> ItemGUID { get; set; }

        /// <summary>
        /// Property Add for SerialType Label
        /// </summary>
        public Nullable<System.Int64> ItemID { get; set; }
        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //ItemNumber
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ItemNumber { get; set; }

        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        public string BinNumber { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }


        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }


        // added by niraj
        public Int32 ItemType { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }

        public string mode { get; set; }
        //OrderDetailID
        public Nullable<System.Guid> OrderDetailGUID { get; set; }
        public Nullable<System.Guid> KitDetailGUID { get; set; }
        public Nullable<System.Guid> TransferDetailGUID { get; set; }

        public Boolean IsCreditPull { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MaximumQuantity { get; set; }

        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        public Nullable<Guid> ProjectSpentGUID { get; set; }

        public string ProjectSpend { get; set; }

        public Nullable<System.Double> Markup { get; set; }

        public Nullable<System.Double> SellPrice { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        public string PackSlipNumber { get; set; }

        public bool IsDefault { get; set; }
        public bool IsItemLevelMinMax { get; set; }
        public Nullable<Guid> WorkOrderGUID { get; set; }
        public double? InitialQuantity { get; set; }
        public bool IsConsignedSerialLot { get; set; }
        public Guid? RefWebSelfGUID { get; set; }
        public Guid? RefPDASelfGUID { get; set; }
        public double? InitialQuantityWeb { get; set; }
        public double? InitialQuantityPDA { get; set; }
        public bool? IsPDAEdit { get; set; }
        public bool? IsWebEdit { get; set; }
        public double MoveQuantity { get; set; }
        public bool IsOnlyFromUI { get; set; }

        public Nullable<System.Guid> CountLineItemDtlGUID { get; set; }
        public Nullable<System.Double> CountCustOrConsQty { get; set; }
        public string InsertedFrom { get; set; }
        public string PullOrderNumber { get; set; }
        public double? ConQRunningTotal { get; set; }
        public double? CustQRunningTotal { get; set; }
        public double? ConQConsumable { get; set; }
        public double? CustQConsumable { get; set; }
        public Nullable<Guid> SupplierAccountGuid { get; set; }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdDate))
                {
                    _createdDate = FnCommon.ConvertDateByTimeZone(Created, true);
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
                if (string.IsNullOrEmpty(_updatedDate))
                {
                    _updatedDate = FnCommon.ConvertDateByTimeZone(Updated, true);
                }
                return _updatedDate;
            }
            set { this._updatedDate = value; }
        }

        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }
        private string _Expiration;
        public string ExpirationStr
        {
            get
            {
                if (string.IsNullOrEmpty(_Expiration))
                {
                    _Expiration = FnCommon.ConvertDateByTimeZone(ExpirationDate, true);
                }
                return _Expiration;
            }
            set { this._Expiration = value; }
        }


        public string ReceivedDateStr { get; set; }


        public System.String PullCredit { get; set; }

        public Guid? PullGUID { get; set; }

        public bool IsStagingLocation { get; set; }

        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        public bool? IsEnforceDefaultPullQuantity { get; set; }

        public Nullable<System.Double> DefaultReorderQuantity { get; set; }

        public bool? IsEnforceDefaultReorderQuantity { get; set; }


        public Guid? PullGUIDForCreditHistory { get; set; }
        public Guid? PullDetailGUIDForCreditHistory { get; set; }
        public string ItemDescription { get; set; }
        public string BinUDF1 { get; set; }
        public string BinUDF2 { get; set; }
        public string BinUDF3 { get; set; }
        public string BinUDF4 { get; set; }
        public string BinUDF5 { get; set; }
    }


    public class ItemLocationLotSerialDTO
    {
        public int SrNo { get; set; }
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> BinID { get; set; }
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }
        public Nullable<System.Double> ConsignedQuantity { get; set; }
        public Nullable<System.Double> CustomerOwnedQuantityEntry { get; set; }
        public Nullable<System.Double> ConsignedQuantityEntry { get; set; }
        public System.String LotNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string Expiration { get; set; }
        public string Received { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Double> PullCost { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public string BinNumber { get; set; }
        public Int32 ItemType { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
        public Nullable<System.Guid> OrderDetailGUID { get; set; }
        public Nullable<System.Guid> KitDetailGUID { get; set; }
        public Nullable<System.Guid> TransferDetailGUID { get; set; }
        public Boolean IsCreditPull { get; set; }
        public Nullable<System.Double> CriticalQuantity { get; set; }
        public Nullable<System.Double> MinimumQuantity { get; set; }
        public Nullable<System.Double> MaximumQuantity { get; set; }
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }
        public Nullable<Guid> ProjectSpentGUID { get; set; }
        public Nullable<System.Double> Markup { get; set; }
        public Nullable<System.Double> SellPrice { get; set; }
        public bool IsDefault { get; set; }
        public bool IsItemLevelMinMax { get; set; }
        public Nullable<Guid> WorkOrderGUID { get; set; }
        public string ItemNumber { get; set; }
        public long? Room { get; set; }
        public bool IsConsignedLotSerial { get; set; }
        public double LotSerialQuantity { get; set; }
        public string LotOrSerailNumber { get; set; }
        public double PullQuantity { get; set; }
        public double? CumulativeTotalQuantity { get; set; }
        public bool IsSelected { get; set; }
        public bool IsStagingLocationLotSerial { get; set; }
        public string ValidationMessage { get; set; }
        public double ConsignedTobePulled { get; set; }
        public double CustomerOwnedTobePulled { get; set; }
        public double TotalTobePulled { get; set; }
        public double TotalPullCost { get; set; }
        public double QuantityToMove { get; set; }
        public Nullable<Guid> MaterialStagingGUID { get; set; }
        public Guid ItemLocationDetailGUID { get; set; }
        public string SerialLotExpirationcombin { get; set; }
        public string strExpirationDate { get; set; }
        public Nullable<Guid> PullGUID { get; set; }
        public Nullable<Guid> PullDetailGUID { get; set; }
    }

    public class CostDTO
    {
        public System.Double ExtCost { get; set; }
        public System.Double AvgCost { get; set; }
        public System.Double? Cost { get; set; }
        public System.Double? Markup { get; set; }
        public System.Double? SellPrice { get; set; }

        public System.Double? PerItemCost { get; set; }
    }
    public class ItemPullInfo
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
        public string ProjectSpendName { get; set; }
        public long ItemID { get; set; }
        public string ItemNumber { get; set; }
        public long BinID { get; set; }
        public string BinNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public double PullQuantity { get; set; }
        public long LastUpdatedBy { get; set; }
        public Guid? PullGUID { get; set; }
        public long CreatedBy { get; set; }
        public double PullCost { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool CanOverrideProjectLimits { get; set; }
        public bool ValidateProjectSpendLimits { get; set; }
        public List<ItemLocationLotSerialDTO> lstItemPullDetails { get; set; }
        public Guid? RequisitionDetailsGUID { get; set; }
        public Guid? WorkOrderItemGUID { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }
        public Guid? WorkOrderDetailGUID { get; set; }
        public Guid? CountLineItemGuid { get; set; }
        public string PullOrderNumber { get; set; }


        public Nullable<System.Guid> ToolGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Nullable<System.Guid> TechnicianGUID { get; set; }

        public string ToolName { get; set; }
        public string Technician { get; set; }
        public bool IsStatgingLocationPull { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
        public Nullable<System.Guid> SupplierAccountGuid { get; set; }

        public bool isValidateExpiredItem { get; set; }
}

    public class ItemPullLotSerialInfo
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
        public string ProjectSpendName { get; set; }
        public long ItemID { get; set; }
        public string ItemNumber { get; set; }
        public long BinID { get; set; }
        public string BinNumber { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public double PullQuantity { get; set; }
        public long LastUpdatedBy { get; set; }
        public Guid? PullGUID { get; set; }
        public long CreatedBy { get; set; }
        public double PullCost { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool CanOverrideProjectLimits { get; set; }
        public bool ValidateProjectSpendLimits { get; set; }
        public List<ItemLocationLotSerialDTO> lstItemPullDetails { get; set; }
        public Guid? RequisitionDetailsGUID { get; set; }
        public Guid? WorkOrderItemGUID { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }
        public Guid? WorkOrderDetailGUID { get; set; }
        public Guid? CountLineItemGuid { get; set; }
        public string PullOrderNumber { get; set; }


        public Nullable<System.Guid> ToolGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Nullable<System.Guid> TechnicianGUID { get; set; }

        public string ToolName { get; set; }
        public string Technician { get; set; }
        public bool IsStatgingLocationPull { get; set; }

        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }

        public System.String LotNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        public string LotOrSerailNumber { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }

        public string ValidationMessage { get; set; }
        public string SerialLotExpirationcombin { get; set; }
        public string strExpirationDate { get; set; }

    }

    public class PullErrorInfo
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ResItemLocationDetails
    {
        private static string ResourceFileName = "ResItemLocationDetails";

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string Action
        {
            get
            {
                return ResourceRead.GetResourceValue("Action", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationDetails {0} already exist! Try with Another!.
        /// </summary>
        public static string Duplicate
        {
            get
            {
                return ResourceRead.GetResourceValue("Duplicate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to HistoryID.
        /// </summary>
        public static string HistoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("HistoryID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IncludeArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Include Deleted:.
        /// </summary>
        public static string IncludeDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IncludeDeleted", ResourceFileName);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Search.
        /// </summary>
        public static string Search
        {
            get
            {
                return ResourceRead.GetResourceValue("Search", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationDetails.
        /// </summary>
        public static string ItemLocationDetailsHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationDetailsHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationDetails.
        /// </summary>
        public static string ItemLocationDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationDetails", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View History.
        /// </summary>
        public static string ViewHistory
        {
            get
            {
                return ResourceRead.GetResourceValue("ViewHistory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        public static string ID
        {
            get
            {
                return ResourceRead.GetResourceValue("ID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemID.
        /// </summary>
        public static string ItemID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinID.
        /// </summary>
        public static string BinID
        {
            get
            {
                return ResourceRead.GetResourceValue("BinID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BinID.
        /// </summary>
        public static string BinNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("BinNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CustomerOwnedQuantity.
        /// </summary>
        public static string CustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantity", ResourceFileName);
            }
        }

        public static string CustomerOwnedQuantityToCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityToCredit", ResourceFileName);
            }
        }

        public static string CustomerOwnedQuantityCount
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityCount", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ConsignedQuantity.
        /// </summary>
        public static string ConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantity", ResourceFileName);
            }
        }

        public static string ConsignedQuantityToCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantityToCredit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ConsignedQuantity.
        /// </summary>
        public static string ConsignedQuantityCount
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantityCount", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MeasurementID.
        /// </summary>
        public static string MeasurementID
        {
            get
            {
                return ResourceRead.GetResourceValue("MeasurementID", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to LotNumber.
        /// </summary>
        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SerialNumber.
        /// </summary>
        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ExpirationDate.
        /// </summary>
        public static string ExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpirationDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ExpirationDate.
        /// </summary>
        public static string ReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedDate", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cost.
        /// </summary>
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMISensorPort.
        /// </summary>
        public static string eVMISensorPort
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorPort", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eVMISensorID.
        /// </summary>
        public static string eVMISensorID
        {
            get
            {
                return ResourceRead.GetResourceValue("eVMISensorID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to GUID.
        /// </summary>
        public static string GUID
        {
            get
            {
                return ResourceRead.GetResourceValue("GUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemGUID.
        /// </summary>
        public static string ItemGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemGUID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Created.
        /// </summary>
        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Updated.
        /// </summary>
        public static string Updated
        {
            get
            {
                return ResourceRead.GetResourceValue("Updated", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreatedBy.
        /// </summary>
        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LastUpdatedBy.
        /// </summary>
        public static string LastUpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdatedBy", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsDeleted.
        /// </summary>
        public static string IsDeleted
        {
            get
            {
                return ResourceRead.GetResourceValue("IsDeleted", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsArchived.
        /// </summary>
        public static string IsArchived
        {
            get
            {
                return ResourceRead.GetResourceValue("IsArchived", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string Room
        {
            get
            {
                return ResourceRead.GetResourceValue("Room", ResourceFileName);
            }
        }


        ///   Looks up a localized string similar to eTurns: Job Types.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ProjectSpend
        /// </summary>
        public static string ProjectSpend
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpend", ResourceFileName);
            }
        }

        public static string AndExpiration
        {
            get
            {
                return ResourceRead.GetResourceValue("AndExpiration", ResourceFileName);
            }
        }

        public static string TrackingItem
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingItem", ResourceFileName);
            }
        }
        public static string AvailableQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableQuantity", ResourceFileName);
            }
        }
        public static string QuantityToMove
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToMove", ResourceFileName);
            }
        }
    }
}


