using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class ItemLocationQTYDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResItemLocationQTY))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 BinID { get; set; }

        public string BinNumber { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //Quantity
        [Display(Name = "Quantity", ResourceType = typeof(ResItemLocationQTY))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double Quantity { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResItemLocationQTY))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String LotNumber { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationQTY))]
        public Nullable<Guid> ItemGUID { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResItemLocationQTY))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> Room { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.Int64> CompanyID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public string ItemNumber { get; set; }

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


        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }

        //CriticalQuantity
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        public Nullable<System.Double> MaximumQuantity { get; set; }

        //SuggestedOrder
        public Nullable<System.Double> SuggestedOrderQuantity { get; set; }

        public Nullable<System.Double> Cost { get; set; }

        public Nullable<System.Double> Markup { get; set; }

        public Nullable<System.Double> SellPrice { get; set; }

        public Nullable<System.Double> ExtendedCost { get; set; }

        public Nullable<System.Int64> CostUOMID { get; set; }

        public Nullable<System.Double> Averagecost { get; set; }

        public string eVMISensorPort { get; set; }

        public Nullable<System.Double> eVMISensorID { get; set; }

        public System.String CostUOMName { get; set; }

        public bool? IsItemLevelMinMaxQtyRequired { get; set; }


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
                    _updatedDate = FnCommon.ConvertDateByTimeZone(LastUpdated, true);
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
        public Nullable<decimal> OnOrderQty { get; set; }

        public Nullable<System.Double> SuggestedTransferQuantity { get; set; }
        public string BinUDF1 { get; set; }
        public string BinUDF2 { get; set; }
        public string BinUDF3 { get; set; }
        public string BinUDF4 { get; set; }
        public string BinUDF5 { get; set; }
    }

    public class ResItemLocationQTY
    {
        private static string ResourceFileName = "ResItemLocationQTY";

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
        ///   Looks up a localized string similar to ItemLocationQTY {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to ItemLocationQTY.
        /// </summary>
        public static string ItemLocationQTYHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationQTYHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ItemLocationQTY.
        /// </summary>
        public static string ItemLocationQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("ItemLocationQTY", ResourceFileName);
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
        ///   Looks up a localized string similar to LastUpdated.
        /// </summary>
        public static string LastUpdated
        {
            get
            {
                return ResourceRead.GetResourceValue("LastUpdated", ResourceFileName);
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
        ///   Looks up a localized string similar to CompanyID.
        /// </summary>
        public static string CompanyID
        {
            get
            {
                return ResourceRead.GetResourceValue("CompanyID", ResourceFileName);
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
        public static string OnOrderQty
        {
            get
            {
                return ResourceRead.GetResourceValue("OnOrderQty", ResourceFileName);
            }
        }

    }
}


