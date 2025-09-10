using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    //Model Tool Master
    //02-Nov-2012

    public class ToolMasterDTO
    {
        // Tool ID
        public Int64 ID { get; set; }

        //Tool Name
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "ToolName", ResourceType = typeof(ResToolMaster))]
        [AllowHtml]
        public string ToolName { get; set; }


        //Tool Serial
        // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "Serial", ResourceType = typeof(ResToolMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Serial { get; set; }

        //Tool Description
        [Display(Name = "Description", ResourceType = typeof(ResToolMaster))]
        [StringLength(1024, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string Description { get; set; }

        //Tool Value
        [Display(Name = "Cost", ResourceType = typeof(ResToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public double? Cost { get; set; }

        //Tool Checked Out?
        [Display(Name = "IsCheckedOut", ResourceType = typeof(ResToolMaster))]
        public Nullable<bool> IsCheckedOut { get; set; }

        //Tool Category ID
        [Display(Name = "ToolCategoryID", ResourceType = typeof(ResToolMaster))]
        public Nullable<Int64> ToolCategoryID { get; set; }

        //Tool Category Name
        [Display(Name = "ToolCategory", ResourceType = typeof(ResToolMaster))]
        public string ToolCategory { get; set; }

        //Tool Created On Date
        [Display(Name = "CreatedOn", ResourceType = typeof(ResCommon))]
        public DateTime Created { get; set; }

        //Tool Update On Date
        [Display(Name = "UpdatedOn", ResourceType = typeof(ResCommon))]
        public Nullable<DateTime> Updated { get; set; }

        //Created By ID
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CreatedBy { get; set; }

        //Created By ID
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> LastUpdatedBy { get; set; }

        //Created By Name
        [Display(Name = "CreatedBy", ResourceType = typeof(ResCommon))]
        public string CreatedByName { get; set; }

        //Updated By Name
        [Display(Name = "UpdatedBy", ResourceType = typeof(ResCommon))]
        public string UpdatedByName { get; set; }

        //Tool Room ID
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> Room { get; set; }

        //Tool Room
        [Display(Name = "RoomName", ResourceType = typeof(ResCommon))]
        public string RoomName { get; set; }

        //Tool GUID
        [Display(Name = "GUID", ResourceType = typeof(ResCommon))]
        public Guid GUID { get; set; }

        //Tool IsArchived
        [Display(Name = "IsArchived", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsArchived { get; set; }

        //Tool IsDeleted
        [Display(Name = "IsDeleted", ResourceType = typeof(ResCommon))]
        public Nullable<bool> IsDeleted { get; set; }

        //Tool Category Name
        [Display(Name = "LocationID", ResourceType = typeof(ResToolMaster))]
        public Nullable<Int64> LocationID { get; set; }

        //Tool Category Name
        [Display(Name = "TechnicianGuID", ResourceType = typeof(ResToolMaster))]
        public Nullable<Guid> TechnicianGuID { get; set; }



        [Display(Name = "Location", ResourceType = typeof(ResToolMaster))]
        public string Location { get; set; }

        [Display(Name = "Technician", ResourceType = typeof(ResToolMaster))]
        public string Technician { get; set; }

        public int? CheckedOutQTYTotal { get; set; }



        [Display(Name = "CompanyID", ResourceType = typeof(ResCommon))]
        public Nullable<Int64> CompanyID { get; set; }

        [Display(Name = "Action", ResourceType = typeof(Resources.ResCommon))]
        public string Action { get; set; }

        [Display(Name = "HistoryID", ResourceType = typeof(Resources.ResCommon))]
        public Int64 HistoryID { get; set; }

        [Display(Name = "UDF1", ResourceType = typeof(ResToolMaster))]
        public string UDF1 { get; set; }

        [Display(Name = "UDF2", ResourceType = typeof(ResToolMaster))]
        public string UDF2 { get; set; }

        [Display(Name = "UDF3", ResourceType = typeof(ResToolMaster))]
        public string UDF3 { get; set; }

        [Display(Name = "UDF4", ResourceType = typeof(ResToolMaster))]
        public string UDF4 { get; set; }

        [Display(Name = "UDF5", ResourceType = typeof(ResToolMaster))]
        public string UDF5 { get; set; }

        [Display(Name = "UDF6", ResourceType = typeof(ResToolMaster))]
        public string UDF6 { get; set; }

        [Display(Name = "UDF7", ResourceType = typeof(ResToolMaster))]
        public string UDF7 { get; set; }

        [Display(Name = "UDF8", ResourceType = typeof(ResToolMaster))]
        public string UDF8 { get; set; }

        [Display(Name = "UDF9", ResourceType = typeof(ResToolMaster))]
        public string UDF9 { get; set; }

        [Display(Name = "UDF10", ResourceType = typeof(ResToolMaster))]
        public string UDF10 { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(ResToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public System.Double Quantity { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "IsGroupOfItems", ResourceType = typeof(ResToolMaster))]
        public Nullable<Int32> IsGroupOfItems { get; set; }

        [Display(Name = "CheckOutStatus", ResourceType = typeof(ResToolCheckInOutHistory))]
        public string CheckOutStatus { get; set; }
        [Display(Name = "CheckedOutQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutQTY { get; set; }
        [Display(Name = "CheckedOutMQTY", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<System.Double> CheckedOutMQTY { get; set; }
        [Display(Name = "CheckOutDate", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<DateTime> CheckOutDate { get; set; }
        [Display(Name = "CheckInDate", ResourceType = typeof(ResToolCheckInOutHistory))]
        public Nullable<DateTime> CheckInDate { get; set; }
        public Nullable<Int64> CheckInCheckOutID { get; set; }

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

        public string AppendedBarcodeString { get; set; }

        public string ToolUDF1 { get; set; }
        public string ToolUDF2 { get; set; }
        public string ToolUDF3 { get; set; }
        public string ToolUDF4 { get; set; }
        public string ToolUDF5 { get; set; }
        public int Count { get; set; }


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

        [Display(Name = "NoOfPastMntsToConsider", ResourceType = typeof(ResAssetMaster))]
        [Range(2, 10, ErrorMessageResourceName = "NoOfPastMntsToConsider", ErrorMessageResourceType = typeof(ResAssetMaster))]
        public int? NoOfPastMntsToConsider { get; set; }

        public int? MaintenanceDueNoticeDays { get; set; }

        [Display(Name = "MaintenanceType", ResourceType = typeof(ResAssetMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public int MaintenanceType { get; set; }

        public bool IsAutoMaintain { get; set; }

        public List<TechnicianMasterDTO> TechnicianList { get; set; }
        public Nullable<System.Double> CheckedOutQuantity { get; set; }
        public Nullable<System.Double> CheckedInQuantity { get; set; }
        public int DaysDiff { get; set; }
        public System.String ToolImageExternalURL { get; set; }
        public string ImageType { get; set; }

        public System.String ImagePath { get; set; }

        public bool IsBeforeCheckOutAndCheckIn { get; set; }

        public Int64? Type { get; set; }

        [Display(Name = "IsBuildBreak", ResourceType = typeof(ResKitToolMaster))]
        public bool IsBuildBreak { get; set; }


        /********* For Kit Build Break  ****************/
        [Display(Name = "NoOfItemsInKit", ResourceType = typeof(ResKitMaster))]
        public int NoOfItemsInKit { get; set; }

        [Display(Name = "WIPKitCost", ResourceType = typeof(ResKitMaster))]
        public double? WIPKitCost { get; set; }


        /******************************** For kit tool *******************************/
        [Display(Name = "KitToolQuantity", ResourceType = typeof(ResKitToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> KitToolQuantity { get; set; }


        //Tool Name
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "KitToolName", ResourceType = typeof(ResKitToolMaster))]
        [AllowHtml]
        public string KitToolName { get; set; }


        //Tool Serial
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        [Display(Name = "KitToolSerial", ResourceType = typeof(ResKitToolMaster))]
        [StringLength(128, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ResMessage))]
        public string KitToolSerial { get; set; }


        /******************************** For kit tool *******************************/

        //AvailableWIPKit
        [Display(Name = "AvailableWIPKit", ResourceType = typeof(ResKitToolMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Int64> AvailableWIPKit { get; set; }
        [Display(Name = "AvailableInGeneralInventory", ResourceType = typeof(ResKitToolMaster))]
        public double AvailableInGeneralInventory { get; set; }
        [Display(Name = "ReOrderType", ResourceType = typeof(ResKitMaster))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<Boolean> ReOrderType { get; set; }
        //KitCategory
        [Display(Name = "KitCategory", ResourceType = typeof(ResKitMaster))]
        public Nullable<System.Int32> KitCategory { get; set; }
        //AvailableKitQuantity
        [Display(Name = "AvailableKitQuantity", ResourceType = typeof(ResKitMaster))]
        [RegularExpression(@"^[0-9\.]*$", ErrorMessageResourceName = "InvalidValue", ErrorMessageResourceType = typeof(ResMessage))]
        public Nullable<System.Double> AvailableKitQuantity { get; set; }

        public List<ToolDetailDTO> ToolKitItemList { get; set; }

        public bool IsAtleaseOneCheckOutCompleted { get; set; }

        public Nullable<Int64> ToolLocationDetailsID { get; set; }

        public Nullable<System.Double> AvailableToolQty { get; set; }

        [Display(Name = "ToolTypeTracking", ResourceType = typeof(ResToolMaster))]
        public string ToolTypeTracking { get; set; }

        private string _ToolTypeTracking;
        public string ToolTypeTrackingStr
        {
            get
            {
                if (string.IsNullOrEmpty(_ToolTypeTracking))
                {
                    _ToolTypeTracking = FnCommon.GetToolTypeUsingEnum(ToolTypeTracking);
                }
                return _ToolTypeTracking;
            }
            set { this._ToolTypeTracking = value; }
        }
        public Nullable<Int64> DefaultLocation { get; set; }

        public string DefaultLocationName { get; set; }

        public Nullable<Int64> BinID { get; set; }

        //SerialNumberTracking
        [Display(Name = "SerialNumberTracking", ResourceType = typeof(ResToolMaster))]
        public Boolean SerialNumberTracking { get; set; }

        //LotNumberTracking
        [Display(Name = "LotNumberTracking", ResourceType = typeof(ResToolMaster))]
        public Boolean LotNumberTracking { get; set; }

        //DateCodeTracking
        [Display(Name = "DateCodeTracking", ResourceType = typeof(ResToolMaster))]
        public Boolean DateCodeTracking { get; set; }
        public System.String WhatWhereAction { get; set; }

        public string IsSerialAvailable { get; set; }

        public string IsCheckOutSerialAvailable { get; set; }
        public Guid? ToolCheckoutGUID { get; set; }

        public double LocationQty { get; set; }


        public Int64 ToolKitDetailID { get; set; }
        public Guid? ToolKitDetailGUID { get; set; }
        public Guid? ToolKitGuid { get; set; }
        public string KitPartNumber { get; set; }

        public string TechnicianCode { get; set; }
        public string ToolNameSerial { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class RPT_ToolsCheckOut
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string ToolCategory { get; set; }
        public string Location { get; set; }
        public string ToolUDF1 { get; set; }
        public string ToolUDF2 { get; set; }
        public string ToolUDF3 { get; set; }
        public string ToolUDF4 { get; set; }
        public string ToolUDF5 { get; set; }
        public string Technician { get; set; }
        public string CheckoutUDF1 { get; set; }
        public string CheckoutUDF2 { get; set; }
        public string CheckoutUDF3 { get; set; }
        public string CheckoutUDF4 { get; set; }
        public string CheckoutUDF5 { get; set; }
        public string TechnicianUDF1 { get; set; }
        public string TechnicianUDF2 { get; set; }
        public string TechnicianUDF3 { get; set; }
        public string TechnicianUDF4 { get; set; }
        public string TechnicianUDF5 { get; set; }
        public Guid ModuleItemGuid { get; set; }
        public string WorkOrder { get; set; }


    }

    public class RPT_Tools
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }
        public string Serial { get; set; }
        public string ToolCategory { get; set; }
        public string Location { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string Technician { get; set; }
        public string CheckoutUDF1 { get; set; }
        public string CheckoutUDF2 { get; set; }
        public string CheckoutUDF3 { get; set; }
        public string CheckoutUDF4 { get; set; }
        public string CheckoutUDF5 { get; set; }
        public Guid ModuleItemGuid { get; set; }
        public string WorkOrder { get; set; }
        public Nullable<Int64> ToolCategoryID { get; set; }
        public Nullable<Int64> LocationID { get; set; }


    }

    public class RPT_ToolMaintanance
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }
        public string Workorder { get; set; }
        public string Maintenance { get; set; }
        public string Serial { get; set; }
        public string ToolCategory { get; set; }
        public string Location { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }



    }

    public class RPT_MaintananceDue
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public string ToolName { get; set; }
        public string AssetName { get; set; }
        public string Maintenance { get; set; }
        public string MaintenanceStatus { get; set; }
    }
    public class ResToolMaster
    {


        private static string resourceFile = "ResToolMaster";



        /// <summary>
        ///   Looks up a localized string similar to Cost.
        /// </summary>
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", resourceFile);
            }
        }
        public static string Type
        {
            get
            {
                return ResourceRead.GetResourceValue("Type", resourceFile);
            }
        }
        public static string QuantityPerKit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityPerKit", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", resourceFile);
            }
        }

        public static string QuantityTexBox
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityTexBox", resourceFile);
            }
        }
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", resourceFile);
            }
        }

        public static string ForMaintanence
        {
            get
            {
                return ResourceRead.GetResourceValue("ForMaintanence", resourceFile);
            }
        }

        public static string IsGroupOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("IsGroupOfItems", resourceFile);
            }
        }

        public static string IsGrpOfItemReq
        {
            get
            {
                return ResourceRead.GetResourceValue("IsGrpOfItemReq", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked Out.
        /// </summary>
        public static string IsCheckedOut
        {
            get
            {
                return ResourceRead.GetResourceValue("IsCheckedOut", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enter proper value..
        /// </summary>
        public static string IvalidCost
        {
            get
            {
                return ResourceRead.GetResourceValue("IvalidCost", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tools.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string DefaultLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("DefaultLocation", resourceFile);
            }
        }
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }

        
        public static string LocationID
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }

        public static string ToolImage
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolImage", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Technician.
        /// </summary>
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Tools.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string Checkedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Checkedout", resourceFile);
            }
        }
        public static string CheckedouttoMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedouttoMaintenance", resourceFile);
            }
        }
        public static string Groupfullycheckedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Groupfullycheckedout", resourceFile);
            }
        }
        public static string Nonecheckedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Nonecheckedout", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Serial.
        /// </summary>
        public static string Serial
        {
            get
            {
                return ResourceRead.GetResourceValue("Serial", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool Category.
        /// </summary>
        public static string ToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategory", resourceFile);
            }
        }
        public static string ToolCheckoutAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckoutAllButton", resourceFile);
            }
        }
        public static string ToolCheckInAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInAllButton", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Tool CategoryID.
        /// </summary>
        public static string ToolCategoryID
        {
            get
            {
                //return ResourceRead.GetResourceValue("ToolCategoryID", resourceFile);
                return ResourceRead.GetResourceValue("ToolCategory", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool.
        /// </summary>
        public static string ToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolName", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string CheckedInBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedInBy", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string CheckedOutBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutBy", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string UpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("Updatedby", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string UpdatedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdatedDate", resourceFile);
            }
        }

        public static string ImagePath
        {
            get
            {
                return ResourceRead.GetResourceValue("ImagePath", resourceFile);
            }
        }

        public static string ToolImageExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolImageExternalURL", resourceFile);
            }
        }

        public static string NoOfPastMntsToConsider
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfPastMntsToConsider", resourceFile);
            }

        }

        public static string MaintenanceDueNoticeDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDueNoticeDays", resourceFile);
            }
        }
        public static string ToolTypeTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolTypeTracking", resourceFile);
            }
        }
        public static string LocationSerialsExpand
        {
            get
            {
                return ResourceRead.GetResourceValue("LocationSerialsExpand", resourceFile);
            }
        }
        public static string ToolType
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolType", resourceFile);
            }
        }

        public static string SerialNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialNumberTracking", resourceFile);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to LotNumberTracking.
        /// </summary>
        public static string LotNumberTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("LotNumberTracking", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DateCodeTracking.
        /// </summary>
        public static string DateCodeTracking
        {
            get
            {
                return ResourceRead.GetResourceValue("DateCodeTracking", resourceFile);
            }
        }

        /// <summary>
        /// Image
        /// </summary>
        public static string Image
        {
            get
            {
                return ResourceRead.GetResourceValue("Image", resourceFile);
            }
        }

        /// <summary>
        /// Upload
        /// </summary>
        public static string Upload
        {
            get
            {
                return ResourceRead.GetResourceValue("Upload", resourceFile);
            }
        }

        /// <summary>
        /// Tool Certification Images
        /// </summary>
        public static string ToolCertificationImages
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCertificationImages", resourceFile);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        public static string Delete
        {
            get
            {
                return ResourceRead.GetResourceValue("Delete", resourceFile);
            }
        }

        /// <summary>
        /// File Name
        /// </summary>
        public static string FileName
        {
            get
            {
                return ResourceRead.GetResourceValue("FileName", resourceFile);
            }
        }

        public static string CheckOut
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckOut", resourceFile);
            }
        }

        public static string WrittenOffTool
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffTool", resourceFile);
            }
        }
        public static string WrittenOffCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffCategory", resourceFile);
            }
        }

        public static string WrittenOffDescription
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffDescription", resourceFile);
            }
        }

        public static string ToolWrittenOffSuccessfully
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolWrittenOffSuccessfully", resourceFile);
            }
        }

        public static string FailToWrittenOffTool
        {
            get
            {
                return ResourceRead.GetResourceValue("FailToWrittenOffTool", resourceFile);
            }
        }

        public static string SerialSelectionCountMisMatch
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialSelectionCountMisMatch", resourceFile);
            }
        }

        public static string InvalidQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidQuantity", resourceFile);
            }
        }
        public static string RequiredWrittenoffCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("RequiredWrittenoffCategory", resourceFile);
            }
        }

        public static string QuantityGreaterThanAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityGreaterThanAvailableQty", resourceFile);
            }
        }

        public static string SerialsNotAbleToWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialsNotAbleToWrittenOff", resourceFile);
            }
        }

        public static string Move
        {
            get
            {
                return ResourceRead.GetResourceValue("Move", resourceFile);
            }
        }

        //
        public static string TrackingDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("TrackingDetails", resourceFile);
            }
        }

        public static string ToolWrittenOffAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolWrittenOffAllButton", resourceFile);
            }
        }

        public static string WrittenOffQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffQuantity", resourceFile);
            }
        }

        public static string AvailableQtyCantBeLessThanOne
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableQtyCantBeLessThanOne", resourceFile);
            }
        }

        public static string QtyNotAvailableToWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyNotAvailableToWrittenOff", resourceFile);
            }
        }

        public static string WrittenOffOnlyAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOffOnlyAvailableQty", resourceFile);
            }
        }

        public static string NotEnoughQtyToWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("NotEnoughQtyToWrittenOff", resourceFile);
            }
        }

        public static string Includestockedouttools
        {
            get
            {
                return ResourceRead.GetResourceValue("Includestockedouttools", resourceFile);
            }
        }

        public static string ReassignTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("ReassignTechnician", resourceFile);
            }
        }

        public static string UnwrittenOffTool
        {
            get
            {
                return ResourceRead.GetResourceValue("UnwrittenOffTool", resourceFile);
            }
        }
        public static string QuantityGreaterThancheckedOutQty
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityGreaterThancheckedOutQty", resourceFile);
            }
        }
        public static string InvalidURL
        {
            get
            {
                return ResourceRead.GetResourceValue("InvalidURL", resourceFile);
            }
        }
        public static string ValidFileName
        {
            get
            {
                return ResourceRead.GetResourceValue("ValidFileName", resourceFile);
            }
        }
        public static string msgtoviewScheduleList
        {
            get
            {
                return ResourceRead.GetResourceValue("msgtoviewScheduleList", resourceFile);
            }
        }

        public static string MsgToolDoesNotExist
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgToolDoesNotExist", resourceFile);
            }
        }
         public static string MsgRecordsNotFullyCheckin
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRecordsNotFullyCheckin", resourceFile);
            }
        }

        public static string MsgCheckoutWithQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutWithQty", resourceFile);
            }
        }

        public static string MsgInvalidToolGuid
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidToolGuid", resourceFile);
            }
        }

        public static string MsgCheckInWithQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckInWithQty", resourceFile);
            }
        }

        public static string MsgInvalidTechnicianWithName
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInvalidTechnicianWithName", resourceFile);
            }
        }

        public static string MsgCheckoutQtyExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutQtyExceed", resourceFile);
            }
        }

        public static string MsgCheckInQtyExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckInQtyExceed", resourceFile);
            }
        }
        public static string MsgTotalCheckInQtyExceed
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgTotalCheckInQtyExceed", resourceFile);
            }
        }
        public static string MsgCheckOutNotFound
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckOutNotFound", resourceFile);
            }
        }
        public static string SchedulerTypeToolOrAsset
        {
            get
            {
                return ResourceRead.GetResourceValue("SchedulerTypeToolOrAsset", resourceFile);
            }
        }
        public static string SerialRequiredForScheduleMapping
        {
            get
            {
                return ResourceRead.GetResourceValue("SerialRequiredForScheduleMapping", resourceFile);
            }
        }
        public static string ToolRequiredForScheduleMapping
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolRequiredForScheduleMapping", resourceFile);
            }
        }
        public static string AssetNameReqForScheduleMapping
        {
            get
            {
                return ResourceRead.GetResourceValue("AssetNameReqForScheduleMapping", resourceFile);
            }
        }

        public static string MsgNoQuantityForCheckout
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoQuantityForCheckout", resourceFile);
            }
        }
        public static string MsgCheckoutOnlyAvailableQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutOnlyAvailableQuantity", resourceFile);
            }
        }
        public static string MsgCheckoutDoneSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutDoneSuccess", resourceFile);
            }
        }
        public static string MsgPullMoreQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullMoreQuantityValidation", resourceFile);
            }
        }
        public static string MsgEnteredPullQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnteredPullQuantityValidation", resourceFile);
            }
        }
        public static string MsgNoPullReasons
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgNoPullReasons", resourceFile);
            }
        }
        public static string MsgCheckoutDoneSucess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutDoneSucess", resourceFile);
            }
        }
        public static string MsgAllCheckoutDoneSucess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgAllCheckoutDoneSucess", resourceFile);
            }
        }
        public static string MsgPullDoneSuccess
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullDoneSuccess", resourceFile);
            }
        }
        public static string MsgCheckoutMoreQuantityValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgCheckoutMoreQuantityValidation", resourceFile);
            }
        }
        public static string MsgPullCreditQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgPullCreditQuantity", resourceFile);
            }
        }
        public static string MsgRowShouldExists
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgRowShouldExists", resourceFile);
            }
        }
        public static string MsgDuplicateLotNumber
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgDuplicateLotNumber", resourceFile);
            }
        }
        public static string MsgStagingHeaderAvailable
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgStagingHeaderAvailable", resourceFile);
            }
        }
        public static string MsgSelectStagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectStagingHeader", resourceFile);
            }
        }
        public static string MsgSelectRecordForScheduleList
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectRecordForScheduleList", resourceFile);
            }
        }
        public static string MsgSelectToolKitTypeOnly
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectToolKitTypeOnly", resourceFile);
            }
        }
        public static string MsgSelectToolToCheckIn
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectToolToCheckIn", resourceFile);
            }
        }
        public static string MsgSelectToolMainGridCheckIn
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectToolMainGridCheckIn", resourceFile);
            }
        }
        public static string MsgEnterQuantityToCheckIn
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgEnterQuantityToCheckIn", resourceFile);
            }
        }
        public static string MsgInsertQuantityCheckOut
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInsertQuantityCheckOut", resourceFile);
            }
        }
        public static string MsgSelectLocationCheckout
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgSelectLocationCheckout", resourceFile);
            }
        }
        public static string MsgQtyCheckOutMandatory
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyCheckOutMandatory", resourceFile);
            }
        }
        public static string MsgQTYCheckOutQTYValidation
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQTYCheckOutQTYValidation", resourceFile);
            }
        }
        public static string ReqToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqToolCategory", resourceFile);
            }
        }
        public static string ReqTechnician
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqTechnician", resourceFile);
            }
        }
        public static string MsgInsertProperQuantityValue
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgInsertProperQuantityValue", resourceFile);
            }
        }
        public static string MsgQtyGreaterThanCheckedOutQty
        {
            get
            {
                return ResourceRead.GetResourceValue("MsgQtyGreaterThanCheckedOutQty", resourceFile);
            }
        }

        public static string ToolCheckOutSuccessfully { get { return ResourceRead.GetResourceValue("ToolCheckOutSuccessfully", resourceFile); } }
        public static string CheckOutUDFSetup { get { return ResourceRead.GetResourceValue("CheckOutUDFSetup", resourceFile); } }
        public static string MsgSelectRecordCheckout { get { return ResourceRead.GetResourceValue("MsgSelectRecordCheckout", resourceFile); } }
        
        
        public static string NotEnoughQtyInWIP { get { return ResourceRead.GetResourceValue("NotEnoughQtyInWIP", resourceFile); } }
        public static string EnterToolName { get { return ResourceRead.GetResourceValue("EnterToolName", resourceFile); } }
        public static string ToolNameDoesNotExistInRoom { get { return ResourceRead.GetResourceValue("ToolNameDoesNotExistInRoom", resourceFile); } }
        public static string CountQtyShouldBeGreaterThanEqualsZero { get { return ResourceRead.GetResourceValue("CountQtyShouldBeGreaterThanEqualsZero", resourceFile); } }
        public static string SerialNoCantBeBlank { get { return ResourceRead.GetResourceValue("SerialNoCantBeBlank", resourceFile); } }
        public static string CountQtyShouldBeOneForSelectedSerialNo { get { return ResourceRead.GetResourceValue("CountQtyShouldBeOneForSelectedSerialNo", resourceFile); } }
        public static string QuantityIsAlreadyAvailableForSelectedSerial { get { return ResourceRead.GetResourceValue("QuantityIsAlreadyAvailableForSelectedSerial", resourceFile); } }


        public static string GroupOfItemsHasInvalidQuantity { get { return ResourceRead.GetResourceValue("GroupOfItemsHasInvalidQuantity", resourceFile); } }
        public static string NoOfPastMntsToConsiderRequireValidQuantity { get { return ResourceRead.GetResourceValue("NoOfPastMntsToConsiderRequireValidQuantity", resourceFile); } }
        public static string DuplicateToolNameFound { get { return ResourceRead.GetResourceValue("DuplicateToolNameFound", resourceFile); } }
        public static string UserDoesnotHaveRightToInsertTool { get { return ResourceRead.GetResourceValue("UserDoesnotHaveRightToInsertTool", resourceFile); } }
        public static string ForSrToolUseToolCheckinCheckOutSerialMethod { get { return ResourceRead.GetResourceValue("ForSrToolUseToolCheckinCheckOutSerialMethod", resourceFile); } }
        public static string MsgAssignToolCategory { get { return ResourceRead.GetResourceValue("MsgAssignToolCategory", resourceFile); } }
        public static string MsgDuplicateToolNotAllow { get { return ResourceRead.GetResourceValue("MsgDuplicateToolNotAllow", resourceFile); } }
        public static string NewTool { get { return ResourceRead.GetResourceValue("NewTool", resourceFile); } }
        public static string SelectProperRecordToCheckout { get { return ResourceRead.GetResourceValue("SelectProperRecordToCheckout", resourceFile); } }

    }

    public class ResKitToolMaster
    {


        private static string resourceFile = "ResKitToolMaster";



        /// <summary>
        ///   Looks up a localized string similar to Cost.
        /// </summary>
        public static string Cost
        {
            get
            {
                return ResourceRead.GetResourceValue("Cost", resourceFile);
            }
        }
        public static string Type
        {
            get
            {
                return ResourceRead.GetResourceValue("Type", resourceFile);
            }
        }
        public static string QuantityPerKit
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityPerKit", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ResourceRead.GetResourceValue("Description", resourceFile);
            }
        }

        public static string QuantityTexBox
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityTexBox", resourceFile);
            }
        }
        public static string Quantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", resourceFile);
            }
        }

                
        public static string KitToolQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("Quantity", resourceFile);
            }
        }

        public static string ForMaintanence
        {
            get
            {
                return ResourceRead.GetResourceValue("ForMaintanence", resourceFile);
            }
        }

        public static string IsGroupOfItems
        {
            get
            {
                return ResourceRead.GetResourceValue("IsGroupOfItems", resourceFile);
            }
        }

        public static string IsGrpOfItemReq
        {
            get
            {
                return ResourceRead.GetResourceValue("IsGrpOfItemReq", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked Out.
        /// </summary>
        public static string IsCheckedOut
        {
            get
            {
                return ResourceRead.GetResourceValue("IsCheckedOut", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enter proper value..
        /// </summary>
        public static string IvalidCost
        {
            get
            {
                return ResourceRead.GetResourceValue("IvalidCost", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tools.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        public static string Location
        {
            get
            {
                return ResourceRead.GetResourceValue("Location", resourceFile);
            }
        }
        public static string ToolImage
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolImage", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Technician.
        /// </summary>
        public static string Technician
        {
            get
            {
                return ResourceRead.GetResourceValue("Technician", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to eTurns: Tools.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFile);
            }
        }

        public static string Checkedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Checkedout", resourceFile);
            }
        }
        public static string CheckedouttoMaintenance
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedouttoMaintenance", resourceFile);
            }
        }
        public static string Groupfullycheckedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Groupfullycheckedout", resourceFile);
            }
        }
        public static string Nonecheckedout
        {
            get
            {
                return ResourceRead.GetResourceValue("Nonecheckedout", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Serial.
        /// </summary>
        public static string Serial
        {
            get
            {
                return ResourceRead.GetResourceValue("Serial", resourceFile);
            }
        }

        
        public static string KitToolSerial
        {
            get
            {
                return ResourceRead.GetResourceValue("Serial", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool Category.
        /// </summary>
        public static string ToolCategory
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategory", resourceFile);
            }
        }
        public static string ToolCheckoutAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckoutAllButton", resourceFile);
            }
        }
        public static string ToolCheckInAllButton
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCheckInAllButton", resourceFile);
            }
        }



        /// <summary>
        ///   Looks up a localized string similar to Tool CategoryID.
        /// </summary>
        public static string ToolCategoryID
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolCategoryID", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool.
        /// </summary>
        public static string ToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolName", resourceFile);
            }
        }
        
        public static string KitToolName
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolName", resourceFile);
            }
        }
        public static string AvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableQty", resourceFile);
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to UDF1.
        /// </summary>
        public static string UDF1
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF1", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF10.
        /// </summary>
        public static string UDF10
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF10", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF2.
        /// </summary>
        public static string UDF2
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF2", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF3.
        /// </summary>
        public static string UDF3
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF3", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF4.
        /// </summary>
        public static string UDF4
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF4", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF5.
        /// </summary>
        public static string UDF5
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF5", resourceFile, true);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF6.
        /// </summary>
        public static string UDF6
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF6", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF7.
        /// </summary>
        public static string UDF7
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF7", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF8.
        /// </summary>
        public static string UDF8
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF8", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to UDF9.
        /// </summary>
        public static string UDF9
        {
            get
            {
                return ResourceRead.GetResourceValue("UDF9", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string CheckedInBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedInBy", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string CheckedOutBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutBy", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string UpdatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("Updatedby", resourceFile);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Checked In by.
        /// </summary>
        public static string UpdatedDate
        {
            get
            {
                return ResourceRead.GetResourceValue("UpdatedDate", resourceFile);
            }
        }

        public static string ImagePath
        {
            get
            {
                return ResourceRead.GetResourceValue("ImagePath", resourceFile);
            }
        }

        public static string ToolImageExternalURL
        {
            get
            {
                return ResourceRead.GetResourceValue("ToolImageExternalURL", resourceFile);
            }
        }

        public static string NoOfPastMntsToConsider
        {
            get
            {
                return ResourceRead.GetResourceValue("NoOfPastMntsToConsider", resourceFile);
            }

        }

        public static string MaintenanceDueNoticeDays
        {
            get
            {
                return ResourceRead.GetResourceValue("MaintenanceDueNoticeDays", resourceFile);
            }
        }
        public static string KitDetails
        {
            get
            {
                return ResourceRead.GetResourceValue("KitDetails", resourceFile);
            }
        }
        public static string AvailableWIPKit
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableWIPKit", resourceFile);
            }
        }
        public static string AvailableInGeneralInventory
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableInGeneralInventory", resourceFile);
            }
        }
        public static string TotalQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("TotalQuantity", resourceFile);
            }
        }
        public static string CheckedOutQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutQTY", resourceFile);
            }
        }
        public static string CheckedOutMQTY
        {
            get
            {
                return ResourceRead.GetResourceValue("CheckedOutMQTY", resourceFile);
            }
        }
        public static string QuantityReadyForAssembly
        {
            get
            {
                return ResourceRead.GetResourceValue("QuantityReadyForAssembly", resourceFile);
            }
        }
        public static string AvailableItemsInWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("AvailableItemsInWIP", resourceFile);
            }
        }
        public static string IsBuildBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("IsBuildBreak", resourceFile);
            }
        }

        public static string QtyToMove
        {
            get
            {
                return ResourceRead.GetResourceValue("QtyToMove", resourceFile);
            }
        }

        public static string MoveIn
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveIn", resourceFile);
            }
        }

        public static string MoveOut
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveOut", resourceFile);
            }
        }

        public static string WrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("WrittenOff", resourceFile);
            }
        }

        public static string EnterQtyToWrittenOff
        {
            get
            {
                return ResourceRead.GetResourceValue("EnterQtyToWrittenOff", resourceFile);
            }
        }

        public static string CantWrittenOffQtyMoreThanAvailableInWIP
        {
            get
            {
                return ResourceRead.GetResourceValue("CantWrittenOffQtyMoreThanAvailableInWIP", resourceFile);
            }
        }

        public static string ReqKitisBuildBreak
        {
            get
            {
                return ResourceRead.GetResourceValue("ReqKitisBuildBreak", resourceFile);
            }
        }
    }
    public enum ToolTypeTracking
    {
        General = 1,
        SerialType = 2
    }

}
