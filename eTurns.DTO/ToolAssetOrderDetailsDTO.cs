using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace eTurns.DTO
{
    [Serializable]
    public class ToolAssetOrderDetailsDTO : ToolAssetViewFields
    {
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> ToolBinID { get; set; }
        public new Nullable<System.Int64> CreatedBy { get; set; }
        public Nullable<System.Int64> LastUpdatedBy { get; set; }
        public Nullable<System.Int64> Room { get; set; }
        public Nullable<System.Int64> CompanyID { get; set; }

        public Guid GUID { get; set; }
        public Nullable<Guid> ToolAssetOrderGUID { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }
        public Nullable<Guid> ToolItemGUID { get; set; }
        public Nullable<Guid> AssetGUID { get; set; }
        public Nullable<int> SessionSr { get; set; }
        public double? QuantityPerKit { get; set; }
        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> RequiredDate { get; set; }

        [Display(Name = "Created", ResourceType = typeof(Resources.ResCommon))]
        public Nullable<System.DateTime> Created { get; set; }

        [Display(Name = "LastUpdated", ResourceType = typeof(ResOrder))]
        public Nullable<System.DateTime> LastUpdated { get; set; }

        public Nullable<System.DateTime> LastEDIDate { get; set; }

        [Display(Name = "RequestedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> RequestedQuantity { get; set; }

        [Display(Name = "ReceivedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> ReceivedQuantity { get; set; }

        [Display(Name = "ApprovedQuantity", ResourceType = typeof(ResOrder))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<double> ApprovedQuantity { get; set; }

        public Nullable<double> InTransitQuantity { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<bool> IsEDIRequired { get; set; }
        public Nullable<bool> IsEDISent { get; set; }

        [Display(Name = "ASNNumber", ResourceType = typeof(ResOrder))]
        public string ASNNumber { get; set; }

        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }



        public Guid? ToolLocationGuid { get; set; }
        public string ToolLocationName { get; set; }

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
        public System.String AddedFrom { get; set; }


        //EditedFrom
        [Display(Name = "EditedFrom", ResourceType = typeof(ResItemMaster))]
        public System.String EditedFrom { get; set; }


        public bool IsHistory { get; set; }
        public int TotalRecords { get; set; }

        public DateTime? ChangeOrderDetailCreated { get; set; }
        public DateTime? ChangeOrderDetailLastUpdated { get; set; }
        public Int64? ChangeOrderDetailCreatedBy { get; set; }
        public Int64? ChangeOrderDetailLastUpdatedBy { get; set; }
        public Guid ChangeOrderDetailGUID { get; set; }
        public Int64 ChangeOrderDetailID { get; set; }
        public Guid? ChangeOrderMasterGUID { get; set; }
        public System.String ODPackSlipNumbers { get; set; }
        public bool IsOnlyFromUI { get; set; }

        public bool? IsCloseTool { get; set; }

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
            set
            { this._ReceivedOnWeb = value; }
        }
        [Display(Name = "RequiredDate", ResourceType = typeof(ResOrder))]
        public string RequiredDateStr { get; set; }
        //public string CostUOM { get; set; }

        public string LineNumber { get; set; }
        public string ControlNumber { get; set; }
        //public bool IsPackslipMandatoryAtReceive { get; set; }
        public string Comment { get; set; }

        public System.String UDF1 { get; set; }
        public System.String UDF2 { get; set; }
        public System.String UDF3 { get; set; }
        public System.String UDF4 { get; set; }
        public System.String UDF5 { get; set; }

        public double? OrderCost { get; set; }

        public Guid? tempDetailsGUID { get; set; }

        public string ToolTypeTracking { get; set; }
        public bool IsSerialDateCode { get; set; }
        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
    }


    [Serializable]
    public class ToolAssetViewFields
    {

        public Nullable<Guid> AssetViewGUID { get; set; }
        public Nullable<Guid> ToolViewGUID { get; set; }
        public Nullable<Int64> ToolID { get; set; }
        public Nullable<Int64> AssetID { get; set; }
        public Nullable<Int64> CreatedBy { get; set; }
        public Nullable<Int64> ToolLastUpdatedBy { get; set; }
        public Nullable<Int64> ToolRoom { get; set; }



        public Nullable<bool> ToolIsDeleted { get; set; }
        public Nullable<bool> ToolIsArchived { get; set; }

        public Nullable<bool> IsBuildBreak { get; set; }

        public Nullable<DateTime> ToolCreated { get; set; }
        public Nullable<DateTime> ToolUpdated { get; set; }

        public Nullable<double> ToolQuantity { get; set; }



        public Nullable<Int64> ToolType { get; set; }

        public Nullable<double> ToolCost { get; set; }

        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string AssetName { get; set; }
        public string ToolDescription { get; set; }
        public string ImagePath { get; set; }
        public string ToolUDF1 { get; set; }
        public string ToolUDF2 { get; set; }
        public string ToolUDF3 { get; set; }
        public string ToolUDF4 { get; set; }
        public string ToolUDF5 { get; set; }
        public string ToolCreatedByName { get; set; }
        public string ToolUpdatedByName { get; set; }
        public string ToolRoomName { get; set; }

        private string _itemcreatedDate;
        private string _itemupdatedDate;
        public string ItemCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_itemcreatedDate))
                {
                    _itemcreatedDate = FnCommon.ConvertDateByTimeZone(ToolCreated, true);
                }
                return _itemcreatedDate;
            }
            set { this._itemcreatedDate = value; }
        }

        public string ItemUpdatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_itemupdatedDate))
                {
                    _itemupdatedDate = FnCommon.ConvertDateByTimeZone(ToolUpdated, true);
                }
                return _itemupdatedDate;
            }
            set { this._itemupdatedDate = value; }
        }

    }






}


