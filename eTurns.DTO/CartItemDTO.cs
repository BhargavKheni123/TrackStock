using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class CartItemDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //ItemName
        [Display(Name = "ItemNumber", ResourceType = typeof(ResCartItem))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String ItemNumber { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResCartItem))]
        public Nullable<Guid> ItemGUID { get; set; }

        //Quantity
        [Display(Name = "Quantity", ResourceType = typeof(ResCartItem))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Quantity { get; set; }

        //Status
        [Display(Name = "Status", ResourceType = typeof(ResCartItem))]
        [StringLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String Status { get; set; }

        //ReplenishType
        [Display(Name = "ReplenishType", ResourceType = typeof(ResCartItem))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String ReplenishType { get; set; }

        //IsKitComponent
        [Display(Name = "IsKitComponent", ResourceType = typeof(ResCartItem))]
        public Nullable<Boolean> IsKitComponent { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResCartItem))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResCartItem))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResCartItem))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResCartItem))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResCartItem))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
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
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "IsTransfer", ResourceType = typeof(ResItemMaster))]
        public bool? IsTransfer { get; set; }

        //IsPurchase
        [Display(Name = "IsPurchase", ResourceType = typeof(ResItemMaster))]
        public bool? IsPurchase { get; set; }

        //IsPurchase
        [Display(Name = "IsAutoMatedEntry", ResourceType = typeof(ResCartItem))]
        public bool IsAutoMatedEntry { get; set; }

        public long SupplierId { get; set; }
        public string SupplierName { get; set; }

        public long? BinId { get; set; }
        public Guid? BinGUID { get; set; }
        public string BinName { get; set; }

        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

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
        public bool IsEnforceDefaultReorderQuantity { get; set; }
        public Nullable<System.Double> DefaultReorderQuantityNew { get; set; }
        public Nullable<System.Double> DefaultReorderQuantityTemp { get; set; }
        public long? BlanketPOID { get; set; }
        public string BlanketPONumber { get; set; }
        public Nullable<System.Int64> CategoryID { get; set; }
        public string CategoryName { get; set; }
        public System.Int64 UOMID { get; set; }
        public string UnitName { get; set; }
        public Nullable<System.Double> PackingQuantity { get; set; }
        public Nullable<System.Double> DefaultReorderQuantity { get; set; }
        public Nullable<System.Double> OnOrderQuantity { get; set; }
        public Nullable<System.Double> OnTransferQuantity { get; set; }
        public Nullable<System.Double> RequisitionedQuantity { get; set; }
        public System.String SupplierPartNo { get; set; }
        public Nullable<System.Int64> ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public System.String ManufacturerNumber { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Double> Markup { get; set; }
        public Nullable<System.Double> SellPrice { get; set; }
        public long ItemId { get; set; }
        public short CartItemCriticalLevel { get; set; }
        public double CriticalQuantity { get; set; }
        public double MinimumQuantity { get; set; }
        public double MaximumQuantity { get; set; }
        public double OnHandQuantity { get; set; }
        public double ItemLocationCriticalQuantity { get; set; }
        public double ItemLocationMinimumQuantity { get; set; }
        public double ItemLocationMaximumQuantity { get; set; }

        public double ItemLocationCustomerOwnedQuantity { get; set; }
        public double ItemLocationConsignedQuantity { get; set; }
        public double ItemLocationOH { get; set; }

        public bool? IsItemLevelMinMaxQtyRequired { get; set; }
        public System.String WhatWhereAction { get; set; }
        public decimal? MaxOrderSize { get; set; }
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
        public string Description { get; set; }
        public double ILOnHandQuantity { get; set; }
        public long EnterpriseId { get; set; }
        public bool EnforsedCartQuanity { get; set; }
        public Nullable<System.Int32> LeadTimeInDays { get; set; }
        public bool IsOnlyFromItemUI { get; set; }
        public bool? issochanged { get; set; }
        public bool? isstchanged { get; set; }

        public bool? IsItemBelowCritcal { get; set; }
        public bool? IsItemBelowMinimum { get; set; }
        public bool? IsItemLocationBelowCritcal { get; set; }
        public bool? IsItemLocationBelowMinimum { get; set; }

        //public string CreatedDate { get; set; }
        //public string UpdatedDate { get; set; }

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

        public string OrderUOM { get; set; }
        public Nullable<System.Int64> OrderUOMValue { get; set; }
        public Nullable<System.Double> OrderUOMQuantity { get; set; }
        public bool? IsAllowOrderCostuom { get; set; }
        public string CostUOM { get; set; }

        public int? TotalRecords { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public int ItemType { get; set; }
        public bool IsItemActive { get; set; }
        public bool IsItemOrderable { get; set; }
        public string LongDescription { get; set; }
        public bool? AllowDuplicateItemandBin { get; set; }
    }

    public class CartChartDTO
    {
        public string ItemNumber { get; set; }
        public Guid ItemGuid { get; set; }
        public Nullable<long> SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double Quantity { get; set; }
        public int CartCount { get; set; }

        public int TotalRecords { get; set; }
    }
    public class ResCartItem
    {
        private static string ResourceFileName = "ResCartItem";

        public static string BinName
        {
            get
            {
                return ResourceRead.GetResourceValue("BinName", ResourceFileName);
            }
        }

        public static string QuantityAdjustmentMessage
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityAdjustmentMessage", ResourceFileName);
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
        ///   Looks up a localized string similar to CartItem {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to Include Archived:.
        /// </summary>
        public static string IsAutoMatedEntry
        {
            get
            {
                return ResourceRead.GetResourceValue("IsAutoMatedEntry", ResourceFileName);
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
        ///   Looks up a localized string similar to CartItem.
        /// </summary>
        public static string CartItemHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("CartItemHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CartItem.
        /// </summary>
        public static string CartItem
        {
            get
            {
                return ResourceRead.GetResourceValue("CartItem", ResourceFileName);
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
        ///   Looks up a localized string similar to ItemNumber.
        /// </summary>
        public static string ItemNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemNumber", ResourceFileName);
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
        ///   Looks up a localized string similar to Quantity.
        /// </summary>
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Status.
        /// </summary>
        public static string Status
        {
            get
            {
                return ResourceRead.GetResourceValue("Status", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ReplenishType.
        /// </summary>
        public static string ReplenishType
        {
            get
            {
                return ResourceRead.GetResourceValue("ReplenishType", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to IsKitComponent.
        /// </summary>
        public static string IsKitComponent
        {
            get
            {
                return ResourceRead.GetResourceValue("IsKitComponent", ResourceFileName);
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
        public static string msgAddAll
        {
            get
            {
                return ResourceRead.GetResourceValue("msgAddAll", ResourceFileName);
            }
        }
        public static string SelectOneRecrod
        {
            get
            {
                return ResourceRead.GetResourceValue("SelectOneRecrod", ResourceFileName);
            }
        }

        public static string msgAddSingle
        {
            get
            {
                return ResourceRead.GetResourceValue("msgAddSingle", ResourceFileName);
            }
        }

        public static string OrderUOMQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("OrderUOMQTY", ResourceFileName);
            }
        }
        public static string PageTitleQuoteToOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitleQuoteToOrder", ResourceFileName);
            }
        }
        public static string CartLegendNotOrderable
        {
            get
            {
                return ResourceRead.GetResourceValue("CartLegendNotOrderable", ResourceFileName);
            }
        }
        public static string MsgNoItemAvailableinreplineshRoom
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoItemAvailableinreplineshRoom", ResourceFileName);
            }
        }
        public static string MsgSelectItemWithSuggestedReturn
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectItemWithSuggestedReturn", ResourceFileName);
            }
        }
        public static string MsgSelectItemWithPurchase
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectItemWithPurchase", ResourceFileName);
            }
        }
        public static string MsgSelectItemWithTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectItemWithTransfer", ResourceFileName);
            }
        }
        public static string ReqItemsToCreateOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqItemsToCreateOrder", ResourceFileName);
            }
        }
        public static string ReqItemsToCreateTransfer
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqItemsToCreateTransfer", ResourceFileName);
            }
        }
        public static string ReqItemsToCreateReturnOrder
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqItemsToCreateReturnOrder", ResourceFileName);
            }
        }
        public static string ReqOrderToCreate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqOrderToCreate", ResourceFileName);
            }
        }
        public static string Select { get { return ResourceRead.GetResourceValue("Select", ResourceFileName); } }
        public static string CreateTransfers { get { return ResourceRead.GetResourceValue("CreateTransfers", ResourceFileName); } }
        public static string CreateOrders { get { return ResourceRead.GetResourceValue("CreateOrders", ResourceFileName); } }
        public static string CreateReturns { get { return ResourceRead.GetResourceValue("CreateReturns", ResourceFileName); } }
        public static string CreateQuotes { get { return ResourceRead.GetResourceValue("CreateQuotes", ResourceFileName); } }
        public static string Create { get { return ResourceRead.GetResourceValue("Create", ResourceFileName); } }
        public static string SuggestedOrders { get { return ResourceRead.GetResourceValue("SuggestedOrders", ResourceFileName); } }
        public static string SuggestedReturns { get { return ResourceRead.GetResourceValue("SuggestedReturns", ResourceFileName); } }
        public static string MsgItemMaximumQuantity { get { return ResourceRead.GetResourceValue("MsgItemMaximumQuantity", ResourceFileName); } }
        public static string MsgBinMaximumQuantity { get { return ResourceRead.GetResourceValue("MsgBinMaximumQuantity", ResourceFileName); } }

        public static string ItemNotExistInRoom { get { return ResourceRead.GetResourceValue("ItemNotExistInRoom", ResourceFileName); } }
        public static string EnterEnterpriseCompanyRoomValidation { get { return ResourceRead.GetResourceValue("EnterEnterpriseCompanyRoomValidation", ResourceFileName); } }
        public static string OrdersWillBeCreated { get { return ResourceRead.GetResourceValue("OrdersWillBeCreated", ResourceFileName); } }
        public static string ReturnOrdersWillBeCreated { get { return ResourceRead.GetResourceValue("ReturnOrdersWillBeCreated", ResourceFileName); } }
        public static string Update { get { return ResourceRead.GetResourceValue("Update", ResourceFileName); } }
        public static string QuantityReplenishValidation { get { return ResourceRead.GetResourceValue("QuantityReplenishValidation", ResourceFileName); } }

    }
}


