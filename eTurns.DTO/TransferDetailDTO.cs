using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class TransferDetailDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }

        //TransferGUID
        [Display(Name = "TransferGUID", ResourceType = typeof(ResTransfer))]
        public Guid TransferGUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResTransfer))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid ItemGUID { get; set; }

        //Bin
        [Display(Name = "Location", ResourceType = typeof(ResTransfer))]
        public Nullable<System.Int64> Bin { get; set; }

        //RequestedQuantity
        [Display(Name = "RequestedQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double RequestedQuantity { get; set; }

        //RequiredDate
        [Display(Name = "RequiredDate", ResourceType = typeof(ResTransfer))]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        //ReceivedQuantity
        [Display(Name = "ReceivedQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ReceivedQuantity { get; set; }

        //ApprovedQuantity
        [Display(Name = "ApprovedQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ApprovedQuantity { get; set; }

        //FulFillQuantity
        [Display(Name = "FulFillQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> FulFillQuantity { get; set; }

        //ReceivedQuantity
        [Display(Name = "IntransitQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> IntransitQuantity { get; set; }

        //FulFillQuantity
        [Display(Name = "ShippedQuantity", ResourceType = typeof(ResTransfer))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ShippedQuantity { get; set; }

        //Created
        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime Created { get; set; }

        //LastUpdated
        [Display(Name = "LastUpdated", ResourceType = typeof(ResTransfer))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.DateTime LastUpdated { get; set; }

        //CreatedBy
        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CreatedBy { get; set; }

        //LastUpdatedBy
        [Display(Name = "LastUpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 LastUpdatedBy { get; set; }

        //Room
        [Display(Name = "Room", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 Room { get; set; }

        //IsDeleted
        public Nullable<Boolean> IsDeleted { get; set; }

        //IsArchived
        public Nullable<Boolean> IsArchived { get; set; }

        //CompanyID
        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 CompanyID { get; set; }

        //GUID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Guid GUID { get; set; }

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
        public Int64 HistoryID { get; set; }

        [Display(Name = "ReceivedOnDate", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.DateTime> ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public System.String EditedFrom { get; set; }

        public bool IsHistory { get; set; }
        public string BinName { get; set; }

        public bool IsOnlyFromUI { get; set; }

        #region ItemMasterFields

        [Display(Name = "ItemID")]
        public System.Int64 ItemID { get; set; }

        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ItemNumber { get; set; }

        [Display(Name = "SupplierPartNo", ResourceType = typeof(ResItemMaster))]
        public System.String SupplierPartNo { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(ResSupplierMaster))]
        public string SupplierName { get; set; }

        [Display(Name = "ManufacturerName", ResourceType = typeof(ResManufacturer))]
        public string ManufacturerName { get; set; }

        [Display(Name = "ManufacturerNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ManufacturerNumber { get; set; }

        [Display(Name = "AverageCost", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> AverageCost { get; set; }

        [Display(Name = "Cost", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> Cost { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResItemMaster))]
        public System.String Description { get; set; }

        [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnTransferQuantity { get; set; }

        [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnOrderQuantity { get; set; }

        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        //[Display(Name = "InTransitquantity", ResourceType = typeof(ResItemMaster))]
        //public Nullable<System.Double> InTransitquantity { get; set; }

        [Display(Name = "DefaultLocation", ResourceType = typeof(ResItemMaster))]
        public Int64 DefaultLocation { get; set; }


        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean SerialNumberTracking { get; set; }

        [Display(Name = "LotNumberTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean LotNumberTracking { get; set; }

        [Display(Name = "DateCodeTracking", ResourceType = typeof(ResItemMaster))]
        public Boolean DateCodeTracking { get; set; }

        public bool IsStaging { get; set; }

        [Display(Name = "StagedQuantity", ResourceType = typeof(ResItemMaster))]
        public double? StagedQuantity { get; set; }

        #endregion

        /// <summary>
        /// Whether Lot Selection is required or not(while receiving transfer)
        /// </summary>
        public bool IsLotSelectionRequire { get; set; }

        /// <summary>
        /// Total of ReturnedQuantity for item
        /// </summary>
        public Nullable<double> ReturnedQuantity { get; set; }

        public int TotalRecords { get; set; }


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

        public string ControlNumber { get; set; }

        private string _RequiredDate;
        public bool IsTransferSelected { get; set; }
        public string RequiredDateString
        {
            get
            {
                if (string.IsNullOrEmpty(_RequiredDate))
                {
                    _RequiredDate = Convert.ToString(FnCommon.ConvertDateByTimeZone(RequiredDate, true, true).Split(' ')[0]);
                }
                return _RequiredDate;
            }
            set { this._RequiredDate = value; }
        }

        public int? TransferRequestType { get; set; }

        public long? DestinationBinId { get; set; }

        public string DestinationBin { get; set; }

        public double? TransferCost { get; set; }

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
    }

}


