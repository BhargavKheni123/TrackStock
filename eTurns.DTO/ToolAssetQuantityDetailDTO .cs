using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    [Serializable]
    public class ToolAssetQuantityDetailDTO
    {

        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public virtual System.Int64 ID { get; set; }

        //GUID
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        //Tool Guid
        [Display(Name = "GUID", ResourceType = typeof(ResToolMaster))]
        public Nullable<Guid> ToolGUID { get; set; }
        public Nullable<Int64> LocationID { get; set; }





        [Display(Name = "Cost", ResourceType = typeof(ResToolMaster))]
        public Nullable<System.Double> Cost { get; set; }



        //Asset Guid
        [Display(Name = "GUID", ResourceType = typeof(ResToolMaster))]
        public Nullable<Guid> AssetGUID { get; set; }


        //Tool Location Name
        [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
        public Nullable<Int64> ToolBinID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }


        [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
        public string Location { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(ResToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double Quantity { get; set; }



        //Tool Room ID
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Int64 RoomID { get; set; }

        //Tool Room
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Int64 CompanyID { get; set; }

        //Tool Room
        [Display(Name = "CompanyName", ResourceType = typeof(ResCommon))]
        public string CompanyName { get; set; }

        //Tool Created On Date
        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Created { get; set; }

        public string CreatedDt { get; set; }
        public string UpdatedDt { get; set; }

        //Tool Update On Date
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Updated { get; set; }

        //Created By ID
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Int64 CreatedBy { get; set; }

        //Created By ID
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Int64 UpdatedBy { get; set; }

        //Created By Name
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        //Updated By Name
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

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

        public System.String WhatWhereAction { get; set; }

        [Display(Name = "ReceivedDate", ResourceType = typeof(ResToolAsset))]
        public Nullable<System.DateTime> ReceivedDate { get; set; }


        [Display(Name = "InitialQuantityWeb", ResourceType = typeof(ResToolAsset))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double InitialQuantityWeb { get; set; }

        [Display(Name = "InitialQuantityPDA", ResourceType = typeof(ResToolAsset))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double InitialQuantityPDA { get; set; }

        [Display(Name = "ExpirationDate", ResourceType = typeof(ResToolAsset))]
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        public System.String EditedOnAction { get; set; }
        //Tool IsArchived
        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public bool IsArchived { get; set; }

        //Tool IsDeleted
        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public bool IsDeleted { get; set; }


        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public System.String UDF5 { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ResToolAssetQuantityDetail))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Description { get; set; }

        public string ToolName { get; set; }
        public string Serial { get; set; }
        public int? Type { get; set; }
        public Guid? ToolAssetOrderDetailGUID { get; set; }
        public string PackSlipNumber { get; set; }

        private double _DispQuantity;
        public double DispQuantity
        {
            get
            {
                if (_DispQuantity == 0)
                {
                    _DispQuantity = Quantity;
                }
                return _DispQuantity;
            }
            set { this._DispQuantity = value; }
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

        public string ErrorMessege { get; set; }

        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }

        public bool SerialNumberTracking { get; set; }
        public bool LotNumberTracking { get; set; }
        public bool DateCodeTracking { get; set; }
        public Guid? ToolDetailsGUID { get; set; }

        public string AvailableInWIP { get; set; }
        public Guid? LocationGUID { get; set; }

        public string IsLocSerialAvailable { get; set; }

        public System.Double AvailableQuantity { get; set; }

        public double? QtyConsumable { get; set; }

        public double MoveQuantity { get; set; }
    }


    public class ToolQuantityLotSerialDTO
    {
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> BinID { get; set; }
        public Nullable<System.Double> Quantity { get; set; }
        public Nullable<System.Double> QuantityEntry { get; set; }
        public System.String LotNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string Expiration { get; set; }
        public string Received { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Double> PullCost { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }
        public string Location { get; set; }
        public Int32 ToolType { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
        public Nullable<System.Guid> ToolAssetOrderDetailGUID { get; set; }
        public Nullable<System.Guid> ToolKitDetailGUID { get; set; }
        public Boolean IsCreditPull { get; set; }

        public bool IsDefault { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public long? Room { get; set; }
        public bool IsConsignedLotSerial { get; set; }
        public double LotSerialQuantity { get; set; }
        public string LotOrSerailNumber { get; set; }
        public double PullQuantity { get; set; }
        public double? CumulativeTotalQuantity { get; set; }
        public bool IsSelected { get; set; }
        public string ValidationMessage { get; set; }
        public double TobePulled { get; set; }
        public double TotalTobePulled { get; set; }
        public double TotalPullCost { get; set; }
        public double QuantityToMove { get; set; }
        public Guid ToolAssetQuantityGUID { get; set; }
        public string SerialLotExpirationcombin { get; set; }
        public string strExpirationDate { get; set; }
    }
    public class ToolAssetPullInfo
    {
        public long EnterpriseId { get; set; }
        public long RoomId { get; set; }
        public long CompanyId { get; set; }
        public long BinID { get; set; }
        public string BinNumber { get; set; }
        public double PullQuantity { get; set; }
        public long LastUpdatedBy { get; set; }
        public Guid? PullGUID { get; set; }
        public long CreatedBy { get; set; }
        public double PullCost { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool CanOverrideProjectLimits { get; set; }
        public bool ValidateProjectSpendLimits { get; set; }
        public List<ToolLocationLotSerialDTO> lstToolPullDetails { get; set; }
        public Guid? RequisitionDetailsGUID { get; set; }
        public Guid? WorkOrderItemGUID { get; set; }
        public List<PullToolAssetErrorInfo> ErrorList { get; set; }
        public double TotalCustomerOwnedTobePulled { get; set; }
        public double TotalConsignedTobePulled { get; set; }
        public Guid? WorkOrderDetailGUID { get; set; }

        public Nullable<System.Guid> ToolGUID { get; set; }
        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }
        public Nullable<System.Guid> TechnicianGUID { get; set; }

        public string ToolName { get; set; }
        public string Technician { get; set; }
        public bool IsMaintenance { get; set; }
        public Guid? ToolDetailsGUID { get; set; }
        public string SerialNumber { get; set; }

    }
    public class ToolLocationLotSerialDTO
    {
        public System.Int64 ID { get; set; }
        public Nullable<System.Int64> BinID { get; set; }
        public Nullable<System.Double> Quantity { get; set; }
        public Nullable<System.Double> QuantityEntry { get; set; }
        public System.String LotNumber { get; set; }
        public System.String SerialNumber { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string Expiration { get; set; }
        public string Received { get; set; }
        public Nullable<System.Double> Cost { get; set; }
        public Nullable<System.Double> PullCost { get; set; }
        public Guid GUID { get; set; }
        public Nullable<Guid> ToolGUID { get; set; }
        public string Location { get; set; }
        public Int32 ToolType { get; set; }
        public Boolean SerialNumberTracking { get; set; }
        public Boolean LotNumberTracking { get; set; }
        public Boolean DateCodeTracking { get; set; }
        public Nullable<System.Guid> ToolAssetOrderDetailGUID { get; set; }
        public Boolean IsCreditPull { get; set; }
        public Nullable<Guid> TechnicianGUID { get; set; }
        public Nullable<Guid> WorkOrderGUID { get; set; }
        public string ToolName { get; set; }
        public long? Room { get; set; }
        public double LotSerialQuantity { get; set; }
        public string LotOrSerailNumber { get; set; }
        public double PullQuantity { get; set; }
        public double? CumulativeTotalQuantity { get; set; }
        public bool IsSelected { get; set; }
        public string ValidationMessage { get; set; }
        public double TobePulled { get; set; }
        public double TotalTobePulled { get; set; }
        public double TotalPullCost { get; set; }
        public double QuantityToMove { get; set; }
        public Guid ToolAssetQuantityDetailGUID { get; set; }
        public string SerialLotExpirationcombin { get; set; }
        public string strExpirationDate { get; set; }
    }
    public class PullToolAssetErrorInfo
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ResToolAsset
    {


        private static string resourceFile = "ResToolAsset";



        /// <summary>
        ///   Looks up a localized string similar to Cost.
        /// </summary>
        public static string ReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedDate", resourceFile);
            }
        }
        public static string BinID
        {
            get
            {
                return ResourceRead.GetResourceValue("BinID", resourceFile);
            }
        }
        public static string InitialQuantityWeb
        {
            get
            {
                return ResourceRead.GetResourceValue("InitialQuantityWeb", resourceFile);
            }
        }
        public static string InitialQuantityPDA
        {
            get
            {
                return ResourceRead.GetResourceValue("InitialQuantityPDA", resourceFile);
            }
        }
        public static string ExpirationDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ExpirationDate", resourceFile);
            }
        }


    }


    public class ResToolAssetQuantityDetail
    {

        private static string ResourceFileName = "ResToolAssetQuantityDetails";
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
        ///   Looks up a localized string similar to SerialNumber.
        /// </summary>
        public static string SerialNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumber", ResourceFileName);
            }
        }

        public static string LotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumber", ResourceFileName);
            }
        }

        //
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", ResourceFileName);
            }
        }

        public static string ReceivedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedDate", ResourceFileName);
            }
        }

        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
            }
        }

        public static string Created
        {
            get
            {
                return ResourceRead.GetResourceValue("Created", ResourceFileName);
            }
        }

        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", ResourceFileName);
            }
        }

        //ReceiveBin
        public static string ReceiveBin
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceiveBin", ResourceFileName);
            }
        }

        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", ResourceFileName);
            }
        }

    }
}
