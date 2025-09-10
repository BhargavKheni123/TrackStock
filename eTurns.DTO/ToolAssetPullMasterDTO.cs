using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    public class ToolAssetPullMasterDTO : ToolMasterDTO
    {
        //ID
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public new System.Int64 ID { get; set; }



        //CustomerOwnedQuantity
        [Display(Name = "Quantity", ResourceType = typeof(ResToolAssetPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public new Nullable<System.Double> Quantity { get; set; }



        //PoolQuantity
        [Display(Name = "PullQuantity", ResourceType = typeof(ResToolAssetPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> PullQuantity { get; set; }


        public Nullable<System.Double> PULLCost { get; set; }



        //CreditCustomerOwnedQuantity
        [Display(Name = "CreditQuantity", ResourceType = typeof(ResToolAssetPullMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> CreditQuantity { get; set; }


        //BinID
        [Display(Name = "BinID", ResourceType = typeof(ResToolAssetPullMaster))]
        public new Nullable<System.Int64> BinID { get; set; }

        //UDF1
        [Display(Name = "UDF1", ResourceType = typeof(ResToolAssetPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF1 { get; set; }

        //UDF2
        [Display(Name = "UDF2", ResourceType = typeof(ResToolAssetPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF2 { get; set; }

        //UDF3
        [Display(Name = "UDF3", ResourceType = typeof(ResToolAssetPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF3 { get; set; }

        //UDF4
        [Display(Name = "UDF4", ResourceType = typeof(ResToolAssetPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF4 { get; set; }

        //UDF5
        [Display(Name = "UDF5", ResourceType = typeof(ResToolAssetPullMaster))]
        [StringLength(255, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [AllowHtml]
        public new System.String UDF5 { get; set; }

        //GUID
        public new Guid GUID { get; set; }

        //ItemGUID
        [Display(Name = "ToolGUID", ResourceType = typeof(ResToolAssetPullMaster))]
        public Nullable<Guid> ToolGUID { get; set; }

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
        public Nullable<System.Int64> RoomID { get; set; }

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
        public long? ToolID { get; set; }
        public IEnumerable<ToolLocationDetailsDTO> lstItemLocationDetails { get; set; }

        //Get set of whole item 
        //public ItemMasterDTO PullItem { get; set; }


        [Display(Name = "ReceivedOn", ResourceType = typeof(ResCommon))]
        public new Nullable<System.DateTime> ReceivedOn { get; set; }

        [Display(Name = "ReceivedOnWeb", ResourceType = typeof(ResCommon))]
        public new Nullable<System.DateTime> ReceivedOnWeb { get; set; }

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
        public string ControlNumber { get; set; }

        public new bool SerialNumberTracking { get; set; }
        public new bool LotNumberTracking { get; set; }
        public new bool DateCodeTracking { get; set; }
        public bool IsForMaintenance { get; set; }


        #region Tools Properties Used in new Pull Popup


        public string ToolCheckoutUDF1 { get; set; }
        public string ToolCheckoutUDF2 { get; set; }
        public string ToolCheckoutUDF3 { get; set; }
        public string ToolCheckoutUDF4 { get; set; }
        public string ToolCheckoutUDF5 { get; set; }



        public Nullable<Guid> WorkOrderDetailGUID { get; set; }


        public Nullable<Guid> RequisitionDetailGUID { get; set; }
        #endregion


    }

    public class ResToolAssetPullMaster
    {
        private static string ResourceFileName = "ResToolAssetPullMaster";


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
        public static string ToolID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolID", ResourceFileName);
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
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", ResourceFileName);
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


        /// <summary>
        ///   Looks up a localized string similar to PoolQuantity.
        /// </summary>
        public static string PullQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("PullQuantity", ResourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to CreditCustomerOwnedQuantity.
        /// </summary>
        public static string CreditQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("CreditQuantity", ResourceFileName);
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
        public static string ToolGUID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolGUID", ResourceFileName);
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
        public static string RoomID
        {
            get
            {
                return ResourceRead.GetResourceValue("RoomID", ResourceFileName);
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



        public static string msgQuantityNotAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("msgQuantityNotAvailable", ResourceFileName);
            }
        }


        public static string PullAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("PullAllButton", ResourceFileName);
            }
        }

        public static string QuantityPull
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityPull", ResourceFileName);
            }
        }



        public static string QuantityCredit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityCredit", ResourceFileName);
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

    }

    [Serializable]
    public class ToolAssetPullMasterViewDTO
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

        [Display(Name = "ItemNumber", ResourceType = typeof(ResProjectMaster))]
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
        public string Serial { get; set; }
        public Guid? ToolGUID { get; set; }

        public string ConsignmentName { get; set; }

        public string ItemBlanketPO { get; set; }
    }



    public class ToolInfoToMSCredit
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

    }

    public class ToolInfoToCredit
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
    }
    public class ToolAssetPullDetailToCredit
    {
        public string Serial { get; set; }
        public string Lot { get; set; }
        public string ExpireDate { get; set; }
        public double Qty { get; set; }
    }

    public class ToolAssetPullDetailToMSCredit
    {
        public string Serial { get; set; }
        public string Lot { get; set; }
        public string ExpireDate { get; set; }
        public double Qty { get; set; }
    }

    public class ToolInfoToPull
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

    }


    public class RPT_ToolAssetPullMasterDTO
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
    }





}


