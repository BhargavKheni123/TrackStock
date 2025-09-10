using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class MaterialStagingPullDetailDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //MaterialStagingID
        [Display(Name = "MaterialStagingID", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Guid> MaterialStagingGUID { get; set; }

        //MaterialStagingdtlID
        [Display(Name = "MaterialStagingdtlID", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Guid> MaterialStagingdtlGUID { get; set; }


        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid ItemGUID { get; set; }

        //ItemCost
        [Display(Name = "ItemCost", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ItemCost { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //PoolQuantity
        [Display(Name = "PoolQuantity", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PoolQuantity { get; set; }

        //SerialNumber
        [Display(Name = "SerialNumber", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LotNumber { get; set; }

        //Expiration
        [Display(Name = "Expiration", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Expiration { get; set; }

        //Received
        [Display(Name = "Received", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Received { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResMaterialStagingPullDetail))]
        public Nullable<System.Int64> BinID { get; set; }

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
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
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

        //PullCredit
        [Display(Name = "PullCredit", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String PullCredit { get; set; }

        //ItemLocationDetailID
        [Display(Name = "ItemLocationDetailID", ResourceType = typeof(ResMaterialStagingPullDetail))]
        public Nullable<System.Guid> ItemLocationDetailGUID { get; set; }

        //StagingBinId
        [Display(Name = "StagingBinId", ResourceType = typeof(ResMaterialStagingPullDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 StagingBinId { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }
        public double ActualAvailableQuantity { get; set; }

        //ItemLocationDetailID
        public Nullable<System.Guid> OrderDetailGUID { get; set; }

        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
        public bool Consignment { get; set; }
        public int ItemType { get; set; }
        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        public string BinNumber { get; set; }
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ItemNumber { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        public string PackSlipNumber { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public string StageBinName { get; set; }

        //Below UDFs used in Receive Module
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }



        //Below field is used for Workorder Credits
        public Nullable<Guid> WorkOrderGuid { get; set; }

        public string InventoryConsuptionMethod { get; set; }

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
        public bool IsOnlyFromItemUI { get; set; }
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

        public double? Cost { get; set; }
        public Nullable<System.Double> SellPrice { get; set; }

        public Guid? ProjectSpentGUID { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool IsOnlyFromUI { get; set; }

        public bool IsPDAEdit { get; set; }

        public bool IsWebEdit { get; set; }

        public string InsertedFrom { get; set; }

        public string PullOrderNumber { get; set; }

        public double InitialQuantity { get; set; }

        public double? InitialQuantityWeb { get; set; }
        public double? InitialQuantityPDA { get; set; }

        public bool IsConsignedSerialLot { get; set; }

        public Guid? CountLineItemDtlGUID { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Quantity { get; set; }

        public Nullable<Guid> TransferDetailGUID { get; set; }

        public Guid? PullGUIDForCreditHistory { get; set; }
        public Guid? PullDetailGUIDForCreditHistory { get; set; }
        public Guid? SupplierAccountGuid { get; set; }
    }


    public class ItemStagePullInfo
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public Guid ItemGUID { get; set; }
        public long ItemID { get; set; }
        public string ItemNumber { get; set; }
        public long BinID { get; set; }
        public string BinNumber { get; set; }

        public long StageBinID { get; set; }
        public string StageBinNumber { get; set; }

        public double PullQuantity { get; set; }
        public long LastUpdatedBy { get; set; }

        public Guid MSGUID { get; set; }
        public Guid MSDetailGUID { get; set; }
        public long CreatedBy { get; set; }
        public double ItemCost { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public List<ItemLocationLotSerialDTO> lstItemPullDetails { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }

    }

    public class ResMaterialStagingPullDetail
    {
        private static string ResourceFileName = "ResMaterialStagingPullDetail";

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
        ///   Looks up a localized string similar to MaterialStagingPullDetail {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to MaterialStagingPullDetail.
        /// </summary>
        public static string MaterialStagingPullDetailHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingPullDetailHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaterialStagingPullDetail.
        /// </summary>
        public static string MaterialStagingPullDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingPullDetail", ResourceFileName);
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
        ///   Looks up a localized string similar to MaterialStagingID.
        /// </summary>
        public static string MaterialStagingID
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaterialStagingdtlID.
        /// </summary>
        public static string MaterialStagingdtlID
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingdtlID", ResourceFileName);
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
        ///   Looks up a localized string similar to ItemCost.
        /// </summary>
        public static string ItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCost", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to PoolQuantity.
        /// </summary>
        public static string PoolQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PoolQuantity", ResourceFileName);
            }
        }

        //CustomerOwnedQuantityAvail
        public static string CustomerOwnedQuantityAvail
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityAvail", ResourceFileName);
            }
        }

        public static string StageQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("StageQuantity", ResourceFileName);
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
        ///   Looks up a localized string similar to Expiration.
        /// </summary>
        public static string Expiration
        {
            get
            {
                return ResourceRead.GetResourceValue("Expiration", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Received.
        /// </summary>
        public static string Received
        {
            get
            {
                return ResourceRead.GetResourceValue("Received", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to PullCredit.
        /// </summary>
        public static string PullCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("PullCredit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationDetailID.
        /// </summary>
        public static string ItemLocationDetailID
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationDetailID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StagingBinId.
        /// </summary>
        public static string StagingBinId
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingBinId", ResourceFileName);
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
        public static string StageBin
        {
            get
            {
                return ResourceRead.GetResourceValue("StageBin", ResourceFileName);
            }
        }
    }
}


