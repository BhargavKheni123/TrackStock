using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    public class QuickListDetailDTO
    {
        public Int64 ID { get; set; }
        [Display(Name = "Quantity", ResourceType = typeof(ResQuickList))]
        public Nullable<double> Quantity { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Guid QuickListGUID { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "HistoryAction", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public Int64 QuickListHistoryID { get; set; }

        public bool IsHistory { get; set; }

        public ItemMasterDTO ItemDetail { get; set; }

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

        [Display(Name = "LongDescription", ResourceType = typeof(ResItemMaster))]
        public System.String LongDescription { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnTransferQuantity { get; set; }

        [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnOrderQuantity { get; set; }

        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        [Display(Name = "PackingQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> PackingQuantity { get; set; }

        [Display(Name = "SellPrice", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> SellPrice { get; set; }

        [Display(Name = "Markup", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> Markup { get; set; }

        public Nullable<double> DefaultReorderQuantity { get; set; }

        public Nullable<double> DefaultPullQuantity { get; set; }

        public System.String Unit { get; set; }

        public bool SerialNumberTracking { get; set; }

        public bool LotNumberTracking { get; set; }

        public bool DateCodeTracking { get; set; }

        public int ItemType { get; set; }

        public string DefaultLocationName { get; set; }

        public Int64? DefaultLocation { get; set; }

        public bool Consignment { get; set; }

        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResQuickList))]
        public Nullable<double> ConsignedQuantity { get; set; }

        public int QuickListType { get; set; }

        #endregion
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


        [Display(Name = "ReceivedOn", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(Resources.ResCommon))]
        public System.DateTime ReceivedOnWeb { get; set; }

        //AddedFrom
        [Display(Name = "AddedFrom", ResourceType = typeof(Resources.ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Resources.ResMessage))]
        public System.String AddedFrom { get; set; }

        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(Resources.ResCommon))]
        [StringLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Resources.ResMessage))]
        public System.String EditedFrom { get; set; }

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
        [Display(Name = "ItemUDF1", ResourceType = typeof(ResQuickList))]
        public string ItemUDF1 { get; set; }

        [Display(Name = "ItemUDF2", ResourceType = typeof(ResQuickList))]
        public string ItemUDF2 { get; set; }

        [Display(Name = "ItemUDF3", ResourceType = typeof(ResQuickList))]
        public string ItemUDF3 { get; set; }

        [Display(Name = "ItemUDF4", ResourceType = typeof(ResQuickList))]
        public string ItemUDF4 { get; set; }

        [Display(Name = "ItemUDF5", ResourceType = typeof(ResQuickList))]
        public string ItemUDF5 { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResQuickList))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResQuickList))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResQuickList))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResQuickList))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResQuickList))]
        public string UDF5 { get; set; }

        public Nullable<Int64> BinID { get; set; }
        public string BinName { get; set; }

        public bool PullQtyScanOverride { get; set; }
    }

    public class QuickListLineItemDTO
    {
        public Int64 ID { get; set; }
        [Display(Name = "Quantity", ResourceType = typeof(ResQuickList))]
        public double Quantity { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Guid QuickListGUID { get; set; }

        [Display(Name = "CreatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> Created { get; set; }

        [Display(Name = "UpdatedOn", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<DateTime> LastUpdated { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> Room { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(Resources.ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(Resources.ResCommon))]
        public string UpdatedByName { get; set; }

        [Display(Name = "HistoryAction", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        public Int64 QuickListHistoryID { get; set; }

        public bool IsHistory { get; set; }

        public int TotalRecords { get; set; }
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

        [Display(Name = "LongDescription", ResourceType = typeof(ResItemMaster))]
        public System.String LongDescription { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = "OnTransferQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnTransferQuantity { get; set; }

        [Display(Name = "OnOrderQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnOrderQuantity { get; set; }

        [Display(Name = "OnHandQuantity", ResourceType = typeof(ResItemMaster))]
        public Nullable<System.Double> OnHandQuantity { get; set; }

        #endregion


    }
    public class QuickListLineItemDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid? GUID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public System.String ItemNumber { get; set; }
        public double Quantity { get; set; }
        public Nullable<Guid> ItemGUID { get; set; }
        public Guid QuickListGUID { get; set; }
        [Display(Name = "UDF1", ResourceType = typeof(ResQuickList))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResQuickList))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResQuickList))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResQuickList))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResQuickList))]
        public string UDF5 { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> NoOfItems { get; set; }
        public string RoomName { get; set; }
        public string AddedFrom { get; set; }
        public string EditedFrom { get; set; }
        public Nullable<DateTime> ReceivedOnWeb { get; set; }
        public Nullable<DateTime> ReceivedOn { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<long> LastUpdatedBy { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public Nullable<DateTime> LastUpdated { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public string UpdatedByName { get; set; }
        public string CreatedByName { get; set; }
        public string BinNumber { get; set; }
        public long? BinId { get; set; }
        public double ConsignedQuantity { get; set; }

    }


}
