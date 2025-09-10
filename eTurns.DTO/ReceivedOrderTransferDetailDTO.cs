using eTurns.DTO.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace eTurns.DTO
{

    public class OrderReturnQuantityDetail
    {
        public double ReturnQuantity { get; set; }
        public Guid ItemGUID { get; set; }
        public Guid OrderDetailGUID { get; set; }
        public Int64 LocationID { get; set; }
        public string LocationName { get; set; }
        public bool IsStaging { get; set; }
    }

    public class ReceivedOrderTransferDetailDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Int64 ID { get; set; }


        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int64> BinID { get; set; }

        //CustomerOwnedQuantity
        [Display(Name = "CustomerOwnedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CustomerOwnedQuantity { get; set; }

        //ConsignedQuantity
        [Display(Name = "ConsignedQuantity", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> ConsignedQuantity { get; set; }

        //MeasurementID
        [Display(Name = "MeasurementID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int32> MeasurementID { get; set; }

        //LotNumber
        [Display(Name = "LotNumber", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String LotNumber { get; set; }

        //SerialNumber
        [Display(Name = "SerialNumber", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String SerialNumber { get; set; }

        //ExpirationDate
        [Display(Name = "ExpirationDate", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.DateTime> ReceivedDate { get; set; }


        //ExpirationDate
        [Display(Name = "ExpirationDate", ResourceType = typeof(ResItemLocationDetails))]
        public string Expiration { get; set; }

        //ReceivedDate
        [Display(Name = "ReceivedDate", ResourceType = typeof(ResItemLocationDetails))]
        public string Received { get; set; }

        //Cost
        [Display(Name = "Cost", ResourceType = typeof(ResItemLocationDetails))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> Cost { get; set; }

        //eVMISensorPort
        [Display(Name = "eVMISensorPort", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<System.Int32> eVMISensorPort { get; set; }

        //eVMISensorID
        [Display(Name = "eVMISensorID", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String eVMISensorID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        //UDF6
        [Display(Name = "UDF6", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF6 { get; set; }

        //UDF7
        [Display(Name = "UDF7", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF7 { get; set; }

        //UDF8
        [Display(Name = "UDF8", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF8 { get; set; }

        //UDF9
        [Display(Name = "UDF9", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF9 { get; set; }

        //UDF10
        [Display(Name = "UDF10", ResourceType = typeof(ResItemLocationDetails))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF10 { get; set; }

        //GUID
        public Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "ItemGUID", ResourceType = typeof(ResItemLocationDetails))]
        public Nullable<Guid> ItemGUID { get; set; }

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
        [Display(Name = "ItemNumber", ResourceType = typeof(ResItemMaster))]
        public System.String ItemNumber { get; set; }

        [Display(Name = "BinNumber", ResourceType = typeof(ResBin))]
        public string BinNumber { get; set; }

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

        // added by niraj
        public Int32 ItemType { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }

        public string mode { get; set; }
        //OrderDetailID
        public Nullable<System.Guid> OrderDetailGUID { get; set; }
        public Nullable<System.Guid> KitDetailGUID { get; set; }
        public Nullable<System.Guid> TransferDetailGUID { get; set; }
        public Nullable<System.Guid> ItemLocationDetailGUID { get; set; }

        public Boolean IsCreditPull { get; set; }
        public bool IsOnlyFromUI { get; set; }

        //CriticalQuantity
        [Display(Name = "CriticalQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CriticalQuantity { get; set; }

        //MinimumQuantity
        [Display(Name = "MinimumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MinimumQuantity { get; set; }

        //MaximumQuantity
        [Display(Name = "MaximumQuantity", ResourceType = typeof(ResItemMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> MaximumQuantity { get; set; }

        [Display(Name = "PackSlipNumber", ResourceType = typeof(ResOrder))]
        public string PackSlipNumber { get; set; }



        public Nullable<bool> IsEDISent { get; set; }
        public Nullable<System.DateTime> LastEDIDate { get; set; }

        private string _expDate;
        public string ExpirationDateStr
        {
            get
            {
                if (string.IsNullOrEmpty(_expDate))
                {
                    _expDate = FnCommon.ConvertDateByTimeZone(ExpirationDate, false, true);
                }
                return _expDate;
            }
            set { this._expDate = value; }
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

        private string _ReceivedDateFromUTC;
        public string ReceivedDateFromUTC
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedDateFromUTC))
                {
                    _ReceivedDateFromUTC = FnCommon.ConvertDateByTimeZone(ReceivedDate, true);
                }
                return _ReceivedDateFromUTC;
            }
            set { this._ReceivedDateFromUTC = value; }
        }

        private string _strExpirationDt;
        public string strExpirationDate
        {
            get
            {
                if (string.IsNullOrEmpty(_strExpirationDt))
                {
                    _strExpirationDt = FnCommon.ConvertDateByTimeZone(ExpirationDate, false, true);
                }
                return _strExpirationDt;
            }
            set { this._strExpirationDt = value; }
        }
        public bool IsItemConsignment { get; set; }
        public string ControlNumber { get; set; }

        public double Quantity { get; set; }
        public string OrderNumber { get; set; }
        public string ReleaseNumber { get; set; }
        public int OrderStatus { get; set; }
        public Guid OrderGuid { get; set; }

        public Int64? OrderUOMID { get; set; }
        public int OrderUOMValue { get; set; }
        public Boolean IsAllowOrderCostuom { get; set; }
        public int? OrderType { get; set; }


        private string _ReceivedDateOrig;
        public string ReceivedDateOrig
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedDateOrig))
                {
                    _ReceivedDateOrig = FnCommon.ConvertDateByTimeZone(ReceivedDate, true, true);
                }
                return _ReceivedDateOrig;
            }
            set { this._ReceivedDateOrig = value; }
        }

        public Nullable<bool> IsReceivedCostChange { get; set; }

        public Nullable<Int32> POLineItemNumber { get; set; }
        public long OrderDetailTrackingID { get; set; }
    }

    public class ReceivedOrderOrderDetails
    {
        public string OrderDetailsGuid { get; set; }
        public string ReceivedDetailGuid { get; set; }
    }
}
