using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class PullMasterDTO : ItemMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public new System.Int64 ID { get; set; }


        //ProjectID
        [Display(Name = "ProjectID", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Guid> ProjectSpendGUID { get; set; }
        public string ProjectSpendName { get; set; }

        [Display(Name = "ProjectName", ResourceType = typeof(ResProjectMaster))]
        public System.String ProjectName { get; set; }

        //UOI
        [Display(Name = "UOI", ResourceType = typeof(ResPullMaster))]
        [StringLength(256, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UOI { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public new Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public new Nullable<System.Double> ConsignedQuantity { get; set; }

        //PoolQuantity
        [Display(Name = "PoolQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PoolQuantity { get; set; }


        public Nullable<System.Double> PULLCost { get; set; }

        public Nullable<System.Double> PULLPrice { get; set; }

        //CreditCustomerOwnedQuantity
        [Display(Name = "CreditCustomerOwnedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CreditCustomerOwnedQuantity { get; set; }

        //CreditConsignedQuantity
        [Display(Name = "CreditConsignedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CreditConsignedQuantity { get; set; }

        //SerialNumber
        [Display(Name = "SerialNumber", ResourceType = typeof(ResPullMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResPullMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LotNumber { get; set; }

        //DateCode
        [Display(Name = "DateCode", ResourceType = typeof(ResPullMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String DateCode { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResPullMaster))]
        public new Nullable<System.Int64> BinID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF5 { get; set; }

        //GUID
        public new Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResPullMaster))]
        public Nullable<Guid> ItemGUID { get; set; }

        //Created
        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.Int64> LastUpdatedBy { get; set; }

        //IsDeleted
        public new Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public new Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.Int64> CompanyID { get; set; }

        //Room
        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public new Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public new string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public new string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public new string UpdatedByName { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public new string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public new Int64 HistoryID { get; set; }

        //[Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string ActionType { get; set; }
        public new System.String WhatWhereAction { get; set; }
        public long? ItemID { get; set; }
        public IEnumerable<ItemLocationDetailsDTO> lstItemLocationDetails { get; set; }
        public string InventoryConsuptionMethod { get; set; }

        [Display(Name = "PullOrderNumber", ResourceType = typeof(ResPullMaster))]
        public string PullOrderNumber { get; set; }
        //Get set of whole item 
        //public ItemMasterDTO PullItem { get; set; }


        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public override Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public override Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public new System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public new System.String EditedFrom { get; set; }

        private string _ReceivedOn;
        public new string ReceivedOnDate
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
        public new string ReceivedOnDateWeb
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

        private string _createdDate;
        private string _updatedDate;
        public new string CreatedDate
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

        public new string UpdatedDate
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
        public Nullable<Guid> WorkOrderDetailGUID { get; set; }
        public string ControlNumber { get; set; }

        public Nullable<Guid> RequisitionDetailGUID { get; set; }
        public new System.String BinNumber { get; set; }
        public Guid? MaterialStagingGUID { get; set; }

        public Nullable<Guid> SupplierAccountGuid { get; set; }

        #region Tools Properties Used in new Pull Popup

        public Nullable<System.Guid> ToolGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Nullable<System.Guid> TechnicianGUID { get; set; }

        public string ToolName { get; set; }
        public string Technician { get; set; }

        #endregion

        public Nullable<System.Double> PullMasterItemCost { get; set; }
        public Nullable<System.Double> ItemSellPrice { get; set; }        
        public Nullable<System.Double> ItemAverageCost { get; set; }
        public Nullable<System.Double> ItemMarkup { get; set; }
        public Nullable<System.Int32> ItemCostUOMValue { get; set; }
        public string MaterialStagingHeaderName { get; set; }

        public Nullable<bool> isPullUDF1Deleted { get; set; }
        public Nullable<bool> isPullUDF2Deleted { get; set; }
        public Nullable<bool> isPullUDF3Deleted { get; set; }
        public Nullable<bool> isPullUDF4Deleted { get; set; }
        public Nullable<bool> isPullUDF5Deleted { get; set; }

        public Nullable<bool> isToolUDF1Deleted { get; set; }
        public Nullable<bool> isToolUDF2Deleted { get; set; }
        public Nullable<bool> isToolUDF3Deleted { get; set; }
        public Nullable<bool> isToolUDF4Deleted { get; set; }
        public Nullable<bool> isToolUDF5Deleted { get; set; }
        public System.Int64 PullType { get; set; }
    }

    public class ResPullMaster
    {
        private static string ResourceFileName = "ResPullMaster";

        public static string PullOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("PullOrderNumber", ResourceFileName);
            }
        }
        public static string PullUDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUDF1", ResourceFileName);
            }
        }
        public static string PullUDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUDF2", ResourceFileName);
            }
        }
        public static string PullUDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUDF3", ResourceFileName);
            }
        }
        public static string PullUDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUDF4", ResourceFileName);
            }
        }
        public static string PullUDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("PullUDF5", ResourceFileName);
            }
        }
        public static string PullCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("PullCredit", ResourceFileName);
            }
        }

        public static string ResultingItemOnHandQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ResultingItemOnHandQty", ResourceFileName);
            }
        }
        public static string ResultingItemLocOnHandQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ResultingItemLocOnHandQty", ResourceFileName);
            }
        }
        public static string PulledPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("PulledPrice", ResourceFileName);
            }
        }

        public static string ResultingItemLocStageQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ResultingItemLocStageQty", ResourceFileName);
            }
        }
        public static string ResultingItemStageQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ResultingItemStageQty", ResourceFileName);
            }
        }
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
        ///   Looks up a localized string similar to PullMaster {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to PullMaster.
        /// </summary>
        public static string PullMasterHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to PullMaster.
        /// </summary>
        public static string PullMaster
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMaster", ResourceFileName);
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
        ///   Looks up a localized string similar to ProjectID.
        /// </summary>
        public static string ProjectID
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UOI.
        /// </summary>
        public static string UOI
        {
            get
            {
                return ResourceRead.GetResourceValue("UOI", ResourceFileName);
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
        public static string CustomerOwnedQuantityAvail
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityAvail", ResourceFileName);
            }
        }
        public static string QuantityAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityAvailable", ResourceFileName);
            }
        }
        public static string QuantityToBePulled
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityToBePulled", ResourceFileName);
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
        public static string ConsignedQuantityAvail
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantityAvail", ResourceFileName);
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

        /// <summary>
        ///   Looks up a localized string similar to CreditCustomerOwnedQuantity.
        /// </summary>
        public static string CreditCustomerOwnedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditCustomerOwnedQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreditConsignedQuantity.
        /// </summary>
        public static string CreditConsignedQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditConsignedQuantity", ResourceFileName);
            }
        }

        public static string PulledQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PulledQuantity", ResourceFileName);
            }
        }

        public static string PulledCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PulledCost", ResourceFileName);
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
        public static string LotSrnumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotSrnumber", ResourceFileName);
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
        ///   Looks up a localized string similar to DateCode.
        /// </summary>
        public static string DateCode
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCode", ResourceFileName);
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
        ///   Looks up a localized string similar to Turns.
        /// </summary>
        public static string Turns
        {
            get
            {
                return ResourceRead.GetResourceValue("Turns", ResourceFileName);
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
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", ResourceFileName);
            }
        }
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", ResourceFileName);
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
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string ConsignmentItemPulledColorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignmentItemPulledColorDescription", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string ConsignmentItemEDISentColorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignmentItemEDISentColorDescription", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Room.
        /// </summary>
        public static string ConsignmentItemBillColorDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignmentItemBillColorDescription", ResourceFileName);
            }
        }
        public static string msgQuantityNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("msgQuantityNotAvailable", ResourceFileName);
            }
        }
        public static string msgProjectSpendLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProjectSpendLimit", ResourceFileName);
            }
        }
        public static string msgProjectSpendItemQtyLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProjectSpendItemQtyLimit", ResourceFileName);
            }
        }
        public static string msgProjectSpendItemamountLimit
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProjectSpendItemamountLimit", ResourceFileName);
            }
        }
        public static string msgReqPullGreaterApproved
        {
            get
            {
                return ResourceRead.GetResourceValue("msgReqPullGreaterApproved", ResourceFileName);
            }
        }
        public static string msgInvalidLot
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInvalidLot", ResourceFileName);
            }
        }
        public static string msgInvalidQuantityLot
        {
            get
            {
                return ResourceRead.GetResourceValue("msgInvalidQuantityLot", ResourceFileName);
            }
        }

        public static string Billing
        {
            get
            {
                return ResourceRead.GetResourceValue("Billing", ResourceFileName);
            }
        }

        public static string PullAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("PullAllButton", ResourceFileName);
            }
        }

        public static string CustomerOwnedQuantityPull
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityPull", ResourceFileName);
            }
        }

        public static string ConsignedQuantityPull
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantityPull", ResourceFileName);
            }
        }

        public static string CustomerOwnedQuantityCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("CustomerOwnedQuantityCredit", ResourceFileName);
            }
        }

        public static string ConsignedQuantityCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ConsignedQuantityCredit", ResourceFileName);
            }
        }

        //
        public static string PullLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("PullLocation", ResourceFileName);
            }
        }

        //
        public static string ItemBlanketPO
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemBlanketPO", ResourceFileName);
            }
        }

        public static string SupplierAccountNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SupplierAccountNumber", ResourceFileName);
            }
        }
        public static string PullMasterItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterItemCost", ResourceFileName);
            }
        }

        public static string PullMasterItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterItemSellPrice", ResourceFileName);
            }
        }

        public static string PullMasterItemAverageCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterItemAverageCost", ResourceFileName);
            }
        }

        public static string PullMasterItemMarkup
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterItemMarkup", ResourceFileName);
            }
        }

        public static string PullMasterItemCostUOMValue
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMasterItemCostUOMValue", ResourceFileName);
            }
        }


        public static string QtyToPull
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyToPull", ResourceFileName);
            }
        }

        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", ResourceFileName);
            }
        }


        public static string PullItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PullItemCost", ResourceFileName);
            }
        }


        public static string PullItemSellPrice
        {
            get
            {
                return ResourceRead.GetResourceValue("PullItemSellPrice", ResourceFileName);
            }
        }


        public static string PullMarkup
        {
            get
            {
                return ResourceRead.GetResourceValue("PullMarkup", ResourceFileName);
            }
        }


        public static string ItemCostOnPullDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCostOnPullDate", ResourceFileName);
            }
        }

        public static string PulledItemCost
        {
            get
            {
                return ResourceRead.GetResourceValue("PulledItemCost", ResourceFileName);
            }
        }

        public static string NoProjectspendOntheFlyRight
        {
            get
            {
                return ResourceRead.GetResourceValue("NoProjectspendOntheFlyRight", ResourceFileName);
            }
        }
        public static string IsCustomerEDISent
        {
            get
            {
                return ResourceRead.GetResourceValue("IsCustomerEDISent", ResourceFileName);
            }
        }
        public static string CannotPullSerialLotItem
        {
            get
            {
                return ResourceRead.GetResourceValue("CannotPullSerialLotItem", ResourceFileName);
            }
        }
        public static string DuplicateSerialNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("DuplicateSerialNotAllowed", ResourceFileName);
            }
        }
        public static string NoRightsToPullConsignItem
        {
            get
            {
                return ResourceRead.GetResourceValue("NoRightsToPullConsignItem", ResourceFileName);
            }
        }
        public static string SerialItemQtyMustBeOne
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialItemQtyMustBeOne", ResourceFileName);
            }
        }
        public static string CreditQtyGreaterThanTotalPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditQtyGreaterThanTotalPullQty", ResourceFileName);
            }
        }
        public static string MsgSaveCreditAjaxError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSaveCreditAjaxError", ResourceFileName);
            }
        }

        public static string MsgPreCreditInfoAjaxError
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPreCreditInfoAjaxError", ResourceFileName);
            }
        }

        public static string MsgQuantityToCreditValid
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuantityToCreditValid", ResourceFileName);
            }
        }
        public static string AllPulldon
        {
            get
            {
                return ResourceRead.GetResourceValue("AllPulldon", ResourceFileName);
            }
        }
        public static string MsgPullQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgReorderQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgReorderQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgBillingDoneDeleteValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgBillingDoneDeleteValidation", ResourceFileName);
            }
        }
        public static string MsgSelectLocationPullCreditValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectLocationPullCreditValidation", ResourceFileName);
            }
        }
        public static string MsgQuantityBlankZeroValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuantityBlankZeroValidation", ResourceFileName);
            }
        }
        public static string MsgNewOldQuantitySameValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNewOldQuantitySameValidation", ResourceFileName);
            }
        }
        public static string MsgUpdatePullQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUpdatePullQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgUpdateCreditQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUpdateCreditQuantityValidation", ResourceFileName);
            }
        }
        public static string MsgUpdatePullQuantityConfirmation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgUpdatePullQuantityConfirmation", ResourceFileName);
            }
        }

        public static string reqQtyToPull
        {
            get
            {
                return ResourceRead.GetResourceValue("reqQtyToPull", ResourceFileName);
            }
        }
        public static string MsgProjectSpendMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProjectSpendMandatory", ResourceFileName);
            }
        }
        public static string MsgLabourItemRequiredHours
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLabourItemRequiredHours", ResourceFileName);
            }
        }
        public static string InventoryLocationMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryLocationMandatory", ResourceFileName);
            }
        }
        public static string MsgCreditBackProjectSpendValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditBackProjectSpendValidation", ResourceFileName);
            }
        }
        public static string MsgMSCreditBackProjectSpendValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMSCreditBackProjectSpendValidation", ResourceFileName);
            }
        }
        public static string SomeItemNotPulled
        {
            get
            {
                return ResourceRead.GetResourceValue("SomeItemNotPulled", ResourceFileName);
            }
        }
        public static string QtyNotAvailableforItemLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyNotAvailableforItemLocation", ResourceFileName);
            }
        }
       
        public static string MsgQtyToPullMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyToPullMandatory", ResourceFileName);
            }
        }
        public static string MsgRecordPartialPullNotDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordPartialPullNotDelete", ResourceFileName);
            }
        }

        public static string MsgRecordNotDeletedPartialPull
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordNotDeletedPartialPull", ResourceFileName);
            }
        }
        public static string MsgMaxAvailableQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMaxAvailableQuantity", ResourceFileName);
            }
        }
        public static string MsgInvalidPullValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidPullValue", ResourceFileName);
            }
        }
        public static string ErrorPullofQL
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorPullofQL", ResourceFileName);
            }
        }
        public static string CreditTransactionProgress
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditTransactionProgress", ResourceFileName);
            }
        }
        public static string msgProjectSpendLimitReached
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProjectSpendLimitReached", ResourceFileName);
            }
        }
        public static string msgProjectSpendLimitConfirmation
        {
            get
            {
                return ResourceRead.GetResourceValue("msgProjectSpendLimitConfirmation", ResourceFileName);
            }
        }
        public static string MsgPullDoneSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullDoneSuccess", ResourceFileName);
            }
        }
        public static string MsgProjectSpendMandatoryatRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgProjectSpendMandatoryatRoom", ResourceFileName);
            }
        }
        public static string MSCreditTransactionProgress
        {
            get
            {
                return ResourceRead.GetResourceValue("MSCreditTransactionProgress", ResourceFileName);
            }
        }
        public static string NoPullRightsInPull
        {
            get
            {
                return ResourceRead.GetResourceValue("NoPullRightsInPull", ResourceFileName);
            }
        }
        public static string ReqRowtoCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRowtoCredit", ResourceFileName);
            }
        }
        public static string ReqProperCreditQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqProperCreditQty", ResourceFileName);
            }
        }
        public static string MsgDuplicateLotNumberExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateLotNumberExpirationDate", ResourceFileName);
            }
        }
        public static string MsgDuplicateExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateExpirationDate", ResourceFileName);
            }
        }
        public static string MsgCreditTransactionDone
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditTransactionDone", ResourceFileName);
            }
        }
        public static string MsgCreditTransactionForSerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditTransactionForSerialNumber", ResourceFileName);
            }
        }
        public static string MsgEnterExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterExpirationDate", ResourceFileName);
            }
        }
        public static string MsgSelectRowToDelete
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRowToDelete", ResourceFileName);
            }
        }
        public static string MsgPullExpiredItemList
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullExpiredItemList", ResourceFileName);
            }
        }
        public static string MsgEnterPullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterPullQuantity", ResourceFileName);
            }
        }
        public static string MsgAlertValidated
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAlertValidated", ResourceFileName);
            }
        }
        public static string MsgQuickListPullReason
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQuickListPullReason", ResourceFileName);
            }
        }
        public static string MsgLotNumberExpDateValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgLotNumberExpDateValidation", ResourceFileName);
            }
        }
        public static string MsgAvailableQtyText
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAvailableQtyText", ResourceFileName);
            }
        }
        public static string MsgPullQuantitySelectedLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullQuantitySelectedLocation", ResourceFileName);
            }
        }
        public static string MsgNoLocationToAdd
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoLocationToAdd", ResourceFileName);
            }
        }
        public static string MsgCreditMoreAvailableQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditMoreAvailableQTY", ResourceFileName);
            }
        }
        public static string MsgCreditMoreAvailablePullQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditMoreAvailablePullQTY", ResourceFileName);
            }
        }

        public static string LocationPullQtyMustBeDefaultPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("LocationPullQtyMustBeDefaultPullQty", ResourceFileName);
            }
        }
        public static string PullQtyMustBeDefaultPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQtyMustBeDefaultPullQty", ResourceFileName);
            }
        }
        public static string PullQtyGreaterThanApproveQty
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQtyGreaterThanApproveQty", ResourceFileName);
            }
        }
        public static string Staging
        {
            get
            {
                return ResourceRead.GetResourceValue("Staging", ResourceFileName);
            }
        }
        public static string IsAreRequired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAreRequired", ResourceFileName);
            }
        }
        public static string ValueRequiredForUDF
        {
            get
            {
                return ResourceRead.GetResourceValue("ValueRequiredForUDF", ResourceFileName);
            }
        }
        public static string ItemDontHavePullToCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemDontHavePullToCredit", ResourceFileName);
            }
        }
        public static string ProjectSpendMandatorySelectIt
        {
            get
            {
                return ResourceRead.GetResourceValue("ProjectSpendMandatorySelectIt", ResourceFileName);
            }
        }
        public static string CreditTransactionDoneForSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditTransactionDoneForSerial", ResourceFileName);
            }
        }
        public static string ItemCredited
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemCredited", ResourceFileName);
            }
        }
        public static string ItemMSCredited
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemMSCredited", ResourceFileName);
            }
        }
        public static string DeletedSoPullUpdateNotAllowed
        {
            get
            {
                return ResourceRead.GetResourceValue("DeletedSoPullUpdateNotAllowed", ResourceFileName);
            }
        }
        public static string CreditQtyGreaterThanPreviousPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditQtyGreaterThanPreviousPullQty", ResourceFileName);
            }
        }
        public static string PullInfoNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("PullInfoNotAvailable", ResourceFileName);
            }
        }
        public static string ItemForExpirationDateExpired
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemForExpirationDateExpired", ResourceFileName);
            }
        }
        public static string IsExpired
        {
            get
            {
                return ResourceRead.GetResourceValue("IsExpired", ResourceFileName);
            }
        }

        public static string CantEditPullForCreditedSerials
        {
            get
            {
                return ResourceRead.GetResourceValue("CantEditPullForCreditedSerials", ResourceFileName);
            }
        }
        public static string EnterValidExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterValidExpirationDate", ResourceFileName);
            }
        }
        public static string CantEditCreditForCreditedSerials
        {
            get
            {
                return ResourceRead.GetResourceValue("CantEditCreditForCreditedSerials", ResourceFileName);
            }
        }        
        
        public static string MsgCreditQtyLessThanPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditQtyLessThanPullQty", ResourceFileName);
            }
        }
        public static string MsgCreditQtyMoreThanPreviousPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCreditQtyMoreThanPreviousPullQty", ResourceFileName);
            }
        }
        public static string ReqQuantitytoCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantitytoCredit", ResourceFileName);
            }
        }
        public static string MsgAllQuantityCredited
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllQuantityCredited", ResourceFileName);
            }
        }
        public static string MsgQtyMoreThanQtyToCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyMoreThanQtyToCredit", ResourceFileName);
            }
        }
        public static string MsgQtyLessThanQtyToCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyLessThanQtyToCredit", ResourceFileName);
            }
        }
        public static string ReqRowtoMSCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqRowtoMSCredit", ResourceFileName);
            }
        }
        public static string MsgMSCreditQtyMoreThanPreviousMSPullQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgMSCreditQtyMoreThanPreviousMSPullQty", ResourceFileName);
            }
        }
        public static string ReqQuantitytoMSCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqQuantitytoMSCredit", ResourceFileName);
            }
        }
        public static string MsgQtyMoreThanQtyToMSCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyMoreThanQtyToMSCredit", ResourceFileName);
            }
        }
        public static string MsgQtyLessThanQtyToMSCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyLessThanQtyToMSCredit", ResourceFileName);
            }
        }
        public static string MsgAllMSQuantityCredited
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllMSQuantityCredited", ResourceFileName);
            }
        }
        public static string CompletePulls { get { return ResourceRead.GetResourceValue("CompletePulls", ResourceFileName); } }
        public static string CreditMS { get { return ResourceRead.GetResourceValue("CreditMS", ResourceFileName); } }
        public static string MsgEnterPullOrderNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterPullOrderNumber", ResourceFileName);
            }
        }
        public static string MsgPullClosedValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullClosedValidation", ResourceFileName);
            }
        }
        public static string MsgExpirationLotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgExpirationLotAvailable", ResourceFileName);
            }
        }

        public static string NotEnoughQuantityForLocation { get { return ResourceRead.GetResourceValue("NotEnoughQuantityForLocation", ResourceFileName); } }
        public static string EnterProperValue { get { return ResourceRead.GetResourceValue("EnterProperValue", ResourceFileName); } }
        public static string NotEnoughCustAndConsQtyForLocation { get { return ResourceRead.GetResourceValue("NotEnoughCustAndConsQtyForLocation", ResourceFileName); } }
        public static string NotEnoughCustomerQuantityForLocation { get { return ResourceRead.GetResourceValue("NotEnoughCustomerQuantityForLocation", ResourceFileName); } }
        public static string ItemBinDoesnotHaveQtyToPullForSerialNo { get { return ResourceRead.GetResourceValue("ItemBinDoesnotHaveQtyToPullForSerialNo", ResourceFileName); } }
        public static string ItemBinDoesnotHaveQtyToPullForLotNo { get { return ResourceRead.GetResourceValue("ItemBinDoesnotHaveQtyToPullForLotNo", ResourceFileName); } }
        public static string ItemBinDoesnotHaveQtyToPullForExpiredDate { get { return ResourceRead.GetResourceValue("ItemBinDoesnotHaveQtyToPullForExpiredDate", ResourceFileName); } }
        public static string DataNotInsertSuccessfully { get { return ResourceRead.GetResourceValue("DataNotInsertSuccessfully", ResourceFileName); } }
        public static string ItemDoesNotHaveSufficientQtyToPull { get { return ResourceRead.GetResourceValue("ItemDoesNotHaveSufficientQtyToPull", ResourceFileName); } }
        public static string ItemBinDoesNotHaveAnyQtyToPull { get { return ResourceRead.GetResourceValue("ItemBinDoesNotHaveAnyQtyToPull", ResourceFileName); } }
        public static string ItemBinDoesNotHaveSufficientQtyToPull { get { return ResourceRead.GetResourceValue("ItemBinDoesNotHaveSufficientQtyToPull", ResourceFileName); } }
        public static string RecordNotAvailable { get { return ResourceRead.GetResourceValue("RecordNotAvailable", ResourceFileName); } }
        public static string CreditTransactionForLotAndExpDateAvailable { get { return ResourceRead.GetResourceValue("CreditTransactionForLotAndExpDateAvailable", ResourceFileName); } }
        public static string MsgDataToPullValidation { get { return ResourceRead.GetResourceValue("MsgDataToPullValidation", ResourceFileName); } }

        public static string MsgItemToPullValidation { get { return ResourceRead.GetResourceValue("MsgItemToPullValidation", ResourceFileName); } }

        public static string MsgAcceptLicensePullValidation { get { return ResourceRead.GetResourceValue("MsgAcceptLicensePullValidation", ResourceFileName); } }

        public static string MsgNoRightsPullValidation { get { return ResourceRead.GetResourceValue("MsgNoRightsPullValidation", ResourceFileName); } }

        public static string MsgCorrectPullBinValidation { get { return ResourceRead.GetResourceValue("MsgCorrectPullBinValidation", ResourceFileName); } }
        public static string MsgProvidePullOrderNumber { get { return ResourceRead.GetResourceValue("MsgProvidePullOrderNumber", ResourceFileName); } }
        public static string MsgSupplierAccountValidation { get { return ResourceRead.GetResourceValue("MsgSupplierAccountValidation", ResourceFileName); } }
        public static string MsgPullGuidExistInList { get { return ResourceRead.GetResourceValue("MsgPullGuidExistInList", ResourceFileName); } }
        public static string MsgPullGuidExist { get { return ResourceRead.GetResourceValue("MsgPullGuidExist", ResourceFileName); } }
        public static string MsgProvideSerialNumber { get { return ResourceRead.GetResourceValue("MsgProvideSerialNumber", ResourceFileName); } }

        public static string MsgDoesNotRightsForPullRequisition { get { return ResourceRead.GetResourceValue("MsgDoesNotRightsForPullRequisition", ResourceFileName); } }
        public static string MsgUserNeedacceptLicenceAgreementForPull { get { return ResourceRead.GetResourceValue("MsgUserNeedacceptLicenceAgreementForPull", ResourceFileName); } }
        public static string MsgLotNumberValidation { get { return ResourceRead.GetResourceValue("MsgLotNumberValidation", ResourceFileName); } }
        public static string MsgExpirationDateValidation { get { return ResourceRead.GetResourceValue("MsgExpirationDateValidation", ResourceFileName); } }
        public static string MsgLotNumberQuantityValidation { get { return ResourceRead.GetResourceValue("MsgLotNumberQuantityValidation", ResourceFileName); } }
        public static string MsgExpirationDatePullQuantityValidation { get { return ResourceRead.GetResourceValue("MsgExpirationDatePullQuantityValidation", ResourceFileName); } }
        public static string MsgPullDetailsDateCodeTracking { get { return ResourceRead.GetResourceValue("MsgPullDetailsDateCodeTracking", ResourceFileName); } }
        public static string MsgPullQuantityNotBeZero { get { return ResourceRead.GetResourceValue("MsgPullQuantityNotBeZero", ResourceFileName); } }
        public static string MsgPullQuantityAvailableQuantity { get { return ResourceRead.GetResourceValue("MsgPullQuantityAvailableQuantity", ResourceFileName); } }
        public static string MsgSerialNumberPullQuantityValidation { get { return ResourceRead.GetResourceValue("MsgSerialNumberPullQuantityValidation", ResourceFileName); } }
        public static string MsgPullCreditSpecificSerialNumber { get { return ResourceRead.GetResourceValue("MsgPullCreditSpecificSerialNumber", ResourceFileName); } }
        public static string ReqCorrectProjectSpend { get { return ResourceRead.GetResourceValue("ReqCorrectProjectSpend", ResourceFileName); } }
        public static string Pull { get { return ResourceRead.GetResourceValue("Pull", ResourceFileName); } }
        public static string BtnPullAll { get { return ResourceRead.GetResourceValue("BtnPullAll", ResourceFileName); } }
        public static string PullDetails { get { return ResourceRead.GetResourceValue("PullDetails", ResourceFileName); } }
        public static string NewConsumePull { get { return ResourceRead.GetResourceValue("NewConsumePull", ResourceFileName); } }
        public static string QtytoMSCredit { get { return ResourceRead.GetResourceValue("QtytoMSCredit", ResourceFileName); } }
        public static string Credit { get { return ResourceRead.GetResourceValue("Credit", ResourceFileName); } }
        public static string QtyToCredit { get { return ResourceRead.GetResourceValue("QtyToCredit", ResourceFileName); } }
        public static string PullSuccessMsg { get { return ResourceRead.GetResourceValue("PullSuccessMsg", ResourceFileName); } }
        public static string ItemQtyInQL { get { return ResourceRead.GetResourceValue("ItemQtyInQL", ResourceFileName); } }
        public static string MsgCreditToSelectRow { get { return ResourceRead.GetResourceValue("MsgCreditToSelectRow", ResourceFileName); } }
        public static string MsgBelowError { get { return ResourceRead.GetResourceValue("MsgBelowError", ResourceFileName); } }
        public static string MSCredit { get { return ResourceRead.GetResourceValue("MSCredit", ResourceFileName); } }
        public static string NewPullColumnsHeader { get { return ResourceRead.GetResourceValue("NewPullColumnsHeader", ResourceFileName); } }
        public static string CreditBinNumber { get { return ResourceRead.GetResourceValue("CreditBinNumber", ResourceFileName); } }
        public static string CreditQty { get { return ResourceRead.GetResourceValue("CreditQty", ResourceFileName); } }
        public static string CreditAll { get { return ResourceRead.GetResourceValue("CreditAll", ResourceFileName); } }
        public static string MSCreditQuantityValidation { get { return ResourceRead.GetResourceValue("MSCreditQuantityValidation", ResourceFileName); } }
        public static string PullQL { get { return ResourceRead.GetResourceValue("PullQL", ResourceFileName); } }
        public static string MSCreditAll { get { return ResourceRead.GetResourceValue("MSCreditAll", ResourceFileName); } }

    }


    [Serializable]
    public class PullMasterViewDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //ItemID
        [Display(Name = "ItemID", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Guid> ItemGUID { get; set; }

        //ProjectID
        [Display(Name = "ProjectID", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Guid> ProjectSpendGUID { get; set; }

        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResProjectMaster))]
        public System.String ProjectName { get; set; }


        [Display(Name = "ProjectSpendName", ResourceType = typeof(ResProjectMaster))]
        public System.String ProjectSpendName { get; set; }

        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        public System.String BinNumber { get; set; }

        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ItemNumber { get; set; }

        //UOI
        [Display(Name = "UOI", ResourceType = typeof(ResPullMaster))]
        [StringLength(256, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UOI { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //PoolQuantity
        [Display(Name = "PoolQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PoolQuantity { get; set; }


        public Nullable<System.Double> PullCost { get; set; }
        public Nullable<System.Double> PullPrice { get; set; }

        //CreditCustomerOwnedQuantity
        [Display(Name = "CreditCustomerOwnedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CreditCustomerOwnedQuantity { get; set; }

        //CreditConsignedQuantity
        [Display(Name = "CreditConsignedQuantity", ResourceType = typeof(ResPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CreditConsignedQuantity { get; set; }



        [Display(Name = "PoolQuantity", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Double> TempPullQTY { get; set; }

        //PoolQuantity
        [Display(Name = "DefaultPullQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> DefaultPullQuantity { get; set; }

        //OnHandQuantity            
        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        //SerialNumber
        [Display(Name = "SerialNumber", ResourceType = typeof(ResPullMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResPullMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LotNumber { get; set; }

        //DateCode
        [Display(Name = "DateCode", ResourceType = typeof(ResPullMaster))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String DateCode { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Int64> BinID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //GUID
        public Guid GUID { get; set; }


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

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //SupplierID
        [Display(Name = "SupplierID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> SupplierID { get; set; }

        //ManufacturerID
        [Display(Name = "ManufacturerID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> ManufacturerID { get; set; }

        //CategoryID
        [Display(Name = "CategoryID", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Int64> CategoryID { get; set; }

        //CategoryID
        [Display(Name = "WorkOrderID", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Guid> WorkOrderGUID { get; set; }

        [Display(Name = "PullCredit", ResourceType = typeof(ResPullMaster))]
        public string PullCredit { get; set; }

        public string ActionType { get; set; }

        public ItemMasterDTO ItemMasterView { get; set; }

        [Display(Name = "ItemType", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int32> ItemType { get; set; }

        public Nullable<Guid> RequisitionDetailGUID { get; set; }
        public Nullable<Guid> RequisitionGUID { get; set; }
        public string RequisitionNumber { get; set; }

        public Nullable<Guid> WorkOrderDetailGUID { get; set; }
        public Nullable<Guid> CountLineItemGuid { get; set; }

        public Nullable<Boolean> Billing { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Range(0, 100, ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Markup { get; set; }

        //SellPrice
        [Display(Name = "SellPrice", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> SellPrice { get; set; }

        public string CategoryName { get; set; }

        //UOMID
        [Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 UOMID { get; set; }

        [Display(Name = "UOMID", ResourceType = typeof(ResItemMaster))]
        [Required]
        public string Unit { get; set; }

        //Description
        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String Description { get; set; }

        public System.String WhatWhereAction { get; set; }

        //PackingQuantity
        [Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> PackingQuantity { get; set; }

        public string ManufacturerNumber { get; set; }
        public string Manufacturer { get; set; }
        public string SupplierPartNo { get; set; }
        public string SupplierName { get; set; }
        public string LongDescription { get; set; }
        public string GLAccount { get; set; }
        public bool Taxable { get; set; }
        public Nullable<double> InTransitquantity { get; set; }
        public Nullable<double> OnOrderQuantity { get; set; }
        public Nullable<double> OnTransferQuantity { get; set; }
        public System.Double CriticalQuantity { get; set; }
        public System.Double MinimumQuantity { get; set; }
        public System.Double MaximumQuantity { get; set; }
        public Nullable<System.Double> AverageUsage { get; set; }
        public Nullable<System.Double> Turns { get; set; }
        public bool? IsItemLevelMinMaxQtyRequired { get; set; }
        public Boolean Consignment { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }

        public string ItemUDF6 { get; set; }
        public string ItemUDF7 { get; set; }
        public string ItemUDF8 { get; set; }
        public string ItemUDF9 { get; set; }
        public string ItemUDF10 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF2
        [Display(Name = "UDF7", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF3
        [Display(Name = "UDF8", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF4
        [Display(Name = "UDF9", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF5
        [Display(Name = "UDF10", ResourceType = typeof(ResPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        public Nullable<System.Double> ItemCost { get; set; }
        public double? ItemOnhandQty { get; set; }
        public bool IsAddedFromPDA { get; set; }
        public bool IsProcessedAfterSync { get; set; }
        //public double? ItemOnHandQty { get; set; }
        public double? ItemLocationOnHandQty { get; set; }
        public double? ItemStageQty { get; set; }
        public double? ItemStageLocationQty { get; set; }
        //For Some of Data Pullcredit i null so need to set via query
        public string tempActionType { get; set; }
        public string tempPullCredit { get; set; }

        public bool IsOnlyFromItemUI { get; set; }
        public bool AllowNegative { get; set; }

        public string RequisitionUDF1 { get; set; }
        public string RequisitionUDF2 { get; set; }
        public string RequisitionUDF3 { get; set; }
        public string RequisitionUDF4 { get; set; }
        public string RequisitionUDF5 { get; set; }
        public string WorkOrderUDF1 { get; set; }
        public string WorkOrderUDF2 { get; set; }
        public string WorkOrderUDF3 { get; set; }
        public string WorkOrderUDF4 { get; set; }
        public string WorkOrderUDF5 { get; set; }

        public Nullable<Guid> SupplierAccountGuid { get; set; }

        //public string CreatedDate
        //{
        //    get
        //    {
        //        return FnCommon.ConvertDateByTimeZone(Created, true);
        //    }
        //}

        //public string UpdatedDate
        //{
        //    get
        //    {
        //        return FnCommon.ConvertDateByTimeZone(Updated, true);
        //    }
        //}
        private string _createdDate;
        private string _updatedDate;
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

        public string WOName { get; set; }

        public bool? IsEDISent { get; set; }


        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }

        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public short ScheduleMode { get; set; }

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

        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }

        [Display(Name = "PullOrderNumber", ResourceType = typeof(ResPullMaster))]
        public string PullOrderNumber { get; set; }
        public string CompanyName { get; set; }
        public string EnterpriseName { get; set; }
        public string ItemRFID { get; set; }
        public string UserName { get; set; }
        public System.String CostUOMName { get; set; }
        public string ControlNumber { get; set; }
        public int? TotalCount { get; set; }
        public string ToolName { get; set; }
        public Guid? ToolGUID { get; set; }

        public string ConsignmentName { get; set; }

        public string ItemBlanketPO { get; set; }

        [Display(Name = "PullItemCost", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Double> PullItemCost { get; set; }

        [Display(Name = "PullItemSellPrice", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Double> PullItemSellPrice { get; set; }

        [Display(Name = "PullMarkup", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Double> PullMarkup { get; set; }

        [Display(Name = "ItemCostOnPullDate", ResourceType = typeof(ResPullMaster))]
        public Nullable<System.Double> ItemCostOnPullDate { get; set; }

        public string PullUDF1 { get; set; }
        public string PullUDF2 { get; set; }
        public string PullUDF3 { get; set; }
        public string PullUDF4 { get; set; }
        public string PullUDF5 { get; set; }

        public long RowNum { get; set; }

        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
        public string InventoryConsuptionMethod { get; set; }

        public Nullable<System.Double> PullMasterItemCost { get; set; }
        public Nullable<System.Double> ItemSellPrice { get; set; }
        public Nullable<System.Double> ItemAverageCost { get; set; }
        public Nullable<System.Double> ItemMarkup { get; set; }
        public Nullable<System.Int32> ItemCostUOMValue { get; set; }
       
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        public string ItemImageExternalURL { get; set; }
        public long? ItemID { get; set; }
        public bool isPullUDF1Deleted { get; set; }
        public bool isPullUDF2Deleted { get; set; }
        public bool isPullUDF3Deleted { get; set; }
        public bool isPullUDF4Deleted { get; set; }
        public bool isPullUDF5Deleted { get; set; }
        public System.Int64 PullType { get; set; }
    }

    public class QuickListInfoToCredit
    {
        public string QuickListName { get; set; }
        public string Bin { get; set; }
        public string ProjectSpend { get; set; }
        public double Quantity { get; set; }
        public List<ItemInfoToCredit> ItemsToCredit { get; set; }
        public List<ItemInfoToMSCredit> ItemsToMSCredit { get; set; }
    }

    public class ItemInfoToMSCredit
    {
        public int RowID { get; set; }
        public Guid? ItemGuid { get; set; }
        public Guid? WOGuid { get; set; }
        public Guid? QLGuid { get; set; }
        public int ItemType { get; set; }
        public string Bin { get; set; }
        public string ProjectName { get; set; }
        public double Quantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ItemTracking { get; set; }
        public bool IsModelShow { get; set; }
        //public List<PullDetailsDTO> lstPrevPulls { get; set; }
        public List<PullDetailToMSCredit> PrevPullsToMSCredit { get; set; }
        public double PrevPullQty { get; set; }
        public string ItemNumber { get; set; }
        public string QuickListName { get; set; }
        public double ItemQtyInQL { get; set; }
        public double CreditQLQty { get; set; }
        public string ErrorMessage { get; set; }
        public string PullOrderNumber { get; set; }
        public string EditedFrom { get; set; }
        public Guid? SupplierAccountGuid { get; set; }
        public Guid? PullGUID { get; set; }
    }

    public class ItemInfoToCredit
    {
        public int RowID { get; set; }
        public Guid? ItemGuid { get; set; }
        public Guid? ProjectSpendGUID { get; set; }
        public Guid? WOGuid { get; set; }
        public Guid? QLGuid { get; set; }
        public int ItemType { get; set; }
        public string Bin { get; set; }
        public string ProjectName { get; set; }
        public double Quantity { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string ItemTracking { get; set; }
        public bool IsModelShow { get; set; }
        //public List<PullDetailsDTO> lstPrevPulls { get; set; }
        public List<PullDetailToCredit> PrevPullsToCredit { get; set; }
        public double PrevPullQty { get; set; }
        public string ItemNumber { get; set; }
        public string QuickListName { get; set; }
        public double ItemQtyInQL { get; set; }
        public double CreditQLQty { get; set; }
        public string ErrorMessage { get; set; }
        public string PullOrderNumber { get; set; }

        public Guid? PullGUID { get; set; }
        public string EditedFrom { get; set; }
        public Guid? SupplierAccountGuid { get; set; }

        public List<ItemInfoToPullDetails> lstPullDetails { get; set; }
        public List<PullErrorInfo> ErrorList { get; set; }
    }
    public class PullDetailToCredit
    {
        public string Serial { get; set; }
        public string Lot { get; set; }
        public string ExpireDate { get; set; }
        public double Qty { get; set; }
    }

    public class PullDetailToMSCredit
    {
        public string Serial { get; set; }
        public string Lot { get; set; }
        public string ExpireDate { get; set; }
        public double Qty { get; set; }
    }

    public class ItemInfoToPull
    {
        public double QtyToPull { get; set; }
        public string PullUDF1 { get; set; }
        public string PullUDF2 { get; set; }
        public string PullUDF3 { get; set; }
        public string PullUDF4 { get; set; }
        public string PullUDF5 { get; set; }
        public string PullOrderNumber { get; set; }

        public ItemMasterDTO ItemDTO { get; set; }
        public CompanyMasterDTO Company { get; set; }
        public RoomDTO Room { get; set; }
        public BinMasterDTO PullBin { get; set; }
        public ProjectMasterDTO ProjectSpend { get; set; }
        public WorkOrderDTO WorkOrder { get; set; }
        public UserMasterDTO User { get; set; }
        public RequisitionMasterDTO RequisitionDTO { get; set; }

        public string CallFrom { get; set; }
        public string ActionType { get; set; }
        public bool AllowNegative { get; set; }
        public bool AllowOverrideProjectSpendLimits { get; set; }
        public string EditedFrom { get; set; }

        public Guid? PullGUID { get; set; }
        public Nullable<Guid> SupplierAccountGuid { get; set; }
        public List<ItemInfoToPullDetails> lstPullDetails { get; set; }
        public double? SellPrice { get; set; }
    }


    public class RPT_PullMasterDTO
    {
        public Int64 ID { get; set; }
        public string StagingName { get; set; }
        public string PullBin { get; set; }
        public string ProjectSpendName { get; set; }
        public double? CustomerOwnedQuantity { get; set; }
        public double? ConsignedQuantity { get; set; }
        public double? PullQuantity { get; set; }
        public double? PullCost { get; set; }
        public double? Total { get; set; }
        public double? CreditCustomerOwnedQuantity { get; set; }
        public double? CreditConsignedQuantity { get; set; }
        public string ActionType { get; set; }
        public string PullCredit { get; set; }
        public bool? Billing { get; set; }
        public Guid GUID { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public bool? IsEDIRequired { get; set; }
        public DateTime? LastEDIDate { get; set; }
        public bool? IsEDISent { get; set; }
        public string WorkOrder { get; set; }
        public string RequisitionNumber { get; set; }
        public string CountName { get; set; }
        public string RoomName { get; set; }
        public string CompanyName { get; set; }
        public string ItemNumber { get; set; }
        public string ItemUniqueNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string SupplierPartNo { get; set; }
        public string UPC { get; set; }
        public string UNSPSC { get; set; }
        public string ItemDescription { get; set; }
        public string LongDescription { get; set; }
        public string ItemUDF1 { get; set; }
        public string ItemUDF2 { get; set; }
        public string ItemUDF3 { get; set; }
        public string ItemUDF4 { get; set; }
        public string ItemUDF5 { get; set; }
        public string ItemBlanketPO { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public string GLAccount { get; set; }
        public string Unit { get; set; }
        public string DefaultLocationName { get; set; }
        public string InventoryClassificationName { get; set; }
        public string CostUOM { get; set; }
        public int? InventoryClassification { get; set; }
        public int? LeadTimeInDays { get; set; }
        public string ItemTypeName { get; set; }
        public double? ItemCost { get; set; }
        public double? SellPrice { get; set; }
        public double? ExtendedCost { get; set; }
        public double? AverageCost { get; set; }
        public string PricePerTerm { get; set; }
        public double? OnHandQuantity { get; set; }
        public double? StagedQuantity { get; set; }
        public double? ItemInTransitquantity { get; set; }
        public double? RequisitionedQuantity { get; set; }
        public double? CriticalQuantity { get; set; }
        public double? MinimumQuantity { get; set; }
        public double? MaximumQuantity { get; set; }
        public double? DefaultReorderQuantity { get; set; }
        public double? DefaultPullQuantity { get; set; }
        public double? AverageUsage { get; set; }
        public string Turns { get; set; }
        public string Markup { get; set; }
        public string WeightPerPiece { get; set; }
        public string Consignment { get; set; }
        public string IsTransfer { get; set; }
        public string IsPurchase { get; set; }
        public string SerialNumberTracking { get; set; }
        public string LotNumberTracking { get; set; }
        public string DateCodeTracking { get; set; }
        public string IsBuildBreak { get; set; }
        public string Taxable { get; set; }
        public Int64 ItemID { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid? WorkOrderDetailguid { get; set; }
        public string RoomInfo { get; set; }
        public string CompanyInfo { get; set; }
        public string BarcodeImage_ItemNumber { get; set; }
        public string BarcodeImage_PullBin { get; set; }
        public int? QuantityDecimalPoint { get; set; }
        public int? CostDecimalPoint { get; set; }
        public string ConsignedPO { get; set; }
        public string ControlNumber { get; set; }

        public string WorkOrderUDF1 { get; set; }
        public string WorkOrderUDF2 { get; set; }
        public string WorkOrderUDF3 { get; set; }
        public string WorkOrderUDF4 { get; set; }
        public string WorkOrderUDF5 { get; set; }

        public Int64? CategoryID { get; set; }

        public Int64? SupplierID { get; set; }

        public Int64? PullBinID { get; set; }

        public Int64? ManufacturerID { get; set; }

        public Guid? ProjectSpendGUID { get; set; }

        public Guid? RequisitionDetailGUID { get; set; }

        public Int64? CreatedByID { get; set; }

        public Int64? GLAccountID { get; set; }
    }

    public class RequisitionItemsToPull
    {
        public Int64 ID { get; set; }
        public string ItemGUID { get; set; }
        public string ProjectGUID { get; set; }
        public double PullCreditQuantity { get; set; }
        public Int64 BinID { get; set; }
        public string PullCredit { get; set; }
        public double TempPullQTY { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string RequisitionDetailGUID { get; set; }
        public string WorkOrderDetailGUID { get; set; }
        public Guid? ICDtlGUID { get; set; }
        public string ProjectSpendName { get; set; }
        public string PullOrderNumber { get; set; }
        public string RequisitionMasterGUID { get; set; }
        public string ItemNumber { get; set; }


        public Guid? ToolGUID { get; set; }
        public Guid? TechnicianGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public string TechnicianName { get; set; }

        public Guid? SupplierAccountGuid { get; set; }
    }

    public class ReqPullAllJsonResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string LocationMSG { get; set; }
        public string PSLimitExceed { get; set; }

        public string ReqDetailGuid { get; set; }
        public string ItemNumber { get; set; }
    }


}


