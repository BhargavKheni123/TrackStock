using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{

    public class KitMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //KitPartNumber
        [Display(Name = "KitPartNumber", ResourceType = typeof(ResKitMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String KitPartNumber { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResKitMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Description { get; set; }

        //ReOrderType
        [Display(Name = "ReOrderType", ResourceType = typeof(ResKitMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<Boolean> ReOrderType { get; set; }

        //KitCategory
        [Display(Name = "KitCategory", ResourceType = typeof(ResKitMaster))]
        public Nullable<System.Int32> KitCategory { get; set; }

        //AvailableKitQuantity
        [Display(Name = "AvailableKitQuantity", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AvailableKitQuantity { get; set; }

        //AvailableWIPKit
        [Display(Name = "AvailableWIPKit", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AvailableWIPKit { get; set; }

        //KitDemand
        [Display(Name = "KitDemand", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> KitDemand { get; set; }

        //KitCost
        [Display(Name = "KitCost", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Nullable<System.Double> KitCost { get; set; }

        //WIPKitCost
        public Nullable<System.Double> WIPKitCost { get; set; }


        //KitSellPrice
        [Display(Name = "KitSellPrice", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> KitSellPrice { get; set; }

        //MinimumKitQuantity
        [Display(Name = "MinimumKitQuantity", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [CheckMinimumWithMaximum("MaximumKitQuantity", ErrorMessage = "Minimum quantity must be less then Maximum quantity")]
        public Nullable<System.Double> MinimumKitQuantity { get; set; }

        //MaximumKitQuantity
        [Display(Name = "MaximumKitQuantity", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MaximumKitQuantity { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResKitMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResKitMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResKitMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResKitMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResKitMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyID { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "AvailableInGeneralInventory", ResourceType = typeof(ResKitMaster))]
        public double AvailableInGeneralInventory { get; set; }

        [Display(Name = "NoOfItemsInKit", ResourceType = typeof(ResKitMaster))]
        public int NoOfItemsInKit { get; set; }

        public List<KitDetailDTO> KitItemList { get; set; }

        public bool IsKitBuildAction { get; set; }
        public bool IsKitBreakAction { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public bool IsHistory { get; set; }

        /// <summary>
        /// Set True if record is able to delete.
        /// </summary>
        public bool IsNotAbleToDelete { get; set; }

        public double? QuantityToBuildBreak { get; set; }


        //MaximumKitQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [CheckCriticleWithMin("MinimumKitQuantity", ErrorMessage = "Critical quantity must be less then minimum quantity.")]
        public Nullable<System.Double> CriticalQuantity { get; set; }
        public string AppendedBarcodeString { get; set; }


        [Display(Name = "Consignment", ResourceType = typeof(ResItemMaster))]
        public Boolean Consignment { get; set; }

        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        [Display(Name = "LotNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean LotNumberTracking { get; set; }

        [Display(Name = "DateCodeTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean DateCodeTracking { get; set; }

        [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
        public System.Int32 ItemType { get; set; }

        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        public long? DefaultLocation { get; set; }


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

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public int TotalRecords { get; set; }

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
        public string ReceivedOnWebDate
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
        
        public List<BinMasterDTO> KitBins { get; set; }
    }

    public class RPT_KitHeader
    {
        public Int64 ID { get; set; }
        public string KitPartNumber { get; set; }
        public Guid KitGuid { get; set; }
    }

    public class RPT_KitSerialHeader
    {
        public Int64 ID { get; set; }
        public string KitPartNumber { get; set; }
        public Guid KitGuid { get; set; }
        public string strKitGuid { get; set; }
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public string ExpirationDate { get; set; }
    }

    public class ResKitMaster
    {
        private static string ResourceFileName = "ResKitMaster";

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
        ///   Looks up a localized string similar to KitMaster {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to KitMaster.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to KitMaster.
        /// </summary>
        public static string KitMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("KitMaster", ResourceFileName);
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
        ///   Looks up a localized string similar to KitPartNumber.
        /// </summary>
        public static string KitPartNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("KitPartNumber", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReOrderType.
        /// </summary>
        public static string ReOrderType
        {
            get
            {
                return ResourceRead.GetResourceValue("ReOrderType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to KitCategory.
        /// </summary>
        public static string KitCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("KitCategory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AvailableKitQuantity.
        /// </summary>
        public static string AvailableKitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableKitQuentity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AvailableWIPKit.
        /// </summary>
        public static string AvailableWIPKit
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableWIPKit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to KitDemand.
        /// </summary>
        public static string KitDemand
        {
            get
            {
                return ResourceRead.GetResourceValue("KitDemand", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to KitCost.
        /// </summary>
        public static string KitCost
        {
            get
            {
                return ResourceRead.GetResourceValue("KitCost", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to KitSellPrice.
        /// </summary>
        public static string KitSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("KitSellPrice", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MinimumKitQuantity.
        /// </summary>
        public static string MinimumKitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MinimumKitQuentity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaximumKitQuantity.
        /// </summary>
        public static string MaximumKitQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MaximumKitQuentity", ResourceFileName);
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

        public static string AvailableInGeneralInventory
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableInGeneralInventory", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityPerKit.
        /// </summary>
        public static string QuantityPerKit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityPerKit", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityReadyForAssembly.
        /// </summary>
        public static string QuantityReadyForAssembly
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityReadyForAssembly", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AvailableItemsInWIP.
        /// </summary>
        public static string AvailableItemsInWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableItemsInWIP", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to QuantityNeededToMeetDemand.
        /// </summary>
        public static string QuantityNeededToMeetDemand
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityNeededToMeetDemand", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to QuantityToMove.
        /// </summary>
        public static string QuantityToMove
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToMove", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to MoveIn.
        /// </summary>
        public static string MoveIn
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveIn", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WrittenOff.
        /// </summary>
        public static string WrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOff", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to MoveOut.
        /// </summary>
        public static string MoveOut
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveOut", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NoOfItemsInKit.
        /// </summary>
        public static string NoOfItemsInKit
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfItemsInKit", ResourceFileName);
            }
        }

        public static string QuantityToBuildKit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuentityToBuildKit", ResourceFileName);
            }
        }

        public static string CriticalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CriticalQuantity", ResourceFileName);
            }
        }

        public static string QtyToMeetDemand
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyToMeetDemand", ResourceFileName);
            }
        }
        public static string IsBuildBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBuildBreak", ResourceFileName);
            }
        }

        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", ResourceFileName);
            }
        }

        public static string KitPartsInWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("KitPartsInWIP", ResourceFileName);
            }
        }


        public static string MsgAddKitComponentToKit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAddKitComponentToKit", ResourceFileName);
            }
        }
        public static string NotAbleToEditItem
        {
            get
            {
                return ResourceRead.GetResourceValue("NotAbleToEditItem", ResourceFileName);
            }
        }
        public static string MoveOutQtyLessThanEqualsWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveOutQtyLessThanEqualsWIP", ResourceFileName);
            }
        }
        public static string EnterQtyToBuildKit
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterQtyToBuildKit", ResourceFileName);
            }
        }
        public static string EnterQtyToBreakKit
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterQtyToBreakKit", ResourceFileName);
            }
        }
        public static string SelectItemsForBulkMove
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectItemsForBulkMove", ResourceFileName);
            }
        }
        public static string ItemHasNotEnoughQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemHasNotEnoughQty", ResourceFileName);
            }
        }
        public static string ItemHasNotEnoughQtyToMoveOut
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemHasNotEnoughQtyToMoveOut", ResourceFileName);
            }
        }
        public static string ItemIsSerialTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemIsSerialTracking", ResourceFileName);
            }
        }
        public static string ItemIsLotTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemIsLotTracking", ResourceFileName);
            }
        }
        public static string ItemIsDateCodeTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemIsDateCodeTracking", ResourceFileName);
            }
        }
        public static string LotNumberCantBeEmpty
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumberCantBeEmpty", ResourceFileName);
            }
        }
        public static string EnoughMoveInQtyNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("EnoughMoveInQtyNotAvailable", ResourceFileName);
            }
        }
        public static string SerialNumberCantBeEmpty
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumberCantBeEmpty", ResourceFileName);
            }
        }
        public static string SerialAlreadyExist
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialAlreadyExist", ResourceFileName);
            }
        }
        public static string AddToolKitComponentToToolKit
        {
            get
            {
                return ResourceRead.GetResourceValue("AddToolKitComponentToToolKit", ResourceFileName);
            }
        }
        public static string AddToolKitComponentToToolKitFromKit
        {
            get
            {
                return ResourceRead.GetResourceValue("AddToolKitComponentToToolKitFromKit", ResourceFileName);
            }
        }
        public static string ToolQtyUpdatedSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolQtyUpdatedSuccessfully", ResourceFileName);
            }
        }
        public static string AddKitComponentRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("AddKitComponentRequired", ResourceFileName);
            }
        }
        

        public static string MsgAtleastOneKitRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAtleastOneKitRequired", ResourceFileName);
            }
        }
        public static string selectexpiredate
        {
            get
            {
                return ResourceRead.GetResourceValue("selectexpiredate", ResourceFileName);
            }
        }
        public static string EnterLotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterLotNumber", ResourceFileName);
            }
        }
        public static string BuildQtyLessThanEqualsWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("BuildQtyLessThanEqualsWIP", ResourceFileName);
            }
        }
        public static string EnterQtyToMove
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterQtyToMove", ResourceFileName);
            }
        }
        public static string SelectBin
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectBin", ResourceFileName);
            }
        }
        public static string MoveOutSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveOutSuccessfully", ResourceFileName);
            }
        }
        public static string MoveInSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveInSuccessfully", ResourceFileName);
            }
        }
        public static string QtyNotAvailableMoveOut
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyNotAvailableMoveOut", ResourceFileName);
            }
        }
        public static string ReqQuantityToSave
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantityToSave", ResourceFileName);
            }
        }

        public static string QuantityBreak { get { return ResourceRead.GetResourceValue("QuantityBreak", ResourceFileName); } }
        public static string MsgQuantityGreaterZero
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuantityGreaterZero", ResourceFileName);
            }
        }
        public static string MsgQuantityAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuantityAvailable", ResourceFileName);
            }
        }

        public static string ItemsHasNotEnoughQuantityToMoveIn { get { return ResourceRead.GetResourceValue("ItemsHasNotEnoughQuantityToMoveIn", ResourceFileName); } }
        public static string QuantityMovedIn { get { return ResourceRead.GetResourceValue("QuantityMovedIn", ResourceFileName); } }
        public static string btnBuild { get { return ResourceRead.GetResourceValue("btnBuild", ResourceFileName); } }
        public static string btnBreak { get { return ResourceRead.GetResourceValue("btnBreak", ResourceFileName); } }
        public static string btnMoveInBulk { get { return ResourceRead.GetResourceValue("btnMoveInBulk", ResourceFileName); } }
        public static string btnMoveOutBulk { get { return ResourceRead.GetResourceValue("btnMoveOutBulk", ResourceFileName); } }
        public static string Bin { get { return ResourceRead.GetResourceValue("Bin", ResourceFileName); } }
        public static string EnterQuantity { get { return ResourceRead.GetResourceValue("EnterQuantity", ResourceFileName); } }
        public static string WIPCost { get { return ResourceRead.GetResourceValue("WIPCost", ResourceFileName); } }
        public static string QtyMoveinKitItems { get { return ResourceRead.GetResourceValue("QtyMoveinKitItems", ResourceFileName); } }
        public static string MsgSelectRowToDelete { get { return ResourceRead.GetResourceValue("MsgSelectRowToDelete", ResourceFileName); } }
        public static string TitleMoveitemquantity { get { return ResourceRead.GetResourceValue("TitleMoveitemquantity", ResourceFileName); } }
        public static string TitleAdditemquantity { get { return ResourceRead.GetResourceValue("TitleAdditemquantity", ResourceFileName); } }
        public static string QuantityPerKitRequired { get { return ResourceRead.GetResourceValue("QuantityPerKitRequired", ResourceFileName); } }
        public static string MsgMoveInOutQuantity { get { return ResourceRead.GetResourceValue("MsgMoveInOutQuantity", ResourceFileName); } }
    }



}


