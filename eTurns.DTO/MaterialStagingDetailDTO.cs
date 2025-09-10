using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class MaterialStagingDetailDTO : ICloneable
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        [Display(Name = "MaterialStagingName", ResourceType = typeof(ResMaterialStagingDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string MaterialStagingName { get; set; }


        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string ItemNumber { get; set; }
        public string Description { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResMaterialStagingDetail))]
        public Nullable<Guid> ItemGUID { get; set; }

        //StagingBinID
        [Display(Name = "StagingBinID", ResourceType = typeof(ResMaterialStagingDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 StagingBinID { get; set; }

        //StagingBinName
        [Display(Name = "StagingBinName", ResourceType = typeof(ResMaterialStagingDetail))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public string StagingBinName { get; set; }

        public Nullable<Guid> StagingBinGUID { get; set; }
        public Nullable<Guid> BinGUID { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(ResMaterialStagingDetail))]
        public Nullable<System.Double> TempStageQTY { get; set; }

        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResMaterialStagingDetail))]
        public long? BinID { get; set; }

        //BinName
        [Display(Name = "BinName", ResourceType = typeof(ResMaterialStagingDetail))]
        public string BinName { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //MaterialStagingGUID
        [Display(Name = "MaterialStagingGUID", ResourceType = typeof(ResMaterialStagingDetail))]
        public Nullable<Guid> MaterialStagingGUID { get; set; }

        //Quantity
        [Display(Name = "Quantity", ResourceType = typeof(ResMaterialStagingDetail))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double Quantity { get; set; }

        //IsDeleted
        public bool IsDeleted { get; set; }

        //IsArchived
        public bool IsArchived { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        //Updated
        [Display(Name = "Updated", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Updated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public long CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public long LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        public long RoomId { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public long CompanyID { get; set; }

        //Added
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        public ItemMasterDTO ItemDetail { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 MSHistoryID { get; set; }

        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResCommon))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResCommon))]
        public System.String EditedFrom { get; set; }

        public bool IsOnlyFromItemUI { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

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

        public Boolean DateCodeTracking { get; set; }

        public Boolean LotNumberTracking { get; set; }
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }
        public Nullable<System.Double> ConsignedQuantity { get; set; }
        public int? TotalRecords { get; set; }

        public long HistoryID { get; set; }
    }

    public class ResMaterialStagingDetail
    {
        private static string ResourceFileName = "ResMaterialStagingDetail";

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
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errEnterQty
        {
            get
            {
                return ResourceRead.GetResourceValue("errEnterQty", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errEnterStagingBin
        {
            get
            {
                return ResourceRead.GetResourceValue("errEnterStagingBin", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errInventoryLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("errInventoryLocation", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("errAvailableQty", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errNOQtyAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("errNOQtyAvailable", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string errNOQtyZero
        {
            get
            {
                return ResourceRead.GetResourceValue("errNOQtyZero", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string lblAvailableQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("lblAvailableQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string lblAvailableCOwnQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("lblAvailableCOwnQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string lblAvailableConsignQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("lblAvailableConsignQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Action.
        /// </summary>
        public static string lblPullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("lblPullQuantity", ResourceFileName);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to MaterialStagingDetail {0} already exist! Try with Another!.
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
        ///   Looks up a localized string similar to MaterialStagingDetail.
        /// </summary>
        public static string MaterialStagingDetailHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingDetailHeader", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to MaterialStagingDetail.
        /// </summary>
        public static string MaterialStagingDetail
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingDetail", ResourceFileName);
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

        public static string MaterialStagingName
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingName", ResourceFileName);
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
        ///   Looks up a localized string similar to StagingBinID.
        /// </summary>
        public static string StagingBinID
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingBinID", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to StagingBinID.
        /// </summary>
        public static string StagingBinName
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingBinName", ResourceFileName);
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

        public static string InventoryLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("InventoryLocation", ResourceFileName);
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
        ///   Looks up a localized string similar to MaterialStagingGUID.
        /// </summary>
        public static string MaterialStagingGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("MaterialStagingGUID", ResourceFileName);
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
        public static string StagingQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("StagingQuantity", ResourceFileName);
            }
        }
    }
}


