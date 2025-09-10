using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace eTurns.DTO
{

    public class ToolAssetOrderReturnQuantityDetail
    {
        public double ReturnQuantity { get; set; }
        public Guid ToolGUID { get; set; }
        public Guid ToolAssetOrderDetailGUID { get; set; }
        public Int64 LocationID { get; set; }
        public string Location { get; set; }

    }

    public class ReceivedToolAssetOrderTransferDetailDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResToolAsset))]
        public Nullable<System.Int64> ToolBinID { get; set; }

        public Nullable<Guid> ToolLocationGUID { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "Quantity", ResourceType = typeof(ResToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Quantity { get; set; }



        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResToolAsset))]
        public Nullable<System.DateTime> ReceivedDate { get; set; }




        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResToolAsset))]
        public string Received { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Cost { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResToolMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "GUID", ResourceType = typeof(ResToolAsset))]
        public Nullable<Guid> ToolGUID { get; set; }

        [Display(Name = "GUID", ResourceType = typeof(ResToolAssetOrder))]
        public Nullable<Guid> ToolAssetOrderDetailGUID { get; set; }


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
        [Display(Name = "ToolName", ResourceType = typeof(ResToolMaster))]
        public System.String ToolName { get; set; }

        [Display(Name = "Serial", ResourceType = typeof(ResToolMaster))]
        public System.String Serial { get; set; }

        [Display(Name = "Location", ResourceType = typeof(ResLocation))]
        public string Location { get; set; }

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


        public Int32? Type { get; set; }


        public string mode { get; set; }
        //OrderDetailID
        public Nullable<System.Guid> OrderDetailGUID { get; set; }
        public Nullable<System.Guid> KitDetailGUID { get; set; }
        public Nullable<System.Guid> TransferDetailGUID { get; set; }
        public Nullable<System.Guid> ToolAssetQuantityDetailGUID { get; set; }
        [Display(Name = "Serial", ResourceType = typeof(ResToolMaster))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        public System.String LotNumber { get; set; }


        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        public string PackSlipNumber { get; set; }



        public Nullable<bool> IsEDISent { get; set; }
        public Nullable<System.DateTime> LastEDIDate { get; set; }


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

        private string _strReceivedDt;
        public string strReceivedDate
        {
            get
            {
                if (string.IsNullOrEmpty(_strReceivedDt))
                {
                    _strReceivedDt = FnCommon.ConvertDateByTimeZone(ReceivedDate, false, true);
                }
                return _strReceivedDt;
            }
            set { this._strReceivedDt = value; }
        }


        public bool IsItemConsignment { get; set; }
        public string ControlNumber { get; set; }

        public string ToolTypeTracking { get; set; }
        public string ToolAssetOrderNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public int ToolAssetOrderStatus { get; set; }
        public Guid ToolAssetOrderGuid { get; set; }

        public bool SerialNumberTracking { get; set; }

        public string Description { get; set; }
    }
}
